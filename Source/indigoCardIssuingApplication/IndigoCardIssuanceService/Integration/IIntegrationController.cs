using Veneka.Indigo.Common;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;

namespace Veneka.Indigo.Integration
{
    public interface IIntegrationController
    {
        void CardManagementSystem(int productId, InterfaceArea interfaceArea, out ExternalSystemFields externalFields, out IConfig config);
        ICardProductionSystem CardProductionSystem(int productId, InterfaceArea interfaceArea, out ExternalSystemFields externalFields, out IConfig config);
        void CoreBankingSystem(int productId, InterfaceArea interfaceArea, out ExternalSystemFields externalFields, out IConfig config);
        IExternalAuthentication ExternalAuthentication(string guid);
        void HardwareSecurityModule(int issuerId, InterfaceArea interfaceArea, out IConfig config);

        void FundsLoadCoreBankingSystem(int productId, out External.ExternalSystemFields externalFields, out Config.IConfig config);
    }
}