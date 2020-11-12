using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Renewal.Entities;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.DataAccess;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Renewal.Reports;

namespace Veneka.Indigo.Renewal.dal
{
    public class RenewalDataAccess : BaseDataAccess, IRenewalDataAccess
    {
        public SystemResponseCode ChangeStatus(long id, RenewalStatusType status, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_id", SqlDbType.BigInt,0,id, ParameterDirection.Input),
                CreateSqlParameter("status", SqlDbType.Int, 0, status, ParameterDirection.Input),
                CreateSqlParameter("editor_id", SqlDbType.BigInt, 0, auditUserId, ParameterDirection.Input),
                CreateSqlParameter("edit_date",SqlDbType.DateTime,0,DateTime.Now, ParameterDirection.Input),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,0, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_update"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
                id = Convert.ToInt64(sqlCommand.Parameters["@renewal_id"].Value);

            }
            return SystemResponseCode.SUCCESS;
        }

        public long Create(RenewalFile Renewal, long auditUserId, string auditWorkstation)
        {
            long id = 0;
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("file_name", SqlDbType.NVarChar,400,Renewal.FileName, ParameterDirection.Input),
                CreateSqlParameter("date_uploaded", SqlDbType.Date, 0, Renewal.DateUploaded, ParameterDirection.Input),
                CreateSqlParameter("status", SqlDbType.Int, 0, Renewal.Status, ParameterDirection.Input),
                CreateSqlParameter("creator_id", SqlDbType.BigInt, 0, Renewal.CreatorId, ParameterDirection.Input),
                CreateSqlParameter("create_date",SqlDbType.DateTime,0,Renewal.CreateDate, ParameterDirection.Input),
                CreateSqlParameter("renewal_id", SqlDbType.BigInt, 0, Renewal.Id, ParameterDirection.InputOutput),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,Renewal.Id, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_create"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
                id = Convert.ToInt64(sqlCommand.Parameters["@renewal_id"].Value);

            }
            return id;
        }

        public long CreateDetail(RenewalDetail entity, long auditUserId, string auditWorkstation)
        {
            long id = 0;
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_id", SqlDbType.BigInt, 0, entity.RenewalId,ParameterDirection.Input,19,0),
                CreateSqlParameter("branch_id", SqlDbType.Int, 0, entity.BranchId,ParameterDirection.Input,10,0),
                CreateSqlParameter("delivery_branch_id", SqlDbType.Int, 0, entity.DeliveryBranchId,ParameterDirection.Input,10,0),
                CreateSqlParameter("product_id", SqlDbType.Int, 0, entity.ProductId,ParameterDirection.Input,10,0),
                CreateSqlParameter("card_number", SqlDbType.NVarChar, 250, entity.CardNumber,ParameterDirection.Input,0,0),
                CreateSqlParameter("ps_action", SqlDbType.NVarChar, 2, entity.PSAction,ParameterDirection.Input,0,0),
                CreateSqlParameter("blocked", SqlDbType.NVarChar, 50, entity.Blocked,ParameterDirection.Input,0,0),
                CreateSqlParameter("expiry_date", SqlDbType.DateTime, 0, entity.ExpiryDate,ParameterDirection.Input,0,0),
                CreateSqlParameter("renewal_date", SqlDbType.DateTime, 0, entity.RenewalDate,ParameterDirection.Input,0,0),
                CreateSqlParameter("status", SqlDbType.NVarChar, 20, entity.Status,ParameterDirection.Input,0,0),
                CreateSqlParameter("contract_number", SqlDbType.NVarChar, 250, entity.ContractNumber,ParameterDirection.Input,0,0),
                CreateSqlParameter("currency_code", SqlDbType.NVarChar, 5, entity.CurrencyCode,ParameterDirection.Input,0,0),
                CreateSqlParameter("od_amount", SqlDbType.Decimal, 0, entity.ODAmount,ParameterDirection.Input,18,2),
                CreateSqlParameter("ol_amount", SqlDbType.Decimal, 0, entity.OLAmount,ParameterDirection.Input,18,2),
                CreateSqlParameter("limit_balance", SqlDbType.Decimal, 0, entity.LimitBalance,ParameterDirection.Input,18,2),
                CreateSqlParameter("embossing_name", SqlDbType.NVarChar, 250, entity.EmbossingName,ParameterDirection.Input,0,0),
                CreateSqlParameter("client_id", SqlDbType.NVarChar, 250, entity.ClientId,ParameterDirection.Input,0,0),
                CreateSqlParameter("birth_date", SqlDbType.DateTime, 0, entity.BirthDate,ParameterDirection.Input,0,0),
                CreateSqlParameter("internal_account_number", SqlDbType.NVarChar, 250, entity.InternalAccountNumber,ParameterDirection.Input,0,0),
                CreateSqlParameter("external_account_number", SqlDbType.NVarChar, 250, entity.ExternalAccountNumber,ParameterDirection.Input,0,0),
                CreateSqlParameter("passport_id_number", SqlDbType.NVarChar, 250, entity.PassportIDNumber,ParameterDirection.Input,0,0),
                CreateSqlParameter("contract_status", SqlDbType.NVarChar, 10, entity.ContractStatus,ParameterDirection.Input,0,0),
                CreateSqlParameter("email_address", SqlDbType.NVarChar, 500, entity.EmailAddress,ParameterDirection.Input,0,0),
                CreateSqlParameter("customer_name", SqlDbType.NVarChar, 500, entity.CustomerName,ParameterDirection.Input,0,0),
                CreateSqlParameter("creation_date", SqlDbType.DateTime, 0, entity.CreationDate,ParameterDirection.Input,0,0),
                CreateSqlParameter("online_status", SqlDbType.NVarChar, 50, entity.OnlineStatus,ParameterDirection.Input,0,0),
                CreateSqlParameter("contact_phone", SqlDbType.NVarChar, 50, entity.ContactPhone,ParameterDirection.Input,0,0),
                CreateSqlParameter("mobile_phone", SqlDbType.NVarChar, 50, entity.MobilePhone,ParameterDirection.Input,0,0),
                CreateSqlParameter("renewal_status", SqlDbType.Int, 0, entity.RenewalStatus,ParameterDirection.Input,0,0),
                CreateSqlParameter("mbr", SqlDbType.VarChar, 10, entity.MBR,ParameterDirection.Input,0,0),
                CreateSqlParameter("renewal_detail_id", SqlDbType.BigInt, 0, entity.RenewalDetailId,ParameterDirection.InputOutput,19,0),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,0, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_detail_create"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
                id = Convert.ToInt64(sqlCommand.Parameters["@renewal_detail_id"].Value);

            }
            return id;
        }

        public SystemResponseCode Delete(long id, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_detail_id", SqlDbType.BigInt, 0, id,ParameterDirection.Input,19,0),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,0, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_delete"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return SystemResponseCode.SUCCESS;
        }

        private RenewalFileListModel Unpack(DataRow row)
        {
            try
            {
                RenewalFileListModel result = new RenewalFileListModel
                {
                    Id = UnpackLong(row, "renewal_id"),
                    FileName = UnpackString(row, "file_name"),
                    CreateDate = UnpackDateTime(row, "create_date"),
                    BranchCount = UnpackInt(row, "branch_count"),
                    CardCount = UnpackInt(row, "card_count"),
                    Status = (RenewalStatusType)UnpackInt(row, "status"),
                    CreatedByName = UnpackString(row, "created_by"),
                    DateUploaded = UnpackDateTime(row, "date_uploaded"),
                    ProductCount = UnpackInt(row, "product_count")
                };
                return result;
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        private RenewalFileViewModel UnpackDetailed(DataRow row)
        {
            try
            {
                RenewalFileViewModel result = new RenewalFileViewModel
                {
                    Id = UnpackLong(row, "renewal_id"),
                    FileName = UnpackString(row, "file_name"),
                    CreateDate = UnpackDateTime(row, "create_date"),
                    BranchCount = UnpackInt(row, "branch_count"),
                    CardCount = UnpackInt(row, "card_count"),
                    Status = (RenewalStatusType)UnpackInt(row, "status"),
                    CreatedByName = UnpackString(row, "created_by"),
                    DateUploaded = UnpackDateTime(row, "date_uploaded"),
                    ProductCount = UnpackInt(row, "product_count"),
                    CreatorId = UnpackLong(row, "creator_id")
                };
                return result;
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        public ICollection<RenewalFileListModel> List(RenewalStatusType status, int languageId, long auditUserId, string auditWorkStation)
        {
            List<RenewalFileListModel> result = new List<RenewalFileListModel>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("status", SqlDbType.Int, 0, (int)status),
                CreateSqlParameter("language_id", SqlDbType.Int, 0 , languageId),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkStation,ParameterDirection.Input,0,0),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_list"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                DataTable dataTable = ExecuteQuery(sqlCommand);
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(Unpack(row));
                }
            }
            return result;
        }

        public ICollection<RenewalDetailListModel> ListRenewalDetail(long RenewalId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation)
        {
            List<RenewalDetailListModel> result = new List<RenewalDetailListModel>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_id", SqlDbType.BigInt, 0, (int)RenewalId),
                CreateSqlParameter("check_masking", SqlDbType.Bit, 0, checkMasking  ),
                CreateSqlParameter("language_id", SqlDbType.Int, 0 , languageId),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkStation,ParameterDirection.Input,0,0),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_detail_list"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnpackRenewalDetail(row));
                }
            }
            return result;
        }

        private RenewalDetailListModel UnpackRenewalDetail(DataRow row)
        {
            try
            {
                RenewalDetailListModel renewal_detail = new RenewalDetailListModel
                {
                    RenewalDetailId = UnpackLong(row, "renewal_detail_id"),
                    RenewalId = UnpackLong(row, "renewal_id"),
                    BranchId = UnpackInt(row, "branch_id"),
                    DeliveryBranchId = UnpackInt(row, "delivery_branch_id"),
                    ProductId = UnpackInt(row, "product_id"),
                    CardNumber = UnpackString(row, "card_number"),
                    PSAction = UnpackString(row, "ps_action"),
                    Blocked = UnpackString(row, "blocked"),
                    ExpiryDate = UnpackDateTime(row, "expiry_date"),
                    RenewalDate = UnpackDateTime(row, "renewal_date"),
                    Status = UnpackString(row, "status"),
                    ContractNumber = UnpackString(row, "contract_number"),
                    CurrencyCode = UnpackString(row, "currency_code"),
                    ODAmount = UnpackDecimal(row, "od_amount"),
                    OLAmount = UnpackDecimal(row, "ol_amount"),
                    LimitBalance = UnpackDecimal(row, "limit_balance"),
                    EmbossingName = UnpackString(row, "embossing_name"),
                    ClientId = UnpackString(row, "client_id"),
                    BirthDate = UnpackDateTime(row, "birth_date"),
                    InternalAccountNumber = UnpackString(row, "internal_account_number"),
                    ExternalAccountNumber = UnpackString(row, "external_account_number"),
                    PassportIDNumber = UnpackString(row, "passport_id_number"),
                    ContractStatus = UnpackString(row, "contract_status"),
                    EmailAddress = UnpackString(row, "email_address"),
                    CustomerName = UnpackString(row, "customer_name"),
                    CreationDate = UnpackDateTime(row, "creation_date"),
                    OnlineStatus = UnpackString(row, "online_status"),
                    ContactPhone = UnpackString(row, "contact_phone"),
                    MobilePhone = UnpackString(row, "mobile_phone"),
                    RenewalStatus = (RenewalDetailStatusType)UnpackInt(row, "renewal_status"),
                    ProductCode = UnpackString(row, "product_code"),
                    ProductName = UnpackString(row, "product_name"),
                    BranchCode = UnpackString(row, "branch_code"),
                    BranchName = UnpackString(row, "branch_name"),
                    IssuerId = UnpackInt(row, "issuer_id"),
                    IssuerName = UnpackString(row, "issuer_name"),
                    DeliveryBranchCode = UnpackString(row, "delivery_branch_code"),
                    DeliveryBranchName = UnpackString(row, "delivery_branch_name"),
                    CBSAccountType = UnpackString(row, "cbs_account_type"),
                    CMSAccountType = UnpackString(row, "cms_account_type"),
                    CurrencyId = UnpackInt(row, "currency_id"),
                    CardId = UnpackLong(row, "card_id"),
                    IssuerCode = UnpackString(row, "issuer_code"),
                    MBR = UnpackString(row, "mbr")
                };
                return renewal_detail;
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        public RenewalFileViewModel Retrieve(long id, int languageId, long auditUserId, string auditWorkStation)
        {
            RenewalFileViewModel result = new RenewalFileViewModel();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_id", SqlDbType.BigInt, 0, id)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_get"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result = UnpackDetailed(row);
                }
            }
            return result;
        }

        public RenewalDetailListModel RetrieveDetail(long id, bool checkMasking, int languageId, long auditUserId, string auditWorkStation)
        {
            RenewalDetailListModel result = new RenewalDetailListModel();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_detail_id", SqlDbType.BigInt, 0, id),
                CreateSqlParameter("check_masking", SqlDbType.Bit, 0, checkMasking  ),
                CreateSqlParameter("language_id", SqlDbType.Int, 0 , languageId),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkStation,ParameterDirection.Input,0,0),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_detail_get"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result = UnpackRenewalDetail(row);
                }
            }
            return result;
        }

        public RenewalDetailListModel RetrieveDetailCard(long cardId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation)
        {
            RenewalDetailListModel result = new RenewalDetailListModel();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_id", SqlDbType.BigInt, 0, cardId),
                CreateSqlParameter("check_masking", SqlDbType.Bit, 0, checkMasking  ),
                CreateSqlParameter("language_id", SqlDbType.Int, 0 , languageId),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkStation,ParameterDirection.Input,0,0),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_detail_get_by_card"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result = UnpackRenewalDetail(row);
                }
            }
            return result;
        }

        public SystemResponseCode UpdateDetailStatus(long renewalDetailId, int deliveryBranchId, int currencyId, string comment,
            string cbs_account_type, string cms_account_type, RenewalDetailStatusType statusType, long auditUserId, string auditWorkStation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_detail_id", SqlDbType.BigInt,0,renewalDetailId, ParameterDirection.Input),
                CreateSqlParameter("delivery_branch_id", SqlDbType.Int,0,deliveryBranchId, ParameterDirection.Input),
                CreateSqlParameter("currency_id", SqlDbType.Int,0,currencyId, ParameterDirection.Input),
                CreateSqlParameter("cbs_account_type", SqlDbType.VarChar,50,cbs_account_type, ParameterDirection.Input),
                CreateSqlParameter("cms_account_type", SqlDbType.VarChar,50,cms_account_type, ParameterDirection.Input),
                CreateSqlParameter("status", SqlDbType.Int, 0, statusType, ParameterDirection.Input),
                CreateSqlParameter("comment", SqlDbType.VarChar, 150, comment, ParameterDirection.Input),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,0, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_detail_update"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return SystemResponseCode.SUCCESS;
        }


        public ICollection<branch> GetBranches()
        {
            List<branch> result = new List<branch>();
            using (SqlCommand sqlCommand = new SqlCommand("usp_get_branches"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnPackBranch(row));
                }
            }
            return result;
        }

        private branch UnPackBranch(DataRow row)
        {
            try
            {
                branch result = new branch
                {
                    branch_id = UnpackInt(row, "branch_id"),
                    branch_status_id = UnpackInt(row, "branch_status_id"),
                    issuer_id = UnpackInt(row, "issuer_id"),
                    branch_code = UnpackString(row, "branch_code"),
                    branch_name = UnpackString(row, "branch_name"),
                    location = UnpackString(row, "location"),
                    contact_person = UnpackString(row, "contact_person"),
                    contact_email = UnpackString(row, "contact_email"),
                    card_centre = UnpackString(row, "card_centre"),
                    emp_branch_code = UnpackString(row, "emp_branch_code"),
                    branch_type_id = UnpackInt(row, "branch_type_id"),
                    main_branch_id = UnpackInt(row, "main_branch_id")
                };
                return result;
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        public ICollection<issuer_product> GetProducts()
        {
            List<issuer_product> result = new List<issuer_product>();
            using (SqlCommand sqlCommand = new SqlCommand("usp_get_issuer_products"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnPackProduct(row));
                }
            }
            return result;
        }

        private issuer_product UnPackProduct(DataRow row)
        {
            try
            {
                issuer_product result = new issuer_product
                {
                    product_id = UnpackInt(row, "product_id"),
                    product_code = UnpackString(row, "product_code"),
                    product_name = UnpackString(row, "product_name"),
                    product_bin_code = UnpackString(row, "product_bin_code"),
                    issuer_id = UnpackInt(row, "issuer_id"),
                    sub_product_code = UnpackString(row, "sub_product_code")
                };
                return result;
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        public List<RenewalBatch> CreateBatch(long auditUserId, string auditWorkstation)
        {
            List<long> result = new List<long>();
            List<RenewalBatch> createdBatches = new List<RenewalBatch>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("queued_status", SqlDbType.Int,0, RenewalDetailStatusType.Approved,ParameterDirection.Input,0,0),
                CreateSqlParameter("batched_status", SqlDbType.BigInt, 0, RenewalDetailStatusType.Batched,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkstation,ParameterDirection.Input,0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_batch_create"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnpackLong(row, "renewal_batch_id"));
                }
                foreach (long newId in result)
                {
                    createdBatches.Add(GetBatch(newId));
                }
            }
            return createdBatches;
        }

        public RenewalBatch ApproveBatch(long renewalBatchId, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_batch_id", SqlDbType.BigInt,0, renewalBatchId,ParameterDirection.Input,0,0),
                CreateSqlParameter("renewal_batch_status", SqlDbType.TinyInt,0, RenewalBatchStatusType.Approved,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkstation,ParameterDirection.Input,0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_batch_change_status"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);

            }
            return GetBatch(renewalBatchId);
        }

        public RenewalBatch DistributeBatch(long renewalBatchId, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_batch_id", SqlDbType.BigInt,0, renewalBatchId,ParameterDirection.Input,0,0),
                CreateSqlParameter("renewal_batch_status", SqlDbType.TinyInt,0, RenewalBatchStatusType.Distribution,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkstation,ParameterDirection.Input,0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_batch_change_status"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);

            }
            return GetBatch(renewalBatchId);
        }

        public RenewalBatch RejectBatch(long renewalBatchId, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_batch_id", SqlDbType.BigInt,0, renewalBatchId,ParameterDirection.Input,0,0),
                CreateSqlParameter("renewal_batch_status", SqlDbType.TinyInt,0, RenewalBatchStatusType.Rejected,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkstation,ParameterDirection.Input,0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_batch_change_status"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);

            }
            return GetBatch(renewalBatchId);
        }

        public RenewalBatch GetBatch(long renewalBatchId)
        {
            RenewalBatch result = new RenewalBatch();
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_batch_id", SqlDbType.BigInt,0, renewalBatchId,ParameterDirection.Input,0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_batch_get"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result = UnpackRenewalBatch(row);
                }
            }
            return result;

        }

        private RenewalBatch UnpackRenewalBatch(DataRow row)
        {
            try
            {
                RenewalBatch result = new RenewalBatch
                {
                    CardCount = UnpackInt(row, "card_count"),
                    BranchCount = UnpackInt(row, "branch_count"),
                    ProductCount = UnpackInt(row, "product_count"),
                    RenewalBatchId = UnpackLong(row, "renewal_batch_id"),
                    RenewalBatchReference = UnpackLong(row, "renewal_batch_id").ToString(),
                    RenewalBatchStatus = (RenewalBatchStatusType)UnpackInt(row, "batch_renewal_status"),
                    RenewalDate = UnpackDateTime(row, "created_date"),
                    ProductBin = UnpackString(row, "product_bin_code"),
                    ProductCode = UnpackString(row, "product_code"),
                    ProductName = UnpackString(row, "product_name"),
                };
                result.CalculatedBatchNumber = result.RenewalDate.ToString("yyyyMMdd") + result.RenewalBatchId.ToString();
                return result;
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        public RenewalBatch RetrieveBatch(long renewalBatchId, long auditUserId, string auditWorkstation)
        {
            return GetBatch(renewalBatchId);
        }

        public ICollection<RenewalBatch> RetrieveBatches(RenewalBatchStatusType statusType, long userId, string auditWorkstation)
        {
            List<RenewalBatch> result = new List<RenewalBatch>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("batch_renewal_status", SqlDbType.TinyInt,0, statusType,ParameterDirection.Input,0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_batch_list"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnpackRenewalBatch(row));
                }
            }
            return result;
        }

        public ICollection<RenewalDetailListModel> ListRenewalDetailInStatus(RenewalDetailStatusType statusType, bool checkMasking, int languageId, long auditUserId, string auditWorkStation)
        {
            List<RenewalDetailListModel> result = new List<RenewalDetailListModel>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_detail_status", SqlDbType.TinyInt, 0, (int)statusType),
                CreateSqlParameter("check_masking", SqlDbType.Bit, 0, checkMasking  ),
                CreateSqlParameter("language_id", SqlDbType.Int, 0 , languageId),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkStation,ParameterDirection.Input,0,0),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_detail_list_in_status"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnpackRenewalDetail(row));
                }
            }
            return result;
        }

        public ICollection<RenewalDetailListModel> RetrieveBatchDetails(long renewalBatchId, bool checkMasking, int languageId, long auditUserId, string auditWorkStation)
        {
            List<RenewalDetailListModel> result = new List<RenewalDetailListModel>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_batch_id", SqlDbType.BigInt, 0, (int)renewalBatchId),
                CreateSqlParameter("check_masking", SqlDbType.Bit, 0, checkMasking  ),
                CreateSqlParameter("language_id", SqlDbType.Int, 0 , languageId),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkStation,ParameterDirection.Input,0,0),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_batch_detail_list"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnpackRenewalDetail(row));
                }
            }
            return result;
        }

        public RenewalBatch ChangeBatchStatus(long renewalBatchId, RenewalBatchStatusType statusType, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("renewal_batch_id", SqlDbType.BigInt,0, renewalBatchId,ParameterDirection.Input,0,0),
                CreateSqlParameter("renewal_batch_status", SqlDbType.TinyInt,0, statusType,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkstation,ParameterDirection.Input,0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_batch_change_status"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);

            }
            return GetBatch(renewalBatchId);
        }

        public long CreateRenewedCard(long renewalDetailId, RenewalResponseDetail entity)
        {
            long id = 0;
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("card_number", SqlDbType.NVarChar, 250, entity.CardNumber,ParameterDirection.Input,0,0),
                CreateSqlParameter("ps_action", SqlDbType.NVarChar, 2, entity.PSAction,ParameterDirection.Input,0,0),
                CreateSqlParameter("blocked", SqlDbType.NVarChar, 50, entity.Blocked,ParameterDirection.Input,0,0),
                CreateSqlParameter("expiry_date", SqlDbType.DateTime, 0, entity.ExpiryDate,ParameterDirection.Input,0,0),
                CreateSqlParameter("renewal_date", SqlDbType.DateTime, 0, entity.RenewalDate,ParameterDirection.Input,0,0),
                CreateSqlParameter("status", SqlDbType.NVarChar, 20, entity.Status,ParameterDirection.Input,0,0),
                CreateSqlParameter("contract_number", SqlDbType.NVarChar, 250, entity.ContractNumber,ParameterDirection.Input,0,0),
                CreateSqlParameter("currency_code", SqlDbType.NVarChar, 5, entity.CurrencyCode,ParameterDirection.Input,0,0),
                CreateSqlParameter("od_amount", SqlDbType.Decimal, 0, entity.ODAmount,ParameterDirection.Input,18,2),
                CreateSqlParameter("ol_amount", SqlDbType.Decimal, 0, entity.OLAmount,ParameterDirection.Input,18,2),
                CreateSqlParameter("limit_balance", SqlDbType.Decimal, 0, entity.LimitBalance,ParameterDirection.Input,18,2),
                CreateSqlParameter("embossing_name", SqlDbType.NVarChar, 250, entity.EmbossingName,ParameterDirection.Input,0,0),
                CreateSqlParameter("client_id", SqlDbType.NVarChar, 250, entity.ClientId,ParameterDirection.Input,0,0),
                CreateSqlParameter("birth_date", SqlDbType.DateTime, 0, entity.BirthDate,ParameterDirection.Input,0,0),
                CreateSqlParameter("internal_account_number", SqlDbType.NVarChar, 250, entity.InternalAccountNumber,ParameterDirection.Input,0,0),
                CreateSqlParameter("external_account_number", SqlDbType.NVarChar, 250, entity.ExternalAccountNumber,ParameterDirection.Input,0,0),
                CreateSqlParameter("passport_id_number", SqlDbType.NVarChar, 250, entity.PassportIDNumber,ParameterDirection.Input,0,0),
                CreateSqlParameter("contract_status", SqlDbType.NVarChar, 10, entity.ContractStatus,ParameterDirection.Input,0,0),
                CreateSqlParameter("email_address", SqlDbType.NVarChar, 500, entity.EmailAddress,ParameterDirection.Input,0,0),
                CreateSqlParameter("customer_name", SqlDbType.NVarChar, 500, entity.CustomerName,ParameterDirection.Input,0,0),
                CreateSqlParameter("creation_date", SqlDbType.DateTime, 0, entity.CreationDate,ParameterDirection.Input,0,0),
                CreateSqlParameter("online_status", SqlDbType.NVarChar, 50, entity.OnlineStatus,ParameterDirection.Input,0,0),
                CreateSqlParameter("contact_phone", SqlDbType.NVarChar, 50, entity.ContactPhone,ParameterDirection.Input,0,0),
                CreateSqlParameter("mobile_phone", SqlDbType.NVarChar, 50, entity.MobilePhone,ParameterDirection.Input,0,0),
                CreateSqlParameter("renewal_detail_id", SqlDbType.BigInt, 0, renewalDetailId,ParameterDirection.Input,19,0),
                CreateSqlParameter("renewal_confirmation_id", SqlDbType.BigInt, 0, id,ParameterDirection.Input,19,0),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,0, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_card_create"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
                id = Convert.ToInt64(sqlCommand.Parameters["@renewal_confirmation_id"].Value);

            }
            return id;
        }

        public SystemResponseCode LinkRenewalToCard(long renewalDetailId, long cardId, string cardNumber, long auditUserId, string auditWorkstation)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>
                {
                    CreateSqlParameter("renewal_detail_id", SqlDbType.BigInt, 0, renewalDetailId),
                    CreateSqlParameter("card_id", SqlDbType.BigInt, 0, cardId  ),
                };

                using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_link_card"))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                    ExecuteNonQuery(sqlCommand);
                }
                return SystemResponseCode.SUCCESS;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<long> CreateRenewalDistributionBatches(long renewalBatchId, long auditUserId, string auditWorkstation)
        {
            try
            {
                CreateRenewalDistributionBatch(renewalBatchId, auditUserId, auditWorkstation);

                ChangeBatchStatus(renewalBatchId, RenewalBatchStatusType.Distribution, auditUserId, auditWorkstation);
                return RenewalDistributionBatches(renewalBatchId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<long> RenewalDistributionBatches(long renewal_batch_id)
        {
            try
            {
                List<long> result = new List<long>();

                List<SqlParameter> sqlParameters = new List<SqlParameter>
                {
                    CreateSqlParameter("renewal_batch_id", SqlDbType.BigInt, 0, renewal_batch_id, ParameterDirection.Input),
                };

                using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_dist_batches_created"))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                    foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                    {
                        result.Add(UnpackLong(row, "dist_batch_id"));
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CreateRenewalDistributionBatch(long renewal_batch_id, long auditUserId, string auditWorkstation)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>
                {
                    CreateSqlParameter("renewal_batch_id", SqlDbType.BigInt, 0, renewal_batch_id, ParameterDirection.Input),
                    CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId, ParameterDirection.Input),
                    CreateSqlParameter("audit_workstation", SqlDbType.VarChar, 100, auditWorkstation, ParameterDirection.Input),
                };

                using (SqlCommand sqlCommand = new SqlCommand("usp_request_create_dist_batch_renewal"))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                    ExecuteNonQuery(sqlCommand);

                }
                return true; ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int NextSequenceNumber(string sequenceName, ResetPeriod resetPeriod)
        {

            using (SqlCommand command = new SqlCommand("usp_get_next_sequence"))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@sequence_name", SqlDbType.VarChar).Value = sequenceName;
                command.Parameters.Add("@reset_period", SqlDbType.Int).Value = (int)resetPeriod;
                command.Connection = dataBaseConnection.SQLConnection;
                var seqNumber = command.ExecuteScalar();
                return int.Parse(seqNumber.ToString());
            }
        }

        public List<ReportField> GetReportFields(int reportId, int languageId)
        {
            List<ReportField> result = new List<ReportField>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("reportid", SqlDbType.Int, 0, reportId),
                CreateSqlParameter("languageid", SqlDbType.Int, 0, languageId)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_report_fields_list"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result.Add(UnPackReportField(row));
                }
            }
            return result;
        }

        private ReportField UnPackReportField(DataRow row)
        {
            try
            {
                ReportField result = new ReportField
                {
                    FieldId = UnpackInt(row, "reportfieldid"),
                    FieldText = UnpackString(row, "language_text"),
                };
                return result;
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        public bool CardPANMBRExists(string pan, int mbr)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("pan", SqlDbType.VarChar, 50, pan,ParameterDirection.Input,0,0),
                CreateSqlParameter("mbr", SqlDbType.Int, 0, mbr,ParameterDirection.Input,0,0),
                CreateSqlParameter("entry_found", SqlDbType.Bit,0,0, ParameterDirection.Output, 0,0)
            };

                using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_card_pan_mbr_check"))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                    ExecuteNonQuery(sqlCommand);
                    bool exists = Convert.ToBoolean(sqlCommand.Parameters["@entry_found"].Value);
                    return exists;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool RenewalPANMBRExists(string pan, int mbr)
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("pan", SqlDbType.VarChar, 50, pan,ParameterDirection.Input,0,0),
                CreateSqlParameter("mbr", SqlDbType.Int, 0, mbr,ParameterDirection.Input,0,0),
                CreateSqlParameter("entry_found", SqlDbType.Bit,0,0, ParameterDirection.Output, 0,0)
            };

                using (SqlCommand sqlCommand = new SqlCommand("usp_renewal_renewal_pan_mbr_check"))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                    ExecuteNonQuery(sqlCommand);
                    bool exists = Convert.ToBoolean(sqlCommand.Parameters["@entry_found"].Value);
                    return exists;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}