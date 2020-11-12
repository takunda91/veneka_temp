using System;
using System.Collections.Generic;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Objects;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Exceptions;
using Veneka.Indigo.Common.License;
using Veneka.Indigo.CardManagement.Reports;
using Veneka.Indigo.Integration.Objects;
using System.Drawing;
using System.IO;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Indigo.Integration.FileLoader.Validation;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Integration.FileLoader;
using System.Text;
using System.Linq;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Common.Models.IssuerManagement;

namespace Veneka.Indigo.CardManagement
{
    public class CardMangementService
    {
        private readonly ICardManagementDAL _cardDAL;
        private readonly ICardLimitDataAccess _cardLimitDAL;
        private readonly IResponseTranslator _translator = new ResponseTranslator();
        private readonly CardsReport _cardReports = new CardsReport();
        private readonly IDataSource _dataSource;

        private Dictionary<Tuple<int, int>, product_interface> _cachedProductInterface = new Dictionary<Tuple<int, int>, product_interface>();

        public CardMangementService() : this(null, new CardManagementDAL(), new ResponseTranslator(), new CardLimitDataAccess()) { }

        public CardMangementService(IDataSource datasource) : this(datasource, new CardManagementDAL(), new ResponseTranslator(), new CardLimitDataAccess()) { }

        public CardMangementService(IDataSource datasource, ICardManagementDAL cardManagementDAL, IResponseTranslator translator, ICardLimitDataAccess cardLimitDAL)
        {
            _dataSource = datasource;
            _cardDAL = cardManagementDAL ?? new CardManagementDAL();
            _translator = translator ?? new ResponseTranslator();
            _cardLimitDAL = cardLimitDAL ?? new CardLimitDataAccess();
        }


        #region CLASSIC CARD METHODS
        /// <summary>
        /// Create a card request for classic card issuing.
        /// </summary>
        /// <param name="customerDetails"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="cardId"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public bool RequestCardForCustomer(CustomerDetails customerDetails, long? renewalDetailId, int languageId, long auditUserId, string auditWorkstation, out long cardId, out string responseMessage)
        {
            var response = _cardDAL.RequestCardForCustomer(customerDetails.DeliveryBranchId, customerDetails.BranchId,
                                                     customerDetails.ProductId, customerDetails.PriorityId,
                                                     customerDetails.AccountNumber, customerDetails.DomicileBranchId,
                                                     customerDetails.AccountTypeId, customerDetails.CardIssueReasonId,
                                                     customerDetails.FirstName, customerDetails.MiddleName, customerDetails.LastName,
                                                     customerDetails.NameOnCard,
                                                     customerDetails.CustomerTitleId,
                                                     customerDetails.CurrencyId,
                                                     customerDetails.CustomerResidencyId,
                                                     customerDetails.CustomerTypeId,
                                                     customerDetails.CmsID, customerDetails.ContractNumber, customerDetails.CustomerIDNumber, customerDetails.ContactNumber, customerDetails.EmailAddress,
                                                     customerDetails.CustomerId,
                                                     customerDetails.FeeWaiverYN, customerDetails.FeeEditbleYN,
                                                     customerDetails.FeeCharge, customerDetails.Vat, customerDetails.VatCharged, customerDetails.TotalCharged,
                                                     customerDetails.FeeOverridenYN,
                                                     customerDetails.ProductFields.Cast<IProductPrintField>().ToList(), customerDetails.CardIssueMethodId, renewalDetailId, customerDetails.CBSAccountType,
                                                     auditUserId, auditWorkstation, out cardId);

            responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool RequestInstantCardForCustomer(CustomerDetails customerDetails, int languageId, long auditUserId, string auditWorkstation, out long cardId, out long new_customer_account_id, out long PrintJobId, out string responseMessage)
        {
            var response = _cardDAL.RequestInstantCardForCustomer(customerDetails.DeliveryBranchId, customerDetails.BranchId,
                                                     customerDetails.ProductId, customerDetails.PriorityId,
                                                     customerDetails.AccountNumber, customerDetails.DomicileBranchId,
                                                     customerDetails.AccountTypeId, customerDetails.CardIssueReasonId,
                                                     customerDetails.FirstName, customerDetails.MiddleName, customerDetails.LastName,
                                                     customerDetails.NameOnCard,
                                                     customerDetails.CustomerTitleId,
                                                     customerDetails.CurrencyId,
                                                     customerDetails.CustomerResidencyId,
                                                     customerDetails.CustomerTypeId,
                                                     customerDetails.CmsID, customerDetails.ContractNumber, customerDetails.CustomerIDNumber,
                                                     customerDetails.ContactNumber, customerDetails.EmailAddress,
                                                     customerDetails.CustomerId,
                                                     customerDetails.FeeWaiverYN, customerDetails.FeeEditbleYN,
                                                     customerDetails.FeeCharge,
                                                     customerDetails.FeeOverridenYN,
                                                     customerDetails.ProductFields.Cast<IProductPrintField>().ToList(), customerDetails.CardIssueMethodId,
                                                     auditUserId, auditWorkstation, out cardId, out PrintJobId, out new_customer_account_id);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_MAKERCHECKER_APPROVED, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }
        public bool RequestHybridCardForCustomer(CustomerDetails customerDetails, int languageId, long auditUserId, string auditWorkstation, out long requestId, out string responseMessage)
        {
            var response = _cardDAL.RequestHybridCardForCustomer(customerDetails.DeliveryBranchId, customerDetails.BranchId,
                                                     customerDetails.ProductId, customerDetails.PriorityId,
                                                     customerDetails.AccountNumber, customerDetails.DomicileBranchId,
                                                     customerDetails.AccountTypeId, customerDetails.CardIssueReasonId,
                                                     customerDetails.FirstName, customerDetails.MiddleName, customerDetails.LastName,
                                                     customerDetails.NameOnCard,
                                                     customerDetails.CustomerTitleId,
                                                     customerDetails.CurrencyId,
                                                     customerDetails.CustomerResidencyId,
                                                     customerDetails.CustomerTypeId,
                                                     customerDetails.CmsID, customerDetails.ContractNumber, customerDetails.CustomerIDNumber, customerDetails.ContactNumber,
                                                     customerDetails.CustomerId,
                                                     customerDetails.FeeWaiverYN, customerDetails.FeeEditbleYN,
                                                     customerDetails.FeeCharge,
                                                     customerDetails.FeeOverridenYN,
                                                     customerDetails.ProductFields.Cast<IProductPrintField>().ToList(), customerDetails.CardIssueMethodId, customerDetails.CBSAccountType,
                                                     auditUserId, auditWorkstation, out requestId);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }
        public bool RequestCardForCustomer(BulkRequestRecord request, int languageId, long auditUserId, string auditWorkstation, out string referenceNumber, out string responseCode, out string response)
        {
            referenceNumber = String.Empty;

            //Check required fields
            if (String.IsNullOrWhiteSpace(request.IssuerCode))
            {
                responseCode = "01";
                response = "Issuer code not set.";
                return false;
            }


            if (String.IsNullOrWhiteSpace(request.BranchCode))
            {
                responseCode = "02";
                response = "Branch code not set.";
                return false;
            }

            if (String.IsNullOrWhiteSpace(request.DomicileBranchCode))
            {
                responseCode = "03";
                response = "Domicile branch  code not set.";
                return false;
            }

            if (String.IsNullOrWhiteSpace(request.ProductCode))
            {
                responseCode = "04";
                response = "Product code not set.";
                return false;
            }

            if (String.IsNullOrWhiteSpace(request.CustomerFirstName))
            {
                responseCode = "05";
                response = "Customer first name not set.";
                return false;
            }

            if (String.IsNullOrWhiteSpace(request.CustomerLastName))
            {
                responseCode = "06";
                response = "Customer last name not set.";
                return false;
            }

            if (String.IsNullOrWhiteSpace(request.CustomerAccountNumber))
            {
                responseCode = "07";
                response = "Customer account not set.";
                return false;
            }

            if (String.IsNullOrWhiteSpace(request.CurrencyCode))
            {
                responseCode = "08";
                response = "Currency code not set.";
                return false;
            }


            BranchValidation branchValidation = new BranchValidation(_dataSource.BranchDAL, "");
            IssuerValidation issuerValidation = new IssuerValidation(_dataSource.IssuerDAL, Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), @"license\machine.vmg"), "");
            ProductValidation productValidation = new ProductValidation(_dataSource.ProductDAL, "");

            List<FileCommentsObject> comments = new List<FileCommentsObject>();
            List<BulkRequestRecord> BulkRequestsFile = new List<BulkRequestRecord> { request };

            StringBuilder commentsBuilder = new StringBuilder();

            //Do lookups and further validation
            //Do product validations
            if (productValidation.ValidateBulkRequestsFile(new BulkRequestsFile(request.IssuerCode, "IndigoAPI", BulkRequestsFile), comments, -1, "IndigoAPI") != FileStatuses.READ)
            {
                foreach (var comment in comments)
                    commentsBuilder.AppendLine(comment.GetFormatedComment());

                responseCode = "20";
                response = commentsBuilder.ToString();
                return false;
            }

            //Do issuer validations
            if (issuerValidation.ValidateBulkRequestsFile(new BulkRequestsFile(request.IssuerCode, "IndigoAPI", BulkRequestsFile), comments, -1, "IndigoAPI") != FileStatuses.READ)
            {
                foreach (var comment in comments)
                    commentsBuilder.AppendLine(comment.GetFormatedComment());

                responseCode = "30";
                response = commentsBuilder.ToString();
                return false;
            }

            //Do branch valaidations
            if (branchValidation.ValidateBulkRequestsFile(new BulkRequestsFile(request.IssuerCode, "IndigoAPI", BulkRequestsFile), comments, -1, "IndigoAPI") != FileStatuses.READ)
            {
                foreach (var comment in comments)
                    commentsBuilder.AppendLine(comment.GetFormatedComment());

                responseCode = "40";
                response = commentsBuilder.ToString();
                return false;
            }

            long cardId;
            //Now insert request and send back reference number.
            var response2 = _cardDAL.RequestCardForCustomer(BulkRequestsFile[0].BranchId, BulkRequestsFile[0].BranchId,
                                                     BulkRequestsFile[0].ProductId.Value, BulkRequestsFile[0].CardPriorityId,
                                                     BulkRequestsFile[0].CustomerAccountNumber, BulkRequestsFile[0].DomicileBranchId,
                                                     BulkRequestsFile[0].AccountTypeId, BulkRequestsFile[0].CardIssueReasonId,
                                                     BulkRequestsFile[0].CustomerFirstName, BulkRequestsFile[0].CustomerMiddleName, BulkRequestsFile[0].CustomerLastName,
                                                     BulkRequestsFile[0].NameOnCard,
                                                     BulkRequestsFile[0].CustomerTitleId,
                                                     BulkRequestsFile[0].CurrencyId,
                                                     BulkRequestsFile[0].ResidentId,
                                                     BulkRequestsFile[0].CustomerTypeId,
                                                     BulkRequestsFile[0].CmsId.ToString(), BulkRequestsFile[0].ContractNumber, BulkRequestsFile[0].IdNumber,
                                                     BulkRequestsFile[0].ContactNumber, BulkRequestsFile[0].EmailAddress,
                                                     BulkRequestsFile[0].CustomerId,
                                                     BulkRequestsFile[0].FeeWaiverYN, BulkRequestsFile[0].FeeEditableYN,
                                                     BulkRequestsFile[0].FeeCharged, 0, 0, BulkRequestsFile[0].FeeCharged,
                                                     BulkRequestsFile[0].FeeOverriddenYN,
                                                     BulkRequestsFile[0].PrintFields, BulkRequestsFile[0].CardIssueMethodId, null, null,
                                                     auditUserId, auditWorkstation, out cardId);

            response = _translator.TranslateResponseCode(response2, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, languageId, auditUserId, auditWorkstation);

            if (response2 == SystemResponseCode.SUCCESS)
            {
                //TODO: get the reference number

                responseCode = "00";
                return true;
            }

            responseCode = "99";
            return false;
        }

        public bool UpdateCustomerDetails(long cardId, long customerAccountId, CustomerDetails customerDetails, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.UpdateCustomerDetails(cardId, customerAccountId, customerDetails, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        #endregion

        /// <summary>
        /// Fetch customer detail based on cardId.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public CustomerDetailsResult GetCustomerDetails(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetCustomerDetails(cardId, languageId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Get account balance details
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>e
        public bool IssueCardCheckCustomerAccountBalance(CustomerDetails customerAccount, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.CheckCustomerAccountBalance(customerAccount, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Returns a list of card priorities.
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<card_priority> GetCardPriorityList(int languageId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetCardPriorityList(languageId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Fetch a list of Branch card codes.
        /// </summary>
        /// <param name="BranchCardCodeType"></param>
        /// <param name="isException"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<branch_card_codes> ListBranchCardCodes(int branchCardCodeType, bool isException, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.ListBranchCardCodes(branchCardCodeType, isException, auditUserId, auditWorkstation);
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
        public List<CardSearchResult> SearchForCards(long userId, int? userRoleId, int? issuerId, int? branchId, string cardNumber,
                                                      string cardLastFourDigits, string cardrefnumber, string batchReference,
                                                      int? loadCardStatusId, int? distCardStatusId, int? branchCardStatusId, long? distBatchId, long? pinBatchId, long? threedBatchId,
                                                      string accountNumber, string firstName, string lastName, string cmsId,
                                                      DateTime? dateFrom, DateTime? dateTo, int? cardIssueMethodId,
                                                      int? productId, int? priorityId, int pageIndex, int rowsPerPage,
                                                      long auditUserId, string auditWorkstation)
        {
            return _cardDAL.SearchForCards(userId, userRoleId, issuerId, branchId, cardNumber, cardLastFourDigits, cardrefnumber, batchReference, loadCardStatusId,
                                            distCardStatusId, branchCardStatusId, distBatchId, pinBatchId, threedBatchId, accountNumber, firstName, lastName, cmsId, dateFrom, dateTo, cardIssueMethodId, productId, priorityId, pageIndex,
                                            rowsPerPage, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// search for  customer cards list 
        /// </summary>
        /// <param name="cardrefno"></param>
        /// <param name="customeraccountno"></param>
        /// <param name="pageIndex"></param>
        /// <param name="RowsPerPage"></param>
        /// <returns></returns>
        public List<CustomercardsearchResult> SearchCustomerCardsList(int? issuerid, int? branchid, int? productid, int? cardissuemethodid, int? priorityid, string cardrefno, 
            string customeraccountno, int pageIndex, int RowsPerPage, long auditUserId, string auditWorkstation, bool renewalSearch, bool activationSearch)
        {
            return _cardDAL.SearchCustomerCardsList(issuerid, branchid, productid, cardissuemethodid, priorityid, cardrefno, customeraccountno, pageIndex, RowsPerPage, auditUserId, auditWorkstation, renewalSearch, activationSearch);
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
        public List<CardSearchResult> GetOperatorCardsInProgress(long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetOperatorCardsInProgress(userId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
        }
        /// <summary>
        /// Fetch all request that are work in progress for the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="languageId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<HybridRequestResult> GetOperatorHybridRequestsInProgress(int? statusId, long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetOperatorHybridRequestsInProgress(statusId, auditUserId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
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
        public List<CardSearchResult> GetCardsInError(long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetCardsInError(userId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
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
        public List<CardSearchResult> SearchForReissueCards(long userId, int pageIndex, int rowsPerPage, long audit_user_id, string auditWorkstation)
        {
            return _cardDAL.SearchForReissueCards(userId, pageIndex, rowsPerPage, audit_user_id, auditWorkstation);
        }

        /// <summary>
        /// Used to approve or reject maker checker.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="approved"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool MakerChecker(long cardId, Boolean approved, string notes, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            int cardIssueMethodId;
            var response = _cardDAL.MakerChecker(cardId, approved, notes, auditUserId, auditWorkstation, out cardIssueMethodId);

            SystemArea systemArea;
            if (cardIssueMethodId == 0)
                systemArea = approved ? SystemArea.CENTRALISED_CARD_STATUS_MAKERCHECKER_APPROVED :
                                                   SystemArea.CENTRALISED_CARD_STATUS_MAKERCHECKER_REJECTED;
            else
                systemArea = approved ? SystemArea.ISSUE_CARD_STATUS_MAKERCHECKER_APPROVED :
                                                   SystemArea.ISSUE_CARD_STATUS_MAKERCHECKER_REJECTED;

            responseMessage = _translator.TranslateResponseCode(response, systemArea, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }



        /// <summary>
        /// Used to approve or reject maker checker.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="approved"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool RequestMakerChecker(long RequestId, Boolean approved, string notes, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            int cardIssueMethodId;
            var response = _cardDAL.RequestMakerChecker(RequestId, approved, notes, auditUserId, auditWorkstation, out cardIssueMethodId);

            SystemArea systemArea;
            if (cardIssueMethodId == 0)
                systemArea = approved ? SystemArea.CENTRALISED_CARD_STATUS_MAKERCHECKER_APPROVED :
                                                   SystemArea.CENTRALISED_CARD_STATUS_MAKERCHECKER_REJECTED;
            else
                systemArea = approved ? SystemArea.ISSUE_CARD_STATUS_MAKERCHECKER_APPROVED :
                                                   SystemArea.ISSUE_CARD_STATUS_MAKERCHECKER_REJECTED;

            responseMessage = _translator.TranslateResponseCode(response, systemArea, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Spoil a branch card.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool SpoilBranchCard(long cardId, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.SpoilBranchCard(cardId, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_UPDATE_GENERAL, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Issues a card to a customer.
        /// </summary>
        /// <param name="customerAccount"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool IssueCardToCustomer(CustomerDetails customerAccount, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.IssueCardToCustomer(customerAccount, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool createPinRequest(PinObject pinDetails, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.createPinRequest(pinDetails, auditUserId, auditWorkstation);
            //  responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, language, auditUserId, auditWorkstation);
            responseMessage = "Pin request has been created for this customer. Please have the custodian approve.";
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool updatePinRequestStatus(PinObject pinDetails, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.updatePinRequestStatus(pinDetails, auditUserId, auditWorkstation);
            //  responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, language, auditUserId, auditWorkstation);
            responseMessage = String.Format("Pin request has been updated to {0} for this customer.", pinDetails.PinRequestStatus);
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        //UpdatePinRequestStatusForReissue
        public bool UpdatePinRequestStatusForReissue(PinObject pinDetails, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.UpdatePinRequestStatusForReissue(pinDetails, auditUserId, auditWorkstation);
            //  responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, language, auditUserId, auditWorkstation);
            responseMessage = String.Format("Pin re-issue request has been updated to {0} for this customer. ", pinDetails.PinRequestStatus);
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool updatePinFileStatus(Integration.Common.TerminalCardData PinBlock, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.updatePinFileStatus(PinBlock, auditUserId, auditWorkstation);
            //  responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, language, auditUserId, auditWorkstation);
            responseMessage = String.Format("Status has been updated for pin block {0}.", PinBlock.PINBlock);
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool updateBatchFileStatus(Integration.Common.TerminalCardData PinBlock, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.updateBatchFileStatus(PinBlock, auditUserId, auditWorkstation);
            //  responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, language, auditUserId, auditWorkstation);
            responseMessage = String.Format("Status has been updated for pin reference {0}.", PinBlock.header_batch_reference);
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        //

        //updatePinFileStatus(TerminalCardData PinBlock
        public bool deletePinBlock(string product_pan_bin_code, string pan_last_four, string expiry_date, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.deletePinBlock(product_pan_bin_code, pan_last_four, expiry_date, auditUserId, auditWorkstation);
            //  responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, language, auditUserId, auditWorkstation);
            responseMessage = String.Format("Pin block has been deleted.");
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool CreateRestParams(RestWebservicesHandler restDetails, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.CreateRestParams(restDetails, auditUserId, auditWorkstation);
            //  responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, language, auditUserId, auditWorkstation);
            responseMessage = "REST structure has been configured successfully ...";
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool UpdateRestParams(RestWebservicesHandler restDetails, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.UpdateRestParams(restDetails, auditUserId, auditWorkstation);
            //  responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_RESERVE_CARD, language, auditUserId, auditWorkstation);
            responseMessage = "REST structure has been updated successfully ...";
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
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
        public bool IssueCardPinCaptured(long cardId, long? pinAuthUserId, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.IssueCardPinCaptured(cardId, pinAuthUserId, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_PIN_CAPTURED, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Mark card as PRINTED
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool IssueCardPrinted(long cardId, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.IssueCardPrinted(cardId, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_PRINT_SUCCESS, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Mark card as PRINTED
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool IssueCardPrintError(long cardId, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.IssueCardPrintError(cardId, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_PRINT_FAILED, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Mark card as SPOILT
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool IssueCardSpoil(long cardId, int spoilResaonId, string spoilComments, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.IssueCardSpoil(cardId, spoilResaonId, spoilComments, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_UPDATE_GENERAL, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Mark card as ISSUED.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool IssueCardComplete(long cardId, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.IssueCardComplete(cardId, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.ISSUE_CARD_STATUS_UPDATE_GENERAL, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Log failed CMS linkage, method tried to find appropriate CMS Response Mapping for further processing by user.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="responseError"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public void IssueCardLinkFail(long cardId, string responseError, long auditUserId, string auditWorkstation)
        {
            _cardDAL.IssueCardLinkFail(cardId, responseError, auditUserId, auditWorkstation);
        }

        public void IssueCardLinkRetry(long cardId, string responseError, long auditUserId, string auditWorkstation)
        {
            _cardDAL.IssueCardLinkRetry(cardId, responseError, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Log successful CMS linkage.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public void IssueCardLinkSuccess(long cardId, long auditUserId, string auditWorkstation)
        {
            _cardDAL.IssueCardLinkSuccess(cardId, auditUserId, auditWorkstation);
        }

        public void ActivateCard(long cardId, long auditUserId, string auditWorkstation)
        {
            _cardDAL.ActivateCard(cardId, auditUserId, auditWorkstation);
        }

        //public void PINReissue(int issuerId, int branchId, int productId, string pan, long? authoriseUserId,
        //                            bool failed, string notes, long auditUserId, string auditWorkstation)
        //{
        //    _cardDAL.PINReissue(issuerId, branchId, productId, pan, authoriseUserId, failed, notes, auditUserId, auditWorkstation);
        //}

        public List<PinReissueResult> PINReissueSearch(int? issuerId, int? branchId, int? userRoleId, int? pinReissueStatusesId, int? pin_reissue_type_id, long? operatorUserId, bool operatorInProgress, long? authoriseUserId, DateTime? dateFrom, DateTime? dateTo, int languageId, int pageIndex, int rowsPerpage, long auditUserId, string auditWorkstation)
        {
            if (operatorInProgress)
                operatorUserId = auditUserId;

            return _cardDAL.PINReissueSearch(issuerId, branchId, userRoleId, pinReissueStatusesId, pin_reissue_type_id, operatorUserId, operatorInProgress, authoriseUserId, dateFrom, dateTo, languageId, pageIndex, rowsPerpage, auditUserId, auditWorkstation);
        }


        public PinReissueResult GetPINReissue(long pinReissueId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetPINReissue(pinReissueId, languageId, auditUserId, auditWorkstation);
        }

        public void UpdateCardPVV(long cardId, string pvv, int languageId, long auditUserId, string auditWorkstation)
        {
            _cardDAL.UpdateCardPVV(cardId, pvv, languageId, auditUserId, auditWorkstation);
        }

        public bool RequestPINReissue(int issuerId, int branchId, int productId, string pan, byte[] index, string mobile_number, int pin_reissue_type, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result, out string responseMessage)
        {
            var response = _cardDAL.RequestPINReissue(issuerId, branchId, productId, pan, index, mobile_number, pin_reissue_type, languageId, auditUserId, auditWorkstation, out result);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }


        public bool ApprovePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result, out string responseMessage)
        {
            var response = _cardDAL.ApprovePINReissue(pinReissueId, notes, languageId, auditUserId, auditWorkstation, out result);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }


        public bool RejectPINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result, out string responseMessage)
        {
            var response = _cardDAL.RejectPINReissue(pinReissueId, notes, languageId, auditUserId, auditWorkstation, out result);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }
        public bool CancelPINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result, out string responseMessage)
        {
            var response = _cardDAL.CancelPINReissue(pinReissueId, notes, languageId, auditUserId, auditWorkstation, out result);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }
        public bool ExpirePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result, out string responseMessage)
        {
            var response = _cardDAL.ExpirePINReissue(pinReissueId, notes, languageId, auditUserId, auditWorkstation, out result);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }


        public bool CompletePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result, out string responseMessage)
        {
            var response = _cardDAL.CompletePINReissue(pinReissueId, notes, languageId, auditUserId, auditWorkstation, out result);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        ///// <summary>
        ///// If customer detail is linked to a card it will be returned.
        ///// </summary>
        ///// <param name="cardId"></param>
        ///// <param name="auditUserId"></param>
        ///// <param name="auditWorkstation"></param>
        ///// <returns></returns>
        //public List<CustomerAccountResult> GetCustomerAccDetailForCard(long cardId, long auditUserId, string auditWorkstation)
        //{
        //    return _cardDAL.GetCustomerAccDetailForCard(cardId, auditUserId, auditWorkstation);
        //}


        public List<IProductPrintField> GetProductPrintFields(long? productId, long? cardId, long? requestId)
        {
            List<IProductPrintField> results = new List<IProductPrintField>();
            foreach (var field in _cardDAL.GetProductPrintFields(productId, cardId, requestId))
            {
                results.Add(PrintFieldFactory.CreatePrintField((int)field.print_field_type_id, (int)field.product_field_id,
                                                                field.field_name,
                                                                (float)field.X.GetValueOrDefault(),
                                                                (float)field.Y.GetValueOrDefault(),
                                                                (float)field.width.GetValueOrDefault(),
                                                                (float)field.height.GetValueOrDefault(),
                                                                field.font, field.font_size ?? 0, 0,
                                                                field.mapped_name, field.label, (int)field.max_length,
                                                               (bool)field.editable, (bool)field.deleted, true, 0,
                                                                field.value));
            }

            return results;
        }

        public ProductField[] GetProductFields(long? productId, long? cardId, long? requestId)
        {
            List<ProductField> results = new List<ProductField>();
            foreach (var printField in GetProductPrintFields(productId, cardId, requestId))
            {
                results.Add(new ProductField(printField));
            }

            return results.ToArray();
        }
        public List<IProductPrintField> GetProductFieldsByCardId(long? cardId)
        {
            List<IProductPrintField> results = new List<IProductPrintField>();
            foreach (var printField in GetProductPrintFields(null, cardId, null))
            {
                results.Add(new ProductField(printField));
            }

            return results;
        }

        public List<ProductField> GetProductFieldsByCardIdTransalated(long? cardId)
        {
            List<ProductField> results = new List<ProductField>();
            foreach (var printField in GetProductPrintFields(null, cardId, null))
            {
                results.Add(new ProductField(printField));
            }
            return results;
        }

        public List<IProductPrintField> GetProductFieldsByRequestId(long? requestId)
        {
            List<IProductPrintField> results = new List<IProductPrintField>();
            foreach (var printField in GetProductPrintFields(null, null, requestId))
            {
                results.Add(new ProductField(printField));
            }

            return results;
        }
        /// <summary>
        /// Get card detail, load batch detail, dist batch detail and customer account detail for a card.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public CardDetails GetCardDetails(long cardId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            var cardResult = _cardDAL.GetCardDetails(cardId, checkMasking, languageId, auditUserId, auditWorkstation);

            CardDetails cardDetails = CardDetails.ConvertTo<CardDetails>(cardResult);

            //fetch product fields
            cardDetails.ProductFields = new List<ProductField>();
            cardDetails.ProductFields.AddRange(GetProductFields(null, cardId, null));

            return cardDetails;
        }
        /// <summary>
        /// to get requestdetails for the request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="checkMasking"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public RequestDetails GetRequestDetails(long requestId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            var cardResult = _cardDAL.GetRequestDetails(requestId, checkMasking, languageId, auditUserId, auditWorkstation);

            RequestDetails cardDetails = RequestDetails.ConvertTo<RequestDetails>(cardResult);

            //fetch product fields
            cardDetails.ProductFields = new List<ProductField>();
            cardDetails.ProductFields.AddRange(GetProductFields(null, null, requestId));

            return cardDetails;
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
        public List<CardSearchResult> SearchBranchCards(int? issuerId, int? branchId, int? userRoleId, int? productId, int? priorityId, int? cardIssueMethodId,
                                                         string cardNumber, int? branchCardStatusId, long? operatorUserId,
                                                         int pageIndex, int rowsPerpPage, int? languageId, long auditUserId, string auditWorkstation)
        {
            var results = _cardDAL.SearchBranchCards(issuerId, branchId, userRoleId, productId, priorityId, cardIssueMethodId, cardNumber, branchCardStatusId, operatorUserId, pageIndex, rowsPerpPage, languageId, auditUserId, auditWorkstation);

            //Check that the issuer has a valid licence and only return those cards that have licenced bins.
            //All the cards should be licenced due to load batch only allowing valid cards.

            if (branchId != null)
            {
                var licensedBinCodes = LicenseManager.ValidateAffiliateKeyByBranch(branchId.Value, auditUserId, auditWorkstation);
                List<CardSearchResult> licencedCards = new List<CardSearchResult>();

                foreach (var item in results)
                {
                    var productBin = item.product_bin_code.Trim() + item.sub_product_code.Trim();

                    foreach (var licBin in licensedBinCodes)
                    {
                        if (productBin.Equals(licBin.Trim()))
                        {
                            licencedCards.Add(item);
                            break;
                        }
                    }
                }

                return licencedCards;
            }
            else
            {
                return results;
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
        public List<SearchBranchCardsResult> CheckInOutCards(long operatorUserId, int branchId, int productId, List<long> checkedOutCards, List<long> checkedInCards, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.CheckInOutCards(operatorUserId, branchId, productId, checkedOutCards, checkedInCards, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Generate PDF Report for checked out cards.
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="branchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public byte[] GenerateCheckedOutCardsReport(int? issuerId, long? operatorId, int branchId, int? userRoleId, string username, int languageId, long auditUserId, string auditWorkstation)
        {
            return _cardReports.GenerateCheckedOutCardsReport(issuerId, branchId, userRoleId, operatorId, username, languageId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Generate PDF Report for checked in cards.
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public byte[] GenerateCheckedInCardsReport(int branchId, List<structCard> cardList, string operatorUsername, string username, int languageId, long auditUserId, string auditWorkstation)
        {
            return _cardReports.GenerateCheckedInCardsReport(branchId, cardList, operatorUsername, username, languageId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Card View Page: Get branch card status
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<CardHistoryStatus> GetCardStatusHistory(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            var results = _cardDAL.GetCardStatusHistory(cardId, languageId, auditUserId, auditWorkstation);
            return results;
        }
        /// <summary>
        /// get request status history
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>

        public List<RequestStatusHistoryResult> GetRequestStatusHistory(long requestId, int languageId, long auditUserId, string auditWorkstation)
        {
            var results = _cardDAL.GetRequestStatusHistory(requestId, languageId, auditUserId, auditWorkstation);
            return results;
        }
        public List<RequestReferenceHistoryResult> GetRequestReferenceHistory(long requestId, int languageId, long auditUserId, string auditWorkstation)
        {
            var results = _cardDAL.GetRequestReferenceHistory(requestId, languageId, auditUserId, auditWorkstation);
            return results;
        }
        /// <summary>
        /// Card View Page: Get card reference history
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<CardHistoryReference> GetCardReferenceHistory(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            var results = _cardDAL.GetCardReferenceHistory(cardId, languageId, auditUserId, auditWorkstation);
            return results;
        }

        public ProductField[] GetPrintFieldsByProductid(int productId)
        {
            return GetProductFields(productId, null, null);
        }
        /// <summary>
        ///  to get pin block formats
        /// </summary>
        /// <param name="connectionParamId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<pin_block_formatResult> LookupPinBlockFormat(long auditUserId, string auditWorkstation)
        {
            return _cardDAL.LookupPinBlockFormat(auditUserId, auditWorkstation);
        }
        /// <summary>
        /// get print field types
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>

        public List<LangLookup> LookupPrintFieldTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.LookupPrintFieldTypes(languageId, auditUserId, auditWorkstation);
        }
        public List<BillingReportResult> GetBillingReport(int? IssuerId, string month, string year, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetBillingReport(IssuerId, month, year, auditUserId, auditWorkstation);
        }
        #region EXPOSED METHODS



        public List<LoadCard> GetLoadCards(int issuerID, DateTime _loadDateFrom, DateTime _loadDateTo,
                                           string cardNumberPrefix, string loadBatchReference, string cardStatus)
        {
            return _cardDAL.GetLoadCards(issuerID, _loadDateFrom, _loadDateTo, cardNumberPrefix, loadBatchReference,
                                        cardStatus);
        }

        public bool SaveCMSConnectionConfig(string ipAddress, string portNumber, bool signOn)
        {
            return false; //_cmsInterfaceService.SaveConnectionDetails(ipAddress, portNumber, signOn);
        }



        public List<ProductlistResult> GetProductsList(int issuerID, int? cardIssueMethodId, bool? deleteYN, int pageIndex, int RowsPerpage)
        {
            List<ProductlistResult> productlist = null;

            productlist = _cardDAL.GetProductsList(issuerID, cardIssueMethodId, deleteYN, pageIndex, RowsPerpage);

            return productlist;
        }

        public List<ProductValidated> GetProductsListValidated(int issuerID, int? cardIssueMethodId, int pageIndex, int RowsPerpage, int languageId, long auditUserId, string auditWorkstation)
        {
            List<ProductValidated> productlistValidated = new List<ProductValidated>();

            var productlist = _cardDAL.GetProductsList(issuerID, cardIssueMethodId, false, pageIndex, RowsPerpage);
            //Check that the issuer has a valid licence and only return those cards that have licenced bins.
            //All the cards should be licenced due to load batch only allowing valid cards.

#if DEBUG
            var licensedBinCodes = new List<string>();
#else
            var licensedBinCodes =  LicenseManager.ValidateAffiliateKey(issuerID, auditUserId, auditWorkstation);
#endif

            foreach (var product in productlist)
            {

#if DEBUG
                string product_bin = string.Empty;
                bool valid = true;
                string messages = "";
#else
            string product_bin = string.Empty;               
                if (!string.IsNullOrEmpty(product.sub_product_code) && product.sub_product_code.Length > 0)
                {
                    product_bin = product.product_bin_code + product.sub_product_code;
                }
                else
                {
                    product_bin = product.product_bin_code;
                }
                bool valid = true;
                string messages = "";
                foreach (var bin in licensedBinCodes)
                {
                    if (bin.Trim().ToUpper().Equals(product_bin.Trim().ToUpper().Trim()))
                    {
                        valid = true;
                        break;
                    }
                }
#endif
                productlistValidated.Add(new ProductValidated()
                {
                    ProductCode = product.product_code,
                    ProductId = product.product_id,
                    ProductName = product.product_name,
                    //SubProductIdLength = product.sub_product_id_length,
                    IssuerInstantPinReissue = true,
                    InstantPinReissue = product.enable_instant_pin_reissue_YN,
                    ValidLicence = valid,
                    ePinRequest = product.e_pin_request_YN ?? false,
                    Messages = valid ? "" : "Product is not licenced.",
                    ActivationByCenterOperator = product.activation_by_center_operator.GetValueOrDefault()
                });
            }

            return productlistValidated;
        }

        /// <summary>
        /// getting Font list to fill dropdown
        /// </summary>
        /// <returns></returns>
        public List<Issuer_product_font> GetFontFamilyList()
        {
            return _cardDAL.GetFontFamilyList();
        }

        public List<ServiceRequestCode> GetServiceRequestCode1()
        {
            return _cardDAL.GetServiceRequestCode1();
        }

        public List<ServiceRequestCode1> GetServiceRequestCode2()
        {
            return _cardDAL.GetServiceRequestCode2();
        }

        public List<ServiceRequestCode2> GetServiceRequestCode3()
        {
            return _cardDAL.GetServiceRequestCode3();
        }

        public List<product_currency1> GetCurreniesbyProduct(int Productid)
        {
            return _cardDAL.GetCurreniesbyProduct(Productid);
        }

        public List<product_interface> GetProductInterfaces(int productId, int? interfaceTypeId, int? interfaceAreaId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetProductInterfaces(productId, interfaceTypeId, interfaceAreaId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Caches found product interfaces from the DB.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="interfaceTypeId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public product_interface GetProductInterface(int productId, int interfaceTypeId, long auditUserId, string auditWorkstation)
        {
            product_interface rtn;

            //Check if it's in cache
            if (!_cachedProductInterface.TryGetValue(Tuple.Create<int, int>(productId, interfaceTypeId), out rtn))
            {

            }

            return rtn;
        }

        public List<CurrencyResult> GetCurrencyList(int languageId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetCurrencyList(languageId, auditUserId, auditWorkstation);
        }

        public List<DistBatchFlows> GetDistBatchFlowList(int card_issue_method_id, int languageId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetDistBatchFlowList(card_issue_method_id, languageId, auditUserId, auditWorkstation);
        }

        public ProductResult GetProduct(int productId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetProduct(productId, auditUserId, auditWorkstation);
        }

        public List<ProductCurrencyResult> GetProductCurrencies(int productid, int? currencyId, bool? active, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetProductCurrencies(productid, currencyId, active);
        }

        public List<ProductAccountTypesResult> GetProductAccountTypes(int productId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetProductAccountTypes(productId, languageId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Search for product mappings
        /// </summary>
        /// <param name="productId">May contain null to search all products</param>
        /// <param name="cbs_account_type">May be null to search all cbs account types</param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<ProductAccountTypeMapping> SearchProductAccountTypeMappings(int? productId, string cbsAccountType, int languageId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetProductAccountTypeMappings(productId, cbsAccountType, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Determins if the cbs account mapping has been configured in the system.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="cbs_account_type"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool TryGetProductAccountTypeMapping(int productId, string cbsAccountType, int languageId, long auditUserId, string auditWorkstation, out ProductAccountTypeMapping accountTypeMapping)
        {
            accountTypeMapping = null;

            if (string.IsNullOrWhiteSpace(cbsAccountType))
            {
                throw new ArgumentNullException(nameof(cbsAccountType), string.Format("Value='{0}' may not be null, empty or white space.", cbsAccountType));
            }

            var results = _cardDAL.GetProductAccountTypeMappings(productId, cbsAccountType, auditUserId, auditWorkstation);

            if (results != null && results.Count > 0)
            {
                if (results.Count == 1)
                {
                    accountTypeMapping = results[0];
                    return true;
                }
                else
                {
                    throw new Exception("More than one account type mapping has bee returned. Expected only one result.");
                }
            }

            return false;
        }

        /// <summary>
        /// Persist new product to database.
        /// </summary>
        /// <param name="productlistresult"></param>
        /// <param name="language"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="productId"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public bool InsertProduct(ProductResult productResult, int language, long auditUserId, string auditWorkstation, out long productId, out string responseMessage)
        {
            var response = _cardDAL.InsertProduct(productResult, auditUserId, auditWorkstation, out productId);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.PRODUCT_ADMIN_CREATE, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Persist changes to product to DB.
        /// </summary>
        /// <param name="productlistresult"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool UpdateProduct(ProductResult productResult, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.UpdateProduct(productResult, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.PRODUCT_ADMIN_UPDATE, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        private string DecodeDBResponse(DBResponseMessage dbResponse)
        {
            switch (dbResponse)
            {
                case DBResponseMessage.SUCCESS:
                    return "";
                case DBResponseMessage.INCORRECT_STATUS:
                    return dbResponse.ToString();
                case DBResponseMessage.CARD_ALREADY_ISSUED:
                    return dbResponse.ToString();
                case DBResponseMessage.INCORRECT_BRANCH:
                    return dbResponse.ToString();
                case DBResponseMessage.NO_RECORDS_RETURNED:
                    return dbResponse.ToString();
                case DBResponseMessage.DUPLICATE_RECORD:
                    throw new BaseIndigoException("Duplicate username, please use another username", SystemResponseCode.DUPLICATE_USERNAME, null);
                case DBResponseMessage.SPROC_ERROR:
                    return dbResponse.ToString();
                case DBResponseMessage.SYSTEM_ERROR:
                    return dbResponse.ToString();
                case DBResponseMessage.FAILURE:
                    return dbResponse.ToString();
                default:
                    return dbResponse.ToString();
            }
        }

        public DBResponseMessage DeleteProduct(int Productid, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.DeleteProduct(Productid, auditUserId, auditWorkstation);
        }

        public SystemResponseCode ActivateProduct(int Productid, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.ActivateProduct(Productid, auditUserId, auditWorkstation);
        }

        public List<FontResult> GetFontListBypage(int pageIndex, int RowsPerpage)
        {
            return _cardDAL.GetFontListBypage(pageIndex, RowsPerpage);

        }

        public FontResult GetFont(int fontid)
        {
            return _cardDAL.GetFont(fontid);
        }

        public DBResponseMessage UpdateFont(FontResult fontresult, long auditUserId, string auditWorkstation)
        {

            return _cardDAL.UpdateFont(fontresult, auditUserId, auditWorkstation);



        }

        public long InsertFont(FontResult fontresult, long auditUserId, string auditWorkstation)
        {

            DBResponseMessage dbResponse;
            var newUserId = _cardDAL.InsertFont(fontresult, auditUserId, auditWorkstation, out dbResponse);
            DecodeDBResponse(dbResponse);
            return newUserId;

        }

        public DBResponseMessage DeleteFont(int Productid, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.DeleteFont(Productid, auditUserId, auditWorkstation);
        }

        public DBResponseMessage DeleteSubProduct(int Productid, int subproductid, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.DeleteSubProduct(Productid, subproductid, auditUserId, auditWorkstation);
        }

        public List<SubProduct_Result> GetSubProductList(int issuer_id, int? product_id, int? cardIssueMethidId, Boolean? deletedYN, int pageIndex, int RowsPerpage)
        {
            return _cardDAL.GetSubProductList(issuer_id, product_id, cardIssueMethidId, deletedYN, pageIndex, RowsPerpage);

        }

        public SubProduct_Result GetSubProduct(int? product_id, int sub_productid)
        {
            return _cardDAL.GetSubProduct(product_id, sub_productid);
        }

        public List<ProductFeeAccountingResult> GetFeeAccountingList(int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetFeeAccountingList(issuerId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
        }

        public ProductFeeAccountingResult GetFeeAccounting(int feeAccountingId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetFeeAccounting(feeAccountingId, auditUserId, auditWorkstation);
        }

        public bool CreateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, int languageId, long auditUserId, string auditWorkstation, out string responseMessage, out ProductFeeAccountingResult accountingDetails)
        {
            int feeAccountingId;
            accountingDetails = null;

            var response = _cardDAL.CreateFeeAccounting(feeAccountingDetails, auditUserId, auditWorkstation, out feeAccountingId);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            if (response == SystemResponseCode.SUCCESS)
            {
                accountingDetails = this.GetFeeAccounting(feeAccountingId, auditUserId, auditWorkstation);
                return true;
            }

            return false;
        }

        public bool UpdateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.UpdateFeeAccounting(feeAccountingDetails, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool DeleteFeeAccounting(int feeAccountingId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.DeleteFeeAccounting(feeAccountingId, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public List<FeeSchemeResult> GetFeeSchemes(int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetFeeSchemes(issuerId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
        }

        public FeeSchemeDetails GetFeeSchemeDetails(int feeSchemeId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetFeeSchemeDetails(feeSchemeId, auditUserId, auditWorkstation);
        }

        public List<FeeDetailResult> GetFeeDetails(int feeDetailId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetFeeDetails(feeDetailId, auditUserId, auditWorkstation);
        }

        public List<FeeChargeResult> GetFeeCharges(int feeDetailId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetFeeCharges(feeDetailId, auditUserId, auditWorkstation);
        }

        public List<ProductFeeDetailsResult> GetFeeDetailByProduct(int productId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetFeeDetailByProduct(productId, auditUserId, auditWorkstation);
        }

        public ProductChargeResult GetCurrentFees(int feeDetailId, int currencyId, int CardIssueReasonId, string CBSAccountType, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetCurrentFees(feeDetailId, currencyId, CardIssueReasonId, CBSAccountType, auditUserId, auditWorkstation);
        }

        public bool UpdateFeeCharges(int feeDetailId, List<FeeChargeResult> fees, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.UpdateFeeCharges(feeDetailId, fees, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool InsertFeeScheme(FeeSchemeDetails feeSchemeDetails, int languageId, long auditUserId, string auditWorkstation, out string responseMessage, out FeeSchemeDetails schemeDetails)
        {
            int feeSchemeId;
            schemeDetails = null;

            var response = _cardDAL.InsertFeeScheme(feeSchemeDetails, auditUserId, auditWorkstation, out feeSchemeId);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            if (response == SystemResponseCode.SUCCESS)
            {
                schemeDetails = this.GetFeeSchemeDetails(feeSchemeId, auditUserId, auditWorkstation);
                return true;
            }

            return false;
        }

        public bool UpdateFeeScheme(FeeSchemeDetails feeSchemeDetails, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.UpdateFeeScheme(feeSchemeDetails, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool DeleteFeeScheme(int feeSchemeId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.DeleteFeeScheme(feeSchemeId, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public void UpdateFeeChargeStatus(long cardId, int cardFeeChargeStatusId, string feeReferenceNumber, string feeReversalRefNumber, long auditUserId, string auditWorkstation)
        {
            _cardDAL.UpdateFeeChargeStatus(cardId, cardFeeChargeStatusId, feeReferenceNumber, feeReversalRefNumber, auditUserId, auditWorkstation);
        }

        public bool CreateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.CreateProductPrintFields(productPrintFields, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool UpdateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _cardDAL.UpdateProductPrintFields(productPrintFields, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);
            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public List<ProductPrintFieldResult> GetProductPrintFields(long? productId, long? cardId, long? requestId, long auditUserId, string auditWorkstation)
        {
            return _cardDAL.GetProductPrintFields(productId, cardId, requestId);
        }

#endregion

        public bool CreateCardLimit(long cardId, decimal limit, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            bool successful = false;
            try
            {
                successful = _cardLimitDAL.CreateLimit(cardId, limit);
                responseMessage = "Successfully created limit.";
            }
            catch (Exception exp)
            {
                responseMessage = exp.Message;
            }
            return successful;
        }

        public bool UpdateCardLimit(long cardId, decimal limit, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            bool successful = false;
            try
            {
                successful = _cardLimitDAL.UpdateLimit(cardId, limit);
                responseMessage = "Successfully updated limit.";
            }
            catch (Exception exp)
            {
                responseMessage = exp.Message;
            }
            return successful;
        }

        public bool ApproveCardLimit(long cardId, decimal limit, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            bool successful = false;
            try
            {
                successful = _cardLimitDAL.ApproveLimit(cardId, limit, auditUserId);
                responseMessage = "Successfully approved limit.";
            }
            catch (Exception exp)
            {
                responseMessage = exp.Message;
            }
            return successful;
        }

        public bool SetCreditStatus(long cardId, int creditStatusId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            bool successful = false;
            try
            {
                successful = _cardLimitDAL.SetCreditStatus(cardId, creditStatusId);
                responseMessage = "Successfully approved limit.";
            }
            catch (Exception exp)
            {
                responseMessage = exp.Message;
            }
            return successful;
        }

        public bool ApproveCardLimitManager(long cardId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            bool successful = false;
            try
            {
                successful = _cardLimitDAL.ApproveLimitManager(cardId, auditUserId);
                responseMessage = "Successfully approved limit.";
            }
            catch (Exception exp)
            {
                responseMessage = exp.Message;
                throw exp;
            }
            return successful;
        }

        public CardLimitModel CardLimitGet(long cardId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            CardLimitModel entry = null;
            try
            {
                entry = _cardLimitDAL.GetLimit(cardId);
                responseMessage = "Successfully retrieved limit.";
            }
            catch (Exception exp)
            {
                responseMessage = exp.Message;
            }
            return entry;
        }

        public bool SetCreditContractNumber(long cardId, string creditContractNumber, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            bool successful = false;
            try
            {
                successful = _cardLimitDAL.SetCreditContractNumber(cardId, creditContractNumber, auditUserId);
                responseMessage = "Successfully updated contract number.";
            }
            catch (Exception exp)
            {
                responseMessage = exp.Message;
            }
            return successful;
        }
    }
}
