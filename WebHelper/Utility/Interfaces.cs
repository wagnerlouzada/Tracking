using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public interface ILogger
    {
        void LogFormat(LoggerLevel ll, String msg, params Object[] pars);
        void LogError(Exception ex, String msg, params Object[] pars);
    }

    public enum LoggerLevel
    {
        DEBUG,
        ERROR,
        FATAL,
    }


}
