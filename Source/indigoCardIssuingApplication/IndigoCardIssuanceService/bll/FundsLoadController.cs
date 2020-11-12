using Common.Logging;
using IndigoCardIssuanceService.bll.Logic;
using IndigoCardIssuanceService.COMS;
using IndigoCardIssuanceService.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Exceptions;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Objects;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.COMS.DataSource;
using Veneka.Indigo.FundsLoad;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Objects;

namespace IndigoCardIssuanceService.bll
{
    internal class FundsLoadController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FundsLoadController));
        private readonly IFundsOperations _fundsOperations = new FundsOperations();
        private IComsCore _comsCore;
        private IComPrepaidSystem _comsPrepaid = COMSController.PrepaidSystem;
        private IIntegrationController _integration;
        private readonly ResponseTranslator _translator = new ResponseTranslator();

        private readonly CardMangementService _cardManService;

        public FundsLoadController() : this(new LocalDataSource(), new CardManagementDAL(), null, null, null, new CardLimitDataAccess())
        {

        }

        public FundsLoadController(IDataSource dataSource, ICardManagementDAL cardManagementDAL, IIntegrationController integration, IComsCore comsCore, IResponseTranslator translator, ICardLimitDataAccess cardLimitDAL)
        {
            _cardManService = new CardMangementService(dataSource ?? new LocalDataSource(), cardManagementDAL, translator, cardLimitDAL);
            _integration = integration ?? IntegrationController.Instance;
            _comsCore = comsCore ?? COMSController.ComsCore;
        }

        internal BaseResponse ApprovalAccept(long id, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                _fundsOperations.ApprovalAccept(id, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        "",
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse ApprovalReject(long id, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                _fundsOperations.ApprovalReject(id, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        "",
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse Create(FundsLoadModel fundsModel, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                _fundsOperations.Create(fundsModel, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        "Funds Load successfully created.",
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse Delete(long id, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        internal Response<List<FundsLoadListModel>> ListByCard(string cardNumber, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var response = _fundsOperations.ListByCard(cardNumber, checkMasking, auditUserId, auditWorkstation).ToList();
                return new Response<List<FundsLoadListModel>>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<FundsLoadListModel>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<FundsLoadListModel>> List(FundsLoadStatusType statusType, int issuerId, int branchId, bool checkMasking, long auditUserId, string auditWorkStation)
        {
            try
            {
                var response = _fundsOperations.List(statusType, issuerId, branchId, checkMasking, auditUserId, auditWorkStation).ToList();
                return new Response<List<FundsLoadListModel>>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<FundsLoadListModel>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse Load(long id, int languageId, long auditUserId, string auditWorkstation, int cardIssueReasonId)
        {
            try
            {
                if (ExecuteFundsLoad(id, auditUserId, auditWorkstation, languageId, cardIssueReasonId))
                {
                    _fundsOperations.Load(id, auditUserId, auditWorkstation);
                    return new BaseResponse(ResponseType.SUCCESSFUL,
                                            "",
                                            "");
                }
                else
                {
                    return new BaseResponse(ResponseType.ERROR,
                                       "Error processing request, please try again.",
                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Could not complete both legs of the funds load" : "");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<FundsLoadListModel> Retrieve(long fundsLoadId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var response = _fundsOperations.Retrieve(fundsLoadId, checkMasking, auditUserId, auditWorkstation);
                return new Response<FundsLoadListModel>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<FundsLoadListModel>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse ReviewAccept(long id, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                _fundsOperations.ReviewAccept(id, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        "",
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse ReviewReject(long id, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                _fundsOperations.ReviewReject(id, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        "",
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse SendSMS(long id, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                _fundsOperations.SendSMS(id, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        "",
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<FundsLoadListModel>> ApproveBulk(FundsLoadStatusType statusType, List<long> fundsLoads, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                List<FundsLoadListModel> result = new List<FundsLoadListModel>();
                bool allProcessed = true;
                foreach (var item in fundsLoads)
                {
                    switch (statusType)
                    {
                        case FundsLoadStatusType.Created:
                            _fundsOperations.ReviewAccept(item, auditUserId, auditWorkstation);
                            break;
                        case FundsLoadStatusType.Reviewed:
                            _fundsOperations.ApprovalAccept(item, auditUserId, auditWorkstation);
                            break;
                        case FundsLoadStatusType.Approved:
                            allProcessed = allProcessed && ExecuteFundsLoadNoCardReason(item, auditUserId, auditWorkstation, languageId);
                            break;
                        default:
                            break;
                    }
                }
                if (allProcessed)
                {
                    return new Response<List<FundsLoadListModel>>(result, ResponseType.SUCCESSFUL,
                                                        string.Empty,
                                                        string.Empty);
                }
                else
                {
                    return new Response<List<FundsLoadListModel>>(null, ResponseType.UNSUCCESSFUL,
                                                                  "Approve Bulk Failed on one or more entries",
                                                                  "Not all entries were successully loaded");
                }
            }
            catch (Exception exp)
            {
                return new Response<List<FundsLoadListModel>>(null, ResponseType.UNSUCCESSFUL,
                                                                   "Approve Bulk Failed on one or more entries",
                                                                   exp.Message);
            }

        }

        internal Response<List<FundsLoadListModel>> RejectBulk(FundsLoadStatusType statusType, List<long> fundsLoads, bool v, long userId, string workstation)
        {
            List<FundsLoadListModel> result = new List<FundsLoadListModel>();
            foreach (var item in fundsLoads)
            {
                switch (statusType)
                {
                    case FundsLoadStatusType.Created:
                        _fundsOperations.ReviewReject(item, userId, workstation);
                        break;
                    case FundsLoadStatusType.Reviewed:
                        _fundsOperations.ApprovalReject(item, userId, workstation);
                        break;
                    default:
                        break;
                }
            }
            return new Response<List<FundsLoadListModel>>(result, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
        }

        internal Response<FundsLoadListModel> Approve(FundsLoadStatusType statusType, long fundsLoadId, bool v, long userId, string workstation, int languageId, int cardIssueReasonId)
        {
            FundsLoadListModel result = new FundsLoadListModel();

            switch (statusType)
            {
                case FundsLoadStatusType.Created:
                    if (_fundsOperations.ReviewAccept(fundsLoadId, userId, workstation) == SystemResponseCode.SUCCESS)
                    {
                        return new Response<FundsLoadListModel>(result, ResponseType.SUCCESSFUL,
                                                    string.Empty,
                                                    string.Empty);
                    }
                    break;
                case FundsLoadStatusType.Reviewed:
                    if (_fundsOperations.ApprovalAccept(fundsLoadId, userId, workstation) == SystemResponseCode.SUCCESS)
                    {
                        return new Response<FundsLoadListModel>(result, ResponseType.SUCCESSFUL,
                                                    string.Empty,
                                                    string.Empty);
                    }
                    break;
                case FundsLoadStatusType.Approved:
                    if (ExecuteFundsLoad(fundsLoadId, userId, workstation, languageId, cardIssueReasonId))
                    {
                        _fundsOperations.Load(fundsLoadId, userId, workstation);
                        return new Response<FundsLoadListModel>(result, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
                    }
                    break;
                default:
                    break;
            }
            return new Response<FundsLoadListModel>(null, ResponseType.UNSUCCESSFUL,
                                   "Funds load action failed",
                                   "Funds load action failed, either insufficient funds or a technical issue.");

        }

        internal Response<FundsLoadListModel> Reject(FundsLoadStatusType statusType, long fundsLoadId, bool v, long userId, string workstation)
        {
            FundsLoadListModel result = new FundsLoadListModel();

            switch (statusType)
            {
                case FundsLoadStatusType.Created:
                    if (_fundsOperations.ReviewReject(fundsLoadId, userId, workstation) == SystemResponseCode.SUCCESS)
                    {
                        return new Response<FundsLoadListModel>(result, ResponseType.SUCCESSFUL,
                                               string.Empty,
                                               string.Empty);
                    }
                    break;
                case FundsLoadStatusType.Reviewed:
                    if (_fundsOperations.ApprovalReject(fundsLoadId, userId, workstation) == SystemResponseCode.SUCCESS)
                    {
                        return new Response<FundsLoadListModel>(result, ResponseType.SUCCESSFUL,
                                               string.Empty,
                                               string.Empty);
                    }
                    break;
                default:
                    break;
            }

            return new Response<FundsLoadListModel>(result, ResponseType.UNSUCCESSFUL,
                                                "Funds load reject action failed.",
                                                "Funds load reject action failed.  Check account and connectivity.");
        }

        internal Response<AccountDetails> CheckBankAccountDetails(int issuerId, int productId, int branchId, string accountNumber, decimal amount, long auditUserId, string auditWorkstation, int languageId, int cardIssueReasonId)
        {
            var auditInfo = new AuditInfo
            {
                AuditUserId = auditUserId,
                AuditWorkStation = auditWorkstation,
                LanguageId = languageId
            };
            IntegrationController _integration = IntegrationController.Instance;
            var accountLookupLogic = new Logic.AccountLookupLogic(_cardManService, _comsCore, _integration);

            // Do CBS Lookup
            var cbsResponse = accountLookupLogic.CoreBankingAccountLookupFundsLoad(issuerId, productId, cardIssueReasonId, branchId, accountNumber, auditInfo);

            if (cbsResponse.ResponseType == ResponseType.SUCCESSFUL)
            {
                string responseMessage;
                log.Trace(t => t("cbsResponse.ResponseType == ResponseType.SUCCESSFUL"));

                log.Trace(t => t("CardManagementAccountLookup = True"));
                var balanceResponse = CheckBalance(cbsResponse, issuerId, productId, branchId, languageId, amount, auditUserId, auditWorkstation, accountNumber, out responseMessage);
                if (balanceResponse == true)
                {
                    return cbsResponse;
                }
                else
                {
                    log.Trace(responseMessage);
                }

                return new Response<AccountDetails>(null, ResponseType.UNSUCCESSFUL, responseMessage, String.Empty);
            }
            return cbsResponse;
        }

        private bool CheckAccountBalance(CustomerDetails customerDetails, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            log.Trace(t => t("CheckAccountBalance: New Path."));
            responseMessage = String.Empty;
            //work out total
            customerDetails.TotalCharged = customerDetails.FeeCharge.GetValueOrDefault();

            Veneka.Indigo.Integration.Config.IConfig config;
            Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;


            IntegrationController _integration = IntegrationController.Instance;
            _integration.FundsLoadCoreBankingSystem(customerDetails.ProductId, out externalFields, out config);

            InterfaceInfo interfaceInfo = new InterfaceInfo
            {
                Config = config,
                InterfaceGuid = config.InterfaceGuid.ToString()
            };
            log.Trace(config.ToString());

            AuditInfo auditInfo = new AuditInfo
            {
                AuditUserId = auditUserId,
                AuditWorkStation = auditWorkstation,
                LanguageId = languageId
            };


            log.Trace(t => t("Calling ComsCore.CheckBalance."));
            var response = COMS.COMSController.ComsCore.CheckBalance(customerDetails, externalFields, interfaceInfo, auditInfo);
            log.Trace(t => t($"ComsCore.CheckBalance Response = {response.ResponseCode} {response.ResponseMessage}"));
            if (response.ResponseCode == 0)
            {
                return true;
            }
            else
            {
                responseMessage = response.ResponseMessage;
                log.Trace(responseMessage);
                return false;
            }
        }

        private bool CheckBalance(Response<AccountDetails> cbsResponse, int issuerId, int productId, int branchId, int languageId, decimal amount, long auditUserId, string auditWorkstation, string accountNumber, out string responseMessage)
        {
            log.Trace(t => t("FundsLoadController.CheckBalance."));
            CustomerDetails customerDetails = ExtractCustomerFromAccount(cbsResponse, branchId, productId, issuerId, amount);
            customerDetails.AccountNumber = accountNumber;
            log.Trace(t => t(customerDetails.ToString()));
            return CheckAccountBalance(customerDetails, languageId, auditUserId, auditWorkstation, out responseMessage);
        }

        internal Response<PrepaidAccountDetail> CheckPrepaidAccountDetails(int productId, string cardNumber, int mbr)
        {
            try
            {
                IntegrationController _integration = IntegrationController.Instance;
                Veneka.Indigo.Integration.Config.IConfig config;
                Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;

                _integration.FundsLoadPrepaidSystem(productId, out externalFields, out config);

                InterfaceInfo interfaceInfo = new InterfaceInfo
                {
                    Config = config,
                    InterfaceGuid = config.InterfaceGuid.ToString()
                };

                log.Debug("calling GetPrepaidAccountDetail");

                AuditInfo auditInfo = new AuditInfo();
                var cbsResponse = _comsPrepaid.GetPrepaidAccountDetail(cardNumber, mbr, externalFields, interfaceInfo, auditInfo);

                if (cbsResponse.ResponseCode == 0)
                {
                    // Sanity check that integration has actually returned an AccountDetails Object
                    if (cbsResponse.Value == null)
                    {
                        throw new Exception("Prepaid Interface responded successfully but AccountDetail is null.");
                    }

                    log.Trace(t => t("Call to Prepaid System returned ResponseCode==0 and cbsResponse.Value!=null."));

                    if (cbsResponse.ResponseCode == 0)
                    {
                        var accountDetails = cbsResponse.Value;
                        if (string.IsNullOrEmpty(accountDetails.AccountNumber))
                        {
                            return new Response<PrepaidAccountDetail>(null, ResponseType.UNSUCCESSFUL, "Prepaid Account Number is empty.", "Prepaid Interface responded successfully with object but account number is empty.");
                        }
                        return new Response<PrepaidAccountDetail>(accountDetails, ResponseType.SUCCESSFUL, cbsResponse.ResponseMessage, "");
                    }
                }
                else
                {
                    log.Trace(t => t("Account NOT found in Prepaid System."));
                }

                return new Response<PrepaidAccountDetail>(null, ResponseType.UNSUCCESSFUL, cbsResponse.ResponseMessage, "");
            }
            catch (Exception exp)
            {
                return new Response<PrepaidAccountDetail>(null, ResponseType.UNSUCCESSFUL, "Error checking prepaid account", exp.Message);
            }
        }

        private bool ExecuteFundsLoadNoCardReason(long id, long auditUserId, string auditWorkstation, int languageId)
        {
            var fundsLoad = _fundsOperations.Retrieve(id, false, auditUserId, auditWorkstation);
            var productDetails = _cardManService.GetProduct(fundsLoad.ProductId, auditUserId, auditWorkstation);
            int cardIssueReasonId = productDetails.CardIssueReasons[0];
            bool successful = ExecuteFundsLoad(id, auditUserId, auditWorkstation, languageId, cardIssueReasonId);
            if (successful)
            {
                _fundsOperations.Load(id, auditUserId, auditWorkstation);
            }
            return successful;
        }

        private bool ExecuteFundsLoad(long id, long auditUserId, string auditWorkstation, int languageId, int cardIssueReasonId)
        {
            try
            {
                var fundsLoad = _fundsOperations.Retrieve(id, false, auditUserId, auditWorkstation);
                var auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };
                IntegrationController _integration = IntegrationController.Instance;
                var accountLookupLogic = new Logic.AccountLookupLogic(_cardManService, _comsCore, _integration);
                log.Trace(t => t("ExecuteFundsLoad: CoreBankingAccountLookup"));
                var cbsResponse = accountLookupLogic.CoreBankingAccountLookup(fundsLoad.IssuerId, fundsLoad.ProductId, cardIssueReasonId, fundsLoad.BranchId, fundsLoad.BankAccountNo, auditInfo);
                string responseMessage;

                if (cbsResponse.ResponseType == ResponseType.SUCCESSFUL)
                {
                    log.Trace(t => t("cbsResponse.ResponseType == ResponseType.SUCCESSFUL"));
                    CustomerDetails customerDetails = ExtractCustomerFromAccount(cbsResponse, fundsLoad.BranchId, fundsLoad.ProductId, fundsLoad.IssuerId, fundsLoad.Amount);
                    customerDetails.AccountNumber = fundsLoad.BankAccountNo;
                    customerDetails.FeeCharge = fundsLoad.Amount;
                    customerDetails.TotalCharged = fundsLoad.Amount;

                    bool accountLegDone = ExecuteBankAccountDebit(fundsLoad, customerDetails, auditUserId, auditWorkstation, languageId);
                    bool prepaidLegDone = false;
                    if (accountLegDone)
                    {
                        log.Trace(t => t("Account Leg Done"));
                        prepaidLegDone = ExecutePrepaidAccountCredit(fundsLoad);
                        if (prepaidLegDone)
                        {
                            log.Trace(t => t("Prepaid Leg Done"));
                            return true;
                        }
                        else
                        {
                            //reverse the transaction to the main account
                            fundsLoad.Amount = fundsLoad.Amount * -1;
                            customerDetails.TotalCharged = fundsLoad.Amount;

                            ExecuteBankAccountDebit(fundsLoad, customerDetails, auditUserId, auditWorkstation, languageId);
                            return false;
                        }
                    }
                    else
                    {
                        log.Trace($"Account Leg Failed");
                    }
                }
                else
                {
                    log.Trace($"ExecuteFundLoad:cbsResponse.ResponseType <> ResponseType.SUCCESSFUL Actual Value is {cbsResponse.ResponseType} with message {cbsResponse.ResponseMessage}");
                    return false;
                }
            }
            catch (Exception exp)
            {
                log.Error($"An exception occurred : { exp.Message}");
                return false;
            }
            return false;
        }

        private bool ExecutePrepaidAccountCredit(FundsLoadListModel fundsLoad)
        {
            IntegrationController _integration = IntegrationController.Instance;
            Veneka.Indigo.Integration.Config.IConfig config;
            Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;

            _integration.FundsLoadPrepaidSystem(fundsLoad.ProductId, out externalFields, out config);

            InterfaceInfo interfaceInfo = new InterfaceInfo
            {
                Config = config,
                InterfaceGuid = config.InterfaceGuid.ToString()
            };

            log.Debug("calling CreditPrepaidAccount");

            AuditInfo auditInfo = new AuditInfo();
            var cbsResponse = _comsPrepaid.CreditPrepaidAccount(fundsLoad.Amount, fundsLoad.PrepaidAccountNo, externalFields, interfaceInfo, auditInfo);

            if (cbsResponse.ResponseCode == 0)
            {
                return true;
            }

            return false;
        }

        private bool ExecuteBankAccountDebit(FundsLoadListModel fundsLoad, CustomerDetails customerDetails, long auditUserId, string auditWorkstation, int languageId)
        {
            try
            {
                log.Trace(t => t("ExecuteBankAccountDebit"));
                Veneka.Indigo.Integration.Config.IConfig config;
                Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;

                IntegrationController _integration = IntegrationController.Instance;
                customerDetails.TotalCharged = customerDetails.FeeCharge.GetValueOrDefault();
                customerDetails.AccountNumber = fundsLoad.BankAccountNo;

                _integration.FundsLoadCoreBankingSystem(customerDetails.ProductId, out externalFields, out config);

                InterfaceInfo interfaceInfo = new InterfaceInfo
                {
                    Config = config,
                    InterfaceGuid = config.InterfaceGuid.ToString()
                };
                log.Trace(config.ToString());

                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };

                FeeChargeLogic feelogic = new FeeChargeLogic(_cardManService, _comsCore, _integration);

                BaseResponse feeresponse = feelogic.ChargeAmount(customerDetails, auditInfo, config, externalFields);
                log.Trace(t => t("ExecuteBankAccountDebit:After feelogic.FeeCharge"));
                if (feeresponse.ResponseType != ResponseType.SUCCESSFUL)
                {
                    log.Trace($"ExecuteBankAccountDebit: feeresponse.ResponseType != ResponseType.SUCCESSFUL || {feeresponse.ResponseMessage}");
                    return false;
                }
                else
                {
                    log.Trace($"ExecuteBankAccountDebit: feeresponse.ResponseType == ResponseType.SUCCESSFUL");
                    return true;
                }
            }
            catch (Exception exp)
            {
                log.Trace($"ExecuteBankAccountDebit: Exception Occurred := {exp.Message}");
                throw;
            }
        }

        private CustomerDetails ExtractCustomerFromAccount(Response<AccountDetails> bankResponse, int branchId, int productId, int issuerId, decimal amount)
        {
            AccountDetails account = bankResponse.Value;
            CustomerDetails customer = new CustomerDetails()
            {
                AccountNumber = account.AccountNumber,
                AccountTypeId = account.AccountTypeId,
                CurrencyId = account.CurrencyId,
                CustomerIDNumber = account.CustomerIDNumber,
                ContractNumber = account.ContractNumber,
                CustomerResidencyId = account.CustomerResidencyId,
                CustomerTitleId = account.CustomerTitleId,
                CustomerTypeId = account.CustomerTypeId,
                FirstName = account.FirstName,
                LastName = account.LastName,
                MiddleName = account.MiddleName,
                NameOnCard = account.NameOnCard,
                ContactNumber = account.ContactNumber,
                CmsID = account.CmsID,
                ProductFields = account.ProductFields,
                CustomerId = account.AccountNumber,
                DomicileBranchId = branchId,
                BranchId = branchId,
                DeliveryBranchId = branchId,
                CardIssueMethodId = 0,
                CardIssueReasonId = null,
                PriorityId = null,
                ProductId = productId,
                CBSAccountType = account.CBSAccountTypeId,
                CMSAccountType = account.CMSAccountTypeId,
                IssuerId = issuerId,
                FeeOverridenYN = false,
                FeeWaiverYN = false,
                FeeEditbleYN = false,
                FeeDetailId = null,
                FeeCharge = amount,
                TotalCharged = amount,
                VatCharged = 0,
                Vat = 0

            };

            return customer;
        }

        public Response<List<ProductValidated>> GetProductsWithFundsLoad(int issuerID, int? cardIssueMethodId, int pageIndex, int RowsPerpage, int languageId, long auditUserId, string auditWorkstation)
        {

            List<ProductValidated> productlist = new List<ProductValidated>();
            try
            {
                List<ProductValidated> allproductlist = _cardManService.GetProductsListValidated(issuerID, cardIssueMethodId, pageIndex, RowsPerpage, languageId, auditUserId, auditWorkstation);
                List<int> productIds = _fundsOperations.ProductsConfiguredForFundsLoad().ToList();

                productlist = allproductlist.Where(p => productIds.Contains(p.ProductId)).ToList();

                return new Response<List<ProductValidated>>(productlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (BaseIndigoException biex)
            {
                var responseMessage = _translator.TranslateResponseCode(biex.SystemCode, 0, languageId, auditUserId, auditWorkstation);
                return new Response<List<ProductValidated>>(null,
                                                            ResponseType.UNSUCCESSFUL,
                                                            responseMessage,
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? biex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ProductValidated>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

    }
}