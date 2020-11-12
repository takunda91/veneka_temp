using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.security;
using Common.Logging;
using System.Web;
using System.Security.Principal;
using indigoCardIssuingWeb.Old_App_Code.service;

namespace indigoCardIssuingWeb.service
{
    public class FundsLoadService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FundsLoadService));

        internal bool CreateFundsLoad(int issuerId, int productId, int branchId,
            string bankAccountNumber, string prepaidAccountNumber, string prepaidCardNumber, decimal amount,
            string firstName, string lastName, string address,
            out long fundsLoadId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            FundsLoadModel fund = new FundsLoadModel()
            {
                IssuerId = issuerId,
                BranchId = branchId,
                ProductId = productId,
                Amount = amount,
                BankAccountNo = bankAccountNumber,
                PrepaidAccountNo = prepaidAccountNumber,
                PrepaidCardNo = prepaidCardNumber,
                Address = address,
                LastName = lastName,
                Firstname = firstName,
                Status = FundsLoadStatusType.Created
            };
            var response = m_indigoApp.FundsLoadCreate(fund, encryptedSessionKey);
            fundsLoadId = 0;
            return base.CheckResponse(response, log, out responseMessage);
        }

        internal List<FundsLoadListModel> ListFundsLoads(FundsLoadStatusType statusType, int issuerId, int branchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.FundsLoadRetrieveList(statusType, issuerId, branchId, encryptedSessionKey);
            return response.Value.ToList();
        }

        internal bool ApproveBulk(List<long> selectedItems, FundsLoadStatusType statusType, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            messages = "Bulk Approval Successful";
            var response = m_indigoApp.ApproveBulk(statusType, selectedItems.ToArray(), encryptedSessionKey);
            messages = response.ResponseMessage;
            return response.ResponseType == ResponseType.SUCCESSFUL;
        }

        internal FundsLoadListModel Retrieve(long fundsLoadId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.FundsLoadRetrieve(fundsLoadId, encryptedSessionKey);
            return response.Value;
        }

        internal bool RejectBulk(List<long> selectedItems, FundsLoadStatusType statusType, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            messages = "Bulk Decline Successful";
            var response = m_indigoApp.RejectBulk(statusType, selectedItems.ToArray(), encryptedSessionKey);
            messages = response.ResponseMessage;
            return response.ResponseType == ResponseType.SUCCESSFUL;
        }

        internal bool Approve(long selectedItem, FundsLoadStatusType statusType, int cardIssueReasonId, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.Approve(statusType, selectedItem, encryptedSessionKey, cardIssueReasonId);
            if (response.ResponseType == ResponseType.SUCCESSFUL)
            {
                messages = "Funds load approval successful.";
                return true;
            }
            else
            {
                messages = "Funds load approval failed.";
                return false;
            }
        }

        internal bool Reject(long selectedItem, FundsLoadStatusType statusType, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.Reject(statusType, selectedItem, encryptedSessionKey);
            if (response.ResponseType == ResponseType.SUCCESSFUL)
            {
                messages = "Funds load rejection successful.";
                return true;
            }
            else
            {
                messages = "Funds load rejection failed.";
                return false;
            }
        }

        internal bool GetAccountDetail(int issuerId, int productId, int branchId, int cardIssueReasonId, string accountNumber, decimal amount, out AccountDetails accountDetails, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.FundsLoadCheckAccount(issuerId, productId, branchId, cardIssueReasonId, accountNumber, amount, encryptedSessionKey);
            accountDetails = response.Value;

            return base.CheckResponse(response, log, out messages);
        }

        internal bool GetProductsListValidated(int issuerid, int? cardIssueMethodId, int pageIndex, int RowsPerpage, out List<ProductValidated> productsList, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                           SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                           SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.FundsLoadGetProductsList(issuerid, cardIssueMethodId, pageIndex, RowsPerpage, encryptedSessionKey);
            productsList = new List<ProductValidated>();

            if (response.Value != null)
                productsList = response.Value.ToList();

            return base.CheckResponse(response, log, out messages);
        }

        public bool GetPrepaidAccountDetail(int productId, string cardNumber, int mbr, out PrepaidAccountDetail accountDetails, out string messages)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.FundsLoadGetPrepaidAccount(productId, cardNumber, mbr, encryptedSessionKey);
            accountDetails = response.Value;

            return base.CheckResponse(response, log, out messages);
        }
    }

}