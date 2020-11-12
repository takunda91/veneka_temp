using System;
using System.Collections.Generic;
using Veneka.Indigo.AuditManagement;
using Veneka.Indigo.Common.Models;
using IndigoCardIssuanceService.DataContracts;
using Common.Logging;
using Veneka.Indigo.Common;

namespace IndigoCardIssuanceService.bll
{
    internal class AuditServiceContoller
    {
        private readonly AuditReportService auditService = new AuditReportService();
        private static readonly ILog log = LogManager.GetLogger(typeof(AuditServiceContoller));

        /// <summary>
        /// Fetch audit log
        /// </summary>
        /// <param name="auditAction"></param>
        /// <param name="userRoleId"></param>
        /// <param name="username"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="issuerId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public Response<List<GetAuditData_Result>> GetAuditData(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
                                                                DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<GetAuditData_Result>>(auditService.GetAudit(auditAction, userRoleId, username, dateFrom, dateTo, issuerId, pageIndex, rowsPerPage, auditUserId, auditWorkstation), 
                                                               ResponseType.SUCCESSFUL,
                                                               "",
                                                               "");            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<GetAuditData_Result>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }            
        }

        public Response<byte[]> GenereateAuditPdfReport(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
                                                              DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, int languageid, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<byte[]>(auditService.GenereateAuditPdfReport(auditAction, userRoleId, username, dateFrom, dateTo, issuerId, pageIndex, rowsPerPage, languageid, auditUserId, auditWorkstation),
                                                               ResponseType.SUCCESSFUL,
                                                               "",
                                                               "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<byte[]>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        public Response<List<string[]>> GetAuditCSVReport(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
                                                             DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, int languageid, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<string[]>>(auditService.GetAuditCSVReport(auditAction, userRoleId, username, dateFrom, dateTo, issuerId, pageIndex, rowsPerPage, languageid, auditUserId, auditWorkstation),
                                                               ResponseType.SUCCESSFUL,
                                                               "",
                                                               "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<string[]>>(null,
                                                               ResponseType.ERROR,
                                                               "Error when processing request.",
                                                               log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        public BaseResponse InsertAudit(long userid, int audit_action_id,  string description, string workstation,
                                   int issuerID)
        {
            try
            {
                auditService.InsertAudit(userid, audit_action_id, description,  description, workstation,null,null, issuerID);
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                                     "Action Was Successful.",
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