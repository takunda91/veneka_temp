using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.Renewal.dal;

namespace Veneka.Indigo.Renewal.Reports
{
    public class RenewalBatchReport : ReportsCommon
    {
        public byte[] GenerateReport(long renewalBatchId, int languageId, string username, long auditUserId, string auditWorkstation)
        {
            IRenewalDataAccess dataAccess = new RenewalDataAccess();
            var header = dataAccess.RetrieveBatch(renewalBatchId, auditUserId, auditWorkstation);
            var details = dataAccess.RetrieveBatchDetails(renewalBatchId, true, languageId, auditUserId, auditWorkstation);

            //need to use the delivery branch as a group
            var deliveryBranches = from p in details
                                   group p by p.DeliveryBranchId into g
                                   select new { DeliveryBranchId = g.Key };

            var result = dataAccess.GetReportFields(30, languageId);

            List<List<string[]>> reportHeaders = new List<List<string[]>>();
            List<List<string[]>> reportDetails = new List<List<string[]>>();

            foreach (var delBranch in deliveryBranches)
            {
                var headerTable = new List<string[]>();
                var detailTable = new List<string[]>();

                Dictionary<int, string> reportfields = new Dictionary<int, string>();

                foreach (var item in result)
                {
                    reportfields.Add(item.FieldId, item.FieldText);
                }

                List<ReportField> headerFields = new List<ReportField>();
                headerFields.Add(result.Where(p => p.FieldId == 28).FirstOrDefault());
                headerFields.Add(result.Where(p => p.FieldId == 105).FirstOrDefault());
                headerFields.Add(result.Where(p => p.FieldId == 120).FirstOrDefault());
                headerFields.Add(result.Where(p => p.FieldId == 5).FirstOrDefault());
                headerFields.Add(result.Where(p => p.FieldId == 121).FirstOrDefault());

                headerTable.Add(headerFields.Select(p => p.FieldText).ToArray());
                string branchCode = "000";
                string branchName = "XXXXXXXXXX";
                int cardCount = 0;
                cardCount = details.Where(p => p.DeliveryBranchId == delBranch.DeliveryBranchId).ToList().Count;
                if (cardCount > 0)
                {
                    var branchToDeliver = details.Where(p => p.DeliveryBranchId == delBranch.DeliveryBranchId).FirstOrDefault();
                    branchCode = branchToDeliver.DeliveryBranchCode;
                    branchName = branchToDeliver.DeliveryBranchName;

                    headerTable.Add(new[] {
                        $"{branchCode} {branchName}",
                        $"{header.ProductCode} {header.ProductName}",
                        header.CalculatedBatchNumber,
                        header.RenewalBatchStatus.ToString(),
                        cardCount.ToString()
                    });
                }

                List<ReportField> detailFields = new List<ReportField>();
                detailFields.Add(result.Where(p => p.FieldId == 7).FirstOrDefault());
                //detailFields.Add(result.Where(p => p.FieldId == 82).FirstOrDefault());
                detailFields.Add(result.Where(p => p.FieldId == 108).FirstOrDefault());
                detailFields.Add(result.Where(p => p.FieldId == 80).FirstOrDefault());
                detailFields.Add(result.Where(p => p.FieldId == 119).FirstOrDefault());
                detailFields.Add(result.Where(p => p.FieldId == 118).FirstOrDefault());

                detailTable.Add(detailFields.Select(p => p.FieldText).ToArray());

                foreach (var record in details.Where(p => p.DeliveryBranchId == delBranch.DeliveryBranchId))
                {
                    detailTable.Add(new string[] {
                    record.CardNumber,
                    //record.ExpiryDate.HasValue? record.ExpiryDate.Value.ToString(StaticFields.DATE_FORMAT):string.Empty,
                    record.CurrencyCode,
                    record.EmbossingName,
                    record.PassportIDNumber,
                    record.RenewalStatus.ToString()});
                }
                reportHeaders.Add(headerTable);
                reportDetails.Add(detailTable);
            }
            float[] headerWidths = new float[] { 200, 250, 150, 150, 50 };
            float[] detailWidths = new float[] { 150, 100, 150, 150, 100 };
            return CreateRenewalFileReportPDF(reportHeaders.ToArray(), reportDetails.ToArray(), username, headerWidths, detailWidths, result);
        }

        private byte[] CreateRenewalFileReportPDF(List<string[]>[] headerTable, List<string[]>[] batchStatusTableData, string userName, float[] headerTable_colWidths, float[] StatusTable_colWidths, List<ReportField> reportfields)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                var document = new Document(PageSize.A4);
                document.AddCreationDate();

                string creatorName = reportfields.Where(p => p.FieldId == 0).FirstOrDefault().FieldText;
                document.AddCreator(creatorName);
                document.AddAuthor(creatorName);
                document.AddTitle(reportfields.Where(p => p.FieldId == 117).FirstOrDefault().FieldText);
                //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfFile, FileMode.Create));
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                writer.CompressionLevel = 9;

                string reportHeader = reportfields.Where(p => p.FieldId == 117).FirstOrDefault().FieldText;
                document.Open();
                document.Add(new Phrase(reportHeader, new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                string labelText = reportfields.Where(p => p.FieldId == 2).FirstOrDefault().FieldText;
                document.Add(new Phrase($"{labelText} {DateTime.Now.ToString(StaticFields.DATE_FORMAT)}", new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));

                document.Add(new Phrase(Environment.NewLine));

                labelText = reportfields.Where(p => p.FieldId == 6).FirstOrDefault().FieldText;
                document.Add(new Phrase($"{labelText} {userName}", new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));

                for (int i = 0; i < headerTable.Length; i++)
                {
                    PdfPTable header = CreateTable(headerTable[i], headerTable_colWidths);

                    document.Add(header);
                    document.Add(CreateTable(batchStatusTableData[i], StatusTable_colWidths));
                    document.Add(new Phrase(Environment.NewLine));
                }

                document.Close();

                writer.Flush();

                return ms.ToArray();
            }
        }
    }
}
