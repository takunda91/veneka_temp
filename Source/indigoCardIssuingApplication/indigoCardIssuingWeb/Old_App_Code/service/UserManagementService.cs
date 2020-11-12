using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.security;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.SearchParameters;
using System.Web;
using Common.Logging;
using indigoCardIssuingWeb.Old_App_Code.security;
using System.Web.Caching;
using System.Web.Security;
using System.Security.Principal;
using indigoCardIssuingWeb.Old_App_Code.service;
using System.Globalization;

namespace indigoCardIssuingWeb.service
{
    public sealed class UserManagementService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserManagementService));
        //private static readonly Service1SoapClient _issuanceService = new Service1SoapClient();
        private static volatile UserManagementService instance;
        private static object syncRoot = new Object();

        public static UserManagementService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new UserManagementService();
                    }
                }

                return instance;
            }
        }
        public List<language> GetLanguages()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetLanguages(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<LangLookup> LangLookupUserStatuses()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupUserStatuses(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<LangLookup> LangLookupUserRoles()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LangLookupUserRoles(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        /// <summary>
        /// Get all the roles assigned to the user. for each role there will be the associated issuers.
        /// </summary>
        /// <param name="encryptedUsername"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public IndigoRoles GetApplicationUserRole(string username)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetUserGroups(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return new IndigoRoles(response.Value.Roles.ToList(), response.Value.StatusFlows.ToList());

            return null;
        }



        /// <summary>
        /// Get all branches for user by issuer and user role.
        /// </summary>
        /// <param name="userRole"></param>
        /// <param name="input"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<BranchesResult> GetBranchesForUser(int? issuerId, UserRole? userRole, int? cardCentreBranchYN)
        {
            string messages = String.Empty;
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetBranchesForUser(issuerId, userRole, cardCentreBranchYN, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;

            //if (response.ResponseType != ResponseType.SUCCESSFUL)
            //{
            //    throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
            //}

            //return new List<BranchesResult>(response.Value);
        }
        /// <summary>
        /// Get all branches for user by issuer and user role.
        /// </summary>
        /// <param name="userRole"></param>
        /// <param name="input"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<BranchesResult> GetBranchesForUserroles(int? issuerId, List<int> userRolesList, bool? cardCentre)
        {


            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetBranchesForUserroles(issuerId, userRolesList.ToArray(), cardCentre, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        /// <summary>
        /// Get all branches for user by issuer and user role.
        /// </summary>
        /// <param name="userRole"></param>
        /// <param name="input"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<BranchesResult> GetBranchesForUserAdmin(int? issuerId, int? branchstatusid)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetBranchesForUserAdmin(issuerId, branchstatusid, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<branch> GetBranchesForIssuer(int issuerId, int? cardCentre)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.getBranchesForIssuer(issuerId, cardCentre, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        /// <summary>
        /// Retrieve a list of branches as well as the number of cards they have according to the load batch status.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="userRole"></param>
        /// <param name="loadCardStatus">May be null to fetch cards in any status</param>
        /// <returns></returns>
        public List<BranchLoadCardCountResult> GetBranchesLoadCardCount(int issuerId, UserRole userRole, LoadCardStatus? loadCardStatus)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetBranchesLoadCardCount(issuerId, userRole, loadCardStatus, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        /// <summary>
        /// Get card count for branch
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="loadBatchStatusId">May be null to fetch cards in any status</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal int? GetBranchLoadCardCount(int branchId, LoadCardStatus? loadCardStatus)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetBranchLoadCardCount(branchId, loadCardStatus, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        /// <summary>
        /// Get a list of decrypted users linked to the specified branch.
        /// </summary>
        /// <param name="branch_id"></param>
        /// <param name="branchStatusId"></param>
        /// <param name="userStatus"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<user_list_result> GetUsersByBranch(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int pageIndex, int rowsPerpage)
        {
            List<user> rtnValue = new List<user>();

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            ResponseOfListOfuser_list_result response = m_indigoApp.GetUsersByBranch(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, pageIndex, rowsPerpage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        public List<user_list_result> GetUserPendingForApproval(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int pageIndex, int rowsPerpage)
        {
            List<user> rtnValue = new List<user>();

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            ResponseOfListOfuser_list_result response = m_indigoApp.GetUsersPendingForApproval(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, pageIndex, rowsPerpage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public List<user_list_result> GetUsersByBranchAdmin(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int pageIndex, int rowsPerpage)
        {
            List<user> rtnValue = new List<user>();

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            ResponseOfListOfuser_list_result response = m_indigoApp.GetUsersByBranchAdmin(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, pageIndex, rowsPerpage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        /// <summary>
        /// Get a list of decrypted users linked to the specified branch.
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<user_list_result> GetUsersByBranch(UserSearchParameters searchParams, int pageIndex, int rowsPerpage)
        {
            return this.GetUsersByBranch(searchParams.IssuerID, searchParams.BranchId, searchParams.BranchStatus,
                                         searchParams.UserStatus, searchParams.UserRole, searchParams.UserName,
                                         searchParams.FirstName, searchParams.LastName, pageIndex, rowsPerpage);
        }
        public List<user_list_result> GetUserPendingForApproval(UserSearchParameters searchParams, int pageIndex, int rowsPerpage)
        {
            return this.GetUserPendingForApproval(searchParams.IssuerID, searchParams.BranchId, searchParams.BranchStatus,
                                         searchParams.UserStatus, searchParams.UserRole, searchParams.UserName,
                                         searchParams.FirstName, searchParams.LastName, pageIndex, rowsPerpage);
        }
        
        public List<user_list_result> GetUsersByBranchAdmin(UserSearchParameters searchParams, int pageIndex, int rowsPerpage)
        {
            return this.GetUsersByBranchAdmin(searchParams.IssuerID, searchParams.BranchId, searchParams.BranchStatus,
                                         searchParams.UserStatus, searchParams.UserRole, searchParams.UserName,
                                         searchParams.FirstName, searchParams.LastName, pageIndex, rowsPerpage);
        }

        /// <summary>
        /// Get a list of decrypted users linked to the specified branch.
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        //public List<user_list_result> GetUserlist(UserSearchParameters searchParams, int pageIndex, int rowsPerpage)
        //{
        //    List<user_list_result> user_list_result = new List<user_list_result>();

        //    string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
        //                                                               SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
        //                                                               SecurityParameters.EXTERNAL_SECURITY_KEY);

        //    ResponseOfListOfuser_list_result response = _issuanceService.GetUserList(searchParams.UserName, searchParams.FirstName, searchParams.LastName, searchParams.BranchId.ToString(), searchParams.UserRole, searchParams.IssuerID, pageIndex, rowsPerpage, encryptedSessionKey);

        //    if (response.ResponseType != ResponseType.SUCCESSFUL)
        //    {
        //        throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
        //    }

        //    //TODO: Should possibly encrypt certian info coming over the web response.
        //    return new List<user_list_result>(response.Value);
        //}

        /// <summary>
        /// Get a list of users who do not belong to any users groups.
        /// </summary>
        /// <returns></returns>
        public List<user_list_result> GetUnassignedUsers(int pageIndex)
        {
            List<user> rtnValue = new List<user>();

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            ResponseOfListOfuser_list_result response = m_indigoApp.GetUnassignedUsers(pageIndex, StaticDataContainer.ROWS_PER_PAGE, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        /// <summary>
        /// Get users groups for specified Issuer.
        /// </summary>
        /// <param name="issuerID"></param>
        /// <returns></returns>
        public List<UserGroupResult> GetUserGroups(int issuerID, UserRole? userRole, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetUserGroupsByIssuer(issuerID, userRole, pageIndex, StaticDataContainer.ROWS_PER_PAGE, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        /// <summary>
        /// Get users groups for specified Issuer.
        /// </summary>
        /// <param name="issuerID"></param>
        /// <returns></returns>
        public List<UserGroupResult> GetUserGroupsByIssuerAndRole(int issuerID, UserRole userRole, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetUserGroupsByIssuerAndRole(issuerID, userRole, pageIndex, StaticDataContainer.ROWS_PER_PAGE, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public decrypted_user GetProfile(string sessionKey)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(sessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            ResponseOfdecrypted_user response = m_indigoApp.GetProfile(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        /// <summary>
        /// Returns a single user based on the username provided.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public decrypted_user GetUserByUsername(string username)
        {
            string encryptedUsername = EncryptionManager.EncryptString(username,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            ResponseOfdecrypted_user response = m_indigoApp.GetUserByUsername(encryptedUsername, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        public decrypted_user GetUserByUsername(string username, string sessionKey)
        {
            string encryptedUsername = EncryptionManager.EncryptString(username,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedSessionKey = EncryptionManager.EncryptString(sessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            ResponseOfdecrypted_user response = m_indigoApp.GetUserByUsername(encryptedUsername, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        /// <summary>
        /// Returns a single user based on the User Id provided.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public decrypted_user GetUserByUserId(long UserId)
        {
            string encryptedUserId = EncryptionManager.EncryptString(UserId.ToString(),
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            ResponseOfdecrypted_user response = m_indigoApp.GetUserByUserId(encryptedUserId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        /// <summary>
        /// Returns a single user from user pendings which not been approved.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public decrypted_user GetPendingUserByUserId(long UserId)
        {
            string encryptedUserId = EncryptionManager.EncryptString(UserId.ToString(),
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            ResponseOfdecrypted_user response = m_indigoApp.GetPendingUserByUserId(encryptedUserId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        /// <summary>
        /// Find the user in Active Directory.
        /// </summary>
        /// <param name="usernameLookup"></param>
        /// <param name="username"></param>
        /// <param name="userpassword"></param>
        /// <param name="ldapIssuerId"></param>
        /// <returns></returns>

        internal decrypted_user LdapLookup(string usernameLookup, string username, string userpassword, int authConfigId, out string message)
        {
            message = "";

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.LdapLookup(usernameLookup, username, userpassword, authConfigId, encryptedSessionKey);


            base.CheckSession(response, log);

            if (response.ResponseType == ResponseType.FAILED)
            {
                message = log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage;
            }
            else if (response.ResponseType != ResponseType.SUCCESSFUL)
            {
                throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
            }

            return response.Value;
        }

        /// <summary>
        /// Persist user to the database
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        internal bool CreateUser(user createUser, long? user_id, List<int> userGroupList, out string messages, out long? newUserId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            if (user_id == null)
            {
                //Encrypting the password
                string encryptedPassword = EncryptionManager.EncryptString(utility.UtilityClass.GetString(createUser.password),
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);
                createUser.password = utility.UtilityClass.GetBytes(encryptedPassword);
            }

            var response = m_indigoApp.CreateUser(createUser, userGroupList.ToArray(), encryptedSessionKey);
            newUserId = response.Value;

            return base.CheckResponseException(response, log, out messages);
        }



        internal bool InsertAuthenticationConfiguration(AuthConfigResult authConfig, out int? authConfigurationId, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.InsertAuthenticationConfiguration(authConfig, encryptedSessionKey);
            authConfigurationId = response.Value;

            return base.CheckResponseException(response, log, out messages);
        }

        internal bool updateAuthenticationConfiguration(AuthConfigResult authConfig, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.UpdateAuthenticationConfiguration(authConfig, encryptedSessionKey);

            return base.CheckResponseException(response, log, out messages);
        }
        internal bool DeleteAuthConfiguration(int authConfig, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.DeleteAuthConfiguration(authConfig, encryptedSessionKey);

            return base.CheckResponseException(response, log, out messages);
        }
        internal AuthConfigResult GetAuthConfiguration(int authConfigId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetAuthConfiguration(authConfigId, 0, 2000, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal List<auth_configuration_result> GetAuthConfigurationList(int pageIndex, int rowsPerPage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetAuthConfigurationList(null, pageIndex, rowsPerPage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        /// <summary>
        /// Persists changes to a user object to the database.
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        internal bool UpdateUser(user updateUser, List<int> userGroupList, out string messages, out long? newPendingUserId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateUser(updateUser, userGroupList.ToArray(), encryptedSessionKey);
            newPendingUserId = response.Value;

            return base.CheckResponseException(response, log, out messages);
        }

        /// <summary>
        /// Persist new password to the DB.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        internal bool ResetUserPassword(long userId, string Password, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedPassword = EncryptionManager.EncryptString(Password,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            BaseResponse response = m_indigoApp.ResetUserPassword(userId, encryptedPassword, encryptedSessionKey);
            return base.CheckResponseException(response, log, out messages);
        }
        internal bool VerifyChallenge(int authConfigId, string username, string text, out string messages)
        {
            string encryptedSessionKey = string.Empty;

            encryptedSessionKey = EncryptionManager.EncryptString("IndigoLoginAttemptVerification" + DateTime.Now.ToString(),
                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);





            var response = m_indigoApp.VerifyChallenge(authConfigId, username, text, encryptedSessionKey);

            if (base.CheckResponse(response, log, out messages))
                return response.Value;

            return response.Value;
        }
        internal bool SendChallenge(int authConfigId, string name, out string responseMessage)

        {
            string encryptedSessionKey = string.Empty;

            encryptedSessionKey = EncryptionManager.EncryptString("IndigoLoginAttemptVerification" + DateTime.Now.ToString(),
                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);



            var response = m_indigoApp.SendChallenge(authConfigId, name, encryptedSessionKey);

            if (base.CheckResponse(response, log, out responseMessage))
                return response.Value;

            return response.Value;
        }

        /// <summary>
        /// Instant Pin Authorisation
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="authPin"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        internal bool GetUserAuthorisationPin(long userid, string authPin)
        {
            // string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
            //                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
            //                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);

            // string encryptedPin = EncryptionManager.EncryptString(authPin,
            //                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
            //                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);

            // BaseResponse response = _issuanceService.GetAuthorisationPin(userid, authPin, encryptedSessionKey);
            //// messages = response.ResponseMessage;

            // if (response.ResponseType == ResponseType.SUCCESSFUL)
            // {
            //     return true;
            // }
            // else if (response.ResponseType == ResponseType.UNSUCCESSFUL)
            // {
            //     return false;
            // }
            // if (response.ResponseType != ResponseType.SUCCESSFUL)
            // {
            //     throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
            // }

            return false;
        }

        /// <summary>
        /// Add and Update the Branch Custodian Instant Authorisation Pin
        /// </summary>
        /// <param name="updateUser"></param>
        /// <param name="newPin"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        internal bool UpdateUserAuthorisationPin(long? userId, string newPin, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedNewPin = EncryptionManager.EncryptString(newPin,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            BaseResponse response = m_indigoApp.UpdateAuthorisationPin(userId, encryptedNewPin, encryptedSessionKey);
            return base.CheckResponseException(response, log, out messages);
        }

        /// <summary>
        /// Change the users password on the DB.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        internal bool UpdateUserPassword(long? userId, string oldPassword, string newPassword, out string messages)
        {
            string encryptedSessionKey = string.Empty;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            }
            else
            {
                encryptedSessionKey = EncryptionManager.EncryptString("IndigoLoginAttemptResetPassword" + DateTime.Now.ToString(),
                                                               SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                             SecurityParameters.EXTERNAL_SECURITY_KEY);
            }
            string encryptedOldPassword = EncryptionManager.EncryptString(oldPassword,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedNewPassword = EncryptionManager.EncryptString(newPassword,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);
            string encryptedAddress = SessionWrapper.ClientAddress;
            encryptedAddress = EncryptionManager.EncryptString(encryptedAddress,
                                                               SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                               SecurityParameters.EXTERNAL_SECURITY_KEY);

            BaseResponse response = m_indigoApp.UpdatePassword(userId, encryptedOldPassword, encryptedNewPassword, encryptedAddress, encryptedSessionKey);
            return base.CheckResponseException(response, log, out messages);
        }

        /// <summary>
        /// Persist new user group to DB
        /// </summary>
        /// <param name="userGroup"></param>
        /// <param name="branchIdList"></param>
        /// <returns></returns>
        internal bool CreateUserGroup(user_group userGroup, List<int> branchIdList, out int userGroupId, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateUserGroup(userGroup, branchIdList.ToArray(), encryptedSessionKey);
            userGroupId = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        /// <summary>
        /// Persist changes to user group to DB
        /// </summary>
        /// <param name="userGroup"></param>
        /// <param name="branchIdList"></param>
        /// <returns></returns>
        internal bool UpdateUserGroup(user_group userGroup, List<int> branchIdList, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateUserGroup(userGroup, branchIdList.ToArray(), encryptedSessionKey);
            return base.CheckResponse(response, log, out messages);
        }

        /// <summary>
        /// Delete a user group.
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public bool DeleteUserGroup(int userGroupId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DeleteUserGroup(userGroupId, encryptedSessionKey);
            return base.CheckResponseException(response, log, out responseMessage);
        }

        /// <summary>
        /// Returns a list of branch id's associated with the user group.
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        internal List<BranchIdResult> GetBranchesForUserGroup(int userGroupId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetBranchesForUserGroup(userGroupId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }



        /// <summary>
        /// Fetch a list of user groups which also indicates if the user is assigned to them.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="userRole">May be null to not filter by user role.</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal List<UserGroupAdminResult> GetUserGroupForUserAdmin(int issuerId, int? userRole, long userId, int? branchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetUserGroupForUserAdmin(issuerId, userRole, userId, branchId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        /// <summary>
        /// Get all users groups the user currently belongs to.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal Dictionary<int, List<UserGroupAdminResult>> GetDictionaryUserGroupForUserAdmin(long? userId, int userRole, int? issuerId, int? branchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetUserGroupForUserAdmin(issuerId, userRole, userId, branchId, encryptedSessionKey);

            base.CheckResponse(response, log);

            Dictionary<int, List<UserGroupAdminResult>> rtnDict = new Dictionary<int, List<UserGroupAdminResult>>();

            foreach (var item in response.Value)
            {
                List<UserGroupAdminResult> values;

                if (rtnDict.TryGetValue(item.issuer_id, out values))
                {
                    rtnDict.Remove(item.issuer_id);
                }
                else
                {
                    values = new List<UserGroupAdminResult>();
                }

                values.Add(item);

                rtnDict.Add(item.issuer_id, values);
            }

            return rtnDict;
        }


        internal Dictionary<int, List<UserGroupAdminResult>> GetUserGroupForPendingUserAdmin(long? userId, int userRole, int? issuerId, int? branchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetUserGroupForPendingUserAdmin(issuerId, userRole, userId, branchId, encryptedSessionKey);

            base.CheckResponse(response, log);

            Dictionary<int, List<UserGroupAdminResult>> rtnDict = new Dictionary<int, List<UserGroupAdminResult>>();

            foreach (var item in response.Value)
            {
                List<UserGroupAdminResult> values;

                if (rtnDict.TryGetValue(item.issuer_id, out values))
                {
                    rtnDict.Remove(item.issuer_id);
                }
                else
                {
                    values = new List<UserGroupAdminResult>();
                }

                values.Add(item);

                rtnDict.Add(item.issuer_id, values);
            }

            return rtnDict;
        }

        /// <summary>
        /// Fetch all the groups a user belongs to as well as the issuer and role of the group is apart of.        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal List<GroupsRolesResult> GetGroupRolesForUser(long userId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetGroupRolesForUser(userId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        internal List<GroupsRolesResult> GetGroupRolesForPendingUser(long userId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetGroupRolesForPendingUser(userId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        /// <summary>
        /// Reset the users logged in status to logged out.
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="userName"></param>
        /// <param name="lockedHost"></param>
        internal void ResetUserLoginStatus(long userId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ResetUserLoginStatus(userId, encryptedSessionKey);
            base.CheckSession(response, log);
        }

        /// <summary>
        /// Fetch user group info
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public user_group GetUserGroup(int userGroupId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetUserGroup(userGroupId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        public UserRole[] GetRolesForUserGroup(string userGroup, int issuerID)
        {
            return new UserRole[] { };

        }

        private bool ValidateCertificate(object requestObject, X509Certificate remoteCertificate,
          X509Chain requestChain, SslPolicyErrors sslErrorPolicie)
        {
            return true;
        }

        public bool LogIn(string username, string password)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateCertificate);

            string encryptedPassword = EncryptionManager.EncryptString(password,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);
            string encryptedUsername = EncryptionManager.EncryptString(username,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedSessionKey = EncryptionManager.EncryptString("IndigoLoginAttempt" + DateTime.Now.ToString(),
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);


            string encryptedAddress = SessionWrapper.ClientAddress;
            encryptedAddress = EncryptionManager.EncryptString(encryptedAddress,
                                                               SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                               SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LogIn(encryptedUsername, encryptedPassword, encryptedAddress, encryptedSessionKey);

            base.CheckSession(response, log);

            if (response.ResponseType == ResponseType.UNSUCCESSFUL)
            {
                throw new Exception(response.ResponseMessage);
            }
            if (response.ResponseType != ResponseType.SUCCESSFUL)
            {
                throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
            }
            else if (response.ResponseType == ResponseType.SUCCESSFUL)
            {
                //Good response
                var userName = EncryptionManager.DecryptString(response.Value.encryptedUsername,
                                                                SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                SecurityParameters.EXTERNAL_SECURITY_KEY);

                //HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey
                var webServiceSessionKeys = EncryptionManager.DecryptString(response.Value.encryptedSessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);

                var userId = long.Parse(EncryptionManager.DecryptString(response.Value.encryptedUserId,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY));




                //Save into current context user
                var identity = new IndigoIdentity(HttpContext.Current.User.Identity, userId, String.Empty, String.Empty,
                                                    String.Empty, response.Value.ldapUser, response.Value.ChangePassword, response.Value.multifactorUser, (int)response.Value.authConfigurationId, webServiceSessionKeys);
                var principal = new IndigoPrincipal(identity);
                HttpContext.Current.User = principal;

                //Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name) = RolesListToDictionary(response.Value.Roles.ToList());
                //SessionWrapper.StatusFlow = response.Value.StatusFlow.ToList();
                SessionWrapper.UserLanguage = response.Value.LanguageId;
                //SessionWrapper.LdapUser = response.Value.ldapUser;
                HttpContext.Current.Session["CultureName"] = UtilityClass.GetLang(2);



                return true;
            }

            return false;
        }

        public IndigoIdentity LogIn2(string username, string password)
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateCertificate);

            string encryptedPassword = EncryptionManager.EncryptString(password,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);
            string encryptedUsername = EncryptionManager.EncryptString(username,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            string encryptedSessionKey = EncryptionManager.EncryptString("IndigoLoginAttempt" + DateTime.Now.ToString(),
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);


            string encryptedAddress = SessionWrapper.ClientAddress;
            encryptedAddress = EncryptionManager.EncryptString(encryptedAddress,
                                                               SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                               SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.LogIn(encryptedUsername, encryptedPassword, encryptedAddress, encryptedSessionKey);

            base.CheckSession(response, log);

            if (response.ResponseType == ResponseType.UNSUCCESSFUL)
            {
                throw new Exception(response.ResponseMessage);
            }
            if (response.ResponseType != ResponseType.SUCCESSFUL)
            {
                throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
            }
            else if (response.ResponseType == ResponseType.SUCCESSFUL)
            {
                //Good response
                var userName = EncryptionManager.DecryptString(response.Value.encryptedUsername,
                                                                SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                SecurityParameters.EXTERNAL_SECURITY_KEY);

                //HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey
                var webServiceSessionKeys = EncryptionManager.DecryptString(response.Value.encryptedSessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);

                var userId = long.Parse(EncryptionManager.DecryptString(response.Value.encryptedUserId,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY));




                //Save into current context user
                return new IndigoIdentity(HttpContext.Current.User.Identity, userId, String.Empty, String.Empty,
                                                    String.Empty, response.Value.ldapUser, response.Value.ChangePassword, response.Value.multifactorUser, (int)response.Value.authConfigurationId, webServiceSessionKeys);

            }

            return null;
        }

        /// <summary>
        /// Perform user logout
        /// </summary>
        /// <returns></returns>
        //public static bool LogOut()
        public bool LogOut()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey))
            {
                string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                             SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                             SecurityParameters.EXTERNAL_SECURITY_KEY);

                var response = m_indigoApp.LogOut(encryptedSessionKey);

                if (response.ResponseType == ResponseType.SUCCESSFUL)
                {
                    SessionWrapper.ClearSession();
                    return true;
                }
            }

            return false;
        }

        internal bool ApproveUser(long pendingUserId, out string messages)
        {

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.ApproveUser(pendingUserId,  encryptedSessionKey);
            return base.CheckResponse(response, log, out messages);
        }
        internal bool RejectUserRequest(long pendingUserId, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RejectUserRequest(pendingUserId, encryptedSessionKey);
            return base.CheckResponse(response, log, out messages);
        }
        //public ApplicationUser GetUser(string userName)
        //{
        //    List<ApplicationUser> appUsers = GetUsers(null, 0, userName);
        //    if (appUsers != null && appUsers.Count > 0)
        //        return appUsers[0];
        //    return null;
        //}

        //public List<ApplicationUser> GetUsersForIssuer(int issuerId)
        //{
        //    return GetUsers(null, issuerId, null);
        //}        

        //internal List<ApplicationUser> SeachUser(UserSearch searchObject)
        //{
        //    var users = new List<ApplicationUser>();            
        //    return users;
        //}  

        //internal List<ApplicationUser> GetUsersForBranch(string branchCode, int issuerId)
        //{
        //    return GetUsers(branchCode, issuerId, null);
        //}

        //private List<ApplicationUser> GetUsers(string branchCode, int issuerId, string username)
        //{
        //    var users = new List<ApplicationUser>();
        //    return users;
        //}

        internal bool UpdateUserLanguage(int selectedLang)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateUserLanguange(null, selectedLang, encryptedSessionKey);

            base.CheckSession(response, log);

            if (response.ResponseType == ResponseType.SUCCESSFUL)
            {
                SessionWrapper.UserLanguage = selectedLang;
                return true;
            }

            return false;
        }

       

        public List<LangLookup> GetLangUserRoles(int? enterprise)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetLangUserRoles(enterprise, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }

        public useradminsettingslist GetUseradminSettings()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetUseradminSettings(encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }
        internal bool CreateUseradminSettings(useradminsettingslist item, out int? usersettingsID, out string result)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateUseradminSettings(item, encryptedSessionKey);
            usersettingsID = response.Value;
            result = response.ResponseMessage;
            if (base.CheckResponse(response, log))
                return true;

            return false;
        }

        internal bool UpdateUseradminSettings(useradminsettingslist item, out string result)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateUseradminSettings(item, encryptedSessionKey);

            base.CheckSession(response, log);
            result = response.ResponseMessage;
            if (response.ResponseType == ResponseType.SUCCESSFUL)
            {

                return true;
            }

            return false;
        }
    }
}