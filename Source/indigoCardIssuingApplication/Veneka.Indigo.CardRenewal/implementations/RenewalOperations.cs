using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Renewal.dal;

using Veneka.Indigo.Common;
using Veneka.Indigo.Renewal.Entities;

namespace Veneka.Indigo.Renewal
{
    public class RenewalOperations : IRenewalOperations
    {
        IRenewalDataAccess dataAccess;

        public RenewalOperations()
        {
            dataAccess = new RenewalDataAccess();
        }

        public RenewalBatch ApproveBatch(long id, long auditUserId, string auditWorkstation)
        {
            return dataAccess.ApproveBatch(id, auditUserId, auditWorkstation);
        }

        public RenewalBatch RejectBatch(long id, long auditUserId, string auditWorkstation)
        {
            return dataAccess.RejectBatch(id, auditUserId, auditWorkstation);
        }

        public RenewalBatch RetrieveBatch(long id, long auditUserId, string auditWorkstation)
        {
            return dataAccess.RetrieveBatch(id, auditUserId, auditWorkstation);
        }

        public RenewalBatch RenewalBatchChangeStatus(long id, RenewalBatchStatusType status, long auditUserId, string auditWorkstation)
        {
            return dataAccess.ChangeBatchStatus(id, status, auditUserId, auditWorkstation);
        }

        public SystemResponseCode ApproveFileLoad(long id, long auditUserId, string auditWorkstation)
        {
            return dataAccess.ChangeStatus(id, RenewalStatusType.Approved, auditUserId, auditWorkstation);
        }

        public SystemResponseCode ConfirmFileLoad(long id, long auditUserId, string auditWorkstation)
        {
            return dataAccess.ChangeStatus(id, RenewalStatusType.LoadConfirmed, auditUserId, auditWorkstation);
        }

        public long Create(RenewalFile Renewal, long auditUserId, string auditWorkstation)
        {
            long RenewalId = dataAccess.Create(Renewal, auditUserId, auditWorkstation);
            Renewal.Id = RenewalId;

            foreach (var item in Renewal.Details)
            {
                item.RenewalId = RenewalId;
                item.RenewalDetailId = dataAccess.CreateDetail(item, auditUserId, auditWorkstation);
            }

            return RenewalId;
        }

        public List<RenewalBatch> CreateBatches(long auditUserId, string auditWorkstation)
        {
            return dataAccess.CreateBatch(auditUserId, auditWorkstation);
        }

        public SystemResponseCode Delete(long id, long auditUserId, string auditWorkstation)
        {
            return dataAccess.Delete(id, auditUserId, auditWorkstation);
        }

        public RenewalBatch DistributeBatch(long id, long auditUserId, string auditWorkstation)
        {
            return dataAccess.DistributeBatch(id, auditUserId, auditWorkstation);
        }

        public SystemResponseCode IssueCard(long id, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public ICollection<RenewalFileListModel> List(RenewalStatusType status, int languageId, long auditUserId, string auditWorkStation)
        {
            return dataAccess.List(status, languageId, auditUserId, auditWorkStation);
        }

        public ICollection<RenewalDetailListModel> ListRenewalDetail(long RenewalId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation)
        {
            return dataAccess.ListRenewalDetail(RenewalId, checkMasking, languageId, auditUserId, auditWorkStation);
        }

        public SystemResponseCode ReceiveAtBranch(long id, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode ReceiveBatch(long id, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode RejectFileLoad(long id, long auditUserId, string auditWorkstation)
        {
            return dataAccess.ChangeStatus(id, RenewalStatusType.Rejected, auditUserId, auditWorkstation);
        }

        public RenewalFileViewModel Retrieve(long id, int languageId, long auditUserId, string auditWorkstation)
        {
            var result = dataAccess.Retrieve(id, languageId, auditUserId, auditWorkstation);
            return result;
        }

        public RenewalDetailListModel RetrieveDetail(long id, bool checkMasking, int languageId, long auditUserId, string auditWorkStation)
        {
            var result = dataAccess.RetrieveDetail(id, checkMasking, languageId, auditUserId, auditWorkStation);
            return result;
        }

        public RenewalDetailListModel RetrieveDetailCard(long cardId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation)
        {
            var result = dataAccess.RetrieveDetailCard(cardId, checkMasking, languageId, auditUserId, auditWorkStation);
            return result;
        }

        public SystemResponseCode UploadBatchToCMS(long id, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public RenewalDetailListModel RenewalDetailChangeStatus(long id, int deliveryBranchId, string comment, int currencyId,
            string cbs_account_type, string cms_account_type,
            RenewalDetailStatusType statusType, long auditUserId, string auditWorkstation)
        {
            dataAccess.UpdateDetailStatus(id, deliveryBranchId, currencyId, comment, cbs_account_type, cms_account_type, statusType, auditUserId, auditWorkstation);
            var result = dataAccess.RetrieveDetail(id, true, 0, auditUserId, auditWorkstation);
            return result;
        }

        public ICollection<RenewalBatch> RetrieveBatches(RenewalBatchStatusType statusType, long userId, string auditWorkstation)
        {
            var result = dataAccess.RetrieveBatches(statusType, userId, auditWorkstation);
            return result;
        }

        public ICollection<RenewalDetailListModel> ListRenewalDetailInStatus(RenewalDetailStatusType statusType, bool checkMasking, int languageId, long auditUserId, string auditWorkStation)
        {
            return dataAccess.ListRenewalDetailInStatus(statusType, checkMasking, languageId, auditUserId, auditWorkStation);
        }

        public ICollection<RenewalDetailListModel> RetrieveBatchDetails(long renewalBatchId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation)
        {
            return dataAccess.RetrieveBatchDetails(renewalBatchId, checkMasking, languageId, auditUserId, auditWorkStation);
        }


        public long CreateRenewalCard(long renewalDetailId, RenewalResponseDetail renewalResponse, long auditUserId, string auditWorkstation)
        {
            return dataAccess.CreateRenewedCard(renewalDetailId, renewalResponse);
        }

        public SystemResponseCode LinkRenewalToCard(long renewalDetailId, long cardId, string cardNumber, long auditUserId, string auditWorkstation)
        {
            try
            {
                if (cardNumber.Contains("-"))
                {
                    var split = cardNumber.Split('-').ToArray();
                    if (split.Length > 1)
                    {
                        cardNumber = split[0];
                    }
                }
                return dataAccess.LinkRenewalToCard(renewalDetailId, cardId, cardNumber, auditUserId, auditWorkstation);
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        public List<long> CreateDistributionBatch(long renewalBatchId, long auditUserId, string auditWorkstation)
        {
            return dataAccess.CreateRenewalDistributionBatches(renewalBatchId, auditUserId, auditWorkstation);
        }

        public byte[] GenerateRenewalFile(long renewalId,string userName, int languageId, long userId, string auditWorkstation)
        {
            Reports.RenewalFileReport runner = new Reports.RenewalFileReport();
            return runner.GenerateReport(renewalId, languageId, userName, userId, auditWorkstation);
        }

        public byte[] GenerateRenewalBatchReport(long renewalBatchId, string userName, int languageId, long userId, string auditWorkstation)
        {
            Reports.RenewalBatchReport runner = new Reports.RenewalBatchReport();
            return runner.GenerateReport(renewalBatchId, languageId, userName, userId, auditWorkstation);
        }

        public byte[] GenerateRenewalNewBatchReport(List<long> renewalBatchIds, string userName, int languageId, long userId, string auditWorkstation)
        {
            Reports.RenewalNewBatchReport runner = new Reports.RenewalNewBatchReport();
            return runner.GenerateReport(renewalBatchIds, languageId, userName, userId, auditWorkstation);
        }

        public bool CardPANMBRExists(string pan, int mbr)
        {
            return dataAccess.CardPANMBRExists(pan, mbr);
        }

        public bool RenewalPANMBRExists(string pan, int mbr)
        {
            return dataAccess.RenewalPANMBRExists(pan, mbr);
        }
    }
}
