using System;
using System.Collections.Generic;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Models.IssuerManagement;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.CardManagement.dal
{
    public interface ICardManagementDAL
    {
        SystemResponseCode ActivateProduct(int Productid, long auditUserId, string auditWorkstation);
        SystemResponseCode ApprovePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result);
        SystemResponseCode CancelPINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result);
        SystemResponseCode CheckCustomerAccountBalance(CustomerDetails customer, long auditUserId, string auditWorkstation);
        List<SearchBranchCardsResult> CheckInOutCards(long operatorUserId, int branchId, int productId, List<long> checkedOutCards, List<long> checkedInCards, long auditUserId, string auditWorkstation);
        SystemResponseCode CompletePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result);
        SystemResponseCode CreateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, long auditUserId, string auditWorkstation, out int feeAccountingId);
        SystemResponseCode CreateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, long auditUserId, string auditWorkstation);
        SystemResponseCode DeleteFeeAccounting(int feeAccountingId, long auditUserId, string auditWorkstation);
        SystemResponseCode DeleteFeeScheme(int feeSchemeId, long auditUserId, string auditWorkstation);
        DBResponseMessage DeleteFont(int Fontid, long auditUserId, string auditWorkstation);
        DBResponseMessage DeleteProduct(int Productid, long auditUserId, string auditWorkstation);
        DBResponseMessage DeleteSubProduct(int Productid, int subproductid, long auditUserId, string auditWorkstation);
        SystemResponseCode ExpirePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result);
        CardDetailResult GetCardDetails(long cardId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation);
        List<card_priority> GetCardPriorityList(int languageId, long auditUserId, string auditWorkstation);
        List<CardHistoryReference> GetCardReferenceHistory(long cardId, int languageId, long auditUserId, string auditWorkstation);
        List<CardSearchResult> GetCardsInError(long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation);
        List<CardHistoryStatus> GetCardStatusHistory(long cardId, int languageId, long auditUserId, string auditWorkstation);
        List<CurrencyResult> GetCurrencyList(int languageId, long auditUserId, string auditWorkstation);
        List<product_currency1> GetCurreniesbyProduct(int productid);
        ProductChargeResult GetCurrentFees(int feeDetailId, int currencyId, int CardIssueReasonId, string CBSAccounType, long auditUserId, string auditWorkstation);
        CustomerDetailsResult GetCustomerDetails(long cardId, int languageId, long auditUserId, string auditWorkstation);
        List<DistBatchFlows> GetDistBatchFlowList(int card_issue_method_id, int languageId, long auditUserId, string auditWorkstation);
        ProductFeeAccountingResult GetFeeAccounting(int feeAccountingId, long auditUserId, string auditWorkstation);
        List<ProductFeeAccountingResult> GetFeeAccountingList(int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation);
        List<FeeChargeResult> GetFeeCharges(int feeDetailId, long auditUserId, string auditWorkstation);
        List<ProductFeeDetailsResult> GetFeeDetailByProduct(int productId, long auditUserId, string auditWorkstation);
        List<FeeDetailResult> GetFeeDetails(int feeSchemeId, long auditUserId, string auditWorkstation);
        FeeSchemeDetails GetFeeSchemeDetails(int feeSchemeId, long auditUserId, string auditWorkstation);
        List<FeeSchemeResult> GetFeeSchemes(int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation);
        FontResult GetFont(int fontid);
        List<Issuer_product_font> GetFontFamilyList();
        List<HybridRequestResult> GetOperatorHybridRequestsInProgress(int? statusId, long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation);
        List<FontResult> GetFontListBypage(int pageIndex, int RowsPerpage);
        List<IssueCard> GetIssueCards(string user, int issuerID, string firstname, string lastname, string customerAccount, DateTime dateFrom, DateTime dateTo, DistCardStatus status);
        List<LoadCard> GetLoadCards(int issuerID, DateTime _loadDateFrom, DateTime _loadDateTo, string cardNumberPrefix, string loadBatchReference, string loadCardStatus);
        List<CardSearchResult> GetOperatorCardsInProgress(long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation);
        PinReissueResult GetPINReissue(long pinReissueId, int languageId, long auditUserId, string auditWorkstation);
        ProductResult GetProduct(int productId, long auditUserId, string auditWorkstation);
        List<ProductAccountTypeMapping> GetProductAccountTypeMappings(int? productId, string cbsAccountType, long audit_userId, string audit_workstation);
        List<ProductAccountTypesResult> GetProductAccountTypes(int productId, int languageId, long auditUserId, string auditWorkstation);
        List<ProductCurrencyResult> GetProductCurrencies(int productid, int? currencyId, bool? active);
        List<ProductExternalSystemResult> GetProductExternalFields(int productid, int? currencyId, bool? active);
        List<product_interface> GetProductInterfaces(int productId, int? interfaceTypeId, int? interfaceAreaId, long auditUserId, string auditWorkstation);
        List<ProductIssueReasonsResult> GetProductIssueReasons(int productId, int languageId, long auditUserId, string auditWorkstation);
        List<ProductPrintFieldResult> GetProductPrintFields(long? productId, long? cardId, long? requestId);
        List<ProductlistResult> GetProductsList(int issuerID, int? cardIssueMethodId, bool? deletedYN, int pageIndex, int RowsPerpage);
        string GetPWKey(int issuerID);
        SystemResponseCode RequestHybridCardForCustomer(int delivery_branch_id, int branch_id, int product_id, int? card_priority_id, string customer_account_number,
            int domicile_branch_id, int? account_type_id, int? card_issue_reason_id, string customer_first_name,
            string customer_middle_name, string customer_last_name, string name_on_card, int customer_title_id,
            int? currency_id, int? resident_id, int? customer_type_id, string cms_id, string contract_number, string idnumber, string contact_number,
            string customer_id, bool? fee_waiver_YN, bool? fee_editable_YN, decimal? fee_charged, bool? fee_overridden_YN, List<IProductPrintField> productFields, int card_issue_method_id,
            string cbsaccounttype, long auditUserId, string auditWorkstation, out long requestId);
        List<RequestStatusHistoryResult> GetRequestStatusHistory(long RequestId, int languageId, long auditUserId, string auditWorkstation);

        RequestDetailResult GetRequestDetails(long cardId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation);
        List<RequestReferenceHistoryResult> GetRequestReferenceHistory(long RequestId, int languageId, long auditUserId, string auditWorkstation);
        SystemResponseCode RequestMakerChecker(long RequestId, Boolean approved, string notes, long auditUserId, string auditWorkstation, out int cardIssueMethodId);

        List<ServiceRequestCode> GetServiceRequestCode1();
        List<ServiceRequestCode1> GetServiceRequestCode2();
        List<ServiceRequestCode2> GetServiceRequestCode3();
        SubProduct_Result GetSubProduct(int? product_id, int sub_productid);
        List<SubProduct_Result> GetSubProductList(int issuer_id, int? product_id, int? cardIssueMethidId, bool? deletedYN, int pageIndex, int RowsPerpage);
        SystemResponseCode InsertFeeScheme(FeeSchemeDetails feeSchemeDetails, long auditUserId, string auditWorkstation, out int feeSchemeId);
        long InsertFont(FontResult fontresult, long auditUserId, string auditWorkstation, out DBResponseMessage dbResponse);
        SystemResponseCode InsertProduct(ProductResult productResult, long auditUserId, string auditWorkstation, out long productId);
        SystemResponseCode IssueCardComplete(long cardId, long auditUserId, string auditWorkstation);
        void IssueCardLinkFail(long cardId, string responseError, long auditUserId, string auditWorkstation);
        void IssueCardLinkRetry(long cardId, string responseError, long auditUserId, string auditWorkstation);
        void IssueCardLinkSuccess(long cardId, long auditUserId, string auditWorkstation);
        SystemResponseCode IssueCardPinCaptured(long cardId, long? pinAuthUserId, long auditUserId, string auditWorkstation);
        SystemResponseCode IssueCardPrinted(long cardId, long auditUserId, string auditWorkstation);
        SystemResponseCode IssueCardPrintError(long cardId, long auditUserId, string auditWorkstation);
        SystemResponseCode IssueCardSpoil(long cardId, int spoilResaonId, string spoilComments, long auditUserId, string auditWorkstation);
        SystemResponseCode IssueCardToCustomer(CustomerDetails customerAccount, long auditUserId, string auditWorkstation);
        SystemResponseCode createPinRequest(PinObject pinDetails, long auditUserId, string auditWorkstation);
        SystemResponseCode updatePinRequestStatus(PinObject pinDetails, long auditUserId, string auditWorkstation);
        //UpdatePinRequestStatusForReissue
        SystemResponseCode UpdatePinRequestStatusForReissue(PinObject pinDetails, long auditUserId, string auditWorkstation);
        SystemResponseCode updatePinFileStatus(Integration.Common.TerminalCardData PinBlock, long auditUserId, string auditWorkstation);
        SystemResponseCode updateBatchFileStatus(Integration.Common.TerminalCardData PinBlock, long auditUserId, string auditWorkstation);
        SystemResponseCode deletePinBlock(string product_pan_bin_code, string pan_last_four, string expiry_date, long auditUserId, string auditWorkstation);
        SystemResponseCode CreateRestParams(RestWebservicesHandler restDetails, long auditUserId, string auditWorkstation);
        SystemResponseCode UpdateRestParams(RestWebservicesHandler restDetails, long auditUserId, string auditWorkstation);
        List<branch_card_codes> ListBranchCardCodes(int BranchCardCodeType, bool isException, long auditUserId, string auditWorkstation);
        List<pin_block_formatResult> LookupPinBlockFormat(long auditUserId, string auditWorkstation);
        List<LangLookup> LookupPrintFieldTypes(int languageId, long auditUserId, string auditWorkstation);
        SystemResponseCode MakerChecker(long cardId, bool approved, string notes, long auditUserId, string auditWorkstation, out int cardIssueMethodId);
        List<PinReissueResult> PINReissueSearch(int? issuerId, int? branchId, int? userRoleId, int? pinReissueStatusId, int? pin_reissue_type_id, long? operatorUserId, bool operatorInProgress, long? authoriseUserId, DateTime? dateFrom, DateTime? dateTo, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation);
        SystemResponseCode RejectPINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result);
        SystemResponseCode RequestCardForCustomer(int delivery_branch_id, int branch_id, int product_id, int? card_priority_id, string customer_account_number,
            int domicile_branch_id, int? account_type_id, int? card_issue_reason_id, string customer_first_name,
            string customer_middle_name, string customer_last_name, string name_on_card, int customer_title_id,
            int? currency_id, int? resident_id, int? customer_type_id, string cms_id, string contract_number, string idnumber, string contact_number, string email_address,
            string customer_id, bool? fee_waiver_YN, bool? fee_editable_YN, decimal? fee_charged, decimal? vat, decimal? vat_charged, decimal? total_charged,
            bool? fee_overridden_YN, List<IProductPrintField> productFields, int card_issue_method_id, long? renewal_detail_id, string cbs_account_type,
            long auditUserId, string auditWorkstation, out long cardId);


        SystemResponseCode RequestInstantCardForCustomer(int delivery_branch_id, int branch_id, int product_id, int? card_priority_id, string customer_account_number,
          int domicile_branch_id, int? account_type_id, int? card_issue_reason_id, string customer_first_name,
          string customer_middle_name, string customer_last_name, string name_on_card, int customer_title_id,
          int? currency_id, int? resident_id, int? customer_type_id, string cms_id, string contract_number, string idnumber, string contact_number, string email_address,
          string customer_id, bool? fee_waiver_YN, bool? fee_editable_YN, decimal? fee_charged, bool? fee_overridden_YN, List<IProductPrintField> productFields, int card_issue_method_id,
          long auditUserId, string auditWorkstation, out long cardId, out long PrintJobId, out long new_customer_account_id);
        SystemResponseCode RequestPINReissue(int issuerId, int branchId, int productId, string pan, byte[] index, string mobile_number, int? pin_reissue_type, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result);
        string ReserveCardForCustomer(CardIssueRequest reserveRequest, DistCardStatus nextReservationStage);
        string ReserveLoadCardsForDistBatch(List<string> cardList, string batchReference, string currentCardStatus, string newLoadCardStatus);
        List<CardSearchResult> SearchBranchCards(int? issuer_id, int? branchId, int? user_role_id, int? productId, int? priorityId, int? cardIssueMethodId, string cardNumber, int? branchCardStatusId, long? operatorUserId, int pageIndex, int rowsPerpPage, int? languageId, long auditUserId, string auditWorkstation);
        List<CustomercardsearchResult> SearchCustomerCardsList(int? issuerid, int? branchid, int? productid, int? cardissuemethodid, int? priorityid, string cardrefno, 
            string customeraccountno, int pageIndex, int RowsPerPage, long auditUserId, string auditWorkstation, bool renewalSearch, bool activationSearch);

        List<CardSearchResult> SearchForCards(long userId, int? userRoleId, int? issuerId, int? branchId, string cardNumber,
                                                        string cardLastFourDigits, string cardrefnumber, string batchReference,
                                                        int? loadCardStatusId, int? distCardStatusId, int? branchCardStatusId, long? distBatchId, long? pinBatchId, long? threedBatchId,
                                                        string accountNumber, string firstName, string lastName, string cmsId,
                                                        DateTime? dateFrom, DateTime? dateTo, int? cardIssueMethodId,
                                                        int? productId, int? priorityId, int pageIndex, int rowsPerPage,
                                                        long auditUserId, string auditWorkstation);
        List<CardSearchResult> SearchForReissueCards(long userId, int pageIndex, int rowsPerPage, long audit_user_id, string auditWorkstation);
        SystemResponseCode SpoilBranchCard(long cardId, long auditUserId, string auditWorkstation);
        void UpdateCardPVV(long cardId, string pvv, int languageId, long auditUserId, string auditWorkstation);
        SystemResponseCode UpdateCustomerDetails(long cardId, long customerAccountId, CustomerDetails customerDetails, long auditUserId, string auditWorkstation);
        SystemResponseCode UpdateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, long auditUserId, string auditWorkstation);
        SystemResponseCode UpdateFeeCharges(int feeDetailId, List<FeeChargeResult> fees, long auditUserId, string auditWorkstation);
        void UpdateFeeChargeStatus(long cardId, int cardFeeChargeStatusId, string feeReferenceNumber, string feeReversalRefNumber, long auditUserId, string auditWorkstation);
        SystemResponseCode UpdateFeeScheme(FeeSchemeDetails feeSchemeDetails, long auditUserId, string auditWorkstation);
        DBResponseMessage UpdateFont(FontResult fontresult, long auditUserId, string auditWorkstation);
        void UpdatePrintFieldValue(IProductPrintField[] printfields, long customeraccountid, long audituserid, string auditworkstation);
        SystemResponseCode UpdateProduct(ProductResult productResult, long auditUserId, string auditWorkstation);
        SystemResponseCode UpdateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, long auditUserId, string auditWorkstation);
        List<BillingReportResult> GetBillingReport(int? IssuerId, string month, string year, long auditUserId, string auditWorkstation);
        bool ActivateCard(long cardId, long auditUserId, string auditWorkstation);
    }
}