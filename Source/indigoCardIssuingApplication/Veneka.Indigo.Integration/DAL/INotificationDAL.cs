using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.DAL
{
    public interface INotificationDAL
    {
        List<Notification> GetBranchOutbox(long auditUserId, string auditWorkStation);
        void LogBranchNotifications(List<Notification> notifications, long auditUserId, string auditWorkStation);
        List<Notification> GetBatchOutbox(long auditUserId, string auditWorkStation);

        DataTable GetBatchUserList(int issuerId, int user_role_id, int branchid, int dist_batch_type_id, long auditUserId, string auditWorkStation);
    }
}
