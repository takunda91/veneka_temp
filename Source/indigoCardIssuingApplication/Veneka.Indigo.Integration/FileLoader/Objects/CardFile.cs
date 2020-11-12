using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    /// <summary>
    /// This class represents the file being read.
    /// </summary>
    public class CardFile : IFile
    {
        public string LoadBatchReference { get; set; }
        public string OrderBatchreference { get; set; }
        public long? OrderBatchId { get; set; } 
        public string IssuerCode { get; set; }
        public string FileIdentifier { get; set; }
        public Issuer FileIssuer { get; set; }

        public List<CardFileRecord> CardFileRecords { get; set; }

        public CardFile(string issuerCode, string fileIdentifier, List<CardFileRecord> cardFileRecords)
        {
            this.IssuerCode = issuerCode;
            this.FileIdentifier = fileIdentifier;
            this.CardFileRecords = cardFileRecords;
        }
    }
}
