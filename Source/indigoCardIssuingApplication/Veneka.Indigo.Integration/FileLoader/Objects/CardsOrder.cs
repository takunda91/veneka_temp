using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    public class CardsOrder
    {
        public long DistBatchId { get; private set; }
        public int IssuerId { get; private set; }
        public int BranchId { get; private set; }
        public int NumberOfCards { get; private set; }
        public string DistBatchRef { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public CardsOrder(int distBatchId, int issuerId, int branchId, int numberOfCards, string distBatchRef, DateTime createdOn)
        {
            this.DistBatchId = distBatchId;
            this.IssuerId = issuerId;
            this.BranchId = branchId;
            this.NumberOfCards = numberOfCards;
            this.DistBatchRef = distBatchRef;
            this.CreatedOn = createdOn;
        }

        public CardsOrder(System.Data.SqlClient.SqlDataReader reader)
        {
            //var dist = reader["dist_batch_id"];
            this.DistBatchId = reader["dist_batch_id"] as long? ?? 0;
            this.IssuerId = reader["issuer_id"] as int? ?? 0;
            this.BranchId = reader["branch_id"] as int? ?? 0;
            this.NumberOfCards = reader["no_cards"] as int? ?? 0;
            this.DistBatchRef = reader["dist_batch_reference"].ToString();
            this.CreatedOn = reader["date_created"] as DateTime? ?? DateTime.Now;
        }
    }
}
