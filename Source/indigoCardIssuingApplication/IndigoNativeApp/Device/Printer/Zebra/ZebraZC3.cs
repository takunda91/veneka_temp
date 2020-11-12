using IndigoDesktopApp.Device.Printer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zebra.Sdk.Card.Printer;
using Zebra.Sdk.Card.Printer.Discovery;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer.Discovery;
using Zebra.Sdk.Card.Enumerations;
using ZebraEnums = Zebra.Sdk.Card.Enumerations;
using Zebra.Sdk.Printer;
using Zebra.Sdk.Card.Zmotif.Printer.Internal;
using Zebra.Sdk.Card.Containers;
using System.Threading;
using Zebra.Sdk.Card.Errors;
using Zebra.Sdk.Card.Job;
using Zebra.Sdk.Card.Graphics;
using System.Drawing;
using Zebra.Sdk.Card.Exceptions;
using System.Linq;
using Zebra.Sdk.Card.Zmotif.Job;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra
{
    public class ZebraZC3 : IPrinter, AlarmHandler
    {
        public event DeviceNotificationEventHandler OnDeviceNotifcation;

        private const int CARD_FEED_TIMEOUT = 30000;

        private readonly DiscoveredPrinter _discoveredPrinter;
        private Connection _connection = null;
        private ZebraCardPrinter _zebraCardPrinter = null;
        private Error.IZebraErrorFactory _errorFactory = null;
        private CardSource _cardSource = CardSource.ATM;
        private int _currentPrintJob = 0;
        private bool _skipPrinting = false;

        public ZebraZC3()
        {
            // Initialise Properties            
            Name = Model = Manufacturer = DeviceId = string.Empty;
        }

        public ZebraZC3(DiscoveredPrinter discoveredPrinter) : this()
        {
            _discoveredPrinter = discoveredPrinter ?? throw new ArgumentNullException(nameof(discoveredPrinter));
            _errorFactory = new Error.ZebraErrorFactory();

            // Set Properties from dicovered printer
            string model;
            if(_discoveredPrinter.DiscoveryDataMap.TryGetValue("MODEL", out model))
            {
                Model = model;
            }

            string mfg;
            if (_discoveredPrinter.DiscoveryDataMap.TryGetValue("MFG", out mfg))
            {
                Manufacturer = mfg;
            }

            string serial;
            if (_discoveredPrinter.DiscoveryDataMap.TryGetValue("SERIAL_NUMBER", out serial))
            {
                DeviceId = serial;
            }

            Name = string.Format("{0}[{1}]", model, serial);
        }

        public ZebraZC3(Connection connection, ZebraCardPrinter zebraCardPrinter, Error.IZebraErrorFactory zebraErrorFactory) : this()
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _zebraCardPrinter = zebraCardPrinter ?? throw new ArgumentNullException(nameof(zebraCardPrinter));
            _errorFactory = zebraErrorFactory ?? throw new ArgumentNullException(nameof(zebraErrorFactory));

            // TODO: See how we can set properties from connection or zebraCardPrinter
        }

        protected virtual void RaiseDeviceNotification(string message, bool isCritical, EventArgs e)
        {
            OnDeviceNotifcation?.Invoke(this, message, isCritical, e);
        }

        public void AlarmEvent(AlarmInfo alarm)
        {
            RaiseDeviceNotification(alarm.Description, true, EventArgs.Empty);
        }  

        #region Properties
        public string Name
        {
            get;
            private set;
        }

        public string Manufacturer
        {
            get;
            private set;
        }

        public string Model
        {
            get;
            private set;
        }

        public string FirmwareVersion
        {
            get;
            private set;
        }

        public int ComPort
        {
            get;
            private set;
        }

        public string DeviceId
        {
            get;
            private set;
        }
        #endregion

        #region Connect/Disconnect
        public short Connect()
        {
            try
            {
                _connection = _discoveredPrinter.GetConnection();
                _connection.Open();

                if (_connection.Connected)
                {
                    _zebraCardPrinter = ZebraCardPrinterFactory.GetInstance(_connection);

                    List<JobStatus> _joblist= _zebraCardPrinter.GetJobList();

                    if (_joblist != null&&_joblist.Count>0)
                    {
                        
                        _currentPrintJob = _joblist.Max(i => i.ID);
                    }
                    else
                        _currentPrintJob = 0;

                    _zebraCardPrinter.RegisterAlarmHandler(this);

                    return PrinterCodes.Success;
                }

                return PrinterCodes.ConnectFailed;
            }
            catch
            {
                CloseQuietly(_connection, _zebraCardPrinter);
                throw;
            }
        }

        public short Disconnect()
        {
            _connection.Close();
            return PrinterCodes.Success;
        }
        #endregion

        public void SetDeviceSettings(Dictionary<string, string> settings)
        {
            if (settings != null)
            {
                string cardSourceStr = string.Empty;
                if (settings.TryGetValue("Zebra.ZC.CardSource", out cardSourceStr))
                {
                    CardSource cardSourceEnum;
                    if (Enum.TryParse<CardSource>(cardSourceStr, true, out cardSourceEnum))
                    {
                        _cardSource = cardSourceEnum;
                    }
                }

                string skipPrint = "false";
                if(settings.TryGetValue("Indigo.Printer.SkipPrint", out skipPrint))
                {
                    _skipPrinting = bool.Parse(skipPrint);
                }
            }
        }

        public IPrinterDetailFactory PrinterDetailFactory()
        {
            return new ZebraDetailFactory();
        }

        public Dictionary<string, string> GetPrinterInfo()
        {
            var printerInfoDictionary = new Dictionary<string, string>();

            printerInfoDictionary.Add("Total Cards Printed", _zebraCardPrinter.GetCardCount().TotalCards.ToString());
            //printerInfoDictionary.Add("Total Cards Printed", _zebraCardPrinter.GetPrinterStatus(). .GetCardCount().TotalCards.ToString());

            var printerStatus = _zebraCardPrinter.GetPrinterStatus();
            var mediaInfo = _zebraCardPrinter.GetMediaInformation();

            if(mediaInfo.Count > 0)
            {
                printerInfoDictionary.Add("Ribbon", mediaInfo[0].Description);                
                printerInfoDictionary.Add("Panels Remaining", mediaInfo[0].PanelsRemaining.ToString());
            }            

            return printerInfoDictionary;
        }

        public short Print(string productBin, ICardPrintDetails cardPrintDetails)
        {

            if (!HasMagEncoder()) // If we have a mag reader then read the mag and check product bin matches card inserted
            {
                IDeviceMagData magData;
                return ReadAndPrint(productBin, cardPrintDetails, out magData);
            }
            else // No mag reader installed, just do print
            {

               

                _currentPrintJob = PositionAndPrint(cardPrintDetails, new Dictionary<string, string> {
                        { ZebraCardJobSettingNames.CARD_SOURCE, _cardSource.ToString() },
                        { ZebraCardJobSettingNames.CARD_DESTINATION, CardDestination.Eject.ToString() }
                    });
                var pollJobTask = PollJobStatusAsync();

                // Poll job status
                if (_currentPrintJob != -99)
                {
                    pollJobTask.Wait();
                    return pollJobTask.Result;
                }
            }
            return PrinterCodes.Success;
        }

        public short ReadAndPrint(string productBin, ICardPrintDetails cardPrintDetails, out IDeviceMagData magData)
        {
            magData = null;

            if (cardPrintDetails == null)
            {
                throw new Exception("No printing data available to print to card.");
            }

            if (!HasMagEncoder())
            {
                throw new Exception("Device does not have a Magstripe encoder installed.");
            }

            string status;
            List<IDeviceErrorDescriptor> errors;
            if (CheckPrinterStatus(out status, out errors))
            {
                try
                {



                    // Set the card destination to 'Hold' after we have read the mag stripe

                    _zebraCardPrinter.SetJobSettings(new Dictionary<string, string> {
                        { ZebraCardJobSettingNames.CARD_SOURCE, _cardSource.ToString() },
                        { ZebraCardJobSettingNames.CARD_DESTINATION, CardDestination.Hold.ToString() }
                    });

                    // Set the card source to 'Internal' and destination to 'Eject'
                    _currentPrintJob = PositionAndPrint(cardPrintDetails, new Dictionary<string, string> {
                        { ZebraCardJobSettingNames.CARD_SOURCE, CardSource.Internal.ToString() },
                        { ZebraCardJobSettingNames.CARD_DESTINATION, CardDestination.Eject.ToString() }
                    });

                    
                     ZebraMagData deviceMagData = new ZebraMagData(_zebraCardPrinter.ReadMagData(DataSource.Track1 | DataSource.Track2 | DataSource.Track3));


                    // Check that product matches BIN
                    if (!deviceMagData.TrackDataToString(2).StartsWith(productBin))
                    {
                        return PrinterCodes.ProductBinAndCardMismatch;
                    }
                    var pollJobTask = PollJobStatusAsync();

                    magData = deviceMagData;

                    // Poll job status
                    if (_currentPrintJob != -99)
                    {
                        pollJobTask.Wait();
                      //  _zebraCardPrinter.EjectCard();
                        return pollJobTask.Result;
                    }

                    return PrinterCodes.Success;
                }
                catch
                {
                    _zebraCardPrinter.EjectCard();
                    throw;
                }
            }

            throw new Exception("Printer in failed state.");
        }

        private int PositionAndPrint(ICardPrintDetails cardPrintDetails, Dictionary<string, string> jobSettings)
        {
            _zebraCardPrinter.SetJobSettings(jobSettings);

            // Build graphics for print job
            List<GraphicsInfo> _graphics = new List<GraphicsInfo>();
            _graphics.Add(CreateFrontGraphics(_zebraCardPrinter, (ZebraCardPrintDetails)cardPrintDetails));
           // _graphics.Add(CreateBackGraphics(_zebraCardPrinter, (ZebraCardPrintDetails)cardPrintDetails));

            if(_skipPrinting)
            {
                return -99;
            }

            // Send job
            return _zebraCardPrinter.Print(1, _graphics);
        }

        public short Cancel()
        {
            if (_currentPrintJob >= 0)
            {
                _zebraCardPrinter.Cancel(_currentPrintJob);
            }
            return PrinterCodes.Success;
        }        

        public short PrintJobStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if there are no issues with the printers status.
        /// </summary>
        /// <param name="errorDescription"></param>
        /// <returns></returns>
        public bool CheckPrinterStatus(out string status, out List<IDeviceErrorDescriptor> errorDescription)
        {
            errorDescription = new List<IDeviceErrorDescriptor>();

            var printerStatus = _zebraCardPrinter.GetPrinterStatus();

            status = printerStatus.Status;

            if(printerStatus.ErrorInfo.Value == ZebraCardErrors.SYSTEM_NO_ERROR && 
                printerStatus.AlarmInfo.Value == ZebraCardErrors.SYSTEM_NO_ERROR)
            {
                // return back no error description;
                errorDescription.Add(_errorFactory.GetErrorDescription(ZebraCardErrors.SYSTEM_NO_ERROR));
                return true;
            }

            // Add AlarmInfo to error description if it triggered error
            if(printerStatus.AlarmInfo.Value != ZebraCardErrors.SYSTEM_NO_ERROR)
            {
                errorDescription.Add(_errorFactory.GetErrorDescription(printerStatus.AlarmInfo.Value));
            }

            // Add ErrorInfo to error description if it triggered and error and the error is different from the AlarmInfo
            if (printerStatus.ErrorInfo.Value != ZebraCardErrors.SYSTEM_NO_ERROR && 
                printerStatus.ErrorInfo.Value != printerStatus.AlarmInfo.Value)
            {
                errorDescription.Add(_errorFactory.GetErrorDescription(printerStatus.ErrorInfo.Value));
            }
            
            return false;
        }

        public bool GetMediaInfo()
        {
            var printerStatus = _zebraCardPrinter.GetPrinterStatus();
             var mediaInfo = _zebraCardPrinter.GetMediaInformation();

            var states = _zebraCardPrinter.GetSensorStates();
            var x2 = _zebraCardPrinter.GetSensorValues();
            var x3 = _zebraCardPrinter.GetCardCount();
            var x4 = _zebraCardPrinter.GetPrinterInformation();

            return false;
        }

        #region Graphics
        public GraphicsInfo CreateFrontGraphics(ZebraCardPrinter zebraCardPrinter, ZebraCardPrintDetails printDetails)
        {
            using (ZebraCardGraphics graphics = new ZebraCardGraphics(zebraCardPrinter))
            {
                int? fillColour = null;
                graphics.Initialize(0, 0, OrientationType.Landscape, PrintType.MonoK, fillColour);

                // Loop through front text, if any
                if(printDetails.FrontPanelText != null & printDetails.FrontPanelText.Length > 0)
                {
                    foreach(var text in printDetails.FrontPanelZebraText)
                    {
                        text.Add(graphics);
                    }
                }

                // Loop through front images, if any
                if (printDetails.FrontPanelImages != null & printDetails.FrontPanelImages.Length > 0)
                {
                    foreach (var image in printDetails.FrontPanelZebraImages)
                    {
                        image.Add(graphics);
                    }
                }

                ZebraCardImageI zebraCardImage = graphics.CreateImage();
                return AddImage(CardSide.Front, PrintType.MonoK, 0, 0, -1, zebraCardImage);                
            }
        }

        public GraphicsInfo CreateBackGraphics(ZebraCardPrinter zebraCardPrinter, ZebraCardPrintDetails printDetails)
        {
            using (ZebraCardGraphics graphics = new ZebraCardGraphics(zebraCardPrinter))
            {
                int? fillColour = null;
                graphics.Initialize(0, 0, OrientationType.Landscape, PrintType.MonoK, fillColour);

                // Loop through back text, if any
                if (printDetails.BackPanelText != null & printDetails.BackPanelText.Length > 0)
                {
                    foreach (var text in printDetails.BackPanelZebraText)
                    {
                        text.Add(graphics);
                    }
                }

                // Loop through back images, if any
                if (printDetails.BackPanelImages != null & printDetails.BackPanelImages.Length > 0)
                {
                    foreach (var image in printDetails.BackPanelZebraImages)
                    {
                        image.Add(graphics);
                    }
                }

                ZebraCardImageI zebraCardImage = graphics.CreateImage();
                return AddImage(CardSide.Back, PrintType.MonoK, 0, 0, -1, zebraCardImage);
            }
        }

        private GraphicsInfo AddImage(CardSide side, PrintType printType, int xOffset, int yOffset, int fillColor, ZebraCardImageI zebraCardImage)
        {
            return new GraphicsInfo
            {
                Side = side,
                PrintType = printType,
                GraphicType = zebraCardImage != null ? GraphicType.BMP : GraphicType.NA,
                XOffset = xOffset,
                YOffset = yOffset,
                FillColor = fillColor,
                Opacity = 0,
                Overprint = false,
                GraphicData = zebraCardImage ?? null
            };
        }
        #endregion Graphics

        #region JobSettings
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ConnectionException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="Zebra.Sdk.Settings.SettingsException"></exception>
        /// <exception cref="Zebra.Sdk.Card.Exceptions.ZebraCardException"></exception>
        public void DisplayJobSettings()
        {
            if (_zebraCardPrinter != null)
            {
                Console.WriteLine("Available Job Settings for myDevice:");
                HashSet<string> availableJobSettings = _zebraCardPrinter.GetJobSettings();
                foreach (string setting in availableJobSettings)
                {
                    Console.WriteLine($"{setting}: Range = ({_zebraCardPrinter.GetJobSettingRange(setting)})");
                }

                Console.WriteLine("\nCurrent Job Setting Values for myDevice:");
                Dictionary<string, string> allJobSettingValues = _zebraCardPrinter.GetAllJobSettingValues();
                foreach (string settingName in allJobSettingValues.Keys)
                {
                    Console.WriteLine($"{settingName}:{allJobSettingValues[settingName]}");
                }
            }
        }
        #endregion JobSettings

        #region JobStatus

         /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ConnectionException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="Zebra.Sdk.Settings.SettingsException"></exception>
        /// <exception cref="Zebra.Sdk.Card.Exceptions.ZebraCardException"></exception>
        private async Task<short> PollJobStatusAsync()
        {
            JobStatusInfo jobStatusInfo = new JobStatusInfo();
            bool isFeeding = false;

            long start = Math.Abs(Environment.TickCount);
            while (true)
            {
                jobStatusInfo = _zebraCardPrinter.GetJobStatus(_currentPrintJob);

                if (!isFeeding)
                {
                    start = Math.Abs(Environment.TickCount);
                }

                isFeeding = jobStatusInfo.CardPosition.Contains("feeding");

                string alarmDesc = jobStatusInfo.AlarmInfo.Value > 0 ? $" ({jobStatusInfo.AlarmInfo.Description})" : "";
                string errorDesc = jobStatusInfo.ErrorInfo.Value > 0 ? $" ({jobStatusInfo.ErrorInfo.Description})" : "";

                RaiseDeviceNotification($"Job {_currentPrintJob}: status:{jobStatusInfo.PrintStatus}, position:{jobStatusInfo.CardPosition}, alarm:{jobStatusInfo.AlarmInfo.Value}{alarmDesc}, error:{jobStatusInfo.ErrorInfo.Value}{errorDesc}",
                    false, EventArgs.Empty);
                

                if (jobStatusInfo.PrintStatus.Contains("done_ok"))
                {
                    return PrinterCodes.Success;                    
                }
                else if (jobStatusInfo.PrintStatus.Contains("error"))
                {
                    RaiseDeviceNotification($"The job encountered an error [{jobStatusInfo.ErrorInfo.Description}] and was cancelled.",
                        true,
                        EventArgs.Empty);
                    return PrinterCodes.PrintJobError;
                }
                else if (jobStatusInfo.PrintStatus.Contains("cancelled"))
                {
                    RaiseDeviceNotification($"The job was cancelled.",
                        true,
                        EventArgs.Empty);
                    return PrinterCodes.PrintJobCancelled;
                }
                else if (jobStatusInfo.ErrorInfo.Value > 0)
                {
                    RaiseDeviceNotification($"The job encountered an error [{jobStatusInfo.ErrorInfo.Description}] and was cancelled.",
                        true,
                        EventArgs.Empty);
                    _zebraCardPrinter.Cancel(_currentPrintJob);
                }
                else if (jobStatusInfo.PrintStatus.Contains("in_progress") && isFeeding)
                {
                    if (Math.Abs(Environment.TickCount) > start + CARD_FEED_TIMEOUT)
                    {
                        RaiseDeviceNotification("The job timed out waiting for a card and was cancelled.",
                        true,
                        EventArgs.Empty);
                        
                        _zebraCardPrinter.Cancel(_currentPrintJob);
                    }
                }

                await Task.Delay(1000);
            }
        }
        #endregion JobStatus

        #region IMagEncoder
        public bool HasMagEncoder()
        {
            if(_zebraCardPrinter != null)
            {
                return _zebraCardPrinter.HasMagneticEncoder();
            }

            throw new Exception("Device not connected! Method may only be used after device has successful Connect().");
        }

        public IDeviceMagData ReadMagStripe()
        {
            if (!HasMagEncoder())
            {
                throw new Exception("Device does not have a Magstripe encoder installed.");
            }

            string status;
            List<IDeviceErrorDescriptor> errors;
            if(CheckPrinterStatus(out status, out errors))
            {
                //if (!ZebraPrinterStatuses.IsStatus(ZebraStatuses.CardReady, status))
                //{
                //    throw new Exception("Please manually feed ");
                //}

                try
                {
                    // Put printer in ATM mode and Eject
                    Dictionary<string, string> jobSettings = new Dictionary<string, string> {
                        { ZebraCardJobSettingNames.CARD_SOURCE, _cardSource.ToString() },
                        { ZebraCardJobSettingNames.CARD_DESTINATION, CardDestination.Eject.ToString() }
                    };

                    _zebraCardPrinter.SetJobSettings(jobSettings);

                    var deviceMagData = _zebraCardPrinter.ReadMagData(DataSource.Track1 | DataSource.Track2 | DataSource.Track3);

                    return new ZebraMagData(deviceMagData);
                }
                catch (ZebraCardException zcEx)
                {
                    if (zcEx.ErrorCode == 13006)
                    {                        
                        RaiseDeviceNotification(PrinterErrorsResource.CardLoadTimeout, false, EventArgs.Empty);
                    }
                    else
                    {
                        RaiseDeviceNotification(zcEx.ErrorCode + " : " + zcEx.Message, true, EventArgs.Empty);
                    }
                }
                finally
                {
                    _zebraCardPrinter.EjectCard();
                }
            }

            throw new Exception("Printer in failed state.");
        }
        #endregion

        #region PrinterList
        /// <summary>
        /// Lists all the printers connected (USB) to the workstation
        /// </summary>
        /// <returns></returns>
        public static ZebraZC3[] GetPrinterList()
        {
            List<ZebraZC3> devices = new List<ZebraZC3>();

            foreach (DiscoveredUsbPrinter usbPrinter in UsbDiscoverer.GetZebraUsbPrinters(new ZebraCardPrinterFilter()))
            {
                devices.Add(new ZebraZC3(usbPrinter));
            }

            return devices.ToArray();

        }
        #endregion

        #region CleanUp
        private static void CloseQuietly(Connection connection, ZebraCardPrinter zebraCardPrinter)
        {
            try
            {
                if (zebraCardPrinter != null)
                {
                    zebraCardPrinter.Destroy();
                }
            }
            catch { }

            try
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            catch { }
        }
        #endregion CleanUp

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                CloseQuietly(_connection, _zebraCardPrinter);
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ZebraZC3x() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
     class NetworkDiscoveryHandler : DiscoveryHandler
    {

        private List<DiscoveredPrinter> printers = new List<DiscoveredPrinter>();
        private AutoResetEvent discoCompleteEvent = new AutoResetEvent(false);

        public void DiscoveryError(string message)
        {
            Console.WriteLine($"An error occurred during discovery: {message}.");
            discoCompleteEvent.Set();
        }

        public void DiscoveryFinished()
        {
            discoCompleteEvent.Set();
        }

        public void FoundPrinter(DiscoveredPrinter printer)
        {
            printers.Add(printer);
        }

        public List<DiscoveredPrinter> DiscoveredPrinters
        {
            get => printers;
        }

        public AutoResetEvent DiscoveryCompleteEvent
        {
            get => discoCompleteEvent;
        }
    }

}
