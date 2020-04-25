using PXColle.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PXColle.Trigger
{
    public class PXTriggerContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public PXTriggerStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public Dictionary<string, string> Config { get; set; }
        [PropertyChanged.DoNotCheckEquality]
        public Dictionary<string, object> LocalStorageData { get; set; } = new Dictionary<string, object>();

        public ILocalStorage LocalStorage { get; set; }

        public PXTriggerContext()
        {
            LocalStorage = new LocalStorageProperty
                ((
                    () => LocalStorageData,
                    value => LocalStorageData = value
                ));
        }

    }

    public enum PXTriggerStatus
    {
        Pending,
        Running,
        InitError,
        Error,
    }
}
