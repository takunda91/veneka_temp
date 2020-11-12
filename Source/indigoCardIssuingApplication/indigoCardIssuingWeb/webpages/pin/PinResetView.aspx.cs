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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class PinResetView : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PinResetView));

        private readonly PINManagementService _pinService = new PINManagementService();
        private UserManagementService _userMan = new UserManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.PIN_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.AUDITOR};
        private enum PinPageLayout
        {
            READ,
            APPROVE_CONFIRM,
            REJECT_CONFIRM,
            UPLOAD_CONFIRM,
            CANCEL_CONFIRM

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
        /// Use this method to initially load the page.ca
        /// </summary>
        /// <param name="distBatchId"></param>
        private void LoadPageData(long? pinReissueId)
        {
            try
            {
                if (SessionWrapper.PinReissueSearchParams != null)
                {
                    SearchParam = SessionWrapper.PinReissueSearchParams;
                    SessionWrapper.PinReissueSearchParams = null;
                }

                //See if theres anything in the session.
                if (pinReissueId == null && SessionWrapper.PinReissueId != null)
                {
                    pinReissueId = SessionWrapper.PinReissueId;
                    SessionWrapper.PinBatchId = null;
                }

                PinReissueWSResult pinReissue = null;




                if (pinReissueId != null)
                {
                    pinReissue = PopulatePage(pinReissueId.GetValueOrDefault());

                    if (pinReissue != null)
                    {
                        SetButtons(pinReissue);
                        PinReissueId = pinReissue.pin_reissue_id;
                    }

                    SessionWrapper.PinReissueId = null;
                }

                if (pinReissueId == null)
                {
                    this.lblErrorMessage.Text = "No pin reissue to show.";
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
        private PinReissueWSResult PopulatePage(long? pinReissueId)
        {
            PinReissueWSResult pinReissue = null;

            if (pinReissueId != null)
            {
                pinReissue = _pinService.GetPINReissue(pinReissueId.Value);
            }

            PinReissueResult = pinReissue;

            return PopulatePage(pinReissue);
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
                    this.tbComments.Enabled = false;
                    PopulatePage(PinReissueId);
                    break;
                case PinPageLayout.APPROVE_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    ApproveConfirmation();
                    break;
                case PinPageLayout.REJECT_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    RejectConfirmation();
                    break;
                case PinPageLayout.CANCEL_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    UploadConfirmation();
                    break;
                case PinPageLayout.UPLOAD_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    UploadConfirmation();
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
        private PinReissueWSResult PopulatePage(PinReissueWSResult pinReissue)
        {
            SetButtons(pinReissue);

            if (pinReissue != null)
            {
                this.tbIssuer.Text = UtilityClass.FormatNameAndCode(pinReissue.issuer_name, pinReissue.issuer_code);
                this.tbBranch.Text = UtilityClass.FormatNameAndCode(pinReissue.branch_name, pinReissue.branch_code);
                this.tbProduct.Text = UtilityClass.FormatNameAndCode(pinReissue.product_name, pinReissue.product_code);
                this.tbRequestDate.Text = Convert.ToDateTime(pinReissue.reissue_date).ToString(DATETIME_FORMAT, CultureInfo.InvariantCulture);
                this.tbCardNumber.Text = pinReissue.card_number;
                this.tbOperator.Text = pinReissue.operator_usename;
                this.tbAuthUser.Text = pinReissue.authorise_username;
                this.tbExpiry.Text = pinReissue.request_expiry.ToString(DATETIME_FORMAT, CultureInfo.InvariantCulture);
                //this.tbStatusDate.Text = pinReissue.status_date.ToString(DATETIME_FORMAT, CultureInfo.InvariantCulture);
                this.tbStatus.Text = pinReissue.pin_reissue_statuses_name;
                this.tbComments.Text = pinReissue.comments;
            }
            else
            {
                this.lblInfoMessage.Text = "Pin reissue not found. Please try again.";
            }

            PinReissueResult = pinReissue;

            return pinReissue;
        }

        /// <summary>
        /// This method sets the pages buttons according to the batches status and the users role.
        /// </summary>
        /// <param name="loadBatch"></param>
        private void SetButtons(PinReissueWSResult pinReissue)
        {
            //disable all buttons by default.
            this.btnBack.Visible = true;
            this.btnUpload.Visible =
            this.btnCancel.Visible =
            this.btnApprove.Visible =
            this.btnReject.Visible =
            this.btnConfirm.Visible = false;
            PinReissueStatusId = null;

            //If a batch has been passed set the buttons accordingly.
            if (pinReissue != null)
            {
                bool canUpdate;
                bool canRead;
                bool canCreate;

                if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_CUSTODIAN, pinReissue.issuer_id, out canRead, out canUpdate, out canCreate))
                {
                    if (pinReissue.pin_reissue_statuses_id == 0 && pinReissue.authorise_pin_reissue_YN &&
                        canUpdate)
                    {
                        this.btnApprove.Visible =
                                this.btnReject.Visible = true;

                    }

                }




                if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.PIN_OPERATOR, pinReissue.issuer_id, out canRead, out canUpdate, out canCreate))
                {
                    if (((pinReissue.pin_reissue_statuses_id == 1 && pinReissue.authorise_pin_reissue_YN) ||
                            (pinReissue.pin_reissue_statuses_id == 0 && !pinReissue.authorise_pin_reissue_YN)) &&
                        canUpdate)
                    {
                        this.btnCancel.Visible = true;
                        this.btnUpload.Visible = true;
                    }


                }

            }
        }

        private void ApproveConfirmation()
        {
            this.tbComments.Enabled = true;
            this.tbComments.Text = "";

            this.btnApprove.Visible =
            this.btnCancel.Visible =
            this.btnReject.Visible = false;

            this.btnConfirm.Visible = true;
            this.tbComments.Enabled = this.btnConfirm.Enabled = true;



        }

        private void RejectConfirmation()
        {
            this.tbComments.Enabled = true;
            this.tbComments.Text = "";

            this.btnApprove.Visible =
                this.btnCancel.Visible =
            this.btnReject.Visible = false;

            this.btnConfirm.Visible = true;
            this.tbComments.Enabled = this.btnConfirm.Enabled = true;
        }

        private void UploadConfirmation()
        {
            this.tbComments.Enabled = true;
            this.tbComments.Text = "";

            this.btnUpload.Visible = false;
            this.btnCancel.Visible = false;

            this.btnConfirm.Visible = true;
            this.tbComments.Enabled = this.btnConfirm.Enabled = true;
        }



        #region ViewState Variables
        private long? PinReissueId
        {
            get
            {
                if (ViewState["PinReissueId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["PinReissueId"].ToString());
            }
            set
            {
                ViewState["PinReissueId"] = value;
            }
        }

        private PinReissueWSResult PinReissueResult
        {
            get
            {
                if (ViewState["PinReissueResult"] == null)
                    return null;
                else
                    return (PinReissueWSResult)ViewState["PinReissueResult"];
            }
            set
            {
                ViewState["PinReissueResult"] = value;
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

        private PinReissueSearchParameters SearchParam
        {
            get
            {
                if (ViewState["SearchParam"] == null)
                    return null;
                else
                    return (PinReissueSearchParameters)ViewState["SearchParam"];
            }
            set
            {
                ViewState["SearchParam"] = value;
            }
        }

        private int? PinReissueStatusId
        {
            get
            {
                if (ViewState["PinReissueStatusId"] == null)
                    return null;
                else
                    return (int?)ViewState["PinReissueStatusId"];
            }
            set
            {
                ViewState["PinReissueStatusId"] = value;
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

        protected void btnApprove_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text =
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                UpdatePageLayout(PinPageLayout.APPROVE_CONFIRM);
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

        protected void btnUpload_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text =
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                UpdatePageLayout(PinPageLayout.UPLOAD_CONFIRM);
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

        protected void btnConfirm_OnClick(object sender, EventArgs e)
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

                if (pinPageLayout == PinPageLayout.REJECT_CONFIRM && String.IsNullOrEmpty(this.tbComments.Text))
                {
                    this.lblInfoMessage.Text = "Please provide reject reason in comments field.";
                    return;
                }

                string responseMessage = String.Empty;
                PinReissueWSResult result = new PinReissueWSResult();

                if (PinReissueId != null)
                {
                    switch (pinPageLayout)
                    {
                        case PinPageLayout.READ:
                            break;
                        case PinPageLayout.APPROVE_CONFIRM:
                            if (_pinService.ApprovePINReissue(PinReissueId.Value, this.tbComments.Text, out result, out responseMessage))
                                this.lblInfoMessage.Text = responseMessage;
                            else
                                this.lblErrorMessage.Text = responseMessage;
                            break;
                        case PinPageLayout.REJECT_CONFIRM:
                            if (_pinService.RejectPINReissue(PinReissueId.Value, this.tbComments.Text, out result, out responseMessage))
                                this.lblInfoMessage.Text = responseMessage;
                            else
                                this.lblErrorMessage.Text = responseMessage;
                            break;
                        case PinPageLayout.CANCEL_CONFIRM:
                            if (_pinService.CancelPINReissue(PinReissueId.Value, this.tbComments.Text, out result, out responseMessage))
                                this.lblInfoMessage.Text = responseMessage;
                            else
                                this.lblErrorMessage.Text = responseMessage;
                            break;
                        case PinPageLayout.UPLOAD_CONFIRM:
                            if (_pinService.CompletePINReissue(PinReissueId.Value, this.tbComments.Text, PinReissueResult.issuer_id, PinReissueResult.product_id, PinReissueResult.primary_index_number, out result, out responseMessage))
                                this.lblInfoMessage.Text = responseMessage;
                            else
                                this.lblErrorMessage.Text = responseMessage;
                            break;
                        //case PinPageLayout.STATUS_CONFIRM:
                        //    if (_pinService.UpdatePinBatchStatus(PinBatchId.Value, PinReissueResult.pin_batch_statuses_id, BatchResult.flow_pin_batch_statuses_id.Value, this.tbStatusNote.Text, out result, out responseMessage))
                        //        this.lblInfoMessage.Text = responseMessage;
                        //    else
                        //        this.lblErrorMessage.Text = responseMessage;
                        //    break;
                        //case PinPageLayout.SPECIAL_CONFIRM:
                        //    if (_pinService.UpdatePinBatchStatus(PinBatchId.Value, PinReissueResult.pin_batch_statuses_id, BatchResult.pin_batch_statuses_id, this.tbStatusNote.Text, out result, out responseMessage))
                        //        this.lblInfoMessage.Text = responseMessage;
                        //    else
                        //        this.lblErrorMessage.Text = responseMessage;
                        //    break;
                        //case PinPageLayout.REJECT_CONFIRM:
                        //    if (isNotesPopulated())
                        //    {
                        //        if (_pinService.PinBatchRejectStatus((long)PinBatchId, (int)PinReissueResult.reject_pin_batch_statuses_id.Value, this.tbStatusNote.Text, out result, out  responseMessage))
                        //            this.lblInfoMessage.Text = responseMessage;
                        //        else
                        //            this.lblErrorMessage.Text = responseMessage;
                        //    }

                        //    break;
                        default:
                            break;
                    }
                }

                if (result != null && result.pin_reissue_id > 0)
                {
                    //PopulatePage(result.pin_reissue_id);
                    UpdatePageLayout(PinPageLayout.READ);
                    //PopulatePage(result);
                    PopulatePage(result);
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
                    SessionWrapper.PinReissueSearchParams = SearchParam;

                Server.Transfer("~\\webpages\\pin\\PinResetList.aspx");
            }
            else
            {
                UpdatePageLayout(PinPageLayout.READ);
            }
        }

        private Boolean isNotesPopulated()
        {
            if (String.IsNullOrWhiteSpace(this.tbComments.Text))
            {
                this.lblErrorMessage.Text = GetGlobalResourceObject("DefaultExceptions", "ValidationRejectReason").ToString();
                return false;
            }

            return true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text =
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                UpdatePageLayout(PinPageLayout.CANCEL_CONFIRM);
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