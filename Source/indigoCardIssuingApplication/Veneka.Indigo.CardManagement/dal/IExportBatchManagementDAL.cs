using System;
using System.Collections.Generic;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement.dal
{
    public interface IExportBatchManagementDAL
    {
        SystemResponseCode ApproveExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result);
        SystemResponseCode GenerateExportBatches(int issuerId, int? productId, int languageId, long auditUserId, string auditWorkstation, out List<CreatedExportBatchResult> exportBatches);
        ExportBatchResult GetExportBatch(long exportBatchId, int languageId, long auditUserId, string auditWorkstation);
        List<ExportBatchCardResult> GetExportBatchCards(long exportBatchId, long auditUserId, string auditWorkStation);
        List<ExportBatchHistoryResult> GetExportBatchHistory(long exportBatchId, int languageId, long auditUserId, string auditWorkstation);
        issuer GetIssuerByCode(string issuerCode, int languageId, long auditUserId, string auditWorkstation);
        SystemResponseCode RejectExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result);
        SystemResponseCode RequestExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result);
        List<ExportBatchResult> SearchExportBatch(int? issuerId, int? productId, int? exportBatchStatusesId, string batchReference, DateTime? dateFrom, DateTime? dateTo, int pageIndex, int rowsPerPage, int languageId, long auditUserId, string auditWorkstation);
        List<ExportBatchCardDetailsResult> GetExportBatchCardsDetailed(long exportBatchId, long auditUserId, string auditWorkStation);
    }
}