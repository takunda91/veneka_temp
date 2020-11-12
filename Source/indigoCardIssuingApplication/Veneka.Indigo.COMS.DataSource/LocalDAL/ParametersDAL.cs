using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Objects;
using System.Data.SqlClient;
using System.Data;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.COMS.DataSource.LocalDAL
{
    public class ParametersDAL : Veneka.Indigo.Integration.DAL.IParametersDAL
    {
        private readonly string connectionString;

        public ParametersDAL()
        {
            this.connectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
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
        public Parameters GetParameterIssuerInterface(int issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            var parameters = this.GetParametersIssuerInterface(issuerId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);

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
        public List<Parameters> GetParametersIssuerInterface(int? issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
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

                    List<Parameters> p = new List<Parameters>();

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? timeout = null;
                            int? bufferSize = null;
                            Veneka.Indigo.Integration.Objects.Parameters.protocol? proto = null;
                            Veneka.Indigo.Integration.Objects.Parameters.authType? auth = null;
                            string username = String.Empty;
                            string password = String.Empty;
                            bool? deleteFile = null;
                            int? fileEncryption = null;
                            bool? duplicateCheck = null;

                            if (reader["timeout_milli"] != null && !string.IsNullOrWhiteSpace(reader["timeout_milli"].ToString()))
                                timeout = int.Parse(reader["timeout_milli"].ToString());

                            if (reader["buffer_size"] != null && !string.IsNullOrWhiteSpace(reader["buffer_size"].ToString()))
                                bufferSize = int.Parse(reader["buffer_size"].ToString());

                            if (reader["protocol"] != null && !string.IsNullOrWhiteSpace(reader["protocol"].ToString()))
                                 proto = (Veneka.Indigo.Integration.Objects.Parameters.protocol)Enum.Parse(typeof(Veneka.Indigo.Integration.Objects.Parameters.protocol), reader["protocol"].ToString());

                            if (reader["auth_type"] != null && !string.IsNullOrWhiteSpace(reader["auth_type"].ToString()))
                                auth = (Veneka.Indigo.Integration.Objects.Parameters.authType)int.Parse(reader["auth_type"].ToString());

                            if (reader["username"] != null && !string.IsNullOrWhiteSpace(reader["username"].ToString()))
                                username = reader["username"].ToString();

                            if (reader["password"] != null && !string.IsNullOrWhiteSpace(reader["password"].ToString()))
                                password = reader["password"].ToString();

                            if (reader["file_delete_YN"] != null && !string.IsNullOrWhiteSpace(reader["file_delete_YN"].ToString()))
                                deleteFile = bool.Parse(reader["file_delete_YN"].ToString());

                            if (reader["file_encryption_type_id"] != null && !string.IsNullOrWhiteSpace(reader["file_encryption_type_id"].ToString()))
                                fileEncryption = int.Parse(reader["file_encryption_type_id"].ToString());

                            if (reader["duplicate_file_check_YN"] != null && !string.IsNullOrWhiteSpace(reader["duplicate_file_check_YN"].ToString()))
                                duplicateCheck = bool.Parse(reader["duplicate_file_check_YN"].ToString());

                            p.Add(new Parameters(0,
                                                reader["path"].ToString(),
                                                reader["name_of_file"].ToString(),
                                                deleteFile ?? false,
                                                fileEncryption ?? 1,
                                                duplicateCheck ?? false,
                                                reader["address"].ToString(),
                                                reader["port"] != null && !string.IsNullOrWhiteSpace(reader["port"].ToString()) ? int.Parse(reader["port"].ToString()) : 0,
                                                proto, 
                                                auth, 
                                                username, 
                                                password,
                                                reader["header_length"] != null && !string.IsNullOrWhiteSpace(reader["header_length"].ToString()) ? int.Parse(reader["header_length"].ToString()) : 0,
                                                reader["identification"].ToString(),
                                                timeout,
                                                bufferSize,
                                                reader["doc_type"] != null && !string.IsNullOrWhiteSpace(reader["doc_type"].ToString()) ? char.Parse(reader["doc_type"].ToString()) : ' ',
                                                reader["private_key"].ToString(), reader["public_key"].ToString()));

                        }
                    }

                    return p;
                }
            }
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
        public Parameters GetParameterProductInterface(int productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            var parameters = this.GetParametersProductInterface(productId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);

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
        public List<Parameters> GetParametersProductInterface(int? productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
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

                    List<Parameters> p = new List<Parameters>();

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? timeout = null;
                            int? bufferSize = null;
                            Veneka.Indigo.Integration.Objects.Parameters.protocol? proto = null;
                            Veneka.Indigo.Integration.Objects.Parameters.authType? auth = null;
                            string username = String.Empty;
                            string password = String.Empty;
                            bool? deleteFile = null;
                            int? fileEncryption = null;
                            bool? duplicateCheck = null;

                            if (reader["timeout_milli"] != null && !string.IsNullOrWhiteSpace(reader["timeout_milli"].ToString()))
                                timeout = int.Parse(reader["timeout_milli"].ToString());

                            if (reader["buffer_size"] != null && !string.IsNullOrWhiteSpace(reader["buffer_size"].ToString()))
                                bufferSize = int.Parse(reader["buffer_size"].ToString());

                            if (reader["protocol"] != null && !string.IsNullOrWhiteSpace(reader["protocol"].ToString()))
                                proto = (Veneka.Indigo.Integration.Objects.Parameters.protocol)Enum.Parse(typeof(Veneka.Indigo.Integration.Objects.Parameters.protocol), reader["protocol"].ToString());

                            if (reader["auth_type"] != null && !string.IsNullOrWhiteSpace(reader["auth_type"].ToString()))
                                auth = (Veneka.Indigo.Integration.Objects.Parameters.authType)int.Parse(reader["auth_type"].ToString());

                            if (reader["username"] != null && !string.IsNullOrWhiteSpace(reader["username"].ToString()))
                                username = reader["username"].ToString();

                            if (reader["password"] != null && !string.IsNullOrWhiteSpace(reader["password"].ToString()))
                                password = reader["password"].ToString();

                            if (reader["file_delete_YN"] != null && !string.IsNullOrWhiteSpace(reader["file_delete_YN"].ToString()))
                                deleteFile = bool.Parse(reader["file_delete_YN"].ToString());

                            if (reader["file_encryption_type_id"] != null && !string.IsNullOrWhiteSpace(reader["file_encryption_type_id"].ToString()))
                                fileEncryption = int.Parse(reader["file_encryption_type_id"].ToString());

                            if (reader["duplicate_file_check_YN"] != null && !string.IsNullOrWhiteSpace(reader["duplicate_file_check_YN"].ToString()))
                                duplicateCheck = bool.Parse(reader["duplicate_file_check_YN"].ToString());

                            p.Add(new Parameters(0,
                                                reader["path"].ToString(),
                                                reader["name_of_file"].ToString(),
                                                deleteFile ?? false,
                                                fileEncryption ?? 1,
                                                duplicateCheck ?? false,
                                                reader["address"].ToString(),
                                                reader["port"] != null && !string.IsNullOrWhiteSpace(reader["port"].ToString()) ? int.Parse(reader["port"].ToString()) : 0,
                                                proto, auth, username, password,
                                                reader["header_length"] != null && !string.IsNullOrWhiteSpace(reader["header_length"].ToString()) ? int.Parse(reader["header_length"].ToString()) : 0,
                                                reader["identification"].ToString(),
                                                timeout,
                                                bufferSize,
                                                reader["doc_type"] != null && !string.IsNullOrWhiteSpace(reader["doc_type"].ToString()) ? char.Parse(reader["doc_type"].ToString()) : ' ',
                                                reader["private_key"].ToString(), reader["public_key"].ToString()));

                        }
                    }

                    return p;
                }
            }
        }
    }
}
