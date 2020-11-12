using System;
using System.Collections.Generic;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.CardManagement.Reports;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement
{
    public class LoadBatchMangementService
    {
        private readonly LoadBatchManagementDAL _loadBatchDAL = new LoadBatchManagementDAL();
        private readonly ResponseTranslator _translator = new ResponseTranslator();

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
        public List<LoadBatchResult> GetLoadBatches(string loadBatchReference, int issuerId, LoadBatchStatus? loadBatchStatus, DateTime? startDate, DateTime? endDate,
                                                     int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            return _loadBatchDAL.GetLoadBatches(loadBatchReference, issuerId, loadBatchStatus, startDate, endDate, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation);
        }

        /// <summary>
        /// Return a single load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <returns></returns>
        public LoadBatchResult GetLoadBatch(long loadBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            return _loadBatchDAL.GetLoadBatch(loadBatchId, languageId, auditUserId, auditWorkStation);
        }

        /// <summary>
        /// Return a list of cards linked to the specified load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <returns></returns>
        public List<LoadCardResult> GetLoadBatchCards(long loadBatchId, long auditUserId, string auditWorkStation)
        {
            return _loadBatchDAL.GetLoadBatchCards(loadBatchId, auditUserId, auditWorkStation);
        }

        /// <summary>
        /// Fetch a load batch status history.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public List<LoadBatchHistoryResult> GetLoadBatchHistory(long loadBatchId,int languageId, long auditUserId, string auditWorkStation)
        {
            return _loadBatchDAL.GetLoadBatchHistory(loadBatchId, languageId,auditUserId, auditWorkStation);
        }

        /// <summary>
        /// Approve the load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public bool ApproveLoadBatch(long loadBatchId, string notes, int language, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            var response = _loadBatchDAL.ApproveLoadBatch(loadBatchId, notes, auditUserId, auditWorkStation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.LOAD_BATCH_APPROVE,
                                                                language, auditUserId, auditWorkStation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reject the load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public bool RejectLoadBatch(long loadBatchId, string notes, int language, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            SystemResponseCode response = SystemResponseCode.GENERAL_FAILURE;
            if (String.IsNullOrWhiteSpace(notes))
            {                
                response = SystemResponseCode.PARAMETER_ERROR;
            }
            else
            {
                response = _loadBatchDAL.RejectLoadBatch(loadBatchId, notes, auditUserId, auditWorkStation);
            }
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.LOAD_BATCH_REJECT,
                                                                language, auditUserId, auditWorkStation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generate card list PDF report for load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public byte[] GenerateLoadBatchReport(long loadBatchId,int languageId, string username, long auditUserId, string auditWorkStation)
        {
            LoadBatchReports loadBatchReports = new LoadBatchReports();

            return loadBatchReports.GenerateLoadBatchReport(loadBatchId,languageId, username, auditUserId, auditWorkStation);
        }

        public byte[] GeneratePinMailerBatchReport(int pinBatchHeaderId, int languageId, string username, long auditUserId, string auditWorkStation)
        {
            LoadBatchReports loadBatchReports = new LoadBatchReports();

            return loadBatchReports.GeneratePinFileBatchReport(pinBatchHeaderId, languageId, username, auditUserId, auditWorkStation);
        }

        /// <summary>
        /// Get a list of file_load
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<FileLoadResult> GetFileLoadList(DateTime dateFrom, DateTime dateTo, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            return _loadBatchDAL.GetFileLoadList(dateFrom, dateTo, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
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
        public List<FileHistoryResult> GetFileHistorys(int? issuerId, DateTime dateFrom, DateTime dateTo, long auditUserId, string auditWorkstation)
        {
            return _loadBatchDAL.GetFileHistorys(issuerId, dateFrom, dateTo, auditUserId, auditWorkstation);
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
            return _loadBatchDAL.GetFileHistory(fileId, auditUserId, auditWorkstation);
        }
    }
}