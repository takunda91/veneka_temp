using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.Old_App_Code.service;
using indigoCardIssuingWeb.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace indigoCardIssuingWeb.service
{
    public class RenewalService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RenewalService));

        internal bool ExtractCardFile(int issuerId, byte[] fileContent, string fileName, out RenewalFileSummary renewalFileSummary, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalUploadFile(issuerId, fileContent, fileName, encryptedSessionKey);
            renewalFileSummary = new RenewalFileSummary()
            {
                BranchCount = response.Value.BranchCount,
                CardCount = response.Value.CardCount,
                FileName = fileName,
                RenewalId = response.Value.RenewalId
            };
            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool ConfirmLoad(long RenewalId, out RenewalFileSummary renewalFileSummary, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalConfirmUpload(RenewalId, encryptedSessionKey);
            renewalFileSummary = new RenewalFileSummary()
            {
                BranchCount = response.Value.BranchCount,
                CardCount = response.Value.CardCount,
            };
            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool RejectLoad(long RenewalId, out RenewalFileSummary renewalFileSummary, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalRejectUpload(RenewalId, encryptedSessionKey);
            renewalFileSummary = new RenewalFileSummary()
            {
                BranchCount = response.Value.BranchCount,
                CardCount = response.Value.CardCount,
            };
            return base.CheckResponse(response, log, out responseMessage);
        }

        internal List<RenewalFileListModel> ListRenewals(RenewalStatusType statusType)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalList(statusType, encryptedSessionKey);
            return response.Value.ToList();
        }

        internal bool ApproveBulk(List<long> selectedItems, RenewalStatusType statusType, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                    SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                    SecurityParameters.EXTERNAL_SECURITY_KEY);
            bool allDone = true;
            responseMessage = "One or more files coult not be approved.";
            foreach (var item in selectedItems)
            {
                string message = string.Empty;
                var response = m_indigoApp.RenewalApproveUpload(item, encryptedSessionKey);
                allDone = allDone && base.CheckResponse(response, log, out message);
            }
            if (allDone)
            {
                responseMessage = "All files successfully approved.";
            }
            return allDone;
        }

        internal bool Approve(long renewalId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalApproveUpload(renewalId, encryptedSessionKey);
            return base.CheckResponse(response, log, out responseMessage);
        }

        internal bool RejectBulk(List<long> selectedItems, RenewalStatusType statusType, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                    SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                    SecurityParameters.EXTERNAL_SECURITY_KEY);
            bool allDone = true;
            responseMessage = "One or more files could not be approved.";
            foreach (var item in selectedItems)
            {
                string message = string.Empty;
                var response = m_indigoApp.RenewalRejectUpload(item, encryptedSessionKey);
                allDone = allDone && base.CheckResponse(response, log, out message);
            }
            if (allDone)
            {
                responseMessage = "All files successfully rejected.";
            }
            return allDone;
        }

        internal bool Reject(long renewalId, out string responseMessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalRejectUpload(renewalId, encryptedSessionKey);
            return base.CheckResponse(response, log, out responseMessage);

        }

        internal RenewalFileViewModel Retrieve(long fundsLoadId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                      SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                      SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.RenewalRetrieve(fundsLoadId, encryptedSessionKey);
            return response.Value;
        }

        internal List<RenewalDetailListModel> ListRenewalDetails(long renewalId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalDetailList(renewalId, encryptedSessionKey);
            return response.Value.ToList();
        }

        internal RenewalDetailListModel GetRenewalDetail(long renewalDetailId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalDetailRetrieve(renewalDetailId, encryptedSessionKey);
            return response.Value;
        }

        internal RenewalDetailListModel GetRenewalDetailCard(long cardId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalDetailRetrieveByCard(cardId, encryptedSessionKey);
            return response.Value;
        }

        internal RenewalDetailListModel ApproveRenewalDetail(long renewalDetailId, int deliveryBranchId, int currencyId, string cmsAccountType, string cbsAccountType)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalDetailApprove(renewalDetailId, deliveryBranchId, currencyId, cbsAccountType, cmsAccountType, encryptedSessionKey);
            return response.Value;
        }

        internal RenewalDetailListModel RejectRenewalDetail(long renewalDetailId, int deliveryBranchId, string comment)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalDetailReject(renewalDetailId, deliveryBranchId, comment, encryptedSessionKey);
            return response.Value;
        }

        internal RenewalDetailListModel RejectRenewalCardReceived(long renewalDetailId, int deliveryBranchId, string comment)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalDetailRejectCardReceived(renewalDetailId, deliveryBranchId, comment, encryptedSessionKey);
            return response.Value;
        }

        internal List<RenewalBatch> CreateRenewalBatch()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalBatchCreate(encryptedSessionKey);
            return response.Value.ToList();
        }

        internal RenewalBatch ApproveRenewalBatch(long renewalBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalBatchApprove(renewalBatchId, encryptedSessionKey);
            return response.Value;
        }

        internal RenewalBatch DistributeRenewalBatch(long renewalBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalBatchDistribute(renewalBatchId, encryptedSessionKey);
            return response.Value;
        }

        internal RenewalBatch ChangeRenewalBatchStatus(long renewalBatchId, RenewalBatchStatusType statusType)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalBatchChangeStatus(renewalBatchId, statusType, encryptedSessionKey);
            return response.Value;
        }

        internal RenewalBatch RejectRenewalBatch(long renewalBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalBatchReject(renewalBatchId, encryptedSessionKey);
            return response.Value;
        }

        internal RenewalBatch GetRenewalBatch(long renewalBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalBatchRetrieve(renewalBatchId, encryptedSessionKey);
            return response.Value;
        }

        internal List<RenewalDetailListModel> GetRenewalBatchDetails(long renewalBatchId, bool masked)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalBatchRetrieveDetails(renewalBatchId, masked, encryptedSessionKey);
            return response.Value.ToList();
        }

        internal List<RenewalDetailListModel> ListRenewalApprovedCards()
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalDetailInStatus(RenewalDetailStatusType.Approved, encryptedSessionKey);
            return response.Value.ToList();
        }

        internal List<RenewalBatch> RenewalBatches(RenewalBatchStatusType statusType)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalBatchRetrieveList(statusType, encryptedSessionKey);
            return response.Value.ToList();
        }

        internal bool LinkRenewalToCard(long renewalDetailId, long cardId, string cardNumber)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.RenewalLinkCard(renewalDetailId, cardId, cardNumber, encryptedSessionKey);
            return response.Value;
        }

        internal List<long> CreateRenewalDistributionBatches(long renewalBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateRenewalDistributionBatches(renewalBatchId, encryptedSessionKey);
            return response.Value.ToList();
        }

        internal byte[] GenerateRenewalFileReport(long renewalId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);
            return m_indigoApp.GenerateRenewalFileReport(renewalId, encryptedSessionKey).Value;
        }

        internal byte[] GenerateRenewalBatchReport(long renewalBatchId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);
            return m_indigoApp.GenerateRenewalBatchReport(renewalBatchId, encryptedSessionKey).Value;
        }

        internal byte[] GenerateRenewalNewBatches(List<long> renewalBatchIds)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                        SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                        SecurityParameters.EXTERNAL_SECURITY_KEY);
            return m_indigoApp.GenerateRenewalNewBatchReport(renewalBatchIds.ToArray(), encryptedSessionKey).Value;
        }

    }
}