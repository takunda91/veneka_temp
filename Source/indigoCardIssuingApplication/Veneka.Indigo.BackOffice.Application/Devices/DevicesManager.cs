using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.Devices
{
    public sealed class DevicesManager : IDevicesManager
    {
        private IDevicesManager[] _deviceManagers = new IDevicesManager[]
        {
            new Zebra.ZebraDevicesManager()
           //new  TestDeviceManager()
        };

        public  IDevice[] FindDevices()
        {
            List<IDevice> devices = new List<IDevice>();
            foreach(var deviceMan in _deviceManagers)
            {
                devices.AddRange(deviceMan.FindDevices());
            }

            return devices.ToArray();
        }

        public IDevice[] FindDevices(DeviceConnectionTypes connectionType)
        {
            List<IDevice> devices = new List<IDevice>();
            foreach (var deviceMan in _deviceManagers)
            {
                devices.AddRange(deviceMan.FindDevices(connectionType) ?? new IDevice[0]);
            }

            return devices.ToArray();
        }
    }
}
