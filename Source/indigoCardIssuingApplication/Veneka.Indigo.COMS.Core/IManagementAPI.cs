using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.COMS.Core
{
    [ServiceContract(Namespace = Constants.IndigoComsURL)]
    public interface IManagementAPI
    {
        [OperationContract]
        ComsResponse<bool> ReloadIntegration(byte[] fileStream, string checkSum);

    }
}
