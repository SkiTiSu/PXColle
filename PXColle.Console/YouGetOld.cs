using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace PXColle.Con
{
    public class YouGetOld
    {
        PXProcesserContext context;
        public float ProgressPercent { get; private set; }
        public PXTaskStatus Status { get; private set; }

        public YouGetOld(PXProcesserContext contextin)
        {
            context = contextin;
        }

        public void Run()
        {
            Status = PXTaskStatus.Running;
            PrintDoc();
        }

        void PrintDoc()
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            //process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            //process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            //process.StartInfo.StandardInputEncoding = Encoding.UTF8;

            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            //process.StartInfo.FileName = GetPlatformCmd("cmd");
            process.StartInfo.FileName = "you-get";
            process.StartInfo.Arguments = 
                @$"--output-dir {AddQuotesIfRequired(context.WorkingDictory.FullName)} --output-filename test {context.Url}";
            //process.StartInfo.WorkingDirectory = "";

            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;

            try
            {
                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                string temp = @"--output-dir C:\ --output-filename test ";
                string name = @" https://www.bilibili.com/video/av28038890";
                process.StandardInput.AutoFlush = true;
                //process.StandardInput.WriteLine(@"sssss");
                //process.StandardInput.WriteLine(GetPlatformCmd("exit"));

                int milliseconds = context.Timeout.HasValue
                    ? (int)context.Timeout.Value.TotalMilliseconds
                    : 5000000;

                process.WaitForExit(milliseconds);
                process.Close();

                if (Status != PXTaskStatus.Error)
                {
                    Status = PXTaskStatus.Finished;
                }
            }
            catch
            {
                Status = PXTaskStatus.Error;
            }
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
                }
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Status = PXTaskStatus.Error;
            }
        }

        string GetPlatformCmd(string cmd = "cmd")
        {
            if (cmd == "cmd")
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Environment.GetEnvironmentVariable("COMSPEC") ?? "cmd";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Environment.GetEnvironmentVariable("SHELL") ?? "/bin/bash";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return Environment.GetEnvironmentVariable("SHELL") ?? "/bin/bash";
                }
            }
            else if (cmd == "exit")
            {
                return "exit";
            }

            return "cmd";
        }

        public string AddQuotesIfRequired(string path)
        {
            return !string.IsNullOrWhiteSpace(path) ?
                path.Contains(" ") && (!path.StartsWith("\"") && !path.EndsWith("\"")) ?
                    "\"" + path + "\"" : path :
                    string.Empty;
        }

    }

    public class PXProcesserContext
    {
        public string Url { get; set; }
        public DirectoryInfo WorkingDictory { get; set; }
        public TimeSpan? Timeout { get; set; }
        public KeyValuePair<string, string> Config { get; set; }
    }
}