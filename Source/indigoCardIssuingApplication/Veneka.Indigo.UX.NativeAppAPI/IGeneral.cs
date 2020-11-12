using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [ServiceContract(Namespace = Constants.NativeAppApiUrl)]
    public interface IGeneral
    {
        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<string> CheckStatus(string guid);
    }
}
