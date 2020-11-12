using IndigoDesktopApp.Device.Printer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZMTGraphics;
using ZXPPRINTERLib;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra
{
    //
    // Need to register the ZXPPrinter.dll when Native app is installed!!!
    // regsvr32 C:\Zebra\ZxpPrinter-SDK\PrinterSDK\x86\ZXPPrinter.dll
    //

    public sealed class ZebraZXP3 : IPrinter
    {
        public event DeviceNotificationEventHandler OnDeviceNotifcation;

        private const string MANUFACURER = "Zebra";
        private const string MODEL = "ZXP Series 1/3";

        public const int NO_ERROR = 0;
        public const int OUT_OF_CARDS = 4016;
        public const int OUT_OF_CARDS_ERROR = 4001;
        public const int ACTION_ID_NOT_FOUND = 13003;
        public const int JOB_CANCELLED_BY_USER = 13006;

        private const int DEFAULT_WIDTH = 1024;
        private const int DEFAULT_HEIGHT = 648;

        private readonly string _deviceName;
        private Job _zebraSDK = null;

        private int _ribbonType;
        private string _ribbonDescription;
        private string _ribbonOem;
        private int _remainingPrints;
        private int _ribbonSize;

        private string _status;
        private int _errorCode;
        private int _jobsPending;
        private int _jobsActive;
        private int _jobsComplete;
        private int _jobErrors;
        private int _jobsTotal;
        private int _nextActionId;

        public ZebraZXP3(string deviceName)
        {
            if (String.IsNullOrWhiteSpace(deviceName))
                throw new ArgumentNullException("deviceName", "Printer name cannot be null or empty");
            _deviceName = deviceName;
            Manufacturer = MANUFACURER;
            Model = MODEL;
        }

        /// <summary>
        /// Lists all the printers connected (USB) to the workstation
        /// </summary>
        /// <returns></returns>
        public static ZebraZXP3[] GetPrinterList()
        {
            string[] deviceList = new string[0];
            Job sdk = new Job();

            try
            {
                object objList = null;

                sdk.GetPrinters(ConnectionTypeEnum.USB, out objList);

                if (objList != null)
                {
                    deviceList = ((System.Collections.IEnumerable)objList).Cast<object>()
                                                                          .Select(x => x.ToString())
                                                                          .ToArray();
                }
            }
            finally //be sure to release the interface to avoid memory leaks
            {
                do
                {
                    System.Threading.Thread.Sleep(10);
                }
                while (System.Runtime.InteropServices.Marshal.FinalReleaseComObject(sdk) > 0);
            }

            List<ZebraZXP3> printers = new List<ZebraZXP3>();
            foreach (var device in deviceList)
            {
                printers.Add(new ZebraZXP3(device));
            }

            return printers.ToArray();
        }

        #region IPrinter
        public short Connect()
        {
            _zebraSDK = new Job();

            //Need to look at when we open the connection and close it;
            var alarm = _zebraSDK.Open(_deviceName);


            if (alarm == 0 && _zebraSDK.IsOpen)
            {
                string vendor, model, serialNumber, mac, headSerialNumber, oemCode, firmwareVersion;
                alarm = _zebraSDK.Device.GetDeviceInfo(out vendor, out model, out serialNumber, out mac, out headSerialNumber, out oemCode, out firmwareVersion);

                DeviceId = serialNumber;
                Manufacturer = vendor;
                Model = model;
                FirmwareVersion = firmwareVersion;

                alarm = _zebraSDK.Device.GetRibbonParams(out _ribbonType, out _ribbonDescription, out _ribbonOem, out _ribbonSize, out _remainingPrints);
                alarm = _zebraSDK.Device.GetPrinterStatus(out _status, out _errorCode, out _jobsPending, out _jobsActive, out _jobsComplete, out _jobErrors, out _jobsTotal, out _nextActionId);
            }
            else
            {
                throw new Exception(alarm.ToString() + "-" + _zebraSDK.Device.GetStatusMessageString(alarm));
            }

            return PrinterCodes.Success;
        }

        public short Disconnect()
        {
            return PrinterCodes.Success;
        }

        //public short AddText(string text, float x, float y, CardSidePanel side, string font, float fontSize, int fontColourARGB, Orientation orientation)
        //{
        //    string errMsg;

        //    ZMotifGraphics.FontTypeEnum zebraFontType;
        //    zebraFontType = (ZMotifGraphics.FontTypeEnum)((int)fontType);

        //    ZMotifGraphics.ImageOrientationEnum zebraOrientation;
        //    zebraOrientation = (ZMotifGraphics.ImageOrientationEnum)((int)orientation);

        //    SideEnum zebraSide;
        //    zebraSide = (SideEnum)((int)side);

        //    //System.Drawing.Color.Black.ToArgb()
        //    byte[] zebraImage = BuildText(text, x, y, font, fontSize, fontColourARGB, zebraFontType, zebraOrientation, (ZMotifGraphics.RibbonTypeEnum)_ribbonType, out errMsg);

        //    if (zebraImage != null)
        //        _zebraSDK.BuildGraphicsLayers(zebraSide, GetPrintType((ZMotifGraphics.RibbonTypeEnum)_ribbonType), GraphicTypeEnum.BMP, zebraImage);

        //    return PrinterCodes.Success;
        //}

        public Dictionary<string, string> GetPrinterInfo()
        {

            return new Dictionary<string, string>();
        }

        IDeviceMagData IMagEncoder.ReadMagStripe()
        {
            throw new NotImplementedException();
        }

        public IPrinterDetailFactory PrinterDetailFactory()
        {
            throw new NotImplementedException();
        }

        public short AddImage(byte[] image, float x, float y, int width, int height, CardSidePanel side, Orientation orientation)
        {
            string errMsg = string.Empty;

            ZMotifGraphics.ImageOrientationEnum zebraOrientation;
            zebraOrientation = (ZMotifGraphics.ImageOrientationEnum)((int)orientation);

            SideEnum zebraSide;
            zebraSide = (SideEnum)((int)side);

            var zebraImage = BuildImageXY(image, x, y, width, height, zebraOrientation, (ZMotifGraphics.RibbonTypeEnum)_ribbonType, out errMsg);

            if (zebraImage != null)
                _zebraSDK.BuildGraphicsLayers(zebraSide, GetPrintType((ZMotifGraphics.RibbonTypeEnum)_ribbonType), GraphicTypeEnum.BMP, zebraImage);

            return PrinterCodes.Success;
        }

        public short Print(string productBin, ICardPrintDetails cardPrintDetails)
        {
            int alarm = 0;
            int actionID = 0;
            int copies = 1;
            string err;
            string printingStatus, magStatus, contactStatus, contactlessStatus;
            int errorCode, copiesCompleted, copiesRequested;

            try
            {
                _zebraSDK.JobControl.FeederSource = FeederSourceEnum.AutoDetect;
                _zebraSDK.JobControl.Destination = DestinationTypeEnum.Eject;

                alarm = _zebraSDK.PrintGraphicsLayers(copies, out actionID);
                int count = 0;
                while (count <= 30)
                {
                    //string printingStatus, magStatus, contactStatus, contactlessStatus;
                    //int errorCode, copiesCompleted, copiesRequested;

                    alarm = _zebraSDK.GetJobStatus(actionID, out printingStatus, out errorCode, out copiesCompleted, out copiesRequested,
                                                        out magStatus, out contactStatus, out contactlessStatus);                   

                    if (printingStatus.Equals("done_ok", StringComparison.OrdinalIgnoreCase)) // job completed successfully                    
                        return PrinterCodes.Success;
                    else if (printingStatus.ToLower().Contains("error")) // job failed                    
                        return PrinterCodes.UnknownError;
                    else if (alarm > 0 && alarm != 4001) //Some alarm raised other than out of cards
                        return PrinterCodes.UnknownError;
                    
                    System.Threading.Thread.Sleep(1000);
                    count++;
                }

                return PrinterCodes.PrintJobTimedOut;
            }
            catch (Exception ex)
            {
                //error += ex.ToString();
            }
            finally
            {                
                _zebraSDK.ClearGraphicsLayers();

                if (alarm > 0 && actionID > 0)
                {
                    _zebraSDK.JobCancel(actionID);
                    err = _zebraSDK.Device.GetStatusMessageString(alarm);
                    //TODO : Log the alarm code
                }
            }

            return PrinterCodes.UnknownError;
        }

        public short PrintJobStatus()
        {
            return PrinterCodes.Success;
        }

        public short Cancel()
        {
            return _zebraSDK.JobCancel(0);
        }
        #endregion

        public void Print(int copies, out string errMsg, out short alarm)
        {
            errMsg = string.Empty;
            alarm = 0;
            int actionID = 0;

            try
            {
                //if (bmpFront == null || bmpBack == null)
                //{
                //    errMsg = "Print Error: No Images to Print ";
                //    return;
                //}

                _zebraSDK.JobControl.FeederSource = FeederSourceEnum.AutoDetect;
                _zebraSDK.JobControl.Destination = DestinationTypeEnum.Eject;
                
                //bmpFront = BuildImage(bmpFront, ZMotifGraphics.ImageOrientationEnum.Landscape, ZMotifGraphics.RibbonTypeEnum.Color, out errMsg);
                byte[] bmpFront = BuildText("FFRROONNT", 50, 50, "Arial", 12, System.Drawing.Color.Black.ToArgb(), ZMotifGraphics.FontTypeEnum.Bold, ZMotifGraphics.ImageOrientationEnum.Landscape, (ZMotifGraphics.RibbonTypeEnum)_ribbonType, out errMsg);

                if (bmpFront != null)
                {
                    _zebraSDK.BuildGraphicsLayers(SideEnum.Front, (PrintTypeEnum)_ribbonType, GraphicTypeEnum.BMP, bmpFront);
                }

                //byte[] bmpBack = BuildImage(bmpBack, ZMotifGraphics.ImageOrientationEnum.Landscape, ZMotifGraphics.RibbonTypeEnum.Color, out errMsg);

                //byte[] bmpBack = BuildText("BACK", ZMotifGraphics.FontTypeEnum.Bold, ZMotifGraphics.ImageOrientationEnum.Landscape, ZMotifGraphics.RibbonTypeEnum.MonoK, out errMsg);

                //if (bmpBack != null)
                //{
                //    _zebraSDK.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.MonoK, GraphicTypeEnum.BMP, bmpBack);
                //}

                alarm = _zebraSDK.PrintGraphicsLayers(copies, out actionID);
                int count = 0;
                while (count <= 60 && alarm == 0)
                {
                    string printingStatus, magStatus, contactStatus, contactlessStatus;
                    int errorCode, copiesCompleted, copiesRequested;

                    alarm = _zebraSDK.GetJobStatus(actionID, out printingStatus, out errorCode, out copiesCompleted, out copiesRequested,
                                                        out magStatus, out contactStatus, out contactlessStatus);

                    System.Threading.Thread.Sleep(1000);
                    count++;
                }
            }
            catch (Exception ex)
            {
                errMsg += ex.ToString();
                
                // System.Windows.Forms.MessageBox.Show(ex.ToString(), "DualSidePrint");
            }
            finally
            {
                _zebraSDK.ClearGraphicsLayers();
            }

            errMsg += _zebraSDK.Device.GetStatusMessageString(alarm);

        }

        public short ReadAndPrint(string productBin, ICardPrintDetails cardPrintDetails, out IDeviceMagData magData)
        {
            throw new NotImplementedException();
        }

        #region Printer Methods   
        #region Open/Close Connection Methods
        private bool OpenConnection(string printerName, out string errMsg)
        {
            errMsg = String.Empty;

            try
            {
                if (string.IsNullOrEmpty(printerName))
                {
                    errMsg = "NO Printer Selected";
                    return false;
                }

                if (!_zebraSDK.IsOpen)
                {
                    _zebraSDK.Open(printerName);

                    if (!_zebraSDK.IsOpen)
                    {
                        errMsg = "Failed to open connection to printer";
                    }
                    else
                        return true;
                }
            }
            catch (Exception ex)
            {
                errMsg = "OpenConnection exception: " + ex.Message;
            }

            return false;
        }

        private bool CloseConnection(out string errMsg)
        {
            errMsg = string.Empty;

            try
            {
                if (_zebraSDK == null)
                    return true;

                _zebraSDK.Close();

                return true;
            }
            catch (Exception ex)
            {
                errMsg = "CloseConnection exception: " + ex.Message;
            }

            return false;
        }

        #endregion Open / Close Connection Methods

        #region Resume & Cancel Job Methods
        private bool CancelJob()
        {
            bool result = true;
            try
            {
                _zebraSDK.JobCancel(0);

                result = true;
            }
            catch
            {
                result = true;
            }
            return result;
        }

        private bool CancelJob(int actionID)
        {
            bool result = true;
            try
            {
                _zebraSDK.JobCancel(actionID);

                result = true;
            }
            catch
            {
                result = true;
            }
            return result;
        }

        private bool ResumeJob(out string errMsg)
        {
            errMsg = string.Empty;

            bool result = true;
            try
            {
                _zebraSDK.JobResume();

                result = true;
            }
            catch (Exception ex)
            {
                errMsg = "ResumeJob Failed: " + ex.Message;
            }
            return result;
        }

        #endregion Resume & Cancel Job Methods

        private byte[] BuildTest(ZMotifGraphics.ImageOrientationEnum ImageOrientation, ZMotifGraphics.RibbonTypeEnum RibbonType, out string errMsg)
        {
            errMsg = string.Empty;

            byte[] TheImage = null;

            ZMotifGraphics graphics = null;

            try
            {
                int dataLen = 0;

                graphics = new ZMotifGraphics();

                graphics.InitGraphics(0, 0, ImageOrientation, RibbonType);

                if (RibbonType != ZMotifGraphics.RibbonTypeEnum.Color)
                {
                    graphics.ColorProfile = string.Empty;
                }

                graphics.DrawRectangle(20, 20, DEFAULT_WIDTH - 40, DEFAULT_HEIGHT - 40, 10, System.Drawing.Color.Black.ToArgb());
                graphics.DrawTextString(25, 25, "PRINT TEST", "Arial", 12, ZMotifGraphics.FontTypeEnum.Bold, System.Drawing.Color.Black.ToArgb());
                                
                TheImage = graphics.CreateBitmap(out dataLen);

                return TheImage;
            }
            catch (Exception ex)
            {
                TheImage = null;
                errMsg = ex.StackTrace;
            }
            finally
            {
                graphics.ClearGraphics();
                graphics.CloseGraphics();
                graphics = null;
            }
            return null;
        }


        private byte[] BuildText(string text, float x, float y, string font, float fontSize, int fontColour, ZMotifGraphics.FontTypeEnum fontType, 
                                    ZMotifGraphics.ImageOrientationEnum ImageOrientation, ZMotifGraphics.RibbonTypeEnum RibbonType, out string errMsg)
        {
            errMsg = string.Empty;

            byte[] TheImage = null;

            ZMotifGraphics graphics = null;

            try
            {
                int dataLen = 0;

                graphics = new ZMotifGraphics();

                graphics.InitGraphics(0, 0, ImageOrientation, RibbonType);

                if (RibbonType != ZMotifGraphics.RibbonTypeEnum.Color)
                {
                    graphics.ColorProfile = string.Empty;
                }

                graphics.DrawTextString(x, y, text, font, fontSize, fontType, fontColour);

                TheImage = graphics.CreateBitmap(out dataLen);

                return TheImage;
            }
            catch (Exception ex)
            {
                TheImage = null;
                errMsg = ex.StackTrace;
            }
            finally
            {
                graphics.ClearGraphics();
                graphics.CloseGraphics();
                graphics = null;
            }
            return null;
        }

        private byte[] BuildImageXY(byte[] image, float x, float y, int width, int height, ZMotifGraphics.ImageOrientationEnum ImageOrientation,
                                    ZMotifGraphics.RibbonTypeEnum RibbonType, out string errMsg)
        {
            errMsg = string.Empty;

            byte[] TheImage = null;

            ZMotifGraphics graphics = null;

            try
            {
                int dataLen = 0;

                graphics = new ZMotifGraphics();

                graphics.InitGraphics(0, 0, ImageOrientation, RibbonType);

                if (RibbonType != ZMotifGraphics.RibbonTypeEnum.Color)
                {
                    graphics.ColorProfile = string.Empty;
                }

                graphics.DrawImage(ref image, x, y, width, height, (float)0.0);


                if (image != null)
                    TheImage = graphics.CreateBitmap(out dataLen);

                return TheImage;
            }
            catch (Exception ex)
            {
                TheImage = null;
                errMsg = ex.StackTrace;
            }
            finally
            {
                graphics.ClearGraphics();
                graphics.CloseGraphics();
                graphics = null;
            }
            return null;
        }

        private byte[] BuildOverlayImage(byte[] image, out string errMsg)
        {
            errMsg = string.Empty;

            byte[] TheImage = null;

            ZMotifGraphics graphics = null;

            try
            {
                int dataLen = 0;

                graphics = new ZMotifGraphics();

                ZMotifGraphics.RibbonTypeEnum RibbonType = ZMotifGraphics.RibbonTypeEnum.Overlay;
                ZMotifGraphics.ImageOrientationEnum ImageOrientation = ZMotifGraphics.ImageOrientationEnum.Landscape;

                graphics.InitGraphics(0, 0, ImageOrientation, RibbonType);

                graphics.ColorProfile = string.Empty;

                graphics.DrawImage(ref image, ZMotifGraphics.ImagePositionEnum.Centered, 1100, 700, 0);

                if (image != null)
                    TheImage = graphics.CreateBitmap(out dataLen);

                return TheImage;
            }
            catch (Exception ex)
            {
                TheImage = null;
                errMsg = ex.StackTrace;
            }
            finally
            {
                graphics.ClearGraphics();
                graphics.CloseGraphics();
                graphics = null;
            }
            return null;
        }
        #endregion

        #region Private Methods
        private PrintTypeEnum GetPrintType(ZMotifGraphics.RibbonTypeEnum ribbonType)
        {
            switch (ribbonType)
            {
                case ZMotifGraphics.RibbonTypeEnum.Color:
                    return PrintTypeEnum.Color;
                case ZMotifGraphics.RibbonTypeEnum.MonoUV:
                    return PrintTypeEnum.MonoK;
                case ZMotifGraphics.RibbonTypeEnum.MonoK:
                    return PrintTypeEnum.MonoK;
                case ZMotifGraphics.RibbonTypeEnum.Overlay:
                    return PrintTypeEnum.Overlay;
                case ZMotifGraphics.RibbonTypeEnum.GrayUV:
                    return PrintTypeEnum.MonoK;
                case ZMotifGraphics.RibbonTypeEnum.GrayDye:
                    return PrintTypeEnum.GrayDye;
                case ZMotifGraphics.RibbonTypeEnum.Monok_NoPanels:
                    return PrintTypeEnum.MonoK;
                case ZMotifGraphics.RibbonTypeEnum.Inhibit:
                    return PrintTypeEnum.MonoK;
                default:
                    return PrintTypeEnum.MonoK;
            }
        }
        #endregion

        #region Properties
        public int ComPort
        {
            get { return -1; }
        }

        public string DeviceId
        {
            get;
            private set;
        }

        public string Manufacturer
        {
            get;
            private set;
        }

        public string Name
        {
            get { return _deviceName; }            
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
        #endregion

        #region ToString
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DEVICE");
            sb.AppendLine("Serial: " + this.DeviceId);
            sb.AppendLine("Manufacturer: " + this.Manufacturer);
            sb.AppendLine("Model: " + this.Model);
            sb.AppendLine("Firmware Version: " + this.FirmwareVersion);
            sb.AppendLine("RIBBON");
            sb.AppendLine("Description: " + this._ribbonDescription);
            sb.AppendLine("Type: " + this._ribbonType);
            sb.AppendLine("OEM: " + this._ribbonOem);
            sb.AppendLine("Size: " + this._ribbonSize);
            sb.AppendLine("Prints Left: " + this._remainingPrints);

            sb.AppendLine("STATUS");
            sb.AppendLine("_status: " + this._status);
            sb.AppendLine("_errorCode: " + this._errorCode);
            sb.AppendLine("_jobsPending: " + this._jobsPending);
            sb.AppendLine("_jobsActive: " + this._jobsActive);
            sb.AppendLine("_jobsComplete: " + this._jobsComplete);
            sb.AppendLine("_jobErrors: " + this._jobErrors);
            sb.AppendLine("_jobsTotal: " + this._jobsTotal);
            sb.AppendLine("_nextActionId: " + this._nextActionId);

            return sb.ToString();
        }
        #endregion

        #region IMagEncoder
        public bool HasMagEncoder()
        {
            return false;
        }

        public IDeviceMagData ReadMagStripe()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls        

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).                
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                try
                {
                    string error;
                    CloseConnection(out error);
                }
                catch (Exception ex)
                {
                    string errMsg = ex.Message;
                }
                finally //be sure to release the interface to avoid memory leaks
                {
                    do
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    while (System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_zebraSDK) > 0);
                }
                

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ZebraZXP3() {
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

        public void SetDeviceSettings(Dictionary<string, string> settings)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
