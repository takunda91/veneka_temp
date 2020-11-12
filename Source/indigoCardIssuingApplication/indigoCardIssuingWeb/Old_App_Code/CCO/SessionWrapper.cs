using System;
using System.Web;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.CardIssuanceService;
using System.Collections.Generic;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.App_Code;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.App_Code.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.webpages.pin;

namespace indigoCardIssuingWeb.CCO
{
    public static class SessionWrapper
    {
      

        #region Private Methods

        private static T GetFromSession<T>(string key)
        {
            object obj = HttpContext.Current.Session[key];
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;
        }

        private static void SetInSession<T>(string key, T value)
        {
            if (value == null)
            {                
                HttpContext.Current.Session.Remove(key);
            }
            else
            {
                HttpContext.Current.Session[key] = value;
            }
        }

        private static T GetFromApplication<T>(string key)
        {
            return (T)HttpContext.Current.Application[key];
        }

        private static void SetInApplication<T>(string key, T value)
        {
            if (value == null)
            {
                HttpContext.Current.Application.Remove(key);
            }
            else
            {
                HttpContext.Current.Application[key] = value;
            }
        }

        private static int ConvertToInt(string val)
        {
            int tempVal = -1;

            tempVal = int.Parse(val);

            return tempVal;
        }

        #endregion

        #region General

        public static int AuthConfigId
        {
            get { return GetFromSession<int>("AuthConfigId"); }
            set { SetInSession("AuthConfigId", value); }
        }

        /// <summary>
        /// Removes EVERYTHING out of session.
        /// </summary>
        public static void ClearSession()
        {
            HttpContext.Current.Session.Clear();
        }

        //public static List<InterfaceWrapper> IssuerInterfaces
        //{
        //    get { return GetFromSession<List<InterfaceWrapper>>("IssuerInterfaces"); }
        //    set { SetInSession("IssuerInterfaces", value); }
        //}
        public static int? FileLoadId
        {
            get { return GetFromSession<int?>("FileLoadId"); }
            set { SetInSession("FileLoadId", value); }
        }
        public static int? ExternalSystemId
        {
            get { return GetFromSession<int?>("ExternalSystemId"); }
            set { SetInSession("ExternalSystemId", value); }
        }
        public static int? ExternalSystemFieldId
        {
            get { return GetFromSession<int?>("ExternalSystemFieldId"); }
            set { SetInSession("ExternalSystemFieldId", value); }
        }
        public static int? ProductFeeAccountingId
        {
            get { return GetFromSession<int?>("ProductFeeAccountingId"); }
            set { SetInSession("ProductFeeAccountingId", value); }
        }

        public static int? ProductFeeSchemeId
        {
            get { return GetFromSession<int?>("ProductFeeSchemeId"); }
            set { SetInSession("ProductFeeSchemeId", value); }
        }

        public static int ProductID
        {
            get { return GetFromSession<int>("ProductID"); }
            set { SetInSession("ProductID", value); }
        }

        public static ProductSearchParameters ProductSearchParameter
        {
            get { return GetFromSession<ProductSearchParameters>("ProductSearchParameters"); }
            set { SetInSession("ProductSearchParameters", value); }
        }

        public static int Product_ID
        {
            get { return GetFromSession<int>("Product_ID"); }
            set { SetInSession("Product_ID", value); }
        }
        public static List<int> CurrencyList
        {
            get { return GetFromSession<List<int>>("CurrencyList"); }
            set { SetInSession("CurrencyList", value); }
        }
        public static int FontId
        {
            get { return GetFromSession<int>("FontId"); }
            set { SetInSession("FontId", value); }
        }
        public static FileUpload UploadControl
        {
            get { return GetFromSession<FileUpload>("UploadControl"); }
            set { SetInSession("UploadControl", value); }
        }
        
        public static PageLayout PresentMode
        {
            get { return GetFromSession<PageLayout>("PresentMode"); }
            set { SetInSession("PresentMode", value); }
        }
        public static bool PreviewYN
        {
            get { return GetFromSession<bool>("PreviewYN"); }
            set { SetInSession("PreviewYN", value); }
        }
        public static string DateType
        {
            get { return GetFromSession<string>("DateType"); }
            set { SetInSession("DateType", value); }
        }
        
        public static long? loadBatchId
        {
            get { return GetFromSession<long?>("loadBatchId"); }
            set { SetInSession("loadBatchId", value); }
        }
        public static long? ThreedBatchId
        {
            get { return GetFromSession<long?>("ThreeBatchId"); }
            set { SetInSession("ThreeBatchId", value); }
        }

        public static long? ExportBatchId
        {
            get { return GetFromSession<long?>("ExportBatchId"); }
            set { SetInSession("ExportBatchId", value); }
        }

        public static long? RemoteCardId
        {
            get { return GetFromSession<long?>("RemoteCardId"); }
            set { SetInSession("RemoteCardId", value); }
        }

        public static long? DistBatchId
        {
            get { return GetFromSession<long?>("DistBatchId"); }
            set { SetInSession("DistBatchId", value); }
        }

        public static long? PinRequestId
        {
            get { return GetFromSession<long?>("PinRequestId"); }
            set { SetInSession("PinRequestId", value); }
        }

        public static int? PinRequestStatus
        {
            get { return GetFromSession<int?>("PinRequestStatus"); }
            set { SetInSession("PinRequestStatus", value); }
        }

        public static int? PinRequestType
        {
            get { return GetFromSession<int?>("PinRequestType"); }
            set { SetInSession("PinRequestType", value); }
        }

        public static string RejectComments
        {
            get { return GetFromSession<string>("RejectComments"); }
            set { SetInSession("RejectComments", value); }
        }

        public static long? PinBatchId
        {
            get { return GetFromSession<long?>("PinBatchId"); }
            set { SetInSession("PinBatchId", value); }
        }
        public static NotificationMessages Message
        {
            get { return GetFromSession<NotificationMessages>("Message"); }
            set { SetInSession("Message", value); }
        }
        public static long? PinReissueId
        {
            get { return GetFromSession<long?>("PinReissueId"); }
            set { SetInSession("PinReissueId", value); }
        }

        public static long? PrintBatchId
        {
            get { return GetFromSession<long?>("PrintBatchId"); }
            set { SetInSession("PrintBatchId", value); }
        }

        
        public static bool SearchUserMode
        {
            get { return GetFromSession<bool>("SearchUserMode"); }
            set { SetInSession("SearchUserMode", value); }
        }

        public static AuditSearch AuditSearch
        {
            get { return GetFromSession<AuditSearch>("AuditSearch"); }
            set { SetInSession("AuditSearch", value); }
        }
        public static ProductResult ProductlistResult
        {
            get { return GetFromSession<ProductResult>("ProductlistResult"); }
            set { SetInSession("ProductlistResult", value); }
        }

        public static UserSearch UserSearch
        {
            get { return GetFromSession<UserSearch>("UserSearch"); }
            set { SetInSession("UserSearch", value); }
        }
        public static UserSearchParameters UserSearchParameters
        {
            get { return GetFromSession<UserSearchParameters>("UserSearchParameters"); }
            set { SetInSession("UserSearchParameters", value); }
        }
        public static CardSearch CardSearch
        {
            get { return GetFromSession<CardSearch>("CardSearch"); }
            set { SetInSession("CardSearch", value); }
        }

        public static CardSearchResult CardSearchResultItem
        {
            get { return GetFromSession<CardSearchResult>("CardSearchResultItem"); }
            set { SetInSession("CardSearchResultItem", value); }
        }



       
        public static List<CardSearchResult> CardSearchResults
        {
            get { return GetFromSession<List<CardSearchResult>>("CardSearchResults"); }
            set { SetInSession("CardSearchResults", value); }
        }

        public static ISearchParameters CardSearchParams
        {
            get { return GetFromSession<ISearchParameters>("CardSearchParams"); }
            set { SetInSession("CardSearchParams", value); }
        }

        public static ISearchParameters RemoteCardUpdateSearchParams
        {
            get { return GetFromSession<ISearchParameters>("RemoteCardUpdateSearchParams"); }
            set { SetInSession("RemoteCardUpdateSearchParams", value); }            
        }

        public static List<RemoteCardUpdateSearchResult> RemoteCardUpdateSearchResults
        {
            get { return GetFromSession<List<RemoteCardUpdateSearchResult>>("RemoteCardUpdateSearchResults"); }
            set { SetInSession("RemoteCardUpdateSearchResults", value); }
        }

        public static string BackURL
        {
            get { return GetFromSession<string>("BackURL"); }
            set { SetInSession("BackURL", value); }
        }

        public static List<LoadBatchResult> LoadBatchSearchResult
        {
            get { return GetFromSession<List<LoadBatchResult>>("LoadBatchSearchResult"); }
            set { SetInSession("LoadBatchSearchResult", value); }
        }
        public static List<ThreedBatchResult> ThreedBatchSearchResult
        {
            get { return GetFromSession<List<ThreedBatchResult>>("ThreedBatchSearchResult"); }
            set { SetInSession("ThreedBatchSearchResult", value); }
        }

        public static LoadBatchSearchParameters LoadBatchSearchParams
        {
            get { return GetFromSession<LoadBatchSearchParameters>("LoadBatchSearchParams"); }
            set { SetInSession("LoadBatchSearchParams", value); }
        }
        public static ThreedBatchSearchParameters ThreedBatchSearchParams
        {
            get { return GetFromSession<ThreedBatchSearchParameters>("ThreedBatchSearchParams"); }
            set { SetInSession("ThreedBatchSearchParams", value); }
        }

        public static List<DistBatchResult> DistributionBatchSearchResult
        {
            get { return GetFromSession<List<DistBatchResult>>("DistributionBatchSearchResult"); }
            set { SetInSession("DistributionBatchSearchResult", value); }
        }
        public static List<PinBatchResult> PinBatchSearchResult
        {
            get { return GetFromSession<List<PinBatchResult>>("PinBatchSearchResult"); }
            set { SetInSession("PinBatchSearchResult", value); }
        }
        public static List<CardIssuanceService.PinRequestList> PinRequestSearchResult
        {
            get { return GetFromSession<List<CardIssuanceService.PinRequestList>>("PinRequestSearchResult"); }
            set { SetInSession("PinRequestSearchResult", value); }
        }

        public static List<PrintBatchResult> PrintBatchSearchResult
        {
            get { return GetFromSession<List<PrintBatchResult>>("PrintBatchSearchResult"); }
            set { SetInSession("PrintBatchSearchResult", value); }
        }
        public static List<HybridRequestResult> HybridRequestSearchResult
        {
            get { return GetFromSession<List<HybridRequestResult>>("HybridRequestSearchResult"); }
            set { SetInSession("HybridRequestSearchResult", value); }
        }
        public static List<PinReissueWSResult> PinReissueSearchResult
        {
            get { return GetFromSession<List<PinReissueWSResult>>("PinReissueSearchResult"); }
            set { SetInSession("PinReissueSearchResult", value); }
        }

        public static ExportBatchSearchParameters ExportBatchSearchParams
        {
            get { return GetFromSession<ExportBatchSearchParameters>("ExportBatchSearchParams"); }
            set { SetInSession("ExportBatchSearchParams", value); }
        }

        public static List<ExportBatchResult> ExportBatchSearchResults
        {
            get { return GetFromSession<List<ExportBatchResult>>("ExportBatchSearchResultss"); }
            set { SetInSession("ExportBatchSearchResultss", value); }
        }

        public static DistBatchSearchParameters DistBatchSearchParams
        {
            get { return GetFromSession<DistBatchSearchParameters>("DistBatchSearchParams"); }
            set { SetInSession("DistBatchSearchParams", value); }
        }

        public static PinBatchSearchParameters PinBatchSearchParams
        {
            get { return GetFromSession<PinBatchSearchParameters>("PinBatchSearchParams"); }
            set { SetInSession("PinBatchSearchParams", value); }
        }

        public static PinRequestSearchParameters PinRequestSearchParams
        {
            get { return GetFromSession<PinRequestSearchParameters>("PinRequestSearchParams"); }
            set { SetInSession("PinRequestSearchParams", value); }
        }

        public static PrintBatchSearchParameters PrintBatchSearchParams
        {
            get { return GetFromSession<PrintBatchSearchParameters>("PrintBatchSearchParams"); }
            set { SetInSession("PrintBatchSearchParams", value); }
        }
        
        public static PinReissueSearchParameters PinReissueSearchParams
        {
            get { return GetFromSession<PinReissueSearchParameters>("PinReissueSearchParams"); }
            set { SetInSession("PinReissueSearchParams", value); }
        }

        public static FileLoadSearchParameters FileLoadParams
        {
            get { return GetFromSession<FileLoadSearchParameters>("FileLoadParams"); }
            set { SetInSession("FileLoadParams", value); }
        }

        public static FileDetailsSearch FileLoadSearchParams
        {
            get { return GetFromSession<FileDetailsSearch>("FileLoadSearchParams"); }
            set { SetInSession("FileLoadSearchParams", value); }
        }

        public static List<GetAuditData_Result> AuditSearchResult
        {
            get { return GetFromSession<List<GetAuditData_Result>>("AuditSearchResult"); }
            set { SetInSession("AuditSearchResult", value); }
        }

        public static List<GetFileLoderLog_Result> FileLoadSearchResult
        {
            get { return GetFromSession<List<GetFileLoderLog_Result>>("FileLoadSearchResult"); }
            set { SetInSession("FileLoadSearchResult", value); }
        }
        public static IndigoComponentLicense IndigoComponentLicense
        {
            get { return GetFromSession<IndigoComponentLicense>("IndigoComponentLicense"); }
            set { SetInSession("IndigoComponentLicense", value); }
        }
        public static int? IssuerID
        {
            get { return GetFromSession<int?>("IssuerID"); }
            set { SetInSession("IssuerID", value); }
        }

        public static int? ManageIssuerID
        {
            get { return GetFromSession<int?>("ManageIssuerID"); }
            set { SetInSession("ManageIssuerID", value); }
        }
        public static int? UserAdminSettingsID
        {
            get { return GetFromSession<int?>("UserAdminSettingsID"); }
            set { SetInSession("UserAdminSettingsID", value); }
        }

        public static int? LDAPID
        {
            get { return GetFromSession<int>("LDAPID"); }
            set { SetInSession("LDAPID", value); }
        }


        public static int? InterfaceConnectionID
        {
            get { return GetFromSession<int>("InterfaceConnectionID"); }
            set { SetInSession("InterfaceConnectionID", value); }
        }
     
       
        public static int UserIssuerID
        {
            get { return GetFromSession<int>("UserIssuerID"); }
            set { SetInSession("UserIssuerID", value); }
        }

        public static string Username
        {
            get { return GetFromSession<string>("Username"); }
            set { SetInSession("Username", value); }
        }

        /// <summary>
        /// Gets the access based on the users roles.
        /// </summary>
        //public static Dictionary<UserRole, List<RolesIssuerResult>> UserRoles
        //{
        //    get { return GetFromSession<Dictionary<UserRole, List<RolesIssuerResult>>>("UserRoles"); }
        //    set { SetInSession("UserRoles", value); }
        //}

        //public static List<StatusFlowRole> StatusFlow
        //{
        //    get { return GetFromSession<List<StatusFlowRole>>("StatusFlow"); }
        //    set { SetInSession("StatusFlow", value); }
        //}

        //public static string SessionUserName
        //{
        //    get { return GetFromSession<string>("ApplicationUser"); }
        //    set { SetInSession("ApplicationUser", value); }
        //}

        public static long? SessionUserId
        {
            get { return GetFromSession<long?>("SessionUserId"); }
            set { SetInSession("SessionUserId", value); }
        }

        public static string SelectedUserName
        {
            get { return GetFromSession<string>("SelectedUserName"); }
            set { SetInSession("SelectedUserName", value); }
        }

        public static long? SelectedPendingUserId
        {
            get { return GetFromSession<long?>("SelectedPendingUserId"); }
            set { SetInSession("SelectedPendingUserId", value); }
        }
        
        public static long? SelectedUserId
        {
            get { return GetFromSession<long?>("SelectedUserId"); }
            set { SetInSession("SelectedUserId", value); }
        }

        public static string UserGroup
        {
            get { return GetFromSession<string>("UserGroup"); }
            set { SetInSession("UserGroup", (value != null ? value.ToUpper() : value)); }
        }

        public static int? SelectedUserGroupId
        {
            get { return GetFromSession<int?>("SelectedUserGroupId"); }
            set { SetInSession("SelectedUserGroupId", value); }
        }

        public static int IssuerExpire
        {
            get { return GetFromSession<int>("Lisence"); }
            set { SetInSession("Lisence", value); }
        }
        public static string CardViewMode
        {
            get { return GetFromSession<string>("CardViewMode"); }
            set { SetInSession("CardViewMode", value); }
        }
        #endregion

        #region Branch

        public static int? BranchID
        {
            get { return GetFromSession<int?>("BranchID"); }
            set { SetInSession("BranchID", value); }
        }

        public static string BranchDetails
        {
            get { return GetFromSession<string>("BranchDetails"); }
            set { SetInSession("BranchDetails", value); }
        }
        public static int? BranchStatusId
        {
            get { return GetFromSession<int?>("BranchStatusId"); }
            set { SetInSession("BranchStatusId", value); }
        }
        public static string UserBranchCode
        {
            get { return GetFromSession<string>("UserBranchCode"); }
            set { SetInSession("UserBranchCode", value); }
        }

        public static string setBranchIDStr
        {
            set { SetInSession("BranchID", ConvertToInt(value)); }
        }

        #endregion

        #region CustomerDetails

        public static CustomerDetails Customer
        {
            get { return GetFromSession<CustomerDetails>("Customer"); }
            set { SetInSession("Customer", value); }
        }

        #endregion

        #region Card

        public static string CardNumber
        {
            get { return GetFromSession<string>("CardNumber"); }
            set { SetInSession("CardNumber", value.ToUpper()); }
        }

        #endregion

        #region PinBlock

        /// <summary>
        ///     temporarily stores encrypted customer selected pin (pin block)
        /// </summary>
        public static string PinBlock
        {
            get { return GetFromSession<String>("PinBlock"); }
            set { SetInSession("PinBlock", value); }
        }

        //public static PINResponse PinIndex
        //{
        //    get { return GetFromSession<PINResponse>("PinIndex"); }
        //    set { SetInSession("PinIndex", value); }
        //}

        public static int TerminalIssuerId
        {
            get { return GetFromSession<int>("TerminalIssuerId"); }
            set { SetInSession("TerminalIssuerId", value); }
        }

        public static int? TerminalProductId
        {
            get { return GetFromSession<int?>("TerminalProductId"); }
            set { SetInSession("TerminalProductId", value); }
        }

        public static long? PinIssueCardId
        {
            get { return GetFromSession<long?>("PinIssueCardId"); }
            set { SetInSession("PinIssueCardId", value); }
        }

        public static int? ReissueBranchId
        {
            get { return GetFromSession<int?>("ReissueBranchId"); }
            set { SetInSession("ReissueBranchId", value); }
        }            

        public static bool PINReissue
        {
            get { return GetFromSession<bool>("PINReissue"); }
            set { SetInSession("PINReissue", value); }
        }

        #endregion
        
        #region Customer ID
        public static string CMSClientID
        {
            get { return GetFromSession<string>("clientID"); }
            set { SetInSession<string>("clientID", value); }
        }
        #endregion

        #region encrypted_PWK

        /// <summary>
        ///     temporarily stores PWK for PinPad
        /// </summary>
        public static string PWK
        {
            get { return GetFromSession<String>("pwk"); }
            set { SetInSession("pwk", value); }
        }

        #endregion

        #region PIN

        public static PINSearchResult PINSearchResult
        {
            get { return GetFromSession<PINSearchResult>("PINSearchResult"); }
            set { SetInSession("PINSearchResult", value); }
        }
        #endregion

        #region User

        public static int UserLanguage
        {
            get { return GetFromSession<int>("UserLanguage"); }
            set { SetInSession("UserLanguage", value); }
        }

        public static string UserState
        {
            get { return GetFromSession<string>("UserState"); }
            set { SetInSession("UserState", value.ToUpper()); }
        }

        public static string PasswordExpiryDate
        {
            //TODO: FIX!
            get { return GetFromSession<string>("PasswordExpiryDate"); }
            set { SetInSession("PasswordExpiryDate", value); }
        }

        public static string ManagedUsername
        {
            get { return GetFromSession<string>("ManagedUsername"); }
            set { SetInSession("ManagedUsername", value); }
        }

        //public static string WebServiceSessionKeys
        //{
        //    get { return GetFromSession<string>("WebServiceSessionKeys"); }
        //    set { SetInSession("WebServiceSessionKeys", value); }
        //}

        public static int IssuanceModeId
        {
            get { return GetFromSession<int>("IssuanceMode"); }
            set { SetInSession("IssuanceMode", value); }
        }

        public static string ClientAddress
        {
            get { return GetFromSession<string>("ClientAddress"); }
            set { SetInSession("ClientAddress", value); }
        }

        public static string SecurityKeyLocal
        {
            get { return GetFromSession<string>("SecurityKeyLocal"); }
            set { SetInSession("SecurityKeyLocal", value); }
        }

        public static string SecurityKeyRemote
        {
            get { return GetFromSession<string>("SecurityKeyRemote"); }
            set { SetInSession("SecurityKeyRemote", value); }
        }

        public static string UserLoggedInWorkstation
        {
            get { return GetFromSession<string>("UserLoggedInWorkstation"); }
            set { SetInSession("UserLoggedInWorkstation", value); }
        }

        //public static bool LdapUser
        //{
        //    get { return GetFromSession<bool>("LdapUser"); }
        //    set { SetInSession("LdapUser", value); }
        //}

        public static Dictionary<UserRole, Dictionary<long, List<int>>> CachedRoles
        {
            get { return GetFromSession<Dictionary<UserRole, Dictionary<long, List<int>>>>("CachedRoles"); }
            set { SetInSession("CachedRoles", value); }
        }

        #endregion

        #region Terminal

        public static TerminalSessionKey TerminalSessionKeys
        {
            get { return GetFromSession<TerminalSessionKey>("TerminalSessionKeys"); }
            set { SetInSession("TerminalSessionKeys", value); }
        }

        public static int MasterkeyId
        {
            get { return GetFromSession<int>("MasterkeyId"); }
            set { SetInSession("MasterkeyId", value); }
        }

        public static int TerminalId
        {
            get { return GetFromSession<int>("TerminalId"); }
            set { SetInSession("TerminalId", value); }
        }
        public static TerminalSearchParams TerminalSearchParms
        {
            get { return GetFromSession<TerminalSearchParams>("TerminalSearchParms"); }
            set { SetInSession("TerminalSearchParms", value); }
        }
        public static List<TerminalListResult> TerminalSearchParmsResult
        {
            get { return GetFromSession<List<TerminalListResult>>("TerminalSearchParmsResult"); }
            set { SetInSession("TerminalSearchParmsResult", value); }
        }

        #endregion

        #region Encryption
        
        public static byte[] FilePrivateKey
        {
            get { return GetFromSession<byte[]>("FilePrivateKey"); }
            set { SetInSession("FilePrivateKey", value); }
        }

        public static byte[] FilePublicKey
        {
            get { return GetFromSession<byte[]>("FilePublicKey"); }
            set { SetInSession("FilePublicKey", value); }
        }
        #endregion

        #region "Request "

        public static string RequestViewMode
        {
            get { return GetFromSession<string>("RequestViewMode"); }
            set { SetInSession("RequestViewMode", value); }
        }
        public static long? RequestId
        {
            get { return GetFromSession<long?>("RequestId"); }
            set { SetInSession("RequestId", value); }
        }

        public static ISearchParameters RequestSearchParam
        {
            get { return GetFromSession<ISearchParameters>("RequestSearchParam"); }
            set { SetInSession("RequestSearchParam", value); }
        }
        #endregion
    }


}


