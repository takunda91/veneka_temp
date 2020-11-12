using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.RemoteManagement.DAL;

namespace Veneka.Indigo.RemoteManagement
{
    public class RemoteTokenService
    {
        private readonly IRemoteTokenDAL _remoteTokenDAL;

        public RemoteTokenService() : this(new RemoteTokenDAL())
        { }

        public RemoteTokenService(IRemoteTokenDAL remoteTokenDAL)
        {
            _remoteTokenDAL = remoteTokenDAL;
        }

        public RemoteTokenIssuerResult GetRemoteToken(Guid remoteToken, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            responseMessage = String.Empty;

            var issuer = _remoteTokenDAL.GetRemoteToken(remoteToken, auditUserId, auditWorkstation);

            if (issuer == null)
            {
                responseMessage = String.Format("Token {0} not present in system.", remoteToken.ToString());
                return null;
            }
            else if(issuer.issuer_status_id != 0)
            {
                responseMessage = String.Format("Issuer {0} not active for token {1}", issuer.issuer_code, remoteToken.ToString());
                return null;
            }
            else if (issuer.remote_token_expiry == null)
            {
                responseMessage = String.Format("Token {0} for issuer {1} not active.", remoteToken.ToString(), issuer.issuer_code);
                return null;
            }
            else if (DateTime.Now > issuer.remote_token_expiry.Value)
            {
                responseMessage = String.Format("Token {0} for issuer {1} has expired, please generate new token.", remoteToken.ToString(), issuer.issuer_code);
                return null;
            }

            return issuer;
        }
    }
}
