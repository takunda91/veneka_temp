using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [DataContract]
    public class ProductSettings
    {
        [DataMember(IsRequired = true)]
        public int ProductId { get; set; }

        [DataMember(IsRequired = true)]
        public int MinPINLength { get; set; }

        [DataMember(IsRequired = true)]
        public int MaxPINLength { get; set; }
    }
}
