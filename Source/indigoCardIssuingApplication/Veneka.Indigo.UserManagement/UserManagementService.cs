
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Veneka.Indigo.UserManagement.dal;
using Veneka.Indigo.UserManagement.objects;
using Veneka.Indigo.Security;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.Common.Exceptions;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common;
using Veneka.Indigo.IssuerManagement;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using Common.Logging;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.Config;

namespace Veneka.Indigo.UserManagement
{
    public class UserManagementService
    {
       
        private readonly IssuerService issuerMan = new IssuerService();
        private readonly AuthConfigurationService authConfig = new AuthConfigurationService();

        private readonly IUserGroupDAL userGroupDAL;
        private readonly IResponseTranslator _translator;
        private readonly IUserDataManagementDAL _dataSource;
        
        public UserManagementService() : this(new UserDataManagement(),new UserGroupDAL(),new AuthConfigurationService(),new ResponseTranslator())
        {

        }
        public UserManagementService(IUserDataManagementDAL dataSource,IUserGroupDAL _usergroupDAL, AuthConfigurationService authservice, IResponseTranslator responseTranslator)
        {
            _dataSource = dataSource;
            userGroupDAL = _usergroupDAL;
            authConfig = authservice;
            _translator = responseTranslator;
        }
        public UserManagementService(IUserDataManagementDAL dataSource)
        {
            _dataSource = dataSource;
        }

       
        private static readonly ILog ldaplog = LogManager.GetLogger("LDAPLogging");

        private bool allowIndigoAuth = bool.Parse(ConfigurationManager.AppSettings["AllowIndigoAuth"].ToString());

        #region Login Section
        /// <summary>
        /// Perform user login
        /// </summary>
        /// <param name="encryptedUserName"></param>
        /// <param name="encryptedPassword"></param>
        /// <param name="encryptedWorkstation"></param>
        /// <returns></returns>
        public login_user LogIn(string encryptedUserName, string encryptedPassword, string encryptedWorkstation, out bool mustChangePassword)
        {
            mustChangePassword = false;

            string decryptedUserName = EncryptionManager.DecryptString(encryptedUserName,
                                                                       StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                       StaticFields.EXTERNAL_SECURITY_KEY);

            string decryptedPassword = EncryptionManager.DecryptString(encryptedPassword,
                                                                       StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                       StaticFields.EXTERNAL_SECURITY_KEY);

            string decryptedWorkstation = EncryptionManager.DecryptString(encryptedWorkstation,
                                                                          StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                          StaticFields.EXTERNAL_SECURITY_KEY);

            login_user result = _dataSource.LogIn(decryptedUserName, decryptedPassword, decryptedWorkstation);

            AuthConfigResult auth_configuration = new AuthConfigResult();
            List<auth_configuration_connectionparams_result> connectonParams = new List<auth_configuration_connectionparams_result>();

            if (result != null)
            {
                bool loginSuccess = false;

                auth_configuration = authConfig.GetAuthConfiguration(result.authentication_configuration_id);

                if (auth_configuration.AuthConfigConnectionParams != null)
                    connectonParams = auth_configuration.AuthConfigConnectionParams.ToList();

                if (result.user_status_id == 0)
                {
                    //If login_issuer_id is null it mean that LDAP has not been configured for the users, 
                    //therefore perform normal password check.
                    if (connectonParams.Count > 0)
                    {
                        if (connectonParams.Any(i => i.auth_type_id == null))
                        {
                            loginSuccess = IndigoAuthLogin(result, decryptedPassword, out mustChangePassword);
                        }
                        //LDAP
                        else if (connectonParams.Any(i => i.connection_parameter_type_id == 4))
                        {
                            loginSuccess = ActiveDirectoryLogin(result, decryptedPassword);
                        }
                        else
                        //EXTERNAL AUTH
                        if (connectonParams.Any(i => i.connection_parameter_type_id == 0) && (bool)result.is_external_auth)
                        {
                            loginSuccess = ExternalAuthLogin(result, connectonParams[0], decryptedPassword, decryptedWorkstation);
                        }
                        else
                            throw new Exception("Unknown authentication type, please contact support.");
                    }
                }
                else
                    throw new LoginFailedException("Your Indigo account is not active. Please contact support.", SystemResponseCode.LOGIN_FAILED_ACCOUNT_INACTIVE);

                _dataSource.FinaliseLogin(loginSuccess, result.user_id, decryptedWorkstation);

                if (!loginSuccess)
                    throw new LoginFailedException("Username and/or password incorrect.", SystemResponseCode.LOGIN_FAIL_CREDENTIALS);

                result.workstation = decryptedWorkstation;
            }
            else
            {
                throw new LoginFailedException("Username and/or password incorrect.", SystemResponseCode.LOGIN_FAIL_CREDENTIALS);
            }

            return result;
        }


        private bool IndigoAuthLogin(login_user result, string decryptedPassword, out bool mustChangePassword)
        {
            bool loginSuccess = false;
            mustChangePassword = false;

            if (allowIndigoAuth)
            {
                string decryptedDBPassword = EncryptionManager.DecryptString(result.clr_password,
                                                                   StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                   StaticFields.DB_SECURITY_KEY);

                var usersettings = GetUseradminSettings(null, null);
                //int maxInvalidPasswordAttempts = 0, validattempts = 0, PasswordAttemptLockoutDuration = 0;

                ////Check number of invalid login attempts
                //if (usersettings != null && usersettings.maxInvalidPasswordAttempts != null)
                //{
                //    maxInvalidPasswordAttempts = (int)usersettings.maxInvalidPasswordAttempts;
                //    PasswordAttemptLockoutDuration = (int)usersettings.PasswordAttemptLockoutDuration;
                //}

                //if (result.number_of_incorrect_logins == null)
                //    result.number_of_incorrect_logins = 0;
                if (usersettings == null)
                    throw new Exception("please check useradmin settings.");

                if (decryptedDBPassword.Equals(decryptedPassword))
                {
                    //Check if user needs to change password
                    if (result.user_id >= 0 && result.last_password_changed_date == null) //Password never set by user and must be changed
                    {
                        mustChangePassword = true;
                    }
                    //Check if last changed date has elapsed, Default to 30 days if no value
                    else if (result.user_id >= 0 && result.last_password_changed_date.Value.AddDays(usersettings.PasswordValidPeriod ?? 30) < DateTime.Now)
                    {
                        mustChangePassword = true;
                    }

                    loginSuccess = true;
                }
                //else //Work out how many more password reties the user has
                //{
                //    validattempts = maxInvalidPasswordAttempts - (int)result.number_of_incorrect_logins;

                //    DateTime last_login_attempt = DateTime.Now;
                //    if (result.last_login_attempt != null)
                //        last_login_attempt = result.last_login_attempt.Value.AddHours(PasswordAttemptLockoutDuration);
                //    else
                //        last_login_attempt = DateTime.Now;


                //    if (validattempts == 0 && last_login_attempt > DateTime.Now)
                //        throw new LoginFailedException("Your Indigo account is not active. Please contact support.", SystemResponseCode.LOGIN_FAILED_ACCOUNT_INACTIVE);
                //    else
                //        if (validattempts > 1)
                //        throw new LoginFailedException("Username and/or password incorrect. You have " + validattempts + " login attempts for Indigo account. ", SystemResponseCode.LOGIN_FAIL_CREDENTIALS);
                //    else if (validattempts == 1)
                //        throw new LoginFailedException("Username and/or password incorrect. You have last chance to Indigo account. ", SystemResponseCode.LOGIN_FAIL_CREDENTIALS);
                //}
            }
            else
            {
                throw new LoginFailedException("Your Indigo account is not active. Please contact support.", SystemResponseCode.LOGIN_FAILED_ACCOUNT_INACTIVE);
            }

            return loginSuccess;
        }


        private bool ActiveDirectoryLogin(login_user result, string decryptedPassword)
        {
            bool loginSuccess = false;

            try
            {
                ldaplog.Debug(m => m("User requires LDAP authentication."));
                ldaplog.Debug(m => m("Starting LDAP authentication..."));

                result.last_password_changed_date = DateTime.Now.AddDays(7);

                //Determine if another account needs to be used to connect to AD
                string domain_username = String.IsNullOrWhiteSpace(result.domain_username) ? null : result.domain_username;
                string domain_password = String.IsNullOrWhiteSpace(result.domain_password) ? null : result.domain_password;

                ldaplog.Trace(m => m("LDAP Server Settings="));
                ldaplog.Trace(m => m("LDAP Server address: " + result.domain_hostname_or_ip));
                ldaplog.Trace(m => m("LDAP Path: " + result.domain_path));
                ldaplog.Trace(m => m("LDAP Username: " + domain_username));
                //ldaplog.Trace(m => m("LDAP Password: " + domain_password));

                //Connect to domain controller, using host name, path and the user's username and password.
                //Note: username is without domain e.g. not like domain\username.
                using (var adContext = new PrincipalContext(ContextType.Domain,
                                                            result.domain_hostname_or_ip,
                                                            result.domain_path,
                                                            domain_username, domain_password))
                {
                    int value = 1;
                    if (ConfigurationManager.AppSettings["ContextOptions"] != null)
                    {
                        value = int.Parse(ConfigurationManager.AppSettings["ContextOptions"]);
                    }
                    ldaplog.Debug(m => m("Connected to LDAP Server... Validating user's credentials."));
                  
                    
                    //Validate the users credentials
                    loginSuccess = adContext.ValidateCredentials(result.clr_username, decryptedPassword, (ContextOptions)value);
                    ldaplog.Trace(m => m("loginSuccess: " + loginSuccess.ToString()));

                    ldaplog.Debug(m => m("Looking up user object to check locked out status."));

                    //Fetch the users object, for lockout checking;
                    UserPrincipal usr = UserPrincipal.FindByIdentity(adContext,
                                           IdentityType.SamAccountName,
                                           result.clr_username);

                    if (usr == null)
                    {
                        ldaplog.Trace(m => m("UserPrincipal object is null."));
                        throw new LoginFailedException("Your Windows account is not active. Please contact Windows support.", SystemResponseCode.LOGIN_FAILED_ACCOUNT_INACTIVE);
                    }

                    if (usr.Enabled == null || usr.Enabled == false)
                    {
                        if (ldaplog.IsTraceEnabled)
                            ldaplog.TraceFormat("UserPrincipal object Enabled is {}.", usr.Enabled == null ? "null" : "false");
                        throw new LoginFailedException("Your Windows account is not active. Please contact Windows support.", SystemResponseCode.LOGIN_FAILED_ACCOUNT_INACTIVE);
                    }

                    if (usr.IsAccountLockedOut() == true)
                    {
                        ldaplog.Trace(m => m("UserPrincipal object IsAccountLockedOut() is true."));
                        throw new LoginFailedException("Your Windows account is not active. Please contact Windows support.", SystemResponseCode.LOGIN_FAILED_ACCOUNT_INACTIVE);
                    }
                }
            }
            catch (PrincipalServerDownException psde)
            {
                ldaplog.Error(psde);
                throw new LoginFailedException("Cannot connect to Active Directory for user authentication. Please contact support", SystemResponseCode.LDAP_SERVER_DOWN);
            }
            //catch (Exception ex)
            //{
            //    ldaplog.Error(ex);
            //}

            return loginSuccess;
        }


        private bool ExternalAuthLogin(login_user result, auth_configuration_connectionparams_result AuthConfig, string decryptedPassword, string decryptedWorkstation)
        {
            bool loginSuccess = false;

            if (AuthConfig.external_interface_id == null)
                throw new LoginFailedException("External interface is not found for this user. Please contact support.", SystemResponseCode.ACCOUNT_VALIDATION_FAIL);

            IConfig config = ConfigFactory.GetConfig((int)AuthConfig.connection_parameter_type_id);

            if (config is ActiveDirectoryConfig)
                config = new ActiveDirectoryConfig(AuthConfig.external_interface_id, result.domain_hostname_or_ip, result.port.Value, result.path, null, result.domain_username, result.domain_password);
            else if (config is WebServiceConfig)
                config = new WebServiceConfig(Guid.Parse(AuthConfig.external_interface_id), (Protocol)result.protocol, result.domain_hostname_or_ip, result.port.Value, result.path, null, (AuthType)result.auth_type.Value, result.domain_username, result.domain_password, null);
            else
                throw new ArgumentException("Only Active Directory and Webservice configurations are supported. Please contact support.");

            ExternalAuthIntegrationController _integration = ExternalAuthIntegrationController.Instance;
            string resultmessage;
            if (_integration.ExternalAuthentication(AuthConfig.external_interface_id).Login(config, result.user_id, null, result.clr_username, decryptedPassword, 0, result.user_id, decryptedWorkstation, out resultmessage))
            {
                loginSuccess = true;
            }
            else
            {
                ldaplog.Error(resultmessage);
                throw new LoginFailedException(resultmessage, SystemResponseCode.ACCOUNT_VALIDATION_FAIL);
            }

            return loginSuccess;
        }



        #endregion

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

        public decrypted_user LdapLookup(string usernameLookup, string username, string userpassword, int authConfigId, long auditUserId, string auditWorkstation)
        {
            decrypted_user rtnUser = new decrypted_user();
            string externalinterfaceid = string.Empty;
            AuthConfigResult auth_configuration = new AuthConfigResult();
            List<auth_configuration_connectionparams_result> connectonParams = new List<auth_configuration_connectionparams_result>();

            auth_configuration = authConfig.GetAuthConfiguration(authConfigId);

            if (auth_configuration.AuthConfigConnectionParams != null)
                connectonParams = auth_configuration.AuthConfigConnectionParams.Where(i => i.auth_type_id == (int)AuthTypes.ExternalAuth).ToList();

            try
            {
                if (connectonParams.Count > 0)
                {
                    ldaplog.Debug(m => m("User requires LDAP authentication."));

                    var result = issuerMan.GetConnectionParameter((int)connectonParams[0].connection_parameter_id, auditUserId, auditWorkstation);
                    externalinterfaceid = connectonParams[0].external_interface_id;
                    ConnectionParamsResult connectionparam = result.ConnectionParams;



                    if (connectionparam.connection_parameter_type_id == 4)//Perform LDAP password check
                    {
                        string domain_username = String.IsNullOrWhiteSpace(connectionparam.username) ? username : connectionparam.username;
                        string domain_password = String.IsNullOrWhiteSpace(connectionparam.password) ? userpassword : connectionparam.password;



                        if (domain_username.Equals(""))
                        {
                            domain_username = null;
                        }

                        if (domain_password.Equals(""))
                        {
                            domain_password = null;
                        }

                        //Connect to domain controller, using host name, path and the user's username and password.
                        //Note: username is without domain e.g. not like domain\username.
                        using (var adContext = new PrincipalContext(ContextType.Domain,
                                                                    connectionparam.address,
                                                                    connectionparam.path,
                                                                    domain_username, domain_password))
                        {

                            ldaplog.Debug(m => m("Looking up user object"));

                            //Fetch the users object, for lockout checking;
                            UserPrincipal usr = UserPrincipal.FindByIdentity(adContext,
                                                                             IdentityType.SamAccountName,
                                                                             usernameLookup);

                            if (usr != null)
                            {
                                rtnUser.first_name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((usr.GivenName ?? "").ToLower());
                                rtnUser.last_name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((usr.Surname ?? "").ToLower());
                                rtnUser.user_email = (usr.EmailAddress ?? "").ToLower();

                                ldaplog.Debug(m => m("Successful user lookup, returning..."));
                            }
                            else
                            {
                                rtnUser = null;
                                ldaplog.Debug(m => m("Unsuccessful user lookup, returning..."));
                            }
                        }
                    }
                    else if (connectionparam.connection_parameter_type_id == 0 && (bool)connectionparam.is_external_auth)//Perform LDAP password check
                    {
                        if (externalinterfaceid == null)
                            throw new LoginFailedException("External interface is not found for selected authentication type.", SystemResponseCode.PARAMETER_ERROR);

                        string domain_username = String.IsNullOrWhiteSpace(connectionparam.username) ? username : connectionparam.username;
                        string domain_password = String.IsNullOrWhiteSpace(connectionparam.password) ? userpassword : connectionparam.password;

                        //Parameters obj = new Parameters(result.path, result.address, 0, (Parameters.protocol)result.protocol.GetValueOrDefault(0), (Parameters.authType)result.auth_type, domain_username, domain_password, null);
                        IConfig config = ConfigFactory.GetConfig(connectionparam.connection_parameter_type_id);

                        //TODO: Implement other configs
                        if (config is ActiveDirectoryConfig)
                            config = new ActiveDirectoryConfig(externalinterfaceid, connectionparam.address, 0, connectionparam.path, null, domain_username, domain_password);
                        else if (config is WebServiceConfig)
                            config = new WebServiceConfig(Guid.Parse(externalinterfaceid), (Protocol)connectionparam.protocol, connectionparam.address, connectionparam.port, connectionparam.path, null, (AuthType)connectionparam.auth_type, domain_username, domain_password, null);

                        ExternalAuthIntegrationController _integration = ExternalAuthIntegrationController.Instance;
                        string resultmessage;

                        UserObject objuser = _integration.ExternalAuthentication(externalinterfaceid).GetUserDetails(config, null, null, usernameLookup, 0, auditUserId, auditWorkstation, out resultmessage);
                        if (objuser != null)
                        {
                            rtnUser.first_name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((objuser.FirstName ?? "").ToLower());
                            rtnUser.last_name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((objuser.LastName ?? "").ToLower());
                            rtnUser.user_email = (objuser.Email ?? "").ToLower();

                            ldaplog.Debug(m => m("Successful user lookup, returning..."));

                        }
                        else
                        {
                            rtnUser = null;
                            ldaplog.Error(resultmessage);



                        }
                    }
                    else
                    {
                        ldaplog.Error("Problem getting authConfig settings for:\t" + authConfigId);
                        throw new BaseIndigoException("There is an issue with the configuration settings, please check settings.", SystemResponseCode.CONFIGURATION_ERROR, null);
                    }
                }
                else
                {
                    ldaplog.Error("Problem getting Connection Parmeter for:\t" + authConfigId);
                    throw new BaseIndigoException("There is an issue with the configuration settings, please check settings.", SystemResponseCode.CONFIGURATION_ERROR, null);
                }

                return rtnUser;
            }
            catch (PrincipalOperationException poex)
            {
                ldaplog.Error(poex);
                throw new BaseIndigoException("Problem looking up user, please contact support.", SystemResponseCode.LDAP_LOOKUP_FAILED, poex);
            }
            catch (PrincipalServerDownException pex)
            {
                ldaplog.Error(pex);
                throw new BaseIndigoException("Cannot connect to Domain controler.", SystemResponseCode.LDAP_SERVER_DOWN, pex);
            }
            catch (Exception ex)
            {
                ldaplog.Error(ex);
                throw new BaseIndigoException("There is an issue with trying to do an LDAP lookup, please contact support.", SystemResponseCode.LDAP_GENERAL_FAILURE, ex);
            }

            //return null;
        }
        /// <summary>
        /// Persist user to the DB.
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        public bool CreateUser(user createUser, List<int> userGroupList, int languageId, long auditUserId, string auditWorkstation, out long? newUserId, out string responseMessage)
        {
            string validPassword;
            string validationException;
             if(TryDecryptAndValidatePassword(UserDataManagement.GetString(createUser.password), out validationException, out validPassword))
            {
                //Change Passwords encryption to that of DB
                string encryptedPassword = EncryptionManager.EncryptString(validPassword,
                                                                            StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                            StaticFields.DB_SECURITY_KEY);

                var responseCode = _dataSource.CreateUser(createUser, encryptedPassword, userGroupList, auditUserId, auditWorkstation, out newUserId);
                responseMessage = _translator.TranslateResponseCode(responseCode, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

                if (responseCode == SystemResponseCode.SUCCESS)
                {
                    return true;
                }
            }
            else
            {
                responseMessage = _translator.TranslateResponseCode(SystemResponseCode.USER_PASSWORD_VALIDATION_FAILED, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            }

            newUserId = 0;

            return false;
        }

        /// <summary>
        /// Persist changes to the user object. This will not persist the users password.
        /// </summary>
        /// <param name="updateUser"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool UpdateUser(user updateUser, List<int> userGroupList, int languageId, long auditUserId, string auditWorkstation, out long? newUserId, out string responseMessage)
        {

            var responseCode = _dataSource.CreateUser(updateUser, null, userGroupList, auditUserId, auditWorkstation, out newUserId);//inserting new pending user record.


            responseMessage = _translator.TranslateResponseCode(responseCode, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (responseCode == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// approve pending user requests
        /// </summary>
        /// <param name="pendingUserId"></param>
        /// <param name="audit_user_id"></param>
        /// <param name="audit_workstation"></param>
        /// <returns></returns>
        public bool ApproveUser(long? pendingUserId, long audit_user_id, string audit_workstation)
        {
            return _dataSource.ApproveUser(pendingUserId, audit_user_id, audit_workstation);
        }

        public bool RejectUserRequest(long? pendingUserId, long audit_user_id, string audit_workstation)
        {
            return _dataSource.RejectUserRequest(pendingUserId, audit_user_id, audit_workstation);
        }

        /// <summary>
        /// Persist new password for user to DB.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="encryptedPassword"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool ResetUserPassword(long userId, string encryptedPassword, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            string validPassword;
            string validationException;
            if (TryDecryptAndValidatePassword(encryptedPassword, out validationException, out validPassword))
            {
                //Change Passwords encryption to that of DB
                string newEncryptedPassword = EncryptionManager.EncryptString(validPassword,
                                                                              StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                              StaticFields.DB_SECURITY_KEY);
                responseMessage = "";
                _dataSource.ResetUserPassword(userId, newEncryptedPassword, auditUserId, auditWorkstation);
                return true;
            }
            else
            {
                responseMessage = _translator.TranslateResponseCode(SystemResponseCode.USER_PASSWORD_VALIDATION_FAILED, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            }

            return false;
        }

        /// <summary>
        /// Capture Branch Custodians authorisation pin 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="encryptedPin"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public bool InsertUserAuthorisationPin(long userId, string encryptedPin, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var responseCode = _dataSource.UpdateAuthorisationPin(userId, encryptedPin, auditUserId, auditWorkstation);

            responseMessage = _translator.TranslateResponseCode(responseCode, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (responseCode == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Persist new password to the DB if the old password is correct and new password doesnt match any passwords previously used.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="encryptedPassword"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool UpdateUserPassword(long userId, string encryptedOldPassword, string encryptedNewPassword, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            bool CurrentPasswordVerified = false;
            bool newPasswordMatchOldPasswords = false;
            string validOldPassword;
            string validNewPassword;
            string validationException;
            SystemResponseCode response = SystemResponseCode.GENERAL_FAILURE;

            if (TryDecryptAndValidatePassword(encryptedOldPassword, out validationException, out validOldPassword))
            {
                if (TryDecryptAndValidatePassword(encryptedNewPassword, out validationException, out validNewPassword))
                {
                    //get the current user's previous passwords
                    var userPasswordResults = _dataSource.GetUserPasswords(userId);
                    if (userPasswordResults != null)
                    {
                        //Loop through all passwords, DB should return current password with date + 99 days and all old password.
                        //Make sure the current password matches the old password and the new password is not the same as the current or previous passwords.
                        foreach (user_passwords userPassword in userPasswordResults)
                        {
                            string decryptedUserPassword = EncryptionManager.DecryptString(userPassword.password,
                                                                                            StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                                            StaticFields.DB_SECURITY_KEY);
                            if (userPassword.date > DateTime.Now)
                            {
                                if (decryptedUserPassword.Equals(validOldPassword))
                                {
                                    CurrentPasswordVerified = true;
                                }
                            }

                            if (decryptedUserPassword.Equals(validNewPassword))
                            {
                                newPasswordMatchOldPasswords = true;
                            }
                        }

                        //No previous passwords so allow
                        if (userPasswordResults.Count == 0)
                            CurrentPasswordVerified = true;


                        //Check if new password matches a previous password
                        if (CurrentPasswordVerified && !newPasswordMatchOldPasswords)
                        {
                            string dbEncryptedNewPassword = EncryptionManager.EncryptString(validNewPassword,
                                                                                            StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                                            StaticFields.DB_SECURITY_KEY);
                            _dataSource.ResetUserPassword(userId, dbEncryptedNewPassword, userId, auditWorkstation);
                            responseMessage = "";
                            return true;
                        }
                        else
                            response = SystemResponseCode.USER_PASSWORD_MATCHES_PREVIOUS_PASSWORD;

                    }
                    else
                    {
                        response = SystemResponseCode.USER_PASSWORD_VALIDATION_FAILED;
                    }
                }
                else
                {
                    response = SystemResponseCode.USER_PASSWORD_VALIDATION_FAILED;
                }
            }

            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            return false;
        }

        /// <summary>
        /// Finalise logout for user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="audit_user_id"></param>
        /// <param name="audit_workstation"></param>
        public void FinaliseUserLogout(long userId, long audit_user_id, string audit_workstation)
        {
            _dataSource.FinaliseUserLogout(userId, audit_user_id, audit_workstation);
        }

        private string DecodeDBResponse(DBResponseMessage dbResponse)
        {
            switch (dbResponse)
            {
                case DBResponseMessage.SUCCESS:
                    return "";
                case DBResponseMessage.INCORRECT_STATUS:
                    return dbResponse.ToString();
                case DBResponseMessage.CARD_ALREADY_ISSUED:
                    return dbResponse.ToString();
                case DBResponseMessage.INCORRECT_BRANCH:
                    return dbResponse.ToString();
                case DBResponseMessage.NO_RECORDS_RETURNED:
                    return dbResponse.ToString();
                case DBResponseMessage.DUPLICATE_RECORD:
                    throw new BaseIndigoException("Duplicate username, please use another username", SystemResponseCode.DUPLICATE_USERNAME, null);
                case DBResponseMessage.SPROC_ERROR:
                    return dbResponse.ToString();
                case DBResponseMessage.SYSTEM_ERROR:
                    return dbResponse.ToString();
                case DBResponseMessage.FAILURE:
                    return dbResponse.ToString();
                default:
                    return dbResponse.ToString();
            }
        }

        /// <summary>
        /// Decrypts and validates the password is in the correct format. Output will contain validation erros if false or the decrypted password if true.
        /// </summary>
        /// <param name="encryptedPassword"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool TryDecryptAndValidatePassword(string encryptedPassword, out string validationException, out string validPassword)
        {
            validationException = null;
            validPassword = null;
            if (String.IsNullOrWhiteSpace(encryptedPassword))
            {
                validationException = "Password cannot be empty.";
                return false;
            }

            string decryptedPassword = EncryptionManager.DecryptString(encryptedPassword,
                                                                       StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                       StaticFields.EXTERNAL_SECURITY_KEY);

            return TryValidatePassword(decryptedPassword, out validationException, out validPassword);
        }

        /// <summary>
        /// Validates a decrypted password. Output will contain validation erros if false or the valid password if true.
        /// </summary>
        /// <param name="decryptedPassword"></param>
        /// <param name="validationException"></param>
        /// <param name="validPassword"></param>
        /// <returns></returns>
        private bool TryValidatePassword(string decryptedPassword, out string validationException, out string validPassword)
        {
            validationException = null;
            validPassword = null;
            if (String.IsNullOrWhiteSpace(decryptedPassword))
            {
                validationException = "Password cannot be empty.";
                return false;
            }
            var usersettings = GetUseradminSettings(null, null);
            string passwordminlength = "0";
            int passwordmaxlength = 0;
            if (usersettings != null)
            {
                passwordminlength = usersettings.PasswordMinLength.ToString();
                passwordmaxlength = (int)usersettings.PasswordMaxLength;
            }
            else
            {
                validationException = "Please Configure UserAdmin Settings.";
                return false;
            }
            var reg = new Regex(@"^.*(?=.{" + passwordminlength + @",})(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z0-9!@#$%~`&+-=_*]+$");

            if (reg.IsMatch(decryptedPassword))
            {
                validPassword = decryptedPassword;
                return true;
            }
            else if (validPassword.Length > passwordmaxlength)
            {
                validationException = "Password should be a maximum of " + passwordmaxlength + " charecters comprised of atleast 1 numeric value and 1 special charecter";
                return false;

            }
            else
            {
                validationException = "Password should be a minimum of " + passwordminlength + " charecters comprised of atleast 1 numeric value and 1 special charecter";
                return false;
            }
        }

        /// <summary>
        /// Calls the DB and returns a list of roles and the issuers linked to the roles for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Veneka.Indigo.Common.Models.RolesIssuerResult> GetUserRoles(long userId)
        {
            List<RolesIssuerResult> userRoles = userGroupDAL.GetUserRoles(userId);
            List<RolesIssuerResult> rtnUserRoles = new List<RolesIssuerResult>();
            List<IssuersForRoleResult> allIssuers = new List<IssuersForRoleResult>();

            foreach (var item in userRoles)
            {
                if (item.issuer_id < 0) //less than 0 indicates an enterprise wide issuer.
                {
                    //Fetch all issuers add to list
                    if (allIssuers.Count == 0)
                    {
                        allIssuers = userGroupDAL.GetAllIssuers(userId);
                    }

                    foreach (var subitem in allIssuers)
                    {
                        RolesIssuerResult roleIssuer = new RolesIssuerResult();
                        roleIssuer.issuer_code = subitem.issuer_code;
                        roleIssuer.issuer_id = subitem.issuer_id;
                        roleIssuer.issuer_name = subitem.issuer_name;
                        roleIssuer.maker_checker_YN = subitem.maker_checker_YN;
                        roleIssuer.account_validation_YN = subitem.account_validation_YN;
                        roleIssuer.allow_multiple_login = item.allow_multiple_login;
                        roleIssuer.auto_create_dist_batch = subitem.auto_create_dist_batch;
                        roleIssuer.classic_card_issue_YN = subitem.classic_card_issue_YN;
                        roleIssuer.instant_card_issue_YN = subitem.instant_card_issue_YN;
                        roleIssuer.enable_instant_pin_YN = subitem.enable_instant_pin_YN;
                        roleIssuer.authorise_pin_issue_YN = subitem.authorise_pin_issue_YN;
                        roleIssuer.authorise_pin_reissue_YN = subitem.authorise_pin_reissue_YN;
                        roleIssuer.pin_mailer_printing_YN = subitem.pin_mailer_printing_YN;
                        roleIssuer.pin_mailer_reprint_YN = subitem.pin_mailer_reprint_YN;
                        roleIssuer.card_ref_preference = subitem.card_ref_preference;
                        roleIssuer.back_office_pin_auth_YN = subitem.back_office_pin_auth_YN;
                        roleIssuer.user_role_id = item.user_role_id;
                        roleIssuer.can_create = item.can_create;
                        roleIssuer.can_read = item.can_read;
                        roleIssuer.can_update = item.can_update;

                        rtnUserRoles.Add(roleIssuer);
                    }
                }
                else
                {
                    rtnUserRoles.Add(item);
                }
            }


            return rtnUserRoles;
        }

        public List<StatusFlowRole> getStatusFlowRoles(List<int> roleIds)
        {
            return userGroupDAL.getStatusFlowRoles(roleIds);
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
        public List<user_list_result> GetUsersByBranch(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            return _dataSource.GetUserByBranch(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation);
        }

        public List<user_list_result> GetUsersByBranchAdmin(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            return _dataSource.GetUserByBranchAdmin(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation);
        }

        /// <summary>
        /// Gets a list of users who are linked to the specified branch.
        /// </summary>
        /// <param name="branch_id"></param>
        /// <returns></returns>
        public List<user_list_result> GetUnassignedUsers(int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            return _dataSource.GetUnassignedUsers(languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation);
        }

        /// <summary>
        /// Returns a single user based on the username provided.
        /// </summary>
        /// <param name="encryptedUserName"></param>
        /// <returns></returns>
        public decrypted_user GetUserByUsername(string encryptedUserName)
        {
            string decryptedUserName = EncryptionManager.DecryptString(encryptedUserName,
                                                                       StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                       StaticFields.EXTERNAL_SECURITY_KEY);

            return _dataSource.GetUserByUsername(decryptedUserName);
        }

        /// <summary>
        /// Returns a single user based on the User Id provided.
        /// </summary>
        /// <param name="encryptedUserId"></param>
        /// <returns></returns>
        public decrypted_user GetUserByUserId(string encryptedUserId)
        {
            string decryptedUserIdStr = EncryptionManager.DecryptString(encryptedUserId,
                                                                       StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                       StaticFields.EXTERNAL_SECURITY_KEY);

            int decryptedUserId;
            if (int.TryParse(decryptedUserIdStr, out decryptedUserId))
            {
                return _dataSource.GetUserByUserId(decryptedUserId);
            }
            else
            {
                throw new ArgumentException("User ID not an int value.");
            }
        }

        /// <summary>
        /// Returns a single user based on the User Id provided.
        /// </summary>
        /// <param name="encryptedUserId"></param>
        /// <returns></returns>
        public decrypted_user GetPendingUserByUserId(string encryptedUserId)
        {

            string decryptedUserIdStr = EncryptionManager.DecryptString(encryptedUserId,
                                                                      StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                      StaticFields.EXTERNAL_SECURITY_KEY);

            int decryptedUserId;
            if (int.TryParse(decryptedUserIdStr, out decryptedUserId))
            {
                return _dataSource.GetPendingUserByUserId(decryptedUserId);
            }
            else
            {
                throw new ArgumentException("User ID not an int value.");
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
            return _dataSource.GetUserGroupForUserAdmin(issuerId, userRole, userId, branchId);
        }


        /// <summary>
        /// Fetch a list of user groups which also indicates if the user is assigned to them.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="userRole">May be null to not filter by user role.</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserGroupAdminResult> GetUserGroupForPendingUserAdmin(int? issuerId, int? userRole, long? userId, int? branchId)
        {
            return _dataSource.GetUserGroupForPendingUserAdmin(issuerId, userRole, userId, branchId);
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
            return _dataSource.GetGroupRolesForUser(userId, languageId, auditUserId, auditWorkstation);
        }


        /// <summary>
        /// Fetch all the groups a user belongs to as well as the issuer and role of the group is apart of.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<GroupsRolesResult> GetGroupRolesForPendingUser(long userId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _dataSource.GetGroupRolesForPendingUser(userId, languageId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Gets a list of users by search criteria
        /// </summary>
        /// <param name="branch_id"></param>
        /// <returns></returns>
        public List<user_list_result> GetUserList(string username, string firstname, string lastname, string branchid, string userrole, int issuerid, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            return _dataSource.GetUserList(username, firstname, lastname, branchid, userrole, issuerid, pageIndex, rowsPerPage, auditUserId, auditWorkStation);
        }

        /// <summary>
        /// Gets a list of users by search criteria
        /// </summary>
        /// <param name="branch_id"></param>
        /// <returns></returns>
        public List<user_list_result> GetUsersPendingForApproval(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            return _dataSource.GetUsersPendingForApproval(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation);
        }

        /// <summary>
        /// Validate that the Authorisation Pin entered matches the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="authPin"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public long? GetUserAuthorisationPin(string username, string passcode, int branchId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            SystemResponseCode response = SystemResponseCode.GENERAL_FAILURE;
            responseMessage = String.Empty;

            var passcodeResult = _dataSource.GetUserAuthPin(username, branchId, auditUserId, auditWorkstation);

            if (passcodeResult != null)
            {
                var dbPasscode = EncryptionManager.DecryptString(passcodeResult.instant_authorisation_pin,
                                                                  StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                  StaticFields.EXTERNAL_SECURITY_KEY);

                if (!String.IsNullOrWhiteSpace(dbPasscode) && dbPasscode.Equals(passcode, StringComparison.Ordinal))
                {
                    responseMessage = _translator.TranslateResponseCode(SystemResponseCode.SUCCESS, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
                    return passcodeResult.user_id;
                }
                else
                    response = SystemResponseCode.USER_AUTHORISATION_PIN_INCORRECT;
            }
            else
                response = SystemResponseCode.USER_AUTHORISATION_PIN_INCORRECT;

            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            return null;
        }


        public bool UpdateUserLanguage(long userId, int languageId, string auditWorkstation)
        {
            return _dataSource.UpdateUserLanguage(userId, languageId, auditWorkstation);
        }
        public bool CreateUseradminSettings(useradminsettingslist item, long? userId, string auditWorkstation, out int user_admin_Id)
        {
            return _dataSource.CreateUseradminSettings(item, userId, auditWorkstation, out user_admin_Id);
        }
        public bool UpdateUseradminSettings(useradminsettingslist item, long? userId, string auditWorkstation)
        {
            return _dataSource.UpdateUseradminSettings(item, userId, auditWorkstation);
        }
        public useradminsettingslist GetUseradminSettings(long? auditUserId, string auditWorkstation)
        {
            return _dataSource.GetUseradminSettings(auditUserId, auditWorkstation);
        }

        public List<LangLookup> GetLangUserRoles(int languageId, int? enterprise, long auditUserId, string auditWorkstation)
        {
            return _dataSource.GetLangUserRoles(languageId, enterprise, auditUserId, auditWorkstation);
        }

        public bool SendChallenge(int authconfigurationId, String username, string auditWorkstation, out string token, out string responseMessage)
        {
            // Reading configurations for the authentication configuration.      
            var auth_configuration = authConfig.GetAuthConfiguration(authconfigurationId);

            // finding multifactor authentication configuration. 
            var connectonParams = auth_configuration.AuthConfigConnectionParams.First(i => i.auth_type_id == (int)AuthTypes.MultiFactor);

            IConfig config = ConfigFactory.GetConfig((int)connectonParams.connection_parameter_type_id);

            // reading connection parameters.

            var result = issuerMan.GetConnectionParameter((int)connectonParams.connection_parameter_id, -1, auditWorkstation);
            Dictionary<string, string> _additionaldata = new Dictionary<string, string>();
            foreach (var item in result.additionaldata)
            {
                _additionaldata.Add(item.key, item.value);
            }

            if (config is WebServiceConfig)
                config = new WebServiceConfig(Guid.Parse(connectonParams.interface_guid), (Protocol)result.ConnectionParams.protocol, result.ConnectionParams.address, result.ConnectionParams.port, result.ConnectionParams.path, null, (AuthType)result.ConnectionParams.auth_type, result.ConnectionParams.username, result.ConnectionParams.password, null);
            else
                throw new ArgumentException("Only Active Directory and Webservice configurations are supported. Please contact support.");

            ExternalAuthIntegrationController _integration = ExternalAuthIntegrationController.Instance;

            responseMessage = string.Empty; token = string.Empty;

            if (_integration.MultiFactorAuthentication(connectonParams.interface_guid).SendChallenge(config, username, _additionaldata, out responseMessage))
            {

                return true;
            }
            else
            {
                throw new LoginFailedException(responseMessage, SystemResponseCode.ACCOUNT_VALIDATION_FAIL);
            }

        }

        public bool VerifyChallenge(int authconfigurationId, string token, string username, string auditWorkstation, out string responseMessage)
        {
            // Reading configurations for the authentication configuration.      
            var auth_configuration = authConfig.GetAuthConfiguration(authconfigurationId);

            // finding multifactor authentication configuration. 
            var connectonParams = auth_configuration.AuthConfigConnectionParams.First(i => i.auth_type_id == (int)AuthTypes.MultiFactor);

            IConfig config = ConfigFactory.GetConfig((int)connectonParams.connection_parameter_type_id);

            // reading connection parameters.

            var result = issuerMan.GetConnectionParameter((int)connectonParams.connection_parameter_id, -1, auditWorkstation);
            Dictionary<string, string> _additionaldata = new Dictionary<string, string>();
            foreach (var item in result.additionaldata)
            {
                _additionaldata.Add(item.key, item.value);
            }

            if (config is WebServiceConfig)
                config = new WebServiceConfig(Guid.Parse(connectonParams.interface_guid), (Protocol)result.ConnectionParams.protocol, result.ConnectionParams.address, result.ConnectionParams.port, result.ConnectionParams.path, null, (AuthType)result.ConnectionParams.auth_type, result.ConnectionParams.username, result.ConnectionParams.password, null);
            else
                throw new ArgumentException("Only Active Directory and Webservice configurations are supported. Please contact support.");

            ExternalAuthIntegrationController _integration = ExternalAuthIntegrationController.Instance;

            responseMessage = string.Empty;
            if (_integration.MultiFactorAuthentication(connectonParams.interface_guid).VerifyChallenge(config, username, _additionaldata, token, out responseMessage))
            {
                return true;
            }
            else
            {
                throw new LoginFailedException(responseMessage, SystemResponseCode.ACCOUNT_VALIDATION_FAIL);
            }

        }
    }
}


