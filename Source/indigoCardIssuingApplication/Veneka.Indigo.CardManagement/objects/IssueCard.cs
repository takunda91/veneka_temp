using System;
using Veneka.Indigo.CardManagement.objects.interfaces;

namespace Veneka.Indigo.CardManagement.objects
{
    public class IssueCard : aCard
    {
        private readonly string _accountNumber;
        private readonly string _accountType;
        private readonly string _branchCode;
        private readonly DistCardStatus _cardStatus;
        private readonly string _customerFirstName;
        private readonly string _customerLastName;
        private readonly DateTime _issueDate;
        private readonly string _issuedBy;
        private readonly int _issuerID;
        private readonly string _reasonForIssue;
        private readonly string _assigned_operator;
        private readonly string _customerTitle;

        public IssueCard(int cardId, string cardNumber, int issuerID, string batchReference, DateTime issuedDate,
                         DistCardStatus cardStatus, string issuedBy, string branchCode, string customerFirstName,
                         string customerLastName, string accountNumber, string accountType, string reasonForIssue,string assignedOperator,string title)
        {
            _cardID = cardId;
            _cardNumber = cardNumber;
            _batchReference = batchReference;
            _issueDate = issuedDate;
            _issuerID = issuerID;
            _cardStatus = cardStatus;
            _issuedBy = issuedBy;
            _branchCode = branchCode;
            _customerFirstName = customerFirstName;
            _customerLastName = customerLastName;
            _accountNumber = accountNumber;
            _accountType = accountType;
            _reasonForIssue = reasonForIssue;
            _assigned_operator = assignedOperator;
            _customerTitle = title;
        }

        public string CustomerTitle
        {
            get
            {
                return this._customerTitle;
            }
        }

        public string CustomerFirstName
        {
            get { return _customerFirstName; }
        }

        public string CustomerLastName
        {
            get { return _customerLastName; }
        }

        public string AccountNumber
        {
            get { return _accountNumber; }
        }

        public string AccountType
        {
            get { return _accountType; }
        }

        public string ReasonForIssue
        {
            get { return _reasonForIssue; }
        }

        public DistCardStatus CardIssuerStatus
        {
            get { return _cardStatus; }
        }

        public int IssuerID
        {
            get { return _issuerID; }
        }

        public string BranchCode
        {
            get { return _branchCode; }
        }

        public override string CardStatus
        {
            get { return _cardStatus.ToString(); }
        }

        public DateTime IssueDate
        {
            get { return _issueDate; }
        }

        public string IssuedBy
        {
            get { return _issuedBy; }
        }

        public string AssignedOperator
        {
            get
            {
                return _assigned_operator;
            }
        }
    }
}