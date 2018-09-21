using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace electroneum_classic_csharp.res
{
    public  class ModelGetTransfers
    {
        public string id { get; set; }
        public string jsonrpc { get; set; }
        public ModelGetTransfersResult result { get; set; }
    }

    public class ModelGetTransfersResult
    {
        [JsonProperty(PropertyName = "in")]
        public List<ModelGetTransfersResultItem> _in { get; set; }

        [JsonProperty(PropertyName = "out")]
        public List<ModelGetTransfersResultItem> _out { get; set; }

        public List<ModelGetTransfersResultItem> pool { get; set; }
        public List<ModelGetTransfersResultItem> pending { get; set; }
        public List<ModelGetTransfersResultItem> failed { get; set; }

    }

    public class ModelGetTransfersResultItem
    {
        public string amount { get; set; }
        public string fee { get; set; }
        public string height { get; set; }
        public string note { get; set; }
        public string payment_id { get; set; }
        public string timestamp { get; set; }
        public string txid { get; set; }
        public string type { get; set; }
        public string unlock_time { get; set; }

    }
}
