using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Module.Delta;

namespace Veneka.Indigo.Integration.TMB.CBS
{
    public class TagPayService
    {
        #region Private Fields

        private const string LOGGER = "TagPayService";
        private static readonly ILog _cbsLog = LogManager.GetLogger(typeof(TagPayService));
        private readonly IDataSource _datasource;
        private readonly string _connectionString;
        private readonly DELTARESTAPIService _client;

        #endregion
        #region Constructors
        public TagPayService(IDataSource datasource)
        {
            _datasource = datasource;
            _client = new Module.Delta.DELTARESTAPIService();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            _cbsLog.Trace(m => m("Creating TagPayService Client."));
        }

        #endregion
        #region Public Methods
        /// <summary>
        /// <summary>
        /// Queries Delta for the desired customers account
        /// </summary>
        /// <param name="acc"></param>
        public bool QueryCustAcc(Config.WebServiceConfig parameters, string mobileNumber, List<ProductPrinting.IProductPrintField> printFields, int languageId, out AccountDetails accDetails, out string responseMessage)
        {
            accDetails = null;
            responseMessage = String.Empty;
            bool validResponse = false;
            string currency = string.Empty;
            if (parameters.AuthType == AuthType.None)
            {
                _client.BasicAuth = false;
            }
            else if (parameters.AuthType == AuthType.Basic)
            {
                _client.BasicAuth = true;
                _client.UserName = parameters.Username;
                _client.Password = parameters.Password;
            }
            else
            {
                _client.BasicAuth = false;
            }
            string encryptedpassword = MD5Hash.MD5Hash.EncryptPassword(parameters.Password);

            var AccountLookupEndPoint = _client.EndPoint = parameters.Protocol + "://" + parameters.Address + "/" + parameters.Path + "merchantid=" + parameters.Username + "&password=" + encryptedpassword + "&client=" + mobileNumber;
            _cbsLog.Debug("EndPoint" + _client.EndPoint);
            _cbsLog.Debug("AccountLookupEndPoint" + AccountLookupEndPoint);


            var AccountLookupRes = _client.MakeRequest(HttpVerb.GET, "text/xml", "");
            _cbsLog.Debug("AccountLookupRes" + AccountLookupRes);
            if (AccountLookupRes != null)
            {
                _cbsLog.Debug("build account details");
                string[] customerData = ProcessAccLookupResponse(AccountLookupRes);
                //check reserve and secondary currency field.If any field has USD retun that .enable this when doing the pin
                //CheckCurrency(customerData, out string validCurrency);
                //_cbsLog.Debug("validCurrency==" + validCurrency);
                //disbale below line for the work around without pin.enable this when doing proper pin
                // accDetails = BuildAccountDetails(customerData[1], customerData[2], customerData[0], validCurrency, "19000000", customerData[1], printFields);
                if (customerData != null)
                {
                    accDetails = BuildAccountDetails(customerData[1], customerData[2], customerData[0], "840", "19000000", customerData[1], printFields);

                    string cleanResponse = customerData[5].ToString();
                    cleanResponse = cleanResponse.Replace(" ", "").Trim().ToUpper();
                    if (cleanResponse.Equals("0:SUCCESS"))
                    {
                        _cbsLog.Trace("QueryCustAcc: 0 : SUCCESS");
                        validResponse = true;
                    }
                    else
                    {
                        _cbsLog.Trace("QueryCustAcc: FAILED");
                        responseMessage = customerData[4].ToString();
                        _cbsLog.Trace($"QueryCustAcc: 0 : FAILED responseMessage IS {responseMessage}");
                        validResponse = false;
                    }
                }
                else
                {
                    responseMessage = "Invalid or missing account details.";
                    validResponse = false;
                }
            }
            else
            {
                responseMessage = "No response recieved";
                validResponse = false;
            }
            return validResponse;

        }

        public string CheckCurrency(string[] customerData, out string currency)
        {
            currency = string.Empty;

            if (customerData[7] == "840")
            {
                currency = customerData[7];
            }
            else
                if (customerData[8] == "840")
            {
                currency = customerData[8];
            }
            return currency;

        }
        public string CheckBalance(string[] customerData, out string balance)
        {

            balance = string.Empty;
            if (customerData[7] == "840")
            {
                balance = customerData[4];
            }
            else
                if (customerData[8] == "840")
            {
                balance = customerData[9];
            }
            return balance;


        }
        public bool CheckBalance(Config.WebServiceConfig parameters, CustomerDetails custDetails, string mobileNumber, int languageId, out string responseMessage)
        {

            responseMessage = String.Empty;
            bool validResponse = false;
            string currency = string.Empty;
            if (parameters.AuthType == AuthType.None)
            {
                _client.BasicAuth = false;
            }
            else if (parameters.AuthType == AuthType.Basic)
            {
                _client.BasicAuth = true;
                _client.UserName = parameters.Username;
                _client.Password = parameters.Password;
            }
            else
            {
                _client.BasicAuth = false;
            }
            string encryptedpassword = MD5Hash.MD5Hash.EncryptPassword(parameters.Password);

            var AccountLookupEndPoint = _client.EndPoint = parameters.Protocol + "://" + parameters.Address + "/" + parameters.Path + "merchantid=" + parameters.Username + "&password=" + encryptedpassword + "&client=" + mobileNumber;
            _cbsLog.Debug("EndPoint" + _client.EndPoint);
            _cbsLog.Debug("AccountLookupEndPoint" + AccountLookupEndPoint);

            var AccountLookupRes = _client.MakeRequest(HttpVerb.GET, "text/xml", "");
            _cbsLog.Debug("AccountLookupRes" + AccountLookupRes);
            if (AccountLookupRes != null)
            {
                string[] customerData = ProcessAccLookupResponse(AccountLookupRes);

                //check reserve and secondary currency field.If any field has USD retun that and check the balace against reserve or secondary 
                CheckBalance(customerData, out string balance);

                //decimal? AvailBalance = System.Convert.ToDecimal(customerData[4]);
                decimal? AvailBalance = System.Convert.ToDecimal(balance);
                _cbsLog.Debug("AvailBalance==" + AvailBalance);
                _cbsLog.Debug("Fee to be charged==" + custDetails.FeeCharge.Value);
                if (AvailBalance < custDetails.FeeCharge.Value)
                {
                    responseMessage = "Available balance of the account is not sufficient.";
                    return false;
                }
            }
            else
            {
                responseMessage = "No response recieved";
                validResponse = false;
            }
            validResponse = true;
            return validResponse;

        }
        public bool ChargeFee(Config.WebServiceConfig parameters, CustomerDetails customerDetails, ExternalSystemFields externalFields, int languageId, out string responseMessage)
        {

            responseMessage = string.Empty;
            bool validResponse = false;
            if (parameters.AuthType == AuthType.None)
            {
                _client.BasicAuth = false;
            }
            else if (parameters.AuthType == AuthType.Basic)
            {
                _client.BasicAuth = true;
                _client.UserName = parameters.Username;
                _client.Password = parameters.Password;
            }
            else
            {
                _client.BasicAuth = false;
            }

            string branchCode;
            branchCode = _datasource.LookupDAL.LookupBranchCode(customerDetails.DomicileBranchId);

            var seq = _datasource.TransactionSequenceDAL.NextSequenceNumber("TagpayCharge", ResetPeriod.DAILY);

            _cbsLog.Debug("TagpayChargeseq==" + seq);
            customerDetails.FeeReferenceNumber = BuildReference(branchCode, seq);

            string encryptedpassword = MD5Hash.MD5Hash.EncryptPassword(parameters.Password);
            int feeInCents = (int)(customerDetails.FeeCharge * 100);//Tagpay need fees in cents.We configuring in dollar in indigo

            //disabled below for the work around without pin  ..Hardcoded 840 as currency and path as we calling this method from check balance and path is for account lookup
            //var feecharge = _client.EndPoint = parameters.Protocol + "://" + parameters.Address + "/" + parameters.Path + "merchantid=" + parameters.Username + "&password=" +
            //   encryptedpassword + "&client=" + customerDetails.AccountNumber + "&amount=" + feeInCents + "&currency=" + EncodeCurrency( customerDetails.CurrencyCode) +
            //   "&referenceid=" + customerDetails.FeeReferenceNumber + "&sms=yes&email=yes";
            string accountNumber = customerDetails.AccountNumber;
            if (!string.IsNullOrWhiteSpace(customerDetails.AccountPin))
            {
                accountNumber = string.Format("{0}&pin={1}", customerDetails.AccountNumber, customerDetails.AccountPin);
            }

            var feecharge = _client.EndPoint = parameters.Protocol + "://" + parameters.Address + "/" + parameters.Path + "merchantid=" + parameters.Username + "&password=" +
              encryptedpassword + "&client=" + accountNumber + "&amount=" + feeInCents + "&currency=" + "840" +
              "&referenceid=" + customerDetails.FeeReferenceNumber + "&sms=yes&email=yes";
            _cbsLog.Debug("feecharge request =" + feecharge);


            var feechargeresponse = _client.MakeRequest(HttpVerb.GET, "text/xml", "");
            _cbsLog.Debug("feechargeresponse==" + feechargeresponse);
            if (feecharge != null)
            {
                string[] BuiltResponse = ProcessChargeFeeResponse(feechargeresponse);
                if (BuiltResponse[0].ToString().ToUpper().Equals("0: SUCCESS"))
                {
                    string indigoReference = String.Format("ICI{0}{1}", DateTime.Now.ToString("yy"), DateTime.Now.DayOfYear);

                    indigoReference += _datasource.TransactionSequenceDAL.NextSequenceNumber("Tagpaychargetxn", ResetPeriod.DAILY)
                                                    .ToString().PadLeft((16 - indigoReference.Length), '0');

                    customerDetails.FeeReferenceNumber = indigoReference;

                    if (indigoReference.Length > 16)
                    {
                        responseMessage = String.Format("Internal Indigo Reference is to long, must be 16 characters {0}", indigoReference);
                        validResponse = false;
                    }
                    validResponse = true;
                }

            }
            else
            {
                validResponse = false;
                responseMessage = "Failed to Charge Fee";
            }
            return validResponse;
        }

        #endregion

        #region Private Methods
        private string[] ProcessAccLookupResponse(string response)
        {
            _cbsLog.Trace(m => m("Response from Server: " + response));
            string[] processedResponse = new string[10];
            try
            {
                XmlDocument responseXML = new XmlDocument();
                responseXML.LoadXml(response);

                XmlNodeList merchant = responseXML.GetElementsByTagName("merchantid");
                XmlNodeList client = responseXML.GetElementsByTagName("client");
                XmlNodeList firstname = responseXML.GetElementsByTagName("firstname");
                XmlNodeList lastname = responseXML.GetElementsByTagName("lastname");
                XmlNodeList postname = responseXML.GetElementsByTagName("postname");
                XmlNodeList amount = responseXML.GetElementsByTagName("secondarybalance");
                XmlNodeList result = responseXML.GetElementsByTagName("result");
                XmlNodeList currency = responseXML.GetElementsByTagName("secondarycurrency");
                XmlNodeList reservedcurrency = responseXML.GetElementsByTagName("reservecurrency");
                XmlNodeList reserverbalance = responseXML.GetElementsByTagName("reservebalance");

                
                processedResponse[0] = client[0].InnerXml;
                processedResponse[1] = firstname[0].InnerXml;
                processedResponse[2] = lastname[0].InnerXml;
                processedResponse[3] = postname[0].InnerXml;
                processedResponse[4] = amount[0].InnerXml;
                processedResponse[5] = result[0].InnerXml;
                processedResponse[6] = merchant[0].InnerXml; 
                processedResponse[7] = currency[0].InnerXml;
                processedResponse[8] = reservedcurrency[0].InnerXml;
                processedResponse[9] = reserverbalance[0].InnerXml;
            }
            catch (Exception)
            {
                processedResponse = null;
            }
            return processedResponse;
        }

        private string[] ProcessAccLookupResponseNoPin(string response)
        {
            //TEST/DEV METHOD: No PIN
            XmlDocument responseXML = new XmlDocument();
            responseXML.LoadXml(response);

            XmlNodeList merchant = responseXML.GetElementsByTagName("merchantid");
            XmlNodeList client = responseXML.GetElementsByTagName("client");
            XmlNodeList firstname = responseXML.GetElementsByTagName("firstname");
            XmlNodeList lastname = responseXML.GetElementsByTagName("lastname");
            XmlNodeList postname = responseXML.GetElementsByTagName("postname");

            XmlNodeList result = responseXML.GetElementsByTagName("result");
            XmlNodeList status = responseXML.GetElementsByTagName("status");


            string[] processedResponse = new string[7];
            try
            {
                processedResponse[5] = merchant[0].InnerXml;
                processedResponse[0] = client[0].InnerXml;
                processedResponse[1] = firstname[0].InnerXml;
                processedResponse[2] = lastname[0].InnerXml;
                processedResponse[3] = postname[0].InnerXml;
                processedResponse[4] = result[0].InnerXml;
                processedResponse[6] = status[0].InnerText;
                // delete till here ----//
            }
            catch (Exception)
            {
                processedResponse = null;
            }
            return processedResponse;
        }

        private string[] ProcessChargeFeeResponse(string response)
        {
            _cbsLog.Trace(m => m("Response from Server: " + response));

            _cbsLog.Trace(m => m("Converting String to XML"));
            XmlDocument responseXML = new XmlDocument();
            responseXML.LoadXml(response);

            XmlNodeList result = responseXML.GetElementsByTagName("result");
            string[] processedResponse = new string[2];
            processedResponse[0] = result[0].InnerXml;

            return processedResponse;
        }
        public string BuildReference(string branchCode, int seqNo)
        {
            //Build unique indigo reference for this transaction
            string indigoReference = String.Format("{0}{1}", branchCode.PadLeft(3, '0').Substring(0, 3),
                                                                DateTime.Now.ToString("yyyyMMdd"));

            indigoReference += seqNo.ToString().PadLeft((5), '0');

            return indigoReference;
        }




        private AccountDetails BuildAccountDetails(string firstname, string lastname, string ACC, string CCY, string DOB, string address, List<ProductPrinting.IProductPrintField> printFields)
        {
            AccountDetails rtn = new AccountDetails
            {
                AccountNumber = ACC,
                //AccountTypeId = DecodeAccountType(ACCTYPE),

                CurrencyId = DecodeCurrency(CCY),
                FirstName = firstname,
                LastName = lastname,
                CBSAccountTypeId = "1"


            };
            rtn.ProductFields = new List<ProductField>();
            _cbsLog.Debug("Calling productFields ");
            foreach (var item in printFields)
            {
                rtn.ProductFields.Add(new ProductField(item));
            }

            return rtn;
        }
        private int DecodeCurrency(string currency)
        {
            switch (currency)
            {
                case "840": return 1; //USD
                                      // case "001": return 14; //RWF
                default:
                    throw new ArgumentException("Currency of " + currency + " is not known.");
            }
        }

        private int EncodeCurrency(string currency)
        {
            switch (currency)
            {
                case "USD": return 840; //USD
                case "EUR": return 978;
                case "CDF": return 976;
                // case "001": return 14; //RWF
                default:
                    throw new ArgumentException("Currency of " + currency + " is not known.");
            }
        }
        #endregion
    }
}


