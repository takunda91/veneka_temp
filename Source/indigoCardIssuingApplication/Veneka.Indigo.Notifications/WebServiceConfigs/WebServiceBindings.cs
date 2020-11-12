using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;


namespace Veneka.Indigo.Notifications.WebServiceConfigs
{
    public abstract class WebServiceBindings
    {
        public enum Protocol { HTTP, HTTPS }
        public enum Authentication { NONE, BASIC }
        protected readonly ILog _log;

        protected WebServiceBindings(Protocol protocol, string address, int port, string path, int? timeoutMilliSeconds,
                                Authentication authentication, string username, string password,
                                 string logger)
        {
            //Check values
            if (String.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address", "Cannot be null, empty or whitespace.");
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path", "Cannot be null, empty or whitespace.");         
            if (String.IsNullOrWhiteSpace(logger))
                throw new ArgumentNullException("logger", "Cannot be null, empty or whitespace.");

            _log = LogManager.GetLogger(logger);

            if (_log.IsDebugEnabled)
            {
                StringBuilder debugInfo = new StringBuilder();
                debugInfo.AppendFormat("{0}--------------------WebService Settings--------------------{0}", Environment.NewLine)
                         .AppendFormat(" Protocol:\t{0}{1}", protocol, Environment.NewLine)
                         .AppendFormat(" Address:\t{0}{1}", address, Environment.NewLine)
                         .AppendFormat(" Port:\t\t{0}{1}", port, Environment.NewLine)
                         .AppendFormat(" Path:\t\t{0}{1}", path, Environment.NewLine)
                         .AppendFormat(" Timeout:\t{0}{1}", timeoutMilliSeconds, Environment.NewLine)
                         .AppendFormat(" Auth:\t\t{0}{1}", authentication, Environment.NewLine)
                         .AppendFormat(" Username:\t{0}{1}", username, Environment.NewLine)
                         .AppendFormat(" Logger:\t{0}{1}", logger, Environment.NewLine)
                         .Append("-----------------------------------------------------------");

                _log.Debug(debugInfo.ToString());
            }
        }

        #region Properties
        public bool IgnoreUntrustedSSL { get; set; }
        #endregion

        #region Protected Methods
        protected BasicHttpBinding BuildBindings(string bindingName, Protocol protocol, int? timeoutMilliSeconds)
        {
            //Default timeout is 1 minute.
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliSeconds.GetValueOrDefault(60000));

            BasicHttpSecurityMode securityMode = BasicHttpSecurityMode.None;
            if (protocol == Protocol.HTTPS)
                securityMode = BasicHttpSecurityMode.Transport;

            BasicHttpBinding binding = new BasicHttpBinding(securityMode);
            binding.Name = bindingName;
            binding.CloseTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            binding.SendTimeout = timeout;

            binding.MaxBufferSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;

            binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;

            return binding;
        }
        protected EndpointAddress BuildEndpointAddress(Protocol protocol, string address, int port, string path)
        {
            UriBuilder uri = new UriBuilder();
            uri.Scheme = protocol.ToString();
            uri.Host = address;
            uri.Port = port;
            uri.Path = path;

            return new EndpointAddress(uri.Uri);
        }
        #endregion

    }
}
