using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PXColle.Trigger
{
    public class TriggerManager
    {
        public event EventHandler<OnTriggerEventArgs> OnTrigger;
        public event EventHandler ContextsChanged;
        private List<IPXTrigger> triggers = new List<IPXTrigger>();

        public TriggerManager()
        {
            
        }

        public void Start(IPXTrigger trigger)
        {
            trigger.OnTrigger += Trigger_OnTrigger;
            trigger.Start();
        }

        public void Start(string id)
        {
            var trigger = triggers.Find(x => x.context.Id == id);
            if (trigger != null)
            {
                Start(trigger);
            }
            //TODO: Throw ArgumentNull
        }

        private void Trigger_OnTrigger(object sender, OnTriggerEventArgs e)
        {
            OnTrigger?.Invoke(sender, e);
        }

        public void Stop(string id) 
            => Stop(triggers.Find(x => x.context.Id == id));

        public void Stop(IPXTrigger trigger)
        {
            if (trigger != null && trigger.context.Status == PXTriggerStatus.Running)
            {
                trigger.Stop();
            }
        }

        public IPXTrigger Add(PXTriggerContext context)
        {
            context.PropertyChanged += Context_PropertyChanged;
            IPXTrigger newTrigger = GetTrigger(context);
            triggers.Add(newTrigger);
            return newTrigger;
        }

        public void Remove(string id)
        {
            var trigger = triggers.Find(x => x.context.Id == id);
            Stop(trigger);
            triggers.Remove(trigger);
            ContextsChanged(this, null);
        }


        private void Context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ContextsChanged?.Invoke(sender, e);
        }

        public IEnumerable<PXTriggerContext> List()
        {
            return triggers.Select(x => x.context);
        }

        public void Resume(IEnumerable<PXTriggerContext> contexts)
        {
            triggers.Clear();
            foreach (var context in contexts)
            {
                var newTrigger = Add(context);
                if (context.Status == PXTriggerStatus.Running)
                {
                    Start(newTrigger);
                }
                
            }
        }

        //public IEnumerable<PXTriggerContext> Save()
        //{
        //    List<PXTriggerContext> contexts = List().ToList();
        //    return contexts;
        //}

        public IPXTrigger GetTrigger(PXTriggerContext context)
        {
            if (context.Key == "rss")
            {
                return new RSS.RSS(context);
            }
            else if (context.Key == "test")
            {
                return new TriggerTest(context);
            }
            return null;
        }

        public IEnumerable<PXConfigItem> GetTriggerConfig(string key)
        {
            if (key == "rss")
            {
                return RSS.RSS.ConfigItems;
            }
            //else if (key == "test")
            //{
            //    return new TriggerTest(context);
            //}
            return null;
        }
    }
}
