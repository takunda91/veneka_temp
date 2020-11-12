using System;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.CCO.objects
{
    public class PINMailer
    {
        //private readonly string _PVVOffset;
        //private readonly string _batchReference;
        //private readonly string _cardNumber;
        //private readonly string _customerAddress;
        //private readonly string _customerName;
        //private readonly string _encryptedPIN;
        //private readonly int _issuerID;
        //private readonly string _pinMailerReference;
        private bool _boolPrintInProgress;
        private bool _boolReprintApproved;
        private bool _boolReprintRequest;
        //private PINMailerStatus _pinMailerStatus;
        //private DateTime _printedDate;
        //private DateTime _reprintedDate;

        public PINMailer()
        {

        }

        //public int IssuerID
        //{
        //    get { return _issuerID; }
        //}

        //public string BatchReference
        //{
        //    get { return _batchReference; }
        //}

        //public string PINMailerReference
        //{
        //    get { return _pinMailerReference; }
        //}

        //public PINMailerStatus Status
        //{
        //    get { return _pinMailerStatus; }
        //    set { _pinMailerStatus = value; }
        //}

        //public string ReprintAction
        //{
        //    get
        //    {
        //        if (_pinMailerStatus.Equals(PINMailerStatus.PRINTED) ||
        //            _pinMailerStatus.Equals(PINMailerStatus.RE_PRINTED))
        //        {
        //            if (!_boolReprintApproved && !_boolPrintInProgress)
        //            {
        //                if (!_boolReprintRequest)
        //                    return ">>";
        //                else
        //                    return ">>  >>";
        //            }
        //            else
        //                return "";
        //        }
        //        else
        //            return "";
        //    }
        //}

        //public string CardNumber
        //{
        //    get { return _cardNumber; }
        //}

        public bool ReprintRequest
        {
            get { return _boolReprintRequest; }
            set { _boolReprintRequest = value; }
        }

        public bool ReprintApproved
        {
            get { return _boolReprintApproved; }
            set { _boolReprintApproved = value; }
        }

        public bool PrintInProgress
        {
            get { return _boolPrintInProgress; }
            set { _boolPrintInProgress = value; }
        }

        public string ReprintStatus
        {
            get
            {
                if (_boolReprintRequest)
                    return "SELECTED";
                else
                    return "";
            }
        }

        //public string MaskedCardNumber
        //{
        //    get { return General.MaskCardNumber(_cardNumber); }
        //}

        //public string PVVOffset
        //{
        //    get { return _PVVOffset; }
        //}

        //public string EncryptedPIN
        //{
        //    get { return _encryptedPIN; }
        //}

        //public string CustomerName
        //{
        //    get { return _customerName; }
        //}

        //public string CustomerAddress
        //{
        //    get { return _customerAddress; }
        //}

        //public DateTime PrintedDate
        //{
        //    get { return _printedDate; }
        //}

        //public DateTime ReprintedDate
        //{
        //    get { return _reprintedDate; }
        //}

        //public string DisplayPrintedDate
        //{
        //    get
        //    {
        //        if (_printedDate.Equals(DateTime.MinValue) || _printedDate.Year == 1900 || _printedDate.Year == 1)
        //            return "";
        //        else
        //            return _printedDate.ToShortDateString();
        //    }
        //}

        //public string DisplayReprintedDate
        //{
        //    get
        //    {
        //        if (_reprintedDate.Equals(DateTime.MinValue) || _reprintedDate.Year == 1900 || _reprintedDate.Year == 1)
        //            return "";
        //        else
        //            return _reprintedDate.ToShortDateString();
        //    }
        //}
    }
}