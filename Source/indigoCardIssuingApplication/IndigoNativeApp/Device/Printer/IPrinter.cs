using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.DesktopApp.Device.Printer
{
    public enum CardSidePanel
    {
        Front = 0,
        Back = 1
    }

    public enum Orientation
    {
        Portrait = 0,
        Landscape = 1
    }


    public interface IPrinter : IDevice, IMagEncoder
    {
        short Connect();
        short Disconnect();

       /// <summary>
       /// returns a PrinterDetailFactory which is used to take print details and create a card printer details object
       /// </summary>
       /// <returns></returns>
        IPrinterDetailFactory PrinterDetailFactory();

        /// <summary>
        /// Start the print job
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        short Print(string productBin, ICardPrintDetails cardPrintDetails);

        short ReadAndPrint(string productBin, ICardPrintDetails cardPrintDetails, out IDeviceMagData magData);

        short PrintJobStatus();

        short Cancel();

        Dictionary<string, string> GetPrinterInfo();
    }
}
