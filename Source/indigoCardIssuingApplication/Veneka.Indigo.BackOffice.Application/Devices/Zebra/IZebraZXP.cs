using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.Devices.Zebra
{
    public class PrinterCodes
    {
        public const short Success = 0;
        public const short UnknownError = 1;
        public const short DeviceNotFound = 2;
        public const short PrintJobTimedOut = 500;
    }
    public interface IZebraZXP : IDevice
    {
        string HeadSerialNo { get; }
        string OemCode { get; }
        string MediaVersion { get; }
        string HeaterVersion { get; }
        string ZMotifVersion { get; }
    }
}
