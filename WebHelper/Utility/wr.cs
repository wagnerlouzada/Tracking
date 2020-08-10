using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class wrException : Exception
    {
        public wrException() : base()
        { }

        public wrException(String Message) : base(Message)
        { }

        public wrException(String Message, Exception InnerException) : base(Message, InnerException)
        { }
    }
    public class wrCommException : wrException
    {
        public String Url { get; }
        public HttpContent Content { get; }
        public HttpMethod Method { get; }

        public wrCommException(String url, HttpMethod method, HttpContent content, String Message) : base(Message)
        {
            Url = url;
            Content = content;
            Method = method;
        }

        public wrCommException(String url, HttpMethod method, HttpContent content, String Message, Exception InnerException) : base(Message, InnerException)
        {
            Url = url;
            Content = content;
            Method = method;
        }
    }

    public class wrNotAFileException : wrException
    {
        public String Url { get; }
        public HttpContent Content { get; }
        public HttpMethod Method { get; }

        public wrNotAFileException(String url, HttpMethod method, HttpContent content, String Message) : base(Message)
        {
            Url = url;
            Content = content;
            Method = method;
        }

        public wrNotAFileException(String url, HttpMethod method, HttpContent content, String Message, Exception InnerException) : base(Message, InnerException)
        {
            Url = url;
            Content = content;
            Method = method;
        }
    }

    public static class HttpContentExtensions
    {
        public static async Task<string> ReadAsStringUTF8Async(this HttpContent content)
        {
            return await content.ReadAsStringAsync(Encoding.UTF8);
        }

        public static async Task<string> ReadAsStringISO8859Async(this HttpContent content)
        {
            return await content.ReadAsStringAsync(Encoding.GetEncoding("ISO-8859-1"));
        }

        public static async Task<string> ReadAsStringAsync(this HttpContent content, Encoding encoding)
        {
            using (var reader = new StreamReader((await content.ReadAsStreamAsync()), encoding))
            {
                return reader.ReadToEnd();
            }
        }
        public static Task ReadAsFileAsync(this HttpContent content, string filename, bool overwrite)
        {
            string pathname = Path.GetFullPath(filename);
            if (!overwrite && File.Exists(filename))
            {
                throw new InvalidOperationException(string.Format("File {0} already exists.", pathname));
            }

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
                return content.CopyToAsync(fileStream).ContinueWith(
                    (copyTask) =>
                    {
                        fileStream.Close();
                    });
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }

                throw;
            }
        }
    }


}
