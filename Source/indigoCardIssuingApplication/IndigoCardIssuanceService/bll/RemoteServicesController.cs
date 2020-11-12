using Common.Logging;
using IndigoCardIssuanceService.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.RemoteManagement;

namespace IndigoCardIssuanceService.bll
{
    public class RemoteServicesController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IssueCardController));
        private readonly RemoteCardUpdateService _remoteService = new RemoteCardUpdateService();
        private readonly ResponseTranslator _translator = new ResponseTranslator();


        public Response<List<RemoteCardUpdateSearchResult>> SearchRemoteCardUpdates(string pan, int? remoteUpdateStatusesId, int? issuerId, int? branchId, int? productId, DateTime? dateFrom,
                                DateTime? dateTo, int pageIndex, int rowsPerPage, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<RemoteCardUpdateSearchResult>>(_remoteService.SearchRemoteCardUpdates(pan, remoteUpdateStatusesId, issuerId, branchId, productId, dateFrom,
                                                                        dateTo, pageIndex, rowsPerPage, languageId, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<RemoteCardUpdateSearchResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        public Response<RemoteCardUpdateDetailResult> GetRemoteCardDetail(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<RemoteCardUpdateDetailResult>(_remoteService.GetRemoteCardDetail(cardId, languageId, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RemoteCardUpdateDetailResult>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        public Response<RemoteCardUpdateDetailResult> ChangeRemoteCardStatus(long cardId, int remoteUpdateStatusesId, string comment, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<RemoteCardUpdateDetailResult>(_remoteService.ChangeRemoteCardStatus(cardId, remoteUpdateStatusesId, comment, languageId, auditUserId, auditWorkstation),
                                                                    ResponseType.SUCCESSFUL,
                                                                    "",
                                                                    "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<RemoteCardUpdateDetailResult>(null, ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
           

        public BaseResponse ChangeRemoteCardsStatus(List<long> cardIds, int remoteUpdateStatusesId, string comment, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                _remoteService.ChangeRemoteCardsStatus(cardIds, remoteUpdateStatusesId, comment, languageId, auditUserId, auditWorkstation);

                return new BaseResponse(ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error when processing request.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
    }
}