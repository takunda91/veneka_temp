using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.Devices
{
    public interface IDevicesManager
    {
        IDevice[] FindDevices();
        IDevice[] FindDevices(DeviceConnectionTypes connectionType);
    }
}
