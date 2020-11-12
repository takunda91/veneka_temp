using System;
using System.Data.SqlClient;
using System.Text;
using Veneka.Indigo.Common.Database;

namespace Veneka.Indigo.Common
{
    public sealed class CommonGeneral
    {
        public static DBResponseMessage ValidateDBResponse(string strResponse)
        {
            int intResponse = 99;
            Int32.TryParse(strResponse, out intResponse);

            switch (intResponse)
            {
                case 0:
                    return DBResponseMessage.SUCCESS;
                case 1:
                    return DBResponseMessage.INCORRECT_STATUS;
                case 2:
                    return DBResponseMessage.CARD_ALREADY_ISSUED;
                case 3:
                    return DBResponseMessage.INCORRECT_BRANCH;
                case 4:
                    return DBResponseMessage.NO_RECORDS_RETURNED;
                case 97:
                    return DBResponseMessage.SPROC_ERROR;
                case 98:
                    return DBResponseMessage.SYSTEM_ERROR;
                case 99:
                    return DBResponseMessage.FAILURE;
                default:
                    return DBResponseMessage.FAILURE;
            }
        }

        public static string Storage_DecryptValue(byte[] cipherValue)
        {
            DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;
            String clearValue = "";
            String command = "EXEC usp_sys_decrypt @cipheredText";

            using (SqlConnection connection = _dbObject.SQLConnection)
            {
                using (var sqlCommand = new SqlCommand(command, connection))
                {
                    sqlCommand.Parameters.AddWithValue("cipheredText", cipherValue);
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        clearValue = reader[0] != null ? reader[0].ToString() : "ERROR DURING DECIHPER";

                        break;
                    }
                }
            }


            return clearValue;
        }

        public static string Storage_EncryptValue(string originalValue)
        {
            DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;
            String encryptedValue = "";

            string command = "EXEC usp_sys_encrypt @clearText";

            using (SqlConnection connection = _dbObject.SQLConnection)
            {
                using (var sqlCommand = new SqlCommand(command, connection))
                {
                    sqlCommand.Parameters.AddWithValue("clearText", originalValue);
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        byte[] result = reader[0] != null
                                            ? (byte[]) reader[0]
                                            : Encoding.ASCII.GetBytes("ERROR DURING CYPHER");

                        encryptedValue = Convert.ToBase64String(result); //System.Text.Encoding.ASCII.GetString(result);
                        break;
                    }
                }
            }

            return encryptedValue;
        }
    }
}