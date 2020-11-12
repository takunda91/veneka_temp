using System;
using System.Collections.Generic;
using System.Linq;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.security;
using Common.Logging;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.service;

namespace indigoCardIssuingWeb.service
{
    public class AudiControllService : BaseService
    {
        //private static readonly Service1SoapClient _issuanceService = new Service1SoapClient();
        private static readonly ILog log = LogManager.GetLogger(typeof(AudiControllService));

        public List<GetAuditData_Result> GetAuditResults(AuditSearch AuditSearchParams, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);
            var audits = new List<GetAuditData_Result>();

            var response = m_indigoApp.GetAudit(AuditSearchParams.AuditAction, (int?)AuditSearchParams.Role,
                                                     AuditSearchParams.UserName, AuditSearchParams.DateFrom,
                                                     AuditSearchParams.DateTo, AuditSearchParams.IssuerId,
                                                     AuditSearchParams.PageIndex, AuditSearchParams.RowsPerPage,               
                                                     encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        

        internal byte[] GenereateAuditReport(AuditSearch AuditSearchParams, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GenereateAuditPdfReport(AuditSearchParams.AuditAction, (int?)AuditSearchParams.Role,
                                                       AuditSearchParams.UserName, AuditSearchParams.DateFrom,
                                                       AuditSearchParams.DateTo, AuditSearchParams.IssuerId,
                                                       AuditSearchParams.PageIndex, AuditSearchParams.RowsPerPage,
                                                       encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal List<string[]> GetAuditCSVReport(AuditSearch AuditSearchParams, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetAuditCSVReport(AuditSearchParams.AuditAction, (int?)AuditSearchParams.Role,
                                                       AuditSearchParams.UserName, AuditSearchParams.DateFrom,
                                                       AuditSearchParams.DateTo, AuditSearchParams.IssuerId,
                                                       AuditSearchParams.PageIndex, AuditSearchParams.RowsPerPage,
                                                       encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal bool InsertAudit(int audit_action_id,string description,int issuerID)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.InsertAudit(audit_action_id,description,issuerID,encryptedSessionKey);

           

            return base.CheckResponse(response, log);
        }
    }
}