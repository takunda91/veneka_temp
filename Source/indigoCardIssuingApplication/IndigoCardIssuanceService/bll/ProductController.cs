using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Logging;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.IssuerManagement.Exceptions;
using Veneka.Indigo.Common.Exceptions;
using Veneka.Indigo.Common.Objects;
using Veneka.Indigo.Common.Language;
using IndigoCardIssuanceService.COMS;
using Veneka.Indigo.COMS.DataSource;

namespace IndigoCardIssuanceService.bll
{
    public class ProductController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        private readonly CardMangementService _cardManService = new CardMangementService(new LocalDataSource());
        private readonly ResponseTranslator _translator = new ResponseTranslator();

        /// <summary>
        /// Persist changes to the product object
        /// </summary>
        /// <param name="productlist"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<long> InsertProduct(ProductResult productResult, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                long productId;
                string responseMessage;
                if (_cardManService.InsertProduct(productResult, language, auditUserId, auditWorkstation, out productId, out responseMessage))
                {
                    return new Response<long>(productId, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<long>(productId, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long>(0,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Persist Product to the DB.
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        internal BaseResponse UpdateProduct(ProductResult productResult, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.UpdateProduct(productResult, language, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
       
        /// <summary>
        /// Persist changes to the product object
        /// </summary>
        /// <param name="Productid"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public BaseResponse DeleteProduct(int Productid, long auditUserId, string auditWorkstation)
        {
            try
            {
                _cardManService.DeleteProduct(Productid, auditUserId, auditWorkstation);
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

        public BaseResponse ActivateProduct(int Productid, long auditUserId, string auditWorkstation)
        {
            try
            {
                _cardManService.ActivateProduct(Productid, auditUserId, auditWorkstation);
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

        /// <summary>
        /// get all active products list
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="pageIndex"></param>
        /// <param name="RowsPerpage"></param>
        /// <returns></returns>
        public Response<List<ProductlistResult>> GetProductsList(int issuerID, int? cardIssueMethodId, bool? deletedYN, int pageIndex, int RowsPerpage)
        {

            List<ProductlistResult> productlist = new List<ProductlistResult>();
            try
            {
                productlist = _cardManService.GetProductsList(issuerID, cardIssueMethodId, deletedYN, pageIndex, RowsPerpage);            

                return new Response<List<ProductlistResult>>(productlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ProductlistResult>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

         }

        public Response<List<ProductValidated>> GetProductsListValidated(int issuerID, int? cardIssueMethodId, int pageIndex, int RowsPerpage, int languageId, long auditUserId, string auditWorkstation)
        {

            List<ProductValidated> productlist = new List<ProductValidated>();
            try
            {
                productlist = _cardManService.GetProductsListValidated(issuerID, cardIssueMethodId, pageIndex, RowsPerpage, languageId, auditUserId, auditWorkstation);

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

        /// <summary>
        /// get all active fontfamily list for filling dropdown
        /// </summary>
        /// <returns></returns>
        public Response<List<Issuer_product_font>> GetFontFamilyList()
        {
            try
            {
                return new Response<List<Issuer_product_font>>(_cardManService.GetFontFamilyList(), 
                                                               ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<Issuer_product_font>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

        public Response<List<ServiceRequestCode>> GetServiceRequestCode1()
        {
            try
            {
                return new Response<List<ServiceRequestCode>>(_cardManService.GetServiceRequestCode1(),
                                                               ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ServiceRequestCode>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

        public Response<List<ServiceRequestCode1>> GetServiceRequestCode2()
        {
            try
            {
                return new Response<List<ServiceRequestCode1>>(_cardManService.GetServiceRequestCode2(),
                                                               ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ServiceRequestCode1>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

        public Response<List<ServiceRequestCode2>> GetServiceRequestCode3()
        {
            try
            {
                return new Response<List<ServiceRequestCode2>>(_cardManService.GetServiceRequestCode3(),
                                                               ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ServiceRequestCode2>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

        public Response<List<product_currency1>> GetCurreniesbyProduct(int Productid)
        {
            try
            {
                return new Response<List<product_currency1>>(_cardManService.GetCurreniesbyProduct(Productid),
                                                               ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<product_currency1>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

        public Response<List<CurrencyResult>> GetCurrencyList(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<CurrencyResult>>(_cardManService.GetCurrencyList(languageId, auditUserId, auditWorkstation), 
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CurrencyResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        public Response<List<DistBatchFlows>> GetDistBatchFlowList(int card_issue_method_id, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<DistBatchFlows>>(_cardManService.GetDistBatchFlowList(card_issue_method_id, languageId, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<DistBatchFlows>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// get product details to edit record.
        /// </summary>
        /// <param name="Productid"></param>
        /// <returns></returns>
        public Response<ProductResult> GetProduct(int productId, long auditUserId, string auditWorkstation)
        {
            ProductResult productlist = new ProductResult();
            try
            {
                productlist = _cardManService.GetProduct(productId, auditUserId, auditWorkstation);

                return new Response<ProductResult>(productlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ProductResult>(null,
                                                    ResponseType.ERROR,
                                                    "Error when processing request.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Persist changes to the Font object
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        internal BaseResponse UpdateFont(FontResult fontlist, long auditUserId, string auditWorkstation)
        {
            try
            {
                _cardManService.UpdateFont(fontlist, auditUserId, auditWorkstation);
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

        /// <summary>
        /// Persist Font to the DB.
        /// </summary>
        /// <param name="productlist"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<long?> InsertFont(FontResult fontresult, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<long?>(_cardManService.InsertFont(fontresult, auditUserId, auditWorkstation),
                                           ResponseType.SUCCESSFUL, "", "");


            }
            catch (BaseIndigoException bex)
            {
                log.Error(bex);
                return new Response<long?>(null,
                                          ResponseType.UNSUCCESSFUL,
                                          bex.Message,
                                          log.IsDebugEnabled || log.IsTraceEnabled ? bex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long?>(null,
                                          ResponseType.ERROR,
                                          "An error occured during processing your request, please try again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// making font object as inactive
        /// </summary>
        /// <param name="Productid"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public BaseResponse DeleteFont(int fontid, long auditUserId, string auditWorkstation)
        {
            try
            {
                _cardManService.DeleteFont(fontid, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        "",
                                        "");
            }
            catch (DuplicateIssuerException dex)
            {
                log.Warn(dex);
                return new BaseResponse(ResponseType.FAILED,
                                        dex.Message,
                                        log.IsDebugEnabled || log.IsTraceEnabled ? dex.Message : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// get all active products list
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="pageIndex"></param>
        /// <param name="RowsPerpage"></param>
        /// <returns></returns>
        public Response<List<FontResult>> GetFontListBypage(int pageIndex, int RowsPerpage)
        {

            List<FontResult> fontlist = new List<FontResult>();
            try
            {
                fontlist = _cardManService.GetFontListBypage(pageIndex, RowsPerpage);              

                return new Response<List<FontResult>>(fontlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<FontResult>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

        /// <summary>
        /// get product details to edit record.
        /// </summary>
        /// <param name="Productid"></param>
        /// <returns></returns>
        public Response<FontResult> GetFont(int fontid)
        {

            FontResult productlist = new FontResult();
            try
            {
                productlist = _cardManService.GetFont(fontid);
                return new Response<FontResult>(productlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<FontResult>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

        public BaseResponse DeleteSubProduct(int Productid,int subProductid, long auditUserId, string auditWorkstation)
        {
            try
            {
                _cardManService.DeleteSubProduct(Productid, subProductid,auditUserId, auditWorkstation);
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

        /// <summary>
        /// get all active products list
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="pageIndex"></param>
        /// <param name="RowsPerpage"></param>
        /// <returns></returns>
        public Response<List<SubProduct_Result>> GetSubProductList(int issuer_id, int? product_id, int? cardIssueMethidId, Boolean? deletedYN, int pageIndex, int RowsPerpage)
        {

            List<SubProduct_Result> fontlist = new List<SubProduct_Result>();
            try
            {
                fontlist = _cardManService.GetSubProductList(issuer_id, product_id, cardIssueMethidId, deletedYN,pageIndex, RowsPerpage);

                return new Response<List<SubProduct_Result>>(fontlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<SubProduct_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

        /// <summary>
        /// get product details to edit record.
        /// </summary>
        /// <param name="Productid"></param>
        /// <returns></returns>
        public Response<SubProduct_Result> GetSubProduct(int? product_id, int sub_productid)
        {

            SubProduct_Result productlist = new SubProduct_Result();
            try
            {
                productlist = _cardManService.GetSubProduct(product_id,sub_productid);
                return new Response<SubProduct_Result>(productlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<SubProduct_Result>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

        internal Response<List<ProductFeeAccountingResult>> GetFeeAccountingList(int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            try
            {
                var results = _cardManService.GetFeeAccountingList(issuerId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
                return new Response<List<ProductFeeAccountingResult>>(results, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ProductFeeAccountingResult>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<ProductFeeAccountingResult> GetFeeAccounting(int feeAccountingId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var results = _cardManService.GetFeeAccounting(feeAccountingId, auditUserId, auditWorkstation);
                return new Response<ProductFeeAccountingResult>(results, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ProductFeeAccountingResult>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<ProductFeeAccountingResult> CreateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                ProductFeeAccountingResult details;
                if (_cardManService.CreateFeeAccounting(feeAccountingDetails, languageId, auditUserId, auditWorkstation, out responseMessage, out details))
                {
                    return new Response<ProductFeeAccountingResult>(details, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<ProductFeeAccountingResult>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ProductFeeAccountingResult>(null, ResponseType.ERROR,
                                                        "Error when processing request.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<ProductFeeAccountingResult> UpdateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.UpdateFeeAccounting(feeAccountingDetails, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<ProductFeeAccountingResult>(null, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<ProductFeeAccountingResult>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ProductFeeAccountingResult>(null, ResponseType.ERROR,
                                         "Error when processing request.",
                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse DeleteFeeAccounting(int feeAccountingId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.DeleteFeeAccounting(feeAccountingId, languageId, auditUserId, auditWorkstation, out responseMessage))
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

        internal Response<List<FeeSchemeResult>> GetFeeSchemes(int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            try
            {
                var results = _cardManService.GetFeeSchemes(issuerId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
                return new Response<List<FeeSchemeResult>>(results, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<FeeSchemeResult>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<FeeSchemeDetails> GetFeeSchemeDetails(int feeSchemeId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var results = _cardManService.GetFeeSchemeDetails(feeSchemeId, auditUserId, auditWorkstation);
                return new Response<FeeSchemeDetails>(results, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<FeeSchemeDetails>(null,
                                                       ResponseType.ERROR,
                                                       "Error when processing request.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<FeeDetailResult>> GetFeeDetails(int feeDetailId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var results = _cardManService.GetFeeDetails(feeDetailId, auditUserId, auditWorkstation);
                return new Response<List<FeeDetailResult>>(results, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<FeeDetailResult>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<FeeChargeResult>> GetFeeCharges(int feeDetailId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var results = _cardManService.GetFeeCharges(feeDetailId, auditUserId, auditWorkstation);
                return new Response<List<FeeChargeResult>>(results, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<FeeChargeResult>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<ProductFeeDetailsResult>> GetFeeDetailByProduct(int productId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var results = _cardManService.GetFeeDetailByProduct(productId, auditUserId, auditWorkstation);
                return new Response<List<ProductFeeDetailsResult>>(results, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ProductFeeDetailsResult>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<ProductChargeResult> GetCurrentFees(int feeDetailId, int currencyId, int CardIssueReasonId,string CBSAccountType, long auditUserId, string auditWorkstation)
        {
            try
            {
                var results = _cardManService.GetCurrentFees(feeDetailId, currencyId, CardIssueReasonId, CBSAccountType, auditUserId, auditWorkstation);
                return new Response<ProductChargeResult>(results, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ProductChargeResult>(null,
                                                      ResponseType.ERROR,
                                                      "Error when processing request.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse UpdateFeeCharges(int feeDetailId, List<FeeChargeResult> fees, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.UpdateFeeCharges(feeDetailId, fees, languageId, auditUserId, auditWorkstation, out responseMessage))
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

        internal Response<FeeSchemeDetails> InsertFeeScheme(FeeSchemeDetails feeSchemeDetails, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                FeeSchemeDetails schemeDetails;
                if (_cardManService.InsertFeeScheme(feeSchemeDetails, languageId, auditUserId, auditWorkstation, out responseMessage, out schemeDetails))
                {
                    return new Response<FeeSchemeDetails>(schemeDetails, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<FeeSchemeDetails>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<FeeSchemeDetails>(null, ResponseType.ERROR,
                                                        "Error when processing request.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<FeeSchemeDetails> UpdateFeeScheme(FeeSchemeDetails feeSchemeDetails, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.UpdateFeeScheme(feeSchemeDetails, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<FeeSchemeDetails>(null, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<FeeSchemeDetails>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<FeeSchemeDetails>(null, ResponseType.ERROR,
                                         "Error when processing request.",
                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse DeleteFeeScheme(int feeSchemeId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.DeleteFeeScheme(feeSchemeId, languageId, auditUserId, auditWorkstation, out responseMessage))
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

        internal Response<bool> CreateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.CreateProductPrintFields(productPrintFields, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<bool>(true, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<bool>(false, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false, ResponseType.ERROR,
                                                        "Error when processing request.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<bool> UpdateProductPrintFields(List<ProductPrintFieldResult> productPrintField, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_cardManService.UpdateProductPrintFields(productPrintField, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<bool>(true, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<bool>(false, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false, ResponseType.ERROR,
                                         "Error when processing request.",
                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<ProductPrintFieldResult>> GetProductPrintFields(long? productId, long? cardId, long? requestId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var results = _cardManService.GetProductPrintFields(productId, cardId, requestId, auditUserId, auditWorkstation);
                return new Response<List<ProductPrintFieldResult>>(results, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ProductPrintFieldResult>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<pin_block_formatResult>> LookupPinBlockFormat(long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<pin_block_formatResult>>(_cardManService.LookupPinBlockFormat(auditUserId, auditWorkstation),
                                                        ResponseType.SUCCESSFUL,
                                                        "",
                                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<pin_block_formatResult>>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<LangLookup>> LookupPrintFieldTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_cardManService.LookupPrintFieldTypes(languageId, auditUserId, auditWorkstation),
                                                        ResponseType.SUCCESSFUL,
                                                        "",
                                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
    }
}

        