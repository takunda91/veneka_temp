using Veneka.Indigo.Common;
namespace Veneka.Indigo.CardManagement.objects
{
    public class Customer
    {
        private readonly AccountType _accountType;
        private readonly string _customerIDnumber;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _primaryAccountNumber;
        private readonly ReasonForIssue _reasonForIssue;
        private readonly string _title;


        public Customer(string title, string firstName, string lastName, string customerIDnumber,
                        string primaryAccountNumber, AccountType accountType, ReasonForIssue reasonForIssue)
        {
            _title = title;
            _firstName = firstName;
            _lastName = lastName;
            _customerIDnumber = customerIDnumber;
            _primaryAccountNumber = primaryAccountNumber;
            _accountType = accountType;
            _reasonForIssue = reasonForIssue;
        }

        //RL --- follow up with TM as to why he did this...
        //Tshepo changed all properties to Public.
        public string Title
        {
            get { return _title; }
        }

        public string FirstName
        {
            get { return _firstName; }
        }

        public string LastName
        {
            get { return _lastName; }
        }

        public string CustomerIDnumber
        {
            get { return _customerIDnumber; }
        }

        public string PrimaryAccountNumber
        {
            get { return _primaryAccountNumber; }
        }

        public AccountType AccountType
        {
            get { return _accountType; }
        }

        public ReasonForIssue ReasonForIssue
        {
            get { return _reasonForIssue; }
        }
    }
}