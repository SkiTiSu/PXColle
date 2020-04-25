using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Trigger
{
    public class OnTriggerEventArgs
    {
        public OnTriggerEventArgs(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public string Name { get; set; }
        public string Url { get; set; }
    }
}
