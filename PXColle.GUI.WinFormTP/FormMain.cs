using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PXColle.Action;
using PXColle.Action.Snapshot;
using PXColle.Action.YouGet;
using PXColle.Master;
using PXColle.Trigger;
using PXColle.Trigger.RSS;

namespace PXColle.GUI.WinFormTP
{
    public partial class FormMain : Form
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool AllocConsole();

        PXColleMaster master;
        PXStorage storm;

        public FormMain()
        {
            InitializeComponent();

            WriteLog("程序启动");

            //AllocConsole();

            Init();
        }

        private void Init()
        {
            storm = new PXStorage();
            if (!storm.CheckWorkingPath())
            {
                tabControlMain.SelectedIndex = 1;
                //storm.SetWorkingPath(@"C:\Users\SkiTiSu\Desktop\PXColle");
            }
            else
            {
                InitMaster();
            }

        }

        private void InitMaster()
        {
            master = new PXColleMaster();
            master.Start(storm);
            master.OnStatusChange += Action_OnStatusChange;
        }

        private void buttonSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                textBoxFolder.Text = foldPath;
            }
        }

        private void WriteLog(string text)
        {
            if (textBoxLog.InvokeRequired)
            {
                textBoxLog.Invoke(new System.Action(() =>
                {
                    textBoxLog.Text += $"[{DateTime.Now.ToString("HH:mm:ss")}] {text}\r\n";
                    textBoxLog.SelectionStart = textBoxLog.Text.Length;
                    //textBoxLog.Select(textBoxLog.Text.Length, 0);
                    textBoxLog.ScrollToCaret();
                }));
            }
            else
            {
                textBoxLog.Text += $"[{DateTime.Now.ToString("HH:mm:ss")}] {text}\r\n";
                textBoxLog.SelectionStart = textBoxLog.Text.Length;
                //textBoxLog.Select(textBoxLog.Text.Length, 0);
                textBoxLog.ScrollToCaret();
            }
        }

        private void buttonSub_Click(object sender, EventArgs e)
        {
            var context = new PXTriggerContext
            {
                Id = "niubi",
                Key = "rss",
                Config = new Dictionary<string, string>() { { "Url", textBoxRssAddress.Text} }
            };
            master.AddTrigger(context);
        }

        private void Action_OnStatusChange(object sender, OnStatusChangeEventArgs e)
        {
            WriteLog($"<{sender.GetType()}|{((IPXAction)sender).context.Url}>[{e.Status}] {e.Message}");
            UpdateActionListView();
        }

        private void UpdateActionListView()
        {
            listViewActions.Invoke(new System.Action(() =>
            {
                listViewActions.BeginUpdate();
                listViewActions.Items.Clear();
                var contexts = master.ActionM.List().ToArray();
                foreach (var context in contexts)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = context.Key;
                    lvi.SubItems.Add(context.Status.ToString());
                    lvi.SubItems.Add(context.StatusDesc);
                    lvi.SubItems.Add(context.Url);
                    listViewActions.Items.Add(lvi);
                }
                listViewActions.EndUpdate();
            }));
        }

        private void buttonFolderOK_Click(object sender, EventArgs e)
        {
            storm.SetWorkingPath(textBoxFolder.Text);
            InitMaster();
            tabControlMain.SelectedIndex = 0;
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {

        }
    }
}
