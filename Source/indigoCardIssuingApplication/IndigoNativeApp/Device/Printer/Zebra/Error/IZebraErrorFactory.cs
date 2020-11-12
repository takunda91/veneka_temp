using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra.Error
{
    public interface IZebraErrorFactory
    {
        IDeviceErrorDescriptor GetErrorDescription(int errorCode);
    }
}
