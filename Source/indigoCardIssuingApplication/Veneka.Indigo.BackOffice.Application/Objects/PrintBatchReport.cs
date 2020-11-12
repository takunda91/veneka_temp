
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace Veneka.Indigo.BackOffice.Application.Objects
{
    extern alias itextsharp;
    using pdftext=itextsharp.iTextSharp.text;
    public static class PrintBatchReport
    {

        public static  byte[] GenerateReport(DataTable printBatchrequests)
        {
            var printbatchrequestTableData = new List<string[]>();
            printbatchrequestTableData.Add(new[] {"S.No", "RequestReference", "PAN", "Printing Status" });
            int sno = 1;
            foreach (DataRow card in printBatchrequests.Rows)
            {
                printbatchrequestTableData.Add(new[] { sno.ToString(), card["request_reference"].ToString(), card["pan"].ToString(), card["printing_status"].ToString() });
                sno++;
            }
            return GetReport(printbatchrequestTableData, new float[] { 500, 500, 500, 500 });
        }
        public static byte[] GetReport(List<string[]> TableData, float[] width)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                var document = new pdftext.Document(pdftext.PageSize.A4);
                document.AddCreationDate();
                document.AddCreator("Indigo Instant Card Issuing System");
                document.AddAuthor("Indigo Instant Card Issuing System");
                document.AddTitle("Batch Status Report");


                //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfFile, FileMode.Create));
                pdftext.pdf.PdfWriter writer = pdftext.pdf.PdfWriter.GetInstance(document, ms);
                writer.CompressionLevel = 9;

                document.Open();
                document.Add(new pdftext.Phrase("Indigo Instant Card Issuing System", new pdftext.Font(pdftext.Font.FontFamily.COURIER, 9, pdftext.Font.NORMAL)));
                document.Add(new pdftext.Phrase(Environment.NewLine));

                document.Add(new pdftext.Phrase("Print Batch Status Report", new pdftext.Font(pdftext.Font.FontFamily.COURIER, 9, pdftext.Font.NORMAL)));
                document.Add(new pdftext.Phrase(Environment.NewLine));
               
                document.Add(new pdftext.Phrase(Environment.NewLine));
               

                document.Add(CreateTable(TableData,width));
              
                document.Close();

                writer.Flush();

                return ms.ToArray();
            }
        }
        private static pdftext.pdf.PdfPTable CreateTable(List<string[]> TableData, float[] colWidths)
        {
            string[] header = TableData[0];
            var table = new pdftext.pdf.PdfPTable(header.Length);
            table.WidthPercentage = 100;
            table.SetWidths(colWidths);
            foreach (string column in header)
            {
                table.AddCell(new pdftext.pdf.PdfPCell(new pdftext.Phrase(column, new pdftext.Font(pdftext.Font.FontFamily.COURIER, 9, pdftext.Font.BOLD))));
            }
            for (int i = 1; i < TableData.Count; i++)
            {
                foreach (string column1 in TableData[i])
                {
                    table.AddCell(new pdftext.pdf.PdfPCell(new pdftext.Phrase(column1, new pdftext.Font(pdftext.Font.FontFamily.COURIER, 8, pdftext.Font.NORMAL))));
                }
            }
            return table;
        }
    }
}
