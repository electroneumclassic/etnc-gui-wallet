using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace electroneum_classic_csharp.res
{
    public class ModelGetAddress
    {
        public string id { get; set; }
        public string jsonrpc { get; set; }
        public ModelGetAddressResult result { get; set; }
    }

    public class ModelGetAddressResult {
        public string address { get; set; }
    }

}
