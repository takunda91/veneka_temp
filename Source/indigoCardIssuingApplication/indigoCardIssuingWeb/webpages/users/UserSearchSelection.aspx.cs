using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.utility;
using System.Threading;
using System.Globalization;
using System.Web.Security;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.users
{
    public partial class UserSearchSelection : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserSearchSelection));
        private readonly UserManagementService userMan = new UserManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.AUDITOR };

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
            #region userlist
            this.ddlIssuer.Items.Clear();

            this.dlUserList.DataSource = null;
            this.dlUserList.DataBind();
            try
            {
                Dictionary<int, ListItem> issuerList = Roles.Provider.ToIndigoRoleProvider()
                                                        .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                this.ddlIssuer.Items.AddRange(issuerList.Values.OrderBy(m => m.Text).ToArray());


                this.ddlIssuer.SelectedIndex = 0;
                if (SessionWrapper.UserSearch != null)
                {
                    issuerid = SessionWrapper.IssuerID;
                    ddlIssuer.SelectedIndex = ddlIssuer.Items.IndexOf(ddlIssuer.Items.FindByValue(issuerid.ToString()));
                    SessionWrapper.IssuerID = 0;
                }
                UserSearchParameters searchParms = new UserSearchParameters(null, null, null, 1);
                searchParms.IssuerID = (int)issuerid;
                DisplayResults(searchParms, 1, null);
            }
            catch (Exception ex)
            {
                this.pnlButtons.Visible =
                    this.pnlDisable.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = "An error has occurred, please reload the page and try again. Id the problem persists please contact support.";
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
                lblErrorMessage.Text = Resources.DefaultExceptions.UnauthorisedPageAccessMessage;
            }
            #endregion userlist
        }

        #region Private methods
        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                UserSearchParameters searchParms = new UserSearchParameters(null, null, null, 1);
                searchParms.IssuerID = int.Parse(ddlIssuer.SelectedValue);
                DisplayResults(searchParms, 1, null);

            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage; ;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "AUDITOR")]
        protected void ButtonDone_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataListItem item in dlUserList.Items)
                {
                    if (((CheckBox)item.FindControl("CheckBoxUserName")).Checked)
                    {
                        string username = Convert.ToString(dlUserList.DataKeys[item.ItemIndex]);

                        SessionWrapper.SelectedUserName = username;
                        SessionWrapper.IssuerID = int.Parse(ddlIssuer.SelectedValue);
                        Server.Transfer("~\\webpages\\audit\\AuditLogSelectionForm.aspx");
                        break;
                    }
                }
                SessionWrapper.SelectedUserName = String.Empty;
                lblErrorMessage.Text = GetLocalResourceObject("UserMessage").ToString();

            }
            catch (Exception ex)
            {
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            try
            {
                SessionWrapper.SelectedUserName = String.Empty;
                Server.Transfer("~\\webpages\\audit\\AuditLogSelectionForm.aspx");
            }
            catch (Exception ex)
            {
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        #endregion Private methods

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = "";
            this.dlUserList.DataSource = null;

            var userSearchParms = (UserSearchParameters)parms;
            SearchParameters = parms;

            string searchtext = SessionWrapper.UserSearch.UserName.ToUpper();
            if (results == null)
            {
                userSearchParms.UserName = searchtext;
                results = userMan.GetUsersByBranch(userSearchParms, pageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
            }

            if (results.Length > 0)
            {

                //var userSearch = from users in results
                //                 where users.username.ToUpper().Contains(searchtext) ||
                //                 users.first_name.ToUpper().Contains(searchtext)
                //                 ||
                //                 users.last_name.ToUpper().Contains(searchtext)
                //                 select users;
                if (results.ToList().Count > 0)
                {
                    this.dlUserList.DataSource = results;
                    TotalPages = ((user_list_result)results[0]).TOTAL_PAGES;
                }
                else
                {
                    TotalPages = 0;
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                }
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlUserList.DataBind();
        }

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
        #endregion
    }
}