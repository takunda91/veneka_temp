using System;
using System.Collections.Generic;
using System.Text;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common;
using Veneka.Indigo.CardManagement.Reports;
using Veneka.Indigo.Common.Language;

namespace Veneka.Indigo.CardManagement
{
    public class DistBatchManagementService
    {
        private readonly CardManagementDAL _cardDAL = new CardManagementDAL();
        private readonly DistBatchManagementDAL _distBatchDAL = new DistBatchManagementDAL();
        private readonly DistBatchReports _distBatchReports = new DistBatchReports();
        private readonly ResponseTranslator _translator = new ResponseTranslator();

        #region PUBLIC METHODS

        /// <summary>
        /// Create a new distribution batch
        /// </summary>
        /// <param name="branchId">Where this distribution batch is to be sent.</param>
        /// <param name="batchCardSize">The number of cards the distribution batch must have.</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public string CreateDistributionBatch(int issuerId, int fromBranchId, int toBranchId, int cardIssueMethodId,
                                                            int productId, int batchCardSize,
                                                            int createBatchOption, string startRef, string endRef, long? fromdistbatchid,
                                                            long auditUserId, string auditWorkstation, out string dist_batch_ref, out int dist_batch_id)
        {
            //if (batchCardSize < 1 && String.IsNullOrWhiteSpace(startRef) && String.IsNullOrWhiteSpace(endRef))
            //{
            //    throw new ArgumentException("Card size must be greater than 0.");
            //}
            //else if (String.IsNullOrWhiteSpace(startRef) || String.IsNullOrWhiteSpace(endRef))
            //{
            //    throw new ArgumentException("Start Refenrene and End ReFerence must be populated.");
            //}

            return DecodeDBResponse(_distBatchDAL.CreateDistributionBatch(issuerId, fromBranchId, toBranchId, cardIssueMethodId,
                                                                            productId, batchCardSize, createBatchOption,
                                                                            startRef, endRef, fromdistbatchid, auditUserId, auditWorkstation, out dist_batch_ref, out dist_batch_id));
        }

        public string GetCentralBatchReferenceNumbers(int issuerId, int fromBranchId, int toBranchId, int cardIssueMethodId,
                                                           int productId, out string refFrom, out string refTo)
        {
            return DecodeDBResponse(_distBatchDAL.GetCentralBatchReferenceNumbers(issuerId, fromBranchId, toBranchId, cardIssueMethodId,
                                                                            productId, out refFrom, out refTo));
        }

        /// <summary>
        /// Create a new card stock order
        /// </summary>       
        /// <returns></returns>
        public bool CreateCardStockOrder(int issuerId, int branchId, int productId, int languageId, int cardIssuerMethodId, int cardPriorityId, int batchCardSize, long auditUserId, string auditWorkstation, out CardRequestBatchResponse cardReqResponse, out string responseMessage)
        {
            var response = _distBatchDAL.CreateCardStockOrder(issuerId, branchId, productId, cardIssuerMethodId, cardPriorityId, batchCardSize, auditUserId, auditWorkstation, out cardReqResponse);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (batchCardSize < 1)
            {
                throw new ArgumentException("Card size must be greater than 0.");
            }
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }
            return false;
        }

        #region Classic Card
        /// <summary>
        /// Create distribution batch based on the request for cards input parameter.
        /// </summary>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="branchId"></param>
        /// <param name="productId"></param>
        /// <param name="cardPriorityId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="cardReqResponse"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public bool CreateCardRequestBatch(int cardIssueMethodId, int issuerId, int? branchId, int productId, int cardPriorityId, int languageId,
                                            long auditUserId, string auditWorkstation, out CardRequestBatchResponse cardReqResponse, out string responseMessage)
        {
            var response = _distBatchDAL.CreateCardRequestBatch(cardIssueMethodId, issuerId, branchId, productId, cardPriorityId, auditUserId, auditWorkstation, out cardReqResponse);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool CreateHybridRequestBatch(int cardIssueMethodId, int issuerId, int? branchId, int productId, int cardPriorityId, int languageId,
                                           long auditUserId, string auditWorkstation, out HybridRequestBatchResponse ReqResponse, out string responseMessage)
        {
            var response = _distBatchDAL.CreateHybridRequestBatch(cardIssueMethodId, issuerId, branchId, productId, cardPriorityId, auditUserId, auditWorkstation, out ReqResponse);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }
        #endregion

        /// <summary>
        /// Returns a list of distribution batches for a user. dist batch status and dates may be null to not do a filter on them.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="distBatchStatus">Value may be NULL</param>
        /// <param name="startDate">Value may be NULL</param>
        /// <param name="endDate">Value may be NULL</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<DistBatchResult> GetDistBatchesForUser(long userId, int? issuerId, string distBatchReference, int? distBatchStatusId, int? flowdistBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                                int? distBatchTypeId, DateTime? startDate, DateTime? endDate, bool? includeOriginBranch, int languageId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.GetDistBatchesForUser(userId, issuerId, distBatchReference, distBatchStatusId, flowdistBatchStatusId, branchId, cardIssueMethodId, distBatchTypeId, startDate, endDate, includeOriginBranch, languageId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);
        }
        public List<DistStatusResult> GetDistBatchesForStatus(long userId, int? issuerId, string distBatchReference, int? distBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                               int? distBatchTypeId, DateTime? startDate, DateTime? endDate, bool cardCountForRedist, int languageId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.GetDistBatchesForStatus(userId, issuerId, distBatchReference, distBatchStatusId, branchId, cardIssueMethodId, distBatchTypeId, startDate, endDate, cardCountForRedist, languageId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);
        }

        public int GetDistBatchCardCountForRedist(long distBatchId, int productId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.GetDistBatchCardCountForRedist(distBatchId, productId, languageId, auditUserId, auditWorkstation);
        }

        public List<card_request_result> GetCardRequestList(int issuerid, int? branchId, int languageId, int pageIndex, int rowsPerPage,
                                                                           long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.GetCardRequestList(issuerid, branchId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
        }

        public List<card_request_result> GetCardRequestList(int issuerid, int? branchId, int cardIssueMethodId, int languageId, int pageIndex, int rowsPerPage,
                                                                           long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.GetCardRequestList(issuerid, branchId, cardIssueMethodId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
        }

        public List<hybrid_request_result> GetHybridRequestList(int issuerid, int? branchId, int? productId, int? hybrid_request_statusId, string batch_refrence, int languageId, int pageIndex, int rowsPerPage,
                                                                       long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.GetHybridRequestList(issuerid, branchId, productId, hybrid_request_statusId, batch_refrence, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
        }


        public List<PrintBatchResult> GetPrintBatchesList(int issuerid, int? branchId, int? productId, string print_batch_refrence, int? print_statues_id, int card_issue_method_id, DateTimeOffset startdate, DateTimeOffset enddate, int languageId, int pageIndex, int rowsPerPage,
                                                                       long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.GetPrintBatchesList(issuerid, branchId, productId, print_batch_refrence, print_statues_id, card_issue_method_id, startdate, enddate, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
        }
        /// <summary>
        /// Retrieve the dist batch
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public DistBatchResult GetDistBatch(long distBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.GetDistBatch(distBatchId, languageId, auditUserId, auditWorkstation);
        }

        public List<DistBatchCardDetail> GetDistBatchCardDetails(long distBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.GetDistBatchCardDetails(distBatchId, languageId, auditUserId, auditWorkstation);
        }

        public List<BatchCardDetailsPagedResult> GetDistBatchCardDetailsPaged(long distBatchId, int languageId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.GetDistBatchCardDetailsPaged(distBatchId, languageId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);
        }

        public List<BatchCardInfo> GetBatchCardInfoPaged(long distBatchId, int languageId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            List<BatchCardInfo> rtn = new List<BatchCardInfo>();
            var cardDetails = _distBatchDAL.GetDistBatchCardDetailsPaged(distBatchId, languageId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);

            foreach (var card in cardDetails)
            {
                rtn.Add(new BatchCardInfo
                {
                    CardId = card.card_id,
                    PAN = card.card_number,
                    CardReferenceNumber = card.card_request_reference,
                    CustomerAccountNumber = card.customer_account_number,
                    CustomerId = card.cms_id,
                    TOTAL_PAGES = card.TOTAL_PAGES.GetValueOrDefault(),
                    TOTAL_ROWS = card.TOTAL_ROWS.GetValueOrDefault()
                });
            }

            return rtn;
        }

        public bool DistBatchRejectStatus(long distBatchId, int distBatchStatusesId, string notes, int languageId, long auditUserId, string auditWorkstation, out DistBatchResult distBatchResult, out string responseMessage)
        {
            if (String.IsNullOrWhiteSpace(notes))
            {
                throw new ArgumentNullException("Notes is null or empty.");
            }

            var resultCode = _distBatchDAL.DistBatchRejectStatus(distBatchId, distBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, out distBatchResult);

            responseMessage = _translator.TranslateResponseCode(resultCode, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (resultCode == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool DistBatchChangeStatus(long distBatchId, int? distBatchStatusesId, int? flowDistBatchStatusesId, string notes, int languageId, long auditUserId, string auditWorkstation, bool autogenerateDistBatch, out DistBatchResult distBatchResult, out string responseMessage)
        {
            var resultCode = _distBatchDAL.DistBatchChangeStatus(distBatchId, distBatchStatusesId, flowDistBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, autogenerateDistBatch, out distBatchResult);

            responseMessage = _translator.TranslateResponseCode(resultCode, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (resultCode == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool DistBatchChangeStatusRenewal(long distBatchId, int? distBatchStatusesId, int? flowDistBatchStatusesId, string notes, int languageId, long auditUserId, string auditWorkstation, bool autogenerateDistBatch, out DistBatchResult distBatchResult, out string responseMessage)
        {
            var resultCode = _distBatchDAL.DistBatchChangeStatusRenewal(distBatchId, distBatchStatusesId, flowDistBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, autogenerateDistBatch, out distBatchResult);

            responseMessage = _translator.TranslateResponseCode(resultCode, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (resultCode == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool DistBatchCancel(long distBatchId, int? distBatchStatusesId, int distbatchtypeid, int? cardissuemethod, string notes, int languageId, long auditUserId, string auditWorkstation, out string responseMessage, out DistBatchResult result)
        {
            var resultCode = _distBatchDAL.DistBatchCancel(distBatchId, distBatchStatusesId, distbatchtypeid, cardissuemethod, notes, languageId, auditUserId, auditWorkstation, out result);

            responseMessage = _translator.TranslateResponseCode(resultCode, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (resultCode == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public DistBatchResult RejectProductionBatch(long distBatchId, string notes, List<RejectCardInfo> rejectedCards, int languageId, long auditUserId, string auditWorkstation, out string resultMessage)
        {
            if (String.IsNullOrWhiteSpace(notes))
            {
                throw new ArgumentNullException("Notes is null or empty.");
            }

            DBResponseMessage resultCode;
            var result = _distBatchDAL.RejectProductionBatch(distBatchId, notes, rejectedCards, languageId, auditUserId, auditWorkstation, out resultCode);

            resultMessage = DecodeDBResponse(resultCode);

            return result;
        }

        /// <summary>
        /// Return a list of cards linked to the specified load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <returns></returns>
        public List<DistCardResult> GetDistBatchCards(long distBatchId, long auditUserId, string auditWorkStation)
        {
            return _distBatchDAL.GetDistBatchCards(distBatchId, auditUserId, auditWorkStation);
        }

        /// <summary>
        /// Fetch a load batch status history.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public List<DistBatchHistoryResult> GetDistBatchHistory(long distBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            return _distBatchDAL.GetDistBatchHistory(distBatchId, languageId, auditUserId, auditWorkStation);
        }

        public DatabaseResponse GetDistributionBatch(string batchReference, int issuerID)
        {
            //try
            //{
            //    DistributionBatch response = distBatchDAL.GetDistributionBatch(batchReference, issuerID);

            //    if (response == null)
            //    {
            //        return new DatabaseResponse(DBResponseMessage.NO_RECORDS_RETURNED, "");
            //    }
            //    else
            //    {
            //        return new DatabaseResponse(DBResponseMessage.SUCCESS, "", response);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return new DatabaseResponse(ex);
            //}

            return null;
        }

        public DatabaseResponse GetDistributionBatches(DistributionBatchStatus batchStatus, int branchId, int issuerId,
                                                       DateTime dateFrom, DateTime dateTo)
        {
            //            List<DistributionBatch> response = null;
            //            try
            //            {
            //                //       if (dateFrom.Year == 1)
            //                //       {
            //                //           //date parameters not being used in search
            //                //           response = distBatchDAL.GetDistributionBatches(batchStatus, branch, issuerID);
            //                //       }
            //                //       else
            //                //       {
            //                //           //date parameters being used in the search
            //                response = null;// distBatchDAL.GetDistributionBatches(batchStatus, branchId, issuerId, dateFrom, dateTo);
            ////}
            //                if (response == null)
            //                {
            //                    return new DatabaseResponse(DBResponseMessage.NO_RECORDS_RETURNED, "");
            //                }
            //                else
            //                {
            //                    return new DatabaseResponse(DBResponseMessage.SUCCESS, "", response);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                return new DatabaseResponse(ex);
            //            }

            return null;
        }

        ///// <summary>
        ///// Creates a distribution batch, if a populated card list is passed it will use those cards for the distribution batch.
        ///// Otherwise a random selection of available cards will be used.
        ///// </summary>
        ///// <param name="batch"></param>
        ///// <param name="username"></param>
        ///// <param name="cardList">If populated with card numbers they will be used to create distribution batch.</param>
        ///// <returns></returns>
        //public DatabaseResponse CreateDistributionBatch(DistributionBatch batch, string username, List<String> cardList)
        //{
        //    string batchReference = CreateBatchReference(batch.IssuerID, batch.BranchCode, batch.NumberOfCards);
        //    string response;

        //    try
        //    {
        //        List<string> cardNumbers = new List<string>();

        //        //If no list of cards provided go grab some available cards, else use the list provided.
        //        if (cardList == null || cardList.Count == 0) 
        //        {
        //            //RL --- ask ETM to show me a better way of reserving cards in DB
        //            //see if there are enough cards to allocate to batch
        //            cardNumbers = cardDAL.GetLoadCardsToDistribute("", batch.NumberOfCards, batch.IssuerID);
        //        }
        //        else
        //        {
        //            cardNumbers = cardList;
        //        }

        //        if (cardNumbers.Count < batch.NumberOfCards)
        //        {
        //            //not enough cards to allocate to batch
        //            return new DatabaseResponse(DBResponseMessage.FAILURE, "Not enough available cards to create batch");
        //        }

        //        //reserve the cards
        //        //TODO RAB Method is looking for load batch reference but we dont have it available in this method.
        //        //I've commented out the check in the stored proc, but this needs to be looked at.
        //        //Seems the auto approve was not well though out.
        //        response = cardDAL.ReserveLoadCardsForDistBatch(cardNumbers, "",
        //                                                        LoadCardStatus.AVAILABLE.ToString(),
        //                                                        LoadCardStatus.ALLOCATED.ToString());

        //        if (!csGeneral.ValidateDBResponse(response).Equals(DBResponseMessage.SUCCESS))
        //        {
        //            //reserving cards failed - cards had been allocated to another batch
        //            return new DatabaseResponse(response);
        //        }

        //        //cards successfully reserved --- create batch
        //        response = distBatchDAL.CreateNewDistributionBatch(batch, cardNumbers, batchReference, username,
        //                                                           DistributionBatchStatus.CREATED.ToString(),
        //                                                           IssueCardStatus.ALLOCATED_TO_BRANCH.ToString());

        //        if (!csGeneral.ValidateDBResponse(response).Equals(DBResponseMessage.SUCCESS))
        //        {
        //            //failure in creating the batch
        //            //RL -- insert code to release the cards
        //            return new DatabaseResponse(DBResponseMessage.FAILURE,
        //                                        "Could not create batch for some reason, this error message will change");
        //        }

        //        return new DatabaseResponse(response, batchReference, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DatabaseResponse(ex);
        //    }
        //}

        //public DatabaseResponse UpdateDistributionBatchStatus(DistributionBatch distBatch, string userName,
        //                                                      string newBatchStatus)
        //{
        //    DistributionBatchStatus enumBatchStatus = csGeneral.GetDistBatchStatus(newBatchStatus);

        //    if (enumBatchStatus.Equals(DistributionBatchStatus.INVALID))
        //    {
        //        //invalid value being sent in strNewBatchStatus (this should neve happen)
        //        return new DatabaseResponse(new Exception("SYSTEM ERROR: INVALID VALUE IN BATCH STATUS"));
        //    }

        //    string newCardStatus = "";
        //    string newLoadCardStatus = "";

        //    switch (enumBatchStatus)
        //    {
        //        case DistributionBatchStatus.APPROVED:
        //            //do not need to update card statuses 
        //            newLoadCardStatus = LoadCardStatus.ALLOCATED.ToString();//
        //            break;

        //        case DistributionBatchStatus.DISPATCHED:
        //            //do not need to update card statuses 
        //            newLoadCardStatus = LoadCardStatus.ALLOCATED.ToString();
        //            break;

        //        case DistributionBatchStatus.RECEIVED_AT_BRANCH:
        //            newCardStatus = IssueCardStatus.RECEIVED_AT_BRANCH.ToString();
        //            newLoadCardStatus = LoadCardStatus.ALLOCATED.ToString();
        //            break;

        //        case DistributionBatchStatus.REJECTED_AT_BRANCH:
        //            newCardStatus = IssueCardStatus.REJECTED.ToString();
        //            newLoadCardStatus = LoadCardStatus.REJECTED.ToString();
        //            break;

        //        case DistributionBatchStatus.REJECT_AND_REISSUE:
        //            newCardStatus = IssueCardStatus.REJECTED.ToString();
        //            newLoadCardStatus = LoadCardStatus.AVAILABLE.ToString();
        //            break;

        //        case DistributionBatchStatus.REJECT_AND_CANCEL:
        //            newCardStatus = IssueCardStatus.CANCELLED.ToString();

        //            newLoadCardStatus = LoadCardStatus.CANCELLED.ToString();

        //            break;
        //    }

        //    try
        //    {
        //        return
        //            new DatabaseResponse(_distBatchDAL.UpdateDistributionBatchStatus(distBatch, userName, newBatchStatus,
        //                                                                            newCardStatus, newLoadCardStatus));
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DatabaseResponse(ex);
        //    }
        //}

        #endregion

        #region PRIVATE METHODS

        private string DecodeDBResponse(DBResponseMessage dbResponse)
        {
            switch (dbResponse)
            {
                case DBResponseMessage.SUCCESS:
                    return "";
                case DBResponseMessage.INCORRECT_STATUS:
                    return "Distribution batch is not in the correct status, please refresh.";
                case DBResponseMessage.CARD_ALREADY_ISSUED:
                    return dbResponse.ToString();
                case DBResponseMessage.INCORRECT_BRANCH:
                    return dbResponse.ToString();
                case DBResponseMessage.NO_RECORDS_RETURNED:
                    return dbResponse.ToString();
                case DBResponseMessage.DUPLICATE_RECORD:
                    return "Duplicate record found.";
                case DBResponseMessage.INSUFFICIENT_AVAILABLE_CARDS:
                    return "There are insufficient cards available to create the distribution batch.";
                case DBResponseMessage.SPROC_ERROR:
                    return dbResponse.ToString();
                case DBResponseMessage.SYSTEM_ERROR:
                    return dbResponse.ToString();
                case DBResponseMessage.FAILURE:
                    return dbResponse.ToString();
                default:
                    return dbResponse.ToString();
            }
        }

        private string CreateBatchReference(int issuerID, string branchCode, int numCards)
        {
            var batchReference = new StringBuilder();

            batchReference.Append(issuerID);
            batchReference.Append("-");
            batchReference.Append(branchCode);
            batchReference.Append("-");
            batchReference.Append(DateTime.Now.ToString("yyMMddHHmmss"));
            //EM (20131117) - Changed format from yyMMddhhmmss to yyMMddHHmmss
            return batchReference.ToString();
        }

        public bool CheckIfCMSUploadCenterOperator(long distBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.CheckIfCMSUploadCenterOperator(distBatchId, languageId, auditUserId, auditWorkstation);
        }

        public bool CheckIfCMSUploadRenewalCards(long distBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _distBatchDAL.CheckIfCMSUploadRenewalCards(distBatchId, languageId, auditUserId, auditWorkstation);
        }

        #endregion

        #region TO DELETE

        /*
        public bool ApproveDistBatch(string batchReference, string branchRejectReason, string centreRejectReason, int issuerID, string username)
        {
            /* 
             * This method is now used for 2 scenarios
             * 1) Centre Manager Approves a newly created batch
             * 2) Centre Manager returns the cards from a batch that has been "Rejected at Branch" to the pool
             * 
             * For both of these situations the cards are returned to the pool for re-issue as part of another batch
             */
        /*
            DistributionBatch batch = distBatchDAL.GetDistributionBatch(batchReference, issuerID);

            //case 1: Centre Manager approves a new batch
            if (batch.BatchStatus == DistributionBatchStatus.CREATED)
            {
                return distBatchDAL.ApproveDistBatch(batchReference, issuerID, username);
            }

            //case 2: batch has been rejected from branch, but cards can be returned to stock
            else if (batch.BatchStatus == DistributionBatchStatus.REJECTED_AT_BRANCH)
            {
                List<string> listCards = batch.Cards;
                return distBatchDAL.RejectsDistBatch(batchReference, branchRejectReason, centreRejectReason, listCards, issuerID, username);
            }
            else
            {
                throw new Veneka.Indigo.CardManagement.exceptions.xCardManagementException("Action not allowed for batch with status : " + batch.BatchStatus);
            }

        }

        public bool RejectsDistBatch(string batchReference, string branchRejectReason, string centreRejectReason, int issuerID, string username)
        {
            //get latest batch from DB
            DistributionBatch batch = distBatchDAL.GetDistributionBatch(batchReference, issuerID);

            //case 1: Cen Man rejecting newly created batch
            if (batch.BatchStatus == DistributionBatchStatus.CREATED)
            {

                List<string> listCards = batch.Cards;
                return distBatchDAL.RejectsDistBatch(batchReference, branchRejectReason, centreRejectReason, listCards, issuerID, username);
            }

            //case 2: Cen Man rejecting batch that has been rejected by Branch Manager ie cards to become invalid, not to be re-issued
            else if (batch.BatchStatus == DistributionBatchStatus.REJECTED_AT_BRANCH)
            {

                return distBatchDAL.RejectDistBatchAndDestroyCards(batchReference, branchRejectReason, centreRejectReason, issuerID, username);
            }
            else
            {

                throw new Veneka.Indigo.CardManagement.exceptions.xCardManagementException("Action not allowed for batch with status : " + batch.BatchStatus);
            }

        }

        public bool DispatchDistBatch(string batchReference, int issuerID, string username)
        {

            DistributionBatch batch = distBatchDAL.GetDistributionBatch(batchReference, issuerID);

            if (batch.BatchStatus == DistributionBatchStatus.APPROVED)
            {
                return distBatchDAL.DispatchDistBatch(batchReference, issuerID, username);
            }
            else
            {
                throw new Veneka.Indigo.CardManagement.exceptions.xCardManagementException("Action not allowed for batch with status : " + batch.BatchStatus);
            }

        }
        public bool RejectsDistpatchedDistBatch(string batchReference, string branchRejReason, int issuerID, string username)
        {

            DistributionBatch batch = distBatchDAL.GetDistributionCardBatch(batchReference, issuerID);

            if (batch.BatchStatus == DistributionBatchStatus.DISPATCHED || batch.BatchStatus == DistributionBatchStatus.APPROVED)
            {
                return distBatchDAL.RejectsDistpatchedDistBatch(batchReference, branchRejReason, issuerID, username);
            }
            else
            {
                throw new Veneka.Indigo.CardManagement.exceptions.xCardManagementException("Action not allowed for batch with status : " + batch.BatchStatus);
            }

        }

        public bool ApproveDistBatch(string batchReference, int issuerID)
        {
            return true;
        }
        public bool ReceiveDistBatch(string batchReference, int issuerID, string username)
        {

            DistributionBatch batch = distBatchDAL.GetDistributionCardBatch(batchReference, issuerID);

            if (batch.BatchStatus == DistributionBatchStatus.APPROVED || batch.BatchStatus == DistributionBatchStatus.DISPATCHED)
            {
                return distBatchDAL.ReceiveDistBatch(batchReference, issuerID, username);
            }
            else
            {
                throw new Veneka.Indigo.CardManagement.exceptions.xCardManagementException("Action not allowed for batch with status : " + batch.BatchStatus);
            }

        }
        */

        #endregion

        #region PDF Reports

        /// <summary>
        /// Generate card list PDF report for distribution batch.
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public byte[] GenerateDistBatchReport(long distBatchId, int languageId, string username, long auditUserId, string auditWorkStation)
        {
            return _distBatchReports.GenerateDistBatchReport(distBatchId, languageId, username, auditUserId, auditWorkStation);
        }

        #endregion
    }
}