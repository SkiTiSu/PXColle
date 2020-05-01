using System;
using System.Collections.Generic;
using System.ComponentModel;
using PXColle.Common;
using PXColle.Trigger;
using PXColle.Action.Manager;
using PXColle.Action;
using System.Diagnostics;
using System.IO;

namespace PXColle.Master
{
    public class PXColleMaster
    {
        public event EventHandler<OnStatusChangeEventArgs> OnStatusChange;
        public bool AutoSave { get; set; } = true;
        public TriggerManager TriggerM { get; set; } = new TriggerManager();
        public ActionManager ActionM { get; set; } = new ActionManager();
        PXPolicyManager PolicyM { get; set; }
        
        PXStorage Storage { get; set; }
        const string triggerFileName = "Triggers";
        ILocalStorage triggerFile;

        public void Start(PXStorage storage)
        {
            Trace.TraceInformation("MASTER START");
            Storage = storage;
            triggerFile = Storage.GetConfigLocalStorage(triggerFileName);
            IEnumerable<PXTriggerContext> contexts = triggerFile.Get<IEnumerable<PXTriggerContext>>("contexts");
            TriggerM.OnTrigger += TriggerM_OnTrigger;
            TriggerM.ContextsChanged += TriggerM_ContextsChanged;
            if (contexts != null)
            {
                Console.WriteLine("RESUME!!!");
                TriggerM.Resume(contexts);
                //TODO: Actions with running status should be set to Error
            }

            List<PXPolicy> policies = new List<PXPolicy>
            {
                new PXPolicy
                {
                    Name = "bilibili",
                    UrlPatterns = new string[] { @"bilibili.com/video/(?<avnum>av\d+)" },
                    PathPattern = "bilibili/{avnum}/{timestamp}",
                    Actions = new string[] { "you-get", "snapshot" },
                },
                //new PXPolicy
                //{
                //    Name = "default",
                //    UrlPatterns = new string[] { @"\s*\S+?" },
                //    PathPattern = "webpage/{md5url}/{timestamp}",
                //    Actions = new string[] { "snapshot" }
                //},
            };
            PolicyM = new PXPolicyManager(policies);

            ActionM.OnStatusChange += ActionM_OnStatusChange;
        }

        private void ActionM_OnStatusChange(object sender, OnStatusChangeEventArgs e)
        {
            var action = (IPXAction)sender;
            Storage.SetAction(action.context);
            if (e.Status == PXActionStatus.Finished)
            {
                Storage.CollectTemp(action.context);
            }

            OnStatusChange?.Invoke(sender, e);
        }

        public IEnumerable<PXActionContext> GetActions()
        {
            return Storage.GetActions();
        }

        private void TriggerM_ContextsChanged(object sender, EventArgs e)
        {
            Save();
        }

        private void TriggerM_OnTrigger(object sender, OnTriggerEventArgs e)
        {
            var policies = PolicyM.GetMatchedPolicy(e.Url);
            foreach (var policy in policies)
            {
                //string workd = Storage.New(e.Name, e.Url);
                foreach (var actionName in policy.Actions)
                {
                    AddAction(actionName, e.Name, e.Url, policy);
                }
            }
        }

        public void Stop()
        {
            Save();
        }

        public void AddTrigger(PXTriggerContext context)
        {
            var trigger = TriggerM.Add(context);
            TriggerM.Start(trigger);
        }

        public void AddAction(string actionName, string name, string url, PXPolicyResult policy)
        {
            Trace.TraceInformation("ADD ACTION " + actionName);
            PXActionContext context = new PXActionContext
            {
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeOffset.Now,
                Name = name,
                Key = actionName,
                Url = url,
                FilenamePrefix = ReplaceInvalidChars(name),
                PolicyInfo = policy
            };
            context.WorkingDictory = Storage.NewTemp(context);
            ActionM.AddAndStart(context);
        }

        public void RetryAction(string id)
        {
            var context = Storage.GetAction(id);
            ActionM.AddAndStart(context);
        }

        public string RemoveInvalidChars(string filename)
        {
            return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }

        public string ReplaceInvalidChars(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        public void Save()
        {
            if (AutoSave)
            {
                triggerFile.Set("contexts", TriggerM.List());
                Trace.TraceInformation("Saved");
            }
        }
    }
}
