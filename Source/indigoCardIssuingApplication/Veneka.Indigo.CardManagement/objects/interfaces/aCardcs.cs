using System;

namespace Veneka.Indigo.CardManagement.objects.interfaces
{
    public abstract class aCard
    {
        protected string _batchReference;
        protected long _cardID;
        protected string _cardNumber;
        protected string _cardSequenceNumber;

        public string cardNumber
        {
            get { return _cardNumber; }
            protected set
            {
                if (value.Length >= 16 && value.Length <= 19)
                    _cardNumber = value;
                else
                    throw new Exception("Invalid Cardnumber");
            }
        }

        public string CardSequenceNumber
        {
            get { return _cardSequenceNumber; }
        }

        public string BatchReference
        {
            get { return _batchReference; }
        }

        public abstract string CardStatus { get; }

        public long CardID
        {
            get { return _cardID; }
        }
    }
}