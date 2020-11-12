using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using indigoCardIssuingWeb.App_Code;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using System.Drawing;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.Web.Security;
using indigoCardIssuingWeb.Old_App_Code.service;
using System.Security.Principal;
using System.Configuration;

namespace indigoCardIssuingWeb.Reporting.Branches
{
    public partial class BranchCardStockMangementReportPage : BasePage
    {
        private readonly BatchManagementService batchService = new BatchManagementService();
        private readonly UserManagementService userMan = new UserManagementService();
        private readonly SystemAdminService sysAdminService = new SystemAdminService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_OPERATOR,
                                                                        UserRole.CENTER_MANAGER,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.BRANCH_OPERATOR};
        private static readonly ILog log = LogManager.GetLogger(typeof(BranchCardStockMangementReportPage));

        #region PRIVATE METHODS
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
                if (Request.QueryString["Page"] != null)
                {
                    PageType = Request.QueryString["Page"].ToString().ToUpper();
                }
                issuer_id = 0;
                InitializePage();


            }
            //this.PrepareReportData();
            //ReportViewer1.Visible = false;
        }

        private void PrepareReportData()
        {
            try
            {
                List<Tuple<string, string>> objReportParams = new List<Tuple<string, string>>();
                // this parameter is not using in sp.

                string branchid = null, issuer_id = null,product_id=null;
                if (ddlbranch.SelectedValue != "-99" && ddlbranch.SelectedValue != "")
                {
                    branchid = this.ddlbranch.SelectedValue;
                }

                if (ddlProduct.SelectedValue != "-99" && ddlProduct.SelectedValue != "")
                {
                    product_id = this.ddlProduct.SelectedValue;
                }
                objReportParams.Add(new Tuple<string, string>("product_id", product_id));

                if (branchid != null)
                {
                    objReportParams.Add(new Tuple<string, string>("ParamBranch", ddlbranch.SelectedItem.Text));

                }
                else
                {
                    objReportParams.Add(new Tuple<string, string>("ParamBranch", "-ALL-"));

                }
                objReportParams.Add(new Tuple<string, string>("branch_id", branchid));
              

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

                objReportParams.Add(new Tuple<string, string>("ParamGeneratedBy", HttpContext.Current.User.Identity.Name));
                long? audit_user_id = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.UserId;
                string audit_workstation = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.WorkStation;

                objReportParams.Add(new Tuple<string, string>("audit_user_id", audit_user_id.ToString()));
                objReportParams.Add(new Tuple<string, string>("audit_workstation", audit_workstation));
                objReportParams.Add(new Tuple<string, string>("languageId", SessionWrapper.UserLanguage.ToString()));

                objReportParams.Add(new Tuple<string, string>("language_id", SessionWrapper.UserLanguage.ToString()));

                objReportParams.Add(new Tuple<string, string>("reportId", "22"));

                objReportParams.Add(new Tuple<string, string>("ParamDateFormat", DATETIME_FORMAT));

                string urlReportServer = ConfigurationManager.AppSettings["urlReportServer"].ToString();
                string ReportServerFolderName = ConfigurationManager.AppSettings["ReportServerFolderName"].ToString();

                ReportViewer1.ProcessingMode = ProcessingMode.Remote; // ProcessingMode will be Either Remote or Local
                ReportViewer1.ServerReport.ReportServerUrl = new Uri(urlReportServer); //Set the ReportServer Url

                if (PageType.ToUpper() == "BRANCH")
                {
                    ReportViewer1.ServerReport.ReportPath = "/" + ReportServerFolderName + "/BranchCardStockManagementReport"; //Passing the Report Path                

                }
                else
                {
                    ReportViewer1.ServerReport.ReportPath = "/" + ReportServerFolderName + "/CardStockManagementReport"; //Passing the Report Path                


                }
                //            ReportViewer1.ServerReport.ReportServerCredentials =
                //System.Net.CredentialCache.DefaultCredentials;

                ReportViewer1.ServerReport.ReportServerCredentials =
                  new ReportServerCredentials();

                //pass parmeters to report

                ReportViewer1.ShowParameterPrompts = false;
                ReportViewer1.ServerReport.SetParameters(ReportParameters.ReportDefaultPatam(objReportParams)); //Set Report Parameters
                if (!IsPostBack)
                    ReportViewer1.ServerReport.Refresh();
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
        protected void ddlbranch_SelectedIndexChanged(object sender, EventArgs e)
        {

            ddlbranch.Visible = true;

        }

        protected void ddlissuerlist_SelectedIndexChanged(object sender, EventArgs e)
        {

            LoadBranches(role, int.Parse(ddlissuerlist.SelectedValue));
            UpdateProductList(int.Parse(ddlissuerlist.SelectedValue));
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

        

        private void InitializePage()
        {
            try
            {

                List<UserRole> currentUserRoles = new List<UserRole>();

                bool hasAccess = false;

                Dictionary<int, ListItem> issuerList = new Dictionary<int, ListItem>();
                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerList))
                {
                    foreach (UserRole userRole in userRolesForPage)
                    {
                        List<RolesIssuerResult> issuers;
                        if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(userRole, out issuers))
                        {
                            hasAccess = true;
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


                    //issuer_id = issuerList.Keys.ToArray()[0];
                    //ddlissuerlist.SelectedValue = issuer_id.ToString();
                    issuer_id = int.Parse(ddlissuerlist.SelectedValue);
                    if (currentUserRoles.Contains(UserRole.CENTER_MANAGER) && issuer_id == -1)
                    {
                        LabelUserDynamicContextOption.Text = GetLocalResourceObject("FilterdbyText").ToString();
                        role = UserRole.CENTER_MANAGER;

                       

                    }
                    else if (currentUserRoles.Contains(UserRole.CENTER_MANAGER) || currentUserRoles.Contains(UserRole.CENTER_OPERATOR) || currentUserRoles.Contains(UserRole.AUDITOR))
                    {
                        LabelUserDynamicContextOption.Text = GetLocalResourceObject("FilterdbyText").ToString();
                        if (currentUserRoles.Contains(UserRole.CENTER_OPERATOR))
                            role = UserRole.CENTER_OPERATOR;
                        if (currentUserRoles.Contains(UserRole.CENTER_MANAGER))
                            role = UserRole.CENTER_MANAGER;
                        if (currentUserRoles.Contains(UserRole.AUDITOR))
                            role = UserRole.AUDITOR;

                       

                    }
                    else
                        if (currentUserRoles.Contains(UserRole.BRANCH_CUSTODIAN))
                    {
                        LabelUserDynamicContextOption.Text = GetLocalResourceObject("FilterdbyText").ToString();
                        if (currentUserRoles.Contains(UserRole.BRANCH_CUSTODIAN))
                            role = UserRole.BRANCH_CUSTODIAN;
                        if (currentUserRoles.Contains(UserRole.BRANCH_ADMIN))
                            role = UserRole.BRANCH_ADMIN;

                      

                    }
                    ddlissuerlist.SelectedValue = issuer_id.ToString();
                    LoadBranches(role, issuer_id);
                    UpdateProductList(issuer_id);
                    string branchid = null;
                    if (ddlbranch.Items.Count == 1)
                    {
                        branchid = ddlbranch.SelectedValue;
                    }

                    if (PageType.ToUpper() == "BRANCH")
                    {
                        lblheading.Text = GetLocalResourceObject("Heading").ToString();
                        Page.Title = GetLocalResourceObject("Heading").ToString();
                    }
                    else
                    {
                        lblheading.Text = GetLocalResourceObject("HeadingCenter").ToString();
                        Page.Title = GetLocalResourceObject("HeadingCenter").ToString();

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

        public string type
        {
            get
            {
                return ViewState["type"].ToString();
            }
            set { ViewState["type"] = value; }
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
        public string PageType
        {
            get
            {
                if (ViewState["PageType"] != null)
                {
                    return ViewState["PageType"].ToString();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ViewState["PageType"] = value;
            }
        }

    }
}