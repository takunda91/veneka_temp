using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.NativeApp.Device.Printer.Zebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.NativeApp.Device.Printer.Zebra.Tests
{
    [TestClass()]
    public class ZebraZXP3Tests
    {
        [TestMethod()]
        public void GetDeviceListTest()
        {
            PrinterSearch search = new PrinterSearch();

            var printers = search.SearchForConnectedPrinters();

            //var printers = ZebraZXP3.GetDeviceList();

            if (printers.Length > 0)
            {
                using (printers[0])
                {                    
                    string errMsg;
                    short alarm;
                    printers[0].Print();
                }  
            }
        }
    }
}