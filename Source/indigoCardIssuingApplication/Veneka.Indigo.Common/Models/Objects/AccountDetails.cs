using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Utilities;

namespace Veneka.Indigo.Common.Objects
{
    class AccountDetails
    {
        public decimal AccountBalance { get; set; }
        public string AccountNumber { get; set; }
        //public int AccountTypeId { get; set; }
        //public int CardIssueMethodId { get; set; }
        //public int CardPriorityId { get; set; }
        //public string CmsID { get; set; }
        //public string ContactNumber { get; set; }
        //public string ContractNumber { get; set; }
        //public int CurrencyId { get; set; }
        //public string CustomerId { get; set; }
        //public string CustomerIDNumber { get; set; }
        //public int CustomerResidencyId { get; set; }
        //public int CustomerTitleId { get; set; }
        //public int CustomerTypeId { get; set; }
        //public bool ExistsInCMS { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string MiddleName { get; set; }
        //public string NameOnCard { get; set; }
        //public string PinOffset { get; set; }
        //public int PriorityId { get; set; }


        public AccountDetails()
        { }

        public AccountDetails(decimal accountBalance, string accountNumber)
        {
            accountBalance = AccountBalance;
            accountNumber = AccountNumber;
        }
    }
}
