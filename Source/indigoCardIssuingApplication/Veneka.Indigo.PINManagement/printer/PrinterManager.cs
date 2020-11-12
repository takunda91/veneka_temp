//using System.Drawing;
//using System.Drawing.Printing;
//using System.Windows.Forms;
//using System.Runtime.InteropServices;

namespace Veneka.Indigo.PINManagement.printer
{
    public static class PrinterManager
    {
        public static bool SendToPrinter(string customerName, string cardNumber, string branch, string pinNumbers,
                                         string pinLetters)
        {
            /*
            using (PrintDocument printDocument = new PrintDocument())
            {
                printDocument.PrinterSettings.PrinterName = "EPSON DFX-9000";
                printDocument.PrintPage += (s, e) =>
                {
                    Font font = new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular);
                    e.Graphics.DrawString(customerName, font, Brushes.Black, 90, 100);
                    e.Graphics.DrawString("CARD: " + cardNumber, font, Brushes.Black, 90, 125);
                    e.Graphics.DrawString("BRANCH: " + branch, font, Brushes.Black, 90, 150);
                    e.Graphics.DrawString(pinNumbers, font, Brushes.Black, 440, 150);
                    e.Graphics.DrawString(pinLetters, font, Brushes.Black, 440, 175);
                    e.HasMorePages = false;
                };
                printDocument.Print();
                printDocument.Dispose();
            }*/
            return true;
        }
    }
}