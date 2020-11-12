using System;
using System.Collections.Generic;
using System.Text;

namespace IndigoFileLoader.objects
{
    /// <summary>
    /// This class represents the decoded card record from a load file.
    /// </summary>
    public sealed class CardFileRecord
    {        
        //Immutable Properties
        public string BranchCode { get; private set; }               
        public string PAN { get; private set; }
        public string PsuedoPAN { get; private set; }
        public string SequenceNumber { get; private set; }

        //Mutable Properties
        public int BranchId { get; set; }
        public int ProductId { get; set; }
        public bool isAlreadyLoaded { get; set; }

        /// <summary>
        /// Card File Record Object, represents a decoded card from a card file.
        /// </summary>
        /// <param name="BranchCode"></param>
        /// <param name="PAN"></param>
        /// <param name="PsuedoPAN"></param>
        /// <param name="SequenceNumber"></param>
        public CardFileRecord(string BranchCode, string PAN, string PsuedoPAN, string SequenceNumber)
        {
            this.BranchCode = BranchCode;
            this.PAN = PAN;
            this.PsuedoPAN = PsuedoPAN;
            this.SequenceNumber = SequenceNumber;            
        }

    }
}
