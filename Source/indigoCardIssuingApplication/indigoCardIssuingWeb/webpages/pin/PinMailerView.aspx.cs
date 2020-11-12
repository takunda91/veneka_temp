using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class PinMailerView : BasePage
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(PinListView));
        private readonly UserRole[] userRolesForPage = new UserRole[]  { UserRole.CENTER_MANAGER, UserRole.CENTER_OPERATOR,
                                                                         UserRole.BRANCH_CUSTODIAN, UserRole.BRANCH_OPERATOR,
                                                                         UserRole.PIN_OPERATOR, UserRole.AUDITOR,
                                                                         UserRole.PIN_PRINTER_OPERATOR, UserRole.CMS_OPERATOR,
                                                                         UserRole.CARD_PRODUCTION, UserRole.CARD_CENTRE_PIN_OFFICER,
                                                                         UserRole.BRANCH_PIN_OFFICER,
                                                                         UserRole.CREDIT_ANALYST, UserRole.CREDIT_MANAGER};

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly CustomerCardIssueService _cardService = new CustomerCardIssueService();
        private readonly PINManagementService _pinService = new PINManagementService();
        private readonly DocumentManagementService _documentMan = new DocumentManagementService();
        private readonly RenewalService _renewalService = new RenewalService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();

        #region PAGE LOAD
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private void LoadPageData()
        {
            try
            {
                this.btnConfirm.Enabled = false;
                this.btnConfirm.Visible = false;
                //this.lblApprovalComments.Visible = false;
                //this.tbApprovalComments.Visible = false;
                this.lblApprovalDate.Visible = false;
                this.tbApprovalDate.Visible = false;
                this.lblApprovalStatus.Visible = false;
                this.tbApprovalStatus.Visible = false;
        
                long? pin_requestId = null;

                if (SessionWrapper.PinRequestId != null)
                {
                    pin_requestId = SessionWrapper.PinRequestId;
                    SessionWrapper.PinRequestId = null;
                }
                if (SessionWrapper.PinRequestSearchParams != null)
                {
                    SearchParam = SessionWrapper.PinRequestSearchParams;
                    SessionWrapper.PinRequestSearchParams = null;
                }
                if (SessionWrapper.CardSearchParams != null && SessionWrapper.CardSearchParams.PreviousSearchParameters != null)
                {
                    SearchParam = (PinRequestSearchParameters)SessionWrapper.CardSearchParams.PreviousSearchParameters;
                    pin_requestId = ((CardSearchParameters)SessionWrapper.CardSearchParams).PinbatchId;
                    SessionWrapper.CardSearchParams = null;
                }
                //See if theres anything in the session.
                if (pinRequestId == null && pin_requestId != null)
                {
                    pinRequestId = pin_requestId;
                    this.tbheaderid.Text = pin_requestId.ToString();
                }

                TerminalCardData pinRequestView = null;

                if (pinRequestId != null)
                {
                    pinRequestView = PopulatePage(pinRequestId.GetValueOrDefault());

                    if (pinRequestView != null)
                    {
                        //   SetButtons(pinBatch);
                        pinRequestId = pinRequestView.PrimaryKeyId;
                    }


                }

                if (pinRequestId == null)
                {
                    this.lblErrorMessage.Text = "No pin list view to show.";
                }


            }
            catch (Exception ex)
            {
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


      
        #region STATE VARIABLES
        private PinRequestSearchParameters SearchParam
        {
            get
            {
                if (ViewState["SearchParam"] == null)
                    return null;
                else
                    return (PinRequestSearchParameters)ViewState["SearchParam"];
            }
            set
            {
                ViewState["SearchParam"] = value;
            }
        }

        private bool ConfirmSave
        {
            get
            {
                if (ViewState["ConfirmSave"] != null)
                    return (bool)ViewState["ConfirmSave"];
                else
                    return false;
            }
            set
            {
                ViewState["ConfirmSave"] = value;
            }
        }


        private long? pinRequestId
        {
            get
            {
                if (ViewState["pinRequestId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["pinRequestId"].ToString());
            }
            set
            {
                ViewState["pinRequestId"] = value;
            }
        }

        private TerminalCardData PopulatePage(long? pinRequestId)
        {
            TerminalCardData pinRequestList = null;

            if (pinRequestId != null)
            {
                pinRequestList = _pinService.GetPinBatchView(pinRequestId.Value);
            }

            RequestResult = pinRequestList;

            return PopulatePage(pinRequestList);
        }

        private TerminalCardData PopulatePage(TerminalCardData pinBlockDetails)
        {
            // SetButtons(pinRequestDetails);

            try
            {
                if (pinBlockDetails != null)
                {

                    var approval_status = string.Empty;
                    if (pinBlockDetails.approval_status != null)
                    {
                        approval_status = pinBlockDetails.approval_status.Trim();


                        this.tbApprovalStatus.Text = approval_status;
                        this.tbApprovalDate.Text = pinBlockDetails.approval_date.ToString("dd/MM/yyyy hh:mm tt");
                        //  this.tbApprovalComments.Text = pinBlockDetails.approval_comment;

                        if (approval_status.Equals("Approved") || approval_status.Equals("Rejected"))
                        {
                            this.lblApprovalComments.Visible =
                            this.tbApprovalComments.Visible =
                            this.lblApprovalDate.Visible =
                            this.tbApprovalDate.Visible =
                            this.lblApprovalStatus.Visible =
                            this.tbApprovalStatus.Visible = true;

                            this.btnApprove.Visible = false;
                            this.btnReject.Visible = false;
                            this.lblApprovalComments.Visible = false;
                            this.tbApprovalComments.Visible = false;

                        }
                    }
                   
                    this.tbBatchReference.Text = pinBlockDetails.header_batch_reference;
                    this.tbUploadDate.Text = pinBlockDetails.header_upload_date.ToString("dd/MM/yyyy hh:mm tt");
                    this.tbNumberOfCards.Text = pinBlockDetails.header_number_of_cards_on_request.ToString();
                    this.tbApprovalStatus.Text = pinBlockDetails.header_approval_status;
                    this.tbApprovalDate.Text = pinBlockDetails.header_approval_date.ToString("dd/MM/yyyy hh:mm tt");

                }
                else
                {
                    this.lblInfoMessage.Text = "Pin File Load Batch Information. Please check from the list.";
                }

                RequestResult = pinBlockDetails;

                return pinBlockDetails;
            }
            catch (Exception ex)
            {

                log.Error(ex);
                this.lblErrorMessage.Text = ex.ToString();
                return pinBlockDetails;
            }


        }


        private TerminalCardData RequestResult
        {
            get
            {
                if (ViewState["RequestResult"] == null)
                    return null;
                else
                    return (TerminalCardData)ViewState["RequestResult"];
            }
            set
            {
                ViewState["RequestResult"] = value;
            }
        }

        private string PinRequestConfirmCurrent
        {
            get
            {
                if (ViewState["PinRequestConfirmCurrent"] == null)
                    return null;
                else
                    return ViewState["PinRequestConfirmCurrent"].ToString();
            }
            set
            {
                ViewState["PinRequestConfirmCurrent"] = value;
            }
        }


        #endregion

        #region ACTION BUTTONS
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            try
            {
                
                    ConfirmSave = true;
                    PinRequestConfirmCurrent = "approve";
                    DisplayAllFields(false);
                    this.btnCancel.Enabled = this.btnCancel.Visible = true;
                    this.btnApprove.Visible = false;
                this.btnGenerateReport.Visible = false;
                this.tbApprovalComments.Enabled = false;
                    this.btnReject.Visible = false;
                    this.lblInfoMessage.Text = "Are you sure you want to approve this pin file upload?";
                
            }
            catch (Exception ex)
            {

                log.Error(ex.ToString());
                this.lblErrorMessage.Text = ex.ToString();
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            try
            {
                if (String.IsNullOrWhiteSpace(this.tbApprovalComments.Text))
                {
                    this.lblErrorMessage.Text = "Please provide a reason for rejecting this pin file upload.";
                }
                else
                {
                    ConfirmSave = true;
                    PinRequestConfirmCurrent = "reject";
                    DisplayAllFields(false);
                    this.btnCancel.Enabled = this.btnCancel.Visible = false;
                    this.btnApprove.Visible = false;
                    this.btnGenerateReport.Visible = false;
                    this.tbApprovalComments.Enabled = false;
                    this.btnReject.Visible = false;
                    this.lblInfoMessage.Text = "Are you sure you want to reject this pin  file upload?";
                }
            }
            catch (Exception ex)
            {

                log.Error(ex.ToString());
                this.lblErrorMessage.Text = ex.ToString();
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            //get PinRequestConfirmCurrent {approve, reject}
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            
            var pin_request_id = pinRequestId.GetValueOrDefault();
            try
            {
                if (PinRequestConfirmCurrent == "reject" || PinRequestConfirmCurrent == "approve")
                {
                    string approval_comment = String.Empty;
                    string approval_status = String.Empty;
                    if (PinRequestConfirmCurrent == "reject")
                    {
                        approval_comment = this.tbApprovalComments.Text;
                        approval_status = "Rejected";
                    }
                    else if(PinRequestConfirmCurrent == "approve")
                    {
                        approval_status = "Approved";
                        approval_comment = !String.IsNullOrWhiteSpace(this.tbApprovalComments.Text) ? this.tbApprovalComments.Text : String.Empty;
                    }

                    TerminalCardData pin_details = new TerminalCardData
                    {
                        header_pin_file_batch_id = int.Parse(this.tbheaderid.Text),
                        approval_comment = approval_comment,
                        approval_status = approval_status
                    };

                    string update_pin_req_response = String.Empty;

                    var update_status = _cardService.updatePinBatchStatus(pin_details, out update_pin_req_response);
                    if (update_status)
                    {
                        log.InfoFormat("Status update successful. Status = {0}", update_pin_req_response);
                        this.lblInfoMessage.Text = String.Format("Pin file batch has been updated to status {0} successfully ...", approval_status);
                    }
                    else
                    {
                        log.ErrorFormat("Status update Failed. Status = {0}", update_pin_req_response);
                        this.lblErrorMessage.Text = update_pin_req_response;
                    }

                    this.btnConfirm.Visible = false;
                    this.btnCancel.Visible = false;
                }
                else
                {
                    this.lblErrorMessage.Text = "Update Status is unknown";
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {

            int pin_header_id = int.Parse(this.tbheaderid.Text);

            TerminalCardData pinRequestList = _pinService.GetPinBatchView(pin_header_id);

            var pin_request_status = pinRequestList.approval_status;
            int status = 0;
            if (String.IsNullOrWhiteSpace(pin_request_status))
            {
                status = 0;
            }
            else if (pin_request_status.Equals("Approved"))
            {
                status = 1;
            }
            else if (pin_request_status.Equals("Rejected"))
            {
                status = 2;
            }
           

            Response.Redirect("~\\webpages\\pin\\PinMailerList.aspx?status=" + status.ToString());
        }
        #endregion

        #region DISPLAY BLOCK ON BUTTON CLICK
        private void DisplayAllFields(bool showAll)
        {
            this.tbBatchReference.Enabled = showAll;
            this.tbNumberOfCards.Enabled = showAll;
            this.tbUploadDate.Enabled = showAll;           
            this.btnConfirm.Visible = showAll ? false : true;
            this.btnApprove.Visible = showAll;
            this.btnReject.Visible = showAll;
            this.btnConfirm.Enabled = showAll ? false : true;

        }
        #endregion

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                var name = String.Empty;
                int pin_header_id = int.Parse(this.tbheaderid.Text);
                var reportBytes = _pinService.GeneratePinMailerBatchReport((int)pin_header_id);

                string reportName = "Pin_Load_Report_" + pin_header_id.ToString() + "_" +
                                        DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                Response.Clear();
                MemoryStream ms = new MemoryStream(reportBytes);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
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