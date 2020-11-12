using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Notifications.WebServiceConfigs
{
   public  class LogClientBehaviour: IEndpointBehavior
    {
        #region Private Fields
        private readonly string _logger;
        private readonly bool _useBasicAuth;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _isCMS;
        #endregion

        #region Constructors
        public LogClientBehaviour(bool useBasicAuth, string username, string password, bool isCMS, string logger)
        {
            _useBasicAuth = useBasicAuth;
            _username = username;
            _password = password;
            _isCMS = isCMS;
            _logger = logger;
        }
        #endregion

        #region IEndpointBehavior Members
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            //if (_isCMS)
            //    clientRuntime.MessageInspectors.Add(new CmsMessageInspector(_useBasicAuth, _username, _password, _logger));
            //else
            //    clientRuntime.MessageInspectors.Add(new MessageInspector(_useBasicAuth, _username, _password, _logger));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
        #endregion
    }
}
