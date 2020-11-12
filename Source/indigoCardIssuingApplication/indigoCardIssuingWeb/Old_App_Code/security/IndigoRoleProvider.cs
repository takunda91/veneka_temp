using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.Old_App_Code.security
{
    public class IndigoRoleProvider : RoleProvider
    {
        public const string USER_ROLE_CACHEKEY = "USER_ROLE_CACHEKEY_{0}";
        private const string _applicationName = "IndigoWebApp";
        private readonly UserManagementService _userMan = new UserManagementService();
        private int _cacheTimeoutInMinutes = 30;

        #region Properties
        public override string ApplicationName
        {
            get
            {
                return _applicationName;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Private Methods
        private IndigoRoles LoadRoles(string username)
        {
            //Return if present in Cache
            var cacheKey = GetRoleKey(username);

            var userRoles = _userMan.GetApplicationUserRole(username);

            if (HttpRuntime.Cache[cacheKey] != null)
                HttpRuntime.Cache.Remove(cacheKey);

            if (userRoles != null)
            {
                //Store in cache
                HttpRuntime.Cache.Insert(cacheKey, userRoles, null, DateTime.Now.AddMinutes(_cacheTimeoutInMinutes), Cache.NoSlidingExpiration);
            }

            return userRoles;
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Initialize values from web.config.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            // Set Properties
            int val;
            if (!string.IsNullOrEmpty(config["cacheTimeoutInMinutes"]) && Int32.TryParse(config["cacheTimeoutInMinutes"], out val))
                _cacheTimeoutInMinutes = val;

            // Call base method
            base.Initialize(name, config);
        }

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role for the configured applicationName.
        /// </summary>
        /// <returns>
        /// true if the specified user is in the specified role for the configured applicationName; otherwise, false.
        /// </returns>
        /// <param name="username">The user name to search for.</param><param name="roleName">The role to search in.</param>
        public override bool IsUserInRole(string username, string roleName)
        {
            //Return if the user is not authenticated
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return false;

            //Return if present in Cache
            var cacheKey = GetRoleKey(username);

            if (HttpRuntime.Cache[cacheKey] != null)
                return ((IndigoRoles)HttpRuntime.Cache[cacheKey]).UserRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);

            //No roles in cache - try load them
            var roles = LoadRoles(username);

            if (roles != null)
                return roles.UserRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);

            return false;
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <returns>
        /// A string array containing the names of all the roles that the specified user is in for the configured applicationName.
        /// </returns>
        /// <param name="username">The user to return a list of roles for.</param>
        public override string[] GetRolesForUser(string username)
        {
            //Return if the user is not authenticated
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return null;

            //Return if present in Cache
            var cacheKey = GetRoleKey(username);

            if (HttpRuntime.Cache[cacheKey] != null)
                return ((IndigoRoles)HttpRuntime.Cache[cacheKey]).UserRoles;

            //No roles in cache - try load them
            var roles = LoadRoles(username);

            if (roles != null)
                return roles.UserRoles;

            return null; 
        }

        public Dictionary<UserRole, List<RolesIssuerResult>> GetRolesDictForUser(string username)
        {
            //Return if the user is not authenticated
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return null;

            //Return if present in Cache
            var cacheKey = GetRoleKey(username);

            if (HttpRuntime.Cache[cacheKey] != null)
                return ((IndigoRoles)HttpRuntime.Cache[cacheKey]).Roles;

            //No roles in cache - try load them
            var roles = LoadRoles(username);

            if (roles != null)
                return roles.Roles;                    

            return null;
        }

        /// <summary>
        /// Validates page access and returns a list of issuer for the user.
        /// </summary>
        /// <param name="userRolesForPage"></param>
        /// <param name="issuerList"></param>
        /// <returns></returns>
        public bool ValidateUserPageRole(string username, UserRole[] userRolesForPage,
                                                out Dictionary<int, ListItem> issuerList,
                                                out Dictionary<int, RolesIssuerResult> roleIssuerList)
        {
            bool hasAccess = false;
            issuerList = new Dictionary<int, ListItem>();
            roleIssuerList = new Dictionary<int, RolesIssuerResult>();
            foreach (UserRole userRole in userRolesForPage)
            {
                List<RolesIssuerResult> issuers;

                if (this.GetRolesDictForUser(username).TryGetValue(userRole, out issuers))
                {
                    hasAccess = true;
                    foreach (var issuer in issuers)
                    {
                        if (!issuerList.ContainsKey(issuer.issuer_id))
                        {                            
                            roleIssuerList.Add(issuer.issuer_id, issuer);
                            issuerList.Add(issuer.issuer_id, utility.UtilityClass.FormatListItem<int>(issuer.issuer_name, issuer.issuer_code, issuer.issuer_id));
                        }
                    }
                }
            }

            return hasAccess;
        }

        /// <summary>
        /// Returns the list of issuers that the user has access for the supplied roles.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userRolesForPage"></param>
        /// <returns></returns>
        public Dictionary<int, ListItem> GetIssuersForRole(string username, UserRole[] userRolesForPage)
        {
            Dictionary<int, RolesIssuerResult> roleIssuerList;
            Dictionary<int, ListItem> issuerList;
            this.ValidateUserPageRole(username, userRolesForPage, out issuerList, out roleIssuerList);

            return issuerList;
        }

        public List<StatusFlowRole> GetStatusFlowRole(string username)
        {
            //Return if the user is not authenticated
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return null;

            //Return if present in Cache
            var cacheKey = GetRoleKey(username);

            if (HttpRuntime.Cache[cacheKey] != null)
                return ((IndigoRoles)HttpRuntime.Cache[cacheKey]).StatusFlows;

            //No roles in cache - try load them
            var roles = LoadRoles(username);

            if (roles != null)
                return roles.StatusFlows;

            return null;
        }

        public void ClearRoles(string username)
        {
            var cacheKey = GetRoleKey(username);

            if (HttpRuntime.Cache[cacheKey] != null)
                HttpRuntime.Cache.Remove(cacheKey);
        }

        private string GetRoleKey(string username)
        {
            return String.Format(USER_ROLE_CACHEKEY, username.Trim().ToUpper());
        }

        private string[] RolesFromCache(string username)
        {
            var cacheKey = GetRoleKey(username);

            if (HttpRuntime.Cache[cacheKey] != null)
                return ((IndigoRoles)HttpRuntime.Cache[cacheKey]).UserRoles;

            return null;
        }
        #endregion

        #region Not Implemented
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }        

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}