using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Veneka.Indigo.AuditManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.Common.Models;
using System.Data.Objects;
using Veneka.Indigo.AuditManagement;
using System.IO;
using iTextSharp.text.pdf;
using Veneka.Indigo.CardManagement;
using iTextSharp.text;
namespace Veneka.Indigo.AuditManagement.dal
{
    internal class AuditContolDal
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        internal void InsertAudit(long userid, int audit_action_id,string userAction, string description, string workstation,
                                  string dataBefore, string dataAfter, int issuerID)
        {
            try
            {
                using (SqlConnection con = _dbObject.SQLConnection)
                {
                    using (SqlCommand command = con.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "usp_insert_audit";
                        command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = userid;
                        command.Parameters.Add("@audit_action_id", SqlDbType.Int).Value = audit_action_id;
                        command.Parameters.Add("@audit_date", SqlDbType.DateTimeOffset).Value = null;
                        command.Parameters.Add("@workstation_address", SqlDbType.VarChar).Value = workstation;
                        command.Parameters.Add("@action_description", SqlDbType.VarChar).Value = description;
                        command.Parameters.Add("@data_changed", SqlDbType.VarChar).Value = null;
                        command.Parameters.Add("@data_before", SqlDbType.VarChar).Value = dataBefore;
                        command.Parameters.Add("@data_after", SqlDbType.VarChar).Value = dataAfter;
                        command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogFileWriter.WriteWebServerError(ToString(), ex);
                
                throw;
            }
        }

        /// <summary>
        /// Fetch audit Log
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
        internal List<GetAuditData_Result> GetAuditData(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
                                                        DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_auditdata((int?)auditAction, userRoleId, username, issuerId, dateFrom, dateTo, pageIndex,
                                                      rowsPerPage, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        private Audit CreateAuditObjectFromReader(SqlDataReader dataReader)
        {
            string auditDate = dataReader["audit_date"] != null ? dataReader["audit_date"].ToString() : null;
            string username = dataReader["user_audit"] != null ? dataReader["user_audit"].ToString() : null;
            string address = dataReader["workstation_address"] != null
                                 ? dataReader["workstation_address"].ToString()
                                 : null;
            string action = dataReader["user_action"] != null ? dataReader["user_action"].ToString() : null;
            string description = dataReader["action_description"] != null
                                     ? dataReader["action_description"].ToString()
                                     : null;
            //string issuerName = dataReader["Issuer"] != null ? dataReader["Issuer"].ToString() : null;
            string dataBefore = dataReader["data_before"] != null ? dataReader["data_before"].ToString() : null;
            string dataAfter = dataReader["data_after"] != null ? dataReader["data_after"].ToString() : null;
            string issuerID = dataReader["issuer_id"] != null ? dataReader["issuer_id"].ToString() : null;
            string id = dataReader["audit_id"] != null ? dataReader["audit_id"].ToString() : null;

            long auditNumber = 0;
            if (id != null)
                Int64.TryParse(id, out auditNumber);

            int idIssuer = 0;

            if (issuerID != null)
                Int32.TryParse(issuerID, out idIssuer);

            DateTime dateAudited = DateTime.Parse(auditDate);

            var audit = new Audit(auditNumber, username, address, action, description, dateAudited, dataBefore,
                                  dataAfter, dataAfter, idIssuer);

            return audit;
        }

        internal byte[] GenereateAuditPdfReport(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
                                                       DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, int languageId, long auditUserId, string auditWorkstation)
        {

            List<string[]> auditreport = GetAuditCSVReport(auditAction, userRoleId, username, dateFrom, dateTo, issuerId, pageIndex, rowsPerPage, languageId, auditUserId, auditWorkstation);

            return CreatePdfReport(auditreport);

        }
        private byte[] CreatePdfReport(List<string[]> auditreport)
        {
            return GeneratePDFReport(auditreport, new float[] { 500, 500, 500, 500, 500, 500, 500, 500, 500 });
        }
        internal List<string[]> GetAuditCSVReport(AuditActionType? auditAction, int? userRoleId, string username, DateTime dateFrom,
                                                        DateTime dateTo, int? issuerId, int pageIndex, int rowsPerPage, int languageId, long auditUserId, string auditWorkstation)
        {
            List<GetAuditData_Result> lstresult = new List<GetAuditData_Result>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_auditdata((int?)auditAction, userRoleId, username, issuerId, dateFrom, dateTo, pageIndex,
                                                      rowsPerPage, auditUserId, auditWorkstation);
                lstresult = results.ToList();
            }
            List<string[]> auditreport = new List<string[]>();
            ReportManagementService reportservice = new ReportManagementService();
            var reportresult = reportservice.GetReportFields(4, languageId);
            Dictionary<int, string> reportfields = new Dictionary<int, string>();

            int i = 0;
            foreach (var item in reportresult)
            {
                reportfields.Add(i, reportresult[i].language_text);
                i++;
            }
            //auditreport.Add(new[] { "TRANSACTION REFERENCE", "TRANSACTION description", "MAKER ID", "MAKER DATE", "MAKER TIME", "MAKER IP ADDRESS", "ACTIVITY", "OLD VALUE", "NEW VALUE" });

            auditreport.Add(new[] { reportfields[0], reportfields[1], reportfields[2], reportfields[3], reportfields[4], reportfields[5], reportfields[6], reportfields[7], reportfields[8] });

            foreach (var result in lstresult)
            {
                auditreport.Add(new[] { result.audit_id.ToString(), result.action_description, result.UserName, result.audit_date, result.audit_Time, result.workstation_address, result.audit_action_name, result.data_before, result.data_after });
            }
            return auditreport;

        }
        private byte[] GeneratePDFReport(List<string[]> TableData, float[] CardTable_colWidths)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                
                var document = new Document(PageSize.A4.Rotate());
                document.AddCreationDate();

                //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfFile, FileMode.Create));
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                writer.CompressionLevel = 9;

                document.Open();
                document.Add(CreateTable(TableData, CardTable_colWidths));
                document.Close();

                writer.Flush();

                return ms.ToArray();
            }

        }

        private static PdfPTable CreateTable(List<string[]> TableData, float[] colWidths)
        {
            string[] header = TableData[0];
            var table = new PdfPTable(header.Length);
            table.WidthPercentage = 100;
            table.SetWidths(colWidths);
            foreach (string column in header)
            {
                table.AddCell(new PdfPCell(new Phrase(column, new Font(Font.FontFamily.COURIER, 9, Font.BOLD))));
            }
            for (int i = 1; i < TableData.Count; i++)
            {
                foreach (string column1 in TableData[i])
                {
                    table.AddCell(new PdfPCell(new Phrase(column1, new Font(Font.FontFamily.COURIER, 8, Font.NORMAL))));
                }
            }
            return table;
        }

      
  
    }
}