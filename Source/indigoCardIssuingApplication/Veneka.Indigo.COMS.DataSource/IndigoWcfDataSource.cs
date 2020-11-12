using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Web;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.COMS.DataSource.Callbacks;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.COMS.WCF.DataSource
{
    /// <summary>
    /// Creates the WCF datasource interface between Indigo Application when the Card Management and Operations module is hosted remotely
    /// </summary>
    public class IndigoWcfDataSource :IDataSource
    {
        
        private readonly IComsCallback _proxy;
        private readonly string urlPath = "ComsCallback.svc";

        public IndigoWcfDataSource(Uri url)
        {
            Uri uri = null;
            if (Uri.TryCreate(url, urlPath, out uri))
            {
                WSHttpBinding binding = new WSHttpBinding();
                binding.Security.Mode = SecurityMode.Transport;

                EndpointAddress endpoint = new EndpointAddress(uri);

                ChannelFactory<IComsCallback> factory = new ChannelFactory<IComsCallback>(binding, endpoint);
                _proxy = factory.CreateChannel();

                // Set the Proxy's settings for call back


                IgnoreUntrustedSSL();
            }
            else
            {
                throw new ArgumentException("URL not in correct format: " + url);
            }
            BranchDAL = new WcfBranchDAL(_proxy);
            CardGeneratorDAL = new WcfCardGeneratorDAL(_proxy);
            CardsDAL = new WcfCardsDAL(_proxy);
            ExportBatchDAL = new WcfExportBatchDAL(_proxy);
            IssuerDAL = new WcfIssuerDAL(_proxy);
            LookupDAL = new WcfLookupDAL(_proxy);
            ParametersDAL = new WcfParametersDAL(_proxy);
            ProductDAL = new WcfProductDAL(_proxy);
            TerminalDAL = new WcfTerminalDAL(_proxy);
            TransactionSequenceDAL = new WcfTransactionSequenceDAL(_proxy);
            CustomDataDAL = new WcfCustomDataDAL(_proxy);
        }
        private void IgnoreUntrustedSSL()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

            ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate,
                                                                        X509Chain chain,
                                                                        SslPolicyErrors sslPolicyErrors) => true;
        }
        public IBranchDAL BranchDAL { get; private set; }

        public ICardGeneratorDAL CardGeneratorDAL { get; private set; }

        public ICardsDAL CardsDAL { get; private set; }

        public IExportBatchDAL ExportBatchDAL { get; private set; }

        public IIssuerDAL IssuerDAL { get; private set; }

        public ILookupDAL LookupDAL { get; private set; }

        public IParametersDAL ParametersDAL { get; private set; }

        public IProductDAL ProductDAL { get; private set; }

        public ITerminalDAL TerminalDAL { get; private set; }

        public ITransactionSequenceDAL TransactionSequenceDAL { get; private set; }

        public ICustomDataDAL CustomDataDAL { get; private set; }
    }
}