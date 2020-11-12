using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.DataSource.LocalDAL
{
    public class CardsDAL : ICardsDAL
    {
        private readonly string _connectionString;

        public CardsDAL()
        {
       
            _connectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
    }

        public CardObject GetCardByPan(string pan, int mbr, string referenceNumber, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    //sp_get_card
                    //sp_get_export_batch_card_details
                    command.CommandText = "[usp_get_card_by_pan]";

                    command.Parameters.Add("@pan", SqlDbType.VarChar).Value = pan;
                    command.Parameters.Add("@mbr", SqlDbType.Int).Value = mbr;
                    command.Parameters.Add("@reference_number", SqlDbType.VarChar).Value = referenceNumber;

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkstation;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new CardObject(reader);
                        }
                    }

                    return null;
                }
            }
        }

        public CardLimitData GetCardLimitDataByContractNumber(string contractNumber)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    //sp_get_card
                    //sp_get_export_batch_card_details
                    command.CommandText = "[usp_card_limit_get_by_contract_number]";

                    command.Parameters.Add("@contract_number", SqlDbType.VarChar).Value = contractNumber;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new CardLimitData(reader);
                        }
                    }

                    return null;
                }
            }
        }

        public CardObject GetCardObject(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    //sp_get_card
                    //sp_get_export_batch_card_details
                    command.CommandText = "[usp_get_card_object]";

                    command.Parameters.Add("@card_id", SqlDbType.BigInt).Value = cardId;
                    //command.Parameters.Add("@check_masking", SqlDbType.Bit).Value = false;

                    //command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkstation;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new CardObject(reader);
                        }
                    }

                    return null;
                }
            }
        }


        public CardObject GetCardObjectFromExport(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.CommandText = "[usp_get_export_batch_card_details]";

                    command.Parameters.Add("@export_batch_id", SqlDbType.BigInt).Value = exportBatchId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkstation;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new CardObject(reader);
                        }
                    }

                    return null;
                }
            }
        }

        public List<CardObject> GetCardsByAccNo(int productId, string accountNumber, long auditUserId, string auditWorkStation)
        {
            List<CardObject> _list = new List<CardObject>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.CommandText = "[usp_get_customer_issuedcards]";
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = null;
                    command.Parameters.Add("@customeraccountno", SqlDbType.BigInt).Value = accountNumber;
                    command.Parameters.Add("@product_id", SqlDbType.BigInt).Value = productId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _list.Add(new CardObject()
                            {
                                ProductName = reader["product_name"].ToString(),
                                CardNumber = reader["card_number"].ToString(),
                                CustomerAccount = new AccountDetails() {
                                    FirstName = reader["first_name"].ToString(),
                                    LastName = reader["last_name"].ToString(),
                                    AccountNumber = reader["account_number"].ToString()
                                },
                            CardId = Convert.ToInt64(reader["card_id"])
                           // CardReferenceNumber = reader["card_request_reference"].ToString(),
                        });


                        }
                        
                    }

                    return _list;

                }
            }
        }

        /// <summary>
        /// Updates the selected cards fee reference numebr
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        public void UpdateCardFeeReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_card_fee_reference]";
                    command.Parameters.Add("@card_id", SqlDbType.BigInt).Value = cardId;
                    command.Parameters.Add("@fee_reference_number", SqlDbType.VarChar).Value = referenceNumber;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Updates the selected cards fee reversal reference number
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        public void UpdateCardFeeReversalReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_card_fee_reversal_ref]";
                    command.Parameters.Add("@card_id", SqlDbType.BigInt).Value = cardId;
                    command.Parameters.Add("@fee_reversal_reference_number", SqlDbType.VarChar).Value = referenceNumber;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

       
    }
}
