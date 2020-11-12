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
    [IntegrationExport("TMBTAGPAY", "4896551A-095E-40B8-A9AE-0304B60E33C5", typeof(ICoreBankingSystem))]
    public class TMBTAGPAY : ICoreBankingSystem
    {
        public IDataSource DataSource { get; set; }


        public DirectoryInfo IntegrationFolder { get; set; }
        public string SQLConnectionString { get; set; }
        private static readonly ILog _cbsLog = LogManager.GetLogger(General.CBS_LOGGER);



        public bool ChargeFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string feeRefrenceNumber, out string responseMessage)
        {

            WebServiceConfig ubsParms;

            if (config is WebServiceConfig)
                ubsParms = (WebServiceConfig)config;
            else
                throw new ArgumentException("Configuration for Core Banking must be for Web Services.", "config");
            feeRefrenceNumber = null;
            try
            {

                TagPayService tagpay = new TagPayService(DataSource);
                _cbsLog.Debug("Calling Tagpay service from TMBTAGPAY.cs");
                return tagpay.ChargeFee(ubsParms, customerDetails, externalFields, languageId, out responseMessage);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool CheckBalance(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            responseMessage = string.Empty;
            //Disabled for the workaround without pin. Getting no account balance in the response withput pin
            _cbsLog.Trace(t => t("Doing Tagpay GetAccount Balance"));
            WebServiceConfig ubsParms;

            if (config is WebServiceConfig)
                ubsParms = (WebServiceConfig)config;
            else
                throw new ArgumentException("Configuration for Core Banking must be for Web Services.", "config");

            try
            {
                _cbsLog.Debug("Calling Tagpay service from TMBTAGPAY.cs");
                TagPayService tagpay = new TagPayService(DataSource);
                string accountNumber = customerDetails.AccountNumber;
                if (!string.IsNullOrWhiteSpace(customerDetails.AccountPin))
                {
                    accountNumber = string.Format("{0}&pin={1}", customerDetails.AccountNumber, customerDetails.AccountPin);
                }
                return tagpay.CheckBalance(ubsParms, customerDetails, accountNumber, languageId, out responseMessage);
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public bool GetAccountDetail(string accountNumber, List<IProductPrintField> printFields, int cardIssueReasonId, int issuerId, int branchId, int productId, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out AccountDetails accountDetails, out string responseMessage)
        {
            _cbsLog.Trace(t => t("Doing Tagpay GetAccountDetAILS"));
            WebServiceConfig ubsParms;

            var splitAccount = accountNumber.Split('|');

            if (splitAccount.Length == 2)
            {
                string pepeleAccountNumber = string.Format("{0}&pin={1}", splitAccount[0], splitAccount[1]);
                if (config is WebServiceConfig)
                    ubsParms = (WebServiceConfig)config;
                else
                    throw new ArgumentException("Configuration for Core Banking must be for Web Services.", "config");
                accountDetails = null;
                try
                {
                    _cbsLog.Debug("Calling Tagpay service from TMBTAGPAY.cs");
                    TagPayService tagpay = new TagPayService(DataSource);
                    return tagpay.QueryCustAcc(ubsParms, pepeleAccountNumber, printFields, languageId, out accountDetails, out responseMessage);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            else
            {
                throw new Exception("Not enough account details supplied to validate.  Check number and PIN.");
            }
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
