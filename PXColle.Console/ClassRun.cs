using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CodeHollow.FeedReader;

namespace PXColle.Con
{
    class ClassRun
    {
        public async Task run()
        {
            var feed = await FeedReader.ReadAsync("https://rsshub.app/bilibili/user/video/2267573");

            Console.WriteLine("Feed Title: " + feed.Title);
            Console.WriteLine("Feed Description: " + feed.Description);
            Console.WriteLine("Feed Image: " + feed.ImageUrl);
            Console.WriteLine("Feed Type: " + feed.Type);

            foreach (var item in feed.Items)
            {
                Console.WriteLine(item.PublishingDate.ToString() + ":" + item.Title + " - " + item.Link);
            }
        }
    }
}
