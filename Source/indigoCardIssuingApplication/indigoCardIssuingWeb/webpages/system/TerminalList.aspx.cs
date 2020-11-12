using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;
using indigoCardIssuingWeb.Old_App_Code.service;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.service;
using System.Web.Security;
using indigoCardIssuingWeb.SearchParameters;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class TerminalList : ListPage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private UserManagementService _userMan = new UserManagementService();
        private const string PageLayoutKey = "PageLayout";
        private static readonly ILog log = LogManager.GetLogger(typeof(TerminalList));
        private readonly TerminalManagementService _terminalService = new TerminalManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };

        #region Page Events

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (ViewState[PageLayoutKey] != null)
            {
                pageLayout = (PageLayout)ViewState[PageLayoutKey];
            }

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                if (issuersList.ContainsKey(-1))
                    issuersList.Remove(-1);

                this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;

                if (ddlIssuer.SelectedValue != "-99")
                {
                    UpdateBranchList(int.Parse(ddlIssuer.SelectedValue));
                }

                if (SessionWrapper.TerminalSearchParms != null &&
                    SessionWrapper.TerminalSearchParmsResult != null)
                {
                    var searchParams = SessionWrapper.TerminalSearchParms;
                    var results = SessionWrapper.TerminalSearchParmsResult.ToArray();

                    SearchParameters = searchParams;

                    DisplayResults(searchParams, 1, results);
                    this.btnBack.Visible = true;
                    lblBranch.Visible = ddlBranch.Visible = false;
                    lblIssuer.Visible = ddlIssuer.Visible = false;

                    SessionWrapper.TerminalSearchParms = null;
                    SessionWrapper.TerminalSearchParmsResult = null;
                }
                else if (SessionWrapper.TerminalSearchParms != null &&
                  SessionWrapper.TerminalSearchParmsResult == null)
                {
                    var searchParams = SessionWrapper.TerminalSearchParms;

                    SearchParameters = searchParams;

                    DisplayResults(searchParams, 1, null);
                    this.btnBack.Visible = true;
                    lblBranch.Visible = ddlBranch.Visible = false;
                    lblIssuer.Visible = ddlIssuer.Visible = false;

                    SessionWrapper.TerminalSearchParms = null;
                    SessionWrapper.TerminalSearchParmsResult = null;
                }
                else
                {
                    if (issuersList.Count > 0)
                    {
                        //PopulateIssuerFields(int.Parse(this.ddlIssuer.SelectedValue));
                        DisplayResults(null, 1, null);
                        this.btnBack.Visible = false;
                        lblBranch.Visible = ddlBranch.Visible = false;
                        lblIssuer.Visible = ddlIssuer.Visible = true;
                    }
                }

                if (!String.IsNullOrWhiteSpace(Request.QueryString["delete"]))
                {
                    var deleted = int.Parse(Request.QueryString["delete"]);
                    this.ddlIssuer.SelectedValue = deleted.ToString();
                    this.lblInfoMessage.Text = "Terminal deleted successfully.";
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
            }
        }

        private void UpdateBranchList(int issuerId)
        {
            this.ddlBranch.Items.Clear();

            List<int> rolesList = new List<int>();
            foreach (var role in userRolesForPage)
            {
                rolesList.Add((int)role);
            }

            var branches = _userMan.GetBranchesForUserroles(issuerId, rolesList, null);

            if (branches.Count > 0)
            {
                List<ListItem> branchList = new List<ListItem>();

                foreach (var item in branches)//Convert branches in item list.
                {
                    branchList.Add(utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
                }

                if (branchList.Count > 0)
                {

                    ddlBranch.Items.AddRange(branchList.OrderBy(m => m.Value).ToArray());

                    if (ddlBranch.Items.Count > 1)
                    {
                        this.ddlBranch.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    }

                    ddlBranch.SelectedIndex = 0;
                }
            }

            if (ddlIssuer.SelectedValue != "-99")
            {
                lblBranch.Visible = true;
                ddlBranch.Visible = true;
            }
            else
            {
                lblBranch.Visible = false;
                ddlBranch.Visible = false;
            }
        }

        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                int issuerId;

                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && User.Identity.Name != null)
                {
                    UpdateBranchList(issuerId);
                    DisplayResults(null, 1, null);
                }

                if (ddlBranch.Visible == true && ddlBranch.Items.Count > 0)
                {
                    //lblErrorMessage.Text = "";
                    pnlDisable.Visible = true;
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

        protected void dlTerminalList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlTerminalList.SelectedIndex = e.Item.ItemIndex;
                string terminalName = ((LinkButton)dlTerminalList.SelectedItem.FindControl("lnkTerminalName")).Text;
                string terminalstr = ((Label)dlTerminalList.SelectedItem.FindControl("lblTerminalId")).Text;

                if (!string.IsNullOrWhiteSpace(terminalstr))
                {
                    SessionWrapper.TerminalId = int.Parse(terminalstr);
                    if (SearchParameters != null)
                        SessionWrapper.TerminalSearchParms = (TerminalSearchParams)SearchParameters;
                    Server.Transfer("~\\webpages\\system\\TerminalManager.aspx");
                }
                else
                {
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.BadSelectionMessage;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);

            }
        }

        #endregion

        #region Methods

        private int? GetDropDownId(string value)
        {
            int selectedId;
            int? Id = null;

            if (int.TryParse(value, out selectedId))
            {
                if (selectedId > 0)
                {
                    Id = selectedId;
                }
            }

            return Id;
        }

        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {            
            this.lblErrorMessage.Text = string.Empty;
            this.lblErrorMessage.Text = "";
            this.dlTerminalList.DataSource = null;
            
            if (results == null && parms == null)
            {
                int? branchId = GetDropDownId(this.ddlBranch.SelectedValue);
                int? issuerId = GetDropDownId(this.ddlIssuer.SelectedValue);
                parms = new TerminalSearchParams(issuerId, branchId, null, null, null);
                parms.RowsPerPage = StaticDataContainer.ROWS_PER_PAGE;
                parms.PageIndex = 1;
                results = _terminalService.GetTerminalList(issuerId, branchId, StaticDataContainer.ROWS_PER_PAGE, pageIndex).ToArray();
            }
            else if (parms != null)
            {
                results = _terminalService.SearchTerminal((TerminalSearchParams)parms, pageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlTerminalList.DataSource = results;
                TotalPages = (int)((TerminalListResult)results[0]).TOTAL_PAGES;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            SearchParameters = parms;

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlTerminalList.DataBind();
        }

        #endregion

        protected void btnBack_Click(object sender, EventArgs e)
        {
            SessionWrapper.TerminalSearchParms = (TerminalSearchParams)SearchParameters;

            Server.Transfer("~\\webpages\\system\\TerminalSearch.aspx");
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayResults(null, 1, null);
        }
    }
}