using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.DesktopApp.Device.Printer.Zebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.ProductPrinting;
using System.Drawing;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra.Tests
{
    [TestClass()]
    public class ZebraZC3IntegrationTests
    {
        [TestMethod()]
        public void ZebraZC3_GetPrinterList()
        {
            var printers = ZebraZC3.GetPrinterList();

            //Assert.Fail();
        }

        [TestMethod()]
        public void ZebraZC3_Connect()
        {
            var printers = ZebraZC3.GetPrinterList();

            using (var printer = printers[0])
            {
                printer.Connect();
            }


            //Assert.Fail();
        }

        [TestMethod()]
        public void ZebraZC3_HasMagEncoder()
        {
            var printers = ZebraZC3.GetPrinterList();

            using (var printer = printers[0])
            {
                if (printer.Connect() == 0)
                {
                    var hasMagEncoder = printer.HasMagEncoder();
                }
            }
        }

        [TestMethod()]
        public void ZebraZC3_GetMagData()
        {
            //Zebra.Sdk.Comm.ConnectionException
            var printers = ZebraZC3.GetPrinterList();

            using (var printer = printers[0])
            {
                printer.Connect();

                if(printer.HasMagEncoder())
                {
                    var magData = printer.ReadMagStripe();
                }
            }
        }

        [TestMethod()]
        public void ZebraZC3_DisplayJobSettings()
        {
            //Zebra.Sdk.Comm.ConnectionException
            var printers = ZebraZC3.GetPrinterList();

            using (var printer = printers[0])
            {
                printer.Connect();                

                printer.DisplayJobSettings();
            }
        }

        [TestMethod()]
        public void ZebraZC3_CheckPrinterStatus()
        {
            //Zebra.Sdk.Comm.ConnectionException
            var printers = ZebraZC3.GetPrinterList();

            using (var printer = printers[0])
            {
                printer.Connect();

                //printer.CheckPrinterStatus();
            }
        }

        [TestMethod()]
        public void ZebraZC3_GetMediaInfo()
        {
            //Zebra.Sdk.Comm.ConnectionException
            var printers = ZebraZC3.GetPrinterList();

            using (var printer = printers[0])
            {
                printer.Connect();

                printer.GetMediaInfo();
            }
        }
        [TestMethod()]
        public void ZebraZC3_ReadandPrint()
        {
            //Zebra.Sdk.Comm.ConnectionException
            var printers = ZebraZC3.GetPrinterList();

            using (var printer = printers[0])
            {
                printer.Connect();

                ProductField nameOnCardField = new ProductField
                {
                    Deleted = false,
                    Printable = true,
                    X = 500,
                    Y = 200,
                    Value = Encoding.UTF8.GetBytes("Veneka Test Printing"),
                    Font = "Consolas",
                    FontColourRGB = Color.Black.ToArgb(),
                    FontSize = 10,
                    PrintSide = 0
                };

                var printDetail = printer.PrinterDetailFactory().Populate(new ProductField[] { nameOnCardField });

                 
                printer.ReadAndPrint("", printDetail,out IDeviceMagData magData);
            }
        }


        [TestMethod()]
        public void ZebraZC3_PrintNameOnCard()
        {
            //Zebra.Sdk.Comm.ConnectionException
            var printers = ZebraZC3.GetPrinterList();

            using (var printer = printers[0])
            {
                printer.Connect();

                ProductField nameOnCardField = new ProductField
                {
                    Deleted = false,
                    Printable = true,
                    X = 500,
                    Y = 200,
                    Value = Encoding.UTF8.GetBytes("DANIEL JACOB"),
                    Font = "Consolas",
                    FontColourRGB = Color.Black.ToArgb(),
                    FontSize = 10,
                    PrintSide = 0
                };

                var printDetail = printer.PrinterDetailFactory().Populate(new ProductField[] { nameOnCardField });                

                printer.Print("123456789", printDetail);                
            }
        }

        
    }
}