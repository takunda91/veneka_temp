using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.CardManagement.objects
{
    public class CardRequestBatchResponse
    {
        public int CardsInBatch { get; set; }
        public int DistBatchId { get; set; }
        public string DistBatchRef { get; set; }

        public CardRequestBatchResponse() { }
        public CardRequestBatchResponse(int cardsInBatch, int distBatchId, string distBatchRef)
        {
            this.CardsInBatch = cardsInBatch;
            this.DistBatchId = distBatchId;
            this.DistBatchRef = distBatchRef;
        }
    }
}
