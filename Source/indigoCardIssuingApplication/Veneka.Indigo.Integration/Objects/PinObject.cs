using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.Integration.Objects
{
    /// <summary>
    /// This class is used as a standard object to pass between Indigo, the Account Validation Service and Core Banking and CMS.
    /// </summary>
    [Serializable]
    //public enum PinRequestType { New, ReIssue }
    public class PinObject
    {

        public int IssuerId { get; set; }
        public int BranchId { get; set; }
        public int DomBranchId { get; set; }
        public string PinRequestReference { get; set; }
        public string PinRequestStatus { get; set; }
        public string PinRequestType { get; set; }
        public int ProductId { get; set; }
        public string ProductBin { get; set; }
        public string LastFourDigitsOfPan { get; set; }
        public int ExpiryPeriod { get; set; }
        public string Channel { get; set; }
        public string CustomerAccountNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerContact { get; set; }
        public int PinRequestId { get; set; }
        public string request_comment { get; set; }
        public string reissue_approval_stage { get; set; }

    }
}
