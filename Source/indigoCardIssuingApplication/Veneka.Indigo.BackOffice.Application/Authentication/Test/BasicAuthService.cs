using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.Application.Authentication.Clients;

namespace Veneka.Indigo.BackOffice.Application.Authentication.Test
{
    public class BasicAuthService : IAuthenticationService
    {
        private readonly IndigoServicesAuthClient _authenticationService;

        public BasicAuthService(Uri uri)
        {
            _authenticationService = new IndigoServicesAuthClient(uri);
        }
        public BackOfficeAppUser AuthenticateUser(string username, string password)
        {
           var respose=_authenticationService.Login(username, password);
            if(respose.ResponseCode== "00")
            return new BackOfficeAppUser(username,string.Empty, new string[0],respose.AuthToken);

            return new BackOfficeAppUser(username, string.Empty, new string[0], null);

        }
    }
}
