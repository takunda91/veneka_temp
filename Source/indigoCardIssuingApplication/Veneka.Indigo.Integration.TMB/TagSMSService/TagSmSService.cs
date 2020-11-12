using Common.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Xml;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.DAL;
using Veneka.Module.IntegrationDataControl;



namespace Veneka.Indigo.Integration.TMB.TagSMS
{
    public class TagSMSService
    {
        #region Private Fields

        private const string LOGGER = "TagSmsService";
        private static readonly ILog _cbsLog = LogManager.GetLogger("TagSMS");
        private readonly LookupDAL _lookupDAL;
        private readonly string _connectionString;
        private readonly TagSMSRESTAPIService _client;
        private readonly ValidateDataControl _validateData;
        private readonly DefaultDataControl _defaultDataControl;

        #endregion

        #region Constructors
        public TagSMSService(string connectionString)
        {
            this._connectionString = connectionString;
            _client = new TagSMSRESTAPIService();//new Module.Delta.TagSMSRESTAPIService();
            _cbsLog.Trace(m => m("Creating Delta Service Client."));
            _lookupDAL = new LookupDAL(connectionString);
            //_sequenceDAL = new TransactionSequenceDAL(connectionString);
            _validateData = new ValidateDataControl(connectionString);
            _defaultDataControl = new DefaultDataControl(connectionString);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// <summary>
        /// Queries Delta for the desired customers account
        /// </summary>
        /// <param name="acc"></param>
        // public bool SendSMS(Config.WebServiceConfig parameters, string accountNumber, int languageId, out AccountDetails accDetails, out string responseMessage)
        public bool SendSMS(Config.WebServiceConfig parameters,string message, string number, out string responseMessage)
        {
            //accDetails = null;
            responseMessage = String.Empty;

            bool validResponse = false;
            string currency = String.Empty;
            string accountType = String.Empty;

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

            try
            {

                //sms.conecttimeout 3000
                //sms.contenttype application/x-www-form-urlencoded
                //sms.group tmb
                //sms.password xxxxxx
                //sms.sender TMBNetBank
                //sms.sockettimeout                         3000
                //sms.url http://192.168.151.3/smsmanager/send.php 
                //sms.user xxxxxxx               
                //The Response is json like { "code":0,"message":"Planned for delivery","UID":28823684}
                
                string group = "tmb";
                string sender = "TMBNetBank";
                string recipient = number;// accDetails.ContactNumber;
                var smsEndpoint = _client.EndPoint = parameters.Protocol + "://" + parameters.Address + ":" + parameters.Port 
                    + "smsmanager/send.php?group=" + group + "&sender=" + sender + "&recipient=" + recipient
                    + "&content=" + message;
                    _cbsLog.Trace(m => m("SMS Request " + smsEndpoint));
                    var smsRespon = _client.MakeRequest(HttpVerb.POST, "application/x-www-form-urlencoded", "");
                    _cbsLog.Trace(m => m("SMS Response " + smsRespon));
            }
            catch (EndpointNotFoundException endpointException)
            {
                _cbsLog.Error(endpointException);
               // responseMessage = "CBS application not reachable, please try again or contact support.";
                validResponse = false;
            }

            return validResponse;
        }

        #endregion

        #region Private Methods

        private int DecodeCurrency(string currency)
        {
            switch (currency)
            {
                case "002": return 1; //USD
                case "001": return 14; //RWF
                default:
                    throw new ArgumentException("Currency of " + currency + " is not known.");
            }
        }

        private int DecodeAccountType(string acctype)
        {
            //0	CURRENT
            //1	SAVINGS
            //2	CHEQUE
            //3	CREDIT
            //4	UNIVERSAL
            //5	INVESTMENT

            switch (acctype.Trim().ToUpper())
            {
                case "001": return 0;//Current
                case "002": return 0;//Current
                case "003": return 0;//Current
                case "004": return 0;//Current
                case "005": return 0;//Current
                case "006": return 0;//Current
                case "007": return 0;//Current
                case "028": return 0;//Current

                case "035": return 1;//Savings
                case "036": return 1;//Savings
                case "037": return 1;//Savings
                case "038": return 1;//Savings
                case "039": return 1;//Savings
                case "040": return 1;//Savings

                default: throw new ArgumentException("Account type " + acctype + " not known");
            }
        }

        private string[] ProcessAccLookupResponse(string response)
        {
            _cbsLog.Trace(m => m("Response from Server: " + response));

            _cbsLog.Trace(m => m("Converting String to XML"));
            XmlDocument responseXML = new XmlDocument();
            responseXML.LoadXml(response);

            XmlNodeList address = responseXML.GetElementsByTagName("ns1:address");
            XmlNodeList dateOfBirth = responseXML.GetElementsByTagName("ns1:date_of_birth");
            XmlNodeList customerID = responseXML.GetElementsByTagName("ns1:customer_id");
            XmlNodeList id_no = responseXML.GetElementsByTagName("ns1:id_no");
            XmlNodeList phone = responseXML.GetElementsByTagName("ns1:phone_number");
            XmlNodeList firstName = responseXML.GetElementsByTagName("ns1:first_name");
            XmlNodeList LastName = responseXML.GetElementsByTagName("ns1:sir_name");
            XmlNodeList Email = responseXML.GetElementsByTagName("ns1:email");

            string[] processedResponse = new string[8];
            processedResponse[0] = dateOfBirth[0].InnerXml;
            processedResponse[1] = customerID[0].InnerXml;
            processedResponse[2] = id_no[0].InnerXml;
            processedResponse[3] = phone[0].InnerXml;
            processedResponse[4] = firstName[0].InnerXml;
            processedResponse[5] = LastName[0].InnerXml;
            processedResponse[6] = address[0].InnerXml;
            processedResponse[7] = Email[0].InnerXml;
            return processedResponse;
        }
        private string[] ProcessAccBalResponse(string response)
        {
            _cbsLog.Trace(m => m("Response from Server: " + response));

            _cbsLog.Trace(m => m("Converting String to XML"));
            XmlDocument responseXML = new XmlDocument();
            responseXML.LoadXml(response);

            XmlNodeList AvailableBalance = responseXML.GetElementsByTagName("ns1:indicative_balance_actual");
            XmlNodeList AccountType = responseXML.GetElementsByTagName("ns1:account_type");
            XmlNodeList Currency = responseXML.GetElementsByTagName("ns1:currency_code");
            string[] processedResponse = new string[4];
            processedResponse[0] = AvailableBalance[0].InnerXml;
            processedResponse[1] = Currency[0].InnerXml;
            processedResponse[2] = AccountType[0].InnerXml;

            return processedResponse;
        }
        private string[] ProcessChargeFeeResponse(string response)
        {
            _cbsLog.Trace(m => m("Response from Server: " + response));

            _cbsLog.Trace(m => m("Converting String to XML"));
            XmlDocument responseXML = new XmlDocument();
            responseXML.LoadXml(response);

            XmlNodeList responseCode = responseXML.GetElementsByTagName("ns1:responseCode");
            string[] processedResponse = new string[2];

            if (responseCode.Count>0)
            processedResponse[0] = responseCode[0].InnerXml;
            else{
                responseCode = responseXML.GetElementsByTagName("ns2:responseCode");
                if (responseCode.Count > 0)
                    processedResponse[0] = responseCode[0].InnerXml;
            }
            return processedResponse;
        }
        private string BuildHtmlMessage(List<Tuple<string, string>> messages)
        {
            StringBuilder msgBuilder = new StringBuilder();
            foreach (var message in messages)
            {
                msgBuilder.AppendFormat("{0}<br />", message.Item2);
            }

            return msgBuilder.ToString();
        }

        #endregion
    }
}
