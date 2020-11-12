
using System.ServiceModel;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.DAL
{
    [ServiceContract]

    public interface ITerminalDAL
    {
        [OperationContract]
        TerminalMK GetTerminalMasterKey(string deviceId, long auditUserId, string auditWorkStation);
        [OperationContract]
        ZoneMasterKey GetZoneMasterKey(int issuerId, long auditUserId, string auditWorkStation);
    }
}