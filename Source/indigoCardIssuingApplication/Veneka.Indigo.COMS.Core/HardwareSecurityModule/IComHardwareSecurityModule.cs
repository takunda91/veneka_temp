using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common.Models.IssuerManagement;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.Core
{
    [ServiceContract(Namespace = Constants.IndigoComsURL)]
    public interface IComHardwareSecurityModule
    {
        [OperationContract]
        ComsResponse<List<CardObject>> GeneratePVV(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInf);

        [OperationContract]
        ComsResponse<List<CardObject>> GenerateCVV(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInf);

        [OperationContract]
        ComsResponse<List<CardObject>> GenerateCardEncryptionData(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInf);

        [OperationContract]
        ComsResponse<List<CardObject>> PrintPins(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditIn);

        [OperationContract]
        ComsResponse<TerminalSessionKey> GenerateRandomKey(int issuerId, string tmk, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn);

        [OperationContract]
        ComsResponse<DecryptedFields> DecryptFields(ZoneMasterKey zmk, TerminalSessionKey tpk, Product product, TerminalCardData termCardData, string operatorCode, InterfaceInfo interfaceInfo, AuditInfo auditIn);
        
        [OperationContract]
        ComsResponse<DecryptedFields> DecryptFieldsWithMessaging(ZoneMasterKey zmk, TerminalSessionKey tpk, Product product, TerminalCardData termCardData, string operatorCode, Common.Models.CustomerDetailsResult customer, Messaging Message, InterfaceInfo interfaceInfo, AuditInfo auditIn);

        [OperationContract]
        ComsResponse<string> GenerateIBMPVV(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn);

        [OperationContract]
        ComsResponse<string> GenerateVisaPVV(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn);

        [OperationContract]
        ComsResponse<string> PinFromPinBlock(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn);
    }
}
