using System;
using System.Collections.Generic;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Objects;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.PINManagement.dal;
using Veneka.Indigo.PINManagement.objects;
using Veneka.Indigo.PinMailerPrinting.objects;
using Veneka.Indigo.PinMailerPrinting.printerimplementations;
using Veneka.Indigo.PinMailerPrinting.utility;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.PINManagement.Reports;
using Veneka.Indigo.Common.Models.IssuerManagement;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.PINManagement
{
    public class PINManagementService
    {
        private readonly PINManagementDAL _pinDAL = new PINManagementDAL();
        private readonly PrintPINMailer _printMailer = new PrintPINMailer();
        private readonly PinBatchReport _reports = new PinBatchReport();
        private readonly ResponseTranslator _translator = new ResponseTranslator();

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
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<PinBatchResult> GetPinBatchesForUser(int? issuerId, string pinBatchReference, int? pinBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                               int? pinBatchTypeId, DateTime? startDate, DateTime? endDate, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinBatchesForUser(issuerId, pinBatchReference, pinBatchStatusId, branchId, cardIssueMethodId,
                                                 pinBatchTypeId, startDate, endDate, langaugeId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);
        }

        public List<PinRequestList> GetPinRequestsForUser(int? issuerId, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinRequestsForUser(issuerId, langaugeId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);
        }
        //GetPinRequestsForUserForStatus
        public List<PinRequestList> GetPinRequestsForUserForStatus(int? issuerId,string request_status, string reissue_approval_stage, string request_type, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinRequestsForUserForStatus(issuerId, request_status, reissue_approval_stage, request_type, langaugeId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);
        }

        public List<PinRequestList> SearchForPinReIssue(int? issuerId, int? BranchId, int? ProductId, string ProductBin, string lastdigits, string accountnumber,
                                                                   string pinrequestref, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.SearchForPinReIssue(issuerId, BranchId, ProductId, ProductBin, lastdigits, accountnumber, pinrequestref, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

        }

        public List<TerminalCardData> GetPinBlockForUser(int? issuerId, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinBlockForUser(issuerId, langaugeId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);
        }
        //GetPinBlockForUserForStatus
        public List<TerminalCardData> GetPinBlockForUserForStatus(int? issuerId, string request_status, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinBlockForUserForStatus(issuerId, request_status, langaugeId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);
        }

        //GetPinBatchForUserForStatus
        public List<TerminalCardData> GetPinBatchForUserForStatus(int? issuerId, string request_status, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinBatchForUserForStatus(issuerId, request_status, langaugeId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);
        }

        public List<TerminalCardData> GetPinMailerBatchHistory( int pin_batch_header_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinMailerBatchHistory(pin_batch_header_id, langaugeId, auditUserId, auditWorkstation);
        }

        public List<TerminalCardData> GetPinMailerBatchCards(int pin_batch_header_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinMailerBatchCards(pin_batch_header_id, langaugeId, auditUserId, auditWorkstation);
        }

        public PinRequestViewDetails GetPinRequestViewDetails(long pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinRequestViewDetails(pin_request_id, langaugeId, auditUserId, auditWorkstation);
        }
        //GetPinBlockViewDetails
        public TerminalCardData GetPinBlockViewDetails(long pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinBlockViewDetails(pin_request_id, langaugeId, auditUserId, auditWorkstation);
        }

        //GetPinBatchViewDetails
        public TerminalCardData GetPinBatchViewDetails(long pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinBatchViewDetails(pin_request_id, langaugeId, auditUserId, auditWorkstation);
        }

        public TerminalCardData GetTerminalCardData(string pan_product_bin, string pan_last_four, string expiry_date, int langaugeId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetTerminalCardData(pan_product_bin, pan_last_four, expiry_date, langaugeId, auditUserId, auditWorkstation);
        }

        public RestWebservicesHandler GetRestParams(int issuer_id, string webservice_type, int langaugeId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetRestParams(issuer_id, webservice_type, langaugeId, auditUserId, auditWorkstation);
        }

        public ZMKZPKDetails GetZoneKeys(int? issuer_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetZoneKeys(issuer_id, langaugeId, auditUserId, auditWorkstation);
        }

        //GetPinRequestViewDetails


        //GetPinRequestsForUser
        /// <summary>
        /// Returns the pin batch details based on the pin batch.
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public PinBatchResult GetPinBatch(long pinBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinBatch(pinBatchId, languageId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Update Pin Batch Status
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="pinBatchStatusId"></param>
        /// <param name="flowDistBatchStatusesId"></param>
        /// <param name="statusNote"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="pinBatchResult"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public bool UpdatePinBatchStatus(long pinBatchId, int pinBatchStatusId, int flowDistBatchStatusesId, string statusNote, int languageId, long auditUserId, string auditWorkstation, out PinBatchResult pinBatchResult, out string responseMessage)
        {
            var response = _pinDAL.UpdatePinBatchStatus(pinBatchId, pinBatchStatusId, flowDistBatchStatusesId, statusNote, languageId, auditUserId, auditWorkstation, out pinBatchResult);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Pin Batch Reject Status
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="pinBatchStatusesId"></param>
        /// <param name="notes"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="result"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public bool PinBatchRejectStatus(long pinBatchId, int pinBatchStatusesId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinBatchResult result, out string responseMessage)
        {
            var response = _pinDAL.PinBatchRejectStatus(pinBatchId, pinBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, out result);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Pin Mailer Reprint Request
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="notes"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public bool PinMailerReprintRequest(long cardId, string notes, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _pinDAL.PinMailerReprintRequest(cardId, notes, languageId, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Pin Mailer Reprint Approval
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="notes"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public bool PinMailerReprintApprove(long cardId, string notes, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _pinDAL.PinMailerReprintApprove(cardId, notes, languageId, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Pin Mailer Reprint Rejection
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="notes"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public bool PinMailerReprintReject(long cardId, string notes, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _pinDAL.PinMailerReprintReject(cardId, notes, languageId, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="branchId"></param>
        /// <param name="languageId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<PinMailerReprintRequestResult> PinMailerReprintList(int? issuerId, int? branchId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.PinMailerReprintList(issuerId, branchId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="issuerId"></param>
        /// <param name="branchId"></param>
        /// <param name="productId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="pinBatchId"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public bool PinMailerReprintBatchCreate(int? cardIssueMethodId, int? issuerId, int? branchId, int? productId, int languageId, long auditUserId, string auditWorkstation, out int pinBatchId, out string responseMessage)
        {
            var response = _pinDAL.PinMailerReprintBatchCreate(cardIssueMethodId, issuerId, branchId, productId, auditUserId, auditWorkstation, out pinBatchId);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<PinBatchCardDetailsResult> GetPinBatchCardDetails(long pinBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinBatchCardDetails(pinBatchId, languageId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="branchId"></param>
        /// <param name="pinMailerReprintStatusId"></param>
        /// <param name="languageId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<PinMailerReprintResult> SearchPinMailerReprint(int? issuerId, int? branchId, int? pinMailerReprintStatusId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.SearchPinMailerReprint(issuerId, branchId, pinMailerReprintStatusId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Pin Batch History for PDF Report
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<PinBatchHistoryResult> GetLoadBatchHistory(long pinBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinBatchHistory(pinBatchId, languageId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Pin Batch Cards for PDF Report
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<PinBatchCardsResult> GetLoadBatchCards(long pinBatchId, long auditUserId, string auditWorkstation)
        {
            return _pinDAL.GetPinBatchCards(pinBatchId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="languageId"></param>
        /// <param name="username"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public byte[] GeneratePinBatchReport(long pinBatchId, int languageId, string username, long auditUserId, string auditWorkStation)
        {
            return _reports.GeneratePinBatchReport(pinBatchId, languageId, username, auditUserId, auditWorkStation);
        }
    }
}