using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.DesktopApp.Device.Printer.Zebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using FluentAssertions;
using Veneka.Indigo.Integration.ProductPrinting;
using System.Drawing;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra.Tests
{
    [TestClass()]
    public class ZebraCardPrintDetailsTests
    {
        [TestMethod()]
        public void PopulateTest()
        {
            //// Arrange
            //ZebraCardPrintDetails cardPrintDetails = new ZebraCardPrintDetails();
            //ProductField[] productFields = new ProductField[]
            //{
            //    new ProductField{ Deleted = false, Printable = true, X = 500, Y = 200, Value = Encoding.UTF8.GetBytes("FrontSideText"),
            //        Font = "Consolas", FontColourRGB = Color.Black.ToArgb(), FontSize = 10, PrintSide = 0 },
            //    new ProductField{ Deleted = false, Printable = true, X = 300, Y = 100, Value = Encoding.UTF8.GetBytes("BackSideText"),
            //        Font = "Arial", FontColourRGB = Color.Blue.ToArgb(), FontSize = 12, PrintSide = 1 }
            //};

            //// Act
            //cardPrintDetails.Populate(productFields);

            // Assert
            Assert.Fail();
        }
    }
}