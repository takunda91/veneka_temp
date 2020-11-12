using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.ServicesAuthentication.API.DataContracts;

namespace Veneka.Indigo.ServicesAuthentication.API
{
    [ServiceContract(Namespace = Constants.ServicesAuthApiUrl)]
    public interface IAuthentication
    {
        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        AuthenticationResponse Login(string username, string password);

        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        AuthenticationResponse MultiFactor(int type, string mfToken, string authToken);

        

    }
}
