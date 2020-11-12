using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.DesktopApp.Device.PINPad;
using Veneka.EFTPay;
using System.Management;
using Veneka.Indigo.UX.NativeAppAPI;
using System.IO;
using static System.Environment;
using Veneka.Data.Hexadecimal;

namespace Veneka.Indigo.DesktopApp.Device.PINPad.PAXS300.VenekaApp
{
    public sealed class VenekaPAXS300 : PaxS300
    {
        private static string NAME = "PAX S300";
        private static string MANUFACTURER = "PAX";
        private static string MODEL = "S300 (Prolin O/S)";

        private string FIRMWAREVERSION;
        private int COMPORT;
        private string DEVICEID;
        private string DLLPATH;


        private EFTPayAPI _api;
        private static bool _isDriverLoaded = false;
        

        public VenekaPAXS300(int comPort)
        {
            // Load the driver dll's
            if (!_isDriverLoaded)
            {
                DLLPATH = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Device\\PINPad\\PAXS300\\VenekaApp\\");
                Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + DllPath);
                _isDriverLoaded = true;
            }

            COMPORT = comPort;
        }

        //public static VenekaPAXS300[] ListConnectedDevices()
        //{
        //    List<VenekaPAXS300> devices = new List<VenekaPAXS300>();

        //    using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity Where Manufacturer Like 'PAX%'"))
        //    using (ManagementObjectCollection collection = searcher.Get())
        //    {
        //        foreach (var device in collection)
        //        {
        //            var comport = System.Text.RegularExpressions.Regex.Match((string)device.GetPropertyValue("Caption"), @"^.*?\([COM]*(\d+)[^\d]*\).*$").Groups[1].Value;

        //            devices.Add(new VenekaPAXS300(int.Parse(comport)));
        //        }
        //    }

        //    return devices.ToArray();
        //}

        public override void InitialisePinPad()
        {
            //string dir = Environment.GetFolderPath(SpecialFolder.LocalApplicationData);
            //FileInfo fileInfo = new FileInfo(Path.Combine("C:", "Veneka", "INA", "eft_pay_log.txt"));

            //if (!fileInfo.Directory.Exists)
            //    fileInfo.Directory.Create();

            //if (!fileInfo.Exists)
            //    fileInfo.Create();

            //_api = new EFTPayAPI(LoggingLevel.ERROR, fileInfo, 2);

            _api = new EFTPayAPI();

            if (_api.InitiateSerialPort(ComPort) != EFTPayAPI.ResponseCodes.SUCCESS)
                throw new Exception();

            var resp = _api.GetDeviceInfo(out Veneka.EFTPay.Objects.DeviceInfo deviceInfo);

            if (resp.ResponseCode == EFTPayAPI.ResponseCodes.SUCCESS)
            {
                DEVICEID = deviceInfo.SerialNo;
                FIRMWAREVERSION = deviceInfo.ProgramVersions[0];
                MANUFACTURER = deviceInfo.Manufacturer;
                MODEL = deviceInfo.DeviceModel;
            }
            else
            {
                throw new Exception(string.Format("{0}-{1}", resp.ResponseCode, resp.AdditionalInfo));
            }
        }

        public override void DisplayText(string text)
        {
            
        }

        public override void DisplayHome()
        {
            
        }

        public override PINPadResponse<string> SetTPK(HexString tpk)
        {
            // tpk.TrimStart('X', 'U', 'T', 'Y')
            var resp = _api.LoadSessionKey(tpk);
            
            return new PINPadResponse<string>(CheckIfSuccess(resp.ResponseCode), resp.ResponseCode.ToString(), resp.AdditionalInfo, String.Empty);
        }
        
        public override PINPadResponse<CardData> GetNewPIN(CardData cardData, short minPINLength, short maxPINLength, string displayMessage)
        {
            EFTPay.Objects.CardData data = new EFTPay.Objects.CardData(EFTPay.Objects.CardType.Unknown, cardData.Track2);

            var resp = _api.GetPINMasterSession(displayMessage, data, minPINLength, maxPINLength, out string encryptedPinBlock);

            cardData.PINBlock = encryptedPinBlock;
            cardData.PINBlockFormat = PINBlockFormats.ISO_0;

            if (resp.ResponseCode != EFTPayAPI.ResponseCodes.SUCCESS)
                return new PINPadResponse<CardData>(false, resp.ResponseCode.ToString(), resp.AdditionalInfo, null);


            return new PINPadResponse<CardData>(CheckIfSuccess(resp.ResponseCode), resp.ResponseCode.ToString(), resp.AdditionalInfo, cardData);
        }

        public override PINPadResponse<CardData> GetNewPINConfirmation(CardData cardData, short minPINLength, short maxPINLength, string displayMessage)
        {
            EFTPay.Objects.CardData data = new EFTPay.Objects.CardData(EFTPay.Objects.CardType.Unknown, cardData.Track2);

            var resp = _api.GetPINMasterSession(displayMessage, data, minPINLength, maxPINLength, out string encryptedPinBlock);

            CardData rtnData = new CardData()
            {
                PINBlock = encryptedPinBlock,
                PINBlockFormat = PINBlockFormats.ISO_0
            };

            if (resp.ResponseCode != EFTPayAPI.ResponseCodes.SUCCESS)
                return new PINPadResponse<CardData>(false, resp.ResponseCode.ToString(), resp.AdditionalInfo, null);


            return new PINPadResponse<CardData>(CheckIfSuccess(resp.ResponseCode), resp.ResponseCode.ToString(), resp.AdditionalInfo, rtnData);
        }

        public override PINPadResponse<CardData> PresentCard(string displayText)
        {
            var resp = _api.GetCard(displayText, out Veneka.EFTPay.Objects.CardData cardData);
            
            
            CardData trackData = null;
            if (resp.ResponseCode == EFTPayAPI.ResponseCodes.SUCCESS)
            {
                trackData = new CardData
                {
                    IsPANEncrypted = false,
                    PAN = cardData.PAN,
                    IsTrack2Encrypted = false,
                    Track2 = cardData.Track2
                };

                if(cardData.CardDataType == EFTPay.Objects.CardType.Chip)
                {
                    trackData.CardInterface = CardInterfaces.Chip_EMV;
                }
                else
                {
                    trackData.CardInterface = CardInterfaces.MagStripe;
                }
            }

            return new PINPadResponse<CardData>(CheckIfSuccess(resp.ResponseCode), resp.ResponseCode.ToString(), resp.AdditionalInfo, trackData);
        }

        public override PINPadResponse<string> RemoveCard(string displayText)
        {
            var resp = _api.RemoveChipCard();

            return new PINPadResponse<string>(CheckIfSuccess(resp.ResponseCode), resp.ResponseCode.ToString(), resp.AdditionalInfo, String.Empty);
        }     
        
        private bool CheckIfSuccess(short responseCode)
        {
            return responseCode == EFTPayAPI.ResponseCodes.SUCCESS ? true : false;
        }

        public override string Name { get { return NAME; } }

        public override string Manufacturer { get { return MANUFACTURER; } }        

        public override string Model { get { return MODEL; } }

        public override string FirmwareVersion { get { return FIRMWAREVERSION; } }

        public override int ComPort { get { return COMPORT; } }

        public override string DeviceId { get { return DEVICEID; } }

        public override string DllPath { get { return DLLPATH; }  }

        #region ToString
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DEVICE");
            sb.AppendLine("Serial: " + this.DeviceId);
            sb.AppendLine("Manufacturer: " + this.Manufacturer);
            sb.AppendLine("Model: " + this.Model);
            sb.AppendLine("Firmware Version: " + this.FirmwareVersion);

            return sb.ToString();
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        public override event DeviceNotificationEventHandler OnDeviceNotifcation;

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                try
                {
                    _api.Disconnect();
                }
                catch { }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PAXS300() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
