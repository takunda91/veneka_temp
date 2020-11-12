using System;
using System.Collections.Generic;
using System.Linq;

namespace indigoCardIssuingWeb.CCO.objects
{
    public class PINBatch
    {
        private readonly string _batchreference;
        private readonly string _branchCode;
        private readonly int _cardCount = -1;
        private readonly int _issuerID;
        private readonly List<string> _pinBatchStatusHistory;
        private int _activePrintItem;
        private DateTime _loadedDate;
        private List<string> _pinCards;
        private List<PINMailer> _pinMailers;
        private int _remainingToPrint;
        private int _reprintCount;

        public PINBatch()
        {
        }

        public PINBatch(string batchReference, int issuerID, DateTime loadedDate,
                        List<string> pinCards, string managerComment, string operatorComment,
                        List<string> pinBatchStatusHistory, string branchCode)
        {
            _batchreference = batchReference;
            _issuerID = issuerID;
            //BatchStatus = batchStatus;
            _loadedDate = loadedDate;
            _pinCards = pinCards;
            if (_pinCards != null)
                _cardCount = _pinCards.Count();
            ManagerComment = managerComment;
            OperatorComment = operatorComment;
            _pinBatchStatusHistory = pinBatchStatusHistory;
            _branchCode = branchCode;
        }

        public string ManagerComment { get; set; }

        public string OperatorComment { get; set; }

        //public BatchType BatchType
        //{
        //    get { return BatchType.PIN_BATCH; }
        //}

        public int CardCount
        {
            get { return _cardCount; }
        }

        public int ActivePrintItem
        {
            get { return _activePrintItem; }
        }

        public DateTime LoadedDate
        {
            get { return _loadedDate; }
        }

        public string LoadedDateShort
        {
            get { return _loadedDate.ToShortDateString(); }
        }

        public string BatchReference
        {
            get { return _batchreference; }
        }

        //public PINBatchStatus BatchStatus { get; set; }

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
            set { _pinCards = value; }
        }

        public bool ReprintOfReject { get; set; }

        public List<PINMailer> PINMailers
        {
            get { return _pinMailers; }
            set
            {
                _pinMailers = value;

                //for (int i = 0; i < _pinMailers.Count; i++)
                //{
                //    if (_pinMailers[i].Status.Equals(PINMailerStatus.NOT_PRINTED))
                //    {
                //        _activePrintItem = i;
                //        break;
                //    }
                //}
                //_remainingToPrint = 0;
                //for (int i = 0; i < _pinMailers.Count; i++)
                //{
                //    if (_pinMailers[i].Status.Equals(PINMailerStatus.NOT_PRINTED))
                //        _remainingToPrint++;
                //}
            }
        }

        public int NumberOfCards
        {
            get { return _cardCount; }
        }

        public int RemainingToPrint
        {
            get { return _remainingToPrint; }
        }

        public string LastCardPrinted { get; set; }

        public string BranchCode
        {
            get { return _branchCode; }
        }

        public int RePrintCount
        {
            get { return _reprintCount; }
        }

        public void RePrintCountUp()
        {
            _reprintCount++;
        }

        public void RePrintCountDown()
        {
            _reprintCount--;
        }
    }
}