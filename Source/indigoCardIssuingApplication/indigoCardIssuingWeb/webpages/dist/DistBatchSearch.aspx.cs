﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.SearchParameters;
using Common.Logging;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;


namespace indigoCardIssuingWeb.webpages.dist
{
    public partial class DistBatchSearch : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DistBatchSearch));

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.CARD_PRODUCTION,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.PIN_PRINTER_OPERATOR,
                                                                        UserRole.AUDITOR};

        #region Load Page
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();

                if (!String.IsNullOrWhiteSpace(Request.QueryString["batchType"]))
                {
                    BatchType = int.Parse(Request.QueryString["batchType"]);

                    if (BatchType == 0)
                        lblSearchDistributionBatch.Text = Resources.MenuItems.ProdBatchAdminSearch;
                    else if (BatchType == 1)
                        lblSearchDistributionBatch.Text = Resources.MenuItems.DistBatchAdminSearch;
                }
            }
        }

        private void LoadPageData()
        {
            try
            {
                BuildStatusDropDownList();

                this.ddlIssuer.Items.Clear();
                this.ddlBranch.Items.Clear();

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

                    int issuerId;
                    if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                    {
                        UpdateIssueMethod(issuerId);
                        UpdateBranchList(issuerId);
                    }

                    if (SessionWrapper.DistBatchSearchParams != null)
                    {
                        DistBatchSearchParameters searchParms = SessionWrapper.DistBatchSearchParams;
                        tbBatchReference.Text = searchParms.BatchReference;
                        ddlStatus.SelectedValue = searchParms.DistBatchStatusId != null ? (searchParms.DistBatchStatusId.Value).ToString() : "-1";
                        ddlIssuer.SelectedValue = searchParms.IssuerId != null ? searchParms.IssuerId.ToString() : "-99";

                        if (searchParms.DistBatchTypeId != null)
                        {
                            if (searchParms.DistBatchTypeId == 0)
                                lblSearchDistributionBatch.Text = Resources.MenuItems.ProdBatchAdminSearch;
                            else if (searchParms.DistBatchTypeId == 1)
                                lblSearchDistributionBatch.Text = Resources.MenuItems.DistBatchAdminSearch;
                        }


                        if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                        {
                            UpdateIssueMethod(issuerId);
                            UpdateBranchList(issuerId);
                        }
                        ddlBranch.SelectedValue = searchParms.BranchId != null ? searchParms.BranchId.ToString() : "-99";

                        if (this.ddlIssueMethod.Items.Count > 0)
                            this.ddlIssueMethod.SelectedValue = searchParms.CardIssueMethodId != null ? searchParms.CardIssueMethodId.Value.ToString() : "-99";

                        this.tbDateFrom.Text = searchParms.DateFrom != null ? searchParms.DateFrom.Value.ToString(DATE_FORMAT) : "";
                        this.tbDateTo.Text = searchParms.DateTo != null ? searchParms.DateTo.Value.ToString(DATE_FORMAT) : "";

                        SessionWrapper.DistBatchSearchParams = null;
                    }
                    if (ddlIssuer.Items.Count == 0)
                    {
                        lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                    }
                    if (ddlBranch.Visible == true && ddlBranch.Items.Count == 0)
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
                    UpdateIssueMethod(issuerId);
                    UpdateBranchList(issuerId);
                }
                if (ddlBranch.Visible == true && ddlBranch.Items.Count > 0)
                {
                    lblErrorMessage.Text = "";
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text =
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                DateTime? dateToNull = null;
                DateTime? dateFromNull = null;

                int distBatchStatusInt;
                int? issuerid, BranchId;

                if (ddlIssuer.SelectedValue != "-99")
                {
                    issuerid = int.Parse(ddlIssuer.SelectedValue);
                }
                else
                {
                    issuerid = null;
                }
                if (ddlBranch.SelectedValue != "-99" && ddlBranch.SelectedValue!="")
                {
                    BranchId = int.Parse(ddlBranch.SelectedValue);
                }
                else
                {
                    BranchId = null;
                }
                if (int.TryParse(this.ddlStatus.SelectedValue, out distBatchStatusInt))
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
                    int? distBatchStatus = null;
                    if(distBatchStatusInt >= 0)
                        distBatchStatus = distBatchStatusInt;

                    int? cardIssueMethod = null;
                    if (this.ddlIssueMethod.Items.Count > 0 && this.ddlIssueMethod.SelectedValue != "-99")
                        cardIssueMethod = int.Parse(this.ddlIssueMethod.SelectedValue);


                    string distBatchRef = String.IsNullOrWhiteSpace(this.tbBatchReference.Text) ? null : this.tbBatchReference.Text;

                    DistBatchSearchParameters searchParms = new DistBatchSearchParameters(null, distBatchRef, distBatchStatus, null, BranchId, issuerid, cardIssueMethod, BatchType, dateFromNull, dateToNull, true, 1);

                    var results = _batchService.GetDistBatchesForUser(searchParms, searchParms.PageIndex);

                    if (results.Count == 0)
                    {
                        this.lblInfoMessage.Text = GetLocalResourceObject("InfoNoRecordsFound").ToString();
                    }
                    else
                    {
                        SessionWrapper.DistributionBatchSearchResult = results;
                        SessionWrapper.DistBatchSearchParams = searchParms;
                        Server.Transfer("~\\webpages\\dist\\DistBatchList.aspx");
                    }
                }
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text =
                this.lblInfoMessage.Text = String.Empty;

            try
            {
                ddlIssuer.SelectedIndex = 0;
                ddlStatus.SelectedIndex = 0;
                ddlBranch.Items.Clear();
                tbBatchReference.Text = "";
                tbDateFrom.Text = "";
                tbDateTo.Text = "";
                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && User.Identity.Name != null)
                {
                    UpdateBranchList(issuerId);
                }
                SessionWrapper.DistBatchSearchParams = null;
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

            foreach (var batchStatus in _batchService.LangLookupDistBatchStatuses().OrderBy(o => o.language_text))
            {
                this.ddlStatus.Items.Add(new ListItem(batchStatus.language_text, batchStatus.lookup_id.ToString()));
            }
            this.ddlStatus.SelectedIndex = 0;
        }

        private void UpdateIssueMethod(int issuerId)
        {
            this.ddlIssueMethod.Items.Clear();
            this.ddlIssueMethod.Visible =
                    this.lblIssueMethod.Visible = true;

            if (issuerId > 0)
            {
                var issuer = Issuers[issuerId];

                if (issuer.classic_card_issue_YN)
                    this.ddlIssueMethod.Items.Add(new ListItem("Classic", "0"));

                if (issuer.instant_card_issue_YN)
                    this.ddlIssueMethod.Items.Add(new ListItem("Instant", "1"));
            }
            else
            {
                if (Issuers.Values.Where(w => w.classic_card_issue_YN == true).Count() > 0)
                    this.ddlIssueMethod.Items.Add(new ListItem("Classic", "0"));

                if (Issuers.Values.Where(w => w.instant_card_issue_YN == true).Count() > 0)
                    this.ddlIssueMethod.Items.Add(new ListItem("Instant", "1"));
            }


            if (this.ddlIssueMethod.Items.Count > 1)
                this.ddlIssueMethod.Items.Insert(0, new ListItem(Resources.ListItemLabels.ANY, "-99"));
            else if (this.ddlIssueMethod.Items.Count == 0)
            {
                this.ddlIssueMethod.Visible =
                    this.lblIssueMethod.Visible = false;
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
                lblBranchAllocated.Visible = true;
                ddlBranch.Visible = true;
            }
            else
            {
                lblBranchAllocated.Visible = false;
                ddlBranch.Visible = false;
            }
        }
        #endregion      

        #region ViewState Properties
        public int? BatchType
        {
            get
            {
                if (ViewState["BatchType"] == null)
                    return null;
                else
                    return Convert.ToInt32(ViewState["BatchType"].ToString());
            }
            set
            {
                ViewState["BatchType"] = value;
            }
        }

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
