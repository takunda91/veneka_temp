using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.DataSource.LocalDAL
{
    public class IssuerDAL : IIssuerDAL
    {
        private readonly string _connectionString;

        public IssuerDAL()
        {
            this._connectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
        }

        /// <summary>
        /// Find all active products linked to an issuer which match on a 6 digit BIN.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="bin"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public Issuer GetIssuer(int issuerId, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_issuer]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new Issuer(reader);                            
                        }
                    }
                }
            }

            return null;
        }
    }
}
