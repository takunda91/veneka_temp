using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using System.Threading;
using System.Globalization;
using System.Web.Security;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.product
{
    public partial class ProductFeeAccounting : BasePage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";

        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly BatchManagementService _batchservice = new BatchManagementService();


        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ISSUER_ADMIN };
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductFeeAccounting));

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (CurrentPageLayout != null)
            {
                pageLayout = (PageLayout)CurrentPageLayout;
            }

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        #region Private Methods
        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                List<Issuer_product_font> list = _batchservice.GetFontFamilyList();

                if (issuersList.ContainsKey(-1))
                    issuersList.Remove(-1);

                ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                ddlIssuer.SelectedIndex = 0;

                if (ddlIssuer.Items.Count == 0)
                {
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                }

                if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    pnlButtons.Visible = false;
                    lblInfoMessage.Text = "";
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SaveActionMessage").ToString();
                }
                else
                {
                    pnlButtons.Visible = true;

                    List<ListItem> revAccountTypes = new List<ListItem>();
                    foreach (var item in _customerCardIssuerService.LangLookupCustomerAccountTypes())
                    {
                        revAccountTypes.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                    }
                    this.ddlRevenueAccountType.Items.AddRange(revAccountTypes.OrderBy(m => m.Text).ToArray());

                    List<ListItem> vatAccountTypes = new List<ListItem>();
                    foreach (var item in _customerCardIssuerService.LangLookupCustomerAccountTypes())
                    {
                        vatAccountTypes.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                    }
                    this.ddlVatAccountType.Items.AddRange(vatAccountTypes.OrderBy(m => m.Text).ToArray());


                    FeeAccountingId = SessionWrapper.ProductFeeAccountingId;
                    SessionWrapper.ProductFeeAccountingId = null;

                    if (FeeAccountingId == null)
                        pageLayout = PageLayout.CREATE;
                    else
                    {
                        var result = _batchservice.GetFeeAccounting(FeeAccountingId.Value);

                        this.ddlIssuer.SelectedValue = result.issuer_id.ToString();

                        //Todo get account types


                        this.tbAccountingName.Text = result.fee_accounting_name;
                        this.tbRevenueAccountNo.Text = result.fee_revenue_account_no;
                        this.ddlRevenueAccountType.SelectedValue = result.fee_revenue_account_type_id.ToString();
                        this.tbRevenueBranchCode.Text = result.fee_revenue_branch_code;
                        this.tbRevenueNarrationEn.Text = result.fee_revenue_narration_en;
                        this.tbRevenueNarrationFr.Text = result.fee_revenue_narration_fr;
                        this.tbRevenueNarrationPt.Text = result.fee_revenue_narration_pt;
                        this.tbRevenueNarrationEs.Text = result.fee_revenue_narration_es;

                        this.tbVatAccountNo.Text = result.vat_account_no;
                        this.tbVatBranchCode.Text = result.vat_account_branch_code;
                        this.ddlVatAccountType.SelectedValue = result.vat_account_type_id.ToString();
                        this.tbVatNarrationEn.Text = result.vat_narration_en;
                        this.tbVatNarrationFr.Text = result.vat_narration_fr;
                        this.tbVatNarrationPt.Text = result.vat_narration_pt;
                        this.tbVatNarrationEs.Text = result.vat_narration_es;
                    }
                }

                UpdateFormLayout(pageLayout);
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private ProductFeeAccountingResult GetValuesFromControls()
        {
            ProductFeeAccountingResult feeAccountingDetail = new ProductFeeAccountingResult();

            if (FeeAccountingId != null)
                feeAccountingDetail.fee_accounting_id = FeeAccountingId.Value;

            feeAccountingDetail.issuer_id = int.Parse(this.ddlIssuer.SelectedValue);
            feeAccountingDetail.fee_accounting_name = this.tbAccountingName.Text;

            feeAccountingDetail.fee_revenue_account_no = this.tbRevenueAccountNo.Text;
            feeAccountingDetail.fee_revenue_account_type_id = int.Parse(this.ddlRevenueAccountType.SelectedValue);
            feeAccountingDetail.fee_revenue_branch_code = this.tbRevenueBranchCode.Text;
            feeAccountingDetail.fee_revenue_narration_en = this.tbRevenueNarrationEn.Text;
            feeAccountingDetail.fee_revenue_narration_fr = this.tbRevenueNarrationFr.Text;
            feeAccountingDetail.fee_revenue_narration_pt = this.tbRevenueNarrationPt.Text;
            feeAccountingDetail.fee_revenue_narration_es = this.tbRevenueNarrationEs.Text;

            feeAccountingDetail.vat_account_no = this.tbVatAccountNo.Text;
            feeAccountingDetail.vat_account_type_id = int.Parse(this.ddlVatAccountType.SelectedValue);
            feeAccountingDetail.vat_account_branch_code = this.tbVatBranchCode.Text;
            feeAccountingDetail.vat_narration_en = this.tbVatNarrationEn.Text;
            feeAccountingDetail.vat_narration_fr = this.tbVatNarrationFr.Text;
            feeAccountingDetail.vat_narration_pt = this.tbVatNarrationPt.Text;
            feeAccountingDetail.vat_narration_es = this.tbVatNarrationEs.Text;
            
            return feeAccountingDetail;
        }
        #endregion

        #region Page Events
        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                UpdateFormLayout(PageLayout.CONFIRM_DELETE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                //if (Validation())
                UpdateFormLayout(PageLayout.CONFIRM_CREATE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {

                //this.btnEdit.Enabled = this.btnEdit.Visible = false;
                UpdateFormLayout(PageLayout.UPDATE);

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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                //if (Validation())
                UpdateFormLayout(PageLayout.CONFIRM_UPDATE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (CurrentPageLayout != null)
                {
                    pageLayout = CurrentPageLayout.Value;
                }
                string messages;
                ProductFeeAccountingResult details;
                ProductFeeAccountingResult feeAccountingDetails;

                switch (pageLayout)
                {
                    case PageLayout.CONFIRM_CREATE:
                        details = GetValuesFromControls();
                        
                        if (_batchservice.CreateFeeAccounting(details, out messages, out feeAccountingDetails))
                        {
                            FeeAccountingId = feeAccountingDetails.fee_accounting_id;
                            pageLayout = PageLayout.READ;
                            UpdateFormLayout(pageLayout);
                            this.lblInfoMessage.Text = messages;
                        }
                        else
                        {
                            this.lblErrorMessage.Text = messages;
                        }
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        if(_batchservice.DeleteFeeAccounting(FeeAccountingId.Value, out messages))
                        {
                            //redirect to list page, display message.
                            this.lblInfoMessage.Text = messages;
                            //UpdateFormLayout(pageLayout);
                            Server.Transfer("~\\webpages\\product\\ProductFeeAccountingList.aspx?delete=1");
                        }
                        else
                        {
                            this.lblErrorMessage.Text = messages;
                        }                        
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        details = GetValuesFromControls();
                        if (_batchservice.UpdateFeeAccounting(details, out messages, out feeAccountingDetails))
                        {                            
                            pageLayout = PageLayout.READ;
                            UpdateFormLayout(pageLayout);
                            this.lblInfoMessage.Text = messages;
                        }
                        else
                        {
                            this.lblErrorMessage.Text = messages;
                        }
                        break;
                    default:
                        break;
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
        #endregion

        public int? FeeAccountingId
        {
            get
            {
                if (ViewState["FeeAccountingId"] == null)
                    return null;
                else
                    return Convert.ToInt32(ViewState["FeeAccountingId"].ToString());
            }
            set
            {
                ViewState["FeeAccountingId"] = value;
            }
        }
        private PageLayout? CurrentPageLayout
        {
            get
            {
                if (ViewState["CurrentPageLayout"] == null)
                    return null;
                else
                    return (PageLayout)ViewState["CurrentPageLayout"];

            }
            set
            {
                ViewState["CurrentPageLayout"] = value;
            }
        }

        #region Page Flow Methods
        private void UpdateFormLayout(PageLayout? toPageLayout)
        {
            if (toPageLayout == null)
            {
                if (CurrentPageLayout != null)
                {
                    pageLayout = CurrentPageLayout.Value;
                }
                toPageLayout = pageLayout;
            }

            switch (toPageLayout)
            {
                case PageLayout.CREATE:
                    EnableFields(true);
                    this.btnCreate.Enabled = this.btnCreate.Visible = true;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.READ:
                    DisableFields(false, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = true;

                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.UPDATE:
                    EnableFields(false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.DELETE:
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    this.btnBack.Visible = false;
                    break;
                case PageLayout.CONFIRM_CREATE:
                    DisableFields(true, true);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.lblInfoMessage.Text = Resources.CommonInfoMessages.ReviewCommitMessage;
                    break;
                case PageLayout.CONFIRM_DELETE:
                    DisableFields(true, false);

                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = Resources.CommonInfoMessages.ReviewCommitMessage;
                    //this.btnUpdate.Text = "Confirm";
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    DisableFields(true, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = Resources.CommonInfoMessages.ReviewCommitMessage;
                    //this.btnUpdate.Text = "Confirm";
                    break;
                default:
                    DisableFields(false, false);
                    //this.btnUpdate.Text = UtilityClass.UppercaseFirst("EDIT");
                    break;
            }

            CurrentPageLayout = toPageLayout;
        }

        private bool Validation()
        {
            return true;
        }

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";


            switch (CurrentPageLayout)
            {
                case PageLayout.RESET:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.UPDATE:
                    //LoadRestoreGroup();
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.DELETE:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.CONFIRM_CREATE:
                    UpdateFormLayout(PageLayout.CREATE);

                    break;
                case PageLayout.CONFIRM_DELETE:
                    UpdateFormLayout(PageLayout.DELETE);
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    UpdateFormLayout(PageLayout.UPDATE);

                    break;
                default:
                    break;
            }
        }

        private void EnableFields(bool isCreate)
        {
            this.ddlIssuer.Enabled = isCreate;

            this.tbAccountingName.Enabled =
            this.tbAccountingName.Enabled =
            this.tbRevenueAccountNo.Enabled =
            this.ddlRevenueAccountType.Enabled =
            this.tbRevenueBranchCode.Enabled =
            this.tbRevenueNarrationEn.Enabled =
            this.tbRevenueNarrationFr.Enabled =
            this.tbRevenueNarrationPt.Enabled =
            this.tbRevenueNarrationEs.Enabled =
            this.tbVatAccountNo.Enabled =
            this.tbVatBranchCode.Enabled =
            this.ddlVatAccountType.Enabled =
            this.tbVatNarrationEn.Enabled =
            this.tbVatNarrationFr.Enabled =
            this.tbVatNarrationPt.Enabled = 
            this.tbVatNarrationEs.Enabled = true;

            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnEdit.Enabled = this.btnEdit.Visible = isCreate ? false : true;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            this.btnDelete.Enabled = this.btnDelete.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
            this.btnBack.Visible = isCreate ? false : true;
        }

        private void DisableFields(bool IsConfirm, bool isCreate)
        {
            this.ddlIssuer.Enabled = false;

            this.tbAccountingName.Enabled =
            this.tbAccountingName.Enabled =
            this.tbRevenueAccountNo.Enabled =
            this.ddlRevenueAccountType.Enabled =
            this.tbRevenueBranchCode.Enabled =
            this.tbRevenueNarrationEn.Enabled =
            this.tbRevenueNarrationFr.Enabled =
            this.tbRevenueNarrationPt.Enabled =
            this.tbRevenueNarrationEs.Enabled =
            this.tbVatAccountNo.Enabled =
            this.tbVatBranchCode.Enabled =
            this.ddlVatAccountType.Enabled =
            this.tbVatNarrationEn.Enabled =
            this.tbVatNarrationFr.Enabled =
            this.tbVatNarrationPt.Enabled = 
            this.tbVatNarrationEs.Enabled = false;

            this.btnBack.Visible = IsConfirm ? true : false;
            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnDelete.Enabled = this.btnDelete.Visible = isCreate;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = isCreate ? false : true;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = IsConfirm;

            ddlIssuer.Enabled = isCreate ? true : false;
        }
        #endregion
    }
}