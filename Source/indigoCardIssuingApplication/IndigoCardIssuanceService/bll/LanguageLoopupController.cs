using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.LanguageLookup;
using Common.Logging;
using IndigoCardIssuanceService.DataContracts;

namespace IndigoCardIssuanceService.bll
{
    internal class LanguageLoopupController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LanguageLoopupController));
        private LanguageLookupService _langService = LanguageLookupService.Instance;

        public LanguageLoopupController() { }

        internal Response<List<language>> GetLanguages(long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<language>>(_langService.GetLanguages(auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<language>>(null,
                                                    ResponseType.ERROR,
                                                    "An error occured during processing your request, please try again.",
                                                    log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Fetch user statuses from cache or db, based on language
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<LangLookup>> LangLookupUserStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupUserStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }            
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Fetch user roles based from cache or db based on language
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<LangLookup>> LangLookupUserRoles(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupUserRoles(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupAuditActions(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupAuditActions(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupBranchCardStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupBranchCardStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupBranchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupBranchStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupCardIssueReasons(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupCardIssueReasons(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupCustomerAccountTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupCustomerAccountTypes(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupDistBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupDistBatchStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupDistCardStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupDistCardStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupFileStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupFileStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupInterfaceTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupInterfaceTypes(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupIssuerStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupIssuerStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupLoadBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupLoadBatchStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupLoadCardStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupLoadCardStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupCustomerResidency(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupCustomerResidency(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupCustomerTitle(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupCustomerTitle(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupCustomerType(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupCustomerType(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookupChar>> LangLookupGenderType(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookupChar>>(_langService.LangLookupGenderType(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookupChar>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupConnectionParameterType(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupConnectionParameterType(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupCardIssueMethod(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupCardIssueMethod(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
        internal Response<List<LangLookup>> LangLookupPinBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupPinBatchStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupPinReissueStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupPinReissueStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupExportBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupExportBatchStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }


        internal Response<List<LangLookup>> LangLookupRemoteUpdateStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupRemoteUpdateStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupProductLoadTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupProductLoadTypes(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupFileEncryptionTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupFileEncryptionTypes(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
        internal Response<List<LangLookup>> LangLookupBranchTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupBranchTypes(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupPrintBatchStatues(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupPrintBatchStatues(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
        internal Response<List<LangLookup>> LangLookupHybridRequestStatues(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupHybridRequestStatues(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<LangLookup>> LangLookupThreedBatchStatuses(int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LangLookup>>(_langService.LangLookupThreedBatchStatuses(languageId, auditUserId, auditWorkstation),
                                                      ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LangLookup>>(null,
                                                      ResponseType.ERROR,
                                                      "An error occured during processing your request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }
    }
}