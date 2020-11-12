using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Veneka.Indigo.Integration.TMB.CBS.Inspector
{
    class LogClientBehaviour : IEndpointBehavior
    {
        #region Private Fields
        private readonly string _logger;
        private readonly bool _useBasicAuth;
        private readonly string _username;
        private readonly string _password;
        #endregion
        #region Constructors
        public LogClientBehaviour(bool useBasicAuth, string username, string password, string logger)
        {
            _useBasicAuth = useBasicAuth;
            _username = username;
            _password = password;
            _logger = logger;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new MessageInspector(_useBasicAuth, _username, _password, _logger));
        }
        #endregion
    }
}
