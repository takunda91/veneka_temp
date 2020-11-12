using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.Application.Authentication.Clients;

namespace Veneka.Indigo.BackOffice.Application.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IndigoServicesAuthClient _authClient = new IndigoServicesAuthClient(new Uri(""));

        private class InternalUserData
        {
            public InternalUserData(string username, string email, string hashedPassword, string[] roles)
            {
                Username = username;
                Email = email;
                HashedPassword = hashedPassword;
                Roles = roles;
            }
            public string Username
            {
                get;
                private set;
            }

            public string Email
            {
                get;
                private set;
            }

            public string HashedPassword
            {
                get;
                private set;
            }

            public string[] Roles
            {
                get;
                private set;
            }
        }

        //private static readonly List<InternalUserData> _users = new List<InternalUserData>()
        //{
        //    new InternalUserData("Mark", "mark@company.com",
        //    "MB5PYIsbI2YzCUe34Q5ZU2VferIoI4Ttd+ydolWV0OE=", new string[] { "Administrators" }),
        //    new InternalUserData("John", "john@company.com",
        //    "hMaLizwzOQ5LeOnMuj+C6W75Zl5CXXYbwDSHWW9ZOXc=", new string[] { })
        //};

        public BackOfficeAppUser AuthenticateUser(string username, string clearTextPassword)
        {
            //InternalUserData userData = _users.FirstOrDefault(u => u.Username.Equals(username)
            //    && u.HashedPassword.Equals(CalculateHash(clearTextPassword, u.Username)));

            var authClientResp = _authClient.Login(username, clearTextPassword);

            if (authClientResp != null && authClientResp.ResponseCode.Equals("00"))
            {
                return new BackOfficeAppUser(username, "", new string[0],"");
            }

            throw new UnauthorizedAccessException("Access denied. Please provide valid credentials.");            
        }

        private string CalculateHash(string clearTextPassword, string salt)
        {
            // Convert the salted password to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);
            // Use the hash algorithm to calculate the hash
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            // Return the hash as a base64 encoded string to be compared to the stored password
            return Convert.ToBase64String(hash);
        }
    }
}
