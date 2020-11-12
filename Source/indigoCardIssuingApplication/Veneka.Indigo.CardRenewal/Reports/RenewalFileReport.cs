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
    public class RenewalFileReport : ReportsCommon
    {
        public byte[] GenerateReport(long renewalId, int languageId, string username, long auditUserId, string auditWorkstation)
        {
            IRenewalDataAccess dataAccess = new RenewalDataAccess();
            var header = dataAccess.Retrieve(renewalId, languageId, auditUserId, auditWorkstation);
            var details = dataAccess.ListRenewalDetail(renewalId, true, languageId, auditUserId, auditWorkstation);

            var headerTable = new List<string[]>();
            var detailTable = new List<string[]>();

            var result = dataAccess.GetReportFields(29, languageId);
            Dictionary<int, string> reportfields = new Dictionary<int, string>();

            foreach (var item in result)
            {
                reportfields.Add(item.FieldId, item.FieldText);
            }

            List<ReportField> headerFields = new List<ReportField>();
            headerFields.Add(result.Where(p => p.FieldId == 116).FirstOrDefault());
            headerFields.Add(result.Where(p => p.FieldId == 4).FirstOrDefault());
            
            headerTable.Add(headerFields.Select(p => p.FieldText).ToArray());
            headerTable.Add(new[] { header.FileName, header.CreateDate.ToString(StaticFields.DATETIME_FORMAT) });
            
            List<ReportField> detailFields = new List<ReportField>();
            detailFields.Add(result.Where(p => p.FieldId == 29).FirstOrDefault());
            detailFields.Add(result.Where(p => p.FieldId == 28).FirstOrDefault());
            detailFields.Add(result.Where(p => p.FieldId == 7).FirstOrDefault());
            //detailFields.Add(result.Where(p => p.FieldId == 82).FirstOrDefault());
            detailFields.Add(result.Where(p => p.FieldId == 70).FirstOrDefault());
            detailFields.Add(result.Where(p => p.FieldId == 80).FirstOrDefault());

            detailTable.Add(detailFields.Select(p => p.FieldText).ToArray());

            foreach (var record in details)
            {
                detailTable.Add(new string[] { 
                    record.BranchCode,
                    record.BranchName, 
                    record.CardNumber,
                    //record.ExpiryDate.HasValue? record.ExpiryDate.Value.ToString(StaticFields.DATE_FORMAT):string.Empty,
                    record.ExternalAccountNumber, 
                    record.CustomerName });
            }

            float[] headerWidths = new float[] { 450, 450 };
            float[] detailWidths = new float[] { 100, 150, 150, 150, 250 };
            return CreateRenewalFileReportPDF(headerTable, detailTable, username, headerWidths, detailWidths, result);
        }

        private byte[] CreateRenewalFileReportPDF(List<string[]> headerTable, List<string[]> batchStatusTableData, string userName, float[] headerTable_colWidths, float[] StatusTable_colWidths, List<ReportField> reportfields)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                var document = new Document(PageSize.A4);
                document.AddCreationDate();

                string creatorName = reportfields.Where(p => p.FieldId == 0).FirstOrDefault().FieldText;
                document.AddCreator(creatorName);
                document.AddAuthor(creatorName);
                document.AddTitle(reportfields.Where(p => p.FieldId == 115).FirstOrDefault().FieldText);
                //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfFile, FileMode.Create));
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                writer.CompressionLevel = 9;

                string reportHeader = reportfields.Where(p => p.FieldId == 115).FirstOrDefault().FieldText;
                document.Open();
                document.Add(new Phrase(reportHeader, new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                string labelText = reportfields.Where(p => p.FieldId == 2).FirstOrDefault().FieldText;
                document.Add(new Phrase($"{labelText} {DateTime.Now.ToString(StaticFields.DATE_FORMAT)}", new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));

                document.Add(new Phrase(Environment.NewLine));

                labelText = reportfields.Where(p => p.FieldId == 6).FirstOrDefault().FieldText;
                document.Add(new Phrase($"{labelText} {userName}", new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));

                document.Add(CreateTable(headerTable, headerTable_colWidths));
                document.Add(new Phrase(Environment.NewLine));

                document.Add(CreateTable(batchStatusTableData, StatusTable_colWidths));
                document.Close();

                writer.Flush();

                return ms.ToArray();
            }
        }
    }
}
