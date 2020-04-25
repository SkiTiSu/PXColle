using System;
using System.Threading.Tasks;
using System.Threading;
using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using System.Collections.Generic;
using System.Diagnostics;

namespace PXColle.Trigger.RSS
{
    public class RSS : PXTriggerBase
    {
        private Timer timer;

        public RSS() : base(null)
        {

        }

        public RSS(PXTriggerContext context)
            : base(context)
        {
        }

        public new static IEnumerable<PXConfigItem> ConfigItems { get; protected set; } = new PXConfigItem[]
        {
            new PXConfigItem
            {
                Key = "Url",
                Name = "Feed Url",
                Type = "string",
                Description = "Supports RSS 0.91, 0.92, 1.0, 2.0 and ATOM",
                Pattern = "(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]",
                Required = true,
                ErrorMessage = "Must be a valid url."
            },
            new PXConfigItem
            {
                Key = "AutoTTL",
                Name = "Auto TTL",
                Type = "bool",
                Description = "Auto detect TTL time. (Only support RSS 2.0)",
                Required = true,
                ErrorMessage = "ERROR"
            }
        };

        public int Init()
        {
            return 0;
        }

        public override void Start()
        {
            context.Status = PXTriggerStatus.Running;
            timer = new Timer(Run, null, 0, 20 * 60 * 1000);
        }

        public override void Stop()
        {
            context.Status = PXTriggerStatus.Pending;
            timer.Dispose();
        }

        private void Run(object o)
        {
            Debug.WriteLine(DateTime.Now.ToString() + ": RSS RUN");
            List<string> triggeredUrls = localStorage.Get<List<string>>("triggeredUrls");
            if (triggeredUrls == null)
            {
                triggeredUrls = new List<string>();
            }
            List<string> newUrls = new List<string>();

            string rsshub = context.Config["Url"];
            Feed feed;
            try
            {
                feed = FeedReader.ReadAsync(rsshub).Result;
            }
            catch
            {
                //ERROR
                return;
            }

            Debug.WriteLine("Feed Title: " + feed.Title);
            Debug.WriteLine("Feed Description: " + feed.Description);
            Debug.WriteLine("Feed Image: " + feed.ImageUrl);
            Debug.WriteLine("Feed Type: " + feed.Type);

            if (feed.Type == FeedType.Rss_2_0)
            {
                var rss20feed = (Rss20Feed)feed.SpecificFeed;
                Debug.WriteLine("Generator: " + rss20feed.Generator);
                if (!string.IsNullOrEmpty(rss20feed.TTL))
                {
                    int.Parse(rss20feed.TTL);
                    Debug.WriteLine("TTL: " + rss20feed.TTL);
                }
            }

            foreach (var item in feed.Items)
            {
                if (!triggeredUrls.Contains(item.Link))
                {
                    Trigger(item.Title, item.Link);
                }
                newUrls.Add(item.Link);
            }

            localStorage.Set("triggeredUrls", newUrls);
            Debug.WriteLine(DateTime.Now.ToString() + ": RSS END");
        }
    }
}
