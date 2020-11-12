using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.API;

namespace Veneka.Indigo.BackOffice.Application.Authentication
{
    public class BackOfficeAppIdentity : IIdentity
    {
        public BackOfficeAppIdentity(string name, string email, string[] roles,string token)
        {
            Name = name;
            Email = email;
            Roles = roles;
            Token = token;

        }

        public string Name { get; private set; }
        public string Email { get; private set; }
        public string[] Roles { get; private set; }
        public string Token { get; private set; }
        public string[] Keys { get; private set; }


        #region IIdentity Members
        public string AuthenticationType { get { return "Custom authentication"; } }

        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion
    }
}
