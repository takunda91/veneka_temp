using Common.Logging;
using IndigoCardIssuanceService.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.Common.Models;

namespace IndigoCardIssuanceService.bll
{
    public class ExportBatchController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ExportBatchController));
        private readonly ExportBatchService _exportBatchService = new ExportBatchService();
        private readonly SessionManager _sessionManager = SessionManager.GetInstance();

        public Response<ExportBatchResult> ApproveExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage;
                ExportBatchResult result;
                if (_exportBatchService.ApproveExportBatch(exportBatchId, notes, languageId, auditUserId, auditWorkstation, out result, out resultMessage))
                {
                    return new Response<ExportBatchResult>(result,
                                                         ResponseType.SUCCESSFUL,
                                                         resultMessage,
                                                         resultMessage);
                }
                else
                {
                    return new Response<ExportBatchResult>(null, ResponseType.UNSUCCESSFUL, resultMessage, resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ExportBatchResult>(null,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        public Response<ExportBatchResult> RejectExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage;
                ExportBatchResult result;
                if (_exportBatchService.RejectExportBatch(exportBatchId, notes, languageId, auditUserId, auditWorkstation, out result, out resultMessage))
                {
                    return new Response<ExportBatchResult>(result,
                                                         ResponseType.SUCCESSFUL,
                                                         resultMessage,
                                                         resultMessage);
                }
                else
                {
                    return new Response<ExportBatchResult>(null, ResponseType.UNSUCCESSFUL, resultMessage, resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ExportBatchResult>(null,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        public Response<ExportBatchResult> RequestExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage;
                ExportBatchResult result;
                if (_exportBatchService.RequestExportBatch(exportBatchId, notes, languageId, auditUserId, auditWorkstation, out result, out resultMessage))
                {
                    return new Response<ExportBatchResult>(result,
                                                         ResponseType.SUCCESSFUL,
                                                         resultMessage,
                                                         resultMessage);
                }
                else
                {
                    return new Response<ExportBatchResult>(null, ResponseType.UNSUCCESSFUL, resultMessage, resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ExportBatchResult>(null,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        public Response<List<ExportBatchResult>> SearchExportBatch(int? issuerId, int? productId, int? exportBatchStatusesId,
                                                            string batchReference, DateTime? dateFrom, DateTime? dateTo,
                                                            int pageIndex, int rowsPerPage,
                                                            int languageId, long auditUserId, string auditWorkstation)
        {            
            try
            {
                return new Response<List<ExportBatchResult>>(_exportBatchService.SearchExportBatch(issuerId, productId, exportBatchStatusesId,
                                                            batchReference, dateFrom, dateTo, pageIndex, rowsPerPage, languageId, auditUserId, auditWorkstation),
                                                           ResponseType.SUCCESSFUL,
                                                           "",
                                                           "");               
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ExportBatchResult>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        public Response<ExportBatchResult> GetExportBatch(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<ExportBatchResult>(_exportBatchService.GetExportBatch(exportBatchId, languageId, auditUserId, auditWorkstation),
                                                           ResponseType.SUCCESSFUL,
                                                           "",
                                                           "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ExportBatchResult>(null,
                                                        ResponseType.ERROR,
                                                        "Error when processing request.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Generate card list PDF report for distribution batch.
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal Response<byte[]> GenerateExportBatchReport(long exportBatchId, int languageId, string username, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<byte[]>(_exportBatchService.GenerateExportBatchReport(exportBatchId, languageId, username, auditUserId, auditWorkStation),
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
    }
}