using System;
using indigoCardIssuingWeb.utility;

namespace indigoCardIssuingWeb.CCO.interfaces
{
    public abstract class aCard
    {
        protected string _cardNumber;
        protected string _cardReferenceNumber;
        protected string _cardSequenceNumber;
        protected long _cardID;

        public string CardNumber
        {
            get { return _cardNumber; }
            set
            {
                if (value.Length >= 16 && value.Length <= 19)
                    _cardNumber = value;
                else
                    throw new Exception("Invalid Cardnumber");
            }
        }

        public string CardReferenceNumber
        {
            get { return _cardReferenceNumber; }
            set
            {
                if (value.Length >= 16 && value.Length <= 19)
                    _cardReferenceNumber = value;
                else
                    throw new Exception("Invalid Card Reference Number");
            }
        }


        public string PartialCardNumber
        {
            get { return UtilityClass.DisplayPartialPAN(_cardNumber, '*'); }
        }

        public long CardID
        {
            get { return _cardID; }
        }
    }
}