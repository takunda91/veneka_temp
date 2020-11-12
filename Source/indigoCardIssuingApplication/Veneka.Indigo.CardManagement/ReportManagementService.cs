using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.CardManagement.dal;
using Common.Logging;

namespace Veneka.Indigo.CardManagement
{
   public class ReportManagementService
    {
       private readonly ReportManagementDAL _reportDAL = new ReportManagementDAL();
       private static readonly ILog log = LogManager.GetLogger(typeof(ReportManagementService));

       /// <summary>
       /// PDF Report Header
       /// </summary>
       /// <param name="reportid"></param>
       /// <param name="languageId"></param>
       /// <returns></returns>
       public List<report_fields_Result> GetReportFields(int reportid, int languageId)
       {
           List<report_fields_Result> rtnValue = new List<report_fields_Result>();
           try
           {
               rtnValue= _reportDAL.GetReportFields(reportid, languageId);
           }
           catch (Exception ex)
           {
               log.Error(ex);

               
           }
           return rtnValue;
       }   

       /// <summary>
       /// SSRS: Branch Order Report
       /// </summary>
       /// <param name="issuerId"></param>
       /// <param name="branchId"></param>
       /// <param name="fromDate"></param>
       /// <param name="toDate"></param>
       /// <param name="languageId"></param>
       /// <returns></returns>
       public List<BranchOrderReportResult> GetBranchOrderReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId)
       {
           return _reportDAL.GetBranchOrderReport(issuerId, branchId, fromDate, toDate, languageId);
       }

       /// <summary>
       /// SSRS: Card Production Report
       /// </summary>
       /// <param name="issuerId"></param>
       /// <param name="branchId"></param>
       /// <param name="fromDate"></param>
       /// <param name="toDate"></param>
       /// <param name="languageId"></param>
       /// <returns></returns>
       public List<CardProductionReportResult> GetCardProductionReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId, long auditUserId, string auditWorkstation)
       {
           return _reportDAL.GetCardProductionReport(issuerId, branchId, fromDate, toDate, languageId, auditUserId, auditWorkstation);
       }

       /// <summary>
       /// SSRS: Card Dispatch Report
       /// </summary>
       /// <param name="issuerId"></param>
       /// <param name="branchId"></param>
       /// <param name="fromDate"></param>
       /// <param name="toDate"></param>
       /// <param name="languageId"></param>
       /// <returns></returns>
       public List<CardDispatchReportResult> GetCardDispatchReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId, long auditUserId, string auditWorkstation)
       {
           return _reportDAL.GetCardDispatchReport(issuerId, branchId, fromDate, toDate, languageId, auditUserId, auditWorkstation);
       }

       /// <summary>
       /// SSRS: Pin Mailer Report
       /// </summary>
       /// <param name="issuerId"></param>
       /// <param name="branchId"></param>
       /// <param name="fromDate"></param>
       /// <param name="toDate"></param>
       /// <param name="languageId"></param>
       /// <returns></returns>
       public List<PinMailerReportResult> GetPinMailerReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId)
       {
           return _reportDAL.GetPinMailerReport(issuerId, branchId, fromDate, toDate, languageId);
       }

       /// <summary>
       /// SSRS: Card Expiry Report
       /// </summary>
       /// <param name="issuerId"></param>
       /// <param name="branchId"></param>
       /// <param name="fromDate"></param>
       /// <param name="languageId"></param>
       /// <returns></returns>
       public List<CardExpiryReportResult> GetCardExpiryReport(int? issuerId, int? branchId, DateTime fromDate, int languageId, long auditUserId, string auditWorkstation)
       {
           return _reportDAL.GetCardExpiryReport(issuerId, branchId, fromDate, languageId, auditUserId, auditWorkstation);
       }

       public List<issuedcardsreport_Result> Getissuecardreport(int? user, int issuerID, int? languageid, DateTime dateFrom, DateTime dateTo,
                                           string status, int? branchid, long auditUserId, string auditWorkstation)
       {
           List<issuedcardsreport_Result> issuercardlist = null;

           issuercardlist = _reportDAL.Getissuecardreport(user, issuerID, languageid, dateFrom, dateTo, status, branchid, auditUserId, auditWorkstation);

           return issuercardlist;
       }

       public List<issuecardsummaryreport_Result> Getissuecardsummaryreport(int? issuerID, int? branchid, int? languageid, DateTime dateFrom, DateTime dateTo)
       {
           List<issuecardsummaryreport_Result> issuercardlist = null;

           issuercardlist = _reportDAL.Getissuecardsummaryreport(issuerID, branchid, languageid, dateFrom, dateTo);

           return issuercardlist;
       }

       public List<Spoilcardsummaryreport_Result> GetSpoilCardsummaryreport(int? issuerID, int? branchid, int? languageid, DateTime dateFrom, DateTime dateTo)
       {

           List<Spoilcardsummaryreport_Result> issuercardlist = null;

           issuercardlist = _reportDAL.GetSpoilCardsummaryreport(issuerID, branchid, languageid, dateFrom, dateTo);

           return issuercardlist;

       }

       public List<spolicardsreport_Result> GetSpoilCardreport(int? issuerID, int? branchid, int? languageid, int? userid, DateTime dateFrom, DateTime dateTo, long auditUserId, string auditWorkstation)
       {
           List<spolicardsreport_Result> issuercardlist = null;

           issuercardlist = _reportDAL.GetSpoilCardreport(issuerID, languageid, userid, branchid, dateFrom, dateTo, auditUserId, auditWorkstation);

           return issuercardlist;
       }
       public List<invetorysummaryreport_Result> GetInventorySummaryReport(int issuerID, int? branchid, int LanguageId, long? audituserid, string workstation)
       {
           List<invetorysummaryreport_Result> issuercardlist = null;

           issuercardlist = _reportDAL.GetInventorySummaryReport(issuerID, branchid, LanguageId, audituserid, workstation);

           return issuercardlist;
       }
       public List<invetorysummaryreport_Result> GetCardCenterInventorySummaryReport(int issuerID, int? branchid, int LanguageId, long? audituserid, string workstation)
       {
           List<invetorysummaryreport_Result> issuercardlist = null;

           issuercardlist = _reportDAL.GetCardCenterInventorySummaryReport(issuerID, branchid, LanguageId, audituserid, workstation);

           return issuercardlist;
       }
       public List<burnrate_report_Result> GetBurnRateReport(int issuerID, int? branchid, int LanguageId, long? audituserid, string workstation)
       {
           List<burnrate_report_Result> issuercardlist = null;

           issuercardlist = _reportDAL.GetBurnRateReport(issuerID, branchid, LanguageId, audituserid, workstation);

           return issuercardlist;
       }

       public List<feerevenue_report_Result> GetFeeRevenueReport(int? issuerID, int? branchid, int? languageid, DateTime dateFrom, DateTime dateTo, long? audituserid, string workstation)
       {
           List<feerevenue_report_Result> issuercardlist = null;

           issuercardlist = _reportDAL.GetFeeRevenueReport(issuerID, branchid, languageid, dateFrom, dateTo, audituserid, workstation);

           return issuercardlist;
       }

       public List<branchcardstock_report_Result> GetBranchCardStockReport(int issuerID, int? branchid, int LanguageId, long auditUserId, string auditWorkstation)
       {
           List<branchcardstock_report_Result> issuercardlist = null;

           issuercardlist = _reportDAL.GetBranchCardStockReport(issuerID, branchid, LanguageId, auditUserId, auditWorkstation);

           return issuercardlist;
       }
       public List<branchcardstock_report_Result> GetCenterCardStockReport(int issuerID, int? branchid, int LanguageId, long auditUserId, string auditWorkstation)
       {
           List<branchcardstock_report_Result> issuercardlist = null;

           issuercardlist = _reportDAL.GetCenterCardStockReport(issuerID, branchid, LanguageId, auditUserId, auditWorkstation);

           return issuercardlist;
       }
       public List<auditreport_usergroup_Result> GetUsersByRoles_AuditReport(int? issuer_id, int? user_group_id, int? user_role_id, int? user_id, long auditUserId, string auditWorkstation)
       {
           return _reportDAL.GetUsersByRoles_AuditReport(issuer_id, user_group_id, user_role_id, user_id, auditUserId, auditWorkstation);
       }

       public List<auditreport_usergroup_Result> GetUserGroup_AuditReport(int? issuer_id, int? user_group_id, int? user_role_id, long auditUserId, string auditWorkstation)
       {
           return _reportDAL.GetUserGroup_AuditReport(issuer_id, user_group_id, user_role_id, auditUserId, auditWorkstation);

       }

       public List<auditreport_usergroup_Result> GetBranchesperusergroup_AuditReport(int? issuer_id, int? user_group_id, int? branch_id, int? role_id, long auditUserId, string auditWorkstation)
       {
           return _reportDAL.GetBranchesperusergroup_AuditReport(issuer_id, user_group_id, branch_id, role_id, auditUserId, auditWorkstation);

       }

       public List<PINReissueReportResult> GetPinReissueReport(int? userid, int issuerID, int? languageid, DateTime dateFrom, DateTime dateTo,
                                         string status, int? branchid, long auditUserId, string auditWorkstation)
       {
           List<PINReissueReportResult> issuercardlist = null;

           issuercardlist = _reportDAL.GetPinReissueReport(userid, issuerID, languageid, dateFrom, dateTo, status, branchid, auditUserId, auditWorkstation);

           return issuercardlist;
       }

       public List<PinMailerReprintReportResult> GetPinMailerReprintReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId,long auditUserId,string auditWorkstation)
       {
           return _reportDAL.GetPinMailerReprintReport(issuerId, branchId, fromDate, toDate, languageId, auditUserId, auditWorkstation);
       }
    }
}
