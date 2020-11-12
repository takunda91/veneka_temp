using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMOTIFPRINTERLib;

namespace Veneka.Indigo.BackOffice.Application.Devices.Zebra
{
   internal class TestZebraDeviceManager : IDevicesManager
    {
        public IDevice[] FindDevices()
        {
            return GetDeviceList(ConnectionTypeEnum.All);
        }
       
        private IZebraZXP IdentifyZMotifPrinter(string deviceName)
        {
            string vendor = string.Empty;
            string model = string.Empty;
            string serialNo = string.Empty;
            string mac = string.Empty;
            string headSerialNo = string.Empty;
            string oemCode = string.Empty;
            string fwVersion = string.Empty;
            string mediaVersion = string.Empty;
            string heaterVersion = string.Empty;
            string zmotifVer = string.Empty;

            

            
                return new ZebraZXP7(deviceName, headSerialNo, oemCode, mediaVersion, heaterVersion, zmotifVer, vendor, model, serialNo, mac, fwVersion);
            
            
        }


        public IDevice[] FindDevices(DeviceConnectionTypes connectionType)
        {
            IDevice[] rtnDevices = null;
            switch (connectionType)
            {
                case DeviceConnectionTypes.USB:
                    rtnDevices = GetDeviceList(ConnectionTypeEnum.USB);
                    break;
                case DeviceConnectionTypes.Ethernet:
                    rtnDevices = GetDeviceList(ConnectionTypeEnum.Ethernet);
                    break;
                default:
                    break;
            }

            return rtnDevices;
        }
        public IZebraZXP[] GetDeviceList(ConnectionTypeEnum zebraConnType)
        {
            IZebraZXP[] rtndevices  = new IZebraZXP[2];
            rtndevices[0]= IdentifyZMotifPrinter("Device 1");
            return rtndevices;
        }
    }
}
