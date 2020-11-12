using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Database;
using Common.Logging;
using Veneka.Indigo.IssuerManagement;

namespace Veneka.Indigo.UserManagement.dal
{
    public class UserDataManagement: IUserDataManagementDAL
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserDataManagement));
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        /// <summary>
        /// Gets a list of users who are linked to the specified branch.
        /// </summary>
        /// <param name="branch_id"></param>
        /// <param name="branchStatusId"></param>
        /// <param name="userStatus"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public List<user_list_result> GetUserByBranch(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            List<user_list_result> rtnValue = new List<user_list_result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<user_list_result> results = context.usp_get_users_by_branch(issuerId, branch_id, (int?)branchStatus, userStatus, (int?)userRole, username, firstName, lastName, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation);

                foreach (user_list_result result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        public List<user_list_result> GetUserByBranchAdmin(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            List<user_list_result> rtnValue = new List<user_list_result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<user_list_result> results = context.usp_get_users_by_branch_admin(issuerId, branch_id, (int?)branchStatus, userStatus, (int?)userRole, username, firstName, lastName, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation);

                foreach (user_list_result result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Get a list of users who do not belong to any users groups.
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public List<user_list_result> GetUnassignedUsers(int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            List<user_list_result> rtnValue = new List<user_list_result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<user_list_result> results = context.usp_get_unassigned_users(languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation);

                foreach (user_list_result result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Perform lookup of user for login.
        /// </summary>
        /// <param name="decryptedUserName"></param>
        /// <param name="decryptedPassword"></param>
        /// <param name="decryptedWorkstation"></param>
        /// <returns></returns>
        public login_user LogIn(string decryptedUserName, string decryptedPassword, string decryptedWorkstation)
        {
            List<login_user> rtnValue = new List<login_user>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<login_user> results = context.usp_get_user_for_login(decryptedUserName);

                foreach (login_user result in results)
                {
                    rtnValue.Add(result);
                }

                if (rtnValue.Count > 0)
                {
                    return rtnValue[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Finalise the users login, if login was successful then login atempt are zero'd. 
        /// If unsuccessful then increment login attemps.
        /// </summary>
        /// <param name="loginSuccess"></param>
        /// <param name="userId"></param>
        /// <param name="workStation"></param>
        public void FinaliseLogin(bool loginSuccess, long userId, string workStation)
        {
            List<login_user> rtnValue = new List<login_user>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                if (loginSuccess)
                {
                    context.usp_finalise_login(userId, workStation);
                }
                else
                {
                    context.usp_finalise_login_failed(userId, workStation);
                }
            }
        }
        /// <summary>
        /// Returns a single user based on the username provided.
        /// </summary>
        /// <param name="decrypted_username"></param>
        /// <returns></returns>
        public decrypted_user GetUserByUsername(string decryptedUserName)
        {
            decrypted_user rtnValue = new decrypted_user();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<decrypted_user> results = context.usp_get_user_by_username(decryptedUserName);
                var userResult = results.ToList<decrypted_user>();

                if (userResult.Count > 1)
                {
                    throw new Exception("Duplicate username found. More than one user was returned, only one user should have been returned.");
                }
                else if (userResult.Count == 0)
                {
                    rtnValue = null;
                }
                else
                {
                    rtnValue = userResult[0];
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Returns a single user based on the User Id provided.
        /// </summary>
        /// <param name="decryptedUserId"></param>
        /// <returns></returns>
        public decrypted_user GetUserByUserId(long decryptedUserId)
        {
            decrypted_user rtnValue = new decrypted_user();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<decrypted_user> results = context.usp_get_user_by_user_id(decryptedUserId);
                var userResult = results.ToList<decrypted_user>();

                if (userResult.Count > 1)
                {
                    throw new Exception("Duplicate username found. More than one user was returned, only one user should have been returned.");
                }
                else if (userResult.Count == 0)
                {
                    rtnValue = null;
                }
                else
                {
                    rtnValue = userResult[0];
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// to get pending user details
        /// </summary>
        /// <param name="decryptedUserId"></param>
        /// <returns></returns>
        public decrypted_user GetPendingUserByUserId(long decryptedUserId)
        {
            decrypted_user rtnValue = new decrypted_user();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<decrypted_user> results = context.usp_get_user_pending_by_user_id(decryptedUserId);
                var userResult = results.ToList<decrypted_user>();

                if (userResult.Count > 1)
                {
                    throw new Exception("Duplicate username found. More than one user was returned, only one user should have been returned.");
                }
                else if (userResult.Count == 0)
                {
                    rtnValue = null;
                }
                else
                {
                    rtnValue = userResult[0];
                }
            }

            return rtnValue;
        }
        /// <summary>
        /// to get list of pending users
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="branch_id"></param>
        /// <param name="branchStatus"></param>
        /// <param name="userStatus"></param>
        /// <param name="userRole"></param>
        /// <param name="username"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="languageId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        public List<user_list_result> GetUsersPendingForApproval(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            List<user_list_result> rtnValue = new List<user_list_result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<user_list_result> results = context.usp_get_user_pending_list(issuerId, branch_id, username, auditUserId, auditWorkStation, pageIndex, rowsPerPage);

                foreach (user_list_result result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }
        /// <summary>
        /// Gets the current and previouse passwords for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<user_passwords> GetUserPasswords(long userId)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<user_passwords> results = context.usp_get_passwords_by_user_id(userId);
                return results.ToList<user_passwords>();
            }
        }

        /// <summary>
        /// Gets the users auth pin.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public AuthPasscodeResult GetUserAuthPin(string username, int branchId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_authpin_by_user_id(username, branchId, auditUserId, auditWorkstation); 
                return results.FirstOrDefault();
            }
        }

        /// <summary>
        /// Fetch LDAP settings for issuer.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public LdapSettingResult GetLdapSetting(int ldapSettingId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_ldap_setting(ldapSettingId, auditUserId, auditWorkstation);
                return results.First();
            }
        }


        /// <summary>
        /// Persist user to the DB.
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        public SystemResponseCode CreateUser(user createUser, string encryptedPassword, List<int> userGroupList, long auditUserId, string auditWorkstation, out long? newUserId)
        {
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_UserGroupList = new DataTable();
                    dt_UserGroupList.Columns.Add("user_group_id", typeof(int));
                    DataRow workRow;

                    foreach (var item in userGroupList)
                    {
                        workRow = dt_UserGroupList.NewRow();
                        workRow["user_group_id"] = item;
                        dt_UserGroupList.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_insert_user]";

                    command.Parameters.Add("@user_status_id", SqlDbType.Int).Value = createUser.user_status_id;
                    command.Parameters.Add("@username", SqlDbType.VarChar).Value = GetString(createUser.username);
                    command.Parameters.Add("@first_name", SqlDbType.VarChar).Value = GetString(createUser.first_name);
                    command.Parameters.Add("@last_name", SqlDbType.VarChar).Value = GetString(createUser.last_name);
                    command.Parameters.Add("@password", SqlDbType.VarChar).Value = encryptedPassword;
                    command.Parameters.Add("@user_email", SqlDbType.VarChar).Value = createUser.user_email;
                    command.Parameters.Add("@online", SqlDbType.Bit).Value = createUser.online;
                    command.Parameters.Add("@employee_id", SqlDbType.VarChar).Value = GetString(createUser.employee_id);
                    command.Parameters.Add("@last_login_date", SqlDbType.DateTimeOffset).Value = createUser.last_login_date;
                    command.Parameters.Add("@last_login_attempt", SqlDbType.DateTimeOffset).Value = createUser.last_login_attempt;
                    command.Parameters.Add("@number_of_incorrect_logins", SqlDbType.Int).Value = createUser.number_of_incorrect_logins;
                    command.Parameters.Add("@last_password_changed_date", SqlDbType.DateTimeOffset).Value = createUser.last_password_changed_date;
                    command.Parameters.Add("@workstation", SqlDbType.VarChar).Value = createUser.workstation;
                    command.Parameters.AddWithValue("@user_group_list", dt_UserGroupList);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@authentication_configuration_id", SqlDbType.Int).Value = createUser.authentication_configuration_id;
                    command.Parameters.Add("@time_zone_Id", SqlDbType.VarChar).Value = createUser.time_zone_id;
                    command.Parameters.Add("@time_zone_utcoffset", SqlDbType.VarChar).Value = createUser.time_zone_utcoffset;

                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = createUser.language_id;
                    command.Parameters.Add("@user_id", SqlDbType.BigInt).Value = createUser.user_id;
                    command.Parameters.Add("@new_user_id", SqlDbType.BigInt).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    newUserId = long.Parse(command.Parameters["@new_user_id"].Value.ToString());
                    int resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                    return (SystemResponseCode)resultCode;
                }
            }
        }

        public SystemResponseCode UpdateAuthorisationPin(long userId , string authorisation_pin_number, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_instant_authorisation_pin]";

                    command.Parameters.Add("@user_id", SqlDbType.BigInt).Value = userId;
                    command.Parameters.Add("@authorisation_pin_number", SqlDbType.VarChar).Value = authorisation_pin_number;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    int resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                    return (SystemResponseCode)resultCode;
                }
            }
        }

        /// <summary>
        /// Persist changes to the user object.
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        public SystemResponseCode UpdateUser(user updateUser, List<int> userGroupList, long auditUserId, string auditWorkstation)
        {
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_UserGroupList = new DataTable();
                    dt_UserGroupList.Columns.Add("user_group_id", typeof(int));
                    DataRow workRow;

                    foreach (var item in userGroupList)
                    {
                        workRow = dt_UserGroupList.NewRow();
                        workRow["user_group_id"] = item;
                        dt_UserGroupList.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_user]";

                    command.Parameters.Add("@user_id", SqlDbType.BigInt).Value = updateUser.user_id;
                    command.Parameters.Add("@user_status_id", SqlDbType.Int).Value = updateUser.user_status_id;
                    command.Parameters.Add("@username", SqlDbType.VarChar).Value = GetString(updateUser.username);
                    command.Parameters.Add("@first_name", SqlDbType.VarChar).Value = GetString(updateUser.first_name);
                    command.Parameters.Add("@last_name", SqlDbType.VarChar).Value = GetString(updateUser.last_name);
                    command.Parameters.Add("@user_email", SqlDbType.VarChar).Value = updateUser.user_email;
                    //command.Parameters.Add("@password", SqlDbType.VarChar).Value = GetString(updateUser.password);
                    //command.Parameters.Add("@online", SqlDbType.Bit).Value = null;
                    command.Parameters.Add("@employee_id", SqlDbType.VarChar).Value = GetString(updateUser.employee_id);
                    //command.Parameters.Add("@last_login_date", SqlDbType.DateTime).Value = null;
                    //command.Parameters.Add("@last_login_attempt", SqlDbType.DateTime).Value = null;
                    //command.Parameters.Add("@number_of_incorrect_logins", SqlDbType.Int).Value = null;
                    //command.Parameters.Add("@last_password_changed_date", SqlDbType.DateTime).Value = null;
                    //command.Parameters.Add("@workstation", SqlDbType.VarChar).Value = null;
                    command.Parameters.AddWithValue("@user_group_list", dt_UserGroupList);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@authentication_configuration_id", SqlDbType.Int).Value = updateUser.authentication_configuration_id;
                    command.Parameters.Add("@time_zone_Id", SqlDbType.VarChar).Value = updateUser.time_zone_id;
                    command.Parameters.Add("@time_zone_utcoffset", SqlDbType.VarChar).Value = updateUser.time_zone_utcoffset;
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = updateUser.language_id;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    int resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                    return (SystemResponseCode)resultCode;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="encryptedPassword"></param>
        /// <param name="audit_user_id"></param>
        /// <param name="audit_workstation"></param>
        /// <returns></returns>
        public bool ApproveUser(long? pendingUserId, long audit_user_id, string audit_workstation)
        {
            var result_code = new ObjectParameter("ResultCode", "0");

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_user_audit_approve(pendingUserId,
                                                audit_user_id,
                                                audit_workstation,result_code);

                return true;
            }
        }


        public bool RejectUserRequest(long? pendingUserId, long audit_user_id, string audit_workstation)
        {
            var result_code = new ObjectParameter("ResultCode", "0");

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_user_audit_reject(pendingUserId,
                                                audit_user_id,
                                                audit_workstation, result_code);

                return true;
            }
        }
        /// <summary>
        /// Persist new password to the database. Any validations must have happened already!
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="decryptedPassword"></param>
        /// <param name="audit_user_id"></param>
        /// <param name="audit_workstation"></param>
        /// <returns></returns>
        public bool ResetUserPassword(long userId, string encryptedPassword, long audit_user_id, string audit_workstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_reset_user_password(encryptedPassword,
                                                userId,
                                                audit_user_id,
                                                audit_workstation);

                return true;
            }
        }

        /// <summary>
        /// Fetch a list of user groups which also indicates if the user is assigned to them.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="userRole">May be null to not filter by user role.</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserGroupAdminResult> GetUserGroupForUserAdmin(int? issuerId, int? userRole, long? userId, int? branchId)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<UserGroupAdminResult> results = context.usp_get_user_groups_admin(userId, issuerId, branchId, userRole);

                return results.ToList();
            }
        }
        /// <summary>
        /// Fetch a list of user groups which also indicates if the user is assigned to them from user_pending
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="userRole"></param>
        /// <param name="userId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>

        public List<UserGroupAdminResult> GetUserGroupForPendingUserAdmin(int? issuerId, int? userRole, long? userId, int? branchId)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<UserGroupAdminResult> results = context.usp_get_user_groups_pending_admin(userId, issuerId, branchId, userRole);

                return results.ToList();
            }
        }
        /// <summary>
        /// Fetch all the groups a user belongs to as well as the issuer and role of the group is apart of.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<GroupsRolesResult> GetGroupRolesForUser(long userId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_groups_roles_for_user(userId, languageId, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// Fetch all the groups a user belongs to as well as the issuer and role of the group is apart of .
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<GroupsRolesResult> GetGroupRolesForPendingUser(long userId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_groups_roles_for_user_pending(userId, languageId, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// Finalise logout for user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="audit_user_id"></param>
        /// <param name="audit_workstation"></param>
        public void FinaliseUserLogout(long userId, long audit_user_id, string audit_workstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_finalise_logout(userId, audit_user_id, audit_workstation);
            }
        }

        public  static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public List<user_list_result> GetUserList(string username, string firstname, string lastname, string branchid, string userrole, int issuerid, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            List<user_list_result> rtnValue = new List<user_list_result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<user_list_result> results = context.usp_search_user(username, firstname, lastname, (int?)issuerid, branchid, userrole, (int?)auditUserId, auditWorkStation, pageIndex, rowsPerPage);

                foreach (user_list_result result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;

        }

        public bool UpdateUserLanguage(long? userId, int languageId, string auditWorkstation)
        {
            var commited = false;

            try
            {
                using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
                {
                    var result_code = new ObjectParameter("result_code", "0");
                    context.usp_update_user_language(userId, languageId, result_code);

                    int resultCode;
                    if (int.TryParse(result_code.Value.ToString(), out resultCode))
                    {
                        commited = resultCode == 0;
                    }// end if (int.TryParse(result_code.Value.ToString(), out resultCode))
                }// end using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            }// end try
            catch (Exception ex)
            {
                log.Error(ex);
            }// end catch (Exception ex)

            return commited;
        }
        public bool CreateUseradminSettings(useradminsettingslist item,long? userId,  string auditWorkstation,out int user_admin_Id)
        {
            var commited = false;

            var user_admin_id = new ObjectParameter("user_admin_id", "0");
                using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
                {
                    context.usp_create_useradminsettings(item.PasswordValidPeriod, item.PasswordMinLength, item.PasswordMaxLength, item.PreviousPasswordsCount,item.maxInvalidPasswordAttempts,item.PasswordAttemptLockoutDuration, (int)userId, auditWorkstation,user_admin_id);
                }


                if (int.TryParse(user_admin_id.Value.ToString(), out user_admin_Id))
                {
                    commited = true;
                }
                return commited;
           
        }

        public bool UpdateUseradminSettings(useradminsettingslist item, long? userId, string auditWorkstation)
        {
            var commited = true;

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_useradminsettings(item.user_admin_id,item.PasswordValidPeriod, item.PasswordMinLength, item.PasswordMaxLength, item.PreviousPasswordsCount,item.maxInvalidPasswordAttempts,item.PasswordAttemptLockoutDuration, (int)userId, auditWorkstation);
            }
          
            return commited;

        }

        public useradminsettingslist GetUseradminSettings(long? auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_useradminsettingslist(auditUserId, auditWorkstation);
                return results.First();
            }
        }
        public List<LangLookup> GetLangUserRoles(int languageId, int? enterprise_only, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_userroles_enterprise(languageId, enterprise_only);
                return results.ToList();
            }
        }


       
    }

}