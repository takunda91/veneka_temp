using Common.Logging;
using IndigoCardIssuanceService.DataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Exceptions;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Objects;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Security;
using Veneka.Indigo.UserManagement;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.Integration.FileLoader.Objects;
using IndigoCardIssuanceService.DataContracts.PINReissue;
using Veneka.Indigo.COMS.Core;
using IndigoCardIssuanceService.COMS;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.AuditManagement;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.IssuerManagement;

using Veneka.Indigo.COMS.DataSource;
using IndigoCardIssuanceService.bll.Logic;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Common.Models.IssuerManagement;
//using Veneka.Indigo.IntegrationInterfaces;

namespace IndigoCardIssuanceService.bll
{
    public class IssueCardController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IssueCardController));
        private readonly CardMangementService _cardManService;
        private readonly ResponseTranslator _translator = new ResponseTranslator();
        private readonly AuditReportService _audit = new AuditReportService();
        private IComsCore _comsCoreInstance;
        private IIntegrationController _integration;
        public PrintBatchManagementService _printservice = new PrintBatchManagementService();

        public IssueCardController() : this(new LocalDataSource(), new CardManagementDAL(), null, null, null, new CardLimitDataAccess())
        {

        }

        public IssueCardController(IDataSource dataSource, ICardManagementDAL cardManagementDAL, IIntegrationController integration, IComsCore comsCore, IResponseTranslator translator, ICardLimitDataAccess cardLimitDAL)
        {
            _cardManService = new CardMangementService(dataSource ?? new LocalDataSource(), cardManagementDAL, translator, cardLimitDAL);
            _integration = integration ?? IntegrationController.Instance;
            _comsCoreInstance = comsCore ?? COMSController.ComsCore;
        }


        internal void LinkCardAndCust(long cardid, string accountId)
        {

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<CustomerDetailsResult> GetCustomerDetails(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<CustomerDetailsResult>(_cardManService.GetCustomerDetails(cardId, languageId, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<CustomerDetailsResult>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Search for a list of cards based on the parameteres provided. Null parameters wont be searched on.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cardNumber">May be null</param>
        /// <param name="distBatchReference">May be null</param>
        /// <param name="cardStatus">May be null</param>
        /// <param name="dateFrom">May be null</param>
        /// <param name="dateTo">May be null</param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<CardSearchResult>> SearchForCards(long userId, int? userRoleId, int? issuerId, int? branchId, string cardNumber,
                                                                string cardLastFourDigits, string cardrefnumber, string batchReference,
                                                                int? loadCardStatusId, int? distCardStatusId, int? branchCardStatusId, long? distBatchId, long? pinBatchId, long? threedBatchId,
                                                                string accountNumber, string firstName, string lastName, string cmsId,
                                                                DateTime? dateFrom, DateTime? dateTo, int? cardIssueMethodId,
                                                                int? productId, int? priorityId, int pageIndex, int rowsPerPage,
                                                                long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<CardSearchResult>>(_cardManService.SearchForCards(userId, userRoleId, issuerId, branchId, cardNumber, cardLastFourDigits, cardrefnumber, batchReference,
                                                            loadCardStatusId, distCardStatusId, branchCardStatusId, distBatchId, pinBatchId, threedBatchId, accountNumber, firstName, lastName, cmsId,
                                                            dateFrom, dateTo, cardIssueMethodId, productId, priorityId,
                                                            pageIndex, rowsPerPage, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CardSearchResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        /// <summary>
        /// Search for CustomerCardsList
        /// </summary>
        /// <param name="cardrefno"></param>
        /// <param name="customeraccountno"></param>
        /// <param name="pageIndex"></param>
        /// <param name="RowsPerPage"></param>
        /// <returns></returns>
        internal Response<List<CustomercardsearchResult>> SearchCustomerCardsList(int? issuerid, int? branchid, int? productid, int? cardissuemethodid,
            int? priorityid, string cardrefno, string customeraccountno, int pageIndex, int RowsPerPage, long auditUserId, string auditWorkstation, bool renewalCards = false, bool activationSearch = false)
        {
            try
            {
                var result = _cardManService.SearchCustomerCardsList(issuerid, branchid, productid, cardissuemethodid, priorityid, cardrefno, customeraccountno, pageIndex, RowsPerPage, auditUserId, auditWorkstation, renewalCards, activationSearch);
                if (renewalCards)
                {
                    result = result.Where(p => p.renewal_detail_id != null).ToList();
                }
                else
                {
                    result = result.Where(p => p.renewal_detail_id == null).ToList();
                }

                return new Response<List<CustomercardsearchResult>>(result,
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CustomercardsearchResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Fetch all cards that are work in progress for the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<CardSearchResult>> GetOperatorCardsInProgress(long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<CardSearchResult>>(_cardManService.GetOperatorCardsInProgress(userId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CardSearchResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        internal Response<List<HybridRequestResult>> GetOperatorHybridRequestsInProgress(int? statusId, long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<HybridRequestResult>>(_cardManService.GetOperatorHybridRequestsInProgress(statusId, userId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<HybridRequestResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Fetch all cards that have an exception status.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<CardSearchResult>> GetCardsInError(long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<CardSearchResult>>(_cardManService.GetCardsInError(userId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CardSearchResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Fetch all cards waiting to be linked to CMS.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<CardSearchResult>> SearchForReissueCards(long userId, int pageIndex, int rowsPerPage, long audit_user_id, string auditWorkstation)
        {
            try
            {
                return new Response<List<CardSearchResult>>(_cardManService.SearchForReissueCards(userId, pageIndex, rowsPerPage, audit_user_id, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CardSearchResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Fetch a list of Branch card codes.
        /// </summary>
        /// <param name="BranchCardCodeType"></param>
        /// <param name="isException"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<branch_card_codes>> ListBranchCardCodes(int BranchCardCodeType, bool isException, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<branch_card_codes>>(_cardManService.ListBranchCardCodes(BranchCardCodeType, isException, auditUserId, auditWorkstation),
                                                             ResponseType.SUCCESSFUL,
                                                             "",
                                                             "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<branch_card_codes>>(null,
                                                             ResponseType.ERROR,
                                                             "Error when processing request.",
                                                             log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Used to approve or reject maker checker.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="approved"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse MakerChecker(long cardId, Boolean approved, string notes, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.MakerChecker(cardId, approved, notes, language, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse RequestMakerChecker(long requestId, Boolean approved, string notes, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.RequestMakerChecker(requestId, approved, notes, language, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        /// <summary>
        /// Spoil a branch card.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse SpoilBranchCard(long cardId, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.SpoilBranchCard(cardId, language, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Issues a card to a customer. This just links the card to a customer in Indigo. Not on the CMS
        /// </summary>
        /// <param name="customerAccount"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse IssueCardToCustomer(CustomerDetails customerAccount, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;

                log.Debug(d => d("CBSAccountType :" + customerAccount.CBSAccountType));
                //Check that there is suffecient balance
                if (!CheckAccountBalance(customerAccount, languageId, auditUserId, auditWorkstation, out responseMessage))
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);

                //Validate Details in CMS
                IntegrationController _integration = IntegrationController.Instance;
                //Veneka.Indigo.Integration.Config.IConfig config;
                Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;
                Veneka.Indigo.Integration.Config.IConfig config;
                _integration.CardManagementSystem(customerAccount.ProductId, InterfaceArea.ISSUING, out externalFields, out config);
                InterfaceInfo interfaceInfo = new InterfaceInfo
                {
                    Config = config,
                    InterfaceGuid = config.InterfaceGuid.ToString()
                };

                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };
                log.Trace("before:COMSController.ComsCore.ValidateCustomerDetails(customerAccount, externalFields, interfaceInfo, auditInfo);");
               // var response = COMSController.ComsCore.ValidateCustomerDetails(customerAccount, externalFields, interfaceInfo, auditInfo);
                log.Trace("after:COMSController.ComsCore.ValidateCustomerDetails(customerAccount, externalFields, interfaceInfo, auditInfo);");
              //  if (response.ResponseCode != 0)
              //     return new BaseResponse(ResponseType.UNSUCCESSFUL, response.ResponseMessage, response.ResponseException);

                //if (!_integration.CardManagementSystem(customerAccount.ProductId, InterfaceArea.ISSUING, out externalFields, out config).ValidateCustomerDetails(customerAccount, externalFields, config, languageId, auditUserId, auditWorkstation, out responseMessage))
                //    return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, "");

                //Now link customer account to the card to contunue issuing.

                //calculate charge -- Takunda
                ProductResult _productresult = _cardManService.GetProduct(customerAccount.ProductId, auditUserId, auditWorkstation);
              

                if ((bool)_productresult.Product.charge_fee_at_cardrequest)// checking charge fee at cardrequest flag is enabled. 
                {
                    FeeChargeLogic feelogic = new FeeChargeLogic(_cardManService, _comsCoreInstance, _integration);
                    BaseResponse feeresponse = feelogic.FeeCharge(customerAccount, auditInfo);
                    if (feeresponse.ResponseType != ResponseType.SUCCESSFUL)
                        return new Response<long>(0, ResponseType.UNSUCCESSFUL, feeresponse.ResponseMessage, feeresponse.ResponseMessage);

                }


                log.Trace("before:IssueCardToCustomer");
                if (_cardManService.IssueCardToCustomer(customerAccount, languageId, auditUserId, auditWorkstation, out responseMessage))
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                log.Trace("after:IssueCardToCustomer");

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse createPinRequest(PinObject PinDetails, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;

                log.Debug(d => d("Pin Request with reference:" + PinDetails.PinRequestReference));
            

                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };
              
              log.Trace("before:Processing Pin Request");
                if (_cardManService.createPinRequest(PinDetails, languageId, auditUserId, auditWorkstation, out responseMessage))
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                log.Trace("after:IssueCardToCustomer");

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse updatePinRequestStatus(PinObject PinDetails, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;

                log.Debug(d => d("Update on Pin Request with reference:" + PinDetails.PinRequestReference));


                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };


                //calculate charge -- Takunda
                //  ProductResult _productresult = _cardManService.GetProduct(customerAccount.ProductId, auditUserId, auditWorkstation);


                log.Trace("before:Processing Pin Request");
                if (_cardManService.updatePinRequestStatus(PinDetails, languageId, auditUserId, auditWorkstation, out responseMessage))
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                log.Trace("after:Status updated");

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        // UpdatePinRequestStatusForReissue
        internal BaseResponse UpdatePinRequestStatusForReissue(PinObject PinDetails, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;

                log.Debug(d => d("Update on Pin Request with reference:" + PinDetails.PinRequestReference));


                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };


                log.Trace("before:Processing Pin ReIssue Request");
                if (_cardManService.UpdatePinRequestStatusForReissue(PinDetails, languageId, auditUserId, auditWorkstation, out responseMessage))
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                log.Trace("after:Status updated");

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        internal BaseResponse updatePinFileStatus(TerminalCardData PinBlock, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;

                log.Debug(d => d("Changing status for pinblock :" + PinBlock.PINBlock));


                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };

                log.Trace("before:Processing Pin Request");
                if (_cardManService.updatePinFileStatus(PinBlock, languageId, auditUserId, auditWorkstation, out responseMessage))
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                log.Trace("after:Status updated");

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        //updateBatchFileStatus
        internal BaseResponse updateBatchFileStatus(TerminalCardData PinBlock, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;

                log.Debug(d => d("Changing status for pinbactch :" + PinBlock.header_batch_reference));


                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };

                log.Trace("before:Processing Pin Request");
                if (_cardManService.updateBatchFileStatus(PinBlock, languageId, auditUserId, auditWorkstation, out responseMessage))
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                log.Trace("after:Status updated");

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse deletePinBlock(string product_pan_bin_code, string pan_last_four, string expiry_date, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;

                log.Debug(d => d("Deleting block:"));


                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };


                log.Trace("before:Deleting pin block on first 0 resend");
                if (_cardManService.deletePinBlock(product_pan_bin_code,pan_last_four, expiry_date, languageId, auditUserId, auditWorkstation, out responseMessage))
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                log.Trace("after:Status updated");

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse CreateRestParams(RestWebservicesHandler restDetails, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;

                log.Debug(d => d("Rest request with reference:" + restDetails.rest_url));


                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };


                //calculate charge -- Takunda
                //  ProductResult _productresult = _cardManService.GetProduct(customerAccount.ProductId, auditUserId, auditWorkstation);


                log.Trace("before:Processing rest request");
                if (_cardManService.CreateRestParams(restDetails, languageId, auditUserId, auditWorkstation, out responseMessage))
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");

                log.Trace("after:process request -  should not get here else bug");

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse UpdateRestParams(RestWebservicesHandler restDetails, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;

                log.Debug(d => d("Updating Rest request with reference:" + restDetails.rest_url));


                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };


                //calculate charge -- Takunda
                //  ProductResult _productresult = _cardManService.GetProduct(customerAccount.ProductId, auditUserId, auditWorkstation);


                log.Trace("before:Processing rest update");
                if (_cardManService.UpdateRestParams(restDetails, languageId, auditUserId, auditWorkstation, out responseMessage))
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");

                log.Trace("after:rest u[date request -  should not get here else bug");

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Upload PIN to Indigo an mark it as having pin captured.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="language"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        internal BaseResponse IssueCardPinCaptured(long cardId, long? pinAuthUserId, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.IssueCardPinCaptured(cardId, pinAuthUserId, language, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Mark card as PRINTED
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse IssueCardPrinted(long cardId, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.IssueCardPrinted(cardId, language, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Mark card as PRINT_ERROR
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse IssueCardPrintError(long cardId, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.IssueCardPrintError(cardId, language, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Mark card as Spoilt
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse IssueCardSpoil(long cardId, int spoilResaonId, string spoilComments, int languageId, long auditUserId, string auditWorkstation)
        {
            string responseMessage = String.Empty;
            IntegrationController _integration = IntegrationController.Instance;
            CustomerDetails customerDetails = null;

            try
            {
                bool instantPinYN;
                bool status;
                //Get card and customer details
                customerDetails = ExtractCustomerDetails(_cardManService.GetCardDetails(cardId, false, languageId, auditUserId, auditWorkstation), out instantPinYN);
                Veneka.Indigo.Integration.Config.IConfig config;
                Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;

                _integration.CardManagementSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out externalFields, out config);

                //Spoilt the card IN CMS, if successful mark card as spoilt in Indigo
                //ICardManagementSystem cardmangemnt = _integration.CardManagementSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out externalFields, out config);
                if (config != null)
                {
                    //_integration.CardManagementSystem(productId, InterfaceArea.ISSUING, out externalFields, out config);
                    InterfaceInfo interfaceInfo = new InterfaceInfo
                    {
                        Config = config,
                        InterfaceGuid = config.InterfaceGuid.ToString()
                    };

                    AuditInfo auditInfo = new AuditInfo
                    {
                        AuditUserId = auditUserId,
                        AuditWorkStation = auditWorkstation,
                        LanguageId = languageId
                    };

                    var response = COMSController.ComsCore.SpoilCard(customerDetails, externalFields, interfaceInfo, auditInfo);

                    responseMessage = response.ResponseMessage;

                    if (response.ResponseCode == 0)
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }
                }
                else
                {
                    status = true;
                }
                if (status)
                    if (_cardManService.IssueCardSpoil(cardId, spoilResaonId, spoilComments, languageId, auditUserId, auditWorkstation, out responseMessage))
                        return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");


                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        ex.Message,
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Mark card as ISSUED
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse IssueCardComplete(long cardId, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.IssueCardComplete(cardId, language, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        ///// <summary>
        ///// If customer detail is linked to a card it will be returned.
        ///// </summary>
        ///// <param name="cardId"></param>
        ///// <param name="auditUserId"></param>
        ///// <param name="auditWorkstation"></param>
        ///// <returns></returns>
        //internal Response<List<CustomerAccountResult>> GetCustomerAccDetailForCard(long cardId, long auditUserId, string auditWorkstation)
        //{
        //    try
        //    {
        //        return new Response<List<CustomerAccountResult>>(_cardManService.GetCustomerAccDetailForCard(cardId, auditUserId, auditWorkstation),
        //                                                             ResponseType.SUCCESSFUL,
        //                                                             "",
        //                                                             "");

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        return new Response<List<CustomerAccountResult>>(null,
        //                                                             ResponseType.ERROR,
        //                                                             "Error when processing request.",
        //                                                             log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
        //    }
        //}

        /// <summary>
        /// Get card detail, load batch detail, dist batch detail and customer account detail for a card.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<CardDetails> GetCardDetails(long cardId, byte[] index, int languageId, long auditUserId, string auditWorkstation)
        {
            Response<CardDetails> response;
            try
            {
                var result = _cardManService.GetCardDetails(cardId, true, languageId, auditUserId, auditWorkstation);
                if (index != null && TerminalManagementController.FetchIndex(index) != null)
                    result.pin_selected = true;


                return new Response<CardDetails>(result,
                                                          ResponseType.SUCCESSFUL,
                                                          "",
                                                          "");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<CardDetails>(null,
                                                          ResponseType.ERROR,
                                                          "Error when processing request.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

            return response;
        }

        internal Response<RequestDetails> GetRequestDetails(long requestId, int languageId, long auditUserId, string auditWorkstation)
        {
            Response<RequestDetails> response;
            try
            {
                var result = _cardManService.GetRequestDetails(requestId, true, languageId, auditUserId, auditWorkstation);


                return new Response<RequestDetails>(result,
                                                          ResponseType.SUCCESSFUL,
                                                          "",
                                                          "");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<RequestDetails>(null,
                                                          ResponseType.ERROR,
                                                          "Error when processing request.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

            return response;
        }

        /// <summary>
        /// Search cards currently at a branch.
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="cardNumber"> May be null</param>
        /// <param name="branchCardStatus">May be null</param>
        /// <param name="operatorUserId">May be null</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<CardSearchResult>> SearchBranchCards(int? issuerId, int? branchId, int? userRoleId, int? productId, int? priorityId, int? cardIssueMethodId, string cardNumber, int? branchCardStatusId,
                                                                    long? operatorUserId, int pageIndex, int rowsPerpPage, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<CardSearchResult>>(_cardManService.SearchBranchCards(issuerId, branchId, userRoleId, productId, priorityId, cardIssueMethodId,
                                                                                                    cardNumber, branchCardStatusId, operatorUserId,
                                                                                                    pageIndex, rowsPerpPage, languageId,
                                                                                                    auditUserId, auditWorkstation),
                                                                   ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (BaseIndigoException biex)
            {
                var responseMessage = _translator.TranslateResponseCode(biex.SystemCode, 0, languageId, auditUserId, auditWorkstation);
                return new Response<List<CardSearchResult>>(null,
                                                                   ResponseType.UNSUCCESSFUL,
                                                                   responseMessage,
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? biex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CardSearchResult>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Persist checked in and out cards for an operator to the DB.
        /// </summary>
        /// <param name="operatorUserId"></param>
        /// <param name="checkedOutCards"></param>
        /// <param name="checkedInCards"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public Response<List<SearchBranchCardsResult>> CheckInOutCards(long operatorUserId, int branchId, int productId, List<long> checkedOutCards, List<long> checkedInCards, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<SearchBranchCardsResult>>(_cardManService.CheckInOutCards(operatorUserId, branchId, productId, checkedOutCards, checkedInCards, auditUserId, auditWorkstation),
                                                                   ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<SearchBranchCardsResult>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Call Flexcube webservice to verify account details.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="accountNumber"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public Response<AccountDetails> GetAccountDetail(int issuerId, int productId, int cardIssueReasonId, int branchId, string accountNumber, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };
                IntegrationController _integration = IntegrationController.Instance;
                var accountLookupLogic = new Logic.AccountLookupLogic(_cardManService, _comsCoreInstance, _integration);

                // Do CBS Lookup
                var cbsResponse = accountLookupLogic.CoreBankingAccountLookup(issuerId, productId, cardIssueReasonId, branchId, accountNumber, auditInfo);

                if (cbsResponse.ResponseType == ResponseType.SUCCESSFUL)
                {
                    string responseMessage;
                    // DO CMS Lookup
                    if (accountLookupLogic.CardManagementAccountLookup(issuerId, productId, cardIssueReasonId, accountNumber, auditInfo, cbsResponse.Value, out responseMessage))
                    {
                        // Validate returned account
                        if (accountLookupLogic.ValidateAccount(productId, cbsResponse.Value, out responseMessage))
                        {
                            return cbsResponse;
                        }
                    }

                    return new Response<AccountDetails>(null, ResponseType.UNSUCCESSFUL, responseMessage, String.Empty);
                }

                return cbsResponse;
            }
            catch (BaseIndigoException iex)
            {
                log.Error(iex);
                return new Response<AccountDetails>(null,
                                                     ResponseType.UNSUCCESSFUL,
                                                     iex.Message,
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? iex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<AccountDetails>(null,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }




        //public BaseResponse PINReissue(int issuerId, int branchId, int productId, long? authoriseUserId, byte[] index, string sessionKey, int languageId, long auditUserId, string auditWorkstation)
        //{
        //    string responseMessage = String.Empty;
        //    IntegrationController _integration = IntegrationController.Instance;
        //    string PAN = String.Empty;

        //    try
        //    {
        //        var decryptedIndex = EncryptionManager.DecryptData(index, StaticFields.PIN_SECURITY_KEY);

        //        var cardAndPin = TerminalManagementController.FetchIndex(decryptedIndex);

        //        //PAN = EncryptionManager.DecryptDataToString(cardAndPin.Item1, sessionKey);
        //        string PVV = EncryptionManager.DecryptDataToString(cardAndPin.Item2, StaticFields.PIN_SECURITY_KEY);
        //        Veneka.Indigo.Integration.Config.IConfig config;
        //        Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;
        //        PAN = cardAndPin.Item1.PAN;
        //        if (_integration.CardManagementSystem(productId, InterfaceArea.ISSUING, out externalFields, out config).UpdatePVV(issuerId, productId, cardAndPin.Item1, PVV, externalFields, config, languageId, auditUserId, auditWorkstation, out responseMessage))
        //        {
        //            _cardManService.PINReissue(issuerId, branchId, productId, PAN, authoriseUserId, false, "", auditUserId, auditWorkstation);

        //            return new BaseResponse(ResponseType.SUCCESSFUL, "Successful", "");
        //        }

        //        _cardManService.PINReissue(issuerId, branchId, productId, PAN, authoriseUserId, true, responseMessage, auditUserId, auditWorkstation);
        //        return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
        //    }
        //    catch (BaseIndigoException iex)
        //    {
        //        _cardManService.PINReissue(issuerId, branchId, productId, PAN, authoriseUserId, true, iex.Message, auditUserId, auditWorkstation);
        //        return new BaseResponse(ResponseType.UNSUCCESSFUL,
        //                                iex.Message,
        //                                log.IsDebugEnabled || log.IsTraceEnabled ? iex.ToString() : "");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        _cardManService.PINReissue(issuerId, branchId, productId, PAN, authoriseUserId, true, ex.Message, auditUserId, auditWorkstation);
        //        responseMessage = _translator.TranslateResponseCode(SystemResponseCode.GENERAL_FAILURE, 0, languageId, auditUserId, auditWorkstation);
        //        return new BaseResponse(ResponseType.ERROR,
        //                                responseMessage,
        //                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
        //    }
        //    finally
        //    {
        //        PAN = String.Empty;
        //    }
        //}w

        public Response<List<PinReissueWSResult>> PINReissueSearch(int? issuerId, int? branchId, int? userRolesId, int? pinReissueStatusesId, int? pin_reissue_type_id, long? operatorUserId, bool operatorInProgress, long? authoriseUserId, byte[] index, DateTime? dateFrom, DateTime? dateTo, int languageId, int pageIndex, int rowsPerpage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<PinReissueWSResult>>(PinReissueWSResult.Create(_cardManService.PINReissueSearch(issuerId, branchId, userRolesId, pinReissueStatusesId, pin_reissue_type_id, operatorUserId, operatorInProgress, authoriseUserId, dateFrom, dateTo, languageId, pageIndex, rowsPerpage, auditUserId, auditWorkstation)),
                                            ResponseType.SUCCESSFUL,
                                            "",
                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<PinReissueWSResult>>(null,
                                            ResponseType.ERROR,
                                            "Error when processing request.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        public Response<PinReissueWSResult> GetPINReissue(long pinReissueId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<PinReissueWSResult>(PinReissueWSResult.Create(_cardManService.GetPINReissue(pinReissueId, languageId, auditUserId, auditWorkstation)),
                                            ResponseType.SUCCESSFUL,
                                            "",
                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<PinReissueWSResult>(null,
                                            ResponseType.ERROR,
                                            "Error when processing request.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        //public Response<PinReissueResult> RequestPINReissue(int issuerId, int branchId, int productId, byte[] index, int languageId, long auditUserId, string auditWorkstation)
        //{
        //    try
        //    {
        //        var decryptedIndex = EncryptionManager.DecryptData(index, StaticFields.PIN_SECURITY_KEY);

        //        var cardAndPin = TerminalManagementController.FetchIndex(decryptedIndex);

        //        if (cardAndPin != null)
        //        {
        //            String PAN = EncryptionManager.DecryptDataToString(cardAndPin.Item1, StaticFields.PIN_SECURITY_KEY);

        //            return new Response<PinReissueResult>(_cardManService.RequestPINReissue(issuerId, branchId, productId, PAN, index, languageId, auditUserId, auditWorkstation),
        //                                        ResponseType.SUCCESSFUL,
        //                                        "",
        //                                        "");
        //        }
        //        else
        //        {
        //            return new Response<PinReissueResult>(null,
        //                                        ResponseType.UNSUCCESSFUL,
        //                                        "PIN reset request has expired, please restart PIN reset procedure.",
        //                                        "PIN reset request has expired, please restart PIN reset procedure.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        return new Response<PinReissueResult>(null,
        //                                    ResponseType.ERROR,
        //                                    "Error when processing request.",
        //                                    log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
        //    }
        //}


        public Response<PinReissueWSResult> ApprovePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                PinReissueResult result = null;
                string responseMessage = null;
                if (_cardManService.ApprovePINReissue(pinReissueId, notes, languageId, auditUserId, auditWorkstation, out result, out responseMessage))
                {
                    return new Response<PinReissueWSResult>(PinReissueWSResult.Create(result),
                                            ResponseType.SUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
                else
                {
                    return new Response<PinReissueWSResult>(null,
                                            ResponseType.UNSUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<PinReissueWSResult>(null,
                                            ResponseType.ERROR,
                                            "Error when processing request.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        public Response<PinReissueWSResult> RejectPINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                PinReissueResult result = null;
                string responseMessage = null;
                if (_cardManService.RejectPINReissue(pinReissueId, notes, languageId, auditUserId, auditWorkstation, out result, out responseMessage))
                {
                    return new Response<PinReissueWSResult>(PinReissueWSResult.Create(result),
                                            ResponseType.SUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
                else
                {
                    return new Response<PinReissueWSResult>(null,
                                            ResponseType.UNSUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<PinReissueWSResult>(null,
                                            ResponseType.ERROR,
                                            "Error when processing request.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        public Response<PinReissueWSResult> CancelPINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var resp = GetPINReissue(pinReissueId, languageId, auditUserId, auditWorkstation);

                if (resp.ResponseType != ResponseType.SUCCESSFUL)
                    return resp;
                PinReissueResult result = null;
                string responseMessage = null;

                if (_cardManService.CancelPINReissue(pinReissueId, notes, languageId, auditUserId, auditWorkstation, out result, out responseMessage))
                {
                    var decryptedIndex = EncryptionManager.DecryptData(resp.Value.primary_index_number, StaticFields.PIN_SECURITY_KEY);

                    var cardAndPin = TerminalManagementController.FetchIndex(decryptedIndex);

                    if (cardAndPin != null)
                    {
                        TerminalManagementController.Remove(resp.Value.primary_index_number);
                        log.Debug("clear cache");
                    }

                    return new Response<PinReissueWSResult>(PinReissueWSResult.Create(result),
                                            ResponseType.SUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
                else
                {
                    return new Response<PinReissueWSResult>(null,
                                            ResponseType.UNSUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<PinReissueWSResult>(null,
                                            ResponseType.ERROR,
                                            "Error when processing request.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        public Response<PinReissueWSResult> ExpirePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                PinReissueResult result = null;
                string responseMessage = null;
                if (_cardManService.ExpirePINReissue(pinReissueId, notes, languageId, auditUserId, auditWorkstation, out result, out responseMessage))
                {
                    return new Response<PinReissueWSResult>(PinReissueWSResult.Create(result),
                                            ResponseType.SUCCESSFUL,
                                            "",
                                            "");
                }
                else
                {
                    return new Response<PinReissueWSResult>(null,
                                            ResponseType.UNSUCCESSFUL,
                                            responseMessage,
                                            responseMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<PinReissueWSResult>(null,
                                            ResponseType.ERROR,
                                            "Error when processing request.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        public Response<PinReissueWSResult> CompletePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            //Lookup pin reissue request
            var resp = GetPINReissue(pinReissueId, languageId, auditUserId, auditWorkstation);

            if (resp.ResponseType != ResponseType.SUCCESSFUL)
                return resp;


            return this.CompletePINReissue(pinReissueId, notes, resp.Value.issuer_id, resp.Value.product_id, resp.Value.primary_index_number, languageId, auditUserId, auditWorkstation);
        }

        public Response<PinReissueWSResult> CompletePINReissue(long pinReissueId, string notes, int issuerId, int productId, byte[] index, int languageId, long auditUserId, string auditWorkstation)
        {
            string responseMessage = String.Empty;
            IntegrationController _integration = IntegrationController.Instance;
            PinReissueResult result = null;
            string PAN = String.Empty;

            try
            {

                var decryptedIndex = EncryptionManager.DecryptData(index, StaticFields.PIN_SECURITY_KEY);

                var cardAndPin = TerminalManagementController.FetchIndex(decryptedIndex);

                if (cardAndPin == null)
                {
                    _cardManService.ExpirePINReissue(pinReissueId, "Expired", languageId, auditUserId, auditWorkstation, out result, out responseMessage);
                    return new Response<PinReissueWSResult>(PinReissueWSResult.Create(result), ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
                }

                //PAN = EncryptionManager.DecryptDataToString(cardAndPin.Item1, StaticFields.PIN_SECURITY_KEY);
                string PVV = EncryptionManager.DecryptDataToString(cardAndPin.Item2, StaticFields.PIN_SECURITY_KEY);
                Veneka.Indigo.Integration.Config.IConfig config;
                Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;

                _integration.CardManagementSystem(productId, InterfaceArea.ISSUING, out externalFields, out config);

                InterfaceInfo interfaceInfo = new InterfaceInfo
                {
                    Config = config,
                    InterfaceGuid = config.InterfaceGuid.ToString()
                };

                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };


                var response = COMSController.ComsCore.UpdatePVV(issuerId, productId, cardAndPin.Item1, PVV, externalFields, interfaceInfo, auditInfo);

                //if (_integration.CardManagementSystem(productId, InterfaceArea.ISSUING, out externalFields, out config).UpdatePVV(issuerId, productId, cardAndPin.Item1, PVV, externalFields, config, languageId, auditUserId, auditWorkstation, out responseMessage))
                if (response.ResponseCode == 0)
                {
                    //_cardManService.PINReissue(issuerId, branchId, productId, PAN, authoriseUserId, false, "", auditUserId, auditWorkstation);
                    _cardManService.CompletePINReissue(pinReissueId, notes, languageId, auditUserId, auditWorkstation, out result, out responseMessage);
                    return new Response<PinReissueWSResult>(PinReissueWSResult.Create(result), ResponseType.SUCCESSFUL, "Successful", "");
                }

                //_cardManService.PINReissue(issuerId, branchId, productId, PAN, authoriseUserId, true, responseMessage, auditUserId, auditWorkstation);
                return new Response<PinReissueWSResult>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (BaseIndigoException iex)
            {
                //_cardManService.PINReissue(issuerId, branchId, productId, PAN, authoriseUserId, true, iex.Message, auditUserId, auditWorkstation);
                return new Response<PinReissueWSResult>(null,
                                                        ResponseType.UNSUCCESSFUL,
                                                        iex.Message,
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? iex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                //_cardManService.PINReissue(issuerId, branchId, productId, PAN, authoriseUserId, true, ex.Message, auditUserId, auditWorkstation);
                responseMessage = _translator.TranslateResponseCode(SystemResponseCode.GENERAL_FAILURE, 0, languageId, auditUserId, auditWorkstation);
                return new Response<PinReissueWSResult>(null, ResponseType.ERROR,
                                        responseMessage,
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Call to Core Banking integration to link the card to a customer/account.
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditworkstation"></param>
        /// <returns></returns>
        internal BaseResponse LinkCardToCustomer(long cardId, byte[] index, int languageId, long auditUserId, string auditWorkstation)
        {
            string responseMessage = String.Empty;
            IntegrationController _integration = IntegrationController.Instance;
            CustomerDetails customerDetails = null;
            Veneka.Indigo.Integration.Config.IConfig config;
            Veneka.Indigo.Integration.Config.IConfig coreconfig;
            Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;
            Veneka.Indigo.Integration.External.ExternalSystemFields CoreexternalFields;
            bool centerActivation = false;

            try //Validation checks: this try catch will not put card into a failed state
            {
                bool instantPinYN;
                //Get card and customer details
                customerDetails = ExtractCustomerDetails(_cardManService.GetCardDetails(cardId, false, languageId, auditUserId, auditWorkstation), out instantPinYN);

                var product = _cardManService.GetProduct(customerDetails.ProductId, auditUserId, auditWorkstation);
                centerActivation = product.Product.activation_by_center_operator.GetValueOrDefault();

                if (centerActivation == false || (centerActivation == true && customerDetails.BranchCardStatusesId != 6))
                {
                    //Check account Balance is the card has not already been issued
                   // ProductResult _productresult = _cardManService.GetProduct(customerDetails.ProductId, auditUserId, auditWorkstation);


                    if (!(bool)product.Product.charge_fee_at_cardrequest)// checking charge fee at cardrequest flag is enabled. 
                    {
                        if (!CheckAccountBalance(customerDetails, languageId, auditUserId, auditWorkstation, out responseMessage))
                            return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
                    }

                    
                }

                //Do we have instant pin on?
                if (instantPinYN)
                {
                    log.Trace(t => t("Instant PIN enabled, check that a PIN has been captured"));
                    //Check if the pvv was set on the table, or should we get it out of secure memory
                    if (String.IsNullOrWhiteSpace(customerDetails.PinOffset))
                        if (index != null)
                        {
                            var panAndPin = TerminalManagementController.FetchIndex(EncryptionManager.DecryptData(index, StaticFields.PIN_SECURITY_KEY));

                            if (panAndPin == null) throw new Exception("PIN is empty, please have customer select PIN.");

                            customerDetails.PinOffset = EncryptionManager.DecryptDataToString(panAndPin.Item2, StaticFields.PIN_SECURITY_KEY);
                        }
                }
                else
                {
                    log.Trace(t => t("Instant PIN not enabled."));
                    customerDetails.PinOffset = String.Empty;
                }
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        ex.Message,
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

            try
            {
                log.Trace(t => t("Link card on CMS."));

                //STEP 2: Link the Card
                _integration.CardManagementSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out externalFields, out config);

                _integration.CoreBankingSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out CoreexternalFields, out coreconfig);

                InterfaceInfo interfaceInfo = new InterfaceInfo
                {
                    Config = config,
                    InterfaceGuid = config.InterfaceGuid.ToString()
                };

                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };


                ComsResponse<string> linkResp = new ComsResponse<string>()
                {
                    ResponseCode = -99,
                    ResponseException = "No action taken",
                    ResponseMessage = "No action taken",
                    Value = string.Empty
                };
                if (centerActivation == false)
                {
                    linkResp = COMSController.ComsCore.LinkCardToAccountAndActive(customerDetails, externalFields, interfaceInfo, auditInfo);
                }
                else
                {
                    if (customerDetails.BranchCardStatusesId == 6)
                    {
                        linkResp = COMSController.ComsCore.LinkCardToAccountAndActive(customerDetails, externalFields, interfaceInfo, auditInfo);
                        //note the card has been activated
                        _cardManService.ActivateCard(cardId, auditUserId, auditWorkstation);
                    }
                    else
                    {
                        linkResp = new ComsResponse<string>()
                        {
                            ResponseCode = 0,
                            ResponseException = "No action taken",
                            ResponseMessage = "No action taken",
                            Value = string.Empty
                        };
                    }
                }
                //var linkResp = _integration.CardManagementSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out externalFields, out config).LinkCardToAccount(customerDetails, externalFields, config, languageId, auditUserId, auditWorkstation, out responseMessage);
                if (linkResp.ResponseCode == 0)
                {
                    if (centerActivation == false || (centerActivation == true && customerDetails.BranchCardStatusesId != 6))
                    {
                        _cardManService.IssueCardLinkSuccess(customerDetails.CardId, auditUserId, auditWorkstation);


                        log.Trace(t => t("Card successfully linked on CMS."));
                        interfaceInfo = new InterfaceInfo
                        {
                            Config = coreconfig,
                            InterfaceGuid = coreconfig.InterfaceGuid.ToString()
                        };
                        var response = COMS.COMSController.ComsCore.UpdateAccount(customerDetails, CoreexternalFields, interfaceInfo, auditInfo);
                        if (response.ResponseCode == 0)
                        {
                            log.Trace(t => t("Updating account in CBS successful."));
                            if (centerActivation == true)
                            {
                                responseMessage = "SUCCESSFUL_ISSUE";
                            }
                        }
                        else
                        {

                            log.Trace(t => t(response.ResponseMessage));
                            responseMessage = string.Empty;
                            log.Trace(t => t("Updating account in CBS failed."));
                            //return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
                        }
                    }
                    else
                    {
                        return new BaseResponse(ResponseType.SUCCESSFUL, "SUCCESSFUL_ACTIVATION", "SUCCESSFUL_ACTIVATION");
                    }

                }
                else if (linkResp.ResponseCode == 15)
                {
                    //set card status to retry
                    _cardManService.IssueCardLinkRetry(customerDetails.CardId, linkResp.ResponseMessage, auditUserId, auditWorkstation);
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, linkResp.ResponseMessage, linkResp.ResponseMessage);
                }
                else if (linkResp.ResponseCode == 9)
                {
                    //set card status to cms error
                    log.Trace(t => t("CMS Error message in here, please retry .... "));
                    linkResp.ResponseMessage = "Failed to upload to CMS";
                    _cardManService.IssueCardLinkFail(customerDetails.CardId, linkResp.ResponseMessage, auditUserId, auditWorkstation);
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, linkResp.ResponseMessage, linkResp.ResponseMessage);
                }
                else //Something went wrong linking the card, but its safe to leave it in current status
                {
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, linkResp.ResponseMessage, linkResp.ResponseMessage);
                }
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                log.Trace(t => t("CMS Error Exception one in here, please retry .... "));
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (BaseIndigoException iex)
            {
                log.Warn(iex);
                log.Trace(t => t("CMS Error Exception two in here, please retry .... "));
                _cardManService.IssueCardLinkFail(customerDetails.CardId, iex.ToString(), auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        iex.Message,
                                        log.IsDebugEnabled || log.IsTraceEnabled ? iex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                log.Trace(t => t("CMS Error Exception three in here, please retry .... "));
                _cardManService.IssueCardLinkFail(customerDetails.CardId, ex.ToString(), auditUserId, auditWorkstation);
                responseMessage = _translator.TranslateResponseCode(SystemResponseCode.GENERAL_FAILURE, 0, languageId, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.ERROR,
                                        responseMessage,
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }


            try
            {
                string feeResponse = String.Empty;
                IConfig feeconfig;
                ExternalSystemFields feeexternalFields;

                _integration.FeeSchemeInterface(customerDetails.ProductId, InterfaceArea.ISSUING, out feeexternalFields, out feeconfig);
                if (feeconfig != null)
                {
                    log.Debug("Found Fee Scheme Configuration");
                    coreconfig = feeconfig;
                    CoreexternalFields = feeexternalFields;
                }
                //STEP 3: Charge Fee if it's greater than 0 and has not already been charged for.
                ProductResult product_info = _cardManService.GetProduct(customerDetails.ProductId, auditUserId, auditWorkstation);

                if (customerDetails.FeeCharge != null && customerDetails.FeeCharge.Value > 0 && String.IsNullOrWhiteSpace(customerDetails.FeeReferenceNumber)
                  && !(bool)product_info.Product.charge_fee_at_cardrequest)
                {
                    log.Trace(t => t("Charge the fee."));
                    string feeReferenceNumber = string.Empty;

                    InterfaceInfo interfaceInfo = new InterfaceInfo
                    {
                        Config = coreconfig,
                        InterfaceGuid = coreconfig.InterfaceGuid.ToString()

                    };


                    AuditInfo auditInfo = new AuditInfo
                    {
                        AuditUserId = auditUserId,
                        AuditWorkStation = auditWorkstation,
                        LanguageId = languageId
                    };
                    var response = COMS.COMSController.ComsCore.ChargeFee(customerDetails, CoreexternalFields, interfaceInfo, auditInfo);
                    if (response.ResponseCode != 0)
                    {
                        _cardManService.UpdateFeeChargeStatus(customerDetails.CardId, 2, null, null, auditUserId, auditWorkstation);
                        responseMessage = String.Format("Fee not debited ({0})", response.ResponseMessage);
                        //return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
                    }
                    else
                        _cardManService.UpdateFeeChargeStatus(customerDetails.CardId, 1, customerDetails.FeeReferenceNumber, null, auditUserId, auditWorkstation);
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(customerDetails.FeeReferenceNumber))
                        if (log.IsDebugEnabled)
                            log.DebugFormat("Fee already charged: Ref{0}", customerDetails.FeeReferenceNumber);
                        else if((bool)product_info.Product.charge_fee_at_cardrequest)
                            log.Trace(t=>t("Fee charged already with a product charge fee marker"));
                            else
                            log.Trace(t => t("Fee already charged."));
                }
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        ex.Message,
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

            return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, responseMessage);
        }


        internal BaseResponse ActiveCard(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            string responseMessage = String.Empty;
            IntegrationController _integration = IntegrationController.Instance;
            CustomerDetails customerDetails = null;
            Veneka.Indigo.Integration.Config.IConfig config;
            Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;

            try
            {
                log.Trace(t => t("Link card on CMS."));

                //STEP 2: Link the Card
                _integration.CardManagementSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out externalFields, out config);

                InterfaceInfo interfaceInfo = new InterfaceInfo
                {
                    Config = config,
                    InterfaceGuid = config.InterfaceGuid.ToString()
                };

                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };


                var linkResp = COMSController.ComsCore.ActiveCard(customerDetails, externalFields, interfaceInfo, auditInfo);

                //var linkResp = _integration.CardManagementSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out externalFields, out config).LinkCardToAccount(customerDetails, externalFields, config, languageId, auditUserId, auditWorkstation, out responseMessage);

                if (linkResp.ResponseCode == 0)
                {
                    _cardManService.IssueCardLinkSuccess(customerDetails.CardId, auditUserId, auditWorkstation);


                    log.Trace(t => t("Card successfully linked on CMS."));
                    _integration.CoreBankingSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out externalFields, out config);
                    interfaceInfo = new InterfaceInfo
                    {
                        Config = config,
                        InterfaceGuid = config.InterfaceGuid.ToString()

                    };
                    var response = COMS.COMSController.ComsCore.UpdateAccount(customerDetails, externalFields, interfaceInfo, auditInfo);
                    if (response.ResponseCode == 0)
                    {
                        log.Trace(t => t("Updating account in CBS successful."));
                    }
                    else
                    {

                        log.Trace(t => t(responseMessage));
                        responseMessage = string.Empty;
                        log.Trace(t => t("Updating account in CBS failed."));
                        //return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
                    }


                }
                else if (linkResp.ResponseCode == 15)
                {
                    //set card status to retry
                    _cardManService.IssueCardLinkRetry(customerDetails.CardId, responseMessage, auditUserId, auditWorkstation);
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
                }
                else if (linkResp.ResponseCode == 9)
                {
                    //set card status to cms error
                    _cardManService.IssueCardLinkFail(customerDetails.CardId, responseMessage, auditUserId, auditWorkstation);
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
                }
                else //Something went wrong linking the card, but its safe to leave it in current status
                {
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
                }
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (BaseIndigoException iex)
            {
                _cardManService.IssueCardLinkFail(customerDetails.CardId, iex.ToString(), auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        iex.Message,
                                        log.IsDebugEnabled || log.IsTraceEnabled ? iex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                _cardManService.IssueCardLinkFail(customerDetails.CardId, ex.ToString(), auditUserId, auditWorkstation);
                responseMessage = _translator.TranslateResponseCode(SystemResponseCode.GENERAL_FAILURE, 0, languageId, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.ERROR,
                                        responseMessage,
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }


            try
            {
                string feeResponse = String.Empty;
                ProductResult product_info = _cardManService.GetProduct(customerDetails.ProductId, auditUserId, auditWorkstation);
                //STEP 3: Charge Fee if it's greater than 0 and has not already been charged for.
                if (customerDetails.FeeCharge != null && customerDetails.FeeCharge.Value > 0 && String.IsNullOrWhiteSpace(customerDetails.FeeReferenceNumber)
                        && !(bool)product_info.Product.charge_fee_at_cardrequest)
                {
                    log.Trace(t => t("Charge the fee."));
                    string feeReferenceNumber = string.Empty;
                    _integration.CoreBankingSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out externalFields, out config);

                    InterfaceInfo interfaceInfo = new InterfaceInfo
                    {
                        Config = config,
                        InterfaceGuid = config.InterfaceGuid.ToString()

                    };


                    AuditInfo auditInfo = new AuditInfo
                    {
                        AuditUserId = auditUserId,
                        AuditWorkStation = auditWorkstation,
                        LanguageId = languageId
                    };

                    var response_fee = COMS.COMSController.ComsCore.ChargeFee(customerDetails, externalFields, interfaceInfo, auditInfo);

                    if (response_fee.ResponseCode != 0)
                    {
                        _cardManService.UpdateFeeChargeStatus(customerDetails.CardId, 2, null, null, auditUserId, auditWorkstation);
                        responseMessage = String.Format("Fee not debited ({0})", response_fee.ResponseMessage);
                        //return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
                    }
                    else
                        _cardManService.UpdateFeeChargeStatus(customerDetails.CardId, 1, customerDetails.FeeReferenceNumber, null, auditUserId, auditWorkstation);
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(customerDetails.FeeReferenceNumber))
                        if (log.IsDebugEnabled)
                            log.DebugFormat("Fee already charged: Ref{0}", customerDetails.FeeReferenceNumber);
                        else
                            log.Trace(t => t("Fee already charged."));
                }
            }
            catch (NotImplementedException nie)
            {
                log.Warn(nie);
                return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                        nie.Message,
                                        nie.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        ex.Message,
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

            return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, responseMessage);
        }


        internal BaseResponse UploadBatchtoCMS(long printbatchId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            string responseMessage = String.Empty;

            Veneka.Indigo.Integration.Config.IConfig config;
            Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;
            int newPrintBatchStatusId;
            IntegrationController _integration = IntegrationController.Instance;
            List<CustomerDetails> lstcustomerdetails = new List<CustomerDetails>();
            List<RequestData> lstrequestdata = new List<RequestData>();
            List<PrintBatchRequestDetails> list = _printservice.GetPrintBatchRequests(printbatchId, 1, 1000, auditUserId, auditWorkstation);
            foreach (PrintBatchRequestDetails p in list)
            //.Where(i => i.hybrid_request_statuses_id != (int)HybridRequestStatuses.PROCESSED_IN_CMS))
            {
                if (p.card_id != null)
                {
                    bool instantPinYN;
                    try //Validation checks: this try catch will not put card into a failed state
                    {
                        log.Debug("card_id :" + p.card_id);
                        CustomerDetails customerDetails = ExtractCustomerDetails(_cardManService.GetCardDetails((long)p.card_id, false, languageId, auditUserId, auditWorkstation), out instantPinYN);

                        if (customerDetails.BranchCardStatusesId == (int)BranchCardStatus.ASSIGN_TO_REQUEST ||
                            customerDetails.BranchCardStatusesId == (int)BranchCardStatus.CMS_ERROR
                            || customerDetails.BranchCardStatusesId == (int)BranchCardStatus.CMS_REUPLOAD
                          )
                        {
                            //Get card and customer details

                            //Check account Balance
                            if (!CheckAccountBalance(customerDetails, languageId, auditUserId, auditWorkstation, out responseMessage))
                                log.Warn(responseMessage);
                            lstcustomerdetails.Add(customerDetails);
                        }

                    }
                    catch (NotImplementedException nie)
                    {
                        log.Warn(nie);
                        return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                                nie.Message,
                                                nie.Message);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        return new BaseResponse(ResponseType.ERROR,
                                                ex.Message,
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
                    }
                }
                else
                {
                    log.Debug("there is no requests in batch to upload to cms.");
                }
            }
            if (lstcustomerdetails.Count > 0)
            {
                try
                {
                    log.Trace(t => t("Link card on CMS."));

                    //STEP 2: Link the Card
                    _integration.CardManagementSystem(lstcustomerdetails[0].ProductId, InterfaceArea.ISSUING, out externalFields, out config);

                    InterfaceInfo interfaceInfo = new InterfaceInfo
                    {
                        Config = config,
                        InterfaceGuid = config.InterfaceGuid.ToString()
                    };

                    AuditInfo auditInfo = new AuditInfo
                    {
                        AuditUserId = auditUserId,
                        AuditWorkStation = auditWorkstation,
                        LanguageId = languageId
                    };
                    Dictionary<long, LinkResponse> responses;

                    var linkResp = COMSController.ComsCore.LinkCardsToAccount(lstcustomerdetails, externalFields, interfaceInfo, auditInfo, out responses);

                    foreach (KeyValuePair<long, LinkResponse> pair in responses)
                    {
                        long CardId = pair.Key;
                        int ResponseCode = (int)(LinkResponse)pair.Value;
                        try
                        {

                            if (ResponseCode == 0)
                            {
                                //_cardManService.update(CardId, auditUserId, auditWorkstation);
                                log.Debug(t => t(CardId.ToString() + " is uploaded to cms."));
                                //1 - successful and 
                                lstrequestdata.Add(new RequestData() { card_number = CardId.ToString(), request_statues_id = 1 });
                            }
                            else if (ResponseCode == 15)
                            {

                                //set card status to retry
                                // _cardManService.IssueCardLinkRetry(CardId, responseMessage, auditUserId, auditWorkstation);
                                log.Debug(t => t(CardId.ToString() + " is should retry in cms."));
                            }
                            else if (ResponseCode == 9)
                            {
                                //set card status to cms error
                                // _cardManService.IssueCardLinkFail(CardId, responseMessage, auditUserId, auditWorkstation);
                                log.Trace(t => t(CardId.ToString() + " is cms_error state."));

                            }
                            else //Something went wrong linking the card, but its safe to leave it in current status
                            {
                                log.Trace(t => t(CardId.ToString() + " responseMessage"));

                            }
                        }
                        catch (BaseIndigoException iex)
                        {
                            log.Error(CardId.ToString() + ":" + iex.Message);
                            // _cardManService.IssueCardLinkFail(CardId, iex.ToString(), auditUserId, auditWorkstation);

                        }
                        catch (Exception ex)
                        {
                            //_cardManService.IssueCardLinkFail(CardId, ex.ToString(), auditUserId, auditWorkstation);
                            responseMessage = _translator.TranslateResponseCode(SystemResponseCode.GENERAL_FAILURE, 0, languageId, auditUserId, auditWorkstation);
                            log.Error(CardId.ToString() + ":" + responseMessage);
                        }
                    }


                    if (linkResp.ResponseCode == 0)
                    {
                        newPrintBatchStatusId = (int)PrintBatchStatuses.PROCESSED_IN_CMS;
                    }
                    else
                    {
                        newPrintBatchStatusId = (int)PrintBatchStatuses.CMS_ERROR;

                    }

                }
                catch (NotImplementedException nie)
                {
                    log.Warn(nie);
                    return new BaseResponse(ResponseType.UNSUCCESSFUL,
                                            nie.Message,
                                            nie.Message);
                }

                _printservice.UploadPrintBatchToCMS(printbatchId, newPrintBatchStatusId, lstrequestdata, notes, true, languageId, auditUserId, auditWorkstation, out responseMessage);
                return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, responseMessage);

            }
            else
            {
                log.Debug("There is no cards in batch: CMS_ERROR/ASSIGN_TO_REQUEST/CMS_REUPLOAD");
                _printservice.UploadPrintBatchToCMS(printbatchId, (int)PrintBatchStatuses.PROCESSED_IN_CMS, lstrequestdata, notes, true, languageId, auditUserId, auditWorkstation, out responseMessage);
                return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, responseMessage);
            }


        }



        private CustomerDetails ExtractCustomerDetails(CardDetailResult cardDetails, out bool isInstantPin)
        {
            CustomerDetails customer = new CustomerDetails();
            customer.IssuerCode = cardDetails.issuer_code;
            if (cardDetails.domicile_branch_id != null)
                customer.DomicileBranchId = cardDetails.domicile_branch_id.Value;
            customer.AccountNumber = cardDetails.customer_account_number;
            customer.AccountPin = cardDetails.customer_account_pin;

            if (cardDetails.account_type_id != null)
                customer.AccountTypeId = cardDetails.account_type_id.Value;
            customer.CMSAccountType = cardDetails.cms_account_type;
            customer.CBSAccountType = cardDetails.cbs_account_type;

            customer.BranchId = cardDetails.branch_id;
            customer.BranchCode = cardDetails.branch_code;
            customer.CardId = cardDetails.card_id;
            customer.BranchCardStatusesId = cardDetails.branch_card_statuses_id.Value;

            customer.ProductId = cardDetails.product_id;
            customer.ProductCode = cardDetails.product_code;
            customer.ProductBinCode = cardDetails.product_bin_code;
            customer.SubProductCode = cardDetails.sub_product_code;

            customer.CardNumber = cardDetails.card_number;
            customer.CardSequence = cardDetails.card_sequence;
            customer.CardReference = cardDetails.card_request_reference;
            customer.CmsID = cardDetails.cms_id;
            customer.CustomerId = cardDetails.CustomerId;
            customer.ContractNumber = cardDetails.contract_number;
            customer.ContactNumber = cardDetails.contact_number;
            if (cardDetails.currency_id != null)
                customer.CurrencyId = cardDetails.currency_id.Value;
            customer.CurrencyCode = cardDetails.currency_code;
            customer.CurrencyNumericCode = cardDetails.iso_4217_numeric_code;
            customer.IsBaseCurrency = cardDetails.is_base_currency;
            customer.CurrencyFields = new Dictionary<string, string>()
            {
                { cardDetails.usr_field_name_1 ?? "1", cardDetails.usr_field_val_1 },
                { cardDetails.usr_field_name_2 ?? "2", cardDetails.usr_field_val_2 }
            };

            if (cardDetails.resident_id != null)
                customer.CustomerResidencyId = cardDetails.resident_id.Value;
            if (cardDetails.customer_type_id != null)
                customer.CustomerTypeId = cardDetails.customer_type_id.Value;
            customer.FirstName = cardDetails.customer_first_name;
            customer.MiddleName = cardDetails.customer_middle_name;
            customer.LastName = cardDetails.customer_last_name;
            if (cardDetails.card_issue_reason_id != null)
                customer.CardIssueReasonId = cardDetails.card_issue_reason_id.Value;
            customer.IssuerId = cardDetails.issuer_id;
            customer.NameOnCard = cardDetails.name_on_card;
            customer.CustomerIDNumber = cardDetails.id_number;

            customer.CountryId = cardDetails.country_id;
            customer.CountryName = cardDetails.country_name;
            customer.CountryCode = cardDetails.country_code;
            customer.CountryCapital = cardDetails.country_capital_city;

            //Fee stuff
            customer.ChargeFeeToIssuingBranch = cardDetails.charge_fee_to_issuing_branch_YN;
            customer.FeeCharge = cardDetails.fee_charged;
            customer.Vat = cardDetails.vat;
            customer.VatCharged = cardDetails.vat_charged;
            customer.TotalCharged = cardDetails.total_charged;
            customer.FeeEditbleYN = cardDetails.fee_editable_YN;
            customer.FeeOverridenYN = cardDetails.fee_overridden_YN;
            customer.FeeWaiverYN = cardDetails.fee_waiver_YN;
            customer.FeeReferenceNumber = cardDetails.fee_reference_number;
            customer.FeeReversalRefNumber = cardDetails.fee_reversal_ref_number;

            customer.FeeRevenueAccountNo = cardDetails.fee_revenue_account_no;
            customer.FeeRevenueAccountTypeId = cardDetails.fee_revenue_account_type_id.GetValueOrDefault();
            customer.VatAccountNo = cardDetails.vat_account_no;
            customer.VatAccountTypeId = cardDetails.vat_account_type_id.GetValueOrDefault();

            //Determin which branchcode to use for fee revenue account
            if (!String.IsNullOrWhiteSpace(cardDetails.vat_account_branch_code))
                customer.FeeRevenueBranchNo = cardDetails.fee_revenue_branch_code;
            else if (cardDetails.charge_fee_to_issuing_branch_YN)
            {
                customer.FeeRevenueBranchNo = cardDetails.branch_code;
                customer.VatBranchNo = cardDetails.branch_code;
            }
            else
            {
                customer.FeeRevenueBranchNo = cardDetails.domicile_branch_code;
                customer.VatBranchNo = cardDetails.domicile_branch_code;
            }

            //overwrite branch code if one was supplied
            if (!String.IsNullOrWhiteSpace(cardDetails.vat_account_branch_code))
                customer.VatBranchNo = cardDetails.vat_account_branch_code;


            //Based on the issuers language use the following narration
            switch (cardDetails.language_id)
            {
                case 0:
                    customer.FeeRevenueNarration = cardDetails.fee_revenue_narration_en;
                    customer.VatNarration = cardDetails.vat_narration_en;
                    break;

                case 1:
                    customer.FeeRevenueNarration = cardDetails.fee_revenue_narration_fr;
                    customer.VatNarration = cardDetails.vat_narration_fr;
                    break;

                case 2:
                    customer.FeeRevenueNarration = cardDetails.fee_revenue_narration_pt;
                    customer.VatNarration = cardDetails.vat_narration_pt;
                    break;
                case 3:
                    customer.FeeRevenueNarration = cardDetails.fee_revenue_narration_es;
                    customer.VatNarration = cardDetails.vat_narration_es;
                    break;
                default:
                    customer.FeeRevenueNarration = cardDetails.fee_revenue_narration_en;
                    customer.VatNarration = cardDetails.vat_narration_en;
                    break;
            }



            customer.IsPINSelected = cardDetails.pin_selected ?? false;
            customer.PinOffset = cardDetails.pvv;
            isInstantPin = cardDetails.enable_instant_pin_YN;
            if (cardDetails.renewal_detail_id.HasValue && cardDetails.renewal_detail_id.Value > 0)
            {
                customer.ActivationMethod = CardActivationMethod.Option1;
            }
            //set the product fields
            if (cardDetails is CardDetails)
            {
                customer.ProductFields = ((CardDetails)cardDetails).ProductFields;
            }

            return customer;
        }

        private bool CheckAccountBalance(CustomerDetails customerDetails, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            responseMessage = String.Empty;

            //Skip Charge fee if card is charged at request

           

            //Check if we are charging a fee
            if (customerDetails.FeeCharge != null)
            {
                if (customerDetails.FeeWaiverYN == null)
                    throw new ArgumentException("Fee waiver cannot be null.");

                if (customerDetails.FeeWaiverYN.Value)
                    customerDetails.FeeCharge = 0;

                //if (customerDetails.FeeCharge == null)
                //    throw new ArgumentNullException("Fee cannot be null.");

                if (customerDetails.FeeCharge.Value < 0)
                    throw new ArgumentException("Fee cannot be a negative value.");

                if (log.IsTraceEnabled)
                    log.TraceFormat("Fee details.. CardId: {0}, FeeWaived: {1}, FeeCharge: {2}, AccountNo: {3}.", customerDetails.CardId,
                                                                                         customerDetails.FeeWaiverYN,
                                                                                         customerDetails.FeeCharge,
                                                                                         customerDetails.AccountNumber);
            }
            else
                log.Trace(t => t("Fee charge empty, not going to charge a fee."));

            //Check Balance
            if (customerDetails.FeeCharge != null && customerDetails.FeeCharge.Value > 0) //Only if we are charging a fee
            {
                //work out total

                customerDetails.TotalCharged = customerDetails.FeeCharge.GetValueOrDefault() * (1M + (customerDetails.Vat.GetValueOrDefault() * 0.01M));

                Veneka.Indigo.Integration.Config.IConfig config;
                Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;

                log.Trace(t => t("Checking balance for fee charge."));
                IntegrationController _integration = IntegrationController.Instance;
                _integration.CoreBankingSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out externalFields, out config);

                InterfaceInfo interfaceInfo = new InterfaceInfo
                {
                    Config = config,
                    InterfaceGuid = config.InterfaceGuid.ToString()

                };

                log.Trace("CheckAccountBalance: InterfaceInfo interfaceInfo = new InterfaceInfo");
                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };
                var response = COMS.COMSController.ComsCore.CheckBalance(customerDetails, externalFields, interfaceInfo, auditInfo);
                log.Trace("CheckAccountBalance: COMS.COMSController.ComsCore.CheckBalance(customerDetails, externalFields, interfaceInfo, auditInfo)");
                if (response.ResponseCode == 0)
                {
                    log.Trace("response.ResponseCode == 0 from ComsCore.CheckBalance");
                    return true;
                }
                else
                {
                    responseMessage = response.ResponseMessage;
                    log.Trace(responseMessage);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Generate PDF Report for checked out cards.
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="branchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public Response<byte[]> GenerateCheckedOutCardsReport(int? issuerId, long? operatorId, int branchId, int? userRoleId, string username, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<byte[]>(_cardManService.GenerateCheckedOutCardsReport(issuerId, operatorId, branchId, userRoleId, username, languageId, auditUserId, auditWorkstation),
                                            ResponseType.SUCCESSFUL,
                                            "",
                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<byte[]>(null,
                                            ResponseType.ERROR,
                                            "Error when processing request.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Generate PDF Report for checked in cards.
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="branchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public Response<byte[]> GenerateCheckedInCardsReport(int branchId, List<structCard> cardList, string operatorUsername, string username, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<byte[]>(_cardManService.GenerateCheckedInCardsReport(branchId, cardList, operatorUsername, username, languageId, auditUserId, auditWorkstation),
                                            ResponseType.SUCCESSFUL,
                                            "",
                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<byte[]>(null,
                                            ResponseType.ERROR,
                                            "Error when processing request.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Returns a list of card priorities.
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<card_priority>> GetCardPriorityList(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<card_priority>>(_cardManService.GetCardPriorityList(languageId, auditUserId, auditWorkstation),
                                                         ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<card_priority>>(null, ResponseType.ERROR,
                                                         "Error when processing request.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<string> EPinRequest(int issuerId, int branchId, int productId, string moblieNumber, string pan, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string indigoReference = Guid.NewGuid().ToString();

                string[] _code = PseudoGenerator.Generate(5, 1, PseudoGenerator.PseudoType.Numeric);

                string PRRN = string.Empty;
                if (_code != null && _code.Count() > 0)
                    PRRN = _code[0];
                if (string.IsNullOrEmpty(PRRN))
                    throw new Exception("An error occured while generating PRRN number.");
                var response = COMSController.ComsCore.EPinRequest(indigoReference, PRRN, moblieNumber, pan);
                if (response != null && response.ResponseCode == 0)
                {
                    PinReissueResult result1 = null; string message;
                    var _searchresult = _cardManService.PINReissueSearch(issuerId, branchId, null, 0, 1, null, false, null, null, null, languageId, 1, 2000, auditUserId, auditWorkstation);
                    if (_searchresult != null)
                    {
                        result1 = _searchresult.FirstOrDefault(i => i.card_number == MaskAccountnumber(pan, 6, 4) && i.mobile_number == moblieNumber);
                    }
                    if (result1 != null)
                        _cardManService.ExpirePINReissue(result1.pin_reissue_id, "expired", languageId, auditUserId, auditWorkstation, out result1, out message);
                    else
                    {
                        PinReissueResult result; string responsemessage;
                        _cardManService.RequestPINReissue(issuerId, branchId, productId, pan, null, moblieNumber, 1, languageId, auditUserId, auditWorkstation, out result, out responsemessage);
                    }
                    return new Response<string>(PRRN,
                                                    ResponseType.SUCCESSFUL, "", "");

                }

                return new Response<string>("",
                                                         ResponseType.UNSUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<string>(null, ResponseType.ERROR,
                                                         "Error when processing request.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        private string MaskAccountnumber(string messageField, int masklength, int rightpad)
        {
            int LeftPad = messageField.Length - rightpad;
            string righttext = messageField.Substring(LeftPad, rightpad);
            string Lefttext = messageField.Substring(0, (LeftPad - masklength));

            return Lefttext + "".PadLeft(masklength, '*')
                        + righttext;


        }
        internal Response<List<CardHistoryReference>> GetCardReferenceHistory(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<CardHistoryReference>>(_cardManService.GetCardReferenceHistory(cardId, languageId, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CardHistoryReference>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<CardHistoryStatus>> GetCardStatusHistory(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<CardHistoryStatus>>(_cardManService.GetCardStatusHistory(cardId, languageId, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CardHistoryStatus>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        internal Response<List<RequestReferenceHistoryResult>> GetRequestReferenceHistory(long requestId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<RequestReferenceHistoryResult>>(_cardManService.GetRequestReferenceHistory(requestId, languageId, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<RequestReferenceHistoryResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<RequestStatusHistoryResult>> GetRequestStatusHistory(long requestId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<RequestStatusHistoryResult>>(_cardManService.GetRequestStatusHistory(requestId, languageId, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<RequestStatusHistoryResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<ProductField[]> GetPrintFieldsByProductid(int productId)
        {
            try
            {
                return new Response<ProductField[]>(_cardManService.GetPrintFieldsByProductid(productId),
                                                         ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ProductField[]>(null, ResponseType.ERROR,
                                                         "Error when processing request.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<ProductField[]> GetProductFieldsByCardId(long CardId)
        {
            try
            {
                return new Response<ProductField[]>(_cardManService.GetProductFields(null, CardId, null),
                                                         ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ProductField[]>(null, ResponseType.ERROR,
                                                         "Error when processing request.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        #region CLASSIC CARD METHODS

        /// <summary>
        /// Create a card request for classic card issuing.
        /// </summary>
        /// <param name="customerDetails"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public Response<long> RequestCardForCustomer(CustomerDetails customerDetails, long? renewalDetailId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                long cardId;

                ProductResult _productresult = _cardManService.GetProduct(customerDetails.ProductId, auditUserId, auditWorkstation);

                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };
                if ((bool)_productresult.Product.charge_fee_at_cardrequest)// checking charge fee at cardrequest flag is enabled. 
                {
                    if (renewalDetailId != null && renewalDetailId.GetValueOrDefault() != 0)
                    {
                        FeeChargeLogic feelogic = new FeeChargeLogic(_cardManService, _comsCoreInstance, _integration);
                        BaseResponse feeresponse = feelogic.FeeCharge(customerDetails, auditInfo);
                        if (feeresponse.ResponseType != ResponseType.SUCCESSFUL)
                            return new Response<long>(0, ResponseType.UNSUCCESSFUL, feeresponse.ResponseMessage, feeresponse.ResponseMessage);
                    }
                }

                if (_productresult.Product.credit_limit_capture.GetValueOrDefault() == true)
                {
                    IntegrationController _integration = IntegrationController.Instance;
                    //Veneka.Indigo.Integration.Config.IConfig config;
                    Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;
                    Veneka.Indigo.Integration.Config.IConfig config;
                    _integration.CardManagementSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out externalFields, out config);

                    InterfaceInfo interfaceInfo = new InterfaceInfo
                    {
                        Config = config,
                        InterfaceGuid = config.InterfaceGuid.ToString()
                    };
                    //update the customer product fields with values from CMS
                    _comsCoreInstance.ValidateCustomerDetails(customerDetails, externalFields, interfaceInfo, auditInfo);
                }

                if (_cardManService.RequestCardForCustomer(customerDetails, renewalDetailId, languageId, auditUserId, auditWorkstation, out cardId, out responseMessage))
                {
                    if (!String.IsNullOrWhiteSpace(customerDetails.FeeReferenceNumber))
                    {
                        _cardManService.UpdateFeeChargeStatus(cardId, 1, customerDetails.FeeReferenceNumber, null, auditUserId, auditWorkstation);
                    }

                    return new Response<long>(cardId, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<long>(0, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long>(0, ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <returns></returns>
        /// 
        public Response<long> RequestInstantCardForCustomer(CustomerDetails customerDetails, out string printJobId, int languageId, long auditUserId, string auditWorkstation)
        {
            printJobId = string.Empty;
            try
            {
                string responseMessage;
                long cardId, new_customer_account_id;



                if (_cardManService.RequestInstantCardForCustomer(customerDetails, languageId, auditUserId, auditWorkstation, out cardId, out new_customer_account_id, out long PrintJobId, out responseMessage))
                {
                    customerDetails.CustomerId = new_customer_account_id.ToString();
                    printJobId = PrintJobId.ToString();

                    return new Response<long>(cardId, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<long>(0, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long>(0, ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        public Response<long> RequestHybridCardForCustomer(CustomerDetails customerDetails, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                long cardId;
                ProductResult _productresult = _cardManService.GetProduct(customerDetails.ProductId, auditUserId, auditWorkstation);
                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };

                if ((bool)_productresult.Product.charge_fee_at_cardrequest)// checking charge fee at cardrequest flag is enabled. 
                {
                    FeeChargeLogic feelogic = new FeeChargeLogic(_cardManService, _comsCoreInstance, _integration);
                    BaseResponse feeresponse = feelogic.FeeCharge(customerDetails, auditInfo);
                    if (feeresponse.ResponseType != ResponseType.SUCCESSFUL)
                        return new Response<long>(0, ResponseType.UNSUCCESSFUL, feeresponse.ResponseMessage, feeresponse.ResponseMessage);

                }

                if (_cardManService.RequestHybridCardForCustomer(customerDetails, languageId, auditUserId, auditWorkstation, out cardId, out responseMessage))
                {
                    if (!String.IsNullOrWhiteSpace(customerDetails.FeeReferenceNumber))
                    {
                        _cardManService.UpdateFeeChargeStatus(cardId, 1, customerDetails.FeeReferenceNumber, null, auditUserId, auditWorkstation);
                    }

                    return new Response<long>(cardId, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<long>(0, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long>(0, ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        public Response<string> RequestCardForCustomer(BulkRequestRecord request)
        {
            try
            {
                string responseCode;
                string responseMessage;
                string referenceNumber;

                if (_cardManService.RequestCardForCustomer(request, 0, -2, "IndigoRequestAPI", out referenceNumber, out responseCode, out responseMessage))
                {
                    return new Response<string>(referenceNumber, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<string>("", ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);

                //Request the card
                //return new Response<string>("Ref", ResponseType.SUCCESSFUL, "00", "Successful.");

                //long cardId;
                //if (_cardManService.RequestCardForCustomer(customerDetails, languageId, auditUserId, auditWorkstation, out cardId, out responseMessage))
                //{
                //    return new Response<long>(cardId, ResponseType.SUCCESSFUL, responseMessage, "");
                //}

                //return new Response<long>(0, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<string>(String.Empty, ResponseType.ERROR,
                                            "Error when processing request.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        public BaseResponse UpdateCustomerDetails(long cardId, long customerAccountId, CustomerDetails customerDetails, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.UpdateCustomerDetails(cardId, customerAccountId, customerDetails, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        #endregion CLASSIC CARD METHODS

        #region EXPOSED METHODS



        #endregion EXPOSED METHODS


        public Response<bool> CreateCardLimit(long cardId, decimal limit, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.CreateCardLimit(cardId, limit, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<bool>(true, ResponseType.SUCCESSFUL, "", "");
                }

                return new Response<bool>(false, ResponseType.UNSUCCESSFUL, "Failed to create card limit", responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false, ResponseType.ERROR, "Failed to create card limit", "General error in creating card limit");
            }
        }

        public Response<bool> UpdateCardLimit(long cardId, decimal limit, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.UpdateCardLimit(cardId, limit, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<bool>(true, ResponseType.SUCCESSFUL, "", "");
                }

                return new Response<bool>(false, ResponseType.UNSUCCESSFUL, "Failed to update card limit", responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false, ResponseType.ERROR, "Failed to update card limit", "General error in updating card limit");
            }
        }

        public Response<bool> ApproveCardLimit(long cardId, decimal limit, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.ApproveCardLimit(cardId, limit, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<bool>(true, ResponseType.SUCCESSFUL, "", "");
                }

                return new Response<bool>(false, ResponseType.UNSUCCESSFUL, "Failed to approve card limit", responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false, ResponseType.ERROR, "Failed to approve card limit", "General error in approving card limit");
            }
        }

        public Response<bool> SetCreditContractNumber(long cardId, string creditContractNumber, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.SetCreditContractNumber(cardId, creditContractNumber, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<bool>(true, ResponseType.SUCCESSFUL, "", "");
                }

                return new Response<bool>(false, ResponseType.UNSUCCESSFUL, "Failed to save contract number", responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false, ResponseType.ERROR, "Failed to save credit contract number", "General error in saving contract number");
            }
        }

        public Response<bool> SetCreditStatus(long cardId, int creditStatusId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.SetCreditStatus(cardId, creditStatusId, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<bool>(true, ResponseType.SUCCESSFUL, "", "");
                }

                return new Response<bool>(false, ResponseType.UNSUCCESSFUL, "Failed to approve card limit", responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false, ResponseType.ERROR, "Failed to approve card limit", "General error in approving card limit");
            }
        }

        public Response<bool> ApproveCardLimitManager(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.ApproveCardLimitManager(cardId, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<bool>(true, ResponseType.SUCCESSFUL, "", "");
                }

                return new Response<bool>(false, ResponseType.UNSUCCESSFUL, "Failed to approve card limit", responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false, ResponseType.ERROR, "Failed to approve card limit", "General error in approving card limit");
            }
        }

        public Response<CardLimitModel> GetCardLimit(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                var result = _cardManService.CardLimitGet(cardId, languageId, auditUserId, auditWorkstation, out responseMessage);

                return new Response<CardLimitModel>(result, ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<CardLimitModel>(null, ResponseType.ERROR, "Failed to get card limit", "General error in retrieving card limit");
            }
        }

    }
}
