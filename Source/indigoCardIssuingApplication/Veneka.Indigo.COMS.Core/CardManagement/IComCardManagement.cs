using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.Core
{
    [ServiceContract(Namespace = Constants.IndigoComsURL)]
    public interface IComCardManagement
    {
        /// <summary>
        /// Performed after an account lookup to validate the details for the customer have been captured correctly.
        /// </summary>
        [OperationContract]
        ComsResponse<string> ValidateCustomerDetails(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);

        /// <summary>
        /// Links a card and account.
        /// </summary>
        [OperationContract]
        ComsResponse<string> LinkCardsToAccount(List<CustomerDetails> customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo, out Dictionary<long, LinkResponse> response);

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
        [OperationContract]
        ComsResponse<string> LinkCardToAccountAndActive(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);
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
        [OperationContract]
        ComsResponse<string> ActiveCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);

        /// <summary>
        /// Updates cards PVV.
        /// </summary>
        [OperationContract]
        ComsResponse<string> UpdatePVV(int issuerId, int productId, Track2 track2, string PVV, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);

        /// <summary>
        /// Does a lookup in CMS system to determin if the account already exists. This lookup helps with selecting and issuing screnario.
        /// </summary>
        [OperationContract]
        ComsResponse<AccountDetails> AccountLookup(int issuerId, int productId, int cardIssueReasonId, string accountNumber, AccountDetails accountDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);

        /// <summary>
        /// Method will upload newly generated cards to the CMS
        /// </summary>
        [OperationContract]
        ComsResponse<string> UploadGeneratedCards(List<CardObject> cardObjects, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);

        /// <summary>
        /// Marks the card as spoilt or unuable in the CMS
        /// </summary>
        [OperationContract]
        ComsResponse<string> SpoilCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo);

        [OperationContract]
        ComsResponse<string>  EPinRequest(string indigoID,string PRRN,string mobilenumber,string pan);
    }
}
