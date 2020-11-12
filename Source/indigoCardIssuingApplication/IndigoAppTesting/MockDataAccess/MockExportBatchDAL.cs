using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;

namespace IndigoAppTesting.IndigoCardIssuanceSerrvice.Remote
{
    public class MockExportBatchDAL : IExportBatchManagementDAL
    {        
        public SystemResponseCode ApproveExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode GenerateExportBatches(int issuerId, int? productId, int languageId, long auditUserId, string auditWorkstation, out List<CreatedExportBatchResult> exportBatches)
        {
            exportBatches = null;

            if(issuerId == 0)
            {
                exportBatches = new List<CreatedExportBatchResult>
                {
                    new CreatedExportBatchResult
                    {
                        batch_reference = "TestABCD-1",
                        date_created = DateTime.Now,
                        export_batch_id = 1,
                        issuer_id = issuerId,
                        no_cards = 10
                    },

                    new CreatedExportBatchResult
                    {
                        batch_reference = "TestABCD-2",
                        date_created = DateTime.Now,
                        export_batch_id = 2,
                        issuer_id = issuerId,
                        no_cards = 50
                    }
                };

                return SystemResponseCode.SUCCESS;
            }

            return SystemResponseCode.GENERAL_FAILURE;
        }

        public ExportBatchResult GetExportBatch(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<ExportBatchCardResult> GetExportBatchCards(long exportBatchId, long auditUserId, string auditWorkStation)
        {
            List<ExportBatchCardResult> cards = new List<ExportBatchCardResult>();

            for (int i = 0; i < 10; i++)
            {
                cards.Add(new ExportBatchCardResult
                {
                    
                });
            }

            return cards;
        }

        public List<ExportBatchCardDetailsResult> GetExportBatchCardsDetailed(long exportBatchId, long auditUserId, string auditWorkStation)
        {
            int no = 10;
            int productId = 0;

            switch (exportBatchId)
            {
                case 0: no = 10; break;
                case 1: no = 50; break;
                default:
                    break;
            }
            List<ExportBatchCardDetailsResult> cards = new List<ExportBatchCardDetailsResult>();

            for (int i = 0; i < no; i++)
            {
                if (i % 10 == 0)
                    productId++;

                cards.Add(new ExportBatchCardDetailsResult
                {
                    issuer_code = "001",
                    product_id = productId,                    
                    card_number = exportBatchId.ToString() + i.ToString().PadLeft(15, '0'),
                    customer_first_name = "FirstName" + i,
                    customer_middle_name = "MiddleName" + i,
                    customer_last_name = "LastName" + i,
                    card_reference_number = i.ToString("000000000000000"),
                    product_code = (i + 100).ToString("000"),
                    name_on_card = "Name On Card" + i
                });
            }

            return cards;
        }

        public List<ExportBatchHistoryResult> GetExportBatchHistory(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public issuer GetIssuerByCode(string issuerCode, int languageId, long auditUserId, string auditWorkstation)
        {
            if (issuerCode == "001")
                return new issuer
                {
                    authorise_pin_issue_YN = false,
                    authorise_pin_reissue_YN = false,
                    back_office_pin_auth_YN = false,
                    card_ref_preference = true,
                    classic_card_issue_YN = false,
                    country_id = 0,
                    enable_instant_pin_YN = false,
                    instant_card_issue_YN = true,
                    issuer_code = issuerCode,
                    issuer_id = 0,
                    issuer_name = "TestIssuer",
                    issuer_status_id = 0,
                    language_id = 0,
                    maker_checker_YN = false

                };

            return null;
        }

        public SystemResponseCode RejectExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode RequestExportBatch(long exportBatchId, string notes, int languageId, long auditUserId, string auditWorkstation, out ExportBatchResult result)
        {
            throw new NotImplementedException();
        }

        public List<ExportBatchResult> SearchExportBatch(int? issuerId, int? productId, int? exportBatchStatusesId, string batchReference, DateTime? dateFrom, DateTime? dateTo, int pageIndex, int rowsPerPage, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }
    }
}
