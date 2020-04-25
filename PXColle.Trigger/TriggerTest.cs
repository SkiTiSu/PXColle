using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Trigger
{
    public class TriggerTest : PXTriggerBase
    {
        public TriggerTest(PXTriggerContext context)
            : base(context)
        {
        }
        public override void Start()
        {
            localStorage.Set("test1", "yes1");
            Console.WriteLine(localStorage.Get("test1"));
        }

        public override void Stop()
        {
            
        }
    }
}
