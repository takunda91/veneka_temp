using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Common;

namespace Veneka.Indigo.Integration.Objects
{
    public sealed class DecryptedFields
    {
        public string OperatorCode { get; set; }
        public string PinBlock { get; set; }
        public string Pin { get; set; }
        public string EncryptedPin { get; set; }
        public Track2 TrackData { get; set; }
        public string responseCode { get; set; }
    }
}
