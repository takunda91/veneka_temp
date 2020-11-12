using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.Web;
using indigoCardIssuingWeb.CardIssuanceService;

namespace indigoCardIssuingWeb.Old_App_Code.security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
AllowMultiple = true, Inherited = false)]
    public sealed class IndigoPrincipalPermission : CodeAccessSecurityAttribute
    {
        public IndigoPrincipalPermission(SecurityAction action)
            : base(action)
        {
        }
        public IndigoPrincipalPermission(SecurityAction action, params UserRole[] values)
            : base(action)
        {
            this.Roles = values;
        }
        
        public bool Authenticated { get; set; }
        public string Name { get; set; }
        public UserRole[] Roles { get; set; }
        public string Role { get; set; }
        public override IPermission CreatePermission()
        {

            IPermission result;
            if (Roles.Count() == 0)
            {
                result = new PrincipalPermission(null, null, true);
            }
            else
            {
                result = new PrincipalPermission(null, Roles[0].ToString());
                for (int i = 1; i < Roles.Count(); i++)
                {
                    result = result.Union(new PrincipalPermission(null, Roles[i].ToString()));
                }
            }

            return result;

        }

    }





}