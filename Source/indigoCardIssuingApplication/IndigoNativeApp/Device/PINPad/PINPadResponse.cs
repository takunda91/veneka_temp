using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.DesktopApp.Device.PINPad
{
    public sealed class PINPadResponse<T>
    {
        public PINPadResponse(bool success, string responseCode, string additionalInfo, T value)
        {
            Success = success;
            ResponseCode = responseCode;
            Value = value;
        }

        public bool Success { get; private set; }
        public string ResponseCode { get; private set; }
        public string AdditionalInfo { get; private set; }
        public T Value { get; private set; }

        public override string ToString()
        {
            return String.Format("{0} - {1}", ResponseCode, AdditionalInfo);
        }
    }
}
