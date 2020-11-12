using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.DataSource.LocalDAL
{
    public class ExportBatchDAL : IExportBatchDAL
    {
        private readonly string _connectionString = String.Empty;

        public ExportBatchDAL()
        {
            this._connectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
        }
        
        public ExportBatchGeneration GenerateBatches(int? issuerId, long auditUserId, string auditWorkStation)
        {
            ExportBatchGeneration rtn = new ExportBatchGeneration();
            var exportBatchIds = new List<int>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_create_export_batches]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkStation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            exportBatchIds.Add(int.Parse(dataReader["export_batch_id"].ToString()));                            
                        }
                    }

                    rtn.ExportBatchIds = exportBatchIds.ToArray();
                    rtn.ResultId = int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                    return rtn;
                }
            }
        }

        public int StatusChangeExported(long exportBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_export_batch_status_exported]";

                    command.Parameters.Add("@export_batch_id", SqlDbType.BigInt).Value = exportBatchId;
                    command.Parameters.Add("@language_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkStation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    connection.Open();
                    command.ExecuteNonQuery();                    

                    return int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }
        }

        public Dictionary<long, string> FindExportBatches(int issuerId, int? productId, int exportBatchStatusesId, int languageId, long auditUserId, string auditWorkstation)
        {
            Dictionary<long, string> exportBatches = new Dictionary<long, string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_search_export_batches]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@export_batch_statuses_id", SqlDbType.Int).Value = exportBatchStatusesId;
                    command.Parameters.Add("@date_from", SqlDbType.DateTime2).Value = null;
                    command.Parameters.Add("@date_to", SqlDbType.DateTime2).Value = null;

                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkstation;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exportBatches.Add(long.Parse(reader["export_batch_id"].ToString()), reader["batch_reference"].ToString());
                        }
                    }
                }
            }

            return exportBatches;
        }

        public List<CardObject> FetchCardObjectsForExportBatch(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            List<CardObject> cardObjects = new List<CardObject>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_export_batch_card_details]";

                    command.Parameters.Add("@export_batch_id", SqlDbType.Int).Value = exportBatchId;

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkstation;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {  
                            cardObjects.Add(new CardObject(reader));
                        }
                    }

                    return cardObjects;
                }
            }
        }
    }
}
