using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [DataContract]
    public class AppOptions
    {
        [DataMember(IsRequired = true)]
        public string Key { get; set; }

        [DataMember(IsRequired = true)]
        public string Value { get; set; }
    }
}
