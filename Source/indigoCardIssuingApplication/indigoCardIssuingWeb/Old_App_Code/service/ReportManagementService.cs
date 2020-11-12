using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using Microsoft.Reporting.WebForms;
using System.Collections;

namespace indigoCardIssuingWeb.Old_App_Code.service
{
    public class ReportManagementService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportManagementService));
        //private static readonly Service1SoapClient _issuanceService = new Service1SoapClient();
      

        internal List<BranchOrderReportResult> GetBranchOrderReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetBranchOrderReport(issuerId, branchId, fromDate, toDate, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<CardProductionReportResult> GetCardProductionReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCardProductionReport(issuerId, branchId, fromDate, toDate, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<CardDispatchReportResult> GetCardDispatchReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCardDispatchReport(issuerId, branchId, fromDate, toDate, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<PinMailerReportResult> GetPinMailerReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPinMailerReport(issuerId, branchId, fromDate, toDate, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<PinMailerReprintReportResult> GetPinMailerReprintReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPinMailerReprintReport(issuerId, branchId, fromDate, toDate, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<CardExpiryReportResult> GetCardExpiryReport(int? issuerId, int? branchId, DateTime fromDate)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCardExpiryReport(issuerId, branchId, fromDate, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
    }

    public static class ReportParameters
    {
        private static ReportParameter CreateReportParameter(string paramName, string pramValue)
        {
            ReportParameter aParam = new ReportParameter(paramName, pramValue);
            return aParam;
        }

        public static ReportParameter[] ReportDefaultPatam(List<Tuple<string, string>> dic)
        {
            ArrayList arrLstDefaultParam = new ArrayList();
            foreach (var tuple in dic)
            {
                arrLstDefaultParam.Add(CreateReportParameter(tuple.Item1, tuple.Item2));
            }
           

            ReportParameter[] param = new ReportParameter[arrLstDefaultParam.Count];
            for (int k = 0; k < arrLstDefaultParam.Count; k++)
            {
                param[k] = (ReportParameter)arrLstDefaultParam[k];
            }

            return param;
        }
    }
}