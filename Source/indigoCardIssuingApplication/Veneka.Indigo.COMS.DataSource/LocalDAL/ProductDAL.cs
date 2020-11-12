using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.COMS.DataSource.LocalDAL
{
    public class ProductDAL : IProductDAL
    {
        private readonly string _connectionString;

        public ProductDAL()
        {
            this._connectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
        }


        public Product GetProduct(int productId, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_getProductbyProductid]";

                    command.Parameters.Add("@productid", SqlDbType.Int).Value = productId;                    

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {                           

                            var product = new Product
                            {
                                BIN = reader["product_bin_code"].ToString(),
                                CVKA = reader["CVKA"].ToString(),
                                CVKB = reader["CVKB"].ToString(),
                                IssuerId = int.Parse(reader["issuer_id"].ToString()),
                                ProductId = int.Parse(reader["product_id"].ToString()),
                                ProductCode = reader["product_code"].ToString(),
                                ProductName = reader["product_name"].ToString(),
                                PVK = reader["PVK"].ToString(),
                                PVKI = int.Parse(reader["PVKI"].ToString()),
                                ServiceCode1 = int.Parse(reader["src1_id"].ToString()),
                                ServiceCode2 = int.Parse(reader["src2_id"].ToString()),
                                ServiceCode3 = int.Parse(reader["src3_id"].ToString()),
                                SubProductCode = reader["sub_product_code"].ToString(),
                                CardIssueMethodId = int.Parse(reader["card_issue_method_id"].ToString()),
                                IsDeleted = reader["DeletedYN"].ToString() == "1" ? true : false,
                                ProductLoadTypeId = int.Parse(reader["product_load_type_id"].ToString()),
                                DecimalisationTable = reader["decimalisation_table"].ToString(),
                                PinValidationData = reader["pin_validation_data"].ToString(),
                                PinBlockFormatId = int.Parse(reader["pin_block_formatid"].ToString()),
                                PinCalcMethodId = int.Parse(reader["pin_calc_method_id"].ToString()),
                                MinPinLength = int.Parse(reader["min_pin_length"].ToString()),
                                MaxPinLength = int.Parse(reader["max_pin_length"].ToString())
                            };

                            product.HasCreditLimit = CheckCreditLimit(reader);

                            if (onlyActiveRecords && !product.IsDeleted)
                            {
                                return product;                                
                            }
                            else if (onlyActiveRecords && product.IsDeleted)
                            {
                                return null;
                            }
                            else
                            {
                                return product;
                            }
                        }
                    }

                    return null;
                }
            }
        }

        private bool CheckCreditLimit(SqlDataReader reader)
        {
            try
            {
                var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                return Convert.ToBoolean(GetValue(reader, "credit_limit_capture", columns, "1"));
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Find all active products linked to an issuer which match on a 6 digit BIN.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="bin"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public List<Product> GetProducts(int? issuerId, string bin, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_products_by_bincode]";

                    command.Parameters.Add("@bin_code", SqlDbType.VarChar).Value = bin;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@only_active_records", SqlDbType.Bit).Value = onlyActiveRecords;
                    //command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    //command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    List<Product> products = new List<Product>();

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                BIN = reader["product_bin_code"].ToString(),
                                CVKA = reader["CVKA"].ToString(),
                                CVKB = reader["CVKB"].ToString(),
                                IssuerId = int.Parse(reader["issuer_id"].ToString()),
                                ProductId = int.Parse(reader["product_id"].ToString()),
                                ProductCode = reader["product_code"].ToString(),
                                ProductName = reader["product_name"].ToString(),
                                PVK = reader["PVK"].ToString(),
                                PVKI = int.Parse(reader["PVKI"].ToString()),
                                ServiceCode1 = int.Parse(reader["src1_id"].ToString()),
                                ServiceCode2 = int.Parse(reader["src2_id"].ToString()),
                                ServiceCode3 = int.Parse(reader["src3_id"].ToString()),
                                SubProductCode = reader["sub_product_code"].ToString(),
                                CardIssueMethodId = int.Parse(reader["card_issue_method_id"].ToString()),
                                IsDeleted = reader["DeletedYN"].ToString() == "1" ? true : false,
                                ProductLoadTypeId = int.Parse(reader["product_load_type_id"].ToString()),
                                DecimalisationTable = reader["decimalisation_table"].ToString(),
                                PinValidationData = reader["pin_validation_data"].ToString(),
                                PinBlockFormatId = int.Parse(reader["pin_block_formatid"].ToString()),
                                PinCalcMethodId = int.Parse(reader["pin_calc_method_id"].ToString()),
                                MinPinLength = int.Parse(reader["min_pin_length"].ToString()),
                                MaxPinLength = int.Parse(reader["max_pin_length"].ToString())
                            };
                            product.HasCreditLimit = CheckCreditLimit(reader);
                            products.Add(product);
                        }
                    }

                    return products;
                }
            }
        }

        /// <summary>
        /// Find all active products linked to an issuer which match on a 6 digit BIN.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="bin"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public List<Product> GetProductsByCode(int? issuerId, string productCode, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_products_by_productcode]";

                    command.Parameters.Add("@product_code", SqlDbType.Char).Value = productCode;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@only_active_records", SqlDbType.Bit).Value = onlyActiveRecords;
                    //command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    //command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    List<Product> products = new List<Product>();

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                BIN = reader["product_bin_code"].ToString(),
                                CVKA = reader["CVKA"].ToString(),
                                CVKB = reader["CVKB"].ToString(),
                                IssuerId = int.Parse(reader["issuer_id"].ToString()),
                                ProductId = int.Parse(reader["product_id"].ToString()),
                                ProductCode = reader["product_code"].ToString(),
                                ProductName = reader["product_name"].ToString(),
                                PVK = reader["PVK"].ToString(),
                                PVKI = int.Parse(reader["PVKI"].ToString()),
                                ServiceCode1 = int.Parse(reader["src1_id"].ToString()),
                                ServiceCode2 = int.Parse(reader["src2_id"].ToString()),
                                ServiceCode3 = int.Parse(reader["src3_id"].ToString()),
                                SubProductCode = reader["sub_product_code"].ToString(),
                                CardIssueMethodId = int.Parse(reader["card_issue_method_id"].ToString()),
                                IsDeleted = reader["DeletedYN"].ToString() == "1" ? true : false,
                                ProductLoadTypeId = int.Parse(reader["product_load_type_id"].ToString()),
                                MinPinLength = int.Parse(reader["min_pin_length"].ToString()),
                                MaxPinLength = int.Parse(reader["max_pin_length"].ToString())
                            };
                            product.HasCreditLimit = CheckCreditLimit(reader);
                            products.Add(product);
                        }
                    }

                    return products;
                }
            }
        }


        public List<Product> GetProductsForExport(int? issuerId, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_products_for_export]";
                    
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@only_active_records", SqlDbType.Bit).Value = onlyActiveRecords;
                    //command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    //command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    List<Product> products = new List<Product>();

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                BIN = reader["product_bin_code"].ToString(),
                                CVKA = reader["CVKA"].ToString(),
                                CVKB = reader["CVKB"].ToString(),
                                IssuerId = int.Parse(reader["issuer_id"].ToString()),
                                ProductId = int.Parse(reader["product_id"].ToString()),
                                ProductCode = reader["product_code"].ToString(),
                                ProductName = reader["product_name"].ToString(),
                                PVK = reader["PVK"].ToString(),
                                PVKI = int.Parse(reader["PVKI"].ToString()),
                                ServiceCode1 = int.Parse(reader["src1_id"].ToString()),
                                ServiceCode2 = int.Parse(reader["src2_id"].ToString()),
                                ServiceCode3 = int.Parse(reader["src3_id"].ToString()),
                                SubProductCode = reader["sub_product_code"].ToString(),
                                CardIssueMethodId = int.Parse(reader["card_issue_method_id"].ToString()),
                                IsDeleted = reader["DeletedYN"].ToString() == "1" ? true : false,
                                ProductLoadTypeId = int.Parse(reader["product_load_type_id"].ToString())
                            };
                            products.Add(product);
                        }
                    }

                    return products;
                }
            }
        }

        public List<IProductPrintField> GetProductPrintFieldsByCode(string productCode, long auditUserId, string auditWorkStation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_product_print_fields_by_code]";

                    command.Parameters.Add("@product_code", SqlDbType.Char).Value = productCode;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    List<IProductPrintField> productFields = new List<IProductPrintField>();

                    connection.Open();

                    DataTable table = new DataTable();
                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(command))
                    {
                        da.Fill(table);
                    }

                    foreach (DataRow row in table.Rows)
                    {                       
                        productFields.Add(PrintFieldFactory.CreatePrintField(
                                                row.Field<int>("print_field_type_id"),
                                                row.Field<int>("product_field_id"),
                                                row.Field<string>("field_name"),
                                                (float)row.Field<decimal?>("X").GetValueOrDefault(),
                                                (float)row.Field<decimal?>("Y").GetValueOrDefault(),
                                                (float)row.Field<decimal?>("width").GetValueOrDefault(),
                                                (float)row.Field<decimal?>("height").GetValueOrDefault(),
                                                row.Field<string>("font"),
                                                row.Field<int?>("font_size").GetValueOrDefault(),
                                                0,//row.Field<int?>("fontRGB").GetValueOrDefault(),
                                                row.Field<string>("mapped_name") ?? String.Empty,
                                                row.Field<string>("label") ?? String.Empty,
                                                row.Field<int?>("max_length").GetValueOrDefault(),
                                                row.Field<bool?>("editable").GetValueOrDefault(true),
                                                row.Field<bool?>("deleted").GetValueOrDefault(false),
                                                row.Field<bool?>("printable").GetValueOrDefault(false),
                                                row.Field<int?>("printside").GetValueOrDefault(-1),
                                                new byte[0]));
                    }


                    return productFields;
                }
            }
        }


        public Product FindBestMatch(string pan, List<Product> products)
        {
            Product rtnProd = null;

            int charCount = 0;
            foreach (var product in products)
            {
                //Concatanate the 6 digit BIN and the SubProductCode to get the actual BIN.
                string BIN = product.BIN.Trim() + product.SubProductCode.Trim();

                //Check the actual PAN, not PSUDO pan... Product bin can be up to 9 characters.
                //Check that the PAN passed in, starts with the BIN of the product
                if (pan.StartsWith(BIN.Trim()))
                {
                    //Need to find the bincode with the most amount of characters, as this result may have more than one result.
                    //E.G. Card number could be 123456-88771199 but the result may contain a record for products with:
                    // 123456 and 123456999 becuse the search is only on the first 6 characters.
                    if (BIN.Length > charCount)
                    {
                        charCount = BIN.Length;
                        rtnProd = product;
                    }
                }
            }

            return rtnProd;
        }

        /// <summary>
        /// Find the product that best matches the supplied BIN
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="pan"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public Product FindBestMatch(int? issuerId, string pan, bool onlyActiveRecords, long auditUserId, string auditWorkstation)
        {            
            var products = this.GetProducts(issuerId, pan, onlyActiveRecords, auditUserId, auditWorkstation);

            return this.FindBestMatch(pan, products);
        }

        private string GetValue(SqlDataReader reader, string fieldName, List<string> columns, string defaultValue = "")
        {
            if (columns.Where(p => p == fieldName).Count() > 0)
            {
                try
                {
                    return reader[fieldName].ToString();
                }
                catch (Exception)
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
