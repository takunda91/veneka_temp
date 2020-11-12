using System.Collections.Generic;
using System.ServiceModel;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.DAL
{
    [ServiceContract]
    public interface IParametersDAL
    {
        [OperationContract]
        Parameters GetParameterIssuerInterface(int issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation);
        [OperationContract]
        Parameters GetParameterProductInterface(int productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation);
        [OperationContract]
        List<Parameters> GetParametersIssuerInterface(int? issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation);
        [OperationContract]
        List<Parameters> GetParametersProductInterface(int? productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation);
    }
}