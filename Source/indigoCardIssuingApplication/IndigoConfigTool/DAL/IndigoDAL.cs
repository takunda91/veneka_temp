using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace IndigoConfigTool.DAL
{
    public class IndigoDAL
    {
        protected string ConnectionString { get; private set; }        

        protected IndigoDAL(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        protected int InjectZMK(int issuerId, string zmk, string key)
        {
            string cmd = "OPEN SYMMETRIC KEY key_injection_keys " +
                         "DECRYPTION BY CERTIFICATE cert_ZoneMasterKeys; " +
                         "  INSERT INTO zone_keys(issuer_id, zone, final) " +
                         "  VALUES(@issuer_id, " +
                         "  ENCRYPTBYKEY(KEY_GUID('key_injection_keys'), CONVERT(varchar(max), @zone)), " +
                         "  ENCRYPTBYKEY(KEY_GUID('key_injection_keys'), CONVERT(varchar(max), @final))) " +
                         "CLOSE SYMMETRIC KEY key_injection_keys;";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = cmd;

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@zone", SqlDbType.VarChar).Value = zmk;
                    command.Parameters.Add("@final", SqlDbType.VarChar).Value = key;

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}
