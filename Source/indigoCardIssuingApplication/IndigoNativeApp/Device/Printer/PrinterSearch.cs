using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.DesktopApp.Device.Printer
{
    public class PrinterSearch
    {
        public delegate IPrinter[] FindPrinters();
        FindPrinters[] printerSearches;

        public PrinterSearch()
        {
            //Add printers to be searched, must implement delegate
            printerSearches = new FindPrinters[]
            {
                Simulator.PrintSimulator.GetPrinterList,
               // Zebra.ZebraZXP3.GetPrinterList,
                Zebra.ZebraZC3.GetPrinterList
            };
        }        

        public IPrinter[] SearchForConnectedPrinters()
        {
            List<IPrinter> printers = new List<IPrinter>();

            foreach (FindPrinters search in printerSearches)
            {
                try
                {
                    printers.AddRange(search());
                }
                catch(Exception ex)
                {
                    //TODO : Log exception
                }
            }

            return printers.ToArray();
        }
    }
}
