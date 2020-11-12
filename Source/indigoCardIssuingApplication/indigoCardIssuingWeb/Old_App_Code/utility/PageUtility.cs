using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using System.Web.Security;

namespace indigoCardIssuingWeb.utility
{
    public class PageUtility
    {
        /// <summary>
        /// Validates page access and returns a list of issuer for the user.
        /// </summary>
        /// <param name="userRolesForPage"></param>
        /// <param name="issuerList"></param>
        /// <returns></returns>
        public static bool ValidateUserPageRole(string username, UserRole[] userRolesForPage, 
                                                out Dictionary<int, ListItem> issuerList,
                                                out Dictionary<int, RolesIssuerResult> roleIssuerList)
        {
            bool hasAccess = false;
            issuerList = new Dictionary<int, ListItem>();
            roleIssuerList = new Dictionary<int, RolesIssuerResult>();
            foreach (UserRole userRole in userRolesForPage)
            {
                List<RolesIssuerResult> issuers;
                
                if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(username).TryGetValue(userRole, out issuers))
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
        /// Validates page access.
        /// </summary>
        /// <param name="userRolesForPage"></param>
        /// <returns></returns>
        public static bool ValidateUserPageRole(string username, UserRole[] userRolesForPage, out Dictionary<int, RolesIssuerResult> roleIssuerList)
        {
            Dictionary<int, ListItem> issuerList;            
            return ValidateUserPageRole(username, userRolesForPage, out issuerList, out roleIssuerList);
        }

        public static bool ValidateUserPageRole(string username, UserRole userRoleForPage, int issuerId, out bool canRead, out bool canUpdate, out bool canCreate)
        {
            canRead = false;
            canUpdate = false;
            canCreate = false;

            List<RolesIssuerResult> issuers;

            bool hasAccess = Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(username).TryGetValue(userRoleForPage, out issuers);
            if (hasAccess)
            {
                foreach (var item in issuers)
                {
                    if (item.issuer_id == issuerId)
                    {
                        if (item.can_read)
                        {
                            canRead = true;
                        }

                        if (item.can_update)
                        {
                            canUpdate = true;
                        }

                        if (item.can_create)
                        {
                            canCreate = true;
                        }
                    }
                }
            }


            return hasAccess;
        }

        public static bool ValidateUserPageRole(string username, UserRole userRoleForPage, out bool canRead, out bool canUpdate, out bool canCreate)
        {
            canRead = false;
            canUpdate = false;
            canCreate = false;

            List<RolesIssuerResult> issuers;
            bool hasAccess = Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(username).TryGetValue(userRoleForPage, out issuers);
            if (hasAccess)
            {
                foreach (var item in issuers)
                {
                    if (item.can_read)
                    {
                        canRead = true;
                    }

                    if (item.can_update)
                    {
                        canUpdate = true;
                    }

                    if (item.can_create)
                    {
                        canCreate = true;
                    }
                }
            }


            return hasAccess;
        }

        /// <summary>
        /// Validates page access.
        /// </summary>
        /// <param name="userRolesForPage"></param>
        /// <returns></returns>
        public static bool ValidateUserPageRole(string username, UserRole[] userRolesForPage, out Dictionary<int, ListItem> issuerList)
        {
            Dictionary<int, RolesIssuerResult> roleIssuerList;
            return ValidateUserPageRole(username, userRolesForPage, out issuerList, out roleIssuerList);
        }

        /// <summary>
        /// Validates page access.
        /// </summary>
        /// <param name="userRolesForPage"></param>
        /// <returns></returns>
        public static bool ValidateUserPageRole(string username, UserRole[] userRolesForPage)
        {
            Dictionary<int, ListItem> issuerList;
            Dictionary<int, RolesIssuerResult> roleIssuerList;
            return ValidateUserPageRole(username, userRolesForPage, out issuerList, out roleIssuerList);
        }
    }
}