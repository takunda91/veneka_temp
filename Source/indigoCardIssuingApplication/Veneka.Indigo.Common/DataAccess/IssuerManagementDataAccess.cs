using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Models.IssuerManagement;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Common.DataAccess
{
    public class IssuerManagementDataAccess : BaseDataAccess
    {
        public List<product_interface> usp_get_product_interfaces(int? product_id, int? interface_type_id, int? interface_area, long? audit_user_id, string audit_workstation)
        {
            List<product_interface> result = new List<product_interface>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("product_id",SqlDbType.Int, 0, product_id),
                CreateSqlParameter("interface_type_id", SqlDbType.Int, 0, interface_type_id),
                CreateSqlParameter("interface_area", SqlDbType.Int, 0, interface_area),
                CreateSqlParameter("audit_user_id", SqlDbType.Int, 0, audit_user_id),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, audit_workstation),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_product_interfaces"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnpackProductInterface(row));
                }
            }
            return result;
        }

        private product_interface UnpackProductInterface(DataRow row)
        {
            try
            {
                product_interface product_interface = new product_interface
                {
                    interface_type_id = UnpackInt(row, "interface_type_id"),
                    product_id = UnpackInt(row, "product_id"),
                    connection_parameter_id = UnpackInt(row, "connection_parameter_id"),
                    interface_area = UnpackInt(row, "interface_area"),
                    interface_guid = UnpackString(row, "interface_guid")
                };
                return product_interface;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ProductlistResult> usp_get_productlist(int? issuer_id, int? card_issue_method_id, bool? deleted_YN, int? pageIndex, int? rowsPerPage)
        {
            List<ProductlistResult> result = new List<ProductlistResult>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Int, 0, issuer_id),
                CreateSqlParameter("card_issue_method_id", SqlDbType.Int, 0, card_issue_method_id),
                CreateSqlParameter("deleted_YN", SqlDbType.Bit, 0, deleted_YN),
                CreateSqlParameter("pageIndex", SqlDbType.Int, 0, pageIndex),
                CreateSqlParameter("rowsPerPage", SqlDbType.Int, 0, rowsPerPage),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_productlist"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                int rowNumber = 0;
                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    ProductlistResult newitem = UnpackProductListResult(row);
                    newitem.ROW_NO = rowNumber;
                    rowNumber++;
                    result.Add(newitem);
                }
                foreach (var item in result)
                {
                    item.TOTAL_PAGES = (rowsPerPage.HasValue && rowsPerPage.Value > 0 ? result.Count / rowsPerPage.GetValueOrDefault() : result.Count);
                    item.TOTAL_ROWS = result.Count;
                }
            }
            return result;
        }

        public List<PinRequestList> usp_get_pinrequests(int? issuer_id, int? pageIndex, int? rowsPerPage)
        {
            List<PinRequestList> result = new List<PinRequestList>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Int, 0, issuer_id),
                CreateSqlParameter("pageIndex", SqlDbType.Int, 0, pageIndex),
                CreateSqlParameter("rowsPerPage", SqlDbType.Int, 0, rowsPerPage),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_pinrequests"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                int rowNumber = 0;
                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    PinRequestList newitem = UnpackPinRequestListResult(row);
                    newitem.ROW_NO = rowNumber;
                    rowNumber++;
                    result.Add(newitem);
                }
                foreach (var item in result)
                {
                    item.TOTAL_PAGES = (rowsPerPage.HasValue && rowsPerPage.Value > 0 ? result.Count / rowsPerPage.GetValueOrDefault() : result.Count);
                    item.TOTAL_ROWS = result.Count;
                }
            }
            return result;
        }

        public List<PinRequestList> usp_get_pinrequests_for_status(int? issuer_id, string request_status, string reissue_approval_stage, string request_type, int? pageIndex, int? rowsPerPage)
        {
            List<PinRequestList> result = new List<PinRequestList>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Int, 0, issuer_id),
                CreateSqlParameter("pin_request_stage",SqlDbType.VarChar, 0, reissue_approval_stage),
                CreateSqlParameter("request_status",SqlDbType.VarChar, 0, request_status),
                CreateSqlParameter("request_type",SqlDbType.NChar, 0, request_type),
                CreateSqlParameter("pageIndex", SqlDbType.Int, 0, pageIndex),
                CreateSqlParameter("rowsPerPage", SqlDbType.Int, 0, rowsPerPage),
            };
            string sp;
            if (string.IsNullOrEmpty(reissue_approval_stage))
            {
                 sp = "usp_get_pinrequests_for_status_for_new_issue";
            }
            else
            {
                 sp = "usp_get_pinrequests_for_status";
            }

            using (SqlCommand sqlCommand = new SqlCommand(sp))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                int rowNumber = 0;
                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    PinRequestList newitem = UnpackPinRequestListResult(row);
                    newitem.ROW_NO = rowNumber;
                    rowNumber++;
                    result.Add(newitem);
                }
                foreach (var item in result)
                {
                    item.TOTAL_PAGES = (rowsPerPage.HasValue && rowsPerPage.Value > 0 ? result.Count / rowsPerPage.GetValueOrDefault() : result.Count);
                    item.TOTAL_ROWS = result.Count;
                }
            }
            return result;
        }

        public List<PinRequestList> usp_search_pin_for_reissue(int? issuerId, int? BranchId, int? ProductId, string ProductBin, string lastdigits, string accountnumber,
                                                                   string pinrequestref, int pageIndex, int? rowsPerPage, long auditUserId, string auditWorkstation)
        {
            List<PinRequestList> result = new List<PinRequestList>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Int, 0, issuerId),
                CreateSqlParameter("branch_id",SqlDbType.Int, 0, BranchId),
                CreateSqlParameter("product_id",SqlDbType.Int, 0, ProductId),
                CreateSqlParameter("product_bin",SqlDbType.VarChar, 0, ProductBin),
                CreateSqlParameter("last_four_of_the_pan",SqlDbType.VarChar, 0, lastdigits),
                CreateSqlParameter("customer_account",SqlDbType.VarChar, 0, accountnumber),
                CreateSqlParameter("request_reference",SqlDbType.VarChar, 0, pinrequestref),
                CreateSqlParameter("pageIndex", SqlDbType.Int, 0, pageIndex),
                CreateSqlParameter("rowsPerPage", SqlDbType.Int, 0, rowsPerPage),
                 CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId),
                CreateSqlParameter("audit_workstation", SqlDbType.VarChar, 0, auditWorkstation)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_search_pin_for_reissue"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                int rowNumber = 0;
                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    PinRequestList newitem = UnpackPinRequestListResult(row);
                    newitem.ROW_NO = rowNumber;
                    rowNumber++;
                    result.Add(newitem);
                }
                foreach (var item in result)
                {
                    item.TOTAL_PAGES = (rowsPerPage.HasValue && rowsPerPage.Value > 0 ? result.Count / rowsPerPage.GetValueOrDefault() : result.Count);
                    item.TOTAL_ROWS = result.Count;
                }
            }
            return result;
        }

        public List<TerminalCardData> usp_get_pinblock(int? issuer_id, int? pageIndex, int? rowsPerPage)
        {
            List<TerminalCardData> result = new List<TerminalCardData>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Int, 0, issuer_id),
                CreateSqlParameter("pageIndex", SqlDbType.Int, 0, pageIndex),
                CreateSqlParameter("rowsPerPage", SqlDbType.Int, 0, rowsPerPage),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_pinrequests"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                int rowNumber = 0;
                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    TerminalCardData newitem = UnpackPinBlockListResult(row);
                    newitem.ROW_NO = rowNumber;
                    rowNumber++;
                    result.Add(newitem);
                }
                foreach (var item in result)
                {
                    item.TOTAL_PAGES = (rowsPerPage.HasValue && rowsPerPage.Value > 0 ? result.Count / rowsPerPage.GetValueOrDefault() : result.Count);
                    item.TOTAL_ROWS = result.Count;
                }
            }
            return result;
        }

        public List<TerminalCardData> usp_get_pinblock_for_status(int? issuer_id, string request_status, int? pageIndex, int? rowsPerPage)
        {
            List<TerminalCardData> result = new List<TerminalCardData>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Int, 0, issuer_id),
                CreateSqlParameter("request_status",SqlDbType.VarChar, 0, request_status),
                CreateSqlParameter("pageIndex", SqlDbType.Int, 0, pageIndex),
                CreateSqlParameter("rowsPerPage", SqlDbType.Int, 0, rowsPerPage),
            };

            var stored_procedure = String.Empty;
            if (String.IsNullOrWhiteSpace(request_status))
            {
                stored_procedure = "usp_get_pinblock_for_status_null";
            }
            else
            {
                stored_procedure = "usp_get_pinblock_for_status";
            }

            using (SqlCommand sqlCommand = new SqlCommand(stored_procedure.ToString()))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                int rowNumber = 0;
                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    TerminalCardData newitem = UnpackPinBlockListResult(row);
                    newitem.ROW_NO = rowNumber;
                    rowNumber++;
                    result.Add(newitem);
                }
                foreach (var item in result)
                {
                    item.TOTAL_PAGES = (rowsPerPage.HasValue && rowsPerPage.Value > 0 ? result.Count / rowsPerPage.GetValueOrDefault() : result.Count);
                    item.TOTAL_ROWS = result.Count;
                }
            }
            return result;
        }

        //usp_get_pinbatch_for_status
        public List<TerminalCardData> usp_get_pinbatch_for_status(int? issuer_id, string request_status, int? pageIndex, int? rowsPerPage)
        {
            List<TerminalCardData> result = new List<TerminalCardData>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Int, 0, issuer_id),
                CreateSqlParameter("request_status",SqlDbType.VarChar, 0, request_status),
                CreateSqlParameter("pageIndex", SqlDbType.Int, 0, pageIndex),
                CreateSqlParameter("rowsPerPage", SqlDbType.Int, 0, rowsPerPage),
            };

            var stored_procedure = String.Empty;
            if (String.IsNullOrWhiteSpace(request_status))
            {
                stored_procedure = "usp_get_pinbatch_for_status_null";
            }
            else
            {
                stored_procedure = "usp_get_pinbatch_for_status";
            }

            using (SqlCommand sqlCommand = new SqlCommand(stored_procedure.ToString()))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                int rowNumber = 0;
                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    TerminalCardData newitem = UnpackPinBatchListResult(row);
                    newitem.ROW_NO = rowNumber;
                    rowNumber++;
                    result.Add(newitem);
                }
                foreach (var item in result)
                {
                    item.TOTAL_PAGES = (rowsPerPage.HasValue && rowsPerPage.Value > 0 ? result.Count / rowsPerPage.GetValueOrDefault() : result.Count);
                    item.TOTAL_ROWS = result.Count;
                }
            }
            return result;
        }

        public List<TerminalCardData> usp_get_pin_mailer_batch_history(int pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            List<TerminalCardData> result = new List<TerminalCardData>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("pin_batch_id",SqlDbType.Int, 0, pin_request_id),
                CreateSqlParameter("languageId",SqlDbType.Int, 0, langaugeId),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId),
                CreateSqlParameter("audit_workstation", SqlDbType.VarChar, 0, auditWorkstation),
            };


            var stored_procedure = "usp_get_pin_mailer_batch_history";
            using (SqlCommand sqlCommand = new SqlCommand(stored_procedure.ToString()))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());               
                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    TerminalCardData newitem = UnpackPinMailerBatchHeaderResult(row);                 
                    result.Add(newitem);
                }
               
            }
            return result;
        }

        public List<TerminalCardData> usp_get_pin_mailer_batch_cards(int pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            List<TerminalCardData> result = new List<TerminalCardData>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("pin_batch_header_id",SqlDbType.Int, 0, pin_request_id),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId),
                CreateSqlParameter("audit_workstation", SqlDbType.VarChar, 0, auditWorkstation),
            };

            var stored_procedure = "usp_get_pin_mailer_batch_cards";
            using (SqlCommand sqlCommand = new SqlCommand(stored_procedure.ToString()))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    TerminalCardData newitem = UnpackPinHeaderCardsListResult(row);
                    result.Add(newitem);
                }

            }
            return result;
        }

        //usp_get_pin_request_object
        public PinRequestViewDetails usp_get_pin_request_object(long pin_request_id)
        {
            PinRequestViewDetails result = new PinRequestViewDetails();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("pin_request_id",SqlDbType.BigInt, 0, pin_request_id),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_pin_request_object"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                DataRow row = ExecuteQuery(sqlCommand).Rows[0];
                result = UnpackPinRequestListViewResult(row);
                               
            }
            return result;
        }

        //usp_get_pin_block_object
        public TerminalCardData usp_get_pin_block_object(long pin_request_id)
        {
            TerminalCardData result = new TerminalCardData();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("pinfile_id",SqlDbType.BigInt, 0, pin_request_id),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_pin_block_details"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                DataRow row = ExecuteQuery(sqlCommand).Rows[0];
                result = UnpackPinBlockListResult(row);

            }
            return result;
        }

        //usp_get_pin_batch_object
        public TerminalCardData usp_get_pin_batch_object(long pin_request_id)
        {
            TerminalCardData result = new TerminalCardData();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("pin_file_batch_id",SqlDbType.BigInt, 0, pin_request_id),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_pin_batch_details"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
                DataRow row = ExecuteQuery(sqlCommand).Rows[0];
                result = UnpackPinBatchListResult(row);

            }
            return result;
        }

        public TerminalCardData usp_get_pin_mailer_link_details(string pan_product_bin, string pan_last_four, string expiry_date)
        {
            TerminalCardData result = new TerminalCardData();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("pan_product_bin_code",SqlDbType.NChar, 10, pan_product_bin),
                CreateSqlParameter("pan_last_four",SqlDbType.NChar, 10, pan_last_four),
                CreateSqlParameter("expiry_date",SqlDbType.VarChar, 10, expiry_date),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_pin_mailer_link_details"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result = UnpackPinFileLoaderViewResult(row);
                }

            }
            return result;
        }

        public RestWebservicesHandler usp_get_webservice_params(int issuer_id, string webservice_type, int langaugeId, long auditUserId, string auditWorkstation)
        {
            RestWebservicesHandler result = new RestWebservicesHandler();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Int, 10, issuer_id),
                CreateSqlParameter("webservice_type",SqlDbType.VarChar, 0, webservice_type),
                CreateSqlParameter("audit_user_id",SqlDbType.BigInt, 11, auditUserId),
                CreateSqlParameter("audit_workstation",SqlDbType.VarChar, 50, auditWorkstation),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_webservice_params"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result = UnpackRestResult(row);
                }

            }
            return result;
        }

        public ZMKZPKDetails usp_get_zone_key(int? issuer_id, long auditUserId, string auditWorkstation)
        {
            ZMKZPKDetails result = new ZMKZPKDetails();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Int, 10, issuer_id),
                CreateSqlParameter("audit_user_id",SqlDbType.BigInt, 10, auditUserId),
                CreateSqlParameter("audit_workstation",SqlDbType.VarChar, 10, auditWorkstation),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_zone_key"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result = UnpackZoneKeysResult(row);
                }

            }
            return result;

        }


        public List<issuer_product> usp_get_products_by_bincode(string bin_code, int? issuer_id, bool? only_active_records)
        {
            List<issuer_product> result = new List<issuer_product>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Char, 6, bin_code),
                CreateSqlParameter("issuer_id", SqlDbType.Int, 0, issuer_id),
                CreateSqlParameter("only_active_records", SqlDbType.Bit, 0, only_active_records),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_products_by_bincode"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnpackIssuerProduct(row));
                }
            }
            return result;
        }

        private issuer_product UnpackIssuerProduct(DataRow row)
        {
            try
            {
                issuer_product issuer_product = new issuer_product
                {
                    product_id = UnpackInt(row, "product_id"),
                    product_code = UnpackString(row, "product_code"),
                    product_name = UnpackString(row, "product_name"),
                    product_bin_code = UnpackString(row, "product_bin_code"),
                    issuer_id = UnpackInt(row, "issuer_id"),
                    DeletedYN = UnpackBoolean(row, "DeletedYN"),
                    src1_id = UnpackInt(row, "src1_id"),
                    src2_id = UnpackInt(row, "src2_id"),
                    src3_id = UnpackInt(row, "src3_id"),
                    PVKI = UnpackString(row, "PVKI"),
                    PVK = UnpackString(row, "PVK"),
                    CVKA = UnpackString(row, "CVKA"),
                    CVKB = UnpackString(row, "CVKB"),
                    min_pin_length = UnpackInt(row, "min_pin_length"),
                    max_pin_length = UnpackInt(row, "max_pin_length"),
                    product_load_type_id = UnpackInt(row, "product_load_type_id"),
                    sub_product_code = UnpackString(row, "sub_product_code"),
                    pin_calc_method_id = UnpackInt(row, "pin_calc_method_id"),
                    card_issue_method_id = UnpackInt(row, "card_issue_method_id"),
                    decimalisation_table = UnpackString(row, "decimalisation_table"),
                    pin_validation_data = UnpackString(row, "pin_validation_data"),
                    pin_block_formatid = UnpackInt(row, "pin_block_formatid"),
                };
                return issuer_product;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private PinRequestList UnpackPinRequestListResult(DataRow row)
        {
            try
            {
                PinRequestList pin_request = new PinRequestList
                {
                    pin_request_id = UnpackInt(row, "pin_request_id"),
                    product_id = UnpackInt(row, "product_id"),
                    last_for_digit_of_pan = UnpackString(row, "pan_last_four_digits"),
                    product_bin_code = UnpackString(row, "product_bin_code"),
                    issuer_id = UnpackInt(row, "issuer_id"),
                    pin_request_reference = UnpackString(row, "pin_request_reference"),
                    pin_request_status = UnpackString(row, "pin_request_status"),
                    pin_create_date = UnpackDateTime(row, "created_date"),
                    issuer_name = UnpackString(row, "issuer_name"),
                    branch_name = UnpackString(row, "main_branch"),     
                    
                };
                return pin_request;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //terminal data listing
        private TerminalCardData UnpackPinBlockListResult(DataRow row)
        {
            try
            {  
		
        TerminalCardData pin_block = new TerminalCardData
                {
                    PINBlock = UnpackString(row, "pinfile_encpin_block"),
                    PAN = UnpackString(row, "file_masked_pan"),
                    ProductName = UnpackString(row, "file_product_name"),
                    ExpiryDate = UnpackDateTime(row, "file_expiry_date"),
                    CardId = UnpackString(row, "card_id"),
                    PINFileUploadDate = UnpackDateTime(row, "file_upload_date"),
                    DummyPan = UnpackString(row, "file_dummy_pan"),
                    approval_status = UnpackString(row, "approval_status"),
                    approval_comment = UnpackString(row, "approval_comment"),
                    approval_date = UnpackDateTime(row, "approval_date"),
                    PrimaryKeyId = UnpackInt(row, "pinfile_id")
                    


                };
                return pin_block;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private TerminalCardData UnpackPinHeaderCardsListResult(DataRow row)
        {
            try
            {

                TerminalCardData pin_block = new TerminalCardData
                {
                  
                    PAN = UnpackString(row, "file_masked_pan"),
                    ProductName = UnpackString(row, "file_product_name"),
                    ExpiryDate = UnpackDateTime(row, "file_expiry_date"),
                    CardId = UnpackString(row, "card_id"),
                 //   PINFileUploadDate = UnpackDateTime(row, "file_upload_date"),
                    DummyPan = UnpackString(row, "file_dummy_pan"),
                 //   approval_status = UnpackString(row, "approval_status"),
                 //   approval_comment = UnpackString(row, "approval_comment"),
                 //   approval_date = UnpackDateTime(row, "approval_date"),
                 //   PrimaryKeyId = UnpackInt(row, "pinfile_id")



                };
                return pin_block;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private TerminalCardData UnpackPinBatchListResult(DataRow row)
        {
            try
            {

                TerminalCardData pin_block = new TerminalCardData
                {
                    header_pin_file_batch_id = UnpackInt(row, "pin_file_batch_id"),
                    header_number_of_cards_on_request = UnpackInt(row, "number_of_cards_on_request"),
                    header_batch_reference = UnpackString(row, "batch_reference"),
                    header_upload_date = UnpackDateTime(row, "upload_date"),
                    header_approval_status = UnpackString(row, "approval_status"),
                    header_approval_comment = UnpackString(row, "approval_comment"),
                    header_approval_date = UnpackDateTime(row, "approval_date")


                };
                return pin_block;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private TerminalCardData UnpackPinMailerBatchHeaderResult(DataRow row)
        {
            try
            {

                TerminalCardData pin_block = new TerminalCardData
                {
                   // header_pin_file_batch_id = UnpackInt(row, "pin_file_batch_id"),
                    header_number_of_cards_on_request = UnpackInt(row, "number_of_cards_on_request"),
                    header_batch_reference = UnpackString(row, "batch_reference"),
                    header_upload_date = UnpackDateTime(row, "status_date"),
                  //  header_approval_status = UnpackString(row, "approval_status"),
                  //  header_approval_comment = UnpackString(row, "approval_comment"),
                  //  header_approval_date = UnpackDateTime(row, "approval_date")


                };
                return pin_block;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private PinRequestViewDetails UnpackPinRequestListViewResult(DataRow row)
        {
            try
            {
   
        //public int expiry_year { get; set; }
        //public string expiry_month { get; set; }
        

        PinRequestViewDetails pin_request = new PinRequestViewDetails
                {
                    pin_request_id = UnpackLong(row, "pin_request_id"),
                    product_id = UnpackInt(row, "product_id"),
                    last_for_digit_of_pan = UnpackString(row, "pan_last_four_digits"),
                    product_bin_code = UnpackString(row, "product_bin_code"),
                    product_name = UnpackString(row, "product_name"),
                    issuer_id = UnpackInt(row, "issuer_id"),
                    pin_request_reference = UnpackString(row, "pin_request_reference"),
                    pin_request_status = UnpackString(row, "pin_status"),
                    expiry_period = UnpackInt(row, "expiry_period"),
                    pin_create_date = UnpackDateTime(row, "created_date"),
                    issuer_name = UnpackString(row, "issuer_name"),
                    branch_name = UnpackString(row, "main_branch"),
                    masked_pan = UnpackString(row, "masked_pan"),
                    pin_dist_method = UnpackString(row, "pin_distribution_channel"),
                    account_number = UnpackString(row, "customer_account_number"),
                    account_email = UnpackString(row, "customer_email"),
                    account_contact = UnpackString(row, "customer_contact_number"),
                    account_name = UnpackString(row, "customer_name"),
                    last_send_date = UnpackDateTime(row, "last_send_date"),
                    number_of_times_sent = UnpackInt(row, "number_of_times_sent"),
                    reject_date = UnpackDateTime(row, "reject_date"),
                    reject_reason = UnpackString(row, "reject_reason")

        };
                return pin_request;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //TerminalCardData
        private TerminalCardData UnpackPinFileLoaderViewResult(DataRow row)
        {
            try
            {

                //public int expiry_year { get; set; }
                //public string expiry_month { get; set; }


                TerminalCardData pin_block = new TerminalCardData
                {
                    PINBlock = UnpackString(row, "pinfile_encpin_block"),
                    PAN = UnpackString(row, "file_dummy_pan"),
                    approval_status = UnpackString(row, "approval_status")                 
                     

                };
                return pin_block;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private RestWebservicesHandler UnpackRestResult(DataRow row)
        {
            try
            {
   
        RestWebservicesHandler rest_web = new RestWebservicesHandler
                {
                issuer_id = UnpackInt(row, "issuer_id"),
                webservices_type = UnpackString(row, "webservice_type"),
                rest_url = UnpackString(row, "rest_url"),
            param_one_name = UnpackString(row, "param_one_name"),
            param_one_value = UnpackString(row, "param_one_value"),
            param_two_name = UnpackString(row, "param_two_name"),
            param_two_value = UnpackString(row, "param_two_value"),
            param_three_name = UnpackString(row, "param_three_name"),
            param_three_value = UnpackString(row, "param_three_value"),
            param_four_name = UnpackString(row, "param_four_name"),
            param_four_value = UnpackString(row, "param_four_value"),
            param_five_name = UnpackString(row, "param_five_name"),
            param_five_value = UnpackString(row, "param_five_value"),
            param_six_name = UnpackString(row, "param_six_name"),
            param_six_value = UnpackString(row, "param_six_value"),
            param_seven_name = UnpackString(row, "param_seven_name"),
            param_seven_value = UnpackString(row, "param_seven_value"),
            param_eight_name = UnpackString(row, "param_eight_name"),
            param_eight_value = UnpackString(row, "param_eight_value"),
            param_nine_name = UnpackString(row, "param_nine_name"),
            param_nine_value = UnpackString(row, "param_nine_value"),
            param_ten_name = UnpackString(row, "param_ten_name"),
            param_ten_value = UnpackString(row, "param_ten_value"),
            webservice_params_id = UnpackInt(row, "restwebservice_params_id")

        };
                return rest_web;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ZMKZPKDetails UnpackZoneKeysResult(DataRow row)
        {
            try
            {

                ZMKZPKDetails ZMKZPK_keys = new ZMKZPKDetails
                {
                    Final = UnpackString(row, "zone"),
                    Zone = UnpackString(row, "final"),
                    RandomKey = UnpackString(row, "RandomKey"),
                    RandomKeyUnderLMK = UnpackString(row, "RandomKeyUnderLMK")

                };
                return ZMKZPK_keys;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ProductlistResult UnpackProductListResult(DataRow row)
        {
            try
            {
                ProductlistResult issuer_product = new ProductlistResult
                {
                    product_id = UnpackInt(row, "product_id"),
                    product_code = UnpackString(row, "product_code"),
                    product_name = UnpackString(row, "product_name"),
                    product_bin_code = UnpackString(row, "product_bin_code"),
                    issuer_id = UnpackInt(row, "issuer_id"),
                    name_on_card_top = UnpackDecimal(row, "name_on_card_top"),
                    name_on_card_left = UnpackDecimal(row, "name_on_card_left"),
                    Name_on_card_font_size = UnpackInt(row, "Name_on_card_font_size"),
                    font_id = UnpackInt(row, "font_id"),
                    DeletedYN = UnpackBoolean(row, "DeletedYN"),
                    src1_id = UnpackInt(row, "src1_id"),
                    src2_id = UnpackInt(row, "src2_id"),
                    src3_id = UnpackInt(row, "src3_id"),
                    PVKI = UnpackString(row, "PVKI"),
                    PVK = UnpackString(row, "PVK"),
                    CVKA = UnpackString(row, "CVKA"),
                    CVKB = UnpackString(row, "CVKB"),
                    expiry_months = UnpackInt(row, "expiry_months"),
                    fee_scheme_id = UnpackInt(row, "fee_scheme_id"),
                    enable_instant_pin_YN = UnpackBoolean(row, "enable_instant_pin_YN"),
                    min_pin_length = UnpackInt(row, "min_pin_length"),
                    max_pin_length = UnpackInt(row, "max_pin_length"),
                    enable_instant_pin_reissue_YN = UnpackBoolean(row, "enable_instant_pin_reissue_YN"),
                    cms_exportable_YN = UnpackBoolean(row, "cms_exportable_YN"),
                    product_load_type_id = UnpackInt(row, "product_load_type_id"),
                    sub_product_code = UnpackString(row, "sub_product_code"),
                    pin_calc_method_id = UnpackInt(row, "pin_calc_method_id"),
                    auto_approve_batch_YN = UnpackBoolean(row, "auto_approve_batch_YN"),
                    account_validation_YN = UnpackBoolean(row, "account_validation_YN"),
                    pan_length = UnpackShort(row, "pan_length"),
                    pin_mailer_printing_YN = UnpackBoolean(row, "pin_mailer_printing_YN"),
                    pin_mailer_reprint_YN = UnpackBoolean(row, "pin_mailer_reprint_YN"),
                    card_issue_method_id = UnpackInt(row, "card_issue_method_id"),
                    decimalisation_table = UnpackString(row, "decimalisation_table"),
                    pin_validation_data = UnpackString(row, "pin_validation_data"),
                    pin_block_formatid = UnpackInt(row, "pin_block_formatid"),
                    production_dist_batch_status_flow = UnpackInt(row, "production_dist_batch_status_flow"),
                    distribution_dist_batch_status_flow = UnpackInt(row, "distribution_dist_batch_status_flow"),
                    charge_fee_to_issuing_branch_YN = UnpackBoolean(row, "charge_fee_to_issuing_branch_YN"),
                    print_issue_card_YN = UnpackBoolean(row, "print_issue_card_YN"),
                    allow_manual_uploaded_YN = UnpackBoolean(row, "allow_manual_uploaded_YN"),
                    allow_reupload_YN = UnpackBoolean(row, "allow_reupload_YN"),
                    remote_cms_update_YN = UnpackBoolean(row, "remote_cms_update_YN"),
                    charge_fee_at_cardrequest = UnpackBoolean(row, "charge_fee_at_cardrequest"),
                    e_pin_request_YN = UnpackBoolean(row, "e_pin_request_YN"),
                    pin_account_validation_YN = UnpackBoolean(row, "pin_account_validation_YN"),
                    renewal_activate_card = UnpackBoolean(row, "renewal_activate_card"),
                    renewal_charge_card = UnpackBoolean(row, "renewal_charge_card"),
                    credit_limit_capture = UnpackBoolean(row, "credit_limit_capture"),
                    credit_limit_update = UnpackBoolean(row, "credit_limit_update"),
                    credit_limit_approve = UnpackBoolean(row, "credit_limit_approve"),
                    email_required = UnpackBoolean(row, "email_required"),
                    generate_contract_number = UnpackBoolean(row, "generate_contract_number"),
                    manual_contract_number = UnpackBoolean(row, "manual_contract_number"),
                    parallel_approval = UnpackBoolean(row, "parallel_approval"),
                    activation_by_center_operator = UnpackBoolean(row, "activation_by_center_operator"),
                    credit_contract_last_sequence = UnpackLong(row, "credit_contract_last_sequence"),
                    credit_contract_prefix = UnpackString(row, "credit_contract_prefix"),
                    credit_contract_suffix_format = UnpackString(row, "credit_contract_suffix_format"),
                    distribution_dist_batch_status_flow_renewal = UnpackInt(row, "distribution_dist_batch_status_flow_renewal"),
                    production_dist_batch_status_flow_renewal = UnpackInt(row, "production_dist_batch_status_flow_renewal"),
                    Is_mtwenty_printed = UnpackBoolean(row,"is_m_twenty_printed")
               
                };
                return issuer_product;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual ProductlistResult usp_getProductbyProductid(Nullable<int> productid)
        {
            ProductlistResult result = new ProductlistResult();
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("productid",SqlDbType.Int, 0, productid)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_getProductbyProductid"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result = UnpackProductListResult(row);
                }
            }
            return result;
        }


        public virtual List<ProductAccountTypesResult> usp_get_product_account_types(int? product_id, int? language_id, long? audit_user_id, string audit_workstation)
        {
            List<ProductAccountTypesResult> result = new List<ProductAccountTypesResult>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("product_id",SqlDbType.Int, 0, product_id),
                CreateSqlParameter("language_id", SqlDbType.Int, 0, language_id),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, audit_user_id),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, audit_workstation)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_product_account_types"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnpackProductAccountTypesResult(row));
                }
            }
            return result;
        }

        private ProductAccountTypesResult UnpackProductAccountTypesResult(DataRow row)
        {
            try
            {
                ProductAccountTypesResult products_account_types = new ProductAccountTypesResult
                {
                    product_id = UnpackInt(row, "product_id"),
                    account_type_id = UnpackInt(row, "account_type_id")
                };
                return products_account_types;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ProductIssueReasonsResult> usp_get_product_issue_reasons(int? product_id, int? language_id, long? audit_user_id, string audit_workstation)
        {
            List<ProductIssueReasonsResult> result = new List<ProductIssueReasonsResult>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("product_id",SqlDbType.Int, 0, product_id),
                CreateSqlParameter("language_id", SqlDbType.Int, 0, language_id),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, audit_user_id),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, audit_workstation)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_product_issue_reasons"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnpackProductIssueReasonResult(row));
                }
            }
            return result;
        } //usp_get_issuer_pin_message

        



        private ProductIssueReasonsResult UnpackProductIssueReasonResult(DataRow row)
        {
            try
            {
                ProductIssueReasonsResult product_issue_reason = new ProductIssueReasonsResult
                {
                    product_id = UnpackInt(row, "product_id"),
                    card_issue_reason_id = UnpackInt(row, "card_issue_reason_id")
                };
                return product_issue_reason;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual List<CurrencyResult> usp_get_currency_list(int? language_id, long? audit_user_id, string audit_workstation)
        {
            List<CurrencyResult> result = new List<CurrencyResult>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("language_id", SqlDbType.Int, 0, language_id),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, audit_user_id),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, audit_workstation)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_currency_list"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnpackCurrencyResult(row));
                }
            }
            return result;
        }

        private CurrencyResult UnpackCurrencyResult(DataRow row)
        {
            try
            {
                CurrencyResult currency = new CurrencyResult
                {
                    currency_id = UnpackInt(row, "currency_id"),
                    currency_code = UnpackString(row, "currency_code"),
                    iso_4217_numeric_code = UnpackString(row, "iso_4217_numeric_code"),
                    iso_4217_minor_unit = UnpackInt(row, "iso_4217_minor_unit"),
                    currency_desc = UnpackString(row, "currency_desc"),
                    active_YN = UnpackBoolean(row, "active_YN")
                };
                return currency;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual List<DistBatchFlows> usp_get_dist_batch_flows(int? card_issue_method_id, long? audit_user_id, string audit_workstation)
        {
            List<DistBatchFlows> result = new List<DistBatchFlows>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_issue_method_id", SqlDbType.Int, 0, card_issue_method_id),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, audit_user_id),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, audit_workstation)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_dist_batch_flows"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnpackDistBatchFlow(row));
                }
            }
            return result;
        }

        private DistBatchFlows UnpackDistBatchFlow(DataRow row)
        {
            try
            {
                DistBatchFlows dist_batch_status_flow = new DistBatchFlows
                {
                    dist_batch_status_flow_id = UnpackInt(row, "dist_batch_status_flow_id"),
                    dist_batch_status_flow_name = UnpackString(row, "dist_batch_status_flow_name"),
                    Dist_batch_type_id = UnpackInt(row, "dist_batch_type_id"),
                    card_issue_method_id = UnpackInt(row, "card_issue_method_id")
                };
                return dist_batch_status_flow;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
