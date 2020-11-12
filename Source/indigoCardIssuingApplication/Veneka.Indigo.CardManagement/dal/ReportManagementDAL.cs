using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;
using System.Data.Objects;

namespace Veneka.Indigo.CardManagement.dal
{
    internal class ReportManagementDAL
    {        
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportManagementDAL));

        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        /// <summary>
        /// PDF Reports: Get heading names from database
        /// </summary>
        /// <param name="reportid"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        internal List<report_fields_Result> GetReportFields(int reportid, int languageId)
        {
            List<report_fields_Result> rtnValue = new List<report_fields_Result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<report_fields_Result> results = context.usp_get_report_fields(reportid, languageId);

                foreach (report_fields_Result result in results)
                {
                    rtnValue.Add(result);
                }
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
        internal List<BranchOrderReportResult> GetBranchOrderReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId)
        {
            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<BranchOrderReportResult> results = context.usp_branch_order_report(issuerId, branchId, fromDate, toDate);

                return results.ToList();
            }
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
        internal List<CardProductionReportResult> GetCardProductionReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<CardProductionReportResult> results = context.usp_card_production_report(issuerId, branchId, fromDate, toDate, auditUserId, auditWorkstation);

                return results.ToList();
            }
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
        internal List<CardDispatchReportResult> GetCardDispatchReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<CardDispatchReportResult> results = context.usp_card_dispatch_report(issuerId, branchId, fromDate, toDate, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// SSRS: Pin Mailer Printing Report
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="branchId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        internal List<PinMailerReportResult> GetPinMailerReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId)
        {
            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<PinMailerReportResult> results = context.usp_pin_mailer_report(issuerId, branchId, fromDate, toDate);

                return results.ToList();
            }
        }

        /// <summary>
        /// SSRS: Card Expiry Reportz
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="branchId"></param>
        /// <param name="fromDate"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        internal List<CardExpiryReportResult> GetCardExpiryReport(int? issuerId, int? branchId, DateTime fromDate, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<CardExpiryReportResult> results = context.usp_card_expiry_report(issuerId, branchId, fromDate, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="issuerID"></param>
        /// <param name="languageid"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="status"></param>
        /// <param name="branchcode"></param>
        /// <returns></returns>
        internal List<issuedcardsreport_Result> Getissuecardreport(int? userid, int issuerID, int? languageid, DateTime dateFrom, DateTime dateTo,
                                               string status, int? branchid, long auditUserId, string auditWorkstation)
        {
         
            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<issuedcardsreport_Result> results = context.usp_get_issuedcardsreport(issuerID, dateFrom, dateTo, userid, branchid, languageid, auditUserId, auditWorkstation);

                return results.ToList();
                
            }           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="branchid"></param>
        /// <param name="languageid"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        internal List<issuecardsummaryreport_Result> Getissuecardsummaryreport(int? issuerID, int? branchid, int? languageid, DateTime dateFrom, DateTime dateTo)
        {


            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<issuecardsummaryreport_Result> results = context.usp_get_issuecardsummaryreport(branchid, issuerID, languageid, dateFrom, dateTo);


                return results.ToList();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="branchid"></param>
        /// <param name="languageid"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        internal List<Spoilcardsummaryreport_Result> GetSpoilCardsummaryreport(int? issuerID, int? branchid, int? languageid, DateTime dateFrom, DateTime dateTo)
        {

            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<Spoilcardsummaryreport_Result> results = context.usp_get_Spoilcardsummaryreport(branchid, issuerID, languageid, dateFrom, dateTo);


                return results.ToList();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="branchid"></param>
        /// <param name="languageid"></param>
        /// <param name="userid"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        internal List<spolicardsreport_Result> GetSpoilCardreport(int? issuerID, int? branchid, int? languageid, int? userid, DateTime dateFrom, DateTime dateTo, long auditUserId, string auditWorkstation)
        {


            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<spolicardsreport_Result> results = context.usp_get_spoilcardsreport(issuerID, languageid, userid, branchid, dateFrom, dateTo, auditUserId, auditWorkstation);


                return results.ToList();

            }

        }


        internal List<feerevenue_report_Result> GetFeeRevenueReport(int? issuerID, int? branchid, int? languageid, DateTime dateFrom, DateTime dateTo, long? audituserid, string workstation)
        {


            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<feerevenue_report_Result> results = context.usp_get_feerevenue_report(branchid, issuerID, languageid, dateFrom, dateTo, audituserid, workstation);


                return results.ToList();

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="branchid"></param>
        /// <param name="LanguageId"></param>
        /// <returns></returns>
        internal List<invetorysummaryreport_Result> GetInventorySummaryReport(int issuerID, int? branchid, int LanguageId, long? audituserid, string workstation)
        {



            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<invetorysummaryreport_Result> results = context.usp_get_invetorysummaryreport(branchid, issuerID, LanguageId, audituserid, workstation);

                return results.ToList();
              
            }
           
        }

        internal List<invetorysummaryreport_Result> GetCardCenterInventorySummaryReport(int issuerID, int? branchid, int LanguageId, long? audituserid, string workstation)
        {

           

            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<invetorysummaryreport_Result> results = context.usp_get_cardinventorysummaryreport(branchid, issuerID, LanguageId, audituserid, workstation);


                return results.ToList();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="branchid"></param>
        /// <param name="LanguageId"></param>
        /// <returns></returns>
        internal List<burnrate_report_Result> GetBurnRateReport(int issuerID, int? branchid, int LanguageId, long? audituserid, string workstation)
        {


            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<burnrate_report_Result> results = context.usp_get_burnrate_report(branchid, issuerID, LanguageId, DateTime.Now, audituserid, workstation);


                return results.ToList();
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="branchid"></param>
        /// <param name="LanguageId"></param>
        /// <returns></returns>
        internal List<branchcardstock_report_Result> GetBranchCardStockReport(int issuerID, int? branchid, int LanguageId, long auditUserId, string auditWorkstation)
        {


            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<branchcardstock_report_Result> results = context.usp_get_branchcardstock_report(branchid, issuerID, LanguageId, auditUserId, auditWorkstation);


                return results.ToList();
            }
        }

        internal List<branchcardstock_report_Result> GetCenterCardStockReport(int issuerID, int? branchid, int LanguageId, long auditUserId, string auditWorkstation)
        {


            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<branchcardstock_report_Result> results = context.usp_get_centercardstock_report(branchid, issuerID, LanguageId, auditUserId, auditWorkstation);


                return results.ToList();

            }
        }

        internal List<auditreport_usergroup_Result> GetUsersByRoles_AuditReport(int? issuer_id, int? user_group_id, int? user_role_id, int? user_id, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                var results = context.usp_get_userbyroles_auditreport(issuer_id, user_group_id, user_role_id, user_id, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        internal List<auditreport_usergroup_Result> GetUserGroup_AuditReport(int? issuer_id, int? user_group_id, int? user_role_id, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                var results = context.usp_get_usergroup_auditreport(issuer_id, user_group_id, user_role_id, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        internal List<auditreport_usergroup_Result> GetBranchesperusergroup_AuditReport(int? issuer_id, int? user_group_id, int? branch_id, int? role_id, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                var results = context.usp_get_brachesperusergroup_auditreport(issuer_id, user_group_id, branch_id, role_id, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        internal List<PINReissueReportResult> GetPinReissueReport(int? userid, int issuerID, int? languageid, DateTime dateFrom, DateTime dateTo,
                                            string status, int? branchid, long auditUserId, string auditWorkstation)
        {



            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<PINReissueReportResult> results = context.usp_get_pinreissue_report(issuerID, dateFrom, dateTo, userid, branchid, languageid, auditUserId, auditWorkstation);


                return results.ToList();
            }


        }
        
        internal List<PinMailerReprintReportResult> GetPinMailerReprintReport(int? issuerId, int? branchId, DateTime fromDate, DateTime toDate, int languageId, long auditUserId, string auditWorkstation)
        {



            using (var context = new indigo_database_ReportsEntities(_dbObject.EFReportsSQLConnectionString))
            {
                ObjectResult<PinMailerReprintReportResult> results = context.usp_pin_mailer_reprint_report(issuerId, branchId, fromDate, toDate, (int)auditUserId, auditWorkstation);


                return results.ToList();
            }


        }
    }
}
