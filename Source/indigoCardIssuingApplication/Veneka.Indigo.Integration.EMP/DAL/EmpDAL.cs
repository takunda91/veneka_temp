using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.EMP.DAL
{
    internal class EmpDAL
    {
        private string connectionString;

        public EmpDAL(string ConnectionString)
        {
            this.connectionString = ConnectionString;
        }

        /// <summary>
        /// Updates the cards with their newly generated card numbers as well as updating the last sequence number for each product used.
        /// </summary>
        /// <param name="cards">Dictionary[CardId, CardNumber]</param>
        /// <param name="products">Dictionary[ProductId, SequenceNumber]</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        public void UpdatePAN(long cardId, string pan, DateTime expiry, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_pan]";

                    command.Parameters.Add("@card_id", SqlDbType.BigInt).Value = cardId;
                    command.Parameters.Add("@pan", SqlDbType.VarChar).Value = pan;
                    command.Parameters.Add("@expiry_date", SqlDbType.DateTime2).Value = expiry;

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkStation;

                    command.ExecuteNonQuery();                    
                }
            }
        }
    }
}
