extern alias textsharp;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Utilities;
using textsharp::iTextSharp.text.pdf;
using textsharp::iTextSharp.text;
using Veneka.Indigo.PINManagement;

namespace Veneka.Indigo.CardManagement.Reports
{
    public class LoadBatchReports
    {
        /// <summary>
        /// Generates a PDF report for a load batch returned as an array of bytes.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        /// 
       
        public byte[] GenerateLoadBatchReport(long loadBatchId,int languageId, string username, long auditUserId, string auditWorkstation)
        {
            LoadBatchMangementService loadBatchService = new LoadBatchMangementService();
            var loadBatchHistory = loadBatchService.GetLoadBatchHistory(loadBatchId,languageId, auditUserId, auditWorkstation);
            var loadBatchCards = loadBatchService.GetLoadBatchCards(loadBatchId, auditUserId, auditWorkstation);

            var loadCardTableData = new List<string[]>();
            var loadBatchStatusTableData = new List<string[]>();

            ReportManagementService reportservice = new ReportManagementService();
            var result =  reportservice.GetReportFields(1, languageId);
            Dictionary<int, string> reportfields = new Dictionary<int, string>();

            int i=0;
            foreach (var item in result)
            {
                reportfields.Add(i, result[i].language_text);
                i++;
            }


            //loadBatchStatusTableData.Add(new[] { "Date", "Batch Status", "User" });
            loadBatchStatusTableData.Add(new[] { reportfields[4], reportfields[5], reportfields[6] });

            foreach (var record in loadBatchHistory)
            {
                loadBatchStatusTableData.Add(new string[]{((DateTime)record.status_date).ToString(StaticFields.DATETIME_FORMAT), 
                                                          record.load_batch_status_name, 
                                                          record.first_name + " " + record.last_name});
            }

            //loadCardTableData.Add(new[] { "Card Number" });
            loadCardTableData.Add(new[] { reportfields[11], reportfields[7], reportfields[9], reportfields[12].Replace(":","") });
            int sno = 1;
            foreach (var card in loadBatchCards)
            {
                //loadCardTableData.Add(new[] { DisplayPartialPAN(card.card_number) });
                loadCardTableData.Add(new[] { sno.ToString(), card.card_number, card.card_reference_number, card.branch_code+"-"+card.branch_name });
                sno++;
            }

           

            return createPDFReport(loadCardTableData, loadBatchStatusTableData,
                            loadBatchHistory[0].load_batch_reference,
                            loadBatchHistory[loadBatchHistory.Count - 1].load_batch_status_name,
                            username, loadCardTableData.Count() - 1, new float[] { 75, 350, 350,150 },
                            new float[] { 450, 450, 450 }, reportfields);
        }

        public byte[] GeneratePinFileBatchReport(long loadBatchId, int languageId, string username, long auditUserId, string auditWorkstation)
        {
            LoadBatchMangementService loadBatchService = new LoadBatchMangementService();
          
            var loadBatchHistory = loadBatchService.GetLoadBatchHistory(loadBatchId, languageId, auditUserId, auditWorkstation);
            var loadBatchCards = loadBatchService.GetLoadBatchCards(loadBatchId, auditUserId, auditWorkstation);

            var loadCardTableData = new List<string[]>();
            var loadBatchStatusTableData = new List<string[]>();

            ReportManagementService reportservice = new ReportManagementService();
            var result = reportservice.GetReportFields(1, languageId);
            Dictionary<int, string> reportfields = new Dictionary<int, string>();

            int i = 0;
            foreach (var item in result)
            {
                reportfields.Add(i, result[i].language_text);
                i++;
            }


            //loadBatchStatusTableData.Add(new[] { "Date", "Batch Status", "User" });
            loadBatchStatusTableData.Add(new[] { reportfields[4], reportfields[5], reportfields[6] });

            foreach (var record in loadBatchHistory)
            {
                loadBatchStatusTableData.Add(new string[]{((DateTime)record.status_date).ToString(StaticFields.DATETIME_FORMAT),
                                                          record.load_batch_status_name,
                                                          record.first_name + " " + record.last_name});
            }

            //loadCardTableData.Add(new[] { "Card Number" });
            loadCardTableData.Add(new[] { reportfields[11], reportfields[7], reportfields[9], reportfields[12].Replace(":", "") });
            int sno = 1;
            foreach (var card in loadBatchCards)
            {
                //loadCardTableData.Add(new[] { DisplayPartialPAN(card.card_number) });
                loadCardTableData.Add(new[] { sno.ToString(), card.card_number, card.card_reference_number, card.branch_code + "-" + card.branch_name });
                sno++;
            }



            return createPDFReport(loadCardTableData, loadBatchStatusTableData,
                            loadBatchHistory[0].load_batch_reference,
                            loadBatchHistory[loadBatchHistory.Count - 1].load_batch_status_name,
                            username, loadCardTableData.Count() - 1, new float[] { 75, 350, 350, 150 },
                            new float[] { 450, 450, 450 }, reportfields);
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
                                       string reportHeader, string batchStatus, string userName,int cardcount,
                                       float[] CardTable_colWidths, float[] StatusTable_colWidths, Dictionary<int, string>  reportfields)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                var document = new Document(PageSize.A4);                
                document.AddCreationDate();
                //document.AddCreator("Indigo Instant Card Issuing System");
                //document.AddAuthor("Indigo Instant Card Issuing System");
                //document.AddTitle("Load Batch Report");

                document.AddCreator(reportfields[0]);
                document.AddAuthor(reportfields[0]);
                document.AddTitle(reportfields[reportfields.Count-1]);
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


                document.Add(new Phrase(reportfields[1] + " "+ batchStatus,
                                       new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

               

                document.Add(new Phrase(reportfields[10] + " " + cardcount.ToString(),
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
