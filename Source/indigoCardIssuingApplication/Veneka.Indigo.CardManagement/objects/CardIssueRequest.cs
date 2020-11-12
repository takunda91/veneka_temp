namespace Veneka.Indigo.CardManagement.objects
{
    public class CardIssueRequest
    {
        private readonly string _branchCode;
        private readonly string _cardNumber;
        private readonly Customer _customer;
        private readonly string _reasonForIssue;
        private readonly string _userIssuing;

        public CardIssueRequest(string cardNumber, string branchCode, string userIssuing, string reasonForIssue,
                                Customer customer)
        {
            _cardNumber = cardNumber;
            _branchCode = branchCode;
            _userIssuing = userIssuing;
            _customer = customer;
            _reasonForIssue = reasonForIssue;
        }

        public string ReasonForIssue
        {
            get { return _reasonForIssue; }
        }

        public string CardNumber
        {
            get { return _cardNumber; }
        }

        public string BranchCode
        {
            get { return _branchCode; }
        }

        public string UserIssuing
        {
            get { return _userIssuing; }
        }

        public Customer Customer
        {
            get { return _customer; }
            
        }
    }
    #region Structs
    public struct structCard
    {
        public string card_number { get; set; }
        public string card_reference_number { get; set; }
    }
    
    #endregion
  
}