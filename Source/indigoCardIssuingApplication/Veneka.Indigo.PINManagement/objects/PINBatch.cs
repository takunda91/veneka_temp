using System;
using System.Collections.Generic;
using System.Linq;

namespace Veneka.Indigo.PINManagement.objects
{
    public class PINBatch
    {
        private readonly string _batchreference;
        private readonly string _branchCode;
        private readonly int _cardCount;
        private readonly int _issuerID;
        private readonly DateTime _loadedDate;
        private readonly List<string> _pinBatchStatusHistory;
        private readonly List<string> _pinCards;

        public PINBatch(string batchReference, int issuerID, PINBatchStatus batchStatus, DateTime loadedDate,
                        List<string> pinCards, string managerComment, string operatorComment,
                        List<string> pinBatchStatusHistory, string branchCode)
        {
            _batchreference = batchReference;
            _issuerID = issuerID;
            BatchStatus = batchStatus;
            _loadedDate = loadedDate;
            _pinCards = pinCards;
            if (_pinCards != null)
                _cardCount = _pinCards.Count();
            ManagerComment = managerComment;
            OperatorComment = operatorComment;
            _pinBatchStatusHistory = pinBatchStatusHistory;
            _branchCode = branchCode;
        }

        public BatchType BatchType
        {
            get { return BatchType.PIN_BATCH; }
        }

        public int CardCount
        {
            get { return _cardCount; }
        }

        public DateTime LoadedDate
        {
            get { return _loadedDate; }
        }

        public string BatchReference
        {
            get { return _batchreference; }
        }

        public PINBatchStatus BatchStatus { get; set; }

        public int IssuerID
        {
            get { return _issuerID; }
        }

        public List<string> PINBatchStatusHistory
        {
            get { return _pinBatchStatusHistory; }
        }

        public List<string> PINCards
        {
            get { return _pinCards; }
        }

        public int NumberOfCards
        {
            get { return _cardCount; }
        }

        public string ManagerComment { get; set; }

        public string OperatorComment { get; set; }

        public string BranchCode
        {
            get { return _branchCode; }
        }
    }
}