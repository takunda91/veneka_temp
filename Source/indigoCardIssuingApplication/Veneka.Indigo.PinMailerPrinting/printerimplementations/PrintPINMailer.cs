using System;
using System.Text;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.PinMailerPrinting.objects;

namespace Veneka.Indigo.PinMailerPrinting.printerimplementations
{
    public class PrintPINMailer
    {
        public bool Print(PrintObject item, string printer)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("\n\n\n\n\n\n\n\n\n\n\n");
                sb.Append("\t" + item.CustomerName);
                sb.Append("\n\t" + item.CardNumber);
                sb.Append("\n\t Branch Code: " + item.BranchName);
                sb.Append("\n\t\t\t\t\t\t" + item.PIN);
                sb.Append("\n\t\t\t\t\t\t" + item.PINWords);
                sb.Append("\f");
                return RawPrinterFeed.SendStringToPrinter(printer, sb.ToString());
            }
            catch (Exception ex)
            {
                LogFileWriter.WriteWebServerError(ToString(), ex);
                return false;
            }
        }
    }
}