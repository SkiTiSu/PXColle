using LiteDB;
using PXColle.Action;
using PXColle.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace PXColle.Master
{
    public class PXStorage
    {
        public DirectoryInfo WorkingDirectory { get; private set; }
        private string WorkingPath { get => WorkingDirectory.FullName; }
        public string ConfigPath { get; private set; }
        private const string defaultConfigFile = "config.json";
        private const string configFolderName = "Config";
        private const string dataFolderName = "Data";
        private const string tempFolderName = "Temp";
        ILocalStorage localStorage;
        const string DatabaseFileName = "Database";
        ILocalStorage Database;

        LiteDatabase db;
        ILiteCollection<PXActionContext> colAction;
        ILiteCollection<PXManagedFile> colPXFile;

        public PXStorage(string configFile = null)
        {
            if (string.IsNullOrEmpty(configFile))
            {
                ConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), defaultConfigFile);
            }
            else
            {
                ConfigPath = configFile;
            }

            localStorage = new LocalStorageRealFile(ConfigPath);
        }

        public void SetAction(PXActionContext context)
        {
            colAction.Upsert(context);
            colAction.EnsureIndex(x => x.Id, true);
            colAction.EnsureIndex(x => x.Status);
            //colAction.EnsureIndex(x => x.CreatedAt);
            colAction.EnsureIndex(x => x.UpdatedAt);
        }

        public IEnumerable<PXActionContext> GetActions()
        {
            IEnumerable<PXActionContext> resrunning = colAction
                .Find(x => x.Status == PXActionStatus.Running || x.Status == PXActionStatus.Pending);
            IEnumerable<PXActionContext> resrecent = colAction.Query()
                .Where(x => x.Status != PXActionStatus.Running && x.Status != PXActionStatus.Pending)
                .OrderByDescending(x => x.UpdatedAt)
                .Limit(30)
                .ToEnumerable();
            return resrunning.Concat(resrecent);
        }

        public void SetPXFile(PXManagedFile file)
        {
            colPXFile.Upsert(file);
            colPXFile.EnsureIndex(x => x.MD5, true);
        }

        public void SetWorkingPath(string path)
        {
            try
            {
                WorkingDirectory = new DirectoryInfo(path);
                localStorage.Set("WorkingPath", path);
                InitWorkingPath();
            }
            catch
            {
                throw;
            }
        }

        public bool CheckWorkingPath()
        {
            if (localStorage.Exists("WorkingPath"))
            {
                SetWorkingPath(localStorage.Get("WorkingPath"));
                return true;
            }
            else
            {
                return false;
            }
        }

        public void InitWorkingPath()
        {
            try
            {
                if (!WorkingDirectory.Exists)
                {
                    WorkingDirectory.Create();
                }
                CheckAndCreate(Path.Combine(WorkingDirectory.FullName, configFolderName));
                CheckAndCreate(Path.Combine(WorkingDirectory.FullName, dataFolderName));
                Database = GetConfigLocalStorage(DatabaseFileName);
                InitDb();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        private void InitDb()
        {
            db = new LiteDatabase(Path.Combine(WorkingPath, configFolderName, DatabaseFileName + ".db"));
            if (db.UserVersion == 0)
            {
                //foreach (var doc in db.Engine.Find("MyCol"))
                //{
                //    doc["NewCol"] = Convert.ToInt32(doc["OldCol"].AsString);
                //    db.Engine.Update("MyCol", doc);
                //}
                //db.Engine.UserVersion = 1;
            }
            colAction = db.GetCollection<PXActionContext>("action");
            colPXFile = db.GetCollection<PXManagedFile>("pxfile");
        }

        public void CheckAndCreate(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public ILocalStorage GetConfigLocalStorage(string name)
        {
            return new LocalStorageRealFile(Path.Combine(WorkingPath, configFolderName, name + ".json"));
        }

        public string New(string name, string url)
        {
            string urlmd5 = Md5Hash(url);
            string timestamp = Base36(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            DirectoryInfo DataD = new DirectoryInfo(Path.Combine(WorkingPath, dataFolderName));
            DirectoryInfo urlfolder = DataD.CreateSubdirectory(urlmd5).CreateSubdirectory(timestamp);

            PXDataDetail detail = new PXDataDetail
            {
                Name = name,
                Timestamp = timestamp,
                CreatedAt = DateTimeOffset.Now
            };
            PXData data;
            if (Database.Exists(urlmd5))
            {
                data = Database.Get<PXData>(urlmd5);
            }
            else
            {
                data = new PXData
                {
                    Name = name,
                    MD5 = urlmd5,
                    Url = url,
                    Vers = new List<PXDataDetail>()
                };
            }
            data.Vers.Add(detail);
            Database.Set(urlmd5, data);

            return urlfolder.FullName;
        }

        public string NewTemp(PXActionContext context)
        {
            // Maybe need a standlone temp id
            string path = TempPath(context.Id);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        
        public void CollectTemp(PXActionContext context)
        {
            Trace.TraceInformation("Collecting " + context.Id);
            DirectoryInfo directory = new DirectoryInfo(context.WorkingDictory);
            context.PolicyInfo.Path = context.PolicyInfo.Path.Replace("{md5url}", Md5Hash(context.Url));
            context.PolicyInfo.Path = context.PolicyInfo.Path.Replace("{timestamp}", Base36(context.CreatedAt.ToUnixTimeSeconds()));
            string dest = Path.Combine(WorkingPath, dataFolderName, context.PolicyInfo.Path);
            Directory.CreateDirectory(dest);

            FileInfo[] files = directory.GetFiles();
            foreach (var file in files)
            {
                PXManagedFile mfile = new PXManagedFile
                {
                    Name = context.Name,
                    Length = file.Length,
                    MD5 = Md5HashFromFile(file.FullName),
                    CreatedAt = DateTimeOffset.Now,
                    From = context.Key,
                    ActionContext = context,
                    Path = context.PolicyInfo.Path + "/" + file.Name
                };
                colPXFile.Insert(mfile);
                file.MoveTo(Path.Combine(dest, file.Name));
            }
            directory.Delete();
        }

        //public IEnumerable<PXManagedFile> Search(string text)
        //{
        //    colPXFile.Query().
        //}

        public string TempPath(string Id)
        { 
            return Path.Combine(WorkingPath, tempFolderName, Id);
        }

        public static string Md5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("X2"));
                }

                return sBuilder.ToString();
            }
        }

        const string base36rainbow = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string Base36(long num)
        {
            string encoded = "";
            do
            {
                encoded = base36rainbow[(int)(num % 36)] + encoded;
                num /= 36;
            } while (num != 0);
            return encoded;
        }

        public static long FromBase36(string encoded)
        {
            encoded = encoded.ToUpper();
            long num = 0;
            for (int i = 0; i < encoded.Length; i++)
            {
                num += base36rainbow.IndexOf(encoded[i]) * (long)Math.Pow(36, encoded.Length - i - 1);
            }
            return num;
        }

        public static string Md5HashFromFile(string filePath)
        {
            // On my PC, it won't be faster if the bufferSize is 16 * 1024 * 1024.
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 32 * 1024))
            {
                using (var sha = new MD5CryptoServiceProvider())
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    byte[] checksum = sha.ComputeHash(stream);

                    sw.Stop();
                    Trace.TraceInformation($"MD5 calc for {filePath} finished, cost {sw.ElapsedMilliseconds} ms");

                    return BitConverter.ToString(checksum).Replace("-", String.Empty);
                }
            }
        }
    }
}
