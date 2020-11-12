using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using System.Drawing;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using indigoCardIssuingWeb.App_Code;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Web.Security;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using System.Security.Principal;
using indigoCardIssuingWeb.Old_App_Code.service;

namespace indigoCardIssuingWeb.Reporting.CardCenter
{
    public partial class IssuerCardsSummaryReportPage : BasePage
    {
        private readonly BatchManagementService batchService = new BatchManagementService();
        private readonly UserManagementService userMan = new UserManagementService();
        private readonly SystemAdminService sysAdminService = new SystemAdminService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_OPERATOR,
                                                                        UserRole.CENTER_MANAGER, UserRole.BRANCH_CUSTODIAN,UserRole.AUDITOR};
        private static readonly ILog log = LogManager.GetLogger(typeof(IssuerCardsSummaryReportPage));

        #region PRIVATE METHODS

        private void GenerateErrorMessage(string strResponse)
        {
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Text = strResponse;
        }

        private void ClearErrorMessage()
        {
            lblErrorMessage.ForeColor = Color.Blue;
            lblErrorMessage.Text = "";
        }

        #endregion

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (
                Request.ServerVariables["http_user_agent"].IndexOf("Chrome", StringComparison.CurrentCultureIgnoreCase) !=
                -1)
                Page.ClientTarget = "uplevel";
            if (
                Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) !=
                -1)
                Page.ClientTarget = "uplevel";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                datepickerFrom.Text = DateTime.Now.ToString("dd/MM/yyyy");
                datepickerTo.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");

                issuer_id = 0;
                InitializePage();
                if (!String.IsNullOrEmpty(this.datepickerFrom.Text) && !String.IsNullOrEmpty(this.datepickerTo.Text))
                {
                    this.PrepareReportData(DateTime.ParseExact(this.datepickerFrom.Text, "dd/MM/yyyy", null, DateTimeStyles.None),
                                DateTime.ParseExact(this.datepickerTo.Text, "dd/MM/yyyy", null, DateTimeStyles.None), null);
                }


            }
        }

        protected override void InitializeCulture()
        {

            int lang = SessionWrapper.UserLanguage;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(UtilityClass.GetLang(lang));
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(UtilityClass.GetLang(lang));


        }
        private void InitializePage()
        {
            try
            {

                List<UserRole> currentUserRoles = new List<UserRole>();


                Dictionary<int, ListItem> issuerList = new Dictionary<int, ListItem>();
                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerList))
                {
                    foreach (UserRole userRole in userRolesForPage)
                    {
                        List<RolesIssuerResult> issuers;
                        if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(userRole, out issuers))
                        {

                            currentUserRoles.Add(userRole);


                        }
                    }


                    this.ddlissuerlist.Items.AddRange(issuerList.Values.OrderBy(m => m.Text).ToArray());

                    if (ddlissuerlist.Items.FindByValue("-1") != null)
                    {
                        ddlissuerlist.Items.RemoveAt(ddlissuerlist.Items.IndexOf(ddlissuerlist.Items.FindByValue("-1")));
                    }
                    if (ddlissuerlist.Items.Count > 1)
                    {
                        ddlissuerlist.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    }
                    this.ddlissuerlist.SelectedIndex = 0;

                    this.ddlissuerlist.Visible = true;
                    this.ddlbranch.Visible = true;
                    lblBranch.Visible = true;
                    lblIssuer.Visible = true;

                    // issuer_id = issuerList.Keys.ToArray()[0];
                    // ddlissuerlist.SelectedValue = issuer_id.ToString();
                    issuer_id = int.Parse(ddlissuerlist.SelectedValue);

                    if (currentUserRoles.Contains(UserRole.CENTER_MANAGER) && issuer_id == -1)
                    {
                        lblvalue.Text = GetLocalResourceObject("FilterByText").ToString();
                        role = UserRole.CENTER_MANAGER;

                        ddlissuerlist.SelectedValue = issuer_id.ToString();
                        LoadBranches(role, issuer_id);
                        UpdateProductList(issuer_id);
                    }
                    else if (currentUserRoles.Contains(UserRole.CENTER_MANAGER) || currentUserRoles.Contains(UserRole.CENTER_OPERATOR) || currentUserRoles.Contains(UserRole.AUDITOR))
                    {
                        lblvalue.Text = GetLocalResourceObject("FilterByText").ToString();
                        if (currentUserRoles.Contains(UserRole.CENTER_OPERATOR))
                            role = UserRole.CENTER_OPERATOR;
                        if (currentUserRoles.Contains(UserRole.CENTER_MANAGER))
                            role = UserRole.CENTER_MANAGER;
                        if (currentUserRoles.Contains(UserRole.AUDITOR))
                            role = UserRole.AUDITOR;

                        ddlissuerlist.SelectedValue = issuer_id.ToString();
                        LoadBranches(role, issuer_id);
                        UpdateProductList(issuer_id);
                    }
                    else if (currentUserRoles.Contains(UserRole.BRANCH_CUSTODIAN) || currentUserRoles.Contains(UserRole.BRANCH_OPERATOR))
                    {
                        lblvalue.Text = GetLocalResourceObject("FilterByText").ToString();
                        if (currentUserRoles.Contains(UserRole.BRANCH_OPERATOR))
                            role = UserRole.BRANCH_OPERATOR;
                        if (currentUserRoles.Contains(UserRole.BRANCH_CUSTODIAN))
                            role = UserRole.BRANCH_CUSTODIAN;


                        LoadBranches(role, issuer_id);
                        UpdateProductList(issuer_id);
                    }



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
        protected void ddlissuerlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBranches(role, int.Parse(ddlissuerlist.SelectedValue));
            UpdateProductList(issuer_id);
        }
        private void UpdateProductList(int issuerId)
        {
            this.ddlProduct.Items.Clear();

            if (issuerId > 0)
            {
                List<ProductValidated> products;
                string messages;
                if (batchService.GetProductsListValidated(issuerId, null, 1, 1000, out products, out messages))
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
                    if (ddlProduct.Items.Count > 1)
                    {
                        ddlProduct.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
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
        private void LoadBranches(UserRole userrole, int issuerid)
        {
            try
            {
                if (issuerid != -99)
                {
                    //  LabelUserDynamicContextOption.Text = "Customer Service Branch";
                    issuer_id = issuerid;

                    this.ddlbranch.Items.Clear();

                    string username = "";
                    if (userrole == UserRole.BRANCH_CUSTODIAN || userrole == UserRole.BRANCH_OPERATOR)
                    {
                        username = User.Identity.Name;
                    }
                    var branches = userMan.GetBranchesForUser(issuerid, userrole, null);

                    if (branches.Count > 0)
                    {
                        List<ListItem> branchList = new List<ListItem>();

                        foreach (var item in branches)//Convert branches in item list.
                        {
                            branchList.Add(new ListItem(string.Format("{0} {1}", new object[] { item.branch_code, item.branch_name }), item.branch_id.ToString()));
                        }


                        if (branchList.Count > 0)
                        {
                            ddlbranch.Items.AddRange(branchList.OrderBy(m => m.Value).ToArray());
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
                    ddlbranch.Items.Clear();
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
                lblErrorMessage.ForeColor = Color.Red;
            }
        }






        protected void PrepareReportData(DateTime? startDate, DateTime? endDate, string branchid)
        {
            try
            {
                List<Tuple<string, string>> objReportParams = new List<Tuple<string, string>>();

                DateTime DateFrom; DateTime DateTo;
                if (startDate.HasValue && endDate.HasValue)
                {
                    DateFrom = startDate.Value;
                    DateTo = endDate.Value;
                }
                else
                {

                    DateFrom = DateTime.MinValue;
                    DateTo = DateTime.Now;
                }
                string DateRangetext = DateFrom.ToString("dd/MM/yyyy") + " To " + DateTo.ToString("dd/MM/yyyy");
                objReportParams.Add(new Tuple<string, string>("ParamDateRangetext", DateRangetext));


                string branch_id = null, issuer_id = null, userid = null, product_id=null;
                if (ddlbranch.SelectedValue != "-99" && ddlbranch.SelectedValue != "")
                {
                    branch_id = this.ddlbranch.SelectedValue;
                }

                if (branch_id != null)
                {
                    objReportParams.Add(new Tuple<string, string>("ParamBranch", ddlbranch.SelectedItem.Text));

                }
                else
                {
                    objReportParams.Add(new Tuple<string, string>("ParamBranch", "-ALL-"));

                }
                objReportParams.Add(new Tuple<string, string>("branch_id", branch_id));

                if (ddlissuerlist.SelectedIndex >= 0)
                {
                    if (ddlissuerlist.SelectedValue != "-99")
                    {
                        issuer_id = ddlissuerlist.SelectedValue;
                        objReportParams.Add(new Tuple<string, string>("ParamIssuer", ddlissuerlist.SelectedItem.Text.Replace("-", "")));


                    }
                    else
                    {
                        objReportParams.Add(new Tuple<string, string>("ParamIssuer", "-ALL-"));

                    }
                }


                objReportParams.Add(new Tuple<string, string>("issuer_id", issuer_id));
                if (ddlProduct.SelectedValue != "-99" && ddlProduct.SelectedValue != "")
                {
                    product_id = this.ddlProduct.SelectedValue;
                }
                objReportParams.Add(new Tuple<string, string>("product_id", product_id));

                objReportParams.Add(new Tuple<string, string>("ParamDateFormat", DATETIME_FORMAT));


                objReportParams.Add(new Tuple<string, string>("ParamGeneratedBy", HttpContext.Current.User.Identity.Name));
                long? audit_user_id = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.UserId;
                string audit_workstation = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.WorkStation;

                objReportParams.Add(new Tuple<string, string>("audit_user_id", audit_user_id.ToString()));
                objReportParams.Add(new Tuple<string, string>("audit_workstation", audit_workstation));
                objReportParams.Add(new Tuple<string, string>("languageId", SessionWrapper.UserLanguage.ToString()));
                objReportParams.Add(new Tuple<string, string>("fromdate", DateFrom.ToString()));
                objReportParams.Add(new Tuple<string, string>("todate", DateTo.ToString()));
                objReportParams.Add(new Tuple<string, string>("language_id", SessionWrapper.UserLanguage.ToString()));

                objReportParams.Add(new Tuple<string, string>("reportId", "9"));
                objReportParams.Add(new Tuple<string, string>("life_cycle", int.Parse(ddllifecycle.SelectedValue).ToString()));


                string urlReportServer = ConfigurationManager.AppSettings["urlReportServer"].ToString();
                string ReportServerFolderName = ConfigurationManager.AppSettings["ReportServerFolderName"].ToString();

                ReportViewer1.ProcessingMode = ProcessingMode.Remote; // ProcessingMode will be Either Remote or Local
                ReportViewer1.ServerReport.ReportServerUrl = new Uri(urlReportServer); //Set the ReportServer Url
                ReportViewer1.ServerReport.ReportPath = "/" + ReportServerFolderName + "/Issuedcards summary report"; //Passing the Report Path                

                //Passing the Report Path                
                //            ReportViewer1.Credentials =
                //System.Net.CredentialCache.DefaultCredentials;

                ReportViewer1.ServerReport.ReportServerCredentials =
              new ReportServerCredentials();

                //pass parmeters to report
                ReportViewer1.ShowParameterPrompts = false;
                ReportViewer1.ServerReport.SetParameters(ReportParameters.ReportDefaultPatam(objReportParams)); //Set Report Parameters
                if (!IsPostBack)
                {
                    ReportViewer1.ServerReport.Refresh();
                }
                ReportViewer1.Visible = true;
                ClearErrorMessage();


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
                lblErrorMessage.ForeColor = Color.Red;
            }
        }




        protected void btnApplyDateRange_Click(object sender, EventArgs e)
        {
            try
            {


                if (!String.IsNullOrEmpty(this.datepickerFrom.Text) && !String.IsNullOrEmpty(this.datepickerTo.Text))
                {
                    this.PrepareReportData(DateTime.ParseExact(this.datepickerFrom.Text, "dd/MM/yyyy", null, DateTimeStyles.None),
                                DateTime.ParseExact(this.datepickerTo.Text, "dd/MM/yyyy", null, DateTimeStyles.None), null);
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

        protected void btnApplySecondLevelFilter_Click(object sender, EventArgs e)
        {
            try
            {


                if (!String.IsNullOrEmpty(this.datepickerFrom.Text) && !String.IsNullOrEmpty(this.datepickerTo.Text))
                {
                    this.PrepareReportData(DateTime.ParseExact(this.datepickerFrom.Text, "dd/MM/yyyy", null, DateTimeStyles.None),
                             DateTime.ParseExact(this.datepickerTo.Text, "dd/MM/yyyy", null, DateTimeStyles.None), null);
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
        public UserRole role
        {
            get
            {
                return (UserRole)ViewState["userRole"];
            }
            set { ViewState["userRole"] = value; }
        }

        public int issuer_id
        {
            get
            {
                return (int)ViewState["issuerid"];
            }
            set { ViewState["issuerid"] = value; }
        }
        public string Language
        {
            get
            {
                return (string)ViewState["Language"];
            }
            set { ViewState["Language"] = value; }
        }

        
    }
}