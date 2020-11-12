using Veneka.Indigo.DesktopApp.Device.Printer.Zebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using ZebraSdk = Zebra.Sdk;
using Microsoft.QualityTools.Testing.Fakes.Shims;
using Zebra.Sdk.Card.Containers;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra.Tests
{
    [TestClass()]
    public class ZebraZC3Tests
    {
        [TestMethod()]
        public void ZebraZC3_GetPrinterList_ReturnsEmptyDeviceList()
        {
            using (ShimsContext.Create())
            {
                // Arrange 
                ZebraSdk.Printer.Discovery.Fakes.ShimUsbDiscoverer.GetZebraUsbPrintersDiscoveredPrinterFilter = (filter) =>
                {
                    return new List<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>();
                };

                // Act
                var printers = ZebraZC3.GetPrinterList();

                // Assert
                printers.Should().BeEmpty(because: "No printers connected to machine and device list returned must be empty");
            }
        }

        [TestMethod()]
        public void ZebraZC3_GetPrinterList_ReturnsSinglePrinterInDeviceList()
        {
            using (ShimsContext.Create())
            {

                // Arrange 
                ZebraSdk.Printer.Discovery.Fakes.ShimUsbDiscoverer.GetZebraUsbPrintersDiscoveredPrinterFilter = (filter) =>
                {
                    return new List<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>()
                    {
                        Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>("symbolicName")
                    };
                };

                // Act
                var printers = ZebraZC3.GetPrinterList();

                // Assert
                printers.Should().HaveCount(1, because: "Single usb printer was returned from UsbDiscoverer and should have returned one device");
            }
        }

        [TestMethod()]
        public void ZebraZC3_GetPrinterList_Returns5PrinterInDeviceList()
        {
            using (ShimsContext.Create())
            {

                // Arrange 
                ZebraSdk.Printer.Discovery.Fakes.ShimUsbDiscoverer.GetZebraUsbPrintersDiscoveredPrinterFilter = (filter) =>
                {
                    return new List<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>()
                    {
                        Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>("symbolicName1"),
                        Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>("symbolicName2"),
                        Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>("symbolicName3"),
                        Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>("symbolicName4"),
                        Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>("symbolicName5")
                    };
                };

                // Act
                var printers = ZebraZC3.GetPrinterList();

                // Assert
                printers.Should().HaveCount(5, because: "Single usb printer was returned from UsbDiscoverer and should have returned one device");
            }
        }

        [TestMethod()]
        public void ZebraZC3_ConstructorWithNullValue_ThrowsArgumentNullException()
        {
            // Arrange 
            Exception exception = null;

            // Act
            try
            {
                ZebraZC3 zebraZC3 = new ZebraZC3(null);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            exception.Should().NotBeNull(because: "We should not be able to create a ZebraZC3 object with a null discovered device.");
            exception.Should().BeOfType(typeof(ArgumentNullException), because: "The check for null value mus return an exception of ArgumentNullException");
            ((ArgumentNullException)exception).ParamName.Should().Be("discoveredPrinter", because: "discoveredPrinter argument is being checked for a null value.");
        }

        [TestMethod()]
        public void ZebraZC3_ConstructorWithUsbDiscoveredPrinter_ZebraZC3ObjectCreated()
        {
            using (ShimsContext.Create())
            {
                // Arrange 
                ZebraSdk.Printer.Discovery.Fakes.ShimDiscoveredPrinter.AllInstances.AddressGet = @this => "Address";
                var usbPrinter = Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredPrinter>("symbolicName1");

                // Act
                ZebraZC3 zebraZC3 = new ZebraZC3(usbPrinter);


                // Assert
                zebraZC3.Should().NotBeNull(because: "Object should have been created.");
                zebraZC3.Should().BeOfType(typeof(ZebraZC3));
            }
        }

        [TestMethod()]
        public void ZebraZC3_ConstructorWithUsbDiscoveredPrinter_DevicePropertySet()
        {
            using (ShimsContext.Create())
            {
                // Arrange 
                ZebraSdk.Printer.Discovery.Fakes.ShimDiscoveredPrinter.AllInstances.AddressGet = @this => "SomeFunnyAddress";
                ZebraSdk.Printer.Discovery.Fakes.ShimDiscoveredPrinter.AllInstances.DiscoveryDataMapGet = @this => new Dictionary<string, string>()
                {
                    { "SERIAL_NUMBER", "aaabbbb111222ccc" },
                    { "MFG", "Zebra Technologies" },
                    { "MODEL", "ZTC ZC350" }
                };
                var usbPrinter = Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredPrinter>("symbolicName");

                // Act
                ZebraZC3 zebraZC3 = new ZebraZC3(usbPrinter);


                // Assert
                var temp = usbPrinter.Received(3).DiscoveryDataMap;
                zebraZC3.DeviceId.Should().Be("aaabbbb111222ccc", because: "SERIAL_NUMBER in DiscoveryDataMap is used to set DeviceId property");
                zebraZC3.Manufacturer.Should().Be("Zebra Technologies", because: "MFG in DiscoveryDataMap is used to set Manufacturer property");
                zebraZC3.Model.Should().Be("ZTC ZC350", because: "MODEL in DiscoveryDataMap is used to set Model property");
                zebraZC3.Name.Should().Be("ZTC ZC350[aaabbbb111222ccc]", because: "Name property is made from Model and DeviceId");
            }
        }

        [TestMethod()]
        public void ZebraZC3_ConnectToUsbPrinter_ConnectionSuccess()
        {
            using (ShimsContext.Create())
            {
                // Arrange 
                ZebraSdk.Printer.Discovery.Fakes.ShimDiscoveredPrinter.AllInstances.AddressGet = @this => "SomeFunnyAddress";
                ZebraSdk.Printer.Discovery.Fakes.ShimDiscoveredPrinter.AllInstances.DiscoveryDataMapGet = @this => new Dictionary<string, string>()
                {
                    { "SERIAL_NUMBER", "aaabbbb111222ccc" },
                    { "MFG", "Zebra Technologies" },
                    { "MODEL", "ZTC ZC350" }
                };
                var conn = Substitute.For<ZebraSdk.Comm.Connection>();
                var usbPrinter = Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>("symbolicName1");
                usbPrinter.GetConnection().Returns(conn);

                var zebraPrinter = Substitute.For<ZebraSdk.Card.Printer.ZebraCardPrinter>();
                ZebraSdk.Card.Printer.Fakes.ShimZebraCardPrinterFactory.GetInstanceConnection = (shimConn) =>
                {
                    return zebraPrinter;
                };

                ZebraZC3 zebraZC3 = new ZebraZC3(usbPrinter);

                // Act
                var respCode = zebraZC3.Connect();

                // Assert
                usbPrinter.Received(1).GetConnection();
                conn.Received(1).Open();
                respCode.Should().Be(0, because: "Sucessful response code from the printer is code=0");
            }
        }

        [TestMethod()]
        public void ZebraZC3_ConnectToUsbPrinter_ZebraCardPrinterFactoryGetInstanceException()
        {
            using (ShimsContext.Create())
            {
                // Arrange 
                ZebraSdk.Printer.Discovery.Fakes.ShimDiscoveredPrinter.AllInstances.AddressGet = @this => "SomeFunnyAddress";
                ZebraSdk.Printer.Discovery.Fakes.ShimDiscoveredPrinter.AllInstances.DiscoveryDataMapGet = @this => new Dictionary<string, string>()
                {
                    { "SERIAL_NUMBER", "aaabbbb111222ccc" },
                    { "MFG", "Zebra Technologies" },
                    { "MODEL", "ZTC ZC350" }
                };
                var conn = Substitute.For<ZebraSdk.Comm.Connection>();

                var usbPrinter = Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>("symbolicName1");
                usbPrinter.GetConnection().Returns(conn);

                var zebraPrinter = Substitute.For<ZebraSdk.Card.Printer.ZebraCardPrinter>();
                ZebraSdk.Card.Printer.Fakes.ShimZebraCardPrinterFactory.GetInstanceConnection = (shimConn) =>
                {
                    throw new Exception();
                };

                ZebraZC3 zebraZC3 = new ZebraZC3(usbPrinter);

                Exception expectedEx = null;
                short? respCode = null;

                // Act
                try
                {
                    respCode = zebraZC3.Connect();
                }
                catch (Exception ex)
                {
                    expectedEx = ex;
                }

                // Assert
                usbPrinter.Received(1).GetConnection();
                conn.Received(1).Open();
                respCode.Should().BeNull(because: "ZebraCardPrinterFactory.GetInstance threw exception which must be cause by calling code. No response code should have been returned");
                expectedEx.Should().NotBeNull(because: "ZebraCardPrinterFactory.GetInstance threw exception and should not have been swallowed");
            }
        }

        [TestMethod()]
        public void ZebraZC3_ConnectToUsbPrinter_ConnectionOpenException()
        {
            using (ShimsContext.Create())
            {
                // Arrange 
                ZebraSdk.Card.Printer.Fakes.ShimZebraCardPrinterFactory.Behavior = ShimBehaviors.NotImplemented;
                ZebraSdk.Printer.Discovery.Fakes.ShimDiscoveredPrinter.AllInstances.AddressGet = @this => "SomeFunnyAddress";
                ZebraSdk.Printer.Discovery.Fakes.ShimDiscoveredPrinter.AllInstances.DiscoveryDataMapGet = @this => new Dictionary<string, string>()
                {
                    { "SERIAL_NUMBER", "aaabbbb111222ccc" },
                    { "MFG", "Zebra Technologies" },
                    { "MODEL", "ZTC ZC350" }
                };
                var conn = Substitute.For<ZebraSdk.Comm.Connection>();
                conn.When(x => x.Open()).Do(x => { throw new Exception(); });

                var usbPrinter = Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>("symbolicName1");
                usbPrinter.GetConnection().Returns(conn);

                ZebraZC3 zebraZC3 = new ZebraZC3(usbPrinter);

                Exception expectedEx = null;
                short? respCode = null;

                // Act
                try
                {
                    respCode = zebraZC3.Connect();
                }
                catch (Exception ex)
                {
                    expectedEx = ex;
                }

                // Assert
                usbPrinter.Received(1).GetConnection();
                conn.Received(1).Open();
                respCode.Should().BeNull(because: "Connection.Open threw exception which must be cause by calling code. No response code should have been returned");
                expectedEx.Should().NotBeNull(because: "Connection.Open threw exception and should not have been swallowed");
                expectedEx.Should().NotBeOfType(typeof(NotImplementedException), because: "Exception should have been thrown on connectopn.Open(). Looks like a call to ZebraCardPrinterFactory.GetInstance Happened.");
            }
        }

        [TestMethod()]
        public void ZebraZC3_GetMagEncoder_ReturnsTrue()
        {
            using (ShimsContext.Create())
            {
                // Arrange 
                var errorFactory = Substitute.For<Error.IZebraErrorFactory>();
                var conn = Substitute.For<ZebraSdk.Comm.Connection>();

                var zebraPrinter = Substitute.For<ZebraSdk.Card.Printer.ZebraCardPrinter>();
                zebraPrinter.SetConnection(conn);
                zebraPrinter.HasMagneticEncoder().Returns(true);

                ZebraZC3 zebraZC3 = new ZebraZC3(conn, zebraPrinter, errorFactory);

                // Act                
                var hasMagEncoder = zebraZC3.HasMagEncoder();

                // Assert        
                zebraPrinter.Received(1).HasMagneticEncoder();
                hasMagEncoder.Should().BeTrue(because: "ZebraCardPrinter has magnetic encoder installed, which returns true when checked. Therefor our method must also send back true.");
            }
        }

        [TestMethod()]
        public void ZebraZC3_GetMagEncoder_ReturnsFalse()
        {
            using (ShimsContext.Create())
            {
                // Arrange 
                var conn = Substitute.For<ZebraSdk.Comm.Connection>();
                var errorFactory = Substitute.For<Error.IZebraErrorFactory>();

                var zebraPrinter = Substitute.For<ZebraSdk.Card.Printer.ZebraCardPrinter>();
                zebraPrinter.SetConnection(conn);
                zebraPrinter.HasMagneticEncoder().Returns(false);

                ZebraZC3 zebraZC3 = new ZebraZC3(conn, zebraPrinter, errorFactory);

                // Act                
                var hasMagEncoder = zebraZC3.HasMagEncoder();

                // Assert        
                zebraPrinter.Received(1).HasMagneticEncoder();
                hasMagEncoder.Should().BeFalse(because: "ZebraCardPrinter does not have a magnetic encoder installed, which returns false when checked. Therefor our method must also send back false.");
            }
        }

        [TestMethod()]
        public void ZebraZC3_GetMagEncoder_ConnectNotCalledBeforeMethodUsed()
        {
            using (ShimsContext.Create())
            {
                // Arrange 
                ZebraSdk.Printer.Discovery.Fakes.ShimDiscoveredPrinter.AllInstances.AddressGet = @this => "SomeFunnyAddress";
                ZebraSdk.Printer.Discovery.Fakes.ShimDiscoveredPrinter.AllInstances.DiscoveryDataMapGet = @this => new Dictionary<string, string>()
                {
                    { "SERIAL_NUMBER", "aaabbbb111222ccc" },
                    { "MFG", "Zebra Technologies" },
                    { "MODEL", "ZTC ZC350" }
                };
                var usbPrinter = Substitute.For<ZebraSdk.Printer.Discovery.DiscoveredUsbPrinter>("symbolicName1");
                var zebraZC3 = new ZebraZC3(usbPrinter);
                Exception receivedException = null;

                // Act            
                try
                {
                    var hasMagEncoder = zebraZC3.HasMagEncoder();
                }
                catch (Exception ex)
                {
                    receivedException = ex;
                }

                // Assert        
                usbPrinter.Received(0).GetConnection();
                receivedException.Should().NotBeNull(because: "The method should check that Connect was called before trying to anything");
                receivedException.Message.Should().Be("Device not connected! Method may only be used after device has successful Connect().");
            }
        }

        [TestMethod()]
        public void ZebraZC3_CheckPrinterStatus_StatusOkayReturnTrue()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var errorDesc = Substitute.For<IDeviceErrorDescriptor>();
                errorDesc.Code.Returns(0);
                errorDesc.Description.Returns("The description");
                errorDesc.HelpfulHint.Returns("A helpful hint for the problem");

                var errorFactory = Substitute.For<Error.IZebraErrorFactory>();
                errorFactory.GetErrorDescription(0).Returns(errorDesc);

                var alarm = new ZebraSdk.Card.Containers.Fakes.ShimAlarmInfo
                {
                    ValueGet = () => { return 0; }
                };

                var printerStatus = new ZebraSdk.Card.Containers.Fakes.ShimPrinterStatusInfo
                {
                    AlarmInfoGet = () => { return alarm; },
                    ErrorInfoGet = () => { return alarm; }
                };

                var conn = Substitute.For<ZebraSdk.Comm.Connection>();
                var zebraPrinter = Substitute.For<ZebraSdk.Card.Printer.ZebraCardPrinter>();
                zebraPrinter.GetPrinterStatus().Returns(printerStatus);

                ZebraZC3 zebraZC3 = new ZebraZC3(conn, zebraPrinter, errorFactory);

                // Act   
                string status;
                List<IDeviceErrorDescriptor> errorList;
                var boolStatus = zebraZC3.CheckPrinterStatus(out status, out errorList);

                // Assert
                zebraPrinter.Received(1).GetPrinterStatus();
                errorFactory.Received(1).GetErrorDescription(0);
                boolStatus.Should().BeTrue(because: "All status checks have passed successfully");
                errorList.Should().HaveCount(1, because: "Only one item should be in the error list.");
                errorList[0].Should().BeEquivalentTo(errorDesc);
            }
        }

        [TestMethod()]
        public void ZebraZC3_CheckPrinterStatus_AlarmInfoHasErrorReturnFalse()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var errorDesc = Substitute.For<IDeviceErrorDescriptor>();
                errorDesc.Code.Returns(7008);
                errorDesc.Description.Returns("Printer door open");
                errorDesc.HelpfulHint.Returns("Ensure the door is closed.");

                var errorFactory = Substitute.For<Error.IZebraErrorFactory>();
                errorFactory.GetErrorDescription(7008).Returns(errorDesc);

                var alarmGood = new ZebraSdk.Card.Containers.Fakes.ShimAlarmInfo
                {
                    ValueGet = () => { return 0; }
                };

                var alarmBad = new ZebraSdk.Card.Containers.Fakes.ShimAlarmInfo
                {
                    ValueGet = () => { return 7008; }
                };

                var printerStatus = new ZebraSdk.Card.Containers.Fakes.ShimPrinterStatusInfo
                {
                    AlarmInfoGet = () => { return alarmBad; },
                    ErrorInfoGet = () => { return alarmGood; }
                };

                var conn = Substitute.For<ZebraSdk.Comm.Connection>();
                var zebraPrinter = Substitute.For<ZebraSdk.Card.Printer.ZebraCardPrinter>();
                zebraPrinter.GetPrinterStatus().Returns(printerStatus);

                ZebraZC3 zebraZC3 = new ZebraZC3(conn, zebraPrinter, errorFactory);

                // Act  
                string status;
                List<IDeviceErrorDescriptor> errorList;
                var boolStatus = zebraZC3.CheckPrinterStatus(out status, out errorList);

                // Assert
                zebraPrinter.Received(1).GetPrinterStatus();
                errorFactory.Received(1).GetErrorDescription(7008);
                boolStatus.Should().BeFalse(because: "AlarmInfo is returning a problem even though ErrorInfo doesn't. Should trigger false on either having an issue or both having an issue.");
                errorList.Should().HaveCount(1, because: "Only one item should be in the error list.");
                errorList[0].Should().BeEquivalentTo(errorDesc);
            }
        }

        [TestMethod()]
        public void ZebraZC3_CheckPrinterStatus_ErrorInfoHasErrorReturnFalse()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var errorDesc = Substitute.For<IDeviceErrorDescriptor>();
                errorDesc.Code.Returns(7028);
                errorDesc.Description.Returns("Printer thingo is open");
                errorDesc.HelpfulHint.Returns("Ensure the thingo is closed.");

                var errorFactory = Substitute.For<Error.IZebraErrorFactory>();
                errorFactory.GetErrorDescription(7028).Returns(errorDesc);

                var alarmGood = new ZebraSdk.Card.Containers.Fakes.ShimAlarmInfo
                {
                    ValueGet = () => { return 0; }
                };

                var alarmBad = new ZebraSdk.Card.Containers.Fakes.ShimAlarmInfo
                {
                    ValueGet = () => { return 7028; }
                };

                var printerStatus = new ZebraSdk.Card.Containers.Fakes.ShimPrinterStatusInfo
                {
                    AlarmInfoGet = () => { return alarmGood; },
                    ErrorInfoGet = () => { return alarmBad; }
                };

                var conn = Substitute.For<ZebraSdk.Comm.Connection>();
                var zebraPrinter = Substitute.For<ZebraSdk.Card.Printer.ZebraCardPrinter>();
                zebraPrinter.GetPrinterStatus().Returns(printerStatus);

                ZebraZC3 zebraZC3 = new ZebraZC3(conn, zebraPrinter, errorFactory);

                // Act   
                string status;
                List<IDeviceErrorDescriptor> errorList;
                var boolStatus = zebraZC3.CheckPrinterStatus(out status, out errorList);

                // Assert
                zebraPrinter.Received(1).GetPrinterStatus();
                errorFactory.Received(1).GetErrorDescription(7028);
                boolStatus.Should().BeFalse(because: "ErrorInfo is returning a problem even though AlarmInfo doesn't. Should trigger false on either having an issue or both having an issue.");
                errorList.Should().HaveCount(1, because: "Only one item should be in the error list.");
                errorList[0].Should().BeEquivalentTo(errorDesc);
            }
        }

        [TestMethod()]
        public void ZebraZC3_CheckPrinterStatus_AlarmInfoAndErrorInfoHaveSameErrorsReturnFalse()
        {
            using (ShimsContext.Create())
            {
                var errorDesc7008 = Substitute.For<IDeviceErrorDescriptor>();
                errorDesc7008.Code.Returns(7008);
                errorDesc7008.Description.Returns("Printer door open");
                errorDesc7008.HelpfulHint.Returns("Ensure the door is closed.");

                var errorFactory = Substitute.For<Error.IZebraErrorFactory>();
                errorFactory.GetErrorDescription(7008).Returns(errorDesc7008);

                // Arrange
                var alarm7008 = new ZebraSdk.Card.Containers.Fakes.ShimAlarmInfo
                {
                    ValueGet = () => { return 7008; }
                };

                var printerStatus = new ZebraSdk.Card.Containers.Fakes.ShimPrinterStatusInfo
                {
                    AlarmInfoGet = () => { return alarm7008; },
                    ErrorInfoGet = () => { return alarm7008; }
                };

                var conn = Substitute.For<ZebraSdk.Comm.Connection>();
                var zebraPrinter = Substitute.For<ZebraSdk.Card.Printer.ZebraCardPrinter>();
                zebraPrinter.GetPrinterStatus().Returns(printerStatus);

                ZebraZC3 zebraZC3 = new ZebraZC3(conn, zebraPrinter, errorFactory);

                // Act 
                string status;
                List<IDeviceErrorDescriptor> errorList;
                var boolStatus = zebraZC3.CheckPrinterStatus(out status, out errorList);

                // Assert
                zebraPrinter.Received(1).GetPrinterStatus();
                errorFactory.Received(1).GetErrorDescription(Arg.Any<int>());
                boolStatus.Should().BeFalse(because: "Both ErrorInfo and AlarmInfo have issues. Should trigger false on either having an issue or both having an issue.");
                errorList.Should().HaveCount(1, because: "Same error thrown by both AlarmInfo and ErrorInfo.");
                errorList[0].Should().BeEquivalentTo(errorDesc7008);
            }
        }

        [TestMethod()]
        public void ZebraZC3_CheckPrinterStatus_AlarmInfoAndErrorInfoHaveDifferentErrorsReturnFalse()
        {
            using (ShimsContext.Create())
            {
                var errorDesc7008 = Substitute.For<IDeviceErrorDescriptor>();
                errorDesc7008.Code.Returns(7008);
                errorDesc7008.Description.Returns("Printer door open");
                errorDesc7008.HelpfulHint.Returns("Ensure the door is closed.");

                var errorDesc7028 = Substitute.For<IDeviceErrorDescriptor>();
                errorDesc7028.Code.Returns(7028);
                errorDesc7028.Description.Returns("Printer thingo is open");
                errorDesc7028.HelpfulHint.Returns("Ensure the thingo is closed.");

                var errorFactory = Substitute.For<Error.IZebraErrorFactory>();
                errorFactory.GetErrorDescription(7008).Returns(errorDesc7008);
                errorFactory.GetErrorDescription(7028).Returns(errorDesc7028);

                // Arrange
                var alarm7008 = new ZebraSdk.Card.Containers.Fakes.ShimAlarmInfo
                {
                    ValueGet = () => { return 7008; }
                };
                var alarm7028 = new ZebraSdk.Card.Containers.Fakes.ShimAlarmInfo
                {
                    ValueGet = () => { return 7028; }
                };


                var printerStatus = new ZebraSdk.Card.Containers.Fakes.ShimPrinterStatusInfo
                {
                    AlarmInfoGet = () => { return alarm7008; },
                    ErrorInfoGet = () => { return alarm7028; }
                };

                var conn = Substitute.For<ZebraSdk.Comm.Connection>();
                var zebraPrinter = Substitute.For<ZebraSdk.Card.Printer.ZebraCardPrinter>();
                zebraPrinter.GetPrinterStatus().Returns(printerStatus);

                ZebraZC3 zebraZC3 = new ZebraZC3(conn, zebraPrinter, errorFactory);

                // Act        
                string status;
                List<IDeviceErrorDescriptor> errorList;
                var boolStatus = zebraZC3.CheckPrinterStatus(out status, out errorList);

                // Assert
                zebraPrinter.Received(1).GetPrinterStatus();
                errorFactory.Received(1).GetErrorDescription(7008);
                errorFactory.Received(1).GetErrorDescription(7028);
                boolStatus.Should().BeFalse(because: "Both ErrorInfo and AlarmInfo have issues. Should trigger false on either having an issue or both having an issue.");
                errorList.Should().HaveCount(2, because: "Two different errors received from alarm and errorinfo.");
                errorList[0].Should().BeEquivalentTo(errorDesc7008);
                errorList[1].Should().BeEquivalentTo(errorDesc7028);
            }
        }


        [TestMethod()]
        public void ZebraZC3_ReadMagStripeEncoderInstalled_ReturnsPopulatedMagneticTrackData()
        {
            using (ShimsContext.Create())
            {
                // Arrange 
                var errorFactory = Substitute.For<Error.IZebraErrorFactory>();

                var trackData = new ZebraSdk.Card.Containers.Fakes.ShimMagTrackData();
                trackData.Track1Get = () => "Track1DataHere";
                trackData.Track2Get = () => "Track2DataHere";
                trackData.Track3Get = () => "Track3DataHere";

                var conn = Substitute.For<ZebraSdk.Comm.Connection>();

                var zebraPrinter = Substitute.For<ZebraSdk.Card.Printer.ZebraCardPrinter>();
                zebraPrinter.HasMagneticEncoder().Returns(true);
                zebraPrinter
                    .ReadMagData(ZebraSdk.Card.Enumerations.DataSource.Track1 | ZebraSdk.Card.Enumerations.DataSource.Track2 | ZebraSdk.Card.Enumerations.DataSource.Track3)
                    .Returns(trackData);

                ZebraZC3 zebraZC3 = new ZebraZC3(conn, zebraPrinter, errorFactory);

                // Act                
                var returnedTrackData = zebraZC3.ReadMagStripe();


                // Assert
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void GenerateCardPrintDetailsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateFrontGraphicsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateBackGraphicsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PrintTest()
        {
            Assert.Fail();
        }
    }
}