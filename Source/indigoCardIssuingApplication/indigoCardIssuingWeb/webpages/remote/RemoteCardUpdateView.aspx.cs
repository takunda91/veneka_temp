using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.remote
{
    public partial class RemoteCardUpdateView : BasePage
    {
        private enum DistPageLayout
        {
            READ,
            RESEND_CONFIRM,
            COMPLETE_CONFIRM
        }

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.AUDITOR};

        private static readonly ILog log = LogManager.GetLogger(typeof(RemoteCardUpdateView));
        private readonly RemoteService _remoteService = new RemoteService();
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
        private void LoadPageData(long? remoteCardId)
        {
            try
            {
                if (SessionWrapper.RemoteCardUpdateSearchParams != null)
                {
                    SearchParam = (RemoteCardUpdateSearchParameters)SessionWrapper.RemoteCardUpdateSearchParams;
                    SessionWrapper.RemoteCardUpdateSearchParams = null;
                }

                //See if theres anything in the session.
                if (remoteCardId == null && SessionWrapper.RemoteCardId != null)
                {
                    remoteCardId = SessionWrapper.RemoteCardId;
                }


                if (Request.QueryString["page"] != null)
                {
                    //PreviousPage = Request.QueryString["page"].ToString();
                }
                RemoteCardUpdateDetailResult remoteCardDetails = null;

                if (remoteCardId != null)
                {
                    remoteCardDetails = PopulatePage(remoteCardId.GetValueOrDefault());

                    if (remoteCardDetails != null)
                    {
                        SetButtons(remoteCardDetails);
                        this.RemoteCardId = remoteCardDetails.card_id;
                        Result = remoteCardDetails;
                    }

                    SessionWrapper.RemoteCardId = null;
                }

                if (remoteCardDetails == null)
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
        /// <param name="remoteCardId"></param>
        /// <returns></returns>
        private RemoteCardUpdateDetailResult PopulatePage(long? remoteCardId)
        {
            RemoteCardUpdateDetailResult remoteCardDetails = null;

            if (remoteCardId != null)
            {
                remoteCardDetails = _remoteService.GetRemoteCardDetail(remoteCardId.Value);
            }

            return PopulatePage(remoteCardDetails);
        }

        /// <summary>
        /// Use this method to populate the page and set the pages buttons according to batch status and user role.         
        /// </summary>
        /// <param name="distBatch"></param>
        /// <returns></returns>
        private RemoteCardUpdateDetailResult PopulatePage(RemoteCardUpdateDetailResult remoteCardDetails)
        {
            SetButtons(remoteCardDetails);
            Result = remoteCardDetails;

            if (remoteCardDetails != null)
            {
                this.tbBranchName.Text = base.FormatNameAndCode(remoteCardDetails.branch_name, remoteCardDetails.branch_code);
                this.tbCardReference.Text = remoteCardDetails.card_request_reference;
                this.tbIssuerName.Text = base.FormatNameAndCode(remoteCardDetails.issuer_name, remoteCardDetails.issuer_code);
                this.tbPAN.Text = remoteCardDetails.card_number;
                this.tbRemoteAddress.Text = remoteCardDetails.remote_component;
                this.tbRemoteStatus.Text = remoteCardDetails.remote_update_statuses_name;
                //this.tbRemoteUpdateTime.Text = remoteCardDetails.remote_updated_time ? remoteCardDetails.remote_updated_time.ToString(DATETIME_FORMAT) : String.Empty;
                //this.tbStatusDate.Text = remoteCardDetails.status_date.ToString(DATETIME_FORMAT);
                this.tbStatusNote.Text = remoteCardDetails.comments;
            }
            else
            {
                this.lblInfoMessage.Text = "Details not found. Please try again.";
            }

            return remoteCardDetails;
        }

        /// <summary>
        /// This method sets the pages buttons according to the batches status and the users role.
        /// </summary>
        /// <param name="loadBatch"></param>
        private void SetButtons(RemoteCardUpdateDetailResult remoteCardDetails)
        {
            //disable all buttons by default.            
            this.btnComplete.Visible = false;
            this.btnResend.Visible = false;
            this.btnConfirm.Visible = false;
            this.tbStatusNote.Enabled = this.btnConfirm.Enabled = false;            

            //If a load batch has been passed set the buttons accordingly.
            if (remoteCardDetails != null)
            {
                bool canUpdate;
                bool canRead;
                bool canCreate;

                //Resend the card
                if ((remoteCardDetails.remote_update_statuses_id != 0 && remoteCardDetails.remote_update_statuses_id != 2 && remoteCardDetails.remote_update_statuses_id != 3)
                    && PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CMS_OPERATOR, remoteCardDetails.issuer_id.Value, out canRead, out canUpdate, out canCreate))
                {
                    if (canUpdate)
                    {
                        this.btnResend.Visible =
                        this.tbStatusNote.Enabled = true;

                        this.tbStatusNote.Text = String.Empty;
                    }
                }

                if ((remoteCardDetails.remote_update_statuses_id != 2)
                            && PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_MANAGER, remoteCardDetails.issuer_id.Value, out canRead, out canUpdate, out canCreate)) //Request to export again
                {
                    if (canUpdate)
                    {
                        this.btnComplete.Visible =
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
                    PopulatePage(this.RemoteCardId);
                    break;
                case DistPageLayout.RESEND_CONFIRM:
                    this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                    RejectConfirmation();
                    break;
                case DistPageLayout.COMPLETE_CONFIRM:
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

            this.btnComplete.Visible = false;
            this.btnResend.Visible = false;

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
                this.lblErrorMessage.Text = GetLocalResourceObject("ValidationCompleteReason").ToString();
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
                    SessionWrapper.RemoteCardUpdateSearchParams = SearchParam;
                    Server.Transfer("~\\webpages\\remote\\RemoteCardUpdateList.aspx");
            }
            else
            {
                UpdatePageLayout(DistPageLayout.READ);
            }
        }

        protected void btnResend_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                UpdatePageLayout(DistPageLayout.RESEND_CONFIRM);
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

        protected void btnComplete_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                UpdatePageLayout(DistPageLayout.COMPLETE_CONFIRM);
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
                RemoteCardUpdateDetailResult remoteCardDetails = null;

                if (RemoteCardId != null)
                {
                    switch (disPageLayout)
                    {
                        case DistPageLayout.READ:
                            break;
                        case DistPageLayout.RESEND_CONFIRM:                            
                                if (_remoteService.ChangeRemoteCardStatus(RemoteCardId.Value, 3, this.tbStatusNote.Text.Trim(), out remoteCardDetails, out responseMessage))
                                    this.lblInfoMessage.Text = responseMessage;
                                else
                                    this.lblErrorMessage.Text = responseMessage;
                            break;
                        case DistPageLayout.COMPLETE_CONFIRM:
                            if (isNotesPopulated())
                                if (_remoteService.ChangeRemoteCardStatus(RemoteCardId.Value, 2, this.tbStatusNote.Text.Trim(), out remoteCardDetails, out responseMessage))
                                    this.lblInfoMessage.Text = responseMessage;
                                else
                                    this.lblErrorMessage.Text = responseMessage;
                            break;
                        default:
                            break;
                    }
                }

                if (remoteCardDetails != null)
                {
                    //UpdatePageLayout(DistPageLayout.READ);
                    PopulatePage(remoteCardDetails);
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

        //protected void btnPDFReport_Click(object sender, EventArgs e)
        //{
        //    this.lblInfoMessage.Text = "";
        //    this.lblErrorMessage.Text = "";

        //    try
        //    {
        //        if (ExportBatchId != null)
        //        {
        //            var reportBytes = _batchService.GenerateExportBatchReport(ExportBatchId.Value);

        //            string reportName = String.Empty;

        //            reportName = "Export_Report_";
        //            reportName += ExportBatchId.Value.ToString() + "_" + DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

        //            Response.Clear();
        //            MemoryStream ms = new MemoryStream(reportBytes);
        //            Response.ContentType = "application/pdf";
        //            Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
        //            Response.Buffer = true;
        //            ms.WriteTo(Response.OutputStream);
        //            Response.Flush();
        //            //Response.End();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
        //        if (log.IsDebugEnabled || log.IsTraceEnabled)
        //        {
        //            this.lblErrorMessage.Text = ex.ToString();
        //        }
        //    }
        //}

        #endregion

        #region ViewState Variables
        private long? RemoteCardId
        {
            get
            {
                if (ViewState["RemoteCardId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["RemoteCardId"].ToString());
            }
            set
            {
                ViewState["RemoteCardId"] = value;
            }
        }

        private RemoteCardUpdateDetailResult Result
        {
            get
            {
                if (ViewState["Result"] == null)
                    return null;
                else
                    return (RemoteCardUpdateDetailResult)ViewState["Result"];
            }
            set
            {
                ViewState["Result"] = value;
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

        private RemoteCardUpdateSearchParameters SearchParam
        {
            get
            {
                if (ViewState["SearchParam"] == null)
                    return null;
                else
                    return (RemoteCardUpdateSearchParameters)ViewState["SearchParam"];
            }
            set
            {
                ViewState["SearchParam"] = value;
            }
        }
        #endregion
    }
}