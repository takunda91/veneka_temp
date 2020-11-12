using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.Core.CoreBanking
{
    [ServiceContract(Namespace = Constants.IndigoComsURL)]
    public interface IComCoreBankingSystem
    {
        [OperationContract]
        ComsResponse<AccountDetails> GetAccountDetail(CardObject obj, int cardIssueReasonId, int issuerId, int branchId, int productId, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);

        [OperationContract]
        ComsResponse<bool> UpdateAccount(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);
        [OperationContract]
        ComsResponse<bool> CheckBalance(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);
        [OperationContract]
        ComsResponse<bool> ChargeFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);
        [OperationContract]
        ComsResponse<bool> ReverseFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);
        
    }
}
