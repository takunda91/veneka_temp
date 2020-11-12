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

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class PinBatchView : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PinBatchView));

        private readonly PINManagementService _pinService = new PINManagementService();
        private UserManagementService _userMan = new UserManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.PIN_PRINTER_OPERATOR,
                                                                        UserRole.CARD_CENTRE_PIN_OFFICER,
                                                                        UserRole.BRANCH_PIN_OFFICER,
                                                                        UserRole.CENTER_MANAGER,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.AUDITOR};
        private enum PinPageLayout
        {
            READ,
            REJECT_CONFIRM,
            STATUS_CONFIRM,
            SPECIAL_CONFIRM
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
        /// <param name="distBatchId"></param>
        private void LoadPageData(long? pinBatchId)
        {
            try
            {
               
                long? pin_BatchId = null;
                if (SessionWrapper.DistBatchId != null)
                {
                    pin_BatchId = SessionWrapper.PinBatchId;
                    SessionWrapper.PinBatchId = null;
                }
                if (SessionWrapper.PinBatchSearchParams!=null)
                {
                    SearchParam = SessionWrapper.PinBatchSearchParams;
                    SessionWrapper.PinBatchSearchParams = null;
                }
                if (SessionWrapper.CardSearchParams != null && SessionWrapper.CardSearchParams.PreviousSearchParameters != null)
                {
                    SearchParam = (PinBatchSearchParameters)SessionWrapper.CardSearchParams.PreviousSearchParameters;
                    pin_BatchId = ((CardSearchParameters)SessionWrapper.CardSearchParams).PinbatchId;
                    SessionWrapper.CardSearchParams = null;
                }
                //See if theres anything in the session.
                if (pinBatchId == null && pin_BatchId != null)
                {
                    pinBatchId = pin_BatchId;
                }

                PinBatchResult pinBatch = null;

                if (pinBatchId != null)
                {
                    pinBatch = PopulatePage(pinBatchId.GetValueOrDefault());

                    if (pinBatch != null)
                    {
                        SetButtons(pinBatch);
                        PinBatchId = pinBatch.pin_batch_id;
                    }


                }

                if (pinBatchId == null)
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
        private PinBatchResult PopulatePage(long? pinBatchId)
        {
            PinBatchResult pinBatch = null;

            if (pinBatchId != null)
            {
                pinBatch = _pinService.GetPinBatch(pinBatchId.Value);
            }

            BatchResult = pinBatch;

            return PopulatePage(pinBatch);
        }

        /// <summary>
        /// Use this method to set page controls bassed on the page layout.
        /// </summary>
        /// <param name="disPageLayout"></param>
        private void UpdatePageLayout(PinPageLayout? pinPageLayout)
        {
            if (pinPageLayout == null)
            {
                pinPageLayout = CurrentPinPageLayout.GetValueOrDefault();
            }

            switch (pinPageLayout)
            {
                case PinPageLayout.READ:
                    this.tbStatusNote.Enabled = false;
                    PopulatePage(PinBatchId);
                    break;
                case PinPageLayout.STATUS_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    RejectConfirmation();
                    break;
                case PinPageLayout.SPECIAL_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    RejectConfirmation();
                    break;
                case PinPageLayout.REJECT_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    RejectConfirmation();
                    break;
                default:
                    break;

            }

            CurrentPinPageLayout = pinPageLayout;
        }

        /// <summary>
        /// Use this method to populate the page and set the pages buttons according to batch status and user role.         
        /// </summary>
        /// <param name="distBatch"></param>
        /// <returns></returns>
        private PinBatchResult PopulatePage(PinBatchResult pinBatch)
        {
            SetButtons(pinBatch);

            if (pinBatch != null)
            {
                this.tbIssueMethod.Text = pinBatch.card_issue_method_name;
                this.tbBatchCreatedDate.Text = Convert.ToDateTime(pinBatch.date_created).ToString(DATETIME_FORMAT);
                this.tbBatchReference.Text = pinBatch.pin_batch_reference;
                this.tbBatchStatus.Text = pinBatch.pin_batch_status_name;
                this.tbNumberOfCards.Text = pinBatch.no_cards.ToString();
                this.tbStatusNote.Text = pinBatch.status_notes;
                this.tbIssuerName.Text = base.FormatNameAndCode(pinBatch.issuer_name, pinBatch.issuer_code);

                if (pinBatch.branch_code != null)
                    this.tbBranchName.Text = base.FormatNameAndCode(pinBatch.branch_name, pinBatch.branch_code);
                else
                    this.tbBranchName.Text = "-";
            }
            else
            {
                this.lblInfoMessage.Text = "Batch not found. Please try again.";
            }

            BatchResult = pinBatch;

            return pinBatch;
        }

        /// <summary>
        /// This method sets the pages buttons according to the batches status and the users role.
        /// </summary>
        /// <param name="loadBatch"></param>
        private void SetButtons(PinBatchResult pinBatch)
        {
            //disable all buttons by default.
            this.btnOther.Visible =
            this.btnStatus.Visible =
            this.btnReject.Visible =
            this.btnConfirm.Visible =
            this.btnPDFReport.Visible = false;
            this.btnDisplayCards.Visible = false;
            PinBatchStatusId = null;

            //If a batch has been passed set the buttons accordingly.
            if (pinBatch != null)
            {
                bool canUpdate;
                bool canRead;
                bool canCreate;

                if (pinBatch.user_role_id != null &&
                    pinBatch.flow_pin_batch_statuses_id != null &&
                    PageUtility.ValidateUserPageRole(User.Identity.Name, (UserRole)pinBatch.user_role_id, pinBatch.issuer_id, out canRead, out canUpdate, out canCreate))
                {
                    this.btnPDFReport.Visible = true;
                    this.btnDisplayCards.Visible = true;
                    if (canUpdate)
                    {
                        this.btnStatus.Text = GetGlobalResourceObject("PinBatchStatusesButton", String.Format("PinBtn{0}", pinBatch.flow_pin_batch_statuses_id)).ToString();
                        this.btnStatus.Visible = true;

                        //Special Cases
                        // TODO: Upload to CMS - Check the status for Pin batch flow:
                        if (pinBatch.pin_batch_statuses_id == 7)
                        {
                            this.btnOther.Text = GetGlobalResourceObject("PinBatchStatusesButton", String.Format("PinBtn{0}", pinBatch.pin_batch_statuses_id)).ToString();
                            this.btnOther.Visible = true;
                        }


                    }
                }
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_PIN_OFFICER, pinBatch.issuer_id, out canRead, out canUpdate, out canCreate))
                {
                    if (pinBatch.reject_pin_batch_statuses_id != null)
                    {
                        this.btnReject.Text = GetGlobalResourceObject("PinBatchStatusesButton", String.Format("PinBtn{0}", pinBatch.reject_pin_batch_statuses_id)).ToString();
                        this.btnReject.Visible = true;
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
            this.btnReject.Visible = false;

            this.btnDisplayCards.Visible = false;

            this.btnConfirm.Visible = true;
            this.tbStatusNote.Enabled = this.btnConfirm.Enabled = true;
        }

        #region ViewState Variables
        private long? PinBatchId
        {
            get
            {
                if (ViewState["PinBatchId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["PinBatchId"].ToString());
            }
            set
            {
                ViewState["PinBatchId"] = value;
            }
        }

        private PinBatchResult BatchResult
        {
            get
            {
                if (ViewState["BatchResult"] == null)
                    return null;
                else
                    return (PinBatchResult)ViewState["BatchResult"];
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

        private PinBatchSearchParameters SearchParam
        {
            get
            {
                if (ViewState["SearchParam"] == null)
                    return null;
                else
                    return (PinBatchSearchParameters)ViewState["SearchParam"];
            }
            set
            {
                ViewState["SearchParam"] = value;
            }
        }

        private int? PinBatchStatusId
        {
            get
            {
                if (ViewState["PinBatchStatusId"] == null)
                    return null;
                else
                    return (int?)ViewState["PinBatchStatusId"];
            }
            set
            {
                ViewState["PinBatchStatusId"] = value;
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

        private PinPageLayout? CurrentPinPageLayout
        {
            get
            {
                if (ViewState["CurrentPinPageLayout"] == null)
                    return null;
                else
                    return (PinPageLayout)ViewState["CurrentPinPageLayout"];
            }
            set
            {
                ViewState["CurrentPinPageLayout"] = value;
            }
        }
        #endregion

        protected void btnOther_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                //SpecialConfirm = true;
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
                UpdatePageLayout(PinPageLayout.SPECIAL_CONFIRM);
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
                UpdatePageLayout(PinPageLayout.STATUS_CONFIRM);
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
                UpdatePageLayout(PinPageLayout.REJECT_CONFIRM);
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
                PinPageLayout pinPageLayout = PinPageLayout.READ;

                if (CurrentPinPageLayout != null)
                {
                    pinPageLayout = CurrentPinPageLayout.GetValueOrDefault();
                }

                string responseMessage = String.Empty;
                PinBatchResult result = new PinBatchResult();

                if (PinBatchId != null)
                {
                    switch (pinPageLayout)
                    {
                        case PinPageLayout.READ:
                            break;
                        case PinPageLayout.STATUS_CONFIRM:
                            if (_pinService.UpdatePinBatchStatus(PinBatchId.Value, BatchResult.pin_batch_statuses_id, BatchResult.flow_pin_batch_statuses_id.Value, this.tbStatusNote.Text, out result, out responseMessage))
                                this.lblInfoMessage.Text = responseMessage;
                            else
                                this.lblErrorMessage.Text = responseMessage;
                            break;
                        case PinPageLayout.SPECIAL_CONFIRM:
                            if (_pinService.UpdatePinBatchStatus(PinBatchId.Value, BatchResult.pin_batch_statuses_id, BatchResult.pin_batch_statuses_id, this.tbStatusNote.Text, out result, out responseMessage))
                                this.lblInfoMessage.Text = responseMessage;
                            else
                                this.lblErrorMessage.Text = responseMessage;
                            break;
                        case PinPageLayout.REJECT_CONFIRM:
                            if (isNotesPopulated())
                            {
                                if (_pinService.PinBatchRejectStatus((long)PinBatchId, (int)BatchResult.reject_pin_batch_statuses_id.Value, this.tbStatusNote.Text, out result, out  responseMessage))
                                    this.lblInfoMessage.Text = responseMessage;
                                else
                                    this.lblErrorMessage.Text = responseMessage;
                            }

                            break;
                        default:
                            break;
                    }
                }

                if (result != null && result.pin_batch_id > 0)
                {
                    UpdatePageLayout(PinPageLayout.READ);
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
                if (PinBatchId > 0)
                {
                    var reportBytes = _pinService.GeneratePinBatchReport((long)PinBatchId);

                    string reportName = "Pin_Batch_Report_" + PinBatchId + "_" +
                                            DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(reportBytes);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.End();
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

            PinPageLayout pageLayout = PinPageLayout.READ;

            if (CurrentPinPageLayout != null)
            {
                pageLayout = CurrentPinPageLayout.GetValueOrDefault();
            }

            if (pageLayout == PinPageLayout.READ)
            {
                if (SearchParam != null)
                    SessionWrapper.PinBatchSearchParams = SearchParam;

                Server.Transfer("~\\webpages\\pin\\PinBatchList.aspx");
            }
            else
            {
                UpdatePageLayout(PinPageLayout.READ);
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
                //SessionWrapper.BackURL = "~\\webpages\\card\\";
                //SessionWrapper.PinBatchId = BatchResult.pin_batch_id;
                //SessionWrapper.PinBatchSearchParams = SearchParam;
                SessionWrapper.CardSearchParams = new CardSearchParameters
                {
                    PinbatchId = BatchResult.pin_batch_id,
                    //BatchReference = BatchResult.dist_batch_reference,
                    IssuerId = BatchResult.issuer_id,
                    RowsPerPage = StaticDataContainer.ROWS_PER_PAGE,
                    PageIndex = 1,
                    PreviousSearchParameters = SearchParam// current page parameters
                };

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
    }
}