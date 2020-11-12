using System;
using System.Collections.Generic;
using System.Linq;
using Veneka.Indigo.Common.Objects;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.PINManagement;
using Veneka.Indigo.PINManagement.objects;
using Veneka.Indigo.Common.Models;
using Common.Logging;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Common;
using System.Collections;
using Veneka.Indigo.COMS.Core;
using IndigoCardIssuanceService.COMS;
using Veneka.Indigo.Common.Models.IssuerManagement;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.PINManagement.Reports;

namespace IndigoCardIssuanceService.bll
{
    public class PINBatchController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PINBatchController));

        private readonly PINManagementService _pinMan = new PINManagementService();
        private SessionManager _sessionManager = SessionManager.GetInstance();

        private static readonly object _lockObject = new object();
        private static readonly object _lockPinPrinting = new object();

        /// <summary>
        /// Searches for all PIN batches that the specific user has access to based on the input parameters.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="pinBatchReference"></param>
        /// <param name="pinBatchStatusId"></param>
        /// <param name="branchId"></param>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="pinBatchTypeId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="langaugeId"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<PinBatchResult>> GetPinBatchesForUser(int? issuerId, string pinBatchReference, int? pinBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                               int? pinBatchTypeId, DateTime? startDate, DateTime? endDate, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            Response<List<PinBatchResult>> response;
            try
            {
                response = new Response<List<PinBatchResult>>(_pinMan.GetPinBatchesForUser(issuerId, pinBatchReference, pinBatchStatusId, branchId, cardIssueMethodId,
                                                                                                   pinBatchTypeId, startDate, endDate, langaugeId, rowsPerPage, pageIndex,
                                                                                                   auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<PinBatchResult>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<List<PinRequestList>> GetPinRequestsForUser(int? issuerId, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            Response<List<PinRequestList>> response;
            try
            {
                response = new Response<List<PinRequestList>>(_pinMan.GetPinRequestsForUser(issuerId, langaugeId, rowsPerPage, pageIndex,
                                                                                                   auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<PinRequestList>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }
        //GetPinRequestsForUserForStatus
        internal Response<List<PinRequestList>> GetPinRequestsForUserForStatus(int? issuerId, string request_status, string reissue_approval_stage, string request_type, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            Response<List<PinRequestList>> response;
            try
            {
                response = new Response<List<PinRequestList>>(_pinMan.GetPinRequestsForUserForStatus(issuerId, request_status, reissue_approval_stage, request_type, langaugeId, rowsPerPage, pageIndex,
                                                                                                   auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<PinRequestList>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<List<PinRequestList>> SearchForPinReIssue(int? issuerId, int? BranchId, int? ProductId, string ProductBin, string lastdigits, string accountnumber,
                                                                   string pinrequestref, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            Response<List<PinRequestList>> response;
            try
            {
                response = new Response<List<PinRequestList>>(_pinMan.SearchForPinReIssue(issuerId, BranchId, ProductId, ProductBin, lastdigits, accountnumber, pinrequestref,
                                                                        pageIndex, rowsPerPage, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<PinRequestList>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<List<TerminalCardData>> GetPinBlockForUser(int? issuerId, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            Response<List<TerminalCardData>> response;
            try
            {
                response = new Response<List<TerminalCardData>>(_pinMan.GetPinBlockForUser(issuerId, langaugeId, rowsPerPage, pageIndex,
                                                                                                   auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<TerminalCardData>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }
        //GetPinBlockForUserForStatus
        internal Response<List<TerminalCardData>> GetPinBlockForUserForStatus(int? issuerId, string request_status, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            Response<List<TerminalCardData>> response;
            try
            {
                response = new Response<List<TerminalCardData>>(_pinMan.GetPinBlockForUserForStatus(issuerId, request_status, langaugeId, rowsPerPage, pageIndex,
                                                                                                   auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<TerminalCardData>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        //GetPinBatchForUserForStatus
         internal Response<List<TerminalCardData>> GetPinBatchForUserForStatus(int? issuerId, string request_status, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            Response<List<TerminalCardData>> response;
            try
            {
                response = new Response<List<TerminalCardData>>(_pinMan.GetPinBatchForUserForStatus(issuerId, request_status, langaugeId, rowsPerPage, pageIndex,
                                                                                                   auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<List<TerminalCardData>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<PinRequestViewDetails> GetPinRequestViewDetails(long pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            Response<PinRequestViewDetails> response;
            try
            {
                response = new Response<PinRequestViewDetails>(_pinMan.GetPinRequestViewDetails(pin_request_id, langaugeId, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<PinRequestViewDetails>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }
        //GetPinBlockViewDetails
        internal Response<TerminalCardData> GetPinBlockViewDetails(long pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            Response<TerminalCardData> response;
            try
            {
                response = new Response<TerminalCardData>(_pinMan.GetPinBlockViewDetails(pin_request_id, langaugeId, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<TerminalCardData>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        //GetPinBatchViewDetails

        internal Response<TerminalCardData> GetPinBatchViewDetails(long pin_request_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            Response<TerminalCardData> response;
            try
            {
                response = new Response<TerminalCardData>(_pinMan.GetPinBatchViewDetails(pin_request_id, langaugeId, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<TerminalCardData>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<TerminalCardData> GetTerminalData(string pan_product_bin, string pan_last_four, string expiry_date, int langaugeId, long auditUserId, string auditWorkstation)
        {
            Response<TerminalCardData> response;
            try
            {
                response = new Response<TerminalCardData>(_pinMan.GetTerminalCardData(pan_product_bin, pan_last_four, expiry_date, langaugeId, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<TerminalCardData>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<RestWebservicesHandler> GetRestParams(int issuer_id, string webservice_type, int langaugeId, long auditUserId, string auditWorkstation)
        {
            Response<RestWebservicesHandler> response;
            try
            {
                response = new Response<RestWebservicesHandler>(_pinMan.GetRestParams(issuer_id, webservice_type, langaugeId, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<RestWebservicesHandler>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal Response<ZMKZPKDetails> GetZoneKeys(int? issuer_id, int langaugeId, long auditUserId, string auditWorkstation)
        {
            Response<ZMKZPKDetails> response;
            try
            {
                response = new Response<ZMKZPKDetails>(_pinMan.GetZoneKeys(issuer_id, langaugeId, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<ZMKZPKDetails>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        //GetGetZoneKeys
        /// <summary>
        /// Returns the pin batch details based on the pin batch.
        /// </summary>
        /// <param name="pinBatchId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<PinBatchResult> GetPinBatch(long pinBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            Response<PinBatchResult> response;
            try
            {
                response = new Response<PinBatchResult>(_pinMan.GetPinBatch(pinBatchId, languageId, auditUserId, auditWorkstation),
                                                        ResponseType.SUCCESSFUL,
                                                        "",
                                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = new Response<PinBatchResult>(null,
                                                         ResponseType.ERROR,
                                                         "Error when processing request.",
                                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }

            return response;
        }

        internal BaseResponse UpdateMuiltplePinBatchStatus(ArrayList pinBatchIds, ArrayList pinBatchStatusIds, ArrayList flowPinBatchStatusesIds, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                for (int i = 0; i < pinBatchIds.Count; i++)
                {
                    long pinBatchId = 0;
                    int pinBatchStatusId = 0, flowPinBatchStatusesId = 0;
                    try
                    {

                        long.TryParse(pinBatchIds[i].ToString(), out pinBatchId);
                        int.TryParse(pinBatchStatusIds[i].ToString(), out pinBatchStatusId);
                        int.TryParse(flowPinBatchStatusesIds[i].ToString(), out flowPinBatchStatusesId);

                        if (pinBatchId > 0)
                        {
                            Response<PinBatchResult> result = UpdatePinBatchStatus(pinBatchId, pinBatchStatusId, flowPinBatchStatusesId, notes, languageId, auditUserId, auditWorkstation);
                            if (result != null)
                            {
                                log.Debug(string.Format("Status of  pinBatchId {0}, pinBatchStatusId {1}, flowPinBatchStatusesId {2} ,response: {3}", pinBatchId, pinBatchStatusId, flowPinBatchStatusesId, result.ResponseMessage));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Failed to Update pinBatchId {0}, pinBatchStatusId {1}, flowPinBatchStatusesId {2} ", pinBatchId, pinBatchStatusId, flowPinBatchStatusesId));
                        log.Error(ex);
                    }

                }
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                                     "Action Was Successful.",
                                                    "");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
        internal Response<PinBatchResult> UpdatePinBatchStatus(long pinBatchId, int pinBatchStatusId, int flowPinBatchStatusesId, string statusNote, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage = String.Empty;
                PinBatchResult BatchResult;

                bool isSuccessful = false;

                //Determin what we need to do before changing the batches status
                switch (flowPinBatchStatusesId)
                {
                    case 7: isSuccessful = PrintPins(pinBatchId, languageId, auditUserId, auditWorkstation, out resultMessage);
                        break;
                    case 9: isSuccessful = CMSUpload(pinBatchId, languageId, auditUserId, auditWorkstation, out resultMessage);
                        break;
                    default: isSuccessful = true;
                        break;
                }

                //If the previous was succesfull then update the batch
                if (isSuccessful)
                {
                    if (_pinMan.UpdatePinBatchStatus(pinBatchId, pinBatchStatusId, flowPinBatchStatusesId, statusNote, languageId, auditUserId, auditWorkstation, out BatchResult, out resultMessage))
                    {
                        return new Response<PinBatchResult>(BatchResult,
                                                             ResponseType.SUCCESSFUL,
                                                             resultMessage,
                                                             resultMessage);
                    }
                    else
                    {
                        return new Response<PinBatchResult>(null, ResponseType.UNSUCCESSFUL, resultMessage, resultMessage);
                    }
                }
                else
                {
                    return new Response<PinBatchResult>(null,
                                                         ResponseType.UNSUCCESSFUL,
                                                         resultMessage,
                                                         resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<PinBatchResult>(null,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<PinBatchResult> PinBatchRejectStatus(long pinBatchId, int pinBatchStatusesId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage;
                PinBatchResult BatchResult;

                if (_pinMan.PinBatchRejectStatus(pinBatchId, pinBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, out BatchResult, out resultMessage))
                {
                    return new Response<PinBatchResult>(BatchResult,
                                                         ResponseType.SUCCESSFUL,
                                                         resultMessage,
                                                         resultMessage);
                }
                else
                {
                    return new Response<PinBatchResult>(null, ResponseType.UNSUCCESSFUL, resultMessage, resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<PinBatchResult>(null,
                                                     ResponseType.ERROR,
                                                     "Error when processing request.",
                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<byte[]> PinBatchGenerateReport(int pinBatchId, int languageId, long auditUserId, string username, string auditWorkstation)
        {
                try
                {
                PinBatchReport pinBatchReport = new PinBatchReport();
                    return new Response<byte[]>(pinBatchReport.GeneratePinMailerBatchReport(pinBatchId, languageId,  auditUserId,  username, auditWorkstation),
                                                ResponseType.SUCCESSFUL,
                                                ResponseType.SUCCESSFUL.ToString(),
                                                "");
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    return new Response<byte[]>(null,
                                                ResponseType.ERROR,
                                                "An error occured during processing your request, please try again.",
                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
                }
           
        }

        internal BaseResponse PinMailerReprintRequest(long cardId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage;

                if (_pinMan.PinMailerReprintRequest(cardId, notes, languageId, auditUserId, auditWorkstation, out resultMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL,
                                             resultMessage,
                                             resultMessage);
                }
                else
                {
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, resultMessage, resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                         "Error when processing request.",
                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal BaseResponse PinMailerReprintApprove(long cardId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage;

                if (_pinMan.PinMailerReprintApprove(cardId, notes, languageId, auditUserId, auditWorkstation, out resultMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL,
                                             resultMessage,
                                             resultMessage);
                }
                else
                {
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, resultMessage, resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                         "Error when processing request.",
                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal BaseResponse PinMailerReprintReject(long cardId, string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string resultMessage;

                if (_pinMan.PinMailerReprintReject(cardId, notes, languageId, auditUserId, auditWorkstation, out resultMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL,
                                             resultMessage,
                                             resultMessage);
                }
                else
                {
                    return new BaseResponse(ResponseType.UNSUCCESSFUL, resultMessage, resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                         "Error when processing request.",
                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<PinMailerReprintResult>> SearchPinMailerReprint(int? issuerId, int? branchId, int? pinMailerReprintStatusId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<PinMailerReprintResult>>(_pinMan.SearchPinMailerReprint(issuerId, branchId, pinMailerReprintStatusId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<PinMailerReprintResult>>(null, ResponseType.ERROR,
                                                                    "Error when processing request.",
                                                                    log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<PinMailerReprintRequestResult>> PinMailerReprintList(int? issuerId, int? branchId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<PinMailerReprintRequestResult>>(_pinMan.PinMailerReprintList(issuerId, branchId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation),
                                                                            ResponseType.SUCCESSFUL,
                                                                            "",
                                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<PinMailerReprintRequestResult>>(null, ResponseType.ERROR,
                                                                         "Error when processing request.",
                                                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<int> PinMailerReprintBatchCreate(int? cardIssueMethodId, int? issuerId, int? branchId, int? productId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                int pinBatchId;
                string resultMessage;

                if (_pinMan.PinMailerReprintBatchCreate(cardIssueMethodId, issuerId, branchId, productId, languageId, auditUserId, auditWorkstation, out pinBatchId, out resultMessage))
                {
                    return new Response<int>(pinBatchId,
                                             ResponseType.SUCCESSFUL,
                                             resultMessage,
                                             resultMessage);
                }
                else
                {
                    return new Response<int>(0, ResponseType.UNSUCCESSFUL, resultMessage, resultMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(0, ResponseType.ERROR,
                                         "Error when processing request.",
                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<byte[]> GeneratePinBatchReport(long pinBatchId, int languageId, string username, long auditUserId, string auditWorkStation)
        {
            try
            {
                return new Response<byte[]>(_pinMan.GeneratePinBatchReport(pinBatchId, languageId, username, auditUserId, auditWorkStation),
                                            ResponseType.SUCCESSFUL,
                                            ResponseType.SUCCESSFUL.ToString(),
                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<byte[]>(null,
                                            ResponseType.ERROR,
                                            "An error occured during processing your request, please try again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        private bool CMSUpload(long pinBatchId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            //Locking this process so that only one thread can run it at a time.
            //this is because we're generating unique card numbers and dont want to threads to try generate the same card numbers
            lock (_lockObject)
            {
                var cardObjects = FetchCardObjectsForPinBatch(pinBatchId, languageId, auditUserId, auditWorkstation);
                IntegrationController _integration = IntegrationController.Instance;
                Veneka.Indigo.Integration.Config.IConfig config;
                Veneka.Indigo.Integration.External.ExternalSystemFields externalFields;

                try
                {
                    _integration.CardManagementSystem(cardObjects[0].IssuerId, InterfaceArea.PRODUCTION, out externalFields, out config);

                    InterfaceInfo interfaceInfo = new InterfaceInfo
                    {
                        Config = config,
                        InterfaceGuid = ""
                    };

                    AuditInfo auditInfo = new AuditInfo
                    {
                        AuditUserId = auditUserId,
                        AuditWorkStation = auditWorkstation,
                        LanguageId = languageId
                    };


                    var response = COMSController.ComsCore.UploadGeneratedCards(cardObjects, externalFields, interfaceInfo, auditInfo);

                    responseMessage = response.ResponseMessage;

                    if (response.ResponseCode == 0)
                    {                        
                        return true;
                    }

                    return false;
                    //return _integration.CardManagementSystem(cardObjects[0].IssuerId, InterfaceArea.PRODUCTION, out externalFields, out config).UploadGeneratedCards(cardObjects, externalFields, config, languageId, auditUserId, auditWorkstation, out responseMessage);
                }
                catch (NotImplementedException nie)
                {
                    log.Warn("UploadGeneratedCards() method in CMS module not implemented.", nie);

                    responseMessage = "CMS module not implemented.";
                    return false;
                }
            }
        }

        private bool PrintPins(long pinBatchId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            //Only allow one batch to be printed at a time.
            lock (_lockPinPrinting)
            {
                var cardObjects = FetchCardObjectsForPinBatch(pinBatchId, languageId, auditUserId, auditWorkstation);

                IntegrationController _integration = IntegrationController.Instance;
                Veneka.Indigo.Integration.Config.IConfig config;
                try
                {
                    _integration.HardwareSecurityModule(cardObjects[0].IssuerId, InterfaceArea.PRODUCTION, out config);

                    InterfaceInfo interfaceInfo = new InterfaceInfo
                    {
                        Config = config,
                        InterfaceGuid = ""
                    };

                    AuditInfo auditInfo = new AuditInfo
                    {
                        AuditUserId = auditUserId,
                        AuditWorkStation = auditWorkstation,
                        LanguageId = languageId
                    };


                    var response = COMSController.ComsCore.PrintPins(cardObjects, interfaceInfo, auditInfo);

                    responseMessage = response.ResponseMessage;

                    if (response.ResponseCode == 0)
                        return true;

                    return false;

                    //return _integration.HardwareSecurityModule(cardObjects[0].IssuerId, InterfaceArea.PRODUCTION, out config).PrintPins(ref cardObjects, config, languageId, auditUserId, auditWorkstation, out responseMessage);
                }
                catch (NotImplementedException nie)
                {
                    log.Warn("PrintPins() method in HSM module not implemented.", nie);
                    responseMessage = "CMS module not implemented.";
                    return false;
                }
            }
        }

        private List<CardObject> FetchCardObjectsForPinBatch(long pinBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            var cards = _pinMan.GetPinBatchCardDetails(pinBatchId, languageId, auditUserId, auditWorkstation);

            List<CardObject> cardObjects = new List<CardObject>();

            foreach (var card in cards)
            {
                //TODO: Remove PAN hardcoded length
                CardObject cardObj = new CardObject(card.card_id, card.card_request_reference, card.issuer_id, card.issuer_code, card.issuer_name,
                                                    card.branch_id, card.branch_code, card.branch_name, card.product_id,
                                                    card.product_code, card.product_bin_code, card.sub_product_code, 16, card.src1_id, card.src2_id,
                                                    card.src3_id, card.PVKI.Value, card.PVK, card.CVKA, card.CVKB, card.card_sequence);

                cardObj.ProductName = card.product_name;
                cardObj.SubProductCode = card.sub_product_code;
                cardObj.CardNumber = card.card_number;

                if (card.card_activation_date != null)
                    cardObj.CardActivatedDate = card.card_activation_date.Value;

                if (card.card_production_date != null)
                    cardObj.CardIssuedDate = card.card_production_date.Value;

                if (card.card_expiry_date != null)
                    cardObj.ExpiryDate = card.card_expiry_date;

                cardObj.ExpiryMonths = card.expiry_months.Value;

                //cardObj.DistBatchId = card.dist_batch_id;
                //cardObj.DistCardStatusId = card.dist_card_status_id;
                cardObj.PVV = card.pvv;

                if (card.card_issue_method_id == 0)
                {
                    var account = new AccountDetails();

                    account.AccountNumber = card.customer_account_number;
                    account.AccountTypeId = card.account_type_id.Value;
                    account.CardIssueMethodId = card.card_issue_method_id;
                    account.CardPriorityId = card.card_priority_id;
                    account.CmsID = card.cms_id;
                    account.ContactNumber = card.contact_number;
                    account.ContractNumber = card.contract_number;
                    account.CurrencyId = card.currency_id.GetValueOrDefault();
                    account.CustomerIDNumber = card.Id_number;
                    account.CustomerResidencyId = card.resident_id.GetValueOrDefault();
                    account.CustomerTitleId = card.customer_title_id.GetValueOrDefault();
                    account.CustomerTypeId = card.customer_type_id.GetValueOrDefault();
                    account.FirstName = card.customer_first_name;
                    account.LastName = card.customer_last_name;
                    account.MiddleName = card.customer_middle_name;
                    account.NameOnCard = card.name_on_card;
                    account.PriorityId = card.card_priority_id;
                    cardObj.CustomerAccount = account;
                }

                cardObjects.Add(cardObj);
            }

            return cardObjects;
        }


    }
}