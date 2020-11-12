using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Veneka.Indigo.Integration.TMB.CBS.Utils
{
    class General
    {
        public const string MODULE_LOGGER = "NANO Flexcube Service";

        public static Binding BuildBindings(string protocol, int? timeoutMilliSeconds)
        {
            //Default timeout is 1 minute.
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliSeconds.GetValueOrDefault(20000));

            BasicHttpSecurityMode securityMode = BasicHttpSecurityMode.None;
            if (protocol.ToUpper().Trim().Equals("HTTPS"))
                securityMode = BasicHttpSecurityMode.None;

            BasicHttpBinding binding = new BasicHttpBinding(securityMode);

            binding.Name = "NANOFlexcubeWebServicesImplPortBinding";
            binding.CloseTimeout = new TimeSpan(0, 1, 0);
            binding.OpenTimeout = new TimeSpan(0, 1, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 1, 0);
            binding.SendTimeout = new TimeSpan(0, 10, 0);
            //binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 128000;
            binding.MaxBufferSize = 128000;
            binding.UseDefaultWebProxy = false;
            binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;

            //CustomBinding customBinding = new CustomBinding(binding);
            //var transportElement = customBinding.Elements.Find<HttpTransportBindingElement>();
            //transportElement.KeepAliveEnabled = false;


            return binding;
        }

        public static EndpointAddress BuildEndpointAddress(string protocol, string address, int port, string path)
        {
            //TODO need logic to determin if address and path in correct format.

            UriBuilder uri = new UriBuilder();
            uri.Scheme = protocol.Trim();
            uri.Host = address;

            if (port > 0)
                uri.Port = port;
            else
                uri.Port = -1;
            uri.Path = path;

            return new EndpointAddress(uri.Uri);
        }
    }
}
