using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Renewal.Entities;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Renewal.Reports;

namespace Veneka.Indigo.Renewal.dal
{
    public interface IRenewalDataAccess
    {
        long Create(RenewalFile Renewal, long auditUserId, string auditWorkstation);

        long CreateDetail(RenewalDetail detail, long auditUserId, string auditWorkstation);

        SystemResponseCode Delete(long id, long auditUserId, string auditWorkstation);

        RenewalFileViewModel Retrieve(long id, int languageId, long auditUserId, string auditWorkStation);

        RenewalDetailListModel RetrieveDetail(long id, bool checkMasking, int languageId, long auditUserId, string auditWorkStation);

        RenewalDetailListModel RetrieveDetailCard(long cardId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation);

        SystemResponseCode ChangeStatus(long id, RenewalStatusType status, long auditUserId, string auditWorkstation);

        ICollection<RenewalFileListModel> List(RenewalStatusType status, int languageId, long auditUserId, string auditWorkStation);

        ICollection<RenewalDetailListModel> ListRenewalDetail(long RenewalId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation);

        SystemResponseCode UpdateDetailStatus(long renewalDetailId,int deliveryBranchId, int currencyId, string comment,
            string cbs_account_type, string cms_account_type, RenewalDetailStatusType statusType, long auditUserId, string auditWorkStation);

        ICollection<branch> GetBranches();

        ICollection<issuer_product> GetProducts();

        List<RenewalBatch> CreateBatch(long auditUserId, string auditWorkstation);

        RenewalBatch ApproveBatch(long renewalBatchId, long auditUserId, string auditWorkstation);

        RenewalBatch RejectBatch(long renewalBatchId, long auditUserId, string auditWorkstation);

        RenewalBatch RetrieveBatch(long renewalBatchId, long auditUserId, string auditWorkstation);

        RenewalBatch ChangeBatchStatus(long renewalBatchId, RenewalBatchStatusType statusType, long auditUserId, string auditWorkstation);

        ICollection<RenewalBatch> RetrieveBatches(RenewalBatchStatusType statusType, long userId, string auditWorkstation);

        ICollection<RenewalDetailListModel> ListRenewalDetailInStatus(RenewalDetailStatusType statusType, bool checkMasking, int languageId, long auditUserId, string auditWorkStation);

        ICollection<RenewalDetailListModel> RetrieveBatchDetails(long renewalBatchId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation);

        long CreateRenewedCard(long renewalDetailId, RenewalResponseDetail renewalResponseDetail);

        SystemResponseCode LinkRenewalToCard(long renewalDetailId, long cardId,string cardNumber, long auditUserId, string auditWorkstation);

        List<long> CreateRenewalDistributionBatches(long renewalBatchId, long auditUserId, string auditWorkstation);

        RenewalBatch DistributeBatch(long renewalBatchId, long auditUserId, string auditWorkstation);

        int NextSequenceNumber(string sequenceName, ResetPeriod resetPeriod);

        List<ReportField> GetReportFields(int reportId, int languageId);

        bool CardPANMBRExists(string pan, int mbr);

        bool RenewalPANMBRExists(string pan, int mbr);
    }
}
