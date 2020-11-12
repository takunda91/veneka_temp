using System.Collections.Generic;
using System.ServiceModel;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.DAL
{
    [ServiceContract]
    public interface IExportBatchDAL
    {
        [OperationContract]
        List<CardObject> FetchCardObjectsForExportBatch(long exportBatchId, int languageId, long auditUserId, string auditWorkstation);
        [OperationContract]
        Dictionary<long, string> FindExportBatches(int issuerId, int? productId, int exportBatchStatusesId, int languageId, long auditUserId, string auditWorkstation);
        [OperationContract]
        ExportBatchGeneration GenerateBatches(int? issuerId, long auditUserId, string auditWorkStation);
        [OperationContract]
        int StatusChangeExported(long exportBatchId, int languageId, long auditUserId, string auditWorkStation);
    }
}