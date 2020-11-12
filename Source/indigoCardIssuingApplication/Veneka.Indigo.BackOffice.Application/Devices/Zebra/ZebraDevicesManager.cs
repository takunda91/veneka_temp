using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMOTIFPRINTERLib;

namespace Veneka.Indigo.BackOffice.Application.Devices.Zebra
{
    public sealed class ZebraDevicesManager : IDevicesManager
    {
        
        public IDevice[] FindDevices()
        {
            return GetDeviceList(ConnectionTypeEnum.All); 
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

        // Gets a list of ZMotif devices
        //     ConnectionTypeEnum { USB, Ethernet, All }
        // --------------------------------------------------------------------------------------------------

        public IZebraZXP[] GetDeviceList(ConnectionTypeEnum zebraConnType)
        {
            IZebraZXP[] rtndevices = null;
            Job job = new Job();
            object deviceList;

            try
            {
                job.GetPrinters(zebraConnType, out deviceList);

                if(deviceList != null)
                {
                    string[] strDeviceList = (string[])deviceList;

                    rtndevices = new IZebraZXP[strDeviceList.Length];

                    for(int i = 0; i < strDeviceList.Length; i++)
                    {                        
                        rtndevices[i] = IdentifyZMotifPrinter(strDeviceList[i], ref job);                        
                    }
                }                
            }
            finally
            {
                ZebraZXPComms.Disconnect(ref job);
            }
                        
            return rtndevices;
        }

        #region Identify ZXP Printer Type

        private IZebraZXP IdentifyZMotifPrinter(string deviceName, ref Job job)
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

            GetDeviceInfo(deviceName, ref job, out vendor, out model, out serialNo, out mac,
                          out headSerialNo, out oemCode, out fwVersion,
                          out mediaVersion, out heaterVersion, out zmotifVer);

            if (model.Contains("7"))
            {
                return new ZebraZXP7(deviceName, headSerialNo, oemCode, mediaVersion, heaterVersion, zmotifVer, vendor, model, serialNo, mac, fwVersion);
            }
            //    _isZXP7 = true;
            //else
            //    _isZXP7 = false;
            return null;
        }

        private short GetDeviceInfo(string deviceName, ref Job job, out string vender, out string model, out string serialNo, out string MAC,
                                    out string headSerialNo, out string OemCode, out string fwVersion, out string mediaVersion,
                                    out string heaterVersion, out string zmotifVersion)
        {
            vender = string.Empty;
            model = string.Empty;
            serialNo = string.Empty;
            MAC = string.Empty;
            headSerialNo = string.Empty;
            OemCode = string.Empty;
            fwVersion = string.Empty;
            mediaVersion = string.Empty;
            heaterVersion = string.Empty;
            zmotifVersion = string.Empty;

            ZebraZXPComms.Connect(deviceName, ref job);

            try
            {
                return job.Device.GetDeviceInfo(out vender, out model, out serialNo, out MAC,
                                                 out headSerialNo, out OemCode, out fwVersion,
                                                 out mediaVersion, out heaterVersion, out zmotifVersion);
            }
            finally
            {
                ZebraZXPComms.Disconnect(ref job);
            }
        }

        #endregion Identify ZXP Printer Type


    }
}
