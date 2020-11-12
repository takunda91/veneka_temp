using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.Integration.TMB.CBS;

namespace Veneka.Indigo.Integration.TMB
{
    [IntegrationExport("TMBFUNDSLOADCBS", "C1F01D09-F747-428C-8034-D0D42969EE85", typeof(ICoreBankingSystem))]
    public class TMBFUNDSLOADCBS : ICoreBankingSystem
    {
        public IDataSource DataSource { get; set; }
        public DirectoryInfo IntegrationFolder { get; set; }
        private static readonly ILog _cbsLog = LogManager.GetLogger(General.CBS_LOGGER);
        public bool ChargeFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string feeRefrenceNumber, out string responseMessage)
        {
            _cbsLog.Trace(t => t("TMBFUNDSLOADCBS: Doing ChargeFees"));
            feeRefrenceNumber = String.Empty;
            responseMessage = String.Empty;

            try
            {
                if (config != null)
                {
                    if (config is Config.WebServiceConfig)
                    {
                        var cbsParms = (Config.WebServiceConfig)config;
                        Common.Protocol protocol = cbsParms.Protocol == Config.Protocol.HTTP ? Common.Protocol.HTTP : Common.Protocol.HTTPS;
                        Common.Authentication auth = cbsParms.AuthType == Config.AuthType.None ? Common.Authentication.NONE : Common.Authentication.BASIC;

                        if (cbsParms == null)
                            throw new ArgumentNullException("Cannot find parameters for Core Banking System.");

                        _cbsLog.Trace($"TMBFUNDSLOADCBS: checkExternalFields");

                        if (externalFields.Field != null)
                        {
                            var item = externalFields.Field.FirstOrDefault(i => i.Key == "cardType");
                            if (item.Key == null)
                            {
                                _cbsLog.Trace("cardType external field not found.");
                                throw new Exception("cardType external field not found.");
                            }

                            _cbsLog.Trace("ABOUT TO CALL: string cardType = externalFields.Field[cardType];");
                            string cardType = externalFields.Field["cardType"];

                            _cbsLog.Trace("ABOUT TO CALL: FlexcubeWebService service = new FlexcubeWebService");
                            FlexcubeWebService service = new FlexcubeWebService(protocol, cbsParms.Address, cbsParms.Port, cbsParms.Path, cbsParms.Timeout, auth, cbsParms.Username, cbsParms.Password, DataSource);

                            string branchCode = DataSource.LookupDAL.LookupBranchCode(customerDetails.DomicileBranchId);
                            _cbsLog.Trace("ABOUT TO CALL: service.ChargeFees(customerDetails, customerDetails.AccountNumber, branchCode, ");
                            if (service.ChargeFees(customerDetails, customerDetails.AccountNumber, branchCode, cbsParms.Username, cardType, cbsParms.Password, out responseMessage))
                            {
                                _cbsLog.Trace("TMBFUNDSLOADCBS: service.ChargeFees returned TRUE");
                                //feeRefrenceNumber = customerDetails.FeeReferenceNumber;
                                //DataSource.CardsDAL.UpdateCardFeeReferenceNumber(customerDetails.CardId, customerDetails.FeeReferenceNumber, auditUserId, auditWorkstation);
                                return true;
                            }
                            else
                            {
                                _cbsLog.Trace("TMBFUNDSLOADCBS: service.ChargeFees returned FALSE");
                                return false;
                            }
                        }
                        else
                        {
                            _cbsLog.Error("ExternalFields.Field is null");
                        }
                    }
                    else
                    {
                        _cbsLog.Error($"TMBFUNDSLOADCBS: config is null");
                        throw new Exception($"TMBFUNDSLOADCBS.ChargeFee: config is null");
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException endpointException)
            {
                _cbsLog.Error(endpointException);
                responseMessage = "Unable to connect to Flexcube, please try again or contact support.";
            }
            catch (Exception ex)
            {
                _cbsLog.Error(ex);
                responseMessage = ex.Message;
            }


            return false;

        }

        public bool CheckBalance(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            _cbsLog.Trace(t => t("TMBFUNDSLOADCBS: Doing CheckBalance"));
            try
            {
                responseMessage = String.Empty;
                if (config is Config.WebServiceConfig)
                {
                    var cbsParms = (Config.WebServiceConfig)config;
                    Common.Protocol protocol = cbsParms.Protocol == Config.Protocol.HTTP ? Common.Protocol.HTTP : Common.Protocol.HTTPS;
                    Common.Authentication auth = cbsParms.AuthType == Config.AuthType.None ? Common.Authentication.NONE : Common.Authentication.BASIC;

                    if (cbsParms == null)
                        throw new ArgumentNullException("TMBFUNDSLOADCBS: Cannot find parameters for Core Banking System.");
                    FlexcubeWebService service = new FlexcubeWebService(protocol, cbsParms.Address, cbsParms.Port, cbsParms.Path, cbsParms.Timeout, auth, cbsParms.Username, cbsParms.Password, DataSource);
                    string branchCode = DataSource.LookupDAL.LookupBranchCode(customerDetails.DomicileBranchId);
                    if (branchCode == null)
                    {
                        branchCode = DataSource.LookupDAL.LookupBranchCode(customerDetails.BranchId);
                    }
                    if (service.CheckBalance(customerDetails, cbsParms.Username, customerDetails.AccountNumber, branchCode, languageId, out responseMessage))
                    {
                        _cbsLog.Trace("TMBFUNDSLOADCBS: service.CheckBalance returned true;");
                        return true;
                    }
                    else
                    {
                        _cbsLog.Trace("TMBFUNDSLOADCBS: service.CheckBalance returned false ;");
                        return false;
                    }
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException endpointException)
            {
                _cbsLog.Error(endpointException);
                responseMessage = "Unable to connect to Flexcube, please try again or contact support.";
            }
            catch (Exception ex)
            {
                _cbsLog.Error(ex);
                responseMessage = ex.Message;
            }

            return false;
        }

        public bool GetAccountDetail(string accountNumber, List<IProductPrintField> printFields, int cardIssueReasonId, int issuerId, int branchId, int productId, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out AccountDetails accountDetails, out string responseMessage)
        {
            _cbsLog.Trace(t => t("Doing GetAccountDetails"));

            if (config is Config.WebServiceConfig)
            {
                var cbsParms = (Config.WebServiceConfig)config;
                Common.Protocol protocol = cbsParms.Protocol == Config.Protocol.HTTP ? Common.Protocol.HTTP : Common.Protocol.HTTPS;
                Common.Authentication auth = cbsParms.AuthType == Config.AuthType.None ? Common.Authentication.NONE : Common.Authentication.BASIC;

                if (cbsParms == null)
                    throw new ArgumentNullException("Cannot find parameters for Core Banking System.");
                FlexcubeWebService service = new FlexcubeWebService(protocol, cbsParms.Address, cbsParms.Port, cbsParms.Path, cbsParms.Timeout, auth, cbsParms.Username, cbsParms.Password, DataSource);

                string branchCode = DataSource.LookupDAL.LookupBranchCode(branchId);
                accountDetails = null;
                bool customerQuerySuccessful = service.QueryCustAcc(accountNumber, branchCode, cbsParms.Username, printFields, languageId, out accountDetails, out responseMessage);
                if (customerQuerySuccessful && accountDetails.ProductFields.Count > 0)
                {
                    ProductField[] productFields = accountDetails.ProductFields.ToArray();
                    List<string> addressFields = new List<string>();
                    for (int i = 0; i < accountDetails.ProductFields.Count; i++)
                    {
                        if (productFields[i].MappedName.Trim().ToLower()== "ind_sys_address")
                        {
                            addressFields.Add(productFields[i].ValueToString());
                        }
                    }
                }
                return customerQuerySuccessful;
            }
            else
                throw new ArgumentException("Config parameters must be for Webservice.");
        }

        public bool ReverseFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            responseMessage = String.Empty;
            return true;
        }

        public bool UpdateAccount(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            responseMessage = String.Empty;
            return true;
        }
    }
}
