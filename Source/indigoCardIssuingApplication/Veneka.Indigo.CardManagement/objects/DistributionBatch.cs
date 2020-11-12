using System;
using System.Collections.Generic;
using Veneka.Indigo.CardManagement.objects.interfaces;

namespace Veneka.Indigo.CardManagement.objects
{
    public class DistributionBatch : aCardBatch
    {
        private readonly string _branchCode;
        private readonly DistributionBatchStatus _distBatchStatus;
        protected string _branchName;
        protected string _branchRejReason;
        protected string _centreRejReason;
        protected List<string> _distBatchStatusHistory;

        public DistributionBatch(int batchID, string brRejectReason, string cenRejectReason, int issuerID,
                                 string batchReference, DateTime createdDate, DistributionBatchStatus batchStatus,
                                 List<string> issueCards, string branchCode, string branchName,
                                 List<string> distBatchStatusHistory)
        {
            _batchReference = batchReference;
            _createdDate = createdDate;
            _issuerID = issuerID;
            _batchID = batchID;
            _branchName = branchName;
            _distBatchStatus = batchStatus;
            _branchCode = branchCode;
            _branchRejReason = brRejectReason;
            _centreRejReason = cenRejectReason;
            _cards = issueCards;
            _cardCount = issueCards.Count;
            _distBatchStatusHistory = distBatchStatusHistory;
        }

        public string BrRejReason
        {
            get { return _branchRejReason; }
        }

        public string CenRejReason
        {
            get { return _centreRejReason; }
        }

        public string BranchName
        {
            get { return _branchName; }
        }

        public string BranchCode
        {
            get { return _branchCode; }
        }

        public DistributionBatchStatus BatchStatus
        {
            get { return _distBatchStatus; }
        }


        public override BatchType BatchType
        {
            get { return BatchType.DISTRIBUTION_BATCH; }
        }

        public List<string> DistBatchStatusHistory
        {
            get { return _distBatchStatusHistory; }
        }
    }
}