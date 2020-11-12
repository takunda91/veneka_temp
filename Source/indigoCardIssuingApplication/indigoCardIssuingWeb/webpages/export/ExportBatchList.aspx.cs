using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.export
{
    public partial class ExportBatchList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ExportBatchList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.AUDITOR};

        private readonly BatchManagementService _batchService = new BatchManagementService();
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
                if (SessionWrapper.ExportBatchSearchResults != null && SessionWrapper.ExportBatchSearchParams != null)
                {
                    var results = SessionWrapper.ExportBatchSearchResults.ToArray();
                    SearchParameters = SessionWrapper.ExportBatchSearchParams;

                    DisplayResults(SearchParameters, SearchParameters.PageIndex, results);

                    // SessionWrapper.DistBatchSearchParams = null;
                    SessionWrapper.ExportBatchSearchResults = null;
                }
                else if (SessionWrapper.ExportBatchSearchParams != null)
                {
                    ExportBatchSearchParameters searchParms = SessionWrapper.ExportBatchSearchParams;
                    SearchParameters = searchParms;

                    DisplayResults(searchParms, searchParms.PageIndex, null);

                    SessionWrapper.ExportBatchSearchParams = null;
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

        #endregion

        #region LIST SELECTION ACTION

        protected void dlBatchList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void dlBatchList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlBatchList.SelectedIndex = e.Item.ItemIndex;
                string exportBatchIdStr = ((Label)dlBatchList.SelectedItem.FindControl("lblExportBatchId")).Text;

                long exportBatchId;
                if (long.TryParse(exportBatchIdStr, out exportBatchId))
                {
                    var searchParms = SearchParameters;
                    searchParms.PageIndex = PageIndex;

                    SessionWrapper.ExportBatchSearchParams = (ExportBatchSearchParameters)searchParms;
                    SessionWrapper.ExportBatchId = exportBatchId;

                    //Server.Transfer("~\\webpages\\card\\DistBatchView.aspx");
                    Response.Redirect("~\\webpages\\export\\ExportBatchView.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    this.lblInfoMessage.Text = "The select item does not contain a valid export batch.";
                }
                //SessionWrapper.batchReference = batchReference;
                //Server.Transfer("~\\webpages\\card\\DistBatchView.aspx");
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

        //private void DisplayResult()
        //{
        //    var results = SessionWrapper.DistributionBatchSearchResult;            
        //    dlBatchList.DataSource = results;
        //    dlBatchList.DataBind();
        //    ViewState[DistBatchListKey] = results; //save into view state
        //    SessionWrapper.DistributionBatchSearchResult = null; //remove from session
        //}

        private void DisplayBatchListForUser()
        {
            dlBatchList.DataSource = null;

            try
            {
                int? searchStatus = null;

                if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
                {
                    searchStatus = int.Parse(Request.QueryString["status"]);
                }                

                ExportBatchSearchParameters searchParams = new ExportBatchSearchParameters(null, null, null, searchStatus, null, null, 1);
                SearchParameters = searchParams;

                DisplayResults(searchParams, searchParams.PageIndex, null);
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

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            SessionWrapper.ExportBatchSearchParams = (ExportBatchSearchParameters)SearchParameters;
            Server.Transfer("~\\webpages\\export\\ExportBatchSearch.aspx");
        }

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {            
            this.lblErrorMessage.Text = "";
            this.dlBatchList.DataSource = null;

            if (results == null)
            {
                results = _batchService.SearchExportBatch((ExportBatchSearchParameters)parms, PageIndex).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlBatchList.DataSource = results;
                TotalPages = ((ExportBatchResult)results[0]).TOTAL_PAGES;
            }
            else
            {
                TotalPages = 1;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlBatchList.DataBind();
        }
        #endregion
    }
}