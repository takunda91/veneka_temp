using System;
using System.Collections.Generic;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.Common.Models;
using Common.Logging;
using System.Collections;
namespace IndigoCardIssuanceService.bll
{
    public class ThreedBatchController
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ThreedBatchController));

        private readonly SessionManager _sessionManager = SessionManager.GetInstance();
        private readonly ThreedBatchManagementService ThreedbatchMan = new ThreedBatchManagementService();

        #region EXPOSED METHODS

        /// <summary>
        /// Return a list of Threed batches based on parameters.
        /// </summary>
        /// <param name="ThreedBatchReference"></param>
        /// <param name="ThreedBatchStatus"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal Response<List<ThreedBatchResult>> GetThreedBatches(string ThreedBatchReference, int issuerId, ThreedBatchStatus? ThreedBatchStatus, DateTime? startDate, DateTime? endDate,
                                                                     int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<List<ThreedBatchResult>>(ThreedbatchMan.GetThreedBatches(ThreedBatchReference, issuerId, ThreedBatchStatus, startDate, endDate, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation),
                                                           ResponseType.SUCCESSFUL,
                                                           ResponseType.SUCCESSFUL.ToString(),
                                                           "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ThreedBatchResult>>(null,
                                                           ResponseType.ERROR,
                                                           "An error occured during processing your request, please try again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Return a single load batch.
        /// </summary>
        /// <param name="ThreedBatchId"></param>
        /// <returns></returns>
        internal Response<ThreedBatchResult> GetThreedBatch(long ThreedBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<ThreedBatchResult>(ThreedbatchMan.GetThreedBatch(ThreedBatchId, languageId, auditUserId, auditWorkStation),
                                                           ResponseType.SUCCESSFUL,
                                                           ResponseType.SUCCESSFUL.ToString(),
                                                           "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ThreedBatchResult>(null,
                                                           ResponseType.ERROR,
                                                           "An error occured during processing your request, please try again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal BaseResponse RecreateThreedBatch(long ThreedBatchId, string notes, int languageId, long userId, string workstation)
        {
            try
            {
                string response;
                if (ThreedbatchMan.RecreateThreedBatch(ThreedBatchId, notes, languageId, userId, workstation, out response))
                    return new BaseResponse(ResponseType.SUCCESSFUL, response, "");
                else
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, response, response);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal BaseResponse ApproveMultipleThreedBatch(ArrayList threedBatchId, string notes, int languageId, long userId, string workstation)
        {
            try
            {
                foreach (var item in threedBatchId)
                {
                    long ThreedBatchId = 0;
                    try
                    {

                        long.TryParse(item.ToString(), out ThreedBatchId);
                        if (ThreedBatchId > 0)
                        {

                            string resultMessage;
                            ThreedbatchMan.RecreateThreedBatch(ThreedBatchId, notes, languageId, userId, workstation, out resultMessage);
                            log.Debug(string.Format("Status of loadbatchId {0} ,response :{1}", ThreedBatchId, resultMessage));

                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Failed to Update loadbatchId {0} ", ThreedBatchId));
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
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
        #endregion
    }
}
    
