using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Renewal.Entities;
using Veneka.Indigo.Common;

namespace Veneka.Indigo.Renewal
{
    public interface IRenewalOperations
    {
        long Create(RenewalFile Renewal, long auditUserId, string auditWorkstation);

        bool CardPANMBRExists(string pan, int mbr);

        bool RenewalPANMBRExists(string pan, int mbr);

        SystemResponseCode Delete(long id, long auditUserId, string auditWorkstation);

        RenewalFileViewModel Retrieve(long id, int languageId, long auditUserId, string auditWorkstation);

        RenewalDetailListModel RetrieveDetail(long id, bool checkMasking, int languageId, long auditUserId, string auditWorkStation);

        RenewalDetailListModel RetrieveDetailCard(long cardId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation);

        SystemResponseCode ConfirmFileLoad(long id, long auditUserId, string auditWorkstation);

        SystemResponseCode RejectFileLoad(long id, long auditUserId, string auditWorkstation);

        SystemResponseCode ApproveFileLoad(long id, long auditUserId, string auditWorkstation);

        RenewalDetailListModel RenewalDetailChangeStatus(long id, int deliveryBranchId, string comment, int currencyId,
                string cbs_account_type, string cms_account_type, RenewalDetailStatusType statusType,
                long auditUserId, string auditWorkstation);       //mark the entry as validated

        SystemResponseCode LinkRenewalToCard(long renewalDetailId, long cardId, string cardNumber, long auditUserId, string auditWorkstation);

        RenewalBatch RenewalBatchChangeStatus(long id, RenewalBatchStatusType status, long auditUserId, string auditWorkstation);

        List<RenewalBatch> CreateBatches(long auditUserId, string auditWorkstation);                          //create the actual batch here

        List<long> CreateDistributionBatch(long renewalBatchId, long auditUserId, string auditWorkstation);                          //create the actual batch here

        RenewalBatch ApproveBatch(long id, long auditUserId, string auditWorkstation);                //approve batch for verification

        RenewalBatch DistributeBatch(long id, long auditUserId, string auditWorkstation);                //approve batch for verification

        RenewalBatch RejectBatch(long id, long auditUserId, string auditWorkstation);                //approve batch for verification

        RenewalBatch RetrieveBatch(long id, long auditUserId, string auditWorkstation);                //approve batch for verification

        SystemResponseCode UploadBatchToCMS(long id, long auditUserId, string auditWorkstation);            //history entry

        SystemResponseCode ReceiveBatch(long id, long auditUserId, string auditWorkstation);                //history entry

        SystemResponseCode ReceiveAtBranch(long id, long auditUserId, string auditWorkstation);             //history entry

        SystemResponseCode IssueCard(long id, long auditUserId, string auditWorkstation);                   //history entry

        ICollection<RenewalFileListModel> List(RenewalStatusType status, int languageId, long auditUserId, string auditWorkStation);

        ICollection<RenewalDetailListModel> ListRenewalDetail(long RenewalId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation);

        ICollection<RenewalDetailListModel> ListRenewalDetailInStatus(RenewalDetailStatusType statusType, bool checkMasking, int languageId, long auditUserId, string auditWorkStation);

        ICollection<RenewalBatch> RetrieveBatches(RenewalBatchStatusType statusType, long userId, string auditWorkstation);

        ICollection<RenewalDetailListModel> RetrieveBatchDetails(long renewalBatchId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation);

        long CreateRenewalCard(long RenewalDetailId, RenewalResponseDetail Renewal, long auditUserId, string auditWorkstation);
        
        byte[] GenerateRenewalFile(long renewalId, string userName, int languageId, long userId, string auditWorkstation);

        byte[] GenerateRenewalBatchReport(long renewalId, string userName, int languageId, long userId, string auditWorkstation);

        byte[] GenerateRenewalNewBatchReport(List<long> renewalBatchIds, string username, int languageId, long userId, string auditWorkstation);
    }
}
