using System;
using PXColle.Action;
using System.Collections.Generic;
using System.IO;
using PuppeteerSharp;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace PXColle.Action.Snapshot
{
    public class Snapshot : PXActionBase
    {
        Browser browser;

        public Snapshot(PXActionContext context)
            : base(context)
        {
        }

        public override void Run()
        {
            // Well, we have to use this ugly code because SynchronizationContext is not UI thread.
            // Use Do().Wait() directly will cause a deadlock.
            // Use async/await from the begin to the end is the best solution.
            Task.Run(() => RunAsync()).Wait();
        }

        private async Task RunAsync()
        {
            //await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);    
            try
            {
                browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = FindChromePath()
                });
            }
            catch
            {
                ChangeStatus(PXActionStatus.InitError, "Cannot find Chrome, you can set ExecutablePath or WS manully. If you cannot solve this problem, have a try on Easy Mode.");
                return;
            }
            using (var page = await browser.NewPageAsync())
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1920,
                    Height = 1080
                });
                ChangeStatus(PXActionStatus.Running, "Init finished. Loading page...");
                //TODO: Change timeout to the 80% of context.Timeout
                await page.GoToAsync(context.Url, 1200000, new[] { WaitUntilNavigation.Networkidle0 });
                ChangeStatus("Auto scorlling...");
                // Auto scorll to the bottom
                await page.EvaluateFunctionAsync(@"() => {
                    return new Promise((resolve, reject) => {
                        let totalHeight = 0;
                        let distance = 100;
                        let timer = setInterval(() => {
                            var scrollHeight = document.body.scrollHeight;
                            window.scrollBy(0, distance);
                            totalHeight += distance;
                            if(totalHeight >= scrollHeight) {
                                clearInterval(timer);
                                resolve();
                            }
                        }, 100);
                    })
                }");
                ChangeStatus("Saving to image...");
                //string screen = await page.ScreenshotBase64Async(new ScreenshotOptions { FullPage = true });
                //File.WriteAllBytes(Path.Combine(context.WorkingDictory, context.FilenamePrefix + ".png"), Convert.FromBase64String(screen));
                await page.ScreenshotAsync(Path.Combine(context.WorkingDictory, context.FilenamePrefix + ".png"), new ScreenshotOptions
                {
                    FullPage = true
                });
                ChangeStatus("Saving to mhtml...");
                var cdp = await page.Target.CreateCDPSessionAsync();
                JObject re = await cdp.SendAsync("Page.captureSnapshot", new { format = "mhtml" });
                File.WriteAllText(Path.Combine(context.WorkingDictory, context.FilenamePrefix + ".mhtml"), re["data"].ToString());
                await browser.CloseAsync();
                ChangeStatus(PXActionStatus.Finished, "Done.");
            }
        }

        public override void Stop()
        {
            
        }

        private string FindChromePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                const string ChromeAppKey = @"\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
                return (string)(Registry.GetValue("HKEY_LOCAL_MACHINE" + ChromeAppKey, "", null) ??
                                Registry.GetValue("HKEY_CURRENT_USER" + ChromeAppKey, "", null)) ??
                                @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "/usr/bin/google-chrome";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "/Applications/Google Chrome.app/";
            }
            return @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
        }

        //~Snapshot()
        //{
        //    browser?.CloseAsync();
        //}
    }
}
