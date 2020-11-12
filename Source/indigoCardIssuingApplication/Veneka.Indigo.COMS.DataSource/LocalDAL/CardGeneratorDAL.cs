using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Objects;
using System.Data.SqlClient;
using System.Data;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Common.Database;

namespace Veneka.Indigo.COMS.DataSource.LocalDAL
{
    public class CardGeneratorDAL : ICardGeneratorDAL
    {
        private string connectionString;

        public CardGeneratorDAL()
        {
            this.connectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
        }

        /// <summary>
        /// Fetch last sequence number for a card product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="ConnectionString"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public int GetLatestSequenceNumber(int productId, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_sequencenumber]";

                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@sub_product_id", SqlDbType.Int).Value = null;
                    command.Parameters.Add("@auditUserId", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@auditWorkStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();
                    var seqNumber = command.ExecuteScalar();

                    return int.Parse(seqNumber.ToString());
                }
            }
        }

        /// <summary>
        /// Updates the cards with their newly generated card numbers as well as updating the last sequence number for each product used.
        /// </summary>
        /// <param name="cards">Dictionary[CardId, CardNumber]</param>
        /// <param name="products">Dictionary[ProductId, SequenceNumber]</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        public void UpdateCardsAndSequenceNumber(Dictionary<long, string> cards, Dictionary<int, int> products, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_card_numbers]";
                    command.Parameters.AddWithValue("@card_list", CreateKeyValueTable<long, string>(cards));
                    command.Parameters.AddWithValue("@product_list", CreateKeyValueTable(products));
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

            private DataTable CreateKeyValueTable<K, V>(Dictionary<K, V> dictionary)
            {
                string key = "key";
                string value = "value";

                DataTable dt = new DataTable();
            dt.Columns.Add(key, typeof(long));
            dt.Columns.Add(value, typeof(string));

            foreach (var item in dictionary)
            {
                dt.Rows.Add(CreateRow<K, V>(item.Key, item.Value, dt.NewRow()));
            }

            return dt;
        }

        private DataTable CreateKeyValueTable(Dictionary<Tuple<int, int?>, int> dictionary)
        {
            string key1 = "key1";
            string key2 = "key2";
            string value = "value";

            DataTable dt = new DataTable();
            dt.Columns.Add(key1, typeof(long));
            DataColumn key2Col = new DataColumn(key2);
            key2Col.DataType = typeof(long);
            key2Col.AllowDBNull = true;
            dt.Columns.Add(key2Col);
            dt.Columns.Add(value, typeof(string));

            foreach (var item in dictionary)
            {
                dt.Rows.Add(CreateBiKeyRow(item.Key, item.Value.ToString(), dt.NewRow()));
            }

            return dt;
        }

        private DataRow CreateRow<K, V>(K key, V value, DataRow workRow)
        {
            workRow["key"] = key;
            workRow["value"] = value;

            return workRow;
        }

        private DataRow CreateBiKeyRow(Tuple<int, int?> key, string value, DataRow workRow)
        {
            workRow["key1"] = key.Item1;

            if (key.Item2 != null)
                workRow["key2"] = key.Item2;
            else
                workRow.SetField("key2", DBNull.Value);

            workRow["value"] = value;

            return workRow;
        }
    }
}
