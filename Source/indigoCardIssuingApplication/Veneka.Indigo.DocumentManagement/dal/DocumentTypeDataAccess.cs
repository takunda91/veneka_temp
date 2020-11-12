using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Veneka.Indigo.Common.DataAccess;

namespace Veneka.Indigo.DocumentManagement.dal
{
    public class DocumentTypeDataAccess : BaseDataAccess, IDocumentTypeDataAccess
    {
        public IList<DocumentType> All(bool activeOnly)
        {
            List<DocumentType> result = new List<DocumentType>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("active_only", SqlDbType.Bit, 0, activeOnly)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_document_type_all"))
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

        public int Create(DocumentType entity)
        {
            int id = 0;
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("document_type_id", SqlDbType.Int, 0, entity.Id,ParameterDirection.InputOutput,0,0),
                CreateSqlParameter("name", SqlDbType.VarChar, 100, entity.Name,ParameterDirection.Input,0,0),
                CreateSqlParameter("description", SqlDbType.VarChar, 500, entity.Description    ,ParameterDirection.Input,0,0),
                CreateSqlParameter("is_active", SqlDbType.Bit, 0, entity.IsActive,ParameterDirection.Input,0,0),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_document_type_create"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
                id = Convert.ToInt32(sqlCommand.Parameters["@document_type_id"].Value);
            }
            return id;
        }

        public bool Delete(int id)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("document_type_id", SqlDbType.Int, 0, id,ParameterDirection.Input,0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_document_type_delete"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return true;
        }

        public DocumentType Retrieve(int id)
        {
            DocumentType result = new DocumentType();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("document_type_id", SqlDbType.BigInt, 0, id)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_document_type_get"))
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

        private DocumentType Unpack(DataRow row)
        {
            try
            {
                DocumentType document_type = new DocumentType
                {
                    Id = UnpackInt(row, "document_type_id"),
                    Name = UnpackString(row, "name"),
                    Description = UnpackString(row, "description"),
                    IsActive = UnpackBoolean(row, "is_active"),
                };
                return document_type;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Update(DocumentType entity)
        {
            int id = entity.Id;
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("document_type_id", SqlDbType.Int, 0, entity.Id,ParameterDirection.InputOutput,0,0),
                CreateSqlParameter("name", SqlDbType.VarChar, 100, entity.Name,ParameterDirection.Input,0,0),
                CreateSqlParameter("description", SqlDbType.VarChar, 500, entity.Description    ,ParameterDirection.Input,0,0),
                CreateSqlParameter("is_active", SqlDbType.Bit, 0, entity.IsActive,ParameterDirection.Input,0,0),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_document_type_update"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return id;
        }
    }
}
