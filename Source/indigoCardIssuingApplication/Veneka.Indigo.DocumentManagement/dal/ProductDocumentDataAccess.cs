using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Veneka.Indigo.Common.DataAccess;

namespace Veneka.Indigo.DocumentManagement.dal
{
    public class ProductDocumentDataAccess : BaseDataAccess, IProductDocumentDataAccess
    {
        public IList<ProductDocumentListModel> All(int productId, bool activeOnly)
        {
            List<ProductDocumentListModel> result = new List<ProductDocumentListModel>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("product_id", SqlDbType.Int, 0, productId),
                CreateSqlParameter("active_only", SqlDbType.Bit, 0, activeOnly)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_product_document_all"))
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

        private ProductDocumentListModel Unpack(DataRow row)
        {
            try
            {
                ProductDocumentListModel product_document = new ProductDocumentListModel
                {
                    Id = UnpackInt(row, "product_document_id"),
                    ProductId = UnpackInt(row, "product_id"),
                    DocumentTypeId = UnpackInt(row, "document_type_id"),
                    IsRequired = UnpackBoolean(row, "is_required"),
                    IsActive = UnpackBoolean(row, "is_active"), 
                    DocumentTypeDescription=UnpackString(row, "document_type_description"),
                    DocumentTypeName = UnpackString(row, "document_type_name"),
                };
                product_document.Included = (product_document.Id != 0);
                return product_document;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected SqlParameter[] SaveParameters(ProductDocument entity)
        {
            List<SqlParameter> result = new List<SqlParameter>
            {
                    CreateSqlParameter("product_document_id", SqlDbType.Int, 0, entity.Id,ParameterDirection.Input,10,0),
                    CreateSqlParameter("product_id", SqlDbType.Int, 0, entity.ProductId,ParameterDirection.Input,10,0),
                    CreateSqlParameter("document_type_id", SqlDbType.Int, 0, entity.DocumentTypeId,ParameterDirection.Input,10,0),
                    CreateSqlParameter("is_required", SqlDbType.Bit, 0, entity.IsRequired,ParameterDirection.Input,0,0),
                    CreateSqlParameter("is_active", SqlDbType.Bit, 0, entity.IsActive,ParameterDirection.Input,0,0),
            };
            return result.ToArray();
        }

        public int Create(ProductDocument entity)
        {
            int id = 0;
            using (SqlCommand sqlCommand = new SqlCommand("usp_product_document_create"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(SaveParameters(entity));

                ExecuteNonQuery(sqlCommand);
                id = Convert.ToInt32(sqlCommand.Parameters["@product_document_id"].Value);
            }
            return id;
        }

        public bool Delete(int id)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("product_document_id", SqlDbType.Int, 0, id,ParameterDirection.Input,0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_product_document_delete"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return true;
        }

        public ProductDocumentListModel Retrieve(int id)
        {
            ProductDocumentListModel result = new ProductDocumentListModel();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("document_type_id", SqlDbType.BigInt, 0, id)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_product_document_get"))
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

        public int Update(ProductDocument entity)
        {
            int id = 0;
            using (SqlCommand sqlCommand = new SqlCommand("usp_product_document_update"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(SaveParameters(entity));

                ExecuteNonQuery(sqlCommand);
                id = Convert.ToInt32(sqlCommand.Parameters["@product_document_id"].Value);
            }
            return id;
        }
    }
}
