using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Text;
using Veneka.Data.Hexadecimal;
using Veneka.Indigo.UX.NativeAppAPI;

namespace Veneka.Indigo.DesktopApp.Device.PINPad.PAXS300
{
    public abstract class PaxS300 : IPINPad
    {
        public abstract string DllPath { get; }
        public abstract string Name { get; }
        public abstract string Manufacturer { get; }
        public abstract string Model { get; }
        public abstract string FirmwareVersion { get; }
        public abstract int ComPort { get; }
        public abstract string DeviceId { get; }

        private enum DeviceDisplays
        {
            All = 0,
            Veneka = 1,
            GPT = 2
        }

        public abstract event DeviceNotificationEventHandler OnDeviceNotifcation;

        private static DeviceDisplays DeviceDisplay()
        {
            DeviceDisplays rtn = DeviceDisplays.Veneka;

            if (ConfigurationManager.AppSettings.HasKeys())
            {
                var strOption = ConfigurationManager.AppSettings.Get("PaxS300");

                if (String.IsNullOrWhiteSpace(strOption))
                    return rtn;

                if (!Enum.TryParse(strOption, out rtn))
                    throw new Exception("'" + strOption + "' not a valid oprion. Must be All/Veneka/GPT");
            }

            return rtn;
        }
            

        public static PaxS300[] ListConnectedDevices()
        {
            // Check config for which devices to list
            var display = DeviceDisplay();

            List<PaxS300> devices = new List<PaxS300>();

            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity Where Manufacturer Like 'PAX%'"))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                foreach (var device in collection)
                {
                    var comport = System.Text.RegularExpressions.Regex.Match((string)device.GetPropertyValue("Caption"), @"^.*?\([COM]*(\d+)[^\d]*\).*$").Groups[1].Value;

                    switch (display)
                    {
                        case DeviceDisplays.All:
                            devices.Add(new VenekaApp.VenekaPAXS300(int.Parse(comport)));
                            devices.Add(new GPT.GptPaxS300PINPad(int.Parse(comport)));
                            break;
                        case DeviceDisplays.Veneka:
                            devices.Add(new VenekaApp.VenekaPAXS300(int.Parse(comport)));
                            break;
                        case DeviceDisplays.GPT:
                            devices.Add(new GPT.GptPaxS300PINPad(int.Parse(comport)));
                            break;
                        default:
                            break;
                    }
                    //devices.Add(new VenekaPAXS300(int.Parse(comport)));
                }
            }

            return devices.ToArray();
        }

        public abstract void DisplayHome();
        public abstract void DisplayText(string text);
        public abstract void Dispose();
        public abstract PINPadResponse<CardData> GetNewPIN(CardData cardData, short minPINLength, short maxPINLength, string displayMessage);
        public abstract PINPadResponse<CardData> GetNewPINConfirmation(CardData cardData, short minPINLength, short maxPINLength, string displayMessage);
        public abstract void InitialisePinPad();
        public abstract PINPadResponse<CardData> PresentCard(string displayText);
        public abstract PINPadResponse<string> RemoveCard(string displayText);
        public abstract PINPadResponse<string> SetTPK(HexString tpk);

        public void SetDeviceSettings(Dictionary<string, string> settings)
        {
            throw new NotImplementedException();
        }
    }
}
