using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;

namespace indigoCardIssuingWeb.webpages.fundsload
{
    public partial class LoadFunds : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoadFunds));
        private readonly FundsLoadService _fundsLoadService = new FundsLoadService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly BatchManagementService _batchService = new BatchManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR, UserRole.BRANCH_PRODUCT_OPERATOR };

        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private Dictionary<int, ProductValidated> Products
        {
            get
            {
                if (ViewState["Products"] != null)
                    return (Dictionary<int, ProductValidated>)ViewState["Products"];
                else
                    return new Dictionary<int, ProductValidated>();
            }
            set
            {
                ViewState["Products"] = value;
            }
        }

        private bool Satellite_Branch_UserYN
        {
            get
            {
                if (ViewState["Satellite_Branch_UserYN"] != null)
                    return (bool)ViewState["Satellite_Branch_UserYN"];
                else
                    return false;
            }
            set
            {
                ViewState["Satellite_Branch_UserYN"] = value;
            }
        }

        private bool Main_Branch_UserYN

        {
            get
            {
                if (ViewState["Main_Branch_UserYN"] != null)
                    return (bool)ViewState["Main_Branch_UserYN"];
                else
                    return false;
            }
            set
            {
                ViewState["Main_Branch_UserYN"] = value;
            }
        }


        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuerListItems = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuersList = new Dictionary<int, RolesIssuerResult>();
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerListItems, out issuersList);

                CardSearchResult cardResult = null;
                //Check if we're being redirected to this page from MakerChecker Reject.
                if (SessionWrapper.CardSearchResultItem != null)
                {
                    cardResult = SessionWrapper.CardSearchResultItem;
                    SessionWrapper.CardSearchResultItem = null;
                }
                else
                {

                }
                if (issuerListItems.ContainsKey(-1))
                {
                    issuerListItems.Remove(-1);
                    issuersList.Remove(-1);
                }

                this.ddlIssuer.Items.AddRange(issuerListItems.Values.OrderBy(m => m.Text).ToArray());
                if (cardResult != null)
                    this.ddlIssuer.SelectedValue = cardResult.issuer_id.ToString();
                else
                    this.ddlIssuer.SelectedIndex = 0;

                IssuersList = issuersList; //save to ViewState property.
                if (issuersList.Any(a => a.Value.SatelliteBranch_YN == true))
                {
                    Satellite_Branch_UserYN = true;
                }
                if (issuersList.Any(a => a.Value.MainBranch_YN == true))
                {
                    Main_Branch_UserYN = true;
                }

                if (issuerListItems.Count > 0)
                {
                    UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue), null);
                    UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue), true);
                }

                lblPrepaidNotFound.Visible = false;
                lblBankAccountNotFound.Visible = false;

                SetAccountInputRestrictions(tbAccountNumber);
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

        protected void btnValidateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                decimal amount = 0;
                decimal.TryParse(tbAmount.Text, out amount);

                bool accountIsValid = BankAccountIsValid(tbAccountNumber.Text, amount);

                tbAccountNumber.Enabled = !accountIsValid;
                tbAmount.Enabled = !accountIsValid;

                panelPrepaid.Visible = accountIsValid;
                lblBankAccountNotFound.Visible = !accountIsValid;
                btnCancel.Enabled = accountIsValid;
                btnCancel.Visible = accountIsValid;
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

        private void ValidateFundsLoad()
        {
            bool prepaidIsValid = PrePaidAccountIsValid(txtPrepaidCard.Text);

            txtPrepaidCard.Enabled = !prepaidIsValid;

            panelPrepaidDetails.Visible = prepaidIsValid;
            lblPrepaidNotFound.Visible = !prepaidIsValid;
            pnlButtons.Visible = prepaidIsValid;
            btnCancel.Enabled = btnCancel.Visible = prepaidIsValid;
            btnSave.Enabled = btnSave.Visible = prepaidIsValid;
        }

        private CustomerDetails Customer
        {
            get
            {
                if (ViewState["Customer"] != null)
                    return (CustomerDetails)ViewState["Customer"];
                else
                    return null; //new CustomerDetails { ProductFields = new ProductField[0] };
            }
            set
            {
                ViewState["Customer"] = value;
            }
        }

        private PrepaidAccountDetail PrepaidAccountDetail
        {
            get
            {
                if (ViewState["PrepaidAccount"] != null)
                    return (PrepaidAccountDetail)ViewState["PrepaidAccount"];
                else
                    return null; //new CustomerDetails { ProductFields = new ProductField[0] };
            }
            set
            {
                ViewState["PrepaidAccount"] = value;
            }
        }

        private CustomerDetails AccountDetailsToCustomerDetails(AccountDetails accountDetails)
        {
            CustomerDetails rtn = new CustomerDetails()
            {
                AccountNumber = accountDetails.AccountNumber,
                AccountTypeId = accountDetails.AccountTypeId,
                CurrencyId = accountDetails.CurrencyId,
                CustomerIDNumber = accountDetails.CustomerIDNumber,
                ContractNumber = accountDetails.ContractNumber,
                CustomerResidencyId = accountDetails.CustomerResidencyId,
                CustomerTitleId = accountDetails.CustomerTitleId,
                CustomerTypeId = accountDetails.CustomerTypeId,
                FirstName = accountDetails.FirstName,
                LastName = accountDetails.LastName,
                MiddleName = accountDetails.MiddleName,
                NameOnCard = accountDetails.NameOnCard,
                ContactNumber = accountDetails.ContactNumber,
                CmsID = accountDetails.CmsID,
                ProductFields = accountDetails.ProductFields
            };
            return rtn;
        }

        private bool BankAccountIsValid(string bankAccountNumber, decimal amount)
        {
            bool result = false;
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            this.lblBankAccountNotFound.Text = "";

            string messages;
            AccountDetails accountDetails;
            int issuerId;
            int branchId = 0;

            int productId = 0;

            if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
            {
                int.TryParse(this.ddlProduct.SelectedValue, out productId);
                int.TryParse(this.ddlBranch.SelectedValue, out branchId);

                var product = _batchService.GetProduct(productId);
                int cardIssueReasonId = product.CardIssueReasons[0];
                bool gotAccountDetail = _fundsLoadService.GetAccountDetail(issuerId,
                        productId,
                        branchId, cardIssueReasonId, this.tbAccountNumber.Text.Trim(), amount, out accountDetails, out messages);

                if (gotAccountDetail)
                {
                    Customer = AccountDetailsToCustomerDetails(accountDetails);
                    if (accountDetails != null)
                    {
                        messages = string.Empty;
                    }

                    if (string.IsNullOrWhiteSpace(messages))
                    {
                        tbFirstName.Text = accountDetails.FirstName;
                        tbLastName.Text = accountDetails.LastName;
                        txtAddress.Text = string.Format("{0}{1}{2}{1}{3}", accountDetails.Address1, Environment.NewLine, accountDetails.Address2, accountDetails.Address3);

                        result = true;
                        pnlCustomerDetail.Visible = true;       //the account is found
                        this.pnlButtons.Visible = true;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = messages;
                    }
                }
                else
                {
                    this.lblBankAccountNotFound.Text = messages;
                }
            }

            return result;
        }

        private bool PrePaidAccountIsValid(string prepaidCardNumber)
        {
            bool result = false;
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            string messages;
            PrepaidAccountDetail accountDetails;

            int productId = 0;

            if (int.TryParse(this.ddlProduct.SelectedValue, out productId))
            {

                bool gotAccountDetail = _fundsLoadService.GetPrepaidAccountDetail(productId,
                    prepaidCardNumber, 0, out accountDetails, out messages);

                messages = string.Empty;
                if (gotAccountDetail)
                {
                    PrepaidAccountDetail = accountDetails;

                    if (string.IsNullOrWhiteSpace(messages))
                    {
                        txtPrepaidAccountNumber.Text = accountDetails.AccountNumber;

                        this.panelPrepaidDetails.Visible = true;
                        result = true;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = messages;
                    }
                }
            }

            return result;
        }

        protected void btnValidatePrepaid_Click(object sender, EventArgs e)
        {
            ValidateFundsLoad();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (btnConfirm.Visible == true)
            {
                btnSave.Visible = true;
                btnConfirm.Visible = false;
            }
            else
            {
                tbAccountNumber.Enabled = true;
                tbAmount.Enabled = true;
                txtPrepaidCard.Enabled = true;
                panelPrepaidDetails.Visible = false;
                pnlCustomerDetail.Visible = false;
                panelPrepaid.Visible = false;
                pnlButtons.Visible = false;
                btnCancel.Visible = false;

                btnSave.Visible = false;
            }
            this.lblInfoMessage.Text = string.Empty;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                this.btnCancel.Enabled = this.btnCancel.Visible = true;
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmAccountInfoMessage").ToString();
                btnSave.Visible = false;
                btnConfirm.Visible = true;
                btnValidateAccount.Enabled = false;
                btnValidatePrepaid.Enabled = false;
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

        protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                {
                    //UpdateAccountValidationControls();
                    UpdateBranchList(issuerId, true);
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

        private void UpdateBranchList(int issuerId, bool useCache)
        {
            if (issuerId >= 0)
            {
                this.ddlBranch.Items.Clear();
                Dictionary<int, ListItem> branchList = new Dictionary<int, ListItem>();

                //if (!useCache /*|| BranchList.Count == 0*/) //should I use cache? if yes check that there is something in page cache.
                //{
                var branches = _userMan.GetBranchesForUser(issuerId, userRolesForPage[0], null);

                foreach (var item in branches)//Convert branches in item list.
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

        private void UpdateProductList(int issuerId, int? productId)
        {
            this.ddlProduct.Items.Clear();

            if (issuerId > 0)
            {
                List<ProductValidated> products;
                string messages;
                if (_fundsLoadService.GetProductsListValidated(issuerId, null, 1, 1000, out products, out messages))
                {
                    List<ListItem> productsList = new List<ListItem>();
                    Dictionary<int, ProductValidated> productDict = new Dictionary<int, ProductValidated>();

                    foreach (var product in products)
                    {
                        if (!productDict.ContainsKey(product.ProductId))
                            productDict.Add(product.ProductId, product);

                        productsList.Add(utility.UtilityClass.FormatListItem<int>(product.ProductName, product.ProductCode, product.ProductId));
                    }

                    Products = productDict;

                    if (productsList.Count > 0)
                    {
                        this.ddlProduct.Items.AddRange(productsList.OrderBy(m => m.Text).ToArray());

                        if (productId != null)
                            this.ddlProduct.SelectedValue = productId.Value.ToString();
                        else
                            this.ddlProduct.SelectedIndex = 0;
                    }

                    this.ddlProduct.Visible = true;
                    this.lblCardProduct.Visible = true;

                }
                else
                {
                    this.lblErrorMessage.Text = messages;
                    this.pnlAccountNumber.Visible = false;
                    this.pnlButtons.Visible = false;
                    this.pnlCustomerDetail.Visible = false;
                }
            }
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblInfoMessage.Text = "";
                this.lblErrorMessage.Text = "";

                long fundsLoadId = 0;
                string resultMessage = string.Empty;
                bool result;

                int issuerId = Convert.ToInt32(ddlIssuer.SelectedValue);
                int branchId = Convert.ToInt32(ddlBranch.SelectedValue);
                int productId = Convert.ToInt32(ddlProduct.SelectedValue);

                result = _fundsLoadService.CreateFundsLoad(issuerId, productId, branchId, 
                    tbAccountNumber.Text, txtPrepaidAccountNumber.Text, txtPrepaidCard.Text, 
                    decimal.Parse(tbAmount.Text), tbFirstName.Text, tbLastName.Text, txtAddress.Text,
                    out fundsLoadId, out resultMessage);
                if (result)
                {
                    this.lblInfoMessage.Text = resultMessage;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                    this.btnCancel.Visible = this.btnCancel.Enabled = false;
                }
                else
                {
                    this.lblErrorMessage.Text = resultMessage;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;

                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        public Dictionary<int, RolesIssuerResult> IssuersList
        {
            get
            {
                if (ViewState["FundsLoadIssuersList"] != null)
                    return (Dictionary<int, RolesIssuerResult>)ViewState["FundsLoadIssuersList"];
                else
                    return new Dictionary<int, RolesIssuerResult>();
            }
            set
            {
                ViewState["FundsLoadIssuersList"] = value;
            }
        }

        protected void TransferDetailsChanged(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                panelPrepaidDetails.Visible = false;
                pnlCustomerDetail.Visible = false;
                panelPrepaid.Visible = false;
                pnlButtons.Visible = false;
            }
        }

        protected void txtPrepaidCard_TextChanged(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                panelPrepaidDetails.Visible = false;
                pnlButtons.Visible = false;
            }
        }
    }
}