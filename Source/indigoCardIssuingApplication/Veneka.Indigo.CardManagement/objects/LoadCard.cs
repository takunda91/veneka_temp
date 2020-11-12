using System;
using Veneka.Indigo.CardManagement.objects.interfaces;

namespace Veneka.Indigo.CardManagement.objects
{
    public class LoadCard : aCard
    {
        private readonly LoadCardStatus _cardStatus;
        private readonly int _issuerID;
        private readonly DateTime _loadDate;

        public LoadCard(int cardId, string cardNumber, string cardSequenceNumber, int issuerID, string batchRefrence,
                        DateTime loadDate, LoadCardStatus cardStatus)
        {
            _cardID = cardId;
            _cardNumber = cardNumber;
            _batchReference = batchRefrence;
            _loadDate = loadDate;
            _issuerID = issuerID;
            _cardStatus = cardStatus;
            _cardSequenceNumber = cardSequenceNumber;
        }

        public LoadCardStatus LoadCardStatus
        {
            get { return _cardStatus; }
        }

        public int IssuerID
        {
            get { return _issuerID; }
        }

        public override string CardStatus
        {
            get { return _cardStatus.ToString(); }
        }

        public DateTime LoadDate
        {
            get { return _loadDate; }
        }
    }
}