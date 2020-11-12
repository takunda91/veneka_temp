using System;
using System.Collections.Generic;

namespace Veneka.Indigo.CardManagement.objects.interfaces
{
    public abstract class aCardBatch
    {
        protected int _batchID;
        protected string _batchReference;
        protected BatchType _batchType;
        protected int _cardCount;
        protected List<string> _cards;
        protected DateTime _createdDate;
        protected int _issuerID;

        public List<string> Cards
        {
            get { return _cards; }
        }

        public int IssuerID
        {
            get { return _issuerID; }
        }

        public int CardCount
        {
            get { return _cardCount; }
        }

        public int BatchID
        {
            get { return _batchID; }
        }

        public DateTime CreatedDate
        {
            get { return _createdDate; }
        }

        public abstract BatchType BatchType { get; }

        public string BatchReference
        {
            get { return _batchReference; }
        }

        public List<string> BatchCards
        {
            get { return _cards; }
        }

        public int NumberOfCards
        {
            get
            {
                if (_cards == null)
                    return 0;

                return _cards.Count;
            }
        }
    }
}