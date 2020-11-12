using Common.Logging;
using IndigoCardIssuanceService.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.Renewal;
using Veneka.Indigo.Renewal.Entities;
using Veneka.Indigo.Renewal.Incoming;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.COMS.DataSource;
using Veneka.Indigo.Integration;
using Veneka.Indigo.IssuerManagement;

namespace IndigoCardIssuanceService.bll
{
    public class RenewalController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RenewalController));
        private readonly RenewalExtractor _extractor = new RenewalExtractor();
        private readonly CardMangementService _cardManService = new CardMangementService(new LocalDataSource());
        private readonly BranchService _branchService = new BranchService();
        private readonly IRenewalOperations _renewalOperations = new RenewalOperations();

        public RenewalController() : this(new LocalDataSource(), new CardManagementDAL(), null, null, null)
        {

        }

        public RenewalController(IDataSource dataSource, Veneka.Indigo.CardManagement.dal.ICardManagementDAL cardManagementDAL, IIntegrationController integration, IComsCore comsCore, IResponseTranslator translator)
        {

        }

        internal Response<RenewalFileSummary> ExtractFile(int issuerId, byte[] fileContent, string fileName, int languageId, long auditUserId, string auditWorkstation)
        {
            return new Response<RenewalFileSummary>(new RenewalFileSummary()
            {
                BranchCount = 0,
                CardCount = 0,
                FileName = fileName,
                RenewalId = 0
            }, ResponseType.SUCCESSFUL,
                                            string.Empty,
                                            string.Empty);
        }

        internal Response<RenewalFileSummary> ConfirmLoad(long RenewalId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                _renewalOperations.ConfirmFileLoad(RenewalId, auditUserId, auditWorkstation);
                return new Response<RenewalFileSummary>(new RenewalFileSummary()
                {
                    BranchCount = 0,
                    CardCount = 0,
                    FileName = string.Empty,
                    RenewalId = RenewalId
                }, ResponseType.SUCCESSFUL,
                                                                string.Empty,
                                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalFileSummary>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<RenewalFileSummary> RejectLoad(long RenewalId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                _renewalOperations.RejectFileLoad(RenewalId, auditUserId, auditWorkstation);
                return new Response<RenewalFileSummary>(new RenewalFileSummary()
                {
                    BranchCount = 0,
                    CardCount = 0,
                    FileName = string.Empty,
                    RenewalId = RenewalId
                }, ResponseType.SUCCESSFUL,
                                                                string.Empty,
                                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalFileSummary>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<RenewalFileListModel>> GetList(RenewalStatusType status, int languageId, long userId, string workstation)
        {
            try
            {
                var response = _renewalOperations.List(status, languageId, userId, workstation).ToList();
                return new Response<List<RenewalFileListModel>>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<RenewalFileListModel>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<RenewalFileViewModel> Retrieve(long RenewalId, int languageId, long userId, string workstation)
        {
            try
            {
                var response = _renewalOperations.Retrieve(RenewalId, languageId, userId, workstation);
                return new Response<RenewalFileViewModel>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalFileViewModel>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<RenewalFileSummary> ApproveFileLoad(long RenewalId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                _renewalOperations.ApproveFileLoad(RenewalId, auditUserId, auditWorkstation);
                return new Response<RenewalFileSummary>(new RenewalFileSummary()
                {
                    BranchCount = 0,
                    CardCount = 0,
                    FileName = string.Empty,
                    RenewalId = RenewalId
                }, ResponseType.SUCCESSFUL, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalFileSummary>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<RenewalDetailListModel>> GetDetailList(long renewalId, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.ListRenewalDetail(renewalId, true, languageId, userId, auditWorkstation).ToList();
                return new Response<List<RenewalDetailListModel>>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<RenewalDetailListModel>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

        internal Response<List<RenewalDetailListModel>> GetDetailListInStatus(RenewalDetailStatusType statusType, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.ListRenewalDetailInStatus(statusType, true, languageId, userId, auditWorkstation).ToList();
                return new Response<List<RenewalDetailListModel>>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<RenewalDetailListModel>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

        }

        internal Response<RenewalDetailListModel> GetDetail(long renewalDetailId, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.RetrieveDetail(renewalDetailId, true, languageId, userId, auditWorkstation);
                return new Response<RenewalDetailListModel>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalDetailListModel>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<RenewalDetailListModel> GetDetailCard(long cardId, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.RetrieveDetailCard(cardId, true, languageId, userId, auditWorkstation);
                return new Response<RenewalDetailListModel>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalDetailListModel>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<RenewalDetailListModel> VerifyCardRenewal(long renewalDetailId, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.RetrieveDetail(renewalDetailId, true, languageId, userId, auditWorkstation);
                return new Response<RenewalDetailListModel>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalDetailListModel>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<RenewalDetailListModel> ApproveCardRenewal(long renewalDetailId, int deliveryBranchId, int currencyId,
            string cbsAccountType, string cmsAccountType, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.RenewalDetailChangeStatus(renewalDetailId, deliveryBranchId, string.Empty, currencyId, cbsAccountType, cmsAccountType, RenewalDetailStatusType.Approved, userId, auditWorkstation);
                return new Response<RenewalDetailListModel>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalDetailListModel>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<long>> CreateDistributionBatches(long renewalBatchId, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.CreateDistributionBatch(renewalBatchId, userId, auditWorkstation);
                return new Response<List<long>>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<long>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal bool LinkRenewalToCard(long renewalDetailId, long cardId, string cardNumber, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.LinkRenewalToCard(renewalDetailId, cardId, cardNumber, userId, auditWorkstation);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        internal Response<RenewalDetailListModel> RejectCardRenewal(long renewalDetailId, int deliveryBranchId, string comment, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.RenewalDetailChangeStatus(renewalDetailId, deliveryBranchId, comment, 0, string.Empty, string.Empty, RenewalDetailStatusType.Rejected, userId, auditWorkstation);
                return new Response<RenewalDetailListModel>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalDetailListModel>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<RenewalDetailListModel> RejectCardRenewalReceived(long renewalDetailId, int deliveryBranchId, string comment, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.RenewalDetailChangeStatus(renewalDetailId, deliveryBranchId, comment, 0, string.Empty, string.Empty, RenewalDetailStatusType.CardProblem, userId, auditWorkstation);
                return new Response<RenewalDetailListModel>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalDetailListModel>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<RenewalBatch>> CreateBatch(long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.CreateBatches(userId, auditWorkstation);
                return new Response<List<RenewalBatch>>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<RenewalBatch>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<RenewalBatch> ApproveBatch(long renewalBatchId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.ApproveBatch(renewalBatchId, userId, auditWorkstation);
                return new Response<RenewalBatch>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalBatch>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<RenewalBatch> RenewalChangeBatchStatus(long renewalBatchId, RenewalBatchStatusType status, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.RenewalBatchChangeStatus(renewalBatchId, status, userId, auditWorkstation);
                return new Response<RenewalBatch>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalBatch>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        internal Response<RenewalBatch> DistributeBatch(long renewalBatchId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.DistributeBatch(renewalBatchId, userId, auditWorkstation);
                return new Response<RenewalBatch>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalBatch>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<RenewalBatch> RejectBatch(long renewalBatchId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.RejectBatch(renewalBatchId, userId, auditWorkstation);
                return new Response<RenewalBatch>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalBatch>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<RenewalBatch> GetBatch(long renewalBatchId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.RetrieveBatch(renewalBatchId, userId, auditWorkstation);
                return new Response<RenewalBatch>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RenewalBatch>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<RenewalBatch>> GetBatches(RenewalBatchStatusType statusType, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.RetrieveBatches(statusType, userId, auditWorkstation);
                return new Response<List<RenewalBatch>>(response.ToList(), ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<RenewalBatch>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<RenewalDetailListModel>> GetBatchDetails(long renewalBatchId, bool masked, int languageId, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.RetrieveBatchDetails(renewalBatchId, masked, languageId, userId, auditWorkstation);
                return new Response<List<RenewalDetailListModel>>(response.ToList(), ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<RenewalDetailListModel>>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<byte[]> GenerateRenewalFile(long renewalId, int languageId, string username, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.GenerateRenewalFile(renewalId, username, languageId, userId, auditWorkstation);
                return new Response<byte[]>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<byte[]>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<byte[]> GenerateRenewalBatchReport(long renewalBatchId, int languageId, string username, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.GenerateRenewalBatchReport(renewalBatchId, username, languageId, userId, auditWorkstation);
                return new Response<byte[]>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<byte[]>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<byte[]> GenerateRenewalNewBatchReport(List<long> renewalBatchIds, int languageId, string username, long userId, string auditWorkstation)
        {
            try
            {
                var response = _renewalOperations.GenerateRenewalNewBatchReport(renewalBatchIds, username, languageId, userId, auditWorkstation);
                return new Response<byte[]>(response, ResponseType.SUCCESSFUL,
                                                string.Empty,
                                                string.Empty);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<byte[]>(null, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
    }
}