using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Integration.Objects
{
    [Serializable]
    [DataContract]
    public class ZoneMasterKey
    {

        public string Zone { get; set; }
        public string Final { get; set; }
        public string DBRandomKey { get; set; }
        public string DBRandomKeyUnderLMK { get; set; }

        public ZoneMasterKey() { }

        public ZoneMasterKey(string zone, string final)            
        {
            this.Zone = zone;
            this.Final = final;
            
        }

        public ZoneMasterKey(string zone, string final, string DBRandomKey, string DBRandomKeyUnderLMK)
        {
            this.Zone = zone;
            this.Final = final;
            this.DBRandomKey = DBRandomKey;
            this.DBRandomKeyUnderLMK = DBRandomKeyUnderLMK;

        }

      
    }
}
