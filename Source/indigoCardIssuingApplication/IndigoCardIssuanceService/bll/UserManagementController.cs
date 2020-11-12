
using System;
using System.Linq;
using System.Collections.Generic;
using Veneka.Indigo.UserManagement;
using IndigoCardIssuanceService.DataContracts;
using Common.Logging;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Exceptions;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.IssuerManagement;
using Veneka.Indigo.Security;
using Veneka.Indigo.UserManagement.objects;

namespace IndigoCardIssuanceService.bll
{
    public class UserManagementController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserManagementController));

        private readonly SessionManager _sessMan = SessionManager.GetInstance();
        private readonly UserGroupService _userGroupMan = new UserGroupService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly AuthConfigurationService _authconfig = new AuthConfigurationService();


        /// <summary>
        /// Do user login request.
        /// </summary>
        /// <param name="encryptedUserName"></param>
        /// <param name="encryptedPasword"></param>
        /// <param name="encryptedWorkstation"></param>
        /// <returns></returns>
        internal Response<UserLogInRes> LogIn(string encryptedUserName, string encryptedPasword, string encryptedWorkstation)
        {
            try
            {
                bool allowMultipleLogins = false;
                bool mustChangePassword;

                var result = _userMan.LogIn(encryptedUserName, encryptedPasword, encryptedWorkstation, out mustChangePassword);
                //Fetch roles for the user
                var rolesResult = _userMan.GetUserRoles(result.user_id);
                //Fetch menu access list
                //var statusFlowRoles = _userMan.getStatusFlowRoles(rolesResult.Select(s => s.user_role_id).Distinct().ToList());


                if (rolesResult.Where(m => m.allow_multiple_login == true).ToList().Count > 0)
                {
                    allowMultipleLogins = true;
                }

                int userLanguage = result.language_id == null ? 0 : result.language_id.Value;


                var session = _sessMan.AddSession(result.clr_username, result.workstation, result.user_id,
                                                  userLanguage, allowMultipleLogins);

                AuthConfigResult authresult = _authconfig.GetAuthConfiguration(result.authentication_configuration_id);

                var config = authresult.AuthConfigConnectionParams;

                bool ladapUser = config.Any(i => i.connection_parameter_id == (int)AuthTypes.ExternalAuth);

                bool multifactorUser = config.Any(i => i.auth_type_id == (int)AuthTypes.MultiFactor);



                UserLogInRes responseObj = new UserLogInRes();
                responseObj.encryptedUsername = encryptedUserName;
                //responseObj.Roles = rolesResult;
                //responseObj.ldapUser = config == null ? false : true;
                responseObj.ldapUser = ladapUser;
                responseObj.multifactorUser = multifactorUser;
                responseObj.authConfigurationId = result.authentication_configuration_id;


                responseObj.encryptedUserId = EncryptionManager.EncryptString(result.user_id.ToString(),
                                                                              StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                              StaticFields.EXTERNAL_SECURITY_KEY);

                responseObj.encryptedSessionKey = EncryptionManager.EncryptString(session.SessionKey,
                                                                                  StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                                  StaticFields.EXTERNAL_SECURITY_KEY);

                responseObj.LanguageId = result.language_id ?? 0;

                //responseObj.StatusFlow = statusFlowRoles;

                responseObj.ChangePassword = mustChangePassword;

                if (multifactorUser)
                {
                    // SendChallenge((int)result.authentication_configuration_id,result.user_id, "");
                }

                //TODO FIX THIS
                //responseObj.ChangePassword = false;
                //if (result.last_password_changed_date == null)
                //    result.last_password_changed_date = DateTime.Now;              


                return new Response<UserLogInRes>(responseObj, ResponseType.SUCCESSFUL, "", "");
            }
            catch (LoginFailedException loginEx)
            {
                log.Trace(m => m(loginEx.Message));

                return new Response<UserLogInRes>(null,
                                                  ResponseType.UNSUCCESSFUL,
                                                  loginEx.Message,
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? loginEx.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<UserLogInRes>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Find the user in Active Directory.
        /// </summary>
        /// <param name="usernameLookup"></param>
        /// <param name="username"></param>
        /// <param name="userpassword"></param>
        /// <param name="ldapIssuerId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<decrypted_user> LdapLookup(string usernameLookup, string username, string userpassword, int authConfigId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<decrypted_user>(_userMan.LdapLookup(usernameLookup, username, userpassword, authConfigId, auditUserId, auditWorkstation),
                                                    ResponseType.SUCCESSFUL, "", "");
            }
            catch (BaseIndigoException bex)
            {
                return new Response<decrypted_user>(null,
                                                    ResponseType.FAILED,
                                                    bex.Message,
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? bex.InnerException.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<decrypted_user>(null,
                                                    ResponseType.ERROR,
                                                    "An error occured during processing your request, please try again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal BaseResponse DeleteAuthConfiguration(int authConfigurationId, int languageId, long auditUserId, string auditWorkstation)
        {

            try
            {
                string responseMessage;
                if (_authconfig.DeleteAuthConfiguration(authConfigurationId, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (BaseIndigoException bex)
            {
                log.Error(bex);
                return new BaseResponse(
                                          ResponseType.UNSUCCESSFUL,
                                          bex.Message,
                                          log.IsDebugEnabled || log.IsTraceEnabled ? bex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                          "An error occured during processing your request, please try again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<AuthConfigResult> GetAuthConfiguration(int? authConfigurationId)
        {
            try
            {
                return new Response<AuthConfigResult>(_authconfig.GetAuthConfiguration(authConfigurationId),
                                                    ResponseType.SUCCESSFUL, "", "");
            }
            catch (BaseIndigoException bex)
            {
                return new Response<AuthConfigResult>(null,
                                                    ResponseType.FAILED,
                                                    bex.Message,
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? bex.InnerException.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<AuthConfigResult>(null,
                                                    ResponseType.ERROR,
                                                    "An error occured during processing your request, please try again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<auth_configuration_result>> GetAuthConfigurationList(int? authConfigurationId, int? pageIndex, int? rowsPerPage)
        {
            try
            {
                return new Response<List<auth_configuration_result>>(_authconfig.GetAuthConfigurationList(authConfigurationId, pageIndex, rowsPerPage),
                                                    ResponseType.SUCCESSFUL, "", "");
            }
            catch (BaseIndigoException bex)
            {
                return new Response<List<auth_configuration_result>>(null,
                                                    ResponseType.FAILED,
                                                    bex.Message,
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? bex.InnerException.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<auth_configuration_result>>(null,
                                                    ResponseType.ERROR,
                                                    "An error occured during processing your request, please try again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<int?> InsertAuthenticationConfiguration(AuthConfigResult authConfig, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                int newauthconfigId = 0;
                if (_authconfig.InsertAuthenticationConfiguration(authConfig, languageId, auditUserId, auditWorkstation, out newauthconfigId, out responseMessage))
                {
                    return new Response<int?>(newauthconfigId, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<int?>(newauthconfigId, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (BaseIndigoException bex)
            {
                log.Error(bex);
                return new Response<int?>(null,
                                          ResponseType.UNSUCCESSFUL,
                                          bex.Message,
                                          log.IsDebugEnabled || log.IsTraceEnabled ? bex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int?>(null,
                                          ResponseType.ERROR,
                                          "An error occured during processing your request, please try again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
        internal BaseResponse UpdateAuthenticationConfiguration(AuthConfigResult authConfig, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_authconfig.UpdateAuthenticationConfiguration(authConfig, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (BaseIndigoException bex)
            {
                log.Error(bex);
                return new BaseResponse(
                                          ResponseType.UNSUCCESSFUL,
                                          bex.Message,
                                          log.IsDebugEnabled || log.IsTraceEnabled ? bex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                          "An error occured during processing your request, please try again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }


        /// <summary>
        /// Persist user to DB
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        internal Response<long?> CreateUser(user createUser, List<int> userGroupList, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                long? newUserId = 0;
                if (_userMan.CreateUser(createUser, userGroupList, languageId, auditUserId, auditWorkstation, out newUserId, out responseMessage))
                {
                    return new Response<long?>(newUserId, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<long?>(newUserId, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (BaseIndigoException bex)
            {
                log.Error(bex);
                return new Response<long?>(null,
                                          ResponseType.UNSUCCESSFUL,
                                          bex.Message,
                                          log.IsDebugEnabled || log.IsTraceEnabled ? bex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long?>(null,
                                          ResponseType.ERROR,
                                          "An error occured during processing your request, please try again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Persist changes to the user object.
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        internal Response<long?> UpdateUser(user updateUser, List<int> userGroupList, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                long? newUserId = 0;
                if (_userMan.UpdateUser(updateUser, userGroupList, languageId, auditUserId, auditWorkstation, out newUserId, out responseMessage))
                {
                    return new Response<long?>(newUserId, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<long?>(newUserId, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (BaseIndigoException bex)
            {
                log.Error(bex);
                return new Response<long?>(null,
                                          ResponseType.UNSUCCESSFUL,
                                          bex.Message,
                                          log.IsDebugEnabled || log.IsTraceEnabled ? bex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long?>(null,
                                          ResponseType.ERROR,
                                          "An error occured during processing your request, please try again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }


        /// <summary>
        /// Admin function to reset a users password.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="encryptedPassword"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse ResetUserPassword(long userId, string encryptedPassword, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_userMan.ResetUserPassword(userId, encryptedPassword, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL,
                                            responseMessage,
                                            "");
                }
                else
                {
                    return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// User function to change password.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="encryptedOldPassword"></param>
        /// <param name="encryptedNewPassword"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse UpdateUserPassword(long userId, string encryptedOldPassword, string encryptedNewPassword, int languageId, long auditUserId, string encryptedWorkstation)
        {
            try
            {

                string decryptedWorkstation = EncryptionManager.DecryptString(encryptedWorkstation,
                                                                              StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                              StaticFields.EXTERNAL_SECURITY_KEY);
                string responseMessage;
                if (_userMan.UpdateUserPassword(userId, encryptedOldPassword, encryptedNewPassword, languageId, auditUserId, decryptedWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL,
                                            responseMessage,
                                            "");
                }
                else
                {
                    return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// User function to add a new authorisation pin.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="encryptedPin"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse InsertUserAuthorisationPin(long userId, string encryptedPin, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_userMan.InsertUserAuthorisationPin(userId, encryptedPin, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL,
                                            responseMessage,
                                            "");
                }
                else
                {
                    return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Get user groups linked to issuer.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <returns></returns>
        internal Response<List<UserGroupResult>> GetUserGroups(int issuerId, UserRole? userRole, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<UserGroupResult>>(_userGroupMan.GetUserGroups(issuerId, userRole, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation),
                                                           ResponseType.SUCCESSFUL,
                                                           ResponseType.SUCCESSFUL.ToString(),
                                                           "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<UserGroupResult>>(null,
                                                           ResponseType.ERROR,
                                                           "An error occured during processing your request, please try again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Get user groups linked to issuer and user role.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="userRole"></param>
        /// <returns></returns>
        internal Response<List<UserGroupResult>> GetUserGroupsByRole(int issuerId, UserRole userRole, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<UserGroupResult>>(_userGroupMan.GetUserGroups(issuerId, userRole, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation),
                                                           ResponseType.SUCCESSFUL,
                                                           ResponseType.SUCCESSFUL.ToString(),
                                                           "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<UserGroupResult>>(null,
                                                           ResponseType.ERROR,
                                                           "An error occured during processing your request, please try again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }


        /// <summary>
        /// Returns a single user based on the username provided.
        /// </summary>
        /// <param name="encryptedUsername"></param>
        /// <returns></returns>
        internal Response<decrypted_user> GetUserByUsername(string encryptedUsername)
        {
            try
            {
                return new Response<decrypted_user>(_userMan.GetUserByUsername(encryptedUsername),
                                                ResponseType.SUCCESSFUL,
                                                ResponseType.SUCCESSFUL.ToString(),
                                                "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<decrypted_user>(null,
                                                        ResponseType.ERROR,
                                                        "An error occured during processing your request, please try again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Returns a single user based on the User Id provided.
        /// </summary>
        /// <param name="encryptedUserId"></param>
        /// <returns></returns>
        internal Response<decrypted_user> GetUserByUserId(string encryptedUserId)
        {
            try
            {
                return new Response<decrypted_user>(_userMan.GetUserByUserId(encryptedUserId),
                                                ResponseType.SUCCESSFUL,
                                                ResponseType.SUCCESSFUL.ToString(),
                                                "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<decrypted_user>(null,
                                                        ResponseType.ERROR,
                                                        "An error occured during processing your request, please try again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }


        internal Response<decrypted_user> GetPendingUserByUserId(string encryptedUserId)
        {
            try
            {
                return new Response<decrypted_user>(_userMan.GetPendingUserByUserId(encryptedUserId),
                                                ResponseType.SUCCESSFUL,
                                                ResponseType.SUCCESSFUL.ToString(),
                                                "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<decrypted_user>(null,
                                                        ResponseType.ERROR,
                                                        "An error occured during processing your request, please try again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Gets the user authorisation pin
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="authPin"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        internal Response<long?> GetUserAuthPin(string encryptedUser, string encryptedPasscode, int branchId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string username = EncryptionManager.DecryptString(encryptedUser,
                                                                    StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                    StaticFields.EXTERNAL_SECURITY_KEY);

                string passcode = EncryptionManager.DecryptString(encryptedPasscode,
                                                                    StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                    StaticFields.EXTERNAL_SECURITY_KEY);
                string responseMessage;

                return new Response<long?>(_userMan.GetUserAuthorisationPin(username, passcode, branchId, languageId, auditUserId, auditWorkstation, out responseMessage),
                                                ResponseType.SUCCESSFUL,
                                                responseMessage,
                                                log.IsDebugEnabled || log.IsTraceEnabled ? responseMessage : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long?>(null,
                                            ResponseType.ERROR,
                                            "An error occured during processing your request, please try again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Persist new user group to DB
        /// </summary>
        /// <param name="userGroupName"></param>
        /// <param name="role"></param>
        /// <param name="issuerId"></param>
        /// <param name="allBranchAccess"></param>
        /// <param name="branchIdList"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstartion"></param>
        /// <returns></returns>
        internal Response<int> CreateUserGroup(user_group userGroup, List<int> branchIdList, int language, long auditUserId, string auditWorkstartion)
        {
            try
            {
                int userGroupId;
                string response;

                if (_userGroupMan.CreateUserGroup(userGroup, branchIdList, language, auditUserId, auditWorkstartion, out userGroupId, out response))
                {
                    return new Response<int>(userGroupId,
                                             ResponseType.SUCCESSFUL,
                                             response,
                                             "");
                }

                return new Response<int>(0,
                                         ResponseType.UNSUCCESSFUL,
                                         response,
                                         response);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(0,
                                         ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Persist changes to user group to DB
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="userGroupName"></param>
        /// <param name="role"></param>
        /// <param name="issuerId"></param>
        /// <param name="allBranchAccess"></param>
        /// <param name="branchIdList"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstartion"></param>
        /// <returns></returns>
        internal BaseResponse UpdateUserGroup(user_group userGroup, List<int> branchIdList, int language, long auditUserId, string auditWorkstartion)
        {
            try
            {
                string response;

                if (_userGroupMan.UpdateUserGroup(userGroup, branchIdList, language, auditUserId, auditWorkstartion, out response))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL,
                                            response,
                                            "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        response,
                                        response);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Delete a user group.
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        internal BaseResponse DeleteUserGroup(int userGroupId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_userGroupMan.DeleteUserGroup(userGroupId, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
                else
                {
                    return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Returns a list of branch id's associated with the user group.
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        internal Response<List<BranchIdResult>> GetBranchesForUserGroup(int userGroupId)
        {
            try
            {
                return new Response<List<BranchIdResult>>(_userGroupMan.GetBranchesForUserGroup(userGroupId),
                                                ResponseType.SUCCESSFUL,
                                                ResponseType.SUCCESSFUL.ToString(),
                                                "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<BranchIdResult>>(null,
                                                        ResponseType.ERROR,
                                                        "An error occured during processing your request, please try again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Fetch a list of user groups which also indicates if the user is assigned to them.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="userRole">May be null to not filter by user role.</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal Response<List<UserGroupAdminResult>> GetUserGroupForUserAdmin(int? issuerId, int? userRole, long? userId, int? branchId)
        {
            try
            {
                return new Response<List<UserGroupAdminResult>>(_userMan.GetUserGroupForUserAdmin(issuerId, userRole, userId, branchId),
                                                                ResponseType.SUCCESSFUL,
                                                                ResponseType.SUCCESSFUL.ToString(),
                                                                "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<UserGroupAdminResult>>(null,
                                                                ResponseType.ERROR,
                                                                "An error occured during processing your request, please try again.",
                                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }


        internal Response<List<UserGroupAdminResult>> GetUserGroupForPendingUserAdmin(int? issuerId, int? userRole, long? userId, int? branchId)
        {
            try
            {
                return new Response<List<UserGroupAdminResult>>(_userMan.GetUserGroupForPendingUserAdmin(issuerId, userRole, userId, branchId),
                                                                ResponseType.SUCCESSFUL,
                                                                ResponseType.SUCCESSFUL.ToString(),
                                                                "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<UserGroupAdminResult>>(null,
                                                                ResponseType.ERROR,
                                                                "An error occured during processing your request, please try again.",
                                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
        /// <summary>
        /// Fetch all the groups a user belongs to as well as the issuer and role of the group is apart of.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<GroupsRolesResult>> GetGroupRolesForUser(long userId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<GroupsRolesResult>>(_userMan.GetGroupRolesForUser(userId, languageId, auditUserId, auditWorkstation),
                                                             ResponseType.SUCCESSFUL,
                                                             ResponseType.SUCCESSFUL.ToString(),
                                                             "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<GroupsRolesResult>>(null,
                                                             ResponseType.ERROR,
                                                             "An error occured during processing your request, please try again.",
                                                             log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }


        /// <summary>
        /// Fetch all the groups a user belongs to as well as the issuer and role of the group is apart of.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<GroupsRolesResult>> GetGroupRolesForPendingUser(long userId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<GroupsRolesResult>>(_userMan.GetGroupRolesForPendingUser(userId, languageId, auditUserId, auditWorkstation),
                                                             ResponseType.SUCCESSFUL,
                                                             ResponseType.SUCCESSFUL.ToString(),
                                                             "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<GroupsRolesResult>>(null,
                                                             ResponseType.ERROR,
                                                             "An error occured during processing your request, please try again.",
                                                             log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal BaseResponse RejectUserRequest(long? pendingUserId, long audit_user_id, string audit_workstation)
        {
            try
            {
                _userMan.RejectUserRequest(pendingUserId, audit_user_id, audit_workstation);

                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        ResponseType.SUCCESSFUL.ToString(),
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
        internal BaseResponse ApproveUser(long? pendingUserId, long audit_user_id, string audit_workstation)
        {
            try
            {
                _userMan.ApproveUser(pendingUserId, audit_user_id, audit_workstation);

                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        ResponseType.SUCCESSFUL.ToString(),
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Log user out and destroy session.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse UserLogOut(long userId, long auditUserId, string auditWorkstation)
        {
            try
            {
                //_sessMan.KillSession(userId);
                _userMan.FinaliseUserLogout(userId, auditUserId, auditWorkstation);

                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        ResponseType.SUCCESSFUL.ToString(),
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Reset user logged in status to logged out.
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="username"></param>
        /// <param name="activeHost"></param>
        /// <returns></returns>
        internal BaseResponse ResetUserLoginStatus(long userId, long auditUserId, string auditWorkstation)
        {
            try
            {
                _sessMan.KillSession(userId); //Find user in session and remove.
                _userMan.FinaliseUserLogout(userId, auditUserId, auditWorkstation);

                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        ResponseType.SUCCESSFUL.ToString(),
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Get user group data.
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<user_group> GetUserGroup(int userGroupId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<user_group>(_userGroupMan.GetUserGroup(userGroupId, auditUserId, auditWorkstation),
                                                                           ResponseType.SUCCESSFUL,
                                                                           ResponseType.SUCCESSFUL.ToString(),
                                                                           "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<user_group>(null,
                                                ResponseType.ERROR,
                                                "An error occured during processing your request, please try again.",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Calls the DB and returns a list of roles and the issuers linked to the roles for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal Response<UserRolesAndFlows> GetUserRoles(long userId)
        {
            try
            {
                var rolesResult = _userMan.GetUserRoles(userId);
                //Fetch menu access list
                var statusFlowRoles = _userMan.getStatusFlowRoles(rolesResult.Select(s => s.user_role_id).Distinct().ToList());

                return new Response<UserRolesAndFlows>(new UserRolesAndFlows { Roles = rolesResult, StatusFlows = statusFlowRoles },
                                                             ResponseType.SUCCESSFUL,
                                                             "",
                                                             "");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<UserRolesAndFlows>(null,
                                                             ResponseType.ERROR,
                                                             "Error when processing request.",
                                                             log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Gets a list of users who are linked to the specified branch.
        /// </summary>
        /// <param name="branch_id"></param>
        /// <param name="branchStatusId"></param>
        /// <param name="userStatus"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal Response<List<user_list_result>> GetUsersByBranch(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<List<user_list_result>>(_userMan.GetUsersByBranch(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation),
                                                ResponseType.SUCCESSFUL,
                                                "",
                                                "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<user_list_result>>(null,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }



        internal Response<List<user_list_result>> GetUsersPendingForApproval(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<List<user_list_result>>(_userMan.GetUsersPendingForApproval(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation),
                                                ResponseType.SUCCESSFUL,
                                                "",
                                                "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<user_list_result>>(null,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<List<user_list_result>> GetUsersByBranchAdmin(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<List<user_list_result>>(_userMan.GetUsersByBranchAdmin(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation),
                                                ResponseType.SUCCESSFUL,
                                                "",
                                                "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<user_list_result>>(null,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Get a list of users who do not belong to any users groups.
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal Response<List<user_list_result>> GetUnassignedUsers(int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<List<user_list_result>>(_userMan.GetUnassignedUsers(languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<user_list_result>>(null,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        /// <summary>
        /// Get a list of users by search criteria.
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal Response<List<user_list_result>> GetUserList(string username, string firstname, string lastname, string branchid, string userrole, int issuerid, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<List<user_list_result>>(_userMan.GetUserList(username, firstname, lastname, branchid, userrole, issuerid, pageIndex, rowsPerPage, auditUserId, auditWorkStation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<user_list_result>>(null,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }



        internal BaseResponse UpdateUserLanguage(long userId, int languageId, string auditWorkstation)
        {
            try
            {
                if (_userMan.UpdateUserLanguage(userId, languageId, auditWorkstation))
                {
                    _sessMan.UpdateSessionLanguage(userId, languageId);
                    return new BaseResponse(ResponseType.SUCCESSFUL, "", "");
                }
                else
                {
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, "Something went wrong", "");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        internal Response<List<LangLookup>> GetLangUserRoles(int languageId, int? enterprise, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_userMan.GetLangUserRoles(languageId, enterprise, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<useradminsettingslist> GetUseradminSettings(long? auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<useradminsettingslist>(_userMan.GetUseradminSettings(auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<useradminsettingslist>(null,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse UpdateUseradminSettings(useradminsettingslist item, long? userId, string auditWorkstation)
        {
            try
            {
                if (_userMan.UpdateUseradminSettings(item, userId, auditWorkstation))
                {

                    return new BaseResponse(ResponseType.SUCCESSFUL, "Record Updated Sucessfully.", "");
                }
                else
                {
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, "Something went wrong", "");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<int?> CreateUseradminSettings(useradminsettingslist item, long? userId, string auditWorkstation)
        {
            try
            {
                int user_admin_Id = 0;
                if (_userMan.CreateUseradminSettings(item, userId, auditWorkstation, out user_admin_Id))
                {

                    return new Response<int?>(user_admin_Id, ResponseType.SUCCESSFUL, "Record Created Successfully.", "");
                }
                else
                {
                    return new Response<int?>(user_admin_Id, ResponseType.UNSUCCESSFUL, "Something went wrong", "");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int?>(null,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<bool> SendChallenge(int authconfigurationId, String auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage = string.Empty, token = string.Empty;

                if (_userMan.SendChallenge(authconfigurationId, auditUserId, auditWorkstation, out token, out responseMessage))
                {

                    return new Response<bool>(true, ResponseType.SUCCESSFUL, "Sent Successfully.", "");
                }
                else
                {
                    return new Response<bool>(false, ResponseType.UNSUCCESSFUL, "Something went wrong", "");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<bool> VerifyChallenge(int authconfigurationId, string token, string username, string auditWorkstation)
        {
            try
            {
                string responseMessage = string.Empty;
                if (_userMan.VerifyChallenge(authconfigurationId, token, username, auditWorkstation, out responseMessage))
                {

                    return new Response<bool>(true, ResponseType.SUCCESSFUL, "Verified Successfully.", "");
                }
                else
                {
                    return new Response<bool>(false, ResponseType.UNSUCCESSFUL, "Something went wrong", "");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false,
                                                ResponseType.ERROR,
                                                "Error Processing Request",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
    }
}