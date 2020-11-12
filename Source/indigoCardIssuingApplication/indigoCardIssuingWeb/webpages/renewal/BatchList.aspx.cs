using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.renewal
{
    public partial class BatchList : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RenewalList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR,
                                                                        UserRole.BRANCH_PRODUCT_OPERATOR,
                                                                        UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.CARD_PRODUCTION,
                                                                        UserRole.PIN_PRINTER_OPERATOR,
                                                                        UserRole.AUDITOR};

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private SystemAdminService sysAdminService = new SystemAdminService();
        private bool showcheckbox = false;
        private readonly RenewalService _cardRenewalService = new RenewalService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!String.IsNullOrWhiteSpace(Request.QueryString["BatchStatus"]))
                {
                    BatchStatusType = (RenewalBatchStatusType)int.Parse(Request.QueryString["BatchStatus"]);
                }
                LoadPageData();
            }
        }

        private void LoadPageData()
        {
            DisplayBatchListForUser();
        }

        public int TotalPages
        {
            get
            {
                if (Session["BatchListPages"] != null)
                {
                    return Convert.ToInt32(Session["BatchListPages"]);
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                Session["BatchListPages"] = value;
            }
        }

        public RenewalBatchStatusType BatchStatusType
        {
            get
            {
                if (Session["RenewalBatchStatusType"] != null)
                {
                    return (RenewalBatchStatusType)Convert.ToInt32(Session["RenewalBatchStatusType"]);
                }
                else
                {
                    return RenewalBatchStatusType.Created;
                }
            }
            set
            {
                Session["RenewalBatchStatusType"] = value;
            }
        }

        protected void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = "";
            this.dlCardRenewalList.DataSource = null;

            lblCardRenewalList.Text = Resources.CommonLabels.CardRenewalList;

            if (results == null)
            {
                results = _cardRenewalService.RenewalBatches(BatchStatusType).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlCardRenewalList.DataSource = results;
                TotalPages = results.Length / 10;
            }
            else
            {
                btnConfirm.Visible = false;
                IsConfirmation = null;
                btnReject.Visible = false;
                btnApprove.Visible = false;
                TotalPages = 1;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlCardRenewalList.DataBind();
        }

        private void DisplayBatchListForUser()
        {
            dlCardRenewalList.DataSource = null;

            try
            {
                DisplayResults(null, 1, null);
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

        protected void dlCardRenewalList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            try
            {
                dlCardRenewalList.SelectedIndex = e.Item.ItemIndex;
                int selectedIndex = dlCardRenewalList.SelectedIndex;

                string renewalBatchId = ((Label)this.dlCardRenewalList.SelectedItem.FindControl("lblRenewalBatchId")).Text;

                long selectedRenewalBatchId;
                if (long.TryParse(renewalBatchId, out selectedRenewalBatchId))
                {
                    string redirectURL = string.Format("~\\webpages\\renewal\\BatchApprove.aspx?id={0}", selectedRenewalBatchId);
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

        }

        protected void dlCardRenewalList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            List<long> selectedItems = new List<long>();
            lblErrorMessage.Text = string.Empty;
            foreach (DataListItem item in dlCardRenewalList.Items)
            {
                CheckBox chk = item.FindControl("chksel") as CheckBox;

                string batchRenewalIdStr = ((Label)item.FindControl("lblRenewalBatchId")).Text;

                if (chk.Checked)
                {
                    long batchRenewalId;
                    long.TryParse(batchRenewalIdStr, out batchRenewalId);
                    selectedItems.Add(batchRenewalId);
                }

            }
            if (selectedItems.Count > 0)
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmBatchBulkApprove").ToString();
                btnConfirm.Visible = true;
                btnReject.Visible = false;
                btnApprove.Visible = false;
                IsConfirmation = true;
            }
            else
            {
                lblErrorMessage.Text = "Select at least one renewal batch to approve.";
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            btnConfirm.Visible = false;
            IsConfirmation = null;
            btnReject.Visible = true;
            btnApprove.Visible = true;
        }


        private bool? IsConfirmation
        {
            get
            {
                if (ViewState["IsConfirm"] != null)
                    return (bool)ViewState["IsConfirm"];
                else
                    return null;
            }
            set
            {
                ViewState["IsConfirm"] = value;
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Text = string.Empty;
            List<long> selectedItems = new List<long>();
            foreach (DataListItem item in dlCardRenewalList.Items)
            {
                CheckBox chk = item.FindControl("chksel") as CheckBox;

                string batchRenewalIdStr = ((Label)item.FindControl("lblRenewalBatchId")).Text;

                if (chk.Checked)
                {
                    long batchRenewalId;
                    long.TryParse(batchRenewalIdStr, out batchRenewalId);
                    selectedItems.Add(batchRenewalId);
                }

            }
            if (selectedItems.Count > 0)
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmBatchBulkReject").ToString();
                IsConfirmation = false;
            }
            else
            {
                lblErrorMessage.Text = "Select at least one batch to reject.";
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsConfirmation != null)
                {
                    bool isConfirm = IsConfirmation.Value;

                    List<long> selectedItems = new List<long>();
                    foreach (DataListItem item in dlCardRenewalList.Items)
                    {
                        CheckBox chk = item.FindControl("chksel") as CheckBox;

                        string batchRenewalIdStr = ((Label)item.FindControl("lblRenewalBatchId")).Text;

                        if (chk.Checked)
                        {
                            long batchRenewalId;
                            long.TryParse(batchRenewalIdStr, out batchRenewalId);
                            selectedItems.Add(batchRenewalId);
                        }

                    }
                    if (selectedItems.Count > 0)
                    {
                        string messages = string.Empty;
                        string successMessages = string.Empty;
                        RenewalCommon processor = new RenewalCommon();
                        processor.StatusType = BatchStatusType;
                        int successfulCount = 0;
                        foreach (var renewaBatchlId in selectedItems)
                        {
                            bool isConfirmation = IsConfirmation.GetValueOrDefault();
                            if (processor.ConfirmAction(renewaBatchlId, isConfirmation))
                            {
                                string positiveAction = "approved";
                                if (BatchStatusType == RenewalBatchStatusType.Received)
                                {
                                    positiveAction = "distributed";
                                }

                                successfulCount++;
                                successMessages = $"{successfulCount} batches {(isConfirmation ? positiveAction : "rejected")} successfully.";
                            }
                        }

                        foreach (var item in processor.errorMessages)
                        {
                            lblErrorMessage.Text += item + "<br/>";
                        }

                        lblInfoMessage.Text = successMessages;

                        btnConfirm.Visible = false;
                        IsConfirmation = null;
                        btnReject.Visible = true;
                        btnApprove.Visible = true;

                        DisplayBatchListForUser();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

           
        }


        protected void lnkFirst_Click(object sender, EventArgs e)
        {

        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {

        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {

        }

        protected void lnkLast_Click(object sender, EventArgs e)
        {

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
}