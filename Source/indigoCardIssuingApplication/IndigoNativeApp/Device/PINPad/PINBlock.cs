using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.DesktopApp.Device.PINPad
{
    public class PINBlock
    {
        public PINBlock(string pinBlock, string pinBlockFormat, bool isEncrypted)
        {
            PINBlockValue = pinBlock;
            PINBlockFormat = pinBlockFormat;
            IsEncrypted = isEncrypted;
        }

        public string PINBlockFormat { get; private set; }
        public bool IsEncrypted { get; private set; }
        public string PINBlockValue { get; private set; }
    }
}
