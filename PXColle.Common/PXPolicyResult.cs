using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Common
{
    public class PXPolicyResult
    {
        public string Name { get; set; }
        public string MatchedUrlPattern { get; set; }
        public string Path { get; set; }
        public IEnumerable<string> Actions { get; set; }
    }
}
