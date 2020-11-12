using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.Core
{
    [ServiceContract(Namespace = Constants.IndigoComsURL)]
    public interface IComPrepaidSystem
    {
        [OperationContract]
        ComsResponse<PrepaidAccountDetail> GetPrepaidAccountDetail(string cardNumber, int mbr, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);

        [OperationContract]
        ComsResponse<PrepaidCreditResponse> CreditPrepaidAccount(decimal amount, string destinationAccountNumber, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);
    }
}
