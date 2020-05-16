using PXColle.Master;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Text;

namespace PXColle.GUI.BaseTP
{
    public class PXNodeViewModel : ReactiveObject
    {
        private readonly PXNode _node;
        public PXNodeViewModel(PXNode node)
        {
            _node = node;

            this.WhenAnyValue(x => x.Name)
                .ToProperty(this, x => x._node.Name);
        }

        [Reactive]
        public string Name { get; set; }
    }
}
