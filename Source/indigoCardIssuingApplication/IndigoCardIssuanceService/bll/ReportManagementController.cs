using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Logging;
using Veneka.Indigo.Common.Models;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.CardManagement;

namespace IndigoCardIssuanceService.bll
{
    public class ReportManagementController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportManagementController));
        private static readonly ReportManagementService _reportService = new ReportManagementService();

        internal Response<List<BranchOrderReportResult>> GetBranchOrderReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId)
        {
            
            try
            {
                var result = _reportService.GetBranchOrderReport(issuerId, branchId, fromDate, toDate, languageId);

                return new Response<List<BranchOrderReportResult>>(result, ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<BranchOrderReportResult>>(null,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<CardProductionReportResult>> GetCardProductionReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var result = _reportService.GetCardProductionReport(issuerId, branchId, fromDate, toDate, languageId, auditUserId, auditWorkstation);

                return new Response<List<CardProductionReportResult>>(result, ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CardProductionReportResult>>(null,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<CardDispatchReportResult>> GetCardDispatchReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var result = _reportService.GetCardDispatchReport(issuerId, branchId, fromDate, toDate, languageId, auditUserId, auditWorkstation);

                return new Response<List<CardDispatchReportResult>>(result, ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CardDispatchReportResult>>(null,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<PinMailerReportResult>> GetPinMailerReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId)
        {
            try
            {
                var result = _reportService.GetPinMailerReport(issuerId, branchId, fromDate, toDate, languageId);

                return new Response<List<PinMailerReportResult>>(result, ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<PinMailerReportResult>>(null,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<CardExpiryReportResult>> GetCardExpiryReport(int? issuerId, int? branchId, DateTime fromDate, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                var result = _reportService.GetCardExpiryReport(issuerId, branchId, fromDate, languageId, auditUserId, auditWorkstation);

                return new Response<List<CardExpiryReportResult>>(result, ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<CardExpiryReportResult>>(null,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        internal Response<List<auditreport_usergroup_Result>> GetUsersByRoles_AuditReport(int? issuer_id, int? user_group_id, int? user_role_id, int? user_id, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<auditreport_usergroup_Result>>(_reportService.GetUsersByRoles_AuditReport(issuer_id, user_group_id, user_role_id, user_id, auditUserId, auditWorkstation),
                                                               ResponseType.SUCCESSFUL,
                                                               "",
                                                               "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<auditreport_usergroup_Result>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<auditreport_usergroup_Result>> GetUserGroup_AuditReport(int? issuer_id, int? user_group_id, int? user_role_id, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<auditreport_usergroup_Result>>(_reportService.GetUserGroup_AuditReport(issuer_id, user_group_id, user_role_id, auditUserId, auditWorkstation),
                                                               ResponseType.SUCCESSFUL,
                                                               "",
                                                               "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<auditreport_usergroup_Result>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<auditreport_usergroup_Result>> GetBranchesperusergroup_AuditReport(int? issuer_id, int? user_group_id, int? branch_id, int? role_id, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<auditreport_usergroup_Result>>(_reportService.GetBranchesperusergroup_AuditReport(issuer_id, user_group_id, branch_id, role_id, auditUserId, auditWorkstation),
                                                               ResponseType.SUCCESSFUL,
                                                               "",
                                                               "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<auditreport_usergroup_Result>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<issuedcardsreport_Result>> Getissuecardreport(int? user, int issuerID, int? languageid, DateTime dateFrom, DateTime dateTo,
                                           string status, int? branchid, long auditUserId, string auditWorkstation)
        {
            List<issuedcardsreport_Result> issuercardlist = new List<issuedcardsreport_Result>();
            try
            {
                issuercardlist = _reportService.Getissuecardreport(user, issuerID, languageid, dateFrom, dateTo, status, branchid, auditUserId, auditWorkstation);

                return new Response<List<issuedcardsreport_Result>>(issuercardlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<issuedcardsreport_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<Spoilcardsummaryreport_Result>> GetSpoilCardsummaryreport(int? issuerID, int? branchid, int? languageid, DateTime dateFrom, DateTime dateTo)
        {
            List<Spoilcardsummaryreport_Result> spoilcardlist = new List<Spoilcardsummaryreport_Result>();
            try
            {
                spoilcardlist = _reportService.GetSpoilCardsummaryreport(issuerID, branchid, languageid, dateFrom, dateTo);

                return new Response<List<Spoilcardsummaryreport_Result>>(spoilcardlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<Spoilcardsummaryreport_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<issuecardsummaryreport_Result>> Getissuecardsummaryreport(int? issuerID, int? branchid, int? languageid, DateTime dateFrom, DateTime dateTo)
        {
            List<issuecardsummaryreport_Result> issuercardlist = new List<issuecardsummaryreport_Result>();
            try
            {
                issuercardlist = _reportService.Getissuecardsummaryreport(issuerID, branchid, languageid, dateFrom, dateTo);

                return new Response<List<issuecardsummaryreport_Result>>(issuercardlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<issuecardsummaryreport_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<invetorysummaryreport_Result>> GetInventorySummaryReport(int issuerID, int? branchid, int LanguageId, long? audituserid, string workstation)
        {
            List<invetorysummaryreport_Result> cardlist = new List<invetorysummaryreport_Result>();
            try
            {
                cardlist = _reportService.GetInventorySummaryReport(issuerID, branchid, LanguageId, audituserid, workstation);

                return new Response<List<invetorysummaryreport_Result>>(cardlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<invetorysummaryreport_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<List<invetorysummaryreport_Result>> GetCardCenterInventorySummaryReport(int issuerID, int? branchid, int LanguageId, long audituserid, string workstation)
        {
            List<invetorysummaryreport_Result> cardlist = new List<invetorysummaryreport_Result>();
            try
            {
                cardlist = _reportService.GetCardCenterInventorySummaryReport(issuerID, branchid, LanguageId, audituserid, workstation);

                return new Response<List<invetorysummaryreport_Result>>(cardlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<invetorysummaryreport_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<List<burnrate_report_Result>> GetBurnRateReport(int issuerID, int? branchid, int LanguageId,long? audituserid,string workstation)
        {
            List<burnrate_report_Result> cardlist = new List<burnrate_report_Result>();
            try
            {
                cardlist = _reportService.GetBurnRateReport(issuerID, branchid, LanguageId,audituserid, workstation);

                return new Response<List<burnrate_report_Result>>(cardlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<burnrate_report_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<List<feerevenue_report_Result>> GetFeeRevenueReport(int? issuerID, int? branchid, int? languageid, DateTime dateFrom, DateTime dateTo, long? audituserid, string workstation)
        {
            List<feerevenue_report_Result> reportresult = new List<feerevenue_report_Result>();
            try
            {
                reportresult = _reportService.GetFeeRevenueReport(issuerID, branchid, languageid, dateFrom, dateTo, audituserid, workstation);

                return new Response<List<feerevenue_report_Result>>(reportresult, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<feerevenue_report_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<List<branchcardstock_report_Result>> GetBranchCardStockReport(int issuerID, int? brachcode, int LanguageId, long auditUserId, string auditWorkstation)
        {
            List<branchcardstock_report_Result> cardlist = new List<branchcardstock_report_Result>();
            try
            {
                cardlist = _reportService.GetBranchCardStockReport(issuerID, brachcode, LanguageId, auditUserId, auditWorkstation);

                return new Response<List<branchcardstock_report_Result>>(cardlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<branchcardstock_report_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<List<branchcardstock_report_Result>> GetCenterCardStockReport(int issuerID, int? brachcode, int LanguageId, long auditUserId, string auditWorkstation)
        {
            List<branchcardstock_report_Result> cardlist = new List<branchcardstock_report_Result>();
            try
            {
                cardlist = _reportService.GetCenterCardStockReport(issuerID, brachcode, LanguageId, auditUserId, auditWorkstation);

                return new Response<List<branchcardstock_report_Result>>(cardlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<branchcardstock_report_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<List<spolicardsreport_Result>> GetSpoilCardreport(int? issuerID, int? branchid, int? languageid, int? userid, DateTime dateFrom, DateTime dateTo, long auditUserId, string auditWorkstation)
        {
            List<spolicardsreport_Result> spoilcardlist = new List<spolicardsreport_Result>();
            try
            {
                spoilcardlist = _reportService.GetSpoilCardreport(issuerID, languageid, userid, branchid, dateFrom, dateTo, auditUserId, auditWorkstation);

                return new Response<List<spolicardsreport_Result>>(spoilcardlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<spolicardsreport_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<PINReissueReportResult>> GetPinReissueReport(int? user, int issuerID, int? languageid, DateTime dateFrom, DateTime dateTo,
                                          string status, int? branchid, long auditUserId, string auditWorkstation)
        {
            List<PINReissueReportResult> issuercardlist = new List<PINReissueReportResult>();
            try
            {
                issuercardlist = _reportService.GetPinReissueReport(user, issuerID, languageid, dateFrom, dateTo, status, branchid, auditUserId, auditWorkstation);

                return new Response<List<PINReissueReportResult>>(issuercardlist, ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<PINReissueReportResult>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        internal Response<List<PinMailerReprintReportResult>> GetPinMailerReprintReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId,long auditUserId,string auditWorkstation)
        {
            try
            {
                var result = _reportService.GetPinMailerReprintReport(issuerId, branchId, fromDate, toDate, languageId, auditUserId, auditWorkstation);

                return new Response<List<PinMailerReprintReportResult>>(result, ResponseType.SUCCESSFUL, "", "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<PinMailerReprintReportResult>>(null,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
    }
}