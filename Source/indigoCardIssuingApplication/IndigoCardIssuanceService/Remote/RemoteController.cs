using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Integration.Remote;
using Veneka.Indigo.RemoteManagement;
using Veneka.Indigo.RemoteManagement.DAL;

namespace Veneka.Indigo.Remote
{
    public class RemoteController
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RemoteController));
        private readonly RemoteTokenService _remoteTokenService;
        private readonly RemoteCardUpdateService _remoteCardUpdateService;
        private readonly RemoteLoggingService _remoteLoggingService;

        public RemoteController()
        {
            _remoteTokenService = new RemoteTokenService();
            _remoteCardUpdateService = new RemoteCardUpdateService();
            _remoteLoggingService = new RemoteLoggingService();
        }

        public RemoteController(IRemoteTokenDAL remoteTokenDAL, IRemoteCardUpdateDAL remoteCardUpdateDAL, IRemoteLoggingDAL remoteLoggingDAL)
        {
            _remoteTokenService = new RemoteTokenService(remoteTokenDAL);
            _remoteCardUpdateService = new RemoteCardUpdateService(remoteCardUpdateDAL);
            _remoteLoggingService = new RemoteLoggingService(remoteLoggingDAL);
        }

        public void LogRequest(string token, string clientIP, int methodCalId, string request)
        {
            _remoteLoggingService.LogRequest(token, clientIP, methodCalId, request);
        }

        public RemoteCardUpdates GetPendingCardUpdates(string token, string clientIP)
        {
            try
            {
                //TODO: Remote component logging


                Guid remoteToken;
                if(!Guid.TryParse(token, out remoteToken))
                {
                    _log.ErrorFormat("Could not parse remote token {0}", token);
                    return new RemoteCardUpdates(RemoteErrorGenerator.ResponseError(RemoteErrorGenerator.Error.TokenParseError));
                }

                //Validate the token
                string tokenResponseMessage = String.Empty;
                var issuer = _remoteTokenService.GetRemoteToken(remoteToken, 0,  -2, clientIP, out tokenResponseMessage);

                if(issuer == null)
                {
                    _log.Warn(tokenResponseMessage);
                    return new RemoteCardUpdates(RemoteErrorGenerator.ResponseError(RemoteErrorGenerator.Error.InvalidTokenError));
                }

                //Fetch pending uploads
                return _remoteCardUpdateService.GetPendingCardUpdates(issuer.issuer_id, clientIP, -2, clientIP);
            }
            catch(Exception ex)
            {
                _log.Error(ex);
            }

            //return data
            return new RemoteCardUpdates(RemoteErrorGenerator.ResponseError(RemoteErrorGenerator.Error.FailedToProcess));
        }

        public Response CardUpdateResults(string token, RemoteCardUpdatesResponse remoteCardUpdatesResponse, string clientIP)
        {
            Guid remoteToken;
            if (!Guid.TryParse(token, out remoteToken))
            {
                _log.ErrorFormat("Could not parse remote token {0}", token);
                return RemoteErrorGenerator.ResponseError(RemoteErrorGenerator.Error.TokenParseError);
            }

            //Validate the token
            string tokenResponseMessage = String.Empty;
            var issuer = _remoteTokenService.GetRemoteToken(remoteToken, 0, -2, clientIP, out tokenResponseMessage);

            if (issuer == null)
            {
                _log.Warn(tokenResponseMessage);
                return  RemoteErrorGenerator.ResponseError(RemoteErrorGenerator.Error.InvalidTokenError);
            }

            _remoteCardUpdateService.SetCardUpdates(clientIP, remoteCardUpdatesResponse.CardsResponse, -2, clientIP);

            return new Response(0, "Success");
        }
    }
}