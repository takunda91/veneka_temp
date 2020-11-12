using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.CardManagement.objects
{
    [DataContract]
    public class RequestData
    {
        [DataMember]
        public long request_id { get; set; }
        [DataMember]
        public int request_statues_id { get; set; }
        [DataMember]
        public string card_number { get; set; }
    }
}
