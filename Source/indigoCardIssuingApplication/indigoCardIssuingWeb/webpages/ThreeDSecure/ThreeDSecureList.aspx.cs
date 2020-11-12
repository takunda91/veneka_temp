using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.SearchParameters;
using Common.Logging;
using indigoCardIssuingWeb.utility;
using System.Threading;
using System.Globalization;
using System.Web.Security;
using System.IO;
using System.Collections;
using System.Linq;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;

namespace indigoCardIssuingWeb.webpages.ThreeDSecure
{
    public partial class ThreeDSecureList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ThreeDSecureList));
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER, UserRole.CENTER_OPERATOR };
        private bool showcheckbox = false;


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
                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                //check if the this is being landed on from a serach
                //BatchSearch search = SessionWrapper.BatchSearch;

                this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                if (ddlIssuer.Items.FindByValue("-1") != null)
                {
                    ddlIssuer.Items.RemoveAt(ddlIssuer.Items.IndexOf(ddlIssuer.Items.FindByValue("-1")));
                }
                if (ddlIssuer.Items.Count > 1)
                {
                    ddlIssuer.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                }
                if (SessionWrapper.ThreedBatchSearchResult != null && SessionWrapper.ThreedBatchSearchParams != null)
                {
                    var results = SessionWrapper.ThreedBatchSearchResult.ToArray();
                    var searchParms = SessionWrapper.ThreedBatchSearchParams;
                    SearchParameters = searchParms;

                    ddlIssuer.SelectedValue = searchParms.IssuerId.ToString();
                    this.ddlIssuer.Visible = searchParms.IsFromSearch ? false : true;
                    this.lblIssuer.Visible = searchParms.IsFromSearch ? false : true;

                    DisplayResults(searchParms, searchParms.PageIndex, results);

                    SessionWrapper.ThreedBatchSearchParams = null;
                    SessionWrapper.ThreedBatchSearchResult = null;
                }
                else if (SessionWrapper.ThreedBatchSearchParams != null)
                {
                    ThreedBatchSearchParameters searchParms = SessionWrapper.ThreedBatchSearchParams;
                    SearchParameters = searchParms;
                    this.ddlIssuer.SelectedValue = searchParms.IssuerId.ToString();

                    this.ddlIssuer.Visible = searchParms.IsFromSearch ? false : true;
                    this.lblIssuer.Visible = searchParms.IsFromSearch ? false : true;
                    ddlIssuer.SelectedValue = searchParms.IssuerId.ToString();

                    DisplayResults(searchParms, searchParms.PageIndex, null);

                    SessionWrapper.ThreedBatchSearchParams = null;
                }
                else
                {
                    if (SessionWrapper.IssuerID != null)
                    {
                        ddlIssuer.SelectedValue = SessionWrapper.IssuerID.ToString();
                    }
                    DisplayBatchListForUser();
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                //this.pnlButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private void DisplayBatchListForUser()
        {
            try
            {
                this.dlBatchList.DataSource = null;

                ThreedBatchSearchParameters searchParms = new ThreedBatchSearchParameters(null, int.Parse(this.ddlIssuer.SelectedValue),
                                                                                      null, null, null, false, 1);
                SearchParameters = searchParms;

                DisplayResults(searchParms, searchParms.PageIndex, null);
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

        #region LIST SELECT ACTION
        protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                ThreedBatchSearchParameters searchParms = (ThreedBatchSearchParameters)SearchParameters;
                searchParms.IssuerId = int.Parse(this.ddlIssuer.SelectedValue);

                SearchParameters = searchParms;

                DisplayResults(searchParms, 1, null);
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
        protected void dlBatchList_ItemDataBound(object sender, DataListItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item ||
            e.Item.ItemType == ListItemType.AlternatingItem)
            {

                CheckBox tb = e.Item.FindControl("chksel") as CheckBox;

                if (tb != null)
                {
                    if (showcheckbox)
                        tb.Visible = true;
                    else
                        tb.Visible = false;

                }

                //ImageButton imgreport = e.Item.FindControl("imgreport") as ImageButton;

                //if (imgreport != null)
                //{
                //    if (showreport)
                //        imgreport.Visible = true;
                //    else
                //        imgreport.Visible = false;

                //}
            }

            if (e.Item.ItemType == ListItemType.Header)
            {
                Label tb = e.Item.FindControl("lblSelect") as Label;

                if (tb != null)
                {
                    if (showcheckbox)
                        tb.Visible = true;
                    else
                        tb.Visible = false;

                }
            }


        }
        protected void dlBatchList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlBatchList.SelectedIndex = e.Item.ItemIndex;
                string ThreedBatchIdStr = ((Label)dlBatchList.SelectedItem.FindControl("lblThreedBatchId")).Text;

                long ThreedBatchId;
                if (e.CommandName.ToUpper() == "REPORT")
                {
                    long.TryParse(ThreedBatchIdStr, out ThreedBatchId);
                    var reportBytes = _batchService.GenerateLoadBatchReport(ThreedBatchId);

                    string reportName = String.Empty;

                    reportName += ThreedBatchId + "_" + DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(reportBytes);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.Flush();
                }
                else
                    if (long.TryParse(ThreedBatchIdStr, out ThreedBatchId))
                {
                    var searchParms = SearchParameters;
                    searchParms.PageIndex = PageIndex;
                    SessionWrapper.ThreedBatchSearchParams = (ThreedBatchSearchParameters)searchParms;
                    SessionWrapper.ThreedBatchId = ThreedBatchId;

                    Server.Transfer("~\\webpages\\ThreeDSecure\\ThreeDSecureView.aspx");
                }
                else
                {
                    this.lblInfoMessage.Text = "The select item does not contain a valid load batch.";
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

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            SearchParameters.PageIndex = 1;
            SessionWrapper.ThreedBatchSearchParams = (ThreedBatchSearchParameters)SearchParameters;

            Server.Transfer("~\\webpages\\ThreeDSecure\\ThreeDSecureSearch.aspx");
        }

        #region Pagination
  
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.dlBatchList.DataSource = null;

            var threedBatchSearchParms = (ThreedBatchSearchParameters)parms;

            if (results == null)
            {
                results = _batchService.GetThreedBatches(threedBatchSearchParms, pageIndex).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlBatchList.DataSource = results;
                TotalPages = ((ThreedBatchResult)results[0]).TOTAL_PAGES;
                List<ThreedBatchResult> list = results.Cast<ThreedBatchResult>().ToList();
                EnableButton(list);
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                pnlremarks.Visible = false;
                btnApprove.Visible = false;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlBatchList.DataBind();
        }
        #endregion

        #region "Private Methods"
        private void EnableButton(List<ThreedBatchResult> list)
        {
            btnApprove.Visible = false;
            pnlremarks.Visible = false;

            long? threed_batch_statuses_id = list[0].threed_batch_statuses_id;

            int count = list.FindAll(i => i.threed_batch_statuses_id == threed_batch_statuses_id).Count;
            if (list.Count == count)
            {

                showcheckbox = true;

            }
            if (list.Count > 0)
            {
                if (showcheckbox)
                {
                    SetButtons(list[0]);

                }
            }


        }
        private void SetButtons(ThreedBatchResult ThreedBatch)
        {
            btnApprove.Visible = false;
            showcheckbox = false;

            pnlremarks.Visible = false;
            List<RolesIssuerResult> issuer;
            Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.CENTER_MANAGER, out issuer);
            //If a load batch has been passed set the buttons accordingly.
            if (ThreedBatch != null)
            {
                if (issuer != null && issuer.Count > 0)
                {
                    bool canUpdate;
                    bool canRead;
                    bool canCreate;
                    if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_MANAGER, out canRead, out canUpdate, out canCreate))
                    {
                        if (ThreedBatch != null && canUpdate)
                        {
                            btnApprove.Visible = true;
                            showcheckbox = true;

                            pnlremarks.Visible = true;
                        }

                    }
                }
            }
        }


        protected void btnApprove_Click(object sender, EventArgs e)
        {

            ArrayList ThreedbatchId = new ArrayList();

            foreach (DataListItem item in dlBatchList.Items)
            {
                CheckBox chk = item.FindControl("chksel") as CheckBox;

                string ThreedBatchIdstr = ((Label)item.FindControl("lblThreedBatchId")).Text;

                if (chk.Checked)
                {
                    long _ThreedBatchId;
                    long.TryParse(ThreedBatchIdstr, out _ThreedBatchId);
                    ThreedbatchId.Add(_ThreedBatchId);
                }

            }
            if (ThreedbatchId.Count > 0)
            {
                string messages = string.Empty;
                if (_batchService.ApproveMultipleLoadBatch(ThreedbatchId, tbStatusNote.Text, out messages))
                {

                    DisplayResults(SearchParameters, SearchParameters.PageIndex, null);
                }
                lblInfoMessage.Text = messages;
            }

        }
        #endregion


    }
}
