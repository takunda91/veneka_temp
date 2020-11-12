using Common.Logging;
using indigoCardIssuingWeb.App_Code;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.Old_App_Code.service;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Security.Principal;
using System.Configuration;

namespace indigoCardIssuingWeb.Reporting.Branches
{
    public partial class PinMailerRePrintReport : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PinMailerRePrintReport));

        private readonly UserManagementService _userService = new UserManagementService();
        private readonly BatchManagementService _batchservice = new BatchManagementService();

        private readonly ReportManagementService _reportService = new ReportManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.REPORT_ADMIN,
                                                                        UserRole.AUDITOR};

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                datepickerFrom.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString(DATE_FORMAT);
                datepickerTo.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString(DATE_FORMAT);

                InitializePage();
                PrepareReportData();
              
            }
        }

        private void InitializePage()
        {
            try
            {
                Dictionary<int, ListItem> issuerList = new Dictionary<int, ListItem>();

                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerList))
                {
                    this.ddlissuerlist.Items.AddRange(issuerList.Values.OrderBy(m => m.Text).ToArray());

                    if (ddlissuerlist.Items.FindByValue("-1") != null)
                        ddlissuerlist.Items.RemoveAt(ddlissuerlist.Items.IndexOf(ddlissuerlist.Items.FindByValue("-1")));                    

                    if (ddlissuerlist.Items.Count > 1)
                        ddlissuerlist.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));                    

                    this.ddlissuerlist.SelectedIndex = 0;

                    this.ddlissuerlist.Visible = true;
                    this.ddlbranch.Visible = true;

                    lblBranch.Visible = true;
                    lblIssuer.Visible = true;

                    LoadBranches(int.Parse(this.ddlissuerlist.SelectedValue));
                    UpdateProductList(int.Parse(this.ddlissuerlist.SelectedValue));

                    if (ddlissuerlist.Items.Count == 0)
                        lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                    
                    if (ddlbranch.Items.Count == 0)
                        lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyBrancheslistMessage").ToString() + "<br/>";
                    
                    if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                    {
                        btnApplyDateRange.Enabled = false;
                        btnApplySecondLevelFilter.Enabled = false;
                        lblInfoMessage.Text = "";
                        lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "ReportMessage").ToString();
                    }
                    else
                    {
                        btnApplyDateRange.Enabled = true;
                        btnApplySecondLevelFilter.Enabled = true;
                    }
                }
                else
                {
                    this.pnlDisable.Visible = false;
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
            }
        }
        private void UpdateProductList(int issuerId)
        {
            this.ddlProduct.Items.Clear();

            if (issuerId > 0)
            {
                List<ProductValidated> products;
                string messages;
                if (_batchservice.GetProductsListValidated(issuerId, null, 1, 1000, out products, out messages))
                {
                    List<ListItem> productsList = new List<ListItem>();
                    Dictionary<int, ProductValidated> productDict = new Dictionary<int, ProductValidated>();

                    foreach (var product in products)
                    {
                        if (!productDict.ContainsKey(product.ProductId))
                            productDict.Add(product.ProductId, product);

                        productsList.Add(utility.UtilityClass.FormatListItem<int>(product.ProductName, product.ProductCode, product.ProductId));
                    }



                    if (productsList.Count > 0)
                    {
                        this.ddlProduct.Items.AddRange(productsList.OrderBy(m => m.Text).ToArray());

                        this.ddlProduct.SelectedIndex = 0;
                    }

                    //this.ddlProduct.Visible = true;
                    //this.lblCardProduct.Visible = true;

                }
                else
                {
                    this.lblErrorMessage.Text = messages;

                }
            }
            else
            {
                this.ddlProduct.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
            }
        }

        private void LoadBranches(int issuerId)
        {
            try
            {
                this.ddlbranch.Items.Clear();

                if (issuerId != -99)
                {
                    var branches = _userService.GetBranchesForUser(issuerId, null, null);

                    if (branches.Count > 0)
                    {
                        List<ListItem> branchList = new List<ListItem>();

                        foreach (var item in branches)//Convert branches in item list.
                        {
                            branchList.Add(new ListItem(string.Format("{0} {1}", new object[] { item.branch_code, item.branch_name }), item.branch_id.ToString()));
                        }


                        if (branchList.Count > 0)
                        {
                            ddlbranch.Items.AddRange(branchList.OrderBy(m => m.Text).ToArray());
                            ddlbranch.SelectedIndex = 0;
                        }

                        if (ddlbranch.Items.Count > 0)
                        {
                            if (ddlbranch.Items.Count > 1)
                            {
                                this.ddlbranch.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                                ddlbranch.SelectedIndex = 0;
                            }
                        }
                    }
                }
                else
                {
                    this.ddlbranch.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
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
            }
        }

        private void PrepareReportData()
        {
            try
            {
                DateTime fromDate = DateTime.ParseExact(this.datepickerFrom.Text, DATE_FORMAT, null, DateTimeStyles.None);
                DateTime toDate = DateTime.ParseExact(this.datepickerTo.Text, DATE_FORMAT, null, DateTimeStyles.None);

                List<Tuple<string, string>> objReportParams = new List<Tuple<string, string>>();
                // this parameter is not using in sp.

               string branchid = null,issuer_id = null, product_id=null;
                if (ddlbranch.SelectedValue != "-99" && ddlbranch.SelectedValue != "")
                {
                    branchid =this.ddlbranch.SelectedValue;
                }


                objReportParams.Add(new Tuple<string, string>("branch_id", branchid));

                if (ddlissuerlist.SelectedIndex >= 0)
                {
                    if (ddlissuerlist.SelectedValue != "-99")
                    {
                        issuer_id =ddlissuerlist.SelectedValue;


                    }
                }


                objReportParams.Add(new Tuple<string, string>("issuer_id", issuer_id));

                //objReportParams.Add(new Tuple<string, string>("ParamGeneratedBy", HttpContext.Current.User.Identity.Name));
                long? audit_user_id = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.UserId;
                string audit_workstation = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.WorkStation;

                objReportParams.Add(new Tuple<string, string>("audit_user_id", audit_user_id.ToString()));
                objReportParams.Add(new Tuple<string, string>("audit_workstation", audit_workstation));
                objReportParams.Add(new Tuple<string, string>("languageId", SessionWrapper.UserLanguage.ToString()));

                objReportParams.Add(new Tuple<string, string>("reportId", "16"));
                objReportParams.Add(new Tuple<string, string>("date_from", fromDate.ToString()));
                objReportParams.Add(new Tuple<string, string>("date_to", toDate.ToString()));
                if (ddlProduct.SelectedValue != "-99" && ddlProduct.SelectedValue != "")
                {
                    product_id = this.ddlProduct.SelectedValue;
                }
                objReportParams.Add(new Tuple<string, string>("product_id", product_id));

                objReportParams.Add(new Tuple<string, string>("ParamDateFormat", DATETIME_FORMAT));

              

                string urlReportServer = ConfigurationManager.AppSettings["urlReportServer"].ToString();
                string ReportServerFolderName = ConfigurationManager.AppSettings["ReportServerFolderName"].ToString();

                ReportViewer1.ProcessingMode = ProcessingMode.Remote; // ProcessingMode will be Either Remote or Local
                ReportViewer1.ServerReport.ReportServerUrl = new Uri(urlReportServer); //Set the ReportServer Url
                ReportViewer1.ServerReport.ReportPath = "/" + ReportServerFolderName + "/PinMailerRePrintReport"; //Passing the Report Path                
                //            ReportViewer1.Credentials =
                //System.Net.CredentialCache.DefaultCredentials;

                ReportViewer1.ServerReport.ReportServerCredentials =
                new ReportServerCredentials();

                //pass parmeters to report
                ReportViewer1.ShowParameterPrompts = false;
                ReportViewer1.ServerReport.SetParameters(ReportParameters.ReportDefaultPatam(objReportParams)); //Set Report Parameters
                if (!IsPostBack)
                    ReportViewer1.ServerReport.Refresh();
                ReportViewer1.Visible = true;
            }
            catch (Exception ex)
            {
                ReportViewer1.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        protected void ddlissuerlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBranches(int.Parse(ddlissuerlist.SelectedValue));
            UpdateProductList(int.Parse(this.ddlissuerlist.SelectedValue));
        }

        protected void btnApplyDateRange_Click(object sender, EventArgs e)
        {
            try
            {
                this.PrepareReportData();
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

        protected void btnApplySecondLevelFilter_Click(object sender, EventArgs e)
        {
            try
            {
                this.PrepareReportData();
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

        /// <summary>
        /// Roles currently assigned to the user.
        /// </summary>
        public List<UserRole> CurrentUserRoles
        {
            get
            {
                if (ViewState["CurrentUserRoles"] != null)
                {
                    return (List<UserRole>)ViewState["CurrentUserRoles"];
                }
                else
                {
                    return new List<UserRole>();
                }
            }
            set
            {
                ViewState["CurrentUserRoles"] = value;
            }
        }
    }
}