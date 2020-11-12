using System;
using indigoCardIssuingWeb.CCO.interfaces;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.Old_App_Code.CCO.objects
{
    public class IssueCard : aCard
    {
        private readonly string accountNumber;
        private readonly string accountType;
        private readonly string branchCode;
        private readonly string customerFirstName;
        private readonly string customerLastName;
        private readonly string distbatchReference;
        private readonly DateTime issueDate;
        private readonly string issuedBy;
        private readonly string reasonForIssue;
        protected BranchCardStatus cardStatus;
        private readonly int issuerID;
        private string assignedOperator;
        private string customerTitle;

        public IssueCard(string cardNumber, string cardReferenceNumber, BranchCardStatus cardStatus, string cardSequence, string distbatchReference)
        {
            this.CardNumber = cardNumber;
            this.CardReferenceNumber = cardReferenceNumber;
            this.cardStatus = cardStatus;
            this._cardSequenceNumber = cardSequence;
            this.distbatchReference = distbatchReference;
        }

        //public IssueCard(IssueCardDTO issuecardDto)
        //{
        //    CardNumber = issuecardDto.cardNumber;
        //    this.distbatchReference = issuecardDto.batchReference;
        //    //this.cardStatus = indigoCardIssuingWeb.CCO.General.GetIsssueCardStatus(issuecardDto.cardStatus);
        //    this.issuerID = issuecardDto.issuerID;
        //    this.branchCode = issuecardDto.branchCode;
        //    DateTime.TryParse(issuecardDto.issueDate, out issueDate);
        //    this.issuedBy = issuecardDto.issuedBy;
        //    this.customerFirstName = issuecardDto.customerFirstName;
        //    this.customerLastName = issuecardDto.customerLastName;
        //    this.accountNumber = issuecardDto.accountNumber;
        //    this.accountType = issuecardDto.accountType;
        //    this.reasonForIssue = issuecardDto.reasonForIssue;
        //    this.assignedOperator = issuecardDto.assigned_operator;
        //    this.customerTitle = issuecardDto.customerTitle;
            
           
            
          
          
        //}

    
        public string CustomerTitle
        {
            get
            {
                return this.customerTitle;
            }
        }

        public string AssignedOperator
        {
            get
            {
                return this.assignedOperator;
            }
        }

        public int IssuerID
        {
            get
            {
                return this.issuerID;
            }
        }

        public string BranchCode
        {
            get
            {
                return this.branchCode;
            }
        }

        public DateTime IssueDate
        {
            get
            {
                return this.issueDate;
            }
        }

        public string IssuedBy
        {
            get
            {
                return this.issuedBy;
            }
        }

        public string MaskedCardNumber
        {
            get
            {
                return indigoCardIssuingWeb.CCO.General.MaskCardNumber(this._cardNumber);
            }
        }

        public string CustomerFirstName
        {
            get
            {
                return this.customerFirstName;
            }
        }

        public string CustomerLastName
        {
            get
            {
                return this.customerLastName;
            }
        }

        public string AccountNumber
        {
            get
            {
                return accountNumber;
            }
        }

        public string AccountType
        {
            get
            {
                return this.accountType;
            }
        }

        public string ReasonForIssue
        {
            get
            {
                return this.reasonForIssue;
            }
        }

        public string DistBatchReference
        {
            get
            {
                return this.distbatchReference;
            }
        }

        public BranchCardStatus CardStatus
        {
            get
            {
                return this.cardStatus;
            }
            set
            {
                this.cardStatus = value;
            }
        }
    }
}