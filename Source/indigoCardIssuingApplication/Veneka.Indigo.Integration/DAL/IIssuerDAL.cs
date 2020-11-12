using System.ServiceModel;
using Veneka.Indigo.Integration.FileLoader.Objects;

namespace Veneka.Indigo.Integration.DAL
{
    [ServiceContract]
    public interface IIssuerDAL
    {
        [OperationContract]
        Issuer GetIssuer(int issuerId, long auditUserId, string auditWorkStation);
    }
}