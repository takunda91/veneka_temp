using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using Veneka.Indigo.BackOffice.API;

namespace Veneka.Indigo.BackOffice.Application.Authentication
{
    public class BackOfficeAppPrincipal : IPrincipal
    {
        private BackOfficeAppIdentity _identity;

        public BackOfficeAppIdentity Identity
        {
            get { return _identity ?? new AnonymousIdentity(); }
            set { _identity = value; }
        }

        #region IPrincipal Members
        IIdentity IPrincipal.Identity
        {
            get { return this.Identity; }
        }

       
        public bool IsInRole(string role)
        {
            return _identity.Roles.Contains(role);
        }
        #endregion
    }
}
