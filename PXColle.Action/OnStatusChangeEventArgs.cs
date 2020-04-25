using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Action
{
    public class OnStatusChangeEventArgs : EventArgs
    {
        public OnStatusChangeEventArgs(PXActionStatus status, string s)
        {
            Status = status;
            Message = s;
        }

        public PXActionStatus Status { get; set; }
        public string Message { get; set; }
    }
}
