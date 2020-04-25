using System;
using System.Collections.Generic;
using System.Text;
using PXColle.Common;

namespace PXColle.Trigger
{
    public abstract class PXTriggerBase : IPXTrigger
    {
        public PXTriggerContext context { get; protected set; }
        public event EventHandler<OnTriggerEventArgs> OnTrigger;
        protected ILocalStorage localStorage;

        public static IEnumerable<PXConfigItem> ConfigItems { get; protected set; }

        public PXTriggerBase(PXTriggerContext context)
        {
            this.context = context;
            this.localStorage = context.LocalStorage;
        }

        public abstract void Start();
        
        public abstract void Stop();

        protected void Trigger(string name, string url)
        {
            OnTrigger?.Invoke(this, new OnTriggerEventArgs(name, url));
        }
    }
}
