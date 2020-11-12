using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndigoDesktopApp.Device.Printer
{
    public class PrinterCodes
    {
        public const short Success = 0;
        public const short UnknownError = 1;
        public const short DeviceNotFound = 2;
        public const short Retry = 10;
        public const short ConnectFailed = 100;
        public const short ProductBinAndCardMismatch = 200;
        public const short PrintJobTimedOut = 500;
        public const short PrintJobCancelled = 501;
        public const short PrintJobError = 502;
    }
}
