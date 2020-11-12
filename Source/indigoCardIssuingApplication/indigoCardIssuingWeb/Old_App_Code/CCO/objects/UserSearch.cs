namespace indigoCardIssuingWeb.CCO.objects
{
    public class UserSearch
    {
        private readonly string _branchName;
        private readonly string _firstName;
        private readonly int _issuerID;
        private readonly string _lastName;
        private readonly string _userGroup;
        private readonly string _userName;

        public UserSearch(string userName, string branchName, string firstName, string lastName, int issuerID,
                          string userGroup)
        {
            _userName = userName;
            _branchName = branchName;
            _firstName = firstName;
            _lastName = lastName;
            _issuerID = issuerID;
            _userGroup = userGroup;
        }

        public string UserName
        {
            get { return _userName; }
        }

        public string BranchName
        {
            get { return _branchName; }
        }

        public string FirstName
        {
            get { return _firstName; }
        }

        public string LastName
        {
            get { return _lastName; }
        }

        public int IssuerID
        {
            get { return _issuerID; }
        }

        public string UserGroup
        {
            get { return _userGroup; }
        }
    }
}