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
    public partial class SpoliCardReportPage : BasePage
    {
        private readonly BatchManagementService batchService = new BatchManagementService();
        private readonly UserManagementService userMan = new UserManagementService();
        private readonly SystemAdminService sysAdminService = new SystemAdminService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_OPERATOR,
                                                                        UserRole.CENTER_MANAGER, 
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.BRANCH_OPERATOR,
                                                                        UserRole.AUDITOR};
        private static readonly ILog log = LogManager.GetLogger(typeof(SpoliCardReportPage));

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

                datepickerFrom.Text = DateTime.Now.ToString("dd/MM/yyyy");
                datepickerTo.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");

                issuer_id = 0;
                InitializePage();
                if (!String.IsNullOrEmpty(this.datepickerFrom.Text) && !String.IsNullOrEmpty(this.datepickerTo.Text))
                {
                    this.PrepareReportData(DateTime.ParseExact(this.datepickerFrom.Text, "dd/MM/yyyy", null, DateTimeStyles.None),
                                DateTime.ParseExact(this.datepickerTo.Text, "dd/MM/yyyy", null, DateTimeStyles.None), null);
                }
                this.ReportViewer1.ServerReport.Refresh();
            }
        }

        private void PrepareReportData(DateTime? startDate, DateTime? endDate, string branchOperator)
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
                objReportParams.Add(new Tuple<string, string>("branchid", branch_id));

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


                objReportParams.Add(new Tuple<string, string>("isuerid", issuer_id));

                if (ddluser.SelectedValue != "-99" && ddluser.SelectedValue != "")
                {
                    userid =ddluser.SelectedValue;
                    objReportParams.Add(new Tuple<string, string>("ParamUser", ddluser.SelectedItem.Text));

                }
                else
                {
                    if (ddluser.SelectedValue == "-99")
                        objReportParams.Add(new Tuple<string, string>("ParamUser", "-ALL-"));

                }
                if (ddlProduct.SelectedValue != "-99" && ddlProduct.SelectedValue != "")
                {
                    product_id = this.ddlProduct.SelectedValue;
                }
                objReportParams.Add(new Tuple<string, string>("product_id", product_id));

                objReportParams.Add(new Tuple<string, string>("ParamDateFormat", DATETIME_FORMAT));
                objReportParams.Add(new Tuple<string, string>("userid", userid));

                objReportParams.Add(new Tuple<string, string>("ParamGeneratedBy", HttpContext.Current.User.Identity.Name));
                long? audit_user_id = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.UserId;
                string audit_workstation = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.WorkStation;

                objReportParams.Add(new Tuple<string, string>("audit_user_id", audit_user_id.ToString()));
                objReportParams.Add(new Tuple<string, string>("audit_workstation", audit_workstation));
                objReportParams.Add(new Tuple<string, string>("languageId", SessionWrapper.UserLanguage.ToString()));
                objReportParams.Add(new Tuple<string, string>("fromdate", DateFrom.ToString()));
                objReportParams.Add(new Tuple<string, string>("todate", DateTo.ToString()));
                objReportParams.Add(new Tuple<string, string>("language_id", SessionWrapper.UserLanguage.ToString()));

                objReportParams.Add(new Tuple<string, string>("reportId", "7"));
                objReportParams.Add(new Tuple<string, string>("life_cycle", int.Parse(ddllifecycle.SelectedValue).ToString()));

                string urlReportServer = ConfigurationManager.AppSettings["urlReportServer"].ToString();
                string ReportServerFolderName = ConfigurationManager.AppSettings["ReportServerFolderName"].ToString();

                ReportViewer1.ProcessingMode = ProcessingMode.Remote; // ProcessingMode will be Either Remote or Local
                ReportViewer1.ServerReport.ReportServerUrl = new Uri(urlReportServer); //Set the ReportServer Url
                ReportViewer1.ServerReport.ReportPath = "/" + ReportServerFolderName + "/SpoilCardReport"; //Passing the Report Path                

                //Passing the Report Path                
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

        protected void ddlbranch_SelectedIndexChanged(object sender, EventArgs e)
        {

            ddlbranch.Visible = true;
            LoadBranchOperators(role, issuer_id, ddlbranch.SelectedValue);
        }

        protected void ddlissuerlist_SelectedIndexChanged(object sender, EventArgs e)
        {

            LoadBranches(role, int.Parse(ddlissuerlist.SelectedValue));
            UpdateProductList(int.Parse(ddlissuerlist.SelectedValue));
        }
        private void LoadBranchOperators(UserRole userRole, int issuerid, string branchid)
        {
            try
            {
                #region old code
                //List<ApplicationUser> users;

                //string branchDetails = SessionWrapper.BranchDetails;
                //int start = 0;
                //if (branchDetails != null)
                //{
                //    start = branchDetails.IndexOf('[');
                //}
                //string branchCode = branchDetails != null
                //                        ? branchDetails.Substring(start + 1, branchDetails.Length - start - 2)
                //                        : null;

                //if (branchCode == null)
                //{
                //    int issuerId = SessionWrapper.IssuerID.Value;
                //    users = userMan.GetUsersForIssuer(issuerId);
                //}
                //else
                //{
                //    int issuerId = SessionWrapper.IssuerID.Value;
                //    users = userMan.GetUsersForBranch(branchCode, issuerId);
                //}


                ////the following section gets "branch operators" - it's a bit messy but will be fixed later
                //for (int i = 0; i < users.Count; i++)
                //{
                //    ApplicationUser appUser = users[i];
                //    if (appUser != null)
                //    {

                //        if (!String.IsNullOrEmpty(appUser.UserGroup))
                //        {

                //            UserRole userRole = userMan.GetApplicationUserRole(appUser.UserName);
                //            if (userRole != UserRole.BRANCH_OPERATOR && userRole != UserRole.PIN_OPERATOR)
                //            {

                //                users.RemoveAt(i);
                //                i--;
                //            }
                //            else
                //            {
                //                //now, if it's an operator, does s/he belong to this branch ? 
                //                if (appUser.BranchCode != SessionWrapper.UserBranchCode)
                //                {
                //                    users.RemoveAt(i);
                //                    i--;
                //                }
                //            }
                //        }
                //        else
                //        {
                //            users.RemoveAt(i);
                //            i--;
                //        }
                //    }
                //    else
                //    {
                //        users.RemoveAt(i);
                //        i--;
                //    }
                //}
                //users.Insert(0, new ApplicationUser("SELECT", "OPERATOR", "-", "-", "INVALID USER", UserStatus.INVALID, "-", "-", -1, "-", DateTime.Now,"",false));



                //ddluser.DataMember = "UserName";
                //ddluser.DataTextField = "FullName";
                //ddluser.DataValueField = "UserName";
                //ddluser.DataSource = users;
                //ddluser.DataBind();
                #endregion
                if (branchid == "" || branchid == "-99")
                    branchid = null;


                //if (userRole != UserRole.BRANCH_OPERATOR)
                //{


                ddluser.Visible = true;
                ddluser.Items.Clear();
                List<user_list_result> operators = new List<user_list_result>();
                string username = null;
                if (userRole == UserRole.BRANCH_OPERATOR)
                {
                    username = User.Identity.Name;
                }
                if (branchid != null)
                {
                    operators = userMan.GetUsersByBranch(issuerid, int.Parse(branchid), BranchStatus.ACTIVE, 0, UserRole.BRANCH_OPERATOR, username, null, null, 1, 1000);
                }
                else
                {
                    operators = userMan.GetUsersByBranch(issuerid, null, BranchStatus.ACTIVE, 0, UserRole.BRANCH_OPERATOR, username, null, null, 1, 1000);
                }

                if (operators.Count > 0)
                {
                    List<ListItem> operatorList = new List<ListItem>();

                    foreach (var item in operators)//Convert to item list.
                    {
                        operatorList.Add(new ListItem(item.username, item.user_id.ToString()));
                    }

                    if (operatorList.Count > 0)
                    {
                        this.ddluser.Items.AddRange(operatorList.OrderBy(m => m.Text).ToArray());
                        ddluser.SelectedIndex = 0;


                    }


                }

                if (role != UserRole.BRANCH_OPERATOR)
                {
                    if ((branchid == null && issuer_id == -99) || ddluser.Items.Count > 1)
                    {
                        this.ddluser.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                        this.ddluser.SelectedIndex = 0;
                    }
                }
                else
                {
                    // ddluser.Items.Clear();
                    ddluser.Items.Add(new ListItem(User.Identity.Name, SessionWrapper.SessionUserId.ToString()));
                }

                //}
                //else
                //{
                //    ddluser.Items.Clear();
                //    ddluser.Items.Add(new ListItem(User.Identity.Name, SessionWrapper.SessionUserId.ToString()));
                //}



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
                            LoadBranchOperators(userrole, issuer_id, ddlbranch.SelectedValue);
                        }

                    }
                    else
                    {
                        ddluser.Items.Clear();
                    }
                }
                else
                {
                    ddlbranch.Items.Clear();
                    ddluser.Items.Clear();
                    this.ddlbranch.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    if (role != UserRole.BRANCH_OPERATOR)
                    {

                        this.ddluser.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    }
                    else
                    {
                        ddluser.Items.Clear();
                        ddluser.Items.Add(new ListItem(User.Identity.Name, SessionWrapper.SessionUserId.ToString()));
                    }

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
        //private void InitForOperator()
        //{
        //    try
        //    {


        //        LabelUserDynamicContextOption.Text = "Customer Service Operator";
        //        List<ApplicationUser> availableUsers = userMan.GetUsersForBranch(SessionWrapper.UserBranchCode, SessionWrapper.IssuerID.Value);

        //        //select only operators for this custodian branch
        //        var branchOperators = from users in availableUsers
        //                              where userMan.GetRolesForUserGroup(users.UserGroup, users.IssuerID)[0] == UserRole.BRANCH_OPERATOR
        //                              &&
        //                              users.BranchCode == SessionWrapper.UserBranchCode &&
        //                              users.UserName == User.Identity.Name
        //                              select users;


        //        ddluser.DataMember = "UserName";
        //        ddluser.DataTextField = "FullName";
        //        ddluser.DataValueField = "UserName";
        //        ddluser.DataSource = branchOperators;
        //        ddluser.DataBind();



        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
        //        if (log.IsDebugEnabled || log.IsTraceEnabled)
        //        {
        //            this.lblErrorMessage.Text = ex.ToString();
        //        }
        //        lblErrorMessage.ForeColor = Color.Red;
        //    }
        //}

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
                    this.ddluser.Visible = true;

                    lblBranch.Visible = true;
                    lblIssuer.Visible = true;
                    lblUser.Visible = true;

                    //issuer_id = issuerList.Keys.ToArray()[0];
                    //ddlissuerlist.SelectedValue = issuer_id.ToString();
                    issuer_id = int.Parse(ddlissuerlist.SelectedValue);
                    if (currentUserRoles.Contains(UserRole.CENTER_MANAGER) && issuer_id == -1)
                    {
                        LabelUserDynamicContextOption.Text = GetLocalResourceObject("FilterdbyText").ToString();
                        role = UserRole.CENTER_MANAGER;

                        ddlissuerlist.SelectedValue = issuer_id.ToString();
                        LoadBranches(role, issuer_id);
                        LoadBranchOperators(role, issuer_id, null);
                        UpdateProductList(int.Parse(this.ddlissuerlist.SelectedValue));
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

                        ddlissuerlist.SelectedValue = issuer_id.ToString();
                        LoadBranches(role, issuer_id);
                        string branchid = null;
                        if (ddlbranch.Items.Count == 1)
                        {
                            branchid = ddlbranch.SelectedValue;
                        }
                        LoadBranchOperators(role, issuer_id, branchid);
                        UpdateProductList(int.Parse(this.ddlissuerlist.SelectedValue));
                    }
                    else
                        if (currentUserRoles.Contains(UserRole.BRANCH_CUSTODIAN))
                        {
                            LabelUserDynamicContextOption.Text = GetLocalResourceObject("FilterdbyText").ToString();
                            if (currentUserRoles.Contains(UserRole.BRANCH_CUSTODIAN))
                                role = UserRole.BRANCH_CUSTODIAN;
                            if (currentUserRoles.Contains(UserRole.BRANCH_ADMIN))
                                role = UserRole.BRANCH_ADMIN;

                            ddlissuerlist.SelectedValue = issuer_id.ToString();
                            LoadBranches(role, issuer_id);
                            string branchid = null;
                            if (ddlbranch.Items.Count == 1)
                            {
                                branchid = ddlbranch.SelectedValue;
                            }
                            LoadBranchOperators(role, issuer_id, branchid);
                        UpdateProductList(int.Parse(this.ddlissuerlist.SelectedValue));
                    }
                        else if (currentUserRoles.Contains(UserRole.BRANCH_OPERATOR))
                        {
                            LabelUserDynamicContextOption.Text = GetLocalResourceObject("FilterdbyText1").ToString();
                            if (currentUserRoles.Contains(UserRole.BRANCH_OPERATOR))
                                role = UserRole.BRANCH_OPERATOR;

                            if (ddlissuerlist.Items.Count > 1)
                            {
                                LabelUserDynamicContextOption.Text = GetLocalResourceObject("FilterdbyText").ToString();
                                this.ddlissuerlist.Visible = true;
                                this.lblIssuer.Visible = true;
                            }
                            else
                            {
                                this.ddlissuerlist.Visible = false;
                                this.lblIssuer.Visible = false;
                            }

                            LoadBranches(role, issuer_id);
                            string branchid = null;
                            if (ddlbranch.Items.Count == 1)
                            {
                                branchid = ddlbranch.SelectedValue;
                            }
                            LoadBranchOperators(role, issuer_id, branchid);
                        UpdateProductList(int.Parse(this.ddlissuerlist.SelectedValue));
                    }

                    if (ddlissuerlist.Items.Count == 0)
                    {
                        lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                    }
                    if (ddlbranch.Items.Count == 0)
                    {
                        lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyBrancheslistMessage").ToString() + "<br/>";
                    }
                    if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                    {
                        btnApplyDateRange.Enabled = false;
                        btnApplySecondLevelFilter.Enabled = false;
                        lblInfoMessage.Text = "";
                        lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "ReportMessage").ToString();
                    }
                    else
                    {
                        // lblErrorMessage.Text = "";
                        btnApplyDateRange.Enabled = true;
                        btnApplySecondLevelFilter.Enabled = true;

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
                this.pnlDisable.Visible = false;
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
                if (this.ddluser.SelectedIndex >= 0)
                {

                    if (!String.IsNullOrEmpty(this.datepickerFrom.Text) && !String.IsNullOrEmpty(this.datepickerTo.Text))
                    {
                        this.PrepareReportData(DateTime.ParseExact(this.datepickerFrom.Text, "dd/MM/yyyy", null, DateTimeStyles.None),
                                 DateTime.ParseExact(this.datepickerTo.Text, "dd/MM/yyyy", null, DateTimeStyles.None), null);
                    }
                    else
                    {
                        this.PrepareReportData(null,
                            null, this.ddluser.SelectedValue);
                    }



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

    }

}