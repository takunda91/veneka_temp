using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Veneka.Indigo.Integration.Remote;
using Veneka.Indigo.Remote;

namespace IndigoCardIssuanceService
{

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RemoteComponent" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RemoteComponent.svc or RemoteComponent.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = Constants.IndigoRemoteComponentURL)]
    public class RemoteComponent : IRemoteComponent
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RemoteComponent));
        private readonly RemoteController _remoteController = new RemoteController();

        #region Private Helper Methods
        private bool TryGetClientEndpoint(out IPEndPoint clientEndpoint)
        {
            clientEndpoint = null;

            try
            {
                RemoteEndpointMessageProperty endpoint = OperationContext
                                                            .Current
                                                            .IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

                IPAddress ipAddress;
                if (IPAddress.TryParse(endpoint.Address, out ipAddress))
                {
                    clientEndpoint = new IPEndPoint(ipAddress, endpoint.Port);
                    return true;
                }
                else
                {
                    _log.WarnFormat("Invalid client endpoint {0} : {1}", endpoint.Address, endpoint.Port);
                }
            }
            catch(Exception ex)
            {
                _log.Error(ex);
            }            

            return false;
        }

        private string GetRequest()
        {
            try
            {
                OperationContext context = OperationContext.Current;

                if (context != null && context.RequestContext != null)
                {
                    Message msg = context.RequestContext.RequestMessage;
                    return msg.ToString();
                }
            }
            catch(Exception ex)
            {
                _log.Error(ex);
            }

            return String.Empty;
        }
        #endregion

        public RemoteCardUpdates GetPendingCardUpdates(string token)
        {
            IPEndPoint endpoint;

            if (TryGetClientEndpoint(out endpoint))
            {
                try
                {
                    _remoteController.LogRequest(token, endpoint.ToString(), 0, GetRequest());
                    _log.Trace(t => t("Call to GetPendingCardUpdates from " + endpoint.ToString()));

                    return _remoteController.GetPendingCardUpdates(token, endpoint.ToString());
                }
                catch(Exception ex)
                {
                    _log.Error(ex);
                }
            }

            return new RemoteCardUpdates();
        }        

        public Response CardUpdateResults(string token, RemoteCardUpdatesResponse remoteCardUpdatesResponse)
        {
            IPEndPoint endpoint;

            if (TryGetClientEndpoint(out endpoint))
            {
                try
                {
                    _remoteController.LogRequest(token, endpoint.ToString(), 1, GetRequest());
                    _log.Trace(t => t("Call to CardUpdateResults from " + endpoint.ToString()));

                    return _remoteController.CardUpdateResults(token, remoteCardUpdatesResponse, endpoint.ToString());
                }
                catch(Exception ex)
                {
                    _log.Error(ex);

                }
            }

            return new Response();
        }
    }
}
