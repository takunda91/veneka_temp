using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.SearchParameters;
using Newtonsoft.Json;
using Common.Logging;
using System.Text;
using indigoCardIssuingWeb.CardIssuanceService;
using System.Threading;
using indigoCardIssuingWeb.utility;
using System.Globalization;
using indigoCardIssuingWeb.CCO.objects;
using System.Web.Security;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.users
{

    public partial class SearchUserForm : BasePage
    {
        #region private_fields
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.USER_ADMIN };
        private static readonly ILog log = LogManager.GetLogger(typeof(SearchUserForm));
        //Standardise look and feel of the Website across all Web Browsers
        private readonly SystemAdminService sysAdminService = new SystemAdminService();
        private readonly UserManagementService userMan = new UserManagementService();
        #endregion

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
                this.ddluserrole.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
                List<ListItem> rolesList = new List<ListItem>();
                foreach (var roles in userMan.LangLookupUserRoles())
                {
                    rolesList.Add(new ListItem(roles.language_text, roles.lookup_id.ToString()));
                }
                this.ddluserrole.Items.AddRange(rolesList.OrderBy(m => m.Text).ToArray());

                //Coming back from previous search, populate the fields.
                if (SessionWrapper.UserSearchParameters != null)
                {
                    UserSearchParameters searchObject = SessionWrapper.UserSearchParameters;

                    this.tbUsername.Text = searchObject.UserName ?? "";
                    this.tbFirstName.Text = searchObject.FirstName ?? "";
                    this.tbLastName.Text = searchObject.LastName ?? "";

                    if (searchObject.UserRole != null)
                        this.ddluserrole.SelectedValue = ((int)searchObject.UserRole).ToString();

                    LoadIssuers(searchObject.IssuerID, searchObject.BranchId);

                    SessionWrapper.UserSearchParameters = null;
                }
                else
                {
                    LoadIssuers(null, null);
                }

                if (ddlissuerlist.Items.Count == 0)
                {
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                }
                if (ddlBranches.Items.Count == 0)
                {
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyBrancheslistMessage").ToString() + "<br/>";
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
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private void LoadIssuers(int? issuerId, int? branchId)
        {
            try
            {
                this.ddlissuerlist.Items.Clear();

                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                this.ddlissuerlist.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                if (ddlissuerlist.Items.FindByValue("-1") != null)
                {
                    ddlissuerlist.Items.RemoveAt(ddlissuerlist.Items.IndexOf(ddlissuerlist.Items.FindByValue("-1")));
                }
                if (ddlissuerlist.Items.Count > 1)
                {
                    ddlissuerlist.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                }
                if (issuerId != null)
                    this.ddlissuerlist.SelectedValue = issuerId.ToString();
                if (issuerId != -1)
                    issuerId = null;

                LoadBranches(int.Parse(this.ddlissuerlist.SelectedValue), branchId);
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

        private void LoadBranches(int? issuerid, int? branchId)
        {
            try
            {
                this.ddlBranches.Items.Clear();
                if (issuerid == -99)
                    issuerid = null;
                var branches = userMan.GetBranchesForUser(issuerid, userRolesForPage[0], null);

                if (branches.Count > 0)
                {
                    if (branches.Count > 1)
                    {
                        this.ddlBranches.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    }

                    List<ListItem> branchList = new List<ListItem>();

                    foreach (var item in branches.OrderBy(m => m.branch_name))//Convert branches in item list.
                    {
                        branchList.Add(utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
                    }

                    if (branchList.Count > 0)
                    {
                        ddlBranches.Items.AddRange(branchList.OrderBy(m => m.Text).ToArray());

                        if (branchId != null)
                            this.ddlBranches.SelectedValue = branchId.ToString();
                    }
                }
                else
                {
                    this.lblInfoMessage.Text = "There are no branches for this issuer.";
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

        protected void btnClear_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            this.tbFirstName.Text = String.Empty;
            this.tbLastName.Text = String.Empty;
            this.tbUsername.Text = String.Empty;
            this.ddlissuerlist.SelectedIndex = 0;
            if (ddlBranches.Items.Count > 0)
            {
                this.ddlBranches.SelectedIndex = 0;
            }
            this.ddluserrole.SelectedIndex = 0;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_ADMIN")]
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                UserSearchParameters searchObject = CreateSearchObject();
                SessionWrapper.UserSearchParameters = searchObject;
                SessionWrapper.SearchUserMode = true;

                Server.Transfer("~\\webpages\\users\\UserList.aspx");
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

        private UserSearchParameters CreateSearchObject()
        {
            UserRole? userRole = null;
            int? branchId = int.Parse(ddlBranches.SelectedValue);
            if (branchId < 0)
                branchId = null;

            int issuerid = int.Parse(ddlissuerlist.SelectedValue);
            int userRoleId = int.Parse(this.ddluserrole.SelectedValue);
            if (userRoleId > 0)
                userRole = (UserRole)userRoleId;

            var search = new UserSearchParameters(issuerid, branchId, null, null, userRole,
                                                  this.tbUsername.Text.Trim(), this.tbFirstName.Text.Trim(),
                                                  this.tbLastName.Text.Trim(), 1, StaticDataContainer.ROWS_PER_PAGE);
            return search;
        }

        protected void ddlissuerlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                LoadBranches(int.Parse(this.ddlissuerlist.SelectedValue), null);
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

        public int issuer_id
        {
            get
            {
                return (int)ViewState["issuerid"];
            }
            set { ViewState["issuerid"] = value; }
        }
    }
}