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
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;

namespace indigoCardIssuingWeb.webpages.hybrid
{
    public partial class PrintBatchList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PrintBatchList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_PRODUCT_MANAGER,
                                                                        UserRole.BRANCH_PRODUCT_OPERATOR,
                                                                        UserRole.CENTER_MANAGER,
                                                                        UserRole.CMS_OPERATOR};

        private readonly IssuerManagementService _issuermanService = new IssuerManagementService();
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
                if (SessionWrapper.PrintBatchSearchResult != null && SessionWrapper.PrintBatchSearchResult != null)
                {
                    var results = SessionWrapper.PrintBatchSearchResult.ToArray();
                    SearchParameters = SessionWrapper.PrintBatchSearchParams;

                    DisplayResults(SearchParameters, SearchParameters.PageIndex, results);

                    SessionWrapper.PrintBatchSearchResult = null;
                    SessionWrapper.PrintBatchSearchParams = null;
                }
                //else 
                if (SessionWrapper.PrintBatchSearchParams != null)
                {
                    PrintBatchSearchParameters searchParms = SessionWrapper.PrintBatchSearchParams;
                    SearchParameters = searchParms;

                    DisplayResults(searchParms, searchParms.PageIndex, null);

                    SessionWrapper.PrintBatchSearchParams = null;
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
            SessionWrapper.PrintBatchSearchParams = (PrintBatchSearchParameters)SearchParameters;
            Server.Transfer("~\\webpages\\hybrid\\PrintBatchSearch.aspx");
        }
        #endregion

        #region LIST SELECTION ACTION

       
        protected void dlPrintList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlPrintList.SelectedIndex = e.Item.ItemIndex;
                string printBatchIdStr = ((Label)dlPrintList.SelectedItem.FindControl("lblPrintBatchId")).Text;

                long printBatchId;
                if (e.CommandName.ToUpper() == "REPORT")
                {
                    long.TryParse(printBatchIdStr, out printBatchId);
                    var reportBytes = _issuermanService.GeneratePrintBatchReport(printBatchId);

                    string reportName = String.Empty;

                    reportName += printBatchId + "_" + DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(reportBytes);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.Flush();
                }
                else
                    if (long.TryParse(printBatchIdStr, out printBatchId))
                {
                    var searchParms = SearchParameters;
                    searchParms.PageIndex = PageIndex;

                    SessionWrapper.PrintBatchSearchParams = (PrintBatchSearchParameters)searchParms;
                    SessionWrapper.PrintBatchId = printBatchId;

                    Server.Transfer("~\\webpages\\hybrid\\PrintBatchView.aspx");
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

        private void SetButtons(PrintBatchResult printBatch)
        {


            //disable all buttons by default.
            //btnApprove.Visible = false;
            //btnApprove.Visible = false;
            showcheckbox = false;
            //showreport = false;
            pnlremarks.Visible = false;
            //List<RolesIssuerResult> issuer;
            //Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.BRANCH_PRODUCT_MANAGER, out issuer);
            ////If a batch has been passed set the buttons accordingly.
            //if (printBatch != null)
            //{
               
            //    if (issuer != null && issuer.Count > 0)
            //    {
            //        bool canUpdate;
            //        bool canRead;
            //        bool canCreate;
            //        if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_PRODUCT_MANAGER, printBatch.issuer_id, out canRead, out canUpdate, out canCreate))
            //        {

            //            if (canUpdate)
            //            {
            //                //this.btnApprove.Text = GetGlobalResourceObject("PinBatchStatusesButton", String.Format("PinBtn{0}", pinBatch.flow_pin_batch_statuses_id)).ToString();
            //                //this.btnApprove.Visible = true;
            //                //showcheckbox = true;
            //                //showreport = true;



            //            }
            //        }
            //    }

            //}


        }
        private void EnableButton(List<PrintBatchResult> list)
        {

            //btnApprove.Visible = false;
            pnlremarks.Visible = false;

            long? print_batch_statuses_id = list[0].print_batch_statuses_id;

            int count = list.FindAll(i => i.print_batch_statuses_id == print_batch_statuses_id).Count;
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
                dlPrintList.DataSource = result.BatchList;
            }
            else
            {
                var batches = new List<PINBatch>();
                batches.Add(result.Batch);
                dlPrintList.DataSource = batches;
            }

            dlPrintList.DataBind();
            SessionWrapper.PINSearchResult = null;
        }

        private void DisplayBatchListForUser()
        {
            dlPrintList.DataSource = null;

            try
            {
                if (SessionWrapper.PrintBatchSearchParams == null)
                {
                    int? searchStatusId = null;
                    int? cardIssueMethodId = null;

                    if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
                    {
                        searchStatusId = int.Parse(Request.QueryString["status"]);
                    }

                    if (!String.IsNullOrWhiteSpace(Request.QueryString["issueMethod"]))
                    {
                        cardIssueMethodId = int.Parse(Request.QueryString["issueMethod"]);
                    }

                
                    PrintBatchSearchParameters searchParams = new PrintBatchSearchParameters(null, searchStatusId, null, null,null, cardIssueMethodId, null, null, 1);
                    SearchParameters = searchParams;
                    DisplayResults(searchParams, searchParams.PageIndex, null);
                }
                else
                {
                    SearchParameters = SessionWrapper.PrintBatchSearchParams;
                    SessionWrapper.PrintBatchSearchParams = null;
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
            this.dlPrintList.DataSource = null;

            var searchparms = (PrintBatchSearchParameters)parms;

            if (results == null)
            {
                results = _issuermanService.GetPrintBatchesForUser(searchparms, PageIndex).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlPrintList.DataSource = results;
                TotalPages = ((PrintBatchResult)results[0]).TOTAL_PAGES;
                List<PrintBatchResult> list = results.Cast<PrintBatchResult>().ToList();
                EnableButton(list);
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                //btnApprove.Visible = false;
                pnlremarks.Visible = false;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlPrintList.DataBind();
        }
        #endregion

        #region Page Events
        


        protected void dlPrintList_ItemDataBound(object sender, DataListItemEventArgs e)
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