using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.PINManagement.objects;
using System.Linq;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.DataAccess;
using Veneka.Indigo.Common.Models.IssuerManagement;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.PINManagement.dal
{
    internal class PINManagementDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

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
        internal List<PinBatchResult> GetPinBatchesForUser(int? issuerId, string pinBatchReference, int? pinBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                               int? pinBatchTypeId, DateTime? startDate, DateTime? endDate, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString)) // var context = new IssuerManagementDataAccess();
            {
         
                ObjectResult<PinBatchResult> results = context.usp_get_pin_batches_for_user(issuerId, pinBatchReference, pinBatchStatusId, branchId, cardIssueMethodId, pinBatchTypeId, startDate,
                                                                                                    endDate, langaugeId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        internal List<PinRequestList> GetPinRequestsForUser(int? issuerId, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
           
            var context = new IssuerManagementDataAccess();
            List<PinRequestList> results = context.usp_get_pinrequests(issuerId, pageIndex, rowsPerPage);

                return results;
            
        }
        //GetPinRequestsForUserForStatus

        internal List<PinRequestList> GetPinRequestsForUserForStatus(int? issuerId, string request_status, string reissue_approval_stage, string request_type, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            List<PinRequestList> results = context.usp_get_pinrequests_for_status(issuerId, request_status, reissue_approval_stage, request_type, pageIndex, rowsPerPage);

            return results;

        }

        internal List<PinRequestList> SearchForPinReIssue(int? issuerId, int? BranchId, int? ProductId, string ProductBin, string lastdigits, string accountnumber,
                                                                    string pinrequestref, int pageIndex, int? rowsPerPage, long auditUserId, string auditWorkstation)
        {
            var context = new IssuerManagementDataAccess();
            List<PinRequestList> results = context.usp_search_pin_for_reissue(issuerId, BranchId, ProductId, ProductBin, lastdigits, accountnumber,
                                                                    pinrequestref, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

            return results;
        }

        internal List<TerminalCardData> GetPinBlockForUser(int? issuerId, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            List<TerminalCardData> results = context.usp_get_pinblock(issuerId, pageIndex, rowsPerPage);
            return results;
        }
        //GetPinBlockForUserForStatus

        internal List<TerminalCardData> GetPinBlockForUserForStatus(int? issuerId, string request_status, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            List<TerminalCardData> results = context.usp_get_pinblock_for_status(issuerId, request_status, pageIndex, rowsPerPage);

            return results;

        }

        //GetPinBatchForUserForStatus
        internal List<TerminalCardData> GetPinBatchForUserForStatus(int? issuerId, string request_status, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            List<TerminalCardData> results = context.usp_get_pinbatch_for_status(issuerId, request_status, pageIndex, rowsPerPage);

            return results;

        }

        //GetPinMailerBatchHistory
        internal List<TerminalCardData> GetPinMailerBatchHistory(int pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            List<TerminalCardData> results = context.usp_get_pin_mailer_batch_history(pin_request_id,langaugeId,auditUserId,auditWorkstation);

            return results;

        }

        internal List<TerminalCardData> GetPinMailerBatchCards(int pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            List<TerminalCardData> results = context.usp_get_pin_mailer_batch_cards(pin_request_id, langaugeId, auditUserId, auditWorkstation);

            return results;

        }


        internal PinRequestViewDetails GetPinRequestViewDetails(long pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            PinRequestViewDetails results = context.usp_get_pin_request_object(pin_request_id);

            return results;

        }

        //GetPinBlockViewDetails
        internal TerminalCardData GetPinBlockViewDetails(long pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            TerminalCardData results = context.usp_get_pin_block_object(pin_request_id);

            return results;

        }
        //GetPinBatchViewDetails
        internal TerminalCardData GetPinBatchViewDetails(long pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            TerminalCardData results = context.usp_get_pin_batch_object(pin_request_id);

            return results;

        }

        internal TerminalCardData GetTerminalCardData(string pan_product_bin, string pan_last_four, string expiry_date, int langaugeId, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            TerminalCardData results = context.usp_get_pin_mailer_link_details(pan_product_bin,pan_last_four,expiry_date);
           
            return results;

        }

        internal RestWebservicesHandler GetRestParams(int issuer_id, string webservice_type, int langaugeId, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            RestWebservicesHandler results = context.usp_get_webservice_params(issuer_id, webservice_type, langaugeId, auditUserId, auditWorkstation);

            return results;

        }

        internal ZMKZPKDetails GetZoneKeys(int? issuer_id, int langaugeId, long auditUserId, string auditWorkstation)
        {

            var context = new IssuerManagementDataAccess();
            ZMKZPKDetails results = context.usp_get_zone_key(issuer_id, auditUserId, auditWorkstation);

            return results;

        }


        //GetPinRequestViewDetails
        /// <summary>
        /// Returns the pin batch details based on the pin batch.
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal PinBatchResult GetPinBatch(long pinBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PinBatchResult> results = context.usp_get_pin_batch(pinBatchId, languageId, auditUserId, auditWorkstation);

                return results.First();
            }
        }

        /// <summary>
        /// Move a pin batch from one status to the next.
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="statusNote"></param>
        /// <param name="pinBatchStatusId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="pinBatchResult"></param>
        /// <returns></returns>
        internal SystemResponseCode UpdatePinBatchStatus(long pinBatchId, int pinBatchStatusId, int flowDistBatchStatusesId, string statusNote, int languageId, long auditUserId, string auditWorkstation, out PinBatchResult pinBatchResult)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PinBatchResult> results = context.usp_pin_batch_status_change(pinBatchId, statusNote, pinBatchStatusId, flowDistBatchStatusesId, languageId, auditUserId, auditWorkstation, ResultCode);

                pinBatchResult = results.First();
            }

            return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="pinBatchStatusesId"></param>
        /// <param name="notes"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal SystemResponseCode PinBatchRejectStatus(long pinBatchId, int pinBatchStatusesId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinBatchResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PinBatchResult> results = context.usp_pin_batch_status_reject(pinBatchId, pinBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, ResultCode);

                result = results.First();

                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }

        /// <summary>
        /// Request for a card to have it's pin mailer reprinted
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="notes"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal SystemResponseCode PinMailerReprintRequest(long cardId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_pin_mailer_reprint_request(cardId, notes, auditUserId, auditWorkstation, ResultCode);

                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }

        /// <summary>
        /// Approve the request to reprint the pin mailer
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="notes"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal SystemResponseCode PinMailerReprintApprove(long cardId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_pin_mailer_reprint_approve(cardId, notes, auditUserId, auditWorkstation, ResultCode);

                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }

        /// <summary>
        /// Reject the request to reprint the pin mailer
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="notes"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal SystemResponseCode PinMailerReprintReject(long cardId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_pin_mailer_reprint_reject(cardId, notes, auditUserId, auditWorkstation, ResultCode);

                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }

        /// <summary>
        /// Search: Pin Mailer Reprint
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
        internal List<PinMailerReprintResult> SearchPinMailerReprint(int? issuerId, int? branchId, int? pinMailerReprintStatusId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PinMailerReprintResult> results = context.usp_search_pin_mailer_reprint(issuerId, branchId, null, pinMailerReprintStatusId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// Fetch list of cards which have ad pin mailer reprinting requests issued against them
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="branchId"></param>
        /// <param name="languageId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<PinMailerReprintRequestResult> PinMailerReprintList(int? issuerId, int? branchId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PinMailerReprintRequestResult> results = context.usp_get_pin_mailer_reprint_requests(issuerId, branchId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// Creates a pin batch and links the cards to the batch for all reprint card requests.
        /// </summary>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="issuerId"></param>
        /// <param name="branchId"></param>
        /// <param name="productId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="pinBatchId"></param>
        /// <returns></returns>
        internal SystemResponseCode PinMailerReprintBatchCreate(int? cardIssueMethodId, int? issuerId, int? branchId, int? productId, long auditUserId, string auditWorkstation, out int pinBatchId)
        {
            ObjectParameter cardsinBatchResult = new ObjectParameter("cards_in_batch", typeof(int));
            ObjectParameter pinBatchIdResult = new ObjectParameter("pin_batch_id", typeof(int));
            ObjectParameter pinBatchRefResult = new ObjectParameter("pin_batch_ref", typeof(string));
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_request_pin_mailer_reprints(cardIssueMethodId, issuerId, branchId, productId, auditUserId, auditWorkstation, cardsinBatchResult, pinBatchIdResult, pinBatchRefResult, ResultCode);

                pinBatchId = 0;// int.Parse(pinBatchIdResult.ToString());
                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<PinBatchCardDetailsResult> GetPinBatchCardDetails(long pinBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PinBatchCardDetailsResult> results = context.usp_get_pin_batch_card_details(pinBatchId, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// Return a list of cards linked to the specified pin batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal List<PinBatchCardsResult> GetPinBatchCards(long pinBatchId, long auditUserId, string auditWorkStation)
        {
            List<PinBatchCardsResult> rtnValue = new List<PinBatchCardsResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PinBatchCardsResult> results = context.usp_get_pin_batch_cards(pinBatchId, auditUserId, auditWorkStation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Fetch a pin batch status history.
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal List<PinBatchHistoryResult> GetPinBatchHistory(long pinBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            List<PinBatchHistoryResult> rtnValue = new List<PinBatchHistoryResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PinBatchHistoryResult> results = context.usp_get_pin_batch_history(pinBatchId, languageId, auditUserId, auditWorkStation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

    }
}