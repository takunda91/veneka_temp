using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Objects;
using System.ComponentModel.Composition;
using Common.Logging;

namespace Veneka.Indigo.Integration
{
    public enum LinkResponse
    {
        SUCCESS = 0,
        WARNING = 1,
        ERROR = 9, //Integer value same as CMS_ERROR in branch card status
        RETRY = 15 //Integer value same as CMS_RETRY in branch card status
    }

    public class DistEventArgs : EventArgs
    {
        public DistEventArgs(long distBatchId, bool success, string responseMessage, Exception processException)
        {
            DistBatchId = distBatchId;
            Success = success;
            ResponseMessage = responseMessage;
            ProcessException = processException;
        }

        public DistEventArgs(long distBatchId, bool success, string responseMessage)
            :this (distBatchId, success, responseMessage, null)
        { }

        public long DistBatchId { get; private set; }
        public bool Success { get; private set; }
        public string ResponseMessage { get; private set; }
        public Exception ProcessException { get; private set; }
    }

    /// <summary>
    /// This interfaces provides a contract between Indigo and the Card Management System integration layer.
    /// </summary>
    [InheritedExport(typeof(ICardManagementSystem))]
    public interface ICardManagementSystem : ICommon
    {
        /// <summary>
        /// Raised if long running upload happnes
        /// </summary>
        event EventHandler<DistEventArgs> OnUploadCompleted;

        bool OnUploadCompletedSubscribed { get; set; }

        /// <summary>
        /// Links the list of cards to the Accounts
        /// </summary>
        /// <param name="customerDetails"></param>
        /// <param name="externalFields"></param>
        /// <param name="config"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        /// , out Dictionary<int, LinkResponse> response
        LinkResponse LinkCardsToAccount(List<CustomerDetails> customerDetails, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation,out  Dictionary<long, LinkResponse> response, out string responseMessage);

        /// <summary>
        /// Links a card and account.
        /// </summary>
        /// <param name="accountDetails"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        LinkResponse LinkCardToAccountAndActive(CustomerDetails customerDetails, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);


        LinkResponse ActiveCard(CustomerDetails customerDetails, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        /// <summary>
        /// Updates cards PVV.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="PAN"></param>
        /// <param name="PVV"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        bool UpdatePVV(int issuerId, int productId, Common.Track2 track2, string PVV, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        /// <summary>
        /// Does a lookup in CMS system to determin if the account already exists. This lookup helps with selecting and issuing screnario.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="accountNumber"></param>
        /// <param name="auditUserId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditWorkStation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        bool AccountLookup(int issuerId, int productId, int cardIssueReasonId, string accountNumber, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, ref AccountDetails accountDetails, out string responseMessage);

        /// <summary>
        /// Performed after an account lookup to validate the details for the customer have been captured correctly.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="productId"></param>
        /// <param name="customerDetails"></param>
        /// <param name="config"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        bool ValidateCustomerDetails(CustomerDetails customerDetails, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        /// <summary>
        /// Method will upload newly generated cards to the CMS
        /// </summary>
        /// <param name="cardObjects"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        bool UploadGeneratedCards(List<CardObject> cardObjects, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage);

        /// <summary>
        /// Marks the card as spoilt or unuable in the CMS
        /// </summary>
        /// <param name="cardObject"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        bool SpoilCard(CustomerDetails customerDetails, External.ExternalSystemFields externalFields, Config.IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage);

        /// <summary>
        /// This method is used in the Remote Component client to fetch details for the CardDetails object.
        /// </summary>
        /// <param name="cardDetails"></param>
        /// <param name="externalFields"></param>
        /// <param name="config"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        bool RemoteFetchDetails(List<Remote.CardDetail> cardDetails, External.ExternalSystemFields externalFields, Config.IConfig config, out List<Remote.CardDetailResponse> failedCards, out string responseMessage);
    }
}
