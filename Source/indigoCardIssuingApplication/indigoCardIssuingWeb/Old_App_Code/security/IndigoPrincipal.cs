using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace indigoCardIssuingWeb.Old_App_Code.security
{
    [Serializable]
    public class IndigoPrincipal : IPrincipal
    {
        public IndigoPrincipal(IndigoIdentity identity)
        {
            Identity = identity;
        }

        public bool IsInRole(string role)
        {
            return ((IndigoRoleProvider)Roles.Provider).IsUserInRole(Identity.Name, role);
        }

        public IIdentity Identity { get; private set; }

        public IndigoIdentity IndigoIdentity { 
            get { return (IndigoIdentity)Identity; } 
            set { Identity = value; } 
        }
    }
}