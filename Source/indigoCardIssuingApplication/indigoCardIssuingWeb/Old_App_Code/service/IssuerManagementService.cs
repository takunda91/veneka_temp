using System;
using System.Linq;
using System.Collections.Generic;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.security;
using Common.Logging;
using System.Web;
using System.Security.Principal;
using indigoCardIssuingWeb.Old_App_Code.service;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;

namespace indigoCardIssuingWeb.service
{
    public class IssuerManagementService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IssuerManagementService));
        //private static readonly Service1SoapClient _issuanceService = new Service1SoapClient();

        private static volatile IssuerManagementService instance;
        private static object syncRoot = new Object();

        public static IssuerManagementService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new IssuerManagementService();
                    }
                }

                return instance;
            }
        }
        #region Lookups

        private static CurrencyResult[] currencyResults = new CurrencyResult[0];

        public List<LangLookup> LangLookupIssuerStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupIssuerStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<LangLookup> LangLookupConnectionParameterType()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupConnectionParameterType(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<LangLookup> LangLookupFileEncryptionTypes()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupFileEncryptionTypes(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        public bool UploadBatchtoCMS(long printbatchId, string notes,out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.UploadBatchtoCMS(printbatchId,notes, encryptedSessionKey);

            if (base.CheckResponseException(response, log, out responseMessage))
            {

                return true;
            }

            return false;
        }
        public List<CurrencyResult> GetCurrencyList()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            if (currencyResults == null || currencyResults.Length == 0)
            {
                var response = m_indigoApp.GetCurrencyList(encryptedSessionKey);

                base.CheckResponse(response, log);

                currencyResults = response.Value;
            }

            return currencyResults.ToList();
        }

        /// <summary>
        /// Fetch all countires
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<country> GetCountries()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCountries(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        #endregion

        #region Issuer

        /// <summary>
        /// Persist new issuer to the DB.
        /// </summary>
        /// <param name="issuer"></param>
        /// <returns></returns>
        internal bool CreateIssuer(IssuerResult issuer, string pin_notification_message, out IssuerResult issuerResult, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateIssuer(issuer, pin_notification_message, encryptedSessionKey);
            issuerResult = response.Value;

            return base.CheckResponseException(response, log, out messages);
        }

        /// <summary>
        /// Persist changes to the DB.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal bool UpdateIssuer(IssuerResult issuer, string pin_notification_message, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateIssuer(issuer, pin_notification_message, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }

        /// <summary>
        /// Lists all the available integration interfaces found in the Indigo integration folder.
        /// </summary>
        /// <returns></returns>
        internal List<IntegrationInterface> ListAvailiableIntegrationInterfaces()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ListAvailiableIntegrationInterfaces(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        internal List<IntegrationInterface> ListAvailiableIntegrationInterfacesByInterfaceTypeId(int interfacetypeid)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ListAvailiableIntegrationInterfacesByInterfaceTypeId(interfacetypeid,encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        ///// <summary>
        ///// Gets all the interfaces' details for an issuer based on the issuer_id
        ///// </summary>
        ///// <param name="issuer_id"></param>
        ///// <returns></returns>
        //public IEnumerable<InterfaceWrapper> GetIssuerInterfaces(int issuer_id)
        //{
        //    string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
        //                                                               SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
        //                                                               SecurityParameters.EXTERNAL_SECURITY_KEY);

        //    List<InterfaceWrapper> interfaces = new List<InterfaceWrapper>();
        //    try
        //    {

        //        var response = _issuanceService.GetIssuerInterfaces(encryptedSessionKey, issuer_id);

        //        if (response.ResponseType != ResponseType.SUCCESSFUL)
        //        {
        //            throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
        //        }// end if (response.ResponseType != ResponseType.SUCCESSFUL)

        //        foreach (var issuerInterface in response.Value)
        //        {
        //            interfaces.Add(issuerInterface);
        //        }// end foreach (var issuerInterface in response.Value)

        //    }// end try
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //    }// end catch (Exception ex)

        //    return interfaces;
        //}// end method IEnumerable<InterfaceWrapper> GetIssuerInterfaces(int issuer_id)

        /// <summary>
        /// Fetch issuer based on issuer id.
        /// </summary>
        /// <param name="connectionParam"></param>
        /// <returns></returns>
        internal IssuerResult GetIssuer(int issuerId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetIssuer(issuerId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal IssuerResult GetIssuerPinMessage(int issuerId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetIssuerPinMessage(issuerId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }


        /// <summary>
        /// Lists all the available integration interfaces found in the Indigo integration folder.
        /// </summary>
        /// <returns></returns>
        internal ProductField[] GetPrintFieldsByProductid(int productId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPrintFieldsByProductid(productId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal List<PrintBatchResult> GetPrintBatchesForUser(PrintBatchSearchParameters searchParams, int pageIndex)
        {
            return this.GetPrintBatchesForUser(searchParams.IssuerId, searchParams.ProductId, searchParams.BatchReference, searchParams.PrintBatchStatusId, searchParams.BranchId,
                                        searchParams.CardIssueMethodId, searchParams.DateFrom, searchParams.DateTo, StaticDataContainer.ROWS_PER_PAGE,
                                        pageIndex);
        }
        internal List<PrintBatchResult> GetPrintBatchesForUser(int? issuerId,int? productId, string pinBatchReference, int? pinBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                               DateTime? startDate, DateTime? endDate, int rowsPerPage, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPrintBatchesForUser(issuerId,productId, pinBatchReference, pinBatchStatusId, branchId, cardIssueMethodId,
                                                                     startDate, endDate, rowsPerPage, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        internal bool SpoilPrintBatch(long print_batch_id, int? new_print_batch_statuses_id, string notes)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.SpoilPrintBatch(print_batch_id, new_print_batch_statuses_id, notes, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return false;
        }
        internal byte[] GeneratePrintBatchReport(long printBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                    SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                    SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GeneratePrintBatchReport(printBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;

            //if (response.ResponseType != ResponseType.SUCCESSFUL)
            //{
            //    throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
            //}

            //return response.Value;
        }

        internal PrintBatchResult GetPrintBatch(long pinBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPrintBatch(pinBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }
        internal bool UpdatePrintBatchStatus(long printBatchId, int printBatchStatusId, int newprintBatchStatusesId, string statusNote, bool autogeneratedistbatch, out PrintBatchResult pinBatchResult, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdatePrintBatchChangeStatus(printBatchId, printBatchStatusId, newprintBatchStatusesId, statusNote, autogeneratedistbatch, encryptedSessionKey);
            pinBatchResult = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

       
       

        #endregion

        #region LDAP Settings

        /// <summary>
        /// Fetch all ldap settings
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<LdapSettingsResult> GetLdapSettings()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetLdapSettings(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        internal List<AuthenticationtypesResult> GetAuthenticationTypes()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetAuthenticationTypes(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        /// <summary>
        /// Persist new LDAP Setting to the database
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <returns></returns>
        internal bool CreateLdapSetting(ref LdapSettingsResult ldapSetting, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateLdapSetting(ldapSetting, encryptedSessionKey);

            if(base.CheckResponseException(response, log, out messages))
            {
                ldapSetting.ldap_setting_id = response.Value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Persist changes to LDAP Setting to DB
        /// </summary>
        /// <param name="ldapSetting"></param>        
        internal bool UpdateLdapSetting(LdapSettingsResult ldapSetting, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateLdapSetting(ldapSetting, encryptedSessionKey);

            return base.CheckResponseException(response, log, out messages);
        }

        /// <summary>
        /// Persist changes to LDAP Setting to DB
        /// </summary>
        /// <param name="ldapSetting"></param>        
        internal string DeleteLdapSetting(int ldap_setting_id)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DeleteLdapSetting(ldap_setting_id, encryptedSessionKey);

            base.CheckResponse(response, log);

            return "";
        }

        //internal List<issuer> GetLdapIssuers()
        //{
        //    string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
        //                                                               SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
        //                                                               SecurityParameters.EXTERNAL_SECURITY_KEY);

        //    var response = _issuanceService.GetLdapIssuers(encryptedSessionKey);

        //    if (response.ResponseType != ResponseType.SUCCESSFUL)
        //    {
        //        throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
        //    }

        //    return response.Value.ToList();
        //}

        #endregion

        #region Connection Parameters / Interfaces

        internal string[] GetFilePathReferences()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetFilePathReference(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal List<ConnectionParamsResult> GetConnectionParameters()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetConnectionParameters(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal ConnectionParametersResult GetConnectionParameter(int connParameterId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetConnectionParameter(connParameterId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        /// <summary>
        /// Persist new connection param to the database
        /// </summary>
        /// <param name="connectionParam"></param>
        /// <returns></returns>
        public ConnectionParametersResult CreateConnectionParam(ConnectionParametersResult connectionParam)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateConnectionParam(connectionParam, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        /// <summary>
        /// Persist changes to connection parameter to DB
        /// </summary>
        /// <param name="connectionParam"></param>
        public ConnectionParametersResult UpdateConnectionParam(ConnectionParametersResult connectionParam)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateConnectionParam(connectionParam, encryptedSessionKey);

            base.CheckResponse(response, log);

            return connectionParam;
        }

        /// <summary>
        /// Persist changes to connection_parameter to DB
        /// </summary>
        /// <param name="ldapSetting"></param>        
        internal string DeleteConnectionParam(int connectionParamId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DeleteConnectionParam(connectionParamId, encryptedSessionKey);

            base.CheckResponse(response, log);

            return "";
        }



        internal List<ProductInterfaceResult> GetProductInterfaces(int connectionParamId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);
            
            var response = m_indigoApp.GetProductInterfaces(connectionParamId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<IssuerInterfaceResult> GetIssuerConnectionParams(int connectionParamId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetIssuerConnectionParams(connectionParamId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        #endregion

        #region "EXTERNAL SYSTEM"
        internal List<LangLookup> LangLookupExternalSystems()
        {

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupExternalSystems(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        internal bool CreateExternalSystem(ExternalSystemFieldResult externalSystemsResult, out int external_system_id, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateExternalSystems(externalSystemsResult, encryptedSessionKey);
            external_system_id = 0;
            if (base.CheckResponseException(response, log, out responsemessage))
            {
                external_system_id = (int)response.Value;
                return true;
            }

            return false;
        }

        internal bool UpdateExternalSystem(ExternalSystemFieldResult externalSystemsResult, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateExternalSystem(externalSystemsResult, encryptedSessionKey);

            return base.CheckResponseException(response, log, out responsemessage);
        }

        internal ExternalSystemFieldResult GetExternalSystem(int? external_system_id, int rowIndex, int rowsperpage)
        {

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetExternalSystems(external_system_id, rowIndex, rowsperpage,encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }
       
        internal bool DeleteExternalSystem(int? external_system_id, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
           
            var response = m_indigoApp.DeleteExternalSystems(external_system_id, encryptedSessionKey);

            base.CheckResponse(response, log);
            responsemessage = response.ResponseMessage;
            return true;
        }


        internal bool CreateExternalSystemFields(ExternalSystemFieldsResult externalsystemfield, out int external_system_id, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateExternalSystemFields(externalsystemfield, encryptedSessionKey);
            external_system_id = 0;
            if (base.CheckResponseException(response, log, out responsemessage))
            {
                external_system_id = (int)response.Value;
                return true;
            }

            return false;
        }

        internal bool UpdateExternalSystemFields(ExternalSystemFieldsResult externalsystemfield, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateExternalSystemFields(externalsystemfield, encryptedSessionKey);

            return base.CheckResponseException(response, log, out responsemessage);
        }

        internal List<ExternalSystemFieldsResult> GetExternalSystemsFields(int? external_system_type_id, int rowIndex, int rowsperpage)
        {

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetExternalSystemsFields(external_system_type_id, rowIndex, rowsperpage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal bool DeleteExternalSystemField(int? external_system_field_id, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DeleteExternalSystemField(external_system_field_id, encryptedSessionKey);

            base.CheckResponse(response, log);
            responsemessage = response.ResponseMessage;
            return true;
        }
        
    }
        #endregion

}