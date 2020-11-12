using Common.Logging;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.Common.Models;
using System;
using System.Collections.Generic;
using Veneka.Indigo.CardManagement.objects;

namespace IndigoCardIssuanceService.bll
{
    public class SystemConfigController
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(IssueCardController));
        private readonly ExternalSystemsManagementService _externalsystem = new ExternalSystemsManagementService();
        private readonly ResponseTranslator _translator = new ResponseTranslator();

        #region "External Systems"

        internal Response<int?> CreateExternalSystems(ExternalSystemFieldResult externalsystem, long auditUserId, string auditWorkstation, int languageId)
        {
            try
            {

                string responseMessage=string.Empty; int? externalsytemid=null;
                if (_externalsystem.CreateExternalSystems(externalsystem, auditUserId, auditWorkstation, languageId, out externalsytemid, out responseMessage))
                {
                    return new Response<int?>(externalsytemid,
                                                      ResponseType.SUCCESSFUL,
                                                      responseMessage,
                                                      "");
                }

                return new Response<int?>(null, ResponseType.UNSUCCESSFUL, responseMessage, "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int?>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Persist changes to the DB.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse UpdateExternalSystem(ExternalSystemFieldResult externalsystem, long auditUserId, string auditWorkstation, int languageId)
        {
            try
            {
                string responseMessage;
                if (_externalsystem.UpdateExternalSystem(externalsystem, auditUserId, auditWorkstation,languageId, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Fetch all countires
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<ExternalSystemFieldResult> GetExternalSystems(int? externalSystemid, int rowindex, int rowsperpage, long auditUserId, string auditWorkstation, int languageId)
        {
            try
            {
                return new Response<ExternalSystemFieldResult>(_externalsystem.GetExternalSystems(externalSystemid, rowindex, rowsperpage, languageId, auditUserId, auditWorkstation),
                                                   ResponseType.SUCCESSFUL,
                                                   "",
                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ExternalSystemFieldResult>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse DeleteExternalSystems(int? externalsystemid, long auditUserId, string auditWorkstation, int languageId)
        {
            try
            {
                string responseMessage;
                if (_externalsystem.DeleteExternalSystems(externalsystemid, auditUserId, auditWorkstation, languageId,out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        internal Response<int?> CreateExternalSystemFields(ExternalSystemFieldsResult externalsystem, long auditUserId, string auditWorkstation, int languageId)
        {
            try
            {

                string responseMessage=string.Empty; int? externalsytemid=null;
                if (_externalsystem.CreateExternalSystemFields(externalsystem, auditUserId, auditWorkstation, languageId, out externalsytemid, out responseMessage))
                {
                    return new Response<int?>(externalsytemid,
                                                      ResponseType.SUCCESSFUL,
                                                      responseMessage,
                                                      "");
                }

                return new Response<int?>(null, ResponseType.UNSUCCESSFUL, responseMessage, "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int?>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Persist changes to the DB.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse UpdateExternalSystemFields(ExternalSystemFieldsResult externalsystem,  long auditUserId, string auditWorkstation,int languageId)
        {
            try
            {
                string responseMessage;
                if (_externalsystem.UpdateExternalSystemFields(externalsystem, auditUserId, auditWorkstation, languageId, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Fetch all external system fields
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<ExternalSystemFieldsResult>> GetExternalSystemsFields(int? externalSystemfieldid, int rowindex, int rowsperpage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<ExternalSystemFieldsResult>>(_externalsystem.GetExternalSystemsFields(externalSystemfieldid, rowindex, rowsperpage, auditUserId, auditWorkstation),
                                                   ResponseType.SUCCESSFUL,
                                                   "",
                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ExternalSystemFieldsResult>>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse DeleteExternalSystemField(int? externalsystemfieldid, long auditUserId, string auditWorkstation, int languageId)
        {
            try
            {
                string responseMessage;
                if (_externalsystem.DeleteExternalSystemField(externalsystemfieldid, auditUserId, auditWorkstation, languageId, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupExternalSystems(int languageId,long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_externalsystem.LangLookupExternalSystems(languageId,auditUserId, auditWorkstation),
                                                   ResponseType.SUCCESSFUL,
                                                   "",
                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        #endregion
    }
}