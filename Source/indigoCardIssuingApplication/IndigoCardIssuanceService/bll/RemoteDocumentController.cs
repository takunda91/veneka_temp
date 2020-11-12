using Common.Logging;
using IndigoCardIssuanceService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.DocumentManagement;

namespace IndigoCardIssuanceService.bll
{
    public class RemoteDocumentController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RemoteDocumentController));

        protected HttpClient client;

        internal List<CardDocument> GetDocuments(string accountNumber)
        {
            List<CardDocument> documents = new List<CardDocument>();
            string baseUrl = ConfigurationManager.AppSettings["RemoteDocumentLocation"];
            string userName = ConfigurationManager.AppSettings["RemoteUser"];
            string password = ConfigurationManager.AppSettings["RemotePassword"];
            string searchMethod = ConfigurationManager.AppSettings["RemoteSearchMethod"];

            string currentToken = Logon(baseUrl, userName, password);
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), $"{baseUrl}/{searchMethod}?term={accountNumber}&orderBy=name"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}", currentToken))));

                    var response = httpClient.SendAsync(request).Result;

                    var content = response.Content.ReadAsStringAsync();

                    dynamic stuff = JObject.Parse(content.Result);


                    dynamic entries = stuff.list.entries;
                    foreach (var array in entries)
                    {
                        foreach (var dbl in array)
                        {
                            foreach (var itm in dbl)
                            {

                                var created = itm.createdAt;
                                var isFolder = itm.isFolder;
                                var isFile = itm.isFile;
                                var modifiedAt = itm.modifiedAt;
                                var name = itm.name;
                                var id = itm.id;
                                var parentId = itm.parentId;

                                CardDocument newDocument = new CardDocument()
                                {
                                    CardId = 0,
                                    Comment = name,
                                    DocumentTypeId = 0,
                                    Id = 0,
                                    Location = id
                                };
                                documents.Add(newDocument);
                            }
                        }
                    }

                }
                return documents;
            }
        }

        internal byte[] DownloadDocument(string documentKey)
        {
            string baseUrl = ConfigurationManager.AppSettings["RemoteDocumentLocation"];
            string userName = ConfigurationManager.AppSettings["RemoteUser"];
            string password = ConfigurationManager.AppSettings["RemotePassword"];
            string queryMethod = ConfigurationManager.AppSettings["RemoteDownloadMethod"];

            string currentToken = Logon(baseUrl, userName, password);

            List<RemoteDocument> documents = new List<RemoteDocument>();
            byte[] content = null;
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), $"{baseUrl}/{queryMethod}/{documentKey}/content"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/pdf");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}", currentToken))));

                    var response = httpClient.SendAsync(request).Result;

                    content = response.Content.ReadAsByteArrayAsync().Result;
                }
            }
            return content;
        }
        internal string Logon(string baseUrl, string userName, string password)
        {
            string token = string.Empty;
            string loginMethod = ConfigurationManager.AppSettings["RemoteLogonMethod"];

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{baseUrl}/{loginMethod}"))
                {
                    request.Content = new StringContent($"{{\"userId\":\"{userName}\",\"password\":\"{password}\"}}");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = httpClient.SendAsync(request).Result;
                    var result = response.Content.ReadAsStringAsync();

                    dynamic stuff = JObject.Parse(result.Result);

                    token = stuff.entry.id;
                }
            }

            return token;
        }

        internal void ConfigureHttpClient(string baseUrl, string currentToken)
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}", currentToken))));

            //ignore certificate error
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        private static async Task PostBasicAsync(object content, CancellationToken cancellationToken, string url)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                var json = JsonConvert.SerializeObject(content);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;

                    using (var response = await client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                        .ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
        }

        internal async Task<string> ExecuteAPICall(string url, string content)
        {
            HttpClient localClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await localClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            return await System.Threading.Tasks.Task.Run(() => responseContent);
        }

        internal async Task<string> ExecuteAPICall(string callMethod, HttpClient localClient)
        {
            var response = await localClient.GetAsync(callMethod);

            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            return await System.Threading.Tasks.Task.Run(() => responseContent);
        }

        internal async Task<string> ExecuteAPICall(string callMethod)
        {
            var response = await client.GetAsync(callMethod);

            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            return await System.Threading.Tasks.Task.Run(() => responseContent);
        }

        private List<RemoteDocument> ExtractResponse(string result)
        {
            List<RemoteDocument> cobList = new List<RemoteDocument>();
            try
            {
                //first check if we only get a single COB.  Will fail if we receive an array
                List<RemoteDocument> cobItems = JsonConvert.DeserializeObject<List<RemoteDocument>>(result);
                cobList.AddRange(cobItems);
            }
            catch (Exception)
            {
                return new List<RemoteDocument>();
            }
            return cobList;
        }
    }
}