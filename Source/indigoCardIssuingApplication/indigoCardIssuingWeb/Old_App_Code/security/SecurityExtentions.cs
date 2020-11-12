using indigoCardIssuingWeb.Old_App_Code.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace System.Security.Principal
{
    public static class SecurityExtentions
    {
        public static IndigoPrincipal ToIndigoPrincipal(this System.Security.Principal.IPrincipal principal)
        {
            return (IndigoPrincipal)principal;
        }
    }
}