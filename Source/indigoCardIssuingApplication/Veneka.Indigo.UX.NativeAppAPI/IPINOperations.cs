using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [ServiceContract(Namespace = Constants.NativeAppApiUrl)]
    public interface IPINOperations
    {
        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<string> Logon(string username, string password);

        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<string[]> GetWorkingKey(Token token);

        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<ProductSettings> GetProductConfig(CardData cardData, Token token);

        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<string> Complete(CardData cardData, Token token);
    }
}
