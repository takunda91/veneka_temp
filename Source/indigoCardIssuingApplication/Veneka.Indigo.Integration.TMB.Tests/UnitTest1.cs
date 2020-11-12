using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.TMB.Tests
{
    [TestClass]
    public class UnitTest1
    {
        string connectionString = "data source =.; initial catalog = Indigo_TMB_2.1.4.0; integrated security = True; Connection Timeout = 1200; MultipleActiveResultSets = True;";

        [TestMethod]
        public void TestMethod1()
        {

            List<DistBatchCardDetail> result = new List<DistBatchCardDetail>();
            using (SqlConnection sqlconnection = new SqlConnection(connectionString))
            {
                sqlconnection.Open();
                using (SqlCommand command = sqlconnection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_dist_batch_card_details]";

                    command.Parameters.Add("@dist_batch_id", SqlDbType.BigInt).Value = 20084;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = 1;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = "System";

                    foreach (DataRow row in ExecuteQuery(command).Rows)
                    {
                        result.Add(UnPackData(row));
                    }
                }
            }

            Config.IConfig config = null;
            External.ExternalSystemFields externalFields;


            List<CardObject> cardObjects = FetchCardObjectsForDistBatch(result);
            //IntegrationController _integration = new IntegrationController();
            if (cardObjects.Count > 0)
            {
                int productId = cardObjects.FirstOrDefault().ProductId;
                externalFields = GetExternalSystemFields((int)External.ExternalSystemType.CardManagementSystem,
                                                            productId, 0, SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION);

                config = GetProductInterfaceConfigs(productId, (int)InterfaceTypes.CARD_MANAGEMENT_SYSTEM,
                                                            (int)InterfaceArea.PRODUCTION,
                                                            null,
                                                            SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION).First();
                //_integration.CardManagementSystem(cardObjects[0].ProductId, InterfaceArea.PRODUCTION, out externalFields, out config);

                CreditCardCMS processor = new CreditCardCMS();

                processor.UploadGeneratedCards(cardObjects, externalFields, config, 1, 1, "System", out string response);
            }
        }

        private DistBatchCardDetail UnPackData(DataRow row)
        {
            DistBatchCardDetail entry = new DistBatchCardDetail()
            {
                card_number = UnpackString(row, "card_number"),
                card_reference_number = UnpackString(row, "card_reference_number"),
                branch_id = UnpackInt(row, "branch_id"),
                card_id = UnpackLong(row, "card_id"),
                card_issue_method_id = UnpackInt(row, "card_issue_method_id"),
                card_priority_id = UnpackInt(row, "card_priority_id"),
                card_request_reference = UnpackString(row, "card_request_reference"),
                card_sequence = UnpackInt(row, "card_sequence"),
                product_id = UnpackInt(row, "product_id"),
                card_activation_date = UnpackDateTime(row, "card_activation_date"),
                card_expiry_date = UnpackDateTime(row, "card_expiry_date"),
                card_production_date = UnpackDateTime(row, "card_production_date"),
                pvv = UnpackString(row, "pvv"),
                dist_batch_reference = UnpackString(row, "dist_batch_reference"),
                dist_batch_id = UnpackLong(row, "dist_batch_id"),
                dist_card_status_id = UnpackInt(row, "dist_card_status_id"),
                dist_card_status_name = UnpackString(row, "dist_card_status_name"),
                account_type_id = UnpackInt(row, "account_type_id"),
                cms_id = UnpackString(row, "cms_id"),
                contract_number = UnpackString(row, "contract_number"),
                currency_id = UnpackInt(row, "currency_id"),
                customer_account_id = UnpackLong(row, "customer_account_id"),
                customer_account_number = UnpackString(row, "customer_account_number"),
                customer_first_name = UnpackString(row, "customer_first_name"),
                customer_last_name = UnpackString(row, "customer_last_name"),
                customer_middle_name = UnpackString(row, "customer_middle_name"),
                name_on_card = UnpackString(row, "name_on_card"),
                Id_number = UnpackString(row, "Id_number"),
                contact_number = UnpackString(row, "contact_number"),
                CustomerId = UnpackString(row, "CustomerId"),
                customer_title_id = UnpackInt(row, "customer_title_id"),
                customer_type_id = UnpackInt(row, "customer_type_id"),
                date_issued = UnpackDateTime(row, "date_issued"),
                resident_id = UnpackInt(row, "resident_id"),
                user_id = UnpackLong(row, "user_id"),
                issuer_id = UnpackInt(row, "issuer_id"),
                issuer_code = UnpackString(row, "issuer_code"),
                issuer_name = UnpackString(row, "issuer_name"),
                branch_code = UnpackString(row, "branch_code"),
                branch_name = UnpackString(row, "branch_name"),
                product_code = UnpackString(row, "product_code"),
                product_name = UnpackString(row, "product_name"),
                product_bin_code = UnpackString(row, "product_bin_code"),
                sub_product_code = UnpackString(row, "sub_product_code"),
                pan_length = UnpackShort(row, "pan_length"),
                src1_id = UnpackInt(row, "src1_id"),
                src2_id = UnpackInt(row, "src2_id"),
                src3_id = UnpackInt(row, "src3_id"),
                PVKI = UnpackInt(row, "PVKI"),
                PVK = UnpackString(row, "PVK"),
                CVKA = UnpackString(row, "CVKA"),
                CVKB = UnpackString(row, "CVKB"),
                expiry_months = UnpackInt(row, "expiry_months"),
                delivery_branch_code = UnpackString(row, "delivery_branch_code"),
                delivery_branch_name = UnpackString(row, "delivery_branch_name"),
                emp_delivery_branch_code = UnpackString(row, "emp_delivery_branch_code")

            };

            return entry;
        }

        private List<CardObject> FetchCardObjectsForDistBatch(List<DistBatchCardDetail> cards)
        {
            List<CardObject> cardObjects = new List<CardObject>();

            foreach (var card in cards)
            {
                //TODO: Remove PAN hardcoded length
                CardObject cardObj = new CardObject(card.card_id, card.card_request_reference, card.issuer_id, card.issuer_code, card.issuer_name,
                                                    card.branch_id, card.branch_code, card.branch_name, card.product_id,
                                                    card.product_code, card.product_bin_code, card.sub_product_code, card.pan_length, card.src1_id, card.src2_id,
                                                    card.src3_id, card.PVKI.GetValueOrDefault(), card.PVK, card.CVKA, card.CVKB, card.card_sequence);

                //AMK: Use the line below when correctly setup
                //CardObject cardObj = new CardObject(card.card_id, card.card_request_reference, card.issuer_id, card.issuer_code, card.issuer_name,
                //                                   card.branch_id, card.branch_code, card.branch_name, card.product_id,
                //                                   card.product_code, card.product_bin_code, card.sub_product_code, card.pan_length, card.src1_id, card.src2_id,
                //                                   card.src3_id, card.PVKI.Value, card.PVK, card.CVKA, card.CVKB, card.card_sequence);

                cardObj.DistBatchReference = card.dist_batch_reference;
                cardObj.ProductName = card.product_name;
                //cardObj.SubProductName = card.sub_product_name;

                cardObj.CardNumber = card.card_number;

                if (card.card_activation_date != null)
                    cardObj.CardActivatedDate = card.card_activation_date.Value;

                if (card.card_production_date != null)
                    cardObj.CardIssuedDate = card.card_production_date.Value;

                if (card.card_expiry_date != null)
                    cardObj.ExpiryDate = card.card_expiry_date;

                cardObj.ExpiryMonths = card.expiry_months.Value;

                cardObj.DistBatchId = card.dist_batch_id;
                cardObj.DistCardStatusId = card.dist_card_status_id;
                cardObj.PVV = card.pvv;
                cardObj.DeliveryBranchCode = card.delivery_branch_code;
                cardObj.DeliveryBranchName = card.delivery_branch_name;
                cardObj.EMPDeliveryBranchCode = card.emp_delivery_branch_code;

                CardManagement.CardMangementService _cardManService = new CardManagement.CardMangementService();
                var productFields = _cardManService.GetProductFields(card.product_id, null, null).Where(p => p.ProductPrintFieldTypeId == 0).ToList();

                List<IProductPrintField> printFields = _cardManService.GetProductFieldsByCardId(card.card_id);
                cardObj.PrintFields = printFields;
                cardObj.CardIssueMethodId = card.card_issue_method_id;

                List<ProductField> accountFields = _cardManService.GetProductFieldsByCardIdTransalated(card.card_id);

                if (card.card_issue_method_id == 0 || card.card_issue_method_id == 2)
                {
                    var account = new AccountDetails();

                    account.AccountNumber = card.customer_account_number;
                    account.AccountTypeId = card.account_type_id.Value;
                    account.CardIssueMethodId = card.card_issue_method_id;
                    account.CardPriorityId = card.card_priority_id;
                    account.CmsID = card.cms_id;
                    account.ContactNumber = card.contact_number;
                    account.ContractNumber = card.contract_number;
                    account.CurrencyId = card.currency_id.GetValueOrDefault();
                    account.CustomerIDNumber = card.Id_number;
                    account.CustomerResidencyId = card.resident_id.GetValueOrDefault();
                    account.CustomerTitleId = card.customer_title_id.GetValueOrDefault();
                    account.CustomerTypeId = card.customer_type_id.GetValueOrDefault();
                    account.FirstName = card.customer_first_name;
                    account.LastName = card.customer_last_name;
                    account.MiddleName = card.customer_middle_name;
                    account.NameOnCard = card.name_on_card;
                    account.PriorityId = card.card_priority_id;
                    account.CreditContractNumber = card.credit_contract_number;
                    account.CreditLimit = card.credit_limit;
                    account.CreditLimitApproved = card.credit_limit_approved;
                    account.EmailAddress = card.customer_email;
                    account.ProductFields = accountFields;

                    cardObj.CustomerAccount = account;

                }


                cardObjects.Add(cardObj);
            }

            return cardObjects;
        }


        protected SqlParameter CreateSqlParameter(string parameterName, SqlDbType dataType, int size, object value, ParameterDirection direction = ParameterDirection.Input, byte precision = 0, byte scale = 0)
        {
            SqlParameter paramCode = new SqlParameter(string.Format("@{0}", parameterName), dataType);
            if (size != 0)
            {
                paramCode = new SqlParameter(string.Format("@{0}", parameterName), dataType, size);
            }
            if (precision != 0)
            {
                paramCode = new SqlParameter(string.Format("@{0}", parameterName), dataType, size, ParameterDirection.Input, false, precision, scale, string.Empty, DataRowVersion.Default, value);
            }

            if (direction != ParameterDirection.Input)
            {
                paramCode.Direction = direction;
            }
            paramCode.Value = (value != null ? value : DBNull.Value);
            return paramCode;
        }

        protected int ExecuteNonQuery(SqlCommand sqlCommand)
        {
            int result = sqlCommand.ExecuteNonQuery();
            return result;
        }

        protected DataTable ExecuteQuery(SqlCommand sqlCommand)
        {
            DataTable result = new DataTable();
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                result.Load(reader);
            }
            return result;
        }

        protected long UnpackLong(DataRow row, string columnName)
        {
            return (row[columnName] != DBNull.Value ? Convert.ToInt64(row[columnName]) : 0);
        }

        protected int UnpackInt(DataRow row, string columnName)
        {
            return (row[columnName] != DBNull.Value ? Convert.ToInt32(row[columnName]) : 0);
        }

        protected short UnpackShort(DataRow row, string columnName)
        {
            Int16 small = 0;
            return (row[columnName] != DBNull.Value ? (short)Convert.ToInt16(row[columnName]) : small);
        }

        protected decimal UnpackDecimal(DataRow row, string columnName)
        {
            return (row[columnName] != DBNull.Value ? Convert.ToDecimal(row[columnName]) : 0.00M);
        }

        protected string UnpackString(DataRow row, string columnName)
        {
            if (row[columnName] == DBNull.Value)
            {
                return null;
            }
            return (row[columnName] != DBNull.Value ? Convert.ToString(row[columnName]) : string.Empty);
        }

        protected byte[] UnpackBinary(DataRow row, string columnName)
        {
            if (row[columnName] == DBNull.Value)
            {
                return null;
            }
            return (row[columnName] != DBNull.Value ? (byte[])row[columnName] : null);
        }

        protected bool UnpackBoolean(DataRow row, string columnName, bool defaultValue = false)
        {
            return (row[columnName] != DBNull.Value ? Convert.ToBoolean(row[columnName]) : defaultValue);
        }

        protected DateTime UnpackDateTime(DataRow row, string columnName)
        {
            try
            {
                return (row[columnName] != DBNull.Value ? Convert.ToDateTime(row[columnName]) : new DateTime(1900, 1, 1));
            }
            catch (Exception)
            {
                return new DateTime(1900, 1, 1);
            }
        }

        protected Guid UnpackGuid(DataRow row, string columnName)
        {
            return (row[columnName] != DBNull.Value ? new Guid(Convert.ToString(row[columnName])) : new Guid());
        }

        public List<IConfig> GetProductInterfaceConfigs(int? productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_parameters_product_interface]";

                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@interface_type_id", SqlDbType.Int).Value = interfaceTypeId;
                    command.Parameters.Add("@interface_area", SqlDbType.Int).Value = interfaceArea;
                    command.Parameters.Add("@interface_guid", SqlDbType.Char).Value = interfaceGuid;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();

                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(command))
                    {
                        da.Fill(table);
                    }
                }
            }

            return ConfigFactory.GetConfigs(table);
        }

        public ExternalSystemFields GetExternalSystemFields(int externalSystemTypeId, int productId, int languageId, long auditUserId, string auditWorkStation)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_product_external_system_fields]";

                    command.Parameters.Add("@external_system_type_id", SqlDbType.Int).Value = externalSystemTypeId;
                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = auditWorkStation;

                    connection.Open();

                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(command))
                    {
                        da.Fill(table);
                    }
                }
            }

            ExternalSystemFields sysFields = new ExternalSystemFields();
            sysFields.Field = new Dictionary<string, string>();

            foreach (DataRow row in table.Rows)
            {
                sysFields.Field.Add(UnpackString(row, "field_name"), UnpackString(row, "field_value"));
            }

            return sysFields;
        }
    }
}
