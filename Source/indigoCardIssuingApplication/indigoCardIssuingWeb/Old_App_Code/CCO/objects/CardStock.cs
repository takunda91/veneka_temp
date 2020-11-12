using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.Old_App_Code.CCO.objects
{    
    [Serializable]
    public class CardStock
    {
        public string Branch { get; set; }
        public string Issuer { get; set; }
        public string Product { get; set; }
        public string NumberOfCards { get; set; }
    }
}