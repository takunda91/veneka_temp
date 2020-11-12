using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement.dal
{
    public class ExportBatchManagementDAL : IExportBatchManagementDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        public SystemResponseCode GenerateExportBatches(int issuerId, int? productId, int languageId, long auditUserId, string auditWorkstation, out List<CreatedExportBatchResult> exportBatches)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<CreatedExportBatchResult> results = context.usp_create_export_batches(issuerId, auditUserId, auditWorkstation, ResultCode);

                exportBatches = results.ToList();

                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }

        public issuer GetIssuerByCode(string issuerCode, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<issuer> results = context.usp_get_issuer_by_code(issuerCode, auditUserId, auditWorkstation);

                return results.FirstOrDefault();
            }
        }

        public SystemResponseCode ApproveExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ExportBatchResult> results = context.usp_export_batch_status_approve(exportBatchId, false, languageId, auditUserId, auditWorkstation, ResultCode);

                result = results.First();

                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }

        public SystemResponseCode RejectExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ExportBatchResult> results = context.usp_export_batch_status_reject(exportBatchId, notes, languageId, auditUserId, auditWorkstation, ResultCode);

                result = results.First();

                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }

        public SystemResponseCode RequestExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ExportBatchResult> results = context.usp_export_batch_status_request(exportBatchId, notes, languageId, auditUserId, auditWorkstation, ResultCode);

                result = results.First();

                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }

        public List<ExportBatchResult> SearchExportBatch(int? issuerId, int? productId, int? exportBatchStatusesId, 
                                                            string batchReference, DateTime? dateFrom, DateTime? dateTo,
                                                            int pageIndex, int rowsPerPage,
                                                            int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ExportBatchResult> results = context.usp_search_export_batches_paged(issuerId, productId,
                                        exportBatchStatusesId, batchReference, dateFrom, dateTo, pageIndex, rowsPerPage,
                                        languageId, auditUserId, auditWorkstation);

                return results.ToList();                
            }
        }

        public ExportBatchResult GetExportBatch(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ExportBatchResult> result = context.usp_get_export_batch(exportBatchId, languageId, auditUserId, auditWorkstation);

                return result.First();
            }
        }

        public List<ExportBatchHistoryResult> GetExportBatchHistory(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
        
            List<ExportBatchHistoryResult> rtnValue = new List<ExportBatchHistoryResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ExportBatchHistoryResult> results = context.usp_get_export_batch_history(exportBatchId, languageId, auditUserId, auditWorkstation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }
            return rtnValue;
        }

        public List<ExportBatchCardResult> GetExportBatchCards(long exportBatchId, long auditUserId, string auditWorkStation)
        {
            List<ExportBatchCardResult> rtnValue = new List<ExportBatchCardResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ExportBatchCardResult> results = context.usp_get_export_batch_cards(exportBatchId, auditUserId, auditWorkStation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        public List<ExportBatchCardDetailsResult> GetExportBatchCardsDetailed(long exportBatchId, long auditUserId, string auditWorkStation)
        {
            List<ExportBatchCardDetailsResult> rtnValue = new List<ExportBatchCardDetailsResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ExportBatchCardDetailsResult> results = context.usp_get_export_batch_card_details(exportBatchId, auditUserId, auditWorkStation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }
    }
}
