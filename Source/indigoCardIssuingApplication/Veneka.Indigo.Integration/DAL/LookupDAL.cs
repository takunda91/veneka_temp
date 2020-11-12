using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Veneka.Indigo.Integration.Objects;
using System.Data.SqlClient;
using System.Data;

namespace Veneka.Indigo.Integration.DAL
{
    public class LookupDAL
    {
        private string connectionString;

        public LookupDAL(string ConnectionString)
        {
            this.connectionString = ConnectionString;
        }

        /// <summary>
        /// Lookup branch code by branch id
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public string LookupBranchCode(int branchId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_branch_by_id]";

                    command.Parameters.Add("@branch_id", SqlDbType.Int).Value = branchId;

                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            return dataReader["branch_code"].ToString();
                        }
                    }
                }
            }

            return String.Empty;
        }



        /// <summary>
        /// Lookup EMP branch code by branch id
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public string LookupEmpBranchCode(int branchId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_branch_by_id]";

                    command.Parameters.Add("@branch_id", SqlDbType.Int).Value = branchId;

                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            return dataReader["emp_branch_code"].ToString();
                        }
                    }
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Lookup branch name by branch id
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public string LookupBranchName(int branchId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_branch_by_id]";

                    command.Parameters.Add("@branch_id", SqlDbType.Int).Value = branchId;

                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            return dataReader["branch_name"].ToString();
                        }
                    }
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Fetch last sequence number for a card product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="ConnectionString"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public int LookupCurrency(string ccy)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_currency]";

                    command.Parameters.Add("@CCY", SqlDbType.VarChar).Value = ccy;

                    connection.Open();
                    var result = command.ExecuteScalar();

                    return int.Parse(result.ToString());
                }
            }
        }

        /// <summary>
        /// Looks up the ISO 4217 Currency Code for currency ID
        /// </summary>
        /// <param name="currencyId"></param>
        /// <returns></returns>
        public string LookupCurrency(int currencyId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_currency_id]";

                    command.Parameters.Add("@currency_id", SqlDbType.VarChar).Value = currencyId;

                    connection.Open();
                    var result = command.ExecuteScalar();

                    return result.ToString();
                }
            }
        }

        /// <summary>
        /// Looks up the ISO 4217 Numeric Code for currency ID
        /// </summary>
        /// <param name="currencyId"></param>
        /// <returns></returns>
        public string LookupCurrencyISONumericCode(int currencyId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_currency_iso_numeric_code]";

                    command.Parameters.Add("@currency_id", SqlDbType.VarChar).Value = currencyId;

                    connection.Open();
                    var result = command.ExecuteScalar();

                    return result.ToString();
                }
            }
        }
    }
}
