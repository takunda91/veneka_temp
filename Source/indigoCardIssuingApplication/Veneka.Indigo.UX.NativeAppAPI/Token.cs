using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [DataContract]
    public class Token
    {
        [DataMember(IsRequired = true)]
        public string DeviceID { get; set; }

        [DataMember(IsRequired = true)]
        public string PrintJobID { get; set; }

        [DataMember(IsRequired = true)]
        public string Session { get; set; }

        [DataMember(IsRequired = true)]
        public string Workstation { get; set; }
    }
}
