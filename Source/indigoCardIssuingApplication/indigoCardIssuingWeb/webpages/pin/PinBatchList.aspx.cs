using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb;
using indigoCardIssuingWeb.CardIssuanceService;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using indigoCardIssuingWeb.SearchParameters;
using System.IO;
using System.Collections;
using System.Linq;
using System.Web.Security;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class PinBatchList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PinBatchList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.PIN_PRINTER_OPERATOR,
                                                                        UserRole.CARD_CENTRE_PIN_OFFICER,
                                                                        UserRole.BRANCH_PIN_OFFICER,
                                                                        UserRole.CENTER_MANAGER,
                                                                        UserRole.CMS_OPERATOR};

        private readonly PINManagementService _pinService = new PINManagementService();
        private SystemAdminService sysAdminService = new SystemAdminService();
        private bool showcheckbox = false;
        //private bool showreport = false;
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
                if (SessionWrapper.PinBatchSearchResult != null && SessionWrapper.PinBatchSearchParams != null)
                {
                    var results = SessionWrapper.PinBatchSearchResult.ToArray();
                    SearchParameters = SessionWrapper.PinBatchSearchParams;

                    DisplayResults(SearchParameters, SearchParameters.PageIndex, results);

                    SessionWrapper.PinBatchSearchParams = null;
                    SessionWrapper.PinBatchSearchResult = null;
                }
                else if (SessionWrapper.PinBatchSearchParams != null)
                {
                    PinBatchSearchParameters searchParms = SessionWrapper.PinBatchSearchParams;
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
            SessionWrapper.PinBatchSearchParams = (PinBatchSearchParameters)SearchParameters;
            Server.Transfer("~\\webpages\\pin\\PinBatchSearch.aspx");
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
                string pinBatchIdStr = ((Label)dlBatchList.SelectedItem.FindControl("lblPinBatchId")).Text;

                long pinBatchId;
                if (e.CommandName.ToUpper() == "REPORT")
                {
                    long.TryParse(pinBatchIdStr, out pinBatchId);
                    var reportBytes = _pinService.GeneratePinBatchReport(pinBatchId);

                    string reportName = String.Empty;

                    reportName += pinBatchId + "_" + DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(reportBytes);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.Flush();
                }
                else
                    if (long.TryParse(pinBatchIdStr, out pinBatchId))
                    {
                        var searchParms = SearchParameters;
                        searchParms.PageIndex = PageIndex;

                        SessionWrapper.PinBatchSearchParams = (PinBatchSearchParameters)searchParms;
                        SessionWrapper.PinBatchId = pinBatchId;

                        Server.Transfer("~\\webpages\\pin\\PinBatchView.aspx");
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

        private void SetButtons(PinBatchResult pinBatch)
        {


            //disable all buttons by default.
            btnApprove.Visible = false;
            btnApprove.Visible = false;
            showcheckbox = false;
            //showreport = false;
            pnlremarks.Visible = false;
            List<RolesIssuerResult> issuer;
            Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.CENTER_MANAGER, out issuer);
            //If a batch has been passed set the buttons accordingly.
            if (pinBatch != null)
            {
                bool canUpdate;
                bool canRead;
                bool canCreate;
                if (issuer != null && issuer.Count > 0)
                {

                    if (pinBatch.user_role_id != null &&
                        pinBatch.flow_pin_batch_statuses_id != null && pinBatch.flow_pin_batch_statuses_id == 11 &&
                        PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CARD_CENTRE_PIN_OFFICER, pinBatch.issuer_id, out canRead, out canUpdate, out canCreate))
                    {

                        if (canUpdate)
                        {
                            this.btnApprove.Text = GetGlobalResourceObject("PinBatchStatusesButton", String.Format("PinBtn{0}", pinBatch.flow_pin_batch_statuses_id)).ToString();
                            this.btnApprove.Visible = true;
                            showcheckbox = true;
                            //showreport = true;
                            


                        }
                    }
                }
             
            }


        }
        private void EnableButton(List<PinBatchResult> list)
        {

            btnApprove.Visible = false;
            pnlremarks.Visible = false;

            long? pin_batch_statuses_id = list[0].pin_batch_statuses_id;

            int count = list.FindAll(i => i.pin_batch_statuses_id == pin_batch_statuses_id).Count;
            if (list.Count == count)
            {

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
        private void DisplaySearchResult()
        {
            PINSearchResult result = SessionWrapper.PINSearchResult;

            if (result.BatchList != null)
            {
                dlBatchList.DataSource = result.BatchList;
            }
            else
            {
                var batches = new List<PINBatch>();
                batches.Add(result.Batch);
                dlBatchList.DataSource = batches;
            }

            dlBatchList.DataBind();
            SessionWrapper.PINSearchResult = null;
        }

        private void DisplayBatchListForUser()
        {
            dlBatchList.DataSource = null;

            try
            {
                if (SessionWrapper.PinBatchSearchParams == null)
                {
                    int? searchStatusId = null;
                    int? cardIssueMethodId = null;
                    int? pinBatchTypeId = null;

                    if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
                    {
                        searchStatusId = int.Parse(Request.QueryString["status"]);
                    }

                    if (!String.IsNullOrWhiteSpace(Request.QueryString["issueMethod"]))
                    {
                        cardIssueMethodId = int.Parse(Request.QueryString["issueMethod"]);
                    }

                    if (!String.IsNullOrWhiteSpace(Request.QueryString["pinBatchTypeId"]))
                    {
                        pinBatchTypeId = int.Parse(Request.QueryString["pinBatchTypeId"]);
                    }
                    PinBatchSearchParameters searchParams = new PinBatchSearchParameters(null, searchStatusId, null, null, cardIssueMethodId, pinBatchTypeId, null, null, 1);
                    SearchParameters = searchParams;
                    DisplayResults(searchParams, searchParams.PageIndex, null);
                }
                else
                {
                    SearchParameters = SessionWrapper.PinBatchSearchParams;
                    SessionWrapper.PinBatchSearchParams = null;
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
            this.dlBatchList.DataSource = null;

            var searchparms = (PinBatchSearchParameters)parms;

            if (results == null)
            {
                results = _pinService.GetPinBatchesForUser(searchparms, PageIndex).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlBatchList.DataSource = results;
                TotalPages = ((PinBatchResult)results[0]).TOTAL_PAGES;
                List<PinBatchResult> list = results.Cast<PinBatchResult>().ToList();
                EnableButton(list);
            }
            else
            {
                TotalPages = 0;
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
          
            ArrayList PinBatchId = new ArrayList();
            ArrayList pinBatchStatusId = new ArrayList();
            ArrayList flowPinBatchId = new ArrayList();
           
            string notes = tbStatusNote.Text;
            foreach (DataListItem item in dlBatchList.Items)
            {
                CheckBox chk = item.FindControl("chksel") as CheckBox;

                string PinBatchIdstr = ((Label)item.FindControl("lblPinBatchId")).Text;
                string pinBatchStatusIdstr = ((Label)item.FindControl("lblPinBatchStatusId")).Text;
                string flowPinBatchIdstr = ((Label)item.FindControl("lblFlowPinBatchId")).Text;

                if (chk.Checked)
                {
                    long _PinBatchId;
                    int _flowDistBatchId, _distBatchStatusId;
                    long.TryParse(PinBatchIdstr, out _PinBatchId);
                    int.TryParse(flowPinBatchIdstr, out _flowDistBatchId);
                    int.TryParse(pinBatchStatusIdstr, out _distBatchStatusId);

                    PinBatchId.Add(_PinBatchId);
                    pinBatchStatusId.Add( _distBatchStatusId);
                    flowPinBatchId.Add( _flowDistBatchId);
                   


                }

            }
            if (PinBatchId.Count > 0)
            {
                string messages = string.Empty;
                if(_pinService.UpdateMuiltplePinBatchStatus(PinBatchId, pinBatchStatusId, flowPinBatchId, notes, out messages))
                {
                  
                    DisplayResults(SearchParameters, SearchParameters.PageIndex, null);
                }

                lblInfoMessage.Text = messages;
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