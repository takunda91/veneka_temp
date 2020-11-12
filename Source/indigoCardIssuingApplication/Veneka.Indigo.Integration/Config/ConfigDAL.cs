using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Veneka.Indigo.Integration.Objects;
using System.Data.SqlClient;
using System.Data;
using Veneka.Indigo.Common;
using System.Runtime.Remoting.Messaging;

namespace Veneka.Indigo.Integration.Config
{
    public class ConfigDAL
    {
        private readonly string connectionString;

        public ConfigDAL(string ConnectionString)
        {
            this.connectionString = ConnectionString;
        }
        
        /// <summary>
        /// Fetchs a single parameter
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceTypeId"></param>
        /// <param name="interfaceArea"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public IConfig GetIssuerInterfaceConfig(int issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            var parameters = this.GetIssuerInterfaceConfigs(issuerId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);

            if (parameters.Count > 0)
                return parameters.First();

            return null;
        }

        /// <summary>
        /// Fetch a list of parameters.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceTypeId"></param>
        /// <param name="interfaceArea"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public List<IConfig> GetIssuerInterfaceConfigs(int? issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_parameters_issuer_interface]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@interface_type_id", SqlDbType.Int).Value = interfaceTypeId;
                    command.Parameters.Add("@interface_area", SqlDbType.Int).Value = interfaceArea;
                    command.Parameters.Add("@interface_guid", SqlDbType.Char).Value = interfaceGuid;
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

            return ConfigFactory.GetConfigs(table);
        }

        /// <summary>
        /// Fetchs a single parameter
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceTypeId"></param>
        /// <param name="interfaceArea"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public IConfig GetProductInterfaceConfig(int productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            var parameters = this.GetProductInterfaceConfigs(productId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);

            if (parameters.Count > 0)
                return parameters.First();

            return null;
        }

        /// <summary>
        /// Fetch a list of parameters.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceTypeId"></param>
        /// <param name="interfaceArea"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public List<IConfig> GetProductInterfaceConfigs(int? productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_parameters_product_interface]";

                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@interface_type_id", SqlDbType.Int).Value = interfaceTypeId;
                    command.Parameters.Add("@interface_area", SqlDbType.Int).Value = interfaceArea;
                    command.Parameters.Add("@interface_guid", SqlDbType.Char).Value = interfaceGuid;
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

            return ConfigFactory.GetConfigs(table);
        }

        public List<IConfig> GetProductInterfaceConfigsByIssuer(int issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_parameters_product_interface_by_issuer]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@interface_type_id", SqlDbType.Int).Value = interfaceTypeId;
                    command.Parameters.Add("@interface_area", SqlDbType.Int).Value = interfaceArea;
                    command.Parameters.Add("@interface_guid", SqlDbType.Char).Value = interfaceGuid;
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

            return ConfigFactory.GetConfigs(table);
        }

        /// <summary>
        /// Fetch issuers that use
        /// </summary>
        /// <param name="interfaceTypeId"></param>
        /// <param name="interfaceArea"></param>
        /// <param name="interfaceGuid"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public List<int> GetIssuersForInterface(int? interfaceTypeId, int? connectionParameterId, string interfaceGuid, int? interfaceArea, long auditUserId, string auditWorkStation)
        {
            List<int> issuerIds = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_issuers by_product_interfaces]";

                    command.Parameters.Add("@interface_type_id", SqlDbType.Int).Value = interfaceTypeId;
                    command.Parameters.Add("@connection_parameter_id", SqlDbType.Int).Value = connectionParameterId;
                    command.Parameters.Add("@interface_guid", SqlDbType.Char).Value = interfaceGuid;
                    command.Parameters.Add("@interface_area", SqlDbType.Int).Value = interfaceArea;               

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();                    
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        issuerIds.Add((int)reader["issuer_id"]);
                    }
                }
            }

            return issuerIds;
        }

        public SystemResponseCode usd_create_pin_mailer(string pin_block, string mask_pan, string product_name, string product_bin_code, string pan_last_four, DateTime expiry_period,string card_id, string header_reference)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_create_pin_mailer]";

                    command.Parameters.Add("@pin_block", SqlDbType.VarChar).Value = pin_block;
                    command.Parameters.Add("@mask_pan", SqlDbType.VarChar).Value = mask_pan;
                    command.Parameters.Add("@prod_name", SqlDbType.VarChar).Value = product_name;
                    command.Parameters.Add("@product_bin_code", SqlDbType.NChar).Value = product_bin_code;
                    command.Parameters.Add("@pan_last_four", SqlDbType.NChar).Value = pan_last_four;
                    command.Parameters.Add("@expiry_period", SqlDbType.DateTime).Value = expiry_period;
                    command.Parameters.Add("@card_id", SqlDbType.VarChar).Value = card_id;
                    command.Parameters.Add("@header_reference", SqlDbType.VarChar).Value = header_reference;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }

            //int resultCode = int.Parse(ResultCode.Value.ToString());

            //return (SystemResponseCode)resultCode;
        }

        public SystemResponseCode usd_create_pin_link(string card_id, string dummy_pan)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_create_pin_link]";

                    command.Parameters.Add("@card_id", SqlDbType.VarChar).Value = card_id;
                    command.Parameters.Add("@dummy_pan", SqlDbType.VarChar).Value = dummy_pan;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }

        }

        public SystemResponseCode usp_create_pin_file_batch_header(string header_ref)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_create_pin_file_batch_header]";

                    command.Parameters.Add("@batch_reference", SqlDbType.VarChar).Value = header_ref;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }

        }
    }
}
