using System.Collections.Generic;

namespace indigoCardIssuingWeb.CCO.objects
{
    public class PINSearchResult
    {
        public List<PINBatch> BatchList { get; set; }

        public PINBatch Batch { get; set; }
    }
}