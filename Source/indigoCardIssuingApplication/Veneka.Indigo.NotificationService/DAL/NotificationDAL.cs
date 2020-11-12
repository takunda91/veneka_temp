using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.NotificationService.DAL
{
    public class NotificationDAL: INotificationDAL
    {
        private readonly string _connectionString;

        public NotificationDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Get messages that are waiting to be sent.
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public List<Notification> GetBranchOutbox(long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_notifications_branch_outbox]";
                    
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();

                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(command))
                    {
                        da.Fill(table);
                    }
                }
            }

            List<Notification> notifications = new List<Notification>();

            foreach (DataRow row in table.Rows)
                notifications.Add(new Notification(row, NotificationType.Branch));

            return notifications;
        }

        /// <summary>
        /// Logs only successfully sent messages
        /// </summary>
        /// <param name="notifications"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        public void LogBranchNotifications(List<Notification> notifications, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DataTable dt_notifications = new DataTable();
                    dt_notifications.Columns.Add("message_id", typeof(Guid));
                    dt_notifications.Columns.Add("message_text", typeof(String));
                    DataRow workRow;

                    foreach (var item in notifications.Where(w => w.IsSetn == true))
                    {
                        workRow = dt_notifications.NewRow();
                        workRow["message_id"] = item.MessageId;
                        workRow["message_text"] = item.Text;
                        dt_notifications.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_notifications_branch_log]";

                    command.Parameters.AddWithValue("@message_list", dt_notifications);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }
        //[usp_notifications_branch_log]

        #region Batch Notifications
        public List<Notification> GetBatchOutbox(long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_notifications_batch_outbox]";

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();

                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(command))
                    {
                        da.Fill(table);
                    }
                }
            }

            List<Notification> notifications = new List<Notification>();

            foreach (DataRow row in table.Rows)
            {
                //Get the users to send too
                foreach(DataRow user in GetBatchUserList(row.Field<int>("issuer_id"), row.Field<int>("user_role_id"), row.Field<int>("branch_id"), row.Field<int>("dist_batch_type_id"),auditUserId, auditWorkStation).Rows)
                    notifications.Add(new Notification(row, user, NotificationType.Batch));
            }

            return notifications;
        }

        public DataTable GetBatchUserList(int issuerId, int user_role_id,int branchid,int dist_batch_type_id, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_notifications_batch_userlist]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@user_role_id", SqlDbType.Int).Value = user_role_id;
                    command.Parameters.Add("@dist_batch_type_id", SqlDbType.Int).Value = dist_batch_type_id;
                    command.Parameters.Add("@branch_id", SqlDbType.Int).Value = branchid;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();

                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(command))
                    {
                        da.Fill(table);
                    }
                }
            }

            return table;
        }

        /// <summary>
        /// Logs only successfully sent messages
        /// </summary>
        /// <param name="notifications"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        public void LogBatchNotifications(List<Notification> notifications, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DataTable dt_notifications = new DataTable();
                    dt_notifications.Columns.Add("message_id", typeof(Guid));
                    dt_notifications.Columns.Add("message_text", typeof(String));
                    DataRow workRow;

                    foreach (var item in notifications.Where(w => w.IsSetn == true))
                    {
                        workRow = dt_notifications.NewRow();
                        workRow["message_id"] = item.MessageId;
                        workRow["message_text"] = item.Text;
                        dt_notifications.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_notifications_batch_log]";

                    command.Parameters.AddWithValue("@message_list", dt_notifications);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }
        #endregion
    }
}
