using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Veneka.Indigo.Common.DataAccess;

namespace Veneka.Indigo.DocumentManagement.dal
{
    public class CardDocumentDataAccess : BaseDataAccess, ICardDocumentDataAccess
    {
        public IList<CardDocument> All(long cardId)
        {
            List<CardDocument> result = new List<CardDocument>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_id", SqlDbType.BigInt, 0, cardId)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_card_document_all"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(Unpack(row));
                }
            }

            return result;
        }

        private CardDocument Unpack(DataRow row)
        {
            try
            {
                CardDocument card_document = new CardDocument
                {
                    Id = UnpackInt(row, "document_id"),
                    CardId = UnpackInt(row, "card_id"),
                    DocumentTypeId = UnpackInt(row, "document_type_id"),
                    Comment = UnpackString(row, "comment"),
                    Location = UnpackString(row, "document_url")
                };
                return card_document;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected SqlParameter[] SaveParameters(CardDocument entity)
        {
            List<SqlParameter> result = new List<SqlParameter>
            {
                    CreateSqlParameter("document_id", SqlDbType.Int, 0, entity.Id,ParameterDirection.InputOutput,10,0),
                    CreateSqlParameter("card_id", SqlDbType.Int, 0, entity.CardId,ParameterDirection.Input,10,0),
                    CreateSqlParameter("document_type_id", SqlDbType.Int, 0, entity.DocumentTypeId,ParameterDirection.Input,10,0),
                    CreateSqlParameter("document_url", SqlDbType.VarChar, 1000, entity.Location,ParameterDirection.Input,0,0),
                    CreateSqlParameter("comment", SqlDbType.VarChar, 100, entity.Comment,ParameterDirection.Input,0,0),
            };
            return result.ToArray();
        }

        public long Create(CardDocument entity)
        {
            int id = 0;
            using (SqlCommand sqlCommand = new SqlCommand("usp_card_document_create"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(SaveParameters(entity));

                ExecuteNonQuery(sqlCommand);
                id = Convert.ToInt32(sqlCommand.Parameters["@document_id"].Value);
            }
            return id;
        }

        public bool Delete(long id)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("document_id", SqlDbType.Int, 0, id,ParameterDirection.Input,0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_card_document_delete"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return true;
        }

        public CardDocument Retrieve(long id)
        {
            CardDocument result = new CardDocument();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("document_id", SqlDbType.BigInt, 0, id)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_card_document_get"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result = Unpack(row);
                }
            }
            return result;
        }

        public long Update(CardDocument entity)
        {
            int id = 0;
            using (SqlCommand sqlCommand = new SqlCommand("usp_card_document_update"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(SaveParameters(entity));

                ExecuteNonQuery(sqlCommand);
                id = Convert.ToInt32(sqlCommand.Parameters["@document_id"].Value);
            }
            return id;
        }
    }
}
