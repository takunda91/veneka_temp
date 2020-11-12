using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.CardManagement.objects
{
    public class FlexCubeAccountInfo
    {

        public bool Authorised { get; set; }

        public string RecordStatus { get; set; }

        public string Branch { get; set; }

        public string AccountNumber { get; set; }

        public string AccountHolderName { get; set; }

        public string AccountType { get; set; }

        public bool Dormant { get; set; }

        public bool Blocked { get; set; }

        public bool Frozen { get; set; }

        public bool CreditAllowed { get; set; }

        public bool DebitAllowed { get; set; }

        public string Affiliate { get; set; }

        public string CustomerID { get; set; }

        public bool ATMFlag { get; set; }

    }
}
