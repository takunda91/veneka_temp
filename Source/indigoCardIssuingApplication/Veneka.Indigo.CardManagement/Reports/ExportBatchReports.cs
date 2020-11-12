extern alias textsharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using textsharp::iTextSharp.text;
using textsharp::iTextSharp.text.pdf;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.Common.Utilities;

namespace Veneka.Indigo.CardManagement.Reports
{
   public class ExportBatchReports
    {

        private readonly ExportBatchManagementDAL _exportBatchDAL = new ExportBatchManagementDAL();
        /// <summary>
        /// Generates a PDF report for a distribution batch returned as an array of bytes.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public byte[] GenerateExportBatchReport(long distBatchId, int languageId, string username, long auditUserId, string auditWorkstation)
        {
            var exportBatchHistory = _exportBatchDAL.GetExportBatchHistory(distBatchId, languageId, auditUserId, auditWorkstation);
            var exportBatchCards = _exportBatchDAL.GetExportBatchCards(distBatchId, auditUserId, auditWorkstation);

            var exportCardTableData = new List<string[]>();
            var exportBatchStatusTableData = new List<string[]>();

            ReportManagementService reportservice = new ReportManagementService();
            var result = reportservice.GetReportFields(6, languageId);
            Dictionary<int, string> reportfields = new Dictionary<int, string>();

            int i = 0;
            foreach (var item in result)
            {
                reportfields.Add(i, result[i].language_text);
                i++;
            }

            //distBatchStatusTableData.Add(new[] { "Date", "Batch Status", "User" });
            exportBatchStatusTableData.Add(new[] { reportfields[4], reportfields[5], reportfields[6], reportfields[8] });

            foreach (var record in exportBatchHistory)
            {
                exportBatchStatusTableData.Add(new string[]{((DateTime)record.status_date).ToString(StaticFields.DATETIME_FORMAT), 
                                                          record.export_batch_status_name, 
                                                         record.username,
                                                          record.comments});
            }

            //distCardTableData.Add(new[] { "Card Number" });
            exportCardTableData.Add(new[] { reportfields[11], reportfields[7], reportfields[9] });
            int sno = 1;
            foreach (var card in exportBatchCards)
            {
                //distCardTableData.Add(new[] { DisplayPartialPAN(card.card_number) });
                exportCardTableData.Add(new[] { sno.ToString(), card.card_number, card.card_reference_number });
                sno++;
            }


            return createPDFReport(exportCardTableData, exportBatchStatusTableData,
                            exportBatchHistory[exportBatchHistory.Count - 1].batch_reference,
                            exportBatchHistory[exportBatchHistory.Count - 1].export_batch_status_name,
                            username, exportCardTableData.Count() - 1, new float[] { 100, 400, 400 },
                            new float[] { 500, 500, 500, 500 }, reportfields);
        }


        /// <summary>
        /// Creates the PDF document.
        /// </summary>
        /// <param name="cardTableData"></param>
        /// <param name="batchStatusTableData"></param>
        /// <param name="reportHeader"></param>
        /// <param name="batchStatus"></param>
        /// <param name="userName"></param>
        /// <param name="CardTable_colWidths"></param>
        /// <param name="StatusTable_colWidths"></param>
        /// <returns></returns>
        private byte[] createPDFReport(List<string[]> cardTableData, List<string[]> batchStatusTableData,
                                       string reportHeader, string batchStatus,string userName, int cardscount,
                                       float[] CardTable_colWidths, float[] StatusTable_colWidths, 
            Dictionary<int, string> reportfields)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                var document = new Document(PageSize.A4);
                document.AddCreationDate();
                //document.AddCreator("Indigo Instant Card Issuing System");
                //document.AddAuthor("Indigo Instant Card Issuing System");
                //document.AddTitle("Distribution Batch Report");

                document.AddCreator(reportfields[0]);
                document.AddAuthor(reportfields[0]);
                document.AddTitle(reportfields[reportfields.Count - 1]);
                //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfFile, FileMode.Create));
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                writer.CompressionLevel = 9;

                document.Open();
                document.Add(new Phrase(reportHeader, new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));
                //document.Add(new Phrase("Current batch status: " + batchStatus,
                //                        new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                //document.Add(new Phrase(Environment.NewLine));
                //document.Add(new Phrase("Date printed: " + DateTime.Now.ToShortDateString(),
                //                        new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                //document.Add(new Phrase(Environment.NewLine));
                //document.Add(new Phrase("Report generated by: " + userName,
                //                        new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(reportfields[1] + " " + batchStatus,
                                      new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                //// branch details
                //document.Add(new Phrase(reportfields[13] + branch,
                //                       new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                //document.Add(new Phrase(Environment.NewLine));

                //document.Add(new Phrase(reportfields[14] + branchCode,
                //                       new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                //document.Add(new Phrase(Environment.NewLine));

                document.Add(new Phrase(reportfields[10] + " " + cardscount.ToString(),
                                      new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));
                document.Add(new Phrase(reportfields[2] + " " + DateTime.Now.ToString(StaticFields.DATE_FORMAT),
                                        new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));
                document.Add(new Phrase(reportfields[3] + " " + userName,
                                        new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));

                document.Add(CreateTable(batchStatusTableData, StatusTable_colWidths));
                document.Add(new Phrase(Environment.NewLine));
                document.Add(CreateTable(cardTableData, CardTable_colWidths));
                document.Close();

                writer.Flush();

                return ms.ToArray();
            }
        }
        private static PdfPTable CreateTable(List<string[]> TableData, float[] colWidths)
        {
            string[] header = TableData[0];
            var table = new PdfPTable(header.Length);
            table.WidthPercentage = 100;
            table.SetWidths(colWidths);
            foreach (string column in header)
            {
                table.AddCell(new PdfPCell(new Phrase(column, new Font(Font.FontFamily.COURIER, 9, Font.BOLD))));
            }
            for (int i = 1; i < TableData.Count; i++)
            {
                foreach (string column1 in TableData[i])
                {
                    table.AddCell(new PdfPCell(new Phrase(column1, new Font(Font.FontFamily.COURIER, 8, Font.NORMAL))));
                }
            }
            return table;
        }
    }
}
