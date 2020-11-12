using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.Integration.TMB.CBS;

namespace Veneka.Indigo.Integration.TMB
{
    [IntegrationExport("TMBCBS", "8B8C7E4C-39CC-493D-A4FF-E9D7ADC8E846", typeof(ICoreBankingSystem))]
   
    public class TMBFLEXCBS : ICoreBankingSystem
    {
        public IDataSource DataSource { get; set; }
       
        public DirectoryInfo IntegrationFolder { get; set; }
        public string SQLConnectionString { get; set; }
        private static readonly ILog _cbsLog = LogManager.GetLogger(General.CBS_LOGGER);


        public bool ChargeFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string feeRefrenceNumber, out string responseMessage)
        {
            _cbsLog.Trace(t => t("Doing ChargeFees"));
            feeRefrenceNumber = String.Empty;
            responseMessage = String.Empty;

            try
            {
                if (config is Config.WebServiceConfig)
                {
                    var cbsParms = (Config.WebServiceConfig)config;
                    Common.Protocol protocol = cbsParms.Protocol == Config.Protocol.HTTP ? Common.Protocol.HTTP : Common.Protocol.HTTPS;
                    Common.Authentication auth = cbsParms.AuthType == Config.AuthType.None ? Common.Authentication.NONE : Common.Authentication.BASIC;


                    if (cbsParms == null)
                        throw new ArgumentNullException("Cannot find parameters for Core Banking System.");

                    var item = externalFields.Field.FirstOrDefault(i => i.Key == "cardType");
                    if (item.Key == null)
                    {
                        throw new Exception("cardType external field not found.");
                    }
                    string cardType = externalFields.Field["cardType"];

                    FlexcubeWebService service = new FlexcubeWebService(protocol, cbsParms.Address, cbsParms.Port, cbsParms.Path, cbsParms.Timeout, auth, cbsParms.Username, cbsParms.Password, DataSource);

                    string branchCode = DataSource.LookupDAL.LookupBranchCode(customerDetails.DomicileBranchId);
                    if (service.ChargeFees(customerDetails, customerDetails.AccountNumber, branchCode, cbsParms.Username, cardType, cbsParms.Password, out responseMessage))
                    {
                        feeRefrenceNumber = customerDetails.FeeReferenceNumber;
                        DataSource.CardsDAL.UpdateCardFeeReferenceNumber(customerDetails.CardId, customerDetails.FeeReferenceNumber, auditUserId, auditWorkstation);
                        return true;
                    }
                    else
                        return false;

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
            _cbsLog.Trace(t => t("Doing CheckBalance"));
            try
            { 
                responseMessage = String.Empty;
                if (config is Config.WebServiceConfig)
                {
                    var cbsParms = (Config.WebServiceConfig)config;
                    Common.Protocol protocol = cbsParms.Protocol == Config.Protocol.HTTP ? Common.Protocol.HTTP : Common.Protocol.HTTPS;
                    Common.Authentication auth = cbsParms.AuthType == Config.AuthType.None ? Common.Authentication.NONE : Common.Authentication.BASIC;

                    if (cbsParms == null)
                        throw new ArgumentNullException("Cannot find parameters for Core Banking System.");
                    FlexcubeWebService service = new FlexcubeWebService(protocol, cbsParms.Address, cbsParms.Port, cbsParms.Path, cbsParms.Timeout, auth, cbsParms.Username, cbsParms.Password, DataSource);
                    string branchCode = DataSource.LookupDAL.LookupBranchCode(customerDetails.DomicileBranchId);
                    if (branchCode==null)
                    {
                        branchCode= DataSource.LookupDAL.LookupBranchCode(customerDetails.BranchId );
                    }
                    if (service.CheckBalance(customerDetails, cbsParms.Username, customerDetails.AccountNumber, branchCode, languageId, out responseMessage))
                    {
                        return true;
                    }
                    else
                        return false;

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
            // WebServiceConfig ubsParms;

            //if (config is WebServiceConfig)
            //    ubsParms = (WebServiceConfig)config;
            //else
            //    throw new ArgumentException("Configuration for Core Banking must be for Web Services.", "config");

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



                //if (service.QueryCustAcc(accountNumber, branchCode, cbsParms.Username, printFields, languageId, out accountDetails, out responseMessage))
                //{
                //    //Populate print fields on return. Access Bank doesnt have additional print fields but must still return them.
                //    accountDetails.ProductFields = printFields.Select(s => new ProductField(s)).ToList();
                //    return true;
                //}


                return service.QueryCustAcc(accountNumber, branchCode, cbsParms.Username, printFields, languageId, out accountDetails, out responseMessage);
                


            }
            else
                throw new ArgumentException("Config parameters must be for Webservice.");


        }

        public bool ReverseFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            responseMessage = string.Empty;
            return true;
        }

        public bool UpdateAccount(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            responseMessage = string.Empty;
            return true;
        }
    }
}
