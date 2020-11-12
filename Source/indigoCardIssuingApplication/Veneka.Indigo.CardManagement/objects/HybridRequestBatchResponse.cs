using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.CardManagement.objects
{
    public class HybridRequestBatchResponse
    {

        public int RequestsInBatch { get; set; }
        public int PinBatchId { get; set; }
        public string PinBatchRef { get; set; }

        public HybridRequestBatchResponse() { }
        public HybridRequestBatchResponse(int requestInBatch, int printBatchId, string printBatchRef)
        {
            this.RequestsInBatch = requestInBatch;
            this.PinBatchId = printBatchId;
            this.PinBatchRef = printBatchRef;
        }
    }
}
