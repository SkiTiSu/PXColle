using PXColle.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PXColle.Action
{
    public class PXActionContext
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public string WorkingDictory { get; set; }
        public string FilenamePrefix { get; set; }
        public TimeSpan? Timeout { get; set; }
        public Dictionary<string, string> Config { get; set; }
        public PXActionStatus Status { get; set; }
        public string StatusDesc { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public PXPolicyResult PolicyInfo { get; set; }
    }
}
