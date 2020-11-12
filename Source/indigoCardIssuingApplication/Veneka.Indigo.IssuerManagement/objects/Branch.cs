namespace Veneka.Indigo.IssuerManagement.objects
{
    public class Branch
    {
        private readonly string _branchCode;
        private readonly string _branchLocation;
        private readonly string _branchName;
        private readonly int _issuerID;
        private readonly InstitutionStatus _status;
        //private int _branchID;
        //private string p;
        //private string p_2;
        //private string p_3;
        //private string p_4;
        //private string p_5;
        //private int p_6;


        public Branch(int _branchID, string branchName, string branchCode, string branchLocation, string contactPerson,
                      string contactEmailAddress, int issuerID, InstitutionStatus status)
        {
            _branchName = branchName;
            _branchCode = branchCode;
            _branchLocation = branchLocation;
            ContactPerson = contactPerson;
            ContactEmailAddress = contactEmailAddress;
            _issuerID = issuerID;
            _status = status;
        }

        public Branch(string branchName, string branchCode, string branchLocation,
                      string contactPerson, string contactEmailAddress, int issuerID)
            : this(
                0, branchName, branchCode, branchLocation, contactPerson, contactEmailAddress, issuerID,
                InstitutionStatus.ACTIVE)
        {
        }

        public int IssuerID
        {
            get { return _issuerID; }
        }

        public string BranchName
        {
            get { return _branchName; }
        }

        public string BranchCode
        {
            get { return _branchCode; }
        }

        public string BranchLocation
        {
            get { return _branchLocation; }
        }


        public InstitutionStatus Status
        {
            get { return _status; }
        }

        //public int BranchID
        //{
        //    get { return _branchID; }
        //}


        public string ContactPerson { get; set; }

        public string ContactEmailAddress { get; set; }

        public override string ToString()
        {
            return BranchName + "-" + BranchCode;
        }
    }
}