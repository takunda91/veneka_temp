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
    public class TerminalDAL : ITerminalDAL
    {
        private string connectionString;

        public TerminalDAL()
        {
            this.connectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
        }        

        public TerminalMK GetTerminalMasterKey(string deviceId, long auditUserId, string auditWorkStation)
        {
            TerminalMK rtn = new TerminalMK();
            rtn.IssuerID = 0;
            rtn.Key = String.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_tmk_by_terminal]";

                    command.Parameters.Add("@device_id", SqlDbType.VarChar).Value = deviceId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            rtn.IssuerID = (int)dataReader["issuer_id"];
                            rtn.Key = dataReader["masterkey"].ToString();
                            
                            return rtn;
                        }
                    }
                }
            }
                        
            return rtn;
        }

        public ZoneMasterKey GetZoneMasterKey(int issuerId, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_zone_key]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            return new ZoneMasterKey(dataReader["zone"].ToString(),
                                                     dataReader["final"].ToString());
                        }
                    }
                }
            }

            return null;
        }
    }
}
