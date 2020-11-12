using Common.Logging;
using IndigoCardIssuanceService.COMS;
using IndigoCardIssuanceService.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;

namespace IndigoCardIssuanceService.bll.Logic
{
    public class AccountLookupLogic
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AccountLookupLogic));
        private readonly CardMangementService _cardManService;
        private readonly IComsCore _comsCore;
        private readonly IIntegrationController _integration;

        public AccountLookupLogic(CardMangementService cardManagementService, IComsCore comsCore, IIntegrationController integration)
        {
            _cardManService = cardManagementService;
            _comsCore = comsCore;
            _integration = integration;
        }

        public Response<AccountDetails> CoreBankingAccountLookupFundsLoad(int issuerId, int productId, int cardIssueReasonId, int branchId, string accountNumber, AuditInfo auditInfo)
        {
            _log.Trace(t => t("Looking up account in Core Banking System."));

            //Find printer fields for product
            List<IProductPrintField> printFields = _cardManService.GetProductPrintFields(productId, null, null);

            // Get configuration and external fields for product
            Veneka.Indigo.Integration.Config.IConfig config;
            ExternalSystemFields externalFields;
            _integration.FundsLoadCoreBankingSystem(productId, out externalFields, out config);

            InterfaceInfo interfaceInfo = new InterfaceInfo
            {
                Config = config,
                InterfaceGuid = config.InterfaceGuid.ToString()
            };

            CardObject _object = new CardObject
            {
                CustomerAccount = new AccountDetails
                {
                    AccountNumber = accountNumber
                },
                PrintFields = printFields,
            };
            _log.Debug("calling GetAccountDetail");

            var cbsResponse = _comsCore.GetAccountDetail(_object, cardIssueReasonId, issuerId, branchId, productId, externalFields, interfaceInfo, auditInfo);

            if (cbsResponse.ResponseCode == 0)
            {
                // Sanity check that integration has actually returned an AccountDetails Object
                if (cbsResponse.Value == null)
                {
                    throw new Exception("Core Banking Interface responded successfuly but AccountDetail is null.");
                }

                //Check that the integration layer sent back the productFields
                if (cbsResponse.Value.ProductFields == null || cbsResponse.Value.ProductFields.Count < printFields.Count)
                {
                    throw new Exception("Integration layer has not returned all Product Print Fields.");
                }

                _log.Trace(t => t("Account successfully found in Core Banking System."));
                var accountDetails = cbsResponse.Value;

                // Do account mapping.                   
                // get the cms acconttype ,indigo_accountypte from account mapping assing to cardobject.
                ProductAccountTypeMapping mapping;
                if (_cardManService.TryGetProductAccountTypeMapping(productId, accountDetails.CBSAccountTypeId, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out mapping))
                {
                    _log.Trace(t => t("CBS account mapping found"));
                    accountDetails.CMSAccountTypeId = mapping.CmsAccountType;
                    accountDetails.AccountTypeId = mapping.IndigoAccountTypeId;
                }
                else
                {
                    throw new Exception(string.Format("No mapping found for product {0} and CBS Account Type {1}", productId, accountDetails.CBSAccountTypeId));
                }

                // Name on card build
                try
                {
                    //Logic for name on card if the filed is not populated, if it is populated then ignore integration layer handled it
                    IProductPrintField obj = printFields.Find(i => i.MappedName.Trim().ToUpper() == "IND_SYS_NOC");
                    if (String.IsNullOrWhiteSpace(accountDetails.NameOnCard) && obj != null)
                    {
                        accountDetails.NameOnCard = Veneka.Indigo.Integration.Common.Utility.BuildNameOnCard(accountDetails.FirstName, accountDetails.MiddleName, accountDetails.LastName);
                    }
                }
                catch (Exception ex)
                {
                    _log.Warn("Issue building name on card.", ex);
                }

                return new Response<AccountDetails>(accountDetails, ResponseType.SUCCESSFUL, cbsResponse.ResponseMessage, "");
            }

            return new Response<AccountDetails>(null, ResponseType.UNSUCCESSFUL, cbsResponse.ResponseMessage, "");
        }

        public Response<AccountDetails> CoreBankingAccountLookup(int issuerId, int productId, int cardIssueReasonId, int branchId, string accountNumber, AuditInfo auditInfo)
        {
            _log.Trace(t => t("Looking up account in Core Banking System."));

            //Find printer fields for product
            List<IProductPrintField> printFields = _cardManService.GetProductPrintFields(productId, null, null);

            // Get configuration and external fields for product
            Veneka.Indigo.Integration.Config.IConfig config;
            ExternalSystemFields externalFields;
            _integration.CoreBankingSystem(productId, InterfaceArea.ISSUING, out externalFields, out config);

            InterfaceInfo interfaceInfo = new InterfaceInfo
            {
                Config = config,
                InterfaceGuid = config.InterfaceGuid.ToString()
            };

            CardObject _object = new CardObject
            {
                CustomerAccount = new AccountDetails
                {
                    AccountNumber = accountNumber
                },
                PrintFields = printFields,
            };
            _log.Debug("calling GetAccountDetail");

            var cbsResponse = _comsCore.GetAccountDetail(_object, cardIssueReasonId, issuerId, branchId, productId, externalFields, interfaceInfo, auditInfo);

            if (cbsResponse.ResponseCode == 0)
            {
                // Sanity check that integration has actually returned an AccountDetails Object
                if (cbsResponse.Value == null)
                {
                    throw new Exception("Core Banking Interface responded successfuly but AccountDetail is null.");
                }

                //Check that the integration layer sent back the productFields
                if (cbsResponse.Value.ProductFields == null || cbsResponse.Value.ProductFields.Count < printFields.Count)
                {
                    throw new Exception("Integration layer has not returned all Product Print Fields.");
                }

                _log.Trace(t => t("Account successfully found in Core Banking System."));
                var accountDetails = cbsResponse.Value;

                // Do account mapping.                   
                // get the cms acconttype ,indigo_accountypte from account mapping assing to cardobject.
                ProductAccountTypeMapping mapping;
                if (_cardManService.TryGetProductAccountTypeMapping(productId, accountDetails.CBSAccountTypeId, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out mapping))
                {
                    _log.Trace(t => t("CBS account mapping found"));
                    accountDetails.CMSAccountTypeId = mapping.CmsAccountType;
                    accountDetails.AccountTypeId = mapping.IndigoAccountTypeId;
                }
                else
                {
                    throw new Exception(string.Format("No mapping found for product {0} and CBS Account Type {1}", productId, accountDetails.CBSAccountTypeId));
                }

                // Name on card build
                try
                {
                    //Logic for name on card if the filed is not populated, if it is populated then ignore integration layer handled it
                    IProductPrintField obj = printFields.Find(i => i.MappedName.Trim().ToUpper() == "IND_SYS_NOC");
                    if (String.IsNullOrWhiteSpace(accountDetails.NameOnCard) && obj != null)
                    {
                        accountDetails.NameOnCard = Veneka.Indigo.Integration.Common.Utility.BuildNameOnCard(accountDetails.FirstName, accountDetails.MiddleName, accountDetails.LastName);
                    }
                }
                catch (Exception ex)
                {
                    _log.Warn("Issue building name on card.", ex);
                }

                return new Response<AccountDetails>(accountDetails, ResponseType.SUCCESSFUL, cbsResponse.ResponseMessage, "");
            }

            return new Response<AccountDetails>(null, ResponseType.UNSUCCESSFUL, cbsResponse.ResponseMessage, "");
        }

        public bool CardManagementAccountLookup(int issuerId, int productId, int cardIssueReasonId, string accountNumber, AuditInfo auditInfo, AccountDetails accountDetails, out string responseMessage)
        {
            _log.Trace(t => t("Looking up account in Card Management System."));

            Veneka.Indigo.Integration.Config.IConfig cmsconfig;
            ExternalSystemFields externalFields;
            _integration.CardManagementSystem(productId, InterfaceArea.ISSUING, out externalFields, out cmsconfig);

            InterfaceInfo cmsinterfaceInfo = new InterfaceInfo
            {
                Config = cmsconfig,
                InterfaceGuid = cmsconfig.InterfaceGuid.ToString()

            };

            //Do lookup in CMS
            var cmsresponse = _comsCore.AccountLookup(issuerId, productId, cardIssueReasonId, accountNumber, accountDetails, externalFields, cmsinterfaceInfo, auditInfo);

            if (cmsresponse.ResponseCode == 0)
            {
                // Sanity check that integration has actually returned an AccountDetails Object
                if (accountDetails == null)
                {
                    throw new Exception("Card Management Interface responded successfuly but AccountDetail is null.");
                }

                _log.Trace(t => t("Account successfully found in Card Management System."));

                //Validate returned account
                return ValidateAccount(productId, accountDetails, out responseMessage);
            }

            responseMessage = cmsresponse.ResponseCode + " " + cmsresponse.ResponseMessage;
            return false;
        }

        public bool ValidateAccount(int productId, AccountDetails accountDetails, out string messages)
        {
            bool result = true;
            messages = String.Empty;

            //Check that the product supports the currency. If ID is -1 ignore this check
            if (accountDetails.CurrencyId != -1)
            {
                var productCurrency = _cardManService.GetProductCurrencies(productId, accountDetails.CurrencyId, true, -2, "SYSTEM");
                if (productCurrency == null || productCurrency.Count == 0)
                {
                    result = false;
                    messages = "Product does not support returned currency.";
                }
            }
            else
            {
                _log.Trace(t => t("CurrencyId set to -1, skipping currency check"));
            }

            //check that the product supports the account type. If ID is -1 ignore this check
            if (accountDetails.AccountTypeId != -1)
            {
                var productAccount = _cardManService.GetProductAccountTypes(productId, 0, -2, "SYSTEM");
                if (productAccount == null || productAccount.Where(w => w.account_type_id == accountDetails.AccountTypeId).Count() == 0)
                {
                    result = false;
                    messages += (String.IsNullOrWhiteSpace(messages) ? "" : "<br />") + "Product does not support returned account type.";
                }
            }
            else
            {
                _log.Trace(t => t("AccountTypeId set to -1, skipping account type check"));
            }

            return result;
        }
    }
}