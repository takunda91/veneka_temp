using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.Old_App_Code.service;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace indigoCardIssuingWeb.service
{
    public class RemoteService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RemoteService));

        public List<LangLookup> LangLookupRemoteUpdateStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupRemoteUpdateStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        public List<RemoteCardUpdateSearchResult> SearchRemoteCardUpdates(RemoteCardUpdateSearchParameters searchParameters, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.SearchRemoteCardUpdates(searchParameters.PAN, searchParameters.RemoteUpdateStatusesId, searchParameters.IssuerId, searchParameters.BranchId,
                                                                searchParameters.ProductId, searchParameters.DateFrom, searchParameters.DateTo,
                                                                pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public RemoteCardUpdateDetailResult GetRemoteCardDetail(long cardId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetRemoteCardDetail(cardId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        public bool ChangeRemoteCardStatus(long cardId, int remoteUpdateStatusesId, string comment, out RemoteCardUpdateDetailResult result, out string responseMsg)
        {
            responseMsg = String.Empty;
            result = null;

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                             SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                             SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ChangeRemoteCardStatus(cardId, remoteUpdateStatusesId, comment, encryptedSessionKey);

            if (base.CheckResponse(response, log))
            {
                result = response.Value;
                return true;
            }

            return false;
        }

        public void ChangeRemoteCardsStatus(List<long> cardIds, int remoteUpdateStatusesId, string comment)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                             SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                             SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ChangeRemoteCardsStatus(cardIds.ToArray(), remoteUpdateStatusesId, comment, encryptedSessionKey);

            base.CheckResponse(response, log);
        }

    }
}