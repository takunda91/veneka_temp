using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.Security.Permissions;
using System.Web.Security;

namespace indigoCardIssuingWeb.webpages.users
{
    public partial class UserGroupCreation : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserGroupCreation));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.USER_GROUP_ADMIN };
        private readonly SystemAdminService sysAdminService = new SystemAdminService();
        private readonly UserManagementService userMan = new UserManagementService();

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
            dlUserGroupsList.DataSource = null;
            dlUserGroupsList.DataBind();
            try
            {
                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                List<ListItem> UserRolesLists = new List<ListItem>();
                foreach (var role in userMan.LangLookupUserRoles())
                {
                    UserRolesLists.Add(new ListItem(role.language_text, role.lookup_id.ToString()));
                }
                this.ddlRole.Items.AddRange(UserRolesLists.OrderBy(m => m.Text).ToArray());
                this.ddlRole.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));

                this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;

                int issuerId;
                int userRoleId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) &&
                    int.TryParse(this.ddlRole.SelectedValue, out userRoleId))
                {
                    UserRole? userRole = null;
                    if (userRoleId >= 0)
                        userRole = (UserRole)userRoleId;

                    UserGroupSearchParameters searchParms = new UserGroupSearchParameters(issuerId, userRole, 1);
                    DisplayResults(searchParms, 1, null);
                    //UpdateGroupsList(issuerId);
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

        protected void dlUserGroupsList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlUserGroupsList.SelectedIndex = e.Item.ItemIndex;
                string userGroupIdStr = ((Label)dlUserGroupsList.SelectedItem.FindControl("lblUserGroupId")).Text;

                int userGroupId;
                if (int.TryParse(userGroupIdStr, out userGroupId))
                {
                    SessionWrapper.SelectedUserGroupId = userGroupId;
                    Server.Transfer("~\\webpages\\users\\UserGroupViewForm.aspx");
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

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_GROUP_ADMIN")]
        protected void btnCreateUserGroupButton_Click(object sender, EventArgs e)
        {
            SessionWrapper.SelectedUserGroupId = null;
            Server.Transfer("~\\webpages\\users\\UserGroupViewForm.aspx");
        }

        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlUserGroupsList.DataSource = null;
                dlUserGroupsList.DataBind();

                int issuerId;
                int userRoleId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) &&
                    int.TryParse(this.ddlRole.SelectedValue, out userRoleId))
                {
                    UserRole? userRole = null;
                    if (userRoleId >= 0)
                        userRole = (UserRole)userRoleId;

                    UserGroupSearchParameters searchParms = new UserGroupSearchParameters(issuerId, userRole, 1);
                    DisplayResults(searchParms, 1, null);
                    //UpdateGroupsList(issuerId);
                }
                else
                {
                    this.lblInfoMessage.Text = "No issuer selected.";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlUserGroupsList.DataSource = null;
                dlUserGroupsList.DataBind();

                int issuerId;
                int userRoleId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) &&
                    int.TryParse(this.ddlRole.SelectedValue, out userRoleId))
                {
                    UserRole? userRole = null;
                    if (userRoleId >= 0)
                        userRole = (UserRole)userRoleId;

                    UserGroupSearchParameters searchParms = new UserGroupSearchParameters(issuerId, userRole, 1);
                    DisplayResults(searchParms, 1, null);
                    //UpdateGroupsList(issuerId);
                }
                else
                {
                    this.lblInfoMessage.Text = "No issuer selected.";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {            
            this.lblErrorMessage.Text = "";
            this.dlUserGroupsList.DataSource = null;

            var userGroupParms = (UserGroupSearchParameters)parms;
            SearchParameters = userGroupParms;

            if (results == null)
            {
                results = userMan.GetUserGroups(userGroupParms.IssuerID, userGroupParms.UserRole, pageIndex).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlUserGroupsList.DataSource = results;
                TotalPages = ((UserGroupResult)results[0]).TOTAL_PAGES;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlUserGroupsList.DataBind();
        }
        #endregion
    }
}