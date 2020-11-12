using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.Devices.Zebra
{
    public class ZebraZXP7 : ZebraZXP
    {
        public ZebraZXP7(string deviceName, string headSerialNo, string oemCode, string mediaVersion, string heaterVersion, string zmotifVersion, 
            string vendor, string model, string serialNo, string mac, string firmwareVersions) : base(deviceName, headSerialNo, oemCode, mediaVersion, 
                heaterVersion, zmotifVersion, vendor, model, serialNo, mac, firmwareVersions)
        {
        }
    }
}
