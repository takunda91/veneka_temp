using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Veneka.Indigo.Integration;
using System.ComponentModel.Composition;
using Common.Logging;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.EMP.FIMI;
using Veneka.Module.TranzwareCompassPlusFIMI.FIMI;
using Veneka.Indigo.Integration.Objects;
using Veneka.Module.TranswareCompassPlusBridge;
using System.Globalization;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Remote;
using System.IO;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.Integration.EMP.DAL;

namespace Veneka.Indigo.Integration.EMP
{
    [IntegrationExport("EmpCMS", "5DB55B3A-0816-4EBE-A5DC-32D226558FA9", typeof(ICardManagementSystem))]
    public class EMPCMS : ICardManagementSystem
    {
        static string loggerName = $"{General.CMS_LOGGER}_EMP";
        private static readonly ILog _cmsLog = LogManager.GetLogger(loggerName);

        public event EventHandler<DistEventArgs> OnUploadCompleted;

        //public string SQLConnectionString { get; set; }
        public DirectoryInfo IntegrationFolder { get; set; }

        public bool OnUploadCompletedSubscribed { get; set; }

        public IDataSource DataSource { get; set; }

        /// <summary>
        /// TO DO: Check what the account status should be. Defaulting it to Open
        /// </summary>
        public enum AccountStatus
        {
            InactiveAccount = 0,
            Open = 1,
            DepositOnly = 2,
            PrimaryOpenAccount = 3,
            DepositOnlyPrimaryAccount = 4,
            InformationOnly = 5,
            Closed = 9
        }
        public enum AccountType
        {
            Checking = 1,
            Savings = 11,
            Credit = 31,
            Bonus = 91
        }

        public bool AccountLookup(int issuerId, int productId, int cardIssueReasonId, string accountNumber, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, ref AccountDetails accountDetails, out string responseMessage)
        {
            _cmsLog.Debug("Account Lookup in CMS");
            responseMessage = String.Empty;
            if (cardIssueReasonId == 4)
            {
                var accounts = DataSource.CardsDAL.GetCardsByAccNo(productId, accountNumber, auditUserId, auditWorkStation);
                if (accounts == null || accounts.Count == 0)
                {
                    throw new Exception("Cannot issue a supplimentary card on this account.  No other card has been issued on the account.");
                }
            }
            accountDetails = new AccountDetails();
            _cmsLog.Debug("Account Lookup return true in CMS");
            return true;

        }

        public bool UpdatePVV(int issuerId, int productId, Track2 track2, string PVV, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            throw new NotImplementedException();
        }

        public bool UploadGeneratedCards(List<CardObject> cardObjects, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            responseMessage = String.Empty;
            _cmsLog.Debug("caaling upload method in EMP CMS");
            Config.FileSystemConfig fileConfig = null;
            if (config is FileSystemConfig)
                fileConfig = (FileSystemConfig)config;
            else
                throw new ArgumentException("Config parameters must be for File System.");

            //ParametersDAL pDal = new ParametersDAL(this.SQLConnectionString);
            //LookupDAL lDal = new LookupDAL(this.SQLConnectionString);

            //TransactionSequenceDAL seqDAL = new TransactionSequenceDAL(this.SQLConnectionString);            

            //First get parameter for CMS production
            //var cmsParms = pDal.GetParameterProductInterface(cardObjects[0].ProductId, 1, 0, null, auditUserId, auditWorkStation);

            //if (cmsParms == null)
            //    throw new ArgumentNullException("Cannot find parameters for Card Management System.");
            var item = externalFields.Field.FirstOrDefault(i => i.Key == "FINPROF");
            if (item.Key == null)
            {
                throw new Exception("FINPROF external field not found.");
            }
            item = externalFields.Field.FirstOrDefault(i => i.Key == "CURRENCYNO");
            if (item.Key == null)
            {
                throw new Exception("CURRENCYNO external field not found.");
            }
            item = externalFields.Field.FirstOrDefault(i => i.Key == "GROUPCMD");
            if (item.Key == null)
            {
                throw new Exception("GROUPCMD external field not found.");
            }
            item = externalFields.Field.FirstOrDefault(i => i.Key == "ACCOUNTTP");
            if (item.Key == null)
            {
                throw new Exception("ACCOUNTTP external field not found.");
            }
            string finprof = string.Empty, currencyNo = string.Empty, groupCMD = string.Empty, accountTP = string.Empty;
            finprof = externalFields.Field["FINPROF"];
            currencyNo = externalFields.Field["CURRENCYNO"];
            groupCMD = externalFields.Field["GROUPCMD"];
            accountTP = externalFields.Field["ACCOUNTTP"];
            int cardissuemethodId = 1;
            if (cardObjects.Count > 0)
            {
                cardissuemethodId = cardObjects[0].CardIssueMethodId;
            }
            if (cardissuemethodId == 1)//1 for instant issuance
            {
                List<Record> bridgeRecords = new List<Record>();
                foreach (var card in cardObjects)
                {
                    bridgeRecords.Add(new Record
                    {
                        CARDPREFIX = card.BIN + card.SubProductCode,
                        PASNOM = "indigo" + card.CardId.ToString(),
                        FINPROF = finprof,
                        CURRENCYNO = currencyNo,
                        GROUPCMD = groupCMD
                    });
                }

                //Build filname
                /* XXXX_ZZZZ_Issue_Cards_NNNN_YYYYMMDD.XML where:
                   XXXX is the bank short name.
                   ZZZZ is the prefix of the product.
                   NNNN is the serial of the file in case the bank needed to send more than one file in the same day.
                   YYYYMMDD is the date of creating the file.*/

                int fileSeq = DataSource.TransactionSequenceDAL.NextSequenceNumber("EMP" + cardObjects[0].IssuerCode + cardObjects[0].ProductCode, ResetPeriod.DAILY);
                string fileName = String.Format("{0}_{1}_Issue_Cards_{2}_{3}.XML", cardObjects[0].IssuerCode,
                                                                                   cardObjects[0].BIN,
                                                                                   fileSeq,
                                                                                   DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture));

                TranswareService transwareService = new TranswareService(new DefaultDataDAL(this.DataSource), null);
                transwareService.SerializeBridgeFile(new BridgeFile { Records = bridgeRecords }, fileConfig.Path, fileName, true);
            }
            else if (cardissuemethodId == 0)//0 for centralised issuing
            {
                List<CentralisedRecord> bridgeRecordsCentralised = new List<CentralisedRecord>();
                foreach (var card in cardObjects)
                {
                    DateTime dob = DateTime.Now;
                    string address = String.Empty;
                    string postCode = String.Empty;
                    List<ProductField> productField = card.CustomerAccount.ProductFields;
                    if (productField != null && productField.Count > 0)
                    {
                        foreach (var field in productField)
                        {
                            if (field.MappedName.Equals("ind_sys_dob", StringComparison.OrdinalIgnoreCase))
                            {
                                dob = DateTime.Parse(Encoding.UTF8.GetString(field.Value));
                                _cmsLog.Debug("EMPDOB field" + dob);
                            }

                            if (field.MappedName.Equals("ind_sys_address", StringComparison.OrdinalIgnoreCase))
                            {
                                address = Encoding.UTF8.GetString(field.Value);
                                _cmsLog.Debug("EMP ADDRESS " + address);
                            }

                            if (field.MappedName.Equals("ind_sys_postcode", StringComparison.OrdinalIgnoreCase))
                            {
                                postCode = Encoding.UTF8.GetString(field.Value);
                                _cmsLog.Debug("EMP Postal Code" + postCode);
                            }
                        }
                    }

                    string fio = $"{card.CustomerAccount.FirstName} {card.CustomerAccount.MiddleName}";
                    fio = $"{fio.Trim()} {card.CustomerAccount.LastName}";

                    bridgeRecordsCentralised.Add(new CentralisedRecord
                    {
                        CARDPREFIX = card.BIN + card.SubProductCode,
                        FIO = fio,
                        SEX = "M",
                        PASNOM = "Indigo" + card.CardId.ToString(),
                        CELLPHONE = card.CustomerAccount.ContactNumber,
                        RESADDRESS = $"{address} {postCode}".Trim(),
                        CORADDRESS = $"{address} {postCode}".Trim(),
                        NAMEONCARD = card.CustomerAccount.NameOnCard,
                        EXTACCOUNT = card.CustomerAccount.AccountNumber,
                        ACCOUNT = card.CustomerAccount.AccountNumber,
                        FINPROF = finprof,
                        CURRENCYNO = currencyNo,
                        GROUPCMD = groupCMD,
                        ACCOUNTTP = accountTP,
                        BRPART = card.EMPDeliveryBranchCode
                    });
                }

                //Build filname
                /* XXXX_ZZZZ_Issue_Cards_NNNN_YYYYMMDD.XML where:
                   XXXX is the bank short name.
                   ZZZZ is the prefix of the product.
                   NNNN is the serial of the file in case the bank needed to send more than one file in the same day.
                   YYYYMMDD is the date of creating the file.*/

                int fileSeq = DataSource.TransactionSequenceDAL.NextSequenceNumber("EMP" + cardObjects[0].IssuerCode + cardObjects[0].ProductCode, ResetPeriod.DAILY);
                string fileName = String.Format("{0}_{1}_Issue_Cards_{2}_{3}.XML", cardObjects[0].IssuerCode,
                                                                                   cardObjects[0].BIN,
                                                                                   fileSeq,
                                                                                   DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture));

                TranswareService transwareService = new TranswareService(new DefaultDataDAL(this.DataSource), null);
                transwareService.SerializeCentralisedBridgeFile(new CentralisedBridgeFile { CentralisedRecords = bridgeRecordsCentralised }, fileConfig.Path, fileName, true);

            }
            return true;

        }

        public bool SpoilCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            responseMessage = String.Empty;

            try
            {
                WebServiceConfig webConfig = null;
                if (config is WebServiceConfig)
                    webConfig = (WebServiceConfig)config;
                else
                    throw new ArgumentException("Config parameters must be for Webservice.");

                // LookupDAL lDal = new LookupDAL(this.SQLConnectionString);

                FIMIWebService fimiService = new FIMIWebService(webConfig, new DefaultDataDAL(this.DataSource));

                return fimiService.SpoilCard(int.Parse(customerDetails.CardReference), customerDetails.IssuerCode, out responseMessage);
            }
            catch (System.ServiceModel.EndpointNotFoundException endpointException)
            {
                _cmsLog.Error(endpointException);
                responseMessage = "Unable to connect to FIMI, please try again or contact support.";
            }
            catch (Exception ex)
            {
                _cmsLog.Error(ex);
                responseMessage = ex.Message;
            }

            return false;
        }

        public bool ValidateCustomerDetails(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            responseMessage = String.Empty;
            try
            {
                WebServiceConfig webConfig = null;
                if (config is WebServiceConfig)
                    webConfig = (WebServiceConfig)config;
                else
                    throw new ArgumentException("Config parameters must be for Webservice.");

                FIMIWebService fimiService = new FIMIWebService(webConfig, new DefaultDataDAL(this.DataSource));

                string issuerCode = customerDetails.IssuerCode;
                string customerIDNumber = customerDetails.CustomerIDNumber;

                Dictionary<string, string> currencyFields = customerDetails.CurrencyFields;

                AccountDetails accountLookup = fimiService.GetAccountDetails(customerIDNumber, issuerCode, out responseMessage);
                if (accountLookup != null)
                {
                    PopulateCustomerField(customerDetails, accountLookup, "ind_sys_dob");
                    PopulateCustomerField(customerDetails, accountLookup, "ind_sys_noc");
                    PopulateCustomerField(customerDetails, accountLookup, "ind_sys_gender");
                }
                return true;
            }
            catch (System.ServiceModel.EndpointNotFoundException endpointException)
            {
                _cmsLog.Error(endpointException);
                responseMessage = "Unable to connect to FIMI, please try again or contact support.";
                return false;
            }
            catch (Exception ex)
            {
                _cmsLog.Error(ex);
                responseMessage = ex.Message;
                return false;
            }
        }

        private void PopulateCustomerField(CustomerDetails customerDetails, AccountDetails accountLookup, string mappedFieldName)
        {
            var lookupField = accountLookup.ProductFields.Where(p => p.MappedName == mappedFieldName).FirstOrDefault();
            if (lookupField != null)
            {
                var customerField = customerDetails.ProductFields.Where(p => p.MappedName == mappedFieldName).FirstOrDefault();
                if (customerField == null)
                {
                    customerField = new ProductField() { MappedName = mappedFieldName, Value = lookupField.Value };
                    customerDetails.ProductFields.Add(customerField);
                }
                else
                {
                    customerField.Value = lookupField.Value;
                }
            }
        }

        public bool RemoteFetchDetails(List<CardDetail> cardDetails, ExternalSystemFields externalFields, IConfig config, out List<CardDetailResponse> failedCards, out string responseMessage)
        {
            responseMessage = String.Empty;
            failedCards = new List<CardDetailResponse>();

            try
            {
                WebServiceConfig webConfig = null;
                if (config is WebServiceConfig)
                    webConfig = (WebServiceConfig)config;
                else
                    throw new ArgumentException("Config parameters must be for Webservice.");

                // LookupDAL lDal = new LookupDAL(this.SQLConnectionString);

                FileInfo fileInfo = new FileInfo(Path.Combine(IntegrationFolder.FullName, "Defaults", "IntegratonDefaults.xml"));

                FIMIWebService fimiService = new FIMIWebService(webConfig, fileInfo);

                foreach (var detail in cardDetails)
                {
                    string pan;
                    DateTime? expiryDate;
                    var resp = fimiService.FetchPan(int.Parse(detail.card_request_reference), detail.issuer_code, detail.id_number, out pan, out expiryDate, out responseMessage);

                    if (resp)
                    {
                        detail.card_number = pan;
                        detail.card_expiry_date = expiryDate;
                        //.Debug(d => d(pan + " " + expiryDate.ToString()));
                        //_cmsLog.Debug(d => d(detail.card_number + " " + detail.card_expiry_date.ToString()));
                    }
                    else
                        failedCards.Add(new CardDetailResponse { CardId = detail.card_id, Detail = responseMessage, TimeUpdated = DateTime.Now, UpdateSuccessful = false });
                }

                //check?
                return true;
            }
            catch (System.ServiceModel.EndpointNotFoundException endpointException)
            {
                _cmsLog.Error(endpointException);
                responseMessage = "Unable to connect to FIMI, please try again or contact support.";
            }
            catch (Exception ex)
            {
                _cmsLog.Error(ex);
                responseMessage = ex.Message;
            }

            return false;
        }

        public LinkResponse LinkCardsToAccount(List<CustomerDetails> customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out Dictionary<long, LinkResponse> response, out string responseMessage)
        {

            responseMessage = String.Empty;
            response = new Dictionary<long, LinkResponse>();
            try
            {
                WebServiceConfig webConfig = null;
                if (config is WebServiceConfig)
                    webConfig = (WebServiceConfig)config;
                else
                    throw new ArgumentException("Config parameters must be for Webservice.");

                // LookupDAL lDal = new LookupDAL(this.SQLConnectionString);

                FIMIWebService fimiService = new FIMIWebService(webConfig, new DefaultDataDAL(this.DataSource));
                bool flag = false;
                foreach (var custdetails in customerDetails)
                {
                    string firstName = custdetails.FirstName;
                    string middleName = custdetails.MiddleName;
                    string lastName = custdetails.LastName;
                    int branchcode = custdetails.BranchId;
                    string issuerCode = custdetails.IssuerCode;
                    string accountNumber = custdetails.AccountNumber;
                    int? accountTypeId = custdetails.AccountTypeId;
                    string customerIDNumber = custdetails.CustomerIDNumber;
                    long cardId = custdetails.CardId;
                    string cardReference = custdetails.CardReference;
                    int? currencyId = custdetails.CurrencyId;
                    string currencyCode = custdetails.CurrencyCode;
                    Dictionary<string, string> currencyFields = custdetails.CurrencyFields;
                    List<ProductField> productField = custdetails.ProductFields;

                    string fullname = firstName;

                    if (!String.IsNullOrWhiteSpace(middleName))
                        fullname += " " + middleName;

                    if (!String.IsNullOrWhiteSpace(lastName))
                        fullname += " " + lastName;




                    string branchCode = DataSource.LookupDAL.LookupEmpBranchCode(branchcode);

                    //string branchCode = branchCodes[customerDetails.BranchId];

                    if (String.IsNullOrWhiteSpace(branchCode))
                        throw new ArgumentNullException("EMPBranchCode", "Branch does not have an EMP branch code specified.");


                    DateTime dob = DateTime.Now;
                    string address = String.Empty;
                    string postCode = String.Empty;

                    if (productField != null && productField.Count > 0)
                    {
                        foreach (var field in productField)
                        {
                            if (field.MappedName.Equals("ind_sys_dob", StringComparison.OrdinalIgnoreCase))
                            {
                                string dateofBirth = Encoding.ASCII.GetString(field.Value);
                                _cmsLog.Debug($"LinkCardToAccount(List<CustomerDetails>).ind_sys_dob = {dateofBirth}");
                                DateTime.TryParse(dateofBirth, out dob);
                                _cmsLog.Debug("EMPDOB field" + dob);
                            }

                            if (field.MappedName.Equals("ind_sys_address", StringComparison.OrdinalIgnoreCase))
                            {
                                address = Encoding.UTF8.GetString(field.Value);
                                _cmsLog.Debug("EMP ADDRESS " + address);
                            }

                            if (field.MappedName.Equals("ind_sys_postcode", StringComparison.OrdinalIgnoreCase))
                            {
                                postCode = Encoding.UTF8.GetString(field.Value);
                                _cmsLog.Debug("EMP Postal Code" + postCode);
                            }
                        }
                    }

                    // Comment field is built up by emp product account type and currency, and branch code
                    // Each product's currency must have the emp_account_type variable set.

                    string empAccountType = String.Empty;
                    if (!currencyFields.TryGetValue("emp_account_type", out empAccountType))
                    {
                        responseMessage = String.Format("emp_account_type not set for product currency {0}. Please check product currency configuration.", currencyCode);
                        return LinkResponse.RETRY;
                    }


                    string comment = String.Format("{0}~{1}", empAccountType, branchCode);

                    //string comment = "~";
                    //if (externalFields.Field != null)
                    //    externalFields.Field.TryGetValue("comment", out comment);


                    if (fimiService.LinkCardsToAccount(cardId, int.Parse(cardReference), issuerCode, fullname, accountNumber,
                       accountTypeId.Value, int.Parse(DataSource.LookupDAL.LookupCurrencyISONumericCode(currencyId.Value)), customerIDNumber, dob,
                       address, postCode, comment, auditUserId, auditWorkStation, out responseMessage))
                    //return LinkResponse.SUCCESS;

                    {
                        response.Add(cardId, LinkResponse.SUCCESS);
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                        response.Add(cardId, LinkResponse.ERROR);
                    }
                    //custdetails.ResponseCode = Convert.ToInt32(LinkResponse.ERROR);
                }

                if (flag)
                {
                    return LinkResponse.SUCCESS;
                }
                else
                {
                    return LinkResponse.ERROR;
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException endpointException)
            {
                _cmsLog.Error(endpointException);
                responseMessage = "Unable to connect to FIMI, please try again or contact support.";
                return LinkResponse.RETRY;
            }
            catch (Exception ex)
            {
                _cmsLog.Error(ex);
                responseMessage = ex.Message;
            }

            return LinkResponse.ERROR;
        }

        public LinkResponse LinkCardToAccountAndActive(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            responseMessage = String.Empty;
            LinkResponse linkResponse = LinkResponse.ERROR;
            switch (customerDetails.ActivationMethod)
            {
                case CardActivationMethod.Default:
                    linkResponse = LinkCardToAccountAndActiveDefault(customerDetails, externalFields, config, languageId, auditUserId, auditWorkStation, out responseMessage);
                    break;
                case CardActivationMethod.Option1:
                    linkResponse = LinkCardToAccountAndActiveAlternative1(customerDetails, externalFields, config, languageId, auditUserId, auditWorkStation, out responseMessage);
                    break;
                case CardActivationMethod.Option2:
                    break;
                case CardActivationMethod.Option3:
                    break;
                default:
                    break;
            }
            return linkResponse;
        }

        public LinkResponse LinkCardToAccountAndActiveDefault(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            responseMessage = String.Empty;

            try
            {
                WebServiceConfig webConfig = null;
                if (config is WebServiceConfig)
                    webConfig = (WebServiceConfig)config;
                else
                    throw new ArgumentException("Config parameters must be for Webservice.");

                // LookupDAL lDal = new LookupDAL(this.SQLConnectionString);

                FIMIWebService fimiService = new FIMIWebService(webConfig, new DefaultDataDAL(this.DataSource));

                string fullname = customerDetails.FirstName;

                if (!String.IsNullOrWhiteSpace(customerDetails.MiddleName))
                    fullname += " " + customerDetails.MiddleName;

                if (!String.IsNullOrWhiteSpace(customerDetails.LastName))
                    fullname += " " + customerDetails.LastName;

                string branchCode = DataSource.LookupDAL.LookupEmpBranchCode(customerDetails.BranchId);

                //string branchCode = branchCodes[customerDetails.BranchId];

                if (String.IsNullOrWhiteSpace(branchCode))
                    throw new ArgumentNullException("EMPBranchCode", "Branch does not have an EMP branch code specified.");


                DateTime dob = DateTime.Now;
                string address = String.Empty;
                string postCode = String.Empty;

                if (customerDetails.ProductFields != null && customerDetails.ProductFields.Count > 0)
                {
                    foreach (var field in customerDetails.ProductFields)
                    {
                        if (field.MappedName.Equals("ind_sys_dob", StringComparison.OrdinalIgnoreCase))
                        {
                            string dateofBirth = Encoding.ASCII.GetString(field.Value);
                            DateTime.TryParse(dateofBirth, out dob);
                            // DateTime.Parse(Encoding.UTF8.GetString(field.Value),CultureInfo.InvariantCulture);
                            _cmsLog.Debug("EMPDOB field" + dob);
                        }

                        if (field.MappedName.Equals("ind_sys_address", StringComparison.OrdinalIgnoreCase))
                        {
                            address = Encoding.UTF8.GetString(field.Value);
                            _cmsLog.Debug("EMP ADDRESS " + address);
                        }

                        if (field.MappedName.Equals("ind_sys_postcode", StringComparison.OrdinalIgnoreCase))
                        {
                            postCode = Encoding.UTF8.GetString(field.Value);
                            _cmsLog.Debug("EMP Postal Code" + postCode);
                        }
                    }
                }

                // Comment field is built up by emp product account type and currency, and branch code
                // Each product's currency must have the emp_account_type variable set.

                string empAccountType = String.Empty;
                _cmsLog.Trace($"Before: customerDetails.CurrencyFields.TryGetValue(emp_account_type...");
                if (!customerDetails.CurrencyFields.TryGetValue("emp_account_type", out empAccountType))
                {
                    responseMessage = String.Format("emp_account_type not set for product currency {0}. Please check product currency configuration.", customerDetails.CurrencyCode);
                    return LinkResponse.RETRY;
                }

                string comment = String.Format("{0}~{1}", empAccountType, branchCode);
                _cmsLog.Trace($"EMP.LinkCardToAccountAndActive:comment:={comment}");
                //string comment = "~";
                //if (externalFields.Field != null)
                //    externalFields.Field.TryGetValue("comment", out comment);
                int cms_account_type;
                if (!string.IsNullOrEmpty(customerDetails.CMSAccountType))
                {
                    _cmsLog.Trace($"int.TryParse(customerDetails.CMSAccountType, out cms_account_type);");
                    _cmsLog.Trace($"customerDetails.CMSAccountType: {customerDetails.CMSAccountType}");
                    int.TryParse(customerDetails.CMSAccountType, out cms_account_type);
                    _cmsLog.Trace($"cms_account_type: {cms_account_type}");
                }
                else
                {
                    throw new Exception("CMSAccountType is null");
                }

                _cmsLog.Trace($"EMPCMS.LinkCardToAccountAndActive: make call to fimiService.LinkCardToAccountAndActive...601");
                if (fimiService.LinkCardToAccountAndActive(customerDetails.CardId, int.Parse(customerDetails.CardReference), customerDetails.IssuerCode, fullname, customerDetails.AccountNumber,
                     cms_account_type, int.Parse(DataSource.LookupDAL.LookupCurrencyISONumericCode(customerDetails.CurrencyId.Value)), customerDetails.CustomerIDNumber, dob,
                     address, postCode, comment, auditUserId, auditWorkStation, out responseMessage))
                    return LinkResponse.SUCCESS;
                else
                    return LinkResponse.ERROR;
            }
            catch (System.ServiceModel.EndpointNotFoundException endpointException)
            {
                _cmsLog.Error(endpointException);
                responseMessage = "Unable to connect to FIMI, please try again or contact support.";
                return LinkResponse.RETRY;
            }
            catch (Exception ex)
            {
                _cmsLog.Error(ex);
                responseMessage = ex.Message;
            }

            return LinkResponse.ERROR;
        }

        public LinkResponse LinkCardToAccountAndActiveAlternative1(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            responseMessage = String.Empty;

            try
            {
                WebServiceConfig webConfig = null;
                if (config is WebServiceConfig)
                    webConfig = (WebServiceConfig)config;
                else
                    throw new ArgumentException("Config parameters must be for Webservice.");

                // LookupDAL lDal = new LookupDAL(this.SQLConnectionString);

                FIMIWebService fimiService = new FIMIWebService(webConfig, new DefaultDataDAL(this.DataSource));

                string fullname = customerDetails.FirstName;

                if (!String.IsNullOrWhiteSpace(customerDetails.MiddleName))
                    fullname += " " + customerDetails.MiddleName;

                if (!String.IsNullOrWhiteSpace(customerDetails.LastName))
                    fullname += " " + customerDetails.LastName;

                string branchCode = DataSource.LookupDAL.LookupEmpBranchCode(customerDetails.BranchId);

                //string branchCode = branchCodes[customerDetails.BranchId];

                if (String.IsNullOrWhiteSpace(branchCode))
                    throw new ArgumentNullException("EMPBranchCode", "Branch does not have an EMP branch code specified.");


                DateTime dob = DateTime.Now;
                string address = String.Empty;
                string postCode = String.Empty;

                if (customerDetails.ProductFields != null && customerDetails.ProductFields.Count > 0)
                {
                    foreach (var field in customerDetails.ProductFields)
                    {
                        if (field.MappedName.Equals("ind_sys_dob", StringComparison.OrdinalIgnoreCase))
                        {
                            string dateofBirth = Encoding.ASCII.GetString(field.Value);
                            DateTime.TryParse(dateofBirth, out dob);
                            _cmsLog.Debug("EMPDOB field" + dob);
                        }

                        if (field.MappedName.Equals("ind_sys_address", StringComparison.OrdinalIgnoreCase))
                        {
                            address = Encoding.UTF8.GetString(field.Value);
                            _cmsLog.Debug("EMP ADDRESS " + address);
                        }

                        if (field.MappedName.Equals("ind_sys_postcode", StringComparison.OrdinalIgnoreCase))
                        {
                            postCode = Encoding.UTF8.GetString(field.Value);
                            _cmsLog.Debug("EMP Postal Code" + postCode);
                        }
                    }
                }

                // Comment field is built up by emp product account type and currency, and branch code
                // Each product's currency must have the emp_account_type variable set.

                string empAccountType = String.Empty;
                if (!customerDetails.CurrencyFields.TryGetValue("emp_account_type", out empAccountType))
                {
                    responseMessage = String.Format("emp_account_type not set for product currency {0}. Please check product currency configuration.", customerDetails.CurrencyCode);
                    return LinkResponse.RETRY;
                }

                string comment = String.Format("{0}~{1}", empAccountType, branchCode);

                //string comment = "~";
                //if (externalFields.Field != null)
                //    externalFields.Field.TryGetValue("comment", out comment);
                int cms_account_type;
                if (!string.IsNullOrEmpty(customerDetails.CMSAccountType))
                {
                    int.TryParse(customerDetails.CMSAccountType, out cms_account_type);
                }
                else
                {
                    throw new Exception("CMSAccountType is null");
                }

                if (fimiService.LinkCardToAccountAndActivePan(customerDetails.CardNumber, customerDetails.CardSequence, auditUserId, auditWorkStation, out responseMessage))
                    return LinkResponse.SUCCESS;
                else
                    return LinkResponse.ERROR;
            }
            catch (System.ServiceModel.EndpointNotFoundException endpointException)
            {
                _cmsLog.Error(endpointException);
                responseMessage = "Unable to connect to FIMI, please try again or contact support.";
                return LinkResponse.RETRY;
            }
            catch (Exception ex)
            {
                _cmsLog.Error(ex);
                responseMessage = ex.Message;
            }

            return LinkResponse.ERROR;
        }

        public LinkResponse ActiveCard(CustomerDetails customerDetails, ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            responseMessage = String.Empty;

            try
            {
                WebServiceConfig webConfig = null;
                if (config is WebServiceConfig)
                    webConfig = (WebServiceConfig)config;
                else
                    throw new ArgumentException("Config parameters must be for Webservice.");

                // LookupDAL lDal = new LookupDAL(this.SQLConnectionString);

                FIMIWebService fimiService = new FIMIWebService(webConfig, new DefaultDataDAL(this.DataSource));

                string fullname = customerDetails.FirstName;

                if (!String.IsNullOrWhiteSpace(customerDetails.MiddleName))
                    fullname += " " + customerDetails.MiddleName;

                if (!String.IsNullOrWhiteSpace(customerDetails.LastName))
                    fullname += " " + customerDetails.LastName;

                string branchCode = DataSource.LookupDAL.LookupEmpBranchCode(customerDetails.BranchId);

                //string branchCode = branchCodes[customerDetails.BranchId];

                if (String.IsNullOrWhiteSpace(branchCode))
                    throw new ArgumentNullException("EMPBranchCode", "Branch does not have an EMP branch code specified.");


                DateTime dob = DateTime.Now;
                string address = String.Empty;
                string postCode = String.Empty;

                if (customerDetails.ProductFields != null && customerDetails.ProductFields.Count > 0)
                {
                    foreach (var field in customerDetails.ProductFields)
                    {
                        if (field.MappedName.Equals("ind_sys_dob", StringComparison.OrdinalIgnoreCase))
                        {
                            dob = DateTime.Parse(Encoding.UTF8.GetString(field.Value));
                            _cmsLog.Debug("EMPDOB field" + dob);
                        }

                        if (field.MappedName.Equals("ind_sys_address", StringComparison.OrdinalIgnoreCase))
                        {
                            address = Encoding.UTF8.GetString(field.Value);
                            _cmsLog.Debug("EMP ADDRESS " + address);
                        }

                        if (field.MappedName.Equals("ind_sys_postcode", StringComparison.OrdinalIgnoreCase))
                        {
                            postCode = Encoding.UTF8.GetString(field.Value);
                            _cmsLog.Debug("EMP Postal Code" + postCode);
                        }
                    }
                }

                // Comment field is built up by emp product account type and currency, and branch code
                // Each product's currency must have the emp_account_type variable set.

                string empAccountType = String.Empty;
                if (!customerDetails.CurrencyFields.TryGetValue("emp_account_type", out empAccountType))
                {
                    responseMessage = String.Format("emp_account_type not set for product currency {0}. Please check product currency configuration.", customerDetails.CurrencyCode);
                    return LinkResponse.RETRY;
                }

                string comment = String.Format("{0}~{1}", empAccountType, branchCode);

                //string comment = "~";
                //if (externalFields.Field != null)
                //    externalFields.Field.TryGetValue("comment", out comment);


                if (fimiService.ActivateCard(customerDetails.CardId, int.Parse(customerDetails.CardReference), customerDetails.IssuerCode, fullname, customerDetails.AccountNumber,
                     customerDetails.AccountTypeId.Value, int.Parse(DataSource.LookupDAL.LookupCurrencyISONumericCode(customerDetails.CurrencyId.Value)), customerDetails.CustomerIDNumber, dob,
                     address, postCode, comment, auditUserId, auditWorkStation, out responseMessage))
                    return LinkResponse.SUCCESS;
                else
                    return LinkResponse.ERROR;
            }
            catch (System.ServiceModel.EndpointNotFoundException endpointException)
            {
                _cmsLog.Error(endpointException);
                responseMessage = "Unable to connect to FIMI, please try again or contact support.";
                return LinkResponse.RETRY;
            }
            catch (Exception ex)
            {
                _cmsLog.Error(ex);
                responseMessage = ex.Message;
            }

            return LinkResponse.ERROR;
        }
    }

}
