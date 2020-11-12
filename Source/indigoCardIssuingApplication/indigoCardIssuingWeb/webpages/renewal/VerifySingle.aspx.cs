using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.renewal
{
    public partial class VerifySingle : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(VerifySingle));

        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly RenewalService _cardRenewalService = new RenewalService();


        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR, UserRole.BRANCH_PRODUCT_OPERATOR };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!String.IsNullOrWhiteSpace(Request.QueryString["id"]))
                {
                    RenewalDetailId = long.Parse(Request.QueryString["id"]);
                }
                //load the details of the screen
                LoadScreenDetails();
            }
        }

        private long RenewalDetailId
        {
            get
            {
                if (ViewState["RenewalDetailId"] != null)
                    return (long)ViewState["RenewalDetailId"];
                else
                    return 0;
            }
            set
            {
                ViewState["RenewalDetailId"] = value;
            }
        }

        private RenewalDetailListModel OriginalDetails
        {
            get
            {
                if (ViewState["RenewalDetail"] != null)
                    return (RenewalDetailListModel)ViewState["RenewalDetail"];
                else
                    return null;
            }
            set
            {
                ViewState["RenewalDetail"] = value;
            }
        }

        private void LoadScreenDetails()
        {
            var result = _cardRenewalService.GetRenewalDetail(RenewalDetailId);

            //load branches
            UpdateBranchList(result.IssuerId, true);
            ddlBranch.SelectedValue = result.DeliveryBranchId.ToString();
            txtBranch.Text = $"{result.BranchCode} - {result.BranchName}";
            txtCardNumber.Text = result.CardNumber;
            txtClientID.Text = result.PassportIDNumber;
            txtCurrencyCode.Text = result.CurrencyCode;
            txtCustomerName.Text = result.CustomerName;
            txtEmailAddress.Text = result.EmailAddress;
            txtEmbossingName.Text = result.EmbossingName;
            txtExpiryDate.Text = result.ExpiryDate.GetValueOrDefault().ToString("dd/MM/yyyy");
            txtExternalAccountNumber.Text = result.ExternalAccountNumber;
            txtInternalAccountNumber.Text = result.InternalAccountNumber;
            txtLimitBalance.Text = result.LimitBalance.GetValueOrDefault().ToString("#,##0.00");
            txtMobileNumber.Text = result.MobilePhone;
            txtProduct.Text = result.ProductName;
            txtRenewalDate.Text = result.RenewalDate.GetValueOrDefault().ToString("dd/MM/yyyy");
            txtRenewalStatus.Text = result.RenewalStatus.ToString();
            OriginalDetails = result;
        }

        /// Populates the branch drop down list.
        /// </summary>
        /// <param name="issuerId"></param>
        private void UpdateBranchList(int issuerId, bool useCache)
        {
            if (issuerId >= 0)
            {
                this.ddlBranch.Items.Clear();
                Dictionary<int, ListItem> branchList = new Dictionary<int, ListItem>();

                var branches = _userMan.GetBranchesForUser(issuerId, userRolesForPage[0], null);

                foreach (var item in branches)
                {
                    branchList.Add(item.branch_id, utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
                }


                if (branchList.Count > 0)
                {
                    ddlBranch.Items.AddRange(branchList.Values.OrderBy(m => m.Text).ToArray());
                    ddlBranch.SelectedIndex = 0;

                    this.lblBranch.Visible = true;
                    this.ddlBranch.Visible = true;
                }
                else
                {

                    lblErrorMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "EmptyBrancheslistMessage").ToString();
                }
            }
        }



        protected void btnValidateAccount_Click(object sender, EventArgs e)
        {
            //validate the account here
            pnlButtons.Visible = true;

            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                int issuerId = OriginalDetails.IssuerId; ;
                int branchId = OriginalDetails.BranchId;
                int productId = OriginalDetails.ProductId;

                string messages;
                AccountDetails accountDetails;
                int issuingScenario = 0;
                bool checkAccountDetails = _customerCardIssuerService.GetAccountDetail(issuerId,
                            productId,
                            issuingScenario,
                            branchId, OriginalDetails.ExternalAccountNumber, out accountDetails, out messages);

                if (checkAccountDetails)
                {
                    currencyId.Value = accountDetails.CurrencyId.ToString();
                    cbsAccountTypeId.Value = accountDetails.CBSAccountTypeId;
                    cmsAccountTypeId.Value = accountDetails.CMSAccountTypeId;
                    this.lblInfoMessage.Text = "Account valid, Confirm or Reject renewal.";
                    pnlButtons.Visible = true;
                }
                else
                {
                    btnConfirm.Visible = false;
                    this.lblErrorMessage.Text = messages;
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

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            btnValidateAccount.Visible = false;
            btnConfirm.Visible = false;
            btnReject.Visible = false;
            bool retry = true;
            try
            {
                int deliveryBranchId = int.Parse(this.ddlBranch.SelectedValue);
                int verifiedCurrencyId = 0;
                int.TryParse(currencyId.Value, out verifiedCurrencyId);

                var result = _cardRenewalService.ApproveRenewalDetail(OriginalDetails.RenewalDetailId, deliveryBranchId, verifiedCurrencyId, cbsAccountTypeId.Value, cmsAccountTypeId.Value);
                LoadScreenDetails();
                if (result.RenewalStatus == RenewalDetailStatusType.Approved)
                {
                    this.lblInfoMessage.Text = "Card renewal approved.";
                    retry = false;
                }
                else
                {
                    this.lblErrorMessage.Text = "Failed to approve the card renewal.";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
            }
            btnConfirm.Visible = retry;
            btnReject.Visible = retry;
            btnValidateAccount.Visible = retry;
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            btnValidateAccount.Visible = false;
            btnConfirm.Visible = false;
            btnReject.Visible = false;
            lblErrorMessage.Text = string.Empty;
            lblInfoMessage.Text = string.Empty;
            bool retry = true;
            try
            {
                int deliveryBranchId = int.Parse(this.ddlBranch.SelectedValue);
                if (!string.IsNullOrWhiteSpace(txtComment.Text))
                {
                    var result = _cardRenewalService.RejectRenewalDetail(OriginalDetails.RenewalDetailId, deliveryBranchId, txtComment.Text.Trim());
                    LoadScreenDetails();
                    if (result.RenewalStatus == RenewalDetailStatusType.Rejected)
                    {
                        this.lblInfoMessage.Text = "Card renewal rejected.";
                        retry = false;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = "Failed to reject the card renewal.";
                    }
                }
                else
                {
                    this.lblErrorMessage.Text = "Comment is required when rejecting a card.";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
            }
            btnConfirm.Visible = retry;
            btnReject.Visible = retry;
            btnValidateAccount.Visible = retry;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~\\webpages\\renewal\\VerifyBatch.aspx?id={OriginalDetails.RenewalId}");
        }
    }
}