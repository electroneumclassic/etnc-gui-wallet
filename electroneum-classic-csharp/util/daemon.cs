using System;
using System.Collections.Generic;
using System.Diagnostics;
/*using System.IO;
using System.Linq;
using System.Text;

namespace electroneum_classic_csharp.util
{
    public class daemon
    {
        public static void clear() {

            try
            {
                //clear login
                var files = Directory.GetFiles(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
                foreach (var file in files)
                {
                    if (file.EndsWith(".login"))
                    {
                        File.Delete(file);
                    }
                }


                //clear process
                Process[] temp = Process.GetProcesses();
                foreach (Process item in temp)
                {
                    var title = item.MainWindowTitle;
                    var processName = item.ProcessName;
                    if (title.Contains("electroneumd") || processName.Contains("electroneumd")) {
                        item.Kill();
                    } else if (title.Contains("electroneum-wallet-rpc") || processName.Contains("electroneum-wallet-rpc"))  {
                        item.Kill();
                    }
                    else if (title.Contains("electroneum-wallet-cli") || processName.Contains("electroneum-wallet-cli"))
                    {
                        item.Kill();
                    }

                }


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }


        public static void run() {

            try
            {

                var process = new Process();
                process.StartInfo.FileName = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "electroneumd.exe");
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.OutputDataReceived += outputDataReceived;
                process.ErrorDataReceived += errorDataReceived;
                process.Exited += exited;

                if (process.Start())
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private static void outputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data)) return;
            log.logger.write("daemon : " + e.Data, false);            
        }

        private static void errorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data)) return;
            log.logger.write("daemon error : " + e.Data, false);
        }

        private static void exited(object sender, EventArgs e)
        {
            log.logger.write("daemon : exit ", false);
        }


    }

}
*/