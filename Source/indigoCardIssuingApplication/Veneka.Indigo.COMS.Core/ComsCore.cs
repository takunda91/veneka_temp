using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Models.IssuerManagement;
using Veneka.Indigo.COMS.Core.Integration;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;
using static Veneka.Indigo.COMS.Core.Integration.IntegrationController;

namespace Veneka.Indigo.COMS.Core
{
    /// <summary>
    /// This is where the Card Operations and Management magic happens.
    /// </summary>
    public class ComsCore : IComsCore
    {
        //public readonly IComsDataSource _comsDataSource;
        public readonly IDataSource _dataSource;

      

        public ComsCore(IDataSource dataSource)
        {
            _dataSource = dataSource;            
        }

        public ComsResponse<bool> ReloadIntegration(byte[] integrationfilestream, string checksum)
        {

            return IntegrationController.Instance.ReloadInterfaces( integrationfilestream, checksum);

        }
        public ComsResponse<List<IntegrationInterface>> GetIntegrationInterfacesbyInterfaceid(int interfaceTypeId)
        {

            return IntegrationController.Instance.GetIntegrationInterfacesbyInterfaceid(interfaceTypeId);

        }

        #region HSM
        public ComsResponse<TerminalSessionKey> GenerateRandomKey(int issuerId, string tmk, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;
            TerminalSessionKey terminalSessionKey;

            try
            {
                var resp = IntegrationController.Instance.HardwareSecurityModule(interfaceInfo, _dataSource)
                           .GenerateRandomKey(issuerId, tmk, deviceId, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage, out terminalSessionKey);

                if (resp)
                {
                    return ComsResponse<TerminalSessionKey>.Success(responseMessage, terminalSessionKey);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<TerminalSessionKey>.Exception(ex, null);
            }

            return ComsResponse<TerminalSessionKey>.Failed(responseMessage, null);
        }

        public ComsResponse<DecryptedFields> DecryptFields(ZoneMasterKey zmk, TerminalSessionKey tpk, Product product, TerminalCardData termCardData, string operatorCode, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;
            DecryptedFields decryptedFields;

            try
            {
                var resp = IntegrationController.Instance.HardwareSecurityModule(interfaceInfo, _dataSource)
                            .DecryptFields(zmk, tpk, product, termCardData, operatorCode, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage, out decryptedFields);

                if (resp)
                {
                    return ComsResponse<DecryptedFields>.Success(responseMessage, decryptedFields);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<DecryptedFields>.Exception(ex, null);
            }

            return ComsResponse<DecryptedFields>.Failed(responseMessage, null);
        }

        public ComsResponse<DecryptedFields> DecryptFieldsWithMessaging(ZoneMasterKey zmk, TerminalSessionKey tpk, Product product, TerminalCardData termCardData, string operatorCode, Common.Models.CustomerDetailsResult customer, Messaging Message, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;
            DecryptedFields decryptedFields;

            try
            {
                var resp = IntegrationController.Instance.HardwareSecurityModule(interfaceInfo, _dataSource)
                            .DecryptFieldsWithMessaging(zmk, tpk, product, termCardData, operatorCode,customer,Message, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage, out decryptedFields);

                if (resp)
                {
                    return ComsResponse<DecryptedFields>.Success(responseMessage, decryptedFields);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<DecryptedFields>.Exception(ex, null);
            }

            return ComsResponse<DecryptedFields>.Failed(responseMessage, null);
        }

        public ComsResponse<string> GenerateIBMPVV(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;
            string pvv;

            try
            {
                var resp = IntegrationController.Instance.HardwareSecurityModule(interfaceInfo, _dataSource)
                            .GenerateIBMPVV(issuerId, product, decryptedFields, deviceId, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage, out pvv);

                if (resp)
                {
                    return ComsResponse<string>.Success(responseMessage, pvv);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<string>.Exception(ex, null);
            }

            return ComsResponse<string>.Failed(responseMessage, null);
        }        

        public ComsResponse<string> GenerateVisaPVV(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;
            string pvv;

            try
            {
                var resp = IntegrationController.Instance.HardwareSecurityModule(interfaceInfo, _dataSource)
                    .GenerateVisaPVV(issuerId, product, decryptedFields, deviceId, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage, out pvv);

                if (resp)
                {
                    return ComsResponse<string>.Success(responseMessage, pvv);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<string>.Exception(ex, null);
            }

            return ComsResponse<string>.Failed(responseMessage, null);
        }        

        public ComsResponse<string> PinFromPinBlock(int issuerId, Product product, DecryptedFields decryptedFields, string deviceId, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;
            string pvv;

            try
            {
                var resp = IntegrationController.Instance.HardwareSecurityModule(interfaceInfo, _dataSource)
                            .PinFromPinBlock(issuerId, product, decryptedFields, deviceId, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage, out pvv);

                if (resp)
                {
                    return ComsResponse<string>.Success(responseMessage, pvv);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<string>.Exception(ex, null);
            }

            return ComsResponse<string>.Failed(responseMessage, null);
        }

        public ComsResponse<List<CardObject>> GenerateCardEncryptionData(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;            

            try
            {
                var resp = IntegrationController.Instance.HardwareSecurityModule(interfaceInfo, _dataSource)
                            .GenerateCardEncryptionData(ref cardObjects, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp)
                {
                    return ComsResponse<List<CardObject>>.Success(responseMessage, cardObjects);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<List<CardObject>>.Exception(ex, null);
            }

            return ComsResponse<List<CardObject>>.Failed(responseMessage, null);
        }

        public ComsResponse<List<CardObject>> GenerateCVV(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.HardwareSecurityModule(interfaceInfo, _dataSource)
                            .GenerateCVV(ref cardObjects, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp)
                {
                    return ComsResponse<List<CardObject>>.Success(responseMessage, cardObjects);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<List<CardObject>>.Exception(ex, null);
            }

            return ComsResponse<List<CardObject>>.Failed(responseMessage, null);
        }

        public ComsResponse<List<CardObject>> GeneratePVV(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.HardwareSecurityModule(interfaceInfo, _dataSource)
                            .GeneratePVV(ref cardObjects, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp)
                {
                    return ComsResponse<List<CardObject>>.Success(responseMessage, cardObjects);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<List<CardObject>>.Exception(ex, null);
            }

            return ComsResponse<List<CardObject>>.Failed(responseMessage, null);
        }

        public ComsResponse<List<CardObject>> PrintPins(List<CardObject> cardObjects, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.HardwareSecurityModule(interfaceInfo, _dataSource)
                            .PrintPins(ref cardObjects, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp)
                {
                    return ComsResponse<List<CardObject>>.Success(responseMessage, cardObjects);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<List<CardObject>>.Exception(ex, null);
            }

            return ComsResponse<List<CardObject>>.Failed(responseMessage, null);
        }
        #endregion

        #region CMS
        public ComsResponse<AccountDetails> AccountLookup(int issuerId, int productId, int cardIssueReasonId, string accountNumber, AccountDetails accountDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance
                            .CardManagementSystem(interfaceInfo, _dataSource)
                            .AccountLookup(issuerId, productId, cardIssueReasonId, accountNumber, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, ref accountDetails, out responseMessage);

                if (resp)
                {
                    return ComsResponse<AccountDetails>.Success(responseMessage, accountDetails);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<AccountDetails>.Exception(ex, null);
            }

            return ComsResponse<AccountDetails>.Failed(responseMessage, null);
        }

        public ComsResponse<string> ActiveCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.CardManagementSystem(interfaceInfo, _dataSource)
                    .ActiveCard(customerDetails, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp == Veneka.Indigo.Integration.LinkResponse.SUCCESS)
                {
                    return ComsResponse<string>.Success(responseMessage, String.Empty);
                }
                else if (resp == Veneka.Indigo.Integration.LinkResponse.RETRY)
                {
                    return ComsResponse<string>.Retry(responseMessage, String.Empty);
                }
            }
            catch (Exception ex)
            {
                return ComsResponse<string>.Exception(ex, String.Empty);
            }

            return ComsResponse<string>.Failed(responseMessage, String.Empty);
        }

        public ComsResponse<string> LinkCardsToAccount(List<CustomerDetails> customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo,out  Dictionary<long, LinkResponse> response)
        {
            string responseMessage;
            response = new Dictionary<long, LinkResponse>();
            try
            {
                var resp = IntegrationController.Instance.CardManagementSystem(interfaceInfo, _dataSource)
                    .LinkCardsToAccount(customerDetails, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation,out response, out responseMessage);

                if (resp == Veneka.Indigo.Integration.LinkResponse.SUCCESS)
                {
                    return ComsResponse<string>.Success(responseMessage, String.Empty);
                }
                else if (resp == Veneka.Indigo.Integration.LinkResponse.RETRY)
                {
                    return ComsResponse<string>.Retry(responseMessage, String.Empty);
                }
            }
            catch (Exception ex)
            {
                return ComsResponse<string>.Exception(ex, String.Empty);
            }

            return ComsResponse<string>.Failed(responseMessage, String.Empty);
        }


        public ComsResponse<string> LinkCardToAccountAndActive(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.CardManagementSystem(interfaceInfo, _dataSource)
                    .LinkCardToAccountAndActive(customerDetails, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp == Veneka.Indigo.Integration.LinkResponse.SUCCESS)
                {
                    return ComsResponse<string>.Success(responseMessage, String.Empty);
                }
                else if (resp == Veneka.Indigo.Integration.LinkResponse.RETRY)
                {
                    return ComsResponse<string>.Retry(responseMessage, String.Empty);
                }
            }
            catch (Exception ex)
            {
                return ComsResponse<string>.Exception(ex, String.Empty);
            }

            return ComsResponse<string>.Failed(responseMessage, String.Empty);
        }

        public ComsResponse<string> SpoilCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.CardManagementSystem(interfaceInfo, _dataSource)
                    .SpoilCard(customerDetails, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp)
                {
                    return ComsResponse<string>.Success(responseMessage, String.Empty);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<string>.Exception(ex, String.Empty);
            }

            return ComsResponse<string>.Failed(responseMessage, String.Empty);
        }

        public ComsResponse<string> UpdatePVV(int issuerId, int productId, Track2 track2, string PVV, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.CardManagementSystem(interfaceInfo, _dataSource)
                    .UpdatePVV(issuerId, productId, track2, PVV, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp)
                {
                    return ComsResponse<string>.Success(responseMessage, String.Empty);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<string>.Exception(ex, String.Empty);
            }

            return ComsResponse<string>.Failed(responseMessage, String.Empty);
        }

        public ComsResponse<string> UploadGeneratedCards(List<CardObject> cardObjects, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.CardManagementSystem(interfaceInfo, _dataSource)
                    .UploadGeneratedCards(cardObjects, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp)
                {
                    return ComsResponse<string>.Success(responseMessage, String.Empty);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<string>.Exception(ex, String.Empty);
            }

            return ComsResponse<string>.Failed(responseMessage, String.Empty);
        }

        public ComsResponse<string> ValidateCustomerDetails(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.CardManagementSystem(interfaceInfo, _dataSource)
                    .ValidateCustomerDetails(customerDetails, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp)
                {
                    return ComsResponse<string>.Success(responseMessage, String.Empty);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<string>.Exception(ex, String.Empty);
            }

            return ComsResponse<string>.Failed(responseMessage, String.Empty);
        }

        public ComsResponse<string> EPinRequest(string indigoID, string PRRN, string mobilenumber, string pan)
        {
            throw new NotImplementedException();
        }



        #endregion

        #region CBS
        public ComsResponse<AccountDetails> GetAccountDetail(CardObject cardobject, int cardIssueReasonId, int issuerId, int branchId, int productId, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            AccountDetails accountDetails;
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance
                            .CoreBankingSystem(interfaceInfo, _dataSource)
                            .GetAccountDetail(cardobject.CustomerAccount.AccountNumber, cardobject.PrintFields,cardIssueReasonId,issuerId,branchId,productId, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation,out accountDetails, out responseMessage);

                if (resp)
                {
                    return ComsResponse<AccountDetails>.Success(responseMessage, accountDetails);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<AccountDetails>.Exception(ex, null);
            }

            return ComsResponse<AccountDetails>.Failed(responseMessage, null);
        }

        public ComsResponse<bool> UpdateAccount(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.CoreBankingSystem(interfaceInfo, _dataSource)
                    .UpdateAccount(customerDetails, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp)
                {
                    return ComsResponse<bool>.Success(responseMessage, true);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<bool>.Exception(ex, false);
            }

            return ComsResponse<bool>.Failed(responseMessage, false);
        }

        public ComsResponse<bool> CheckBalance(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.CoreBankingSystem(interfaceInfo, _dataSource)
                    .CheckBalance(customerDetails, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp)
                {
                    return ComsResponse<bool>.Success(responseMessage, true);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<bool>.Exception(ex, false);
            }

            return ComsResponse<bool>.Failed(responseMessage, false);
        }

        public ComsResponse<bool> ChargeFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;
            string chargefeeno;

            try
            {
                var resp = IntegrationController.Instance.CoreBankingSystem(interfaceInfo, _dataSource)
                    .ChargeFee(customerDetails, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation,out chargefeeno, out responseMessage);

                if (resp)
                {
                    customerDetails.FeeReferenceNumber = chargefeeno;

                    return ComsResponse<bool>.Success(responseMessage, true);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<bool>.Exception(ex, false);
            }

            return ComsResponse<bool>.Failed(responseMessage, false);
        }

        public ComsResponse<bool> ReverseFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, InterfaceInfo interfaceInfo, AuditInfo auditInfo)
        {
            string responseMessage;

            try
            {
                var resp = IntegrationController.Instance.CoreBankingSystem(interfaceInfo, _dataSource)
                    .ReverseFee(customerDetails, externalFields, interfaceInfo.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage);

                if (resp)
                {
                    return ComsResponse<bool>.Success(responseMessage, true);
                }

            }
            catch (Exception ex)
            {
                return ComsResponse<bool>.Exception(ex, false);
            }

            return ComsResponse<bool>.Failed(responseMessage, false);
        }

        public ComsResponse<bool> CheckConnection()
        {
            return  ComsResponse<bool>.Success("ComsCore is running.", true); 
        }
        #endregion

    }
}
