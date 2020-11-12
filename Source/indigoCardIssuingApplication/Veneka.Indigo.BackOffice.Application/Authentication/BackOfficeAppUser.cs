using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.API;

namespace Veneka.Indigo.BackOffice.Application.Authentication
{
    public class BackOfficeAppUser
    {
        public BackOfficeAppUser(string username, string email, string[] roles, string token)
        {
            Username = username;
            Email = email;
            Roles = roles;
            Token = token;
        }
        public string Username
        {
            get;
            set;
        }
        public string Token { get; private set; }
        public string Email
        {
            get;
            set;
        }

        public string[] Roles
        {
            get;
            set;
        }
    }
}
