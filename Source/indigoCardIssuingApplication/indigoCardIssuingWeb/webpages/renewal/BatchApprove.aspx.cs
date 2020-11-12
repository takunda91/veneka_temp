using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.renewal
{
    public partial class BatchApprove : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RenewalApprove));
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private SystemAdminService sysAdminService = new SystemAdminService();
        private readonly RenewalService _cardRenewalService = new RenewalService();
        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();

        bool reroute;

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

        private long RenewalBatchId
        {
            get
            {
                if (ViewState["RenewalBatchId"] != null)
                    return (long)ViewState["RenewalBatchId"];
                else
                    return 0;
            }
            set
            {
                ViewState["RenewalBatchId"] = value;
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
                lblVerifyBatchHeader.Text = GetStatusHeader(value);
            }
        }

        private string GetStatusHeader(RenewalBatchStatusType value)
        {
            switch (value)
            {
                case RenewalBatchStatusType.Created:
                    return "Approve Renewal Batch";
                case RenewalBatchStatusType.Exported:
                    return "Receive Renewal Batch";
                case RenewalBatchStatusType.Received:
                    return "Approve Renewal Batch";
                case RenewalBatchStatusType.Approved:
                    return "Distribute Renewal Batch";
                case RenewalBatchStatusType.Rejected:
                    return "Reject Renewal Batch";
                case RenewalBatchStatusType.Distribution:
                    return "Distribute Renewal Batch";
                default:
                    return "Renewal Batch";
            }
        }

        private void LoadPageData()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(Request.QueryString["id"]))
                {
                    RenewalBatchId = long.Parse(Request.QueryString["id"]);
                }

                if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
                {
                    StatusType = (RenewalBatchStatusType)Enum.Parse(typeof(RenewalBatchStatusType), Request.QueryString["status"]);
                }

                LoadBatchDetails();

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

        private void LoadBatchDetails()
        {
            RenewalBatch model = _cardRenewalService.GetRenewalBatch(RenewalBatchId);
            txtBatchNumber.Text = model.CalculatedBatchNumber;
            txtCreated.Text = model.RenewalDate.ToShortDateString();
            txtStatus.Text = model.RenewalBatchStatus.ToString();
            txtBranchCount.Text = model.BranchCount.ToString();
            txtProductCount.Text = model.ProductName.ToString();
            txtCardCount.Text = model.CardCount.ToString();
            StatusType = model.RenewalBatchStatus;
            DisplayRenewalDetails(RenewalBatchId, 1);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                btnConfirm.Visible = true;
                btnReject.Visible = false;
                btnApprove.Visible = false;
                IsConfirmation = false;
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
            bool retry = true;
            btnReject.Visible = false;
            btnApprove.Visible = false;
            btnConfirm.Visible = false;
            try
            {
                RenewalCommon processor = new RenewalCommon();
                processor.StatusType = StatusType;
                if (processor.ConfirmAction(RenewalBatchId, IsConfirmation))
                {
                    retry = false;
                    string positiveAction = "approved";
                    if (StatusType == RenewalBatchStatusType.Received)
                    {
                        positiveAction = "distributed";
                    }
                    LoadBatchDetails();
                    lblInfoMessage.Text = $"Batch {(IsConfirmation ? positiveAction : "rejected")} successfully.";
                    btnPrint.Visible = true;
                    if (reroute)
                    {
                        Server.Transfer("\\webpages\\renewal\\BatchList.aspx?batchstatus" + ((int)StatusType).ToString());
                    }
                }
                else
                {
                    foreach (var item in processor.errorMessages)
                    {
                        lblErrorMessage.Text += item + "<br/>";
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "The following error occured while approving the batch: " + ex.Message;
            }
            btnReject.Visible = retry;
            btnApprove.Visible = retry;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Server.Transfer("\\webpages\\renewal\\BatchList.aspx?batchstatus" + ((int)RenewalBatchStatusType.Created).ToString());
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
                    string redirectURL = string.Format("~\\webpages\\renewal\\ApproveSingle.aspx?id={0}&status={1}&batchId={2}", selectedCardRenewalDetailId, StatusType, RenewalBatchId);
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
                bool disableLinks = (item.RenewalStatus == RenewalDetailStatusType.CardProblem || item.RenewalStatus == RenewalDetailStatusType.Distributed) ||
                    StatusType == RenewalBatchStatusType.Received || StatusType == RenewalBatchStatusType.Approved;

                if (disableLinks)
                {
                    DisableLinkButton((LinkButton)e.Item.FindControl("lblLineCardNumber"));
                    DisableLinkButton((LinkButton)e.Item.FindControl("lblLineCustomerName"));
                    DisableLinkButton((LinkButton)e.Item.FindControl("lblLineEmbossingName"));
                    DisableLinkButton((LinkButton)e.Item.FindControl("lblLineExpiryDate"));
                }
            }
        }

        private void DisableLinkButton(LinkButton linkButton)
        {
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

        protected void dlCardRenewalList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private RenewalDetailStatusType GetDetailStatus()
        {
            RenewalDetailStatusType expectedStatus = RenewalDetailStatusType.Batched;
            switch (StatusType)
            {
                case RenewalBatchStatusType.Created:
                    expectedStatus = RenewalDetailStatusType.Batched;
                    break;
                case RenewalBatchStatusType.Exported:
                    expectedStatus = RenewalDetailStatusType.Batched;
                    break;
                case RenewalBatchStatusType.Received:
                    expectedStatus = RenewalDetailStatusType.Batched;
                    break;
                case RenewalBatchStatusType.Distribution:
                    expectedStatus = RenewalDetailStatusType.Distributed;
                    break;
                default:
                    break;
            }
            return expectedStatus;
        }

        private void DisplayRenewalDetails(long renewalBatchId, int pageIndex)
        {
            this.lblErrorMessage.Text = "";
            this.dlCardRenewalList.DataSource = null;

            lblCardRenewalList.Text = Resources.CommonLabels.CardRenewalList;
            btnConfirm.Visible = false;
            RenewalDetailStatusType expectedStatus = GetDetailStatus();
            List<RenewalDetailListModel> results = _cardRenewalService.GetRenewalBatchDetails(renewalBatchId, true)
                                                    .Where(p => p.RenewalStatus == expectedStatus).ToList();
            int totalPages = 0;
            if (results.Count > 0)
            {
                this.dlCardRenewalList.DataSource = results;
                totalPages = results.Count / 10;
            }
            else
            {
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                btnApprove.Visible = false;
                btnReject.Visible = false;
                btnConfirm.Visible = false;
                reroute = true;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, totalPages);
            this.dlCardRenewalList.DataBind();
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

            DisplayRenewalDetails(RenewalBatchId, PageIndex);
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

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                btnConfirm.Visible = true;
                btnReject.Visible = false;
                btnApprove.Visible = false;
                IsConfirmation = true;
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

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {

                var reportBytes = _cardRenewalService.GenerateRenewalBatchReport(RenewalBatchId);
                string reportName = String.Empty;
                reportName = $"Renewal_Batch_Report_{RenewalBatchId}_{StatusType.ToString()}_{DateTime.Now.ToString("ddd_dd_MMMM_yyyy") }.pdf";
                Response.Clear();
                Response.ClearHeaders();
                MemoryStream ms = new MemoryStream(reportBytes);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.Flush();
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

        //private bool CreateCards(long renewalBatchId, out List<string> processingErrors)
        //{
        //    bool successful = true;
        //    processingErrors = new List<string>();
        //    var details = _cardRenewalService.GetRenewalBatchDetails(renewalBatchId, false);
        //    List<Tuple<RenewalDetailListModel, CustomerDetails>> newCards = new List<Tuple<RenewalDetailListModel, CustomerDetails>>();
        //    foreach (var item in details)
        //    {
        //        if (item.RenewalStatus == RenewalDetailStatusType.Batched)
        //        {
        //            var packedEntry = CreateCustomerDetails(item);
        //            if (packedEntry != null)
        //            {
        //                newCards.Add(Tuple.Create(item, packedEntry));
        //            }
        //        }
        //    }
        //    if (newCards.Count == 0)
        //    {
        //        processingErrors.Add("No cards found");
        //        return false;
        //    }
        //    else
        //    {
        //        foreach (var item in newCards)
        //        {
        //            long cardId = 0;
        //            string resultMessage = string.Empty;
        //            successful = successful && _customerCardIssuerService.RequestCardForCustomer(item.Item2, item.Item1.RenewalDetailId, out cardId, out resultMessage);
        //            if (cardId != 0)
        //            {
        //                //approve the card
        //                _customerCardIssuerService.MakerChecker(cardId, true, string.Empty, out resultMessage);
        //            }
        //            else
        //            {
        //                processingErrors.Add(resultMessage);
        //            }
        //        }
        //    }
        //    return successful;
        //}

        //private CustomerDetails CreateCustomerDetails(RenewalDetailListModel item)
        //{
        //    try
        //    {
        //        var product = _batchService.GetProduct(item.ProductId);

        //        var customer = new CustomerDetails();

        //        customer.CustomerTitleId = 7;
        //        string[] names = item.CustomerName.Split(' ');

        //        customer.FirstName = names[0].Trim();
        //        customer.MiddleName = names[1].Trim();
        //        if (names.Length > 2)
        //        {
        //            customer.LastName = names[2].Trim();
        //        }
        //        else
        //        {
        //            customer.MiddleName = null;
        //            customer.LastName = names[1].Trim();
        //        }

        //        customer.CmsID = item.ClientId;
        //        customer.CustomerId = item.InternalAccountNumber;
        //        customer.ContractNumber = item.ContractNumber;
        //        customer.AccountNumber = item.ExternalAccountNumber;
        //        customer.DomicileBranchId = item.BranchId;
        //        customer.CardReference = item.CardNumber;
        //        customer.CardNumber = item.CardNumber;

        //        customer.DeliveryBranchId = item.DeliveryBranchId;
        //        customer.CardIssueMethodId = 0;

        //        customer.PriorityId = 1;

        //        customer.CardIssueReasonId = 2;
        //        customer.AccountTypeId = product.AccountTypes.FirstOrDefault();
        //        customer.CustomerTypeId = 0;
        //        customer.CurrencyId = item.CurrencyId;
        //        customer.CustomerResidencyId = 0;

        //        customer.IssuerId = product.Product.issuer_id;
        //        customer.BranchId = item.BranchId;
        //        customer.ProductId = item.ProductId;
        //        customer.CBSAccountType = item.CBSAccountType;
        //        customer.CMSAccountType = item.CMSAccountType;
        //        customer.NameOnCard = item.EmbossingName;
        //        customer.CustomerIDNumber = item.PassportIDNumber;
        //        customer.ContactNumber = item.ContractNumber;
        //        customer.FeeOverridenYN = false;
        //        customer.FeeWaiverYN = product.Product.renewal_charge_card;
        //        customer.FeeEditbleYN = false;
        //        customer.FeeCharge = null;
        //        customer.Vat = null;
        //        customer.TotalCharged = null;

        //        var feeDetails = _batchService.GetFeeDetailByProduct(item.ProductId).FirstOrDefault();
        //        if (feeDetails != null)
        //        {
        //            customer.FeeDetailId = feeDetails.fee_detail_id;

        //            var response = _batchService.GetCurrentFees(feeDetails.fee_detail_id,
        //                                                       customer.CurrencyId.GetValueOrDefault(),
        //                                                       customer.CardIssueReasonId.GetValueOrDefault(),
        //                                                       customer.CBSAccountType);
        //            if (response != null)
        //            {
        //                customer.FeeCharge = response.fee_charge;
        //                customer.Vat = response.vat;
        //                customer.VatCharged = customer.FeeCharge * (customer.Vat.Value / 100);
        //                customer.TotalCharged = customer.FeeCharge + customer.VatCharged;
        //            }
        //        }

        //        customer.ProductFields = new ProductField[0];

        //        ProductField[] list = new ProductField[0];
        //        list = _issuerMan.GetPrintFieldsByProductid(item.ProductId);

        //        customer.ProductFields = list.ToArray();

        //        foreach (var field in customer.ProductFields)
        //        {
        //            if (field.MappedName.ToUpper() == StaticFields.IND_SYS_NOC) //System default of name on card
        //                field.Value = System.Text.Encoding.UTF8.GetBytes(customer.NameOnCard.ToUpper());
        //            else if (field.MappedName.ToUpper() == StaticFields.IND_SYS_PAN) //System default of PAN
        //                field.Value = System.Text.Encoding.UTF8.GetBytes(System.Text.RegularExpressions.Regex.Replace(customer.CardNumber.Replace("-", ""), ".{4}", "$0 "));
        //            else if (field.MappedName.ToUpper() == StaticFields.ING_NOC) //System default of name on card
        //                field.Value = System.Text.Encoding.UTF8.GetBytes(string.Empty);
        //            else if (field.MappedName.ToUpper() == "IND_SYS_ADDRESS")
        //                field.Value = System.Text.Encoding.UTF8.GetBytes(string.Empty);
        //            else if (field.MappedName.ToUpper() == "CASHLIMIT")
        //                field.Value = System.Text.Encoding.UTF8.GetBytes(item.LimitBalance.GetValueOrDefault().ToString());
        //            else if (field.MappedName.ToUpper() == "IND_SYS_DOB")
        //                field.Value = System.Text.Encoding.UTF8.GetBytes(item.BirthDate.GetValueOrDefault().ToString("dd-MM-yyyy"));
        //            else if (field.MappedName.ToUpper() == "IND_SYS_POSTCODE")
        //                field.Value = System.Text.Encoding.UTF8.GetBytes(string.Empty);
        //        }

        //        return customer;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        //private void CreateProductionBatches(long renewalBatchId)
        //{
        //    var details = _cardRenewalService.CreateRenewalDistributionBatches(renewalBatchId);
        //    //approve  for production
        //    foreach (var distBatchId in details)
        //    {
        //        DistBatchResult distBatch = _batchService.GetDistBatch(distBatchId);
        //        string responseMessage;
        //        int fromStatusId = 0;
        //        int toStatusId = 9;
        //        _batchService.DistBatchChangeStatusRenewal(distBatchId, fromStatusId, toStatusId, string.Empty, false, out distBatch, out responseMessage);

        //        //upload to cms
        //        fromStatusId = 9;
        //        toStatusId = 10;
        //        distBatch = _batchService.GetDistBatch(distBatchId);
        //        _batchService.DistBatchChangeStatusRenewal(distBatchId, fromStatusId, toStatusId, string.Empty, false, out distBatch, out responseMessage);

        //        //upload successful
        //        fromStatusId = 10;
        //        toStatusId = 11;
        //        distBatch = _batchService.GetDistBatch(distBatchId);
        //        _batchService.DistBatchChangeStatusRenewal(distBatchId, fromStatusId, toStatusId, string.Empty, false, out distBatch, out responseMessage);

        //        //receive from production
        //        distBatch = _batchService.GetDistBatch(distBatchId);
        //        fromStatusId = 11;
        //        toStatusId = 20;
        //        _batchService.DistBatchChangeStatusRenewal(distBatchId, fromStatusId, toStatusId, string.Empty, false, out distBatch, out responseMessage);

        //        distBatch = _batchService.GetDistBatch(distBatchId);
        //        fromStatusId = 20;
        //        toStatusId = 14;
        //        _batchService.DistBatchChangeStatusRenewal(distBatchId, fromStatusId, toStatusId, string.Empty, false, out distBatch, out responseMessage);

        //        distBatch = _batchService.GetDistBatch(distBatchId);
        //        fromStatusId = 14;
        //        toStatusId = 1;
        //        _batchService.DistBatchChangeStatusRenewal(distBatchId, fromStatusId, toStatusId, string.Empty, false, out distBatch, out responseMessage);
        //    }

        //}

    }
    #endregion
}