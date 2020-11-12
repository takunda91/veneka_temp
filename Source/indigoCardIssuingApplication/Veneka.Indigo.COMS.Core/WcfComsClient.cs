using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common.Models.IssuerManagement;
using Veneka.Indigo.COMS.Core.Behavior;
using Veneka.Indigo.COMS.Core.Integration;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.Core
{
   
    /// <summary>
    /// If the Card Operations and Management module is installed remotely this class provides the WCF interface.
    /// </summary>
    public class WcfComsClient : IComsCore
    {
        private readonly IComsCore _proxy;
        private readonly string urlPath = "SystemsAPI.svc";       

        public WcfComsClient(Uri url)
        {
            Uri uri = null;
            if (Uri.TryCreate(url, urlPath, out uri))
            {               
                WSHttpBinding binding = new WSHttpBinding();                
                binding.Security.Mode = SecurityMode.Transport;

                EndpointAddress endpoint = new EndpointAddress(uri);
              
                
                ChannelFactory<IComsCore> factory = new ChannelFactory<IComsCore>(binding, endpoint);
                 factory.Endpoint.EndpointBehaviors.Add(new ClientTrackerEndpointBehavior());
                _proxy = factory.CreateChannel();

                // Set the Proxy's settings for call back
                IgnoreUntrustedSSL();
               
            }
            else
            {
                throw new ArgumentException("URL not in correct format: " + url);
            }
        }

       

        private void IgnoreUntrustedSSL()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

            ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate,
                                                                        X509Chain chain,
                                                                        SslPolicyErrors sslPolicyErrors) => true;
        }
        public WcfComsClient(IComsCore comsCoreAPI)
        {
            _proxy = comsCoreAPI;
        }
        

        public ComsResponse<DecryptedFields> DecryptFields(ZoneMasterKey zmk, TerminalSessionKey tpk, Product product, TerminalCardData termCardData, string operatorCode, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _proxy.DecryptFields(zmk, tpk, product, termCardData, operatorCode, interfaceInfo, auditIn);
        }

        public ComsResponse<DecryptedFields> DecryptFieldsWithMessaging(ZoneMasterKey zmk, TerminalSessionKey tpk, Product product, TerminalCardData termCardData, string operatorCode, Common.Models.CustomerDetailsResult customer, Messaging Message, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _proxy.DecryptFieldsWithMessaging(zmk, tpk, product, termCardData, operatorCode,customer, Message, interfaceInfo, auditIn);
        }

        public ComsResponse<TerminalSessionKey> GenerateRandomKey(int issuerId, string tmk, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _proxy.GenerateRandomKey(issuerId, tmk, deviceId, interfaceInfo, auditIn);
        }

        public ComsResponse<string> GenerateIBMPVV(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _proxy.GenerateIBMPVV(issuerId, product, decryptedFields, deviceId, interfaceInfo, auditIn);
        }

        public ComsResponse<string> GenerateVisaPVV(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _proxy.GenerateVisaPVV(issuerId, product, decryptedFields, deviceId, interfaceInfo, auditIn);
        }       

        public ComsResponse<string> PinFromPinBlock(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _proxy.PinFromPinBlock(issuerId, product, decryptedFields, deviceId, interfaceInfo, auditIn);
        }

        public ComsResponse<List<CardObject>> GenerateCVV(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInf)
        {
            return _proxy.GenerateCVV(cardObjects, interfaceInfo, auditInf);
        }

        public ComsResponse<List<CardObject>> GeneratePVV(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInf)
        {
            return _proxy.GeneratePVV(cardObjects, interfaceInfo, auditInf);
        }

        public ComsResponse<List<CardObject>> GenerateCardEncryptionData(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInf)
        {
            return _proxy.GenerateCardEncryptionData(cardObjects, interfaceInfo, auditInf);
        }

        public ComsResponse<List<CardObject>> PrintPins(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditIn)
        {
            return _proxy.PrintPins(cardObjects, interfaceInfo, auditIn);
        }

        public ComsResponse<AccountDetails> AccountLookup(int issuerId, int productId, int cardIssueReasonId, string accountNumber, AccountDetails accountDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _proxy.AccountLookup(issuerId, productId, cardIssueReasonId, accountNumber, accountDetails, externalFields, interfaceInfo, auditInfo);
        }
        public ComsResponse<string> LinkCardToAccountAndActive(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _proxy.LinkCardToAccountAndActive(customerDetails, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<string> LinkCardsToAccount(List<CustomerDetails> customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo, out Dictionary<long, LinkResponse> response)
        {
            return _proxy.LinkCardsToAccount(customerDetails, externalFields, interfaceInfo, auditInfo,out response);
        }

        public ComsResponse<string> ActiveCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _proxy.ActiveCard(customerDetails, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<string> SpoilCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _proxy.SpoilCard(customerDetails, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<string> UpdatePVV(int issuerId, int productId, Track2 track2, string PVV, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _proxy.UpdatePVV(issuerId, productId, track2, PVV, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<string> UploadGeneratedCards(List<CardObject> cardObjects, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _proxy.UploadGeneratedCards(cardObjects, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<string> ValidateCustomerDetails(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _proxy.ValidateCustomerDetails(customerDetails, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<List<IntegrationController.IntegrationInterface>> GetIntegrationInterfacesbyInterfaceid(int interfaceTypeId)
        {
            return _proxy.GetIntegrationInterfacesbyInterfaceid(interfaceTypeId);
        }
        public ComsResponse<bool> CheckConnection()
        {
            return _proxy.CheckConnection();
        }
        public ComsResponse<bool> ReloadIntegration(byte[] integrationfilestream, string checksum)
        {
            return _proxy.ReloadIntegration( integrationfilestream,  checksum);
        }
        
        public ComsResponse<string> EPinRequest(string indigoID, string PRRN, string mobilenumber, string pan)
        {
            return _proxy.EPinRequest(indigoID, PRRN, mobilenumber, pan);
        }


        #region CBS
        public ComsResponse<AccountDetails> GetAccountDetail(CardObject cardobject, int cardIssueReasonId, int issuerId, int branchId, int productId, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {


            return _proxy.GetAccountDetail(cardobject, cardIssueReasonId, issuerId, branchId, productId, externalFields, interfaceInfo, auditInfo);

        }

        public ComsResponse<bool> UpdateAccount(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _proxy.UpdateAccount(customerDetails, externalFields, interfaceInfo, auditInfo);
        }

        public ComsResponse<bool> CheckBalance(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _proxy.CheckBalance( customerDetails,  externalFields,  interfaceInfo,  auditInfo);
        }

        public ComsResponse<bool> ChargeFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _proxy.ChargeFee( customerDetails,  externalFields,  interfaceInfo,  auditInfo);
        }

        public ComsResponse<bool> ReverseFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            return _proxy.ReverseFee( customerDetails,  externalFields,  interfaceInfo,  auditInfo);
        }

      
        #endregion
    }
}
