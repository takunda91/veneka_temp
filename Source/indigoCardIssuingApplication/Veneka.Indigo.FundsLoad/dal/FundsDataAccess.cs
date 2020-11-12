using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common;

namespace Veneka.Indigo.FundsLoad.dal
{
    public class FundsDataAccess : BaseDataAccess, IFundsDataAccess
    {
        public SystemResponseCode ApprovalAccept(long id, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("funds_load_id", SqlDbType.BigInt, 0, id),
                CreateSqlParameter("approver_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("approve_date", SqlDbType.DateTime, 0, DateTime.Now,ParameterDirection.Input,0,0),
                CreateSqlParameter("approve_accepted", SqlDbType.Bit, 0, true,ParameterDirection.Input,0,0),
                CreateSqlParameter("status", SqlDbType.Int,0,FundsLoadStatusType.Approved,0,0),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,0, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_funds_load_approve"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return SystemResponseCode.SUCCESS;
        }

        public SystemResponseCode ApprovalReject(long id, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("funds_load_id", SqlDbType.BigInt, 0, id),
                CreateSqlParameter("approver_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("approve_date", SqlDbType.DateTime, 0, DateTime.Now,ParameterDirection.Input,0,0),
                CreateSqlParameter("approve_accepted", SqlDbType.Bit, 0, false,ParameterDirection.Input,0,0),
                CreateSqlParameter("status", SqlDbType.Int,0,FundsLoadStatusType.Approved,0,0),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,0, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_funds_load_approve"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return SystemResponseCode.SUCCESS;
        }

        public SystemResponseCode Delete(long id, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public ICollection<FundsLoadListModel> ListByCard(string cardNumber, bool checkMasking, long auditUserId, string auditWorkStation)
        {
            List<FundsLoadListModel> result = new List<FundsLoadListModel>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("prepaid_card_no", SqlDbType.Int, 0, cardNumber),
                CreateSqlParameter("check_masking", SqlDbType.Bit, 0, checkMasking),
                CreateSqlParameter("language_id", SqlDbType.Int, 0 , 0),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkStation,ParameterDirection.Input,0,0),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_funds_load_list_prepaid_card"))
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

        private FundsLoadListModel Unpack(DataRow row)
        {
            try
            {
                FundsLoadListModel country = new FundsLoadListModel
                {
                    Id = UnpackInt(row, "funds_load_id"),
                    BankAccountNo = UnpackString(row, "bank_account_no"),
                    BranchCode = UnpackString(row, "branch_code"),
                    BranchName = UnpackString(row, "branch_name"),
                    IssuerCode = UnpackString(row, "issuer_code"),
                    IssuerName = UnpackString(row, "issuer_name"),
                    PrepaidCardNo = UnpackString(row, "prepaid_card_no"),
                    Created = UnpackDateTime(row, "create_date"),
                    Amount = UnpackDecimal(row, "amount"),
                    Status = (FundsLoadStatusType)UnpackInt(row, "status"),
                    ProductId = UnpackInt(row, "product_id"),
                    IssuerId = UnpackInt(row, "issuer_id"),
                    BranchId = UnpackInt(row, "branch_id"),
                    CreatedBy = UnpackString(row, "created_by"),
                    ReviewedBy = UnpackString(row, "reviewed_by"),
                    ApprovedBy = UnpackString(row, "approved_by"),
                    LoadedBy = UnpackString(row, "loaded_by"),
                    Address = UnpackString(row, "address"),
                    Firstname = UnpackString(row, "first_name"),
                    LastName = UnpackString(row, "last_name"),
                    PrepaidAccountNo = UnpackString(row, "prepaid_account_no"),
                    Reviewed = UnpackDateTime(row, "review_date"),
                    Approved = UnpackDateTime(row, "approve_date"),
                    Loaded = UnpackDateTime(row, "load_date")
                };
                if (country.Reviewed == new DateTime(1900, 1, 1))
                {
                    country.Reviewed = null;
                }
                if (country.Approved == new DateTime(1900, 1, 1))
                {
                    country.Approved = null;
                }
                if (country.Loaded == new DateTime(1900, 1, 1))
                {
                    country.Loaded = null;
                }
                return country;
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        public ICollection<FundsLoadListModel> List(FundsLoadStatusType statusType, int issuerId, int branchId, bool checkMasking, long auditUserId, string auditWorkStation)
        {
            List<FundsLoadListModel> result = new List<FundsLoadListModel>();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("status", SqlDbType.Int, 0, (int)statusType),
                CreateSqlParameter("issuer_id", SqlDbType.Int, 0, issuerId),
                CreateSqlParameter("branch_id", SqlDbType.Int, 0, branchId),
                CreateSqlParameter("check_masking", SqlDbType.Bit, 0, checkMasking),
                CreateSqlParameter("language_id", SqlDbType.Int, 0 , 0),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkStation,ParameterDirection.Input,0,0),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_funds_load_list"))
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

        public SystemResponseCode Load(long id, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("funds_load_id", SqlDbType.BigInt, 0, id),
                CreateSqlParameter("loader_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("load_date", SqlDbType.DateTime, 0, DateTime.Now,ParameterDirection.Input,0,0),
                CreateSqlParameter("status", SqlDbType.Int,0,FundsLoadStatusType.Loaded,0,0),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,0, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_funds_load_load"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return SystemResponseCode.SUCCESS;
        }

        public FundsLoadListModel Retrieve(long fundsLoadId, bool checkMasking, long auditUserId, string auditWorkStation)
        {
            FundsLoadListModel result = new FundsLoadListModel();
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("funds_load_id", SqlDbType.BigInt, 0, fundsLoadId),
                CreateSqlParameter("check_masking", SqlDbType.Bit, 0, checkMasking),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, auditWorkStation,ParameterDirection.Input,0,0),
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_funds_load_get"))
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

        public SystemResponseCode ReviewAccept(long id, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("funds_load_id", SqlDbType.BigInt, 0, id),
                CreateSqlParameter("reviewer_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("review_date", SqlDbType.DateTime, 0, DateTime.Now,ParameterDirection.Input,0,0),
                CreateSqlParameter("review_accepted", SqlDbType.Bit, 0, true,ParameterDirection.Input,0,0),
                CreateSqlParameter("status", SqlDbType.Int,0,FundsLoadStatusType.Reviewed,0,0),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,0, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_funds_load_review"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return SystemResponseCode.SUCCESS;
        }

        public SystemResponseCode ReviewReject(long id, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("funds_load_id", SqlDbType.BigInt, 0, id),
                CreateSqlParameter("reviewer_id", SqlDbType.BigInt, 0, auditUserId,ParameterDirection.Input,0,0),
                CreateSqlParameter("review_date", SqlDbType.DateTime, 0, DateTime.Now,ParameterDirection.Input,0,0),
                CreateSqlParameter("review_accepted", SqlDbType.Bit, 0, false,ParameterDirection.Input,0,0),
                CreateSqlParameter("status", SqlDbType.Int,0,FundsLoadStatusType.Reviewed,0,0),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,0, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_funds_load_review"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return SystemResponseCode.SUCCESS;
        }

        public SystemResponseCode Save(FundsLoadModel entity, long auditUserId, string auditWorkStation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Int, 0, entity.IssuerId),
                CreateSqlParameter("branch_id", SqlDbType.Int, 0, entity.BranchId),
                CreateSqlParameter("product_id", SqlDbType.Int, 0, entity.ProductId),
                CreateSqlParameter("bank_account_no", SqlDbType.NVarChar, 100, entity.BankAccountNo),
                CreateSqlParameter("prepaid_card_no", SqlDbType.NVarChar, 100, entity.PrepaidCardNo,ParameterDirection.Input,0,0),
                CreateSqlParameter("prepaid_account_no", SqlDbType.NVarChar, 100, entity.PrepaidAccountNo,ParameterDirection.Input,0,0),
                CreateSqlParameter("first_name", SqlDbType.NVarChar, 100, entity.Firstname,ParameterDirection.Input,0,0),
                CreateSqlParameter("last_name", SqlDbType.NVarChar, 100, entity.LastName,ParameterDirection.Input,0,0),
                CreateSqlParameter("address", SqlDbType.NVarChar, 100, entity.Address,ParameterDirection.Input,0,0),
                CreateSqlParameter("amount", SqlDbType.Decimal, 0, entity.Amount,ParameterDirection.Input,18,2),
                CreateSqlParameter("status", SqlDbType.Int,0,entity.Status,0,0),
                CreateSqlParameter("creator_id", SqlDbType.BigInt, 0, auditUserId),
                CreateSqlParameter("create_date", SqlDbType.DateTime, 0, DateTime.Now),
                CreateSqlParameter("funds_load_id", SqlDbType.BigInt,0,entity.Id, ParameterDirection.Output, 0,0),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,entity.Id, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_funds_load_create"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return SystemResponseCode.SUCCESS;
        }

        public SystemResponseCode SendSMS(long id, long auditUserId, string auditWorkstation)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("funds_load_id", SqlDbType.BigInt, 0, id),
                CreateSqlParameter("sms_sent_date", SqlDbType.DateTime, 0, DateTime.Now),
                CreateSqlParameter("status", SqlDbType.Int,0,FundsLoadStatusType.SMSSent,0,0),
                CreateSqlParameter("ResultCode", SqlDbType.Int,0,0, ParameterDirection.Output, 0,0)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_funds_load_sms_send"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                ExecuteNonQuery(sqlCommand);
            }
            return SystemResponseCode.SUCCESS;
        }

        public ICollection<int> ProductsConfiguredForFundsLoad()
        {
            List<int> productIds = new List<int>();
            using (SqlCommand sqlCommand = new SqlCommand("usp_funds_load_product_list"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    productIds.Add(UnpackInt(row, "product_id"));
                }
            }
            return productIds;
        }
    }
}
