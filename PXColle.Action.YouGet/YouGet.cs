using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PXColle.Action.YouGet
{
    public class YouGet : PXActionBase
    {
        public YouGet(PXActionContext context)
            : base(context)
        {
        }

        Process process;

        public override void Run()
        {
            process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            //process.StartInfo.StandardInputEncoding = Encoding.UTF8;

            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.StartInfo.FileName = "you-get";
            process.StartInfo.Arguments =
                $@"--output-dir {AddQuotesIfRequired(context.WorkingDictory)} --output-filename {AddQuotesIfRequired(context.FilenamePrefix)} {context.Url}";
            //process.StartInfo.WorkingDirectory = "";
            ChangeStatus(PXActionStatus.Running, $">{process.StartInfo.FileName} {process.StartInfo.Arguments}");
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;

            try
            {
                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.StandardInput.AutoFlush = true;

                int milliseconds = context.Timeout.HasValue
                    ? (int)context.Timeout.Value.TotalMilliseconds
                    : 5000000;

                ChangeStatus(PXActionStatus.Running, "Start Downloading...");

                process.WaitForExit(milliseconds); //TODO: Task.Run
                process.Close();



                //TODO: 有一定几率Error信息在正常信息之前打印，导致误判是正常退出，考虑使用process.ExitCode
                if (Status != PXActionStatus.Error)
                {
                    ChangeStatus(PXActionStatus.Finished, "Done.");
                }
            }
            catch (Exception e)
            {
                ChangeStatus(PXActionStatus.InitError, "Cannot find you-get. Have you installed it?");
            }
        }

        public override void Stop()
        {
            if (process != null && process.HasExited == false)
            {
                process.Kill();
            }
        }

        public void Restart()
        {
            Stop();
            Run();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                string curLine = e.Data;
                string pattern = @"\d+(\.\d+)?(?=%)";
                Match res = Regex.Match(curLine, pattern);
                if (res.Success)
                {
                    ProgressPercent = float.Parse(res.Value);
                    ChangeStatus(PXActionStatus.Running, $"Downloading... {ProgressPercent}%");
                }
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Contains("file already exists"))
                {
                    ChangeStatus(PXActionStatus.Finished, e.Data);
                }
                else
                {
                    ChangeStatus(PXActionStatus.Error, $"{e.Data}"); //Unknown Error. Maybe wrong args or bad internet connection?
                }
            }
        }

        private string AddQuotesIfRequired(string path)
        {
            return !string.IsNullOrWhiteSpace(path) ?
                path.Contains(" ") && (!path.StartsWith("\"") && !path.EndsWith("\"")) ?
                    "\"" + path + "\"" : path :
                    string.Empty;
        }
    }
}
