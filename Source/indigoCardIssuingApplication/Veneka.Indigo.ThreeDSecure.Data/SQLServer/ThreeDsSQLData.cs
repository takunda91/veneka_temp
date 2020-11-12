using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.ThreeDSecure.Data.Objects;

namespace Veneka.Indigo.ThreeDSecure.Data.SQLServer
{
   public class ThreeDsSQLData : I3DSDataAccess
    {
        private readonly string _connectionString;

        public ThreeDsSQLData(string connectionString)
        {
            _connectionString = connectionString;
       }

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
        public List<ThreeDSecureCardDetails> GetUnregisteredCards(int issuerId, string interfaceGuid, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            //List<ThreeDSecureCardDetails> cards = new List<ThreeDSecureCardDetails>();
            DataTable table = new DataTable();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_3ds_unregistered_cards]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@interface_guid", SqlDbType.NChar).Value = interfaceGuid;
                    //command.Parameters.Add("@config_id", SqlDbType.Int).Value = configId;
                    command.Parameters.Add("@check_masking", SqlDbType.Bit).Value = false;
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    con.Open();

                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(command))
                    {
                        da.Fill(table);
                    }

                    return ThreeDSecureCardDetails.FillObject(table);
                }
            }
        }

        public bool Update3DSecureBatchRegistered(long threedsBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_3ds_batch_registered]";

                    command.Parameters.Add("@threeds_batch_id", SqlDbType.BigInt).Value = threedsBatchId;
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    command.ExecuteNonQuery();

                    int resultCode = (int)command.Parameters["@ResultCode"].Value;

                    if (resultCode == 0)
                        return true;
                }
            }

            return false;
        }

        public List<ThreeDSecureCardDetails> GetCards(long threedBatchId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            DataTable table = new DataTable();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_3ds_batch_cards]";

                    command.Parameters.Add("@threed_batch_id", SqlDbType.BigInt).Value = threedBatchId;
                    command.Parameters.Add("@check_masking", SqlDbType.Bit).Value = false;
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    con.Open();

                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(command))
                    {
                        da.Fill(table);
                    }

                    return ThreeDSecureCardDetails.FillObject(table);
                }
            }
        }

        public List<ThreeDSecureBatch> GetRecreateBatches(int issuerId, string interfaceGuid, int languageId, long auditUserId, string auditWorkstation)
        {
            DataTable table = new DataTable();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_3ds_recreate_batches]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@interface_guid", SqlDbType.NChar).Value = interfaceGuid;
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    con.Open();

                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(command))
                    {
                        da.Fill(table);
                    }

                    return ThreeDSecureBatch.FillObject(table);
                }
            }
        }

        public bool UpdateBatchStatus(long threedsBatchId, int statusId, string statusNote, int languageId, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_threed_batch_status]";


                    command.Parameters.Add("@threed_batch_id", SqlDbType.BigInt).Value = threedsBatchId;
                    command.Parameters.Add("@threed_batch_statuses_id", SqlDbType.Int).Value = statusId;                    
                    command.Parameters.Add("@status_note", SqlDbType.VarChar).Value = statusNote;
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    command.ExecuteNonQuery();

                    int resultCode = (int)command.Parameters["@ResultCode"].Value;

                    if (resultCode == 0)
                        return true;
                }
            }

            return false;
        }

        public List<ThreeDSecureCardDetails> GetUploadedCardsCustomers(int? product_id)
        {
            //List<ThreeDSecureCardDetails> cards = new List<ThreeDSecureCardDetails>();
            DataTable table = new DataTable();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_create_xml_for_issued_cards]";

                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = product_id;
                    //command.Parameters.Add("@interface_guid", SqlDbType.NChar).Value = interfaceGuid;
                    //command.Parameters.Add("@config_id", SqlDbType.Int).Value = configId;
                    //command.Parameters.Add("@check_masking", SqlDbType.Bit).Value = false;
                    //command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    //command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    //command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    con.Open();

                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(command))
                    {
                        da.Fill(table);
                    }

                    return ThreeDSecureCardDetails.FillCustomersObject(table);
                }
            }
        }

        public List<int> GetIssuedProducts()
        {
            List<int> issuerIds = new List<int>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[show_issued_products]";

                    //command.Parameters.Add("@interface_type_id", SqlDbType.Int).Value = interfaceTypeId;
                    //command.Parameters.Add("@connection_parameter_id", SqlDbType.Int).Value = connectionParameterId;
                    //command.Parameters.Add("@interface_guid", SqlDbType.Char).Value = interfaceGuid;
                    //command.Parameters.Add("@interface_area", SqlDbType.Int).Value = interfaceArea;

                    //command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    //command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        issuerIds.Add((int)reader["product_id"]);
                    }
                }
            }

            return issuerIds;
        }

        public List<ThreeDSecureCardDetails> GetFileHeaderInfo(int product_id, int issuer_id)
        {
          //  DataTable table = new DataTable();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[get_xml_file_name_details]";

                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = product_id;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuer_id;
                    command.Parameters.Add("@product_prefix", SqlDbType.NVarChar,50).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@issuer_code", SqlDbType.VarChar,10).Direction = ParameterDirection.Output;
                    //command.Parameters.Add("@config_id", SqlDbType.Int).Value = configId;
                    //command.Parameters.Add("@check_masking", SqlDbType.Bit).Value = false;
                    //command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    //command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    //command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    con.Open();
                    command.ExecuteNonQuery();

                    return ThreeDSecureCardDetails.FillFileHeaderDetails((string)command.Parameters["@issuer_code"].Value, (string)command.Parameters["@product_prefix"].Value);
                }
            }
        }



    }
}
