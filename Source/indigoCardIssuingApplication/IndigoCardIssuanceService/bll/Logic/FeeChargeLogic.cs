using Common.Logging;
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


    public class FeeChargeLogic
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AccountLookupLogic));
        private readonly CardMangementService _cardManService;
        private readonly IComsCore _comsCore;
        private readonly IIntegrationController _integration;

        public FeeChargeLogic(CardMangementService cardManagementService, IComsCore comsCore, IIntegrationController integration)
        {
            _cardManService = cardManagementService;
            _comsCore = comsCore;
            _integration = integration;
        }

        public BaseResponse FeeCharge(CustomerDetails customerDetails, AuditInfo auditInfo)
        {
            List<IProductPrintField> printFields = _cardManService.GetProductPrintFields(customerDetails.ProductId, null, null);
            _log.Trace(t => t("Looking up account in Core Banking System."));
            // IntegrationController _integration = IntegrationController.Instance;
            Veneka.Indigo.Integration.Config.IConfig config;
            Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;
            _integration.CoreBankingSystem(customerDetails.ProductId, InterfaceArea.ISSUING, out externalFields, out config);

            InterfaceInfo interfaceInfo = new InterfaceInfo
            {
                Config = config,
                InterfaceGuid = config.InterfaceGuid.ToString()

            };

            CardObject _object = new CardObject();
            _object.CustomerAccount = new AccountDetails();
            _object.CustomerAccount.AccountNumber = customerDetails.AccountNumber;
            _object.PrintFields = printFields;
            var response = _comsCore.CheckBalance(customerDetails, externalFields, interfaceInfo, auditInfo);
            if (response.ResponseCode == 0)
            {

                // var accountLookupLogic = new Logic.AccountLookupLogic(_cardManService, _comsCoreInstance, _integration);

                //Validate returned account
                //   if (accountLookupLogic.ValidateAccount(customerDetails.ProductId, response.Value, out responseMessage))

                {
                    try
                    {
                        string feeResponse = String.Empty;

                        // Charge Fee if it's greater than 0 and has not already been charged for.
                        if (customerDetails.FeeCharge != null && customerDetails.FeeCharge.Value > 0 && String.IsNullOrWhiteSpace(customerDetails.FeeReferenceNumber))
                        {
                            _log.Trace(t => t("calling Charge the fee."));
                            string feeReferenceNumber = string.Empty;

                            var response_fee = _comsCore.ChargeFee(customerDetails, externalFields, interfaceInfo, auditInfo);

                            if (response_fee.ResponseCode == 0)
                            {
                                // customerDetails.FeeReferenceNumber = feeReferenceNumber;
                                _log.DebugFormat("FeeReferenceNumber " + customerDetails.FeeReferenceNumber);


                                _log.DebugFormat("Fee  charged: Ref{0}", customerDetails.FeeReferenceNumber);

                                return new BaseResponse(ResponseType.SUCCESSFUL, feeResponse, feeResponse);

                            }
                            else
                            {
                                _log.Trace(t => t(feeResponse));

                                return new BaseResponse(ResponseType.UNSUCCESSFUL, feeResponse, feeResponse);

                            }

                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(customerDetails.FeeReferenceNumber))
                                if (_log.IsDebugEnabled)
                                    _log.DebugFormat("Fee already charged: Ref{0}", customerDetails.FeeReferenceNumber);
                                else
                                    _log.Trace(t => t("Fee already charged."));
                            return new BaseResponse(ResponseType.SUCCESSFUL, feeResponse, feeResponse);

                        }

                    }
                    catch (NotImplementedException nie)
                    {
                        _log.Warn(nie);
                        return new BaseResponse(ResponseType.ERROR,
                                                nie.Message,
                                                nie.Message);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                        return new BaseResponse(ResponseType.ERROR,
                                                ex.Message,
                                                _log.IsDebugEnabled || _log.IsTraceEnabled ? ex.Message : "");
                    }
                }
            }

            return new BaseResponse(ResponseType.UNSUCCESSFUL, response.ResponseMessage, response.ResponseMessage);

        }

        public BaseResponse ChargeAmount(CustomerDetails customerDetails, AuditInfo auditInfo, Veneka.Indigo.Integration.Config.IConfig config, ExternalSystemFields externalFields)
        {
            List<IProductPrintField> printFields = _cardManService.GetProductPrintFields(customerDetails.ProductId, null, null);
            _log.Trace(t => t("Looking up account in Core Banking System."));

            if (config == null)
            {
                _log.Trace("FeeChargeLogic: ChargeAmount config IS NULL");
            }

            InterfaceInfo interfaceInfo = new InterfaceInfo
            {
                Config = config,
                InterfaceGuid = config.InterfaceGuid.ToString()
            };

            _log.Trace("InterfaceInfo interfaceInfo = new InterfaceInfo : No Issues");

            CardObject _object = new CardObject();
            _object.CustomerAccount = new AccountDetails();
            _object.CustomerAccount.AccountNumber = customerDetails.AccountNumber;
            _object.PrintFields = printFields;
            var response = _comsCore.CheckBalance(customerDetails, externalFields, interfaceInfo, auditInfo);
            if (response.ResponseCode == 0)
            {
                {
                    try
                    {
                        string feeResponse = String.Empty;

                        // Charge Fee if it's greater than 0 and has not already been charged for.
                        if (customerDetails.FeeCharge != null && customerDetails.FeeCharge.Value > 0 && String.IsNullOrWhiteSpace(customerDetails.FeeReferenceNumber))
                        {
                            _log.Trace(t => t("FeeChargeLogic: ChargeAmount : calling Charge the fee."));
                            string feeReferenceNumber = string.Empty;

                            var response_fee = _comsCore.ChargeFee(customerDetails, externalFields, interfaceInfo, auditInfo);

                            if (response_fee.ResponseCode == 0)
                            {
                                // customerDetails.FeeReferenceNumber = feeReferenceNumber;
                                _log.DebugFormat($"FeeReferenceNumber {customerDetails.FeeReferenceNumber}");
                                return new BaseResponse(ResponseType.SUCCESSFUL, feeResponse, feeResponse);
                            }
                            else
                            {
                                _log.Trace(t => t(feeResponse));

                                return new BaseResponse(ResponseType.UNSUCCESSFUL, feeResponse, feeResponse);
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(customerDetails.FeeReferenceNumber))
                                if (_log.IsDebugEnabled)
                                    _log.DebugFormat("Fee already charged: Ref{0}", customerDetails.FeeReferenceNumber);
                                else
                                    _log.Trace(t => t("Fee already charged."));
                            return new BaseResponse(ResponseType.SUCCESSFUL, feeResponse, feeResponse);
                        }
                    }
                    catch (NotImplementedException nie)
                    {
                        _log.Warn(nie);
                        return new BaseResponse(ResponseType.ERROR,
                                                nie.Message,
                                                nie.Message);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                        return new BaseResponse(ResponseType.ERROR,
                                                ex.Message,
                                                _log.IsDebugEnabled || _log.IsTraceEnabled ? ex.Message : "");
                    }
                }
            }

            return new BaseResponse(ResponseType.UNSUCCESSFUL, response.ResponseMessage, response.ResponseMessage);

        }

    }

}