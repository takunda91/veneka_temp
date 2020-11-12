using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace indigoCardIssuingWeb.Reporting.CardCenter
{
    public partial class BillingReport : BasePage
    {

        private readonly SystemAdminService sysAdminService = new SystemAdminService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_OPERATOR,
                                                                        UserRole.CENTER_MANAGER,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.AUDITOR};
      
        private static readonly ILog log = LogManager.GetLogger(typeof(BillingReport));
        #region PRIVATE METHODS

        private void GenerateErrorMessage(string strResponse)
        {
            log.Error(strResponse);
            this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
            if (log.IsDebugEnabled || log.IsTraceEnabled)
            {
                this.lblErrorMessage.Text = strResponse;
            }
            lblErrorMessage.ForeColor = Color.Red;

        }

        private void ClearErrorMessage()
        {
            lblErrorMessage.ForeColor = Color.Blue;
            lblErrorMessage.Text = "";
        }

        #endregion

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {

                InitializePage();


            }
        }
        public static DataTable ToDataTable<BillingReportResult>(List<BillingReportResult> items)
        {
            DataTable dataTable = new DataTable(typeof(BillingReportResult).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(BillingReportResult).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (var item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        private void GenerateReport()
        {
            try
            {
                List<BillingReportResult> _result = sysAdminService.GetBillingReport(null, ddlMonth.SelectedValue, ddlyear.SelectedValue);

                GridView gvExp = new GridView();
                gvExp.AutoGenerateColumns = false;
                DataTable _resultdata = ToDataTable(_result);

                string reportName = "Billing_Report_" +
                                       DateTime.Now.ToString("ddd_dd_MMMM_yyyy_ssss") + ".csv";
                StringBuilder sb = new StringBuilder();

                IEnumerable<string> columnNames = _resultdata.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in _resultdata.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

                //Response.Clear();
                //Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                //Response.AddHeader("Content-Type", "application/vnd.ms-excel");


                //StringWriter tw = new StringWriter();
                //HtmlTextWriter hw = new HtmlTextWriter(tw);

                //gvExp.RenderControl(hw);
                //Response.Write(tw.ToString());
                //Response.End();
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition",
                    "attachment;filename="+ reportName);
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();

               
            }
            catch (Exception ex)
            {
                log.Debug(ex);
                throw ex;
            }

        }


        //public void ExportToExcel(List<BillingReportResult> records)
        //{
        //    // Load Excel application
        //    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

        //    // Create empty workbook
        //    excel.Workbooks.Add();

        //    // Create Worksheet from active sheet
        //    Microsoft.Office.Interop.Excel._Worksheet workSheet = new ;

        //    // I created Application and Worksheet objects before try/catch,
        //    // so that i can close them in finnaly block.
        //    // It's IMPORTANT to release these COM objects!!
        //    try
        //    {
        //        // ------------------------------------------------
        //        // Creation of header cells
        //        // ------------------------------------------------
        //        workSheet.Cells[1, "A"] = "Issued Year";
        //        workSheet.Cells[1, "B"] = "Issued Month";
        //        workSheet.Cells[1, "C"] = "Branch";
        //        workSheet.Cells[1, "D"] = "Date Issued";
        //        workSheet.Cells[1, "E"] = "Last 4 Digits";
        //        workSheet.Cells[1, "F"] = "Account Number";
        //        workSheet.Cells[1, "G"] = "Customer First Name";
        //        workSheet.Cells[1, "H"] = "Customer Middle Name";

        //        workSheet.Cells[1, "I"] = "Customer Last Name";
        //        workSheet.Cells[1, "J"] = "Is Currently in Issued Status?";
        //        // ------------------------------------------------
        //        // Populate sheet with some real data from "cars" list
        //        // ------------------------------------------------
        //        int row = 2; // start row (in row 1 are header cells)
        //        foreach (BillingReportResult item in records)
        //        {



        //            workSheet.Cells[row, "A"] = item.issued_year;
        //            workSheet.Cells[row, "B"] = item.issued_month;
        //            workSheet.Cells[row, "C"] = item.branch;
        //            workSheet.Cells[row, "D"] = item.date_issued;
        //            workSheet.Cells[row, "E"] = item.last_4_digits;
        //            workSheet.Cells[row, "F"] = item.account_number.ToString();
        //            workSheet.Cells[row, "G"] = item.customer_first_name;
        //            workSheet.Cells[row, "H"] = item.customer_middle_name;
        //            workSheet.Cells[row, "I"] = item.customer_last_name;
        //            workSheet.Cells[row, "J"] = item.currently_in_issued_status__==true?"YES":"NO";
        //            row++;
        //        }

        //        // Apply some predefined styles for data to look nicely :)
        //        workSheet.Range["A1"].AutoFormat(Microsoft.Office.Interop.Excel.XlRangeAutoFormat.xlRangeAutoFormatClassic1);

        //        // Define filename
        //          

        //        // Save this data as a file
        //        workSheet.SaveAs(reportName);

        //        //Response.Clear();
        //        //Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
        //        //Response.AddHeader("Content-Type", "application/vnd.ms-excel");

        //        //Response.BinaryWrite();
        //        //Response.End();

        //        // Display SUCCESS message
        //    }
        //    catch (Exception exception)
        //    {
        //        log.Debug(exception);
        //    }
        //    finally
        //    {
        //        // Quit Excel application
        //        excel.Quit();

        //        // Release COM objects (very important!)
        //        //if (excel != null)
        //        //    System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);

        //        //if (workSheet != null)
        //        //    System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet);

        //        // Empty variables
        //        excel = null;
        //        workSheet = null;

        //        // Force garbage collector cleaning
        //        GC.Collect();
        //    }
        //}


        private void DownloadFile(string path, string FileName)
        {

            FileInfo file = new FileInfo(path);
            string Outgoingfile = FileName + ".xlsx";
            if (file.Exists)
            {
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.WriteFile(file.FullName);

            }
            else
            {
                Response.Write("This file does not exist.");
            }

        }
        private void InitializePage()
        {
            try
            {

                List<UserRole> currentUserRoles = new List<UserRole>();

                //bool hasAccess = false;

                Dictionary<int, ListItem> issuerList = new Dictionary<int, ListItem>();
                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerList))
                {
                    foreach (UserRole userRole in userRolesForPage)
                    {
                        List<RolesIssuerResult> issuers;
                        if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(userRole, out issuers))
                        {
                            //hasAccess = true;
                            currentUserRoles.Add(userRole);


                        }
                    }
                    //this.ddlissuerlist.Items.AddRange(issuerList.Values.OrderBy(m => m.Text).ToArray());

                    //if (ddlissuerlist.Items.FindByValue("-1") != null)
                    //{
                    //    ddlissuerlist.Items.RemoveAt(ddlissuerlist.Items.IndexOf(ddlissuerlist.Items.FindByValue("-1")));
                    //}
                    //if (ddlissuerlist.Items.Count > 1)
                    //{
                    //    ddlissuerlist.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    //}
                    //this.ddlissuerlist.SelectedIndex = 0;

                    //this.ddlissuerlist.Visible = true;

                    for (int i = 1; i <= 12; i++)
                    {
                        DateTime date = new DateTime(1900, i, 1);
                        ddlMonth.Items.Add(new ListItem(date.ToString("MMMM"), i.ToString()));
                    }
                    ddlMonth.SelectedValue = DateTime.Today.Month.ToString();


                    var nowY = DateTime.Now.Year;


                    for (var Y = nowY; Y >= 2018; Y--)
                    {
                        ddlyear.Items.Add(new ListItem(Y.ToString(), Y.ToString()));
                    }

                    ddlyear.SelectedValue = DateTime.Today.Year.ToString();


                }
                else
                {
                    this.pnlDisable.Visible = false;
                    // this.pnlButtons.Visible = false;
                    if (!log.IsTraceEnabled)
                    {
                        log.Warn("A user tried to access a page that he/she does not have access to.");
                    }
                    log.Trace(m => m(String.Format("User {0} tried to access a page he/she does not have access to.", User.Identity.Name)));
                    lblErrorMessage.Text = Resources.DefaultExceptions.UnauthorisedPageAccessMessage;
                    return;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
                lblErrorMessage.ForeColor = Color.Red;
            }
        }



        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
                log.Debug(ex);
            }
        }



    }
}