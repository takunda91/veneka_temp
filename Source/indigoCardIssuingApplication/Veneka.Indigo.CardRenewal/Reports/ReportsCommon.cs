using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Renewal.Reports
{
    public class ReportsCommon
    {
        internal static PdfPTable CreateTable(List<string[]> TableData, float[] colWidths)
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

        internal static string DisplayPartialPAN(string clearPAN)
        {
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
