using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Common.Logging;


namespace Veneka.Indigo.Integration.TMB.TagSMS
{
    public class TagSMSRESTAPIService
    {
        private static readonly ILog _Log = LogManager.GetLogger("TagSMS Service");

        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }


        public string UserName { get; set; }
        public string Password { get; set; }

        public bool BasicAuth { get; set; }
        public TagSMSRESTAPIService()
        {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "application/x-www-form-urlencoded";//"text/xml";
        }
        public TagSMSRESTAPIService(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "application/x-www-form-urlencoded";//"text/xml";
        }
        public TagSMSRESTAPIService(string endpoint, HttpVerb method)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/x-www-form-urlencoded";//"text/xml";

        }

        public TagSMSRESTAPIService(string endpoint, HttpVerb method, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/x-www-form-urlencoded";//"text/xml";

        }


        public string MakeRequest(HttpVerb method, string ContentType, string data)
        {


            var request = (HttpWebRequest)WebRequest.Create(EndPoint);
            ServicePointManager.ServerCertificateValidationCallback = delegate (
            Object obj, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
            {
                return (true);
            };
            if (BasicAuth == true)
            {
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(UserName + ":" + Password));
                request.Headers.Add("Authorization", "Basic " + encoded);
            }
            request.Method = method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;
            request.Timeout = 3000;//timeout between sending and getting response
            request.ReadWriteTimeout = 3000;//how long it should take to write req/read response to/from stream

            if (!string.IsNullOrEmpty(data) && method == HttpVerb.POST)
            {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(data);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                            _Log.Debug("Reponse :" + responseValue);
                        }
                }
                return responseValue;
            }
        }
    }
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}
