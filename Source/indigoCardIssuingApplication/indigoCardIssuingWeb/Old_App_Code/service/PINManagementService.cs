using System;
using System.Collections.Generic;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.security;
using Common.Logging;
using System.Linq;
using indigoCardIssuingWeb.SearchParameters;
using System.Web;
using System.Security.Principal;
using indigoCardIssuingWeb.Old_App_Code.service;
using System.Collections;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using Org.BouncyCastle.Asn1.Ocsp;

namespace indigoCardIssuingWeb.service
{
    public class PINManagementService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PINManagementService));
        //private static readonly Service1SoapClient _issuanceService = new Service1SoapClient();

        public List<LangLookup> LangLookupPinReissueStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupPinReissueStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal string GetOperatorSessionKey()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetOperatorSessionKey(encryptedSessionKey);

           if (base.CheckResponse(response, log))
                return EncryptionManager.DecryptString(response.Value,
                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);

            return null;
        }

        /// <summary>
        /// Instant Pin Authorisation
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="authPin"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        internal long? GetUserAuthorisationPin(string username, string passcode, int branchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedUsername = EncryptionManager.EncryptString(username,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedPasscode = EncryptionManager.EncryptString(passcode,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetAuthorisationPin(encryptedUsername, encryptedPasscode, branchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal byte[] GeneratePinMailerBatchReport(int loadBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                    SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                    SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GeneratePinMailerBatchReport(loadBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;

            
        }

        internal bool AuthorisationPinApprove(string username, string passcode, int branchId, long pinReissueId, string comments, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedUsername = EncryptionManager.EncryptString(username,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedPasscode = EncryptionManager.EncryptString(passcode,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.AuthorisationPinApprove(encryptedUsername, encryptedPasscode, pinReissueId, comments, branchId, encryptedSessionKey);

            if (base.CheckResponse(response, log, out responseMessage))
                return true;

            return false;
        }

        internal bool AuthorisationPinReject(string username, string passcode, int branchId, long pinReissueId, string comments, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedUsername = EncryptionManager.EncryptString(username,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedPasscode = EncryptionManager.EncryptString(passcode,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.AuthorisationPinReject(encryptedUsername, encryptedPasscode, pinReissueId, comments, branchId, encryptedSessionKey);

            if (base.CheckResponse(response, log, out responseMessage))
                return true;

            return false;
        }

        internal List<PinReissueWSResult> PINReissueSearch(int? issuerId, int? branchId, int? userRoleId, int? pinReissueStatusId,int? pin_reissue_type, long? operatorUserId, bool operatorInProgress, long? authoriseUserId, byte[] index, DateTime? dateFrom, DateTime? dateTo, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.PINReissueSearch(issuerId, branchId, userRoleId, pinReissueStatusId, pin_reissue_type, operatorUserId, operatorInProgress, authoriseUserId, index, dateFrom, dateTo, pageIndex, rowsPerPage, encryptedSessionKey);
            // messages = response.ResponseMessage;

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<PinReissueWSResult> PINReissueSearch(PinReissueSearchParameters searchParams, int? pageIndex)
        {
            return this.PINReissueSearch(searchParams.IssuerId, searchParams.BranchId, null, searchParams.PinReissueStatusesId,searchParams.PinReissueTypeId, 
                    searchParams.OperatorUserId, searchParams.OperatorInProgress, searchParams.AuthoriseUserId, searchParams.Index, searchParams.DateFrom, searchParams.DateTo,
                     pageIndex ?? searchParams.PageIndex,  searchParams.RowsPerPage);
        }


        internal PinReissueWSResult GetPINReissue(long pinReissueId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPINReissue(pinReissueId, encryptedSessionKey);
            // messages = response.ResponseMessage;

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }


        //internal PinReissueResult RequestPINReissue(int issuerId, int branchId, int productId, byte[] index)
        //{
        //    string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
        //                                                             SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
        //                                                             SecurityParameters.EXTERNAL_SECURITY_KEY);

        //    var response = _issuanceService.RequestPINReissue(issuerId, branchId, productId, index, encryptedSessionKey);
        //    //messages = response.ResponseMessage;

        //    if (response.ResponseType == ResponseType.SUCCESSFUL)
        //    {
        //        return response.Value;
        //    }
        //    else if (response.ResponseType == ResponseType.UNSUCCESSFUL)
        //    {
        //        return null;
        //    }
        //    if (response.ResponseType != ResponseType.SUCCESSFUL)
        //    {
        //        throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
        //    }

        //    return null;
        //}

        internal bool EPinRequest(int issuerId, int branchId, int productId, string moblieNumber, string pan,out string responseMessage,out string result)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.EPinRequest(issuerId, branchId, productId, moblieNumber,pan, encryptedSessionKey);
            result = response.Value;

            return base.CheckResponseException(response, log, out responseMessage);
        }
        internal bool CancelPINReissue(long pinReissueId, string notes, out PinReissueWSResult result, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CancelPINReissue(pinReissueId, notes, encryptedSessionKey);
            result = response.Value;

            return base.CheckResponseException(response, log, out responseMessage);
        }

        internal bool ApprovePINReissue(long pinReissueId, string notes, out PinReissueWSResult result, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ApprovePINReissue(pinReissueId, notes, encryptedSessionKey);
            result = response.Value;

            return base.CheckResponseException(response, log, out responseMessage);
        }


        internal bool RejectPINReissue(long pinReissueId, string notes, out PinReissueWSResult result, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RejectPINReissue(pinReissueId, notes, encryptedSessionKey);
            result = response.Value;

            return base.CheckResponseException(response, log, out responseMessage);
        }


        internal bool CompletePINReissue(long pinReissueId, string notes, int issuerId, int productId, byte[] index, out PinReissueWSResult result, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CompletePINReissue(pinReissueId, notes,issuerId,productId,index, encryptedSessionKey);
            result = response.Value;

            return base.CheckResponseException(response, log, out responseMessage);
        }

        internal bool CompletePINReissue(long pinReissueId, string notes, out PinReissueWSResult result, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CompletePINReissue2(pinReissueId, notes, encryptedSessionKey);
            result = response.Value;

            return base.CheckResponseException(response, log, out responseMessage);
        }


        /// <summary>
        /// Searches for all PIN batches that the specific user has access to based on the input parameters.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="pinBatchReference"></param>
        /// <param name="pinBatchStatusId"></param>
        /// <param name="branchId"></param>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="pinBatchTypeId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="langaugeId"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        internal List<PinBatchResult> GetPinBatchesForUser(int? issuerId, string pinBatchReference, int? pinBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                               int? pinBatchTypeId, DateTime? startDate, DateTime? endDate, int rowsPerPage, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPinBatchesForUser(issuerId, pinBatchReference, pinBatchStatusId, branchId, cardIssueMethodId,
                                                                    pinBatchTypeId, startDate, endDate, rowsPerPage, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<PinRequestList> GetPinRequestsForUser(int? issuerId, string request_status, string reissue_approval_stage,  string request_type, int rowsPerPage, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPinRequestsForUser(issuerId, request_status, reissue_approval_stage, request_type, rowsPerPage, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<PinRequestList> SearchForPinReIssue(PinRequestSearchParameters RequestParams)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

             var response = m_indigoApp.SearchForPinReIssue(RequestParams.IssuerId,RequestParams.BranchId, RequestParams.ProductId,RequestParams.ProductBin,RequestParams.LastFourDigits,RequestParams.PinCustomerAccount,RequestParams.PinRequestReference,RequestParams.PageIndex, RequestParams.RowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<TerminalCardData> GetPinBlockForUser(int? issuerId, string request_status, int rowsPerPage, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPinBlockForUser(issuerId, request_status, rowsPerPage, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<TerminalCardData> GetPinBacthForUser(int? issuerId, string request_status, int rowsPerPage, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPinBatchForUser(issuerId, request_status, rowsPerPage, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        /// <summary>
        /// Searches for all PIN batches that the specific user has access to based on the input parameters.
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        internal List<PinBatchResult> GetPinBatchesForUser(PinBatchSearchParameters searchParams, int pageIndex)
        {
            return this.GetPinBatchesForUser(searchParams.IssuerId, searchParams.BatchReference, searchParams.PinBatchStatusId, searchParams.BranchId,
                                        searchParams.CardIssueMethodId, searchParams.PinBatchTypeId, searchParams.DateFrom, searchParams.DateTo, StaticDataContainer.ROWS_PER_PAGE,
                                        pageIndex);
        }

        internal List<PinRequestList> GetPinRequestsForUser(PinRequestSearchParameters searchParams, int pageIndex)
        {
            return this.GetPinRequestsForUser(searchParams.IssuerId, searchParams.PinRequestStatus,searchParams.PinApprovalStage, searchParams.PinRequestType, StaticDataContainer.ROWS_PER_PAGE, pageIndex);
        }

        /// <summary>
        /// Returns the pin batch details based on the pin batch.
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <returns></returns>
        internal PinBatchResult GetPinBatch(long pinBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPinBatch(pinBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal PinRequestViewDetails GetPinRequestView(long pinRequestId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPinRequestDetails(pinRequestId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal TerminalCardData GetPinBlockView(long pinRequestId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPinBlockDetails(pinRequestId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }


        internal TerminalCardData GetPinBatchView(long pinRequestId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPinBatchDetails(pinRequestId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }



        internal TerminalCardData GetTerminalCardData(string product_pan_bin_code, string pan_last_four, string expiry_date)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetTerminalCardData(product_pan_bin_code, pan_last_four, expiry_date, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal ZMKZPKDetails GetZoneKeys(int? issuer_id)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetZoneKeys(issuer_id, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal bool UpdatePinBatchStatus(long pinBatchId, int pinBatchStatusId, int flowDistBatchStatusesId, string statusNote, out PinBatchResult pinBatchResult, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdatePinBatchStatus(pinBatchId, pinBatchStatusId, flowDistBatchStatusesId, statusNote, encryptedSessionKey);
            pinBatchResult = response.Value;

            return base.CheckResponse(response, log, out messages);
        }
        internal bool UpdateMuiltplePinBatchStatus(ArrayList pinBatchId, ArrayList pinBatchStatusId, ArrayList flowPinBatchStatusesId, string notes, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateMuiltplePinBatchStatus(pinBatchId.ToArray(), pinBatchStatusId.ToArray(), flowPinBatchStatusesId.ToArray(), notes, encryptedSessionKey);
          

            return base.CheckResponse(response, log, out messages);
        }
        internal bool PinBatchRejectStatus(long pinBatchId, int pinBatchStatusId, string statusNote, out PinBatchResult pinBatchResult, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.PinBatchRejectStatus(pinBatchId, pinBatchStatusId, statusNote, encryptedSessionKey);
            pinBatchResult = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        internal bool PinMailerReprintRequest(long cardId, string notes, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.PinMailerReprintRequest(cardId, notes, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }

        internal bool PinMailerReprintApprove(long cardId, string notes, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.PinMailerReprintApprove(cardId, notes, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }

        internal bool PinMailerReprintReject(long cardId, string notes, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.PinMailerReprintReject(cardId, notes, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }

        internal List<PinMailerReprintRequestResult> PinMailerReprintList(int? issuerId, int? branchId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.PinMailerReprintList(issuerId, branchId, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal bool PinMailerReprintBatchCreate(int? cardIssueMethodId, int? issuerId, int? branchId, int? productId, out int pinBatchId, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.PinMailerReprintBatchCreate(cardIssueMethodId, issuerId, branchId, productId, encryptedSessionKey);
            pinBatchId = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        internal List<PinMailerReprintResult> SearchPinMailerReprint(int? issuerId, int? branchId, int? pinMailerReprintStatusId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.SearchPinMailerReprint(issuerId, branchId, pinMailerReprintStatusId, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<PinMailerReprintResult> SearchPinMailerReprint(PinMailerReprintSearchParameters searchParam, int? pageIndex, int? rowsPerPage)
        {
            return this.SearchPinMailerReprint(searchParam.IssuerId, searchParam.BranchId, searchParam.PinMailerReprintStatusId, pageIndex ?? searchParam.PageIndex, rowsPerPage ?? searchParam.RowsPerPage);
        }
        
        public List<LangLookup> LangLookupPinBatchStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupPinBatchStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal byte[] GeneratePinBatchReport(long pinBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                    SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                    SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GeneratePinBatchReport(pinBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }
    }
}