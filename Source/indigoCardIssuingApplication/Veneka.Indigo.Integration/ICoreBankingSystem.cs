using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.Integration
{   
    /// <summary>
    /// This interfaces provides a contract between Indigo and the Core Banking System integration layer.
    /// </summary>
    [InheritedExport(typeof(ICoreBankingSystem))]
    public interface ICoreBankingSystem : ICommon
    {
        bool GetAccountDetail(string accountNumber, List<IProductPrintField> printFields, int cardIssueReasonId, int issuerId, int branchId, int productId, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkstation, out AccountDetails accountDetails, out string responseMessage);

        bool UpdateAccount(CustomerDetails customerDetails, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage);

        bool CheckBalance(CustomerDetails customerDetails, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage);

        bool ChargeFee(CustomerDetails customerDetails, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkstation, out string feeRefrenceNumber, out string responseMessage);

        bool ReverseFee(CustomerDetails customerDetails, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage);
    }
}
