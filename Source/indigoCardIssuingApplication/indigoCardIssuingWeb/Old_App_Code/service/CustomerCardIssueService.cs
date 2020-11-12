using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.security;
using Common.Logging;
using System.Web;
using System.Security.Principal;
using indigoCardIssuingWeb.Old_App_Code.service;


namespace indigoCardIssuingWeb.service
{
    public class CustomerCardIssueService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CustomerCardIssueService));
        private static Hashtable _CardsBeingProcessed = new Hashtable();
        private static Hashtable _loggedOnUser = new Hashtable();
        //private static readonly Service1SoapClient _issuanceService = new Service1SoapClient();

        //private static LangLookup[] issueReasons = new LangLookup[0];
        //private static LangLookup[] accountTypes = new LangLookup[0];

        #region PUBLIC METHODS

        public List<LangLookup> LangLookupCardIssueReasons()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupCardIssueReasons(encryptedSessionKey);
            base.CheckResponse(response, log);
            return response.Value.ToList();

            //if (issueReasons == null || issueReasons.Length == 0)
            //{
            //    var response = m_indigoApp.LangLookupCardIssueReasons(encryptedSessionKey);

            //    base.CheckResponse(response, log);

            //    issueReasons = response.Value;
            //}

            //return issueReasons.ToList();
        }

        public List<LangLookup> LangLookupCustomerAccountTypes()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupCustomerAccountTypes(encryptedSessionKey);

            base.CheckResponse(response, log);
            return response.Value.ToList();

            //if (accountTypes == null || accountTypes.Length == 0)
            //{
            //    var response = m_indigoApp.LangLookupCustomerAccountTypes(encryptedSessionKey);

            //    base.CheckResponse(response, log);
            //    accountTypes = response.Value;
            //}

            //return accountTypes.ToList();
        }

        public List<LangLookup> LangLookupCustomerTypes()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupCustomerType(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<LangLookupChar> LangLookupGenderTypes()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupGenderType(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<LangLookup> LangLookupCustomerTitles()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupCustomerTitle(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<LangLookup> LangLookupCustomerResidency()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupCustomerResidency(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        /// <summary>
        /// Fetch a list of Branch card codes.
        /// </summary>
        /// <param name="BranchCardCodeType"></param>
        /// <param name="isException"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<branch_card_codes> ListBranchCardCodes(int BranchCardCodeType, bool isException)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ListBranchCardCodes(BranchCardCodeType, isException, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        #region CLASSIC CARD METHODS
        /// <summary>
        /// Create a card request for classic card issuing.
        /// </summary>
        /// <param name="customerDetails"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal bool RequestCardForCustomer(CustomerDetails customerDetails, long? renewalDetailId, out long cardId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RequestCardForCustomer(customerDetails, renewalDetailId, encryptedSessionKey);
            cardId = response.Value;

            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool RequestInstantCardForCustomer(CustomerDetails customerDetails, out long cardId, out string printJobId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RequestInstantCardForCustomer(customerDetails, encryptedSessionKey, out printJobId);
            cardId = response.Value;

            return base.CheckResponse(response, log, out responseMessage);
        }
        internal bool RequestHybridCardForCustomer(CustomerDetails customerDetails, out long cardId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RequestHybridCardForCustomer(customerDetails, encryptedSessionKey);
            cardId = response.Value;

            return base.CheckResponse(response, log, out responseMessage);
        }
        internal bool UpdateCustomerDetails(long cardId, long customerId, CustomerDetails customerDetails, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateCustomerDetails(cardId, customerId, customerDetails, encryptedSessionKey);
            return base.CheckResponse(response, log, out responseMessage);
        }
        #endregion

        internal CustomerDetailsResult GetCustomerDetails(long cardId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCustomerDetails(cardId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        /// <summary>
        /// Issue card to customer.
        /// </summary>
        /// <param name="customerDetails"></param>
        /// <param name="statusReservedFor"></param>
        /// <returns></returns>
        internal bool IssueCardToCustomer(CustomerDetails customerAccount, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.IssueCardToCustomer(customerAccount, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool createPinRequest(PinObject pinDetails, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.CreatePinRequest(pinDetails, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool updatePinRequestStatus(PinObject pinDetails, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.UpdatePinRequestStatus(pinDetails, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool updatePinRequestStatusForRole(PinObject pinDetails, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.UpdatePinRequestStatusForReissue(pinDetails, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool updatePinBlockStatus(TerminalCardData pinDetails, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.updatePinFileStatus(pinDetails, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool updatePinBatchStatus(TerminalCardData pinDetails, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.updateBatchFileStatus(pinDetails, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool deletePinBlock(string product_pan_bin_code, string pan_last_four, string expiry_date, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.deletePinBlock(product_pan_bin_code, pan_last_four,expiry_date, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }


        //internal bool PINReissue(int issuerId, int branchId, int productId, long? authoriseUserId, byte[] pinIndex, out string responseMessage)
        //{
        //    string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
        //                                                               SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
        //                                                               SecurityParameters.EXTERNAL_SECURITY_KEY);

        //    var response = m_indigoApp.PINReissue(issuerId, branchId, productId, authoriseUserId, pinIndex, encryptedSessionKey);

        //    return base.CheckResponse(response, log, out responseMessage);
        //}

        /// <summary>
        /// Used to approve or reject maker checker.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="approved"></param>
        /// <returns></returns>
        internal bool MakerChecker(long cardId, Boolean approved, string comments, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.MakerChecker(cardId, approved, comments, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        /// <summary>
        /// Upload PIN to Indigo an mark it as having pin captured.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        internal bool IssueCardPinCaptured(long cardId, long? pinAuthUserId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.IssueCardPinCaptured(cardId, pinAuthUserId, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        /// <summary>
        /// Mark card as PRINTED
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        internal bool IssueCardPrinted(long cardId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.IssueCardPrinted(cardId, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        /// <summary>
        /// Mark card as PRINTED_ERROR
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        internal bool IssueCardPrintError(long cardId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.IssueCardPrintError(cardId, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool RequestMakerChecker(long requestId, bool approved, string comments, out string responseMessage)
        {

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RequestMakerChecker(requestId, approved, comments, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        /// <summary>
        /// Mark card as SPOILT
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        internal bool IssueCardSpoil(long cardId, int branchCardCodeId, string spoilComments, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.IssueCardSpoil(cardId, branchCardCodeId, spoilComments, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        /// <summary>
        /// Mark card as ISSUED
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        internal bool IssueCardComplete(long cardId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.IssueCardComplete(cardId, encryptedSessionKey);

            return base.CheckResponse(response, log, out responseMessage);
        }

        /// <summary>
        /// Call the CoreBankingSystem to do an account lookup.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="accountNumber"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public bool GetAccountDetail(int issuerId, int productId, int cardIssueReasonId, int branchId, string accountNumber, out AccountDetails accountDetails, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetAccountDetail(issuerId, productId, cardIssueReasonId, branchId, accountNumber, encryptedSessionKey);
            accountDetails = response.Value;

            return base.CheckResponse(response, log, out messages);
        }


        public bool PinFieldDecryption(int issuerId, ZoneMasterKey zmk, TerminalCardData cardData, TerminalSessionKey zpk, string operatorCode, Product product, out DecryptedFields decryptedFields, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);
           

            var response = m_indigoApp.PinFieldDecryption(issuerId, zmk, cardData, zpk, operatorCode, product, encryptedSessionKey);
            decryptedFields = response.Value;

            return base.CheckResponse(response, log, out responseMessage);
        }

        public bool PinFieldDecryptionWithMessaging(int issuerId, ZoneMasterKey zmk, TerminalCardData cardData, TerminalSessionKey zpk, string operatorCode, Product product, CustomerDetailsResult customer, Messaging message_params, out DecryptedFields decryptedFields, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.PinFieldDecryptionWithMessaging(issuerId, zmk, cardData, zpk, operatorCode, product, customer, message_params, encryptedSessionKey);
            decryptedFields = response.Value;

            return base.CheckResponse(response, log, out responseMessage);
        }

        public List<LangLookup> LangLookupCardIssueMethod()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupCardIssueMethod(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<CardHistoryReference> GetCardHistory(long cardId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCardReferenceHistory(cardId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<CardHistoryStatus> GetCardStatus(long cardId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCardStatusHistory(cardId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<RequestStatusHistoryResult> GetRequestStatusHistory(long requestId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetRequestStatusHistory(requestId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<RequestReferenceHistoryResult> GetRequestReferceHistory(long requestId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetRequestReferenceHistory(requestId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal bool CardLimitCreate(long cardId, decimal limit)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CardLimitCreate(cardId, limit, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return false;
        }

        internal bool CardLimitUpdate(long cardId, decimal limit)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CardLimitUpdate(cardId, limit, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return false;
        }

        internal bool CardLimitApprove(long cardId, decimal limit)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CardLimitApprove(cardId, limit, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return false;
        }

        internal bool CardLimitApproveManager(long cardId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CardLimitApproveManager(cardId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return false;
        }

        internal bool CardLimitContractNumber(long cardId, string contractNumber)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CardLimitSetManualContractNumber(cardId, contractNumber, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return false;
        }

        internal CardLimitModel CardLimitGet(long cardId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CardLimitGet(cardId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal bool CardSetReviewStatus(long cardId, int statusId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.SetCreditStatus(cardId, statusId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return false;
        }
        #endregion
    }
}