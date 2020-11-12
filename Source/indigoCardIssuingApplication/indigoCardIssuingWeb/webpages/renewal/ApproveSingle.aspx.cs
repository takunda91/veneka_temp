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
    public partial class ApproveSingle : System.Web.UI.Page
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
                if (!String.IsNullOrWhiteSpace(Request.QueryString["batchid"]))
                {
                    BatchId = long.Parse(Request.QueryString["batchid"]);
                }
                if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
                {
                    StatusType = (RenewalBatchStatusType)Enum.Parse(typeof(RenewalBatchStatusType), Request.QueryString["status"]);
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

        private long BatchId
        {
            get
            {
                if (ViewState["BatchId"] != null)
                    return (long)ViewState["BatchId"];
                else
                    return 0;
            }
            set
            {
                ViewState["BatchId"] = value;
            }
        }

        private RenewalBatchStatusType StatusType
        {
            get
            {
                if (ViewState["StatusType"] != null)
                    return (RenewalBatchStatusType)ViewState["StatusType"];
                else
                    return RenewalBatchStatusType.Created;
            }
            set
            {
                ViewState["StatusType"] = value;
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
            btnReject.Visible = (result.RenewalStatus == RenewalDetailStatusType.Batched);
        }

        /// Populates the branch drop down list.
        /// </summary>
        /// <param name="issuerId"></param>
        private void UpdateBranchList(int issuerId, bool useCache)
        {
            ddlBranch.Enabled = false;
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

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            btnConfirm.Visible = false;
            btnReject.Visible = false;
            bool retry = true;
            try
            {
                int deliveryBranchId = int.Parse(this.ddlBranch.SelectedValue);
                int verifiedCurrencyId = 0;
                int.TryParse(currencyId.Value, out verifiedCurrencyId);
                if (!string.IsNullOrWhiteSpace(txtComment.Text))
                {
                    var result = _cardRenewalService.RejectRenewalCardReceived(OriginalDetails.RenewalDetailId, deliveryBranchId, txtComment.Text.Trim());
                    LoadScreenDetails();
                    if (result.RenewalStatus == RenewalDetailStatusType.CardProblem)
                    {
                        txtComment.Enabled = false;
                        this.lblInfoMessage.Text = "Card renewal revoked - card problem.";
                        retry = false;
                    }
                    else
                    {
                        this.lblInfoMessage.Text = string.Empty;
                        this.lblErrorMessage.Text = "Failed to approve the card renewal.";
                    }
                }
                else
                {
                    this.lblInfoMessage.Text = string.Empty;
                    this.lblErrorMessage.Text = "Comment is required when rejecting a card.";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
            }
            btnReject.Visible = retry;
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            btnConfirm.Visible = true;
            btnReject.Visible = false;
            lblErrorMessage.Text = string.Empty;
            lblInfoMessage.Text = string.Empty;
            txtComment.Enabled = false;
            this.btnCancel.Enabled = this.btnCancel.Visible = true;
            this.lblInfoMessage.Text = GetLocalResourceObject("RejectRenewalInfoMessage").ToString();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RenewalBatch model = _cardRenewalService.GetRenewalBatch(BatchId);
            if (model.RenewalBatchStatus == RenewalBatchStatusType.Rejected)
            {
                Response.Redirect($"~\\webpages\\renewal\\BatchList.aspx?batchStatus=0");
            }
            else
            {
                Response.Redirect($"~\\webpages\\renewal\\BatchApprove.aspx?id={BatchId}");
            }
        }
    }
}