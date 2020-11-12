using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.ThreeDSecure.Data.Objects;

namespace Veneka.Indigo.ThreeDSecure.Data
{
    public interface I3DSDataAccess
    {
        /// <summary>
        /// This will register a new 3D Secure registration batch and send back the cards in that batch
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceGuid"></param>
        /// <param name="configId"></param>
        /// <param name="checkMasking"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        List<ThreeDSecureCardDetails> GetUnregisteredCards(int issuerId, string interfaceGuid, bool checkMasking, int languageId, long auditUserId, string auditWorkstation);

        /// <summary>
        /// Updates the 3DS batch to the new status. returns false if the batch is not in the correct status for the update
        /// </summary>
        /// <param name="threedsBatchId"></param>
        /// <param name="newBatchStatusId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        bool Update3DSecureBatchRegistered(long threedsBatchId, int languageId, long auditUserId, string auditWorkstation);

        /// <summary>
        /// Lists any batches that are in recreated status
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceGuid"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        List<ThreeDSecureBatch> GetRecreateBatches(int issuerId, string interfaceGuid, int languageId, long auditUserId, string auditWorkstation);

        /// <summary>
        /// Lists all cards for the 3D Secure batch
        /// </summary>
        /// <param name="threed_batch_id"></param>
        /// <param name="checkMasking"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        List<ThreeDSecureCardDetails> GetCards(long threedBatchId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation);


        /// <summary>
        /// Changes batch status
        /// </summary>
        /// <param name="threedsBatchId"></param>
        /// <param name="statusId"></param>
        /// <param name="statusNote"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        bool UpdateBatchStatus(long threedsBatchId, int statusId, string statusNote, int languageId, long auditUserId, string auditWorkstation);


        List<ThreeDSecureCardDetails> GetUploadedCardsCustomers(int? productid);

        List<ThreeDSecureCardDetails> GetFileHeaderInfo(int product_id, int issuer_id);

        List<int> GetIssuedProducts();

    }
}
