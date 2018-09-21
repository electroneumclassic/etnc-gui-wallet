using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace electroneum_classic_csharp.res
{
    public class ModelTransfer
    {
        public string id { get; set; }
        public string jsonrpc { get; set; }
        public ModelTransferResult result { get; set; }
    }

    public class ModelTransferResult
    {
        public string fee { get; set; }
        public string tx_key { get; set; }
        public string tx_hash { get; set; }
    }
}
