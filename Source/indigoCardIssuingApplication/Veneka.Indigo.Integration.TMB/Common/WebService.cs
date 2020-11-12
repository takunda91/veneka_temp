using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Integration.TMB.Common
{
    #region Static Enumerations
    public enum Protocol { HTTP, HTTPS }
    public enum Authentication { NONE, BASIC }
    #endregion
   public abstract  class WebService
    {
        #region Fields
        protected readonly ILog _log;
        #endregion

        /// <summary>
        /// Default constructor for base class. Allows dervided class to provide module name for logger.
        /// </summary>
        /// <param name="moduleName"></param>
        protected WebService(Protocol protocol, string address, int port, string path, int? timeoutMilliSeconds,
                                Authentication authentication, string username, string password,
                                IDataSource dataSource, string logger)
        {
            //Check values
            if (String.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address", "Cannot be null, empty or whitespace.");
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path", "Cannot be null, empty or whitespace.");
            //if (String.IsNullOrWhiteSpace(dataSource))
            //    throw new ArgumentNullException("connectionString", "Cannot be null, empty or whitespace.");
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
                         //.AppendFormat(" DB Conn:\t{0}{1}", connectionString, Environment.NewLine)
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

            //binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;

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

        protected void AddUntrustedSSL()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Ignore Untrusted SSL:\t", IgnoreUntrustedSSL);

            if (IgnoreUntrustedSSL)
            {
                _log.Debug(d => d("Ignoring untrusted certificate."));
                //ServicePointManager.Expect100Continue = false;
                //ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate,
                                                                            X509Chain chain,
                                                                            SslPolicyErrors sslPolicyErrors) => true;
            }


        }
        #endregion
    }
}
