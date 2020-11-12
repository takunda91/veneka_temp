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
    [IntegrationExport("TMBPREPAIDCBS", "EDC9051F-F52C-4FC3-87E4-C41F23251075", typeof(ICoreBankingSystem))]
    public class TMBPREPAIDCBS : ICoreBankingSystem
    {
        public IDataSource DataSource { get; set ; }
        public DirectoryInfo IntegrationFolder { get ; set ; }
        public string SQLConnectionString { get; set; }
        private static readonly ILog _cbsLog = LogManager.GetLogger(General.CBS_LOGGER);

        public bool ChargeFee(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string feeRefrenceNumber, out string responseMessage)
        {
            feeRefrenceNumber = null;
            _cbsLog.Trace("TMBPREPAIDCBS: Please Note that the logic in this class does not exits.");
            responseMessage = String.Empty;
            return true;
        }

        public bool CheckBalance(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            responseMessage = String.Empty;
            _cbsLog.Trace("TMBPREPAIDCBS: Please Note that the logic in this class does not exits.");
            return true;

        }

        public bool GetAccountDetail(string accountNumber, List<IProductPrintField> printFields, int cardIssueReasonId, int issuerId, int branchId, int productId, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out AccountDetails accountDetails, out string responseMessage)
        {
            //Check in flexcube customer exist or not. If exit continue with prepaid process
            _cbsLog.Trace(t => t("Doing GetAccountDetails"));
            
            AccountDetails PrepaidAccountDetails = new AccountDetails();

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

                var flexResponse = service.QueryCustAcc(accountNumber, branchCode, cbsParms.Username, printFields, languageId, out accountDetails, out responseMessage);
                _cbsLog.Debug("Response recieved from flexcube");
                if (responseMessage != "Account Found")
                {
                    responseMessage = "Not Exist Account";
                    return false;

                }


                PrepaidAccountDetails = accountDetails;
                _cbsLog.Debug("account details response name on card + first name = " + accountDetails.NameOnCard + " " + accountDetails.FirstName);
            }

                //For prepaid account -Overwrite the account with prepaid account number and account type
                _cbsLog.Debug("Calling GetAccountDetails");

                responseMessage = string.Empty;
                //accountDetails = null;
                int accountNum = DataSource.TransactionSequenceDAL.NextSequenceNumber("PREPAIDACCOUNTNUMBER", Integration.Common.ResetPeriod.NONE);
                _cbsLog.Debug(" Accountnumber Returned ==" + accountNum);
                accountDetails = BuildAccountDetails(accountNum, PrepaidAccountDetails);

                _cbsLog.Debug("Done BuildAccount");

                accountDetails.ProductFields = new List<ProductField>();
                _cbsLog.Debug("Calling productFields ");
                foreach (var item in printFields)
                {
                    accountDetails.ProductFields.Add(new ProductField(item));
                }

            
                return true;
            
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

        private AccountDetails BuildAccountDetails(int ACC, AccountDetails AccDetails)
        {
            string accountNumber = Convert.ToString(ACC);
            int accountLength = accountNumber.Length;
            AccountDetails rtn = new AccountDetails
            {
                AccountNumber ="PREP_USD_" + ACC.ToString().PadLeft(11 , '0'),
                CurrencyId = DecodeCurrency("USD"),
                CBSAccountTypeId="NotMapped",
                NameOnCard=AccDetails.NameOnCard,
                FirstName=AccDetails.FirstName,
                MiddleName=AccDetails.MiddleName,
                LastName=AccDetails.LastName,
                
                
                
                             
            };
            _cbsLog.Debug("Account Details CurrencyID==" + rtn.CurrencyId);
            return rtn;
        }
        private int DecodeCurrency(string ccy)
        {
            return DataSource.LookupDAL.LookupCurrency(ccy);

        }

    }
}
