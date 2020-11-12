using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.API;
using Veneka.Indigo.BackOffice.Application.Authentication;
using Veneka.Indigo.ServicesAuthentication.API.DataContracts;

namespace Veneka.Indigo.BackOffice.Application.BackOfficeApiClient
{
    public class BackOfficeClient:IBackOfficeAPI
    {
        private IBackOfficeAPI _proxy;
        private Token apiToken;
        private readonly string urlPath = "BackOfficeAPI.svc";

        public BackOfficeClient(Uri url)
        {
            Uri uri = null;
            if (Uri.TryCreate(url, urlPath, out uri))
            {
                WSHttpBinding binding = new WSHttpBinding();
                binding.Security.Mode = SecurityMode.Transport;
                EndpointAddress endpoint = new EndpointAddress(uri);

                ChannelFactory<IBackOfficeAPI> factory = new ChannelFactory<IBackOfficeAPI>(binding, endpoint);
                _proxy = factory.CreateChannel();

                IgnoreUntrustedSSL();
            }
            else
            {
                throw new ArgumentException("URL not in correct format: " + url);
            }
            // when instance is created get token for the principal
            BackOfficeAppPrincipal _backofficeapi = (BackOfficeAppPrincipal)Thread.CurrentPrincipal;
            apiToken= new Token();
            apiToken.Session= _backofficeapi.Identity.Token;
            
        }
        private void IgnoreUntrustedSSL()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

            ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate,
                                                                        X509Chain chain,
                                                                        SslPolicyErrors sslPolicyErrors) => true;
        }
        public BackOfficeClient(IBackOfficeAPI backOfficeApi)
        {
            if(backOfficeApi is BackOfficeClient)
            {
                throw new ArgumentException("BackOfficeClient class not a valid parameter", nameof(backOfficeApi));
            }

            _proxy = backOfficeApi;
        }

        public Response<string> CheckStatus(string guid)
        {
         var response= _proxy.CheckStatus(guid);

            return response;
        }

        public Response<List<GetPrintBatchDetails>> GetApprovedPrintBatches(string apiToken)
        {
            var response = _proxy.GetApprovedPrintBatches(apiToken);
            if (response.Success)
                apiToken = response.Session;
            return response;
        }

        public Response<List<ProductTemplate>> GetProductTemplate(int productId,string apiToken)
        {
            var response = _proxy.GetProductTemplate(productId, apiToken);
            if (response.Success)
                apiToken = response.Session;
            return response;
           
        }

        public Response<List<RequestDetails>> GetRequestsforBatch(long printBatchId, int startIndex, int size,string apiToken)
        {
            var response = _proxy.GetRequestsforBatch(printBatchId, startIndex, size, apiToken);
            if (response.Success)
                apiToken = response.Session;
            return response;
           
        }

        public Response<bool> updatePrintBatchStatus(UpdatePrintBatchDetails printBatch,string apiToken)
        {
            var response = _proxy.updatePrintBatchStatus(printBatch, apiToken);
            if (response.Success)
                apiToken = response.Session;
            return response;
          
        }


        public Response<string[]> GetWorkStationKey(string workStation,int size,string apiToken)
        {
            var response = _proxy.GetWorkStationKey(workStation, size, apiToken); 
            if (response.Success)
                apiToken = response.Session;
            return response;
        }

       
    }
}
