using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Web;
using System.IO;
using System.Text;
using System.Reflection;
using System.Web.Security;
using System.Security.Permissions;
using indigoCardIssuingWeb.SearchParameters;

namespace indigoCardIssuingWeb.webpages.audit
{
    public partial class AuditLogView : ListPage
    {
        //Standardise look and feel of the Website across all Web Browsers
        private AudiControllService _auditService = new AudiControllService();
        private static readonly ILog log = LogManager.GetLogger(typeof(AuditLogView));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.AUDITOR };
        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            try
            {
                if (SessionWrapper.AuditSearchResult != null && SessionWrapper.AuditSearch != null)
                {
                    var results = SessionWrapper.AuditSearchResult.ToArray();
                    SearchParameters = SessionWrapper.AuditSearch;

                    DisplayResults(SearchParameters, SearchParameters.PageIndex, results);

                    // SessionWrapper.DistBatchSearchParams = null;
                    SessionWrapper.AuditSearchResult = null;
                }
                else if (SessionWrapper.AuditSearch != null)
                {
                    AuditSearch searchParms = SessionWrapper.AuditSearch;
                    SearchParameters = searchParms;

                    DisplayResults(searchParms, searchParms.PageIndex, null);

                    // SessionWrapper.DistBatchSearchParams = null;
                }
                else
                {
                    // DisplayResults(null,1,null);
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        #endregion

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {            
            this.lblErrorMessage.Text = String.Empty;
            this.dlAuditList.DataSource = null;

            try
            {
                var auditSearchParms = (AuditSearch)parms;
                auditSearchParms.PageIndex = pageIndex;
                SearchParameters = auditSearchParms;                

                if (results == null)
                {
                    results = _auditService.GetAuditResults(auditSearchParms, pageIndex).ToArray();
                }

                if (results.Length > 0)
                {
                    this.dlAuditList.DataSource = results;
                    pnlpage.Visible = true;
                    TotalPages = ((GetAuditData_Result)results[0]).TOTAL_PAGES;
                    btnExportToCSV.Enabled = true;
                    btnExportToPdf.Enabled = true;
                }
                else
                {
                    TotalPages = 0;
                    pnlpage.Visible = false;
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                    btnExportToCSV.Enabled = false;
                    btnExportToPdf.Enabled = false;
                }

                this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
                this.dlAuditList.DataBind();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            SessionWrapper.AuditSearch = (AuditSearch)SearchParameters;
            Server.Transfer("~\\webpages\\audit\\AuditLogSelectionForm.aspx");
        }        
        #endregion

        #region  Events
        [PrincipalPermission(SecurityAction.Demand, Role = "AUDITOR")]
        protected void btnExportToPdf_Click(object sender, EventArgs e)
        {
            try
            {
                int totalrows = TotalPages.Value * 20;
                SearchParameters.RowsPerPage = totalrows;
                var reportBytes = _auditService.GenereateAuditReport((AuditSearch)SearchParameters, 1);

                string reportName = "Audit_Report_" +
                                        DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";
                MemoryStream ms = new MemoryStream(reportBytes);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "AUDITOR")]
        protected void btnExportToCSV_Click(object sender, EventArgs e)
        {
            try
            {

                int totalrows = TotalPages.Value * 20;
                SearchParameters.RowsPerPage = totalrows;
                var results = _auditService.GetAuditCSVReport((AuditSearch)SearchParameters, 1);

                string reportName = "Audit_Report_" +
                                          DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".csv";

                string csv = GetCSV(results);
                ExportCSV(csv, reportName);
            }
            catch (Exception ex) { log.Error(ex); }
        }


        public void ExportCSV<T>(List<String[]> list, string filename)
        {
            try
            {
                string csv = GetCSV(list);
                ExportCSV(csv, filename);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        public void ExportCSV(string csv, string filename)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", filename));
            HttpContext.Current.Response.ContentType = "text/csv";
            HttpContext.Current.Response.AddHeader("Pragma", "public");
            HttpContext.Current.Response.Write(csv);
            HttpContext.Current.Response.End();
        }
        public string GetCSV(List<String[]> list)
        {
            StringBuilder sb = new StringBuilder();

            string[] header = list[0];

            for (int i = 0; i <= header.Length - 1; i++)
            {
                sb.Append(header[i]);

                if (i < header.Length - 1)
                {
                    sb.Append(",");
                }
            }

            sb.AppendLine();

            //Loop through the collection, then the properties and add the values
            for (int i = 1; i < list.Count; i++)
            {

                for (int j = 0; j < list[i].Length; j++)
                {

                    string value = list[i][j];
                    if (!string.IsNullOrEmpty(value))
                    {
                        //Check if the value contans a comma and place it in quotes if so
                        if (value.Contains(","))
                        {
                            value = string.Concat("\"", value, "\"");
                        }

                        //Replace any \r or \n special characters from a new line with a space
                        if (value.Contains("\r"))
                        {
                            value = value.Replace("\r", " ");
                        }
                        if (value.Contains("\n"))
                        {
                            value = value.Replace("\n", " ");
                        }

                        sb.Append(value);
                    }
                    else
                    {
                        sb.Append(string.Empty);
                    }
                    if (j < header.Length - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.AppendLine();
            }




            return sb.ToString();
        }
        public string GetCSV<T>(List<T> list)
        {
            StringBuilder sb = new StringBuilder();

            //Get the properties for type T for the headers
            PropertyInfo[] propInfos = typeof(T).GetProperties();
            for (int i = 0; i <= propInfos.Length - 1; i++)
            {
                sb.Append(propInfos[i].Name);

                if (i < propInfos.Length - 1)
                {
                    sb.Append(",");
                }
            }

            sb.AppendLine();

            //Loop through the collection, then the properties and add the values
            for (int i = 0; i <= list.Count - 1; i++)
            {
                T item = list[i];
                for (int j = 0; j <= propInfos.Length - 1; j++)
                {
                    object o = item.GetType().GetProperty(propInfos[j].Name).GetValue(item, null);
                    if (o != null)
                    {
                        string value = o.ToString();

                        //Check if the value contans a comma and place it in quotes if so
                        if (value.Contains(","))
                        {
                            value = string.Concat("\"", value, "\"");
                        }

                        //Replace any \r or \n special characters from a new line with a space
                        if (value.Contains("\r"))
                        {
                            value = value.Replace("\r", " ");
                        }
                        if (value.Contains("\n"))
                        {
                            value = value.Replace("\n", " ");
                        }

                        sb.Append(value);
                    }

                    if (j < propInfos.Length - 1)
                    {
                        sb.Append(",");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
        #endregion        
    }
}