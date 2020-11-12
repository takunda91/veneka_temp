using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.ServicesAuthentication.API;
using Veneka.Indigo.ServicesAuthentication.API.DataContracts;

namespace Veneka.Indigo.BackOffice.Application.Authentication.Clients
{
    public class IndigoServicesAuthClient : IAuthentication
    {
        private readonly IAuthentication _proxy;
        private readonly string urlPath = "AuthenticationAPI.svc";

        public IndigoServicesAuthClient(Uri url)
        {
            Uri uri = null;
            if (Uri.TryCreate(url, urlPath, out uri))
            {
                WSHttpBinding binding = new WSHttpBinding();
                binding.Security.Mode = SecurityMode.Transport;
                EndpointAddress endpoint = new EndpointAddress(uri);

                ChannelFactory<IAuthentication> factory = new ChannelFactory<IAuthentication>(binding, endpoint);
                _proxy = factory.CreateChannel();

                IgnoreUntrustedSSL();
            }
            else
            {
                throw new ArgumentException("URL not in correct format: " + url);
            }
        }
        private void IgnoreUntrustedSSL()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

            ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate,
                                                                        X509Chain chain,
                                                                        SslPolicyErrors sslPolicyErrors) => true;
        }
        public IndigoServicesAuthClient(IAuthentication auth)
        {
            _proxy = auth;
        }


        public AuthenticationResponse Login(string username, string password)
        {
            return _proxy.Login(username, password);
        }

        public AuthenticationResponse MultiFactor(int type, string mfToken, string authToken)
        {
            return _proxy.MultiFactor(type, mfToken, authToken);
        }
    }
}
