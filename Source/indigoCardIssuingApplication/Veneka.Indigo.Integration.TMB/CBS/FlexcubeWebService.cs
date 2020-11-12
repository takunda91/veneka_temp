using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.Integration.TMB.Common;
using Veneka.Indigo.Integration.TMB.NanoFlexcubeService;

namespace Veneka.Indigo.Integration.TMB.CBS
{
    public class FlexcubeWebService : WebService
    {

        #region Private Fields
        private const string APP_NAME = "Indigo";
        private const string FLEX_ACC_SUCCESS = "AC-000";
        private const string FLEX_BAL_SUCCESS = "AC-000";
        private const string FLEX_TRAN_SUCCESS = "AC-000";
        private const string FLEX_ACC_NOTFOUND = "AC-002";

        private const string AUTH_TRAN_SUCCESS = "AC-000";
        private const string LOGGER = "CBSWebService";
        private static readonly ILog _cbsLog = LogManager.GetLogger(General.CBS_LOGGER);
        // private readonly LookupDAL _lookupDAL;
        private readonly NanoVenekaFlexcubeWsClient _client;
        private readonly string _connectionString;
        private readonly IDataSource _dataSource;

        // private readonly TransactionSequenceDAL _sequenceDAL;

        #endregion

        #region Constructors
        public FlexcubeWebService(Common.Protocol protocol, string address, int port, string path, int? timeoutMilliSeconds,
                                  Authentication authentication, string username, string password, IDataSource dataSource)
             : base(protocol, address, port, path, timeoutMilliSeconds, authentication, username, password, dataSource, LOGGER)
        {
            //this._connectionString = connectionString;


            _client = new NanoVenekaFlexcubeWsClient(base.BuildBindings("CBSBindings", protocol, timeoutMilliSeconds),
                                            base.BuildEndpointAddress(protocol, address, port, path));

            _client.Endpoint.Behaviors.Add(new Inspector.LogClientBehaviour(authentication == Authentication.BASIC ? true : false,
                                                                               username, password, LOGGER));
            _log.Trace(m => m("Creating Core Banking WebService Client."));
            IgnoreUntrustedSSL = true;
            _dataSource = dataSource;
            // _lookupDAL = new LookupDAL(connectionString);
            // _sequenceDAL = new TransactionSequenceDAL(connectionString);


        }
        #endregion
        #region Properties
        public bool IgnoreUntrustedSSL { get; set; }
        #endregion
        public bool QueryCustAcc(string accountNumber, string branchCode, string username, List<IProductPrintField> printfields, int languageId, out AccountDetails accDetails, out string responseMessage)
        {
            _cbsLog.Trace(t => t("Doing QueryCustAcc"));
            responseMessage = string.Empty;
            try
            {
                accDetails = null;
                var QueryCustAccRequest = new queryCustAccRequest
                {
                    userToken = null,
                    accountNumber = accountNumber,
                    appName = APP_NAME,
                    userId = username,
                    branchCode = branchCode

                };
                var response = _client.queryCustAcc(QueryCustAccRequest);
                _cbsLog.Debug("step 1 ==Got successfull account lookup response");
                if (response != null)
                {
                    _cbsLog.Debug("step 2 ==response not null");
                    //var type = response.GetType();
                    //_cbsLog.Debug("step 2.1 ==GetField Ecode not null" + type.GetField("ECode"));
                    //if (type.GetField("ECode") != null)

                    //{
                    _cbsLog.Debug("step 3 ==GetField Ecode not null");
                    if (response.ECode != null)
                    {
                        _cbsLog.Debug("step 4 ==response.Ecode not null");
                        responseMessage = response.EMessage;
                        return false;
                    }

                    // }

                    else
                        //    _cbsLog.Debug("step 4.1 ==GetField Wcode not null" + type.GetField("WCode"));
                        //if (type.GetField("WCode") != null)
                        //{
                        _cbsLog.Debug("step 5 ==GetField Wcode not null");

                    if (!string.IsNullOrWhiteSpace(response.dorm))
                    {
                        if (response.dorm.Trim().ToLower() == "y")
                        {
                            responseMessage = "Account is dormant";
                            return false;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(response.frozen))
                    {
                        if (response.frozen.Trim().ToLower() == "y")
                        {
                            responseMessage = "Account is frozen";
                            return false;
                        }
                    }


                    if (response.WCode.ToUpper().Equals(FLEX_ACC_SUCCESS))
                    {
                        _cbsLog.Debug("step 6 ==WCode.ToUpper().Equals(FLEX_ACC_SUCCESS===" + response.WCode.ToUpper().Equals(FLEX_ACC_SUCCESS));
                        string custName = response.custName;
                        string ccy = response.ccy;
                        string dob = response.dateOfBirth;
                        string address = response.addr1;
                        if (custName == null)
                        {
                            throw new ArgumentException("customer Name is empty");
                        }
                        if (ccy == null)
                        {
                            throw new ArgumentException("Currency is null");
                        }
                        if (dob == null)
                        {
                            //throw new ArgumentException("date of birth is null");
                        }
                        if (address == null)
                        {
                            throw new ArgumentException("Address is empty");
                        }

                        accDetails = BuildAccountDetails(response.custName, accountNumber, response.accType, response.ccy);
                        accDetails.ProductFields = new List<ProductField>();
                        //set the address fields

                        accDetails.Address1 = (response.addr1 != null ? response.addr1 : string.Empty);
                        accDetails.Address2 = (response.addr2 != null ? response.addr2 : string.Empty);
                        accDetails.Address3 = (response.addr3 != null ? response.addr3 : string.Empty);

                        foreach (var printField in printfields)
                        {
                            if (printField is PrintStringField)
                            {
                                switch (printField.MappedName.ToLower())
                                {
                                    case "ind_sys_dob":
                                        if (dob != null)
                                        {
                                            ((PrintStringField)printField).Value = dob;
                                            accDetails.ProductFields.Add(new ProductField(printField));
                                            _cbsLog.Debug("Date of birth" + response.dateOfBirth);
                                        }
                                        else
                                        {
                                            ((PrintStringField)printField).Value = DateTime.Today.ToString("dd-MM-yyyy");
                                            accDetails.ProductFields.Add(new ProductField(printField));
                                            _cbsLog.Debug("Date of birth set to default " + DateTime.Today.ToString("dd-MM-yyyy"));
                                        }
                                        break;
                                    case "ind_sys_address":
                                        ((PrintStringField)printField).Value = address;
                                        accDetails.ProductFields.Add(new ProductField(printField));
                                        _cbsLog.Debug("address value is :" + response.adesc);
                                        break;
                                    case "ind_sys_postcode":
                                        ((PrintStringField)printField).Value = "0000";
                                        accDetails.ProductFields.Add(new ProductField(printField));
                                        break;
                                    default: accDetails.ProductFields.Add(new ProductField(printField)); break;
                                }

                            }
                            else if (printField is PrintImageField)
                            {
                                ((PrintImageField)printField).Value = new byte[0];
                                accDetails.ProductFields.Add(new ProductField(printField));
                            }


                        }
                        _cbsLog.Debug("step 7 ==Account found");
                        responseMessage = "Account Found";
                        return true;

                    }
                    else
                    {
                        _cbsLog.Debug("step 8 ==" + response.WMessage);
                        responseMessage = response.WMessage;
                        return false;
                    }
                    //}

                }
                else
                {
                    _cbsLog.Debug("step 9 ==Response is empty");
                    responseMessage = "Response is empty";
                    return false;
                }
                //return false;
            }

            catch (Exception ex)
            {
                _cbsLog.Error(ex.InnerException);
                throw ex;

            }
        }




        public bool CheckBalance(CustomerDetails customerDetails, string username, string accountNumber, string branchCode, int languageId, out string responseMessage)
        {
            _cbsLog.Trace(t => t("Doing CheckBalance"));
            _cbsLog.Debug("Branch code =+" + branchCode);
            responseMessage = string.Empty;
            try
            {
                var queryAccBalRequest = new queryAccBalRequest
                {
                    accountNumber = accountNumber,
                    appName = APP_NAME,
                    branchCode = branchCode,

                    userId = username,
                    userToken = null
                };
                _cbsLog.Debug("Branch code =+" + branchCode);
                var respone = _client.queryAccBal(queryAccBalRequest);
                if (respone.WCode.ToUpper().Equals(FLEX_BAL_SUCCESS))
                {
                    decimal? AvailBalance = System.Convert.ToDecimal(respone.avlBal);
                    if (AvailBalance.GetValueOrDefault() < customerDetails.FeeCharge.GetValueOrDefault())
                    {
                        responseMessage = "Available balance of the account is not sufficient.";
                        return false;
                    }

                }
                else
                {
                    responseMessage = respone.ECode + " " + respone.EMessage;
                    return false;
                }
                return true;

            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool ChargeFees(CustomerDetails customerDetails, string accountNumber, string branchCode, string username, string cardType, string password, out string responseMessage)
        {
            _cbsLog.Trace(t => t("Doing charge Fees"));
            responseMessage = string.Empty;
            _cbsLog.Debug("Fee charge==" + customerDetails.FeeCharge.Value);
            _cbsLog.Debug("totalcharged ==" + customerDetails.TotalCharged);

            try
            {
                var chargeFeeUploadRequest = new chargeFeeUploadRequest
                {
                    appName = APP_NAME,
                    branchCode = branchCode,
                    cardType = cardType,
                    sourceAccount = accountNumber,
                    txnAmount = customerDetails.TotalCharged.ToString(),//customerDetails.FeeCharge.Value.ToString(),
                    txnRefnum = null,
                    userId = username,
                    userToken = null

                };

                var response = _client.chargeFeeUpload(chargeFeeUploadRequest);
                if (response.WCode.ToUpper().Equals(FLEX_TRAN_SUCCESS))
                {
                    responseMessage = response.transRef;
                    customerDetails.FeeReferenceNumber = response.transRef;
                    return true;

                }
                else
                {
                    responseMessage = response.ECode + " " + response.EMessage;
                    return false;
                }

            }

            catch (Exception ex)
            {

                throw ex;
            }


        }
        private int DecodeCurrency(string ccy)
        {
            return _dataSource.LookupDAL.LookupCurrency(ccy);

        }

        private string encodeCurrency(int currencyId)
        {
            return _dataSource.LookupDAL.LookupCurrency(currencyId);

        }

        private AccountDetails BuildAccountDetails(string CUSTNAME, string ACC, string ACCTYPE, string CCY)
        {
            string firstName, middlename, lastname;

            DecodeName(CUSTNAME, out firstName, out middlename, out lastname);

            AccountDetails rtn = new AccountDetails
            {
                AccountNumber = ACC,
                //AccountTypeId = DecodeAccountType(ACCTYPE),

                CBSAccountTypeId = ACCTYPE,
                CurrencyId = DecodeCurrency(CCY),
                FirstName = firstName,
                LastName = lastname,
                MiddleName = middlename
            };

            return rtn;
        }
        //private int DecodeAccountType(string acctype)
        //{
        //    //0	CURRENT
        //    //1	SAVINGS
        //    //2	CHEQUE
        //    //3	CREDIT
        //    //4	UNIVERSAL
        //    //5	INVESTMENT

        //    switch (acctype.Trim().ToUpper())
        //    {
        //        case "C": return 0;//to confirm
        //        case "S": return 1;//to confirm
        //        default: throw new ArgumentException("Unknown Account Type.");
        //    }
        //}

        private void DecodeName(string custname, out string firstName, out string middleName, out string lastName)
        {
            firstName = String.Empty;
            middleName = String.Empty;
            lastName = String.Empty;

            string[] splitName = custname.Trim().Split();

            lastName = splitName[splitName.Length - 1].Trim();

            for (int i = 0; i < splitName.Length - 1; i++)
            {
                if (!String.IsNullOrWhiteSpace(splitName[i]))
                {
                    if (String.IsNullOrWhiteSpace(firstName) && i == 0)
                        firstName = splitName[i];
                    else
                        middleName += splitName[i] + " ";
                }
                //if (splitName.Length > 1)
                //    firstName = splitName[0].Trim();

                //if (splitName.Length > 2)
                //{
                //    middleName = splitName[1].Trim();
                //}
            }

            firstName = firstName.Trim();
            middleName = middleName.Trim();
            lastName = lastName.Trim();
        }
    }

}
