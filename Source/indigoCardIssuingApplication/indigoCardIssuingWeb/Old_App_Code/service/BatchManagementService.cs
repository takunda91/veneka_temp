using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using indigoCardIssuingWeb.Old_App_Code.service;
using System.Collections;

namespace indigoCardIssuingWeb.service
{
    public class BatchManagementService : BaseService
    {
        //protected new readonly ILog log = LogManager.GetLogger(typeof(CustomerCardIssueService));
        private static readonly ILog log = LogManager.GetLogger(typeof(BatchManagementService));
        //private static Service1SoapClient _issuanceService = new Service1SoapClient();

        #region LOAD BATCH SERVICES

        public List<LangLookup> LangLookupLoadBatchStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupLoadBatchStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        public List<LangLookup> LangLookupThreedBatchStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupThreedBatchStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        public List<LangLookup> LangLookupLoadCardStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupLoadCardStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

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
        internal List<LoadBatchResult> GetLoadBatches(string loadBatchReference, int issuerId, LoadBatchStatus? loadBatchStatus, DateTime? startDate, DateTime? endDate, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetLoadBatches(loadBatchReference, issuerId, loadBatchStatus, startDate, endDate, pageIndex, StaticDataContainer.ROWS_PER_PAGE, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        /// <summary>
        /// Return a list of load batches based on parameters.
        /// </summary>
        /// <param name="searchParameters"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        internal List<LoadBatchResult> GetLoadBatches(LoadBatchSearchParameters searchParameters, int pageIndex)
        {
            return this.GetLoadBatches(searchParameters.BatchReference, searchParameters.IssuerId, searchParameters.LoadBatchStatus, searchParameters.DateFrom,
                                        searchParameters.DateTo, pageIndex);
        }

        /// <summary>
        /// Return a single load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <returns></returns>
        internal LoadBatchResult GetLoadBatch(long loadBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetLoadBatch(loadBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;
        }

        internal List<ThreedBatchResult> GetThreedBatches(string BatchReference, int issuerId, ThreedBatchStatus? BatchStatus, DateTime? startDate, DateTime? endDate, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetThreedBatches(BatchReference, issuerId, BatchStatus, startDate, endDate, pageIndex, StaticDataContainer.ROWS_PER_PAGE, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        /// <summary>
        /// Return a list of load batches based on parameters.
        /// </summary>
        /// <param name="searchParameters"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        internal List<ThreedBatchResult> GetThreedBatches(ThreedBatchSearchParameters searchParameters, int pageIndex)
        {
            return this.GetThreedBatches(searchParameters.BatchReference, searchParameters.IssuerId, searchParameters.BatchStatus, searchParameters.DateFrom,
                                        searchParameters.DateTo, pageIndex);
        }

        /// <summary>
        /// Return a single load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <returns></returns>
        internal ThreedBatchResult GetThreedBatch(long ThreedBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetThreedBatch(ThreedBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;
        }
        internal bool RecreateThreedBatch(long threedBatchId, string notes, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RecreateThreedBatch(threedBatchId, notes, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }
        internal bool ApproveMultipleThreedBatch(ArrayList loadBatchId, string notes, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ApproveMultipleLoadBatch(loadBatchId.ToArray(), notes, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }
        /// <summary>
        /// Approve the load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal bool ApproveLoadBatch(long loadBatchId, string notes, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ApproveLoadBatch(loadBatchId, notes, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }

        internal RequestDetails GetRequestDetails(long requestId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetRequestDetails(requestId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;
        }


        /// <summary>
        /// Approve Multiple Batches
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        internal bool ApproveMultipleLoadBatch(ArrayList loadBatchId, string notes, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ApproveMultipleLoadBatch(loadBatchId.ToArray(), notes, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }
        /// <summary>
        /// Reject the load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal bool RejectLoadBatch(long loadBatchId, string notes, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RejectLoadBatch(loadBatchId, notes, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);

            //messages = response.ResponseMessage;

            //if (response.ResponseType != ResponseType.SUCCESSFUL)
            //{
            //    throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
            //}

            //if (response.ResponseType == ResponseType.SUCCESSFUL)
            //    return true;
            //else if (response.ResponseType == ResponseType.UNSUCCESSFUL)
            //    return false;
            //else if (response.ResponseType != ResponseType.SUCCESSFUL)
            //{
            //    messages = log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage;
            //}

            //return false;
        }

        /// <summary>
        /// Generate card list PDF report for load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <returns></returns>
        internal byte[] GenerateLoadBatchReport(long loadBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                    SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                    SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GenerateLoadBatchReport(loadBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;

            //if (response.ResponseType != ResponseType.SUCCESSFUL)
            //{
            //    throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
            //}

            //return response.Value;
        }

        /// <summary>
        /// Generate card list PDF report for load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <returns></returns>
        internal byte[] GenerateExportBatchReport(long exportBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                    SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                    SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GenerateExportBatchReport(exportBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;
        }

        /// <summary>
        /// Generate card list PDF report for distribution batch.
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <returns></returns>
        internal byte[] GenerateDistBatchReport(long distBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GenerateDistBatchReport(distBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;
        }

        /// <summary>
        /// Generate Checked out cards report.
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        internal byte[] GenerateCheckedOutCardsReport(long operatorId, int branchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GenerateCheckedOutCardsReport(null, operatorId, branchId, null, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;
        }

        /// <summary>
        /// Generate Checked out cards report.
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        internal byte[] GenerateCheckedInCardsReport(int branchId, List<structCard> cardList, string operatorUsername)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GenerateCheckedInCardsReport(branchId, cardList.ToArray(), operatorUsername, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;
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
        public List<FileHistoryResult> GetFileHistorys(int? issuerId, DateTime dateFrom, DateTime dateTo)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                             SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                             SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetFileHistorys(issuerId, dateFrom, dateTo, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        /// <summary>
        /// Fetch specific file history
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public FileHistoryResult GetFileHistory(long fileId, long auditUserId, string auditWorkstation)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                             SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                             SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetFileHistory(fileId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;
        }

        #endregion

        #region EXPORT BATCH SERVICE

        internal List<LangLookup> LangLookupExportBatchStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupExportBatchStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        internal bool ApproveExportBatch(long exportBatchId, string notes, out ExportBatchResult result, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ApproveExportBatch(exportBatchId, notes, encryptedSessionKey);
            result = response.Value;

            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool RejectExportBatch(long exportBatchId, string notes, out ExportBatchResult result, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RejectExportBatch(exportBatchId, notes, encryptedSessionKey);
            result = response.Value;

            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool RequestExportBatch(long exportBatchId, string notes, out ExportBatchResult result, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RequestExportBatch(exportBatchId, notes, encryptedSessionKey);
            result = response.Value;

            return base.CheckResponse(response, log, out responseMessage);
        }

        internal List<ExportBatchResult> SearchExportBatch(int? issuerId, int? productId, int? exportBatchStatusesId,
                                                             string batchReference, DateTime? dateFrom, DateTime? dateTo,
                                                             int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            List<ExportBatchResult> rtnValue = new List<ExportBatchResult>();

            var response = m_indigoApp.SearchExportBatch(issuerId, productId, exportBatchStatusesId,
                                                                batchReference, dateFrom, dateTo,
                                                                pageIndex, StaticDataContainer.ROWS_PER_PAGE, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        internal List<ExportBatchResult> SearchExportBatch(ExportBatchSearchParameters searchParams, int pageIndex)
        {
            return SearchExportBatch(searchParams.IssuerId, searchParams.ProductId, searchParams.ExportBatchStatusesId, searchParams.BatchReference,
                                        searchParams.DateFrom, searchParams.DateTo, pageIndex, StaticDataContainer.ROWS_PER_PAGE);
        }

        internal ExportBatchResult GetExportBatch(long exportBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetExportBatch(exportBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;
        }

        #endregion

        #region DISTRIBUTION BATCH SERVICES

        public List<LangLookup> LangLookupDistBatchStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupDistBatchStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        public List<LangLookup> LangLookupDistCardStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupDistCardStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        internal int GetBranchCardCount(int branchId, int productId, int? cardIssueMethodId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetBranchCardCount(branchId, productId, cardIssueMethodId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return 0;
        }

        internal int GetDistBatchCardCountForRedist(int distBatchId, int productId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetDistBatchCardCountForRedist(distBatchId, productId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return 0;
        }

        internal int GetDistBatchCount(long batchId, int branchId, int productId, int? cardIssueMethodId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetDistBatchCount(batchId, branchId, productId, cardIssueMethodId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return 0;
        }
        /// <summary>
        /// Create a new distribution batch
        /// </summary>
        /// <param name="branchId">Where this distribution batch is to be sent.</param>
        /// <param name="batchCardSize">The number of cards the distribution batch must have.</param>
        /// <returns></returns>
        internal bool CreateDistributionBatch(int issuerId, int fromBranchId, int toBranchId, int cardIssueMethodId,
                                                    int productId, int batchCardSize, int createBatchOption,
                                                     string startRef, string endRef, long? from_dist_batch_id, out string responseMessage, out int dist_batch_id, out string dist_batch_ref)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateDistributionBatch(issuerId, fromBranchId, toBranchId, cardIssueMethodId,
                                                                    productId, batchCardSize, createBatchOption,
                                                                    startRef, endRef, from_dist_batch_id, encryptedSessionKey, out dist_batch_ref);

            dist_batch_id = response.Value;
            return base.CheckResponse(response, log, out responseMessage);
        }

        internal string CreateDistributionBatch(int fromBranchId, int toBranchId, int productId, int? subProductId, string StartRefNo, string EndRefNo)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            //var response = _issuanceService.CreateDistributionBatch(branchId, batchCardSize, encryptedSessionKey);

            //if (response.ResponseType != ResponseType.SUCCESSFUL)
            //{
            //    throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
            //}

            return "";
        }

        /// <summary>
        /// Create a new card stock order
        /// </summary>        
        /// <returns></returns>
        internal bool CreateCardStockRequest(int issuerId, int branchId, int productId, int cardIssueMethodId, int cardPriority, int batchCardSize, out CardRequestBatchResponse cardRequestResp, out string ResponseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateCardStockOrder(issuerId, branchId, productId, cardIssueMethodId, cardPriority, batchCardSize, encryptedSessionKey);
            cardRequestResp = response.Value;

            return base.CheckResponse(response, log, out ResponseMessage);
        }

        /// <summary>
        /// Create distribution batch based on the request for cards input parameter.
        /// </summary>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="branchId"></param>
        /// <param name="productId"></param>
        /// <param name="cardPriorityId"></param>
        /// <param name="cardRequestResp"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        internal bool CreateCardRequestBatch(int cardIssueMethodId, int issuerid, int? branchId, int productId, int cardPriorityId, out CardRequestBatchResponse cardRequestResp, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateCardRequestBatch(cardIssueMethodId, issuerid, branchId, productId, cardPriorityId, encryptedSessionKey);
            cardRequestResp = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        internal bool CreateHybridRequestBatch(int cardIssueMethodId, int issuerid, int? branchId, int productId, int cardPriorityId, out HybridRequestBatchResponse cardRequestResp, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateHybridRequestBatch(cardIssueMethodId, issuerid, branchId, productId, cardPriorityId, encryptedSessionKey);
            cardRequestResp = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        /// <summary>
        /// Returns a list of distribution batches for a user. dist batch status and dates may be null to not do a filter on them.
        /// </summary>
        /// <param name="userId">If value null, session user Id will be used</param>
        /// <param name="distBatchStatus">Value may be NULL</param>
        /// <param name="startDate">Value may be NULL</param>
        /// <param name="endDate">Value may be NULL</param>
        /// <returns></returns>
        internal List<DistBatchResult> GetDistBatchesForUser(long? userId, int? issuerId, string distBatchReference, int? distBatchStatusId, int? flowdistBatchStatusId,
                                                           int? branchId, int? cardIssueMethodId, int? distBatchTypeId, DateTime? startDate, DateTime? endDate, bool? includeOriginBranch, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetDistBatchesForUser(userId, issuerId, distBatchReference, distBatchStatusId, flowdistBatchStatusId, branchId, cardIssueMethodId, distBatchTypeId,
                                                                   startDate, endDate, includeOriginBranch, StaticDataContainer.ROWS_PER_PAGE, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        internal List<DistStatusResult> GetDistBatchesForStatus(long? userId, int? issuerId, string distBatchReference, int? distBatchStatusId,
                                                          int? branchId, int? cardIssueMethodId, int? distBatchTypeId, DateTime? startDate, DateTime? endDate, bool cardCountForRedist, int? rowsPerPage, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetDistBatchesForStatus(userId, issuerId, distBatchReference, distBatchStatusId, branchId, cardIssueMethodId, distBatchTypeId,
                                                                   startDate, endDate, cardCountForRedist, rowsPerPage.HasValue ? rowsPerPage.Value : StaticDataContainer.ROWS_PER_PAGE, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        internal List<card_request_result> GetCardRequestList(int issuerid, int? branchId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCardRequestList(issuerid, branchId, rowsPerPage, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        internal List<card_request_result> GetCardRequestList(int issuerid, int? branchId, int cardIssueMethodId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCardRequestListByIssueMethod(issuerid, branchId, cardIssueMethodId, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        public List<LangLookup> LangLookupHybridRequestStatues()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupHybridRequestStatues(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        public List<LangLookup> LangLookupPrintBatchStatues()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupPrintBatchStatues(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        internal List<hybrid_request_result> GetHybridRequestList(int issuerid, int? branchId, int? productId, string batchrefrence, int? hybrid_request_statuesId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetHybridRequestList(issuerid, branchId, productId, batchrefrence, hybrid_request_statuesId, rowsPerPage, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        internal int GetStockinBranch(int issuerid, int? branchId, int? productId, int card_issue_method)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetStockinBranch(issuerid, branchId, productId, card_issue_method, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return 0;
        }


        internal List<HybridRequestResult> SearchHybridRequests(int issuerid, int? branchId, int? productId, string requestrefrence, int? hybrid_request_statuesId, int? cardIssueMethodId, bool checkmasking, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.SearchHybridRequestList(issuerid, branchId, productId, hybrid_request_statuesId, requestrefrence, cardIssueMethodId, checkmasking, rowsPerPage, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        internal List<BatchCardInfo> GetBatchCardInfoPaged(long distBatchId, int rowsPerPage, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetBatchCardInfoPaged(distBatchId, rowsPerPage, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        internal List<BatchCardInfo> GetBatchCardInfoPaged(BatchCardInfoSearchParameters parms, int rowsPerPage)
        {
            return this.GetBatchCardInfoPaged(parms.DistBatchId, rowsPerPage, parms.PageIndex);
        }

        /// <summary>
        /// Returns a list of distribution batches for a user. dist batch status and dates may be null to not do a filter on them.
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<DistBatchResult> GetDistBatchesForUser(DistBatchSearchParameters searchParams, int pageIndex)
        {
            return this.GetDistBatchesForUser(searchParams.UserId, searchParams.IssuerId, searchParams.BatchReference, searchParams.DistBatchStatusId, searchParams.FlowDistBatchStatusId,
                                                searchParams.BranchId, searchParams.CardIssueMethodId, searchParams.DistBatchTypeId, searchParams.DateFrom, searchParams.DateTo, searchParams.IncludeOriginBranch, pageIndex);
        }

        /// <summary>
        /// Retrieve dist batch
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <returns></returns>
        public DistBatchResult GetDistBatch(long distBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetDistBatch(distBatchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;
        }

        public bool DistBatchRejectStatus(long distBatchId, int distBatchStatusesId, string notes, out DistBatchResult distBatchResult, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DistBatchRejectStatus(distBatchId, distBatchStatusesId, notes, encryptedSessionKey);
            distBatchResult = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        public bool DistBatchChangeStatus(long distBatchId, int distBatchStatusesId, int flowDistBatchStatusesId, string notes, bool autogenerateDistBatch, out DistBatchResult distBatchResult, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DistBatchChangeStatus(distBatchId, distBatchStatusesId, flowDistBatchStatusesId, notes, autogenerateDistBatch, encryptedSessionKey);
            distBatchResult = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        public bool DistBatchChangeStatusRenewal(long distBatchId, int distBatchStatusesId, int flowDistBatchStatusesId, string notes, bool autogenerateDistBatch, out DistBatchResult distBatchResult, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DistBatchChangeStatusRenewal(distBatchId, distBatchStatusesId, flowDistBatchStatusesId, notes, autogenerateDistBatch, encryptedSessionKey);
            distBatchResult = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        public bool DistBatchCancel(long distBatchId, int? distBatchStatusesId, int distbatchtypeid, int? cardissuemethod, string notes, out string messages, out DistBatchResult distBatchResult)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DistBatchCancel(distBatchId, distBatchStatusesId, distbatchtypeid, cardissuemethod, notes, encryptedSessionKey);
            distBatchResult = response.Value;

            return base.CheckResponse(response, log, out messages);
        }
        public bool MultipleDistBatchChangeStatus(ArrayList distbatchId, ArrayList distbatchStatusId, ArrayList flowdisbatchstausId, bool autogenerateDistBatch, string notes, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.MultipleDistBatchChangeStatus(distbatchId.ToArray(), distbatchStatusId.ToArray(), flowdisbatchstausId.ToArray(), notes, autogenerateDistBatch, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }
        internal bool RejectProductionBatch(long distBatchId, string notes, List<RejectCardInfo> rejectCards, out DistBatchResult distBatchResult, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RejectProductionBatch(distBatchId, notes, rejectCards.ToArray(), encryptedSessionKey);
            distBatchResult = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        internal bool GetCentralBatchReferenceNumbers(int issuerId, int fromBranchId, int toBranchId, int cardIssueMethodId,
                                         int productId, out string responseMessage, out string fromRef, out string toRef)
        {
            responseMessage = string.Empty;
            fromRef = string.Empty;
            toRef = string.Empty;
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCentralBatchReferenceNumbers(issuerId, fromBranchId, toBranchId, cardIssueMethodId,
                                                                    productId, encryptedSessionKey, out responseMessage, out fromRef, out toRef);

            return base.CheckResponse(response, log, out responseMessage);
        }
        #endregion

        #region ISSUE CARD METHODS

        public List<LangLookup> LangLookupBranchCardStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupBranchCardStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        /// <summary>
        /// Fetch the list of card priorities.
        /// </summary>
        /// <returns></returns>
        public List<card_priority> GetCardPriorityList()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCardPriorityList(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        /// <summary>
        /// Returns a sorted list of card Priorities
        /// </summary>
        /// <param name="selectedValue">This is the value which should be set as the default selected value</param>
        /// <returns></returns>
        public List<System.Web.UI.WebControls.ListItem> GetCardPriorityList(out int selectedValue)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCardPriorityList(encryptedSessionKey);

            base.CheckResponse(response, log);


            List<System.Web.UI.WebControls.ListItem> priorityList = new List<System.Web.UI.WebControls.ListItem>();
            selectedValue = response.Value.ToList().OrderBy(m => m.card_priority_order).First().card_priority_id;
            //Order the list and find which value should be the default selected value
            foreach (var priority in response.Value.ToList().OrderBy(m => m.card_priority_order))
            {
                if (priority.default_selection)
                    selectedValue = priority.card_priority_id;

                priorityList.Add(new System.Web.UI.WebControls.ListItem(priority.card_priority_name, priority.card_priority_id.ToString()));
            }

            return priorityList;
        }


        /// <summary>
        /// Search for a list of cards based on the parameteres provided. Null parameters wont be searched on.
        /// </summary>
        /// <param name="userId">If null, session user will be used.</param>
        /// <param name="cardNumber">May be null</param>
        /// <param name="distBatchReference">May be null</param>
        /// <param name="cardStatus">May be null</param>
        /// <param name="dateFrom">May be null</param>
        /// <param name="dateTo">May be null</param>
        /// <returns></returns>
        internal List<CardSearchResult> SearchForCards(long? userId, int? userRoleId, int? issuerId, int? branchId, string cardNumber,
                                                        string cardLastFourDigits, string cardrefnumber, string batchReference,
                                                        int? loadCardStatusId, int? distCardStatusId, int? branchCardStatusId, long? distBatchId, long? pinBatchId, long? threedBatchId,
                                                        string accountNumber, string firstName, string lastName, string cmsId,
                                                        DateTime? dateFrom, DateTime? dateTo, int? cardIssueMethodId,
                                                        int? productId, int? priorityId,
                                                        int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            List<DistBatchResult> rtnValue = new List<DistBatchResult>();

            var response = m_indigoApp.SearchForCards(userId, userRoleId, issuerId, branchId, cardNumber, cardLastFourDigits, cardrefnumber,
                                                            batchReference, loadCardStatusId, distCardStatusId, branchCardStatusId, distBatchId, pinBatchId, threedBatchId,
                                                            accountNumber, firstName, lastName, cmsId,
                                                            dateFrom, dateTo, cardIssueMethodId, productId, priorityId,
                                                            pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        internal List<CustomercardsearchResult> SearchCustomerCardsList(CardSearchParameters cardparms, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.SearchCustomerCardsList(cardparms.IssuerId,
                cardparms.BranchId,
                cardparms.ProductId,
                cardparms.CardIssueMethodId,
                cardparms.PriorityId,
                cardparms.CardNumber,
                cardparms.AccountNumber,
                cardparms.IsRenewalSearch, 
                cardparms.IsActivationSearch,
                pageIndex,
                StaticDataContainer.ROWS_PER_PAGE,
                encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }
        /// <summary>
        /// Search for a list of cards based on the parameteres provided. Null parameters wont be searched on.
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        internal List<CardSearchResult> SearchForCards(CardSearchParameters searchParams, int? pageIndex, int? rowsPerPage)
        {
            return this.SearchForCards(searchParams.UserId, searchParams.UserRoleId, searchParams.IssuerId, searchParams.BranchId, searchParams.CardNumber, searchParams.CardLastFourDigits, searchParams.CardRefNumber,
                                       searchParams.BatchReference, searchParams.LoadCardStatusId, searchParams.DistCardStatusId, searchParams.BranchCardStatusId, searchParams.DistBatchId, searchParams.PinbatchId, searchParams.ThreedBatchId,
                                       searchParams.AccountNumber, searchParams.FirstName, searchParams.LastName, searchParams.CmsId,
                                       searchParams.DateFrom, searchParams.DateTo, searchParams.CardIssueMethodId, searchParams.ProductId, searchParams.PriorityId,
                                       pageIndex ?? searchParams.PageIndex, rowsPerPage ?? searchParams.RowsPerPage);
        }

        /// <summary>
        /// Fetch all cards that are work in progress for the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<CardSearchResult> GetOperatorCardsInProgress(long? userId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            List<DistBatchResult> rtnValue = new List<DistBatchResult>();

            var response = m_indigoApp.GetOperatorCardsInProgress(userId, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }
        internal List<HybridRequestResult> GetOperatorHybridRequestsInProgress(int? status_id, long? userId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetOperatorHybridRequestsInProgress(status_id, userId, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }
        internal List<HybridRequestResult> GetRequestsByPrintBatch(long printbatchId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetRequestsByPrintBatch(printbatchId, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        /// <summary>
        /// Fetch all cards that have an exception status.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<CardSearchResult> GetCardsInError(long? userId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            List<DistBatchResult> rtnValue = new List<DistBatchResult>();

            var response = m_indigoApp.GetCardsInError(userId, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        ///// <summary>
        ///// If customer detail is linked to a card it will be returned.
        ///// </summary>
        ///// <param name="cardId"></param>
        ///// <param name="auditUserId"></param>
        ///// <param name="auditWorkstation"></param>
        ///// <returns></returns>
        //public CustomerAccountResult GetCustomerAccDetailForCard(long cardId)
        //{
        //    string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
        //                                                               SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
        //                                                               SecurityParameters.EXTERNAL_SECURITY_KEY);

        //    var response = _issuanceService.GetCustomerAccDetailForCard(cardId, encryptedSessionKey);

        //    if (response.ResponseType != ResponseType.SUCCESSFUL)
        //    {
        //        throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
        //    }

        //    if (response.Value.Length > 0)
        //    {
        //        return response.Value[0];
        //    }

        //    return null;
        //}

        /// <summary>
        /// Fetch all cards waiting to be linked to CMS.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<CardSearchResult> SearchForReissueCards(int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.SearchForReissueCards(pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        /// <summary>
        /// Get card detail, load batch detail, dist batch detail and customer account detail for a card.
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        internal CardDetails GetCardDetails(long cardId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCardDetails(cardId, null, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;
            else
                return null;
        }

        /// <summary>
        /// Search cards currently at a branch.
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="cardNumber"> May be null</param>
        /// <param name="branchCardStatus">May be null</param>
        /// <param name="operatorUserId">May be null</param>        
        /// <returns></returns>
        internal List<CardSearchResult> SearchBranchCards(int? issuerId, int? branchId, int? userRoleId, int? productId, int? priorityId, int? cardIssueMethodId,
                                                                  string cardNumber, int? branchCardStatusId, long? operatorUserId, int pageIndex, int rowsPerPage, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.SearchBranchCards(issuerId, branchId, userRoleId, productId, priorityId, cardIssueMethodId, cardNumber, branchCardStatusId, operatorUserId, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponseException(response, log, out messages))
                return response.Value.ToList();

            return new List<CardSearchResult>();
        }

        internal List<CardSearchResult> SearchBranchCards(BranchCardSearchParameters parms, int? pageIndex, int? rowsPerPage, out string messages)
        {
            return this.SearchBranchCards(parms.IssuerId, parms.BranchId, parms.UserRoleId, parms.ProductId,
                                            parms.PriorityId, parms.CardIssueMethodId, parms.CardNumber, parms.BranchCardStatusId, parms.OperatorUserId,
                                            pageIndex ?? parms.PageIndex, rowsPerPage ?? parms.RowsPerPage, out messages);
        }

        /// <summary>
        /// Search cards currently at a branch where the logged in users is the operator the cards have been checked out to.
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="cardNumber">May be null</param>
        /// <param name="branchCardStatus">May be null</param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <returns></returns>
        internal List<CardSearchResult> SearchBranchCards(int branchId, int? productId, int? priorityId, int? cardIssueMethodId,
                                                                 string cardNumber, int? branchCardStatusesId,
                                                                 int pageIndex, int rowsPerPage, out string messages)
        {
            //messages = "";
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            //var response = _issuanceService.SearchBranchCardsSelf(branchId, productId, priorityId, cardIssueMethodId, cardNumber, branchCardStatusesId, pageIndex, rowsPerPage, encryptedSessionKey);
            var response = m_indigoApp.SearchBranchCardsSelf(branchId, productId, priorityId, cardIssueMethodId, cardNumber, branchCardStatusesId, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log, out messages))
                return new List<CardSearchResult>(response.Value);
            else
                return new List<CardSearchResult>();
        }

        /// <summary>
        /// Persist checked in and out cards for an operator to the DB.
        /// </summary>
        /// <param name="operatorUserId"></param>
        /// <param name="checkedOutCards"></param>
        /// <param name="checkedInCards"></param>
        /// <returns></returns>
        internal List<SearchBranchCardsResult> CheckInOutCards(long operatorUserId, int branchId, int productId, List<long> checkedOutCards, List<long> checkedInCards)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CheckInOutCards(operatorUserId, branchId, productId, checkedOutCards.ToArray(), checkedInCards.ToArray(), encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();
            else
                return null;
        }

        /// <summary>
        /// Link Customer Account and Card in CMS
        /// </summary>
        /// <param name="customerDetails"></param>
        /// <returns></returns>
        internal bool LinkCardToAccount(long cardId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            PINResponse pinResp;
            var gotIndex = TerminalService.GetPINIndex(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey, out pinResp);

            var response = m_indigoApp.LinkCardToCustomer(cardId, gotIndex ? pinResp.PinIndex : new Byte[0], encryptedSessionKey);

            if (base.CheckResponseException(response, log, out responseMessage))
            {
                TerminalService.ClearPINIndex(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey);
                //SessionWrapper.PinIndex = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Mark issue card as ISSUED. Last status.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        internal bool IssueCardComplete(long cardId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.IssueCardComplete(cardId, encryptedSessionKey);

            return base.CheckResponseException(response, log, out responseMessage);
        }

        //internal bool PINReissue(int issuerId, int branchId, int productId, long? authoriseUserId, byte[] pinIndex, out string responseMessage)
        //{
        //    string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
        //                                                                 SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
        //                                                                 SecurityParameters.EXTERNAL_SECURITY_KEY);

        //    var response = m_indigoApp.PINReissue(issuerId, branchId, productId, authoriseUserId, pinIndex, encryptedSessionKey);

        //    return base.CheckResponseException(response, log, out responseMessage);
        //}

        internal List<issuedcardsreport_Result> Getissuecardreport(CardSearch cardsearch)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);
            var audits = new List<issuedcardsreport_Result>();

            var response = m_indigoApp.Getissuecardreport(cardsearch.Userid, cardsearch.DateFrom, cardsearch.DateTo, cardsearch.CardStatus, cardsearch.issuerid, cardsearch.branchid, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<PINReissueReportResult> GetPinReissueCardReport(CardSearch cardsearch)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetPinReissueReport(cardsearch.Userid, cardsearch.DateFrom, cardsearch.DateTo, cardsearch.CardStatus, cardsearch.issuerid, cardsearch.branchid, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<invetorysummaryreport_Result> GetInventorySummaryReport(CardSearch cardsearch)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetInventorySummaryReport(cardsearch.issuerid, cardsearch.branchid, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<invetorysummaryreport_Result> GetCardCenterInventorySummaryReport(CardSearch cardsearch)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetCardCenterInventorySummaryReport(cardsearch.issuerid, cardsearch.branchid, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<feerevenue_report_Result> GetFeeRevenueReport(CardSearch cardsearch)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetFeeRevenueReport(cardsearch.issuerid, cardsearch.branchid, cardsearch.DateFrom, cardsearch.DateTo, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<branchcardstock_report_Result> GetBranchCardStockReport(CardSearch cardsearch)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetBranchCardStockReport(cardsearch.issuerid, cardsearch.branchid, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<branchcardstock_report_Result> GetCardCenterStockReport(CardSearch cardsearch)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetCenterCardStockReport(cardsearch.issuerid, cardsearch.branchid, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<burnrate_report_Result> GetBurnRateReport(CardSearch cardsearch)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetBurnRateReport(cardsearch.issuerid, cardsearch.branchid, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<issuecardsummaryreport_Result> GetIssuecardsummaryreport(CardSearch cardsearch)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.Getissuecardsummaryreport(cardsearch.issuerid, cardsearch.branchid, cardsearch.DateFrom, cardsearch.DateTo, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<Spoilcardsummaryreport_Result> GetSpoilCardsummaryreport(CardSearch cardsearch)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetSpoilCardsummaryreport(cardsearch.issuerid, cardsearch.branchid, cardsearch.DateFrom, cardsearch.DateTo, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<spolicardsreport_Result> GetSpoilCardreport(CardSearch cardsearch)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetSpoilCardreport(cardsearch.issuerid, cardsearch.branchid, cardsearch.Userid, cardsearch.DateFrom, cardsearch.DateTo, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        #endregion

        #region Product Screen Methods

        internal List<LangLookup> LangLookupProductLoadTypes()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupProductLoadTypes(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal bool UpdateProduct(ProductResult product, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateProduct(product, encryptedSessionKey);

            return base.CheckResponseException(response, log, out messages);
        }

        internal bool InsertProduct(ProductResult product, out long productId, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.InsertProduct(product, encryptedSessionKey);
            productId = response.Value;

            return base.CheckResponseException(response, log, out messages);
        }

        internal string DeleteProduct(int Productid)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DeleteProduct(Productid, encryptedSessionKey);

            base.CheckResponse(response, log);

            return "";
        }

        internal string ActivateProduct(int Productid)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ActivateProduct(Productid, encryptedSessionKey);

            base.CheckResponse(response, log);

            return "";
        }

        internal List<ProductlistResult> GetProductsList(int issuerid, int? cardIssueMethodId, bool? deletedYN, int pageIndex, int RowsPerpage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetProductsList(issuerid, cardIssueMethodId, deletedYN, pageIndex, RowsPerpage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal bool GetProductsListValidated(int issuerid, int? cardIssueMethodId, int pageIndex, int RowsPerpage, out List<ProductValidated> productsList, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetProductsListValidated(issuerid, cardIssueMethodId, pageIndex, RowsPerpage, encryptedSessionKey);
            productsList = new List<ProductValidated>();

            if (response.Value != null)
                productsList = response.Value.ToList();

            return base.CheckResponse(response, log, out messages);
        }

        internal List<product_currency1> GetCurrencyByProduct(int Productid)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetCurreniesbyProduct(Productid, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<Issuer_product_font> GetFontFamilyList()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetFontFamilyList(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<ServiceRequestCode> GetServiceRequestCode1()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetServiceRequestCode1(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<ServiceRequestCode1> GetServiceRequestCode2()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetServiceRequestCode2(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<ServiceRequestCode2> GetServiceRequestCode3()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetServiceRequestCode3(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal ProductResult GetProduct(int Productid)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetProduct(Productid, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal List<DistBatchFlows> GetDistBatchFlows(int CardIssueMethodId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetDistBatchFlowList(CardIssueMethodId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal string UpdateFont(FontResult fontresult)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateFont(fontresult, encryptedSessionKey);

            base.CheckResponse(response, log);

            return "";
        }

        internal long? InsertFont(FontResult fontresult, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.InsertFont(fontresult, encryptedSessionKey);

            if (base.CheckResponseException(response, log, out messages))
                return response.Value;

            return null;
        }

        internal string DeleteSubProduct(int? Productid, int subproductid)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DeleteSubProduct((int)Productid, subproductid, encryptedSessionKey);

            base.CheckResponse(response, log);

            return "";
        }

        internal string DeleteFont(int fontid)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DeleteFont(fontid, encryptedSessionKey);

            base.CheckResponse(response, log);

            return "";
        }

        internal List<FontResult> GetFontlist(int pageIndex, int RowsPerpage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetFontlist(pageIndex, RowsPerpage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal FontResult GetFont(int fontid)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetFont(fontid, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }


        internal ProductFeeAccountingResult GetFeeAccounting(int feeAccountingId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetFeeAccounting(feeAccountingId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal List<ProductFeeAccountingResult> GetFeeAccountingList(int issuerId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetFeeAccountingList(issuerId, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal bool CreateFeeAccounting(ProductFeeAccountingResult feeAccountingDetail, out string messages, out ProductFeeAccountingResult resultAccountingDetail)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.CreateFeeAccounting(feeAccountingDetail, encryptedSessionKey);
            resultAccountingDetail = response.Value;


            return base.CheckResponse(response, log, out messages);
        }

        internal bool UpdateFeeAccounting(ProductFeeAccountingResult feeAccountingDetail, out string messages, out ProductFeeAccountingResult resultAccountingDetail)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.UpdateFeeAccounting(feeAccountingDetail, encryptedSessionKey);
            resultAccountingDetail = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        internal bool DeleteFeeAccounting(int feeAccountingId, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.DeleteFeeAccounting(feeAccountingId, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }


        internal List<FeeSchemeResult> GetFeeSchemes(int? issuerId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetFeeSchemes(issuerId, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal FeeSchemeDetails GetFeeSchemeDetails(int feeSchemeId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetFeeSchemeDetails(feeSchemeId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal List<FeeDetailResult> GetFeeDetails(int feeSchemeId, int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetFeeDetails(feeSchemeId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<FeeChargeResult> GetFeeCharges(int feeDetailId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetFeeCharges(feeDetailId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<ProductFeeDetailsResult> GetFeeDetailByProduct(int productId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetFeeDetailByProduct(productId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal ProductChargeResult GetCurrentFees(int feeDetailId, int currencyId, int CardIssueReasonId, string CBSAccountType)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetCurrentFees(feeDetailId, currencyId, CardIssueReasonId, CBSAccountType, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal bool UpdateFeeCharges(int feeDetailId, List<FeeChargeResult> fees, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.UpdateFeeCharges(feeDetailId, fees.ToArray(), encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }

        internal bool InsertFeeScheme(FeeSchemeDetails feeSchemeDetails, out string messages, out FeeSchemeDetails resultfeeschemedetails)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.InsertFeeScheme(feeSchemeDetails, encryptedSessionKey);
            resultfeeschemedetails = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        internal bool UpdateFeeScheme(FeeSchemeDetails feeSchemeDetails, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.UpdateFeeScheme(feeSchemeDetails, encryptedSessionKey);
            feeSchemeDetails = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        internal bool DeleteFeeScheme(int feeSchemeId, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.DeleteFeeScheme(feeSchemeId, encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }

        #region Printing Settings

        public bool CreateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.CreateProductPrintFields(productPrintFields.ToArray(), encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }

        internal List<ProductPrintFieldResult> GetProductPrintingFields(int? productId, int? cardId, int? requestId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                              SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                              SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetProductPrintFields(productId, cardId, requestId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal bool UpdateProductPrintingFields(List<ProductPrintFieldResult> productPrintFields, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                              SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                              SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.UpdateProductPrintFields(productPrintFields.ToArray(), encryptedSessionKey);

            return base.CheckResponse(response, log, out messages);
        }

        #endregion

        //#region Sub Product Screen Methods

        //internal bool UpdateSubProduct(SubProduct_Result subproductlist, out string messages)
        //{
        //    string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
        //                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
        //                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);

        //    var response = _issuanceService.UpdateSubProduct(subproductlist, encryptedSessionKey);
        //    messages = response.ResponseMessage;

        //    if (response.ResponseType == ResponseType.SUCCESSFUL)
        //    {
        //        return true;
        //    }
        //    else if (response.ResponseType == ResponseType.UNSUCCESSFUL)
        //    {
        //        return false;
        //    }
        //    if (response.ResponseType != ResponseType.SUCCESSFUL)
        //    {
        //        throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
        //    }

        //    return false;
        //}

        //internal bool InsertSubProduct(SubProduct_Result subproductlist, out string messages)
        //{
        //    string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
        //                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
        //                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);

        //    var response = _issuanceService.InsertSubProduct(subproductlist, encryptedSessionKey);
        //    messages = response.ResponseMessage;


        //    if (response.ResponseType == ResponseType.SUCCESSFUL)
        //    {
        //        return true;
        //    }
        //    else if (response.ResponseType == ResponseType.UNSUCCESSFUL)
        //    {
        //        return false;
        //    }
        //    else if (response.ResponseType != ResponseType.SUCCESSFUL)
        //    {
        //        throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
        //    }

        //    return false;
        //}

        //internal List<SubProduct_Result> GetSubProductsList(int issuerid, int? productid, int? cardIssueMethidId, Boolean? deleteYN, int pageIndex, int RowsPerpage)
        //{
        //    string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
        //                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
        //                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);


        //    var response = _issuanceService.GetSubProductList(issuerid, productid, cardIssueMethidId, deleteYN, pageIndex, RowsPerpage, encryptedSessionKey);

        //    if (response.ResponseType != ResponseType.SUCCESSFUL)
        //    {
        //        throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
        //    }

        //    return new List<SubProduct_Result>(response.Value);
        //}

        //internal SubProduct_Result GetSubProduct(int? Productid, int Sub_Product_id)
        //{
        //    string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
        //                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
        //                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);


        //    var response = _issuanceService.GetSubProduct(Productid, Sub_Product_id, encryptedSessionKey);

        //    if (response.ResponseType != ResponseType.SUCCESSFUL)
        //    {
        //        throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
        //    }

        //    return response.Value;
        //}

        //#endregion

        internal List<auditreport_usergroup_Result> GetUsersByRoles_AuditReport(int? issuer_id, int? user_group_id, int? user_role_id, int? user_id)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetUsersByRoles_AuditReport(issuer_id, user_group_id, user_role_id, user_id, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<auditreport_usergroup_Result> GetBranchesperusergroup_AuditReport(int? issuer_id, int? user_group_id, int? branch_id, int? role_id)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetBranchesperusergroup_AuditReport(issuer_id, user_group_id, branch_id, role_id, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<auditreport_usergroup_Result> GetUserGroup_AuditReport(int? issuer_id, int? user_group_id, int? user_role_id)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetUserGroup_AuditReport(issuer_id, user_group_id, user_role_id, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        #endregion

        internal List<pin_block_formatResult> LookupPinBlockFormat()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.LookupPinBlockFormat(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }


        internal List<LangLookup> LookupPrintFieldTypes()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.LookupPrintFieldTypes(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

    }
}
