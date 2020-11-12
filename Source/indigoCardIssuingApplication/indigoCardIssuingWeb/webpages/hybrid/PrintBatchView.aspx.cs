using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.UI;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.constants;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CardIssuanceService;
using System.Threading;
using System.Globalization;
using Common.Logging;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;

namespace indigoCardIssuingWeb.webpages.hybrid
{
    public partial class PrintBatchView : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PrintBatchView));

        private readonly IssuerManagementService _issuerman = new IssuerManagementService();
        private UserManagementService _userMan = new UserManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_PRODUCT_MANAGER,
                                                                        UserRole.BRANCH_PRODUCT_OPERATOR,
                                                                        UserRole.CENTER_MANAGER,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.AUDITOR};
        private enum PageLayout
        {
            READ,
            REJECT_CONFIRM,
            STATUS_CONFIRM,
            SPECIAL_CONFIRM,
            SPOIL_CONFIRM
        }

        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData(null);
                //CurrentDisPageLayout = DistPageLayout.READ;
            }
        }
        #endregion

        /// <summary>
        /// Use this method to initially load the page.
        /// </summary>
        /// <param name="printBatchId"></param>
        private void LoadPageData(long? printBatchId)
        {
            try
            {
                long? print_BatchId = null;
                if (SessionWrapper.PrintBatchId != null)
                {
                    print_BatchId = SessionWrapper.PrintBatchId;
                    SessionWrapper.PrintBatchId = null;
                }
                if (SessionWrapper.PrintBatchSearchParams != null)
                {
                    SearchParam = SessionWrapper.PrintBatchSearchParams;
                    SessionWrapper.PrintBatchSearchParams = null;
                }

                //See if theres anything in the session.


                PrintBatchResult printBatch = null;

                if (print_BatchId != null)
                {
                    printBatch = PopulatePage(print_BatchId);

                    if (printBatch != null)
                    {
                        SetButtons(printBatch);
                        PrintBatchId = printBatch.print_batch_id;
                    }


                }

                if (PrintBatchId == null)
                {
                    this.lblErrorMessage.Text = "No pin batch to show.";
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        /// <summary>
        /// Use this method to populate the page and set the pages buttons according to batch status and user role. 
        /// This method does a call to the DB to get latest batch information if the batch id is not null.
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <returns></returns>
        private PrintBatchResult PopulatePage(long? printBatchId)
        {
            PrintBatchResult printBatch = null;

            if (printBatchId != null)
            {
                printBatch = _issuerman.GetPrintBatch(printBatchId.Value);
            }

            BatchResult = printBatch;
            PopulatePage(printBatch);
            return printBatch;
        }

        /// <summary>
        /// Use this method to set page controls bassed on the page layout.
        /// </summary>
        /// <param name="disPageLayout"></param>
        private void UpdatePageLayout(PageLayout? pageLayout)
        {
            if (pageLayout == null)
            {
                pageLayout = CurrentPageLayout.GetValueOrDefault();
            }

            switch (pageLayout)
            {
                case PageLayout.READ:
                    this.tbStatusNote.Enabled = false;
                    PopulatePage(PrintBatchId);
                    break;
                case PageLayout.STATUS_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    RejectConfirmation();
                    break;
                case PageLayout.SPECIAL_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    RejectConfirmation();
                    break;
                case PageLayout.REJECT_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    RejectConfirmation();
                    break;
                default:
                    break;

            }

            CurrentPageLayout = pageLayout;
        }

        /// <summary>
        /// Use this method to populate the page and set the pages buttons according to batch status and user role.         
        /// </summary>
        /// <param name="distBatch"></param>
        /// <returns></returns>
        private PrintBatchResult PopulatePage(PrintBatchResult _printBatch)
        {
            SetButtons(_printBatch);

            if (_printBatch != null)
            {
                this.tbIssueMethod.Text = _printBatch.card_issue_method_name;
                this.tbBatchCreatedDate.Text = Convert.ToDateTime(_printBatch.date_created).ToString(DATETIME_FORMAT);
                this.tbBatchReference.Text = _printBatch.print_batch_reference;
                this.tbBatchStatus.Text = _printBatch.print_batch_status_name;
                this.tbNumberOfCards.Text = _printBatch.no_cards.ToString();
                this.tbStatusNote.Text = _printBatch.status_notes;
                this.tbIssuerName.Text = base.FormatNameAndCode(_printBatch.issuer_name, _printBatch.issuer_code);

                if (_printBatch.branch_code != null)
                    this.tbBranchName.Text = base.FormatNameAndCode(_printBatch.branch_name, _printBatch.branch_code);
                else
                    this.tbBranchName.Text = "-";
            }
            else
            {
                this.lblInfoMessage.Text = "Batch not found. Please try again.";
            }

            BatchResult = _printBatch;

            return _printBatch;
        }

        /// <summary>
        /// This method sets the pages buttons according to the batches status and the users role.
        /// </summary>
        /// <param name="loadBatch"></param>
        private void SetButtons(PrintBatchResult printBatch)
        {
            //disable all buttons by default.
            this.btnOther.Visible =
            this.btnSpoilBatch.Visible=
            this.btnStatus.Visible =
            this.btnReject.Visible =
            this.btnConfirm.Visible =
            this.btnPDFReport.Visible = false;
            this.btnDisplayCards.Visible = false;
            PrintBatchStatusId = null;

            //If a batch has been passed set the buttons accordingly.
            if (printBatch != null)
            {
                bool canUpdate;
                bool canRead;
                bool canCreate;

                if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_PRODUCT_MANAGER, printBatch.issuer_id, out canRead, out canUpdate, out canCreate))
                {
                    this.btnPDFReport.Visible = true;
                    this.btnDisplayCards.Visible = true;
                    if (canUpdate)
                    {
                        if (printBatch.print_batch_statuses_id == (int)PrintBatchStatuses.CREATED)
                        {
                            this.btnStatus.Text = GetGlobalResourceObject("PrintBatchStatusesButton", String.Format("PrintBtn{0}", printBatch.print_batch_statuses_id)).ToString();
                            this.btnStatus.Visible = true;
                            this.btnReject.Visible = true;
                        }

                    }
                }


                else if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_PRODUCT_OPERATOR, printBatch.issuer_id, out canRead, out canUpdate, out canCreate))
                {
                    this.btnPDFReport.Visible = true;
                    this.btnDisplayCards.Visible = true;
                    if (canUpdate)
                    {


                        if (printBatch.print_batch_statuses_id == (int)PrintBatchStatuses.PRINT_SUCESSFUL 
                            || printBatch.print_batch_statuses_id == (int)PrintBatchStatuses.PARTIAL_PRINT_SUCESSFUL
                            || printBatch.print_batch_statuses_id == (int)PrintBatchStatuses.CMS_ERROR)
                        {
                            this.btnOther.Text = GetGlobalResourceObject("PrintBatchStatusesButton", String.Format("PrintBtn{0}", printBatch.print_batch_statuses_id)).ToString();
                            this.btnOther.Visible = true;
                        }
                        if(printBatch.print_batch_statuses_id == (int)PrintBatchStatuses.CMS_ERROR)
                        {
                            this.btnSpoilBatch.Visible = true;
                        }
                        //Special Cases
                        // TODO: Upload to CMS - Check the status for Pin batch flow:



                    }
                }

            }
            }

            private void RejectConfirmation()
            {
                this.tbStatusNote.Enabled = true;
                this.tbStatusNote.Text = "";

                this.btnPDFReport.Visible = false;
                this.btnStatus.Visible = false;
                this.btnOther.Visible = false;
                this.btnSpoilBatch.Visible = false;
                this.btnReject.Visible = false;

                this.btnDisplayCards.Visible = false;

                this.btnConfirm.Visible = true;
                this.tbStatusNote.Enabled = this.btnConfirm.Enabled = true;
            }

        #region ViewState Variables
        private long? PrintBatchId
        {
            get
            {
                if (ViewState["PrintBatchId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["PrintBatchId"].ToString());
            }
            set
            {
                ViewState["PrintBatchId"] = value;
            }
        }

        private PrintBatchResult BatchResult
        {
            get
            {
                if (ViewState["BatchResult"] == null)
                    return null;
                else
                    return (PrintBatchResult)ViewState["BatchResult"];
            }
            set
            {
                ViewState["BatchResult"] = value;
            }
        }

        private bool isReject
        {
            get
            {
                if (ViewState["isReject"] == null)
                    return false;
                else
                    return (bool)ViewState["isReject"];
            }
            set
            {
                ViewState["isReject"] = value;
            }
        }

        private PrintBatchSearchParameters SearchParam
        {
            get
            {
                if (ViewState["SearchParam"] == null)
                    return null;
                else
                    return (PrintBatchSearchParameters)ViewState["SearchParam"];
            }
            set
            {
                ViewState["SearchParam"] = value;
            }
        }

        private int? PrintBatchStatusId
        {
            get
            {
                if (ViewState["PrintBatchStatusId"] == null)
                    return null;
                else
                    return (int?)ViewState["PrintBatchStatusId"];
            }
            set
            {
                ViewState["PrintBatchStatusId"] = value;
            }
        }

        private bool SpecialConfirm
        {
            get
            {
                if (ViewState["SpecialConfirm"] == null)
                    return false;
                else
                    return Boolean.Parse(ViewState["SpecialConfirm"].ToString());
            }
            set
            {
                ViewState["SpecialConfirm"] = value;
            }
        }

        private PageLayout? CurrentPageLayout
        {
            get
            {
                if (ViewState["CurrentPageLayout"] == null)
                    return null;
                else
                    return (PageLayout)ViewState["CurrentPageLayout"];
            }
            set
            {
                ViewState["CurrentPageLayout"] = value;
            }
        }
        #endregion

        protected void btnOther_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {

                UpdatePageLayout(PageLayout.SPECIAL_CONFIRM);
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

        protected void btnStatus_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text =
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                UpdatePageLayout(PageLayout.STATUS_CONFIRM);
                //SpecialConfirm = false;

                //this.btnOther.Visible =
                //this.btnStatus.Visible =
                //this.btnReject.Visible =
                //this.btnBack.Visible =
                //this.btnConfirm.Visible =
                //this.btnPDFReport.Visible = false;

                //this.btnConfirm.Visible =
                //    this.btnBack.Visible = true;

                //this.tbStatusNote.Text = String.Empty;
                //this.tbStatusNote.Enabled = true;
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

        protected void btnReject_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text =
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                UpdatePageLayout(PageLayout.REJECT_CONFIRM);
                //SpecialConfirm = false;
                //this.btnOther.Visible =
                //this.btnStatus.Visible =
                //this.btnReject.Visible =
                //this.btnBack.Visible =
                //this.btnConfirm.Visible =
                //this.btnPDFReport.Visible = false;

                //this.btnConfirm.Visible =
                //    this.btnBack.Visible = true;

                //this.tbStatusNote.Text = String.Empty;
                //this.tbStatusNote.Enabled = true;

                //isReject = true;
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

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                PageLayout PageLayout = PageLayout.READ;

                if (CurrentPageLayout != null)
                {
                    PageLayout = CurrentPageLayout.GetValueOrDefault();
                }

                string responseMessage = String.Empty;
                PrintBatchResult result = new PrintBatchResult();
                int newprintbatch_statuses_id = -1;
                if ((int)BatchResult.print_batch_statuses_id == (int)PrintBatchStatuses.CREATED)
                    newprintbatch_statuses_id = (int)PrintBatchStatuses.APPROVED;

                if (PrintBatchId != null)
                {
                    switch (PageLayout)
                    {
                        case PageLayout.READ:
                            break;
                        case PageLayout.STATUS_CONFIRM:

                            if (_issuerman.UpdatePrintBatchStatus(PrintBatchId.Value, (int)BatchResult.print_batch_statuses_id, newprintbatch_statuses_id, this.tbStatusNote.Text, false, out result, out responseMessage))
                                this.lblInfoMessage.Text = responseMessage;
                            else
                                this.lblErrorMessage.Text = responseMessage;
                            break;
                        case PageLayout.SPOIL_CONFIRM:
                            if (_issuerman.SpoilPrintBatch((long)PrintBatchId, (int)PrintBatchStatuses.SPOIL, tbStatusNote.Text))
                            this.lblInfoMessage.Text = "Batch is Spoiled.";
                            break;
                        case PageLayout.REJECT_CONFIRM:
                            if (isNotesPopulated())
                            {
                                if (_issuerman.UpdatePrintBatchStatus(PrintBatchId.Value, (int)BatchResult.print_batch_statuses_id, (int)PrintBatchStatuses.REJECT, this.tbStatusNote.Text, false, out result, out responseMessage))
                                    this.lblInfoMessage.Text = responseMessage;
                                else
                                    this.lblErrorMessage.Text = responseMessage;
                            }

                            break;

                        case PageLayout.SPECIAL_CONFIRM:

                            if (_issuerman.UploadBatchtoCMS(PrintBatchId.Value, tbStatusNote.Text, out responseMessage))
                            {
                                this.lblInfoMessage.Text = responseMessage;
                                UpdatePageLayout(PageLayout.READ);
                            }
                            else
                                this.lblErrorMessage.Text = responseMessage;


                            break;
                        default:
                            break;
                    }
                }

                if (result != null && result.print_batch_id > 0)
                {
                    UpdatePageLayout(PageLayout.READ);
                    //PopulatePage(result);
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

        protected void btnPDFReport_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                if (PrintBatchId > 0)
                {
                    var reportBytes = _issuerman.GeneratePrintBatchReport((long)PrintBatchId);

                    string reportName = "Print_Batch_Report_" + PrintBatchId + "_" +
                                            DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(reportBytes);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    //Response.End();
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

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            PageLayout pageLayout = PageLayout.READ;

            if (CurrentPageLayout != null)
            {
                pageLayout = CurrentPageLayout.GetValueOrDefault();
            }

            if (pageLayout == PageLayout.READ)
            {
                if (SearchParam != null)
                    SessionWrapper.PrintBatchSearchParams = SearchParam;

                Server.Transfer("~\\webpages\\Hybrid\\PrintBatchList.aspx");
            }
            else
            {
                UpdatePageLayout(PageLayout.READ);
            }
        }

        private Boolean isNotesPopulated()
        {
            if (String.IsNullOrWhiteSpace(this.tbStatusNote.Text))
            {
                this.lblErrorMessage.Text = GetGlobalResourceObject("DefaultExceptions", "ValidationRejectReason").ToString();
                return false;
            }

            return true;
        }

        protected void btnDisplayCards_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                SessionWrapper.BackURL = "~\\webpages\\hybrid\\";
                SessionWrapper.PrintBatchId = BatchResult.print_batch_id;
                SessionWrapper.RequestSearchParam = SearchParam;
                SessionWrapper.RequestSearchParam = new RequestSearchParamters
                {
                    printbatchId = BatchResult.print_batch_id,
                    IssuerId = BatchResult.issuer_id,
                    RowsPerPage = StaticDataContainer.ROWS_PER_PAGE,
                    PageIndex = 1,
                    PreviousSearchParameters = SearchParam// current page parameters
                };

                Server.Transfer("~\\webpages\\hybrid\\HybridRequestList.aspx", false);
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

        protected void btnSpoilBatch_Click(object sender, EventArgs e)
        {

            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {

                UpdatePageLayout(PageLayout.SPOIL_CONFIRM);
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
    }
}