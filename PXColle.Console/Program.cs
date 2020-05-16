using System;
using System.Threading.Tasks;
using PXColle.Action;
using PXColle.Action.Snapshot;
using PXColle.Action.YouGet;
using PXColle.Trigger.RSS;
using PXColle.Common;
using PXColle.Trigger;
using PXColle.Master;
using System.Collections.Generic;

namespace PXColle.Con
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //ClassRun cr = new ClassRun();
            //cr.run().Wait();

            //YouGet youGet = new YouGet(null);
            //youGet.Run();
            //Console.ReadKey();

            //var con = new PXActionContext
            //{
            //    Url = "https://www.bilibili.com/video/av15530330",
            //    WorkingDictory = "bilibili",
            //    FilenamePrefix = "av15530330"
            //};

            //Console.WriteLine(">>>WebpageSnapshot");
            //IPXAction webpageSnapshot = new Snapshot(con);
            //webpageSnapshot.OnStatusChange += WebpageSnapshot_OnStatusChange;
            //webpageSnapshot.Run().Wait();
            //Console.WriteLine(">>>YouGet");
            //IPXAction youGet = new YouGet(con);
            //youGet.OnStatusChange += WebpageSnapshot_OnStatusChange;
            //youGet.Run().Wait();

            //RSS rSS = new RSS(new Trigger.PXTriggerContext
            //{
            //    Id = "niubi"
            //});
            //rSS.Start();
            //Console.Read();

            //ILocalStorage configs = new LocalStorageRealFile("test.json");
            //ILocalStorage localStorage1 = new LocalStorage("snapshot-1", configs);
            //ILocalStorage localStorage2 = new LocalStorage("snapshot-2", configs);
            //ILocalStorage local22 = new LocalStorage("2-2", localStorage2);
            ////localStorage1.Set("1-key", "1-value");
            ////localStorage1.Set("1-key-2", "1-value-2");
            ////localStorage1.Set("1-key-3", "");
            ////localStorage1.Set("1-key-4", null);
            ////localStorage1.Set("1-object", new testclass());
            ////localStorage2.Set("2-key", "2-value");
            ////local22.Set("22-key", "22-value");
            //Console.WriteLine(localStorage1.Get("1-key"));
            //Console.WriteLine(localStorage2.Get("2-key"));
            //Console.WriteLine(localStorage1.Get<testclass>("1-object"));
            //Console.WriteLine(local22.Get("22-key"));

            //ILocalStorage localStorage1 = new LocalStorage("snapshot-1.json", realFile);
            //localStorage1.Set("1-key", "1-value");
            //localStorage1.Set("1-key-2", "1-value-2");
            //localStorage1.Set("1-object", new { a = 1, b = 2 });

            //PXColleMaster pm = new PXColleMaster();
            //pm.Start();
            //PXTriggerContext context = new PXTriggerContext
            //{
            //    Id = "nb",
            //    Key = "test"
            //};
            //pm.AddTrigger(context);
            //pm.TriggerM.Start("nb");
            //Console.Read();

            var a = new PXNodeType
            {
                Key = "123",
                Templates = new Dictionary<string, string>
                {
                    { "homepages", "url[]" },
                    { "realname", "string" }
                },
                CreatedAt = new DateTimeOffset(2020, 5, 14,0,0,0, TimeSpan.Zero)
            };
            
            var b = new PXNodeType
            {
                Key = "456",
                Templates = new Dictionary<string, string>
                {
                    { "homepages", "url[]" },
                    { "realname", "string" }
                }
            };

            var aa = new List<PXNodeType>();
            aa.Add(a);
            aa.Add(b);

            var url3 = new PXUrl
            {
                Id = "1-3",
                Name = "https://www.instagram.com/mafumafu_ig",
                Connects = new Dictionary<string, string>
                {
                    { "1", "prop.homepages" }
                }
            };

            //var serializer = new YamlDotNet.Serialization.Serializer();
            //serializer.Serialize(Console.Out, aa);

            PXNode c = a;
            PXNode d = new PXNode();

            Console.WriteLine(c.Get());
            Console.WriteLine(d.Get());

            Console.ReadKey();
        }



        public class testclass
        {
            public string a = "test";
            public bool b = false;

            public override string ToString()
            {
                return $"a={a} b={b}";
            }
        }

        //private static void WebpageSnapshot_OnStatusChange(object sender, OnStatusChangeEventArgs e)
        //{
        //    Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] {e.Status.ToString()}: {e.Message}");
        //}
    }
}
