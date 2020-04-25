using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PXColle.Action
{
    public abstract class PXActionBase : IPXAction
    {
        public string Id { get; protected set;}
        public float ProgressPercent { get; protected set; }
        public PXActionStatus Status { get; protected set; }
        public string StatusDesc { get; protected set; }

        public event EventHandler<OnStatusChangeEventArgs> OnStatusChange;
        public PXActionContext context { get; protected set; }
        
        public PXActionBase(PXActionContext context)
        {
            this.context = context;
            this.Id = context.Id;

            ChangeStatus(PXActionStatus.Pending);
        }

        public abstract void Run();
        public abstract void Stop();

        protected void ChangeStatus(PXActionStatus status, string desc = "")
        {
            Status = status;
            StatusDesc = desc;
            context.Status = status;
            context.StatusDesc = desc;
            context.UpdatedAt = DateTimeOffset.Now;
            OnStatusChange?.Invoke(this, new OnStatusChangeEventArgs(status, desc));
        }

        protected void ChangeStatus(string desc) => 
            ChangeStatus(Status, desc);
    }
}
