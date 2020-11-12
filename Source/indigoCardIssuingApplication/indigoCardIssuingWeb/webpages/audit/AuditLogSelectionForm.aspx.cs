using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using System.Threading;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CCO.objects;
using System.Drawing;
using System.Web.Security;
using System.Security.Permissions;
using indigoCardIssuingWeb.SearchParameters;

namespace indigoCardIssuingWeb.webpages.audit
{
    public partial class AuditLogSelectionForm : BasePage
    {      
        private SystemAdminService sysAdService = new SystemAdminService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.AUDITOR };
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private static readonly ILog log = LogManager.GetLogger(typeof(AuditLogSelectionForm));
        private AudiControllService _auditService = new AudiControllService();

        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuerList = Roles.Provider.ToIndigoRoleProvider()
                                                        .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                this.ddlIssuer.Items.AddRange(issuerList.Values.OrderBy(m => m.Text).ToArray());

                this.ddlUserAction.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
                List<ListItem> userActionList = new List<ListItem>();
                foreach (AuditActionType userAction in Enum.GetValues(typeof(AuditActionType)))
                {
                    userActionList.Add(new ListItem(userAction.ToString(), ((int)userAction).ToString()));
                }
                this.ddlUserAction.Items.AddRange(userActionList.OrderBy(m => m.Text).ToArray());

                this.ddlRoles.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
                List<ListItem> userRolesList = new List<ListItem>();
                foreach (UserRole userRole in Enum.GetValues(typeof(UserRole)))
                {
                    userRolesList.Add(new ListItem(userRole.ToString(), ((int)userRole).ToString()));
                }
                this.ddlRoles.Items.AddRange(userRolesList.OrderBy(m => m.Text).ToArray());

                txbDateFrom.Text = DateTime.Now.ToString(DATE_FORMAT);
                txbDateTo.Text = DateTime.Now.ToString(DATE_FORMAT);
                if (SessionWrapper.SelectedUserName != null)
                {
                    tbUsername.Text = SessionWrapper.SelectedUserName;
                    SessionWrapper.SelectedUserName = string.Empty;
                    if (SessionWrapper.IssuerID != null)
                        this.ddlIssuer.SelectedValue = SessionWrapper.IssuerID.ToString();
                    SessionWrapper.IssuerID = 0;
                }

                if (SessionWrapper.AuditSearch != null)
                {
                    AuditSearch auditSearch = SessionWrapper.AuditSearch;
                    if (auditSearch.IssuerId != null)
                        this.ddlIssuer.SelectedValue = auditSearch.IssuerId.ToString();

                    if (auditSearch.Role != null)
                        this.ddlRoles.SelectedValue = ((int)auditSearch.Role).ToString();

                    if (auditSearch.AuditAction != null)
                        this.ddlUserAction.SelectedValue = ((int)auditSearch.AuditAction).ToString();

                    this.tbUsername.Text = auditSearch.UserName;
                    this.txbDateFrom.Text = auditSearch.DateFrom.ToString(DATE_FORMAT);
                    this.txbDateTo.Text = auditSearch.DateTo.ToString(DATE_FORMAT);

                    SessionWrapper.AuditSearch = null;
                }
                if (ddlIssuer.Items.Count == 0)
                {
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                }
                if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    pnlButtons.Visible = false;
                    lblInfoMessage.Text = "";
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SearchActionMessage").ToString();
                }
                else
                {
                    // lblErrorMessage.Text = "";
                    pnlButtons.Visible = true;
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        #endregion

        protected void SetControls(bool flag)
        {
            tbUsernameSearch.Enabled = flag;
            btnSearchUser.Enabled = flag;
            ddlUserAction.Enabled = flag;
            tbUsername.Enabled = flag;
            txbDateFrom.Enabled = flag;
            txbDateTo.Enabled = flag;
            btnGenerateReport.Enabled = flag;
            // dlAuditList.Visible = false;
        }

        #region BUTTON CLICKS
        [PrincipalPermission(SecurityAction.Demand, Role = "AUDITOR")]
        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            //reset error message
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                var auditSearch = new AuditSearch();

                //check date field entered
                if (txbDateFrom.Text.Trim() == "" || txbDateTo.Text.Trim() == "")
                {
                    lblErrorMessage.Text = GetLocalResourceObject("ValidationDateEmpty").ToString();
                }
                else
                {
                    int auditActionId = int.Parse(this.ddlUserAction.SelectedValue);
                    AuditActionType? auditAction = null;
                    if (auditActionId >= 0)
                        auditAction = (AuditActionType)auditActionId;

                    int userRoleId = int.Parse(this.ddlRoles.SelectedValue);
                    UserRole? userRole = null;
                    if (userRoleId >= 0)
                        userRole = (UserRole)userRoleId;

                    //capture inputs
                    auditSearch.UserName = String.IsNullOrEmpty(this.tbUsername.Text) ? null : this.tbUsername.Text.Trim().Replace("?", "%");
                    auditSearch.IssuerId = int.Parse(this.ddlIssuer.SelectedValue);
                    auditSearch.keyword = "";
                    auditSearch.AuditAction = auditAction;
                    auditSearch.Role = userRole;
                    auditSearch.DateFrom = DateTime.ParseExact(this.txbDateFrom.Text, DATE_FORMAT, null, DateTimeStyles.None);
                    auditSearch.DateTo = DateTime.ParseExact(this.txbDateTo.Text, DATE_FORMAT, null, DateTimeStyles.None);
                    auditSearch.PageIndex = 1;
                    auditSearch.RowsPerPage = StaticDataContainer.ROWS_PER_PAGE;
                    SessionWrapper.AuditSearch = auditSearch;

                    List<GetAuditData_Result> results = _auditService.GetAuditResults(auditSearch, 1);
                    if (results.Count > 0)
                    {
                        SessionWrapper.AuditSearchResult = results;
                        //search
                        Server.Transfer("~\\webpages\\audit\\AuditLogView.aspx");
                    }
                    else
                    {
                        this.lblInfoMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
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
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "AUDITOR")]
        protected void btnSearchUser_Click(object sender, EventArgs e)
        {
            try
            {

                issuerid = int.Parse(ddlIssuer.SelectedValue);
                lblInfoMessage.Text = "";

                if (String.IsNullOrEmpty(this.tbUsernameSearch.Text) ||
                    this.tbUsernameSearch.Text.Length < 4)
                {
                    lblErrorMessage.Text = GetLocalResourceObject("ValidationUsernameLength").ToString();
                    return;
                }

                UserSearch searchUser = new UserSearch(
                    this.tbUsernameSearch.Text, "", this.tbUsernameSearch.Text,
                    this.tbUsernameSearch.Text, (int)issuerid, "");
                //UserManagementService userService = new UserManagementService();
                SessionWrapper.IssuerID = issuerid;
                SessionWrapper.UserSearch = searchUser;
                Server.Transfer("~\\webpages\\users\\UserSearchSelection.aspx");
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
        #endregion

        public int? issuerid
        {
            get
            {
                if (ViewState["issuerid"] == null)
                    return 1;
                else
                    return Convert.ToInt32(ViewState["issuerid"].ToString());
            }
            set
            {
                ViewState["issuerid"] = value;
            }
        }
    }
}