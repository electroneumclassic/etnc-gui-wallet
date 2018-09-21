using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace electroneum_classic_csharp.log
{
    public class logger
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void write(string data, bool error) {
            if (error)
            {
                log.Error(data);
            }
            else {
                log.Info(data);
            }
        }

    }
}
