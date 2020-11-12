using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.remote
{
    public partial class RemoteCardUpdateSearch : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RemoteCardUpdateSearch));

        private readonly RemoteService _remoteService = new RemoteService();
        private readonly UserManagementService _userMan = new UserManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.AUDITOR};

        #region Load Page
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
                BuildStatusDropDownList();

                this.ddlIssuer.Items.Clear();

                //Grab all the distinct issuers from the groups the user belongs to that match the role for this page.
                Dictionary<int, ListItem> issuersList = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuers = new Dictionary<int, RolesIssuerResult>();
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuersList, out issuers);

                try
                {
                    Issuers = issuers;


                    this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());

                    if (ddlIssuer.Items.FindByValue("-1") != null)
                    {
                        ddlIssuer.Items.RemoveAt(ddlIssuer.Items.IndexOf(ddlIssuer.Items.FindByValue("-1")));
                    }

                    if (ddlIssuer.Items.Count > 1)
                    {
                        ddlIssuer.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    }
                    this.ddlIssuer.SelectedIndex = 0;

                    UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue));
                    ddlBranch.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));


                    if (SessionWrapper.RemoteCardUpdateSearchParams != null)
                    {
                        RemoteCardUpdateSearchParameters searchParms = (RemoteCardUpdateSearchParameters)SessionWrapper.RemoteCardUpdateSearchParams;
                        tbRemoteAddress.Text = searchParms.RemoteAddress;
                        ddlStatus.SelectedValue = searchParms.RemoteUpdateStatusesId != null ? (searchParms.RemoteUpdateStatusesId.Value).ToString() : "-1";
                        ddlIssuer.SelectedValue = searchParms.IssuerId != null ? searchParms.IssuerId.ToString() : "-99";
                        ddlBranch.SelectedValue = searchParms.BranchId != null ? searchParms.BranchId.ToString() : "-99";
                        this.tbDateFrom.Text = searchParms.DateFrom != null ? searchParms.DateFrom.Value.ToString(DATE_FORMAT) : "";
                        this.tbDateTo.Text = searchParms.DateTo != null ? searchParms.DateTo.Value.ToString(DATE_FORMAT) : "";

                        SessionWrapper.RemoteCardUpdateSearchParams = null;
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
                    log.Error(ex);
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                    if (log.IsDebugEnabled || log.IsTraceEnabled)
                    {
                        this.lblErrorMessage.Text = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private void UpdateBranchList(int issuerId)
        {
            this.ddlBranch.Items.Clear();
            if (issuerId != -99)
            {
                Dictionary<int, ListItem> branchList = new Dictionary<int, ListItem>();
                var results = _userMan.GetBranchesForUser(issuerId, null, null);

                if (results.Count > 1)
                {
                    this.ddlBranch.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
                }

                foreach (var result in results)
                {
                    if (!branchList.ContainsKey(result.branch_id))
                    {
                        branchList.Add(result.branch_id, utility.UtilityClass.FormatListItem<int>(result.branch_name, result.branch_code, result.branch_id));
                    }
                }

                this.ddlBranch.Items.AddRange(branchList.Values.OrderBy(m => m.Text).ToArray());
            }
            else if(issuerId == -99)
                this.ddlBranch.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
        }
        #endregion

        #region Page Events
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

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CMS_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "AUDITOR")]
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text =
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                DateTime? dateToNull = null;
                DateTime? dateFromNull = null;

                int remoteUpdateStatusesIdInt;
                int? issuerid;
                int? branchId;

                if (ddlIssuer.SelectedValue != "-99")                
                    issuerid = int.Parse(ddlIssuer.SelectedValue);                
                else                
                    issuerid = null;

                if (ddlBranch.SelectedValue != "-99")
                    branchId = int.Parse(ddlBranch.SelectedValue);
                else
                    branchId = null;


                if (int.TryParse(this.ddlStatus.SelectedValue, out remoteUpdateStatusesIdInt))
                {
                    DateTime dateFrom;
                    //If date from is not empty, check that it's in correct format
                    if (!String.IsNullOrWhiteSpace(this.tbDateFrom.Text))
                    {
                        if (DateTime.TryParseExact(this.tbDateFrom.Text, DATE_FORMAT, null, DateTimeStyles.None, out dateFrom))
                        {
                            dateFromNull = dateFrom;
                        }
                        else
                        {
                            this.lblErrorMessage.Text = GetLocalResourceObject("ToDateMessage").ToString();
                            return;
                        }
                    }

                    DateTime dateTo;
                    //If date to is not empty, check that it's in correct format
                    if (!String.IsNullOrWhiteSpace(this.tbDateTo.Text))
                    {
                        if (DateTime.TryParseExact(this.tbDateTo.Text, DATE_FORMAT, null, DateTimeStyles.None, out dateTo))
                        {
                            dateToNull = dateTo;
                        }
                        else
                        {
                            this.lblErrorMessage.Text = GetLocalResourceObject("FromDateMessage").ToString();
                            return;
                        }
                    }

                    //If a valid distbatches status use it, else pass null to not filter by status.
                    int? remoteUpdateStatusesId = null;
                    if (remoteUpdateStatusesIdInt >= 0)
                        remoteUpdateStatusesId = remoteUpdateStatusesIdInt;
                                        
                    RemoteCardUpdateSearchParameters searchParms = new RemoteCardUpdateSearchParameters(null, remoteUpdateStatusesId, issuerid, branchId, null, 
                                                                                                        dateFromNull, dateToNull, 1, StaticDataContainer.ROWS_PER_PAGE);

                    var results = _remoteService.SearchRemoteCardUpdates(searchParms, searchParms.PageIndex, searchParms.RowsPerPage);

                    if (results.Count == 0)
                    {
                        this.lblInfoMessage.Text = GetLocalResourceObject("InfoNoRecordsFound").ToString();
                    }
                    else
                    {
                        searchParms.IsSearch = true;
                        SessionWrapper.RemoteCardUpdateSearchResults = results;
                        SessionWrapper.RemoteCardUpdateSearchParams = searchParms;
                        Server.Transfer("~\\webpages\\remote\\RemoteCardUpdateList.aspx");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text =
                this.lblInfoMessage.Text = String.Empty;

            try
            {
                ddlIssuer.SelectedIndex = 0;
                ddlBranch.SelectedIndex = 0;
                ddlStatus.SelectedIndex = 0;
                tbRemoteAddress.Text = "";
                tbDateFrom.Text = "";
                tbDateTo.Text = "";
                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && User.Identity.Name != null)
                {
                }
                SessionWrapper.RemoteCardUpdateSearchParams = null;
            }
            catch (Exception ex)
            {
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Populates Batch status drop down the items from DisttributionBatchStatus enum.
        /// </summary>
        private void BuildStatusDropDownList()
        {
            this.ddlStatus.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-1"));

            foreach (var batchStatus in _remoteService.LangLookupRemoteUpdateStatuses())
            {
                this.ddlStatus.Items.Add(new ListItem(batchStatus.language_text, batchStatus.lookup_id.ToString()));
            }
            this.ddlStatus.SelectedIndex = 0;
        }
        #endregion

        #region ViewState Properties
        public Dictionary<int, RolesIssuerResult> Issuers
        {
            get
            {
                if (ViewState["Issuers"] == null)
                    return null;
                else
                    return (Dictionary<int, RolesIssuerResult>)ViewState["Issuers"];
            }
            set
            {
                ViewState["Issuers"] = value;
            }
        }
        #endregion
    }
}