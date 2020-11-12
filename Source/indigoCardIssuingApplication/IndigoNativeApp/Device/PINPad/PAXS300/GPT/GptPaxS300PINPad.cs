using GPT;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Text;
using Veneka.Data.Hexadecimal;
using Veneka.Indigo.UX.NativeAppAPI;

namespace Veneka.Indigo.DesktopApp.Device.PINPad.PAXS300.GPT
{
    public sealed class GptPaxS300PINPad : PaxS300
    {
        public static string NAME = "PAX S300 (GPT)";
        public static string MANUFACTURER = "PAX";
        public static string MODEL = "S300 (GPT O/S)";

        private string FIRMWAREVERSION;
        private int COMPORT;
        private string DEVICEID;
        private string DLLPATH;

        private static readonly int SuccessCode = 0;

        private readonly PINPadAPI pinPadAPI = new PINPadAPI();

        private bool isDriverLoaded = false;
        private bool hasSessionKey = false;
        private string _tpk = String.Empty;

        public GptPaxS300PINPad(int comPort)
        {
            COMPORT = comPort;
        }


        //public static GptPaxS300PINPad[] ListConnectedDevices()
        //{
        //    // Check Config
            

        //    List<GptPaxS300PINPad> devices = new List<GptPaxS300PINPad>();

        //    using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity Where Manufacturer Like 'PAX%'"))
        //    using (ManagementObjectCollection collection = searcher.Get())
        //    {
        //        foreach (var device in collection)
        //        {
        //            var comport = System.Text.RegularExpressions.Regex.Match((string)device.GetPropertyValue("Caption"), @"^.*?\([COM]*(\d+)[^\d]*\).*$").Groups[1].Value;

        //            devices.Add(new GptPaxS300PINPad(int.Parse(comport)));
        //        }
        //    }

        //    return devices.ToArray();
        //}


        public override void InitialisePinPad()
        {
            // Load the driver dll's
            if (!isDriverLoaded)
            {
                var dllDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Device\\PINPad\\PAXS300\\GPT\\");
                Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDirectory);
                isDriverLoaded = true;
            }

            var respCode = pinPadAPI.Init("COM" + ComPort.ToString(), "termId");

            if (respCode == SuccessCode)
            {
                FetchTerminalId();
                string build, buildDate;

                pinPadAPI.GetAPIVersionNumber(out build, out buildDate);                

                FIRMWAREVERSION = build + " (" + buildDate + ")";

                SetHomeScreen();
                disposedValue = false;
            }
            else
            {
                // Could not connect   
                throw new Exception("Could not connect, responce code " + respCode);
            }
        }

        private void SetHomeScreen()
        {
            pinPadAPI.StartScreen();
            pinPadAPI.Display(0, 0, "Terminal Ready");
            //pinPadAPI.Display(6, 0, "          Powered By ", 6);
            //pinPadAPI.Display(7, 0, "              Veneka ", 6);

            //6,2
        }

        private int FetchTerminalId()
        {
            string terminalId = String.Empty;

            // Connected get device ID
            int respCode = pinPadAPI.GetSerialNumber(out terminalId);

            if (respCode == 0)
                DEVICEID = terminalId;
            else
                DEVICEID = String.Empty;

            return respCode;
        }

        private int SetSessionKey(string key)
        {
            hasSessionKey = false;
            string checkValue = String.Empty;

            int respCode = pinPadAPI.SetSessionKey(key, out checkValue);

            if (respCode == 0)
            {
                _tpk = key;
                hasSessionKey = true;
            }

            return respCode;
        }

        public override PINPadResponse<string> SetTPK(HexString tpk)
        {
            int resp = this.SetSessionKey(tpk);

            return new PINPadResponse<string>(resp == SuccessCode, "", resp.ToString(), "");
        }

        public override PINPadResponse<CardData> PresentCard(string displayText)
        {
            int timeout = 30;
            int error = 0;
            int where;
            bool done = false;
            string resultData = String.Empty;
            CardData cardData = null;

            for (where = 0; error == SuccessCode && !done; where++)
            {
                switch (where)
                {
                    default:
                        error = 1;
                        break;
                    case 0:
                        error = pinPadAPI.WaitForCard(displayText, timeout, "3", out resultData);
                        break;
                    case 1:
                        if (resultData.Equals("1")) // Mag reader used
                        {
                            string[] trackData = new string[3];
                            error = pinPadAPI.GetTrackData(2, trackData, _tpk);

                            if (error == SuccessCode)
                            {
                                cardData = new CardData()
                                {
                                    CardInterface = CardInterfaces.MagStripe,
                                    IsTrack2Encrypted = true,
                                    Track2 = trackData[0]
                                };
                            }
                        }
                        else  // Chip reader used
                        {
                            // For Visa
                            error = pinPadAPI.AddApplication("VISA CREDIT", "A0000000031010", "0", "0", "0", "1", "1", "1", "10000", "0", "0010000000",
                                    "D84004F800", "D84000A800", "00000123456", "039F3704", "0F9F02065F2A029A039C0195059F3704", "");

                            // For MasterCard
                            error = pinPadAPI.AddApplication("MCHIP", "A0000000041010", "0", "0", "0", "1", "1", "1", "10000", "0", "0400000000",
                                    "F850ACF800", "FC50ACA000", "00000123456", "039F3704", "0F9F02065F2A029A039C0195059F3704", "");

                            if (error == SuccessCode)
                            {
                                error = pinPadAPI.SelectApplication(1023);   // 1023 = Transaction Sequence Number

                                if (error == SuccessCode)
                                {
                                    error = pinPadAPI.ReadApplicationData();

                                    if (error == SuccessCode)
                                    {
                                        string trackData;
                                        error = pinPadAPI.GetTLVData("57", _tpk, out trackData); // Track 2 Equivalent Data
                                        //Error = PINPadTests_PINPadAPI.GetTLVData("5A", "08D7B4FB629D088508D7B4FB629D0885", ResultData); // PAN

                                        //cardData = new CardTrackData(trackData, CardTrackDataType.Track2, true);
                                        if (error == SuccessCode)
                                        {
                                            cardData = new CardData()
                                            {
                                                CardInterface = CardInterfaces.Chip_EMV,
                                                IsTrack2Encrypted = true,
                                                Track2 = trackData
                                            };
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    case 2:
                        done = true;
                        break;
                }
            }

            //ValidateErrorCode(Error);

            return new PINPadResponse<CardData>(error == SuccessCode, error.ToString(), error.ToString(), cardData);
        }

        public override PINPadResponse<CardData> GetNewPIN(CardData cardData, short minPINLength, short maxPINLength, string displayMessage)
        {
            string pinBlock = String.Empty;

            int error = pinPadAPI.GetPIN(out pinBlock, displayMessage, minPINLength, maxPINLength, 100);

            if (error == SuccessCode)
            {
                CardData rtnData = new CardData()
                {
                    PINBlock = pinBlock,
                    PINBlockFormat = PINBlockFormats.ISO_0
                };

                return new PINPadResponse<CardData>(true, error.ToString(), "", rtnData);
            }

            return new PINPadResponse<CardData>(false, error.ToString(), "", null);
        }

        public override PINPadResponse<CardData> GetNewPINConfirmation(CardData cardDataConfirm, short minPINLength, short maxPINLength, string displayMessage)
        {
            string pinBlock = String.Empty;

            int error = pinPadAPI.GetPIN(out pinBlock, displayMessage, minPINLength, maxPINLength, 100);

            if (error == SuccessCode)
            {
                CardData rtnData = new CardData()
                {
                    PINBlock = pinBlock,
                    PINBlockFormat = PINBlockFormats.ISO_0
                };

                return new PINPadResponse<CardData>(true, error.ToString(), "", rtnData);
            }

            return new PINPadResponse<CardData>(false, error.ToString(), "", null);
        }

        public override PINPadResponse<string> RemoveCard(string displayText)
        {
            int error = 0;
            int count = 0;

            do
            {
                pinPadAPI.BeepTerminal(0);
                error = pinPadAPI.RemoveCard(displayText, 10);
                count++;
            } while (error == 5 && count < 3);

            return new PINPadResponse<string>(true, "", "", "");
        }

        public override void DisplayText(string text)
        {
            pinPadAPI.StartScreen();
            pinPadAPI.Display(0, 0, text);
        }

        public override void DisplayHome()
        {
            SetHomeScreen();
        }

        public override string Name { get { return NAME; } }

        public override string Manufacturer { get { return MANUFACTURER; } }

        public override string Model { get { return MODEL; } }

        public override string FirmwareVersion { get { return FIRMWAREVERSION; } }

        public override int ComPort { get { return COMPORT; } }

        public override string DeviceId { get { return DEVICEID; } }

        public override string DllPath { get { return DLLPATH; } }

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
                    if (pinPadAPI != null)
                    {
                        pinPadAPI.StartScreen();
                        pinPadAPI.Term();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~GptPaxS300PINPad() {
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
