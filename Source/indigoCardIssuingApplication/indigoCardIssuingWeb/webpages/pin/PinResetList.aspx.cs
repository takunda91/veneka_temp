using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class PinResetList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PinResetList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.PIN_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.AUDITOR};

        private readonly PINManagementService _pinService = new PINManagementService();
        private SystemAdminService sysAdminService = new SystemAdminService();

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
                if (SessionWrapper.PinReissueSearchResult != null && SessionWrapper.PinReissueSearchParams != null)
                {
                    var results = SessionWrapper.PinReissueSearchResult.ToArray();
                    SearchParameters = SessionWrapper.PinReissueSearchParams;

                    DisplayResults(SearchParameters, SearchParameters.PageIndex, results);

                    SessionWrapper.PinReissueSearchParams = null;
                    SessionWrapper.PinReissueSearchResult = null;
                }
                else if (SessionWrapper.PinBatchSearchParams != null)
                {
                    PinReissueSearchParameters searchParms = SessionWrapper.PinReissueSearchParams;
                    SearchParameters = searchParms;

                    DisplayResults(searchParms, searchParms.PageIndex, null);

                    SessionWrapper.DistBatchSearchParams = null;
                }
                else
                {
                    DisplayBatchListForUser();
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

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            SessionWrapper.PinReissueSearchParams = (PinReissueSearchParameters)SearchParameters;
            Server.Transfer("~\\webpages\\pin\\PinBatchSearch.aspx");
        }
        #endregion

        #region LIST SELECTION ACTION

        protected void dlPinResetList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void dlPinResetList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {

                dlPinResetList.SelectedIndex = e.Item.ItemIndex;
                string pinReissueIdStr = ((Label)dlPinResetList.SelectedItem.FindControl("lblPinReissueId")).Text;

                long pinReissueId;
                if (long.TryParse(pinReissueIdStr, out pinReissueId))
                {
                    var searchParms = (PinReissueSearchParameters)SearchParameters;
                    searchParms.PageIndex = PageIndex;

                    SessionWrapper.PinReissueSearchParams = searchParms;
                    SessionWrapper.PinReissueId = pinReissueId;

                    Server.Transfer("~\\webpages\\pin\\PinResetView.aspx");
                }
                else
                {
                    this.lblInfoMessage.Text = "The select item does not contain a valid dist batch.";
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

        private void DisplaySearchResult()
        {
            PINSearchResult result = SessionWrapper.PINSearchResult;

            if (result.BatchList != null)
            {
                dlPinResetList.DataSource = result.BatchList;
            }
            else
            {
                var batches = new List<PINBatch>();
                batches.Add(result.Batch);
                dlPinResetList.DataSource = batches;
            }

            dlPinResetList.DataBind();
            SessionWrapper.PINSearchResult = null;
        }

        private void DisplayBatchListForUser()
        {
            dlPinResetList.DataSource = null;

            try
            {
                if (SessionWrapper.PinBatchSearchParams == null)
                {
                    int? pinReissueStatusesId = null;
                    long? operatorUserId = null;
                    long? authoriseUserId = null;
                    bool operatorInProgress = false;

                    if (!String.IsNullOrWhiteSpace(Request.QueryString["pinReissueStatusesId"]))
                    {
                        pinReissueStatusesId = int.Parse(Request.QueryString["pinReissueStatusesId"]);
                    }

                    if (!String.IsNullOrWhiteSpace(Request.QueryString["operatorFinalise"]))
                    {
                        operatorInProgress = bool.Parse(Request.QueryString["operatorFinalise"]);
                        operatorUserId = SessionWrapper.SelectedUserId;
                    }

                    PinReissueSearchParameters searchParams = new PinReissueSearchParameters(null, null, pinReissueStatusesId,0, operatorUserId, operatorInProgress, null, null, null, null, 1, StaticDataContainer.ROWS_PER_PAGE);
                    SearchParameters = searchParams;
                    //DisplayResults(searchParams, searchParams.PageIndex, null);
                }
                else
                {
                    SearchParameters = SessionWrapper.PinReissueSearchParams;
                    SessionWrapper.PinReissueSearchParams = null;
                }


                DisplayResults(SearchParameters, SearchParameters.PageIndex, null);
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

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {            
            this.lblErrorMessage.Text = "";
            this.dlPinResetList.DataSource = null;            

            if (results == null)
            {
                results = _pinService.PINReissueSearch((PinReissueSearchParameters)parms, PageIndex).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlPinResetList.DataSource = results;
                TotalPages = ((PinReissueWSResult)results[0]).TOTAL_PAGES;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlPinResetList.DataBind();
        }
        #endregion        
    }
}