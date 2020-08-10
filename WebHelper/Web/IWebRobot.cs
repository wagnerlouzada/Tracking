using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helper
{
    public delegate void LogEvent(String Msg);

    public interface IWebCrawler
    {

        Task<Dictionary<String, Object>> ExecuteAction(IDictionary<String, Object> parameters);

        void onLogEvent(String Mensagem);
        event LogEvent OnlogEvent;
    }

    public class WebException : Exception
    {
        public int ErrorCode { get; set; }

        public WebException(String message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}