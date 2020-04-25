using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PXColle.Action;
using PXColle.Master;

namespace PXColle.GUI.BlazorTP
{
    public class MasterHelper
    {
        public bool NeedSetup { get; private set; }
        public PXColleMaster Master { get; private set; }
        private PXStorage storm;

        public MasterHelper()
        {
            //Test();
            Init();
        }

        private void Init()
        {
            storm = new PXStorage();
            if (!storm.CheckWorkingPath())
            {
                NeedSetup = true;
            }
            else
            {
                InitMaster();
            }
        }

        private void InitMaster()
        {
            Master = new PXColleMaster();
            Master.Start(storm);
        }
    }
}
