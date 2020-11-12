using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer
{
    public interface IMagEncoder
    {
        /// <summary>
        /// Checks that the device is capable of magnetic encoding
        /// </summary>
        /// <returns></returns>
        bool HasMagEncoder();

        /// <summary>
        /// Reads magnetic stripe of the card and returns track data
        /// </summary>
        /// <returns></returns>
        IDeviceMagData ReadMagStripe();
    }
}
