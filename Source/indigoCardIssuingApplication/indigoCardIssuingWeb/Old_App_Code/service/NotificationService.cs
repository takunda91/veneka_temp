using indigoCardIssuingWeb.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Web;
using System.Security.Principal;

namespace indigoCardIssuingWeb.Old_App_Code.service
{
    public class NotificationService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NotificationService));
        internal List<notification_batch_ListResult> ListNotificationBatches(int issuerId, int rOWS_PER_PAGE, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ListNotificationBatches(issuerId, pageIndex, rOWS_PER_PAGE, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal bool UpdateNotificationforBatch( NotificationMessages notifications, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateNotificationforBatch( notifications, encryptedSessionKey);

            if (base.CheckResponse(response, log, out responsemessage))
            {
                return true;
            }
            else
            {
                responsemessage = response.ResponseMessage;
                //throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
                return false;
            }
        }

        internal bool InsertNotificationforBatch( NotificationMessages notifications, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.InsertNotificationforBatch(notifications, encryptedSessionKey);

            if (base.CheckResponse(response, log, out responsemessage))
            {
                return true;
            }
            else
            {
                responsemessage = response.ResponseMessage;
                //throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
                return false;
            }
        }

        internal bool DeleteNotificationBatch(NotificationMessages notifications, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DeleteNotificationBatch(notifications, encryptedSessionKey);

            if (base.CheckResponse(response, log, out responsemessage))
            {
                return true;
            }
            else
            {
                responsemessage = response.ResponseMessage;
                //throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
                return false;
            }
        }

        internal List<notification_batchResult> GetNotificationBatch(NotificationMessages notifications)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetNotificationBatch(notifications, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }


        internal List<notification_branch_ListResult> ListNotificationBranches(int issuerId, int rOWS_PER_PAGE, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ListNotificationBraches(issuerId, pageIndex, rOWS_PER_PAGE, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal bool UpdateNotificationforBranch( NotificationMessages notifications, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateNotificationforBranch(notifications, encryptedSessionKey);

            if (base.CheckResponse(response, log, out responsemessage))
            {
                return true;
            }
            else
            {
                responsemessage = response.ResponseMessage;
                //throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
                return false;
            }
        }

        internal bool InsertNotificationforBranch( NotificationMessages notifications, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.InsertNotificationforBranch( notifications, encryptedSessionKey);

            if (base.CheckResponse(response, log, out responsemessage))
            {
                return true;
            }
            else
            {
                responsemessage = response.ResponseMessage;
                //throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
                return false;
            }
        }

        internal bool DeleteNotificationBranch(NotificationMessages notifications, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DeleteNotificationBranch(notifications, encryptedSessionKey);

            if (base.CheckResponse(response, log, out responsemessage))
            {
                return true;
            }
            else
            {
                responsemessage = response.ResponseMessage;
                //throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
                return false;
            }
        }

        internal List<notification_branchResult> GetNotificationBranch(NotificationMessages notifications)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetNotificationBranch(notifications, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
    }
}
