using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Con
{
    public class PXTask
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public PXTaskStatus Status { get; set; }

    }

    public enum PXTaskStatus
    {
        Pending,
        Running,
        Finished,
        Error,
    }
}
