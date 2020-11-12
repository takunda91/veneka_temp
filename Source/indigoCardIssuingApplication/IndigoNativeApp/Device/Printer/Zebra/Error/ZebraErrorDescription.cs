using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra.Error
{
    public class ZebraErrorDescription : IDeviceErrorDescriptor
    {

        internal ZebraErrorDescription(int code, string description, string helpfulHint)
        {
            Code = code;
            Description = description ?? string.Empty;
            HelpfulHint = helpfulHint ?? string.Empty;
        }

        public int Code
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public string HelpfulHint
        {
            get;
            private set;
        }
    }
}
