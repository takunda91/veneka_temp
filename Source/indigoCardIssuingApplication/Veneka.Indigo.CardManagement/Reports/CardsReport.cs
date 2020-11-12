extern alias textsharp;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.CardManagement.objects;
using textsharp::iTextSharp.text.pdf;
using textsharp::iTextSharp.text;

namespace Veneka.Indigo.CardManagement.Reports
{
    public class CardsReport
    {
        private readonly CardManagementDAL _cardManDAL = new CardManagementDAL();

        /// <summary>
        /// PDF report Generates a for a checked out cards returned as an array of bytes.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal byte[] GenerateCheckedOutCardsReport(int? issuerId, int? branchId, int? user_role_id,  long? operatorId, string username, int languageId, long auditUserId, string auditWorkstation)
        {
            var checkedOutCards = _cardManDAL.SearchBranchCards(issuerId, branchId, user_role_id, null, null, null, null, (int)BranchCardStatus.AVAILABLE_FOR_ISSUE, operatorId, 1, 2000, languageId, auditUserId, auditWorkstation);

            var checkedOutCardsTableData = new List<string[]>();

            ReportManagementService reportservice = new ReportManagementService();
            var result = reportservice.GetReportFields(3, languageId);
            Dictionary<int, string> reportfields = new Dictionary<int, string>();

            int i = 0;
            foreach (var item in result)
            {
                reportfields.Add(i, result[i].language_text);
                i++;
            }

            //checkedOutCardsTableData.Add(new[] { "Card Number", "Date"/*, "Operator"*/ });
            //checkedOutCardsTableData.Add(new[] { reportfields[4], reportfields[3]/*, "Cardrefrencenumber"*/ });
            checkedOutCardsTableData.Add(new[] {reportfields[11], reportfields[4], reportfields[9], reportfields[3] });

            string operatorUsername = "";
            int sno = 1;
            foreach (var card in checkedOutCards)
            {
                checkedOutCardsTableData.Add(new[] {sno.ToString(), card.card_number,card.card_request_reference,
                                                     ((DateTime)card.status_date).ToString(StaticFields.DATETIME_FORMAT)});
                operatorUsername = card.operator_username;
                sno++; 
            }


            return createPDFReport(checkedOutCardsTableData,
                           reportfields[reportfields.Count-1],
                            username, checkedOutCardsTableData.Count()-1,operatorUsername, new float[] {100, 400, 400, 400 }, reportfields);
        }

        /// <summary>
        /// Generates a PDF report for a checked out cards returned as an array of bytes.
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="branchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal byte[] GenerateCheckedInCardsReport(int branchId, List<structCard> checkedOutCards, string operatorUsername, string username, int languageId, long auditUserId, string auditWorkstation)
        {
            //var checkedOutCards = _cardManDAL.SearchBranchCards(branchId, null, BranchCardStatus.CHECKED_IN, null, auditUserId, auditWorkstation);

            var checkedOutCardsTableData = new List<string[]>();
            ReportManagementService reportservice = new ReportManagementService();
            var result = reportservice.GetReportFields(3, languageId);
            Dictionary<int, string> reportfields = new Dictionary<int, string>();

            int i = 0;
            foreach (var item in result)
            {
                reportfields.Add(i, result[i].language_text);
                i++;
            }

            //checkedOutCardsTableData.Add(new[] { "Card Number", "Date"/*, "Operator"*/ });
            checkedOutCardsTableData.Add(new[] { reportfields[11],reportfields[4], reportfields[9],reportfields[3]/*, "Operator"*/ });
            int sno = 1;
            foreach (var card in checkedOutCards)
            {
                checkedOutCardsTableData.Add(new[] { sno.ToString(),UtilityClass.DisplayPartialPAN(card.card_number),card.card_reference_number,
                                                     DateTime.Now.ToString(StaticFields.DATETIME_FORMAT)
                                                     //,operatorUsername
                                                   });
                sno++; 
            }

            return createPDFReport(checkedOutCardsTableData,
                           reportfields[reportfields.Count-2],
                            username, checkedOutCardsTableData.Count()-1, operatorUsername, new float[] {100, 400, 400, 400 }, reportfields);
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
        private byte[] createPDFReport(List<string[]> checkedOutCardsTableData,
                                       string reportHeader, string userName,int cardscount, string operatorUsername,
                                       float[] CardTable_colWidths, Dictionary<int, string> reportfields)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                var document = new Document(PageSize.A4);
                document.AddCreationDate();
                //document.AddCreator("Indigo Instant Card Issuing System");
                //document.AddAuthor("Indigo Instant Card Issuing System");
                //document.AddTitle("Card Report");

                document.AddCreator(reportfields[0]);
                document.AddAuthor(reportfields[0]);
                document.AddTitle(reportfields[reportfields.Count - 3]);// for card report label

                //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfFile, FileMode.Create));
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                writer.CompressionLevel = 9;

                document.Open();
                document.Add(new Phrase(reportHeader, new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));
                //document.Add(new Phrase("Date printed: " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"),
                //                        new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                //document.Add(new Phrase(Environment.NewLine));
                //document.Add(new Phrase("Report generated by: " + userName,
                //                        new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                //document.Add(new Phrase(Environment.NewLine));
                //document.Add(new Phrase("Operator: " + operatorUsername,
                //                        new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));

                document.Add(new Phrase(reportfields[1] + " " + DateTime.Now.ToString(StaticFields.DATETIME_FORMAT),
                                      new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                document.Add(new Phrase(reportfields[10] + " " + cardscount.ToString(),
                                    new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));

                document.Add(new Phrase(reportfields[2] + " " + userName,
                                        new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(new Phrase(Environment.NewLine));
                document.Add(new Phrase(reportfields[5] + " " + operatorUsername,
                                        new Font(Font.FontFamily.COURIER, 9, Font.NORMAL)));
                document.Add(CreateTable(checkedOutCardsTableData, CardTable_colWidths));
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
