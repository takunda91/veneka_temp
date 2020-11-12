using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.Integration.Objects
{
    [KnownType(typeof(ProductField))]
    [Serializable]
    [DataContract]
    public class AccountDetails
    {
        public AccountDetails()
        {
        }
        [DataMember]
        public string CustomerId { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }

        [DataMember]
        public int AccountTypeId { get; set; }
        [DataMember]
        public string CBSAccountTypeId { get; set; }
        [DataMember]
        public string CMSAccountTypeId { get; set; }
        [DataMember]
        public int CardIssueMethodId { get; set; }
        [DataMember]
        public int CardPriorityId { get; set; }
        [DataMember]
        public string CmsID { get; set; }
        [DataMember]
        public string ContractNumber { get; set; }
        [DataMember]
        public int CurrencyId { get; set; }
        [DataMember]
        public string CustomerIDNumber { get; set; }
        [DataMember]
        public int CustomerResidencyId { get; set; }
        [DataMember]
        public int CustomerTitleId { get; set; }
        [DataMember]
        public int CustomerTypeId { get; set; }
        [DataMember]
        public bool ExistsInCMS { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string MiddleName { get; set; }

        private string nameonCard;
        [DataMember]
        public string NameOnCard { get { return this.nameonCard; } set { this.nameonCard = value==null?string.Empty:  value.ToUpper(); } }
        [DataMember]
        public string PinOffset { get; set; }
        [DataMember]
        public int PriorityId { get; set; }
        [DataMember]
        public string ContactNumber { get; set; }
        [DataMember]
        public decimal AccountBalance { get; set; }
        //public int AccountBalanceCurrencyId { get; set; }
        [DataMember]
        public string Address1 { get; set; }
        [DataMember]
        public string Address2 { get; set; }
        [DataMember]
        public string Address3 { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public List<ProductField> ProductFields { get; set; }
        [DataMember]
        public List<int> AllowedCardReasons { get; set; }
        [DataMember]
        public List<CMSCard> CMSCards { get; set; }

        [DataMember]
        public string CreditContractNumber { get; set; }

        [DataMember]
        public decimal? CreditLimit { get; set; }

        [DataMember]
        public decimal? CreditLimitApproved { get; set; }

        [DataMember]
        public int? CardIssueReasonId { get; set; }
    }
}