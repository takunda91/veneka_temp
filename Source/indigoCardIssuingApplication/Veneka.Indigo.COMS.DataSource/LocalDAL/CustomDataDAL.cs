using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.COMS.DataSource.LocalDAL
{
    public class CustomDataDAL : ICustomDataDAL
    {
        private string connectionString;

        public CustomDataDAL()
        {
            this.connectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
        }
        public List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters, Dictionary<string, string> outputparameters)
        {

            List<Dictionary<string, string>> response = new List<Dictionary<string, string>>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spname;

                    foreach (KeyValuePair<string, string> pair in inputparameters)
                    {
                        command.Parameters.Add(pair.Key, SqlDbType.NVarChar).Value = pair.Value;
                    }
                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Dictionary<string, string> _item = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, string> pair in outputparameters)
                            {
                                _item.Add(pair.Key, dataReader[pair.Key].ToString());
                            }
                            response.Add(_item);
                        }
                    }
                }
            }
            return response;
        }

        public List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters, Dictionary<string, DataTable> inputkeyvaluetable, Dictionary<string, string> outputparameters)
        {
            List<Dictionary<string, string>> response = new List<Dictionary<string, string>>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spname;

                    foreach (KeyValuePair<string, string> pair in inputparameters)
                    {
                        command.Parameters.Add(pair.Key, SqlDbType.NVarChar).Value = pair.Value;
                    }
                    foreach (var pair1 in inputkeyvaluetable)
                    {
                        command.Parameters.AddWithValue(pair1.Key, pair1.Value);
                    }

                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Dictionary<string, string> _item = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, string> pair in outputparameters)
                            {
                                _item.Add(pair.Key, dataReader[pair.Key].ToString());
                            }
                            response.Add(_item);
                        }
                    }
                }
            }
            return response;
        }
    }
}
