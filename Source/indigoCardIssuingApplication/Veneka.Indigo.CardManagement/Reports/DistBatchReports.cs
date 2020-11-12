extern alias textsharp;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.Common.Utilities;
using textsharp::iTextSharp.text.pdf;
using textsharp::iTextSharp.text;

namespace Veneka.Indigo.CardManagement.Reports
{
    public class DistBatchReports
    {
        private readonly DistBatchManagementDAL _distBatchDAL = new DistBatchManagementDAL();
        private readonly PrintBatchManagementDAL _printBatchDAL = new PrintBatchManagementDAL();

        /// <summary>
        /// Generates a PDF report for a distribution batch returned as an array of bytes.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public byte[] GenerateDistBatchReport(long distBatchId,int languageId, string username, long auditUserId, string auditWorkstation)
        {
            var distBatchHistory = _distBatchDAL.GetDistBatchHistory(distBatchId,languageId, auditUserId, auditWorkstation);
            var distBatchCards = _distBatchDAL.GetDistBatchCards(distBatchId, auditUserId, auditWorkstation);

            var distCardTableData = new List<string[]>();
            var distBatchStatusTableData = new List<string[]>();

            ReportManagementService reportservice = new ReportManagementService();
            var result = reportservice.GetReportFields(2, languageId);
            Dictionary<int, string> reportfields = new Dictionary<int, string>();

            int i = 0;
            foreach (var item in result)
            {
                reportfields.Add(i, result[i].language_text);
                i++;
            }

            //distBatchStatusTableData.Add(new[] { "Date", "Batch Status", "User" });
            distBatchStatusTableData.Add(new[] { reportfields[4], reportfields[5], reportfields[6], reportfields[9] });

            foreach (var record in distBatchHistory)
            {
                distBatchStatusTableData.Add(new string[]{((DateTime)record.status_date).ToString(StaticFields.DATETIME_FORMAT), 
                                                          record.dist_batch_status_name, 
                                                         record.username,
                                                          record.status_notes});
            }

            //distCardTableData.Add(new[] { "Card Number" });
            distCardTableData.Add(new[] { reportfields[12],reportfields[7],reportfields[10] });
            int sno = 1;
            foreach (var card in distBatchCards)
            {
                //distCardTableData.Add(new[] { DisplayPartialPAN(card.card_number) });
                distCardTableData.Add(new[] { sno.ToString(), card.card_number, card.card_reference_number });
                sno++;
            }
            string originbranchname=string.Empty, originbranchcode=string.Empty;
            if(distBatchCards.Count>0)
            {
                originbranchname = distBatchCards[0].origin_branch_name;
                originbranchcode = distBatchCards[0].origin_branch_code;

            }

            return createPDFReport(distCardTableData, distBatchStatusTableData,
                            distBatchHistory[distBatchHistory.Count - 1].dist_batch_reference,
                            distBatchHistory[distBatchHistory.Count - 1].dist_batch_status_name, distBatchHistory[distBatchHistory.Count - 1].branch_name,  distBatchHistory[distBatchHistory.Count - 1].branch_code,
                            originbranchname, originbranchcode
                            , username,distCardTableData.Count()-1, new float[] {100, 400,400 },
                            new float[] { 500, 500, 500, 500 },reportfields);
        }



        public byte[] GeneratePrintBatchReport(long printBatchId, int languageId, string username, long auditUserId, string auditWorkstation)
        {
            var printBatchHistory = _printBatchDAL.GetPrintBatchHistory(printBatchId, languageId, auditUserId, auditWorkstation);
            var printBatchrequests = _printBatchDAL.GetPrintBatchRequests(printBatchId, 1, 2000, languageId, auditUserId, auditWorkstation);

            var printbatchrequestTableData = new List<string[]>();
            var printbatchStatusTableData = new List<string[]>();

            ReportManagementService reportservice = new ReportManagementService();
            var result = reportservice.GetReportFields(28, languageId);
            Dictionary<int, string> reportfields = new Dictionary<int, string>();

            int i = 0;
            foreach (var item in result)
            {
                reportfields.Add(i, result[i].language_text);
                i++;
            }

            //distBatchStatusTableData.Add(new[] { "Date", "Batch Status", "User" });
            printbatchStatusTableData.Add(new[] { reportfields[4], reportfields[5], reportfields[6], reportfields[7] });

            foreach (var record in printBatchHistory)
            {
                printbatchStatusTableData.Add(new string[]{((DateTime)record.status_date).ToString(StaticFields.DATETIME_FORMAT),
                                                          record.print_batch_status_name,
                                                         record.username,
                                                          record.status_notes});
            }

            //distCardTableData.Add(new[] { "Card Number" });
            printbatchrequestTableData.Add(new[] { reportfields[9], reportfields[16], reportfields[7] });
            int sno = 1;
            foreach (var card in printBatchrequests)
            {
                //distCardTableData.Add(new[] { DisplayPartialPAN(card.card_number) });
                printbatchrequestTableData.Add(new[] { sno.ToString(), card.request_reference, card.request_reference });
                sno++;
            }
            string originbranchname = string.Empty, originbranchcode = string.Empty;
            if (printBatchrequests.Count > 0)
            {
                originbranchname = printBatchHistory[0].branch_name;
                originbranchcode = printBatchHistory[0].branch_name;

            }

            return createPrintPDFReport(printbatchrequestTableData, printbatchStatusTableData,
                            printBatchHistory[printBatchHistory.Count - 1].print_batch_reference,
                            printBatchHistory[printBatchHistory.Count - 1].print_batch_status_name, printBatchHistory[printBatchHistory.Count - 1].branch_name, printBatchHistory[printBatchHistory.Count - 1].branch_code,
                            originbranchname, originbranchcode
                            , username, printbatchrequestTableData.Count() - 1, new float[] { 100, 400, 400 },
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
                                       string reportHeader, string batchStatus, string branch, string branchCode, string originbranch, string originbranchCode, string userName,int cardscount,
                                       float[] CardTable_colWidths, float[] StatusTable_colWidths, Dictionary<int, string> reportfields)
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

                // branch details
                document.Add(new Phrase(reportfields[13] + " " + branch,
                                       new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                document.Add(new Phrase(reportfields[14] + " " + branchCode,
                                       new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                document.Add(new Phrase(reportfields[15] + " " + originbranch,
                                      new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                document.Add(new Phrase(reportfields[16] + " " + originbranchCode,
                                       new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                document.Add(new Phrase(reportfields[11] + " " + cardscount.ToString(),
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

        private byte[] createPrintPDFReport(List<string[]> cardTableData, List<string[]> batchStatusTableData,
                                      string reportHeader, string batchStatus, string branch, string branchCode, string originbranch, string originbranchCode, string userName, int cardscount,
                                      float[] CardTable_colWidths, float[] StatusTable_colWidths, Dictionary<int, string> reportfields)
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

                // branch details
                document.Add(new Phrase(reportfields[10] + " " + branch,
                                       new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                document.Add(new Phrase(reportfields[11] + " " + branchCode,
                                       new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                document.Add(new Phrase(reportfields[12] + " " + originbranch,
                                      new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                document.Add(new Phrase(reportfields[13] + " " + originbranchCode,
                                       new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                document.Add(new Phrase(reportfields[15] + " " + cardscount.ToString(),
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

        public static void createVaultPDFReport(List<string[]> cardTableData, List<string[]> vaultInfo, string pdfFile, string reportHeader, string userName, float[] CardTable_colWidths, float[] vaultInfor_colWidths, string vaultOperationName)
        {
            var document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfFile, FileMode.Create));
            document.Open();
            document.Add(new Phrase(reportHeader, new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
            document.Add(new Phrase(Environment.NewLine));
            document.Add(new Phrase("Vault report: " + vaultOperationName,
                                    new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
            document.Add(new Phrase(Environment.NewLine));
            document.Add(new Phrase("Date printed: " + DateTime.Now.ToString(StaticFields.DATE_FORMAT),
                                    new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
            document.Add(new Phrase(Environment.NewLine));
            document.Add(new Phrase("Report generated by: " + userName,
                                    new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
            document.Add(new Phrase(Environment.NewLine));
            document.Add(CreateTable(vaultInfo, vaultInfor_colWidths));
            document.Add(new Phrase(Environment.NewLine));
            document.Add(CreateTable(cardTableData, CardTable_colWidths));
            document.Close();
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

        public static string DisplayPartialPAN(string clearPAN)
        {
            //if (clearPAN != null && clearPAN.Length > 10)
            //{
            //    return clearPAN.Substring(0, 6) + "*".PadRight(clearPAN.Length - 10, '*') +
            //           clearPAN.Substring(clearPAN.Length - 4, 4);
            //}


            //rather format the card for ecobank impementation
            if (clearPAN != null && clearPAN.Length > 10)
            {
                return clearPAN.Substring(0, 6) + "-" + clearPAN.Substring(6, clearPAN.Length - 10) + "-" +
                       clearPAN.Substring(clearPAN.Length - 4, 4);
            }

            return clearPAN;
        }
    }
}
