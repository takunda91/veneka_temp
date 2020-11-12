using Common.Logging;
using IndigoCardIssuanceService.bll;
using IndigoCardIssuanceService.DataContracts;
using IndigoFileLoader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Web.Script.Services;
using System.Web.Services;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Objects;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.IssuerManagement;
using Veneka.Indigo.UserManagement;
using Veneka.Indigo.UserManagement.objects;
using Veneka.Licensing.Common;
using System.Collections;
using Veneka.Indigo.IssuerManagement.objects;
using IndigoCardIssuanceService.DataContracts.PINReissue;
using Veneka.Indigo.FundsLoad;
using Veneka.Indigo.Renewal.Incoming;
using Veneka.Indigo.Renewal.Entities;
using Veneka.Indigo.DocumentManagement;
using IndigoCardIssuanceService.Models;
using System.Configuration;
using Veneka.Indigo.Common.Models.IssuerManagement;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.PINManagement.objects;

namespace IndigoCardIssuanceService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://indigocardIssuing.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class Service1 : WebService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Service1));
        private readonly SessionManager _SessinManager = SessionManager.GetInstance();
        private readonly AuditServiceContoller _auditServiceController = new AuditServiceContoller();
        private readonly DistributionBatchController _distBatchController = new DistributionBatchController();
        private readonly IssueCardController _issueCardController = new IssueCardController();
        private readonly IssuerManagementController _issuerManController = new IssuerManagementController();
        private readonly LoadBatchController _loadBatchContoller = new LoadBatchController();
        private readonly ThreedBatchController _threedBatchContoller = new ThreedBatchController();
        private readonly LoadCardController _loadCardController = new LoadCardController();
        private readonly PINBatchController _pinBatchController = new PINBatchController();
        private readonly UserManagementController _userManContoller = new UserManagementController();
        private readonly LicenseController _licenseController = new LicenseController();
        private readonly ProductController _productcontroller = new ProductController();
        private readonly LanguageLoopupController _languageController = new LanguageLoopupController();
        private readonly ReportManagementController _reportController = new ReportManagementController();
        private readonly TerminalManagementController _terminalController = new TerminalManagementController();
        private readonly ExportBatchController _exportBatchController = new ExportBatchController();
        private readonly SystemConfigController _systemconfigController = new SystemConfigController();
        private readonly NotificationController _notificationController = new NotificationController();
        private readonly RemoteServicesController _remoteServicesController = new RemoteServicesController();
        private readonly PrintJobController _printJobController = new PrintJobController();
        private readonly FundsLoadController _fundsLoadController = new FundsLoadController();
        private readonly RenewalController _cardRenewalController = new RenewalController();
        private readonly DocumentManagementController _documentManagementController = new DocumentManagementController();
        private readonly RemoteDocumentController _remoteDocumentController = new RemoteDocumentController();
        public Service1()
        {
            if (System.Web.Configuration.WebConfigurationManager.AppSettings["InstrumentationKey"] != null)
                Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = System.Web.Configuration.WebConfigurationManager.AppSettings["InstrumentationKey"];
        }
        #region LOGIN / LOGOUT

        [WebMethod]
        public Response<UserLogInRes> LogIn(string encryptedUsername, string encryptedPassword, string encryptedWorkstation, string encryptedSessionKey)
        {
            if (_SessinManager.isValidSession(encryptedSessionKey, true) != null)
            {
                return _userManContoller.LogIn(encryptedUsername, encryptedPassword, encryptedWorkstation);
            }

            return new Response<UserLogInRes>(null,
                                              ResponseType.SESSION_ERROR,
                                              "Problem with Session, please login again.",
                                              log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse LogOut(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                _SessinManager.EndUserSession(session.SessionKey);
                return _userManContoller.UserLogOut(session.UserId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<decrypted_user> GetProfile(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                string encrypted = Veneka.Indigo.Security.EncryptionManager.EncryptString(session.UserId.ToString(),
                                                                       Veneka.Indigo.Common.Utilities.StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                       Veneka.Indigo.Common.Utilities.StaticFields.EXTERNAL_SECURITY_KEY);
                return _userManContoller.GetUserByUserId(encrypted);
            }

            return new Response<decrypted_user>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #endregion

        #region LOAD BATCH MANAGEMENT

        [WebMethod]
        public Response<List<LoadBatchResult>> GetLoadBatches(string loadBatchReference, int issuerId, LoadBatchStatus? loadBatchStatus, DateTime? startDate, DateTime? endDate,
                                                                int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _loadBatchContoller.GetLoadBatches(loadBatchReference, issuerId, loadBatchStatus, startDate, endDate, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<LoadBatchResult>>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<LoadBatchResult> GetLoadBatch(long loadBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _loadBatchContoller.GetLoadBatch(loadBatchId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<LoadBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<ThreedBatchResult>> GetThreedBatches(string ThreedBatchReference, int issuerId, ThreedBatchStatus? ThreeBatchStatus, DateTime? startDate, DateTime? endDate,
                                                               int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _threedBatchContoller.GetThreedBatches(ThreedBatchReference, issuerId, ThreeBatchStatus, startDate, endDate, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<ThreedBatchResult>>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ThreedBatchResult> GetThreedBatch(long threedBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _threedBatchContoller.GetThreedBatch(threedBatchId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<ThreedBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public BaseResponse ApproveLoadBatch(long loadBatchId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _loadBatchContoller.ApproveLoadBatch(loadBatchId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public BaseResponse ApproveMultipleLoadBatch(ArrayList loadBatchId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _loadBatchContoller.ApproveMultipleLoadBatch(loadBatchId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse RecreateThreedBatch(long ThreedBatchId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _threedBatchContoller.RecreateThreedBatch(ThreedBatchId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public BaseResponse ApproveMultipleThreedBatch(ArrayList ThreedBatchId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _threedBatchContoller.ApproveMultipleThreedBatch(ThreedBatchId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public BaseResponse RejectLoadBatch(long loadBatchId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _loadBatchContoller.RejectLoadBatch(loadBatchId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GenerateLoadBatchReport(long loadBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _loadBatchContoller.GenerateLoadBatchReport(loadBatchId, session.LanguageId, session.Username, session.UserId, session.Workstation);
            }

            return new Response<byte[]>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GeneratePinMailerBatchReport(int pin_batch_header_id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.PinBatchGenerateReport(pin_batch_header_id, session.LanguageId, session.UserId, session.Username, session.Workstation);
            }

            return new Response<byte[]>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<FileLoadResult>> GetFileLoadList(DateTime dateFrom, DateTime dateTo, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _loadBatchContoller.GetFileLoadList(dateFrom, dateTo, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<FileLoadResult>>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<FileHistoryResult>> GetFileHistorys(int? issuerId, DateTime dateFrom, DateTime dateTo, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _loadBatchContoller.GetFileHistorys(issuerId, dateFrom, dateTo, session.UserId, session.Workstation);
            }

            return new Response<List<FileHistoryResult>>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<FileHistoryResult> GetFileHistory(long fileId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _loadBatchContoller.GetFileHistory(fileId, session.UserId, session.Workstation);
            }

            return new Response<FileHistoryResult>(null,
                                                   ResponseType.SESSION_ERROR,
                                                   "Problem with Session, please login again.",
                                                   log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #endregion

        #region EXPORT BATCH MANAGEMENT

        [WebMethod]
        public Response<ExportBatchResult> ApproveExportBatch(long exportBatchId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _exportBatchController.ApproveExportBatch(exportBatchId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<ExportBatchResult>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ExportBatchResult> RejectExportBatch(long exportBatchId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _exportBatchController.RejectExportBatch(exportBatchId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<ExportBatchResult>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ExportBatchResult> RequestExportBatch(long exportBatchId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _exportBatchController.RequestExportBatch(exportBatchId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<ExportBatchResult>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<ExportBatchResult>> SearchExportBatch(int? issuerId, int? productId, int? exportBatchStatusesId,
                                                                string batchReference, DateTime? dateFrom, DateTime? dateTo,
                                                                 int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _exportBatchController.SearchExportBatch(issuerId, productId, exportBatchStatusesId,
                                                                batchReference, dateFrom, dateTo,
                                                                 pageIndex, rowsPerPage, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<ExportBatchResult>>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ExportBatchResult> GetExportBatch(long exportBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _exportBatchController.GetExportBatch(exportBatchId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<ExportBatchResult>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GenerateExportBatchReport(long exportBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _exportBatchController.GenerateExportBatchReport(exportBatchId, session.LanguageId, session.Username, session.UserId, session.Workstation);
            }

            return new Response<byte[]>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #endregion

        #region DISTRIBUTION BATCH MANAGEMENT

        [WebMethod]
        public DistributionBatchStatus ExposeDistBatchStatus(DistributionBatchStatus distBatchStatus)
        {
            return distBatchStatus;
        }

        [WebMethod]
        public LoadBatchStatus ExposeLoadBatchStatus(LoadBatchStatus loadBatchStatus)
        {
            return loadBatchStatus;
        }

        [WebMethod]
        public Response<int> CreateDistributionBatch(int issuerId, int fromBranchId, int toBranchId, int cardIssueMethodId,
                                                    int productId, int batchCardSize,
                                                    int createBatchOption, string startRef, string endRef, long? fromdistbatchid, string encryptedSessionKey, out string dist_batch_ref)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            dist_batch_ref = "0";
            if (session != null)
            {
                return _distBatchController.CreateDistributionBatch(issuerId, fromBranchId, toBranchId, cardIssueMethodId,
                                                                    productId, batchCardSize,
                                                                    createBatchOption, startRef, endRef, fromdistbatchid,
                                                                    session.UserId, session.Workstation, out dist_batch_ref);
            }

            return new Response<int>(0, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<int> GetCentralBatchReferenceNumbers(int issuerId, int fromBranchId, int toBranchId, int cardIssueMethodId,
                                                    int productId, string encryptedSessionKey, out string response, out string refFrom, out string refTo)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            refFrom = string.Empty;
            refTo = string.Empty;
            response = string.Empty;
            if (session != null)
            {
                return _distBatchController.GetCentralBatchReferenceNumbers(issuerId, fromBranchId, toBranchId, cardIssueMethodId,
                                                                    productId, out refFrom, out refFrom);
            }

            return new Response<int>(0, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        //[WebMethod]
        //public BaseResponse CreateDistributionBatchRange(int fromBranchId, int toBranchId, int productId, int? subProductId, string startRef, string endRef, string encryptedSessionKey)
        //{
        //    var session = _SessinManager.isValidSession(encryptedSessionKey);
        //    if (session != null)
        //    {
        //        return _distBatchController.CreateDistributionBatch(branchId, batchCardSize, session.UserId, session.Workstation);
        //    }

        //    return new BaseResponse(ResponseType.SESSION_ERROR,
        //                            "Problem with Session, please login again.",
        //                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        //}

        [WebMethod]
        public Response<CardRequestBatchResponse> CreateCardStockOrder(int issuerId, int branchId, int productId, int cardIssueMethodId, int cardPriority, int batchCardSize, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.CreateCardStockOrder(issuerId, branchId, productId, session.LanguageId, cardIssueMethodId, cardPriority, batchCardSize, session.UserId, session.Workstation);
            }

            return new Response<CardRequestBatchResponse>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<CardRequestBatchResponse> CreateCardRequestBatch(int cardIssueMethodId, int issuerId, int? branchId, int productId, int cardPriorityId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.CreateCardRequestBatch(cardIssueMethodId, issuerId, branchId, productId, cardPriorityId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<CardRequestBatchResponse>(null, ResponseType.SESSION_ERROR,
                                                            "Problem with Session, please login again.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<HybridRequestBatchResponse> CreateHybridRequestBatch(int cardIssueMethodId, int issuerId, int? branchId, int productId, int cardPriorityId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.CreateHybridRequestBatch(cardIssueMethodId, issuerId, branchId, productId, cardPriorityId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<HybridRequestBatchResponse>(null, ResponseType.SESSION_ERROR,
                                                            "Problem with Session, please login again.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<card_request_result>> GetCardRequestList(int issuerid, int? branchId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                //If userId is null, we will use the audit user id as the user id as this will be the Id of the currently logged in user.
                return _distBatchController.GetCardRequestList(issuerid, branchId, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);

            }

            return new Response<List<card_request_result>>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<card_request_result>> GetCardRequestListByIssueMethod(int issuerid, int? branchId, int cardIssueMethodId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                //If userId is null, we will use the audit user id as the user id as this will be the Id of the currently logged in user.
                return _distBatchController.GetCardRequestList(issuerid, branchId, cardIssueMethodId, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);

            }

            return new Response<List<card_request_result>>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<hybrid_request_result>> GetHybridRequestList(int issuerid, int? branchId, int? productId, string batch_reference, int? hybrid_request_statuesId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                //If userId is null, we will use the audit user id as the user id as this will be the Id of the currently logged in user.
                return _distBatchController.GetHybridRequestList(issuerid, branchId, productId, hybrid_request_statuesId, batch_reference, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);

            }

            return new Response<List<hybrid_request_result>>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<DistBatchResult>> GetDistBatchesForUser(long? userId, int? issuerId, string distBatchReference, int? distBatchStatusId, int? flowdistBatchStatusId, int? branchId,
                                                                        int? cardIssueMethodId, int? distBatchTypeId, DateTime? startDate, DateTime? endDate, bool? includeOriginBranch, int rowsPerPage, int pageIndex, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                //If userId is null, we will use the audit user id as the user id as this will be the Id of the currently logged in user.
                return _distBatchController.GetDistBatchesForUser(userId ?? session.UserId, issuerId, distBatchReference, distBatchStatusId, flowdistBatchStatusId, branchId, cardIssueMethodId,
                                                                    distBatchTypeId, startDate, endDate, includeOriginBranch, session.LanguageId, rowsPerPage, pageIndex, session.UserId, session.Workstation);

            }

            return new Response<List<DistBatchResult>>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }



        [WebMethod]
        public Response<List<DistStatusResult>> GetDistBatchesForStatus(long? userId, int? issuerId, string distBatchReference, int? distBatchStatusId, int? branchId,
                                                                       int? cardIssueMethodId, int? distBatchTypeId, DateTime? startDate, DateTime? endDate, bool cardCountForRedist, int rowsPerPage, int pageIndex, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                //If userId is null, we will use the audit user id as the user id as this will be the Id of the currently logged in user.
                return _distBatchController.GetDistBatchesForStatus(userId ?? session.UserId, issuerId, distBatchReference, distBatchStatusId, branchId, cardIssueMethodId,
                                                                    distBatchTypeId, startDate, endDate, cardCountForRedist, session.LanguageId, rowsPerPage, pageIndex, session.UserId, session.Workstation);

            }

            return new Response<List<DistStatusResult>>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<int> GetDistBatchCardCountForRedist(long distBatchId, int productId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.GetDistBatchCardCountForRedist(distBatchId, productId, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<int>(0,
                                    ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<DistBatchResult> GetDistBatch(long distBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.GetDistBatch(distBatchId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<DistBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<DistBatchResult> DistBatchRejectStatus(long distBatchId, int distBatchStatusesId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.DistBatchRejectStatus(distBatchId, distBatchStatusesId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<DistBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<DistBatchResult> DistBatchChangeStatus(long distBatchId, int distBatchStatusesId, int flowDistBatchStatusesId, string notes, bool autogenerateDistBatch, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.DistBatchChangeStatus(distBatchId, distBatchStatusesId, flowDistBatchStatusesId, notes, autogenerateDistBatch, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<DistBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<DistBatchResult> DistBatchChangeStatusRenewal(long distBatchId, int distBatchStatusesId, int flowDistBatchStatusesId, string notes, bool autogenerateDistBatch, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.DistBatchChangeStatusRenewal(distBatchId, distBatchStatusesId, flowDistBatchStatusesId, notes, autogenerateDistBatch, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<DistBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse MultipleDistBatchChangeStatus(ArrayList distbatchId, ArrayList distbatchStatusId, ArrayList flowdisbatchstausId, string notes, bool autogenerateDistBatch, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.MultipleDistBatchChangeStatus(distbatchId, distbatchStatusId, flowdisbatchstausId, notes, autogenerateDistBatch, session.LanguageId, session.UserId, session.Workstation);

            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]


        public Response<DistBatchResult> DistBatchCancel(long distBatchId, int? distBatchStatusesId, int distbatchtypeid, int? cardissuemethod, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.DistBatchCancel(distBatchId, distBatchStatusesId, distbatchtypeid, cardissuemethod, notes, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<DistBatchResult>(null, ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> SpoilPrintBatch(long print_batch_id, int? new_print_batch_statuses_id, string notes,
                                                                       string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.SpoilPrintBatch(print_batch_id, new_print_batch_statuses_id, notes, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        //[WebMethod]
        //public Response<bool> updatePrintBatchStatus(long print_batch_id, int print_batch_statuses_id, int new_print_batch_statuses_id, List<RequestData> requestdata, List<string> cardstospoil, string notes,  string encryptedSessionKey)
        //{
        //    var session = _SessinManager.isValidSession(encryptedSessionKey);
        //    if (session != null)
        //    {
        //        return _distBatchController.updatePrintBatchStatus(print_batch_id, print_batch_statuses_id, new_print_batch_statuses_id, requestdata, cardstospoil, notes, session.LanguageId, session.UserId, session.Workstation);

        //    }

        //    return new Response<bool>(false, ResponseType.SESSION_ERROR,
        //                                         "Problem with Session, please login again.",
        //                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        //}




        [WebMethod]
        public Response<DistBatchResult> RejectProductionBatch(long distBatchId, string notes, List<RejectCardInfo> rejectedCards, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.RejectProductionBatch(distBatchId, notes, rejectedCards, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<DistBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GenerateDistBatchReport(long distBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.GenerateDistBatchReport(distBatchId, session.LanguageId, session.Username, session.UserId, session.Workstation);
            }

            return new Response<byte[]>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GeneratePrintBatchReport(long printBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.GeneratePrintBatchReport(printBatchId, session.LanguageId, session.Username, session.UserId, session.Workstation);
            }

            return new Response<byte[]>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GenerateCheckedOutCardsReport(int? issuerId, long operatorId, int branchId, int? userRoleId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GenerateCheckedOutCardsReport(issuerId, operatorId, branchId, userRoleId, session.Username, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<byte[]>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GenerateCheckedInCardsReport(int branchId, List<structCard> cardList, string operatorUsername, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GenerateCheckedInCardsReport(branchId, cardList, operatorUsername, session.Username, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<byte[]>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<BatchCardInfo>> GetBatchCardInfoPaged(long distBatchId, int rowsPerPage, int pageIndex, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.GetBatchCardInfoPaged(distBatchId, session.LanguageId, rowsPerPage, pageIndex, session.UserId, session.Workstation);
            }

            return new Response<List<BatchCardInfo>>(null,
                                                     ResponseType.SESSION_ERROR,
                                                     "Problem with Session, please login again.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #endregion

        #region PIN MANAGEMENT

        [WebMethod]
        public Response<List<PinBatchResult>> GetPinBatchesForUser(int? issuerId, string pinBatchReference, int? pinBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                               int? pinBatchTypeId, DateTime? startDate, DateTime? endDate, int rowsPerPage, int pageIndex, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.GetPinBatchesForUser(issuerId, pinBatchReference, pinBatchStatusId, branchId, cardIssueMethodId, pinBatchTypeId,
                                                                 startDate, endDate, session.LanguageId, rowsPerPage, pageIndex, session.UserId, session.Workstation);

            }

            return new Response<List<PinBatchResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<PinRequestList>> GetPinRequestsForUser(int? issuerId,string request_status, string reissue_approval_stage, string request_type, int rowsPerPage, int pageIndex, string encryptedSessionKey)
        {
          
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (request_status.Equals("ALL"))
                {
                    return _pinBatchController.GetPinRequestsForUser(issuerId, session.LanguageId, rowsPerPage, pageIndex, session.UserId, session.Workstation);

                }
                else
                {
                    return _pinBatchController.GetPinRequestsForUserForStatus(issuerId, request_status, reissue_approval_stage, request_type, session.LanguageId, rowsPerPage, pageIndex, session.UserId, session.Workstation);

                }

            }

            return new Response<List<PinRequestList>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<PinRequestList>> SearchForPinReIssue(int? issuerId, int? BranchId, int? ProductId, string ProductBin, string lastdigits, string accountnumber,
                                                                   string pinrequestref, int pageindex, int rowsperpage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _pinBatchController.SearchForPinReIssue(issuerId, BranchId, ProductId, ProductBin, lastdigits, accountnumber, pinrequestref, pageindex, rowsperpage, session.UserId, session.Workstation);



            }

            return new Response<List<PinRequestList>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<TerminalCardData>> GetPinBlockForUser(int? issuerId, string request_status, int rowsPerPage, int pageIndex, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (request_status.Equals("ALL"))
                {
                    return _pinBatchController.GetPinBlockForUser(issuerId, session.LanguageId, rowsPerPage, pageIndex, session.UserId, session.Workstation);

                }
                else
                {
                    return _pinBatchController.GetPinBlockForUserForStatus(issuerId, request_status, session.LanguageId, rowsPerPage, pageIndex, session.UserId, session.Workstation);

                }

            }
            return new Response<List<TerminalCardData>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<TerminalCardData>> GetPinBatchForUser(int? issuerId, string request_status, int rowsPerPage, int pageIndex, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (request_status.Equals("ALL"))
                {
                   // return _pinBatchController.GetPinBatchForUser(issuerId, session.LanguageId, rowsPerPage, pageIndex, session.UserId, session.Workstation);

                }
                else
                {
                    return _pinBatchController.GetPinBatchForUserForStatus(issuerId, request_status, session.LanguageId, rowsPerPage, pageIndex, session.UserId, session.Workstation);

                }

            }
            return new Response<List<TerminalCardData>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<PinRequestViewDetails> GetPinRequestDetails(long pin_request_id, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.GetPinRequestViewDetails(pin_request_id, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<PinRequestViewDetails>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<TerminalCardData> GetPinBlockDetails(long pin_request_id, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.GetPinBlockViewDetails(pin_request_id, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<TerminalCardData>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<TerminalCardData> GetPinBatchDetails(long pin_request_id, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.GetPinBatchViewDetails(pin_request_id, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<TerminalCardData>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        [WebMethod]
        public Response<TerminalCardData> GetTerminalCardData(string pan_product_bin, string pan_last_four, string expiry_date, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.GetTerminalData(pan_product_bin, pan_last_four, expiry_date, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<TerminalCardData>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RestWebservicesHandler> GetRestParams(int issuer_id, string webservice_type, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.GetRestParams(issuer_id, webservice_type, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<RestWebservicesHandler>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }



        [WebMethod]
        public Response<ZMKZPKDetails> GetZoneKeys(int? issuer_id, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.GetZoneKeys(issuer_id, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<ZMKZPKDetails>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<PinBatchResult> GetPinBatch(long pinBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.GetPinBatch(pinBatchId, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<PinBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<PinBatchResult> UpdatePinBatchStatus(long pinBatchId, int pinBatchStatusId, int flowDistBatchStatusesId, string statusNote, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.UpdatePinBatchStatus(pinBatchId, pinBatchStatusId, flowDistBatchStatusesId, statusNote, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<PinBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public BaseResponse UpdateMuiltplePinBatchStatus(ArrayList pinBatchId, ArrayList pinBatchStatusId, ArrayList flowPinBatchStatusesId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.UpdateMuiltplePinBatchStatus(pinBatchId, pinBatchStatusId, flowPinBatchStatusesId, notes, session.LanguageId, session.UserId, session.Workstation);

            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<PinBatchResult> PinBatchRejectStatus(long pinBatchId, int pinBatchStatusesId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.PinBatchRejectStatus(pinBatchId, pinBatchStatusesId, notes, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<PinBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse PinMailerReprintRequest(long cardId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.PinMailerReprintRequest(cardId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<PinBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse PinMailerReprintApprove(long cardId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.PinMailerReprintApprove(cardId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<PinBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse PinMailerReprintReject(long cardId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.PinMailerReprintReject(cardId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<PinBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<PinMailerReprintResult>> SearchPinMailerReprint(int? issuerId, int? branchId, int? pinMailerReprintStatusId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.SearchPinMailerReprint(issuerId, branchId, pinMailerReprintStatusId, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<PinMailerReprintResult>>(null,
                                                                ResponseType.SESSION_ERROR,
                                                                "Problem with Session, please login again.",
                                                                log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<PinMailerReprintRequestResult>> PinMailerReprintList(int? issuerId, int? branchId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.PinMailerReprintList(issuerId, branchId, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<PinMailerReprintRequestResult>>(null,
                                                                     ResponseType.SESSION_ERROR,
                                                                     "Problem with Session, please login again.",
                                                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<int> PinMailerReprintBatchCreate(int? cardIssueMethodId, int? issuerId, int? branchId, int? productId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.PinMailerReprintBatchCreate(cardIssueMethodId, issuerId, branchId, productId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<int>(0,
                                     ResponseType.SESSION_ERROR,
                                     "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GeneratePinBatchReport(long pinBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _pinBatchController.GeneratePinBatchReport(pinBatchId, session.LanguageId, session.Username, session.UserId, session.Workstation);
            }

            return new Response<byte[]>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #endregion

        #region TERMINAL MANAGEMENT
        [WebMethod]
        public Response<string> GetOperatorSessionKey(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.GetOperatorSessionKey(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<string>(null, ResponseType.UNSUCCESSFUL, "Problem with Session, please login again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<TerminalSessionKey> GetTerminalSessionKey(string deviceId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.GetTerminalSessionKey(deviceId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<TerminalSessionKey>(null, ResponseType.UNSUCCESSFUL, "Problem with Session, please login again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<TerminalParametersResult> LoadDeviceParameters(int issuerId, string deviceId, Veneka.Indigo.Integration.Common.TerminalCardData cardData, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                //return _terminalController.LoadParameters(issuerId, deviceId, reissue, encryptedKeyUnderLMK, cardData, session.LanguageId, session.UserId, session.Workstation);
                return _terminalController.GetProductParameters(issuerId, deviceId, cardData, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, "Problem with Session, please login again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<PINResponse> UpdatePin(string deviceId, Veneka.Indigo.Integration.Common.TerminalCardData cardData, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _terminalController.TerminalPINReset(deviceId, cardData, session.LanguageId, session.UserId, session.Workstation);
                //return _terminalController.UpdatePin(deviceId, encryptedTpk, cardData, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<PINResponse>(null, ResponseType.UNSUCCESSFUL, "Problem with Session, please login again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<int> CreateTerminal(string terminal_name, string terminal_model, string device_id, int branch_id, int terminal_masterkey_id, string password, bool chkMacUsed, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.CreateTerminal(terminal_name, terminal_model, device_id, branch_id,
                    terminal_masterkey_id, password, chkMacUsed, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<int>(0, ResponseType.UNSUCCESSFUL, "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateTerminal(int terminal_id, string terminal_name, string terminal_model, string device_id, int branch_id, int terminal_masterkey_id, string password, bool chkMacUsed, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.UpdateTerminal(terminal_id, terminal_name, terminal_model, device_id, branch_id,
                    terminal_masterkey_id, password, chkMacUsed, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.UNSUCCESSFUL, "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse DeleteMasterkey(int MasterkeyId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.DeleteMasterkey(MasterkeyId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.UNSUCCESSFUL, "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse DeleteTerminal(int terminal_id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.DeleteTerminal(terminal_id, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.UNSUCCESSFUL, "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<TerminalListResult>> GetTerminalsList(int? IssuerId, int? BranchId, int rowsPerPage, int pageIndex, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.GetTerminalsList(IssuerId, BranchId, session.LanguageId, pageIndex, rowsPerPage);
            }

            return new Response<List<TerminalListResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<MasterkeyResult> GetMasterkey(int MasterkeyId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.GetMasterkey(MasterkeyId);
            }

            return new Response<MasterkeyResult>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<TerminalResult> GetTerminals(int terminal_id, int rowsPerPage, int pageIndex, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.GetTerminals(terminal_id);

            }

            return new Response<TerminalResult>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<TerminalListResult>> SearchTerminals(int? IssuerId, int? BranchId, string terminalname, string deviceid, string terminalmodel, int PageIndex, int RowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.SearchTerminals(IssuerId, BranchId, terminalname, deviceid, terminalmodel, PageIndex, RowsPerPage);

            }

            return new Response<List<TerminalListResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<TerminalTMKIssuerResult>> GetTMKByIssuer(int issuer_id, int PageIndex, int rowsperpage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.GetTMKByIssuer(issuer_id, PageIndex, rowsperpage, session.UserId, session.Workstation);

            }

            return new Response<List<TerminalTMKIssuerResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<int> CreateMasterkey(string Masterkey, string MasterkeyName, int IssuerId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.CreateMasterkey(Masterkey, MasterkeyName, IssuerId, session.UserId, session.Workstation, session.LanguageId);
            }

            return new Response<int>(0, ResponseType.UNSUCCESSFUL, "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateMasterkey(int MasterkeyId, string Masterkey, string MasterkeyName, int IssuerId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _terminalController.UpdateMasterkey(MasterkeyId, Masterkey, MasterkeyName, IssuerId, session.UserId, session.Workstation, session.LanguageId);
            }

            return new BaseResponse(ResponseType.UNSUCCESSFUL, "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #endregion

        #region ISSUE CARD MANAGEMENT

        [WebMethod]
        public DistCardStatus ExposeDistCardStatus(DistCardStatus distCardStatus)
        {
            return distCardStatus;
        }

        [WebMethod]
        public LoadCardStatus ExposeLoadCardStatus(LoadCardStatus loadCardStatus)
        {
            return loadCardStatus;
        }

        [WebMethod]
        public BranchCardStatus ExposeBranchCardStatus(BranchCardStatus branchCardStatus)
        {
            return branchCardStatus;
        }

        [WebMethod]
        public Response<CustomerDetailsResult> GetCustomerDetails(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetCustomerDetails(cardId, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<CustomerDetailsResult>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CardSearchResult>> SearchForCards(long? userId, int? userRoleId, int? issuerId, int? branchId, string cardNumber,
                                                                string cardLastFourDigits, string cardrefnumber, string batchReference,
                                                                int? loadCardStatusId, int? distCardStatusId, int? branchCardStatusId, long? distBatchId, long? pinBatchId, long? threedBatchId,
                                                                string accountNumber, string firstName, string lastName, string cmsId,
                                                                DateTime? dateFrom, DateTime? dateTo, int? cardIssueMethodId,
                                                                int? productId, int? priorityId, int pageIndex, int rowsPerPage,
                                                                string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                //If userId is null, we will use the audit user id as the user id as this will be the Id of the currently logged in user.
                return _issueCardController.SearchForCards(userId ?? session.UserId, userRoleId, issuerId, branchId, cardNumber, cardLastFourDigits,
                                                            cardrefnumber, batchReference, loadCardStatusId, distCardStatusId, branchCardStatusId, distBatchId, pinBatchId, threedBatchId,
                                                            accountNumber, firstName, lastName, cmsId, dateFrom, dateTo, cardIssueMethodId, productId, priorityId,
                                                            pageIndex, rowsPerPage, session.UserId, session.Workstation);

            }

            return new Response<List<CardSearchResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CustomercardsearchResult>> SearchCustomerCardsList(int? issuerid, int? branchid, int? productid, int? cardissuemethodid, int? priorityid,
            string cardrefno, string customeraccountno, bool renewalCards, bool activationSearch, int pageIndex, int RowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                //If userId is null, we will use the audit user id as the user id as this will be the Id of the currently logged in user.
                return _issueCardController.SearchCustomerCardsList(issuerid, branchid, productid, cardissuemethodid, priorityid, cardrefno, customeraccountno,
                    pageIndex, RowsPerPage, session.UserId, session.Workstation, renewalCards, activationSearch);

            }

            return new Response<List<CustomercardsearchResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CardSearchResult>> GetOperatorCardsInProgress(long? userId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetOperatorCardsInProgress(userId ?? session.UserId, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<CardSearchResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<HybridRequestResult>> GetOperatorHybridRequestsInProgress(int? statusId, long? userId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetOperatorHybridRequestsInProgress(statusId, userId ?? session.UserId, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<HybridRequestResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        [WebMethod]
        public Response<PrintBatchResult> UpdatePrintBatchChangeStatus(long printBatchId, int printBatchStatusId, int newPrintBatchStatusesId, string statusNote, bool autogeneratebatch, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.UpdatePrintBatchChangeStatus(printBatchId, printBatchStatusId, newPrintBatchStatusesId, statusNote, autogeneratebatch, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<PrintBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<PrintBatchResult>> GetPrintBatchesForUser(int? issuerId, int? productId, string pinBatchReference, int? pinBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                              DateTime? startDate, DateTime? endDate, int rowsPerPage, int pageIndex, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.GetPrintBatchesForUser(issuerId, productId, pinBatchReference, pinBatchStatusId, branchId, cardIssueMethodId,
                                                                 startDate, endDate, session.LanguageId, rowsPerPage, pageIndex, session.UserId, session.Workstation);

            }

            return new Response<List<PrintBatchResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        [WebMethod]
        public Response<PrintBatchResult> GetPrintBatch(long pinBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.GetPrintBatch(pinBatchId, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<PrintBatchResult>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }



        [WebMethod]
        public BaseResponse UpdateCustomerDetails(long cardId, long customerAccountId, CustomerDetails customerDetails, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.UpdateCustomerDetails(cardId, customerAccountId, customerDetails, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<long>(0, ResponseType.SESSION_ERROR,
                                      "Problem with Session, please login again.",
                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #endregion

        [WebMethod]
        public BaseResponse IssueCardToCustomer(CustomerDetails customerAccount, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.IssueCardToCustomer(customerAccount, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse CreatePinRequest(PinObject PinDetails, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.createPinRequest(PinDetails, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdatePinRequestStatus(PinObject PinDetails, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.updatePinRequestStatus(PinDetails, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdatePinRequestStatusForReissue(PinObject PinDetails, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.UpdatePinRequestStatusForReissue(PinDetails, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse updatePinFileStatus(TerminalCardData PinBlock, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.updatePinFileStatus(PinBlock, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse updateBatchFileStatus(TerminalCardData PinBlock, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.updateBatchFileStatus(PinBlock, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse deletePinBlock(string product_pan_bin_code, string pan_last_four, string expiry_date, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.deletePinBlock(product_pan_bin_code, pan_last_four, expiry_date, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse CreateRestParams(RestWebservicesHandler RestDetails, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.CreateRestParams(RestDetails, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateRestParams(RestWebservicesHandler RestDetails, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.UpdateRestParams(RestDetails, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse IssueCardPinCaptured(long cardId, long? pinAuthUserId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.IssueCardPinCaptured(cardId, pinAuthUserId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse IssueCardPrinted(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.IssueCardPrinted(cardId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse IssueCardPrintError(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.IssueCardPrintError(cardId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse IssueCardSpoil(long cardId, int spoilResaonId, string spoilComments, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.IssueCardSpoil(cardId, spoilResaonId, spoilComments, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse IssueCardComplete(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.IssueCardComplete(cardId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<AccountDetails> GetAccountDetail(int issuerId, int productId, int cardIssueReasonId, int branchId, string accountNumber, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetAccountDetail(issuerId, productId, cardIssueReasonId, branchId, accountNumber, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<AccountDetails>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        /* The function can be moved later on, just trying to follow the account lookup logic ****/
        [WebMethod]
        public Response<DecryptedFields> PinFieldDecryption(int issuerId, ZoneMasterKey zmk, TerminalCardData cardData, TerminalSessionKey zpk, string operatorCode, Product product, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
          
            if (session != null)
            {
                return _terminalController.FieldDecryptionWithZoneMaster(issuerId, zmk, cardData, zpk,  operatorCode, product, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<DecryptedFields>(null,  ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<DecryptedFields> PinFieldDecryptionWithMessaging(int issuerId, ZoneMasterKey zmk, TerminalCardData cardData, TerminalSessionKey zpk, string operatorCode, Product product, CustomerDetailsResult customer, Messaging message_params, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);

            if (session != null)
            {
                return _terminalController.FieldDecryptionWithZoneMasterWithMessaging(issuerId, zmk, cardData, zpk, operatorCode, product, customer, message_params, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<DecryptedFields>(null, ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<HybridRequestResult>> SearchHybridRequestList(int issuerId, int? branchId, int? productId, int? hybridrequeststatusId, string requestreference, int? cardIssueMethodId, bool checkmasking
                                                                        , int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.SearchHybridRequestList(issuerId, branchId, productId, hybridrequeststatusId, requestreference, cardIssueMethodId, checkmasking, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);

            }

            return new Response<List<HybridRequestResult>>(null,
                                                 ResponseType.SESSION_ERROR,
                                                 "Problem with Session, please login again.",
                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<HybridRequestResult>> GetRequestsByPrintBatch(long pinBatchId, int startindex, int size, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.GetRequestsByPrintBatch(pinBatchId, startindex, size, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<List<HybridRequestResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CardSearchResult>> GetCardsInError(long? userId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetCardsInError(userId ?? session.UserId, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<CardSearchResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CardSearchResult>> SearchForReissueCards(int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                //If userId is null, we will use the audit user id as the user id as this will be the Id of the currently logged in user.
                return _issueCardController.SearchForReissueCards(session.UserId, pageIndex, rowsPerPage, session.UserId, session.Workstation);

            }

            return new Response<List<CardSearchResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse MakerChecker(long cardId, Boolean approved, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.MakerChecker(cardId, approved, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<PinReissueWSResult>(null,
                                    ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #region REMOTE SERVICES
        [WebMethod]
        public Response<List<RemoteCardUpdateSearchResult>> SearchRemoteCardUpdates(string pan, int? remoteUpdateStatusesId, int? issuerId, int? branchId, int? productId, DateTime? dateFrom,
                                DateTime? dateTo, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _remoteServicesController.SearchRemoteCardUpdates(pan, remoteUpdateStatusesId, issuerId, branchId, productId, dateFrom,
                             dateTo, pageIndex, rowsPerPage, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<List<RemoteCardUpdateSearchResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RemoteCardUpdateDetailResult> GetRemoteCardDetail(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _remoteServicesController.GetRemoteCardDetail(cardId, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<RemoteCardUpdateDetailResult>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }



        [WebMethod]
        public BaseResponse ChangeRemoteCardsStatus(List<long> cardIds, int remoteUpdateStatusesId, string comment, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _remoteServicesController.ChangeRemoteCardsStatus(cardIds, remoteUpdateStatusesId, comment, session.LanguageId, session.UserId, session.Workstation);

            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse RequestMakerChecker(long requestId, Boolean approved, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.RequestMakerChecker(requestId, approved, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse SpoilBranchCard(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.SpoilBranchCard(cardId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<country>>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<IntegrationController.IntegrationInterface>> ListAvailiableIntegrationInterfaces(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                IntegrationController _integration = IntegrationController.Instance;
                return _integration.GetIntegrationInterfaces();
            }

            return new Response<List<IntegrationController.IntegrationInterface>>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<IntegrationController.IntegrationInterface>> ListAvailiableIntegrationInterfacesByInterfaceTypeId(int interfacetypeid, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                IntegrationController _integration = IntegrationController.Instance;
                return _integration.GetIntegrationInterfacesbyInterfaceid(interfacetypeid);
            }

            return new Response<List<IntegrationController.IntegrationInterface>>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<issuers_Result>> GetIssuers(int? pageIndex, int? Rowsperpage, string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);

            if (session != null)
            {

                //RL ---AUDIT??
                return _issuerManController.GetAllIssuers(session.LanguageId, pageIndex, Rowsperpage);
            }
            else
            {
                return new Response<List<issuers_Result>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
            }
        }

        //[WebMethod]
        //public Response<List<issuer>> GetLdapIssuers(string encryptedSessionKey)
        //{
        //    var session = _SessinManager.isValidSession(encryptedSessionKey);

        //    if (session != null)
        //    {
        //        return _issuerManController.GetLdapIssuers(session.UserId, session.Workstation);
        //    }
        //    else
        //    {
        //        return new Response<List<issuer>>(null,
        //                                          ResponseType.SESSION_ERROR,
        //                                          "Problem with Session, please login again.",
        //                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        //    }
        //}

        [WebMethod]
        public Response<Veneka.Indigo.IssuerManagement.objects.IssuerResult> GetIssuer(int issuerId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetIssuer(issuerId, session.UserId, session.Workstation);
            }

            return new Response<Veneka.Indigo.IssuerManagement.objects.IssuerResult>(null,
                                                                             ResponseType.SESSION_ERROR,
                                                                             "Problem with Session, please login again.",
                                                                             log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<Veneka.Indigo.IssuerManagement.objects.IssuerResult> GetIssuerPinMessage(int issuerId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetIssuerPinMessage(issuerId, session.UserId, session.Workstation);
            }

            return new Response<Veneka.Indigo.IssuerManagement.objects.IssuerResult>(null,
                                                                             ResponseType.SESSION_ERROR,
                                                                             "Problem with Session, please login again.",
                                                                             log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<pin_block_formatResult>> LookupPinBlockFormat(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.LookupPinBlockFormat(session.UserId, session.Workstation);
            }

            return new Response<List<pin_block_formatResult>>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LookupPrintFieldTypes(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.LookupPrintFieldTypes(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        [WebMethod]
        public Response<ProductField[]> GetPrintFieldsByProductid(int productId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetPrintFieldsByProductid(productId);
            }

            return new Response<ProductField[]>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<ProductField[]> GetProductFieldsByCardId(long CardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetProductFieldsByCardId(CardId);
            }

            return new Response<ProductField[]>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        #endregion

        #region LDAP Settings

        [WebMethod]
        public Response<PinReissueWSResult> CancelPINReissue(long pinReissueId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.CancelPINReissue(pinReissueId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<PinReissueWSResult>(null,
                                    ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<PinReissueWSResult> CompletePINReissue(long pinReissueId, string notes, int issuerId, int productId, byte[] index, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.CompletePINReissue(pinReissueId, notes, issuerId, productId, index, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<PinReissueWSResult>(null,
                                    ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<PinReissueWSResult> CompletePINReissue2(long pinReissueId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.CompletePINReissue(pinReissueId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<PinReissueWSResult>(null,
                                    ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LdapSettingsResult>> GetLdapSettings(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetLdapSettings(session.UserId, session.Workstation);
            }

            return new Response<List<LdapSettingsResult>>(null,
                                                    ResponseType.SESSION_ERROR,
                                                    "Problem with Session, please login again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<AuthenticationtypesResult>> GetAuthenticationTypes(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetAuthenticationTypes(session.UserId, session.Workstation);
            }

            return new Response<List<AuthenticationtypesResult>>(null,
                                                    ResponseType.SESSION_ERROR,
                                                    "Problem with Session, please login again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<int> CreateLdapSetting(LdapSettingsResult ldapSetting, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.CreateLdapSetting(ldapSetting, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<int>(0,
                                     ResponseType.SESSION_ERROR,
                                     "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateLdapSetting(LdapSettingsResult ldapSetting, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.UpdateLdapSetting(ldapSetting, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        #endregion

        [WebMethod]
        public Response<CardDetails> GetCardDetails(long cardId, byte[] index, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetCardDetails(cardId, index, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<CardDetails>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<RequestDetails> GetRequestDetails(long requestId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetRequestDetails(requestId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<RequestDetails>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        /// <summary>
        /// Searchers for cards at the branch.
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="cardNumber">Pass null to not search on a card number.</param>
        /// <param name="branchCardStatus">Pass null to search on all branch card statuses.</param>
        /// <param name="operatorUserId">Pass null to search on all users.</param>
        /// <param name="encryptedSessionKey"></param>
        /// <returns></returns>
        [WebMethod]
        public Response<List<CardSearchResult>> SearchBranchCards(int? issuerId, int? branchId, int? userRoleId, int? productId, int? priorityId, int? cardIssueMethodId,
                                                                            string cardNumber, int? branchCardStatusId, long? operatorUserId, int pageIndex, int rowsPerpPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.SearchBranchCards(issuerId, branchId, userRoleId, productId, priorityId, cardIssueMethodId, cardNumber, branchCardStatusId, operatorUserId,
                                                              pageIndex, rowsPerpPage, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<CardSearchResult>>(null,
                                                               ResponseType.SESSION_ERROR,
                                                               "Problem with Session, please login again.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        /// <summary>
        /// Does the same as Search Branch Cards but replaces operator id with the users who called the web service.
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="cardNumber"></param>
        /// <param name="branchCardStatus"></param>
        /// <param name="encryptedSessionKey"></param>
        /// <returns></returns>


        [WebMethod]
        public Response<List<CardSearchResult>> SearchBranchCardsSelf(int branchId, int? productId, int? priorityId, int? cardIssueMethodId, string cardNumber, int? branchCardStatusId,
                                                                        int pageIndex, int rowsPerpPage, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.SearchBranchCards(null, branchId, null, productId, priorityId, cardIssueMethodId, cardNumber, branchCardStatusId, session.UserId,
                                                              pageIndex, rowsPerpPage, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<List<CardSearchResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<RemoteCardUpdateDetailResult> ChangeRemoteCardStatus(long cardId, int remoteUpdateStatusesId, string comment, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _remoteServicesController.ChangeRemoteCardStatus(cardId, remoteUpdateStatusesId, comment, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<RemoteCardUpdateDetailResult>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        [WebMethod]
        public Response<List<SearchBranchCardsResult>> CheckInOutCards(long operatorUserId, int branchId, int productId, List<long> checkedOutCards, List<long> checkedInCards, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.CheckInOutCards(operatorUserId, branchId, productId, checkedOutCards, checkedInCards,
                                                              session.UserId, session.Workstation);
            }

            return new Response<List<SearchBranchCardsResult>>(null,
                                                               ResponseType.SESSION_ERROR,
                                                               "Problem with Session, please login again.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<branch_card_codes>> ListBranchCardCodes(int BranchCardCodeType, bool isException, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.ListBranchCardCodes(BranchCardCodeType, isException, session.UserId, session.Workstation);
            }

            return new Response<List<branch_card_codes>>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<card_priority>> GetCardPriorityList(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetCardPriorityList(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<card_priority>>(null,
                                                     ResponseType.SESSION_ERROR,
                                                     "Problem with Session, please login again.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #region CLASSIC CARD METHODS

        [WebMethod]
        public Response<long> RequestCardForCustomer(CustomerDetails customerDetails, long? renewalDetailId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.RequestCardForCustomer(customerDetails, renewalDetailId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<long>(0, ResponseType.SESSION_ERROR,
                                      "Problem with Session, please login again.",
                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<long> RequestInstantCardForCustomer(CustomerDetails customerDetails, out string printJobId, string encryptedSessionKey)
        {
            printJobId = string.Empty;
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.RequestInstantCardForCustomer(customerDetails, out printJobId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<long>(0, ResponseType.SESSION_ERROR,
                                      "Problem with Session, please login again.",
                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<long> RequestHybridCardForCustomer(CustomerDetails customerDetails, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.RequestHybridCardForCustomer(customerDetails, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<long>(0, ResponseType.SESSION_ERROR,
                                      "Problem with Session, please login again.",
                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }








        [WebMethod]
        public BaseResponse InsertAudit(int audit_action_id, string description, int issuerID, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _auditServiceController.InsertAudit(session.UserId, audit_action_id, description, session.Workstation, issuerID);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }




        [WebMethod]
        public BaseResponse LinkCardToCustomer(long cardId, byte[] index, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.LinkCardToCustomer(cardId, index, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse ActiveCard(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.ActiveCard(cardId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UploadBatchtoCMS(long printbatchId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.UploadBatchtoCMS(printbatchId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<string> EPinRequest(int issuerId, int branchId, int productId, string moblieNumber, string pan, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.EPinRequest(issuerId, branchId, productId, moblieNumber, pan, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<string>("", ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        //[WebMethod]
        //public BaseResponse PINReissue(int issuerId, int branchId, int productId, long? authoriseUserId, byte[] index, string encryptedSessionKey)
        //{
        //    var session = _SessinManager.isValidSession(encryptedSessionKey);
        //    if (session != null)
        //    {
        //        return _issueCardController.PINReissue(issuerId, branchId, productId, authoriseUserId, index, session.SessionKey, session.LanguageId, session.UserId, session.Workstation);
        //    }

        //    return new BaseResponse(ResponseType.SESSION_ERROR,
        //                            "Problem with Session, please login again.",
        //                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        //}


        [WebMethod]
        public Response<List<PinReissueWSResult>> PINReissueSearch(int? issuerId, int? branchId, int? userRoleId, int? pinReissueStatusesId, int? pin_reissue_type, long? operatorUserId, bool operatorInProgress, long? authoriseUserId, byte[] index, DateTime? dateFrom, DateTime? dateTo, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.PINReissueSearch(issuerId, branchId, userRoleId, pinReissueStatusesId, pin_reissue_type, operatorUserId, operatorInProgress, authoriseUserId, index, dateFrom, dateTo, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<PinReissueWSResult>>(null,
                                    ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<PinReissueWSResult> GetPINReissue(long pinReissueId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetPINReissue(pinReissueId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<PinReissueWSResult>(null,
                                    ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        //[WebMethod]
        //public Response<PinReissueResult> RequestPINReissue(int issuerId, int branchId, int productId, byte[] index, string encryptedSessionKey)
        //{
        //    var session = _SessinManager.isValidSession(encryptedSessionKey);
        //    if (session != null)
        //    {
        //        return _issueCardController.RequestPINReissue(issuerId, branchId, productId, index, session.LanguageId, session.UserId, session.Workstation);
        //    }

        //    return new Response<PinReissueResult>(null,
        //                            ResponseType.SESSION_ERROR,
        //                            "Problem with Session, please login again.",
        //                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        //}

        [WebMethod]
        public Response<PinReissueWSResult> ApprovePINReissue(long pinReissueId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.ApprovePINReissue(pinReissueId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<PinReissueWSResult>(null,
                                    ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }








        #endregion

        #region LOAD CARD MANAGEMENT

        #endregion

        #region ISSUER MANAGEMENT

        #region EXPOSE ENUMS

        [WebMethod]
        public Veneka.Indigo.IssuerManagement.InstitutionStatus ExposeInstitutionStatus(Veneka.Indigo.IssuerManagement.InstitutionStatus input) //Expose the enum to the WSDL   
        {
            return input;
        }

        [WebMethod]
        public InterfaceTypes ExposeInterfaceTypes(InterfaceTypes input)
        {
            return input;
        }

        [WebMethod]
        public AuthenticationTypes ExposeAuthenticationTypes(AuthenticationTypes input)
        {
            return input;
        }


        [WebMethod]
        public FileEncryptionType ExposeFileEncryptionTypes(FileEncryptionType input)
        {
            return input;
        }

        //[WebMethod]
        //public IndigoLanguages ExposeIndigoLanguages(IndigoLanguages input)
        //{
        //    return input;
        //}

        [WebMethod]
        public ConnectionProtocol ExposeConnectionProtocol(ConnectionProtocol input)
        {
            return input;
        }

        #endregion

        #region Issuer

        //[WebMethod]
        //public Response<ldap_setting> GetIssuerLdapSettings(string encryptedSessionKey, int issuerId)
        //{
        //    var session = _SessinManager.isValidSession(encryptedSessionKey);
        //    if (session != null)
        //    {
        //        return _issuerManController.GetIssuerLdapSettings(issuerId);
        //    }// end if (_SessinManager.isValidSession(encryptedSessionKey))

        //    return new Response<ldap_setting>(null,
        //                                      ResponseType.SESSION_ERROR,
        //                                      "Problem with Session, please login again.",
        //                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        //}// end method Response<ldap_setting> GetIssuerLdapSettings(string encryptedUsername, int issuerId)

        [WebMethod]
        public Response<Veneka.Indigo.IssuerManagement.objects.IssuerResult> CreateIssuer(Veneka.Indigo.IssuerManagement.objects.IssuerResult issuer, string pin_notification_message, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.CreateIssuer(issuer, pin_notification_message, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<Veneka.Indigo.IssuerManagement.objects.IssuerResult>(null,
                                                                             ResponseType.SESSION_ERROR,
                                                                             "Problem with Session, please login again.",
                                                                             log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateIssuer(Veneka.Indigo.IssuerManagement.objects.IssuerResult issuer, string pin_notification_message, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.UpdateIssuer(issuer, pin_notification_message, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<country>> GetCountries(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetCountries(session.UserId, session.Workstation);
            }

            return new Response<List<country>>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }





        //[WebMethod]
        //public Response<List<issuer>> GetLdapIssuers(string encryptedSessionKey)
        //{
        //    var session = _SessinManager.isValidSession(encryptedSessionKey);

        //    if (session != null)
        //    {
        //        return _issuerManController.GetLdapIssuers(session.UserId, session.Workstation);
        //    }
        //    else
        //    {
        //        return new Response<List<issuer>>(null,
        //                                          ResponseType.SESSION_ERROR,
        //                                          "Problem with Session, please login again.",
        //                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        //    }
        //}





        [WebMethod]
        public Response<int> GetStockinBranch(int issuerid, int? branchId, int? productId, int? card_issue_method_id,
                                                              string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.GetStockinBranch(issuerid, branchId, productId, card_issue_method_id, session.UserId, session.Workstation);
            }

            return new Response<int>(0,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        #endregion

        #region LDAP Settings





        [WebMethod]
        public BaseResponse DeleteLdapSetting(int ldap_setting_id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.DeleteLdapSetting(ldap_setting_id, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #endregion

        #region Connection Parameters

        [WebMethod]
        public Response<string[]> GetFilePathReference(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return new Response<string[]>(Veneka.Indigo.Integration.Common.FilePath.GetFilePathReferences(),
                                                ResponseType.SUCCESSFUL,
                                                "", "");
            }

            return new Response<string[]>(null,
                                            ResponseType.SESSION_ERROR,
                                            "Problem with Session, please login again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<ConnectionParamsResult>> GetConnectionParameters(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetConnectionParameters(session.UserId, session.Workstation);
            }

            return new Response<List<ConnectionParamsResult>>(null,
                                                             ResponseType.SESSION_ERROR,
                                                             "Problem with Session, please login again.",
                                                             log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ConnectionParametersResult> GetConnectionParameter(int connParameterId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetConnectionParameter(connParameterId, session.UserId, session.Workstation);
            }

            return new Response<ConnectionParametersResult>(null,
                                                             ResponseType.SESSION_ERROR,
                                                             "Problem with Session, please login again.",
                                                             log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ConnectionParametersResult> CreateConnectionParam(ConnectionParametersResult connectionParam, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.CreateConnectionParam(connectionParam, session.UserId, session.Workstation);
            }

            return new Response<ConnectionParametersResult>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateConnectionParam(ConnectionParametersResult connectionParam, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.UpdateConnectionParam(connectionParam, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse DeleteConnectionParam(int connectionParamId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.DeleteConnectionParam(connectionParamId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<ProductInterfaceResult>> GetProductInterfaces(int connectionParamId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetProductInterfaces(connectionParamId, session.UserId, session.Workstation);
            }

            return new Response<List<ProductInterfaceResult>>(null,
                                                             ResponseType.SESSION_ERROR,
                                                             "Problem with Session, please login again.",
                                                             log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }

        [WebMethod]
        public Response<List<IssuerInterfaceResult>> GetIssuerConnectionParams(int connectionParamId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetIssuerConnectionParams(connectionParamId, session.UserId, session.Workstation);
            }

            return new Response<List<IssuerInterfaceResult>>(null,
                                                             ResponseType.SESSION_ERROR,
                                                             "Problem with Session, please login again.",
                                                             log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }

        //[WebMethod]
        //public Response<List<InterfaceWrapper>> GetIssuerInterfaces(string encryptedSessionKey, int issuerID)
        //{
        //    var session = _SessinManager.isValidSession(encryptedSessionKey);
        //    if (session != null)
        //    {
        //        return _issuerManController.GetIssuerInterfaces(issuerID);
        //    }// end if (_SessinManager.isValidSession(encryptedSessionKey))

        //    return new Response<List<InterfaceWrapper>>(null,
        //                                      ResponseType.SESSION_ERROR,
        //                                      "Problem with Session, please login again.",
        //                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        //}// end method Response<List<InterfaceWrapper>> GetIssuerInterfaces(string encryptedSessionKey, int issuerID)

        //[WebMethod]
        //public Response<InterfaceWrapper> GetIssuerInterfaceDetails(string encryptedSessionKey, int interfaceID)
        //{
        //    var session = _SessinManager.isValidSession(encryptedSessionKey);
        //    if (session != null)
        //    {
        //        return _issuerManController.GetInterfaceDetails(interfaceID);
        //    }// end if (_SessinManager.isValidSession(encryptedSessionKey))

        //    return new Response<InterfaceWrapper>(null,
        //                                      ResponseType.SESSION_ERROR,
        //                                      "Problem with Session, please login again.",
        //                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        //}// end method Response<InterfaceWrapper> GetIssuerInterfaceDetails(string encryptedSessionKey, int interfaceID)        

        #endregion

        #endregion

        #region BRANCH MANAGEMENT

        #region EXPOSE BRANCH ENUMS

        [WebMethod]
        public HybridRequestStatuses ExposeHybridRequestStatuses(HybridRequestStatuses input) //Expose the enum to the WSDL   
        {
            return input;
        }


        [WebMethod]
        public PrintJobStatuses ExposePrintJobStatuses(PrintJobStatuses input) //Expose the enum to the WSDL   
        {
            return input;
        }
        [WebMethod]

        public PrintBatchStatuses ExposePrintBatchStatuses(PrintBatchStatuses input) //Expose the enum to the WSDL   
        {
            return input;
        }



        [WebMethod]
        public BranchStatus ExposeBranchStatus(BranchStatus input) //Expose the enum to the WSDL   
        {
            return input;
        }

        [WebMethod]
        public BranchTypes ExposeBranchTypes(BranchTypes input) //Expose the enum to the WSDL   
        {
            return input;
        }

        #endregion

        [WebMethod]
        public Response<int> CreateBranch(branch createBranch, List<int> satellite_branches, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.CreateBranch(createBranch, satellite_branches, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<int>(0,
                                     ResponseType.SESSION_ERROR,
                                     "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateBranch(branch updateBranch, List<int> satellite_branches, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.UpdateBranch(updateBranch, satellite_branches, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<branch>> getBranchesForIssuer(int issuerId, int? cardCentre, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.getBranchesForIssuer(issuerId, cardCentre, session.LanguageId, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<List<branch>>(null,
                                                      ResponseType.SESSION_ERROR,
                                                      "Problem with Session, please login again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<BranchesResult>> GetBranchesForUser(int? issuerId, UserRole? userRole, int? cardCentreBranchYN, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.getAllBranchesForUser(issuerId, session.UserId, userRole, cardCentreBranchYN, session.LanguageId, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<List<BranchesResult>>(null,
                                                      ResponseType.SESSION_ERROR,
                                                      "Problem with Session, please login again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<BranchesResult>> GetBranchesForUserAdmin(int? issuerId, int? branchstatusid, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.getAllBranchesForUserAdmin(issuerId, branchstatusid, session.UserId, session.LanguageId, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<List<BranchesResult>>(null,
                                                      ResponseType.SESSION_ERROR,
                                                      "Problem with Session, please login again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<BranchesResult>> GetBranchesForUserroles(int? issuer_id, List<int> userRolesList, bool? branch_type_id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.getBranchesForUserroles(issuer_id, session.UserId, userRolesList, branch_type_id, session.LanguageId, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<List<BranchesResult>>(null,
                                                      ResponseType.SESSION_ERROR,
                                                      "Problem with Session, please login again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<BranchLoadCardCountResult>> GetBranchesLoadCardCount(int issuerId, UserRole userRole, LoadCardStatus? loadCardStatus, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetBranchesLoadCardCount(issuerId, session.UserId, userRole, loadCardStatus, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<List<BranchLoadCardCountResult>>(null,
                                                                 ResponseType.SESSION_ERROR,
                                                                 "Problem with Session, please login again.",
                                                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<int> GetBranchCardCount(int branchId, int productId, int? cardIssueMethidId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetBranchCardCount(branchId, productId, cardIssueMethidId, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<int>(0,
                                     ResponseType.SESSION_ERROR,
                                     "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        [WebMethod]
        public Response<int> GetDistBatchCount(long batchId, int branchId, int productId, int? cardIssueMethidId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetDistBatchCount(batchId, branchId, productId, cardIssueMethidId, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<int>(0,
                                     ResponseType.SESSION_ERROR,
                                     "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<int?> GetBranchLoadCardCount(int branchId, Veneka.Indigo.CardManagement.LoadCardStatus? loadCardStatus, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetBranchLoadCardCount(branchId, loadCardStatus, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<int?>(null,
                                      ResponseType.SESSION_ERROR,
                                      "Problem with Session, please login again.",
                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<branch> GetBranchById(int branchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.GetBranchById(branchId);
            }

            //Default response
            return new Response<branch>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CardHistoryReference>> GetCardReferenceHistory(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetCardReferenceHistory(cardId, session.LanguageId, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<List<CardHistoryReference>>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CardHistoryStatus>> GetCardStatusHistory(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetCardStatusHistory(cardId, session.LanguageId, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<List<CardHistoryStatus>>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<RequestStatusHistoryResult>> GetRequestStatusHistory(long requestId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetRequestStatusHistory(requestId, session.LanguageId, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<List<RequestStatusHistoryResult>>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<RequestReferenceHistoryResult>> GetRequestReferenceHistory(long requestId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.GetRequestReferenceHistory(requestId, session.LanguageId, session.UserId, session.Workstation);
            }

            //Default response
            return new Response<List<RequestReferenceHistoryResult>>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        #endregion

        #region USER MANAGEMENT

        #region EXPOSE USER MANAGEMENT ENUMS

        [WebMethod]
        public UserRole ExposeUserType(UserRole input) //Expose the enum to the WSDL   
        {
            return input;
        }

        [WebMethod]
        public AuthTypes ExposeAuthType(AuthTypes input) //Expose the enum to the WSDL   
        {
            return input;
        }

        //[WebMethod]
        //public UserStatus ExposeUserStatus(UserStatus input) //Expose the enum to the WSDL   
        //{
        //    return input;
        //}

        [WebMethod]
        public Gender ExposeGender(Gender input) //Expose the enum to the WSDL   
        {
            return input;
        }

        [WebMethod]
        public PrintFieldType ExposePrintFieldTypes(PrintFieldType input) //Expose the enum to the WSDL   
        {
            return input;
        }

        #endregion

        [WebMethod]
        public BaseResponse UpdateUserLanguange(int? userId, int languageId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.UpdateUserLanguage(session.UserId, languageId, session.Workstation);
            }
            else
            {
                return new BaseResponse(ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
            }
        }

        [WebMethod]
        public BaseResponse ResetUserPassword(long userId, string encryptedNewPassword, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.ResetUserPassword(userId, encryptedNewPassword, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdatePassword(long? userId, string encryptedOldPassword, string encryptedNewPassword, string encryptedWorkstation, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.UpdateUserPassword(userId == null ? session.UserId : userId.GetValueOrDefault(), encryptedOldPassword, encryptedNewPassword, session.LanguageId, session.UserId, encryptedWorkstation);
            }
            else if (_SessinManager.isValidSession(encryptedSessionKey, true) != null)
            {
                return _userManContoller.UpdateUserPassword((int)userId, encryptedOldPassword, encryptedNewPassword, 1, (int)userId, encryptedWorkstation);

            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<long?> GetAuthorisationPin(string encryptedUser, string encryptedPasscode, int branchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUserAuthPin(encryptedUser, encryptedPasscode, branchId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<long?>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<PinReissueWSResult> AuthorisationPinApprove(string encryptedUser, string encryptedPasscode, long pinReissueId, string comments, int branchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var managerAuth = _userManContoller.GetUserAuthPin(encryptedUser, encryptedPasscode, branchId, session.LanguageId, session.UserId, session.Workstation);

                if (managerAuth.ResponseType == ResponseType.SUCCESSFUL && managerAuth.Value.HasValue)
                {
                    // Make sure the audit user is the manager's user ID as he is the one approving the request.
                    return _issueCardController.ApprovePINReissue(pinReissueId, comments, session.LanguageId, managerAuth.Value.Value, session.Workstation);
                }
                else if (managerAuth.ResponseType == ResponseType.SUCCESSFUL && !managerAuth.Value.HasValue)
                    return new Response<PinReissueWSResult>(null, ResponseType.UNSUCCESSFUL,
                                    managerAuth.ResponseMessage,
                                    log.IsDebugEnabled || log.IsTraceEnabled ? managerAuth.ResponseException : "");
                else
                    return new Response<PinReissueWSResult>(null, managerAuth.ResponseType,
                                    managerAuth.ResponseMessage,
                                    log.IsDebugEnabled || log.IsTraceEnabled ? managerAuth.ResponseException : "");
            }

            return new Response<PinReissueWSResult>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<PinReissueWSResult> RejectPINReissue(long pinReissueId, string notes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issueCardController.RejectPINReissue(pinReissueId, notes, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<PinReissueWSResult>(null,
                                    ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<PinReissueWSResult> AuthorisationPinReject(string encryptedUser, string encryptedPasscode, long pinReissueId, string comments, int branchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var managerAuth = _userManContoller.GetUserAuthPin(encryptedUser, encryptedPasscode, branchId, session.LanguageId, session.UserId, session.Workstation);

                if (managerAuth.ResponseType == ResponseType.SUCCESSFUL && managerAuth.Value.HasValue)
                {
                    // Make sure the audit user is the manager's user ID as he is the one approving the request.
                    return _issueCardController.RejectPINReissue(pinReissueId, comments, session.LanguageId, managerAuth.Value.Value, session.Workstation);
                }
                else if (managerAuth.ResponseType == ResponseType.SUCCESSFUL && !managerAuth.Value.HasValue)
                    return new Response<PinReissueWSResult>(null, ResponseType.UNSUCCESSFUL,
                                    managerAuth.ResponseMessage,
                                    log.IsDebugEnabled || log.IsTraceEnabled ? managerAuth.ResponseException : "");
                else
                    return new Response<PinReissueWSResult>(null, managerAuth.ResponseType,
                                    managerAuth.ResponseMessage,
                                    log.IsDebugEnabled || log.IsTraceEnabled ? managerAuth.ResponseException : "");
            }

            return new Response<PinReissueWSResult>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateAuthorisationPin(long? userId, string encryptedPin, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.InsertUserAuthorisationPin(userId == null ? session.UserId : userId.GetValueOrDefault(), encryptedPin, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<int> CreateUserGroup(user_group userGroup, List<int> branchIdList, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.CreateUserGroup(userGroup, branchIdList, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<int>(0,
                                     ResponseType.SESSION_ERROR,
                                     "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateUserGroup(user_group userGroup, List<int> branchIdList, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.UpdateUserGroup(userGroup, branchIdList, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse DeleteUserGroup(int userGroupId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.DeleteUserGroup(userGroupId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<BranchIdResult>> GetBranchesForUserGroup(int userGroupId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetBranchesForUserGroup(userGroupId);
            }

            return new Response<List<BranchIdResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<UserGroupAdminResult>> GetUserGroupForUserAdmin(int? issuerId, int? userRole,
                                                                                long? userId, int? branchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUserGroupForUserAdmin(issuerId, userRole, userId, branchId);
            }

            return new Response<List<UserGroupAdminResult>>(null,
                                                            ResponseType.SESSION_ERROR,
                                                            "Problem with Session, please login again.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<UserGroupAdminResult>> GetUserGroupForPendingUserAdmin(int? issuerId, int? userRole,
                                                                              long? userId, int? branchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUserGroupForPendingUserAdmin(issuerId, userRole, userId, branchId);
            }

            return new Response<List<UserGroupAdminResult>>(null,
                                                            ResponseType.SESSION_ERROR,
                                                            "Problem with Session, please login again.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<GroupsRolesResult>> GetGroupRolesForUser(long userId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetGroupRolesForUser(userId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<GroupsRolesResult>>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<GroupsRolesResult>> GetGroupRolesForPendingUser(long userId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetGroupRolesForPendingUser(userId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<GroupsRolesResult>>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public BaseResponse ResetUserLoginStatus(long userId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.ResetUserLoginStatus(userId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public BaseResponse ApproveUser(long userId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.ApproveUser(userId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse RejectUserRequest(long userId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.RejectUserRequest(userId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<GetFileLoderLog_Result>> SearchFileLoadLog(int? fileLoadId, FileStatus? fileStatus, string fileName, int? issuerId,
                                                                          DateTime? dateFrom, DateTime? dateTo, int pageIndex, int rowsPerpage,
                                                                          string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _issuerManController.SearchFileLoadLog(fileLoadId, fileStatus, fileName, issuerId, dateFrom, dateTo, session.LanguageId, pageIndex, rowsPerpage,
                                                              session.UserId, session.Workstation);
            }

            return new Response<List<GetFileLoderLog_Result>>(null,
                                                              ResponseType.SESSION_ERROR,
                                                              "Problem with Session, please login again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");


        }

        [WebMethod]
        public Response<List<UserGroupResult>> GetUserGroupsByIssuer(int issuerID, UserRole? userRole, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUserGroups(issuerID, userRole, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<UserGroupResult>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<user_group> GetUserGroup(int userGroupId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUserGroup(userGroupId, session.UserId, session.Workstation);
            }

            return new Response<user_group>(null,
                                            ResponseType.SESSION_ERROR,
                                            "Problem with Session, please login again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<UserGroupResult>> GetUserGroupsByIssuerAndRole(int issuerID, UserRole userRole, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUserGroupsByRole(issuerID, userRole, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<UserGroupResult>>(null,
                                                       ResponseType.SESSION_ERROR,
                                                       "Problem with Session, please login again.",
                                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<decrypted_user> LdapLookup(string usernameLookup, string username, string userpassword, int authConfigId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);

            if (session != null)
            {
                return _userManContoller.LdapLookup(usernameLookup, username, userpassword, authConfigId, session.UserId, session.Workstation);
            }

            return new Response<decrypted_user>(null,
                                                ResponseType.SESSION_ERROR,
                                                "Problem with Session, please login again.",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<long?> CreateUser(user createUser, List<int> userGroupList, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.CreateUser(createUser, userGroupList, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<long?>(null,
                                       ResponseType.SESSION_ERROR,
                                       "Problem with Session, please login again.",
                                       log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<long?> UpdateUser(user updateUser, List<int> userGroupList, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.UpdateUser(updateUser, userGroupList, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<long?>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<UserRolesAndFlows> GetUserGroups(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUserRoles(session.UserId);
            }

            return new Response<UserRolesAndFlows>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<List<user_list_result>> GetUsersByBranch(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUsersByBranch(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<user_list_result>>(null,
                                            ResponseType.SESSION_ERROR,
                                            "Problem with Session, please login again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<user_list_result>> GetUsersPendingForApproval(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUsersPendingForApproval(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<user_list_result>>(null,
                                            ResponseType.SESSION_ERROR,
                                            "Problem with Session, please login again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<user_list_result>> GetUsersByBranchAdmin(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUsersByBranchAdmin(issuerId, branch_id, branchStatus, userStatus, userRole, username, firstName, lastName, session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<user_list_result>>(null,
                                            ResponseType.SESSION_ERROR,
                                            "Problem with Session, please login again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<user_list_result>> GetUnassignedUsers(int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUnassignedUsers(session.LanguageId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<user_list_result>>(null,
                                            ResponseType.SESSION_ERROR,
                                            "Problem with Session, please login again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<decrypted_user> GetUserByUsername(string encryptedUserName, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUserByUsername(encryptedUserName);
            }

            return new Response<decrypted_user>(null,
                                                ResponseType.SESSION_ERROR,
                                                "Problem with Session, please login again.",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<decrypted_user> GetUserByUserId(string encryptedUserId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUserByUserId(encryptedUserId);
            }

            return new Response<decrypted_user>(null,
                                                ResponseType.SESSION_ERROR,
                                                "Problem with Session, please login again.",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }
        [WebMethod]
        public Response<decrypted_user> GetPendingUserByUserId(string encryptedUserId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetPendingUserByUserId(encryptedUserId);
            }

            return new Response<decrypted_user>(null,
                                                ResponseType.SESSION_ERROR,
                                                "Problem with Session, please login again.",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }
        [WebMethod]
        public Response<List<user_list_result>> GetUserList(string username, string firstname, string lastname, string branchid, string userrole, int issuerid, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUserList(username, firstname, lastname, branchid, userrole, issuerid, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<user_list_result>>(null,
                                            ResponseType.SESSION_ERROR,
                                            "Problem with Session, please login again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> GetLangUserRoles(int? enterprise, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetLangUserRoles(session.LanguageId, enterprise, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        [WebMethod]
        public Response<int?> CreateUseradminSettings(useradminsettingslist item, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.CreateUseradminSettings(item, session.UserId, session.Workstation);
            }

            return new Response<int?>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public BaseResponse UpdateUseradminSettings(useradminsettingslist item, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.UpdateUseradminSettings(item, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<useradminsettingslist> GetUseradminSettings(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetUseradminSettings(session.UserId, session.Workstation);
            }

            return new Response<useradminsettingslist>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        #endregion

        #region Audit Control Region

        [WebMethod]
        public AuditActionType ExposeAuditActionType(AuditActionType auditAction)
        {
            return auditAction;
        }
        [WebMethod]
        public PrintSide ExposePrintSide(PrintSide printSide)
        {
            return printSide;
        }

        [WebMethod]
        public Response<List<GetAuditData_Result>> GetAudit(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
                                                            DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _auditServiceController.GetAuditData(auditAction, userRoleId, username, dateFrom, dateTo, issuerId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<GetAuditData_Result>>(null,
                                                           ResponseType.SESSION_ERROR,
                                                           "Problem with Session, please login again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }

        #endregion

        #region System

        [WebMethod]
        public Response<List<CurrencyResult>> GetCurrencyList(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetCurrencyList(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<CurrencyResult>>(null,
                                      ResponseType.SESSION_ERROR,
                                      "Problem with Session, please login again.",
                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<DistBatchFlows>> GetDistBatchFlowList(int card_issue_method_id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetDistBatchFlowList(card_issue_method_id, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<DistBatchFlows>>(null,
                                      ResponseType.SESSION_ERROR,
                                      "Problem with Session, please login again.",
                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<long> InsertProduct(ProductResult productResult, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.InsertProduct(productResult, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<long>(0,
                                      ResponseType.SESSION_ERROR,
                                      "Problem with Session, please login again.",
                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateProduct(ProductResult prodresult, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.UpdateProduct(prodresult, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

      

        [WebMethod]
        public BaseResponse DeleteProduct(int Productid, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.DeleteProduct(Productid, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse ActivateProduct(int Productid, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.ActivateProduct(Productid, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "InGetProductsListvalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<ProductlistResult>> GetProductsList(int issuerid, int? cardIssueMethodId, bool? deletedYN, int pageIndex, int RowsPerpage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetProductsList(issuerid, cardIssueMethodId, deletedYN, pageIndex, RowsPerpage);
            }

            return new Response<List<ProductlistResult>>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<List<ProductValidated>> GetProductsListValidated(int issuerid, int? cardIssueMethodId, int pageIndex, int RowsPerpage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetProductsListValidated(issuerid, cardIssueMethodId, pageIndex, RowsPerpage, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<ProductValidated>>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<List<BillingReportResult>> GetBillingReport(int? IssuerId, string month, string year, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _distBatchController.GetBillingReport(IssuerId, month, year, session.UserId, session.Workstation);
            }

            return new Response<List<BillingReportResult>>(null,
                                      ResponseType.SESSION_ERROR,
                                      "Problem with Session, please login again.",
                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<Issuer_product_font>> GetFontFamilyList(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetFontFamilyList();
            }

            return new Response<List<Issuer_product_font>>(null,
                                                           ResponseType.SESSION_ERROR,
                                                           "Problem with Session, please login again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<List<ServiceRequestCode>> GetServiceRequestCode1(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetServiceRequestCode1();
            }

            return new Response<List<ServiceRequestCode>>(null,
                                                           ResponseType.SESSION_ERROR,
                                                           "Problem with Session, please login again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<List<ServiceRequestCode1>> GetServiceRequestCode2(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetServiceRequestCode2();
            }

            return new Response<List<ServiceRequestCode1>>(null,
                                                           ResponseType.SESSION_ERROR,
                                                           "Problem with Session, please login again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<List<ServiceRequestCode2>> GetServiceRequestCode3(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetServiceRequestCode3();
            }

            return new Response<List<ServiceRequestCode2>>(null,
                                                           ResponseType.SESSION_ERROR,
                                                           "Problem with Session, please login again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<ProductResult> GetProduct(int productId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetProduct(productId, session.UserId, session.Workstation);
            }

            return new Response<ProductResult>(null,
                                                ResponseType.SESSION_ERROR,
                                                "Problem with Session, please login again.",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<List<product_currency1>> GetCurreniesbyProduct(int Productid, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetCurreniesbyProduct(Productid);
            }

            return new Response<List<product_currency1>>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<long?> InsertFont(FontResult fontresult, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.InsertFont(fontresult,
                                                            session.UserId, session.Workstation);
            }

            return new Response<long?>(null,
                                      ResponseType.SESSION_ERROR,
                                      "Problem with Session, please login again.",
                                      log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateFont(FontResult fontresult, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.UpdateFont(fontresult,
                                                            session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<FontResult> GetFont(int fontid, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetFont(fontid);
            }

            return new Response<FontResult>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<List<FontResult>> GetFontlist(int pageIndex, int RowsPerpage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetFontListBypage(pageIndex, RowsPerpage);
            }

            return new Response<List<FontResult>>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public BaseResponse DeleteFont(int fontid, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.DeleteFont(fontid,
                                                            session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse DeleteSubProduct(int Productid, int subProductid, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.DeleteSubProduct(Productid, subProductid,
                                                            session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<SubProduct_Result>> GetSubProductList(int issuer_id, int? product_id, int? cardIssueMethidId, Boolean? deletedYN, int pageIndex, int RowsPerpage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetSubProductList(issuer_id, product_id, cardIssueMethidId, deletedYN, pageIndex, RowsPerpage);
            }

            return new Response<List<SubProduct_Result>>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }

        [WebMethod]
        public Response<SubProduct_Result> GetSubProduct(int? product_id, int sub_productid, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetSubProduct(product_id, sub_productid);
            }

            return new Response<SubProduct_Result>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }


        [WebMethod]
        public Response<List<ProductFeeAccountingResult>> GetFeeAccountingList(int? issuerId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetFeeAccountingList(issuerId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<ProductFeeAccountingResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ProductFeeAccountingResult> GetFeeAccounting(int feeAccountingId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetFeeAccounting(feeAccountingId, session.UserId, session.Workstation);
            }

            return new Response<ProductFeeAccountingResult>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ProductFeeAccountingResult> CreateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.CreateFeeAccounting(feeAccountingDetails, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<ProductFeeAccountingResult>(null, ResponseType.SESSION_ERROR,
                                                    "Problem with Session, please login again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ProductFeeAccountingResult> UpdateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.UpdateFeeAccounting(feeAccountingDetails, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<ProductFeeAccountingResult>(null, ResponseType.SESSION_ERROR,
                                                    "Problem with Session, please login again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse DeleteFeeAccounting(int deeAccountingId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.DeleteFeeAccounting(deeAccountingId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                                    "Problem with Session, please login again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<FeeSchemeResult>> GetFeeSchemes(int? issuerId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetFeeSchemes(issuerId, pageIndex, rowsPerPage, session.UserId, session.Workstation);
            }

            return new Response<List<FeeSchemeResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<FeeSchemeDetails> GetFeeSchemeDetails(int feeSchemeId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetFeeSchemeDetails(feeSchemeId, session.UserId, session.Workstation);
            }

            return new Response<FeeSchemeDetails>(null,
                                                   ResponseType.SESSION_ERROR,
                                                   "Problem with Session, please login again.",
                                                   log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<FeeDetailResult>> GetFeeDetails(int feeDetailId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetFeeDetails(feeDetailId, session.UserId, session.Workstation);
            }

            return new Response<List<FeeDetailResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<FeeChargeResult>> GetFeeCharges(int feeDetailId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetFeeCharges(feeDetailId, session.UserId, session.Workstation);
            }

            return new Response<List<FeeChargeResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<ProductFeeDetailsResult>> GetFeeDetailByProduct(int productId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetFeeDetailByProduct(productId, session.UserId, session.Workstation);
            }

            return new Response<List<ProductFeeDetailsResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ProductChargeResult> GetCurrentFees(int feeDetailId, int currencyId, int CardIssueReasonId, string CBSAccountType, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetCurrentFees(feeDetailId, currencyId, CardIssueReasonId, CBSAccountType, session.UserId, session.Workstation);
            }

            return new Response<ProductChargeResult>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateFeeCharges(int feeDetailId, List<FeeChargeResult> fees, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.UpdateFeeCharges(feeDetailId, fees, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                     "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<FeeSchemeDetails> InsertFeeScheme(FeeSchemeDetails feeSchemeDetails, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.InsertFeeScheme(feeSchemeDetails, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<FeeSchemeDetails>(null, ResponseType.SESSION_ERROR,
                                                    "Problem with Session, please login again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<FeeSchemeDetails> UpdateFeeScheme(FeeSchemeDetails feeSchemeDetails, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.UpdateFeeScheme(feeSchemeDetails, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<FeeSchemeDetails>(null, ResponseType.SESSION_ERROR,
                                                    "Problem with Session, please login again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse DeleteFeeScheme(int feeSchemeId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.DeleteFeeScheme(feeSchemeId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<ProductPrintFieldResult>> GetProductPrintFields(int? productId, int? cardId, int? requestId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.GetProductPrintFields(productId, cardId, requestId, session.UserId, session.Workstation);
            }

            return new Response<List<ProductPrintFieldResult>>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> CreateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.CreateProductPrintFields(productPrintFields, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                                    "Problem with Session, please login again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> UpdateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _productcontroller.UpdateProductPrintFields(productPrintFields, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                                    "Problem with Session, please login again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #endregion

        #region Licensing

        [WebMethod]
        public Response<string> GetMachineId(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _licenseController.GetMachineId();

            }

            return new Response<string>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<IndigoComponentLicense> LoadIssuerLicense(byte[] licenseFileBytes, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _licenseController.LoadIssuerLicense(licenseFileBytes, session.UserId, session.Workstation);
            }

            //return _licenseController.LoadIssuerLicense(issuerId, licenseFileLocation, 2, "UNIT TEST");

            return new Response<IndigoComponentLicense>(null,
                                                        ResponseType.SESSION_ERROR,
                                                        "Problem with Session, please login again.",
                                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<IndigoComponentLicense>> GetLicenseListIssuers(bool? licensedIssuers, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _licenseController.GetLicenseListIssuers(licensedIssuers, session.UserId, session.Workstation);
            }

            return new Response<List<IndigoComponentLicense>>(null,
                                                              ResponseType.SESSION_ERROR,
                                                              "Problem with Session, please login again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        #endregion

        #region Language Lookups

        [WebMethod]
        public Response<List<language>> GetLanguages(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.GetLanguages(session.UserId, session.Workstation);
            }

            return new Response<List<language>>(null,
                                                ResponseType.SESSION_ERROR,
                                                "Problem with Session, please login again.",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupUserStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupUserStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupUserRoles(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupUserRoles(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupAuditActions(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupAuditActions(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupBranchCardStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupBranchCardStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupBranchStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupBranchStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupCardIssueReasons(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupCardIssueReasons(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupCustomerAccountTypes(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupCustomerAccountTypes(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupDistBatchStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupDistBatchStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupDistCardStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupDistCardStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupFileStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupFileStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupInterfaceTypes(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupInterfaceTypes(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupIssuerStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupIssuerStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupLoadBatchStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupLoadBatchStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<LangLookup>> LangLookupThreedBatchStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupThreedBatchStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupLoadCardStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupLoadCardStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupCustomerResidency(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupCustomerResidency(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupCustomerTitle(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupCustomerTitle(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupCustomerType(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupCustomerType(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookupChar>> LangLookupGenderType(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupGenderType(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookupChar>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupConnectionParameterType(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupConnectionParameterType(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupCardIssueMethod(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupCardIssueMethod(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupPinBatchStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupPinBatchStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupPinReissueStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupPinReissueStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupExportBatchStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupExportBatchStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupRemoteUpdateStatuses(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupRemoteUpdateStatuses(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupProductLoadTypes(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupProductLoadTypes(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        [WebMethod]
        public Response<List<LangLookup>> LangLookupFileEncryptionTypes(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupFileEncryptionTypes(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupBranchTypes(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupBranchTypes(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<LangLookup>> LangLookupPrintBatchStatues(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupPrintBatchStatues(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupHybridRequestStatues(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _languageController.LangLookupHybridRequestStatues(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                                  ResponseType.SESSION_ERROR,
                                                  "Problem with Session, please login again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        #endregion

        #region Reporting

        [WebMethod]
        public Response<List<BranchOrderReportResult>> GetBranchOrderReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _reportController.GetBranchOrderReport(issuerId, branchId, fromDate, toDate, session.LanguageId);
            }

            return new Response<List<BranchOrderReportResult>>(null,
                                                              ResponseType.SESSION_ERROR,
                                                              "Problem with Session, please login again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CardProductionReportResult>> GetCardProductionReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _reportController.GetCardProductionReport(issuerId, branchId, fromDate, toDate, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<CardProductionReportResult>>(null,
                                                              ResponseType.SESSION_ERROR,
                                                              "Problem with Session, please login again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CardDispatchReportResult>> GetCardDispatchReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _reportController.GetCardDispatchReport(issuerId, branchId, fromDate, toDate, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<CardDispatchReportResult>>(null,
                                                              ResponseType.SESSION_ERROR,
                                                              "Problem with Session, please login again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<PinMailerReportResult>> GetPinMailerReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _reportController.GetPinMailerReport(issuerId, branchId, fromDate, toDate, session.LanguageId);
            }

            return new Response<List<PinMailerReportResult>>(null,
                                                              ResponseType.SESSION_ERROR,
                                                              "Problem with Session, please login again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<PinMailerReprintReportResult>> GetPinMailerReprintReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _reportController.GetPinMailerReprintReport(issuerId, branchId, fromDate, toDate, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<PinMailerReprintReportResult>>(null,
                                                              ResponseType.SESSION_ERROR,
                                                              "Problem with Session, please login again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CardExpiryReportResult>> GetCardExpiryReport(int? issuerId, int? branchId, DateTime fromDate, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _reportController.GetCardExpiryReport(issuerId, branchId, fromDate, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<CardExpiryReportResult>>(null,
                                                              ResponseType.SESSION_ERROR,
                                                              "Problem with Session, please login again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GenereateAuditPdfReport(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
                                                            DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _auditServiceController.GenereateAuditPdfReport(auditAction, userRoleId, username, dateFrom, dateTo, issuerId, pageIndex, rowsPerPage, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<byte[]>(null,
                                                           ResponseType.SESSION_ERROR,
                                                           "Problem with Session, please login again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }
        [WebMethod]
        public Response<List<string[]>> GetAuditCSVReport(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
            DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _auditServiceController.GetAuditCSVReport(auditAction, userRoleId, username, dateFrom, dateTo, issuerId, pageIndex, rowsPerPage, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<string[]>>(null,
                                                           ResponseType.SESSION_ERROR,
                                                           "Problem with Session, please login again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }

        [WebMethod]
        public Response<List<auditreport_usergroup_Result>> GetUsersByRoles_AuditReport(int? issuer_id, int? user_group_id, int? user_role_id, int? user_id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _reportController.GetUsersByRoles_AuditReport(issuer_id, user_group_id, user_role_id, user_id, session.UserId, session.Workstation);
            }

            return new Response<List<auditreport_usergroup_Result>>(null,
                                                           ResponseType.SESSION_ERROR,
                                                           "Problem with Session, please login again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }

        [WebMethod]
        public Response<List<auditreport_usergroup_Result>> GetUserGroup_AuditReport(int? issuer_id, int? user_group_id, int? user_role_id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _reportController.GetUserGroup_AuditReport(issuer_id, user_group_id, user_role_id, session.UserId, session.Workstation);
            }

            return new Response<List<auditreport_usergroup_Result>>(null,
                                                           ResponseType.SESSION_ERROR,
                                                           "Problem with Session, please login again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }

        [WebMethod]
        public Response<List<issuedcardsreport_Result>> Getissuecardreport(int? user, DateTime dateFrom, DateTime dateTo, string status, int issuerID, int? branchid,
                                            string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);
            if (session != null)
            {


                return _reportController.Getissuecardreport(user, issuerID, session.LanguageId, dateFrom, dateTo, status, branchid, session.UserId, session.Workstation);
            }
            return new Response<List<issuedcardsreport_Result>>(null,
                                                          ResponseType.SESSION_ERROR,
                                                          "Problem with Session, please login again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<issuecardsummaryreport_Result>> Getissuecardsummaryreport(int? issuerID, int? branchid, DateTime dateFrom, DateTime dateTo,
                                            string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);
            if (session != null)
            {


                return _reportController.Getissuecardsummaryreport(issuerID, branchid, session.LanguageId, dateFrom, dateTo);
            }
            return new Response<List<issuecardsummaryreport_Result>>(null,
                                                          ResponseType.SESSION_ERROR,
                                                          "Problem with Session, please login again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<Spoilcardsummaryreport_Result>> GetSpoilCardsummaryreport(int? issuerID, int? branchid, DateTime dateFrom, DateTime dateTo,
                                            string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);
            if (session != null)
            {


                return _reportController.GetSpoilCardsummaryreport(issuerID, branchid, session.LanguageId, dateFrom, dateTo);
            }
            return new Response<List<Spoilcardsummaryreport_Result>>(null,
                                                          ResponseType.SESSION_ERROR,
                                                          "Problem with Session, please login again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<spolicardsreport_Result>> GetSpoilCardreport(int? issuerID, int? branchid, int? userid, DateTime dateFrom, DateTime dateTo,
                                            string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);
            if (session != null)
            {
                return _reportController.GetSpoilCardreport(issuerID, session.LanguageId, userid, branchid, dateFrom, dateTo, session.UserId, session.Workstation);
            }
            return new Response<List<spolicardsreport_Result>>(null,
                                                          ResponseType.SESSION_ERROR,
                                                          "Problem with Session, please login again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<invetorysummaryreport_Result>> GetInventorySummaryReport(int issuerID, int? branchid, string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);
            if (session != null)
            {
                return _reportController.GetInventorySummaryReport(issuerID, branchid, session.LanguageId, session.UserId, session.Workstation);
            }
            return new Response<List<invetorysummaryreport_Result>>(null,
                                                          ResponseType.SESSION_ERROR,
                                                          "Problem with Session, please login again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<invetorysummaryreport_Result>> GetCardCenterInventorySummaryReport(int issuerID, int? branchid, string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);
            if (session != null)
            {
                return _reportController.GetCardCenterInventorySummaryReport(issuerID, branchid, session.LanguageId, session.UserId, session.Workstation);
            }
            return new Response<List<invetorysummaryreport_Result>>(null,
                                                          ResponseType.SESSION_ERROR,
                                                          "Problem with Session, please login again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<branchcardstock_report_Result>> GetBranchCardStockReport(int issuerID, int? branchid, string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);
            if (session != null)
            {
                return _reportController.GetBranchCardStockReport(issuerID, branchid, session.LanguageId, session.UserId, session.Workstation);
            }
            return new Response<List<branchcardstock_report_Result>>(null,
                                                          ResponseType.SESSION_ERROR,
                                                          "Problem with Session, please login again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<branchcardstock_report_Result>> GetCenterCardStockReport(int issuerID, int? branchid, string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);
            if (session != null)
            {
                return _reportController.GetCenterCardStockReport(issuerID, branchid, session.LanguageId, session.UserId, session.Workstation);
            }
            return new Response<List<branchcardstock_report_Result>>(null,
                                                          ResponseType.SESSION_ERROR,
                                                          "Problem with Session, please login again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<burnrate_report_Result>> GetBurnRateReport(int issuerID, int? branchid, string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);
            if (session != null)
            {
                return _reportController.GetBurnRateReport(issuerID, branchid, session.LanguageId, session.UserId, session.Workstation);
            }
            return new Response<List<burnrate_report_Result>>(null,
                                                          ResponseType.SESSION_ERROR,
                                                          "Problem with Session, please login again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<feerevenue_report_Result>> GetFeeRevenueReport(int? issuerID, int? branchid, DateTime dateFrom, DateTime dateTo, string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);
            if (session != null)
            {
                return _reportController.GetFeeRevenueReport(issuerID, branchid, session.LanguageId, dateFrom, dateTo, session.UserId, session.Workstation);
            }
            return new Response<List<feerevenue_report_Result>>(null,
                                                          ResponseType.SESSION_ERROR,
                                                          "Problem with Session, please login again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<auditreport_usergroup_Result>> GetBranchesperusergroup_AuditReport(int? issuer_id, int? user_group_id, int? branch_id, int? role_id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _reportController.GetBranchesperusergroup_AuditReport(issuer_id, user_group_id, branch_id, role_id, session.UserId, session.Workstation);
            }

            return new Response<List<auditreport_usergroup_Result>>(null,
                                                           ResponseType.SESSION_ERROR,
                                                           "Problem with Session, please login again.",
                                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }

        [WebMethod]
        public Response<List<PINReissueReportResult>> GetPinReissueReport(int? user, DateTime dateFrom, DateTime dateTo, string status, int issuerID, int? branchid,
                                            string sessionKey)
        {
            var session = _SessinManager.isValidSession(sessionKey);
            if (session != null)
            {


                return _reportController.GetPinReissueReport(user, issuerID, session.LanguageId, dateFrom, dateTo, status, branchid, session.UserId, session.Workstation);
            }
            return new Response<List<PINReissueReportResult>>(null,
                                                          ResponseType.SESSION_ERROR,
                                                          "Problem with Session, please login again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #endregion

        #region Global Region

        [WebMethod]
        public string GetApplicationVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }

        #endregion

        #region "External Systems"

        [WebMethod]
        public Response<int?> CreateExternalSystems(ExternalSystemFieldResult externalsystem, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _systemconfigController.CreateExternalSystems(externalsystem, session.UserId, session.Workstation, session.LanguageId);
            }

            return new Response<int?>(null,
                                    ResponseType.SESSION_ERROR, "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateExternalSystem(ExternalSystemFieldResult externalsystem, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _systemconfigController.UpdateExternalSystem(externalsystem, session.UserId, session.Workstation, session.LanguageId);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR, "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<ExternalSystemFieldResult> GetExternalSystems(int? externalsystemid, int rowindex, int rowsperpage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _systemconfigController.GetExternalSystems(externalsystemid, rowindex, rowsperpage, session.UserId, session.Workstation, session.LanguageId);
            }

            return new Response<ExternalSystemFieldResult>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse DeleteExternalSystems(int? externalsystemid, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _systemconfigController.DeleteExternalSystems(externalsystemid, session.UserId, session.Workstation, session.LanguageId);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR, "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        [WebMethod]
        public Response<int?> CreateExternalSystemFields(ExternalSystemFieldsResult externalsystemfield, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _systemconfigController.CreateExternalSystemFields(externalsystemfield, session.UserId, session.Workstation, session.LanguageId);
            }

            return new Response<int?>(null,
                                    ResponseType.SESSION_ERROR, "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateExternalSystemFields(ExternalSystemFieldsResult externalsystemfield, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _systemconfigController.UpdateExternalSystemFields(externalsystemfield, session.UserId, session.Workstation, session.LanguageId);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR, "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<ExternalSystemFieldsResult>> GetExternalSystemsFields(int? externalsystemfieldid, int rowindex, int rowsperpage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _systemconfigController.GetExternalSystemsFields(externalsystemfieldid, rowindex, rowsperpage, session.UserId, session.Workstation);
            }

            return new Response<List<ExternalSystemFieldsResult>>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse DeleteExternalSystemField(int? externalsystemfieldid, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _systemconfigController.DeleteExternalSystemField(externalsystemfieldid, session.UserId, session.Workstation, session.LanguageId);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR, "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LangLookup>> LangLookupExternalSystems(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _systemconfigController.LangLookupExternalSystems(session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<LangLookup>>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        #endregion
        
        #region "Notification Services"
        [WebMethod]
        public Response<long> InsertNotificationforBatch(NotificationMessages notifications, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _notificationController.InsertNotificationforBatch(notifications, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<long>(0,
                                           ResponseType.SESSION_ERROR,
                                           "Problem with Session, please login again.",
                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }
        [WebMethod]
        public Response<long> UpdateNotificationforBatch(NotificationMessages notifications, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _notificationController.UpdateNotificationforBatch(notifications, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<long>(0,
                                           ResponseType.SESSION_ERROR,
                                           "Problem with Session, please login again.",
                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }
        [WebMethod]
        public Response<List<notification_batchResult>> GetNotificationBatch(NotificationMessages messages, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _notificationController.GetNotificationBatch(messages, session.UserId, session.Workstation);

            }

            return new Response<List<notification_batchResult>>(null,
                                           ResponseType.SESSION_ERROR,
                                           "Problem with Session, please login again.",
                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<notification_batch_ListResult>> ListNotificationBatches(int issuerid, int pageIndex, int rowsperpage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _notificationController.ListNotificationBatches(issuerid, pageIndex, rowsperpage, session.UserId, session.Workstation);

            }

            return new Response<List<notification_batch_ListResult>>(null,
                                           ResponseType.SESSION_ERROR,
                                           "Problem with Session, please login again.",
                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<bool> DeleteNotificationBatch(NotificationMessages messages, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _notificationController.DeleteNotificationBatch(messages, session.UserId, session.Workstation);

            }

            return new Response<bool>(false,
                                           ResponseType.SESSION_ERROR,
                                           "Problem with Session, please login again.",
                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }


        [WebMethod]
        public Response<long> InsertNotificationforBranch(NotificationMessages notifications, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _notificationController.InsertNotificationforBranch(notifications, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<long>(0,
                                           ResponseType.SESSION_ERROR,
                                           "Problem with Session, please login again.",
                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }
        [WebMethod]
        public Response<long> UpdateNotificationforBranch(NotificationMessages notifications, string encryptedSessionKey)
        {

            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _notificationController.UpdateNotificationforBranch(notifications, session.LanguageId, session.UserId, session.Workstation);

            }

            return new Response<long>(0,
                                           ResponseType.SESSION_ERROR,
                                           "Problem with Session, please login again.",
                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }
        [WebMethod]
        public Response<List<notification_branchResult>> GetNotificationBranch(NotificationMessages messages, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _notificationController.GetNotificationBranch(messages, session.UserId, session.Workstation);

            }

            return new Response<List<notification_branchResult>>(null,
                                           ResponseType.SESSION_ERROR,
                                           "Problem with Session, please login again.",
                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<notification_branch_ListResult>> ListNotificationBraches(int issuerid, int pageIndex, int rowsperpage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _notificationController.ListNotificationBraches(issuerid, pageIndex, rowsperpage, session.UserId, session.Workstation);

            }

            return new Response<List<notification_branch_ListResult>>(null,
                                           ResponseType.SESSION_ERROR,
                                           "Problem with Session, please login again.",
                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<bool> DeleteNotificationBranch(NotificationMessages messages, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {

                return _notificationController.DeleteNotificationBranch(messages, session.UserId, session.Workstation);

            }

            return new Response<bool>(false,
                                           ResponseType.SESSION_ERROR,
                                           "Problem with Session, please login again.",
                                           log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        #endregion

        #region Authentication Configuration
        [WebMethod]
        public Response<int?> InsertAuthenticationConfiguration(AuthConfigResult authConfig, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.InsertAuthenticationConfiguration(authConfig, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<int?>(null,
                                    ResponseType.SESSION_ERROR, "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse UpdateAuthenticationConfiguration(AuthConfigResult authConfig, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.UpdateAuthenticationConfiguration(authConfig, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR, "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<List<auth_configuration_result>> GetAuthConfigurationList(int? authConfigurationId, int? pageIndex, int? rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetAuthConfigurationList(authConfigurationId, pageIndex, rowsPerPage);
            }

            return new Response<List<auth_configuration_result>>(null,
                                               ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<AuthConfigResult> GetAuthConfiguration(int? authConfigurationId, int? pageIndex, int? rowsPerPage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.GetAuthConfiguration(authConfigurationId);
            }

            return new Response<AuthConfigResult>(null, ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> SendChallenge(int authconfigurationId, string username, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey, true);
            if (session != null)
            {
                return _userManContoller.SendChallenge(authconfigurationId, username, session.Workstation);
            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> VerifyChallenge(int authconfigurationId, String Username, string token, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey, true);
            if (session != null)
            {
                return _userManContoller.VerifyChallenge(authconfigurationId, token, Username, session.Workstation);
            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                               "Problem with Session, please login again.",
                                               log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse DeleteAuthConfiguration(int authConfigurationId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _userManContoller.DeleteAuthConfiguration(authConfigurationId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR, "Problem with Session, please login again.",
                                     log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        #endregion

        #region Print Job 

        [WebMethod]
        public Response<PrintJobStatus> GetPrintJobStatus(string printJobId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _printJobController.GetPrintJobStatus(printJobId, session.LanguageId, session.UserId, session.Workstation);
            }
            else
            {
                return new Response<PrintJobStatus>(null, ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
            }
        }

        [WebMethod]
        public Response<string> InsertPrintJob(string serialNo, long customerId, long cardId, int printJobStatusesId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _printJobController.InsertPrintJob(serialNo, customerId, cardId, printJobStatusesId, session.UserId, session.Workstation);
            }
            else
            {
                return new Response<string>(null, ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
            }
        }
        [WebMethod]
        public BaseResponse UpdatePrintCount(string serialNo, int totalPrints, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _printJobController.UpdatePrintCount(serialNo, totalPrints, session.UserId, session.Workstation);
            }
            else
            {
                return new BaseResponse(ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
            }
        }
        [WebMethod]
        public BaseResponse UpdatePrintJobStatus(string printerJobId, int printJobStatusesId, string comments, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _printJobController.UpdatePrintJobStatus(printerJobId, printJobStatusesId, comments, session.UserId, session.Workstation);
            }
            else
            {
                return new BaseResponse(ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
            }
        }


        [WebMethod]
        public BaseResponse PrintingCompleted(string encryptedCardNumber, long cardId, string printJobId, int printJobStatusesId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _printJobController.PrintingCompleted(encryptedCardNumber, cardId, printJobId, printJobStatusesId, session.UserId, session.Workstation);
            }
            else
            {
                return new BaseResponse(ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
            }
        }
        [WebMethod]
        public BaseResponse RegisterPrinter(Printer printer, string printJobId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _printJobController.RegisterPrinter(printer, printJobId, session.UserId, session.Workstation);
            }
            else
            {
                return new BaseResponse(ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
            }
        }
        #endregion

        #region FundsLoad

        [WebMethod]
        public BaseResponse FundsLoadCreate(FundsLoadModel fundsLoad, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.Create(fundsLoad, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse FundsLoadReviewAccept(long fundsLoadId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.ReviewAccept(fundsLoadId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse FundsLoadReviewReject(long fundsLoadId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.ReviewReject(fundsLoadId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse FundsLoadApproveAccept(long fundsLoadId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.ApprovalAccept(fundsLoadId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse FundsLoadApproveReject(long fundsLoadId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.ApprovalReject(fundsLoadId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse FundsLoadLoad(long fundsLoadId, string encryptedSessionKey, int cardIssueReasonId)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.Load(fundsLoadId, session.LanguageId, session.UserId, session.Workstation, cardIssueReasonId);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public BaseResponse FundsLoadSendSMS(long fundsLoadId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.SendSMS(fundsLoadId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new BaseResponse(ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<FundsLoadListModel> FundsLoadRetrieve(long fundsLoadId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.Retrieve(fundsLoadId, true, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<FundsLoadListModel>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<FundsLoadListModel>> FundsLoadRetrieveList(FundsLoadStatusType statusType, int issuerId, int branchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.List(statusType, issuerId, branchId, true, session.UserId, session.Workstation);
            }

            return new Response<List<FundsLoadListModel>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<FundsLoadListModel>> ApproveBulk(FundsLoadStatusType statusType, List<long> fundsLoads, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.ApproveBulk(statusType, fundsLoads, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<FundsLoadListModel>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<FundsLoadListModel>> RejectBulk(FundsLoadStatusType statusType, List<long> fundsLoads, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.RejectBulk(statusType, fundsLoads, true, session.UserId, session.Workstation);
            }

            return new Response<List<FundsLoadListModel>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<FundsLoadListModel> Approve(FundsLoadStatusType statusType, long fundsLoadId, string encryptedSessionKey, int cardIssueReasonId)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.Approve(statusType, fundsLoadId, true, session.UserId, session.Workstation, session.LanguageId, cardIssueReasonId);
            }

            return new Response<FundsLoadListModel>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<FundsLoadListModel> Reject(FundsLoadStatusType statusType, long fundsLoadId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.Reject(statusType, fundsLoadId, true, session.UserId, session.Workstation);
            }

            return new Response<FundsLoadListModel>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<AccountDetails> FundsLoadCheckAccount(int issuerId, int productId, int branchId, int cardIssueReasonId, string accountNumber, decimal amount, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.CheckBankAccountDetails(issuerId, productId, branchId, accountNumber, amount, session.UserId, session.Workstation, session.LanguageId, cardIssueReasonId);
            }

            return new Response<AccountDetails>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<PrepaidAccountDetail> FundsLoadGetPrepaidAccount(int productId, string cardNumber, int mbr, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.CheckPrepaidAccountDetails(productId, cardNumber, mbr);
            }

            return new Response<PrepaidAccountDetail>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<ProductValidated>> FundsLoadGetProductsList(int issuerid, int? cardIssueMethodId, int pageIndex, int RowsPerpage, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _fundsLoadController.GetProductsWithFundsLoad(issuerid, cardIssueMethodId, pageIndex, RowsPerpage, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<ProductValidated>>(null,
                                                         ResponseType.SESSION_ERROR,
                                                         "Problem with Session, please login again.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key" : "");
        }
        #endregion

        #region Card Renewals
        [WebMethod]
        public Response<RenewalFileSummary> RenewalUploadFile(int issuerId, byte[] fileContent, string fileName, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _cardRenewalController.ExtractFile(issuerId, fileContent, fileName, session.LanguageId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<RenewalFileSummary>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalFileSummary> RenewalConfirmUpload(long cardRenewalId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.ConfirmLoad(cardRenewalId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<RenewalFileSummary>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalFileSummary> RenewalRejectUpload(long cardRenewalId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.RejectLoad(cardRenewalId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<RenewalFileSummary>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<RenewalFileListModel>> RenewalList(RenewalStatusType status, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.GetList(status, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<RenewalFileListModel>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalFileViewModel> RenewalRetrieve(long cardRenewalId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.Retrieve(cardRenewalId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<RenewalFileViewModel>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalFileSummary> RenewalApproveUpload(long cardRenewalId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.ApproveFileLoad(cardRenewalId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<RenewalFileSummary>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<RenewalDetailListModel>> RenewalDetailList(long renewalId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.GetDetailList(renewalId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<RenewalDetailListModel>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalDetailListModel> RenewalDetailRetrieve(long renewalDetailId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.GetDetail(renewalDetailId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<RenewalDetailListModel>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalDetailListModel> RenewalDetailRetrieveByCard(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.GetDetailCard(cardId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<RenewalDetailListModel>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        [WebMethod]
        public Response<RenewalDetailListModel> RenewalDetailApprove(long renewalDetailId, int deliveryBranchId, int currencyId, string cmsAccountType, string cbsAccountType, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.ApproveCardRenewal(renewalDetailId, deliveryBranchId, currencyId, cbsAccountType, cmsAccountType, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<RenewalDetailListModel>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> RenewalLinkCard(long renewalDetailId, long cardId, string cardNumber, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                bool result = _cardRenewalController.LinkRenewalToCard(renewalDetailId, cardId, cardNumber, session.UserId, session.Workstation);
                return new Response<bool>(result, ResponseType.SUCCESSFUL,
                                    string.Empty,
                                    string.Empty);
            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalDetailListModel> RenewalDetailReject(long renewalDetailId, int deliveryBranchId, string comment, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.RejectCardRenewal(renewalDetailId, deliveryBranchId, comment, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<RenewalDetailListModel>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalDetailListModel> RenewalDetailRejectCardReceived(long renewalDetailId, int deliveryBranchId, string comment, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.RejectCardRenewalReceived(renewalDetailId, deliveryBranchId, comment, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<RenewalDetailListModel>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<RenewalBatch>> RenewalBatchCreate(string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _cardRenewalController.CreateBatch(session.UserId, session.Workstation);
                return result;
            }

            return new Response<List<RenewalBatch>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalBatch> RenewalBatchApprove(long renewalBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _cardRenewalController.ApproveBatch(renewalBatchId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<RenewalBatch>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalBatch> RenewalBatchChangeStatus(long renewalBatchId, RenewalBatchStatusType statusType, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _cardRenewalController.RenewalChangeBatchStatus(renewalBatchId, statusType, session.UserId, session.Workstation);
                return result;
            }

            return new Response<RenewalBatch>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalBatch> RenewalBatchDistribute(long renewalBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _cardRenewalController.DistributeBatch(renewalBatchId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<RenewalBatch>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalBatch> RenewalBatchReject(long renewalBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _cardRenewalController.RejectBatch(renewalBatchId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<RenewalBatch>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<RenewalBatch> RenewalBatchRetrieve(long renewalBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _cardRenewalController.GetBatch(renewalBatchId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<RenewalBatch>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");

        }

        [WebMethod]
        public Response<List<RenewalDetailListModel>> RenewalBatchRetrieveDetails(long renewalBatchId, bool masked, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _cardRenewalController.GetBatchDetails(renewalBatchId, masked, session.LanguageId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<List<RenewalDetailListModel>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<RenewalBatch>> RenewalBatchRetrieveList(RenewalBatchStatusType statusType, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _cardRenewalController.GetBatches(statusType, session.UserId, session.Workstation);
                return result;
            }

            return new Response<List<RenewalBatch>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<RenewalDetailListModel>> RenewalDetailInStatus(RenewalDetailStatusType statusType, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.GetDetailListInStatus(statusType, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<RenewalDetailListModel>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<long>> CreateRenewalDistributionBatches(long renewalBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.CreateDistributionBatches(renewalBatchId, session.LanguageId, session.UserId, session.Workstation);
            }

            return new Response<List<long>>(null, ResponseType.SESSION_ERROR,
                                                            "Problem with Session, please login again.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GenerateRenewalFileReport(long renewalId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.GenerateRenewalFile(renewalId, session.LanguageId, session.Username, session.UserId, session.Workstation);
            }

            return new Response<byte[]>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GenerateRenewalBatchReport(long renewalBatchId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.GenerateRenewalBatchReport(renewalBatchId, session.LanguageId, session.Username, session.UserId, session.Workstation);
            }

            return new Response<byte[]>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> GenerateRenewalNewBatchReport(List<long> renewalBatchIds, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                return _cardRenewalController.GenerateRenewalNewBatchReport(renewalBatchIds, session.LanguageId, session.Username, session.UserId, session.Workstation);
            }

            return new Response<byte[]>(null,
                                        ResponseType.SESSION_ERROR,
                                        "Problem with Session, please login again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
        #endregion

        [WebMethod]
        public Response<int> DocumentTypeCreate(DocumentType entity, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (entity.Id == 0)
                {
                    var result = _documentManagementController.DocumentTypeSave(entity, session.LanguageId, session.UserId, session.Workstation);
                    return result;
                }
                return new Response<int>(0, ResponseType.ERROR,
                                  "Entity is not a new entry.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Entity Id." : "");
            }

            return new Response<int>(0, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<int> DocumentTypeUpdate(DocumentType entity, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (entity.Id != 0)
                {
                    var result = _documentManagementController.DocumentTypeSave(entity, session.LanguageId, session.UserId, session.Workstation);
                    return result;
                }
                return new Response<int>(0, ResponseType.ERROR,
                                  "Entity does not exist.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Entity.  Does not exist." : "");
            }

            return new Response<int>(0, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<SystemResponseCode> DocumentTypeDelete(int id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (id == 0)
                {
                    var result = _documentManagementController.DocumentTypeDelete(id, session.LanguageId, session.UserId, session.Workstation);
                    return result;
                }
                return new Response<SystemResponseCode>(SystemResponseCode.DELETE_FAIL, ResponseType.ERROR,
                                  "Entity does not exist.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Entity.  Does not exist." : "");
            }

            return new Response<SystemResponseCode>(SystemResponseCode.SESSIONKEY_AUTHORISATION_FAIL, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<DocumentType> DocumentTypeGet(int id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _documentManagementController.DocumentTypeGet(id, session.LanguageId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<DocumentType>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<DocumentType>> DocumentTypeGetAll(bool activeOnly, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _documentManagementController.DocumentTypeAll(activeOnly, session.LanguageId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<List<DocumentType>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<int> ProductDocumentCreate(ProductDocument entity, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (entity.Id == 0)
                {
                    var result = _documentManagementController.ProductDocumentSave(entity, session.LanguageId, session.UserId, session.Workstation);
                    return result;
                }
                return new Response<int>(0, ResponseType.ERROR,
                                  "Entity is not a new entry.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Entity Id." : "");
            }

            return new Response<int>(0, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<int> ProductDocumentUpdate(ProductDocument entity, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (entity.Id != 0)
                {
                    var result = _documentManagementController.ProductDocumentSave(entity, session.LanguageId, session.UserId, session.Workstation);
                    return result;
                }
                return new Response<int>(0, ResponseType.ERROR,
                                  "Entity does not exist.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Entity.  Does not exist." : "");
            }

            return new Response<int>(0, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<SystemResponseCode> ProductDocumentDelete(int id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (id == 0)
                {
                    var result = _documentManagementController.ProductDocumentDelete(id, session.LanguageId, session.UserId, session.Workstation);
                    return result;
                }
                return new Response<SystemResponseCode>(SystemResponseCode.DELETE_FAIL, ResponseType.ERROR,
                                  "Entity does not exist.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Entity.  Does not exist." : "");
            }

            return new Response<SystemResponseCode>(SystemResponseCode.SESSIONKEY_AUTHORISATION_FAIL, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ProductDocumentListModel> ProductDocumentGet(int id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _documentManagementController.ProductDocumentGet(id, session.LanguageId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<ProductDocumentListModel>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<ProductDocumentListModel>> ProductDocumentGetAll(int productId, bool activeOnly, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _documentManagementController.ProductDocumentAll(productId, activeOnly, session.LanguageId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<List<ProductDocumentListModel>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<long>> CardDocumentCreateBulk(List<CardDocument> bulkDocuments, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            List<long> result = new List<long>();
            if (session != null)
            {
                foreach (var item in bulkDocuments)
                {
                    result.Add(_documentManagementController.CardDocumentSave(item, session.LanguageId, session.UserId, session.Workstation).Value);
                }
                return new Response<List<long>>(result, ResponseType.ERROR,
                                  "Entity is not a new entry.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Entity Id." : "");
            }

            return new Response<List<long>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<long> CardDocumentCreate(CardDocument entity, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (entity.Id == 0)
                {
                    var result = _documentManagementController.CardDocumentSave(entity, session.LanguageId, session.UserId, session.Workstation);
                    return result;
                }
                return new Response<long>(0, ResponseType.ERROR,
                                  "Entity is not a new entry.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Entity Id." : "");
            }

            return new Response<long>(0, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<long> CardDocumentUpdate(CardDocument entity, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (entity.Id != 0)
                {
                    var result = _documentManagementController.CardDocumentSave(entity, session.LanguageId, session.UserId, session.Workstation);
                    return result;
                }
                return new Response<long>(0, ResponseType.ERROR,
                                  "Entity does not exist.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Entity.  Does not exist." : "");
            }

            return new Response<long>(0, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<SystemResponseCode> CardDocumentDelete(int id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                if (id == 0)
                {
                    var result = _documentManagementController.CardDocumentDelete(id, session.LanguageId, session.UserId, session.Workstation);
                    return result;
                }
                return new Response<SystemResponseCode>(SystemResponseCode.DELETE_FAIL, ResponseType.ERROR,
                                  "Entity does not exist.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Entity.  Does not exist." : "");
            }

            return new Response<SystemResponseCode>(SystemResponseCode.SESSIONKEY_AUTHORISATION_FAIL, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<CardDocument> CardDocumentGet(long id, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _documentManagementController.CardDocumentGet(id, session.LanguageId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<CardDocument>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CardDocument>> CardDocumentGetAll(long cardId, bool activeOnly, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _documentManagementController.CardDocumentAll(cardId, activeOnly, session.LanguageId, session.UserId, session.Workstation);
                return result;
            }

            return new Response<List<CardDocument>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<ProductDocumentStructure> GetProductDocuments(int productId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            DocumentStorageType storageType = DocumentStorageType.Unknown;
            ProductDocumentStructure response = new ProductDocumentStructure()
            {
                StorageType = DocumentStorageType.Unknown,
                ProductDocuments = new List<ProductDocumentListModel>()
            };

            if (session != null)
            {
                var localFolder = ConfigurationManager.AppSettings["LocalDocumentsLocation"];
                var remoteLocation = ConfigurationManager.AppSettings["RemoteDocumentLocation"];
                if (localFolder != null)
                {
                    if (localFolder.ToString() != string.Empty)
                    {
                        storageType = DocumentStorageType.Local;
                    }
                }
                if (storageType == DocumentStorageType.Unknown)
                {
                    if (remoteLocation != null)
                    {
                        if (remoteLocation.ToString() != string.Empty)
                        {
                            storageType = DocumentStorageType.Remote;
                        }
                    }
                }

                response.ProductDocuments.AddRange(_documentManagementController.ProductDocumentAll(productId, true, session.LanguageId, session.UserId, session.Workstation).Value);
                response.StorageType = storageType;
                return new Response<ProductDocumentStructure>(response, ResponseType.SUCCESSFUL,
                                 "",
                                 "");
            }

            return new Response<ProductDocumentStructure>(response, ResponseType.SESSION_ERROR,
                                 "Problem with Session, please login again.",
                                 log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LocalFileModel>> DocumentUploadLocal(string accountNumber, string customerId, List<LocalFileModel> documents, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                try
                {
                    List<LocalFileModel> filePaths = _documentManagementController.SaveLocalFiles(accountNumber, customerId, documents);

                    return new Response<List<LocalFileModel>>(null, ResponseType.SUCCESSFUL,
                                         "",
                                         "");
                }
                catch (Exception)
                {
                    return new Response<List<LocalFileModel>>(null, ResponseType.ERROR,
                                          "Problem with file save, please login again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? "General file error." : "");
                }
            }
            return new Response<List<LocalFileModel>>(null, ResponseType.SESSION_ERROR,
                                  "Problem with Session, please login again.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<LocalFileModel>> DocumentUploadRemote(string accountNumber, string customerId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                try
                {
                    List<LocalFileModel> filePaths = new List<LocalFileModel>();

                    return new Response<List<LocalFileModel>>(null, ResponseType.SUCCESSFUL,
                                         "",
                                         "");
                }
                catch (Exception)
                {
                    return new Response<List<LocalFileModel>>(null, ResponseType.ERROR,
                                          "Problem with file save, please login again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? "General file error." : "");
                }
            }
            return new Response<List<LocalFileModel>>(null, ResponseType.SESSION_ERROR,
                                  "Problem with Session, please login again.",
                                  log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> CardLimitCreate(long cardId, decimal limit, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _issueCardController.CreateCardLimit(cardId, limit, session.LanguageId, session.UserId, session.Workstation);

                return result;
            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> CardLimitUpdate(long cardId, decimal limit, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _issueCardController.UpdateCardLimit(cardId, limit, session.LanguageId, session.UserId, session.Workstation);

                return result;
            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> SetCreditStatus(long cardId, int creditStatus, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _issueCardController.SetCreditStatus(cardId, creditStatus, session.LanguageId, session.UserId, session.Workstation);

                return result;
            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> CardLimitApprove(long cardId, decimal limit, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _issueCardController.ApproveCardLimit(cardId, limit, session.LanguageId, session.UserId, session.Workstation);

                return result;
            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> CardLimitSetManualContractNumber(long cardId, string contractNumber, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _issueCardController.SetCreditContractNumber(cardId, contractNumber, session.LanguageId, session.UserId, session.Workstation);

                return result;
            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<bool> CardLimitApproveManager(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _issueCardController.ApproveCardLimitManager(cardId, session.LanguageId, session.UserId, session.Workstation);

                return result;
            }

            return new Response<bool>(false, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<CardLimitModel> CardLimitGet(long cardId, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _issueCardController.GetCardLimit(cardId, session.LanguageId, session.UserId, session.Workstation);

                return result;
            }

            return new Response<CardLimitModel>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<List<CardDocument>> GetRemoteDocuments(string accountNumber, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _remoteDocumentController.GetDocuments(accountNumber);

                return new Response<List<CardDocument>>(result, ResponseType.SUCCESSFUL,
                                    "",
                                    "");
            }

            return new Response<List<CardDocument>>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }

        [WebMethod]
        public Response<byte[]> DownloadRemoteDocument(string documentKey, string encryptedSessionKey)
        {
            var session = _SessinManager.isValidSession(encryptedSessionKey);
            if (session != null)
            {
                var result = _remoteDocumentController.DownloadDocument(documentKey);

                return new Response<byte[]>(result, ResponseType.SUCCESSFUL,
                                    "",
                                    "");
            }

            return new Response<byte[]>(null, ResponseType.SESSION_ERROR,
                                    "Problem with Session, please login again.",
                                    log.IsDebugEnabled || log.IsTraceEnabled ? "Invalid Session Key." : "");
        }
    }
}