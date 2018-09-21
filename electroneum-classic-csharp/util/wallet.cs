using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace electroneum_classic_csharp.util
{
    public class wallet
    {
        //public static string remote_daemon_host = "electroneum-classic.com";
        //public static string remote_daemon_host = "173.208.215.74";
        public static string remote_daemon_host = "wallet.electroneum-classic.com";
        public static string remote_daemon_port = "26978";

        public static string address;
        public static double balance = 0.00;
        public static double unlocked = 0.00;
        public static List<electroneum_classic_csharp.res.ModelGetTransfersResultItem> txResult =
            new List<res.ModelGetTransfersResultItem>();
        public static Process processWalletRpc = new Process();

        public static void Create(string filename, string password) {
            try
            {
                //create wallet
                var process = new Process();
                process.StartInfo.FileName = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "electroneum-wallet-cli.exe");
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                var arg = string.Format("--daemon-address {0}:{1} --generate-new-wallet {2} --password \"{3}\" --mnemonic-language English --allow-mismatched-daemon-version",
                                         remote_daemon_host,
                                         remote_daemon_port,
                                         filename,
                                         password);

                process.StartInfo.Arguments = arg;
                process.OutputDataReceived += outputDataReceived;
                process.ErrorDataReceived += errorDataReceived;
                process.Exited += exited;

                if (process.Start())
                {
                    process.StandardInput.WriteLine("exit");

                    // Begin redirecting output
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void CreateFromMnemonic(string filename, string password, string mnemonic_seed, string restore_height)
        {
            try
            {
                //create wallet
                var process = new Process();
                process.StartInfo.FileName = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "electroneum-wallet-cli.exe");
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                var arg = string.Format("--daemon-address {0}:{1} --generate-new-wallet {2} --password \"{3}\" --restore-deterministic-wallet",
                                         remote_daemon_host,
                                         remote_daemon_port,
                                         filename,
                                         password);

                process.StartInfo.Arguments = arg;
                process.OutputDataReceived += outputDataReceived;
                process.ErrorDataReceived += errorDataReceived;
                process.Exited += exited;

                if (process.Start())
                {
                    process.StandardInput.WriteLine(mnemonic_seed);
                    process.StandardInput.WriteLine(restore_height);
                    //process.StandardInput.WriteLine(System.Environment.NewLine);
                    process.StandardInput.WriteLine("exit");

                    // Begin redirecting output
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            if (processWalletRpc != null)
            {
                processWalletRpc.Kill();
            }
        }

        public static void RunRPC(string filename, string password)
        {
            try
            {
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "electroneum-wallet-rpc.26980.login"));

                AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

                processWalletRpc.StartInfo.FileName = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "electroneum-wallet-rpc.exe");
                processWalletRpc.StartInfo.CreateNoWindow = true;
                processWalletRpc.StartInfo.UseShellExecute = false;
                processWalletRpc.StartInfo.RedirectStandardInput = true;
                processWalletRpc.StartInfo.RedirectStandardOutput = true;
                processWalletRpc.StartInfo.RedirectStandardError = true;

                //filename = Path.ChangeExtension(filename, null);
                var arg = string.Format("--daemon-address {0}:{1} --wallet-file \"{2}\" --password \"{3}\" --rpc-bind-port 26980 --rpc-login etnc:etnc",
                                         remote_daemon_host,
                                         remote_daemon_port,
                                         filename,
                                         password);

                processWalletRpc.StartInfo.Arguments = arg;
                processWalletRpc.OutputDataReceived += outputDataReceived;
                processWalletRpc.ErrorDataReceived += errorDataReceived;
                processWalletRpc.Exited += exited;

                if (processWalletRpc.Start())
                {
                    // Begin redirecting output
                    processWalletRpc.BeginOutputReadLine();
                    processWalletRpc.BeginErrorReadLine();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public static res.ModelGetAddress GetWalletAddress() {
            try
            {
                // Create a POST request
                HttpWebRequest HttpWebRequest = (HttpWebRequest)WebRequest.Create(@"http://127.0.0.1:26980/json_rpc");
                HttpWebRequest.Credentials = new NetworkCredential("etnc", "etnc");
                HttpWebRequest.ContentType = "application/json-rpc";
                HttpWebRequest.Method = "POST";

                // Create a JSON request
                JObject JRequest = new JObject();                
                JRequest.Add(new JProperty("jsonrpc", "2.0"));
                JRequest.Add(new JProperty("id", "0"));
                JRequest.Add(new JProperty("method", "getaddress"));
                JRequest.Add(new JProperty("params", "{}"));
                String Request = JRequest.ToString();

                // Send bytes to server
                byte[] ByteArray = Encoding.UTF8.GetBytes(Request);
                HttpWebRequest.ContentLength = ByteArray.Length;
                Stream Stream = HttpWebRequest.GetRequestStream();
                Stream.Write(ByteArray, 0, ByteArray.Length);
                Stream.Close();

                // Receive reply from server
                WebResponse WebResponse = HttpWebRequest.GetResponse();
                StreamReader reader = new StreamReader(WebResponse.GetResponseStream(), Encoding.UTF8);

                // Get response
                res.ModelGetAddress result = JsonConvert.DeserializeObject<res.ModelGetAddress>(JObject.Parse(reader.ReadToEnd()).ToString());

                // Dispose of pieces
                reader.Dispose();

                address = result.result.address;
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public static res.ModelGetBalance GetBalance()
        {
            try
            {
                // Create a POST request
                HttpWebRequest HttpWebRequest = (HttpWebRequest)WebRequest.Create(@"http://127.0.0.1:26980/json_rpc");
                HttpWebRequest.Credentials = new NetworkCredential("etnc", "etnc");
                HttpWebRequest.ContentType = "application/json-rpc";
                HttpWebRequest.Method = "POST";

                // Create a JSON request
                JObject JRequest = new JObject();
                JRequest.Add(new JProperty("jsonrpc", "2.0"));
                JRequest.Add(new JProperty("id", "0"));
                JRequest.Add(new JProperty("method", "getbalance"));
                JRequest.Add(new JProperty("params", "{}"));
                String Request = JRequest.ToString();

                // Send bytes to server
                byte[] ByteArray = Encoding.UTF8.GetBytes(Request);
                HttpWebRequest.ContentLength = ByteArray.Length;
                Stream Stream = HttpWebRequest.GetRequestStream();
                Stream.Write(ByteArray, 0, ByteArray.Length);
                Stream.Close();

                // Receive reply from server
                WebResponse WebResponse = HttpWebRequest.GetResponse();
                StreamReader reader = new StreamReader(WebResponse.GetResponseStream(), Encoding.UTF8);

                // Get response
                res.ModelGetBalance result = JsonConvert.DeserializeObject<res.ModelGetBalance>(JObject.Parse(reader.ReadToEnd()).ToString());

                // Dispose of pieces
                reader.Dispose();

                balance = double.Parse(result.result.balance) / 100;
                unlocked = double.Parse(result.result.unlocked_balance) / 100;

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public static res.ModelGetTransfers GetTransaction()
        {

            try
            {
                // Create a POST request
                HttpWebRequest HttpWebRequest = (HttpWebRequest)WebRequest.Create(@"http://127.0.0.1:26980/json_rpc");
                HttpWebRequest.Credentials = new NetworkCredential("etnc", "etnc");
                HttpWebRequest.ContentType = "application/json-rpc";
                HttpWebRequest.Method = "POST";

                JObject JParam = new JObject();
                JParam.Add(new JProperty("in", true));
                JParam.Add(new JProperty("out", true));
                JParam.Add(new JProperty("pool", true));
                JParam.Add(new JProperty("pending", true));
                JParam.Add(new JProperty("failed", true));

                // Create a JSON request
                JObject JRequest = new JObject();
                JRequest.Add(new JProperty("jsonrpc", "2.0"));
                JRequest.Add(new JProperty("id", "0"));
                JRequest.Add(new JProperty("method", "get_transfers"));
                JRequest.Add(new JProperty("params", JObject.FromObject(JParam)));
                String Request = JRequest.ToString();

                // Send bytes to server
                byte[] ByteArray = Encoding.UTF8.GetBytes(Request);
                HttpWebRequest.ContentLength = ByteArray.Length;
                Stream Stream = HttpWebRequest.GetRequestStream();
                Stream.Write(ByteArray, 0, ByteArray.Length);
                Stream.Close();

                // Receive reply from server
                WebResponse WebResponse = HttpWebRequest.GetResponse();
                StreamReader reader = new StreamReader(WebResponse.GetResponseStream(), Encoding.UTF8);

                // Get response
                res.ModelGetTransfers result = JsonConvert.DeserializeObject<res.ModelGetTransfers>(JObject.Parse(reader.ReadToEnd()).ToString());

                // Dispose of pieces
                reader.Dispose();

                txResult.Clear();
                if (result.result._in != null)
                {
                    foreach (res.ModelGetTransfersResultItem item in result.result._in)
                    {
                        txResult.Add(item);
                    }
                }
                if (result.result._out != null)
                {
                    foreach (res.ModelGetTransfersResultItem item in result.result._out)
                    {
                        txResult.Add(item);
                    }
                }
                if (result.result.pending != null)
                {
                    foreach (res.ModelGetTransfersResultItem item in result.result.pending)
                    {
                        txResult.Add(item);
                    }
                }
                if (result.result.failed != null)
                {
                    foreach (res.ModelGetTransfersResultItem item in result.result.failed)
                    {
                        txResult.Add(item);
                    }
                }
                if (result.result.pool != null)
                {
                    foreach (res.ModelGetTransfersResultItem item in result.result.pool)
                    {
                        txResult.Add(item);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public static string Transfer(string address, int amount, string paymentid)
        {
            try
            {
                // Create a POST request
                HttpWebRequest HttpWebRequest = (HttpWebRequest)WebRequest.Create(@"http://127.0.0.1:26980/json_rpc");
                HttpWebRequest.Credentials = new NetworkCredential("etnc", "etnc");
                HttpWebRequest.ContentType = "application/json-rpc";
                HttpWebRequest.Method = "POST";

                JObject JParams = new JObject();
                JParams.Add(new JProperty("jsonrpc", "2.0"));

                JObject JParam = new JObject();
                JParam.Add(new JProperty("get_tx_key", true));
                if (paymentid != null)
                {
                    JParam.Add(new JProperty("payment_id ", paymentid));
                }
                JParam.Add(new JProperty("mixin", 1));

                JObject dest = new JObject();
                dest.Add(new JProperty("amount", amount));
                dest.Add(new JProperty("address", address));

                JArray arr = new JArray();
                arr.Add(dest);

                JParam.Add(new JProperty("destinations", arr));

                // Create a JSON request
                JObject JRequest = new JObject();
                JRequest.Add(new JProperty("jsonrpc", "2.0"));
                JRequest.Add(new JProperty("id", "0"));
                JRequest.Add(new JProperty("method", "transfer"));
                JRequest.Add(new JProperty("params", JObject.FromObject(JParam)));
                String Request = JRequest.ToString();

                // Send bytes to server
                byte[] ByteArray = Encoding.UTF8.GetBytes(Request);
                HttpWebRequest.ContentLength = ByteArray.Length;
                Stream Stream = HttpWebRequest.GetRequestStream();
                Stream.Write(ByteArray, 0, ByteArray.Length);
                Stream.Close();

                // Receive reply from server
                WebResponse WebResponse = HttpWebRequest.GetResponse();
                StreamReader reader = new StreamReader(WebResponse.GetResponseStream(), Encoding.UTF8);

                // Get response
                res.ModelTransfer result = JsonConvert.DeserializeObject<res.ModelTransfer>(JObject.Parse(reader.ReadToEnd()).ToString());

                // Dispose of pieces
                reader.Dispose();

                if (result.result != null)
                {
                    return result.result.tx_hash;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private static void outputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data)) return;
            log.logger.write("wallet : " + e.Data, false);
        }

        private static void errorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Data)) return;
            log.logger.write("wallet error : " + e.Data, false);
        }

        private static void exited(object sender, EventArgs e)
        {
            log.logger.write("wallet : exit ", false);
        }

        public static bool ping(string host, string port)
        {
            using (TcpClient tcpc = new TcpClient())
            {
                try
                {
                    tcpc.Connect(host, int.Parse(port));
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

    }
}
