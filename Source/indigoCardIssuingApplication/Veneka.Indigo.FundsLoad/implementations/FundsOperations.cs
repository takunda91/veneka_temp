using System;
using System.Collections.Generic;
using Veneka.Indigo.Common;
using Veneka.Indigo.FundsLoad.dal;

namespace Veneka.Indigo.FundsLoad
{
    public class FundsOperations : IFundsOperations
    {
        IFundsDataAccess dataAccess;

        public FundsOperations()
        {
            dataAccess = new FundsDataAccess();
        }

        public SystemResponseCode ApprovalAccept(long id, long auditUserId, string auditWorkstation)
        {
            SystemResponseCode result = dataAccess.ApprovalAccept(id, auditUserId, auditWorkstation);
            return result;
        }

        public SystemResponseCode ApprovalReject(long id, long auditUserId, string auditWorkstation)
        {
            SystemResponseCode result = dataAccess.ApprovalReject(id, auditUserId, auditWorkstation);
            return result;
        }

        public SystemResponseCode Create(FundsLoadModel fundsModel, long auditUserId, string auditWorkstation)
        {
            SystemResponseCode result = dataAccess.Save(fundsModel, auditUserId, auditWorkstation);
            return result;
        }

        public SystemResponseCode Delete(long id, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public ICollection<FundsLoadListModel> ListByCard(string prepaid_card_no, bool checkMasking, long auditUserId, string auditWorkStation)
        {
            var result = dataAccess.ListByCard(prepaid_card_no, checkMasking, auditUserId, auditWorkStation);
            return result;
        }

        public ICollection<FundsLoadListModel> List(FundsLoadStatusType statusType, int issuerId, int branchId, bool checkMasking, long auditUserId, string auditWorkStation)
        {
            var result = dataAccess.List(statusType,issuerId, branchId, checkMasking, auditUserId, auditWorkStation);
            return result;
        }

        public SystemResponseCode Load(long id, long auditUserId, string auditWorkstation)
        {
            SystemResponseCode result = dataAccess.Load(id, auditUserId, auditWorkstation);
            return result;
        }

        public FundsLoadListModel Retrieve(long fundsLoadId, bool checkMasking, long auditUserId, string auditWorkStation)
        {
            var result = dataAccess.Retrieve(fundsLoadId, checkMasking, auditUserId, auditWorkStation);
            return result;
        }

        public SystemResponseCode ReviewAccept(long id, long auditUserId, string auditWorkstation)
        {
            SystemResponseCode result = dataAccess.ReviewAccept(id, auditUserId, auditWorkstation);
            return result;
        }

        public SystemResponseCode ReviewReject(long id, long auditUserId, string auditWorkstation)
        {
            SystemResponseCode result = dataAccess.ReviewReject(id, auditUserId, auditWorkstation);
            return result;
        }

        public SystemResponseCode SendSMS(long id, long auditUserId, string auditWorkstation)
        {
            SystemResponseCode result = dataAccess.SendSMS(id, auditUserId, auditWorkstation);
            return result;
        }

        public ICollection<int> ProductsConfiguredForFundsLoad()
        {
            return dataAccess.ProductsConfiguredForFundsLoad();
        }
    }
}
