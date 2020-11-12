using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.DesktopApp.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using FluentAssertions;
using Veneka.Indigo.UX.NativeAppAPI;
using Veneka.Indigo.DesktopApp.Device.Printer;
using Veneka.Indigo.Integration.ProductPrinting;
using IndigoDesktopApp.Device.Printer;
using Veneka.Indigo.DesktopApp.Device;
using Microsoft.QualityTools.Testing.Fakes;

namespace Veneka.Indigo.DesktopApp.Logic.Tests
{
    [TestClass()]
    public class CardPrintingLogicTests
    {
        [TestMethod()]
        public void DoPrintJob_SuccessfulRunThrough()
        {
            // Arrange
            var printJobDetails = new PrintJob()
            {
                ApplicationOptions = new AppOptions[0],
                MustReturnCardData = false,
                PrintJobId = "IndigoTest",
                ProductFields = new ProductField[]
                {
                    new ProductField { Deleted = false, Editable = false, Font = "Arial", FontColourRGB = 100, FontSize = 12, Height = 10, Label = "lbl", MappedName = "mpd",
                    MaxSize = 25, Name = "name", Printable = true, PrintSide = 0, ProductPrintFieldTypeId = 0, Value = Encoding.UTF8.GetBytes("SomeText"), Width = 23, X = 50, Y = 60 }
                }
            };
            var printJobresp = new Response<PrintJob>(true, "All good", printJobDetails, "sessionStr");

            var cardPrinting = Substitute.For<ICardPrinting>();
            cardPrinting.GetPrintJob(new Token(), new PrinterInfo()).ReturnsForAnyArgs(printJobresp);

            var cardPrintDetails = Substitute.For<ICardPrintDetails>();

            var printerDetailFactory = Substitute.For<IPrinterDetailFactory>();
            printerDetailFactory.Populate(new ProductField[0]).ReturnsForAnyArgs(cardPrintDetails);

            var printer = Substitute.For<IPrinter>();
            printer.Print("productBin", cardPrintDetails).ReturnsForAnyArgs(PrinterCodes.Success);
            printer.PrinterDetailFactory().Returns(printerDetailFactory);

            var cardPrintLogic = new CardPrintingLogic(cardPrinting, printer);

            var uiHandler = Substitute.For<UiUpdateEventHandler>();
            cardPrintLogic.UiUpdate += uiHandler;

            // Act
            cardPrintLogic.DoPrintJob();

            // Assert
            cardPrinting.ReceivedWithAnyArgs(1).GetPrintJob(new Token(), new PrinterInfo());
            printer.Received(1).Connect();
            printer.ReceivedWithAnyArgs(1).SetDeviceSettings(new Dictionary<string, string>());
            printer.Received(1).PrinterDetailFactory();
            printerDetailFactory.ReceivedWithAnyArgs(1).Populate(new ProductField[0]);
            printer.ReceivedWithAnyArgs(1).Print("productBin", cardPrintDetails);
            cardPrinting.ReceivedWithAnyArgs(1).PrintingComplete(new Token(), new CardData());
            cardPrinting.ReceivedWithAnyArgs(1).PrinterAnalytics(new Token());
            printer.Received(1).Disconnect();
            printer.Received(1).Dispose();

            uiHandler.ReceivedWithAnyArgs(4).Invoke(uiHandler, "", false, false, null);
        }

        [TestMethod()]
        public void PrinterConnectAndSetup_PrinterConnectsSuccessfully()
        {
            // Arrange 
            var printingWebService = Substitute.For<ICardPrinting>();
            var printer = Substitute.For<IPrinter>();
            printer.Connect().Returns(PrinterCodes.Success);

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);

            // Act
            Token token = null;
            string additionalInfo;
            var success = cardPrintLogic.PrinterConnectAndSetup(out token, out additionalInfo);

            // Assert
            success.Should().BeTrue(because: "The printer object returned that it had connected successfully");
        }

        [TestMethod()]
        public void PrinterConnectAndSetup_PrinterDoesNotConnectSuccessfully()
        {
            // Arrange 
            var printingWebService = Substitute.For<ICardPrinting>();
            var printer = Substitute.For<IPrinter>();
            printer.Connect().Returns(PrinterCodes.ConnectFailed);

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);

            // Act
            Token token = null;
            string additionalInfo;
            var success = cardPrintLogic.PrinterConnectAndSetup(out token, out additionalInfo);

            // Assert
            success.Should().BeFalse(because: "The printer object returned that it had connected unsuccessfully");
        }

        [TestMethod()]
        public void PrinterConnectAndSetup_PrinterConnectsSuccessfully_SubscribeToDeviceNotificationEvent()
        {
            // Arrange 
            var printingWebService = Substitute.For<ICardPrinting>();            

            var printer = Substitute.For<IPrinter>();
            printer.Connect().Returns(PrinterCodes.Success);

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);
            var uiHandler = Substitute.For<UiUpdateEventHandler>();
            cardPrintLogic.UiUpdate += uiHandler;            
            EventArgs args = new EventArgs();

            // Act
            Token token = null;
            string additionalInfo;
            var success = cardPrintLogic.PrinterConnectAndSetup(out token, out additionalInfo);
            //parameters: object sender, string message, bool isCritical, EventArgs e
            printer.OnDeviceNotifcation += Raise.Event<DeviceNotificationEventHandler>(printer, "TheMessage", false, args);

            // Assert
            success.Should().BeTrue(because: "The printer object returned that it had connected successfully");
            uiHandler.Received().Invoke(printer, "TheMessage", false, false, args);                       
        }

        [TestMethod()]
        public void PrinterConnectAndSetup_PrinterDoesNotConnectSuccessfully_ShouldNotSubscribeToDeviceNotificationEvent()
        {
            // Arrange 
            var printingWebService = Substitute.For<ICardPrinting>();

            var printer = Substitute.For<IPrinter>();
            printer.Connect().Returns(PrinterCodes.ConnectFailed);

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);
            var uiHandler = Substitute.For<UiUpdateEventHandler>();
            cardPrintLogic.UiUpdate += uiHandler;
            EventArgs args = new EventArgs();

            // Act
            Token token = null;
            string additionalInfo;
            var success = cardPrintLogic.PrinterConnectAndSetup(out token, out additionalInfo);
            //parameters: object sender, string message, bool isCritical, EventArgs e
            printer.OnDeviceNotifcation += Raise.Event<DeviceNotificationEventHandler>(printer, "TheMessage", false, args);

            // Assert
            success.Should().BeFalse(because: "The printer object returned that it had connected unsuccessfully");
            uiHandler.DidNotReceive().Invoke(printer, "TheMessage", false, false, args);
        }

        [TestMethod()]
        public void PrinterConnectAndSetup_PrinterConnectsSuccessfully_PopulatedTokenReturned()
        {
            // Arrange 
            IndigoDesktopApp.StartupProperties.Token = "StartupToken";
            var printingWebService = Substitute.For<ICardPrinting>();
            var printer = Substitute.For<IPrinter>();
            printer.Connect().Returns(PrinterCodes.Success);
            printer.DeviceId.Returns("TheDeviceId");

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);
            var validToken = new Token
            {
                DeviceID = "TheDeviceId",
                Session = "StartupToken",
                Workstation = Environment.MachineName
            };

            // Act
            Token token = null;
            string additionalInfo;
            var success = cardPrintLogic.PrinterConnectAndSetup(out token, out additionalInfo);

            // Assert
            success.Should().BeTrue(because: "The printer object returned that it had connected successfully");
            token.Should().BeEquivalentTo(validToken);
        }

        [TestMethod()]
        public void PrinterConnectAndSetup_PrinterDoesNotConnectsSuccessfully_NullTokenReturned()
        {
            // Arrange 
            IndigoDesktopApp.StartupProperties.Token = "StartupToken";
            var printingWebService = Substitute.For<ICardPrinting>();
            var printer = Substitute.For<IPrinter>();
            printer.Connect().Returns(PrinterCodes.ConnectFailed);
            printer.DeviceId.Returns("TheDeviceId");

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);

            // Act
            Token token = null;
            string additionalInfo;
            var success = cardPrintLogic.PrinterConnectAndSetup(out token, out additionalInfo);

            // Assert
            success.Should().BeFalse(because: "The printer object returned that it had connected unsuccessfully");
            token.Should().BeNull(because: "Token could not be populated as printer did not connect");
        }


        [TestMethod()]
        public void GetPrintJob_ICardPrintingGetPrintJobSuccess_ReturnsValidPrintJob()
        {
            // Arrange        
            var printer = Substitute.For<IPrinter>();
            
            var token = new Token { DeviceID = "DeviceId", Session = "Session", Workstation = "Workstation" };
            // TODO: PrinterInfo need to be built from information from IPrinter
            var printerInfo = new PrinterInfo();

            var printingWebService = Substitute.For<ICardPrinting>();
            printingWebService.GetPrintJob(token, printerInfo).Returns(new Response<PrintJob>
            {
                AdditionalInfo = "Call was successful",
                Session = "SessionString",
                Success = true,
                Value = new PrintJob()
            });            

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);            

            // Act
            string additionalInfo;
            IPrintJob printJob = null;
            var success = cardPrintLogic.GetPrintJob(token, out printJob, out additionalInfo);

            // Assert
            printingWebService.Received().GetPrintJob(token, printerInfo);
            success.Should().BeTrue(because: "The call to ICardPrinting returns success as true. Which should mean the calling method is successful");
            printJob.Should().NotBeNull(because: "Successful call to ICardPrinting should return a PrintJob object returned in output argument");
        }

        [TestMethod()]
        public void GetPrintJob_ICardPrintingGetPrintJobUnsuccessful_ReturnNullPrintJob()
        {
            // Arrange             
            var printingWebService = Substitute.For<ICardPrinting>();
            printingWebService.GetPrintJob(new Token(), new PrinterInfo()).ReturnsForAnyArgs(new Response<PrintJob>
            {
                AdditionalInfo = "Call was not successful",
                Session = "SessionString",
                Success = false,
                Value = null
            });

            var printer = Substitute.For<IPrinter>();

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);
            var token = new Token();
            var printerInfo = new PrinterInfo();

            // Act
            string additionalInfo;
            IPrintJob printJob = null;
            var success = cardPrintLogic.GetPrintJob(token, out printJob, out additionalInfo);

            // Assert
            success.Should().BeFalse(because: "The call to ICardPrinting returns success as false. Which should mean the calling method is unsuccessful");
            printJob.Should().BeNull(because: "UnSuccessful call to ICardPrinting should return a null PrintJob in output argument");
            additionalInfo.Should().Be("Call was not successful", because: "Calling method should pass on information received from ICardPrinting.GetPrintJob's Reponse object");
        }

        [TestMethod()]
        public void GetPrintJob_ICardPrintingGetPrintJobUnsuccessful_ReturnPrintJobObject()
        {
            // Arrange             
            var printingWebService = Substitute.For<ICardPrinting>();
            printingWebService.GetPrintJob(new Token(), new PrinterInfo()).ReturnsForAnyArgs(new Response<PrintJob>
            {
                AdditionalInfo = "Call was not successful",
                Session = "SessionString",
                Success = false,
                Value = new PrintJob()
            });

            var printer = Substitute.For<IPrinter>();

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);
            var token = new Token();
            var printerInfo = new PrinterInfo();

            // Act
            string additionalInfo;
            IPrintJob printJob = null;
            var success = cardPrintLogic.GetPrintJob(token, out printJob, out additionalInfo);

            // Assert
            success.Should().BeFalse(because: "The call to ICardPrinting returns success as false. Which should mean the calling method is unsuccessful");
            printJob.Should().BeNull(because: "UnSuccessful call to ICardPrinting. Calling method should return a null PrintJob in output argument");
        }

        [TestMethod()]
        public void SetPrinterSetting_PrintJobHasSettings_SuccessfulSetOptions()
        {
            // Arrange             
            var jobOptions = new Dictionary<string, string>()
            {
                { "Option1", "Value1" },
                { "Option2", "Value2" }
            };

            var printJob = Substitute.For<IPrintJob>();
            printJob.AppOptionToDictionary().Returns(jobOptions);

            var printingWebService = Substitute.For<ICardPrinting>();
            var printer = Substitute.For<IPrinter>();

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);


            // Act
            string additionalInfo;
            var success = cardPrintLogic.SetPrinterSetting(printJob, out additionalInfo);

            // Assert
            printer.Received(1).SetDeviceSettings(jobOptions);
            success.Should().BeTrue();            
        }

        [TestMethod()]
        public void StartPhysicalPrint_MustReturnCardDataIsFalse_SuccessfulPrint()
        {
            // Arrange
            var printJob = Substitute.For<IPrintJob>();
            printJob.MustReturnCardData.Returns(false);

            var printingWebService = Substitute.For<ICardPrinting>();
            var printDetails = Substitute.For<ICardPrintDetails>();

            var printer = Substitute.For<IPrinter>();
            printer.Print("productBin", printDetails).ReturnsForAnyArgs(PrinterCodes.Success);

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);

            // Act
            CardData cardData = null;
            string additionalInfo;
            var success = cardPrintLogic.StartPhysicalPrint(printJob, out cardData, out additionalInfo);

            // Assert
            printer.ReceivedWithAnyArgs(1).Print("productCode", printDetails);
            IDeviceMagData magData;
            printer.DidNotReceiveWithAnyArgs().ReadAndPrint("productCode", printDetails, out magData);
            success.Should().BeTrue();
            cardData.Should().BeNull(because: "PrintJob object has property MustReturnCardData set to false. therefore we are not expecting to recceive a populated CardData object, even if the call was successful");
        }

        [TestMethod()]
        public void StartPhysicalPrint_MustReturnCardDataIsFalse_UnsuccessfulPrint()
        {
            // Arrange
            var printJob = Substitute.For<IPrintJob>();
            printJob.MustReturnCardData.Returns(false);

            var printingWebService = Substitute.For<ICardPrinting>();
            var printDetails = Substitute.For<ICardPrintDetails>();

            var printer = Substitute.For<IPrinter>();
            printer.Print("productBin", printDetails).ReturnsForAnyArgs(PrinterCodes.UnknownError);

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);

            // Act
            CardData cardData = null;
            string additionalInfo;
            var success = cardPrintLogic.StartPhysicalPrint(printJob, out cardData, out additionalInfo);

            // Assert
            printer.ReceivedWithAnyArgs(1).Print("productCode", printDetails);
            IDeviceMagData magData;
            printer.DidNotReceiveWithAnyArgs().ReadAndPrint("productCode", printDetails, out magData);
            success.Should().BeFalse();
            cardData.Should().BeNull(because: "PrintJob object has property MustReturnCardData set to false. therefore we are not expecting to recceive a populated CardData object");
        }

        [TestMethod()]
        public void StartPhysicalPrint_MustReturnCardDataIsFalse_PrinterHasMagEncoderToCheckBIN_BinDoesNotMatchCardInserted()
        {
            // Arrange
            var printJob = Substitute.For<IPrintJob>();
            printJob.MustReturnCardData.Returns(false);

            var printingWebService = Substitute.For<ICardPrinting>();
            var printDetails = Substitute.For<ICardPrintDetails>();

            var printer = Substitute.For<IPrinter>();
            printer.Print("productBin", printDetails).ReturnsForAnyArgs(PrinterCodes.ProductBinAndCardMismatch);

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);

            // Act
            CardData cardData = null;
            string additionalInfo;
            var success = cardPrintLogic.StartPhysicalPrint(printJob, out cardData, out additionalInfo);

            // Assert
            printer.ReceivedWithAnyArgs(1).Print("productCode", printDetails);
            IDeviceMagData magData;
            printer.DidNotReceiveWithAnyArgs().ReadAndPrint("productCode", printDetails, out magData);
            success.Should().BeFalse();
            cardData.Should().BeNull(because: "PrintJob object has property MustReturnCardData set to false. therefore we are not expecting to recceive a populated CardData object");
        }

        [TestMethod()]
        public void StartPhysicalPrint_MustReturnCardDataIsTrue_SuccessfulReadAndPrint()
        {
            // Arrange
            var printJob = Substitute.For<IPrintJob>();
            printJob.MustReturnCardData.Returns(true);

            var printingWebService = Substitute.For<ICardPrinting>();
            var printDetails = Substitute.For<ICardPrintDetails>();

            var rtnMagData = Substitute.For<IDeviceMagData>();            
            rtnMagData.TrackDataToString(2).Returns("1234567890123456=ThisIsTrack2");

            var printer = Substitute.For<IPrinter>();
            IDeviceMagData tempMagData;
            printer.ReadAndPrint("productBin", printDetails, out tempMagData).ReturnsForAnyArgs(x =>
            {
                x[2] = rtnMagData;
                return PrinterCodes.Success;
            });

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);

            // Act
            CardData cardData = null;
            string additionalInfo;
            var success = cardPrintLogic.StartPhysicalPrint(printJob, out cardData, out additionalInfo);

            // Assert
            IDeviceMagData magData;
            printer.ReceivedWithAnyArgs(1).ReadAndPrint("productCode", printDetails, out magData);
            printer.DidNotReceiveWithAnyArgs().Print("productCode", printDetails);
            success.Should().BeTrue();
            cardData.Should().NotBeNull(because: "PrintJob object has property MustReturnCardData set to true. therefore we are expecting to recceive a populated CardData object on successful print");
        }

        [TestMethod()]
        public void StartPhysicalPrint_MustReturnCardDataIsTrue_PanAndTrack2SetCorrectly_SuccessfulReadAndPrint()
        {
            // Arrange
            var printJob = Substitute.For<IPrintJob>();
            printJob.MustReturnCardData.Returns(true);

            var printingWebService = Substitute.For<ICardPrinting>();
            var printDetails = Substitute.For<ICardPrintDetails>();

            var rtnMagData = Substitute.For<IDeviceMagData>();
            rtnMagData.TrackDataToString(2).Returns("1234567890123456=ThisIsTrack2");

            var printer = Substitute.For<IPrinter>();
            IDeviceMagData tempMagData;
            printer.ReadAndPrint("productBin", printDetails, out tempMagData).ReturnsForAnyArgs(x =>
            {
                x[2] = rtnMagData;
                return PrinterCodes.Success;
            });

            var cardPrintLogic = new CardPrintingLogic(printingWebService, printer);

            // Act
            CardData cardData = null;
            string additionalInfo;
            var success = cardPrintLogic.StartPhysicalPrint(printJob, out cardData, out additionalInfo);

            // Assert
            IDeviceMagData magData;
            printer.ReceivedWithAnyArgs(1).ReadAndPrint("productCode", printDetails, out magData);
            printer.DidNotReceiveWithAnyArgs().Print("productCode", printDetails);
            success.Should().BeTrue();
            cardData.Should().NotBeNull(because: "PrintJob object has property MustReturnCardData set to true. therefore we are expecting to recceive a populated CardData object on successful print");
            cardData.Track2.Should().Be("1234567890123456=ThisIsTrack2");
            cardData.PAN.Should().Be("1234567890123456");
        }
    }
}