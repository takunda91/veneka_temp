using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.API
{

    [ServiceContract(Namespace = Constants.BackOfficeApiUrl)]
    public interface IBulkPrintingAPI 
    {
        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<List<GetPrintBatchDetails>> GetApprovedPrintBatches(string token);


        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<List<ProductTemplate>> GetProductTemplate(int productId, string token);


        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<List<RequestDetails>> GetRequestsforBatch(long printBatchId, int startIndex, int endindex, string token);


        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<bool> updatePrintBatchStatus(UpdatePrintBatchDetails printBatch, string token);
    }
}
