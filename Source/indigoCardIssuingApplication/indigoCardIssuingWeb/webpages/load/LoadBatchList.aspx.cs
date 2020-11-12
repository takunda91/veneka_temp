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

namespace indigoCardIssuingWeb.webpages.load
{
    public partial class LoadBatchList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoadBatchList));
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER, UserRole.CARD_PRODUCTION };
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
                if (SessionWrapper.LoadBatchSearchResult != null && SessionWrapper.LoadBatchSearchParams != null)
                {
                    var results = SessionWrapper.LoadBatchSearchResult.ToArray();
                    var searchParms = SessionWrapper.LoadBatchSearchParams;
                    SearchParameters = searchParms;

                    ddlIssuer.SelectedValue = searchParms.IssuerId.ToString();
                    this.ddlIssuer.Visible = searchParms.IsFromSearch ? false : true;
                    this.lblIssuer.Visible = searchParms.IsFromSearch ? false : true;

                    DisplayResults(searchParms, searchParms.PageIndex, results);

                    SessionWrapper.LoadBatchSearchParams = null;
                    SessionWrapper.LoadBatchSearchResult = null;
                }
                else if (SessionWrapper.LoadBatchSearchParams != null)
                {
                    LoadBatchSearchParameters searchParms = SessionWrapper.LoadBatchSearchParams;
                    SearchParameters = searchParms;
                    this.ddlIssuer.SelectedValue = searchParms.IssuerId.ToString();

                    this.ddlIssuer.Visible = searchParms.IsFromSearch ? false : true;
                    this.lblIssuer.Visible = searchParms.IsFromSearch ? false : true;
                    ddlIssuer.SelectedValue = searchParms.IssuerId.ToString();

                    DisplayResults(searchParms, searchParms.PageIndex, null);

                    SessionWrapper.LoadBatchSearchParams = null;
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
                this.pnlButtons.Visible = false;
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

                LoadBatchSearchParameters searchParms = new LoadBatchSearchParameters(null, int.Parse(this.ddlIssuer.SelectedValue),
                                                                                      LoadBatchStatus.LOADED, null, null, false, 1);
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
                LoadBatchSearchParameters searchParms = (LoadBatchSearchParameters)SearchParameters;
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

        protected void dlBatchList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlBatchList.SelectedIndex = e.Item.ItemIndex;
                string loadBatchIdStr = ((Label)dlBatchList.SelectedItem.FindControl("lblLoadBatchId")).Text;

                long loadBatchId;
                if (e.CommandName.ToUpper() == "REPORT")
                {
                    long.TryParse(loadBatchIdStr, out loadBatchId);
                    var reportBytes = _batchService.GenerateLoadBatchReport(loadBatchId);

                    string reportName = String.Empty;

                    reportName += loadBatchId + "_" + DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(reportBytes);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.Flush();
                }
                else
                    if (long.TryParse(loadBatchIdStr, out loadBatchId))
                    {
                        var searchParms = SearchParameters;
                        searchParms.PageIndex = PageIndex;
                        SessionWrapper.LoadBatchSearchParams = (LoadBatchSearchParameters)searchParms;
                        SessionWrapper.loadBatchId = loadBatchId;

                        Server.Transfer("~\\webpages\\load\\LoadBatchView.aspx");
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
        #endregion

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            SearchParameters.PageIndex = 1;
            SessionWrapper.LoadBatchSearchParams = (LoadBatchSearchParameters)SearchParameters;

            Server.Transfer("~\\webpages\\load\\LoadBatchSearch.aspx");
        }

        #region Pagination
        protected void btnApprove_Click(object sender, EventArgs e)
        {
          
            ArrayList loadbatchId = new ArrayList();
           
            foreach (DataListItem item in dlBatchList.Items)
            {
                CheckBox chk = item.FindControl("chksel") as CheckBox;

                string LoadBatchIdstr = ((Label)item.FindControl("lblLoadBatchId")).Text;

                if (chk.Checked)
                {
                    long _distBatchId;
                    long.TryParse(LoadBatchIdstr, out _distBatchId);
                    loadbatchId.Add(_distBatchId);
                }

            }
            if (loadbatchId.Count > 0)
            {
                string messages = string.Empty;
                if(_batchService.ApproveMultipleLoadBatch(loadbatchId, tbStatusNote.Text, out messages))
                {
                   
                    DisplayResults(SearchParameters, SearchParameters.PageIndex, null);
                }
                lblInfoMessage.Text = messages;
            }

        }
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.dlBatchList.DataSource = null;

            var loadBatchSearchParms = (LoadBatchSearchParameters)parms;

            if (results == null)
            {
                results = _batchService.GetLoadBatches(loadBatchSearchParms, pageIndex).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlBatchList.DataSource = results;
                TotalPages = ((LoadBatchResult)results[0]).TOTAL_PAGES;
                List<LoadBatchResult> list = results.Cast<LoadBatchResult>().ToList();
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
        private void EnableButton(List<LoadBatchResult> list)
        {
            btnApprove.Visible = false;
            pnlremarks.Visible = false;

            long? load_batch_statuses_id = list[0].load_batch_statuses_id;

            int count = list.FindAll(i => i.load_batch_statuses_id == load_batch_statuses_id).Count;
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
        private void SetButtons(LoadBatchResult loadBatch)
        {
            btnApprove.Visible = false;
            showcheckbox = false;
          
            pnlremarks.Visible = false;
            List<RolesIssuerResult> issuer;
            Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.CENTER_MANAGER, out issuer);
            //If a load batch has been passed set the buttons accordingly.
            if (loadBatch != null)
            {
                if (issuer != null && issuer.Count > 0)
                {
                    bool canUpdate;
                    bool canRead;
                    bool canCreate;
                    if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_MANAGER, out canRead, out canUpdate, out canCreate))
                    {
                        if (loadBatch != null && canUpdate &&
                             (LoadBatchStatus)loadBatch.load_batch_statuses_id == LoadBatchStatus.LOADED)
                        {
                            btnApprove.Visible = true;
                            showcheckbox = true;
                        
                            pnlremarks.Visible = true;
                        }

                    }
                }
            }
        }
        #endregion

       
    }
}
