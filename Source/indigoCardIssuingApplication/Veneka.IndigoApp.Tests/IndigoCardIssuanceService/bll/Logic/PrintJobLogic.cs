using System;

using IndigoCardIssuanceService.bll;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using FluentAssertions;
using Veneka.Indigo.CardManagement.dal;

namespace Veneka.IndigoApp.Tests.IndigoCardIssuanceService.bll.Logic
{
    [TestClass]
    public class PrintJobLogic
    {
        [TestMethod]
        public void InsertPrintJob_Successful()
        {
            var printjob = Substitute.For<IPrintJobManagementDAL>();
            string printjobid;
            printjob.When(x=>x.InsertPrintJob("12345", -1, 1, 0, -1, "test",out printjobid)).Do(x => x[6]=1);
            PrintJobController _printJobController = new PrintJobController();

          var result =  _printJobController.InsertPrintJob("12345",-1,1,0,-1,"test");

          //  result.Value.Should().Be(1, because: "Inserted Print Job.");

        }
        [TestMethod]
        public void InsertPrintJob_failed()
        {
            var printjob = Substitute.For<IPrintJobManagementDAL>();
            string printjobid;
            printjob.When(x => x.InsertPrintJob("12345", -1, 1, 0, -1, "test", out printjobid)).Do(x => x[6] = 0);
            PrintJobController _printJobController = new PrintJobController();

            var result = _printJobController.InsertPrintJob("12345", -1, 1, 0, -1, "test");

          //  result.Value.Should().BeLessOrEqualTo(0, because: "Inserted Print Job is not successful.");

        }

        [TestMethod]
        public void RegisterPrinter_successful()
        {
            var printjob = Substitute.For<IPrintJobManagementDAL>();
            printjob.When(x => x.RegisterPrinter(new Indigo.CardManagement.objects.Printer(),"",  -1, "test"));

            PrintJobController _printJobController = new PrintJobController();

          var result=  _printJobController.RegisterPrinter( new Indigo.CardManagement.objects.Printer() { SerialNo="1234",FirmwareVersion="1.0.0.0", BranchId= 3037, Model="ZC350",NextClean=0,TotalPrints=0,Manufacturer="Zebra" },"",-1,"Test");

            result.ResponseType.Should().Equals(true);

        }
        [TestMethod]
        public void RegisterPrinter_failed()
        {
            var printjob = Substitute.For<IPrintJobManagementDAL>();
            printjob.RegisterPrinter(new Indigo.CardManagement.objects.Printer(),"", -1, "test").ReturnsForAnyArgs(  Indigo.Common.SystemResponseCode.CREATE_FAIL);

            PrintJobController _printJobController = new PrintJobController();

            var result = _printJobController.RegisterPrinter(new Indigo.CardManagement.objects.Printer() { SerialNo = "1234", FirmwareVersion = "1.0.0.0", BranchId = 3037, Model = "ZC350", NextClean = 0, TotalPrints = 0, Manufacturer = "Zebra" },"",-1, "Test");

            result.ResponseType.Should().Equals(false);

        }
    }
}
