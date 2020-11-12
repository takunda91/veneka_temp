using System.Collections.Generic;
using Veneka.Indigo.Common;

namespace Veneka.Indigo.FundsLoad.dal
{
    public interface IFundsDataAccess
    {
        SystemResponseCode Save(FundsLoadModel fundsModel, long auditUserId, string auditWorkStation);
        ICollection<FundsLoadListModel> ListByCard(string cardNumber, bool checkMasking, long auditUserId, string auditWorkStation);
        ICollection<FundsLoadListModel> List(FundsLoadStatusType statusType, int issuerId, int branchId, bool checkMasking, long auditUserId, string auditWorkStation);

        SystemResponseCode ReviewAccept(long id, long auditUserId, string auditWorkstation);
        SystemResponseCode ReviewReject(long id, long auditUserId, string auditWorkstation);

        SystemResponseCode ApprovalAccept(long id, long auditUserId, string auditWorkstation);
        SystemResponseCode ApprovalReject(long id, long auditUserId, string auditWorkstation);

        SystemResponseCode Load(long id, long auditUserId, string auditWorkstation);

        SystemResponseCode Delete(long id, long auditUserId, string auditWorkstation);

        SystemResponseCode SendSMS(long id, long auditUserId, string auditWorkstation);

        FundsLoadListModel Retrieve(long fundsLoadId, bool checkMasking, long auditUserId, string auditWorkStation);

        ICollection<int> ProductsConfiguredForFundsLoad();
    }
}
