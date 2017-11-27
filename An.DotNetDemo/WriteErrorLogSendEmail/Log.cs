using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteErrorLogSendEmail
{

    public class Log
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("logger");

        public static void Debug(string message)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(message);
            }
        }

        public static void Debug(System.Exception ex1)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(ex1.Message.ToString() + "/r/n" + ex1.Source.ToString() + "/r/n" + ex1.TargetSite.ToString() + "/r/n" + ex1.StackTrace.ToString());
            }
        }

        public static void Error(string message)
        {
            if (log.IsErrorEnabled)
            {
                log.Error(message);
            }
        
}

        public static void Fatal(string message)
        {

            if (log.IsFatalEnabled)
            {
                log.Fatal(message);
            }
        }

        public static void Info(string message)
        {
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
        }

        public static void Warn(string message)
        {
            if (log.IsWarnEnabled)
            {
                log.Warn(message);
            }
        }
    }
}

