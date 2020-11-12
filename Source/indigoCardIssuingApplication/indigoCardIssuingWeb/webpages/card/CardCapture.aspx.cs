using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using System.Web.UI.HtmlControls;
using System.Security.Permissions;
using indigoCardIssuingWeb.utility.ProductFieldUtility;
using System.IO;
using System.Web;
using System.Data;
using System.Reflection;

namespace indigoCardIssuingWeb.webpages.card
{
    public partial class CardCapture : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CardCapture));

        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly DocumentManagementService _documentMan = new DocumentManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR, UserRole.BRANCH_PRODUCT_OPERATOR };

        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
            }

            int productId = 0;
            if (int.TryParse(this.ddlProduct.SelectedValue, out productId))
            {
                if (Customer != null)
                    BuildPrintFields(productId, Customer.ProductFields);
                else
                {
                    var productFields = BuildPrintFields(productId, null);
                    Customer = new CustomerDetails { ProductFields = productFields };
                }
            }
        }

        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuerListItems = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuersList = new Dictionary<int, RolesIssuerResult>();
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerListItems, out issuersList);

                int issueType;
                CardSearchResult cardResult = null;
                //Check if we're being redirected to this page from MakerChecker Reject.
                if (SessionWrapper.CardSearchResultItem != null)
                {
                    cardResult = SessionWrapper.CardSearchResultItem;
                    SessionWrapper.CardSearchResultItem = null;
                    issueType = cardResult.card_issue_method_id;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(Request.QueryString["issuetype"]))
                    {
                        issueType = int.Parse(Request.QueryString["issuetype"]);
                    }
                    else
                    {
                        throw new Exception("Unkown issue type, please specify");
                    }
                }

                CardIssueMethodId = issueType;


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

                    UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue), true);
                    UpdateDomicileBranchList(int.Parse(this.ddlIssuer.SelectedValue));

                    if (cardResult != null)
                        UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue), cardResult.product_id);
                    else
                        UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue), null);


                    LoadFeeDetails();

                    //If we're loading from maker/checker reject
                    if (cardResult != null)
                    {
                        this.ddlBranch.SelectedValue = cardResult.branch_id.ToString();
                        //this.ddlProduct.SelectedValue = cardResult.product_id.ToString();
                        this.ddlPriority.SelectedValue = cardResult.card_priority_id.ToString();

                        CardIssueMethodId = cardResult.card_issue_method_id;
                        CardId = cardResult.card_id;

                        this.tbComments.Text = cardResult.comments;
                        this.pnlOther.Visible = true;
                    }
                }

                //Classic Issue Method, load additional page elements
                if (CardIssueMethodId == 0 || (CardIssueMethodId == 1 && Satellite_Branch_UserYN))
                {
                    this.lblPriority.Visible = true;
                    this.ddlPriority.Visible = true;
                    this.lblDelBranch.Visible = true;
                    this.ddlDelBranch.Visible = true;
                    this.ddlPriority.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                    int selectedValue;
                    var priorities = _batchService.GetCardPriorityList(out selectedValue);

                    this.ddlPriority.Items.AddRange(priorities.ToArray());
                    this.ddlPriority.SelectedValue = selectedValue.ToString();

                }
                else if (CardIssueMethodId == 1 && Main_Branch_UserYN)
                {
                    this.ddlCardNumber.Visible = true;
                }

                //Populate customer type drop down.
                List<ListItem> customerTypes = new List<ListItem>();
                this.ddlCustomerType.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                foreach (var item in _customerCardIssuerService.LangLookupCustomerTypes())
                {
                    customerTypes.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                }
                this.ddlCustomerType.Items.AddRange(customerTypes.OrderBy(m => m.Text).ToArray());
                this.ddlCustomerType.SelectedValue = "0";


                //Populate Resident Drop Down
                List<ListItem> resident = new List<ListItem>();
                this.ddlResident.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                foreach (var item in _customerCardIssuerService.LangLookupCustomerResidency())
                {
                    resident.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                }
                this.ddlResident.Items.AddRange(resident.OrderBy(m => m.Text).ToArray());
                this.ddlResident.SelectedValue = "0";

                //Populate Title Drop Down
                List<ListItem> title = new List<ListItem>();
                this.ddlTitle.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                foreach (var item in _customerCardIssuerService.LangLookupCustomerTitles())
                {
                    title.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                }
                this.ddlTitle.Items.AddRange(title.ToArray());

                if (Cache["custsettings"] != null)
                {
                    List<ConfigurationSettings> elements = (List<ConfigurationSettings>)Cache["custsettings"];
                    elements = elements.FindAll(i => i.PageName == "cardcapture").ToList();

                    ContentPlaceHolder myContent = (ContentPlaceHolder)this.Master.FindControl("MainContent");
                    HtmlGenericControl divcontrol = null;
                    foreach (var element in elements)
                    {
                        divcontrol = myContent.FindControl("div" + element.Key) as HtmlGenericControl;
                        RequiredFieldValidator req = myContent.FindControl("req" + element.Key) as RequiredFieldValidator;
                        if (element.Visibility)
                        {
                            divcontrol.Attributes.Add("style", "display:block");


                            if (divcontrol.FindControl("tb" + element.Key) != null)
                            {
                                TextBox tb = (TextBox)divcontrol.FindControl("tb" + element.Key);
                                tb.MaxLength = element.Length;

                            }
                            if (req != null && element.Visibility && element.Required)
                            {

                                req.Enabled = true;
                            }
                            else
                            {
                                req.Enabled = false;
                            }
                        }
                        else
                        {
                            divcontrol.Attributes.Add("style", "display:none");


                            if (req != null)
                            {
                                req.Enabled = false;
                            }
                        }
                    }
                }

                if (cardResult != null)
                {
                    var customer = _customerCardIssuerService.GetCustomerDetails(cardResult.card_id);

                    CustomerId = customer.customer_account_id;
                    this.ddlDomBranch.SelectedValue = customer.domicile_branch_id.ToString();
                    //this.ddlDelBranch.SelectedValue = customer.
                    this.tbAccountNumber.Text = customer.customer_account_number;
                    this.tbContactnumber.Text = customer.contact_number;
                    this.tbContractNumber.Text = customer.contract_number;
                    this.tbCustomerID.Text = customer.cms_id;
                    this.tbFirstName.Text = customer.customer_first_name;
                    this.tbIDNumber.Text = customer.Id_number;
                    this.tbLastName.Text = customer.customer_last_name;
                    this.tbMiddleName.Text = customer.customer_middle_name;
                    this.tbNameOnCard.Text = customer.name_on_card;
                    this.ddlAccountType.SelectedValue = customer.account_type_id.ToString();
                    this.ddlCurrency.SelectedValue = customer.currency_id.ToString();
                    this.ddlCustomerType.SelectedValue = customer.customer_type_id.ToString();
                    this.ddlReasonForIssue.SelectedValue = customer.card_issue_reason_id.ToString();
                    this.ddlResident.SelectedValue = customer.resident_id.ToString();
                    this.ddlTitle.SelectedValue = customer.customer_title_id.ToString();
                    this.tbEmailAddress.Text = customer.customer_email;
                }

                ChangeProduct();

                SetAccountInputRestrictions(tbAccountNumber);

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

        #endregion

        private void ChangeProduct()
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                if (this.ddlProduct.Items.Count > 0)
                {
                    int productId;
                    if (int.TryParse(this.ddlProduct.SelectedValue, out productId))
                    {
                        //get the product details
                        var product = _batchService.GetProduct(productId);

                        UpdateAccountValidationControls(product.Product.account_validation_YN);
                        IsAccountValidation = product.Product.account_validation_YN;
                        IsPinAccountValidation = product.Product.pin_account_validation_YN.GetValueOrDefault();
                        if (IsPinAccountValidation == true)
                        {
                            showPinValidationCapture.Visible = true;
                            tbAccPin.MaxLength = product.Product.max_pin_length;
                        }
                        else
                        {
                            showPinValidationCapture.Visible = false;
                        }
                        UpdateCardList(int.Parse(this.ddlBranch.SelectedValue), productId);
                        LoadFeeDetails();
                        UpdateAccountTypes(product.AccountTypes);
                        UpdateIssueReasons(product.CardIssueReasons);
                        UpdateCurrency(product.Currency.Select(s => s.currency_id).ToArray());

                        LoadGenders();

                        if (Customer == null)
                            Customer = new CustomerDetails { ProductFields = new ProductField[0] };

                        Customer.ProductFields = BuildPrintFields(product.Product.product_id, null);


                        ProductDocumentStructure docStructure = _documentMan.GetProductDocumentStructure(product.Product.product_id);

                        DocumentStructure = docStructure;

                        DisplayDocumentsGrid();
                        if (product.Product.credit_limit_capture.GetValueOrDefault() == true)
                        {
                            divCreditLimit.Visible = true;
                        }
                        else
                        {
                            divCreditLimit.Visible = false;
                        }

                        EmailAddressRequired = product.Product.email_required.GetValueOrDefault();

                        if (product.Product.email_required.GetValueOrDefault() == false)
                        {
                            reqEmailAddress.Enabled = false;
                        }

                        if (product.Product.credit_limit_capture.GetValueOrDefault() == true)
                        {
                            if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_OPERATOR, product.Product.issuer_id, out bool canRead, out bool canUpdate, out bool canCreate))
                            {
                                tbInternalAccountNo.Enabled = true;
                            }
                        }
                    }
                    else
                    {
                        this.lblErrorMessage.Text = "Error with product id";
                    }
                }
                else
                {
                    this.lblErrorMessage.Text = "There are no products available to choose from, please check configuration.";
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

        private void LoadGenders()
        {
            ddlGender.Items.Clear();
            var genderList = _customerCardIssuerService.LangLookupGenderTypes();
            this.ddlAccountType.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));

            //Populate account type drop down.
            List<ListItem> genders = new List<ListItem>();
            foreach (var item in genderList)
            {
                genders.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
            }
            this.ddlGender.Items.AddRange(genders.OrderBy(m => m.Text).ToArray());
        }

        private ProductDocumentStructure DocumentStructure
        {
            get
            {
                if (ViewState["ProductDocumentStructure"] != null)
                    return (ProductDocumentStructure)ViewState["ProductDocumentStructure"];
                else
                    return new ProductDocumentStructure();
            }
            set
            {
                ViewState["ProductDocumentStructure"] = value;
            }
        }

        /// <summary>
        /// Set screen based on issuer account validation.
        /// </summary>
        /// <param name="isAccValidation"></param>
        private void UpdateAccountValidationControls(bool isAccValidation)
        {
            //bool isAccValidation = IssuersList[int.Parse(this.ddlIssuer.SelectedValue)].account_validation_YN;

            this.btnValidateAccount.Visible =
                this.btnValidateAccount.Enabled = isAccValidation;
            //this.btnCancel.Visible =
            //this.btnCancel.Enabled = isAccValidation;

            this.pnlCustomerDetail.Visible =
                //this.tbAccountNumber.ReadOnly = 
                this.btnSave.Visible =
                this.btnSave.Enabled = isAccValidation ? false : true;

            DisplayDocumentsGrid();
            //if (!isAccValidation)
            //    PopulateIssueReasonDropDown();            
        }

        private void DisplayDocumentsGrid()
        {
            if (pnlCustomerDetail.Visible)
            {
                documentPanel.Visible = false;
                remoteDocuments.Visible = false;

                switch (DocumentStructure.StorageType)
                {
                    case DocumentStorageType.Local:
                        if (DocumentStructure.ProductDocuments != null &&
                            DocumentStructure.ProductDocuments.Length > 0)
                        {
                            documentPanel.Visible = true;
                        }
                        break;
                    case DocumentStorageType.Remote:
                        if (DocumentStructure.ProductDocuments != null &&
                            DocumentStructure.ProductDocuments.Length > 0)
                        {
                            remoteDocuments.Visible = true;
                        }
                        break;
                    case DocumentStorageType.Unknown:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                documentPanel.Visible = false;
            }
        }

        private void UpdateAccountTypes(int[] allowedAccounts)
        {
            this.ddlAccountType.Items.Clear();
            if (allowedAccounts != null && allowedAccounts.Length > 1)
                this.ddlAccountType.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));

            if (allowedAccounts == null || allowedAccounts.Length <= 0)
            {
                this.ddlAccountType.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                throw new Exception("No account types setup for product. Please check product setup.");
            }

            //Populate account type drop down.
            List<ListItem> accountTypes = new List<ListItem>();
            foreach (var item in _customerCardIssuerService.LangLookupCustomerAccountTypes())
            {
                if (allowedAccounts.Contains(item.lookup_id))
                    accountTypes.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
            }
            this.ddlAccountType.Items.AddRange(accountTypes.OrderBy(m => m.Text).ToArray());
        }

        private void UpdateIssueReasons(int[] allowedReasons)
        {
            ddlReasonForIssue.Items.Clear();
            if (allowedReasons != null && allowedReasons.Length > 1)
                ddlReasonForIssue.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));

            if (allowedReasons == null || allowedReasons.Length <= 0)
            {
                ddlReasonForIssue.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                throw new Exception("No issueing scenarios setup for product. Please check product setup.");
            }

            //Populate reason for issue drop down.            
            foreach (var reasonForIssue in _customerCardIssuerService.LangLookupCardIssueReasons())
            {
                if (allowedReasons.Contains(reasonForIssue.lookup_id))
                    this.ddlReasonForIssue.Items.Add(new ListItem(reasonForIssue.language_text, reasonForIssue.lookup_id.ToString()));
            }
        }

        private void UpdateCurrency(int[] allowedCurrency)
        {
            this.ddlCurrency.Items.Clear();
            if (allowedCurrency != null && allowedCurrency.Length > 1)
                this.ddlCurrency.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));

            if (allowedCurrency == null || allowedCurrency.Length <= 0)
            {
                this.ddlCurrency.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                throw new Exception("No currency setup for product. Please check product setup.");
            }

            //Populate Currency Drop Down
            List<ListItem> currency = new List<ListItem>();
            foreach (var currencyItem in _issuerMan.GetCurrencyList())
            {
                if (allowedCurrency.Contains(currencyItem.currency_id))
                    currency.Add(new ListItem(currencyItem.currency_code, currencyItem.currency_id.ToString()));
            }
            this.ddlCurrency.Items.AddRange(currency.OrderBy(m => m.Text).ToArray());
        }

        private void UpdateCardFees()
        {

        }

        /// <summary>
        /// Populates the branch drop down list.
        /// </summary>
        /// <param name="issuerId"></param>
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
                    if (branchList.Count > 0)//If there's only one branch then fetch cards.
                    {
                        if (this.ddlProduct.Items.Count > 0)
                            UpdateCardList(int.Parse(this.ddlBranch.SelectedValue), int.Parse(this.ddlProduct.SelectedValue));

                        //TODO: Hide branch ddl if only one branch?
                    }
                }
                else
                {

                    lblErrorMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "EmptyBrancheslistMessage").ToString();
                }
            }
        }


        private void UpdateDomicileBranchList(int issuerId)
        {
            if (issuerId >= 0)
            {
                this.ddlDomBranch.Items.Clear();
                this.ddlDelBranch.Items.Clear();

                var branches = _userMan.GetBranchesForIssuer(issuerId, null);

                List<ListItem> branchList = new List<ListItem>();

                foreach (var item in branches)//Convert branches in item list.
                {
                    branchList.Add(utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
                }

                if (branchList.Count > 0)
                {
                    ddlDomBranch.Items.AddRange(branchList.OrderBy(m => m.Text).ToArray());
                    ddlDelBranch.Items.AddRange(branchList.OrderBy(m => m.Text).ToArray());

                    ddlDomBranch.SelectedIndex = 0;
                    ddlDelBranch.SelectedIndex = 0;

                    if (!String.IsNullOrWhiteSpace(this.ddlBranch.SelectedValue))
                        ddlDelBranch.SelectedValue = ddlDomBranch.SelectedValue = this.ddlBranch.SelectedValue;
                }
            }
        }

        /// <summary>
        /// Updates the products dropdown with the issuers products
        /// </summary>
        /// <param name="issuerId"></param>
        private void UpdateProductList(int issuerId, int? productId)
        {
            this.ddlProduct.Items.Clear();

            if (issuerId > 0)
            {
                List<ProductValidated> products;
                string messages;
                if (_batchService.GetProductsListValidated(issuerId, CardIssueMethodId, 1, 1000, out products, out messages))
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
                    DisplayDocumentsGrid();
                }
            }
        }

        private bool ValidateProductLicenced()
        {
            ProductValidated productValidated;
            if (Products.TryGetValue(int.Parse(this.ddlProduct.SelectedValue), out productValidated))
            {
                if (productValidated.ValidLicence)
                {
                    this.pnlButtons.Visible =
                    this.pnlAccountNumber.Visible =
                    this.pnlCustomerDetail.Visible = true;

                    DisplayDocumentsGrid();
                    //UpdateAccountValidationControls();

                    return true;
                }
                else
                {
                    this.lblErrorMessage.Text = productValidated.Messages;
                    this.pnlButtons.Visible =
                    this.pnlAccountNumber.Visible =
                    this.pnlCustomerDetail.Visible = false;
                    DisplayDocumentsGrid();
                }
            }

            return false;
        }



        private void UpdateCardList(int branchId, int productId)
        {
            this.ddlCardNumber.Items.Clear();

            if (CardIssueMethodId == 1 && Main_Branch_UserYN)
            {
                this.pnlButtons.Visible = false;
                if (branchId >= 0)
                {
                    string messages;
                    var cards = _batchService.SearchBranchCards(branchId, productId, null, 1, null, (int)BranchCardStatus.AVAILABLE_FOR_ISSUE, 1, 1000, out messages);

                    if (String.IsNullOrWhiteSpace(messages))
                    {
                        if (cards.Count > 0)
                        {
                            Dictionary<long, CardSearchResult> cardsList = new Dictionary<long, CardSearchResult>();
                            //Populate cards drop down.
                            List<ListItem> itemList = new List<ListItem>();
                            ddlCardNumber.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                            foreach (var card in cards)
                            {
                                if (cardsList.ContainsKey(card.card_id))
                                    cardsList.Add(card.card_id, card);

                                if (IssuersList[int.Parse(ddlIssuer.SelectedValue)].card_ref_preference)
                                {
                                    this.lblCardNumber.Text = Resources.CommonLabels.CardNumber;
                                    itemList.Add(new ListItem(utility.UtilityClass.FormatPAN(card.card_number), card.card_id.ToString()));
                                }
                                else
                                {
                                    this.lblCardNumber.Text = Resources.CommonLabels.CardReferenceNumber;
                                    itemList.Add(new ListItem(card.card_request_reference, card.card_id.ToString()));
                                }
                            }
                            ddlCardNumber.Items.AddRange(itemList.OrderBy(m => m.Text).ToArray());
                            CardsList = cardsList;

                            CardListVisible(true);
                            this.pnlButtons.Visible = true;
                        }
                        else
                        {
                            CardListVisible(false);
                            this.lblInfoMessage.Text = GetLocalResourceObject("NoCardsCheckedoutMessage").ToString();
                        }
                    }
                    else
                    {
                        CardListVisible(false);
                        this.lblErrorMessage.Text = messages;
                    }
                }
            }
            else
            {
                CardListVisible(true);
                this.lblCardNumber.Visible = false;
                this.ddlCardNumber.Visible = false;
            }
        }

        //private void UpdateCustomerDetails(long cardId)
        //{
        //    SearchBranchCardsResult card;
        //    if (CardsList.TryGetValue(cardId, out card))
        //    {
        //        //Why is this happening for classic card?
        //        if (card.card_issue_method_id == 0)
        //        {
        //            var customerDetails = _customerCardIssuerService.GetCustomerDetails(cardId);

        //            this.tbAccountNumber.Text = customerDetails.customer_account_number;
        //            this.tbFirstName.Text = customerDetails.customer_first_name ?? "";
        //            this.tbMiddleName.Text = customerDetails.customer_middle_name ?? "";
        //            this.tbLastName.Text = customerDetails.customer_last_name ?? "";
        //            this.tbNameOnCard.Text = customerDetails.name_on_card ?? "";
        //            this.tbContractNumber.Text = customerDetails.contract_number ?? "";
        //            this.tbCustomerID.Text = customerDetails.cms_id ?? "";

        //            if (customerDetails.account_type_id != null)
        //            {
        //                this.ddlAccountType.SelectedValue = customerDetails.account_type_id.ToString();
        //            }
        //            else
        //            {
        //                this.ddlAccountType.SelectedValue = "-99";
        //            }
        //            if (customerDetails.currency_id != null)
        //            {
        //                this.ddlCurrency.SelectedValue = customerDetails.currency_id.ToString();
        //            }
        //            else
        //            {
        //                this.ddlCurrency.SelectedValue = "-99";
        //            }
        //            if (customerDetails.customer_type_id != null)
        //            {
        //                this.ddlCustomerType.SelectedValue = customerDetails.customer_type_id.ToString();
        //            }
        //            else
        //            {
        //                this.ddlCustomerType.SelectedValue = "-99";
        //            }
        //            if (customerDetails.card_issue_reason_id != null)
        //            {
        //                this.ddlReasonForIssue.SelectedValue = customerDetails.card_issue_reason_id.ToString();
        //            }
        //            else
        //            {
        //                this.ddlReasonForIssue.SelectedValue = "-99";
        //            }
        //            if (customerDetails.resident_id != null)
        //            {
        //                this.ddlResident.SelectedValue = customerDetails.resident_id.ToString();
        //            }
        //            else
        //            {
        //                this.ddlResident.SelectedValue = "-99";
        //            }
        //            if (customerDetails.customer_title_id != null)
        //            {
        //                this.ddlTitle.SelectedValue = customerDetails.customer_title_id.ToString();
        //            }
        //            else
        //            {
        //                this.ddlTitle.SelectedValue = "-99";
        //            }

        //            this.tbIDNumber.Text = customerDetails.Id_number;
        //            this.tbContactnumber.Text = customerDetails.contact_number;
        //            DisplayAllFields(false);
        //        }
        //    }
        //}

        private void CardListVisible(bool visible)
        {
            this.lblCardNumber.Visible = visible;
            this.ddlCardNumber.Visible = visible;
            this.lblReason.Visible = visible;
            this.ddlReasonForIssue.Visible = visible;
            this.pnlAccountNumber.Visible = visible;
        }

        #region VALIDATIONS
        //private List<string> validateAccountNumber(string AccountNumber)
        //{
        //    List<string> validationErrors = new List<string>();

        //    if (String.IsNullOrEmpty(AccountNumber))
        //    {
        //        validationErrors.Add("Account number is empty. Please enter an account number.");
        //    }
        //    else if (!utility.ValidationUtility.ValidateAlphaNumericOnly(AccountNumber))
        //    {
        //        validationErrors.Add("Account number may only contains letters and numbers.");
        //    }
        //    //TODO RAB 14Feb2014 - I don't like this. It seems like hard coded and client specific.
        //    else if (AccountNumber.StartsWith("998"))
        //    {
        //        validationErrors.Add("Account class 998 not allowed for issuing.");
        //    }
        //    else if (AccountNumber.Length <= 15)
        //    {
        //        validationErrors.Add("Account number too short. Current length is " + AccountNumber.Length);
        //    }
        //    else if (AccountNumber.Length >= 20)
        //    {
        //        validationErrors.Add("Account number too long. Current length is " + AccountNumber.Length);
        //    }
        //    //else if (_customerCardIssuerService.isAccountBlocked(AccountNumber))
        //    //{
        //    //    GenerateErrorMessage("ACCOUNT NOT ALLOWED FOR CARD ISSUING (ACCOUNT IS EXCLUDED)");
        //    //}                       

        //    return validationErrors;
        //}

        //private List<string> validateCardNumber(string CardNumber)
        //{
        //    List<string> validationErrors = new List<string>();

        //    if (String.IsNullOrEmpty(CardNumber))
        //    {
        //        validationErrors.Add("Card number is empty. Please enter a card number.");
        //    }
        //    else if (!utility.ValidationUtility.ValidateAlphaNumericOnly(CardNumber))
        //    {
        //        validationErrors.Add("Card number may only contains letters and numbers.");
        //    }
        //    else if (CardNumber.Length <= 15)
        //    {
        //        validationErrors.Add("Card number too short. Current length is " + CardNumber.Length);
        //    }
        //    else if (CardNumber.Length >= 20)
        //    {
        //        validationErrors.Add("Card number too long. Current length is " + CardNumber.Length);
        //    }

        //    return validationErrors;
        //}

        /// <summary>
        /// Validate user correctly filled in the form
        /// </summary>
        /// <returns></returns>
        private bool validateForm(bool validateDocuments = false)
        {
            if (CardIssueMethodId == 1 && this.ddlCardNumber.Visible == true && int.Parse(this.ddlCardNumber.SelectedValue) < 0 && Main_Branch_UserYN)
                this.lblErrorMessage.Text = GetLocalResourceObject("ValidationNoCardSelected").ToString();


            if (this.tbAccountNumber.Text.Length < 7)
            {
                if (this.lblErrorMessage.Text.Length > 0)
                    this.lblErrorMessage.Text += "<br />";

                this.lblErrorMessage.Text += GetLocalResourceObject("ValidationAccountNoShort").ToString();
            }

            if (this.ddlReasonForIssue.Visible == true && int.Parse(this.ddlReasonForIssue.SelectedValue) < 0)
            {
                if (this.lblErrorMessage.Text.Length > 0)
                    this.lblErrorMessage.Text += "<br />";

                this.lblErrorMessage.Text += GetLocalResourceObject("ValidationReasonForIssue").ToString();
            }

            if (validateDocuments)
            {
                if (EmailAddressRequired)
                {
                    if (string.IsNullOrEmpty(this.tbEmailAddress.Text))
                    {
                        if (this.lblErrorMessage.Text.Length > 0)
                        {
                            this.lblErrorMessage.Text += "<br/>";
                        }
                        this.lblErrorMessage.Text += GetLocalResourceObject("reqEmailAddressResource").ToString();
                    }
                    else
                    {
                        if (!IsValidEmail(this.tbEmailAddress.Text))
                        {
                            if (this.lblErrorMessage.Text.Length > 0)
                            {
                                this.lblErrorMessage.Text += "<br/>";
                            }
                            this.lblErrorMessage.Text += GetLocalResourceObject("reqEmailAddressResource").ToString();
                        }
                    }
                }

                if (DocumentStructure.StorageType == DocumentStorageType.Local && DocumentStructure.ProductDocuments != null)
                {
                    string missingDocumentValidation = ValidateRequiredDocuments();
                    if (missingDocumentValidation.Trim().Length > 0)
                    {
                        if (this.lblErrorMessage.Text.Length > 0)
                        {
                            this.lblErrorMessage.Text += "<br/>";
                        }
                        this.lblErrorMessage.Text += missingDocumentValidation;
                    }
                }
            }

            if (this.lblErrorMessage.Text.Length > 0)
                return false;
            else
                return true;
        }

        #endregion

        #region PRIVATE METHODS

        //private bool checkIfNumeric(string input)
        //{
        //    input.Trim();
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        if (!char.IsNumber(input[i]))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        private CustomerDetails CreateCustomerDetails()
        {
            string reasonForIssue = ddlReasonForIssue.SelectedItem.Text;
            string accountType = ddlAccountType.SelectedValue;

            var customer = new CustomerDetails();

            customer.CustomerTitleId = int.Parse(this.ddlTitle.SelectedValue);
            customer.FirstName = this.tbFirstName.Text.Trim();
            customer.MiddleName = this.tbMiddleName.Text.Trim();
            customer.LastName = this.tbLastName.Text.Trim();
            customer.CmsID = this.tbCustomerID.Text.Trim(); //Name of text box and field are wrong because of previous releases, so dont change
            customer.CustomerId = tbInternalAccountNo.Text.Trim();
            customer.ContractNumber = this.tbContractNumber.Text.Trim();
            customer.AccountNumber = this.tbAccountNumber.Text.Trim();
            customer.DomicileBranchId = int.Parse(this.ddlDomBranch.SelectedValue);

            customer.DeliveryBranchId = ddlDelBranch.Visible ? int.Parse(this.ddlDelBranch.SelectedValue) : 0;
            customer.CardIssueMethodId = CardIssueMethodId;


            if (CardIssueMethodId == 1 && Main_Branch_UserYN)
            {
                customer.CardId = long.Parse(this.ddlCardNumber.SelectedValue);
                customer.CardNumber = this.ddlCardNumber.SelectedItem.Text;
            }
            else
            {
                if (ddlPriority.Visible)
                {
                    if (this.ddlPriority.SelectedValue != "-99")
                    {
                        customer.PriorityId = int.Parse(this.ddlPriority.SelectedValue);
                    }
                    else
                    {
                        customer.PriorityId = null;
                    }
                }
            }

            customer.CardIssueReasonId = int.Parse(this.ddlReasonForIssue.SelectedValue);
            if (this.ddlCurrency.SelectedValue != "-99")
            {
                customer.AccountTypeId = int.Parse(this.ddlAccountType.SelectedValue);
            }
            else
            {
                customer.AccountTypeId = null;
            }
            if (this.ddlCustomerType.SelectedValue != "-99")
            {
                customer.CustomerTypeId = int.Parse(this.ddlCustomerType.SelectedValue);
            }
            else
            {
                customer.CustomerTypeId = null;
            }
            if (this.ddlReasonForIssue.SelectedValue != "-99")
            {
                customer.CardIssueReasonId = int.Parse(this.ddlReasonForIssue.SelectedValue);
            }
            else
            {
                customer.CardIssueReasonId = null;
            }
            if (this.ddlCurrency.SelectedValue != "-99")
            {
                customer.CurrencyId = int.Parse(this.ddlCurrency.SelectedValue);
            }
            else
            {
                customer.CurrencyId = null;
            }
            if (this.ddlResident.SelectedValue != "-99")
            {
                customer.CustomerResidencyId = int.Parse(this.ddlResident.SelectedValue);
            }
            else
            {
                customer.CustomerResidencyId = null;
            }
            customer.IssuerId = int.Parse(this.ddlIssuer.SelectedValue);
            customer.BranchId = int.Parse(this.ddlBranch.SelectedValue);
            customer.ProductId = int.Parse(this.ddlProduct.SelectedValue);
            customer.CBSAccountType = hdnCBSAccounType.Value;
            customer.CMSAccountType = hdnCMSAccounType.Value;
            customer.NameOnCard = this.tbNameOnCard.Text;
            customer.CustomerIDNumber = this.tbIDNumber.Text;
            customer.ContactNumber = this.tbContactnumber.Text;
            customer.FeeOverridenYN = this.chkOverrideFee.Checked;
            customer.FeeWaiverYN = this.chkWaiveFee.Checked;
            customer.FeeEditbleYN = this.tbApplicableFee.Enabled;
            customer.FeeDetailId = this.FeeDetailId;
            customer.EmailAddress = this.tbEmailAddress.Text;

            if (IsPinAccountValidation)
            {
                customer.AccountPin = this.ValidatedAccountPIN;
            }

            if (this.ddlFeeType.Items.Count > 0)
            {
                customer.FeeCharge = decimal.Parse(this.tbApplicableFee.Text);
                customer.Vat = decimal.Parse(this.tbVatRate.Text);
                if (customer.Vat != null && customer.FeeCharge != null)
                {
                    customer.VatCharged = customer.FeeCharge * (customer.Vat.Value / 100);
                }
                if (!String.IsNullOrWhiteSpace(this.tbTotalFee.Text))
                    customer.TotalCharged = decimal.Parse(this.tbTotalFee.Text);
            }

            customer.ProductFields = new ProductField[0];

            //Cutomer is populated from the start with print fields
            if (Customer != null && Customer.ProductFields != null)
                customer.ProductFields = Customer.ProductFields;

            return customer;
        }

        private void DisplayAllFields(bool showAll)
        {
            this.tbAccountNumber.Enabled = showAll;
            this.ddlCardNumber.Enabled = showAll;
            this.tbNameOnCard.Enabled = showAll;
            this.tbFirstName.Enabled = showAll;
            this.tbMiddleName.Enabled = showAll;
            this.tbLastName.Enabled = showAll;
            this.tbIDNumber.Enabled = showAll;
            this.tbContactnumber.Enabled = showAll;
            this.ddlAccountType.Enabled = showAll;
            this.ddlReasonForIssue.Enabled = showAll;
            this.ddlTitle.Enabled = showAll;
            this.tbEmailAddress.Enabled = showAll;
            this.ddlIssuer.Enabled = showAll;
            this.ddlBranch.Enabled = showAll;
            this.ddlCurrency.Enabled = showAll;
            this.ddlCustomerType.Enabled = showAll;
            this.ddlResident.Enabled = showAll;
            this.ddlProduct.Enabled = showAll;
            this.ddlDomBranch.Enabled =
            this.ddlDelBranch.Enabled =
            this.ddlFeeType.Enabled =
            this.tbCustomerID.Enabled =
            //this.reqCustomerID.Enabled =
            this.tbContractNumber.Enabled =
            this.chkOverrideFee.Enabled =
            this.chkWaiveFee.Enabled =
            //this.reqContractNumber.Enabled = 
            this.pnlCustomerDetail.Enabled = showAll;

            if (CardIssueMethodId == 0 || (CardIssueMethodId == 1 && Satellite_Branch_UserYN))
                this.ddlPriority.Enabled = showAll;

            this.btnSave.Visible = showAll;
            this.btnConfirm.Visible = showAll ? false : true;
            btnDocumentLocal.Visible = showAll;
            btnDocumentRemote.Visible = showAll;

            DisplayDocumentsGrid();
        }

        //private void EnableAllFields()
        //{
        //    tbAccountNumber.Enabled = false;
        //    ddlCardNumber.Enabled = true;
        //    ddlReasonForIssue.Enabled = true;
        //    ddlTitle.Enabled = true;
        //    btnCancel.Visible = true;

        //    int reasonForIssue;
        //    if (int.TryParse(this.ddlReasonForIssue.SelectedValue, out reasonForIssue))
        //    {
        //        //if ((IssuanceModes)reasonForIssue == IssuanceModes.Account_Existing_Customer)
        //        if (reasonForIssue == 1)
        //        {
        //            this.tbCustomerID.Enabled = true;
        //            this.tbContractNumber.Enabled = true;
        //        }
        //        else
        //        {
        //            this.tbCustomerID.Enabled = false;
        //            this.tbCustomerID.Text = "";
        //            this.tbContractNumber.Enabled = false;
        //            this.tbContractNumber.Text = "";
        //        }

        //        //if ((IssuanceModes)reasonForIssue == IssuanceModes.Supplement)
        //        if (reasonForIssue == 4)
        //        {
        //            this.tbNameOnCard.Enabled = true;
        //        }
        //    }
        //}

        //private void SetToCaptureMode()
        //{
        //    btnSave.Text = "Save";
        //    SessionWrapper.FormState = FormState.NEW;
        //    EnableAllFields();

        //    if (ddlReasonForIssue.Items[ddlReasonForIssue.SelectedIndex].Text == "NEW ACCOUNT - EXISTING CUSTOMER")
        //    {
        //        this.tbCustomerID.Enabled = true;
        //        this.tbContractNumber.Enabled = true;
        //    }
        //    else
        //    {
        //        this.tbCustomerID.Enabled = false;
        //        this.tbCustomerID.Text = "";
        //        this.tbContractNumber.Enabled = false;
        //        this.tbContractNumber.Text = "";
        //    }
        //}

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
            hdnCMSAccounType.Value = accountDetails.CMSAccountTypeId;
            hdnCBSAccounType.Value = accountDetails.CBSAccountTypeId;
            return rtn;
        }

        private void GenerateErrorMessage(string message)
        {
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Text += message + "<br/>";
        }

        private void GenerateErrorMessages(List<string> messages)
        {
            foreach (string message in messages)
            {
                lblErrorMessage.Text += "-" + message + "<br/>";
            }

            lblErrorMessage.ForeColor = Color.Red;
        }
        #endregion

        #region Page Events
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
                    UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue), null);

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

        protected void ddlBranch_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                int branchId;
                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId))
                {
                    UpdateCardList(branchId, int.Parse(this.ddlProduct.SelectedValue));
                }
                else
                {
                    this.lblErrorMessage.Text = "Error with branch id";
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

        protected void ddlProduct_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeProduct();
        }

        protected void ddlReasonForIssue_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                int reasonForIssue;
                if (int.TryParse(this.ddlReasonForIssue.SelectedValue, out reasonForIssue))
                {
                    if (reasonForIssue >= 1) //New Account - Existing Customer
                    {
                        //this.tbCustomerID.Enabled = true;
                        //this.reqCustomerID.Enabled = true;
                        //this.tbContractNumber.Enabled = true;
                        //this.reqContractNumber.Enabled = true;

                    }
                    else
                    {
                        //this.tbCustomerID.Enabled =
                        //this.reqCustomerID.Enabled =
                        this.tbContractNumber.Enabled =
                        this.reqContractNumber.Enabled = false;
                    }

                    if (reasonForIssue == 4) //Supplementary
                    {
                        //this.tbNameOnCard.ReadOnly = false;
                    }
                }

                LoadFees();
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

        protected void ddlFeeType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                ProductFeeDetailsResult detail;
                if (FeeDetails.TryGetValue(int.Parse(this.ddlFeeType.SelectedValue), out detail))
                {
                    SetFeeDetail(detail);
                }

                LoadFees();
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



        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        protected void btnValidateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                if (validateForm())
                {

                    int issuerId;
                    int branchId = int.Parse(this.ddlDomBranch.SelectedValue);
                    if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                    {
                        string messages;
                        AccountDetails accountDetails;
                        int issuingScenario = int.Parse(this.ddlReasonForIssue.SelectedValue);

                        string AccNum = this.IsPinAccountValidation ? string.Format("{0}|{1}", tbAccountNumber.Text, tbAccPin.Text) : this.tbAccountNumber.Text.Trim();

                        if (_customerCardIssuerService.GetAccountDetail(issuerId,
                                    int.Parse(this.ddlProduct.SelectedValue),
                                    issuingScenario,
                                    branchId, AccNum, out accountDetails, out messages))
                        {
                            Customer = AccountDetailsToCustomerDetails(accountDetails);

                            DisableAccountLookup();


                            this.tbNameOnCard.Text = accountDetails.NameOnCard;

                            this.tbAccountNumber.Text = accountDetails.AccountNumber;
                            this.tbFirstName.Text = accountDetails.FirstName;
                            this.tbMiddleName.Text = accountDetails.MiddleName;
                            this.tbLastName.Text = accountDetails.LastName;
                            this.tbIDNumber.Text = accountDetails.CustomerIDNumber;
                            this.tbContactnumber.Text = accountDetails.ContactNumber;

                            this.tbEmailAddress.Text = accountDetails.EmailAddress;

                            if (IsPinAccountValidation)
                            {
                                ValidatedAccountPIN = AccNum.Split('|')[1];
                                showPinValidationCapture.Visible = false;
                                this.tbAccountNumber.Text = AccNum.Split('|')[0];
                            }

                            this.ddlTitle.Enabled = true;

                            this.tbFirstName.ReadOnly = true;
                            this.tbMiddleName.ReadOnly = true;
                            this.tbLastName.ReadOnly = true;

                            if (accountDetails.AccountTypeId >= 0)
                            {
                                this.ddlAccountType.Enabled = false;
                                this.ddlAccountType.SelectedValue = accountDetails.AccountTypeId.ToString();
                            }
                            this.ddlCurrency.Enabled = false;
                            this.ddlCurrency.SelectedValue = accountDetails.CurrencyId.ToString();
                            this.ddlResident.Enabled = true;
                            this.ddlCustomerType.Enabled = true;

                            this.tbCustomerID.Text = accountDetails.CmsID;
                            if (String.IsNullOrWhiteSpace(accountDetails.CmsID))
                                this.tbCustomerID.Enabled = true;
                            else
                                this.tbCustomerID.Enabled = false;

                            this.tbInternalAccountNo.Text = accountDetails.CustomerId;

                            //this.tbNameOnCard.ReadOnly = true;  

                            this.pnlButtons.Visible = true;

                            //Depending on issuing scenario and if cards in list, display them
                            //If the card list is empty we assume the integration worked correctly but might not return cards for display
                            //Therefor is response was true the assume the call to integration account lookups is successful.
                            if (issuingScenario > 1 && accountDetails.CMSCards != null && accountDetails.CMSCards.Length > 0)
                            {
                                dlCardslist.DataSource = accountDetails.CMSCards;
                                dlCardslist.DataBind();
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "showCardDialog", "showCardDialog();", true);
                            }


                            LoadFeeDetails();
                            BuildPrintFields(int.Parse(this.ddlProduct.SelectedValue), accountDetails.ProductFields);

                            LoadRemoteDocuments();

                            foreach (ProductField c in accountDetails.ProductFields)
                            {
                                if (divPrintFields.FindControl("lbl_" + c.ProductPrintFieldId) != null)
                                {



                                    switch (c.MappedName.ToUpper().Trim())
                                    {
                                        case "ADDR1":
                                            ((TextBox)divPrintFields.FindControl("tb_" + c.ProductPrintFieldId)).Text = accountDetails.Address1;
                                            break;
                                        case "ADDR2":
                                            ((TextBox)divPrintFields.FindControl("tb_" + c.ProductPrintFieldId)).Text = accountDetails.Address2;
                                            break;
                                        case "ADDR3":
                                            ((TextBox)divPrintFields.FindControl("tb_" + c.ProductPrintFieldId)).Text = accountDetails.Address3;
                                            break;
                                    }

                                }
                            }
                        }
                        else
                        {
                            this.lblErrorMessage.Text = messages;
                        }
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

        private void LoadRemoteDocuments()
        {
            if (DocumentStructure != null)
            {
                if (DocumentStructure.StorageType == DocumentStorageType.Remote)
                {
                    if (DocumentStructure.ProductDocuments != null && DocumentStructure.ProductDocuments.Length > 0)
                    {
                        var remoteDocuments = _documentMan.CardDocumentsRemote(tbCustomerID.Text);
                        if (DocumentTable != null)
                        {
                            DocumentTable.Rows.Clear();     //remove any data
                        }
                        DocumentTable = ToDocumentDataTable(remoteDocuments);
                        grdRemoteDocuments.DataSource = DocumentTable;
                        grdRemoteDocuments.DataBind();
                    }
                }
            }
        }

        protected void dlCardslist_ItemCommand(object source, DataListCommandEventArgs e)
        {
            var cardNumber = ((LinkButton)e.CommandSource).Text;
            //tbSelectedCard.Text = cardNumber;
        }

        protected ProductField[] BuildPrintFields(int productId, ProductField[] productFields)
        {
            divPrintFields.Controls.Clear();
            divGender.Visible = false;
            reqGender.Enabled = false;

            ProductField[] list = new ProductField[0];

            if (Customer == null)
                Customer = new CustomerDetails();

            if (productFields == null)
                list = _issuerMan.GetPrintFieldsByProductid(productId);
            else
                list = productFields;


            foreach (var p in list)
            {
                if (p.Deleted == false && p.MappedName.ToUpper() != StaticFields.IND_SYS_NOC
                    && p.MappedName.ToUpper() != StaticFields.IND_SYS_PAN
                    && p.MappedName.ToUpper() != StaticFields.IND_SYS_GENDER
                    && p.MappedName.ToUpper() != StaticFields.IND_SYS_DOB)
                {
                    var controls = ProductFieldControl.Create(p);

                    divPrintFields.Controls.Add(controls.Item1);
                    divPrintFields.Controls.Add(controls.Item2);
                }
                if (p.Deleted == false && p.MappedName.ToUpper() == StaticFields.IND_SYS_GENDER)
                {
                    divGender.Visible = true;
                    reqGender.Enabled = true;
                    var gender = (p.Value != null ? System.Text.Encoding.UTF8.GetString(p.Value).ToUpper() : "F");
                    if (gender != "M" && gender != "F")
                    {
                        gender = "-99";
                    }
                    if (ddlGender.Items.Count > 0)
                    {
                        try
                        {
                            ddlGender.SelectedValue = gender;
                        }
                        catch (Exception)
                        {
                            //silent incase we get a gender that is different from external system
                        }
                    }
                }

                if (p.Deleted == false && p.MappedName.ToUpper() == StaticFields.IND_SYS_DOB)
                {
                    divDOB.Visible = true;
                    txtDOB.CssClass = "input";
                    txtDOB.Enabled = p.Editable;
                    requiredDOB.Enabled = txtDOB.Enabled;
                    if (p.Value != null && p.Value.Length > 0)
                    {
                        txtDOB.Text = System.Text.Encoding.UTF8.GetString(p.Value);
                    }
                    lblDateOfBirth.Text = p.Name;
                    lblDateOfBirth.CssClass = "label leftclr";
                }
            }

            return list.ToArray();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                //Pass all validations, set to confirm.
                if (validateForm(true))
                {
                    btnDocumentLocal.Visible = false;
                    btnDocumentRemote.Visible = false;

                    if (Customer != null)
                    {
                        var customer = Customer;

                        //TODO: images may slow down performance, maybe look at a way to cache server side.
                        //Get and save the print fields data here so that we dont loose file upload data
                        //Populates the product fields from the created text fields
                        foreach (var field in customer.ProductFields)
                        {
                            if (field.MappedName.ToUpper() == StaticFields.IND_SYS_NOC) //System default of name on card
                                field.Value = System.Text.Encoding.UTF8.GetBytes(tbNameOnCard.Text.ToUpper());
                            else if (field.Name.ToUpper() == StaticFields.IND_SYS_PAN) //System default of PAN
                                field.Value = System.Text.Encoding.UTF8.GetBytes(System.Text.RegularExpressions.Regex.Replace(this.ddlCardNumber.SelectedItem.Text.Replace("-", ""), ".{4}", "$0 "));
                            else if (field.MappedName.ToUpper() == StaticFields.ING_NOC) //System default of name on card
                                field.Value = System.Text.Encoding.UTF8.GetBytes(ddlPassporttype.SelectedValue);
                            else if (field.MappedName.ToUpper() == StaticFields.IND_SYS_GENDER) //gender
                            {
                                if (ddlGender.SelectedValue == "-99")
                                {
                                    field.Value = System.Text.Encoding.UTF8.GetBytes("M");
                                }
                                else
                                {
                                    field.Value = System.Text.Encoding.UTF8.GetBytes(ddlGender.SelectedValue);
                                }
                            }
                            else if (field.MappedName.ToUpper() == StaticFields.IND_SYS_DOB)
                            {
                                DateTime dob;
                                DateTime.TryParse(txtDOB.Text, out dob);
                                field.Value = System.Text.Encoding.UTF8.GetBytes(dob.ToString("dd/MM/yyyy"));
                            }
                            else
                            {
                                byte[] value;
                                if (divPrintFields.FindControl(ProductFieldControl.GetInputControlID(field)) != null)
                                {
                                    if (ProductFieldControl.TryGetValue(divPrintFields.FindControl(ProductFieldControl.GetInputControlID(field)), field, out value))
                                        field.Value = value;
                                }
                            }
                        }

                        Customer = customer;
                    }


                    ConfirmSave = true;
                    DisplayAllFields(false);
                    this.btnCancel.Enabled = this.btnCancel.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmAccountInfoMessage").ToString();
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

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblInfoMessage.Text = "";
                this.lblErrorMessage.Text = "";

                //if (SessionWrapper.IssuanceModeId == 1) //New Account - Existing Customer
                //{
                //if (String.IsNullOrEmpty(this.tbCustomerID.Text))
                //{
                //    GenerateErrorMessage("Please supply a valid Client ID");
                //    return;
                //}
                //}

                RolesIssuerResult issuerResult;
                if (IssuersList.TryGetValue(int.Parse(this.ddlIssuer.SelectedValue), out issuerResult))
                {
                    CustomerDetails customer;
                    if (CardId != null && CustomerId != null) //CardID should have a value only if it was set from reject
                    {
                        customer = Customer;//CreateCustomerDetails();
                        if (CardIssueMethodId == 1 && Main_Branch_UserYN)
                        {
                            customer.CardId = int.Parse(this.ddlCardNumber.SelectedValue);
                            customer.CardNumber = this.ddlCardNumber.SelectedItem.Text;
                        }

                        customer.IssuerId = int.Parse(this.ddlIssuer.SelectedValue);
                        customer.BranchId = int.Parse(this.ddlBranch.SelectedValue);
                        customer.ProductId = int.Parse(this.ddlProduct.SelectedValue);

                        if (customer.AccountTypeId == null || customer.AccountTypeId.Value < 0)
                            customer.AccountTypeId = int.Parse(ddlAccountType.SelectedValue);

                        customer.CBSAccountType = hdnCBSAccounType.Value;
                        customer.CMSAccountType = hdnCMSAccounType.Value;

                        customer.DomicileBranchId = int.Parse(this.ddlDomBranch.SelectedValue);
                        customer.DeliveryBranchId = int.Parse(this.ddlDelBranch.SelectedValue);

                        customer.PriorityId = ddlPriority.Visible ? int.Parse(this.ddlPriority.SelectedValue) : (int?)null;
                        customer.CustomerTitleId = int.Parse(this.ddlTitle.SelectedValue);
                        customer.CmsID = this.tbCustomerID.Text.Trim();
                        customer.ContractNumber = this.tbContractNumber.Text.Trim();
                        customer.CardIssueReasonId = int.Parse(this.ddlReasonForIssue.SelectedValue);
                        customer.CustomerResidencyId = int.Parse(this.ddlResident.SelectedValue);
                        customer.CustomerTypeId = int.Parse(this.ddlCustomerType.SelectedValue);
                        customer.NameOnCard = this.tbNameOnCard.Text;
                        customer.CustomerIDNumber = this.tbIDNumber.Text;
                        customer.ContactNumber = this.tbContactnumber.Text;
                        customer.FeeOverridenYN = this.chkOverrideFee.Checked;
                        customer.FeeWaiverYN = this.chkWaiveFee.Checked;
                        customer.FeeEditbleYN = this.tbApplicableFee.Enabled;
                        customer.CardIssueMethodId = CardIssueMethodId;

                        if (!String.IsNullOrWhiteSpace(this.tbApplicableFee.Text))
                            customer.FeeCharge = decimal.Parse(this.tbApplicableFee.Text);

                        if (!String.IsNullOrWhiteSpace(this.tbVatRate.Text))
                        {
                            customer.Vat = decimal.Parse(this.tbVatRate.Text);
                            if (customer.Vat != null && customer.FeeCharge != null)
                            {
                                customer.VatCharged = customer.FeeCharge * (customer.Vat.Value / 100);
                            }
                        }
                        if (!String.IsNullOrWhiteSpace(this.tbTotalFee.Text))
                            customer.TotalCharged = decimal.Parse(this.tbTotalFee.Text);
                    }
                    else
                    {
                        customer = CreateCustomerDetails();
                    }

                    string resultMessage;
                    bool result;
                    long cardId;

                    if (CardId != null && CustomerId != null) //Updating record
                    {
                        result = _customerCardIssuerService.UpdateCustomerDetails(CardId.Value, CustomerId.Value, customer, out resultMessage);
                        cardId = CardId.Value;

                    }
                    else //Creating new record
                    {
                        if (CardIssueMethodId == 0)
                        {
                            result = _customerCardIssuerService.RequestCardForCustomer(customer, null, out cardId, out resultMessage);
                        }
                        else if (CardIssueMethodId == 1 && Satellite_Branch_UserYN)
                        {
                            result = _customerCardIssuerService.RequestHybridCardForCustomer(customer, out cardId, out resultMessage);
                        }
                        else
                        {
                            result = _customerCardIssuerService.IssueCardToCustomer(customer, out resultMessage);
                            cardId = customer.CardId;
                        }
                    }


                    if (result)
                    {
                        SaveDocuments(cardId);
                        SaveCardLimit(cardId);
                        if (issuerResult.maker_checker_YN)
                        {
                            ConfirmSave = false;
                            this.lblInfoMessage.Text = resultMessage;
                            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                            this.btnCancel.Visible = this.btnCancel.Enabled = false;
                        }
                        else
                        {
                            CardSearchResult cardSearchResult = new CardSearchResult();
                            cardSearchResult.card_id = cardId;
                            SessionWrapper.CardSearchResultItem = cardSearchResult;
                            Server.Transfer(@"~\webpages\card\CardView.aspx");
                        }
                    }
                    else
                    {
                        this.lblErrorMessage.Text = resultMessage;
                    }
                }
                else
                {
                    this.lblErrorMessage.Text = "Could not find details for the issuer, please reload the page and try again.";
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

        private void SaveCardLimit(long cardId)
        {
            if (divCreditLimit.Visible)
            {
                decimal creditLimit;
                decimal.TryParse(tbCreditLimit.Text, out creditLimit);
                _customerCardIssuerService.CardLimitCreate(cardId, creditLimit);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            var isAccValidation = IsAccountValidation;// IssuersList[int.Parse(this.ddlIssuer.SelectedValue)].account_validation_YN;

            if (ConfirmSave && !isAccValidation) //Cancelling confirm and we did not use account lookup
            {
                //TODO: Enable controls
                ConfirmSave = false;
                DisplayAllFields(true);
                btnCancel.Visible = btnCancel.Enabled = false;
            }
            else if (ConfirmSave && isAccValidation) //Cancelling confirm but we did use account lookup
            {
                //TODO: Enable controls
                ConfirmSave = false;
                DisplayAllFields(true);
                DisableAccountLookup();
            }
            else if (!ConfirmSave && isAccValidation) //Cancel save and go back to account lookup
            {
                EnableAccountLookup();
                ClearControls();
            }
            else //Cancel save and clear all fields
            {
                DisplayAllFields(true);
                ClearControls();
            }



            //if (ConfirmSave)
            //{
            //    if (!IssuersList[int.Parse(this.ddlIssuer.SelectedValue)].account_validation_YN)
            //    {
            //        EnableAccountLookup();
            //    }

            //    int reasonForIssue;
            //    if (int.TryParse(this.ddlReasonForIssue.SelectedValue, out reasonForIssue))
            //    {
            //        if (reasonForIssue == 1) //New Account - Existing Customer
            //        {
            //            //this.tbCustomerID.Enabled = true;
            //            //this.reqCustomerID.Enabled = true;
            //            this.tbContractNumber.Enabled = true;
            //            this.reqContractNumber.Enabled = true;
            //        }
            //    }

            //    this.tbNameOnCard.Enabled =
            //    this.tbFirstName.Enabled =
            //    this.tbMiddleName.Enabled =
            //    this.tbLastName.Enabled =
            //    this.ddlTitle.Enabled =
            //    this.tbIDNumber.Enabled =
            //    this.tbContactnumber.Enabled =
            //    this.ddlCustomerType.Enabled =
            //    this.ddlResident.Enabled =
            //    this.ddlAccountType.Enabled =
            //    this.ddlReasonForIssue.Enabled =
            //    this.ddlCurrency.Enabled = true;

            //    this.tbCustomerID.Enabled = true;

            //    if (CardIssueMethodId == 0)
            //        this.ddlPriority.Enabled = true;

            //    this.btnSave.Visible = true;
            //    this.btnConfirm.Visible = false;
            //    ConfirmSave = false;
            //}
            //else
            //{
            //    //UpdateAccountValidationControls();                

            //    ClearControls();
            //}
        }

        private void ClearControls()
        {
            this.tbAccountNumber.Text = String.Empty;
            this.ddlReasonForIssue.SelectedIndex = 0;
            this.ddlReasonForIssue.Enabled = true;
            this.ddlProduct.Enabled = true;
            this.tbContractNumber.Text = String.Empty;
            this.tbNameOnCard.Text = String.Empty;
            this.tbCustomerID.Text = String.Empty;
            this.tbFirstName.Text = String.Empty;
            this.tbLastName.Text = String.Empty;
            this.tbMiddleName.Text = String.Empty;
            this.tbIDNumber.Text = String.Empty;
            this.tbContactnumber.Text = String.Empty;
            this.ddlCurrency.SelectedIndex = 0;

            this.ddlIssuer.Enabled = true;
            this.ddlBranch.Enabled = true;
            this.ddlCardNumber.Enabled = true;
            this.tbAccountNumber.Enabled = true;
            this.tbAccountNumber.ReadOnly = false;

            this.btnCancel.Visible = false;
        }

        private void EnableAccountLookup()
        {
            AccountLookupControls(true);
        }

        private void DisableAccountLookup()
        {
            AccountLookupControls(false);
        }

        private void AccountLookupControls(bool enable)
        {
            this.ddlIssuer.Enabled =
            this.ddlBranch.Enabled =
            this.ddlCardNumber.Enabled =
            this.ddlProduct.Enabled =
            this.ddlDomBranch.Enabled =
            this.ddlDelBranch.Enabled =
            this.ddlReasonForIssue.Enabled =
            this.tbAccountNumber.Enabled =
            this.btnValidateAccount.Visible = enable;

            this.tbAccountNumber.ReadOnly =
            this.btnCancel.Visible =
            this.btnSave.Visible =
            this.btnSave.Enabled =
            this.btnCancel.Visible =
            this.btnCancel.Enabled =
            this.pnlButtons.Visible =
            this.pnlCustomerDetail.Visible = !enable;

            DisplayDocumentsGrid();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this.ddlBranch.Items.Count == 0)
            {
                this.lblBranch.Visible =
                    this.ddlBranch.Visible = false;
            }

            if (this.ddlProduct.Items.Count == 0)
            {
                this.lblCardProduct.Visible =
                    this.ddlProduct.Visible = false;
            }

            if (this.ddlCardNumber.Items.Count == 0)
            {
                this.lblCardNumber.Visible =
                this.ddlCardNumber.Visible = false;
            }
        }
        #endregion

        #region ViewState Properties
        private long? CardId
        {
            get
            {
                if (ViewState["CardId"] != null)
                    return (long)ViewState["CardId"];
                else
                    return null;
            }
            set
            {
                ViewState["CardId"] = value;
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
        private long? CustomerId
        {
            get
            {
                if (ViewState["CustomerId"] != null)
                    return (long)ViewState["CustomerId"];
                else
                    return null;
            }
            set
            {
                ViewState["CustomerId"] = value;
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

        public bool IsAccountValidation
        {
            get
            {
                return (bool)ViewState["IsAccountValidation"];
            }
            set
            {
                ViewState["IsAccountValidation"] = value;
            }
        }

        public bool IsPinAccountValidation
        {
            get
            {
                return (bool)ViewState["IsPinAccountValidation"];
            }
            set
            {
                ViewState["IsPinAccountValidation"] = value;
            }
        }

        private string ValidatedAccountPIN
        {
            get
            {
                return (string)ViewState["ValidatedAccountPIN"];
            }
            set
            {
                ViewState["ValidatedAccountPIN"] = value;
            }

        }

        private bool EmailAddressRequired
        {
            get
            {
                return (bool)ViewState["EmailAddressRequired"];
            }
            set
            {
                ViewState["EmailAddressRequired"] = value;
            }

        }

        public Dictionary<int, RolesIssuerResult> IssuersList
        {
            get
            {
                if (ViewState["CardCaptureIssuersList"] != null)
                    return (Dictionary<int, RolesIssuerResult>)ViewState["CardCaptureIssuersList"];
                else
                    return new Dictionary<int, RolesIssuerResult>();
            }
            set
            {
                ViewState["CardCaptureIssuersList"] = value;
            }
        }

        public Dictionary<long, CardSearchResult> CardsList
        {
            get
            {
                if (ViewState["CardsList"] != null)
                    return (Dictionary<long, CardSearchResult>)ViewState["CardsList"];
                else
                    return new Dictionary<long, CardSearchResult>();
            }
            set
            {
                ViewState["CardsList"] = value;
            }
        }

        /// <summary>
        /// Cache the issuing type, 0 for request card, 1 for issue card.
        /// </summary>
        private int CardIssueMethodId
        {
            get
            {
                if (ViewState["CardIssueMethodId"] != null)
                    return (int)ViewState["CardIssueMethodId"];
                else
                    return 0;
            }
            set
            {
                ViewState["CardIssueMethodId"] = value;
            }
        }

        private int? FeeDetailId
        {
            get
            {
                if (ViewState["FeeDetailId"] != null)
                    return (int)ViewState["FeeDetailId"];
                else
                    return null;
            }
            set
            {
                ViewState["FeeDetailId"] = value;
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

        private Dictionary<int, ProductFeeDetailsResult> FeeDetails
        {
            get
            {
                if (ViewState["FeeDetails"] != null)
                    return (Dictionary<int, ProductFeeDetailsResult>)ViewState["FeeDetails"];
                else
                    return new Dictionary<int, ProductFeeDetailsResult>();
            }
            set
            {
                ViewState["FeeDetails"] = value;
            }
        }
        #endregion

        private void LoadFeeDetails()
        {
            this.ddlFeeType.Items.Clear();
            this.pnlCardFees.Visible = false;

            if (this.pnlCustomerDetail.Visible == true)
            {
                var response = _batchService.GetFeeDetailByProduct(int.Parse(this.ddlProduct.SelectedValue));

                if (response != null && response.Count > 0)
                {
                    this.pnlCardFees.Visible = true;

                    foreach (var item in response.OrderBy(o => o.fee_detail_name))
                    {
                        this.ddlFeeType.Items.Add(new ListItem(item.fee_detail_name, item.fee_detail_id.ToString()));
                    }

                    if (this.ddlFeeType.Items.Count > 0)
                        this.ddlFeeType.SelectedIndex = 0;

                    FeeDetails = response.ToDictionary(v => v.fee_detail_id, v => v);

                    SetFeeDetail(FeeDetails[int.Parse(this.ddlFeeType.SelectedValue)]);

                    LoadFees();
                }
            }
        }

        private void SetFeeDetail(ProductFeeDetailsResult detail)
        {
            this.chkOverrideFee.Enabled = detail.fee_editable_YN;
            this.chkWaiveFee.Enabled = detail.fee_waiver_YN;
        }

        private void LoadFees()
        {
            int feeDetailId = -1;
            int currencyId = -1;
            int reasonForIssue = -1;

            if (this.ddlFeeType.Items.Count > 0)
                int.TryParse(this.ddlFeeType.SelectedValue, out feeDetailId);

            if (this.ddlCurrency.Items.Count > 0)
                int.TryParse(this.ddlCurrency.SelectedValue, out currencyId);

            if (this.ddlReasonForIssue.Items.Count > 0)
                int.TryParse(this.ddlReasonForIssue.SelectedValue, out reasonForIssue);

            if (feeDetailId >= 0 && currencyId >= 0 && reasonForIssue >= 0)
            {
                var response = _batchService.GetCurrentFees(int.Parse(this.ddlFeeType.SelectedValue),
                                                            int.Parse(this.ddlCurrency.SelectedValue),
                                                            int.Parse(this.ddlReasonForIssue.SelectedValue), hdnCBSAccounType.Value);

                if (response != null)
                {
                    this.FeeDetailId = response.fee_detail_id;
                    this.tbApplicableFee.Text = response.fee_charge.ToString();
                    this.tbVatRate.Text = response.vat.ToString();
                    this.tbTotalFee.Text = (response.fee_charge * (1M + (response.vat * 0.01M))).ToString();
                }
                else
                    this.tbApplicableFee.Text =
                        this.tbVatRate.Text =
                        this.tbTotalFee.Text = "0.00";
            }
            else
            {
                this.tbApplicableFee.Text =
                        this.tbVatRate.Text =
                        this.tbTotalFee.Text = "0.00";
            }
        }

        protected void ddlCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text =
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                LoadFees();
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

        protected void chkWaiveFee_CheckedChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text =
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                if (this.chkWaiveFee.Checked)
                {
                    this.tbApplicableFee.Text = "0.00";
                    this.tbTotalFee.Text = "0.00";
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

        protected void chkOverrideFee_CheckedChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text =
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                if (this.chkOverrideFee.Checked)
                    this.tbApplicableFee.Enabled = true;
                else
                    this.tbApplicableFee.Enabled = false;
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

        protected void btnDocumentLocal_Click(object sender, EventArgs e)
        {
            localFileUpload.Visible = true;
            grdDocuments.Visible = false;
            btnDocumentLocal.Visible = false;
        }

        protected void btnLocalUpload_Click(object sender, EventArgs e)
        {
            if (localUploader.HasFile)
            {
                localFileUpload.Visible = false;
                grdDocuments.Visible = true;
                btnDocumentLocal.Visible = true;
                try
                {
                    List<LocalFileModel> files = new List<LocalFileModel>();
                    log.Trace("Start File Loop");
                    foreach (HttpPostedFile postedFile in localUploader.PostedFiles)
                    {
                        string fileName = Path.GetFileName(postedFile.FileName);

                        log.Trace($"Process {fileName}");
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(postedFile.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                        }
                        if (fileData != null)
                        {
                            files.Add(new LocalFileModel()
                            {
                                AccountNumber = tbAccountNumber.Text,
                                CustomerId = tbCustomerID.Text,
                                Content = fileData,
                                FileName = fileName
                            });
                        }
                    }

                    log.Trace("Files queued");
                    List<CardDocument> queuedFiles = _documentMan.UploadLocalFiles(tbAccountNumber.Text, tbCustomerID.Text, files);
                    log.Trace("Files uploaded");
                    DocumentTable = ToDocumentDataTable(queuedFiles);
                    grdDocuments.DataSource = DocumentTable;
                    grdDocuments.DataBind();
                }
                catch (Exception ex)
                {
                    log.Debug(ex.Message);
                    lblErrorMessage.Text = "Failed to upload selected files.";
                }
            }
        }

        private DataTable DocumentTable
        {
            get
            {
                if (ViewState["DocumentTable"] != null)
                    return (DataTable)ViewState["DocumentTable"];
                else
                    return null;
            }
            set
            {
                ViewState["DocumentTable"] = value;
            }
        }

        protected void grdDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                {
                    List<ListItem> documentTypes = new List<ListItem>();
                    foreach (var item in DocumentStructure.ProductDocuments)
                    {
                        documentTypes.Add(new ListItem()
                        {
                            Text = item.DocumentTypeName,
                            Value = item.DocumentTypeId.ToString()
                        });
                    }

                    DropDownList ddlDocs = (DropDownList)e.Row.FindControl("ddlDocumentType");

                    ddlDocs.Items.AddRange(documentTypes.OrderBy(m => m.Text).ToArray());
                }
            }
        }

        public DataTable ToDocumentDataTable<CardDocument>(List<CardDocument> items)
        {
            DataTable dataTable = new DataTable(typeof(CardDocument).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(CardDocument).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            dataTable.Columns.Add("ShortName");

            int lastId = 0;
            if (DocumentTable != null)
            {
                dataTable = DocumentTable;
                DataRow lastRow = dataTable.Select().LastOrDefault();
                if (lastRow != null)
                {
                    lastId = Math.Abs(Convert.ToInt32(lastRow["Id"]));
                }
            }

            foreach (CardDocument item in items)
            {
                var values = new object[dataTable.Columns.Count];
                string location = string.Empty;
                string comment = string.Empty;
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                    if (Props[i].Name == "Location")
                    {
                        location = values[i].ToString();
                    }
                    if (Props[i].Name == "Id")
                    {
                        values[i] = ((lastId + 1) * -1).ToString();
                    }
                    if (Props[i].Name == "Comment")
                    {
                        comment = values[i].ToString();
                    }
                }
                lastId++;
                if (DocumentStructure.StorageType == DocumentStorageType.Local)
                {
                    values[dataTable.Columns.Count - 1] = Path.GetFileName(location);
                }
                else
                {
                    values[dataTable.Columns.Count - 1] = comment;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        private string ValidateRequiredDocuments()
        {
            string result = string.Empty;
            if (DocumentStructure.ProductDocuments.Length > 0)
            {
                var requiredDocuments = DocumentStructure.ProductDocuments.Where(p => p.IsRequired).ToList();
                if (requiredDocuments.Count > 0)
                {
                    List<CardDocument> documents = new List<CardDocument>();
                    List<string> missingDocuments = new List<string>();

                    if (grdDocuments != null && grdDocuments.Rows != null)
                    {
                        foreach (GridViewRow row in grdDocuments.Rows)
                        {
                            HiddenField hidId = (HiddenField)row.FindControl("hidDocumentId");
                            HiddenField hidLocation = (HiddenField)row.FindControl("hidFullPath");
                            DropDownList ddlDocs = (DropDownList)row.FindControl("ddlDocumentType");

                            CardDocument toSave = new CardDocument()
                            {
                                DocumentTypeId = Convert.ToInt32(ddlDocs.SelectedValue),
                                Id = Convert.ToInt32(hidId.Value),
                                Location = hidLocation.Value
                            };
                            documents.Add(toSave);
                        }
                    }

                    foreach (var item in requiredDocuments)
                    {
                        var finder = documents.Where(p => p.DocumentTypeId == item.DocumentTypeId).FirstOrDefault();
                        if (finder == null)
                        {
                            missingDocuments.Add(item.DocumentTypeName);
                        }
                    }

                    if (missingDocuments.Count > 0)
                    {
                        result = $"Required documents are missing: {string.Join(", ", missingDocuments)}";
                    }
                }
            }

            return result;
        }

        private List<CardDocument> GetLocalDocuments(long cardId)
        {
            List<CardDocument> cardDocuments = new List<CardDocument>();
            grdDocuments.Enabled = false;

            if (grdDocuments.Rows.Count > 0)
            {
                foreach (GridViewRow row in grdDocuments.Rows)
                {
                    HiddenField hidId = (HiddenField)row.FindControl("hidDocumentId");
                    HiddenField hidLocation = (HiddenField)row.FindControl("hidFullPath");
                    DropDownList ddlDocs = (DropDownList)row.FindControl("ddlDocumentType");

                    CardDocument toSave = new CardDocument()
                    {
                        DocumentTypeId = Convert.ToInt32(ddlDocs.SelectedValue),
                        Id = Convert.ToInt32(hidId.Value),
                        CardId = cardId,
                        Location = hidLocation.Value
                    };
                    if (toSave.Id < 0)
                    {
                        toSave.Id = 0;
                    }
                    cardDocuments.Add(toSave);
                }
            }

            return cardDocuments;
        }

        private List<CardDocument> GetRemoteDocuments(long cardId)
        {
            List<CardDocument> cardDocuments = new List<CardDocument>();
            grdRemoteDocuments.Enabled = false;

            if (grdRemoteDocuments.Rows.Count > 0)
            {
                foreach (GridViewRow row in grdRemoteDocuments.Rows)
                {
                    HiddenField hidId = (HiddenField)row.FindControl("hidDocumentId");
                    HiddenField hidLocation = (HiddenField)row.FindControl("hidFullPath");
                    DropDownList ddlDocs = (DropDownList)row.FindControl("ddlDocumentType");
                    HiddenField hidComment = (HiddenField)row.FindControl("hidComment");

                    CardDocument toSave = new CardDocument()
                    {
                        DocumentTypeId = Convert.ToInt32(ddlDocs.SelectedValue),
                        Id = Convert.ToInt32(hidId.Value),
                        CardId = cardId,
                        Location = hidLocation.Value,
                        Comment = hidComment.Value
                    };
                    if (toSave.Id < 0)
                    {
                        toSave.Id = 0;
                    }
                    cardDocuments.Add(toSave);
                }
            }

            return cardDocuments;
        }

        private void SaveDocuments(long cardId)
        {
            List<CardDocument> cardDocuments = new List<CardDocument>();
            if (DocumentStructure.ProductDocuments.Length > 0)
            {
                if (DocumentStructure.StorageType == DocumentStorageType.Local)
                {
                    cardDocuments = GetLocalDocuments(cardId);
                }
                else if (DocumentStructure.StorageType == DocumentStorageType.Remote)
                {
                    cardDocuments = GetRemoteDocuments(cardId);
                }
            }

            foreach (var toSave in cardDocuments)
            {
                _documentMan.CardDocumentSave(toSave);
            }
        }

        protected void grdDocuments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Delete"))
            {

                DataRow dr = DocumentTable.Select("Id = " + e.CommandArgument).FirstOrDefault();
                if (dr != null)
                {
                    DocumentTable.Rows.Remove(dr);
                    DocumentTable.AcceptChanges();
                }

                grdDocuments.DataSource = DocumentTable;
                grdDocuments.DataBind(); // Bind your gridview again. 
            }
            if (e.CommandName.Equals("View"))
            {
                string fileName = e.CommandArgument.ToString();
                if (File.Exists(fileName))
                {
                    Response.Clear();
                    Response.AddHeader("content-disposition", "attachment;filename=" + Path.GetFileName(fileName));
                    Response.WriteFile(fileName);
                    Response.End();
                }
            }
        }

        protected void grdDocuments_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void grdDocuments_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {

        }

        protected void btnDocumentRemote_Click(object sender, EventArgs e)
        {
            LoadRemoteDocuments();
        }

        protected void grdRemoteDocuments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Delete"))
            {

                DataRow dr = DocumentTable.Select("Id = " + e.CommandArgument).FirstOrDefault();
                if (dr != null)
                {
                    DocumentTable.Rows.Remove(dr);
                    DocumentTable.AcceptChanges();
                }

                grdDocuments.DataSource = DocumentTable;
                grdDocuments.DataBind(); // Bind your gridview again. 
            }
            if (e.CommandName.Equals("View"))
            {
                string fileName = e.CommandArgument.ToString();

                Response.Clear();
                GridViewRow gvRow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                HiddenField hidDocumentTypeId = (HiddenField)gvRow.FindControl("hidDocumentTypeId");
                int documentTypeId;
                int.TryParse(hidDocumentTypeId.Value, out documentTypeId);
                string downloadFileName = $"Document_{tbAccountNumber.Text}_{DateTime.Now.ToString("yyyyMMddhhmmss")}.pdf";
                if (documentTypeId > 0)
                {
                    var documentType = DocumentStructure.ProductDocuments.Where(p => p.DocumentTypeId == documentTypeId).FirstOrDefault();
                    if (documentType != null)
                    {
                        downloadFileName = $"{documentType.DocumentTypeName.Replace("-", "_").Replace(" ", "")}_{downloadFileName}";
                    }
                }
                byte[] content = _documentMan.DownloadRemoteDocument(fileName);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + downloadFileName);
                Response.BinaryWrite(content);
                Response.End();
            }
        }

        protected void grdRemoteDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                {
                    List<ListItem> documentTypes = new List<ListItem>();

                    foreach (var item in DocumentStructure.ProductDocuments)
                    {
                        documentTypes.Add(new ListItem()
                        {
                            Text = item.DocumentTypeName,
                            Value = item.DocumentTypeId.ToString()
                        });
                    }

                    DropDownList ddlDocs = (DropDownList)e.Row.FindControl("ddlDocumentType");

                    ddlDocs.Items.Add(new ListItem() { Text = "--Link--", Value = "-99" });
                    ddlDocs.Items.AddRange(documentTypes.OrderBy(m => m.Text).ToArray());

                    LinkButton linkButton = (LinkButton)e.Row.FindControl("btnUnlink");
                    HiddenField hidId = (HiddenField)e.Row.FindControl("hidDocumentId");
                    long documentId = 0;
                    long.TryParse(hidId.Value, out documentId);
                    if (documentId <= 0)
                    {
                        linkButton.Visible = false;
                        ddlDocs.SelectedValue = "-99";
                    }
                }
            }

        }

        protected void grdRemoteDocuments_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {

        }

        protected void grdRemoteDocuments_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }
    }
}