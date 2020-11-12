using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.COMS.DataSource.LocalDAL
{  
    /// <summary>
    /// Data Access Layer for fetching and updating the latest transaction sequence.
    /// Allows you to use your own sequence which can be reset daily, weekly, monthly or yearly
    /// </summary>  
    public class TransactionSequenceDAL : ISequenceGenerator, ITransactionSequenceDAL
    {
        private static readonly object _lockObject = new object();
        private readonly string _connectionString;

        public TransactionSequenceDAL()
        {
            _connectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
        }

        public void Dispose()
        {
            
        }

        /// <summary>
        /// Fetch the next sequence number to be used. This method is thread-safe.
        /// </summary>
        /// <param name="sequenceName">Name of the sequence</param>
        /// <param name="resetPeriod">How often this sequence is reset.</param>
        /// <returns></returns>
        public int NextSequenceNumber(string sequenceName, ResetPeriod resetPeriod)
        {
            lock(_lockObject)
            {
                //Fetch latest sequence number for transaction. 
                //Should be stored proc which updates the sequence
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "[usp_get_next_sequence]";

                        command.Parameters.Add("@sequence_name", SqlDbType.VarChar).Value = sequenceName;
                        command.Parameters.Add("@reset_period", SqlDbType.Int).Value = (int)resetPeriod;
                        var seqNumber = command.ExecuteScalar();

                        return int.Parse(seqNumber.ToString());
                    }
                }
            }
        }

        public long NextSequenceNumberLong(string sequenceName, ResetPeriod resetPeriod)
        {
            lock (_lockObject)
            {
                //Fetch latest sequence number for transaction. 
                //Should be stored proc which updates the sequence
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "[usp_get_next_sequence]";

                        command.Parameters.Add("@sequence_name", SqlDbType.VarChar).Value = sequenceName;
                        command.Parameters.Add("@reset_period", SqlDbType.Int).Value = (int)resetPeriod;
                        var seqNumber = command.ExecuteScalar();

                        return long.Parse(seqNumber.ToString());
                    }
                }
            }
        }
    }
}
