using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Objects
{
   public class ThreeDSecureCardDetails
    {
        public string CardNumber { get; set; }
        public string NameOnCard { get; set; }
        public DateTime?  CardExpiryDate { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustoemrMiddleName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerTitle { get; set; }
        public string ContactNumber { get; set; }
        public string ContactEmail { get; set; }
        public string CustomerAccountNumber { get; set; }

    }
}
