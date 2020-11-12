using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.export
{
    public partial class ExportBatchView : BasePage
    {
        private enum DistPageLayout
        {
            READ,
            REJECT_CONFIRM,
            APPROVE_CONFIRM,
            REQUEST_CONFIRM
        }

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.AUDITOR};

        private static readonly ILog log = LogManager.GetLogger(typeof(ExportBatchView));
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private UserManagementService _userMan = new UserManagementService();

        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData(null);
                CurrentDisPageLayout = DistPageLayout.READ;
            }
        }

        /// <summary>
        /// Use this method to initially load the page.
        /// </summary>
        /// <param name="distBatchId"></param>
        private void LoadPageData(long? exportBatchId)
        {
            try
            {
                if (SessionWrapper.ExportBatchSearchParams != null)
                {
                    SearchParam = SessionWrapper.ExportBatchSearchParams;
                    SessionWrapper.ExportBatchSearchParams = null;
                }

                //See if theres anything in the session.
                if (exportBatchId == null && SessionWrapper.ExportBatchId != null)
                {
                    exportBatchId = SessionWrapper.ExportBatchId;
                }

                if (SessionWrapper.CardSearchParams != null && SessionWrapper.CardSearchParams.PreviousSearchParameters != null)
                {
                    SearchParam = (ExportBatchSearchParameters)SessionWrapper.CardSearchParams.PreviousSearchParameters;
                    exportBatchId = ((ExportBatchSearchParameters)SessionWrapper.CardSearchParams).ExportBatchId;
                    SessionWrapper.CardSearchParams = null;
                }
                if (Request.QueryString["page"] != null)
                {
                    PreviousPage = Request.QueryString["page"].ToString();
                }
                ExportBatchResult exportBatch = null;

                if (exportBatchId != null)
                {
                    exportBatch = PopulatePage(exportBatchId.GetValueOrDefault());

                    if (exportBatch != null)
                    {
                        SetButtons(exportBatch);
                        ExportBatchId = exportBatch.export_batch_id;
                        BatchResult = exportBatch;
                    }

                    SessionWrapper.ExportBatchId = null;
                }

                if (exportBatch == null)
                {
                    this.lblErrorMessage.Text = "No export batch to show.";
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

        /// <summary>
        /// Use this method to populate the page and set the pages buttons according to batch status and user role. 
        /// This method does a call to the DB to get latest batch information if the batch id is not null.
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <returns></returns>
        private ExportBatchResult PopulatePage(long? exportBatchId)
        {
            ExportBatchResult exportBatch = null;

            if (exportBatchId != null)
            {
                exportBatch = _batchService.GetExportBatch(exportBatchId.Value);
            }

            return PopulatePage(exportBatch);
        }

        /// <summary>
        /// Use this method to populate the page and set the pages buttons according to batch status and user role.         
        /// </summary>
        /// <param name="distBatch"></param>
        /// <returns></returns>
        private ExportBatchResult PopulatePage(ExportBatchResult exportBatch)
        {
            SetButtons(exportBatch);
            BatchResult = exportBatch;

            if (exportBatch != null)
            {
                //this.tbCreatedBy.Text = distBatch.username;
                this.tbBatchCreatedDate.Text =Convert.ToDateTime(exportBatch.date_created).ToString(DATETIME_FORMAT);
                this.tbBatchReference.Text = exportBatch.batch_reference;
                this.tbBatchStatus.Text = exportBatch.export_batch_statuses_name;
                this.tbNumberOfCards.Text = exportBatch.no_cards.ToString();
                this.tbStatusNote.Text = exportBatch.comments;
                this.tbIssuerName.Text = base.FormatNameAndCode(exportBatch.issuer_name, exportBatch.issuer_code);
                //this.tbProductType.Text = distBatch.product_name;
            }
            else
            {
                this.lblInfoMessage.Text = "Export batch not found. Please try again.";
            }

            return exportBatch;
        }

        /// <summary>
        /// This method sets the pages buttons according to the batches status and the users role.
        /// </summary>
        /// <param name="loadBatch"></param>
        private void SetButtons(ExportBatchResult exportBatch)
        {
            //disable all buttons by default.            
            this.btnApprove.Visible = false;
            this.btnRequestExport.Visible = false;
            this.btnReject.Visible = false;
            this.btnConfirm.Visible = false;
            this.tbStatusNote.Enabled = this.btnConfirm.Enabled = false;
            this.btnDisplayCards.Visible = false;
            this.btnPDFReport.Visible = true;

            //If a load batch has been passed set the buttons accordingly.
            if (exportBatch != null)
            {
                bool canUpdate;
                bool canRead;
                bool canCreate;

                //Needs Approval
                if ( (exportBatch.export_batch_statuses_id == 0 || exportBatch.export_batch_statuses_id == 3)
                    && PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_MANAGER, exportBatch.issuer_id, out canRead, out canUpdate, out canCreate))
                {
                    if(canUpdate)
                    {
                        this.btnApprove.Visible =
                            this.btnReject.Visible =
                            this.tbStatusNote.Enabled = true;

                        this.tbStatusNote.Text = String.Empty;
                    }
                }
                else if ( (exportBatch.export_batch_statuses_id == 2 || exportBatch.export_batch_statuses_id == 4)
                            && PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CMS_OPERATOR, exportBatch.issuer_id, out canRead, out canUpdate, out canCreate)) //Request to export again
                {
                    if(canUpdate)
                    {
                        this.btnRequestExport.Visible =
                            this.tbStatusNote.Enabled = true;
                        this.tbStatusNote.Text = String.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Use this method to set page controls bassed on the page layout.
        /// </summary>
        /// <param name="disPageLayout"></param>
        private void UpdatePageLayout(DistPageLayout? disPageLayout)
        {
            if (disPageLayout == null)
            {
                disPageLayout = CurrentDisPageLayout.GetValueOrDefault();
            }

            switch (disPageLayout)
            {
                case DistPageLayout.READ:
                    this.tbStatusNote.Enabled = false;
                    PopulatePage(ExportBatchId);
                    break;
                case DistPageLayout.APPROVE_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    RejectConfirmation();
                    break;
                case DistPageLayout.REJECT_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    RejectConfirmation();
                    break;
                case DistPageLayout.REQUEST_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    RejectConfirmation();
                    break;
                default:
                    break;

            }

            CurrentDisPageLayout = disPageLayout;
        }

        private void RejectConfirmation()
        {
            this.tbStatusNote.Enabled = true;
            this.tbStatusNote.Text = "";

            this.btnPDFReport.Visible = false;
            this.btnApprove.Visible = false;
            this.btnRequestExport.Visible = false;
            this.btnReject.Visible = false;

            this.btnDisplayCards.Visible = false;

            this.btnConfirm.Visible = true;
            this.tbStatusNote.Enabled = this.btnConfirm.Enabled = true;
        }

        /// <summary>
        /// Returns true if notes text box has been populated.
        /// </summary>
        /// <returns></returns>
        private Boolean isNotesPopulated()
        {
            if (String.IsNullOrWhiteSpace(this.tbStatusNote.Text))
            {
                //this.lblErrorMessage.Text = GetLocalResourceObject("ValidationRejectReason").ToString();
                this.lblErrorMessage.Text = GetGlobalResourceObject("DefaultExceptions", "ValidationRejectReason").ToString();
                return false;
            }

            return true;
        }

        #endregion

        #region Page Events

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            DistPageLayout pageLayout = DistPageLayout.READ;

            if (CurrentDisPageLayout != null)
            {
                pageLayout = CurrentDisPageLayout.GetValueOrDefault();
            }

            if (pageLayout == DistPageLayout.READ)
            {
                if (SearchParam != null)
                    SessionWrapper.ExportBatchSearchParams = SearchParam;
                if (string.IsNullOrEmpty(PreviousPage))
                {
                    Server.Transfer("~\\webpages\\export\\ExportBatchList.aspx");
                }
            }
            else
            {
                UpdatePageLayout(DistPageLayout.READ);
            }
        }

        protected void btnDisplayCards_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                //SessionWrapper.BackURL = "~\\webpages\\card\\";
                SearchParam.ExportBatchId = BatchResult.export_batch_id;
                SessionWrapper.CardSearchParams = new CardSearchParameters
                {
                    DistBatchId = BatchResult.export_batch_id,                   
                    //BatchReference = BatchResult.dist_batch_reference,
                    IssuerId = BatchResult.issuer_id,
                    RowsPerPage = StaticDataContainer.ROWS_PER_PAGE,
                    PageIndex = 1,
                    PreviousSearchParameters = SearchParam,
                };

                //SessionWrapper.ExportBatchSearchParams = SearchParam;
                //Response.Redirect("~\\webpages\\card\\CardList.aspx");

                Server.Transfer("~\\webpages\\card\\CardList.aspx", false);
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

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                UpdatePageLayout(DistPageLayout.APPROVE_CONFIRM);
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

        protected void btnRequestExport_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                UpdatePageLayout(DistPageLayout.REQUEST_CONFIRM);
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

        protected void btnReject_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                UpdatePageLayout(DistPageLayout.REJECT_CONFIRM);
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
                DistPageLayout disPageLayout = DistPageLayout.READ;

                if (CurrentDisPageLayout != null)
                {
                    disPageLayout = CurrentDisPageLayout.GetValueOrDefault();
                }

                string responseMessage = String.Empty;
                ExportBatchResult exportBatch = new ExportBatchResult();

                if (ExportBatchId != null)
                {
                    switch (disPageLayout)
                    {
                        case DistPageLayout.READ:
                            break;
                        case DistPageLayout.APPROVE_CONFIRM:
                            if (isNotesPopulated())
                                if (_batchService.ApproveExportBatch(ExportBatchId.Value, this.tbStatusNote.Text.Trim(), out exportBatch, out responseMessage))
                                    this.lblInfoMessage.Text = responseMessage;
                                else
                                    this.lblErrorMessage.Text = responseMessage;
                            break;
                        case DistPageLayout.REJECT_CONFIRM:
                            if (isNotesPopulated())
                                if (_batchService.RejectExportBatch(ExportBatchId.Value, this.tbStatusNote.Text.Trim(), out exportBatch, out responseMessage))
                                    this.lblInfoMessage.Text = responseMessage;
                                else
                                    this.lblErrorMessage.Text = responseMessage;
                            break;
                        case DistPageLayout.REQUEST_CONFIRM:
                            if (isNotesPopulated())
                                if (_batchService.RequestExportBatch(ExportBatchId.Value, this.tbStatusNote.Text.Trim(), out exportBatch, out responseMessage))
                                    this.lblInfoMessage.Text = responseMessage;
                                else
                                    this.lblErrorMessage.Text = responseMessage;
                            break;
                        default:
                            break;
                    }
                }

                if (exportBatch != null && exportBatch.export_batch_id > 0)
                {
                    //UpdatePageLayout(DistPageLayout.READ);
                    PopulatePage(exportBatch);
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
                if (ExportBatchId != null)
                {
                    var reportBytes = _batchService.GenerateExportBatchReport(ExportBatchId.Value);

                    string reportName = String.Empty;

                    reportName = "Export_Report_";
                    reportName += ExportBatchId.Value.ToString() + "_" + DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(reportBytes);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.Flush();
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

        #endregion

        #region ViewState Variables
        private long? ExportBatchId
        {
            get
            {
                if (ViewState["ExportBatchId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["ExportBatchId"].ToString());
            }
            set
            {
                ViewState["ExportBatchId"] = value;
            }
        }

        private ExportBatchResult BatchResult
        {
            get
            {
                if (ViewState["BatchResult"] == null)
                    return null;
                else
                    return (ExportBatchResult)ViewState["BatchResult"];
            }
            set
            {
                ViewState["BatchResult"] = value;
            }
        }

        private DistPageLayout? CurrentDisPageLayout
        {
            get
            {
                if (ViewState["CurrentDisPageLayout"] == null)
                    return null;
                else
                    return (DistPageLayout)ViewState["CurrentDisPageLayout"];
            }
            set
            {
                ViewState["CurrentDisPageLayout"] = value;
            }
        }

        private ExportBatchSearchParameters SearchParam
        {
            get
            {
                if (ViewState["SearchParam"] == null)
                    return null;
                else
                    return (ExportBatchSearchParameters)ViewState["SearchParam"];
            }
            set
            {
                ViewState["SearchParam"] = value;
            }
        }
        private String PreviousPage
        {
            get
            {
                if (ViewState["PreviousPage"] == null)
                    return null;
                else
                    return (String)ViewState["PreviousPage"];
            }
            set
            {
                ViewState["PreviousPage"] = value;
            }
        }
        #endregion
    }
}