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
   public class ThreedBatchManagementService
    {
        private readonly ThreedBatchManagementDAL _threedBatchDAL = new ThreedBatchManagementDAL();
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
        public List<ThreedBatchResult> GetThreedBatches(string ThreedBatchReference, int issuerId, ThreedBatchStatus? ThreedBatchStatus, DateTime? startDate, DateTime? endDate,
                                                     int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            return _threedBatchDAL.GetThreedBatches(ThreedBatchReference, issuerId, ThreedBatchStatus, startDate, endDate, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation);
        }

        /// <summary>
        /// Return a single load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <returns></returns>
        public ThreedBatchResult GetThreedBatch(long ThreedBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            return _threedBatchDAL .GetThreedBatch(ThreedBatchId, languageId, auditUserId, auditWorkStation);
        }

        public bool RecreateThreedBatch(long ThreedBatchId, string notes, int language, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            var response = _threedBatchDAL.RecreateThreedBatch(ThreedBatchId,(int)ThreedBatchStatus.RECREATED, notes, language, auditUserId, auditWorkStation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.LOAD_BATCH_APPROVE,
                                                                language, auditUserId, auditWorkStation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;

        }
    }
}
