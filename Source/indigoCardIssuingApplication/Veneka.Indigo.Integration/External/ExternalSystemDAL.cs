using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.External
{
    public class ExternalSystemDAL
    {
        private readonly string connectionString;

        public ExternalSystemDAL(string ConnectionString)
        {
            this.connectionString = ConnectionString;
        }

        public ExternalSystemFields GetExternalSystemFields(int externalSystemTypeId, int productId, int languageId, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_product_external_system_fields]";

                    command.Parameters.Add("@external_system_type_id", SqlDbType.Int).Value = externalSystemTypeId;
                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
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

            ExternalSystemFields sysFields = new ExternalSystemFields();
            sysFields.Field = new Dictionary<string, string>();

            foreach (DataRow row in table.Rows)
            {
                sysFields.Field.Add(row.Field<string>("field_name"),
                                    row.Field<string>("field_value"));
            }

            return sysFields;
        }
    }
}
