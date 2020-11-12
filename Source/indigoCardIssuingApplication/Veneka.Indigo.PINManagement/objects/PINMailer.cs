using System;

namespace Veneka.Indigo.PINManagement.objects
{
    public class PINMailer
    {
        private readonly string _PVVOffset;
        private readonly string _batchReference;
        private readonly string _cardNumber;
        private readonly string _customerAddress;
        private readonly string _customerName;
        private readonly string _encryptedPIN;
        private readonly int _issuerID;
        private readonly string _pinMailerReference;
        private readonly PINMailerStatus _pinMailerStatus;
        private readonly DateTime _printedDate;
        private readonly DateTime _reprintedDate;

        public PINMailer(int issuerID, string batchReference, string pinMailerReference, PINMailerStatus pinMailerStatus,
                         string cardNumber, string PVVOffset, string encryptedPIN, string customerName,
                         string customerAddress,
                         DateTime printedDate, DateTime reprintedDate)
        {
            _issuerID = issuerID;
            _batchReference = batchReference;
            _pinMailerReference = pinMailerReference;
            _pinMailerStatus = pinMailerStatus;
            _cardNumber = cardNumber;
            _PVVOffset = PVVOffset;
            _encryptedPIN = encryptedPIN;
            _customerName = customerName;
            _customerAddress = customerAddress;
            _printedDate = printedDate;
            _reprintedDate = reprintedDate;
        }

        public int IssuerID
        {
            get { return _issuerID; }
        }

        public string BatchReference
        {
            get { return _batchReference; }
        }

        public string PINMailerReference
        {
            get { return _pinMailerReference; }
        }

        public PINMailerStatus Status
        {
            get { return _pinMailerStatus; }
        }

        public string CardNumber
        {
            get { return _cardNumber; }
        }

        public string PVVOffset
        {
            get { return _PVVOffset; }
        }

        public string EncryptedPIN
        {
            get { return _encryptedPIN; }
        }

        public string CustomerName
        {
            get { return _customerName; }
        }

        public string CustomerAddress
        {
            get { return _customerAddress; }
        }

        public DateTime PrintedDate
        {
            get { return _printedDate; }
        }

        public DateTime ReprintedDate
        {
            get { return _reprintedDate; }
        }
    }
}