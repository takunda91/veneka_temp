using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.renewal
{
    public partial class VerifyBatch : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RenewalApprove));
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private SystemAdminService sysAdminService = new SystemAdminService();
        private readonly RenewalService _cardRenewalService = new RenewalService();
        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR, UserRole.BRANCH_PRODUCT_OPERATOR };

        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private bool IsConfirmation
        {
            get
            {
                if (ViewState["IsConfirmation"] != null)
                    return (bool)ViewState["IsConfirmation"];
                else
                    return false;
            }
            set
            {
                ViewState["IsConfirmation"] = value;
            }
        }

        private long RenewalId
        {
            get
            {
                if (ViewState["RenewalId"] != null)
                    return (long)ViewState["RenewalId"];
                else
                    return 0;
            }
            set
            {
                ViewState["RenewalId"] = value;
            }
        }


        private RenewalStatusType StatusType
        {
            get
            {
                if (ViewState["StatusType"] != null)
                    return (RenewalStatusType)ViewState["StatusType"];
                else
                    return RenewalStatusType.LoadConfirmed;
            }
            set
            {
                ViewState["StatusType"] = value;
            }
        }

        private void LoadPageData()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(Request.QueryString["id"]))
                {
                    RenewalId = long.Parse(Request.QueryString["id"]);
                }

                RenewalFileViewModel model = _cardRenewalService.Retrieve(RenewalId);
                if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
                {
                    StatusType = model.Status;
                }

                txtFileName.Text = model.FileName;
                txtUploaded.Text = model.DateUploaded.ToShortDateString();
                txtStatus.Text = model.Status.ToString();
                txtCreatedBy.Text = model.CreatedByName;
                txtCreatedDate.Text = model.CreateDate.ToString();
                DisplayRenewalDetails(RenewalId, 1);

            }
            catch (Exception ex)
            {
                this.pnlButtons.Visible = false;

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
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                List<long> selectedItems = new List<long>();

                //find number of checked entries
                foreach (DataListItem item in dlCardRenewalList.Items)
                {
                    CheckBox chk = item.FindControl("chksel") as CheckBox;

                    string lineRenewalDetailId = ((Label)item.FindControl("lblLineRenewalDetailId")).Text;

                    if (chk.Checked)
                    {
                        long renewalDetailId;
                        long.TryParse(lineRenewalDetailId, out renewalDetailId);
                        selectedItems.Add(renewalDetailId);
                    }

                }
                if (selectedItems.Count > 0)
                {
                    this.lblInfoMessage.Text = GetLocalResourceObject("RejectVerifyInfoMessage").ToString();
                    btnVerifyAll.Visible = false;
                    btnReject.Visible = false;
                    btnConfirm.Visible = true;
                    IsConfirmation = false;
                }
                else
                {
                    lblErrorMessage.Text = "Please select at least one card to verify.";
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
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            btnReject.Visible = false;
            btnVerifyAll.Visible = false;
            try
            {
                ConfirmAction(IsConfirmation);
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "The following error occured while processing cards: " + ex.Message;
                btnReject.Visible = true;
                btnVerifyAll.Visible = true;
            }
        }

        protected void btnVerifyAll_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                List<long> selectedItems = new List<long>();

                //find number of checked entries
                foreach (DataListItem item in dlCardRenewalList.Items)
                {
                    CheckBox chk = item.FindControl("chksel") as CheckBox;

                    string lineRenewalDetailId = ((Label)item.FindControl("lblLineRenewalDetailId")).Text;

                    if (chk.Checked)
                    {
                        long renewalDetailId;
                        long.TryParse(lineRenewalDetailId, out renewalDetailId);
                        selectedItems.Add(renewalDetailId);
                    }

                }
                if (selectedItems.Count > 0)
                {
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmVerifyInfoMessage").ToString();
                    btnVerifyAll.Visible = false;
                    btnReject.Visible = false;
                    btnConfirm.Visible = true;
                    IsConfirmation = true;
                }
                else
                {
                    lblErrorMessage.Text = "Please select at least one card to verify.";
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

        private void ConfirmAction(bool isConfirm)
        {
            List<long> selectedItems = new List<long>();

            //find number of checked entries
            foreach (DataListItem item in dlCardRenewalList.Items)
            {
                CheckBox chk = item.FindControl("chksel") as CheckBox;

                string lineRenewalDetailId = ((Label)item.FindControl("lblLineRenewalDetailId")).Text;

                if (chk.Checked)
                {
                    long renewalDetailId;
                    long.TryParse(lineRenewalDetailId, out renewalDetailId);
                    selectedItems.Add(renewalDetailId);
                }

            }
            bool allValid = true;

            if (selectedItems.Count > 0)
            {
                foreach (var renewalDetailId in selectedItems)
                {
                    allValid = allValid && ProcessIndividualEntry(renewalDetailId, isConfirm);
                }

                string messages = string.Empty;
                DisplayRenewalDetails(RenewalId, 1);
                if (allValid)
                {
                    messages = $"All selected entries were successfully {(isConfirm ? "approved" : "rejected")}.";
                    lblInfoMessage.Text = messages;
                }
                else
                {
                    messages = $"Some or all selected entries could not be {(isConfirm ? "approved" : "rejected")}.";
                    lblErrorMessage.Text = messages;
                }
                
            }
            else
            {
                lblErrorMessage.Text = "Please select at least one card to verify.";
            }
        }

        private bool ProcessIndividualEntry(long renewalDetailId, bool isConfirm)
        {
            var result = _cardRenewalService.GetRenewalDetail(renewalDetailId);
            bool successful = false;
            if (result != null)
            {
                int issuingScenario = 0;
                AccountDetails accountDetails;
                string messages;
                bool checkAccountDetails = _customerCardIssuerService.GetAccountDetail(result.IssuerId,
                           result.ProductId,
                           issuingScenario,
                           result.BranchId, result.ExternalAccountNumber, out accountDetails, out messages);
                if (checkAccountDetails)
                {
                    if (result.DeliveryBranchId == 0)
                    {
                        result.DeliveryBranchId = result.BranchId;
                    }

                    if (isConfirm)
                    {
                        var resultConfirm = _cardRenewalService.ApproveRenewalDetail(renewalDetailId, result.DeliveryBranchId, accountDetails.CurrencyId, accountDetails.CBSAccountTypeId, accountDetails.CMSAccountTypeId);
                        if (resultConfirm != null && resultConfirm.RenewalStatus == RenewalDetailStatusType.Approved)
                        {
                            successful = true;
                        }
                    }
                    else
                    {
                        var resultReject = _cardRenewalService.RejectRenewalDetail(renewalDetailId, result.DeliveryBranchId, string.Empty);
                        if (resultReject != null && resultReject.RenewalStatus == RenewalDetailStatusType.Rejected)
                        {
                            successful = true;
                        }
                    }
                }
            }
            return successful;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Server.Transfer("\\webpages\\renewal\\VerifyList.aspx?");
        }

        protected void dlCardRenewalList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            try
            {
                dlCardRenewalList.SelectedIndex = e.Item.ItemIndex;
                int selectedIndex = dlCardRenewalList.SelectedIndex;

                string CardRenewalDetailIdStr = ((Label)this.dlCardRenewalList.SelectedItem.FindControl("lblLineRenewalDetailId")).Text;

                long selectedCardRenewalDetailId;
                if (long.TryParse(CardRenewalDetailIdStr, out selectedCardRenewalDetailId))
                {
                    string redirectURL = string.Format("~\\webpages\\renewal\\VerifySingle.aspx?id={0}&status={1}", selectedCardRenewalDetailId, StatusType);
                    Response.Redirect(redirectURL);
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

        protected void dlCardRenewalList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RenewalDetailListModel item = (RenewalDetailListModel)e.Item.DataItem;
                if (item.RenewalStatus == RenewalDetailStatusType.Batched ||
                    item.RenewalStatus == RenewalDetailStatusType.Rejected ||
                    item.RenewalStatus == RenewalDetailStatusType.Approved)
                {
                    e.Item.FindControl("chksel").Visible = false;
                    LinkButton linkButton = (LinkButton)e.Item.FindControl("lblLineCardNumber");
                    linkButton.Attributes.Remove("href");
                    linkButton.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "default";
                    linkButton.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                    if (linkButton.Enabled != false)
                    {
                        linkButton.Enabled = false;
                    }

                    if (linkButton.OnClientClick != null)
                    {
                        linkButton.OnClientClick = null;
                    }
                }
            }
        }

        protected void dlCardRenewalList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DisplayRenewalDetails(long renewalId, int pageIndex)
        {
            this.lblErrorMessage.Text = "";
            this.dlCardRenewalList.DataSource = null;

            btnConfirm.Visible = false;
            RenewalDetailStatusType expectedStatus = GetDetailStatus();
            List<RenewalDetailListModel> results = _cardRenewalService.ListRenewalDetails(renewalId).Where(p => p.RenewalStatus == expectedStatus).ToList() ;
            int totalPages = 0;
            if (results.Count > 0)
            {
                this.dlCardRenewalList.DataSource = results;
                totalPages = results.Count / 10;
                btnVerifyAll.Visible = true;
                btnReject.Visible = true;
                btnConfirm.Visible = false;
            }
            else
            {
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                btnVerifyAll.Visible = false;
                btnReject.Visible = false;
                btnConfirm.Visible = false;
                Server.Transfer("\\webpages\\renewal\\VerifyList.aspx?");
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, totalPages);
            this.dlCardRenewalList.DataBind();
        }

        private RenewalDetailStatusType GetDetailStatus()
        {
            RenewalDetailStatusType expectedStatus = RenewalDetailStatusType.Loaded;
            switch (StatusType)
            {
                case RenewalStatusType.Loaded:
                    expectedStatus= RenewalDetailStatusType.Loaded;
                    break;
                case RenewalStatusType.LoadConfirmed:
                    expectedStatus= RenewalDetailStatusType.Loaded;
                    break;
                case RenewalStatusType.Approved:
                    expectedStatus= RenewalDetailStatusType.Loaded;
                    break;
                case RenewalStatusType.Verified:
                    expectedStatus = RenewalDetailStatusType.Loaded;
                    break;
                case RenewalStatusType.BatchCreated:
                    break;
                case RenewalStatusType.Rejected:
                    break;
                default:
                    break;
            }
            return expectedStatus;
        }

        #region ResultsNavigation
        private void ChangePage(ResultNavigation resultNavigation)
        {

            ////Clear error messages
            //var errorLabel = FindControl("lblErrorMessage");
            //if(errorLabel != null && errorLabel is Label)
            //    ((Label)errorLabel).Text = String.Empty;

            switch (resultNavigation)
            {
                case ResultNavigation.FIRST:
                    PageIndex = 1;
                    break;
                case ResultNavigation.NEXT:
                    if (PageIndex < TotalPages)
                    {
                        PageIndex = PageIndex + 1;
                    }
                    break;
                case ResultNavigation.PREVIOUS:
                    if (PageIndex > 1)
                    {
                        PageIndex = PageIndex - 1;
                    }
                    break;
                case ResultNavigation.LAST:
                    PageIndex = TotalPages.GetValueOrDefault();
                    break;
                default:
                    break;
            }

            DisplayRenewalDetails(RenewalId, PageIndex);
        }



        protected void lnkFirst_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.FIRST);
        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.PREVIOUS);
        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.NEXT);
        }

        protected void lnkLast_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.LAST);
        }

        public int PageIndex
        {
            get
            {
                if (ViewState["PageIndex"] == null)
                    return 1;
                else
                    return Convert.ToInt32(ViewState["PageIndex"].ToString());
            }
            set
            {
                ViewState["PageIndex"] = value;
            }
        }

        public int? TotalPages
        {
            get
            {
                if (ViewState["TotalPages"] == null)
                    return 1;
                else
                    return Convert.ToInt32(ViewState["TotalPages"].ToString());
            }
            set
            {
                ViewState["TotalPages"] = value;
            }
        }
        #endregion
    }
}
