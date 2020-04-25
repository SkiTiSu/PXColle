using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PXColle.Action
{
    public interface IPXAction
    {
        float ProgressPercent { get; }
        PXActionStatus Status { get; }
        string StatusDesc { get; }
        PXActionContext context { get; }
        event EventHandler<OnStatusChangeEventArgs> OnStatusChange;
        void Run();
        void Stop();
    }

    public enum PXActionStatus
    {
        Pending,
        Running,
        Finished,
        InitError,
        Error,
        Timeout
    }
}
