using Common.Logging;
using indigoCardIssuingWeb.App_Code;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Principal;
using indigoCardIssuingWeb.Old_App_Code.service;
using Microsoft.Reporting.WebForms;
using System.Configuration;

namespace indigoCardIssuingWeb.Reporting.System
{
    public partial class AuditReport_Branchesperusergroup : BasePage
    {
        private readonly BatchManagementService batchService = new BatchManagementService();
        private readonly UserManagementService userMan = new UserManagementService();
        private readonly SystemAdminService sysAdminService = new SystemAdminService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.AUDITOR,
                                                                        UserRole.USER_GROUP_ADMIN };
        private static readonly ILog log = LogManager.GetLogger(typeof(AuditReport_Branchesperusergroup));
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
                GenerateReport();
                ReportViewer1.Visible = false;
               
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
                    if (currentUserRoles.Contains(UserRole.USER_ADMIN))
                    {
                        this.CurrentRole = UserRole.USER_ADMIN;

                    }
                    else
                    {
                        this.CurrentRole = UserRole.AUDITOR;

                    }

                    this.ddlIssuer.Items.AddRange(issuerList.Values.OrderBy(m => m.Text).ToArray());

                    if (ddlIssuer.Items.FindByValue("-1") != null)
                    {
                        ddlIssuer.Items.RemoveAt(ddlIssuer.Items.IndexOf(ddlIssuer.Items.FindByValue("-1")));
                    }
                    if (ddlIssuer.Items.Count > 1)
                    {
                        ddlIssuer.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    }
                    this.ddlIssuer.SelectedIndex = 0;

                    this.ddlIssuer.Visible = true;
                    this.ddlusergroup.Visible = true;
                    lblUsergroup.Visible = true;
                    lblIssuer.Visible = true;
                    this.ddlRole.Visible = true;
                    lbluserrole.Visible = true;

                    // issuer_id = issuerList.Keys.ToArray()[0];
                    // ddlissuerlist.SelectedValue = issuer_id.ToString();
                    issuer_id = int.Parse(ddlIssuer.SelectedValue);

                    ddlRole.Items.Clear();
                    List<ListItem> userRolesList = new List<ListItem>();
                    int? isenterprise = null;
                    //only the enterprise user can create useradmin
                    if (!issuerList.ContainsKey(-1))
                    {
                        isenterprise = 0;

                    }
                    foreach (var userRole in userMan.GetLangUserRoles(isenterprise))
                    {

                        userRolesList.Add(new ListItem(userRole.language_text, userRole.lookup_id.ToString()));
                    }
                    ddlRole.Items.AddRange(userRolesList.OrderBy(m => m.Text).ToArray());
                    ddlRole.Items.Insert(0, new ListItem("Select", "-99"));
                    updateBranchList(issuer_id);
                    LoadGroupsForUser(null);
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
            int issuerid = 0;
            int.TryParse(ddlIssuer.SelectedValue, out issuerid);
            updateBranchList(issuerid);
        }
        protected void btnApplySecondLevelFilter_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport();

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

        protected void GenerateReport()
        {
            try
            {
                List<Tuple<string, string>> objReportParams = new List<Tuple<string, string>>();

                string branchid = null, issuerid = null, usergroupid = null, roleid = null;
                if (ddlbranch.SelectedValue != "-99" && ddlbranch.SelectedValue != "")
                {
                    branchid = this.ddlbranch.SelectedValue;
                   //objreportlabel.Branch = ddlbranch.SelectedItem.Text;
                }
                //else if (ddlbranch.SelectedValue == "-99")
                //{
                //    objreportlabel.Branch = ddlbranch.SelectedItem.Text;
                //}

                objReportParams.Add(new Tuple<string, string>("branch_id", branchid));
                if (ddlRole.SelectedIndex >= 0)
                {
                    if (ddlRole.SelectedValue != "-99")
                    {
                        roleid = ddlRole.SelectedValue;
                        objReportParams.Add(new Tuple<string, string>("ParamUserrole", ddlRole.SelectedItem.Text)); 
                    }
                    else
                    {
                        objReportParams.Add(new Tuple<string, string>("ParamUserrole", "-ALL-")); 
                    }

                }             
                 
                
                objReportParams.Add(new Tuple<string, string>("user_role_id", roleid));
                if (ddlusergroup.SelectedIndex >= 0)
                {
                    if (ddlusergroup.SelectedValue != "-99")
                    {
                        usergroupid = ddlusergroup.SelectedValue;
                        objReportParams.Add(new Tuple<string, string>("ParamUsergroupname", ddlusergroup.SelectedItem.Text)); 

                    }
                    else
                    {
                        objReportParams.Add(new Tuple<string, string>("ParamUsergroupname",  "-ALL-")); 

                    }
                }
                objReportParams.Add(new Tuple<string, string>("user_group_id", usergroupid));

                if (ddlIssuer.SelectedIndex >= 0)
                {
                    if (ddlIssuer.SelectedValue != "-99")
                    {
                        issuerid =ddlIssuer.SelectedValue;
                        objReportParams.Add(new Tuple<string, string>("ParamIssuer", ddlIssuer.SelectedItem.Text.Replace("-", ""))); 
                        

                    }
                    else
                    {
                        objReportParams.Add(new Tuple<string, string>("ParamIssuer", "-ALL-")); 

                    }
                }


                objReportParams.Add(new Tuple<string, string>("issuer_id", issuerid));
                objReportParams.Add(new Tuple<string, string>("ParamGeneratedBy", HttpContext.Current.User.Identity.Name)); 
                long? audit_user_id = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.UserId;
                string audit_workstation = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.WorkStation;

                objReportParams.Add(new Tuple<string, string>("audit_user_id", audit_user_id.ToString()));
                objReportParams.Add(new Tuple<string, string>("audit_workstation", audit_workstation));
                objReportParams.Add(new Tuple<string, string>("languageId", SessionWrapper.UserLanguage.ToString()));
                objReportParams.Add(new Tuple<string, string>("reportId", "12"));


                string urlReportServer = ConfigurationManager.AppSettings["urlReportServer"].ToString();
                string ReportServerFolderName = ConfigurationManager.AppSettings["ReportServerFolderName"].ToString();

                ReportViewer1.ProcessingMode = ProcessingMode.Remote; // ProcessingMode will be Either Remote or Local
                ReportViewer1.ServerReport.ReportServerUrl = new Uri(urlReportServer); //Set the ReportServer Url
                ReportViewer1.ServerReport.ReportPath = "/" + ReportServerFolderName + "/AuditReport_Branchesperusergroup"; //Passing the Report Path                
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

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                LoadGroupsForUser(null);
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

        private void LoadGroupsForUser(long? userId)
        {
            ddlusergroup.Items.Clear();

            int issuerId;
            int roleTypeId;
            int branchId;
            if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) &&
                    int.TryParse(this.ddlRole.SelectedValue, out roleTypeId) &&
                    int.TryParse(this.ddlbranch.SelectedValue, out branchId))
            {
                int? selBranchId = branchId;
                if (branchId == -99)
                {
                    selBranchId = null;
                }

                Dictionary<int, List<UserGroupAdminResult>> results;

                results = userMan.GetDictionaryUserGroupForUserAdmin(userId.GetValueOrDefault(),
                                                                        roleTypeId,
                                                                        issuerId,
                                                                        selBranchId);

                List<ListItem> displayGroups = new List<ListItem>();

                if (results.Count > 0)
                {
                    foreach (var item in results[issuerId])
                    {
                        GroupsRolesResult groupRole = new GroupsRolesResult();
                        groupRole.user_group_name = item.user_group_name;
                        groupRole.user_role = this.ddlRole.SelectedItem.Text;
                        groupRole.issuer_code = this.ddlIssuer.SelectedItem.Text;
                        groupRole.issuer_name = "";

                        ListItem groupItem = new ListItem(item.user_group_name, item.user_group_id.ToString());
                        groupItem.Selected = item.is_in_group == 1 ? true : false;


                        displayGroups.Add(groupItem);
                    }
                }


                this.ddlusergroup.Items.AddRange(displayGroups.OrderBy(m => m.Text).ToArray());
                ddlusergroup.Items.Insert(0, new ListItem("Select", "-99"));
            }
        }

        private void updateBranchList(int issuerId)
        {
            this.ddlbranch.Items.Clear();

            List<ListItem> branchList = new List<ListItem>();
            if (this.CurrentRole == UserRole.USER_ADMIN)
            {
                var branches = userMan.GetBranchesForIssuer(issuerId, null);
                foreach (var item in branches)//Convert branches in item list.
                {
                    ListItem listItem = UtilityClass.FormatListItem(item.branch_name, item.branch_code, item.branch_id);

                    branchList.Add(listItem);
                }

            }
            else
            {
                var branches = userMan.GetBranchesForUser(issuerId, userRolesForPage[0], null);
                foreach (var item in branches)//Convert branches in item list.
                {
                    ListItem listItem = UtilityClass.FormatListItem(item.branch_name, item.branch_code, item.branch_id);

                    branchList.Add(listItem);
                }

            }

            this.ddlbranch.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
            if (branchList.Count > 0)
            {
                this.ddlbranch.Items.AddRange(branchList.OrderBy(m => m.Text).ToArray());
            }

            LoadGroupsForUser(null);

        }

        protected void ddlbranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGroupsForUser(null);
        }
        public UserRole CurrentRole
        {
            get
            {
                return (UserRole)ViewState["Currentrole"];
            }
            set { ViewState["Currentrole"] = value; }
        }
    }
}