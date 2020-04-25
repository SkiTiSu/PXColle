using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PXColle.GUI.BlazorTP
{
    public class TestContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Lists { get; set; }

        public TestContext()
        {
            //Lists.CollectionChanged
        }
    }
}
