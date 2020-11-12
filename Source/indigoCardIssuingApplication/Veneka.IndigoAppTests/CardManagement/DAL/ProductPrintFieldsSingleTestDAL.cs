using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.IndigoAppTests.CardManagement.DAL
{
    public class ProductPrintFieldsSingleTestDAL : ICardManagementDAL
    {
        #region The test data
        // -----------------------------------------------------------------------------------------------------------------------------
        private static readonly Tuple<int, List<ProductPrintFieldResult>> _singleRowSingleProductTestData = new Tuple<int, List<ProductPrintFieldResult>>(
            2, new List<ProductPrintFieldResult>()
                {
                    new ProductPrintFieldResult()
                    {
                        deleted = false, editable = false,
                        field_name = "Field name", font = "Arial", font_size = 10, height = 20,
                        label = "label", mapped_name = "mappedname", max_length = 12,
                        print_field_name = "print field name", print_field_type_id = 0,
                        product_field_id = 0, product_id = 2, value = new byte[1] { 1 },
                        width = 15, X = 1, Y = 2
                    }
                });
        /// <summary>
        /// Test data used when product id 2 passed in and card id is null. will return a single record back
        /// </summary>
        /// <returns></returns>
        public static Tuple<int, List<ProductPrintFieldResult>> SingleRowForProductTestData()
        {
            return _singleRowSingleProductTestData;
        }

        // -----------------------------------------------------------------------------------------------------------------------------
        private static readonly Tuple<long, List<ProductPrintFieldResult>> _singleRowSingleCardTestData =
         new Tuple<long, List<ProductPrintFieldResult>>(
             504, new List<ProductPrintFieldResult>() { 
                new ProductPrintFieldResult()
                {
                    deleted = false,
                    editable = false,
                    field_name = "Field name",
                    font = "Arial",
                    font_size = 10,
                    height = 20,
                    label = "label",
                    mapped_name = "mappedname",
                    max_length = 12,
                    print_field_name = "print field name",
                    print_field_type_id = 0,
                    product_field_id = 0,
                    product_id = 1,
                    value = new byte[1] { 1 },
                    width = 15,
                    X = 1,
                    Y = 2
                }
             });
        /// <summary>
        /// Test data used when product id is empty and card if is 504. will return a single record back
        /// </summary>
        /// <returns></returns>
        public static Tuple<long, List<ProductPrintFieldResult>> SingleRowForCardTestData()
        {
            return _singleRowSingleCardTestData;
        }        

        // -----------------------------------------------------------------------------------------------------------------------------
        private static readonly Tuple<int, long, List<ProductPrintFieldResult>> _singleRowSingleProductAndCardTestData =
         new Tuple<int, long, List<ProductPrintFieldResult>>(
             5, 5044, new List<ProductPrintFieldResult>() {
                new ProductPrintFieldResult()
                {
                    deleted = false,
                    editable = false,
                    field_name = "Field name",
                    font = "Arial",
                    font_size = 10,
                    height = 20,
                    label = "label",
                    mapped_name = "mappedname",
                    max_length = 12,
                    print_field_name = "print field name",
                    print_field_type_id = 0,
                    product_field_id = 0,
                    product_id = 1,
                    value = new byte[1] { 1 },
                    width = 15,
                    X = 1,
                    Y = 2
                }
             });
        /// <summary>
        /// Test data used when product id is empty and card if is 504. will return a single record back
        /// </summary>
        /// <returns></returns>
        public static Tuple<int, long, List<ProductPrintFieldResult>> SingleRowSingleProductAndCardTestData()
        {
            return _singleRowSingleProductAndCardTestData;
        }

        // -----------------------------------------------------------------------------------------------------------------------------
        private static readonly Tuple<int, long, List<ProductPrintFieldResult>> _singleRowTestData =
         new Tuple<int, long, List<ProductPrintFieldResult>>(
             5, 8544, new List<ProductPrintFieldResult>() {
                new ProductPrintFieldResult()
                {
                    deleted = false,
                    editable = false,
                    field_name = "Field name",
                    font = "Arial",
                    font_size = 10,
                    height = 20,
                    label = "label",
                    mapped_name = "mappedname",
                    max_length = 12,
                    print_field_name = "print field name",
                    print_field_type_id = 0,
                    product_field_id = 0,
                    product_id = 1,
                    value = new byte[1] { 1 },
                    width = 15,
                    X = 1,
                    Y = 2
                }
             });
        /// <summary>
        /// Test data used when product id is empty and card if is 504. will return a single record back
        /// </summary>
        /// <returns></returns>
        public static Tuple<int, long, List<ProductPrintFieldResult>> SingleRowTestData()
        {
            return _singleRowTestData;
        }
        #endregion

        public List<ProductPrintFieldResult> GetProductPrintFields(long? productId, long? cardId)
        {
            // both null
            if (productId == null && cardId == null)
            {
                return SingleRowTestData().Item3;
            }

            // product data
            if(productId != null && productId == 1 && cardId == null)
            {
                return SingleRowForProductTestData().Item2;
            }


            // card data
            if (cardId != null && cardId == 1 && productId == null)
            {
                return SingleRowForCardTestData().Item2;
            }

            // both card and product
            if (cardId == 1 && productId == 1)
            {
                return SingleRowSingleProductAndCardTestData().Item3;
            }

            throw new NotImplementedException("Looks like we shouldnt have got here. Please check your test code!");
        }               

        #region Not needed, throw NotImplementedException
        public SystemResponseCode ActivateProduct(int Productid, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode ApprovePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode CancelPINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode CheckCustomerAccountBalance(Indigo.Integration.Objects.CustomerDetails customer, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<SearchBranchCardsResult> CheckInOutCards(long operatorUserId, int branchId, int productId, List<long> checkedOutCards, List<long> checkedInCards, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode CompletePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode CreateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, long auditUserId, string auditWorkstation, out int feeAccountingId)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode CreateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode DeleteFeeAccounting(int feeAccountingId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode DeleteFeeScheme(int feeSchemeId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public DBResponseMessage DeleteFont(int Fontid, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public DBResponseMessage DeleteProduct(int Productid, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public DBResponseMessage DeleteSubProduct(int Productid, int subproductid, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode ExpirePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            throw new NotImplementedException();
        }

        public CardDetailResult GetCardDetails(long cardId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<card_priority> GetCardPriorityList(int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<CardHistoryReference> GetCardReferenceHistory(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<CardSearchResult> GetCardsInError(long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<CardHistoryStatus> GetCardStatusHistory(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<CurrencyResult> GetCurrencyList(int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<product_currency1> GetCurreniesbyProduct(int productid)
        {
            throw new NotImplementedException();
        }

        public ProductChargeResult GetCurrentFees(int feeDetailId, int currencyId, int CardIssueReasonId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public CustomerDetailsResult GetCustomerDetails(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<DistBatchFlows> GetDistBatchFlowList(int card_issue_method_id, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public ProductFeeAccountingResult GetFeeAccounting(int feeAccountingId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<ProductFeeAccountingResult> GetFeeAccountingList(int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<FeeChargeResult> GetFeeCharges(int feeDetailId, int issueReasonId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<ProductFeeDetailsResult> GetFeeDetailByProduct(int productId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<FeeDetailResult> GetFeeDetails(int feeSchemeId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public FeeSchemeDetails GetFeeSchemeDetails(int feeSchemeId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<FeeSchemeResult> GetFeeSchemes(int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public FontResult GetFont(int fontid)
        {
            throw new NotImplementedException();
        }

        public List<Issuer_product_font> GetFontFamilyList()
        {
            throw new NotImplementedException();
        }

        public List<FontResult> GetFontListBypage(int pageIndex, int RowsPerpage)
        {
            throw new NotImplementedException();
        }

        public List<IssueCard> GetIssueCards(string user, int issuerID, string firstname, string lastname, string customerAccount, DateTime dateFrom, DateTime dateTo, DistCardStatus status)
        {
            throw new NotImplementedException();
        }

        public List<LoadCard> GetLoadCards(int issuerID, DateTime _loadDateFrom, DateTime _loadDateTo, string cardNumberPrefix, string loadBatchReference, string loadCardStatus)
        {
            throw new NotImplementedException();
        }

        public List<CardSearchResult> GetOperatorCardsInProgress(long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public PinReissueResult GetPINReissue(long pinReissueId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public ProductResult GetProduct(int productId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }        

        public List<ProductAccountTypesResult> GetProductAccountTypes(int productId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<ProductCurrencyResult> GetProductCurrencies(int productid, int? currencyId, bool? active)
        {
            throw new NotImplementedException();
        }

        public List<ProductExternalSystemResult> GetProductExternalFields(int productid, int? currencyId, bool? active)
        {
            throw new NotImplementedException();
        }

        public List<product_interface> GetProductInterfaces(int productId, int? interfaceTypeId, int? interfaceAreaId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<ProductIssueReasonsResult> GetProductIssueReasons(int productId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<ProductlistResult> GetProductsList(int issuerID, int? cardIssueMethodId, bool? deletedYN, int pageIndex, int RowsPerpage)
        {
            throw new NotImplementedException();
        }

        public string GetPWKey(int issuerID)
        {
            throw new NotImplementedException();
        }

        public List<ServiceRequestCode> GetServiceRequestCode1()
        {
            throw new NotImplementedException();
        }

        public List<ServiceRequestCode1> GetServiceRequestCode2()
        {
            throw new NotImplementedException();
        }

        public List<ServiceRequestCode2> GetServiceRequestCode3()
        {
            throw new NotImplementedException();
        }

        public SubProduct_Result GetSubProduct(int? product_id, int sub_productid)
        {
            throw new NotImplementedException();
        }

        public List<SubProduct_Result> GetSubProductList(int issuer_id, int? product_id, int? cardIssueMethidId, bool? deletedYN, int pageIndex, int RowsPerpage)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode InsertFeeScheme(FeeSchemeDetails feeSchemeDetails, long auditUserId, string auditWorkstation, out int feeSchemeId)
        {
            throw new NotImplementedException();
        }

        public long InsertFont(FontResult fontresult, long auditUserId, string auditWorkstation, out DBResponseMessage dbResponse)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode InsertProduct(ProductResult productResult, long auditUserId, string auditWorkstation, out long productId)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode IssueCardComplete(long cardId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public void IssueCardLinkFail(long cardId, string responseError, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public void IssueCardLinkRetry(long cardId, string responseError, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public void IssueCardLinkSuccess(long cardId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode IssueCardPinCaptured(long cardId, long? pinAuthUserId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode IssueCardPrinted(long cardId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode IssueCardPrintError(long cardId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode IssueCardSpoil(long cardId, int spoilResaonId, string spoilComments, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode IssueCardToCustomer(Indigo.Integration.Objects.CustomerDetails customerAccount, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<branch_card_codes> ListBranchCardCodes(int BranchCardCodeType, bool isException, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<pin_block_formatResult> LookupPinBlockFormat(long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<LangLookup> LookupPrintFieldTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode MakerChecker(long cardId, bool approved, string notes, long auditUserId, string auditWorkstation, out int cardIssueMethodId)
        {
            throw new NotImplementedException();
        }

        public List<PinReissueResult> PINReissueSearch(int? issuerId, int? branchId, int? userRoleId, int? pinReissueStatusId, long? operatorUserId, bool operatorInProgress, long? authoriseUserId, DateTime? dateFrom, DateTime? dateTo, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode RejectPINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode RequestCardForCustomer(int delivery_branch_id, int branch_id, int product_id, int? card_priority_id, string customer_account_number, int domicile_branch_id, int? account_type_id, int? card_issue_reason_id, string customer_first_name, string customer_middle_name, string customer_last_name, string name_on_card, int customer_title_id, int? currency_id, int? resident_id, int? customer_type_id, string cms_id, string contract_number, string idnumber, string contact_number, string customer_id, bool? fee_waiver_YN, bool? fee_editable_YN, decimal? fee_charged, bool? fee_overridden_YN, List<IProductPrintField> productFields, long auditUserId, string auditWorkstation, out long cardId)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode RequestPINReissue(int issuerId, int branchId, int productId, string pan, byte[] index, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            throw new NotImplementedException();
        }

        public string ReserveCardForCustomer(CardIssueRequest reserveRequest, DistCardStatus nextReservationStage)
        {
            throw new NotImplementedException();
        }

        public string ReserveLoadCardsForDistBatch(List<string> cardList, string batchReference, string currentCardStatus, string newLoadCardStatus)
        {
            throw new NotImplementedException();
        }

        public List<CardSearchResult> SearchBranchCards(int? issuer_id, int? branchId, int? user_role_id, int? productId, int? priorityId, int? cardIssueMethodId, string cardNumber, int? branchCardStatusId, long? operatorUserId, int pageIndex, int rowsPerpPage, int? languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<CustomercardsearchResult> SearchCustomerCardsList(int? issuerid, int? branchid, int? productid, int? cardissuemethodid, int? priorityid, string cardrefno, string customeraccountno, int pageIndex, int RowsPerPage, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<CardSearchResult> SearchForCards(long userId, int? userRoleId, int? issuerId, int? branchId, string cardNumber, string cardLastFourDigits, string cardrefnumber, string batchReference, int? loadCardStatusId, int? distCardStatusId, int? branchCardStatusId, long? distBatchId, long? pinBatchId, string accountNumber, string firstName, string lastName, string cmsId, DateTime? dateFrom, DateTime? dateTo, int? cardIssueMethodId, int? productId, int? priorityId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<CardSearchResult> SearchForReissueCards(long userId, int pageIndex, int rowsPerPage, long audit_user_id, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode SpoilBranchCard(long cardId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public void UpdateCardPVV(long cardId, string pvv, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode UpdateCustomerDetails(long cardId, long customerAccountId, Indigo.Integration.Objects.CustomerDetails customerDetails, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode UpdateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode UpdateFeeCharges(int feeDetailId, int cardIssueReasonId, List<FeeChargeResult> fees, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public void UpdateFeeChargeStatus(long cardId, int cardFeeChargeStatusId, string feeReferenceNumber, string feeReversalRefNumber, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode UpdateFeeScheme(FeeSchemeDetails feeSchemeDetails, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public DBResponseMessage UpdateFont(FontResult fontresult, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public void UpdatePrintFieldValue(IProductPrintField[] printfields, long customeraccountid, long audituserid, string auditworkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode UpdateProduct(ProductResult productResult, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode UpdateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<accounttypes_mappings_Result> GetProductAccountTypeMappings(int? productId, string cbsAccountType, long audit_userId, string audit_workstation)
        {
            throw new NotImplementedException();
        }

        public ProductChargeResult GetCurrentFees(int feeDetailId, int currencyId, int CardIssueReasonId, string CBSAccounType, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<FeeChargeResult> GetFeeCharges(int feeDetailId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<HybridRequestResult> GetOperatorHybridRequestsInProgress(int? statusId, long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        List<ProductAccountTypeMapping> ICardManagementDAL.GetProductAccountTypeMappings(int? productId, string cbsAccountType, long audit_userId, string audit_workstation)
        {
            throw new NotImplementedException();
        }

        public List<ProductPrintFieldResult> GetProductPrintFields(long? productId, long? cardId, long? requestId)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode RequestHybridCardForCustomer(int delivery_branch_id, int branch_id, int product_id, int? card_priority_id, string customer_account_number, int domicile_branch_id, int? account_type_id, int? card_issue_reason_id, string customer_first_name, string customer_middle_name, string customer_last_name, string name_on_card, int customer_title_id, int? currency_id, int? resident_id, int? customer_type_id, string cms_id, string contract_number, string idnumber, string contact_number, string customer_id, bool? fee_waiver_YN, bool? fee_editable_YN, decimal? fee_charged, bool? fee_overridden_YN, List<IProductPrintField> productFields, int card_issue_method_id, string cbsaccounttype, long auditUserId, string auditWorkstation, out long requestId)
        {
            throw new NotImplementedException();
        }

        public List<RequestStatusHistoryResult> GetRequestStatusHistory(long RequestId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public RequestDetailResult GetRequestDetails(long cardId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<RequestReferenceHistoryResult> GetRequestReferenceHistory(long RequestId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode RequestMakerChecker(long RequestId, bool approved, string notes, long auditUserId, string auditWorkstation, out int cardIssueMethodId)
        {
            throw new NotImplementedException();
        }

        public List<PinReissueResult> PINReissueSearch(int? issuerId, int? branchId, int? userRoleId, int? pinReissueStatusId, int? pin_reissue_type_id, long? operatorUserId, bool operatorInProgress, long? authoriseUserId, DateTime? dateFrom, DateTime? dateTo, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode RequestCardForCustomer(int delivery_branch_id, int branch_id, int product_id, int? card_priority_id, string customer_account_number, int domicile_branch_id, int? account_type_id, int? card_issue_reason_id, string customer_first_name, string customer_middle_name, string customer_last_name, string name_on_card, int customer_title_id, int? currency_id, int? resident_id, int? customer_type_id, string cms_id, string contract_number, string idnumber, string contact_number, string customer_id, bool? fee_waiver_YN, bool? fee_editable_YN, decimal? fee_charged, bool? fee_overridden_YN, List<IProductPrintField> productFields, int card_issue_method_id, long auditUserId, string auditWorkstation, out long cardId)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode RequestInstantCardForCustomer(int delivery_branch_id, int branch_id, int product_id, int? card_priority_id, string customer_account_number, int domicile_branch_id, int? account_type_id, int? card_issue_reason_id, string customer_first_name, string customer_middle_name, string customer_last_name, string name_on_card, int customer_title_id, int? currency_id, int? resident_id, int? customer_type_id, string cms_id, string contract_number, string idnumber, string contact_number, string customer_id, bool? fee_waiver_YN, bool? fee_editable_YN, decimal? fee_charged, bool? fee_overridden_YN, List<IProductPrintField> productFields, int card_issue_method_id, long auditUserId, string auditWorkstation, out long cardId, out long PrintJobId, out long new_customer_account_id)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode RequestPINReissue(int issuerId, int branchId, int productId, string pan, byte[] index, string mobile_number, int? pin_reissue_type, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            throw new NotImplementedException();
        }

        public List<CardSearchResult> SearchForCards(long userId, int? userRoleId, int? issuerId, int? branchId, string cardNumber, string cardLastFourDigits, string cardrefnumber, string batchReference, int? loadCardStatusId, int? distCardStatusId, int? branchCardStatusId, long? distBatchId, long? pinBatchId, long? threedBatchId, string accountNumber, string firstName, string lastName, string cmsId, DateTime? dateFrom, DateTime? dateTo, int? cardIssueMethodId, int? productId, int? priorityId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode UpdateFeeCharges(int feeDetailId, List<FeeChargeResult> fees, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<BillingReportResult> GetBillingReport(int? IssuerId, string month, string year, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
