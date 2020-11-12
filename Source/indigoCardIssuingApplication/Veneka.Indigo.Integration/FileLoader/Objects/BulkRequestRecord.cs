using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    [DataContract(Name = "Request", Namespace = "http://schemas.veneka.com/Indigo")]
    public class BulkRequestRecord
    {
        public BulkRequestRecord(int lineNumber, string issuerCode, string productCode, string branchCode, string cardNumber,
            string requestReferenceNumber, int cardPriorityId, string customerAccountNumber, int domicileBranchId, 
            int accountTypeId, int cardIssueReasonId, string customerFirstName, string customerMiddleName, 
            string customerLastName, string nameOnCard, int customerTitleId, int currencyId, int residentId, 
            int customerTypeId, long cmsId, string contractNumber, string idNumber, string contactNumber,
            string customerId, bool feeWaiverYN, bool feeEditableYN, decimal feeCharged, bool feeOverriddenYN,
            DateTime orderDate, string username, int numberOfRecords, List<ProductPrinting.IProductPrintField> printFields,int card_issue_methodId)
        {
            LineNumber = lineNumber;
            this.IssuerCode = issuerCode;
            this.ProductCode = productCode;
            this.BranchCode = branchCode;

            this.CardNumber = cardNumber.Trim();
            this.RequestReferenceNumber = requestReferenceNumber.Trim();
            this.OrderDate = orderDate;
            this.Username = username.Trim();
            this.NumberOfRecords = numberOfRecords;

            //this.BranchId = branchId;
            //this.ProductId = productId;
            //this.SubProductId = subProduct;

            this.CardPriorityId = cardPriorityId;
            this.CustomerAccountNumber = customerAccountNumber;
            this.DomicileBranchId = domicileBranchId;
            this.AccountTypeId = accountTypeId;
            this.CardIssueReasonId = cardIssueReasonId;
            this.CardIssueMethodId=card_issue_methodId;

            this.CustomerFirstName = customerFirstName.Trim();
            this.CustomerMiddleName = customerMiddleName.Trim();
            this.CustomerLastName = customerLastName.Trim();
            this.NameOnCard = nameOnCard.Trim();

            this.CustomerTitleId = CustomerTitleId;
            this.CurrencyId = currencyId;
            this.ResidentId = residentId;
            this.CustomerTypeId = customerTypeId;

            this.CmsId = cmsId;
            this.ContractNumber = contractNumber;
            this.IdNumber = idNumber;
            this.ContactNumber = contactNumber;
            this.CustomerId = CustomerId;

            this.FeeWaiverYN = feeWaiverYN;
            this.FeeEditableYN = FeeEditableYN;
            this.FeeCharged = feeCharged;
            this.FeeOverriddenYN = feeOverriddenYN;

            this.PrintFields = printFields;
        }               

        public int LineNumber { get; set; }

        [DataMember(IsRequired = true)]
        public string IssuerCode { get; set; }

        [DataMember(IsRequired = true)]
        public string BranchCode { get; set; }

        [DataMember(IsRequired = true)]
        public string DomicileBranchCode { get; set; }

        [DataMember(IsRequired = true)]
        public string ProductCode { get; set; }

        [DataMember(Name = "Priority")]
        public int CardPriorityId { get; set; }

        [DataMember(IsRequired = true)]
        public string CustomerAccountNumber { get; set; }

        [DataMember(Name = "AccountType", IsRequired = true)]
        public int AccountTypeId { get; set; }

        [DataMember(Name = "CardIssueType", IsRequired = true)]
        public int CardIssueReasonId { get; set; }

        [DataMember(IsRequired = true)]
        public int CardIssueMethodId { get; set; }

        [DataMember(IsRequired = true)]
        public string CustomerFirstName { get; set; }

        [DataMember]
        public string CustomerMiddleName { get; set; }

        [DataMember(IsRequired = true)]
        public string CustomerLastName { get; set; }

        [DataMember]
        public string NameOnCard { get; set; }

        [DataMember(Name = "CustomerTitle", IsRequired = true)]
        public int CustomerTitleId { get; set; }

        [DataMember(IsRequired = true)]
        public string CurrencyCode { get; set; }

        [DataMember(Name = "Residency", IsRequired = true)]
        public int ResidentId { get; set; }

        [DataMember(Name = "CustomerType", IsRequired = true)]
        public int CustomerTypeId { get; set; }

        [DataMember]
        public long CmsId { get; set; }

        [DataMember]
        public string ContractNumber { get; set; }

        [DataMember]
        public string IdNumber { get; set; }

        [DataMember]
        public string ContactNumber { get; set; }

        [DataMember]
        public string CustomerId { get; set; }

        [DataMember]
        public string RequestReferenceNumber { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        public int? IssuerId { get; set; }
        public int BranchId { get; set; }
        public int DomicileBranchId { get; set; }
        public int CurrencyId { get; set; }
        public int? ProductId { get; set; }
        public string BIN { get; set; }
        public string CardNumber { get; set; }
        public int? ProductLoadTypeId { get; set; }
        public string SubProductCode { get; set; }                
        public bool FeeWaiverYN { get; set; }        
        public bool FeeEditableYN { get; set; }        
        public decimal FeeCharged { get; set; }        
        public bool FeeOverriddenYN { get; set; }        

        public DateTime OrderDate { get; private set; }
        public string Username { get; private set; }
        public int NumberOfRecords { get; private set; }
        public long CardId { get; set; }
        public int ResultCode { get; set; }
        public List<ProductPrinting.IProductPrintField> PrintFields { get; private set; }
    }
}
