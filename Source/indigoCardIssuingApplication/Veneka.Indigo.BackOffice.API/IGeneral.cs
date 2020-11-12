using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.API
{
    [ServiceContract(Namespace = Constants.BackOfficeApiUrl)]
    public interface IGeneral
    {
        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<string> CheckStatus(string guid);

      

        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        Response<string[]> GetWorkStationKey(string workStation,int size, string token);

    }
}
