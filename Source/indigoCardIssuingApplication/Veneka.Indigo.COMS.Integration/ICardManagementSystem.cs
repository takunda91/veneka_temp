using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration;
using System.IO;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Common;

namespace Veneka.Indigo.COMS.Integration
{
    public class ICardManagementSystemCOMS
    {
        public string SQLConnectionString { get; set; }

        public DirectoryInfo IntegrationFolder { get; set; }

        public static bool OnUploadEventSubscribed { get; set; }

    }
    
    public interface ICardManagementSystem : ICommon
    {
        /// <summary>
        /// Performed after an account lookup to validate the details for the customer have been captured correctly.
        /// </summary>
        ComsResponse<string> ValidateCustomerDetails(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, AuditInfo auditInfo);

        /// <summary>
        /// Links a card and account.
        /// </summary>

        ComsResponse<string> LinkCardsToAccount(List<CustomerDetails> customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        /// <summary>
        /// Link Account and Active
        /// </summary>
        /// <param name="customerDetails"></param>
        /// <param name="externalFields"></param>
        /// <param name="config"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        ComsResponse<string> LinkCardToAccountAndActive(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);
        /// <summary>
        /// Active Card
        /// </summary>
        /// <param name="customerDetails"></param>
        /// <param name="externalFields"></param>
        /// <param name="config"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>

        ComsResponse<string> ActiveCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        /// <summary>
        /// Updates cards PVV.
        /// </summary>
        ComsResponse<string> UpdatePVV(int issuerId, int productId, Track2 track2, string PVV, ExternalSystemFields externalFields, IConfig config, AuditInfo auditInfo);

        /// <summary>
        /// Does a lookup in CMS system to determin if the account already exists. This lookup helps with selecting and issuing screnario.
        /// </summary>
        ComsResponse<AccountDetails> AccountLookup(int issuerId, int productId, int cardIssueReasonId, string accountNumber, AccountDetails accountDetails, ExternalSystemFields externalFields, IConfig config, AuditInfo auditInfo);

        /// <summary>
        /// Method will upload newly generated cards to the CMS
        /// </summary>
        ComsResponse<string> UploadGeneratedCards(List<CardObject> cardObjects, ExternalSystemFields externalFields, IConfig config, AuditInfo auditInfo);

        /// <summary>
        /// Marks the card as spoilt or unuable in the CMS
        /// </summary>
        ComsResponse<string> SpoilCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, AuditInfo auditInfo);

        /// <summary>
        /// Link Account and Active
        /// </summary>
        /// <param name="customerDetails"></param>
        /// <param name="externalFields"></param>
        /// <param name="config"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        ComsResponse<string> ActivateCardWithPAN(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        /// <summary>
        /// Link Account and Active
        /// </summary>
        /// <param name="customerDetails"></param>
        /// <param name="externalFields"></param>
        /// <param name="config"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        ComsResponse<string> ActivateCardWithPANAndActivate(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);
    }
}
