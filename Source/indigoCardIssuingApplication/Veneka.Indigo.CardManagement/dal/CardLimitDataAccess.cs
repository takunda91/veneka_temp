using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common.DataAccess;

namespace Veneka.Indigo.CardManagement.dal
{
    public class CardLimitDataAccess : BaseDataAccess, ICardLimitDataAccess
    {
        public bool CreateLimit(long cardId, decimal limit)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_id",SqlDbType.BigInt, 0, cardId),
                CreateSqlParameter("limit", SqlDbType.Decimal, 0, limit, ParameterDirection.Input, 18,4)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_card_limit_create"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return true;
        }

        public bool UpdateLimit(long cardId, decimal limit)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_id",SqlDbType.BigInt, 0, cardId),
                CreateSqlParameter("limit", SqlDbType.Decimal, 0, limit, ParameterDirection.Input, 18,4)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_card_limit_update"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return true;
        }

        public bool ApproveLimit(long cardId, decimal limit, long userId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_id",SqlDbType.BigInt, 0, cardId),
                CreateSqlParameter("user_id",SqlDbType.BigInt, 0, userId),
                CreateSqlParameter("limit",SqlDbType.Decimal, 0, limit, ParameterDirection.Input,18,4),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_card_limit_approve"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return true;
        }

        public CardLimitModel GetLimit(long cardId)
        {
            CardLimitModel result = null;
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_id",SqlDbType.BigInt, 0, cardId)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_card_limit_get"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result = UnPack(row);
                }
            }
            return result;
        }

        private CardLimitModel UnPack(DataRow row)
        {
            try
            {
                CardLimitModel result = new CardLimitModel
                {
                    CardId = UnpackLong(row, "card_id"),
                    CardLimitId = UnpackLong(row, "card_limit_id"),
                    Limit = UnpackDecimal(row, "limit"),
                    LimitApproved = UnpackDecimal(row, "limit_approved"),
                    ContractNumber = UnpackString(row, "contract_number"),
                    CreditAnalystId = UnpackLong(row, "analyst_id"),
                    CreditManagerId = UnpackLong(row, "manager_id")

                };
                return result;
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        public bool ApproveLimitManager(long cardId, long userId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_id",SqlDbType.BigInt, 0, cardId),
                CreateSqlParameter("user_id",SqlDbType.BigInt, 0, userId),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_card_limit_approve_manager"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return true;
        }

        public bool UpdateContractNumber(long cardId, string contract_number)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_id",SqlDbType.BigInt, 0, cardId),
                CreateSqlParameter("contract_number", SqlDbType.VarChar, 50, contract_number, ParameterDirection.Input, 18,4)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_card_limit_update_contract_number"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return true;
        }

        public bool SetCreditStatus(long cardId, int creditStatusId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_id",SqlDbType.BigInt, 0, cardId),
                CreateSqlParameter("status_id",SqlDbType.Int, 0, creditStatusId),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_card_limit_by_pass"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return true;
        }

        public bool SetCreditContractNumber(long cardId, string creditContractNumber, long userId)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_id",SqlDbType.BigInt, 0, cardId),
                CreateSqlParameter("user_id",SqlDbType.BigInt, 0, userId),
                CreateSqlParameter("contract_number",SqlDbType.VarChar, 50, creditContractNumber, ParameterDirection.Input),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_card_limit_contract_number"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return true;
        }
    }
}
