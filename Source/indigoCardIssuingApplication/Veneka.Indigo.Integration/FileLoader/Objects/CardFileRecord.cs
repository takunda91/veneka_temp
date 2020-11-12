using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    public sealed class CardFileRecord
    {
        private long? _cardId = null;
        //Immutable Properties
        public int LineNumber { get; private set; }
        
        public string BIN { get; private set; }
        public string SubProduct { get; private set; }
        public string BranchCode { get; private set; }
        public string PAN { get; private set; }
        public string PsuedoPAN { get; private set; }
        public string SequenceNumber { get; private set; }
        public DateTime? ExpiryDate { get; private set; }

        //Mutable Properties
        public long? CardId {
            get
            {
                return _cardId; 
            }
            set
            {
                if (_cardId == null)
                    _cardId = value;
                else
                    throw new ArgumentException("Cannot set value as it already has a value.");
            }
        }
        public string CardReference { get; set; }
        internal int? BranchId { get; set; }
        internal int? ProductId { get; set; }
        internal int? IssuerId { get; set; }
        public string SubProductCode { get; set; }
        internal int? CardIssueMethodId { get; set; }
        internal int? ProductLoadTypeId { get; set; }
        internal bool? isAlreadyLoaded { get; set; }

        /// <summary>
        /// Card File Record Object, represents a decoded card from a card file.
        /// </summary>
        /// <param name="bin">First 6 digits of the PAN</param>
        /// <param name="branchCode"></param>
        /// <param name="pan"></param>
        /// <param name="psuedoPAN"></param>
        /// <param name="subproduct">This must be populated with remainder of card number after BIN to allow proper lookup</param>
        /// <param name="sequenceNumber"></param>
        /// <param name="expiryDate"></param>
        /// <param name="cardId"></param>
        public CardFileRecord(int lineNumber, string bin, string branchCode, string pan, string psuedoPAN, string subproduct, string sequenceNumber, DateTime? expiryDate, long? cardId)
        {
            if (String.IsNullOrWhiteSpace(bin))
                throw new ArgumentNullException("bin", "Cannot be null, empty or whitespace.");

            if (String.IsNullOrWhiteSpace(pan))
                throw new ArgumentNullException("pan", "Cannot be null, empty or whitespace.");

            if (bin.Trim().Length != 6)
                throw new ArgumentException("Must be 6 digits long.", "bin");

            this.LineNumber = lineNumber;
            this.CardId = cardId;
            this.BIN = bin.Trim();
            this.PAN = pan.Trim();
            this.PsuedoPAN = psuedoPAN;
            this.SequenceNumber = sequenceNumber.Trim();
            this.ExpiryDate = expiryDate;
            this.SubProduct = subproduct;
            this.BranchCode = branchCode;
        }

        public CardFileRecord(int lineNumber, string bin, string branchCode, string pan, string psuedoPAN, string subproduct, string sequenceNumber, DateTime? expiryDate, long? cardId, string referenceNumber)
            : this(lineNumber, bin, branchCode, pan, psuedoPAN, subproduct, sequenceNumber, expiryDate, cardId)
        {
            this.CardReference = referenceNumber;
        }
    }
}
