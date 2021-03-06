﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Linq;
using System.Web.Security;
using System.Configuration;

namespace indigoCardIssuingWeb.webpages.dist
{
    public partial class DistBatchList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DistBatchList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.CARD_PRODUCTION,
                                                                        UserRole.PIN_PRINTER_OPERATOR,
                                                                        UserRole.BRANCH_PRODUCT_MANAGER, //add BPM
                                                                        UserRole.AUDITOR};

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private SystemAdminService sysAdminService = new SystemAdminService();
        private bool showcheckbox = false;
        //private bool showreport = false;
        public bool autogenerateDistBatch = true;

        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
                if (ConfigurationManager.AppSettings["AutoCreatedistbatch"] != null)
                {
                    autogenerateDistBatch = bool.Parse(ConfigurationManager.AppSettings["AutoCreatedistbatch"].ToString());
                }
            }
        }

        private void LoadPageData()
        {
            try
            {
                if (SessionWrapper.DistributionBatchSearchResult != null && SessionWrapper.DistBatchSearchParams != null)
                {
                    var results = SessionWrapper.DistributionBatchSearchResult.ToArray();
                    SearchParameters = SessionWrapper.DistBatchSearchParams;

                    DisplayResults(SearchParameters, SearchParameters.PageIndex, results);

                    SessionWrapper.DistBatchSearchParams = null;
                    SessionWrapper.DistributionBatchSearchResult = null;
                }
                else if (SessionWrapper.DistBatchSearchParams != null)
                {
                    DistBatchSearchParameters searchParms = SessionWrapper.DistBatchSearchParams;
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
                string distBatchIdStr = ((Label)dlBatchList.SelectedItem.FindControl("lblDistBatchId")).Text;

                long distBatchId;
                if (e.CommandName.ToUpper() == "REPORT")
                {
                    long.TryParse(distBatchIdStr, out distBatchId);
                    var reportBytes = _batchService.GenerateDistBatchReport(distBatchId);

                    string reportName = String.Empty;

                    reportName += distBatchId + "_" + DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(reportBytes);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.Flush();
                }
                else
                    if (long.TryParse(distBatchIdStr, out distBatchId))
                {
                    var searchParms = (DistBatchSearchParameters)SearchParameters;
                    searchParms.PageIndex = PageIndex;

                    SessionWrapper.DistBatchSearchParams = searchParms;
                    SessionWrapper.DistBatchId = distBatchId;

                    //Server.Transfer("~\\webpages\\card\\DistBatchView.aspx");
                    Response.Redirect("~\\webpages\\dist\\DistBatchView.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    this.lblInfoMessage.Text = "The select item does not contain a valid dist batch.";
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
        private void SetButtons(DistBatchResult distBatch)
        {

            //showreport = false;
            showcheckbox = false;
            btnApprove.Visible = false;
            pnlremarks.Visible = false;
            //If a load batch has been passed set the buttons accordingly.
            if (distBatch != null)
            {
                //bool canUpdate=false;
                //bool canRead;
                //bool canCreate;
                List<RolesIssuerResult> issuer;
                Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.CENTER_MANAGER, out issuer);
                List<RolesIssuerResult> issuer_op;
                Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.CENTER_OPERATOR, out issuer_op);

                if ((issuer != null && issuer.Count > 0) || (issuer_op != null && issuer_op.Count > 0))
                {
                    if (distBatch.user_role_id != null &&
                        distBatch.flow_dist_batch_statuses_id != null && ((distBatch.flow_dist_batch_statuses_id == 1 || distBatch.flow_dist_batch_statuses_id == 9)
                        || (distBatch.flow_dist_batch_statuses_id == 2 || distBatch.flow_dist_batch_statuses_id == 19))
                        //PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_MANAGER, distBatch.issuer_id, out canRead, out canUpdate, out canCreate)
                        )
                    {
                        this.btnApprove.Text = GetGlobalResourceObject("DistBatchStatusButtons", String.Format("Btn{0}", distBatch.flow_dist_batch_statuses_id)).ToString();
                        this.btnApprove.Visible = true;
                        pnlremarks.Visible = true;
                        showcheckbox = true;
                    }
                }

            }
        }
        private void DisplayBatchListForUser()
        {
            dlBatchList.DataSource = null;

            try
            {
                int? searchStatus = null;
                int? cardIssueMethodId = null;
                int? distBatchTypeId = null;
                int? flowstatus = null;

                if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
                {
                    searchStatus = int.Parse(Request.QueryString["status"]);
                }
                if (!String.IsNullOrWhiteSpace(Request.QueryString["flowstatus"]))
                {
                    flowstatus = int.Parse(Request.QueryString["flowstatus"]);
                }

                if (!String.IsNullOrWhiteSpace(Request.QueryString["issueMethod"]))
                {
                    cardIssueMethodId = int.Parse(Request.QueryString["issueMethod"]);
                }

                if (!String.IsNullOrWhiteSpace(Request.QueryString["distBatchTypeId"]))
                {
                    distBatchTypeId = int.Parse(Request.QueryString["distBatchTypeId"]);
                }


                // Change page name to correspond with the menu labels 

                if (searchStatus.Equals(0) && distBatchTypeId.Equals(0))
                    lblDistributionBatchList.Text = Resources.SubMenu.SubMenuId9;
                if ((searchStatus.Equals(1) || searchStatus.Equals(13)) && distBatchTypeId.Equals(0))
                    lblDistributionBatchList.Text = Resources.SubMenu.SubMenuId14;
                else if (searchStatus.Equals(19) && cardIssueMethodId.Equals(1) && distBatchTypeId.Equals(1))
                    lblDistributionBatchList.Text = Resources.SubMenu.SubMenuId20;
                else if (searchStatus.Equals(11) && cardIssueMethodId.Equals(1) && distBatchTypeId.Equals(0))
                    lblDistributionBatchList.Text = Resources.SubMenu.SubMenuId1;
                else if ((searchStatus.Equals(9) || searchStatus.Equals(18)) && distBatchTypeId.Equals(0))
                    lblDistributionBatchList.Text = Resources.SubMenu.SubMenuId10;
                else if (searchStatus.Equals(10) && distBatchTypeId.Equals(0))
                    lblDistributionBatchList.Text = Resources.SubMenu.SubMenuId11;
                else if ((searchStatus.Equals(2) || searchStatus.Equals(13)) && distBatchTypeId.Equals(1))
                    lblDistributionBatchList.Text = "Receive Batches";
                else
                    lblDistributionBatchList.Text = Resources.CommonLabels.DistributionBatchList;


                DistBatchSearchParameters searchParams = new DistBatchSearchParameters(null, null, searchStatus, flowstatus, null, null, cardIssueMethodId, distBatchTypeId, null, null, false, 1);
                SearchParameters = searchParams;
                //var distBatches = _batchService.GetDistBatchesForUser(searchParams, searchParams.PageIndex);

                DisplayResults(searchParams, searchParams.PageIndex, null);

                //if (distBatches != null && distBatches.Count > 0)
                //{
                //    dlBatchList.DataSource = distBatches;
                //    ViewState[DistBatchListKey] = distBatches;
                //}
                //else
                //{
                //    this.lblErrorMessage.Text = "There are no distribution batches awaiting approval";
                //}
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

        private void EnableButton(List<DistBatchResult> list)
        {

            btnApprove.Visible = false;
            pnlremarks.Visible = false;

            long? dist_batch_Statues_id = list[0].dist_batch_statuses_id;

            int count = list.FindAll(i => i.dist_batch_statuses_id == dist_batch_Statues_id).Count;
            if (list.Count == count)
            {
                //showreport = true;
                showcheckbox = true;
            }
            if (list.Count() > 0)
            {
                if (showcheckbox)
                {
                    SetButtons(list[0]);
                }

            }

        }
        #endregion


        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            SessionWrapper.DistBatchSearchParams = (DistBatchSearchParameters)SearchParameters;
            Server.Transfer("~\\webpages\\dist\\DistBatchSearch.aspx");
        }

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = "";
            this.dlBatchList.DataSource = null;

            var searchParm = (DistBatchSearchParameters)parms;


            if (searchParm.DistBatchTypeId != null && searchParm.DistBatchTypeId == 0)
                lblDistributionBatchList.Text = Resources.CommonLabels.ProductionBatchList;
            else if (searchParm.DistBatchTypeId != null && searchParm.DistBatchTypeId == 1)
                lblDistributionBatchList.Text = Resources.CommonLabels.DistributionBatchList;

            if (results == null)
            {
                results = _batchService.GetDistBatchesForUser(searchParm, PageIndex).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlBatchList.DataSource = results;
                TotalPages = ((DistBatchResult)results[0]).TOTAL_PAGES;
                List<DistBatchResult> list = results.Cast<DistBatchResult>().ToList();
                EnableButton(list);

            }
            else
            {
                TotalPages = 1;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                btnApprove.Visible = false;
                pnlremarks.Visible = false;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlBatchList.DataBind();
        }
        #endregion

        #region Page Events
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {

                ArrayList distbatchId = new ArrayList();
                ArrayList distbatchStatusId = new ArrayList();
                ArrayList flowdisbatchstausId = new ArrayList();
                string notes = tbStatusNote.Text;
                foreach (DataListItem item in dlBatchList.Items)
                {
                    CheckBox chk = item.FindControl("chksel") as CheckBox;

                    string distBatchIdStr = ((Label)item.FindControl("lblDistBatchId")).Text;
                    string distBatchStatusId = ((Label)item.FindControl("lblDistBatchStatusId")).Text;
                    string flowDistBatchId = ((Label)item.FindControl("lblFlowDistBatchId")).Text;

                    if (chk.Checked)
                    {
                        long _distBatchId;
                        int _flowDistBatchId, _distBatchStatusId;
                        long.TryParse(distBatchIdStr, out _distBatchId);
                        int.TryParse(flowDistBatchId, out _flowDistBatchId);
                        int.TryParse(distBatchStatusId, out _distBatchStatusId);

                        distbatchId.Add(_distBatchId);
                        distbatchStatusId.Add(_distBatchStatusId);

                        flowdisbatchstausId.Add(_flowDistBatchId);


                    }

                }
                if (distbatchId.Count > 0)
                {
                    string messages = string.Empty;
                    if (_batchService.MultipleDistBatchChangeStatus(distbatchId, distbatchStatusId, flowdisbatchstausId, autogenerateDistBatch, notes, out messages))
                    {

                        DisplayResults(SearchParameters, SearchParameters.PageIndex, null);
                    }
                    lblInfoMessage.Text = messages;
                    tbStatusNote.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
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

    }
}
