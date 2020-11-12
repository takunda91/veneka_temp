using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.Integration.Remote;

namespace Veneka.IndigoAppTests.IndigoCardIssuanceService.bll.TestData.IssuerCardController.AccountLookup
{
    public class AccountLookupData
    {
        #region Integration Controller
        public class AccountLookup_IntegrationController : IIntegrationController
        {
            public ICardManagementSystem CardManagementSystem(int productId, InterfaceArea interfaceArea, out ExternalSystemFields externalFields, out IConfig config)
            {
                config = null;
                return new CMS();
            }

            public ICoreBankingSystem CoreBankingSystem(int productId, InterfaceArea interfaceArea, out ExternalSystemFields externalFields, out IConfig config)
            {
                config = null;
                return new CBS();
            }

            #region CBS
            public class CBS : ICoreBankingSystem
            {
                public static AccountDetails CreateAccountDetails(string accountNumber, List<IProductPrintField> printFields)
                {
                    return new AccountDetails()
                    {
                        AccountNumber = accountNumber,
                        CBSAccountTypeId = "503",
                        CurrencyId = 1,
                        FirstName = "FirstName",
                        MiddleName = "MiddleName",
                        LastName = "LastName",
                        ProductFields = printFields.Select(s => new Indigo.Integration.ProductPrinting.ProductField(s)).ToList(),
                        AccountBalance = 10,
                        Address1 = "Address1",
                        Address2 = "Address2",
                        Address3 = "Address3",
                        CardIssueMethodId = 1,
                        CardPriorityId = 1,
                        CmsID = "CmsId",
                        ContactNumber = "ContactNumber",
                        ContractNumber = "ContractNumber",
                        CustomerId = "CustomerId",
                        CustomerIDNumber = "CustomerIdNumber",
                        CustomerResidencyId = 1,
                        CustomerTitleId = 1,
                        CustomerTypeId = 1,
                        PriorityId = 1
                    };
                }

                public bool GetAccountDetail(string accountNumber, List<IProductPrintField> printFields, int cardIssueReasonId, int issuerId, int branchId, int productId, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out AccountDetails accountDetails, out string responseMessage)
                {
                    responseMessage = "Success";
                    accountDetails = CreateAccountDetails(accountNumber, printFields);

                    return true;
                }

                #region Not Used
                public string SQLConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
                public DirectoryInfo IntegrationFolder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
                public IDataSource DataSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

                public bool ChargeFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string feeRefrenceNumber, out string responseMessage)
                {
                    throw new NotImplementedException();
                }

                public bool CheckBalance(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
                {
                    throw new NotImplementedException();
                }

                public bool ReverseFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
                {
                    throw new NotImplementedException();
                }

                public bool UpdateAccount(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
                {
                    throw new NotImplementedException();
                }
                #endregion
            }
            #endregion

            #region CMS
            public class CMS : ICardManagementSystem
            {
                public bool AccountLookup(int issuerId, int productId, int cardIssueReasonId, string accountNumber, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, ref AccountDetails accountDetails, out string responseMessage)
                {
                    responseMessage = "Success";
                    return true;
                }

                #region Not Used
                public bool OnUploadCompletedSubscribed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
                public string SQLConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
                public DirectoryInfo IntegrationFolder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
                public IDataSource DataSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

                public event EventHandler<DistEventArgs> OnUploadCompleted;

                public LinkResponse LinkCardToAccount(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
                {
                    throw new NotImplementedException();
                }

                public bool RemoteFetchDetails(List<CardDetail> cardDetails, ExternalSystemFields externalFields, IConfig config, out List<CardDetailResponse> failedCards, out string responseMessage)
                {
                    throw new NotImplementedException();
                }

                public bool SpoilCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
                {
                    throw new NotImplementedException();
                }

                public bool UpdatePVV(int issuerId, int productId, Track2 track2, string PVV, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
                {
                    throw new NotImplementedException();
                }

                public bool UploadGeneratedCards(List<CardObject> cardObjects, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
                {
                    throw new NotImplementedException();
                }

                public bool ValidateCustomerDetails(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
                {
                    throw new NotImplementedException();
                }

                public LinkResponse LinkCardsToAccount(List<CustomerDetails> customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out Dictionary<long, LinkResponse> response, out string responseMessage)
                {
                    throw new NotImplementedException();
                }

                public LinkResponse LinkCardToAccountAndActive(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
                {
                    throw new NotImplementedException();
                }

                public LinkResponse ActiveCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
                {
                    throw new NotImplementedException();
                }
                #endregion
            }
            #endregion

            #region Not Used
            public ICardProductionSystem CardProductionSystem(int productId, InterfaceArea interfaceArea, out ExternalSystemFields externalFields, out IConfig config)
            {
                throw new NotImplementedException();
            }

            public IExternalAuthentication ExternalAuthentication(string guid)
            {
                throw new NotImplementedException();
            }

            public IHardwareSecurityModule HardwareSecurityModule(int issuerId, InterfaceArea interfaceArea, out IConfig config)
            {
                throw new NotImplementedException();
            }

            void IIntegrationController.CardManagementSystem(int productId, InterfaceArea interfaceArea, out ExternalSystemFields externalFields, out IConfig config)
            {
                throw new NotImplementedException();
            }

            void IIntegrationController.CoreBankingSystem(int productId, InterfaceArea interfaceArea, out ExternalSystemFields externalFields, out IConfig config)
            {
                throw new NotImplementedException();
            }

            void IIntegrationController.HardwareSecurityModule(int issuerId, InterfaceArea interfaceArea, out IConfig config)
            {
                throw new NotImplementedException();
            }
            #endregion
        }
        #endregion

        #region CardManagementDAL
        public class AccountLookup_CardManagementDAL : ICardManagementDAL
        {
            private static readonly int productId = 1;
            private static readonly int accountTypeId = 1;

            public static int ProductId()
            {
                return productId;
            }

            public static int AccountTypeId()
            {
                return accountTypeId;
            }

            #region Account Type Mapping Data
            private static readonly List<accounttypes_mappings_Result> _accountTypeMappingData = new List<accounttypes_mappings_Result>()
        {
            new accounttypes_mappings_Result{ cbs_account_type = "503", cms_account_type = "10", indigo_account_type = accountTypeId.ToString() }
        };

            public static List<accounttypes_mappings_Result> AccountTypeMappingData()
            {
                return _accountTypeMappingData;
            }
            #endregion

            #region Product Print Fields
            private static readonly List<ProductPrintFieldResult> _productPrintFieldsData = new List<ProductPrintFieldResult>()
        {
            new ProductPrintFieldResult()
            {
                deleted = false, editable = false,
                field_name = "IND_SYS_NOC", font = "Arial", font_size = 10, height = 20,
                label = "Name on card", mapped_name = "IND_SYS_NOC", max_length = 12,
                print_field_name = "IND_SYS_NOC", print_field_type_id = 0,
                product_field_id = 0, product_id = productId, value = new byte[1] { 0x00 },
                width = 15, X = 1, Y = 2
            },
            new ProductPrintFieldResult()
            {
                deleted = false, editable = false,
                field_name = "Field name1", font = "Arial", font_size = 10, height = 20,
                label = "label1", mapped_name = "mappedname1", max_length = 12,
                print_field_name = "print field name1", print_field_type_id = 0,
                product_field_id = 1, product_id = productId, value = new byte[1] { 0x00 },
                width = 15, X = 1, Y = 2
            }
        };

            public static List<ProductPrintFieldResult> ProductPrintFieldsData()
            {
                return _productPrintFieldsData;
            }

            public static List<IProductPrintField> PrintFieldsData()
            {
                List<IProductPrintField> results = new List<IProductPrintField>();
                foreach (var field in _productPrintFieldsData)
                {
                    results.Add(PrintFieldFactory.CreatePrintField((int)field.print_field_type_id, (int)field.product_field_id,
                                                                    field.field_name,
                                                                    (float)field.X.GetValueOrDefault(),
                                                                    (float)field.Y.GetValueOrDefault(),
                                                                    (float)field.width.GetValueOrDefault(),
                                                                    (float)field.height.GetValueOrDefault(),
                                                                    field.font, field.font_size ?? 0, 0,
                                                                    field.mapped_name, field.label, (int)field.max_length,
                                                                   (bool)field.editable, (bool)field.deleted, false,1,
                                                                    field.value));
                }

                return results;
            }
            #endregion

            #region Product Currencies
            private static readonly List<ProductCurrencyResult> _productCurrenciesData = new List<ProductCurrencyResult>()
        {
            new ProductCurrencyResult()
            {
                active_YN = true,
                currency_code = "code",
                currency_desc = "currency desc",
                currency_id = 1,
                iso_4217_minor_unit = 2,
                iso_4217_numeric_code = "iso numeric code",
                is_base =  false,
                product_id = productId
            }
        };

            public static List<ProductCurrencyResult> ProductCurrenciesData()
            {
                return _productCurrenciesData;
            }
            #endregion

            #region Product Account Types
            private static readonly List<ProductAccountTypesResult> _productAccountTypesData = new List<ProductAccountTypesResult>()
        {
            new ProductAccountTypesResult()
            {
                product_id = productId,
                account_type_id = accountTypeId
            }
        };

            public static List<ProductAccountTypesResult> ProductAccountTypesData()
            {
                return _productAccountTypesData;
            }
            #endregion

            public List<accounttypes_mappings_Result> GetProductAccountTypeMappings(int? productId, string cbsAccountType, long audit_userId, string audit_workstation)
            {
                return _accountTypeMappingData;
            }

            public List<ProductPrintFieldResult> GetProductPrintFields(long? productId, long? cardId)
            {
                return _productPrintFieldsData;
            }

            public List<ProductCurrencyResult> GetProductCurrencies(int productid, int? currencyId, bool? active)
            {
                return _productCurrenciesData;
            }

            public List<ProductAccountTypesResult> GetProductAccountTypes(int productId, int languageId, long auditUserId, string auditWorkstation)
            {
                return _productAccountTypesData;
            }

            #region Not used
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

            public SystemResponseCode CheckCustomerAccountBalance(CustomerDetails customer, long auditUserId, string auditWorkstation)
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

            public SystemResponseCode IssueCardToCustomer(CustomerDetails customerAccount, long auditUserId, string auditWorkstation)
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

            public SystemResponseCode UpdateCustomerDetails(long cardId, long customerAccountId, CustomerDetails customerDetails, long auditUserId, string auditWorkstation)
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
        #endregion
    }
}
