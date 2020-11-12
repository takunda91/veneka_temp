using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Objects
{
    [Serializable]
    public class CMSCard
    {        
        public string PAN { get; set; }
        public string Reference { get; set; }
        public string CardStatus { get; set; }
        public string StopCause { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsBaseCard { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public Dictionary<string, string> OtherFields { get; set; }
    }
}
