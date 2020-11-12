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
using Veneka.Indigo.UX.NativeAppAPI;
using System.Security.Principal;
using indigoCardIssuingWeb.Old_App_Code.service;

namespace indigoCardIssuingWeb.webpages.card
{
    public partial class InstantCard : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InstantCard));

        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly TerminalService _teminalService = TerminalService.Instance;
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
                //if (CardIssueMethodId == 0 || (CardIssueMethodId == 1 && Satellite_Branch_UserYN))
                //{
                //    this.lblPriority.Visible = true;
                //    this.ddlPriority.Visible = true;
                //    this.lblDelBranch.Visible = true;
                //    this.ddlDelBranch.Visible = true;
                //    this.ddlPriority.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                //    int selectedValue;
                //    var priorities = _batchService.GetCardPriorityList(out selectedValue);

                //    this.ddlPriority.Items.AddRange(priorities.ToArray());
                //    this.ddlPriority.SelectedValue = selectedValue.ToString();

                //}
                //else if (CardIssueMethodId == 1 && Main_Branch_UserYN )
                //{
                //    this.ddlCardNumber.Visible = true;
                //}

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

                        LoadFeeDetails();
                        UpdateAccountTypes(product.AccountTypes);
                        UpdateIssueReasons(product.CardIssueReasons);
                        UpdateCurrency(product.Currency.Select(s => s.currency_id).ToArray());

                        if (Customer == null)
                            Customer = new CustomerDetails { ProductFields = new ProductField[0] };

                        Customer.ProductFields = BuildPrintFields(product.Product.product_id, null);
                        this.pnlButtons.Visible = true;
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
                this.pnlAccountNumber.Visible =
               this.tbAccountNumber.Visible =
                this.btnSave.Visible =
                this.btnSave.Enabled = isAccValidation ? false : true;
            this.tbAccountNumber.ReadOnly = isAccValidation ? true : false;

            //if (!isAccValidation)
            //    PopulateIssueReasonDropDown();            
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
                        //if (this.ddlProduct.Items.Count > 0)
                        //    UpdateCardList(int.Parse(this.ddlBranch.SelectedValue), int.Parse(this.ddlProduct.SelectedValue));

                        //TODO: Hide branch ddl if only one branch?
                        this.pnlButtons.Visible = true;
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
                //this.ddlDelBranch.Items.Clear();

                var branches = _userMan.GetBranchesForIssuer(issuerId, null);

                List<ListItem> branchList = new List<ListItem>();

                foreach (var item in branches)//Convert branches in item list.
                {
                    branchList.Add(utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
                }

                if (branchList.Count > 0)
                {
                    ddlDomBranch.Items.AddRange(branchList.OrderBy(m => m.Text).ToArray());
                    //ddlDelBranch.Items.AddRange(branchList.OrderBy(m => m.Text).ToArray());

                    ddlDomBranch.SelectedIndex = 0;
                    //ddlDelBranch.SelectedIndex = 0;

                    if (!String.IsNullOrWhiteSpace(this.ddlBranch.SelectedValue))
                        ddlDomBranch.SelectedValue = this.ddlBranch.SelectedValue;
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
                        {
                            this.ddlProduct.SelectedIndex = 0;

                        }

                    }

                    this.ddlProduct.Visible = true;
                    this.lblCardProduct.Visible = true;
                    ChangeProduct();
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

                    UpdateAccountValidationControls(false);

                    return true;
                }
                else
                {
                    this.lblErrorMessage.Text = productValidated.Messages;
                    this.pnlButtons.Visible =
                    this.pnlAccountNumber.Visible =
                    this.pnlCustomerDetail.Visible = false;
                }
            }

            return false;
        }

        //private void UpdateCardList(int branchId, int productId)
        //{
        //    this.ddlCardNumber.Items.Clear();

        //    if (CardIssueMethodId == 1 && Main_Branch_UserYN)
        //    {
        //        this.pnlButtons.Visible = false;
        //        if (branchId >= 0)
        //        {
        //            string messages;
        //            var cards = _batchService.SearchBranchCards(branchId, productId, null, 1, null, (int)BranchCardStatus.AVAILABLE_FOR_ISSUE, 1, 1000, out messages);

        //            if (String.IsNullOrWhiteSpace(messages))
        //            {
        //                if (cards.Count > 0)
        //                {
        //                    Dictionary<long, CardSearchResult> cardsList = new Dictionary<long, CardSearchResult>();
        //                    //Populate cards drop down.
        //                    List<ListItem> itemList = new List<ListItem>();
        //                    ddlCardNumber.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
        //                    foreach (var card in cards)
        //                    {
        //                        if (cardsList.ContainsKey(card.card_id))
        //                            cardsList.Add(card.card_id, card);

        //                        if (IssuersList[int.Parse(ddlIssuer.SelectedValue)].card_ref_preference)
        //                        {
        //                            this.lblCardNumber.Text = Resources.CommonLabels.CardNumber;
        //                            itemList.Add(new ListItem(utility.UtilityClass.FormatPAN(card.card_number), card.card_id.ToString()));
        //                        }
        //                        else
        //                        {
        //                            this.lblCardNumber.Text = Resources.CommonLabels.CardReferenceNumber;
        //                            itemList.Add(new ListItem(card.card_request_reference, card.card_id.ToString()));
        //                        }
        //                    }
        //                    ddlCardNumber.Items.AddRange(itemList.OrderBy(m => m.Text).ToArray());
        //                    CardsList = cardsList;

        //                    CardListVisible(true);
        //                    this.pnlButtons.Visible = true;
        //                }
        //                else
        //                {
        //                    CardListVisible(false);
        //                    this.lblInfoMessage.Text = GetLocalResourceObject("NoCardsCheckedoutMessage").ToString();
        //                }
        //            }
        //            else
        //            {
        //                CardListVisible(false);
        //                this.lblErrorMessage.Text = messages;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        CardListVisible(true);
        //        this.lblCardNumber.Visible = false;
        //        this.ddlCardNumber.Visible = false;
        //    }
        //}

        //private void CardListVisible(bool visible)
        //{
        //    this.lblCardNumber.Visible = visible;
        //    this.ddlCardNumber.Visible = visible;
        //    this.lblReason.Visible = visible;
        //    this.ddlReasonForIssue.Visible = visible;
        //    this.pnlAccountNumber.Visible = visible;
        //}

        #region VALIDATIONS
        private List<string> validateAccountNumber(string AccountNumber)
        {
            List<string> validationErrors = new List<string>();

            if (String.IsNullOrEmpty(AccountNumber))
            {
                validationErrors.Add("Account number is empty. Please enter an account number.");
            }
            else if (!utility.ValidationUtility.ValidateAlphaNumericOnly(AccountNumber))
            {
                validationErrors.Add("Account number may only contains letters and numbers.");
            }
            ////TODO RAB 14Feb2014 - I don't like this. It seems like hard coded and client specific.
            //else if (AccountNumber.StartsWith("998"))
            //{
            //    validationErrors.Add("Account class 998 not allowed for issuing.");
            //}
            else if (AccountNumber.Length <= 15)
            {
                validationErrors.Add("Account number too short. Current length is " + AccountNumber.Length);
            }
            else if (AccountNumber.Length >= 20)
            {
                validationErrors.Add("Account number too long. Current length is " + AccountNumber.Length);
            }
            //else if (_customerCardIssuerService.isAccountBlocked(AccountNumber))
            //{
            //    GenerateErrorMessage("ACCOUNT NOT ALLOWED FOR CARD ISSUING (ACCOUNT IS EXCLUDED)");
            //}                       

            return validationErrors;
        }

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
        private bool validateForm()
        {
            //if (CardIssueMethodId == 1 && this.ddlCardNumber.Visible == true && int.Parse(this.ddlCardNumber.SelectedValue) < 0 && Main_Branch_UserYN )
            //    this.lblErrorMessage.Text = GetLocalResourceObject("ValidationNoCardSelected").ToString();


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

            //customer.DeliveryBranchId = ddlDelBranch.Visible ? int.Parse(this.ddlDelBranch.SelectedValue) : 0;
            customer.DeliveryBranchId = 0;
            customer.CardIssueMethodId = CardIssueMethodId;


            //if (CardIssueMethodId == 1 && Main_Branch_UserYN)
            //{
            //    customer.CardId = long.Parse(this.ddlCardNumber.SelectedValue);
            //    customer.CardNumber = this.ddlCardNumber.SelectedItem.Text;
            //}
            //else
            //{

            //    if (ddlPriority.Visible)
            //    {
            //        if (this.ddlPriority.SelectedValue != "-99")
            //        {
            //            customer.PriorityId = int.Parse(this.ddlPriority.SelectedValue);
            //        }
            //        else
            //        {
            //            customer.PriorityId = null;
            //        }
            //    }

            //}

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

            if (this.ddlFeeType.Items.Count > 0)
            {
                customer.FeeCharge = decimal.Parse(this.tbApplicableFee.Text);
                customer.Vat = decimal.Parse(this.tbVatRate.Text);
            }

            //var list = _issuerMan.GetPrintFieldsByProductid(int.Parse(this.ddlProduct.SelectedValue));
            //List<ProductField> productfieldlist = new List<ProductField>();

            //foreach (var p in list)
            //{
            //    ProductField productfield = new ProductField();
            //    productfield.ProductPrintFieldId = p.product_field_id;
            //    productfield.Name = p.field_name;
            //    productfield.MappedName = p.mapped_name;
            //    productfield.Deleted = p.deleted;
            //    productfield.Editable = p.editable;
            //    productfieldlist.Add(productfield);


            //}
            //if (customer != null)
            //    customer.ProductFields = productfieldlist.ToArray();

            customer.ProductFields = new ProductField[0];

            //Cutomer is populated from the start with print fields
            if (Customer != null && Customer.ProductFields != null)
                customer.ProductFields = Customer.ProductFields;

            return customer;
        }

        private void DisplayAllFields(bool showAll)
        {
            this.tbAccountNumber.Enabled = showAll;
            //this.ddlCardNumber.Enabled = showAll;
            this.tbNameOnCard.Enabled = showAll;
            this.tbFirstName.Enabled = showAll;
            this.tbMiddleName.Enabled = showAll;
            this.tbLastName.Enabled = showAll;
            this.tbIDNumber.Enabled = showAll;
            this.tbContactnumber.Enabled = showAll;
            this.ddlAccountType.Enabled = showAll;
            this.ddlReasonForIssue.Enabled = showAll;
            this.ddlTitle.Enabled = showAll;
            this.ddlIssuer.Enabled = showAll;
            this.ddlBranch.Enabled = showAll;
            this.ddlCurrency.Enabled = showAll;
            this.ddlCustomerType.Enabled = showAll;
            this.ddlResident.Enabled = showAll;
            this.ddlProduct.Enabled = showAll;
            this.ddlDomBranch.Enabled =
            //this.ddlDelBranch.Enabled =
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
                    this.pnlButtons.Visible = true;
                    //UpdateCardList(branchId, int.Parse(this.ddlProduct.SelectedValue));
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
                        if (_customerCardIssuerService.GetAccountDetail(issuerId,
                                    int.Parse(this.ddlProduct.SelectedValue),
                                    issuingScenario,
                                    branchId, this.tbAccountNumber.Text.Trim(), out accountDetails, out messages))
                        {
                            Customer = AccountDetailsToCustomerDetails(accountDetails);

                            DisableAccountLookup();


                            this.tbNameOnCard.Text = accountDetails.NameOnCard;

                            this.tbAccountNumber.Text = accountDetails.AccountNumber;
                            this.tbFirstName.Text = accountDetails.FirstName;
                            this.tbMiddleName.Text = accountDetails.MiddleName;
                            this.tbLastName.Text = accountDetails.LastName;
                            this.tbIDNumber.Text = accountDetails.CustomerIDNumber;
                            this.tbContactnumber.Text = accountDetails.ContractNumber;


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

        protected void dlCardslist_ItemCommand(object source, DataListCommandEventArgs e)
        {
            var cardNumber = ((LinkButton)e.CommandSource).Text;
            //tbSelectedCard.Text = cardNumber;
        }

        protected ProductField[] BuildPrintFields(int productId, ProductField[] productFields)
        {
            divPrintFields.Controls.Clear();

            ProductField[] list = new ProductField[0];

            if (Customer == null)
                Customer = new CustomerDetails();

            if (productFields == null)
                list = _issuerMan.GetPrintFieldsByProductid(productId);
            else
                list = productFields;


            foreach (var p in list)
            {
                if (p.Deleted == false && p.MappedName.ToUpper() != StaticFields.IND_SYS_NOC && p.MappedName.ToUpper() != StaticFields.IND_SYS_PAN)
                {
                    var controls = ProductFieldControl.Create(p);

                    divPrintFields.Controls.Add(controls.Item1);
                    divPrintFields.Controls.Add(controls.Item2);
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
                List<string> errormessages = validateAccountNumber(tbAccountNumber.Text);
                if (validateForm() && (errormessages.Count == 0))
                {
                    if (Customer != null)
                    {
                        var customer = Customer;
                        //TODO: images may slow down performance, maybe look at a way to cache server side.
                        //Get and save the print fields data here so that we dont loose file upload data
                        //Populates the product fields from the created text fields
                        foreach (var field in customer.ProductFields)
                        {
                            if (field.MappedName.ToUpper() == StaticFields.IND_SYS_NOC) //System default of name on card
                            {
                                field.Value = System.Text.Encoding.UTF8.GetBytes(tbNameOnCard.Text.ToUpper());
                            }
                            else if (field.Name.ToUpper() == StaticFields.IND_SYS_PAN) //System default of PAN
                            {
                                // field.Value = System.Text.Encoding.UTF8.GetBytes(System.Text.RegularExpressions.Regex.Replace(this.ddlCardNumber.SelectedItem.Text.Replace("-", ""), ".{4}", "$0 "));
                            }
                            else if (field.MappedName.ToUpper() == StaticFields.ING_NOC) //System default of name on card
                            {
                                field.Value = System.Text.Encoding.UTF8.GetBytes(ddlPassporttype.SelectedValue);
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
                else
                {
                    foreach (string s in errormessages)
                    {
                        this.lblInfoMessage.Text += s;
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

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblInfoMessage.Text = "";
                this.lblErrorMessage.Text = "";

                RolesIssuerResult issuerResult;
                if (IssuersList.TryGetValue(int.Parse(this.ddlIssuer.SelectedValue), out issuerResult))
                {
                    CustomerDetails customer;
                    if (CardId != null && CustomerId != null) //CardID should have a value only if it was set from reject
                    {
                        customer = Customer;//CreateCustomerDetails();
                        //if (CardIssueMethodId == 1 && Main_Branch_UserYN)
                        //{
                        //    customer.CardId = int.Parse(this.ddlCardNumber.SelectedValue);
                        //    customer.CardNumber = this.ddlCardNumber.SelectedItem.Text;
                        //}

                        customer.IssuerId = int.Parse(this.ddlIssuer.SelectedValue);
                        customer.BranchId = int.Parse(this.ddlBranch.SelectedValue);
                        customer.ProductId = int.Parse(this.ddlProduct.SelectedValue);

                        if (customer.AccountTypeId == null || customer.AccountTypeId.Value < 0)
                            customer.AccountTypeId = int.Parse(ddlAccountType.SelectedValue);

                        customer.CBSAccountType = hdnCBSAccounType.Value;
                        customer.CMSAccountType = hdnCMSAccounType.Value;

                        customer.DomicileBranchId = int.Parse(this.ddlDomBranch.SelectedValue);
                        //customer.DeliveryBranchId = int.Parse(this.ddlDelBranch.SelectedValue);

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
                    }
                    else
                    {
                        customer = CreateCustomerDetails();
                    }


                    string resultMessage = string.Empty;
                    bool result = false;
                    long cardId = -1;

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
                        else if (CardIssueMethodId == 1)
                        {
                            string printJobId;
                            result = _customerCardIssuerService.RequestInstantCardForCustomer(customer, out cardId, out printJobId, out resultMessage);
                            CardId = cardId;
                            PrintJobId = printJobId;

                        }


                    }


                    if (result)
                    {
                        this.lblInfoMessage.Text = resultMessage;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        this.btnCancel.Visible = this.btnCancel.Enabled = false;
                        this.btnPrint.Visible = this.btnPrint.Enabled = true;

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
            //this.ddlCardNumber.Enabled = true;
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
            //this.ddlCardNumber.Enabled =
            this.ddlProduct.Enabled =
            this.ddlDomBranch.Enabled =
            //this.ddlDelBranch.Enabled =
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

            //if (this.ddlCardNumber.Items.Count == 0)
            //{
            //    this.lblCardNumber.Visible =
            //    this.ddlCardNumber.Visible = false;
            //}
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
        private string PrintJobId
        {
            get
            {
                if (ViewState["PrintJobId"] != null)
                    return ViewState["PrintJobId"].ToString();
                else
                    return null;
            }
            set
            {
                ViewState["PrintJobId"] = value;
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

        #region Fees
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
                                                            int.Parse(this.ddlReasonForIssue.SelectedValue), string.Empty);

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
        #endregion

        #region Printing
        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        protected void btnPrint_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.btnPrint.Visible = false;
                this.btnRefresh.Visible = true;

                this.lblInfoMessage.Text = "Starting Indigo Desktop App.";

                var sessionGuid = NativeAPIController.CreateStatusSession();
                hdnGuid.Value = sessionGuid.ToString();

                var _product = _batchService.GetProduct(int.Parse(ddlProduct.SelectedValue));
                var token = NativeAPIController.CreateToken(sessionGuid,
                                                            User.ToIndigoPrincipal().IndigoIdentity,
                                                            Veneka.Indigo.UX.NativeAppAPI.Action.PrintCard,
                                                            (long)CardId, int.Parse(this.ddlBranch.SelectedValue), PrintJobId, true, _product.Product.product_bin_code);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "showPinApp", String.Format("showPinApplet('{0}','{1}');", token, 0), true);
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

        protected void btnRefresh_Click(object sender, EventArgs e)
        {

            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";
            this.btnRefresh.Visible = false;
            this.btnViewCard.Visible = true;
            try
            {
                if (!string.IsNullOrEmpty(PrintJobId))
                {
                    var result = _teminalService.GetPrintJobStatus(PrintJobId);
                    // get print job status.
                    lblErrorMessage.Text = result.comments;
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

        protected void btnViewCard_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                CardSearchResult card = new CardSearchResult();
                card.card_id = (long)CardId;

                SessionWrapper.CardSearchResultItem = card;


                Server.Transfer("~\\webpages\\card\\CardView.aspx");
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


    }
}