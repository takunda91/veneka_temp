using System;
using System.Drawing.Printing;
using System.Text;

namespace Veneka.Indigo.PinMailerPrinting.utility
{
    public class PrinterUtilities
    {
        public static bool PrintTestPage(string PrinterName)
        {
            if (!ValidatePrinter(PrinterName))
                return false;
            try
            {
                var sb = new StringBuilder();
                for (int i = 0; i < 20; i++)
                {
                    sb.Append("*** *** *** *** *** Indigo Card Test Page *** *** *** *** ***");
                }
                sb.Append("\f");
                return RawPrinterFeed.SendStringToPrinter(PrinterName, sb.ToString());
            }
            catch (Exception ex)
            {
                ex.ToString();
                // LogFileWriter.WriteWebServerError("Veneka.Indigo.PinMailerPrinting.printerimplementations.PrintPINMailer", ex);
                return false;
            }
        }

        public static bool ValidatePrinter(string PrinterName)
        {
            var pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = PrinterName;
            return pd.PrinterSettings.IsValid;
        }
    }
}