using System;
using System.Collections.Generic;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.Common.Models;
using Common.Logging;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Integration;
using System.ComponentModel.Composition;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Common;
using System.Collections;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.COMS.Core;
using IndigoCardIssuanceService.COMS;
using Veneka.Indigo.Integration.Config;
using System.Security.Cryptography;
using System.Linq;

namespace IndigoCardIssuanceService.bll
{
    public class DistributionBatchController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DistributionBatchController));
        private readonly DistBatchManagementService _distbatchMan = new DistBatchManagementService();
        private readonly SessionManager _sessionManager = SessionManager.GetInstance();
        //private static readonly IntegrationController _integration = new IntegrationController();
        private readonly CardMangementService _cardManService = new CardMangementService();
        private readonly PrintBatchManagementService _printservice = new PrintBatchManagementService();


        private static readonly object _lockObject = new object();
        private static readonly object _lockPinPrinting = new object();
        private static readonly object _lockCardProduction = new object();
        private static readonly object _lockDistBatchCreate = new object();
        private static readonly object _lockCardReqCreate = new object();

        private static readonly object _lockhybridReqCreate = new object();


        #region EXPOSED METHODS

        /// <summary>
        /// Create a new distribution batch
        /// </summary>
        /// <param name="branchId">Where this distribution batch is to be sent.</param>
        /// <param name="batchCardSize">The number of cards the distribution batch must have.</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<int> CreateDistributionBatch(int issuerId, int fromBranchId, int toBranchId, int cardIssueMethodId,
                                                            int productId, int batchCardSize,
                                                            int createBatchOption, string startRef, string endRef, long? fromdistbatchid,
                                                            long auditUserId, string auditWorkstation, out string dist_batch_ref)
        {
            dist_batch_ref = "0";
            int dist_batch_id = 0;
            try
            {

                lock (_lockDistBatchCreate)
                {
                    string response = _distbatchMan.CreateDistributionBatch(issuerId, fromBranchId, toBranchId, cardIssueMethodId,
                                                                             productId, batchCardSize,
                                                                             createBatchOption, startRef, endRef, fromdistbatchid,
                                                                             auditUserId, auditWorkstation, out dist_batch_ref, out dist_batch_id);

                    if (String.IsNullOrWhiteSpace(response))
                    {
                        return new Response<int>(dist_batch_id, ResponseType.SUCCESSFUL,
                                                ResponseType.SUCCESSFUL.ToString(),
                                                "");
                    }
                    else
                    {
                        return new Response<int>(dist_batch_id, ResponseType.UNSUCCESSFUL,
                                                response,
                                                response);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(dist_batch_id, ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<int> GetCentralBatchReferenceNumbers(int issuerId, int fromBranchId, int toBranchId, int cardIssueMethodId,
                                                           int productId, out string refFrom, out string refTo)
        {
            refFrom = string.Empty;
            refTo = string.Empty;

            try
            {
                lock (_lockDistBatchCreate)
                {
                    string response = _distbatchMan.GetCentralBatchReferenceNumbers(issuerId, fromBranchId, toBranchId, cardIssueMethodId,
                                                                             productId, out refFrom, out refTo);

                    if (String.IsNullOrWhiteSpace(response))
                    {
                        return new Response<int>(0, ResponseType.SUCCESSFUL,
                                                ResponseType.SUCCESSFUL.ToString(),
                                                "");
                    }
                    else
                    {
                        return new Response<int>(-1, ResponseType.UNSUCCESSFUL,
                                                response,
                                                response);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(-1, ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        //internal BaseResponse CreateDistributionBatch(int fromBranchId, int toBranchId, int productId, int? subProductId, string startRef, string endRef, long auditUserId, string auditWorkstation)
        //{
        //    try
        //    {
        //        string response = _distbatchMan.CreateDistributionBatch(branchId, batchCardSize, auditUserId, auditWorkstation);

        //        if (String.IsNullOrWhiteSpace(response))
        //        {
        //            return new BaseResponse(ResponseType.SUCCESSFUL,
        //                                    ResponseType.SUCCESSFUL.ToString(),
        //                                    "");
        //        }
        //        else
        //        {
        //            return new BaseResponse(ResponseType.UNSUCCESSFUL,
        //                                    response,
        //                                    response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        return new BaseResponse(ResponseType.ERROR,
        //                                "An error occured during processing your request, please try again.",
        //                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
        //    }
        //}


        /// <summary>
        /// Create a new card stock order
        /// </summary>        
        /// <returns></returns>
        internal Response<CardRequestBatchResponse> CreateCardStockOrder(int issuerId, int branchId, int productId, int languageId, int cardIssuerMethodId, int cardPriorityId, int batchCardSize, long auditUserId, string auditWorkstation)
        {
            try
            {

                CardRequestBatchResponse cardRequestResp;
                string response;

                lock (_lockCardReqCreate)
                {
                    if (_distbatchMan.CreateCardStockOrder(issuerId, branchId, productId, languageId, cardIssuerMethodId, cardPriorityId, batchCardSize, auditUserId, auditWorkstation, out cardRequestResp, out response))
                    {
                        return new Response<CardRequestBatchResponse>(cardRequestResp, ResponseType.SUCCESSFUL, response, "");
                    }
                    else
                    {
                        return new Response<CardRequestBatchResponse>(null, ResponseType.UNSUCCESSFUL, response, "");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<CardRequestBatchResponse>(null,
                                                             ResponseType.ERROR,
                                                             "An error occured during processing your request, please try again.",
                                                             log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
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
        /// <returns></returns>
        internal Response<CardRequestBatchResponse> CreateCardRequestBatch(int cardIssueMethodId, int issuerId, int? branchId, int productId, int cardPriorityId, int languageId,
                                            long auditUserId, string auditWorkstation)
        {
            try
            {
                CardRequestBatchResponse cardRequestResp;
                string response;

                lock (_lockCardReqCreate)
                {
                    if (_distbatchMan.CreateCardRequestBatch(cardIssueMethodId, issuerId, branchId, productId, cardPriorityId, languageId,
                                                             auditUserId, auditWorkstation, out cardRequestResp, out response))
                    {
                        return new Response<CardRequestBatchResponse>(cardRequestResp, ResponseType.SUCCESSFUL, response, "");
                    }
                    else
                    {
                        return new Response<CardRequestBatchResponse>(null, ResponseType.UNSUCCESSFUL, response, "");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<CardRequestBatchResponse>(null,
                                                              ResponseType.ERROR,
                                                              "An error occured during processing your request, please try again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<HybridRequestBatchResponse> CreateHybridRequestBatch(int cardIssueMethodId, int issuerId, int? branchId, int productId, int cardPriorityId, int languageId,
                                          long auditUserId, string auditWorkstation)
        {
            try
            {
                HybridRequestBatchResponse RequestResp;
                string response;

                lock (_lockhybridReqCreate)
                {
                    if (_distbatchMan.CreateHybridRequestBatch(cardIssueMethodId, issuerId, branchId, productId, cardPriorityId, languageId,
                                                             auditUserId, auditWorkstation, out RequestResp, out response))
                    {
                        return new Response<HybridRequestBatchResponse>(RequestResp, ResponseType.SUCCESSFUL, response, "");
                    }
                    else
                    {
                        return new Response<HybridRequestBatchResponse>(null, ResponseType.UNSUCCESSFUL, response, "");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<HybridRequestBatchResponse>(null,
                                                              ResponseType.ERROR,
                                                              "An error occured during processing your request, please try again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
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
        internal Response<List<DistBatchResult>> GetDistBatchesForUser(long userId, int? issuerId, string distBatchReference, int? distBatchStatusId, int? flowdistBatchStatusId, int? branchId,
                                                                         int? cardIssueMethodId, int? distBatchTypeId, DateTime? startDate, DateTime? endDate, bool? includeOriginBranch, int languageId, int rowsPerPage, int pageIndex,
                                                                            long auditUserId, string auditWorkstation)
        {
            Response<List<DistBatchResult>> response;
            try
            {
                response = new Response<List<DistBatchResult>>(_distbatchMan.GetDistBatchesForUser(userId, issuerId, distBatchReference, distBatchStatusId, flowdistBatchStatusId, branchId, cardIssueMethodId,
                                                                                                   distBatchTypeId, startDate, endDate, includeOriginBranch, languageId, rowsPerPage, pageIndex,
                                                                                                   auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");

                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<DistBatchResult>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }
        /// <summary>
        /// Get Distbatches depending upon status with count
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="issuerId"></param>
        /// <param name="distBatchReference"></param>
        /// <param name="distBatchStatusId"></param>
        /// <param name="branchId"></param>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="distBatchTypeId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="languageId"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<DistStatusResult>> GetDistBatchesForStatus(long userId, int? issuerId, string distBatchReference, int? distBatchStatusId, int? branchId,
                                                                        int? cardIssueMethodId, int? distBatchTypeId, DateTime? startDate, DateTime? endDate, bool cardCountForRedist, int languageId, int rowsPerPage, int pageIndex,
                                                                           long auditUserId, string auditWorkstation)
        {
            Response<List<DistStatusResult>> response;
            try
            {
                response = new Response<List<DistStatusResult>>(_distbatchMan.GetDistBatchesForStatus(userId, issuerId, distBatchReference, distBatchStatusId, branchId, cardIssueMethodId,
                                                                                                   distBatchTypeId, startDate, endDate, cardCountForRedist, languageId, rowsPerPage, pageIndex,
                                                                                                   auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");

                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<DistStatusResult>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<int> GetDistBatchCardCountForRedist(long distBatchId, int productId, int languageId, long auditUserId, string auditWorkstation)
        {
            Response<int> response;

            try
            {
                response = new Response<int>(_distbatchMan.GetDistBatchCardCountForRedist(distBatchId, productId, languageId, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");

                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<int>(0,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<List<BatchCardInfo>> GetBatchCardInfoPaged(long distBatchId, int languageId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            Response<List<BatchCardInfo>> response;
            try
            {
                response = new Response<List<BatchCardInfo>>(_distbatchMan.GetBatchCardInfoPaged(distBatchId, languageId, rowsPerPage, pageIndex, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");

                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<BatchCardInfo>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<List<card_request_result>> GetCardRequestList(int issuerid, int? branchId, int languageId, int pageIndex, int rowsPerPage,
                                                                            long auditUserId, string auditWorkstation)
        {
            Response<List<card_request_result>> response;
            try
            {
                response = new Response<List<card_request_result>>(_distbatchMan.GetCardRequestList(issuerid, branchId, languageId, pageIndex, rowsPerPage,
                                                                                                   auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");

                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<card_request_result>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<List<card_request_result>> GetCardRequestList(int issuerid, int? branchId, int cardIssueMethodId, int languageId, int pageIndex, int rowsPerPage,
                                                                           long auditUserId, string auditWorkstation)
        {
            Response<List<card_request_result>> response;
            try
            {
                response = new Response<List<card_request_result>>(_distbatchMan.GetCardRequestList(issuerid, branchId, cardIssueMethodId, languageId, pageIndex, rowsPerPage,
                                                                                                   auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");

                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<card_request_result>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }


        internal Response<List<hybrid_request_result>> GetHybridRequestList(int issuerid, int? branchId, int? productId, int? hybrid_request_statusId, string batch_refrence, int languageId, int pageIndex, int rowsPerPage,
                                                                       long auditUserId, string auditWorkstation)
        {
            Response<List<hybrid_request_result>> response;
            try
            {
                response = new Response<List<hybrid_request_result>>(_distbatchMan.GetHybridRequestList(issuerid, branchId, productId, hybrid_request_statusId, batch_refrence, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation),
                ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");

                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<hybrid_request_result>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<bool> SpoilPrintBatch(long print_batch_id, int? new_print_batch_statuses_id, string notes, int languageId,
                                                                       long auditUserId, string auditWorkstation)
        {
            try
            {
                string responeMessage;
                return new Response<bool>(_printservice.SpoilPrintBatch(print_batch_id, new_print_batch_statuses_id, notes, languageId, auditUserId, auditWorkstation, out responeMessage),
                                                     ResponseType.SUCCESSFUL,
                                                     "",
                                                     "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }


        /// <summary>
        /// Retrieve the dist batch
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<DistBatchResult> GetDistBatch(long distBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<DistBatchResult>(_distbatchMan.GetDistBatch(distBatchId, languageId, auditUserId, auditWorkstation),
                                                     ResponseType.SUCCESSFUL,
                                                     "",
                                                     "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<DistBatchResult>(null,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<DistBatchResult> DistBatchRejectStatus(long distBatchId, int distBatchStatusesId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage;
                DistBatchResult distBatchResult;
                if (_distbatchMan.DistBatchRejectStatus(distBatchId, distBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, out distBatchResult, out resultMessage))
                {
                    return new Response<DistBatchResult>(distBatchResult,
                                                         ResponseType.SUCCESSFUL,
                                                         resultMessage,
                                                         resultMessage);
                }
                else
                {
                    return new Response<DistBatchResult>(null, ResponseType.UNSUCCESSFUL, resultMessage, resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<DistBatchResult>(null,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
        internal BaseResponse MultipleDistBatchChangeStatus(ArrayList distbatchIds, ArrayList distbatchStatusIds, ArrayList flowdisbatchstausIds, string notes, bool autogenerateDistBatch, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                for (int i = 0; i < distbatchIds.Count; i++)
                {
                    long distbatchId = 0;
                    int distbatchStatusId = 0, flowdisbatchstausId = 0;
                    try
                    {

                        long.TryParse(distbatchIds[i].ToString(), out distbatchId);
                        int.TryParse(distbatchStatusIds[i].ToString(), out distbatchStatusId);

                        int.TryParse(flowdisbatchstausIds[i].ToString(), out flowdisbatchstausId);

                        if (distbatchId > 0)
                        {
                            Response<DistBatchResult> result = DistBatchChangeStatus(distbatchId, distbatchStatusId, flowdisbatchstausId, notes, autogenerateDistBatch, languageId, auditUserId, auditWorkstation);
                            if (result != null)
                            {
                                log.Debug(string.Format("Status of  distbatchId {0}, distbatchstatusId {1}, distbatchflowstatusId {2} ,response: {3}", distbatchId, distbatchStatusId, flowdisbatchstausId, result.ResponseMessage));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Failed to Update distbatchId {0}, distbatchstatusId {1}, distbatchflowstatusId {2} ", distbatchId, distbatchStatusId, flowdisbatchstausId));
                        log.Error(ex);
                    }

                }
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                                     "Action Was Successful.",
                                                    "");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<DistBatchResult> DistBatchChangeStatusRenewal(long distBatchId, int distBatchStatusesId, int flowDistBatchStatusesId, string notes, bool autogenerateDistBatch, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage = String.Empty;

                //if the status is 10, check which status we should upload to CMS on
                log.Trace($"dist_batch_id: {distBatchId}, fromStatusId: {distBatchStatusesId}, toStatusId:{flowDistBatchStatusesId}");
                bool uploadToCMS = false;
                if (flowDistBatchStatusesId == 10)
                {
                    uploadToCMS = _distbatchMan.CheckIfCMSUploadRenewalCards(distBatchId, languageId, auditUserId, auditWorkstation);
                }

                return ChangeDistributionBatchStatus(distBatchId, distBatchStatusesId, flowDistBatchStatusesId, notes, autogenerateDistBatch, languageId, -2, "IndigoSystem", uploadToCMS, true);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<DistBatchResult>(null,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<DistBatchResult> DistBatchChangeStatus(long distBatchId, int distBatchStatusesId, int flowDistBatchStatusesId, string notes, bool autogenerateDistBatch, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {



                //if the status is 10|20, check which status we should upload to CMS on
                bool uploadToCMS = true;
                if (flowDistBatchStatusesId == 10 || flowDistBatchStatusesId == 20)
                {
                    bool centerOperatorActivation = _distbatchMan.CheckIfCMSUploadCenterOperator(distBatchId, languageId, auditUserId, auditWorkstation);
                    if (flowDistBatchStatusesId == 10 && centerOperatorActivation == true)
                    {
                        uploadToCMS = false;
                    }
                    if (flowDistBatchStatusesId == 20 && centerOperatorActivation == false)
                    {
                        uploadToCMS = false;
                    }
                }

                return ChangeDistributionBatchStatus(distBatchId, distBatchStatusesId, flowDistBatchStatusesId, notes, autogenerateDistBatch, languageId, auditUserId, auditWorkstation, uploadToCMS, false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<DistBatchResult>(null,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        private Response<DistBatchResult> ChangeDistributionBatchStatus(long distBatchId, int distBatchStatusesId, int flowDistBatchStatusesId, string notes,
            bool autogenerateDistBatch, int languageId, long auditUserId, string auditWorkstation, bool uploadToCMS, bool isRenewal)
        {
            bool isSuccessful = false;
            string resultMessage = String.Empty;
            log.Trace($"changeDistributionBatchStatus: uploadToCMS: {uploadToCMS}, isRenewal: {isRenewal}");
            //Determin what we need to do before changing the batches status
            switch (flowDistBatchStatusesId)
            {
                case 10:
                    if (uploadToCMS)
                    {
                        isSuccessful = CMSUpload(distBatchId, languageId, auditUserId, auditWorkstation, out resultMessage);
                    }
                    else
                    {
                        isSuccessful = true;
                    }
                    break;
                case 12:
                    isSuccessful = CardProduction(distBatchId, languageId, auditUserId, auditWorkstation, out resultMessage);
                    break;
                case 17:
                    isSuccessful = PrintPins(distBatchId, languageId, auditUserId, auditWorkstation, out resultMessage);
                    break;
                case 20:
                    if (uploadToCMS)
                    {
                        isSuccessful = CMSUpload(distBatchId, languageId, auditUserId, auditWorkstation, out resultMessage);
                    }
                    else
                    {
                        isSuccessful = true;
                    }
                    break;
                default:
                    isSuccessful = true;
                    break;
            }

            //If the previous was succesfull then update the batch
            if (isSuccessful)
            {
                DistBatchResult distBatchResult;
                log.Trace($"changeDistributionBatchStatus: isSuccessful: {isSuccessful}");
                bool statusChanged;
                if (!isRenewal)
                {
                    statusChanged = _distbatchMan.DistBatchChangeStatus(distBatchId, distBatchStatusesId, flowDistBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, autogenerateDistBatch, out distBatchResult, out resultMessage);
                }
                else
                {
                    statusChanged = _distbatchMan.DistBatchChangeStatusRenewal(distBatchId, distBatchStatusesId, flowDistBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, autogenerateDistBatch, out distBatchResult, out resultMessage);
                }
                if (statusChanged)
                {
                    return new Response<DistBatchResult>(distBatchResult,
                                                         ResponseType.SUCCESSFUL,
                                                         resultMessage,
                                                         resultMessage);
                }
                else
                {
                    return new Response<DistBatchResult>(null, ResponseType.UNSUCCESSFUL, resultMessage, resultMessage);
                }
            }
            else
            {
                return new Response<DistBatchResult>(null,
                                                     ResponseType.UNSUCCESSFUL,
                                                     resultMessage,
                                                     resultMessage);
            }
        }

        private bool CMSUpload(long distBatchId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            //Locking this process so that only one thread can run it at a time.
            //this is because we're generating unique card numbers and dont want to threads to try generate the same card numbers
            lock (_lockObject)
            {
                var cardObjects = FetchCardObjectsForDistBatch(distBatchId, languageId, auditUserId, auditWorkstation);
                IntegrationController _integration = IntegrationController.Instance;
                Veneka.Indigo.Integration.Config.IConfig config;
                Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;


                try
                {
                    _integration.CardManagementSystem(cardObjects[0].ProductId, InterfaceArea.PRODUCTION, out externalFields, out config);


                    InterfaceInfo interfaceInfo = new InterfaceInfo
                    {
                        Config = config,
                        InterfaceGuid = config.InterfaceGuid.ToString()

                    };


                    AuditInfo auditInfo = new AuditInfo
                    {
                        AuditUserId = auditUserId,
                        AuditWorkStation = auditWorkstation,
                        LanguageId = languageId
                    };


                    //integration.OnUploadCompleted -= onCmsUploadCompleted;
                    //if (!COMS.COMSController.ComsCore.OnUploadCompletedSubscribed)
                    //{
                    //    COMS.COMSController.ComsCore.OnUploadCompleted += onCmsUploadCompleted;
                    //    COMS.COMSController.ComsCore.OnUploadCompletedSubscribed = true;
                    //}

                    //Change sstatus to sent to cms

                    var resp = COMS.COMSController.ComsCore.UploadGeneratedCards(cardObjects, externalFields, interfaceInfo, auditInfo);

                    responseMessage = resp.ResponseMessage;

                    if (resp.ResponseCode == 0)
                        return true;

                    return false;

                    //return _integration.CardManagementSystem(cardObjects[0].ProductId, InterfaceArea.PRODUCTION, out externalFields, out config).UploadGeneratedCards(cardObjects, externalFields, config, languageId, auditUserId, auditWorkstation, out responseMessage);
                }
                catch (NotImplementedException nie)
                {
                    log.Warn("UploadGeneratedCards() method in CMS module not implemented.", nie);

                    responseMessage = "CMS module not implemented.";
                    return false;
                }
            }
        }

        private void onCmsUploadCompleted(object sender, Veneka.Indigo.COMS.Core.CardManagement.DistEventArgs e)
        {
            log.Trace(t => t("{0} onCmsUploadCompleted", e.DistBatchId));
            //success = 11
            //failed = 15

            log.Debug(d => d("{0} {1} {2}", e.DistBatchId, e.Success, e.ResponseMessage));

            //check the status and do the update
            DistBatchResult distBatchResult;
            string resultMessage = String.Empty;

            if (e.Success)
                if (_distbatchMan.DistBatchChangeStatus(e.DistBatchId, null, null, e.ResponseMessage, 0, -2, "IndigoSystem", true, out distBatchResult, out resultMessage))
                    log.Trace(t => t("{0} uploaded to cms successful.", e.DistBatchId));
                else
                    log.ErrorFormat("{0} {1}", e.DistBatchId, resultMessage);
            else
                if (_distbatchMan.DistBatchRejectStatus(e.DistBatchId, 15, e.ResponseMessage, 0, -2, "IndigoSystem", out distBatchResult, out resultMessage))
                log.Trace(t => t("{0} uploaded to cms unsuccessful.", e.DistBatchId));
            else
                log.ErrorFormat("{0} {1}", e.DistBatchId, resultMessage);
        }

        private bool PrintPins(long distBatchId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            //Only allow one batch to be printed at a time.
            lock (_lockPinPrinting)
            {
                var cardObjects = FetchCardObjectsForDistBatch(distBatchId, languageId, auditUserId, auditWorkstation);

                IntegrationController _integration = IntegrationController.Instance;
                Veneka.Indigo.Integration.Config.IConfig config;

                try
                {
                    _integration.HardwareSecurityModule(cardObjects[0].IssuerId, InterfaceArea.PRODUCTION, out config);

                    InterfaceInfo interfaceInfo = new InterfaceInfo
                    {
                        Config = config,
                        InterfaceGuid = ""
                    };

                    AuditInfo auditInfo = new AuditInfo
                    {
                        AuditUserId = auditUserId,
                        AuditWorkStation = auditWorkstation,
                        LanguageId = languageId
                    };


                    var response = COMSController.ComsCore.PrintPins(cardObjects, interfaceInfo, auditInfo);

                    responseMessage = response.ResponseMessage;

                    if (response.ResponseCode == 0)
                        return true;

                    return false;



                    //return _integration.HardwareSecurityModule(cardObjects[0].IssuerId, InterfaceArea.PRODUCTION, out config).PrintPins(ref cardObjects, config, languageId, auditUserId, auditWorkstation, out responseMessage);
                }
                catch (NotImplementedException nie)
                {
                    log.Warn("PrintPins() method in HSM module not implemented.", nie);
                    responseMessage = "CMS module not implemented.";
                    return false;
                }
            }
        }

        private bool CardProduction(long distBatchId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            lock (_lockCardProduction)
            {
                var cardObjects = FetchCardObjectsForDistBatch(distBatchId, languageId, auditUserId, auditWorkstation);

                IntegrationController _integration = IntegrationController.Instance;
                Veneka.Indigo.Integration.Config.IConfig config;
                Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;

                try
                {
                    return _integration.CardProductionSystem(cardObjects[0].ProductId, InterfaceArea.PRODUCTION, out externalFields, out config).UploadToCardProduction(ref cardObjects, externalFields, config, languageId, auditUserId, auditWorkstation, out responseMessage);

                }
                catch (NotImplementedException nie)
                {
                    log.Warn("UploadToCardProduction() method in CPS module not implemented.", nie);


                    responseMessage = "CPS module not implemented.";
                    return false;
                }
            }
        }


        /// <summary>
        /// Reject a production batch.
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="rejectedCards"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<DistBatchResult> RejectProductionBatch(long distBatchId, string notes, List<RejectCardInfo> rejectedCards, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage;
                var result = _distbatchMan.RejectProductionBatch(distBatchId, notes, rejectedCards, languageId, auditUserId, auditWorkstation, out resultMessage);

                return new Response<DistBatchResult>(result,
                                                     String.IsNullOrWhiteSpace(resultMessage) ? ResponseType.SUCCESSFUL : ResponseType.UNSUCCESSFUL,
                                                     resultMessage,
                                                     resultMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<DistBatchResult>(null,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
        /// <summary>
        /// Canel the Dist batch
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="rejectedCards"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>


        internal Response<DistBatchResult> DistBatchCancel(long distBatchId, int? distBatchStatusesId, int distbatchtypeid, int? cardissuemethod, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage;
                DistBatchResult distbatchresult;
                var result = _distbatchMan.DistBatchCancel(distBatchId, distBatchStatusesId, distbatchtypeid, cardissuemethod, notes, languageId, auditUserId, auditWorkstation, out resultMessage, out distbatchresult);

                return new Response<DistBatchResult>(distbatchresult, String.IsNullOrWhiteSpace(resultMessage) ? ResponseType.SUCCESSFUL : ResponseType.UNSUCCESSFUL,
                                                     resultMessage,
                                                     resultMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<DistBatchResult>(null, ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }


        internal Response<bool> UpdatePrintBatchRequestsStatus(long print_batch_id, int print_batch_statuses_id, bool Successful, List<RequestData> requestdata, List<string> cardstospoil, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            PrintBatchResult printbatchResult = null;
            string resultMessage = string.Empty;
            Response<bool> response = null;
            try
            {

                if (_printservice.UpdatePrintBatchRequestsStatus(print_batch_id, print_batch_statuses_id, Successful, requestdata, cardstospoil, notes, languageId, auditUserId, auditWorkstation, out resultMessage))
                {
                    response = new Response<bool>(true,
                                                         ResponseType.SUCCESSFUL,
                                                         resultMessage,
                                                         resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<bool>(false, ResponseType.UNSUCCESSFUL, ex.Message, resultMessage);
            }
            return response;
        }
        internal Response<PrintBatchResult> UpdatePrintBatchChangeStatus(long printBatchId, int? printBatchStatusesId, int? newprintBatchStatusesId, string notes, bool autogenerateDistBatch, int languageId, long auditUserId, string auditWorkstation)
        {
            PrintBatchResult printbatchResult = null;
            string resultMessage = string.Empty;
            Response<PrintBatchResult> response = null;
            try
            {

                if (_printservice.UpdatePrintBatchChangeStatus(printBatchId, printBatchStatusesId, newprintBatchStatusesId, notes, autogenerateDistBatch, languageId, auditUserId, auditWorkstation, out printbatchResult, out resultMessage))
                {
                    response = new Response<PrintBatchResult>(printbatchResult,
                                                         ResponseType.SUCCESSFUL,
                                                         resultMessage,
                                                         resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<PrintBatchResult>(null, ResponseType.UNSUCCESSFUL, ex.Message, resultMessage);
            }
            return response;
        }
        internal Response<int> GetStockinBranch(int issuerid, int? branchId, int? productId, int? card_issue_method_id,
                                                              long auditUserId, string auditWorkstation)
        {
            Response<int> response;
            try
            {
                response = new Response<int>(_printservice.GetStockinBranch(issuerid, branchId, productId, card_issue_method_id,
                                                               auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<int>(0,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }
        internal Response<List<PrintBatchResult>> GetPrintBatchesForUser(int? issuerId, int? productId, string pinBatchReference, int? pinBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                           DateTime? startDate, DateTime? endDate, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            Response<List<PrintBatchResult>> response;
            try
            {
                response = new Response<List<PrintBatchResult>>(_printservice.GetPrintBatchesForUser(issuerId, productId, pinBatchReference, pinBatchStatusId, branchId, cardIssueMethodId,
                                                                                                    startDate, endDate, langaugeId, rowsPerPage, pageIndex,
                                                                                                   auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<PrintBatchResult>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<List<HybridRequestResult>> GetRequestsByPrintBatch(long printBatchId, int startindex, int size, int languageId, long auditUserId, string auditWorkStation)
        {
            Response<List<HybridRequestResult>> response;
            try
            {
                response = new Response<List<HybridRequestResult>>(_printservice.GetRequestsByPrintBatch(printBatchId, startindex, size, languageId, auditUserId, auditWorkStation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<HybridRequestResult>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<List<HybridRequestResult>> SearchHybridRequestList(int issuerId, int? branchId, int? productId, int? hybridrequeststatusId, string requestreference, int? cardIssueMethodId, bool checkmasking, int languageId, int pageIndex, int rowsPerPage,
                                                                     long auditUserId, string auditWorkstation)
        {
            Response<List<HybridRequestResult>> response;
            try
            {
                response = new Response<List<HybridRequestResult>>(_printservice.SearchHybridRequestList(issuerId, branchId, productId, hybridrequeststatusId, requestreference, cardIssueMethodId,
                                                 checkmasking, languageId, rowsPerPage, pageIndex, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<HybridRequestResult>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        public Response<PrintBatchResult> GetPrintBatch(long pinBatchId, int languageId, long auditUserId, string auditWorkstation)
        {


            Response<PrintBatchResult> response;
            try
            {
                response = new Response<PrintBatchResult>(_printservice.GetPrintBatch(pinBatchId, languageId, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<PrintBatchResult>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;

        }

        internal Response<List<PrintBatchRequestDetails>> GetPrintBatchRequests(long pinBatchId, int startindex, int size, long auditUserId, string auditWorkstation)
        {
            Response<List<PrintBatchRequestDetails>> response;
            try
            {
                response = new Response<List<PrintBatchRequestDetails>>(_printservice.GetPrintBatchRequests(pinBatchId, startindex, size, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<PrintBatchRequestDetails>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }


        public Response<string[]> GetWorkStationKey(string auditWorkstation, int Size)
        {
            Response<string[]> response = null;
            try
            {

                return new Response<string[]>(_printservice.GetWorkStationKey(auditWorkstation, Size), ResponseType.SUCCESSFUL,
                                                               "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<string[]>(null, ResponseType.SUCCESSFUL,
                                                               "",
                                                               "");
            }
            return response;
        }



        /// <summary>
        /// Generate card list PDF report for distribution batch.
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal Response<byte[]> GenerateDistBatchReport(long distBatchId, int languageId, string username, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<byte[]>(_distbatchMan.GenerateDistBatchReport(distBatchId, languageId, username, auditUserId, auditWorkStation),
                                            ResponseType.SUCCESSFUL,
                                            ResponseType.SUCCESSFUL.ToString(),
                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<byte[]>(null,
                                            ResponseType.ERROR,
                                            "An error occured during processing your request, please try again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }


        internal Response<byte[]> GeneratePrintBatchReport(long printBatchId, int languageId, string username, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<byte[]>(_printservice.GeneratePrintBatchReport(printBatchId, languageId, username, auditUserId, auditWorkStation),
                                            ResponseType.SUCCESSFUL,
                                            ResponseType.SUCCESSFUL.ToString(),
                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<byte[]>(null,
                                            ResponseType.ERROR,
                                            "An error occured during processing your request, please try again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        #endregion

        #region Private Methods

        public Response<List<BillingReportResult>> GetBillingReport(int? IssuerId, string month, string year, long auditUserId, string auditWorkstation)
        {
            try
            {
                var result = _cardManService.GetBillingReport(IssuerId, month, year, auditUserId, auditWorkstation);

                return new Response<List<BillingReportResult>>(result, ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<BillingReportResult>>(null,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        private List<CardObject> FetchCardObjectsForDistBatch(long distBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            var cards = _distbatchMan.GetDistBatchCardDetails(distBatchId, languageId, auditUserId, auditWorkstation);

            List<CardObject> cardObjects = new List<CardObject>();

            foreach (var card in cards)
            {
                //TODO: Remove PAN hardcoded length
                CardObject cardObj = new CardObject(card.card_id, card.card_request_reference, card.issuer_id, card.issuer_code, card.issuer_name,
                                                    card.branch_id, card.branch_code, card.branch_name, card.product_id,
                                                    card.product_code, card.product_bin_code, card.sub_product_code, card.pan_length, card.src1_id, card.src2_id,
                                                    card.src3_id, card.PVKI.GetValueOrDefault(), card.PVK, card.CVKA, card.CVKB, card.card_sequence);

                //AMK: Use the line below when correctly setup
                //CardObject cardObj = new CardObject(card.card_id, card.card_request_reference, card.issuer_id, card.issuer_code, card.issuer_name,
                //                                   card.branch_id, card.branch_code, card.branch_name, card.product_id,
                //                                   card.product_code, card.product_bin_code, card.sub_product_code, card.pan_length, card.src1_id, card.src2_id,
                //                                   card.src3_id, card.PVKI.Value, card.PVK, card.CVKA, card.CVKB, card.card_sequence);

                cardObj.DistBatchReference = card.dist_batch_reference;
                cardObj.ProductName = card.product_name;
                //cardObj.SubProductName = card.sub_product_name;

                cardObj.CardNumber = card.card_number;

                if (card.card_activation_date != null)
                    cardObj.CardActivatedDate = card.card_activation_date.Value;

                if (card.card_production_date != null)
                    cardObj.CardIssuedDate = card.card_production_date.Value;

                if (card.card_expiry_date != null)
                    cardObj.ExpiryDate = card.card_expiry_date;

                cardObj.ExpiryMonths = card.expiry_months.Value;

                cardObj.DistBatchId = card.dist_batch_id;
                cardObj.DistCardStatusId = card.dist_card_status_id;
                cardObj.PVV = card.pvv;
                cardObj.DeliveryBranchCode = card.delivery_branch_code;
                cardObj.DeliveryBranchName = card.delivery_branch_name;
                cardObj.EMPDeliveryBranchCode = card.emp_delivery_branch_code;
                List<IProductPrintField> printFields = _cardManService.GetProductFieldsByCardId(card.card_id);
                cardObj.PrintFields = printFields;
                cardObj.CardIssueMethodId = card.card_issue_method_id;
                var productFields = _cardManService.GetProductFields(card.product_id, null, null).Where(p => p.ProductPrintFieldTypeId == 0).ToList();

                List<ProductField> accountFields = new List<ProductField>();
                try
                {
                    accountFields = _cardManService.GetProductFieldsByCardIdTransalated(card.card_id);
                }
                catch (Exception)
                {

                }

                if (card.card_issue_method_id == 0 || card.card_issue_method_id == 2)
                {
                    var account = new AccountDetails();

                    account.AccountNumber = card.customer_account_number;
                    account.AccountTypeId = card.account_type_id.Value;
                    account.CardIssueMethodId = card.card_issue_method_id;
                    account.CardPriorityId = card.card_priority_id;
                    account.CmsID = card.cms_id;
                    account.ContactNumber = card.contact_number;
                    account.ContractNumber = card.contract_number;
                    account.CurrencyId = card.currency_id.GetValueOrDefault();
                    account.CustomerIDNumber = card.Id_number;
                    account.CustomerResidencyId = card.resident_id.GetValueOrDefault();
                    account.CustomerTitleId = card.customer_title_id.GetValueOrDefault();
                    account.CustomerTypeId = card.customer_type_id.GetValueOrDefault();
                    account.FirstName = card.customer_first_name;
                    account.LastName = card.customer_last_name;
                    account.MiddleName = card.customer_middle_name;
                    account.NameOnCard = card.name_on_card;
                    account.PriorityId = card.card_priority_id;

                    account.CreditContractNumber = card.credit_contract_number;
                    account.CreditLimit = card.credit_limit;
                    account.CreditLimitApproved = card.credit_limit_approved;
                    account.EmailAddress = card.customer_email;
                    account.CardIssueReasonId = card.card_issue_reason_id;
                    try
                    {
                        account.ProductFields = accountFields;
                    }
                    catch (Exception)
                    {

                    }


                    cardObj.CustomerAccount = account;
                }


                cardObjects.Add(cardObj);
            }

            return cardObjects;
        }
        #endregion
    }
}