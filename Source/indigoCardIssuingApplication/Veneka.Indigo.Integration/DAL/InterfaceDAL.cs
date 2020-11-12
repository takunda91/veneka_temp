using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.DAL
{
    public class InterfaceDAL
    {
        private readonly string _connectionString = String.Empty;

        public InterfaceDAL(string ConnectionString)
        {
            this._connectionString = ConnectionString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceTypeId"></param>
        /// <param name="interfaceAreaId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public Dictionary<int, Guid> GetIssuerInterface(int? issuerId, int interfaceTypeId, int interfaceAreaId, long auditUserId, string auditWorkStation)
        {
            Dictionary<int, Guid> issuersAndInterface = new Dictionary<int, Guid>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_issuer_interfaces]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@interface_type_id", SqlDbType.Int).Value = interfaceTypeId;
                    command.Parameters.Add("@interface_area", SqlDbType.Int).Value = interfaceAreaId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            if (!String.IsNullOrWhiteSpace(dataReader["interface_guid"].ToString()))
                            {
                                string gd = dataReader["interface_guid"].ToString();
                                issuersAndInterface.Add(int.Parse(dataReader["issuer_id"].ToString()), Guid.Parse(gd));
                                                            //Guid.Parse(dataReader["interface_guid"].ToString()));
                            }                           
                        }
                    }

                }
            }

            return issuersAndInterface;
        }
    }
}
