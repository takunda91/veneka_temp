using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core.CoreBanking;
using Veneka.Indigo.COMS.Core.Integration;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;
using static Veneka.Indigo.COMS.Core.Integration.IntegrationController;

namespace Veneka.Indigo.COMS.Core
{
    [ServiceContract(Namespace = Constants.IndigoComsURL)]
    public interface IComsCore : IComCardManagement, IComHardwareSecurityModule, IComCoreBankingSystem
    {
        [OperationContract]
        ComsResponse<List<IntegrationInterface>> GetIntegrationInterfacesbyInterfaceid(int interfaceTypeId);

        [OperationContract]
        ComsResponse<bool> CheckConnection();

        [OperationContract]
        ComsResponse<bool> ReloadIntegration(byte[] integrationfilestream,string checksum);

    }
}
