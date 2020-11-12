using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration
{
    [InheritedExport(typeof(IPrepaidAccountProcessor))]
    public interface IPrepaidAccountProcessor : ICommon
    {
        bool GetPrepaidAccountDetail(ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, string pan, int mbr, out PrepaidAccountDetail prepaidAccountDetail, out string reponseMessage);

        bool CreditPrepaidAccount(ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, decimal amount, string destinationAccountNumber, out PrepaidCreditResponse creditResponse, out string responseMessage);
    }
}
