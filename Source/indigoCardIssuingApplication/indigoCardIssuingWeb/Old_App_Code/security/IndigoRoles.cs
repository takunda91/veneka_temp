using indigoCardIssuingWeb.CardIssuanceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.Old_App_Code.security
{
    /// <summary>
    /// Wrapper class for Indigo's roles and status flows.
    /// </summary>
    public class IndigoRoles
    {
        private readonly Dictionary<UserRole, List<RolesIssuerResult>> _roles;
        private readonly List<StatusFlowRole> _statusFlows;

        #region Properties
        public Dictionary<UserRole, List<RolesIssuerResult>> Roles
        {
            get { return _roles; }
        }

        public string[] UserRoles
        {
            get { return _roles.Keys.Select(s => s.ToString()).ToArray(); }
        }

        public List<StatusFlowRole> StatusFlows
        {
            get { return _statusFlows; }
        }
        #endregion

        #region Constructors
        public IndigoRoles(Dictionary<UserRole, List<RolesIssuerResult>> roles, List<StatusFlowRole> statusFlows)
        {
            this._roles = roles;
            this._statusFlows = statusFlows;
        }

        public IndigoRoles(List<RolesIssuerResult> roles, List<StatusFlowRole> statusFlows)
        {
            this._roles = RolesListToDictionary(roles);
            this._statusFlows = statusFlows;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Convert roles list into dictionary form.
        /// </summary>
        /// <param name="rolesList"></param>
        /// <returns></returns>
        private Dictionary<UserRole, List<RolesIssuerResult>> RolesListToDictionary(List<RolesIssuerResult> rolesList)
        {
            Dictionary<UserRole, List<RolesIssuerResult>> rtnValue = new Dictionary<UserRole, List<RolesIssuerResult>>();

            foreach (RolesIssuerResult result in rolesList)
            {
                foreach (UserRole UserRole in Enum.GetValues(typeof(UserRole)))
                {
                    if (result.user_role_id == (int)UserRole)
                    {
                        if (rtnValue.ContainsKey(UserRole))
                        {
                            rtnValue[UserRole].Add(result);
                        }
                        else
                        {
                            List<RolesIssuerResult> values = new List<RolesIssuerResult>();
                            values.Add(result);
                            rtnValue.Add(UserRole, values);
                        }

                        break;
                    }
                }
            }

            return rtnValue;
        }
        #endregion
    }
}