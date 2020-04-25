using PXColle.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PXColle.Plugin
{
    public class PXPluginContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public PXPluginStatus Status { get; set; }
        public PXPluginContext PrevContext { get; set; }

        [PropertyChanged.DoNotCheckEquality]
        public Dictionary<string, object> LocalStorageData { get; set; } = new Dictionary<string, object>();

        public ILocalStorage LocalStorage { get; set; }

        public PXPluginContext()
        {
            LocalStorage = new LocalStorageProperty
                ((
                    () => LocalStorageData,
                    value => LocalStorageData = value
                ));
        }
    }

    public class PXPluginStatus
    {
        public bool IsRunning { get => Step == PXPluginStatusStep.Running; }
        
        public PXPluginStatusStep Step;

        public string Description;

    }

    public enum PXPluginStatusStep
    {
        Pending = 0,
        Init = 1,
        Running = 2,
        Finished = 3,
        Timeout = -3,
    }
}
