using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.COMS.Core.Integration;
using Veneka.Indigo.COMS.WCF.DataSource;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = Constants.IndigoComsURL)]
    public class SystemsAPI : IComsCore
    {
        private readonly IComsCore _comsCore = new ComsCore(new IndigoWcfDataSource(new Uri(ConfigurationManager.AppSettings["Uri"])));

        public ComsResponse<AccountDetails> AccountLookup(int issuerId, int productId, int cardIssueReasonId, string accountNumber, AccountDetails accountDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _comsCore.AccountLookup(issuerId, productId, cardIssueReasonId, accountNumber, accountDetails, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<DecryptedFields> DecryptFields(ZoneMasterKey zmk, TerminalSessionKey tpk, Product product, TerminalCardData termCardData, string operatorCode, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _comsCore.DecryptFields(zmk, tpk, product, termCardData, operatorCode, interfaceInfo, auditIn);
        }

      

        public ComsResponse<List<CardObject>> GenerateCardEncryptionData(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInf)
        {
            return _comsCore.GenerateCardEncryptionData(cardObjects, interfaceInfo, auditInf);
        }

        public ComsResponse<List<CardObject>> GenerateCVV(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInf)
        {
            return _comsCore.GenerateCVV(cardObjects, interfaceInfo, auditInf);
        }

        public ComsResponse<string> GenerateIBMPVV(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _comsCore.GenerateIBMPVV(issuerId, product, decryptedFields, deviceId, interfaceInfo, auditIn);
        }

        public ComsResponse<List<CardObject>> GeneratePVV(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInf)
        {
            return _comsCore.GeneratePVV(cardObjects, interfaceInfo, auditInf);
        }

        public ComsResponse<TerminalSessionKey> GenerateRandomKey(int issuerId, string tmk, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _comsCore.GenerateRandomKey(issuerId, tmk, deviceId, interfaceInfo, auditIn);
        }

        public ComsResponse<string> GenerateVisaPVV(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _comsCore.GenerateVisaPVV(issuerId, product, decryptedFields, deviceId, interfaceInfo, auditIn);
        }

       

        public ComsResponse<List<IntegrationController.IntegrationInterface>> GetIntegrationInterfacesbyInterfaceid(int interfacetypeid)
        {
            return _comsCore.GetIntegrationInterfacesbyInterfaceid(interfacetypeid);
        }

        public ComsResponse<string> ActiveCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _comsCore.ActiveCard(customerDetails, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<string> LinkCardToAccountAndActive(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _comsCore.LinkCardToAccountAndActive(customerDetails, externalFields, interfaceInfo, auditInfo);
        }
        public ComsResponse<string> LinkCardsToAccount(List<CustomerDetails> customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo, out Dictionary<long, LinkResponse> response)
        {
           
            return _comsCore.LinkCardsToAccount(customerDetails, externalFields, interfaceInfo, auditInfo,out response);
        }
        public ComsResponse<string> PinFromPinBlock(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _comsCore.PinFromPinBlock(issuerId, product, decryptedFields, deviceId, interfaceInfo, auditIn);
        }

        public ComsResponse<List<CardObject>> PrintPins(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        { 
            return _comsCore.PrintPins(cardObjects, interfaceInfo, auditIn);
        }

        public ComsResponse<string> SpoilCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _comsCore.SpoilCard(customerDetails, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<string> UpdatePVV(int issuerId, int productId, Track2 track2, string PVV, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _comsCore.UpdatePVV(issuerId, productId, track2, PVV, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<string> UploadGeneratedCards(List<CardObject> cardObjects, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        { 
            return _comsCore.UploadGeneratedCards(cardObjects, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<string> ValidateCustomerDetails(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _comsCore.ValidateCustomerDetails(customerDetails, externalFields, interfaceInfo, auditInfo);
        }
        public ComsResponse<string> EPinRequest(string indigoID, string PRRN, string mobilenumber, string pan)
        {
            return _comsCore.EPinRequest(indigoID, PRRN, mobilenumber, pan);
        }

        public ComsResponse<bool> CheckConnection()
        {
            return _comsCore.CheckConnection();
        }
        public ComsResponse<bool> ReloadIntegration(byte[] integrationfilestream, string checksum)
        {
            return _comsCore.ReloadIntegration(integrationfilestream,  checksum);
        }
        
        public ComsResponse<AccountDetails> GetAccountDetail(CardObject obj, int cardIssueReasonId, int issuerId, int branchId, int productId, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _comsCore.GetAccountDetail(obj, cardIssueReasonId, issuerId, branchId, productId, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<bool> UpdateAccount(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _comsCore.UpdateAccount(customerDetails, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<bool> CheckBalance(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _comsCore.CheckBalance(customerDetails, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<bool> ChargeFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _comsCore.ChargeFee(customerDetails, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<bool> ReverseFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _comsCore.ReverseFee(customerDetails, externalFields, interfaceInfo, auditInfo);
        }
    }
}
