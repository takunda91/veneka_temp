using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Veneka.Indigo.Notifications.NotificationChannels;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class PinListView : BasePage
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
                this.btnConfirmReissue.Enabled = false;
                this.btnConfirmReissue.Visible = false;
                this.lblNumberOfTimesSent.Visible = false;
                this.tbNumberOfTimesSent.Visible = false;
                this.lblLastSentDate.Visible = false;
                this.tbLastSentDate.Visible = false;
                this.btnResendPin.Visible = false;
                this.lblReadOnlyRejectComment.Visible = false;
                this.tbReadOnlyRejectComment.Visible = false;
                this.lblReadOnlyRejectDate.Visible = false;
                this.tbReadOnlyRejectDate.Visible = false;

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
                }

                PinRequestViewDetails pinRequestView = null;

                if (pinRequestId != null)
                {
                    pinRequestView = PopulatePage(pinRequestId.GetValueOrDefault());

                    if (pinRequestView != null)
                    {
                        //   SetButtons(pinBatch);
                        pinRequestId = pinRequestView.pin_request_id;
                    }


                }

                if (pinRequestId == null)
                {
                    this.lblErrorMessage.Text = "No pin request details to show.";
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

        private PinRequestViewDetails PopulatePage(PinRequestViewDetails pinRequestDetails)
        {
            // SetButtons(pinRequestDetails);

            try
            {
                if (pinRequestDetails != null)
                {

                    var month = pinRequestDetails.expiry_period.ToString();
                    var pin_request_status = pinRequestDetails.pin_request_status.Trim();
                    this.tbRequestStatus.Text = pin_request_status;
                    this.tbNumberOfTimesSent.Text = pinRequestDetails.number_of_times_sent.ToString();
                    this.tbLastSentDate.Text = pinRequestDetails.last_send_date.ToLongDateString();
                    int? pin_request_type = SessionWrapper.PinRequestType;

                    if(pin_request_type != null)
                    {
                        if(pin_request_type == 0)
                        {
                            this.btnReissueApprove.Visible =
                            this.btnReissueReject.Visible = 
                            this.btnConfirmReissue.Visible = false;
                        }
                        else if(pin_request_type == 1)
                        {
                            this.btnDecryptPin.Visible =
                            this.btnRejectPin.Visible =
                            this.btnResendPin.Visible = 
                            this.btnConfirm.Visible = false;
                            this.lblRejectComments.Text = "Re-Issue Reject Comments";
                        }
                    }
                    else
                    {
                        //error message for null request type
                        this.lblErrorMessage.Text = "Pin request type is not defined";

                        return pinRequestDetails;
                    }

                    if (pin_request_status.Equals("Sent"))
                    {
                        this.lblNumberOfTimesSent.Visible =
                        this.tbNumberOfTimesSent.Visible =
                        this.lblLastSentDate.Visible =
                        this.btnResendPin.Visible =
                        this.tbLastSentDate.Visible = true;


                        this.btnDecryptPin.Visible = false;
                        this.btnRejectPin.Visible = false;
                        this.lblRejectComments.Visible = false;
                        this.tbRejectComments.Visible = false;
                         
                    }
                    else if (pin_request_status.Equals("Rejected"))
                    {
                        this.lblReadOnlyRejectComment.Visible =
                        this.tbReadOnlyRejectComment.Visible =
                        this.lblReadOnlyRejectDate.Visible =
                        this.tbReadOnlyRejectDate.Visible = true;

                        this.pnlButtons.Visible = false;
                        this.lblRejectComments.Visible = false;
                        this.tbRejectComments.Visible = false;
                    }

                    string month_name = null;
                    if (month.Substring(month.Length - 2) == "01")
                    {
                        month_name = "January";
                    }
                    else if (month.Substring(month.Length - 2) == "02")
                    {
                        month_name = "February";
                    }
                    else if (month.Substring(month.Length - 2) == "03")
                    {
                        month_name = "March";
                    }
                    else if (month.Substring(month.Length - 2) == "04")
                    {
                        month_name = "April";
                    }
                    else if (month.Substring(month.Length - 2) == "05")
                    {
                        month_name = "May";
                    }
                    else if (month.Substring(month.Length - 2) == "06")
                    {
                        month_name = "June";
                    }
                    else if (month.Substring(month.Length - 2) == "07")
                    {
                        month_name = "July";
                    }
                    else if (month.Substring(month.Length - 2) == "08")
                    {
                        month_name = "August";
                    }
                    else if (month.Substring(month.Length - 2) == "09")
                    {
                        month_name = "September";
                    }
                    else if (month.Substring(month.Length - 2) == "10")
                    {
                        month_name = "October";
                    }
                    else if (month.Substring(month.Length - 2) == "11")
                    {
                        month_name = "November";
                    }
                    else if (month.Substring(month.Length - 2) == "12")
                    {
                        month_name = "December";
                    }
                    this.tbRequestReference.Text = pinRequestDetails.pin_request_reference;
                    this.tbProductName.Text = pinRequestDetails.product_name;
                    this.tbIssuer.Text = pinRequestDetails.issuer_name;
                    this.tbIssuingBranch.Text = pinRequestDetails.branch_name;
                    this.tbCardPan.Text = pinRequestDetails.masked_pan;
                    this.tbExpiryYear.Text = pinRequestDetails.expiry_period.ToString().Substring(0, 4);
                    this.tbExpiryMonth.Text = month_name;
                    this.tbPinDistMethod.Text = pinRequestDetails.pin_dist_method;
                    this.tbAccountNumber.Text = pinRequestDetails.account_number;
                    this.tbEmail.Text = pinRequestDetails.account_email;
                    this.tbContactNumber.Text = pinRequestDetails.account_contact;
                    this.tbProductId.Text = pinRequestDetails.product_id.ToString();
                    this.tbMonthNumber.Text = month;
                    this.tbIssuerId.Text = pinRequestDetails.issuer_id.ToString();
                    this.tbReadOnlyRejectComment.Text = pinRequestDetails.reject_reason;
                    this.tbReadOnlyRejectDate.Text = pinRequestDetails.reject_date.ToLongDateString();
                }
                else
                {
                    this.lblInfoMessage.Text = "Card Request not found. Please try again.";
                }

                RequestResult = pinRequestDetails;

                return pinRequestDetails;
            }
            catch (Exception ex)
            {

                log.Error(ex);
                this.lblErrorMessage.Text = ex.ToString();
                return pinRequestDetails;
            }

           
        }

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

        private PinRequestViewDetails PopulatePage(long? pinRequestId)
        {
            PinRequestViewDetails pinRequestList = null;

            if(pinRequestId != null)
            {
                pinRequestList = _pinService.GetPinRequestView(pinRequestId.Value);
            }
              
                 RequestResult = pinRequestList;

            return PopulatePage(pinRequestList);
        }

        
        private PinRequestViewDetails RequestResult
        {
            get
            {
                if (ViewState["RequestResult"] == null)
                    return null;
                else
                    return (PinRequestViewDetails)ViewState["RequestResult"];
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

        protected void btnDecryptPin_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            
            try
            {
                //Pass all validations, set to confirm.
                if (validateForm(true))
                {
                    ConfirmSave = true;
                    DisplayAllFields(false);
                    this.btnCancel.Enabled = this.btnCancel.Visible = true;
                    this.btnResendPin.Visible = 
                    this.btnRejectPin.Visible = 
                    this.tbRejectComments.Visible = 
                    this.lblRejectComments.Visible = 
                    this.btnConfirmReissue.Visible = false;

                    this.lblInfoMessage.Text = "Please click confirm button to confirm ...";
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

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {

            int? pin_request_typer = SessionWrapper.PinRequestType;
            if(pin_request_typer == 0)
            {
                this.decryptPin();
            }           

        }

        protected void btnConfirmReissue_Click(object sender, EventArgs e)
        {
            try
            {
                string ReissueStatus = PinRequestConfirmCurrent;
                string approval_user_role = null;
                string approval_comment = null;

                List<RolesIssuerResult> issuers;

                if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.BRANCH_CUSTODIAN, out issuers))
                {
                    approval_user_role = "BRANCH_CUSTODIAN";
                }
                else if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.CENTER_OPERATOR, out issuers))
                {
                    approval_user_role = "CENTER_OPERATOR";
                
                }
                else if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.CENTER_MANAGER, out issuers))
                {
                   approval_user_role = "CENTER_MANAGER";
                }

                if(ReissueStatus == "Rejected")
                {
                    approval_comment = this.tbRejectComments.Text;
                }


                PinObject ReIssueObject = new PinObject
                {
                    reissue_approval_stage = approval_user_role,
                    PinRequestStatus = ReissueStatus,
                    PinRequestId = (int)pinRequestId.GetValueOrDefault(),
                    request_comment = approval_comment

                };

                //update status for role
                var responseMessage = String.Empty;

                var update_status_for_role = _cardService.updatePinRequestStatusForRole(ReIssueObject, out responseMessage);
                if (update_status_for_role)
                {
                    //if center manager  send message to client on approval 
                    if(approval_user_role == "CENTER_MANAGER" && ReissueStatus == "Approved")
                    {
                        int web_service_feedback;
                        string message = String.Format("Dear Customer, a pin re-issue request has been approved and your pin is ready for collection. Please use this reference ({0}) on retrieval .. ", this.tbRequestReference.Text);
                        SMSController sms_block = new SMSController(this.tbContactNumber.Text,"Access Bank",message);
                        bool response = sms_block.sendSMS(out web_service_feedback);
                        if(web_service_feedback == 1701)
                        {
                            log.InfoFormat("SMS has been sent to client with status : {0}", web_service_feedback);
                        }
                        else
                        {
                            log.ErrorFormat("SMS failed to send with status : {0}", web_service_feedback);
                        }
                    }

                    this.lblInfoMessage.Text = responseMessage;
                    this.btnConfirmReissue.Visible = false;
                    this.btnCancel.Visible = false;
                }
                else
                {
                    this.lblErrorMessage.Text = responseMessage;
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

        protected void decryptPin()
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            var pin_request_id = pinRequestId.GetValueOrDefault();
            try
            {
               
                if (PinRequestConfirmCurrent == "reject")
                {
                    // reject logic
                    PinObject pin_details = new PinObject
                    {
                        PinRequestId = (int)pinRequestId.GetValueOrDefault(),
                        PinRequestStatus = "Rejected",
                        request_comment = this.tbRejectComments.Text
                    };

                    string update_pin_req_response = String.Empty;

                    var update_status = _cardService.updatePinRequestStatus(pin_details, out update_pin_req_response);
                    if (update_status)
                    {
                        log.InfoFormat("Status update successful. Status = {0}", update_pin_req_response);
                        this.lblInfoMessage.Text = "Pin request has been rejected successfully ...";
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

                    int issuerId;
                    //int branchId = int.Parse(this.tbIssuingBranch.Text);
                    var masked_pan = this.tbCardPan.Text;
                    //#if DEBUG
                    //                var product_bin_code = masked_pan.Substring(0, masked_pan.IndexOf('*'));
                    //#else
                    var product_bin_code = masked_pan.Substring(0, 4);
                    //#endif
                    var last_four_of_pan = masked_pan.Substring(masked_pan.LastIndexOf('*') + 1);
                    var expiry_date = this.tbMonthNumber.Text;
                    var pin_dist_method = this.tbPinDistMethod.Text;
                    var terminal_data = _pinService.GetTerminalCardData(product_bin_code, last_four_of_pan, expiry_date);
                    // terminal_data.
                    string operator_code = String.Empty;
                    int product_id = int.Parse(this.tbProductId.Text);

                    int number_of_times_sent = int.Parse(this.tbNumberOfTimesSent.Text.Trim()) - 1;

                    if (number_of_times_sent < 0)
                    {
                        number_of_times_sent = 0;
                    }

                    if (int.TryParse(this.tbIssuerId.Text, out issuerId) && terminal_data.PINBlock != null)
                    {

                        var approval_status = String.Empty;
                        if (!String.IsNullOrWhiteSpace(terminal_data.approval_status))
                        {
                            approval_status = terminal_data.approval_status.Trim();
                        }

                        if (approval_status == "Approved")
                        {
                            var zone_info = _pinService.GetZoneKeys(issuerId);
                            var pin_message = _issuerMan.GetIssuerPinMessage(issuerId);

                            int config_max_allowable_pin_send_days = pin_message.max_number_of_pin_send;
                            int block_delete_days = pin_message.pin_block_delete_days;


                            if (number_of_times_sent > 0 && (number_of_times_sent >= config_max_allowable_pin_send_days))
                            {
                                this.lblErrorMessage.Text = "You have reached a maximum number of times a pin resend can be done";
                                log.Error(this.lblErrorMessage.Text);
                            }
                            else
                            {


                                TerminalCardData cardData = new TerminalCardData
                                {
                                    PINBlock = terminal_data.PINBlock,
                                    PAN = terminal_data.PAN
                                };

                                ZoneMasterKey zmk = new ZoneMasterKey
                                {
                                    Zone = zone_info.Zone,
                                    Final = zone_info.Final
                                };

                                TerminalSessionKey zpk = new TerminalSessionKey
                                {
                                    RandomKey = zone_info.RandomKey,
                                    RandomKeyUnderLMK = zone_info.RandomKeyUnderLMK
                                };

                                Product product = new Product
                                {
                                    ProductId = product_id,
                                    BIN = product_bin_code,
                                    ProductName = this.tbProductName.Text
                                };

                                Messaging message = new Messaging
                                {
                                    pin_dist_method = pin_dist_method.Trim(),
                                    message_body = pin_message.pin_notification_message
                                };

                                DecryptedFields decryptedFields;
                                string responseMessage;

                                CustomerDetailsResult customer_info = new CustomerDetailsResult
                                {
                                    contact_number = this.tbContactNumber.Text,
                                    customer_account_number = masked_pan,
                                    customer_first_name = this.tbEmail.Text
                                };




                                if (_cardService.PinFieldDecryptionWithMessaging(issuerId, zmk, cardData, zpk, operator_code, product, customer_info, message, out decryptedFields, out responseMessage))
                                {
                                    log.Info(decryptedFields);

                                    if (responseMessage.Trim() == "Message sent successfully.")
                                    {


                                        PinObject pin_details = new PinObject
                                        {
                                            PinRequestId = (int)pinRequestId.GetValueOrDefault(),
                                            PinRequestStatus = "Sent",
                                            request_comment = null
                                        };

                                        string update_pin_req_response = String.Empty;

                                        var update_status = _cardService.updatePinRequestStatus(pin_details, out update_pin_req_response);
                                        if (update_status)
                                        {
                                            number_of_times_sent = number_of_times_sent + 1;
                                            log.InfoFormat("Status update successful. Status = {0}", update_pin_req_response);

                                            if ((config_max_allowable_pin_send_days == 0 || block_delete_days == 0)
                                                        || number_of_times_sent > 0 && (number_of_times_sent >= config_max_allowable_pin_send_days))
                                            {
                                                var deletePinBlock = _cardService.deletePinBlock(product_bin_code, last_four_of_pan, expiry_date, out update_pin_req_response);

                                                if (deletePinBlock)
                                                {
                                                    log.InfoFormat("Pin Block delete with status = {0}", update_pin_req_response);
                                                }
                                            }

                                        }
                                        else
                                        {
                                            log.ErrorFormat("Status update Failed. Status = {0}", update_pin_req_response);
                                        }




                                        this.btnConfirm.Visible = false;
                                        this.btnCancel.Visible = false;


                                    }
                                    this.lblInfoMessage.Text = responseMessage;



                                }
                                else
                                {
                                    #region JUST DEBUGGING
                                    //string update_pin_req_response = String.Empty;
                                    //number_of_times_sent = number_of_times_sent + 1;
                                    //log.InfoFormat("Status update successful. Status = {0}", update_pin_req_response);
                                    //PinObject pin_details = new PinObject
                                    //{
                                    //    PinRequestId = (int)pinRequestId.GetValueOrDefault(),
                                    //    PinRequestStatus = "Sent",
                                    //    request_comment = String.Empty
                                    //};

                                    //var update_status = _cardService.updatePinRequestStatus(pin_details, out update_pin_req_response);

                                    //if ((config_max_allowable_pin_send_days == 0 || block_delete_days == 0)
                                    //            || number_of_times_sent > 0 && (number_of_times_sent >= config_max_allowable_pin_send_days))
                                    //{
                                    //    var deletePinBlock = _cardService.deletePinBlock(product_bin_code, last_four_of_pan, expiry_date, out update_pin_req_response);

                                    //    if (deletePinBlock)
                                    //    {
                                    //        log.InfoFormat("Pin Block delete with status = {0}", update_pin_req_response);
                                    //    }

                                    #endregion

                                    this.lblErrorMessage.Text = responseMessage;
                                    log.Error(responseMessage);
                                }
                            }
                        }
                        else
                        {
                            this.lblErrorMessage.Text = String.Format("Pin File has not been approved for pin issue. Please have the center manager approve first");
                            log.Error(String.Format("Terminal data for pin block {0} needs approval first..", terminal_data.PINBlock));
                        }
                    }
                    else
                    {
                        this.lblErrorMessage.Text = String.Format("No Terminal Data or Pin block found for Pan {0} and Expiry period {1} with special request pan {2}.", masked_pan, expiry_date, product_bin_code);
                        log.Error("No Pin Block Retrieved");
                    }
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
            PinRequestViewDetails pinRequestList = _pinService.GetPinRequestView(pinRequestId.Value);
            var pin_request_status = pinRequestList.pin_request_status;
            int status = 0;
            if (pin_request_status.Equals("NewRequest"))
            {
                status = 0;
            }
            else if (pin_request_status.Equals("Sent"))
            {
                status = 1;
            }
            else if (pin_request_status.Equals("Rejected"))
            {
                status = 2;
            }

            int? pin_request_type = SessionWrapper.PinRequestType;

            Response.Redirect("~\\webpages\\pin\\PinRequestList.aspx?status=" + status.ToString() + "&requestType=" + pin_request_type.ToString());
               
        }

        private bool validateForm(bool validateDocuments = false)
        {
            int issuerId;
            try
            {

           
            if(int.TryParse(this.tbIssuerId.Text, out issuerId))
            {
                var pin_message = _issuerMan.GetIssuerPinMessage(issuerId);

                if(String.IsNullOrWhiteSpace(pin_message.pin_notification_message))
                {
                    this.lblErrorMessage.Text = "Default Message is not set for this issuer, please configure in Issuer settings.";
                }
            }
            else
            {
                this.lblErrorMessage.Text = "Issuer not found";
            }

            
            // validate send message in configs
            if (this.lblErrorMessage.Text.Length > 0)
                return false;
            else
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = ex.ToString();
                return false;
            }
        }

      

        private void DisplayAllFields(bool showAll)
        {
            this.tbAccountNumber.Enabled = showAll;
            this.tbCardPan.Enabled = showAll;
            this.tbContactNumber.Enabled = showAll;
            this.tbEmail.Enabled = showAll;
            this.tbExpiryMonth.Enabled = showAll;
            this.tbExpiryYear.Enabled = showAll;
            this.tbIssuer.Enabled = showAll;
            this.tbPinDistMethod.Enabled = showAll;
            this.tbProductName.Enabled = showAll;
            this.tbRequestReference.Enabled =
            this.tbContactNumber.Enabled = showAll;
            this.btnConfirm.Visible = showAll ? false : true;
            this.btnConfirmReissue.Visible = showAll ? false : true;
            this.btnDecryptPin.Visible = showAll;
            this.btnConfirm.Enabled = showAll ? false : true;
            this.btnConfirmReissue.Enabled = showAll ? false : true;
            this.btnConfirmReissue.Visible = showAll ? false : true;

        }

        private void DisplayAllFieldsOnResend(bool showAll)
        {
            this.tbAccountNumber.Enabled = showAll;
            this.tbCardPan.Enabled = showAll;
            this.tbContactNumber.Enabled = showAll;
            this.tbEmail.Enabled = showAll;
            this.tbExpiryMonth.Enabled = showAll;
            this.tbExpiryYear.Enabled = showAll;
            this.tbIssuer.Enabled = showAll;
            this.tbPinDistMethod.Enabled = showAll;
            this.tbProductName.Enabled = showAll;
            this.tbRequestReference.Enabled =
            this.tbContactNumber.Enabled = showAll;
            this.btnConfirm.Visible = showAll ? false : true;
            this.btnConfirmReissue.Visible = showAll ? false : true;
            this.btnResendPin.Visible = showAll;
            this.btnConfirm.Enabled = showAll ? false : true;
            this.btnConfirmReissue.Enabled = showAll ? false : true;

        }

        protected void btnResendPin_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                //Pass all validations, set to confirm.
                if (validateForm(true))
                {
                    ConfirmSave = true;
                    DisplayAllFieldsOnResend(false);
                    this.btnCancel.Enabled = this.btnCancel.Visible = true;
                    this.btnDecryptPin.Visible = false;
                    this.btnRejectPin.Visible = false;
                    this.btnConfirmReissue.Visible = false;
                    this.lblInfoMessage.Text = "Pin resend request has been initiated. Please confirm ...";
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

        protected void btnRejectPin_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            try
            {
                if (String.IsNullOrWhiteSpace(this.tbRejectComments.Text))
                {
                    this.lblErrorMessage.Text = "Please provide comment for rejecting this pin request.";
                }
                else
                {
                    ConfirmSave = true;
                    PinRequestConfirmCurrent = "reject";
                    DisplayAllFields(false);
                    this.btnCancel.Enabled = this.btnCancel.Visible = true;
                    this.btnDecryptPin.Visible = false;
                    this.btnResendPin.Visible = false;
                    this.tbRejectComments.Enabled = false;
                    this.btnRejectPin.Visible = false;
                    this.btnConfirmReissue.Visible = false;
                    this.lblInfoMessage.Text = "Are you sure you want to reject this pin  request..";
                }
            }
            catch (Exception ex)
            {

                log.Error(ex.ToString());
                this.lblErrorMessage.Text = ex.ToString();
            }
           
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        protected void btnApproveReissue_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                PinRequestConfirmCurrent = "Approved";
                    ConfirmSave = true;
                    DisplayAllFields(false);
                    this.btnCancel.Enabled = this.btnCancel.Visible = true;
                this.btnResendPin.Visible =
                this.btnRejectPin.Visible =
                this.tbRejectComments.Visible =
                this.lblRejectComments.Visible =
                this.btnDecryptPin.Visible =
                this.btnReissueReject.Visible =
                this.btnConfirm.Visible =
                this.btnReissueApprove.Visible = false;

                    this.lblInfoMessage.Text = "Please click confirm button to approve reissue ...";
                
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

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        protected void btnRejectReissue_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            try
            {
                if (String.IsNullOrWhiteSpace(this.tbRejectComments.Text))
                {
                    this.lblErrorMessage.Text = "Please provide comment for rejecting this pin request.";
                }
                else
                {
                    ConfirmSave = true;
                    PinRequestConfirmCurrent = "Rejected";
                    DisplayAllFields(false);
                    this.btnCancel.Enabled = this.btnCancel.Visible = true;
                    this.btnDecryptPin.Visible =
                    this.btnResendPin.Visible =
                    this.tbRejectComments.Enabled =
                    this.btnRejectPin.Visible =
                    this.btnConfirm.Visible =
                    this.btnReissueReject.Visible = 
                    this.btnReissueApprove.Visible = false;

                    this.lblInfoMessage.Text = "Are you sure you want to reject this pin re-issue request..";
                }
            }
            catch (Exception ex)
            {

                log.Error(ex.ToString());
                this.lblErrorMessage.Text = ex.ToString();
            }
        }

        
    }
}