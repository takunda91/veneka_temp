using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [DataContract]
    public class PrinterInfo
    {
        [DataMember]
        public int PrinterId { get; set; }
        [DataMember]
        public string SerialNo { get; set; }
        [DataMember]
        public string Manufacturer { get; set; }
        [DataMember]
        public string Model { get; set; }
        [DataMember]
        public string FirmwareVersion { get; set; }
        [DataMember]
        public int BranchId { get; set; }
        [DataMember]
        public int TotalPrints { get; set; }
        [DataMember]
        public int NextClean { get; set; }
    }
}
