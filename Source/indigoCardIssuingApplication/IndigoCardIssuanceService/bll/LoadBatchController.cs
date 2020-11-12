using System;
using System.Collections.Generic;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.Common.Models;
using Common.Logging;
using System.Collections;


namespace IndigoCardIssuanceService.bll
{
    public class LoadBatchController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoadBatchController));

        private readonly SessionManager _sessionManager = SessionManager.GetInstance();
        private readonly LoadBatchMangementService loadbatchMan = new LoadBatchMangementService();

        #region EXPOSED METHODS

        /// <summary>
        /// Return a list of load batches based on parameters.
        /// </summary>
        /// <param name="loadBatchReference"></param>
        /// <param name="loadBatchStatus"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal Response<List<LoadBatchResult>> GetLoadBatches(string loadBatchReference, int issuerId, LoadBatchStatus? loadBatchStatus, DateTime? startDate, DateTime? endDate,
                                                                 int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<List<LoadBatchResult>>(loadbatchMan.GetLoadBatches(loadBatchReference, issuerId, loadBatchStatus, startDate, endDate, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation),
                                                           ResponseType.SUCCESSFUL,
                                                           ResponseType.SUCCESSFUL.ToString(),
                                                           "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LoadBatchResult>>(null,
                                                           ResponseType.ERROR,
                                                           "An error occured during processing your request, please try again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Return a single load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <returns></returns>
        internal Response<LoadBatchResult> GetLoadBatch(long loadBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<LoadBatchResult>(loadbatchMan.GetLoadBatch(loadBatchId, languageId, auditUserId, auditWorkStation),
                                                           ResponseType.SUCCESSFUL,
                                                           ResponseType.SUCCESSFUL.ToString(),
                                                           "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<LoadBatchResult>(null,
                                                           ResponseType.ERROR,
                                                           "An error occured during processing your request, please try again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Approve the load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal BaseResponse ApproveLoadBatch(long loadBatchId, string notes, int language, long auditUserId, string auditWorkStation)
        {
            try
            {
                string response;
                if (loadbatchMan.ApproveLoadBatch(loadBatchId, notes, language, auditUserId, auditWorkStation, out response))
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

        internal BaseResponse ApproveMultipleLoadBatch(ArrayList loadBatchIds, string notes, int language, long auditUserId, string auditWorkStation)
        {
            try
            {
                foreach (var item in loadBatchIds)
                {
                    long loadBatchId=0;
                    try
                    {
                       
                        long.TryParse(item.ToString(),out loadBatchId);
                        if (loadBatchId>0)
                        {
                            
                            string resultMessage;
                            loadbatchMan.ApproveLoadBatch(loadBatchId, notes, language, auditUserId, auditWorkStation, out resultMessage);
                            log.Debug(string.Format("Status of loadbatchId {0} ,response :{1}", loadBatchId, resultMessage));
                          
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Failed to Update loadbatchId {0} ", loadBatchId));
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
        /// <summary>
        /// Reject the load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal BaseResponse RejectLoadBatch(long loadBatchId, string notes, int language, long auditUserId, string auditWorkStation)
        {
            try
            {
                string response;

                if (loadbatchMan.RejectLoadBatch(loadBatchId, notes, language, auditUserId, auditWorkStation, out response))
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

        /// <summary>
        /// Generate card list PDF report for load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal Response<byte[]> GenerateLoadBatchReport(long loadBatchId, int languageId, string username, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<byte[]>(loadbatchMan.GenerateLoadBatchReport(loadBatchId, languageId, username, auditUserId, auditWorkStation),
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

        internal Response<byte[]> GeneratePinMailerBatchReport(int pinBatchHeaderId, int languageId, string username, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<byte[]>(loadbatchMan.GeneratePinMailerBatchReport(pinBatchHeaderId, languageId, username, auditUserId, auditWorkStation),
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

        /// <summary>
        /// Fetch list of file_load
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public Response<List<FileLoadResult>> GetFileLoadList(DateTime dateFrom, DateTime dateTo, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<FileLoadResult>>(loadbatchMan.GetFileLoadList(dateFrom, dateTo, pageIndex, rowsPerPage, auditUserId, auditWorkstation),
                                                        ResponseType.SUCCESSFUL,
                                                        ResponseType.SUCCESSFUL.ToString(),
                                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<FileLoadResult>>(null,
                                                     ResponseType.ERROR,
                                                     "An error occured during processing your request, please try again.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Fetch all file histories
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public Response<List<FileHistoryResult>> GetFileHistorys(int? issuerId, DateTime dateFrom, DateTime dateTo, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<FileHistoryResult>>(loadbatchMan.GetFileHistorys(issuerId, dateFrom, dateTo, auditUserId, auditWorkstation),
                                                        ResponseType.SUCCESSFUL,
                                                        ResponseType.SUCCESSFUL.ToString(),
                                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<FileHistoryResult>>(null,
                                                        ResponseType.ERROR,
                                                        "An error occured during processing your request, please try again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Fetch specific file history
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<FileHistoryResult> GetFileHistory(long fileId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<FileHistoryResult>(loadbatchMan.GetFileHistory(fileId, auditUserId, auditWorkstation),
                                                       ResponseType.SUCCESSFUL,
                                                       ResponseType.SUCCESSFUL.ToString(),
                                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<FileHistoryResult>(null,
                                                  ResponseType.ERROR,
                                                  "An error occured during processing your request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
        #endregion

    }
}