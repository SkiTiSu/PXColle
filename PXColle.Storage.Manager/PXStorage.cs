using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PXColle.Storage.Manager
{
    public class PXStorage
    {
        public DirectoryInfo WorkingPath { get; private set; }
        public PXStorage()
        {
        }

        public void Init(string path)
        {
            WorkingPath = new DirectoryInfo(path);
            try
            {
                if (WorkingPath.Exists)
                {
                    Console.WriteLine("That path exists already.");
                }
                else
                {
                    WorkingPath.Create();
                    Console.WriteLine("The directory was created successfully.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public string New(string url)
        {
            string urlmd5 = GetMd5Hash(url);
            if (Directory.Exists(Path.Combine(WorkingPath.FullName, urlmd5)))
            {
                return null;
            }
            else
            {
                DirectoryInfo urlfolder = WorkingPath.CreateSubdirectory(urlmd5);
                return urlfolder.FullName;
            }
        }

        public static string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }
    }
}
