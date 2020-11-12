using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.IssuerManagement.objects;
using Veneka.Indigo.Common.Models;
using System.Data.Objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Objects;
using Veneka.Licensing.Common;
using Common.Logging;
using Veneka.Indigo.Common.DataAccess;

namespace Veneka.Indigo.IssuerManagement.dal
{
    public class IssuerManagementDal : BaseDataAccess, IIssuerManagementDal
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;
        private static readonly ILog log = LogManager.GetLogger(typeof(IssuerManagementDal));

        #region EXPOSED METHODS

        #region Issuer

        /// <summary>
        /// Persist new issuer to the DB.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode CreateIssuer(IssuerResult issuerObj, string pin_notification_message, long auditUserId, string auditWorkstation, out IssuerResult issuerResult)
        {
            List<Tuple<long, long, string>> prodInterfaceParams = new List<Tuple<long, long, string>>();
            foreach (var item in issuerObj.ProdInterfaces)
            {
                prodInterfaceParams.Add(new Tuple<long, long, string>(item.interface_type_id, item.connection_parameter_id, item.interface_guid));
            }

            List<Tuple<long, long, string>> issueInterfaceParams = new List<Tuple<long, long, string>>();
            foreach (var item in issuerObj.IssueInterfaces)
            {
                issueInterfaceParams.Add(new Tuple<long, long, string>(item.interface_type_id, item.connection_parameter_id, item.interface_guid));
            }

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_create_issuer]";

                    command.Parameters.Add("@issuer_status_id", SqlDbType.Int).Value = issuerObj.Issuer.issuer_status_id;
                    command.Parameters.Add("@country_id", SqlDbType.Int).Value = issuerObj.Issuer.country_id;
                    command.Parameters.Add("@issuer_name", SqlDbType.VarChar).Value = issuerObj.Issuer.issuer_name;
                    command.Parameters.Add("@issuer_code", SqlDbType.VarChar).Value = issuerObj.Issuer.issuer_code;
                    command.Parameters.Add("@instant_card_issue_YN", SqlDbType.Bit).Value = issuerObj.Issuer.instant_card_issue_YN;
                    command.Parameters.Add("@enable_instant_pin_YN", SqlDbType.Bit).Value = issuerObj.Issuer.enable_instant_pin_YN;
                    command.Parameters.Add("@authorise_pin_issue_YN", SqlDbType.Bit).Value = issuerObj.Issuer.authorise_pin_issue_YN;
                    command.Parameters.Add("@authorise_pin_reissue_YN", SqlDbType.Bit).Value = issuerObj.Issuer.authorise_pin_reissue_YN;
                    command.Parameters.Add("@allow_branches_to_search_cards_YN", SqlDbType.Bit).Value = issuerObj.Issuer.allow_branches_to_search_cards;

                    command.Parameters.Add("@maker_checker_YN", SqlDbType.Bit).Value = issuerObj.Issuer.maker_checker_YN;
                    command.Parameters.Add("@back_office_pin_auth_YN", SqlDbType.Bit).Value = issuerObj.Issuer.back_office_pin_auth_YN;
                    command.Parameters.Add("@license_file", SqlDbType.VarChar).Value = issuerObj.Issuer.license_file;
                    command.Parameters.Add("@license_key", SqlDbType.VarChar).Value = issuerObj.Issuer.license_key;
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = issuerObj.Issuer.language_id;
                    command.Parameters.Add("@card_ref_preference", SqlDbType.Bit).Value = issuerObj.Issuer.card_ref_preference;
                    command.Parameters.Add("@classic_card_issue_YN", SqlDbType.Bit).Value = issuerObj.Issuer.classic_card_issue_YN;
                    command.Parameters.AddWithValue("@prod_interface_parameters_list", UtilityClass.CreateBiKeyValueTable(prodInterfaceParams));
                    command.Parameters.AddWithValue("@issue_interface_parameters_list", UtilityClass.CreateBiKeyValueTable(issueInterfaceParams));
                    command.Parameters.Add("@remote_token", SqlDbType.UniqueIdentifier).Value = issuerObj.Issuer.remote_token;
                    command.Parameters.Add("@remote_token_expiry", SqlDbType.DateTime).Value = issuerObj.Issuer.remote_token_expiry;
                    command.Parameters.Add("@pin_notification_message", SqlDbType.VarChar).Value = pin_notification_message;
                    command.Parameters.Add("@number_pin_resend", SqlDbType.Int).Value = issuerObj.max_number_of_pin_send;
                    command.Parameters.Add("@pin_block_delete_days", SqlDbType.Int).Value = issuerObj.pin_block_delete_days;

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@new_issuer_id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    issuerResult = issuerObj;
                    issuerResult.Issuer.issuer_id = int.Parse(command.Parameters["@new_issuer_id"].Value.ToString());
                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }
        }

        /// <summary>
        /// Persist changes to the DB.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode UpdateIssuer(IssuerResult issuerObj, string pin_notification_message, long auditUserId, string auditWorkstation)
        {
            List<Tuple<long, long, string>> prodInterfaceParams = new List<Tuple<long, long, string>>();
            foreach (var item in issuerObj.ProdInterfaces)
            {
                prodInterfaceParams.Add(new Tuple<long, long, string>(item.interface_type_id, item.connection_parameter_id, item.interface_guid));
            }

            List<Tuple<long, long, string>> issueInterfaceParams = new List<Tuple<long, long, string>>();
            foreach (var item in issuerObj.IssueInterfaces)
            {
                issueInterfaceParams.Add(new Tuple<long, long, string>(item.interface_type_id, item.connection_parameter_id, item.interface_guid));
            }


            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_issuer]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerObj.Issuer.issuer_id;
                    command.Parameters.Add("@issuer_status_id", SqlDbType.Int).Value = issuerObj.Issuer.issuer_status_id;
                    command.Parameters.Add("@country_id", SqlDbType.Int).Value = issuerObj.Issuer.country_id;
                    command.Parameters.Add("@issuer_name", SqlDbType.VarChar).Value = issuerObj.Issuer.issuer_name;
                    command.Parameters.Add("@issuer_code", SqlDbType.VarChar).Value = issuerObj.Issuer.issuer_code;
                    command.Parameters.Add("@instant_card_issue_YN", SqlDbType.Bit).Value = issuerObj.Issuer.instant_card_issue_YN;
                    command.Parameters.Add("@maker_checker_YN", SqlDbType.Bit).Value = issuerObj.Issuer.maker_checker_YN;
                    command.Parameters.Add("@enable_instant_pin_YN", SqlDbType.Bit).Value = issuerObj.Issuer.enable_instant_pin_YN;
                    command.Parameters.Add("@authorise_pin_issue_YN", SqlDbType.Bit).Value = issuerObj.Issuer.authorise_pin_issue_YN;
                    command.Parameters.Add("@authorise_pin_reissue_YN", SqlDbType.Bit).Value = issuerObj.Issuer.authorise_pin_reissue_YN;
                    command.Parameters.Add("@back_office_pin_auth_YN", SqlDbType.Bit).Value = issuerObj.Issuer.back_office_pin_auth_YN;
                    command.Parameters.Add("@allow_branches_to_search_cards_YN", SqlDbType.Bit).Value = issuerObj.Issuer.allow_branches_to_search_cards;

                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = issuerObj.Issuer.language_id;
                    command.Parameters.Add("@card_ref_preference", SqlDbType.Int).Value = issuerObj.Issuer.card_ref_preference;
                    command.Parameters.Add("@classic_card_issue_YN", SqlDbType.Bit).Value = issuerObj.Issuer.classic_card_issue_YN;
                    command.Parameters.AddWithValue("@prod_interface_parameters_list", UtilityClass.CreateBiKeyValueTable(prodInterfaceParams));
                    command.Parameters.AddWithValue("@issue_interface_parameters_list", UtilityClass.CreateBiKeyValueTable(issueInterfaceParams));

                    command.Parameters.Add("@remote_token", SqlDbType.UniqueIdentifier).Value = issuerObj.Issuer.remote_token;
                    command.Parameters.Add("@remote_token_expiry", SqlDbType.DateTime).Value = issuerObj.Issuer.remote_token_expiry;
                    command.Parameters.Add("@pin_notification_message", SqlDbType.VarChar).Value = pin_notification_message;
                    command.Parameters.Add("@number_pin_resend", SqlDbType.Int).Value = issuerObj.max_number_of_pin_send;
                    command.Parameters.Add("@pin_block_delete_days", SqlDbType.Int).Value = issuerObj.pin_block_delete_days;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }
        }

        /// <summary>
        /// Fetch all countires
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<country> GetCountries(long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_all_countries(auditUserId, auditWorkstation);

                return result.ToList();
            }
        }

        /// <summary>
        /// Fetches a list of issuer_interface based on supplied parameters.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceTypeId"></param>
        /// <param name="interfaceAreaId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<issuer_interface> GetIssuerInterfaces(int issuerId, int? interfaceTypeId, int? interfaceAreaId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_issuer_interfaces(issuerId, interfaceTypeId, interfaceAreaId, auditUserId, auditWorkstation);

                return result.ToList();
            }
        }

        /// <summary>
        /// Fetch issuer based on issuer id.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public IssuerResult GetIssuer(int issuerId, long auditUserId, string auditWorkstation)
        {
            IssuerResult rtn = new IssuerResult();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_issuer(issuerId, auditUserId, auditWorkstation);

                rtn.Issuer = result.First();

                var prodInterfaceResult = context.usp_get_issuer_interfaces(issuerId, null, 0, auditUserId, auditWorkstation);
                rtn.ProdInterfaces = prodInterfaceResult.ToList();

                var issueInterfaceResult = context.usp_get_issuer_interfaces(issuerId, null, 1, auditUserId, auditWorkstation);
                rtn.IssueInterfaces = issueInterfaceResult.ToList();
            }

            return rtn;
        }

        public IssuerResult GetIssuerPinMessage(int issuer_id, long audit_user_id, string audit_workstation)
        {
            IssuerResult result = new IssuerResult();

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                CreateSqlParameter("issuer_id",SqlDbType.Int, 0, issuer_id),
                CreateSqlParameter("audit_user_id", SqlDbType.BigInt, 0, audit_user_id),
                CreateSqlParameter("audit_workstation", SqlDbType.NVarChar, 100, audit_workstation)
            };

            using (SqlCommand sqlCommand = new SqlCommand("usp_get_issuer_pin_message"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());

                foreach (DataRow row in ExecuteQuery(sqlCommand).Rows)
                {
                    result = UnPackProductMessage(row);
                }
            }
            return result;
        } //usp_get_issuer_pin_message

        private IssuerResult UnPackProductMessage(DataRow row)
        {
            try
            {
                IssuerResult pin_notification_message = new IssuerResult
                {
                    pin_notification_message = UnpackString(row, "pin_notification_message"),
                    pin_block_delete_days = UnpackInt(row, "pin_block_delete_days"),
                    max_number_of_pin_send = UnpackInt(row, "max_number_of_pin_send")
                };
                return pin_notification_message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IssuerResult GetIssuer(string issuerCode, long auditUserId, string auditWorkstation)
        {
            IssuerResult rtn = new IssuerResult();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_issuer_by_code(issuerCode, auditUserId, auditWorkstation);

                rtn.Issuer = result.First();

                var prodInterfaceResult = context.usp_get_issuer_interfaces(rtn.Issuer.issuer_id, null, 0, auditUserId, auditWorkstation);
                rtn.ProdInterfaces = prodInterfaceResult.ToList();

                var issueInterfaceResult = context.usp_get_issuer_interfaces(rtn.Issuer.issuer_id, null, 1, auditUserId, auditWorkstation);
                rtn.IssueInterfaces = issueInterfaceResult.ToList();
            }

            return rtn;
        }

        public List<issuer> SearchIssuer(int issuerID, string issuerName, InstitutionStatus status)
        {
            List<issuer> issuers = null;

            try
            {
                using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
                {
                    if (string.IsNullOrEmpty(status.ToString()) || status.ToString().Equals("UNKNOWN"))
                    {
                        var result = context.usp_search_issuer_by_id(issuerID).ToList();

                        if (result.Count() > 0)
                        {
                            issuers = result;
                        }// end if (result.Count() > 0) 
                    }// end if (string.IsNullOrEmpty(status.ToString()) || status.ToString().Equals("UNKNOWN"))
                    else
                    {
                        var result = context.usp_search_issuer(issuerName, status.ToString(), issuerID).ToList();

                        if (result.Count() > 0)
                        {
                            issuers = result;
                        }// end if (result.Count() > 0) 
                    }// end else

                }// end using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            }// end try
            catch (Exception ex)
            {
                log.Error(ex);
            }// end catch (Exception ex)

            return issuers;
        }// end method List<issuer> SearchIssuer(int issuerID, string issuerName, InstitutionStatus status)

        /// <summary>
        /// Returns a list of all issuers
        /// </summary>
        /// <returns></returns>
        public List<issuers_Result> GetAllIssuers(int? LanguageId, int? pageIndex, int? Rowsperpage)
        {
            List<issuers_Result> issuerlist = new List<issuers_Result>();
            try
            {
                #region Old Code

                //using (SqlConnection con = _dbObject.SQLConnection)
                //{
                //    string sql = "SELECT * FROM  issuer ";

                //    using (var command = new SqlCommand(sql, con))
                //    {
                //        using (SqlDataReader reader = command.ExecuteReader())
                //        {
                //            if (reader != null)
                //            {
                //                while (reader.Read())
                //                {
                //                    issuers.Add(ConstructIssuerObject(reader));
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion

                using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
                {
                    var results = context.usp_get_all_issuers(LanguageId, pageIndex, Rowsperpage);

                    foreach (var issuerItem in results)
                    {
                        issuerlist.Add(issuerItem);
                    }
                }// end using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return issuerlist;
        }// end method List<issuer> GetAllIssuers()

        #endregion

        #region LDAP Settings

        /// <summary>
        /// Fetch all ldap settings
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<LdapSettingsResult> GetLdapSettings(long auditUserId, string auditWorkstation)
        {
            List<LdapSettingsResult> rtnValue = new List<LdapSettingsResult>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_ldap_settings(auditUserId, auditWorkstation);

                rtnValue = results.ToList();
            }

            return rtnValue;
        }

        /// <summary>
        /// Persist new LDAP Setting to the database
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode CreateLdapSetting(LdapSettingsResult ldapSetting, long auditUserId, string auditWorkstation, out int ldapSettingId)
        {
            ObjectParameter LdapSettingId = new ObjectParameter("new_ldap_id", typeof(int));
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_create_ldap(ldapSetting.ldap_setting_name,
                                                    ldapSetting.hostname_or_ip,
                                                    ldapSetting.path,
                                                    ldapSetting.domain_name,
                                                    ldapSetting.username,
                                                    ldapSetting.password,
                                                    ldapSetting.auth_type_id,
                                                    ldapSetting.external_inteface_id,
                                                    auditUserId, auditWorkstation,
                                                    LdapSettingId,
                                                    ResultCode);
            }

            ldapSettingId = int.Parse(LdapSettingId.Value.ToString());
            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Persist changes to LDAP Setting to DB
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public SystemResponseCode UpdateLdapSetting(LdapSettingsResult ldapSetting, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_ldap(ldapSetting.ldap_setting_id,
                                       ldapSetting.ldap_setting_name,
                                       ldapSetting.hostname_or_ip,
                                       ldapSetting.path,
                                       ldapSetting.domain_name,
                                       ldapSetting.username,
                                       ldapSetting.password,
                                       ldapSetting.auth_type_id,
                                                    ldapSetting.external_inteface_id,
                                       auditUserId, auditWorkstation,
                                       ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        public List<AuthenticationtypesResult> GetAuthenticationTypes(long auditUserId, string auditWorkstation)
        {
            List<AuthenticationtypesResult> rtnValue = new List<AuthenticationtypesResult>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_authenticationtypes(auditUserId, auditWorkstation);

                rtnValue = results.ToList();
            }

            return rtnValue;
        }

        /// <summary>
        /// Delete Ldap Settings
        /// </summary>
        /// <param name="ldap_setting_id"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public void DeleteLdapSetting(int ldap_setting_id, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_delete_ldap(ldap_setting_id,
                                       auditUserId, auditWorkstation);
            }
        }

        #endregion

        #region Interface Connection

        /// <summary>
        /// Fetch interface details for issuer.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceTypes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public ConnectionParamsResult GetIssuerInterface(int issuerId, int interfaceTypeId, int interfaceAreaId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_issuer_interface_conn(issuerId, interfaceTypeId, interfaceAreaId, auditUserId, auditWorkstation);

                if (result.Count() > 0)
                {
                    return result.First();
                }
            }

            return null;
        }

        /// <summary>
        /// Fetch all connection parameters.
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<ConnectionParamsResult> GetConnectionParameters(long auditUserId, string auditWorkstation)
        {
            List<ConnectionParamsResult> rtnValue = new List<ConnectionParamsResult>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_connection_params(auditUserId, auditWorkstation);

                rtnValue = results.ToList();
            }

            return rtnValue;
        }

        public List<ConnectionParamAdditionalDataResult> GetConnectionParametersAdditionalData(int connParameterId,long auditUserId, string auditWorkstation)
        {
            List<ConnectionParamAdditionalDataResult> rtnValue = new List<ConnectionParamAdditionalDataResult>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_connection_parameter_additionaldata(connParameterId,auditUserId, auditWorkstation);

                rtnValue = results.ToList();
            }

            return rtnValue;
        }

        public ConnectionParamsResult GetConnectionParameter(int connParameterId, long auditUserId, string auditWorkstation)
        {
            List<ConnectionParamsResult> rtnValue = new List<ConnectionParamsResult>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_connection_parameter(connParameterId, auditUserId, auditWorkstation);

                return results.FirstOrDefault();
            }
        }

        /// <summary>
        /// Persist new connection param to the database
        /// </summary>
        /// <param name="connectionParam"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>



        public ConnectionParametersResult CreateConnectionParam(ConnectionParametersResult connectionParam, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_additionaldata = new DataTable();
                    dt_additionaldata.Columns.Add("key1", typeof(string));
                    dt_additionaldata.Columns.Add("value", typeof(string));

                    DataRow workRow;

                    foreach (var item in connectionParam.additionaldata)
                    {
                        workRow = dt_additionaldata.NewRow();
                        workRow["key1"] = item.key;
                        workRow["value"] = item.value;
                        dt_additionaldata.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_connection_parameter_create]";

                    command.Parameters.Add("@connection_name", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.connection_name;
                    command.Parameters.Add("@connection_parameter_type_id", SqlDbType.Int).Value = connectionParam.ConnectionParams.connection_parameter_type_id;
                    command.Parameters.Add("@address", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.address;
                    command.Parameters.Add("@port", SqlDbType.Int).Value = connectionParam.ConnectionParams.port;
                    command.Parameters.Add("@path", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.path;

                    command.Parameters.Add("@name_of_file", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.name_of_file;
                    command.Parameters.Add("@file_delete_YN", SqlDbType.Bit).Value = connectionParam.ConnectionParams.file_delete_YN;
                    command.Parameters.Add("@file_encryption_type_id", SqlDbType.Bit).Value = connectionParam.ConnectionParams.file_encryption_type_id;
                    command.Parameters.Add("@duplicate_file_check_YN", SqlDbType.Bit).Value = connectionParam.ConnectionParams.duplicate_file_check_YN;
                    command.Parameters.Add("@protocol", SqlDbType.Int).Value = connectionParam.ConnectionParams.protocol;
                    command.Parameters.Add("@auth_type", SqlDbType.Int).Value = connectionParam.ConnectionParams.auth_type;
                    command.Parameters.Add("@header_length", SqlDbType.Int).Value = connectionParam.ConnectionParams.header_length;
                    command.Parameters.Add("@identification", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.identification;
                    command.Parameters.AddWithValue("@additional_data", dt_additionaldata);
                    command.Parameters.Add("@timeout_milli", SqlDbType.Int).Value = connectionParam.ConnectionParams.timeout_milli;
                    command.Parameters.Add("@buffer_size", SqlDbType.Int).Value = connectionParam.ConnectionParams.buffer_size;

                    command.Parameters.Add("@doc_type", SqlDbType.Char).Value = connectionParam.ConnectionParams.doc_type;
                    command.Parameters.Add("@auth_username", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.username;
                    command.Parameters.Add("@auth_password", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.password;
                    command.Parameters.Add("@auth_nonce", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.nonce;
                    command.Parameters.Add("@private_key", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.private_key;
                    command.Parameters.Add("@public_key", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.public_key;
                    command.Parameters.Add("@domain_name", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.domain_name;
                    command.Parameters.Add("@is_external_auth", SqlDbType.Bit).Value = connectionParam.ConnectionParams.is_external_auth;
                    command.Parameters.Add("@remote_port", SqlDbType.Int).Value = connectionParam.ConnectionParams.remote_port;
                    command.Parameters.Add("@remote_username", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.remote_username;
                    command.Parameters.Add("@remote_password", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.remote_password;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    command.Parameters.Add("@connection_parameter_id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();
                    int connection_parameter_id = int.Parse(command.Parameters["@connection_parameter_id"].Value.ToString());
                    connectionParam.ConnectionParams.connection_parameter_id = connection_parameter_id;
                }
            }
            return connectionParam;

        }


        public void UpdateConnectionParam(ConnectionParametersResult connectionParam, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_additionaldata = new DataTable();
                    dt_additionaldata.Columns.Add("key1", typeof(string));
                    dt_additionaldata.Columns.Add("value", typeof(string));

                    DataRow workRow;

                    foreach (var item in connectionParam.additionaldata)
                    {
                        workRow = dt_additionaldata.NewRow();
                        workRow["key1"] = item.key;
                        workRow["value"] = item.value;
                        dt_additionaldata.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_connection_parameter_update]";

                    command.Parameters.Add("@connection_name", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.connection_name;
                    command.Parameters.Add("@connection_parameter_type_id", SqlDbType.Int).Value = connectionParam.ConnectionParams.connection_parameter_type_id;
                    command.Parameters.Add("@address", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.address;
                    command.Parameters.Add("@port", SqlDbType.Int).Value = connectionParam.ConnectionParams.port;
                    command.Parameters.Add("@name_of_file", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.name_of_file;
                    command.Parameters.Add("@path", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.path;

                    command.Parameters.Add("@file_delete_YN", SqlDbType.Bit).Value = connectionParam.ConnectionParams.file_delete_YN;
                    command.Parameters.Add("@file_encryption_type_id", SqlDbType.Bit).Value = connectionParam.ConnectionParams.file_encryption_type_id;
                    command.Parameters.Add("@duplicate_file_check_YN", SqlDbType.Bit).Value = connectionParam.ConnectionParams.duplicate_file_check_YN;
                    command.Parameters.Add("@protocol", SqlDbType.Int).Value = connectionParam.ConnectionParams.protocol;
                    command.Parameters.Add("@auth_type", SqlDbType.Int).Value = connectionParam.ConnectionParams.auth_type;
                    command.Parameters.Add("@header_length", SqlDbType.Int).Value = connectionParam.ConnectionParams.header_length;
                    command.Parameters.Add("@identification", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.identification;
                    command.Parameters.AddWithValue("@additional_data", dt_additionaldata);
                    command.Parameters.Add("@timeout_milli", SqlDbType.Int).Value = connectionParam.ConnectionParams.timeout_milli;
                    command.Parameters.Add("@buffer_size", SqlDbType.Int).Value = connectionParam.ConnectionParams.buffer_size;

                    command.Parameters.Add("@doc_type", SqlDbType.Char).Value = connectionParam.ConnectionParams.doc_type;
                    command.Parameters.Add("@auth_username", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.username;
                    command.Parameters.Add("@auth_password", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.password;
                    command.Parameters.Add("@auth_nonce", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.nonce;
                    command.Parameters.Add("@private_key", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.private_key;
                    command.Parameters.Add("@public_key", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.public_key;
                    command.Parameters.Add("@domain_name", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.domain_name;
                    command.Parameters.Add("@is_external_auth", SqlDbType.Bit).Value = connectionParam.ConnectionParams.is_external_auth;
                    command.Parameters.Add("@remote_port", SqlDbType.Int).Value = connectionParam.ConnectionParams.remote_port;
                    command.Parameters.Add("@remote_username", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.remote_username;
                    command.Parameters.Add("@remote_password", SqlDbType.VarChar).Value = connectionParam.ConnectionParams.remote_password;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    command.Parameters.Add("@connection_parameter_id", SqlDbType.Int).Value = connectionParam.ConnectionParams.connection_parameter_id;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();
                  
                }
            }

        }

        /// <summary>
        /// Persist changes to connection parameter to DB
        /// </summary>
        /// <param name="connectionParam"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>resu
        public void UpdateConnectionParam(ConnectionParamsResult connectionParam, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            List<ConnectionParamsResult> rtnValue = new List<ConnectionParamsResult>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_connection_parameter_update(connectionParam.connection_parameter_id,
                                                       connectionParam.connection_name,
                                                       connectionParam.connection_parameter_type_id,
                                                       connectionParam.address,
                                                       connectionParam.port,
                                                       connectionParam.name_of_file,
                                                       connectionParam.file_delete_YN,
                                                       connectionParam.file_encryption_type_id,
                                                       connectionParam.duplicate_file_check_YN,
                                                       connectionParam.path,
                                                       connectionParam.protocol,
                                                       connectionParam.auth_type,
                                                       connectionParam.header_length,
                                                       connectionParam.identification,
                                                       connectionParam.timeout_milli,
                                                       connectionParam.buffer_size,
                                                       connectionParam.doc_type,
                                                       connectionParam.username,
                                                       connectionParam.password,
                                                       connectionParam.nonce,
                                                       connectionParam.private_key,
                                                       connectionParam.public_key,
                                                       connectionParam.domain_name,
                                                       connectionParam.is_external_auth,
                                                       connectionParam.remote_port,
                                                       connectionParam.remote_username,
                                                       connectionParam.remote_password,
                                                       auditUserId, auditWorkstation, ResultCode);
            }
        }

        /// <summary>
        /// Delete connection parameter from DB
        /// </summary>
        /// <param name="connectionParamId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public void DeleteConnectionParam(int connectionParamId, long auditUserId, string auditWorkstation)
        {            
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_delete_connenction_params(connectionParamId, auditUserId, auditWorkstation);
            }
        }

        public List<ProductInterfaceResult> GetProductInterfaces(int connectionParamId, long auditUserId, string auditWorkstation)
        {
            //List<ProductInterfacesResult> result = new List<ProductInterfacesResult>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_product_by_interfaces(connectionParamId, auditUserId, auditWorkstation);
                return result.ToList();
            }
        }

        public List<IssuerInterfaceResult> GetIssuerConnectionParams(int connectionParamId, long auditUserId, string auditWorkstation)
        {
            //List<ProductInterfacesResult> result = new List<ProductInterfacesResult>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_issuer_by_interface(connectionParamId, auditUserId, auditWorkstation);
                return result.ToList();
            }
        }

        #endregion

        #region Licenses

        /// <summary>
        /// Persist license for issuer to DB.
        /// </summary>
        /// <param name="license"></param>
        /// <param name="key"></param>
        /// <param name="xmlLicenseFile"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public DBResponseMessage LoadLicense(IndigoComponentLicense license, string key, byte[] xmlLicenseFile, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_load_issuer_license(license.IssuerName.Trim(), license.IssuerCode.Trim(), key, xmlLicenseFile, auditUserId, auditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (DBResponseMessage)resultCode;
        }

      

        #endregion

        #endregion
    }

}