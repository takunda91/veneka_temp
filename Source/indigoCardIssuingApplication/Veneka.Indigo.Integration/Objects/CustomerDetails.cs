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
    public class CustomerDetails
    {
        private readonly string _customerIDNumber;
        private readonly string _customerTitle;
        private readonly string _pinOffset;
        //private string _cardNumber;
        public int IssuerId { get; set; }
        public string IssuerCode { get; set; }
        public int BranchId { get; set; }
        public string PrintJobId { get; set; }

        public int DeliveryBranchId { get; set; }
        public string BranchCode { get; set; }
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string CountryCapital { get; set; }
        public int? PriorityId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        private string _nameoncard;
        public string NameOnCard { get { return _nameoncard; } set { _nameoncard = value.ToUpper(); } }
        public int? CustomerTypeId { get; set; }
        public int? CustomerResidencyId { get; set; }
        public string AccountNumber { get; set; }
        public int DomicileBranchId { get; set; }

        public int? AccountTypeId { get; set; }

        public string CMSAccountType { get; set; }

        public string CBSAccountType { get; set; }
        public decimal AccountBalance { get; set; }
        public int CustomerTitleId { get; set; }
        public string ReasonForIssue { get; set; }
        public int? CardIssueReasonId { get; set; }
        public int CardIssueMethodId { get; set; }

        public int? CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyNumericCode { get; set; }
        public bool? IsBaseCurrency { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public Dictionary<string, string> CurrencyFields { get; set; }

        public string CustomerIDNumber { get; set; }
        public string ContactNumber { get; set; }
        public string ContractNumber { get; set; }
        public string CardNumber { get; set; }
        public string CardReference { get; set; }
        public string CustomerId { get; set; }
        public long CardId { get; set; }
        public int BranchCardStatusesId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductBinCode { get; set; }
        public string SubProductCode { get; set; }
        public string CmsID { get; set; }
        public bool ExistsInCMS { get; set; }
        public string PinOffset { get; set; }

        public string FeeRevenueAccountNo { get; set; }
        public string FeeRevenueBranchNo { get; set; }
        public int FeeRevenueAccountTypeId{ get; set; }
        public string FeeRevenueNarration { get; set; }


        public string VatAccountNo { get; set; }
        public string VatBranchNo { get; set; }
        public int VatAccountTypeId { get; set; }
        public string VatNarration { get; set; }

        public int? FeeDetailId { get; set; }
        public bool ChargeFeeToIssuingBranch { get; set; }
        public bool? FeeWaiverYN { get; set; }
        public bool? FeeEditbleYN { get; set; }
        public bool? FeeOverridenYN { get; set; }
        public decimal? FeeCharge { get; set; }
        public decimal? Vat { get; set; }
        public decimal? VatCharged { get; set; }
        public decimal? TotalCharged { get; set; }
        public bool? HasFeeBeenCharged { get; set; }
        public string FeeReferenceNumber { get; set; }
        public string FeeReversalRefNumber { get; set; }

        public string EmailAddress { get; set; }
        public int CardSequence { get; set; }

        //public int? SubProductId { get; set; }

        public bool IsPINSelected { get; set; }

        public string AccountPin { get; set; }

        public CardActivationMethod ActivationMethod { get; set; }

        public List<ProductField> ProductFields { get; set; }

        public CustomerDetails()
        {
            ActivationMethod = CardActivationMethod.Default;
        }

        public CustomerDetails(string customerTitle, string firstName, string lastName, string accountNumber,
                       int accountTypeId, string reasonIssue, string cardNumber, string pinOffset,
                       string customerIDNumber, long cardId, decimal accountBalance)
        {
            _customerTitle = customerTitle;
            FirstName = firstName;
            LastName = lastName;
            AccountNumber = accountNumber;
            AccountTypeId = accountTypeId;
            CardNumber = cardNumber;
            _customerIDNumber = customerIDNumber;
            _pinOffset = pinOffset;
            ReasonForIssue = reasonIssue;
            this.CardId = cardId;
            accountBalance = AccountBalance;
        }        

        public string GetInitializedMiddleName()
        {
            string rValue = "";
            if (!String.IsNullOrEmpty(this.MiddleName))
            {
                string[] parts = this.MiddleName.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in parts)
                {
                    rValue += s[0] + ". ";
                }

            }
            return rValue;
        }
    }
}
