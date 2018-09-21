using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace electroneum_classic_csharp.res
{
    public class ModelGetBalance
    {
        public string id { get; set; }
        public string jsonrpc { get; set; }
        public ModelGetBalanceResult result { get; set; }
    }

    public class ModelGetBalanceResult
    {
        public string balance { get; set; }
        public string unlocked_balance { get; set; }
    }

}
