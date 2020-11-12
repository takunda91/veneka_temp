using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    public class BulkRequestsFile : IFile
    {
        public string IssuerCode { get; set; }
        public string FileIdentifier { get; set; }
        public Issuer FileIssuer { get; set; }

        internal IssuerProduct FileProduct { get; set; }

        public List<BulkRequestRecord> CardRequestFileRecords { get; set; }

        public BulkRequestsFile(string issuerCode, string fileIdentifier, List<BulkRequestRecord> cardRequestFileRecords)
        {
            this.IssuerCode = issuerCode;
            this.FileIdentifier = fileIdentifier;
            this.CardRequestFileRecords = cardRequestFileRecords;
        }

    }
}
