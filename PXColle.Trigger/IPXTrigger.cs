using System;
using System.Collections.Generic;
using System.Text;

namespace PXColle.Trigger
{
    public interface IPXTrigger
    {
        PXTriggerContext context { get; }
        event EventHandler<OnTriggerEventArgs> OnTrigger;

        //static IEnumerable<PXConfigItem> ConfigItems { get; } //Need .NET Core 3.0+

        void Start();
        void Stop();
    }
}
