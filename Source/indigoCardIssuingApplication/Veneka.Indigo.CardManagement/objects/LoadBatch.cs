using System;
using System.Collections.Generic;
using Veneka.Indigo.CardManagement.objects.interfaces;

namespace Veneka.Indigo.CardManagement.objects
{
    public class LoadBatch : aCardBatch
    {
        private readonly LoadBatchStatus _loadBatchStatus;
        public List<string> _loadBatchStatusHistory;

        public LoadBatch(int batchID, int issuerID, string batchRefrence, DateTime createdDate,
                         LoadBatchStatus loadBatchStatus, List<string> loadCards, List<string> loadBatchStatusHistory)
        {
            _batchReference = batchRefrence;
            _createdDate = createdDate;
            _issuerID = issuerID;
            _batchID = batchID;
            _loadBatchStatus = loadBatchStatus;
            _cards = loadCards;
            _cardCount = loadCards.Count;
            _loadBatchStatusHistory = loadBatchStatusHistory;
        }

        public LoadBatchStatus BatchStatus
        {
            get { return _loadBatchStatus; }
        }

        public override BatchType BatchType
        {
            get { return BatchType.LOAD_BATCH; }
        }

        public List<string> LoadBatchStatusHistory
        {
            get { return _loadBatchStatusHistory; }
        }
    }
}