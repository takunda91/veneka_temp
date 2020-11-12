namespace IndigoFileLoader.objects
{
    public class IssuerFileConfig
    {
        private readonly string _cardFileDirectory;
        private readonly FileExtensionType _cardsFileType;
        private readonly bool _ignoreHeader;
        private readonly bool _ignoreTrailer;
        private readonly bool _instantCardIssuing;
        private readonly string _issuerCode;
        private readonly int _issuerID;
        private readonly string _issuerName;
        private readonly string _pinFileDirectory;
        private readonly FileExtensionType _pinFileType;
        private readonly bool _pinMailerPrinting;
        private readonly IssuerStatus _status;
        private bool _deleteCardFile;
        //private bool _deletePinFile;

        public IssuerFileConfig(int issuerID, string issuerCode, string issuerName, IssuerStatus issuerStatus,
                                bool boolIntantIssue, string cardFileLocation, FileExtensionType cardsFileType,
                                bool boolPinMailerPrinting, string pinFileLocation, FileExtensionType pinFileType,
                                bool ignoreHeader, bool ignoreTrailer,bool deleteCardFile)
        {
            _issuerName = issuerName;
            _issuerCode = issuerCode;
            _issuerID = issuerID;
            _status = issuerStatus;
            _ignoreHeader = ignoreHeader;
            _ignoreTrailer = ignoreTrailer;
            _instantCardIssuing = boolIntantIssue;
            _pinMailerPrinting = boolPinMailerPrinting;
            _deleteCardFile = deleteCardFile;

            if (boolIntantIssue)
            {
                _cardFileDirectory = cardFileLocation;
                _cardsFileType = cardsFileType;
            }
            if (boolPinMailerPrinting)
            {
                _pinFileDirectory = pinFileLocation;
                _pinFileType = pinFileType;
            }
        }

        public IssuerStatus IssuerStatus
        {
            get { return _status; }
        }

        public string IssuerName
        {
            get { return _issuerName; }
        }

        public string IssuerCode
        {
            get { return _issuerCode; }
        }

        public string CardFileLocation
        {
            get { return _cardFileDirectory; }
        }

        public string PinFileLocation
        {
            get { return _pinFileDirectory; }
        }

        public int IssuerID
        {
            get { return _issuerID; }
        }

        public bool PinPrintingFuntion
        {
            get { return _pinMailerPrinting; }
        }

        public bool InstantIssueFuntion
        {
            get { return _instantCardIssuing; }
        }

        public FileExtensionType CardsFileType
        {
            get { return _cardsFileType; }
        }

        public FileExtensionType PinFileType
        {
            get { return _pinFileType; }
        }

        //public bool DeletePinFile
        //{
        //    get { return _deletePinFile; }
        //}

        public bool DeleteCardFile
        {
            get { return _deleteCardFile; }
        }

        public bool IgnoreHeader
        {
            get { return _ignoreHeader; }
        }

        public bool IgnoreTrailer
        {
            get { return _ignoreTrailer; }
        }
    }
}