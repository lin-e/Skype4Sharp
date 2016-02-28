using System.Net;

namespace Skype4Sharp.Helpers
{
    public class WebRequestFactory
    {
        public WebProxy mainProxy;
        public CookieContainer mainContainer;
        public string userAgent;
        public WebRequestFactory(WebProxy proxyToUse, CookieContainer containerToUse, string userAgentToUse = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36")
        {
            mainProxy = proxyToUse;
            mainContainer = containerToUse;
            userAgent = userAgentToUse;
        }
        public HttpWebRequest createWebRequest_GET(string targetURL, string[][] requestHeaders)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(targetURL);
            webRequest.Proxy = mainProxy;
            webRequest.UserAgent = userAgent;
            webRequest.CookieContainer = mainContainer;
            webRequest.Method = "GET";
            foreach (string[] headerPair in requestHeaders)
            {
                webRequest.Headers.Add(headerPair[0], headerPair[1]);
            }
            return webRequest;
        }
        public HttpWebRequest createWebRequest_PUT(string targetURL, string[][] requestHeaders, byte[] postData, string contentType)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(targetURL);
            webRequest.Proxy = mainProxy;
            webRequest.UserAgent = userAgent;
            webRequest.CookieContainer = mainContainer;
            webRequest.Method = "PUT";
            webRequest.ContentType = contentType;
            foreach (string[] headerPair in requestHeaders)
            {
                webRequest.Headers.Add(headerPair[0], headerPair[1]);
            }
            webRequest.ContentLength = postData.Length;
            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(postData, 0, postData.Length);
            }
            return webRequest;
        }
        public HttpWebRequest createWebRequest_POST(string targetURL, string[][] requestHeaders, byte[] postData, string contentType)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(targetURL);
            webRequest.Proxy = mainProxy;
            webRequest.UserAgent = userAgent;
            webRequest.CookieContainer = mainContainer;
            webRequest.Method = "POST";
            webRequest.ContentType = contentType;
            foreach (string[] headerPair in requestHeaders)
            {
                webRequest.Headers.Add(headerPair[0], headerPair[1]);
            }
            webRequest.ContentLength = postData.Length;
            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(postData, 0, postData.Length);
            }
            return webRequest;
        }
        public HttpWebRequest createWebRequest_DELETE(string targetURL, string[][] requestHeaders)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(targetURL);
            webRequest.Proxy = mainProxy;
            webRequest.UserAgent = userAgent;
            webRequest.CookieContainer = mainContainer;
            webRequest.Method = "DELETE";
            foreach (string[] headerPair in requestHeaders)
            {
                webRequest.Headers.Add(headerPair[0], headerPair[1]);
            }
            return webRequest;
        }
    }
}