using System;
using System.Drawing;
using System.Web.UI;
using System.Linq;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Text;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using System.Web.UI.HtmlControls;
using System.IO;
using indigoCardIssuingWeb.Old_App_Code.service;
using System.Web.Services;
using System.Web;
using System.Security.Principal;
using Veneka.Indigo.UX.NativeAppAPI;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;

namespace indigoCardIssuingWeb.webpages.hybrid
{
    public partial class HybridRequestView : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HybridRequestView));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.BRANCH_OPERATOR,
                                                                        UserRole.PIN_OPERATOR,
                                                                        UserRole.CARD_PRODUCTION,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.AUDITOR,
                                                                        UserRole.BRANCH_PRODUCT_MANAGER,
                                                                           UserRole.BRANCH_PRODUCT_OPERATOR };

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly CustomerCardIssueService _cardService = new CustomerCardIssueService();
        private readonly PINManagementService _pinService = new PINManagementService();
        private bool spoilcardflag = true;
        private enum RequestConfirm
        {
            CONFIRM_APPROVE,
            CONFIRM_REJECT
        }

        #region Page Load
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
                this.btnApprove.Enabled = this.btnApprove.Visible = false;

                RequestSearchParamters request;

                if (SessionWrapper.RequestSearchParam != null&& SessionWrapper.RequestId==null)
                {
                     request =(RequestSearchParamters)SessionWrapper.RequestSearchParam;

                    FetchCardDetails(request.RequestId);
                    RequestId = request.RequestId;
                    RequestSearchParamters = request;
                    SessionWrapper.RequestSearchParam = null; //Clear item out of the session.


                }
                else if (SessionWrapper.RequestId != null)
                {
                    RequestId = SessionWrapper.RequestId;

                    FetchCardDetails(RequestId);


                    SessionWrapper.RequestId = null; //Clear item out of the session.


                }
                else
                {
                    this.lblInfoMessage.Text = "Sorry there is no request information to display.";
                }

                if (Cache["custsettings"] != null)
                {
                    List<ConfigurationSettings> elements = (List<ConfigurationSettings>)Cache["custsettings"];
                    elements = elements.FindAll(i => i.PageName == "cardcapture").ToList();

                    ContentPlaceHolder myContent = (ContentPlaceHolder)this.Master.FindControl("MainContent");
                    HtmlGenericControl divcontrol = null;
                    foreach (var element in elements)
                    {
                        divcontrol = myContent.FindControl("div" + element.Key) as HtmlGenericControl;
                        if (divcontrol != null)
                        {
                            if (element.Visibility)
                            {
                                divcontrol.Attributes.Add("style", "display:block");

                            }
                            else
                            {
                                divcontrol.Attributes.Add("style", "display:none");

                            }
                        }
                    }
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


        private void ClearControls()
        {

            this.tbProductName.Text = String.Empty;
            this.tbCardNumber.Text = String.Empty;
            this.tbStatusDate.Text = String.Empty;
            this.tbFirstName.Text = String.Empty;
            this.tbIssuer.Text = String.Empty;
            this.tbRequestedBranch.Text = String.Empty;
            this.tbLastName.Text = String.Empty;
            this.tbIdNumber.Text = String.Empty;
            this.tbContactNumber.Text = String.Empty;
            this.tbMiddleName.Text = String.Empty;

            this.tbTitle.Text = String.Empty;
            //this.tbNameOnCard.Text = String.Empty;

            this.tbFee.Text = String.Empty;
            this.tbFeeRefNo.Text = String.Empty;
            this.tbFeeRevNo.Text = String.Empty;
            this.chkFeeOverridden.Checked = false;
            this.chkFeeWaived.Checked = false;

            this.tbRequestReference.Text = String.Empty;
            this.tbRequestPriority.Text = String.Empty;
            this.tbCardIssueMethod.Text = String.Empty;
        }

        #endregion

        #region Page Methods

        private RequestDetails FetchCardDetails(long? requestId)
        {
            ClearControls();

            if (requestId != null)
            {
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;

                var requestDetails = _batchService.GetRequestDetails(requestId.GetValueOrDefault());

                if (requestDetails != null)
                {
                    if (requestDetails.ProductFields != null)
                        AdditionalFields = requestDetails.ProductFields.ToList();



                    //Card Details
                    this.tbCardNumber.Text = utility.UtilityClass.FormatPAN(requestDetails.card_number) ?? "";
                    this.tbProductName.Text = requestDetails.product_name;
                    this.tbIssuer.Text = base.FormatNameAndCode(requestDetails.issuer_name, requestDetails.issuer_code);
                    this.tbRequestedBranch.Text = base.FormatNameAndCode(requestDetails.branch_name, requestDetails.branch_code);
                    this.tbCardIssueMethod.Text = requestDetails.card_issue_method_name;
                    this.tbRequestPriority.Text = requestDetails.card_priority_name;
                    this.tbRequestReference.Text = requestDetails.request_reference;

                    this.tbFee.Text = requestDetails.total_charged == null ? String.Empty : requestDetails.total_charged.Value.ToString();
                    this.tbFeeRefNo.Text = requestDetails.fee_reference_number;
                    this.tbFeeRevNo.Text = requestDetails.fee_reversal_ref_number;
                    this.chkFeeOverridden.Checked = requestDetails.fee_overridden_YN == null ? false : requestDetails.fee_overridden_YN.Value;
                    this.chkFeeWaived.Checked = requestDetails.fee_waiver_YN == null ? false : requestDetails.fee_waiver_YN.Value;

                    this.tbHybridCardStatus.Text = requestDetails.branch_card_statuses_name;

                    //Customer account details
                    if (!String.IsNullOrWhiteSpace(requestDetails.customer_account_number))
                    {
                        //this.hdnNameOnCard.Value = cardDetails.name_on_card;
                        this.tbStatusDate.Text = requestDetails.status_date != null ? requestDetails.status_date.ToString() : "N/A";
                        this.tbAccountNumber.Text = requestDetails.customer_account_number;
                        this.tbDomicileBranch.Text = utility.UtilityClass.FormatNameAndCode(requestDetails.domicile_branch_name, requestDetails.domicile_branch_code);
                        this.tbAccountType.Text = requestDetails.customer_account_type_name;
                        //this.tbIssuedDate.Text = requestDetails.date_issued != null ? requestDetails.date_issued.ToString() : "N/A";
                        this.tbFirstName.Text = requestDetails.customer_first_name;
                        this.tbMiddleName.Text = requestDetails.customer_middle_name;
                        //this.tbIssuedBy.Text = requestDetails.username;// cardDetails.user_first_name + " " + cardDetails.user_last_name;
                        this.tbLastName.Text = requestDetails.customer_last_name;

                        this.tbTitle.Text = requestDetails.customer_title_name;
                        this.tbCurrency.Text = String.Format("{0}/{1}", requestDetails.currency_code, requestDetails.iso_4217_numeric_code);
                        this.tbResident.Text = requestDetails.customer_residency_name;
                        this.tbCustomerType.Text = requestDetails.customer_type_name;
                        this.tbContractNumber.Text = requestDetails.contract_number ?? "";
                        this.tbContractId.Text = requestDetails.cms_id ?? "";
                        this.tbContactNumber.Text = requestDetails.contract_number;
                        this.tbIdNumber.Text = requestDetails.id_number;
                        this.tbContactNumber.Text = requestDetails.contact_number;

                        string firstName = String.IsNullOrEmpty(requestDetails.customer_first_name) ? "" : requestDetails.customer_first_name;
                        string middleName = String.IsNullOrEmpty(requestDetails.customer_middle_name) ? "" : requestDetails.customer_middle_name;
                        string initializedMiddleName = String.IsNullOrEmpty(GetInitializedMiddleName(requestDetails.customer_middle_name)) ? "" : GetInitializedMiddleName(requestDetails.customer_middle_name);
                        string lastName = String.IsNullOrEmpty(requestDetails.customer_last_name) ? "" : requestDetails.customer_last_name;
                        //this.tbNameOnCard.Text = String.IsNullOrEmpty(cardDetails.name_on_card) ? "" : cardDetails.name_on_card;
                    }

                    bool canRead;
                    bool canUpdate;
                    bool canCreate;

                    if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_CUSTODIAN, requestDetails.issuer_id, out canRead, out canUpdate, out canCreate))
                    {
                        if (requestDetails.hybrid_request_statuses_id == null)
                        {

                        }
                        else
                        {


                            if (HybridRequestStatuses.CREATED == (HybridRequestStatuses)requestDetails.hybrid_request_statuses_id &&
                                requestDetails.maker_checker_YN &&
                                canUpdate)
                            {
                                this.btnApprove.Enabled = this.btnApprove.Visible = true;
                                this.btnReject.Enabled = this.btnReject.Visible = true;

                            }
                        }
                    }
                    long? UserId = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.UserId;

                    if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_OPERATOR, requestDetails.issuer_id, out canRead, out canUpdate, out canCreate))
                    {

                        if (requestDetails.hybrid_request_statuses_id == null)
                        {

                        }
                        else
                        {


                            if (HybridRequestStatuses.ASSIGN_TO_BATCH == (HybridRequestStatuses)requestDetails.hybrid_request_statuses_id &&
                                   canUpdate &&
                                   requestDetails.card_issue_method_id == 1)
                            {
                                btnRefresh.Visible = true;
                            }


                        }


                    }


                }
                else
                {
                    //AdditionalFields = null;
                    this.lblInfoMessage.Text = "There are no card details to show.";
                }

                return requestDetails;
            }
            else
            {
                log.Error("Card Id is null, unable to fetch card details.");
                this.lblErrorMessage.Text = "There was an issue retrieving the card details. Please try reload the page.";
            }

            return null;
        }

        private string GetInitializedMiddleName(string middlename)
        {
            string rValue = "";
            if (!String.IsNullOrEmpty(middlename))
            {
                string[] parts = middlename.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in parts)
                {
                    rValue += s[0] + ". ";
                }

            }
            return rValue;
        }

        private void Rejectrequest()
        {
            string result;

            if (_cardService.RequestMakerChecker(RequestId.Value, false, this.tbSpoilComments.Text, out result))
            {
                this.lblInfoMessage.Text = result;
                spoilcardflag = false;
                FetchCardDetails(RequestId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;

                RequestConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = result;
            }
        }

        private void ApproveRequest()
        {
            string result;

            if (_cardService.RequestMakerChecker(RequestId.Value, true, this.tbSpoilComments.Text, out result))
            {
                this.lblInfoMessage.Text = result;
                spoilcardflag = false;
                FetchCardDetails(RequestId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;

                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                RequestConfirmCurrent = null;

            }
            else
            {
                this.lblErrorMessage.Text = result;
            }
        }



        #endregion

        #region Page Events

        protected void btnRefresh_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                FetchCardDetails(RequestId);
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoRefresh").ToString();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
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
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmApprove").ToString();

                RequestConfirmCurrent = RequestConfirm.CONFIRM_APPROVE;
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnBack.Visible = this.btnBack.Enabled = true;
                this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                this.tbSpoilComments.Enabled = false;

                this.btnApprove.Visible = this.btnApprove.Enabled =
                    this.btnReject.Visible = this.btnReject.Enabled =
                    this.btnRefresh.Visible = this.btnRefresh.Enabled = false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
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
                if (String.IsNullOrWhiteSpace(this.tbSpoilComments.Text))
                {
                    this.lblErrorMessage.Text = "Please provide comments for rejecting.";
                }
                else
                {
                    this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmReject").ToString();

                    RequestConfirmCurrent = RequestConfirm.CONFIRM_REJECT;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled =
                    this.btnBack.Visible = this.btnBack.Enabled = true;
                    this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                    this.tbSpoilComments.Enabled = false;

                    this.btnApprove.Visible = this.btnApprove.Enabled =
                        this.btnReject.Visible = this.btnReject.Enabled =
                        this.btnRefresh.Visible = this.btnRefresh.Enabled = false;

                    this.tbSpoilComments.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
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
                switch (RequestConfirmCurrent)
                {
                    case RequestConfirm.CONFIRM_APPROVE:
                        ApproveRequest();
                        break;
                    case RequestConfirm.CONFIRM_REJECT:
                        Rejectrequest();
                        break;


                }
            }
            catch (Exception ex)
            {
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
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.btnRefresh.Enabled = this.btnRefresh.Visible = true;

                switch (RequestConfirmCurrent)
                {
                    case RequestConfirm.CONFIRM_APPROVE:
                        this.btnApprove.Enabled = this.btnApprove.Visible =
                            this.btnReject.Enabled = this.btnReject.Visible = true;
                        this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled =

                        this.btnBack.Visible = this.btnBack.Enabled = true;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        RequestConfirmCurrent = null;
                        break;
                    case RequestConfirm.CONFIRM_REJECT:
                        this.btnApprove.Enabled = this.btnApprove.Visible =
                            this.btnReject.Enabled = this.btnReject.Visible = true;
                        this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled =

                        this.btnBack.Visible = this.btnBack.Enabled = true;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        this.tbSpoilComments.Enabled = true;
                        RequestConfirmCurrent = null;
                        break;

                    default:
                        if (RequestSearchParamters != null)

                        {
                            SessionWrapper.RequestSearchParam = RequestSearchParamters;
                            Server.Transfer("HybridRequestList.aspx");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        #endregion

        #region Page Properties

        private DistBatchSearchParameters SearchParam
        {
            get
            {
                if (ViewState["SearchParam"] == null)
                    return null;
                else
                    return (DistBatchSearchParameters)ViewState["SearchParam"];
            }
            set
            {
                ViewState["SearchParam"] = value;
            }
        }

        private RequestSearchParamters RequestSearchParamters
        {
            get
            {
                if (ViewState["RequestSearchParamters"] == null)
                    return null;
                else
                    return (RequestSearchParamters)ViewState["RequestSearchParamters"];
            }
            set
            {
                ViewState["RequestSearchParamters"] = value;
            }
        }

        public long? RequestId
        {
            get
            {
                if (ViewState["RequestId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["RequestId"].ToString());
            }
            set
            {
                ViewState["RequestId"] = value;
            }
        }

        //TODO: Temp fix, must be removed and call DB
        public List<ProductField> AdditionalFields
        {
            get
            {
                if (ViewState["AdditionalFields"] == null)
                    return null;
                else
                    return (List<ProductField>)ViewState["AdditionalFields"];
            }
            set
            {
                ViewState["AdditionalFields"] = value;
            }
        }



        public String CardViewMode
        {
            get
            {
                if (ViewState["CardViewMode"] == null)
                    return null;
                else
                    return ViewState["CardViewMode"].ToString();
            }
            set
            {
                ViewState["CardViewMode"] = value;
            }
        }



        public ISearchParameters CardSearchParametersParams
        {
            get
            {
                if (ViewState["CardSearchParametersParams"] != null)
                    return (ISearchParameters)ViewState["CardSearchParametersParams"];
                else
                    return null;
            }
            set
            {
                ViewState["CardSearchParametersParams"] = value;
            }
        }



        private RequestConfirm? RequestConfirmCurrent
        {
            get
            {
                if (ViewState["RequestConfirm"] == null)
                    return null;
                else
                    return (RequestConfirm)ViewState["RequestConfirm"];
            }
            set
            {
                ViewState["RequestConfirm"] = value;
            }
        }



        public object ListItems { get; set; }

        #endregion

        #region dlgCardStatus

        private void LoadAdditionalFields()
        {
            dlFields.DataSource = null;
            dlFields.DataBind();

            if (AdditionalFields != null)
            {
                dlFields.DataSource = AdditionalFields;
                dlFields.DataBind();
            }
            else
            {
                log.Warn(w => w("AdditionalFields empty."));
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showAddFields", "showAddFields();", true);
        }

        private void LoadCardStatus()
        {
            dlRequestStatus.DataSource = null;
            dlRequestStatus.DataBind();

            if (RequestId == null)
            {
                this.lblStatusError.Text = "Request Details are not valid.";
                log.Warn(w => w("Null RequestId"));
            }
            else
            {
                var results = _cardService.GetRequestStatusHistory(RequestId.Value).ToList();
                dlRequestStatus.DataSource = results;
                dlRequestStatus.DataBind();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showStatus", "showStatus();", true);
        }
        protected void btnBatchReference_Click(object sender, EventArgs e)
        {
            LoadBatchReference();
        }
        private void LoadBatchReference()
        {
            dlBatchReference.DataSource = null;
            dlBatchReference.DataBind();

            if (RequestId == null)
            {
                this.lblStatusError.Text = "Request Details are not valid.";
                log.Warn(w => w("Null RequestId"));
            }
            else
            {
                var results = _cardService.GetRequestReferceHistory(RequestId.Value).ToList();
                dlBatchReference.DataSource = results;
                dlBatchReference.DataBind();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showReference", "showReference();", true);
        }

        protected void dlRequestStatus_ItemCommand(object source, DataListCommandEventArgs e)
        {

        }

        protected void btnAddFields_Click(object sender, EventArgs e)
        {
            LoadAdditionalFields();
        }

        protected void btnCloseAddFields_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "hideStatus", "hideStatus();", true);
        }

        protected void btnCardStatus_Click(object sender, EventArgs e)
        {
            LoadCardStatus();
        }

        protected void btnCloseStatus_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "hideStatus", "hideStatus();", true);
        }

        #endregion

        #region dlgBatchReference



        protected void btnCloseReference_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "hideReference", "hideReference();", true);
        }


        #endregion

        protected void dlBatchReference_ItemCommand(object source, DataListCommandEventArgs e)
        {
            this.lblInfoMessage.Text = "";

            try
            {
                dlBatchReference.SelectedIndex = e.Item.ItemIndex;
                string distbatchref = ((LinkButton)dlBatchReference.SelectedItem.FindControl("lblBatchRefeferenceField")).Text;
                string distbatchstr = ((Label)dlBatchReference.SelectedItem.FindControl("lblPrintBatchId")).Text;

                if (!string.IsNullOrWhiteSpace(distbatchref))
                {
                    SessionWrapper.DistBatchId = int.Parse(distbatchstr);
                    Server.Transfer("~\\webpages\\hybrid\\printbatchview.aspx");
                }
                else
                {
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.BadSelectionMessage;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}

