using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.SearchParameters;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.Web.Security;

namespace indigoCardIssuingWeb.webpages.users
{
    public partial class UserList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserList));
        private readonly UserManagementService userMan = new UserManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.USER_ADMIN };

        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        protected void dlUserList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                if (e.Item != null)
                {
                    //if (SessionWrapper.SearchUserMode)
                    //{
                    //    e.Item.Enabled = false;
                    //}
                    //else
                    //{
                    //    e.Item.Enabled = true;
                    //}

                }

            }
        }

        private void LoadPageData()
        {
            this.ddlIssuer.Items.Clear();
            this.ddlBranch.Items.Clear();
            this.dlUserList.DataSource = null;
            this.dlUserList.DataBind();

            ddlIssuer.Visible = true;
            ddlBranch.Visible = true;
            try
            {
                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                        .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                if (SessionWrapper.SearchUserMode)
                {
                    lblErrorMessage.Text = "";
                    SearchParameters = SessionWrapper.UserSearchParameters;
                    SessionWrapper.UserSearchParameters = null;
                    lblBranch.Visible = false;
                    lblIssuer.Visible = false;
                    this.ddlIssuer.Visible = false;
                    this.ddlBranch.Visible = false;
                    this.btnBack.Visible = true;
                    SearchMode = true;
                    DisplayResults(SearchParameters, 1, null);
                    SessionWrapper.SearchUserMode = false;
                }
                else
                {
                    //this.ddlIssuer.Items.Add(new ListItem(GetLocalResourceObject("ListItemUnassigned").ToString(), "-99"));
                    this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());

                    ddlIssuer.SelectedIndex = 0;

                    if (SessionWrapper.UserSearchParameters != null)
                    {
                        SearchParameters = SessionWrapper.UserSearchParameters;
                        ddlIssuer.SelectedValue = ((UserSearchParameters)SearchParameters).IssuerID.ToString();

                    }
                    UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue));
                    SessionWrapper.UserSearchParameters = null;
                    this.btnBack.Visible = false;
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
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        #endregion

        #region PRIVATE METHODS

        protected void dlUserList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlUserList.SelectedIndex = e.Item.ItemIndex;
                string userDetails = ((LinkButton)dlUserList.SelectedItem.FindControl("lbtnUserName")).Text;
                string userIdStr = ((Label)dlUserList.SelectedItem.FindControl("lblUserId")).Text;

                long userId;
                if (long.TryParse(userIdStr, out userId) &&
                    !String.IsNullOrWhiteSpace(userDetails))
                {
                    SessionWrapper.SelectedUserName = userDetails;
                    SessionWrapper.SelectedUserId = userId;

                    if (SearchMode)
                    {

                        SessionWrapper.SearchUserMode = true;
                    }
                    else
                    {
                        int issuerid, branchid;
                        int.TryParse(ddlIssuer.SelectedValue, out issuerid);
                        int.TryParse(ddlBranch.SelectedValue, out branchid);

                        var search = new UserSearchParameters(issuerid, branchid, BranchStatus.ACTIVE, null, null,
                                                  null, null,
                                                 null, 1, StaticDataContainer.ROWS_PER_PAGE);
                        SearchParameters = search;
                    }
                    SessionWrapper.UserSearchParameters = (UserSearchParameters)SearchParameters;
                    Server.Transfer("~\\webpages\\users\\ManageUser.aspx");
                }
                else
                {
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.BadSelectionMessage;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                {
                    UpdateBranchList(issuerId);
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

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                UserSearchParameters searchParms = new UserSearchParameters(null, null, null, 1);


                int branchId;
                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId))
                {
                    if (branchId == -99)
                    {
                        searchParms.BranchId = null;
                        DisplayResults(searchParms, 1, null);
                        //UpdateUserList(null);
                    }
                    else
                    {
                        searchParms.BranchId = branchId;
                        DisplayResults(searchParms, 1, null);
                        //UpdateUserList(branchId);
                    }
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (SearchMode)
            {
                SessionWrapper.UserSearchParameters = (UserSearchParameters)SearchParameters;
                Server.Transfer("~\\webpages\\users\\SearchUserForm.aspx");
            }
            else
            {
                Server.Transfer("~\\webpages\\issuer\\IssuerManagement.aspx");
            }
        }

        private void UpdateBranchList(int issuerId)
        {
            this.ddlBranch.Items.Clear();
            this.dlUserList.DataSource = null;
            this.dlUserList.DataBind();

            UserSearchParameters searchParms = new UserSearchParameters(null, null, null, 1);

            if (issuerId == -99)
            {
                //this.ddlBranch.Items.Add(new ListItem(GetLocalResourceObject("ListItemUnassigned").ToString(), "-99"));
                searchParms.BranchId = null;
                DisplayResults(searchParms, 1, null);
                //UpdateUserList(null);
            }
            else
            {
                var branches = userMan.GetBranchesForUser(issuerId, userRolesForPage[0], null);

                if (branches.Count > 0)
                {
                    List<ListItem> branchList = new List<ListItem>();

                    foreach (var item in branches)//Convert branches in item list.
                    {
                        branchList.Add(utility.UtilityClass.FormatListItem(item.branch_name, item.branch_code, item.branch_id.ToString()));
                    }

                    if (branchList.Count > 0)
                    {
                        ddlBranch.Items.AddRange(branchList.OrderBy(m => m.Text).ToArray());
                        ddlBranch.SelectedIndex = 0;
                        if (SessionWrapper.UserSearchParameters != null && SessionWrapper.UserSearchParameters.BranchId != null)
                        {
                            ddlBranch.SelectedValue = SessionWrapper.UserSearchParameters.BranchId.ToString();
                            SessionWrapper.UserSearchParameters = null;
                            
                        }
                        int branchId;
                        if (int.TryParse(ddlBranch.SelectedValue, out branchId))
                        {
                            searchParms.BranchId = branchId;
                            DisplayResults(searchParms, 1, null);
                            //UpdateUserList(branchId);
                        }
                    }
                }
            }
        }

        //private void UpdateUserList(int? branchId, int pageIndex)
        //{


        //    dlUserList.DataSource = users.OrderBy(m => m.username).ToList();
        //    dlUserList.DataBind();
        //}
        #endregion

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            var userSearchParms = (UserSearchParameters)parms;
            SearchParameters = userSearchParms;

            if (userSearchParms.IssuerID == -99)
            {
                userSearchParms.IssuerID = null;
            }
            if (results == null)
            {
                if (SearchMode)
                {
                    results = userMan.GetUsersByBranchAdmin(userSearchParms, pageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
                }
                else
                {
                    if (userSearchParms.BranchId != null)
                    {
                        results = userMan.GetUsersByBranchAdmin(userSearchParms, pageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
                    }
                    else
                    {
                        results = userMan.GetUnassignedUsers(pageIndex).ToArray();
                    }
                }
            }

            if (results.Length > 0)
            {
                this.dlUserList.DataSource = results;
                TotalPages = ((user_list_result)results[0]).TOTAL_PAGES;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlUserList.DataBind();
        }



        //public UserSearchParameters UserSearchParams
        //{
        //    get
        //    {
        //        if (ViewState["UserSearchParams"] != null)
        //            return (UserSearchParameters)ViewState["UserSearchParams"];
        //        else
        //            return null;
        //    }
        //    set
        //    {
        //        ViewState["UserSearchParams"] = value;
        //    }
        //}

        public bool SearchMode
        {
            get
            {
                if (ViewState["SearchMode"] == null)
                    return false;
                else
                    return (bool)ViewState["SearchMode"];
            }
            set
            {
                ViewState["SearchMode"] = value;
            }
        }
        #endregion
    }
}
