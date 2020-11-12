using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.IssuerManagement.objects;

namespace Veneka.Indigo.IssuerManagement.dal
{
    public class NotificationDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;


        #region"BATCH NOTIFICATION CURD OPERATIONS "

        public SystemResponseCode InsertNotificationforBatch(NotificationMessages notifications, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(_dbObject.SQLConnectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DataTable dt_notifications = new DataTable();
                    dt_notifications.Columns.Add("language_id", typeof(Int16));
                    dt_notifications.Columns.Add("channel_id", typeof(Int16));
                    dt_notifications.Columns.Add("notification_text", typeof(String));
                    dt_notifications.Columns.Add("subject_text", typeof(String));
                    dt_notifications.Columns.Add("from_address", typeof(String));
                    DataRow workRow;

                    foreach (var item in notifications.messages)
                    {
                        workRow = dt_notifications.NewRow();
                        workRow["language_id"] = item.language_id;
                        workRow["channel_id"] = 0;

                        workRow["notification_text"] = item.notification_text;
                        workRow["subject_text"] = item.subject_text;
                        workRow["from_address"] = item.from_address;

                        dt_notifications.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_insert_notification_batch]";

                    command.Parameters.AddWithValue("@issuer_id", notifications.issuerid);
                    command.Parameters.AddWithValue("@dist_batch_type_id", notifications.distbatchtypeid);
                    command.Parameters.AddWithValue("@dist_batch_statuses_id", notifications.distbatchstatusesid);
                    command.Parameters.AddWithValue("@channel_id", notifications.channel_id);

                    command.Parameters.AddWithValue("@notifications_lang_messages", dt_notifications);

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    connection.Open();

                    command.ExecuteNonQuery();
                    int resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                    return (SystemResponseCode)resultCode;
                }

            }
        }

        public SystemResponseCode UpdateNotificationforBatch(NotificationMessages notifications, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(_dbObject.SQLConnectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DataTable dt_notifications = new DataTable();
                    dt_notifications.Columns.Add("language_id", typeof(Int16));
                    dt_notifications.Columns.Add("channel_id", typeof(Int16));

                    dt_notifications.Columns.Add("notification_text", typeof(String));
                    dt_notifications.Columns.Add("subject_text", typeof(String));
                    dt_notifications.Columns.Add("from_address", typeof(String));

                    DataRow workRow;

                    foreach (var item in notifications.messages)
                    {
                        workRow = dt_notifications.NewRow();
                        workRow["language_id"] = item.language_id;
                        workRow["channel_id"] = 0;

                        workRow["notification_text"] = item.notification_text;
                        workRow["subject_text"] = item.subject_text;
                        workRow["from_address"] = item.from_address;

                        dt_notifications.Rows.Add(workRow);
                    }
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_notification_batch]";

                    command.Parameters.AddWithValue("@issuer_id", notifications.issuerid);
                    command.Parameters.AddWithValue("@dist_batch_type_id", notifications.distbatchtypeid);
                    command.Parameters.AddWithValue("@dist_batch_statuses_id", notifications.distbatchstatusesid);
                    command.Parameters.AddWithValue("@channel_id", notifications.channel_id);

                    command.Parameters.AddWithValue("@notifications_lang_messages", dt_notifications);

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    connection.Open();

                    command.ExecuteNonQuery();
                    int resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                    return (SystemResponseCode)resultCode;
                }

            }
        }

        internal List<notification_batchResult> GetNotificationBatch(NotificationMessages messasge, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_notification_batch(messasge.issuerid, messasge.distbatchtypeid, messasge.distbatchstatusesid, messasge.channel_id);
                return result.ToList();
            }
        }

        internal List<notification_batch_ListResult> ListNotificationBatches(int issuerid, int pageIndex, int rowsperpage)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_notification_batch_List(issuerid, pageIndex, rowsperpage);
                return result.ToList();
            }
        }
        internal void DeleteNotificationBatch(NotificationMessages message, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_delete_notification_batch(message.issuerid, message.distbatchtypeid, message.distbatchstatusesid, message.channel_id, auditUserId, auditWorkstation);
            }
        }
        #endregion

        #region"BRANCH NOTIFICATION CURD OPERATIONS "

        public SystemResponseCode InsertNotificationforBranch(NotificationMessages notifications, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(_dbObject.SQLConnectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DataTable dt_notifications = new DataTable();
                    dt_notifications.Columns.Add("language_id", typeof(Int16));
                    dt_notifications.Columns.Add("channel_id", typeof(Int16));

                    dt_notifications.Columns.Add("notification_text", typeof(String));
                    dt_notifications.Columns.Add("subject_text", typeof(String));
                    dt_notifications.Columns.Add("from_address", typeof(String));
                    DataRow workRow;

                    foreach (var item in notifications.messages)
                    {
                        workRow = dt_notifications.NewRow();
                        workRow["language_id"] = item.language_id;
                        workRow["channel_id"] = 0;

                        workRow["notification_text"] = item.notification_text;
                        workRow["subject_text"] = item.subject_text;
                        workRow["from_address"] = item.from_address;

                        dt_notifications.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_insert_notification_branch]";

                    command.Parameters.AddWithValue("@issuer_id", notifications.issuerid);
                    command.Parameters.AddWithValue("@branch_card_statuses_id", notifications.branchcardstatusesid);
                    command.Parameters.AddWithValue("@card_issue_method_id", notifications.cardissuemethodid);
                    command.Parameters.AddWithValue("@channel_id", notifications.channel_id);

                    command.Parameters.AddWithValue("@notifications_lang_messages", dt_notifications);

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    connection.Open();

                    command.ExecuteNonQuery();
                    int resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                    return (SystemResponseCode)resultCode;
                }

            }
        }

        public SystemResponseCode UpdateNotificationforBranch( NotificationMessages notifications, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(_dbObject.SQLConnectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    DataTable dt_notifications = new DataTable();
                    dt_notifications.Columns.Add("language_id", typeof(Int16));
                    dt_notifications.Columns.Add("channel_id", typeof(Int16));
                   

                    dt_notifications.Columns.Add("notification_text", typeof(String));                  
                    dt_notifications.Columns.Add("subject_text", typeof(String));
                    dt_notifications.Columns.Add("from_address", typeof(String));
                    DataRow workRow;

                    foreach (var item in notifications.messages)
                    {
                        workRow = dt_notifications.NewRow();
                        workRow["language_id"] = item.language_id;
                        workRow["channel_id"] = 0;

                        workRow["notification_text"] = item.notification_text;
                        workRow["subject_text"] = item.subject_text;
                        workRow["from_address"] = item.from_address;

                        dt_notifications.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_notification_branch]";

                    command.Parameters.AddWithValue("@issuer_id", notifications.issuerid);
                    command.Parameters.AddWithValue("@branch_card_statuses_id", notifications.branchcardstatusesid);
                    command.Parameters.AddWithValue("@card_issue_method_id", notifications.cardissuemethodid);
                    command.Parameters.AddWithValue("@channel_id", notifications.channel_id);

                    command.Parameters.AddWithValue("@notifications_lang_messages", dt_notifications);

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    connection.Open();

                    command.ExecuteNonQuery();
                    int resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                    return (SystemResponseCode)resultCode;
                }

            }
        }

        internal List<notification_branchResult> GetNotificationBranch(NotificationMessages message, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_notification_branch(message.issuerid, message.branchcardstatusesid, message.cardissuemethodid, message.channel_id);
                return result.ToList();
            }
        }

        internal List<notification_branch_ListResult> ListNotificationBraches(int issuerid, int pageIndex, int rowsperpage)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_notification_branch_List(issuerid, pageIndex, rowsperpage);
                return result.ToList();
            }
        }
        internal void DeleteNotificationBranch(NotificationMessages message, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_delete_notification_branch(message.issuerid, message.branchcardstatusesid, message.cardissuemethodid, message.channel_id, auditUserId, auditWorkstation);
            }
        }
        #endregion

    }
}
