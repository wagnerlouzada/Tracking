using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
//using Tradimus.WebAutomation.Extensions;
//using Tradimus.WebAutomation.Interfaces;
//using Tradimus.WebAutomation.Utils;
using System.Reflection;
//using System.Windows.Forms;

namespace Helper
{
    public class WebHelper : IDisposable
    {
        public CookieContainer cookieContainer;

        public WebRequestHandler httpHandler;
        public HttpClient client;
        public int timeOut = 120;
        public Uri baseAddress = null;
        private Boolean started = false;
        public Encoding charCode = null;
        public CultureInfo culture = new CultureInfo("pt-BR");
        public String urlAddress;
        public String Referer;
        public HttpResponseHeaders lastResponseHeaders;
        public HttpContentHeaders lastContentHeaders;
        public int retryCount = 3;
        public int delayRetry = 15;
        private Boolean autoRedirect = true;
        private Boolean cookiesValidation = false;
        public ILogger logEngine;

        public void CultureConfig(String cultura)
        {
            culture = new CultureInfo(cultura);
        }

        public WebHelper()
        {
            // Client = null;
            cookieContainer = null;
            httpHandler = null;
            timeOut = 120;
            baseAddress = null;
            started = false;
            //connections = null;
            charCode = null;
            urlAddress = null;
            Referer = null;
            lastResponseHeaders = null;
            lastContentHeaders = null;
            logEngine = null;
        }

        // wl em 16/3/2020
        public Boolean ProcessCookies { get; set; } = true;

        public List<KeyValuePair<String, String>> AllCookies;

        ///      
        /// <summary>
        /// 
        ///  Get values like JSESSIONID from cookie
        ///  --------------------------------------
        ///  First try with exact parameter name, if not success try with contains parnName at cookie item name
        ///  if find with exact paramNaame, returns only parmValue
        ///  else, returns parmName = parmValue
        ///  
        /// All domains
        ///  List<variavel> = GetCookieListItem("JsessionId");
        ///  get only jsessionid if exists... return only parm value
        ///  
        /// Specifi domain
        ///  List<variavel> = GetCookieListItem("session", "autenticacao.frg.com.br");
        ///  get all values with parm like session... returns JSESSIONID = <value>
        ///  
        /// used at RoboRealGrandeza.cs
        /// 
        /// </summary>
        /// 
        /// <param name="ParmName", [domainname without http or https]></param>
        /// <returns></returns>
        /// 
        public List<String> GetCookieListItem(String ParmName, Boolean ReturnFull = false)
        {
            List<String> coockieValue = new List<String>();

            CookieContainer cookies = new CookieContainer();
            cookies = this.cookieContainer;

            var allCookies = new CookieCollection();
            var domainTableField = cookies.GetType().GetRuntimeFields().FirstOrDefault(x => x.Name == "m_domainTable");
            var domains = (IDictionary)domainTableField.GetValue(cookies);

            foreach (var val in domains.Values)
            {
                var type = val.GetType().GetRuntimeFields().First(x => x.Name == "m_list");
                var values = (IDictionary)type.GetValue(val);
                foreach (CookieCollection cookiesdata in values.Values)
                {
                    allCookies.Add(cookiesdata);
                }
            }

            foreach (var itms in allCookies)
            {
                string[] ckVal = itms.ToString().Split('=');
                if (ckVal[0].Trim().ToUpper() == ParmName.ToUpper())
                {
                    if (ReturnFull)
                    {
                        coockieValue.Add(itms.ToString());
                    }
                    else
                    {
                        coockieValue.Add(ckVal[1]);
                    }
                }
            }

            //
            // tentar match
            //
            if (coockieValue.Count() == 0)
            {
                foreach (var itms in allCookies)
                {
                    string[] ckVal = itms.ToString().Split('=');
                    if (ckVal[0].Trim().ToUpper().Contains(ParmName.ToUpper()))
                    {
                        string parm = itms.ToString();
                        coockieValue.Add(parm);
                    }
                }
            }

            return coockieValue;
        }

        public List<String> GetCookieListItem(String ParmName, String Domain, Boolean ReturnFull = false)
        {
            List<String> coockieValue = new List<String>();

            CookieContainer cookies = new CookieContainer();
            cookies = this.cookieContainer;

            var allCookies = new CookieCollection();
            var domainTableField = cookies.GetType().GetRuntimeFields().FirstOrDefault(x => x.Name == "m_domainTable");
            var domains = (IDictionary)domainTableField.GetValue(cookies);

            var val = domains[Domain];
            {
                var type = val.GetType().GetRuntimeFields().First(x => x.Name == "m_list");
                var values = (IDictionary)type.GetValue(val);
                foreach (CookieCollection cookiesdata in values.Values)
                {
                    allCookies.Add(cookiesdata);
                }
            }

            foreach (var itms in allCookies)
            {
                string[] ckVal = itms.ToString().Split('=');
                if (ckVal[0].Trim().ToUpper() == ParmName.ToUpper())
                {
                    if (ReturnFull)
                    {
                        coockieValue.Add(itms.ToString());
                    }
                    else
                    {
                        coockieValue.Add(ckVal[1]);
                    }
                }
            }

            //
            // tentar match
            //
            if (coockieValue.Count() == 0)
            {
                foreach (var itms in allCookies)
                {
                    string[] ckVal = itms.ToString().Split('=');
                    if (ckVal[0].Trim().ToUpper().Contains(ParmName.ToUpper()))
                    {
                        coockieValue.Add(itms.ToString());
                    }
                }
            }

            return coockieValue;
        }

        public void ReadCookies(HttpResponseMessage response)
        {
            if (!cookiesValidation)
                return;

            var pageUri = response.RequestMessage.RequestUri;

            //var cookieContainer = new CookieContainer();
            IEnumerable<string> cookies;
            if (response.Headers.TryGetValues("set-cookie", out cookies))
            {
                if (ProcessCookies) // wl em 2020-3-16
                {
                    foreach (var c in cookies)
                    {
                        //AllCookies.Add(new KeyValuePair<string, string>(pageUri.ToString(), c ));

                        if (c.IndexOf(';') != -1)
                        {
                            String[] cparts = c.Split(';').Select(s => s.Trim()).ToArray();
                            String ckName = cparts[0].Split('=').First();
                            String ckValue = cparts[0].Split('=').Last();
                            String ckPath = cparts.Where(s => s.ToLower().IndexOf("path=") != -1).FirstOrDefault();

                            if (ckPath != null) ckPath = ckPath.Split('=').Last();

                            String storedCookieHeader = cookieContainer.GetCookieHeader(new Uri(baseAddress.OriginalString + ckPath.Substring(1)));

                            if (storedCookieHeader.IndexOf(ckValue) == -1)
                            {
                                Cookie ck = new Cookie(ckName, ckValue, ckPath);
                                cookieContainer.Add(new Uri(baseAddress.OriginalString + ckPath.Substring(1)), ck);
                            }
                        }
                    }
                    // cookieContainer.SetCookies(pageUri, c);
                }
                else // wl em 2020.3.16
                {
                    // AllCookies.Add(new KeyValuePair<string, string>(pageUri.ToString(), cookies)); // wl em 2020-3-16
                }
            }
            //return cookieContainer;
        }

        //private ConcurrentQueue<HttpClient> connections = null;
        public WebHelper(String enderecoBase, int timeout = 120, Encoding enc = null, Boolean DescompressaoAutomatica = false, Dictionary<String, String> defheaders = null, Boolean defaultCacheControlHeader = true, Boolean redirecionamentoAutomatico = true, Boolean validarCookies = false, ILogger logger = null)
        {
            logEngine = logger;
            AllCookies = new List<KeyValuePair<string, String>>();
            autoRedirect = redirecionamentoAutomatico;
            cookiesValidation = validarCookies;
            // Client = null;
            cookieContainer = null;
            httpHandler = null;
            timeOut = 120;
            baseAddress = null;
            started = false;
            //connections = null;
            charCode = null;
            urlAddress = null;
            Referer = null;
            lastResponseHeaders = null;
            lastContentHeaders = null;

            Initialize(enderecoBase, timeout, enc, DescompressaoAutomatica, defheaders, defaultCacheControlHeader);
        }

        /// <summary>
        /// Inicializa as conexoes de comunicacao.
        /// </summary>
        /// <param name="bAddress">Endereço raiz do site</param>
        /// <param name="timeout">Tempo maximo de espera em segundos</param>
        public void Initialize(String bAddress, int timeout = 120, Encoding enc = null, Boolean autoDecomp = false, Dictionary<String, String> defheaders = null, Boolean defaultCacheControlHeader = true)
        {
            if (!started)
            {
                baseAddress = new Uri(bAddress);
                timeOut = timeout;
                cookieContainer = new CookieContainer();

                // HttpRequestCachePolicy reqCachePolicy = new HttpRequestCachePolicy( HttpCacheAgeControl.MaxAge, TimeSpan.FromSeconds(1));
                RequestCachePolicy reqCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

                // cenario de teste
                ServicePointManager.MaxServicePointIdleTime = timeout;
                // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12; //768 for TLS 1.1 and 3072 for TLS 1.2
                // ServicePointManager.CertificatePolicy = new MyPolicy();
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                if (autoDecomp)
                    httpHandler = new WebRequestHandler() { CookieContainer = cookieContainer, CachePolicy = reqCachePolicy, UseCookies = true, AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
                else httpHandler = new WebRequestHandler() { CookieContainer = cookieContainer, CachePolicy = reqCachePolicy, UseCookies = true };

                httpHandler.AllowAutoRedirect = autoRedirect;

                client = new HttpClient(httpHandler) { BaseAddress = baseAddress, Timeout = TimeSpan.FromSeconds(timeOut), MaxResponseContentBufferSize = Int32.MaxValue };

                if (defaultCacheControlHeader)
                {
                    client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();
                    client.DefaultRequestHeaders.CacheControl.MaxAge = TimeSpan.FromSeconds(0);
                }

                if (defheaders != null)
                    foreach (KeyValuePair<String, String> kv in defheaders)
                        client.DefaultRequestHeaders.Add(kv.Key, kv.Value);

                if (enc != null)
                    charCode = enc;
                else
                    charCode = Encoding.UTF8;
                started = true;
            }
        }

        public void changeContexEncoding(Encoding newEncoding)
        {
            charCode = newEncoding;
        }

        private void logDebug(String msg, params Object[] pars)
        {
            if (logEngine != null)
            {
                long id = Thread.CurrentThread.ManagedThreadId;
                logEngine.LogFormat(LoggerLevel.DEBUG, "Thread ID:" + id.ToString() + " - " + msg, pars);
            }
        }

        private void logError(String msg, params object[] pars)
        {
            if (logEngine != null)
            {
                long id = Thread.CurrentThread.ManagedThreadId;
                logEngine.LogFormat(LoggerLevel.ERROR, "Thread ID:" + id.ToString() + " - " + msg, pars);
            }
        }

        private void logError(Exception error, String msg, params object[] pars)
        {
            if (logEngine != null)
            {
                logEngine.LogError(error, msg, pars);
            }
        }

        public void AddCookie(Cookie cookie)
        {
            cookieContainer.Add(cookie);
        }

        public void AddDefaultHeader(String key, String value)
        {
            client.DefaultRequestHeaders.Remove(key);
            client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
        }

        // informa Falhas que são passíveis de re-tentativas...
        private bool IsTransient(Exception ex)
        {
            System.Net.WebException webException = null;
            if (ex is AggregateException)
            {
                AggregateException aex = (ex as AggregateException);
                foreach (Exception fex in aex.Flatten().InnerExceptions)
                {
                    webException = fex as System.Net.WebException;
                    if (webException != null)
                        break;
                    if (fex.InnerException is System.Net.WebException)
                    {
                        webException = fex.InnerException as System.Net.WebException;
                        break;
                    }
                }
            }

            if (webException == null)
                webException = ex as System.Net.WebException;

            if (webException != null)
            {
                // If the web exception contains one of the following status values
                // it might be transient.
                return new[] {WebExceptionStatus.ConnectionClosed,
                  WebExceptionStatus.Timeout,
                  WebExceptionStatus.RequestCanceled }.
                        Contains(webException.Status);
            }

            // Additional exception checking logic goes here.
            return false;
        }

        public wrResponse GetPageR(String url, TDictionary fields = null, Dictionary<String, String> requestHeaders = null, bool keepAlive = true, int getRetryCount = -1)
        {
            if (client == null)
                throw new ArgumentException("Conexão não inicializada!");

            if (url.StartsWith("/") && client.BaseAddress.OriginalString.EndsWith("/"))
                url = url.Substring(1);

            string urlfinal;
            if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                urlfinal = url;
            else
                urlfinal = client.BaseAddress + url;

            // adiniona os parametros a url...
            if (fields != null && fields.Count > 0)
            {
                String iniSep = "?";
                if (urlfinal.Contains("?"))
                {
                    iniSep = "&";
                }
                String aux = "";
                foreach (var f in fields)
                    aux = aux + "&" + f.Key + "=" + f.Value;
                aux = aux.Substring(1);
                urlfinal = urlfinal + iniSep + aux;
            }

            if (getRetryCount == -1) // utiliza o padrao 
                getRetryCount = retryCount;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(urlfinal),
                        Method = HttpMethod.Get,
                    };

                    // Header padrão
                    //foreach (var defHeader in Client.DefaultRequestHeaders)
                    //    request.Headers.TryAddWithoutValidation(defHeader.Key, defHeader.Value);

                    // Header para o request
                    if (requestHeaders != null)
                        foreach (KeyValuePair<String, String> kv in requestHeaders)
                        {
                            request.Headers.Remove(kv.Key);
                            request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                        }

                    if (!keepAlive)
                    {
                        request.Headers.ConnectionClose = true;
                        //    request.Headers.ExpectContinue = true;
                    }

                    var GETResponse = client.SendAsync(request);
                    var ResponseResult = GETResponse.Result;

                    lastContentHeaders = ResponseResult.Content.Headers;
                    lastResponseHeaders = ResponseResult.Headers;

                    ReadCookies(ResponseResult);
                    //var cookies = cookieContainer.GetCookies(new Uri(urlfinal));

                    if (urlAddress != null)
                        Referer = urlAddress;

                    urlAddress = urlfinal;

                    wrResponse resp = new wrResponse()
                    {
                        StatusCode = ResponseResult.StatusCode,
                        cHeaders = lastContentHeaders,
                        rHeaders = lastResponseHeaders,
                        content = LerConteudo(ResponseResult.Content)
                    };

                    //HtmlDocument html = LerConteudo(GETResponse.Result.Content);

                    return resp;
                }
                catch (Exception ex)
                {
                    currentRetry++;
                    logError(ex, "Falha de comunicação... Retry:{0}", currentRetry);
                    if (currentRetry > getRetryCount || !IsTransient(ex))
                    {
                        // If this isn't a transient error or we shouldn't retry, 
                        // rethrow the exception.
                        throw;
                    }
                    if (delayRetry > 0)
                        Thread.Sleep(1000 * delayRetry);
                }
            }
        }

        public HtmlDocument GetPage(String url, TDictionary fields, String strHeaders)
        {
            Dictionary<String, String> headers = null;
            if (!String.IsNullOrEmpty(strHeaders.Trim()))
            {
                headers = new Dictionary<string, string>();
                String[] aux = strHeaders.Split(new String[] { "||" }, StringSplitOptions.None);
                foreach (string s in aux)
                {
                    String[] kv = s.Split(new String[] { "=>" }, StringSplitOptions.None);
                    headers.Add(kv[0], kv[1].Trim());
                }
            }

            return GetPage(url, fields, headers);
        }

        public HtmlDocument GetPage(String url, TDictionary fields, Dictionary<String, String> requestHeaders = null, bool keepAlive = true, int getRetryCount = -1)
        {
            if (client == null)
                throw new ArgumentException("Conexão não inicializada!");

            if (url.StartsWith("/") && client.BaseAddress.OriginalString.EndsWith("/"))
                url = url.Substring(1);

            string urlfinal;
            if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                urlfinal = url;
            else
                urlfinal = client.BaseAddress + url;

            // adiniona os parametros a url...
            if (fields != null && fields.Count > 0)
            {
                String iniSep = "?";
                if (urlfinal.Contains("?"))
                {
                    iniSep = "&";
                }
                String aux = "";
                foreach (var f in fields)
                    aux = aux + "&" + f.Key + "=" + f.Value;
                aux = aux.Substring(1);
                urlfinal = urlfinal + iniSep + aux;
            }

            if (getRetryCount == -1) // utiliza o padrao 
                getRetryCount = retryCount;
            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(urlfinal),
                        Method = HttpMethod.Get,
                    };

                    // Header padrão
                    //foreach (var defHeader in Client.DefaultRequestHeaders)
                    //    request.Headers.TryAddWithoutValidation(defHeader.Key, defHeader.Value);

                    // Header para o request
                    if (requestHeaders != null)
                        foreach (KeyValuePair<String, String> kv in requestHeaders)
                        {
                            request.Headers.Remove(kv.Key);
                            request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                        }

                    if (!keepAlive)
                    {
                        request.Headers.ConnectionClose = true;
                        //    request.Headers.ExpectContinue = true;
                    }

                    var GETResponse = client.SendAsync(request);
                    GETResponse.Result.EnsureSuccessStatusCode();

                    lastContentHeaders = GETResponse.Result.Content.Headers;
                    lastResponseHeaders = GETResponse.Result.Headers;

                    ReadCookies(GETResponse.Result);
                    //var cookies = cookieContainer.GetCookies(new Uri(urlfinal));

                    if (urlAddress != null)
                        Referer = urlAddress;

                    urlAddress = urlfinal;

                    HtmlDocument html = LerConteudo(GETResponse.Result.Content);

                    return html;
                }
                catch (Exception ex)
                {
                    currentRetry++;
                    logError(ex, "Falha de comunicação... Retry:{0}", currentRetry);
                    if (currentRetry > getRetryCount || !IsTransient(ex))
                    {
                        // If this isn't a transient error or we shouldn't retry, 
                        // rethrow the exception.
                        throw;
                    }
                    if (delayRetry > 0)
                        Thread.Sleep(1000 * delayRetry);
                }
            }
        }

        public HtmlDocument GetPage(String url, int getRetryCount, Dictionary<String, String> requestHeaders = null, bool keepAlive = true)
        {
            return GetPage(url, null, requestHeaders, keepAlive, getRetryCount);
        }

        public HtmlDocument GetPage(String url, Dictionary<String, String> requestHeaders = null, bool keepAlive = true)
        {
            return GetPage(url, null, requestHeaders, keepAlive);
        }

        public byte[] GetFile(HttpMethod method, String url, HttpContent content, Boolean testFileHeaders = true, Dictionary<String, String> requestHeaders = null)
        {
            String NomeArq = null;
            return GetFile(method, url, content, out NomeArq, testFileHeaders, requestHeaders);
        }

        public byte[] GetFile(HttpMethod method, String url, HttpContent content, out String nomeArqOriginal, Boolean testFileHeaders = true, Dictionary<String, String> requestHeaders = null)
        {
            HttpResponseMessage resFile = null;

            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    if (url.StartsWith("/") && client.BaseAddress.OriginalString.EndsWith("/"))
                        url = url.Substring(1);

                    string urlfinal;
                    if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                        urlfinal = url;
                    else
                        urlfinal = client.BaseAddress + url;


                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(urlfinal),
                        Content = (method == HttpMethod.Post) ? content : null,
                        Method = (method == HttpMethod.Post) ? HttpMethod.Post : HttpMethod.Get,
                    };

                    if (requestHeaders != null)
                        foreach (KeyValuePair<String, String> kv in requestHeaders)
                        {
                            request.Headers.Remove(kv.Key);
                            request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                        }

                    var GETResponse = client.SendAsync(request);
                    //    GETResponse.Result.EnsureSuccessStatusCode();

                    //if (method == HttpMethod.Post)
                    //    resFile = Client.PostAsync(urlfinal, content).Result;
                    //else
                    //    resFile = Client.GetAsync(urlfinal).Result;

                    nomeArqOriginal = null;
                    if (GETResponse.Result.IsSuccessStatusCode)
                    {
                        resFile = GETResponse.Result;
                        if (testFileHeaders)
                        {
                            if (resFile.Content.Headers.ContentDisposition != null && !String.IsNullOrEmpty(resFile.Content.Headers.ContentDisposition.FileName)) // esta retornando um arquivo
                            {
                                nomeArqOriginal = resFile.Content.Headers.ContentDisposition.FileName;

                                return resFile.Content.ReadAsByteArrayAsync().Result;
                            }
                            else
                                throw new wrNotAFileException(url, method, content, "Retorno fora do esperado, não há informações no cabeçalho http.");
                        }
                        else
                        {
                            return resFile.Content.ReadAsByteArrayAsync().Result;
                        }
                    }

                    return null;

                }
                catch (Exception ex)
                {
                    currentRetry++;
                    logError(ex, "Falha de comunicação... Retry:{0}", currentRetry);
                    if (currentRetry > retryCount || !IsTransient(ex))
                    {
                        // If this isn't a transient error or we shouldn't retry, 
                        // rethrow the exception.
                        if (ex is AggregateException)
                        {
                            AggregateException aex = ex as AggregateException;
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("Erro: " + aex.Message);
                            sb.AppendLine("StackTrace: " + aex.StackTrace);
                            int cex = 0;
                            foreach (Exception ex2 in aex.Flatten().InnerExceptions)
                            {
                                cex++;
                                sb.AppendLine("-------------------------------------------------");
                                sb.AppendLine("Erro " + cex.ToString() + " (" + ex2.GetType().Name + "): " + ex2.Message);
                                sb.AppendLine("StackTrace: " + ex2.StackTrace);
                                if (ex2.InnerException != null)
                                {
                                    sb.AppendLine("InnerException: " + ex2.InnerException.ToString() + " (" + ex2.InnerException.GetType().Name + "): " + ex2.InnerException.Message);
                                    sb.AppendLine("InnerException StackTrace: " + ex2.InnerException.StackTrace);
                                }
                            }

                            throw new wrCommException(url, method, content, sb.ToString(), aex.Flatten());
                        }

                        throw;
                    }
                    if (delayRetry > 0)
                        Thread.Sleep(1000 * delayRetry);
                }
            }
        }

        public Task<byte[]> GetFileAsync(HttpMethod method, String url, HttpContent content, out String nomeArqOriginal, Boolean testFileHeaders = true, Dictionary<String, String> requestHeaders = null)
        {
            HttpResponseMessage resFile = null;

            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    if (url.StartsWith("/") && client.BaseAddress.OriginalString.EndsWith("/"))
                        url = url.Substring(1);

                    string urlfinal;
                    if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                        urlfinal = url;
                    else
                        urlfinal = client.BaseAddress + url;


                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(urlfinal),
                        Content = (method == HttpMethod.Post) ? content : null,
                        Method = (method == HttpMethod.Post) ? HttpMethod.Post : HttpMethod.Get,
                    };

                    if (requestHeaders != null)
                        foreach (KeyValuePair<String, String> kv in requestHeaders)
                        {
                            request.Headers.Remove(kv.Key);
                            request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                        }

                    var GETResponse = client.SendAsync(request);
                    //    GETResponse.Result.EnsureSuccessStatusCode();

                    //if (method == HttpMethod.Post)
                    //    resFile = Client.PostAsync(urlfinal, content).Result;
                    //else
                    //    resFile = Client.GetAsync(urlfinal).Result;

                    nomeArqOriginal = null;
                    if (GETResponse.Result.IsSuccessStatusCode)
                    {
                        resFile = GETResponse.Result;
                        if (testFileHeaders)
                        {
                            if (resFile.Content.Headers.ContentDisposition != null && !String.IsNullOrEmpty(resFile.Content.Headers.ContentDisposition.FileName)) // esta retornando um arquivo
                            {
                                nomeArqOriginal = resFile.Content.Headers.ContentDisposition.FileName;

                                return resFile.Content.ReadAsByteArrayAsync();
                            }
                            else
                                throw new wrNotAFileException(url, method, content, "Retorno fora do esperado, não há informações no cabeçalho http.");
                        }
                        else
                        {
                            return resFile.Content.ReadAsByteArrayAsync();
                        }
                    }

                    return null;

                }
                catch (Exception ex)
                {
                    currentRetry++;
                    logError(ex, "Falha de comunicação... Retry:{0}", currentRetry);
                    if (currentRetry > retryCount || !IsTransient(ex))
                    {
                        // If this isn't a transient error or we shouldn't retry, 
                        // rethrow the exception.
                        if (ex is AggregateException)
                        {
                            AggregateException aex = ex as AggregateException;
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("Erro: " + aex.Message);
                            sb.AppendLine("StackTrace: " + aex.StackTrace);
                            int cex = 0;
                            foreach (Exception ex2 in aex.Flatten().InnerExceptions)
                            {
                                cex++;
                                sb.AppendLine("-------------------------------------------------");
                                sb.AppendLine("Erro " + cex.ToString() + " (" + ex2.GetType().Name + "): " + ex2.Message);
                                sb.AppendLine("StackTrace: " + ex2.StackTrace);
                                if (ex2.InnerException != null)
                                {
                                    sb.AppendLine("InnerException: " + ex2.InnerException.ToString() + " (" + ex2.InnerException.GetType().Name + "): " + ex2.InnerException.Message);
                                    sb.AppendLine("InnerException StackTrace: " + ex2.InnerException.StackTrace);
                                }
                            }

                            throw new wrCommException(url, method, content, sb.ToString(), aex.Flatten());
                        }

                        throw;
                    }
                    if (delayRetry > 0)
                        Thread.Sleep(1000 * delayRetry);
                }
            }
        }

        public HtmlDocument PostPage(String url, HttpContent content, String strHeaders)
        {
            Dictionary<String, String> headers = null;
            if (!String.IsNullOrEmpty(strHeaders.Trim()))
            {
                headers = new Dictionary<string, string>();
                String[] aux = strHeaders.Split(new String[] { "||" }, StringSplitOptions.None);
                foreach (string s in aux)
                {
                    String[] kv = s.Split(new String[] { "=>" }, StringSplitOptions.None);
                    headers.Add(kv[0], kv[1].Trim());
                }
            }

            return PostPage(url, content, headers);
        }

        public wrResponse PostPageR(String url, HttpContent content, Dictionary<String, String> requestHeaders = null, bool keepAlive = true)
        {
            if (client == null)
                throw new ArgumentException("Conexão não inicializada!");

            if (url.StartsWith("/") && client.BaseAddress.OriginalString.EndsWith("/"))
                url = url.Substring(1);

            string urlfinal;
            if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                urlfinal = url;
            else
                urlfinal = client.BaseAddress + url;

            int currentRetry = 0;

            for (; ; )
            {
                try
                {

                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(urlfinal),
                        Method = HttpMethod.Post,
                        Content = content,
                    };

                    if (!keepAlive)
                        request.Headers.ConnectionClose = true;

                    // Header padrão - já esta no client
                    //foreach (var defHeader in Client.DefaultRequestHeaders)
                    //    request.Headers.TryAddWithoutValidation(defHeader.Key, defHeader.Value);

                    if (requestHeaders != null)
                        foreach (KeyValuePair<String, String> kv in requestHeaders)
                        {
                            if (request.Headers.Contains(kv.Key))
                                request.Headers.Remove(kv.Key);
                            request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                        }


                    var PostResponce = client.SendAsync(request);// (url, content);
                    var ResponseResult = PostResponce.Result;
                    // PostResponce.Result.EnsureSuccessStatusCode();

                    lastContentHeaders = ResponseResult.Content.Headers;
                    lastResponseHeaders = ResponseResult.Headers;

                    ReadCookies(ResponseResult);

                    if (urlAddress != null)
                        Referer = urlAddress;

                    urlAddress = urlfinal;

                    wrResponse resp = new wrResponse()
                    {
                        StatusCode = ResponseResult.StatusCode,
                        cHeaders = lastContentHeaders,
                        rHeaders = lastResponseHeaders,
                        content = LerConteudo(ResponseResult.Content)
                    };

                    //HtmlDocument html = null;
                    //html = LerConteudo(PostResponce.Result.Content);
                    return resp;
                }
                catch (Exception ex)
                {
                    currentRetry++;
                    logError(ex, "Falha de comunicação... Retry:{0}", currentRetry);
                    if (currentRetry > retryCount || !IsTransient(ex))
                    {
                        // If this isn't a transient error or we shouldn't retry, 
                        // rethrow the exception.
                        throw;
                    }
                    if (delayRetry > 0)
                        Thread.Sleep(1000 * delayRetry);
                }
            }
        }

        public HtmlDocument PostPage(String url, HttpContent content, Dictionary<String, String> requestHeaders = null, bool keepAlive = true)
        {
            if (client == null)
                throw new ArgumentException("Conexão não inicializada!");

            if (url.StartsWith("/") && client.BaseAddress.OriginalString.EndsWith("/"))
                url = url.Substring(1);

            string urlfinal;
            if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                urlfinal = url;
            else
                urlfinal = client.BaseAddress + url;

            int currentRetry = 0;

            for (; ; )
            {
                try
                {

                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(urlfinal),
                        Method = HttpMethod.Post,
                        Content = content,
                    };

                    if (!keepAlive)
                        request.Headers.ConnectionClose = true;

                    // Header padrão - já esta no client
                    //foreach (var defHeader in Client.DefaultRequestHeaders)
                    //    request.Headers.TryAddWithoutValidation(defHeader.Key, defHeader.Value);

                    if (requestHeaders != null)
                    {
                        foreach (KeyValuePair<String, String> kv in requestHeaders)
                        {
                            try
                            {
                                if (request.Headers.Contains(kv.Key))
                                {
                                    request.Headers.Remove(kv.Key);
                                }
                            }
                            catch (Exception ex) { }
                            request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                        }
                    }

                    var PostResponce = client.SendAsync(request);// (url, content);
                    PostResponce.Result.EnsureSuccessStatusCode();

                    lastContentHeaders = PostResponce.Result.Content.Headers;
                    lastResponseHeaders = PostResponce.Result.Headers;

                    ReadCookies(PostResponce.Result);

                    if (urlAddress != null) Referer = urlAddress;

                    urlAddress = urlfinal;

                    HtmlDocument html = null;

                    html = LerConteudo(PostResponce.Result.Content);
                    return html;
                }
                catch (Exception ex)
                {
                    currentRetry++;
                    logError(ex, "Falha de comunicação... Retry:{0}", currentRetry);
                    if (currentRetry > retryCount || !IsTransient(ex))
                    {
                        // If this isn't a transient error or we shouldn't retry, 
                        // rethrow the exception.
                        throw;
                    }
                    if (delayRetry > 0)
                        Thread.Sleep(1000 * delayRetry);
                }
            }
        }

        public String jsTimeStamp()
        {
            var timestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

            return timestamp.ToString();
        }

        public String decodeHtmlString(String htmlString)
        {
            return HttpUtility.HtmlDecode(htmlString);
        }

        public String encodeHtmlString(String valueString)
        {
            return HttpUtility.HtmlEncode(valueString);
        }

        public String decodeUrlString(String urlString)
        {
            return HttpUtility.UrlDecode(urlString);
        }

        public String encodeUrlString(String valueString)
        {
            return HttpUtility.UrlEncode(valueString);
        }

        public String PostAPI(String url, HttpContent content, String strHeaders)
        {
            Dictionary<String, String> headers = null;
            if (!String.IsNullOrEmpty(strHeaders.Trim()))
            {
                headers = new Dictionary<string, string>();
                String[] aux = strHeaders.Split(new String[] { "||" }, StringSplitOptions.None);
                foreach (string s in aux)
                {
                    String[] kv = s.Split(new String[] { "=>" }, StringSplitOptions.None);
                    headers.Add(kv[0], kv[1].Trim());
                }
            }

            return PostAPI(url, content, headers);
        }

        public String PostAPI(String url, HttpContent content, Dictionary<String, String> requestHeaders = null, HttpMethod mType = null)
        {
            if (client == null)
                throw new ArgumentException("Conexão não inicializada!");

            if (url.StartsWith("/") && client.BaseAddress.OriginalString.EndsWith("/"))
                url = url.Substring(1);

            string urlfinal;
            if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                urlfinal = url;
            else
                urlfinal = client.BaseAddress + url;

            if (mType == null)
                mType = HttpMethod.Post;

            int currentRetry = 0;

            for (; ; )
            {
                try
                {
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(urlfinal),
                        Method = mType,
                        Content = content,
                    };

                    // Header padrão - já esta no client
                    //foreach (var defHeader in Client.DefaultRequestHeaders)
                    //    request.Headers.TryAddWithoutValidation(defHeader.Key, defHeader.Value);

                    if (requestHeaders != null)
                        foreach (KeyValuePair<String, String> kv in requestHeaders)
                        {
                            if (request.Headers.Contains(kv.Key))
                                request.Headers.Remove(kv.Key);
                            request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                        }


                    var PostResponce = client.SendAsync(request);// (url, content);
                    PostResponce.Result.EnsureSuccessStatusCode();

                    lastContentHeaders = PostResponce.Result.Content.Headers;
                    lastResponseHeaders = PostResponce.Result.Headers;

                    ReadCookies(PostResponce.Result);

                    if (urlAddress != null)
                        Referer = urlAddress;

                    urlAddress = urlfinal;

                    String html = PostResponce.Result.Content.ReadAsStringAsync(charCode).Result;
                    return html;

                }
                catch (Exception ex)
                {
                    currentRetry++;
                    logError(ex, "Falha de comunicação... Retry:{0}", currentRetry);
                    if (currentRetry > retryCount || !IsTransient(ex))
                    {
                        // If this isn't a transient error or we shouldn't retry, 
                        // rethrow the exception.
                        throw;
                    }
                    if (delayRetry > 0)
                        Thread.Sleep(1000 * delayRetry);
                }
            }
        }

        //public void CreateHttpClientPool(int maxConnections, int? timeout = null)
        //{
        //    if (timeout == null)
        //        timeout = TimeOut;
        //    if (connections==null)
        //        throw new ArgumentException("Conexão não inicializada!");


        //    if (connections.Count() < maxConnections)
        //    {
        //        int max = maxConnections - connections.Count();
        //        for (int i = 0; i < maxConnections; i++)
        //        {
        //            HttpClient hc = new HttpClient(httpHandler) { BaseAddress = EnderecoBase, Timeout = TimeSpan.FromSeconds(timeout.Value) };
        //            connections.Enqueue(hc);
        //        }
        //    }
        //}

        /// <summary>
        /// Obtem novas conexões para o mesmo endereço base, utilizar este metodo para casos de multiplas conexoes simultaneas
        /// </summary>
        /// <param name="timeout">Tempo maximo de espera em segundos, se não informado utlizará o padrão</param>
        /// <returns>Retorna uma conexão httpClient, Atenção libera-la ou reutiliza-la</returns>
        //public HttpClient ObterConexao()
        //{
        //    if (connections.Count > 0)
        //    {
        //        HttpClient hc = null;
        //        if (connections.TryDequeue(out hc))
        //            return hc;
        //    }
        //    return null;
        //}

        //public void LiberaConexao(HttpClient hc)
        //{
        //    if (connections != null)
        //        connections.Enqueue(hc);
        //}

        public HtmlDocument LerConteudo(HttpContent conteudo)
        {
            String htmContent = conteudo.ReadAsStringAsync(charCode).Result;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmContent);

            return doc;
        }

        public void Dispose()
        {
            //if (connections != null)
            //{
            //    HttpClient hc = null;
            //    while (connections.TryDequeue(out hc))
            //    {
            //        hc.Dispose();
            //        hc = null;
            //    }
            //}
            if (client != null)
                client.Dispose();
            if (httpHandler != null)
                httpHandler.Dispose();
        }
    }

    public class MyPolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(
              ServicePoint srvPoint
            , X509Certificate certificate
            , WebRequest request
            , int certificateProblem)
        {

            //Return True to force the certificate to be accepted.
            return true;

        } // end CheckValidationResult
    } // class MyPolicy

    public class wrResponse
    {
        public HttpStatusCode StatusCode;
        public HttpContentHeaders cHeaders;
        public HttpHeaders rHeaders;
        public HtmlDocument content;
        public String urlAction;
    }

    // extensions


}
