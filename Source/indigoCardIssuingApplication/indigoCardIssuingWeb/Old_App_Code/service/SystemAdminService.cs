using System;
using System.Collections.Generic;
using System.Linq;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.security;
using System.Web;
using System.Security.Principal;
using Common.Logging;
using indigoCardIssuingWeb.App_Code.SearchParameters;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.service;

namespace indigoCardIssuingWeb.service
{
    public class SystemAdminService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SystemAdminService));
        //private readonly Service1SoapClient _issuanceService = new Service1SoapClient();

        #region ISSUER SERVICES 

        public List<LangLookup> LangLookupBranchStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupBranchStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        public List<LangLookup> LangLookupBranchTypes()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupBranchTypes(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        

        public List<issuers_Result> GetIssuers(int? pageIndex, int? Rowsperpage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetIssuers(pageIndex, Rowsperpage,encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;

        }

        //public List<Branch> GetBranches(int issuerID)
        //{
        //    return GetBranches(issuerID, null, null);
        //}


        /// <summary>
        /// Return a branch by its Id.
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public branch GetBranchById(int branchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);
            
            ResponseOfbranch response = m_indigoApp.GetBranchById(branchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        /// <summary>
        /// Persist new branch to the database
        /// </summary>
        /// <param name="createBranch"></param>
        /// <returns></returns>
        public bool CreateBranch(branch createBranch, List<int> satellite_branches, out int branchId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateBranch(createBranch, satellite_branches.ToArray(), encryptedSessionKey);
            branchId = response.Value;

            return base.CheckResponseException(response, log, out responseMessage);
        }

        /// <summary>
        /// Persist changes to a branch to the database
        /// </summary>
        /// <param name="updateBranch"></param>
        /// <returns></returns>
        public bool UpdateBranch(branch updateBranch, List<int> satellite_branches,out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                           SecurityParameters.EXTERNAL_SECURITY_KEY);

            BaseResponse response = m_indigoApp.UpdateBranch(updateBranch, satellite_branches.ToArray(), encryptedSessionKey);

            return base.CheckResponseException(response, log, out responseMessage);
        }

        public List<FileLoadResult> GetFileLoadList(FileLoadSearchParameters filedetails)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetFileLoadList(filedetails.DateFrom, filedetails.DateTo, filedetails.PageIndex,
                                                              filedetails.RowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<GetFileLoderLog_Result> GetLoadFileLog(FileDetailsSearch filedetails)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.SearchFileLoadLog(filedetails.FileLoadId, filedetails.FileLoaderStatus, filedetails.FileName, filedetails.IssuerId,
                                                              filedetails.DateFrom, filedetails.DateTo, filedetails.PageIndex,
                                                              filedetails.RowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        #endregion

        #region Licensing Services

        internal string GetMachineId()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetMachineId(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }
        public List<BillingReportResult> GetBillingReport(int? issuerId, string month, string year)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetBillingReport(issuerId, month, year, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        internal bool LoadIssuerLicenseFile(byte[] licenseFile, out IndigoComponentLicense license, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LoadIssuerLicense(licenseFile, encryptedSessionKey);
            license = response.Value;

            return base.CheckResponseException(response, log, out messages);
        }

        internal List<IndigoComponentLicense> GetLicenseListIssuers(bool? licensedIssuers)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetLicenseListIssuers(licensedIssuers, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        #endregion
    }
}