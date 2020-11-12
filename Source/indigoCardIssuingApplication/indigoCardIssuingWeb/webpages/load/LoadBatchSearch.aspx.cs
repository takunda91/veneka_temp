using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.SearchParameters;
using Common.Logging;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using System.Web.Security;
using System.Security.Permissions;


namespace indigoCardIssuingWeb.webpages.load
{
    public partial class LoadBatchSearch : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoadBatchSearch));

        private readonly BatchManagementService batchService = new BatchManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER, 
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.CARD_PRODUCTION,
                                                                        UserRole.AUDITOR}; 

        #region PAGE LOAD
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
            lblErrorMessage.Text = "";

            try
            {
                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                if (issuersList.ContainsKey(-1))
                    issuersList.Remove(-1);

                this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());

                if (ddlIssuer.Items.FindByValue("-1") != null)
                {
                    ddlIssuer.Items.RemoveAt(ddlIssuer.Items.IndexOf(ddlIssuer.Items.FindByValue("-1")));
                }
                if (ddlIssuer.Items.Count > 1)
                {
                    ddlIssuer.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                }

                //BuildDateFields();
                BuildStatusDropDownList();
                //check if this is a failed search
                if (SessionWrapper.LoadBatchSearchParams != null)
                {
                    var searchParams = SessionWrapper.LoadBatchSearchParams;

                    if (!String.IsNullOrWhiteSpace(searchParams.BatchReference))
                        this.tbBatchReference.Text = searchParams.BatchReference;

                    if (searchParams.DateFrom != null)
                        this.tbDateFrom.Text = searchParams.DateFrom.Value.ToString(DATE_FORMAT);

                    if (searchParams.DateTo != null)
                        this.tbDateTo.Text = searchParams.DateTo.Value.ToString(DATE_FORMAT);

                    if (searchParams.LoadBatchStatus != null)
                        this.ddlStatus.SelectedValue = ((int)searchParams.LoadBatchStatus).ToString();

                    this.ddlIssuer.SelectedValue = searchParams.IssuerId.ToString();

                    SessionWrapper.LoadBatchSearchParams = null;
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
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }
        #endregion

        #region BUTTON CLICK METHODS
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CARD_PRODUCTION")]
        [PrincipalPermission(SecurityAction.Demand, Role = "AUDITOR")]
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                string loadBatchRef = String.IsNullOrWhiteSpace(this.tbBatchReference.Text.Trim()) ? null : this.tbBatchReference.Text.Trim();

                LoadBatchStatus? loadBatchStatus = null;
                int loadBatchStatusId;
                if (!String.IsNullOrWhiteSpace(ddlStatus.SelectedValue) && int.TryParse(ddlStatus.SelectedValue, out loadBatchStatusId))
                {
                    loadBatchStatus = (LoadBatchStatus?)loadBatchStatusId;
                }

                DateTime? dateToNull = null;
                DateTime? dateFromNull = null;

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
                        this.lblErrorMessage.Text = "To Date not in correct format, please correct and try again.";
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
                        this.lblErrorMessage.Text = "From Date not in correct format, please correct and try again.";
                        return;
                    }
                }

                int issuerid = 0;
                if (ddlIssuer.SelectedValue != "-99")
                {
                    issuerid=int.Parse(this.ddlIssuer.SelectedValue);
                }

                LoadBatchSearchParameters searchParms = new LoadBatchSearchParameters(loadBatchRef, issuerid,
                                                                                      loadBatchStatus, dateFromNull, dateToNull, true, 1);

                var results = batchService.GetLoadBatches(searchParms, searchParms.PageIndex);

                if (results.Count > 0)
                {
                    SessionWrapper.LoadBatchSearchResult = results;
                    SessionWrapper.LoadBatchSearchParams = searchParms;
                    Server.Transfer("~\\webpages\\load\\LoadBatchList.aspx");
                }
                else
                {
                    this.lblInfoMessage.Text = GetLocalResourceObject("InfoNoRecordsFound").ToString();
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
    

        #endregion

        #region PRIVATE METHODS
        private void BuildStatusDropDownList()
        {
            ddlStatus.Items.Add(Resources.ListItemLabels.SELECT);

            List<ListItem> batchStatusList = new List<ListItem>();
            foreach (var batchStatus in batchService.LangLookupLoadBatchStatuses())
            {
                batchStatusList.Add(new ListItem(batchStatus.language_text, batchStatus.lookup_id.ToString()));
            }

            ddlStatus.Items.AddRange(batchStatusList.OrderBy(m => m.Text).ToArray());
            ddlStatus.SelectedIndex = 0;
        }
        #endregion
    }
}
