using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZMOTIFPRINTERLib;
using ZMTGraphics;

namespace Veneka.Indigo.BackOffice.Application.Devices.Zebra
{
    public abstract class ZebraZXP : IZebraZXP
    {
        private object _cardTypeList = null,
                        _deviceList = null;

        private bool _isContact = false,
                       _isContactless = false,
                       _isMag = false,
                        _isConnected = false;
        private short _alarm = 0;

        private Job _zebraSDK;
        private int _ribbonType;
        private string _ribbonDescription;
        private string _ribbonpaternumber;
        private string _ribbonOem;
        private int _remainingPrints;
        private int _ribbonSize;
        private int _panelRemaining;
        private static readonly ILog log = LogManager.GetLogger(typeof(ZebraZXP));

        public ZebraZXP(string deviceName, string headSerialNo, string oemCode, string mediaVersion, string heaterVersion, string zmotifVersion,
            string vendor, string model, string serialNo, string mac, string firmwareVersions)
        {
            DeviceName = deviceName;

            if (deviceName.Contains(","))
            {
                ConnectionType = DeviceConnectionTypes.Ethernet;
            }
            else
            {
                ConnectionType = DeviceConnectionTypes.USB;
            }

            HeadSerialNo = headSerialNo;
            OemCode = oemCode;
            MediaVersion = mediaVersion;
            HeaterVersion = heaterVersion;
            ZMotifVersion = zmotifVersion;
            Vendor = vendor;
            Model = model;
            SerialNo = serialNo;
            MAC = mac;
            FirmwareVersion = firmwareVersions;
        }

        public string DeviceName { get; private set; }
        public DeviceConnectionTypes ConnectionType { get; private set; }
        public string HeadSerialNo { get; private set; }
        public string OemCode { get; private set; }
        public string MediaVersion { get; private set; }
        public string HeaterVersion { get; private set; }
        public string ZMotifVersion { get; private set; }
        public string Vendor { get; private set; }
        public string Model { get; private set; }
        public string SerialNo { get; private set; }
        public string MAC { get; private set; }
        public string FirmwareVersion { get; private set; }


        public bool Connect()
        {
            if (_zebraSDK == null)
            {
                _zebraSDK = new Job();
            }


            if (_zebraSDK != null && !_zebraSDK.IsOpen)
            {
                _isConnected = ZebraZXPComms.Connect(DeviceName, ref _zebraSDK);
                return _isConnected;
            }
            return false;
        }

        public bool Disconnect()
        {
            if (_zebraSDK != null && _zebraSDK.IsOpen)
            {
                if (ZebraZXPComms.Disconnect(ref _zebraSDK))
                {
                    _isConnected = false;
                    return true;
                }
            }
            return false;
        }

        public bool Eject(out string errMsg)
        {
            errMsg = string.Empty;
            bool result = false;
            try
            {
                _zebraSDK.EjectCard();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = "EjectCard excpetion: " + ex.Message;
            }
            return result;
        }
        // Gets the printer configuration
        // --------------------------------------------------------------------------------------------------

        private bool GetPrinterConfiguration(ref Job j)
        {
            bool bRet = true;

            _isContact = _isContactless = _isMag = false;

            try
            {
                string headType, stripeLocation;
                j.Device.GetMagneticEncoderConfiguration(out headType, out stripeLocation);
                if (headType != "none" && headType != "")
                    _isMag = true;

                string commChannel, contact, contactless;
                j.Device.GetSmartCardConfiguration(out commChannel, out contact, out contactless);
                j.Device.GetRibbonParams(out _ribbonType, out _ribbonpaternumber, out _ribbonDescription, out _ribbonOem, out _ribbonSize, out _panelRemaining);
                log.Debug("_ribbonType" + _ribbonType);
                if (contact != "" && contact != "none")
                    _isContact = true;

                if (contactless != "" && contactless != "none")
                    _isContactless = true;
            }
            catch
            {
                bRet = false;
            }
            return bRet;
        }

        #region Versions

        #region Mag Stripe Methods
        // Reads Magnetic Tracks
        // --------------------------------------------------------------------------------------------------

        public bool MagRead(out string _track1Data, out string _track2Data, out string _track3Data, out string _msg)
        {
            bool bRet = true;
            _msg = string.Empty;
            _track1Data = string.Empty;
            _track2Data = string.Empty;
            _track3Data = string.Empty;


            //// Opens a connection with a ZXP Printer
            ////     if it is in an alarm condition, exit function
            //// -------------------------------------------------

            if (!_isConnected)
            {
                _msg = "printer is not connected.";
                return false;
            }

            if (_alarm != 0)
            {
                _msg = "Device is in alarm condition";
                return false;
            }

            // Determines if the ZXP device supports magnetic encoding
            // -------------------------------------------------------

            if (!GetPrinterConfiguration(ref _zebraSDK))
            {
                return false;
            }

            if (!_isMag)
            {
                _msg = "Printer is not configured for Magnetic Encoding";
                return false;
            }

            //if (!_isZXP7)
            //    _zebraSDK.JobControl.CardType = GetMagneticCardType(ref job);

            // Sets the source and destination positions
            // -----------------------------------------

            _zebraSDK.JobControl.FeederSource = FeederSourceEnum.CardFeeder;
            _zebraSDK.JobControl.Destination = DestinationTypeEnum.Hold;

            // Runs a magnetic read job - no need to wait for job to complete. Job will be completed when function returns
            // ------------------------
            int actionID = 0;
            DataSourceEnum tracksToEncode = DataSourceEnum.NoData;

            tracksToEncode = DataSourceEnum.Track1Data | DataSourceEnum.Track2Data | DataSourceEnum.Track3Data;
            _zebraSDK.ReadMagData(tracksToEncode, out _track1Data, out _track2Data, out _track3Data, out actionID);

            return bRet;
        }

        private string GetMagneticCardType(ref object job)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Printing
        // Print_1
        //     images, shapes, text, print
        // --------------------------------------------------------------------------------------------------
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
        public short AddImage(byte[] image, float x, float y, int width, int height, CardSide side, Orientation orientation)
        {
            string errMsg = string.Empty;

            ZMotifGraphics.ImageOrientationEnum zebraOrientation;
            zebraOrientation = (ZMotifGraphics.ImageOrientationEnum)((int)orientation);

            SideEnum zebraSide;
            zebraSide = (SideEnum)((int)side);

            var zebraImage = BuildImageXY(image, x, y, width, height, zebraOrientation, (ZMotifGraphics.RibbonTypeEnum)_ribbonType, out errMsg);

            if (zebraImage != null)
                _zebraSDK.BuildGraphicsLayers(zebraSide, GetPrintType((ZMotifGraphics.RibbonTypeEnum)_ribbonType), (int)x, (int)y, 0, -1, GraphicTypeEnum.BMP, zebraImage);

            return PrinterCodes.Success;
        }
        public short AddText(string text, float x, float y, CardSide side, string font, float fontSize, int fontColourARGB, FontType fontType, Orientation orientation)
        {
            string errMsg;

            ZMotifGraphics.FontTypeEnum zebraFontType;
            zebraFontType = (ZMotifGraphics.FontTypeEnum)((int)fontType);

            ZMotifGraphics.ImageOrientationEnum zebraOrientation;
            zebraOrientation = (ZMotifGraphics.ImageOrientationEnum)((int)orientation);

            SideEnum zebraSide;
            zebraSide = (SideEnum)((int)side);

            //System.Drawing.Color.Black.ToArgb()
            log.Debug((ZMotifGraphics.RibbonTypeEnum)_ribbonType);
            byte[] zebraImage = BuildText(text, x, y, font, fontSize, fontColourARGB, zebraFontType, zebraOrientation, (ZMotifGraphics.RibbonTypeEnum)_ribbonType, out errMsg);

            if (zebraImage != null)
                _zebraSDK.BuildGraphicsLayers(zebraSide, GetPrintType((ZMotifGraphics.RibbonTypeEnum)ZMotifGraphics.RibbonTypeEnum.MonoK), (int)x, (int)y, 0, -1, GraphicTypeEnum.BMP, zebraImage);

            return PrinterCodes.Success;
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

                graphics.InitGraphics(0, 0, ImageOrientation, ZMotifGraphics.RibbonTypeEnum.MonoK);

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

        public bool Print(PrintField[] printfields, out string errMsg)
        {
            errMsg = string.Empty;
            bool bRet = true;
            int copies = 1;
            try
            {

                // Opens a connection with a ZXP Printer
                //     if it is in an alarm condition, exit function
                // -------------------------------------------------

                if (!_isConnected)
                {
                    errMsg = "Unable to open device [" + this.DeviceName + "]\r\n";
                    return false;
                }

                if (_alarm != 0)
                {
                    errMsg = "Printer is in alarm condition\r\n" + "Error: " + _zebraSDK.Device.GetStatusMessageString(_alarm);
                    return false;
                }
                _zebraSDK.ClearGraphicsLayers();
                foreach (var field in printfields)
                {
                    if (field.PrintFieldTypeId == (int)PrintFieldTypeId.Text)
                    {

                        errMsg = "Adding Text to " + field.ValueToString();
                        AddText(field.ValueToString(), field.X, field.Y,
                            field.PrintSide == 0 ? CardSide.Front : CardSide.Back,
                            field.Font, field.FontSize, field.FontColourRGB, FontType.Bold, Orientation.Landscape);
                    }
                    else if (field.PrintFieldTypeId == (int)PrintFieldTypeId.Image)
                    {
                        AddImage(field.Value, field.X, field.Y, Convert.ToInt32(field.Width), Convert.ToInt32(field.Height),
                            field.PrintSide == 0 ? CardSide.Front : CardSide.Back,
                            Orientation.Landscape);
                    }
                    else
                    {

                        throw new ArgumentException("Unknown printer field in array.", "fields");
                    }
                }
                errMsg += "before start";
                _zebraSDK.JobControl.FeederSource = FeederSourceEnum.AutoDetect;
                _zebraSDK.JobControl.Destination = DestinationTypeEnum.Eject;


                int actionID = 0;
                errMsg += "set graphics";
                _zebraSDK.PrintGraphicsLayers(copies, out actionID);
                errMsg += "after print graphics";


                string status = string.Empty;
                JobWait(ref _zebraSDK, actionID, 180, out status);

            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
                bRet = false;
                // System.Windows.Forms.MessageBox.Show(ex.ToString(), "DualSidePrint");
            }
            finally
            {

                _zebraSDK.ClearGraphicsLayers();

                //Disconnect(ref job);
            }
            return bRet;
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

                graphics.InitGraphics(0, 0, ImageOrientation, ZMotifGraphics.RibbonTypeEnum.MonoK);

                if (RibbonType != ZMotifGraphics.RibbonTypeEnum.Color)
                {
                    graphics.ColorProfile = string.Empty;
                }

                //graphics.DrawTextString(x, y, text, font, fontSize, fontType, fontColour);
                graphics.DrawTextString(x, y, text, font, fontSize, fontType, graphics.IntegerFromColor(System.Drawing.Color.Black));

                TheImage = graphics.CreateBitmap(out dataLen);

                return TheImage;
            }
            catch (Exception ex)
            {
                TheImage = null;
                errMsg = ex.StackTrace;
                log.Debug(errMsg);
            }
            finally
            {
                graphics.ClearGraphics();
                graphics.CloseGraphics();
                graphics = null;
            }
            return null;
        }
        // Waits for a job to complete
        // --------------------------------------------------------------------------------------------------

        public void JobWait(ref Job job, int actionID, int loops, out string status)
        {
            status = string.Empty;

            JobStatusStruct js = new JobStatusStruct();

            while (loops > 0)
            {
                try
                {
                    _alarm = job.GetJobStatus(actionID, out js.uuidJob, out js.printingStatus,
                                out js.cardPosition, out js.errorCode, out js.copiesCompleted,
                                out js.copiesRequested, out js.magStatus, out js.contactStatus,
                                out js.contactlessStatus);

                    if (js.printingStatus == "done_ok" || js.printingStatus == "cleaning_up")
                    {
                        status = js.printingStatus + ": " + "Indicates a job completed successfully";
                        break;
                    }
                    else if (js.printingStatus.Contains("cancelled"))
                    {
                        status = js.printingStatus;
                        break;
                    }

                    if (js.contactStatus.ToLower().Contains("error"))
                    {
                        status = js.contactStatus;
                        break;
                    }

                    if (js.printingStatus.ToLower().Contains("error"))
                    {
                        status = "Printing Status Error";
                        break;
                    }

                    if (js.contactlessStatus.ToLower().Contains("error"))
                    {
                        status = js.contactlessStatus;
                        break;
                    }

                    if (js.magStatus.ToLower().Contains("error"))
                    {
                        status = js.magStatus;
                        break;
                    }

                    if (_alarm != 0 && _alarm != 4016) //no error or out of cards
                    {
                        status = "Error: " + job.Device.GetStatusMessageString(_alarm);
                        break;
                    }
                }
                catch (Exception e)
                {
                    status = "Job Wait Exception: " + e.Message;
                    break;
                }

                if (_alarm == 0)
                {
                    if (--loops <= 0)
                    {
                        status = "Job Status Timeout";
                        break;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private struct JobStatusStruct
        {
            public int copiesCompleted,
                          copiesRequested,
                          errorCode;
            public string cardPosition,
                          contactlessStatus,
                          contactStatus,
                          magStatus,
                          printingStatus,
                          uuidJob;
        }
        #endregion

        //public string GetVersions()
        //{
        //    string versions = "";

        //    Job j = new Job();
        //    ZMotifGraphics g = new ZMotifGraphics();

        //    try
        //    {
        //        byte major, minor, build, revision;

        //        if (!ZebraZXPComms.Connect("", ref j))
        //        {                    
        //            return "Unable to open device";
        //        }

        //        if ((_alarm != 0) && (_alarm != 4016))
        //        {
        //            ZebraZXPComms.Disconnect(ref j);
        //            return "Printer is in alarm condition";
        //        }

        //        g.GetSDKVersion(out major, out minor, out build, out revision);
        //        versions = "Graphic SDK = " + major.ToString() + "." +
        //            minor.ToString() + "." +
        //            build.ToString() + "." +
        //            revision.ToString() + ";  ";

        //        j.GetSDKVersion(out major, out minor, out build, out revision);
        //        versions += "Printer SDK = " + major.ToString() + "." +
        //            minor.ToString() + "." +
        //            build.ToString() + "." +
        //            revision.ToString() + ";  ";


        //        string fwVersion, junk;
        //        j.Device.GetDeviceInfo(out junk, out junk, out junk, out junk, out junk, out junk,
        //            out fwVersion, out junk, out junk, out junk);

        //        versions += "Firmware = " + fwVersion;
        //    }
        //    catch (Exception e)
        //    {
        //        versions = "Exception: " + e.Message;
        //    }
        //    finally
        //    {
        //        g = null;
        //        ZebraZXPComms.Disconnect(ref j);
        //    }

        //    return versions;
        //}

        #endregion


    }


}
