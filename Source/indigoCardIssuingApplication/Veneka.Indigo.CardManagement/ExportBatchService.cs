using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.CardManagement.Reports;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement
{
   public class ExportBatchService
    {
       private readonly IExportBatchManagementDAL _exportBatchDAL;
       private readonly IResponseTranslator _translator;
       private readonly ExportBatchReports _exportreport = new ExportBatchReports();

        public ExportBatchService() 
            : this(new ExportBatchManagementDAL(), new ResponseTranslator())
        {  }

        public ExportBatchService(IExportBatchManagementDAL exportBatchManagementDAL, IResponseTranslator responseTranslator)
        {
            _exportBatchDAL = exportBatchManagementDAL;
            _translator = responseTranslator;
        }

        public bool GenerateExportBatches(string issuerCode, int languageId, long auditUserId, string auditWorkstation, out List<CreatedExportBatchResult> exportBatches, out string responseMessage)
        {          
            //Check the issuer
            var exportIssuer = _exportBatchDAL.GetIssuerByCode(issuerCode, languageId, auditUserId, auditWorkstation);

            if(exportIssuer == null || (exportIssuer != null && exportIssuer.issuer_status_id != 0))
            {
                exportBatches = null;
                responseMessage = string.Format("Invalid issuer({0}), please contact support.", issuerCode);
                return false;
            }

            return this.GenerateExportBatches(exportIssuer.issuer_id, null, languageId, auditUserId, auditWorkstation, out exportBatches, out responseMessage);
        }

        public bool GenerateExportBatches(int issuerId, int? productId, int languageId, long auditUserId, string auditWorkstation, out List<CreatedExportBatchResult> exportBatches, out string responseMessage)
        {
            var resultCode = _exportBatchDAL.GenerateExportBatches(issuerId, productId, languageId, auditUserId, auditWorkstation, out exportBatches);

            responseMessage = _translator.TranslateResponseCode(resultCode, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (resultCode == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

       public bool ApproveExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result, out string responseMessage)
       {
           if (String.IsNullOrWhiteSpace(notes))           
               throw new ArgumentNullException("notes", "Notes cannot be null or empty.");


           var resultCode = _exportBatchDAL.ApproveExportBatch(exportBatchId, notes, languageId, auditUserId, auditWorkstation, out result);

           responseMessage = _translator.TranslateResponseCode(resultCode, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

           if (resultCode == SystemResponseCode.SUCCESS)
           {
               return true;
           }

           return false;
       }

       public bool RejectExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result, out string responseMessage)
       {
           if (String.IsNullOrWhiteSpace(notes))
               throw new ArgumentNullException("notes", "Notes cannot be null or empty.");


           var resultCode = _exportBatchDAL.RejectExportBatch(exportBatchId, notes, languageId, auditUserId, auditWorkstation, out result);

           responseMessage = _translator.TranslateResponseCode(resultCode, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

           if (resultCode == SystemResponseCode.SUCCESS)
           {
               return true;
           }

           return false;
       }

       public bool RequestExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result, out string responseMessage)
       {
           if (String.IsNullOrWhiteSpace(notes))
               throw new ArgumentNullException("notes", "Notes cannot be null or empty.");


           var resultCode = _exportBatchDAL.RequestExportBatch(exportBatchId, notes, languageId, auditUserId, auditWorkstation, out result);

           responseMessage = _translator.TranslateResponseCode(resultCode, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

           if (resultCode == SystemResponseCode.SUCCESS)
           {
               return true;
           }

           return false;
       }

       public List<ExportBatchResult> SearchExportBatch(int? issuerId, int? productId, int? exportBatchStatusesId,
                                                            string batchReference, DateTime? dateFrom, DateTime? dateTo,
                                                            int pageIndex, int rowsPerPage,
                                                            int languageId, long auditUserId, string auditWorkstation)
       {
           return _exportBatchDAL.SearchExportBatch(issuerId, productId, exportBatchStatusesId, batchReference, dateFrom, 
                                                        dateTo, pageIndex, rowsPerPage, languageId, auditUserId, auditWorkstation);
       }

       public ExportBatchResult GetExportBatch(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
       {
           return _exportBatchDAL.GetExportBatch(exportBatchId, languageId, auditUserId, auditWorkstation);
       }
       public List<ExportBatchHistoryResult> GetExportBatchHistory(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
       {
           return _exportBatchDAL.GetExportBatchHistory(exportBatchId, languageId, auditUserId, auditWorkstation);
       }
       public List<ExportBatchCardResult> GetExportBatchCards(long exportBatchId, long auditUserId, string auditWorkstation)
       {
           return _exportBatchDAL.GetExportBatchCards(exportBatchId, auditUserId, auditWorkstation);
       }

        public List<ExportBatchCardDetailsResult> GetExportBatchCardsDetailed(long exportBatchId, long auditUserId, string auditWorkstation)
        {
            return _exportBatchDAL.GetExportBatchCardsDetailed(exportBatchId, auditUserId, auditWorkstation);
        }

        #region PDF Reports

       /// <summary>
       /// Generate card list PDF report for distribution batch.
       /// </summary>
       /// <param name="distBatchId"></param>
       /// <param name="auditUserId"></param>
       /// <param name="auditWorkStation"></param>
       /// <returns></returns>
       public byte[] GenerateExportBatchReport(long exportBatchId, int languageId, string username, long auditUserId, string auditWorkStation)
       {
           return _exportreport.GenerateExportBatchReport(exportBatchId, languageId, username, auditUserId, auditWorkStation);
       }

       #endregion
    }
}
