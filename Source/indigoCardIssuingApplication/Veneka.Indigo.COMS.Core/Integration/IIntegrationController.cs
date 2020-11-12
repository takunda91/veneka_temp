using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using static Veneka.Indigo.COMS.Core.Integration.IntegrationController;

namespace Veneka.Indigo.COMS.Core.Integration
{
    [ServiceContract(Namespace = Constants.IndigoComsURL)]
    public interface IIntegrationController
    {
        [OperationContract]
        ComsResponse<bool> ReloadInterfaces(byte[] integrationfilestream,string checksum);

        [OperationContract]
        ComsResponse<List<IntegrationInterface>> GetIntegrationInterfaces();
        [OperationContract]
        ComsResponse<List<IntegrationInterface>> GetIntegrationInterfacesbyInterfaceid(int interfacetypeid);

    }
}
