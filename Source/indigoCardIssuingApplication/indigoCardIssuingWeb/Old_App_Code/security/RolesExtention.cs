using indigoCardIssuingWeb.Old_App_Code.security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Web.Security
{
    public static class RolesExtention
    {
        public static IndigoRoleProvider ToIndigoRoleProvider(this RoleProvider rolesProvider)
        {
            return (IndigoRoleProvider)rolesProvider;
        }
    }
}