using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Veneka.Indigo.AuditManagement.dal;
using Veneka.Indigo.AuditManagement.objects;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common;

namespace Veneka.Indigo.AuditManagement
{
    public class AuditReportService
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;
        private readonly AuditContolDal auditor = new AuditContolDal();

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
        public List<GetAuditData_Result> GetAudit(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
                                                        DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            return auditor.GetAuditData(auditAction, userRoleId, username, dateFrom, dateTo, issuerId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
        }

        public List<Audit> GetReportDataList(AuditSearchParameters searchParameters)
        {
            var audiList = new List<Audit>();
            try
            {
                using (SqlConnection con = _dbObject.SQLConnection)
                {
                    using (SqlCommand command = con.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "usp_generate_audit_report";
                        command.Parameters.Add("@User", SqlDbType.VarChar).Value = searchParameters.User;
                        command.Parameters.Add("@ActionType", SqlDbType.VarChar).Value = searchParameters.User;
                        command.Parameters.Add("@IssuerID", SqlDbType.VarChar).Value = searchParameters.User;
                        command.Parameters.Add("@DateFrom", SqlDbType.DateTime).Value = searchParameters.User;
                        command.Parameters.Add("@DateTo", SqlDbType.DateTime).Value = searchParameters.User;
                        command.Parameters.Add("@Keyword", SqlDbType.DateTime).Value = searchParameters.User;

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                Audit audit = CreateAudit(dataReader);
                                audiList.Add(audit);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogFileWriter.WriteWebServerError(ToString(), ex);
            }
            return audiList;
        }

        private Audit CreateAudit(SqlDataReader dataReader)
        {
            string auditID = dataReader["audit_id"] != null ? dataReader["audit_id"].ToString() : null;
            string userAudit = dataReader["user_audit"] != null ? dataReader["user_audit"].ToString() : null;
            string workstationAddress = dataReader["workstation_address"] != null
                                            ? dataReader["workstation_address"].ToString()
                                            : null;
            string userAction = dataReader["user_action"] != null ? dataReader["user_action"].ToString() : null;
            string actionDescription = dataReader["action_description"] != null
                                           ? dataReader["action_description"].ToString()
                                           : null;
            string auditDate = dataReader["audit_date"] != null ? dataReader["audit_date"].ToString() : null;
            string dataChanged = dataReader["data_changed"] != null ? dataReader["data_changed"].ToString() : null;
            string dataBefore = dataReader["data_before"] != null ? dataReader["data_before"].ToString() : null;
            string dataAfter = dataReader["data_after"] != null ? dataReader["data_after"].ToString() : null;
            string issuerID = dataReader["issuer_id"] != null ? dataReader["issuer_id"].ToString() : null;

            long idAudit = 0;
            Int64.TryParse(auditID, out idAudit);

            int issuerIntID = 0;
            Int32.TryParse(issuerID, out issuerIntID);

            DateTime dateAudited = DateTime.MinValue;
            if (auditDate != null)
                dateAudited = DateTime.Parse(auditDate);

            var audit = new Audit(idAudit, userAudit, workstationAddress, userAction, actionDescription, dateAudited,
                                  dataChanged, dataBefore, dataAfter, issuerIntID);

            return audit;
        }

        public byte[] GenereateAuditPdfReport(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
                                                      DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, int languageid, long auditUserId, string auditWorkstation)
        {

            return auditor.GenereateAuditPdfReport(auditAction, userRoleId, username, dateFrom, dateTo, issuerId, pageIndex, rowsPerPage, languageid, auditUserId, auditWorkstation);
        }
        public List<string[]> GetAuditCSVReport(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
                                                      DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, int languageid, long auditUserId, string auditWorkstation)
        {

            return auditor.GetAuditCSVReport(auditAction, userRoleId, username, dateFrom, dateTo, issuerId, pageIndex, rowsPerPage, languageid, auditUserId, auditWorkstation);
        }


        public void InsertAudit(long userid, int audit_action_id, string userAction, string description, string workstation,
                                  string dataBefore, string dataAfter, int issuerID)
        {

             auditor.InsertAudit( userid,  audit_action_id,  userAction,  description,  workstation,
                                   dataBefore,  dataAfter,  issuerID);
        }
    }
}