using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.EMP.DAL;
using Veneka.Indigo.Integration.Objects;
using Veneka.Module.IntegrationDataControl.DAL;
using Veneka.Module.TranzwareCompassPlusFIMI;
using Veneka.Module.TranzwareCompassPlusFIMI.FIMI;
using Veneka.Module.TranzwareCompassPlusFIMI.ResponseCodes;

namespace Veneka.Indigo.Integration.EMP.FIMI
{
    public class FIMIWebService
    {
        static string loggerName = $"{General.CMS_LOGGER}_EMP.FIMI";
        private static readonly ILog _cbsLog = LogManager.GetLogger(loggerName);
        private readonly FIMIServicesValidated _fimi;
        private readonly WebServiceConfig _parameters;
        //private readonly string _connectionString;
        //private readonly EmpDAL _empDAL;

        #region Constructors
        public FIMIWebService(WebServiceConfig parameters, IDefaultDataDAL dataSource)
        {
            this._parameters = parameters;
            //this._connectionString = connectionString;

            FIMIServicesValidated.Protocol protocol = FIMIServicesValidated.Protocol.HTTP;

            switch (parameters.Protocol)
            {
                case Protocol.HTTP: protocol = FIMIServicesValidated.Protocol.HTTP; break;
                case Protocol.HTTPS: protocol = FIMIServicesValidated.Protocol.HTTPS; break;
                default: break;
            }

            _cbsLog.Trace($"FIMIWebService(WebServiceConfig parameters, IDefaultDataDAL dataSource):: _fimi = new FIMIServicesValidated(...");

            _fimi = new FIMIServicesValidated(protocol, parameters.Address, parameters.Port, parameters.Path,
                                                parameters.Timeout, ServicesValidated.Authentication.NONE, dataSource, parameters.Username,
                                                parameters.Password, loggerName, false);

            //_lookupDAL = new LookupDAL(connectionString);

            //_sequenceDAL = new TransactionSequenceDAL(connectionString);
            //_empDAL = new EmpDAL(connectionString);
        }

        /// <summary>
        /// Use this one when doing remote call to FIMI.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="fileInfo"></param>
        public FIMIWebService(WebServiceConfig parameters, FileInfo fileInfo)
        {
            this._parameters = parameters;

            FIMIServicesValidated.Protocol protocol = FIMIServicesValidated.Protocol.HTTP;

            switch (parameters.Protocol)
            {
                case Protocol.HTTP: protocol = FIMIServicesValidated.Protocol.HTTP; break;
                case Protocol.HTTPS: protocol = FIMIServicesValidated.Protocol.HTTPS; break;
                default: break;
            }
            _cbsLog.Trace($"FIMIWebService(WebServiceConfig parameters, FileInfo fileInfo):: _fimi = new FIMIServicesValidated(...");
            _fimi = new FIMIServicesValidated(protocol, parameters.Address, parameters.RemotePort ?? parameters.Port, parameters.Path,
                                                parameters.Timeout, ServicesValidated.Authentication.NONE, fileInfo,
                                                parameters.RemoteUsername ?? parameters.Username,
                                                parameters.RemotePassword ?? parameters.Password,
                                                General.CMS_LOGGER, false);

            //_lookupDAL = new LookupDAL(connectionString);

            //_sequenceDAL = new TransactionSequenceDAL(connectionString);
            //_empDAL = new EmpDAL(connectionString);
        }
        #endregion

        #region WORK METHODS
        public bool LinkCardToAccountAndActivePan(string pan, int mbr, long auditUserId, string auditWorkStation, out string responseMessage)
        {

            responseMessage = String.Empty;
            int sessionId;
            string sessionKey, nextChallenge;
            _cbsLog.Trace($"LinkCardToAccountAndActivePan");
            if (!_fimi.InitSession(out sessionId, out sessionKey, out nextChallenge))
                return false;

            if (!_fimi.Logon(sessionId, sessionKey, ref nextChallenge))
                return false;

            GetCardInfoRp1 cardInfo = null;
            _cbsLog.Trace($"_fimi.GetCardInfo(pan:={pan}, mbr:={mbr},...");
            int responseCode = _fimi.GetCardInfo(pan, mbr, sessionId, sessionKey, ref nextChallenge, out cardInfo);
            _cbsLog.Trace($"_fimi.GetCardInfo::Response:: cardInfo::{cardInfo.Response.CardUID}");
            
            if (responseCode != 1)
            {
                responseMessage = Response.GetResponse(responseCode);
                return false;
            }

            BLL.Tracer.PrintProperties(cardInfo, 1);

            //do the second action of activating the card
            _cbsLog.Trace($"cardInfo:{cardInfo.Response}");
            _cbsLog.Trace($"_fimi.SetCardStatus::CardUID: {cardInfo.Response.CardUID}");
            string cardUID = cardInfo.Response.CardUID;
            if (string.IsNullOrWhiteSpace(cardUID))
            {
                if (cardInfo.Response.FoundCards == null)
                {
                    _cbsLog.Trace("cardInfo.Response.FoundCards == null");
                }
                else
                {
                    if (cardInfo.Response.FoundCards.Row == null)
                    {
                        _cbsLog.Trace("cardInfo.Response.FoundCards.Row == null");
                    }
                    else
                    {
                        if (cardInfo.Response.FoundCards.Row.Length <= 0)
                        {
                            _cbsLog.Trace($"cardInfo.Response.FoundCards.Row.Length:={cardInfo.Response.FoundCards.Row.Length}");
                        }
                        else
                        {
                            if (cardInfo.Response.FoundCards != null && cardInfo.Response.FoundCards.Row != null && cardInfo.Response.FoundCards.Row.Length > 0)
                            {
                                cardUID = cardInfo.Response.FoundCards.Row[0].CardUID;
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(cardUID))
            {
                responseMessage = "No cards found";
                return false;
            }

            responseCode = _fimi.SetCardStatus(cardUID, 1, sessionId, sessionKey, ref nextChallenge);
            if (responseCode != 1)
            {
                responseMessage = Response.GetResponse(responseCode);
                return false;
            }


            return true;
        }

        public bool LinkCardToAccountAndActive(long cardId, int id, string instName, string fullNames, string accountNumber, int accountTypeId, /*string branchCode,*/ int currency,
            string idNumber, DateTime dob, string address, string postcode, string comment,
            long auditUserId, string auditWorkStation, out string responseMessage)
        {
            responseMessage = String.Empty;
            int sessionId;
            string sessionKey, nextChallenge;
            _cbsLog.Trace($"EMP.FIMIWebService:Start LinkCardToAccountAndActive");
            if (!_fimi.InitSession(out sessionId, out sessionKey, out nextChallenge))
                return false;

            if (!_fimi.Logon(sessionId, sessionKey, ref nextChallenge))
                return false;
            if (address.Length > 35)
            {
                address = address.Substring(0, 35);
            }

            GetPersonInfoRp1 personInfo;
            GetPersonInfoRp1 personInfo2 = null;
            bool usePersonInfo2 = false;

            _cbsLog.Trace($"LinkCardToAccountAndActive:_fimi.GetPersonInfo");
            int responseCode = _fimi.GetPersonInfo(id, instName, sessionId, sessionKey, ref nextChallenge, out personInfo);
            if (responseCode != 1)
            {
                responseMessage = Response.GetResponse(responseCode);
                return false;
            }

            // Check if Person account in FIMI already based on ID/Passport
            if (!String.IsNullOrWhiteSpace(idNumber))
            {
                int count = 0;
                bool done = false;
                IdentificationType idType = IdentificationType.NationalID;

                while (!done && count < 2)
                {
                    count++;
                    responseCode = _fimi.GetPersonInfo(idNumber, idType, instName, sessionId, sessionKey, ref nextChallenge, out personInfo2);
                    if (responseCode != 1)
                    {
                        responseMessage = Response.GetResponse(responseCode);
                        return false;
                    }

                    if (personInfo2 != null && personInfo2.Response.Info != null && personInfo2.Response.Info.Row.Length > 0)
                    {
                        usePersonInfo2 = true;
                        done = true;
                    }
                    else if (idType == IdentificationType.NationalID)
                    {
                        idType = IdentificationType.Passport;
                    }
                    else
                    {
                        done = true;
                    }
                }
            }


            //This was done for UniBank
            //Update PAN in Indigo, this is needed for exporting to other CMS's
            //if (String.IsNullOrWhiteSpace(personInfo.Response.Cards.Row[0].PAN))
            //    throw new ArgumentNullException("PAN", "Empty PAN received from FIMI. Please contact support.");

            //_empDAL.UpdatePAN(cardId, 
            //                    personInfo.Response.Cards.Row[0].PAN.Trim(), 
            //                    personInfo.Response.Cards.Row[0].ExpDate, 
            //                    auditUserId, auditWorkStation);

            if (usePersonInfo2)
            {
                responseCode = _fimi.SetCardPerson(personInfo2.Response.Info.Row[0].PersonId, personInfo.Response.Cards.Row[0].CardUID, sessionId, sessionKey, ref nextChallenge);

                if (responseCode != 1)
                {
                    responseMessage = Response.GetResponse(responseCode);
                    return false;
                }
            }
            else
            {
                responseCode = _fimi.UpdatePerson(instName, (int)personInfo.Response.Info.Row[0].PersonId, fullNames, idNumber, dob, address, postcode, sessionId, sessionKey, ref nextChallenge);

                if (responseCode != 1)
                {
                    responseMessage = Response.GetResponse(responseCode);
                    return false;
                }
            }

            //comment += branchCode;
            _cbsLog.Trace($"LinkCardToAccountAndActive:responseCode = _fimi.CreateAccount(accountNumber, currency, 0, 0,");
            responseCode = _fimi.CreateAccount(accountNumber, currency, 0, 0,
                usePersonInfo2 ? personInfo2.Response.Info.Row[0].PersonId
                                : personInfo.Response.Info.Row[0].PersonId,
                                        EncodeAccountType(accountTypeId),
                                        Module.TranzwareCompassPlusFIMI.Utils.General.AccountStatus.Open, comment,
                                        sessionId, sessionKey, ref nextChallenge);

            if (responseCode == 1 || responseCode == 15)
            {
                _cbsLog.Trace($"LinkCardToAccountAndActive: responseCode == 1 || 15");
                if ((responseCode = _fimi.ResetCard2AcctLinkRq(accountNumber,
                    usePersonInfo2 ? personInfo2.Response.Info.Row[0].PersonId
                                   : personInfo.Response.Info.Row[0].PersonId, personInfo.Response.Cards.Row[0].CardUID, sessionId, sessionKey, ref nextChallenge)) != 1)
                {
                    responseMessage = Response.GetResponse(responseCode);
                    return false;
                }

                _cbsLog.Trace($"LinkCardToAccountAndActive: _fimi.SetCardStatus()!=1");
                if ((responseCode = _fimi.SetCardStatus(personInfo.Response.Cards.Row[0].CardUID, 1, sessionId, sessionKey, ref nextChallenge)) != 1)
                {
                    responseMessage = Response.GetResponse(responseCode);
                    return false;
                }

                var resp = _fimi.ChangeECStatus(personInfo.Response.Cards.Row[0].CardUID, 1, sessionId, sessionKey, ref nextChallenge);

                _cbsLog.Trace($"LinkCardToAccountAndActive: Transition from status #1-'Ready for enrollment' ");
                if (resp.Response.Response != 1 && !resp.Response.Echo.Equals("Transition from status #1-'Ready for enrollment' to status #1-'Ready for enrollment' is not allowed", StringComparison.OrdinalIgnoreCase))
                {
                    responseMessage = Response.GetResponse(resp.Response.Response);
                    return false;
                }
            }
            else
            {
                responseMessage = Response.GetResponse(responseCode);
                return false;
            }

            return true;
        }

        public AccountDetails GetAccountDetails(string idNumber, string instName, out string responseMessage)
        {
            responseMessage = String.Empty;
            int sessionId;
            string sessionKey, nextChallenge;

            if (!_fimi.InitSession(out sessionId, out sessionKey, out nextChallenge))
                return null;

            if (!_fimi.Logon(sessionId, sessionKey, ref nextChallenge))
                return null;

            GetPersonInfoRp1 personInfo;

            int responseCode = _fimi.GetPersonInfo(idNumber, IdentificationType.NationalID, instName, sessionId, sessionKey, ref nextChallenge, out personInfo);
            if (responseCode != 1)
            {
                responseCode = _fimi.GetPersonInfo(idNumber, IdentificationType.Passport, instName, sessionId, sessionKey, ref nextChallenge, out personInfo);
            }

            if (responseCode != 1)
            {
                //failed to get the account details.  return null
                responseMessage = Response.GetResponse(responseCode);
                return null;
            }

            if (personInfo == null && personInfo.Response.Info != null && personInfo.Response.Info.Row.Length > 0)
            {
                responseMessage = "PersonInfo returned empty data.";
                return null;
            }

            if (personInfo.Response.Info == null)
            {
                responseMessage = "PersonInfo returned empty data.";
                return null;
            }

            if (personInfo.Response.Info.Row.Length == 0)
            {
                responseMessage = "PersonInfo returned empty data.";
                return null;
            }

            var coreInfo = personInfo.Response.Info.Row[0];
            AccountDetails accountDetails = new AccountDetails() { NameOnCard = coreInfo.FIO, ProductFields = new List<ProductPrinting.ProductField>() };
            try
            {
                accountDetails.ProductFields.Add(new ProductPrinting.ProductField() { MappedName = "ind_sys_gender", Value = Encoding.UTF8.GetBytes(coreInfo.Sex) });
            }
            catch (Exception)
            {

            }
            try
            {
                accountDetails.ProductFields.Add(new ProductPrinting.ProductField() { MappedName = "ind_sys_dob", Value = Encoding.UTF8.GetBytes(coreInfo.Birthday.Date.ToString()) });
            }
            catch (Exception)
            {

            }
            try
            {
                accountDetails.ProductFields.Add(new ProductPrinting.ProductField() { MappedName = "ind_sys_noc", Value = Encoding.UTF8.GetBytes(coreInfo.FIO) });
            }
            catch (Exception)
            {

            }

            return accountDetails;
        }

        public bool ActivateCard(long cardId, int id, string instName, string fullNames, string accountNumber, int accountTypeId, /*string branchCode,*/ int currency,
            string idNumber, DateTime dob, string address, string postcode, string comment,
            long auditUserId, string auditWorkStation, out string responseMessage)
        {
            responseMessage = String.Empty;
            int sessionId;
            string sessionKey, nextChallenge;

            if (!_fimi.InitSession(out sessionId, out sessionKey, out nextChallenge))
                return false;

            if (!_fimi.Logon(sessionId, sessionKey, ref nextChallenge))
                return false;

            if (address.Length > 35)
            {
                address = address.Substring(0, 35);
            }
            GetPersonInfoRp1 personInfo;


            int responseCode = _fimi.GetPersonInfo(id, instName, sessionId, sessionKey, ref nextChallenge, out personInfo);
            if (responseCode != 1)
            {
                responseMessage = Response.GetResponse(responseCode);
                return false;
            }

            if ((responseCode = _fimi.SetCardStatus(personInfo.Response.Cards.Row[0].CardUID, 1, sessionId, sessionKey, ref nextChallenge)) != 1)
            {
                responseMessage = Response.GetResponse(responseCode);
                return false;
            }

            var resp = _fimi.ChangeECStatus(personInfo.Response.Cards.Row[0].CardUID, 1, sessionId, sessionKey, ref nextChallenge);

            if (resp.Response.Response != 1 && !resp.Response.Echo.Equals("Transition from status #1-'Ready for enrollment' to status #1-'Ready for enrollment' is not allowed", StringComparison.OrdinalIgnoreCase))
            {
                responseMessage = Response.GetResponse(resp.Response.Response);
                return false;
            }
            else
            {
                return true;
            }


        }

        public bool LinkCardsToAccount(long cardId, int id, string instName, string fullNames, string accountNumber, int accountTypeId, /*string branchCode,*/ int currency,
            string idNumber, DateTime dob, string address, string postcode, string comment,
            long auditUserId, string auditWorkStation, out string responseMessage)

        {
            responseMessage = String.Empty;
            int sessionId;
            string sessionKey, nextChallenge;

            if (!_fimi.InitSession(out sessionId, out sessionKey, out nextChallenge))
                return false;

            if (!_fimi.Logon(sessionId, sessionKey, ref nextChallenge))
                return false;

            if (address.Length > 35)
            {
                address = address.Substring(0, 35);
            }
            GetPersonInfoRp1 personInfo;
            GetPersonInfoRp1 personInfo2 = null;
            bool usePersonInfo2 = false;

            int responseCode = _fimi.GetPersonInfo(id, instName, sessionId, sessionKey, ref nextChallenge, out personInfo);
            if (responseCode != 1)
            {
                responseMessage = Response.GetResponse(responseCode);
                return false;
            }

            // Check if Person account in FIMI already based on ID/Passport
            if (!String.IsNullOrWhiteSpace(idNumber))
            {
                int count = 0;
                bool done = false;
                IdentificationType idType = IdentificationType.NationalID;

                while (!done && count < 2)
                {
                    count++;
                    responseCode = _fimi.GetPersonInfo(idNumber, idType, instName, sessionId, sessionKey, ref nextChallenge, out personInfo2);
                    if (responseCode != 1)
                    {
                        responseMessage = Response.GetResponse(responseCode);
                        return false;
                    }

                    if (personInfo2 != null && personInfo2.Response.Info != null && personInfo2.Response.Info.Row.Length > 0)
                    {
                        usePersonInfo2 = true;
                        done = true;
                    }
                    else if (idType == IdentificationType.NationalID)
                    {
                        idType = IdentificationType.Passport;
                    }
                    else
                    {
                        done = true;
                    }
                }
            }


            //This was done for UniBank
            //Update PAN in Indigo, this is needed for exporting to other CMS's
            //if (String.IsNullOrWhiteSpace(personInfo.Response.Cards.Row[0].PAN))
            //    throw new ArgumentNullException("PAN", "Empty PAN received from FIMI. Please contact support.");

            //_empDAL.UpdatePAN(cardId, 
            //                    personInfo.Response.Cards.Row[0].PAN.Trim(), 
            //                    personInfo.Response.Cards.Row[0].ExpDate, 
            //                    auditUserId, auditWorkStation);

            if (usePersonInfo2)
            {
                responseCode = _fimi.SetCardPerson(personInfo2.Response.Info.Row[0].PersonId, personInfo.Response.Cards.Row[0].CardUID, sessionId, sessionKey, ref nextChallenge);

                if (responseCode != 1)
                {
                    responseMessage = Response.GetResponse(responseCode);
                    return false;
                }
            }
            else
            {
                responseCode = _fimi.UpdatePerson(instName, (int)personInfo.Response.Info.Row[0].PersonId, fullNames, idNumber, dob, address, postcode, sessionId, sessionKey, ref nextChallenge);

                if (responseCode != 1)
                {
                    responseMessage = Response.GetResponse(responseCode);
                    return false;
                }
            }




            //TODO: generate account number based on issuer and product and range.
            //string accountNumber = "00000000125";
            //TransactionSequenceDAL seqDal = new TransactionSequenceDAL(this._connectionString);
            //string accountNumber = seqDal.NextSequenceNumber("emp_icbg", ResetPeriod.NONE).ToString("00000000000");


            //comment += branchCode;

            responseCode = _fimi.CreateAccount(accountNumber, currency, 0, 0,
                usePersonInfo2 ? personInfo2.Response.Info.Row[0].PersonId
                                : personInfo.Response.Info.Row[0].PersonId,
                                        EncodeAccountType(accountTypeId),
                                        Module.TranzwareCompassPlusFIMI.Utils.General.AccountStatus.Open, comment,
                                        sessionId, sessionKey, ref nextChallenge);

            if (responseCode == 1 || responseCode == 15)
            {
                if ((responseCode = _fimi.ResetCard2AcctLinkRq(accountNumber,
                    usePersonInfo2 ? personInfo2.Response.Info.Row[0].PersonId
                                   : personInfo.Response.Info.Row[0].PersonId, personInfo.Response.Cards.Row[0].CardUID, sessionId, sessionKey, ref nextChallenge)) != 1)
                {
                    responseMessage = Response.GetResponse(responseCode);
                    return false;
                }

            }
            else
            {
                responseMessage = Response.GetResponse(responseCode);
                return false;
            }

            return true;
        }

        //public void ecCard()
        //{
        //    string nextChallenge = "AABBAA";
        //    var resp = _fimi.ChangeECStatus("abcd", 1, 1, "AABBAA", ref nextChallenge);

        //    if (resp.Response.Response != 1 && !resp.Response.Echo.Equals("Transition from status #1-'Ready for enrollment' to status #1-'Ready for enrollment' is not allowed", StringComparison.OrdinalIgnoreCase))
        //    {
        //        resp.Response.Echo = "abc";
        //        //responseMessage = Response.GetResponse(resp.Response.Response);
        //        //return false;
        //    }
        //}


        public bool FetchPan(int id, string instName, string idNumber, out string PAN, out DateTime? expiryDate, out string responseMessage)
        {
            responseMessage =
            PAN = String.Empty;
            expiryDate = null;

            int sessionId;
            string sessionKey, nextChallenge;

            if (!_fimi.InitSession(out sessionId, out sessionKey, out nextChallenge))
                return false;

            if (!_fimi.Logon(sessionId, sessionKey, ref nextChallenge))
                return false;

            //Check if existing account
            if (!String.IsNullOrWhiteSpace(idNumber))
            {
                int count = 0;
                bool done = false;
                IdentificationType idType = IdentificationType.NationalID;

                GetPersonInfoRp1 personInfo2;
                int responseCode2;
                while (!done && count < 2)
                {
                    count++;
                    responseCode2 = _fimi.GetPersonInfo(idNumber, idType, instName, sessionId, sessionKey, ref nextChallenge, out personInfo2);
                    if (responseCode2 != 1)
                    {
                        responseMessage = Response.GetResponse(responseCode2);
                        return false;
                    }

                    if (personInfo2 != null && personInfo2.Response.Info != null && personInfo2.Response.Info.Row.Length > 0)
                    {
                        PAN = personInfo2.Response.Cards.Row[0].PAN;
                        expiryDate = personInfo2.Response.Cards.Row[0].ExpDate;
                        return true;
                    }
                    else if (idType == IdentificationType.NationalID)
                    {
                        idType = IdentificationType.Passport;
                    }
                    else
                    {
                        done = true;
                    }
                }
            }


            GetPersonInfoRp1 personInfo;
            int responseCode = _fimi.GetPersonInfo(id, instName, sessionId, sessionKey, ref nextChallenge, out personInfo);
            if (responseCode != 1)
            {
                responseMessage = Response.GetResponse(responseCode);
                return false;
            }

            PAN = personInfo.Response.Cards.Row[0].PAN;
            expiryDate = personInfo.Response.Cards.Row[0].ExpDate;
            return true;
        }

        public bool SpoilCard(int id, string instName, out string responseMessage)
        {
            responseMessage = String.Empty;
            int sessionId;
            string sessionKey, nextChallenge;

            if (!_fimi.InitSession(out sessionId, out sessionKey, out nextChallenge))
                return false;

            if (!_fimi.Logon(sessionId, sessionKey, ref nextChallenge))
                return false;

            GetPersonInfoRp1 personInfo;
            int responseCode = _fimi.GetPersonInfo(id, instName, sessionId, sessionKey, ref nextChallenge, out personInfo);

            if (responseCode != 1)
            {
                responseMessage = Response.GetResponse(responseCode);
                return false;
            }

            responseCode = _fimi.SetCardStatus(personInfo.Response.Cards.Row[0].CardUID, 9, sessionId, sessionKey, ref nextChallenge);

            if (responseCode != 1)
            {
                responseMessage = Response.GetResponse(responseCode);
                return false;
            }

            return true;
        }

        public bool CreditPrepaidAccount(decimal amount, string destinationAccountNumber, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            responseMessage = null;

            int sessionId;
            string sessionKey, nextChallenge;

            if (!_fimi.InitSession(out sessionId, out sessionKey, out nextChallenge))
                return false;

            if (!_fimi.Logon(sessionId, sessionKey, ref nextChallenge))
                return false;

            //Check if existing account
            if (!String.IsNullOrWhiteSpace(destinationAccountNumber))
            {
                int count = 0;
                bool done = false;
                IdentificationType idType = IdentificationType.NationalID;

                AcctCreditRp1 creditInfo;
                int responseCode2;
                while (!done && count < 2)
                {
                    count++;
                    responseCode2 = _fimi.CreditPrepaidAccount(sessionId, sessionKey, ref nextChallenge, amount, destinationAccountNumber, out creditInfo);
                    if (responseCode2 != 1)
                    {
                        responseMessage = Response.GetResponse(responseCode2);
                        return false;
                    }

                    if (creditInfo != null && creditInfo.Response.ApprovalCode != null)
                    {
                        return true;
                    }
                    else if (idType == IdentificationType.NationalID)
                    {
                        idType = IdentificationType.Passport;
                    }
                    else
                    {
                        done = true;
                    }
                }
            }

            return true;
        }

        public bool GetPrepaidAccountDetail(string pan, int mbr, long auditUserId, string auditWorkStation, out PrepaidAccountDetail prepaidAccountDetail, out string responseMessage)
        {
            responseMessage = null;
            _cbsLog.Trace("GetPrepaidAccountDetail(....,...,...)");
            int sessionId;
            string sessionKey, nextChallenge;
            prepaidAccountDetail = new PrepaidAccountDetail();
            if (!_fimi.InitSession(out sessionId, out sessionKey, out nextChallenge))
            {
                _cbsLog.Trace("_fimi.InitSession(....,...,...) = FALSE");
                return false;
            }
            if (!_fimi.Logon(sessionId, sessionKey, ref nextChallenge))
            {
                _cbsLog.Trace("_fimi.Logon(....,...,...) = FALSE");
                return false;
            }

            //Check if existing account
            if (!String.IsNullOrWhiteSpace(pan))
            {
                _cbsLog.Trace("GetPrepaidAccountDetail : PAN not empty");
                int count = 0;
                bool done = false;
                IdentificationType idType = IdentificationType.NationalID;

                GetCardInfoRp1 cardInfo;
                int responseCode2;
                while (!done && count < 2)
                {
                    count++;
                    _cbsLog.Trace($"GetPrepaidAccountDetail : count = {count}");
                    responseCode2 = _fimi.GetPrepaidAccountDetail(sessionId, sessionKey, ref nextChallenge, pan, mbr, out cardInfo);
                    if (responseCode2 != 1)
                    {
                        _cbsLog.Trace($"responseCode2 = {responseCode2}");
                        responseMessage = Response.GetResponse(responseCode2);
                        _cbsLog.Trace($"responseMessage = {responseMessage}");
                        return false;
                    }

                    if (cardInfo != null && cardInfo.Response != null)
                    {
                        _cbsLog.Trace($"cardInfo != null and cardInfo.Reponse != null so return true.");
                        if (cardInfo.Response.Accounts != null && cardInfo.Response.Accounts.Row.Length > 0)
                        {
                            _cbsLog.Trace($"cardInfo.Response.Accounts!=null && cardInfo.Response.Account.Row.Length>0");
                            var accountDetail = cardInfo.Response.Accounts.Row[0];
                            if (accountDetail != null)
                            {
                                prepaidAccountDetail = new PrepaidAccountDetail()
                                {
                                    AccountNumber = accountDetail.AcctNo,
                                    AvailBalance = accountDetail.AvailBalance,
                                    LedgerBalance = accountDetail.LedgerBalance,
                                    Status = accountDetail.Status
                                };
                            }
                            else
                            {
                                _cbsLog.Trace("var accountDetail = cardInfo.Response.Accounts.Row[0] GAVE US NULL;");
                            }
                        }
                        else
                        {
                            _cbsLog.Trace("Something wrong with the accounts");
                        }
                        return true;
                    }
                    else if (idType == IdentificationType.NationalID)
                    {
                        _cbsLog.Trace($"Switch to IdentificationType.Passport.");
                        idType = IdentificationType.Passport;
                    }
                    else
                    {
                        _cbsLog.Trace($"done set to true.");
                        done = true;
                    }
                }
            }

            return false;
        }

        private Module.TranzwareCompassPlusFIMI.Utils.General.AccountTypes EncodeAccountType(int accountTypeId)
        {
            switch (accountTypeId)
            {
                case 0: return Module.TranzwareCompassPlusFIMI.Utils.General.AccountTypes.Checkings;
                case 1: return Module.TranzwareCompassPlusFIMI.Utils.General.AccountTypes.Savings;
                case 2: return Module.TranzwareCompassPlusFIMI.Utils.General.AccountTypes.Checkings;
                case 3: return Module.TranzwareCompassPlusFIMI.Utils.General.AccountTypes.Credit;
                case 11:return Module.TranzwareCompassPlusFIMI.Utils.General.AccountTypes.Savings;
                default: throw new ArgumentException("No account type mapped to FIMI account types " + accountTypeId);
            }

        }
        #endregion
    }
}