using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class instantEPinRequest : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(instantEPinRequest));
        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly DocumentManagementService _documentMan = new DocumentManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.PIN_OPERATOR };

        protected void Page_Load(object sender, EventArgs e)
        {

            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
            }

            int productId = 0;
            //if (int.TryParse(this.ddlProduct.SelectedValue, out productId))
            //{
            //    if (Customer != null)
            //        BuildPrintFields(productId, Customer.ProductFields);
            //    else
            //    {
            //        var productFields = BuildPrintFields(productId, null);
            //        Customer = new CustomerDetails { ProductFields = productFields };
            //    }
            //}
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

                if (issuerListItems.Count > 0)
                {

                    UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue), true);
                    UpdateDomicileBranchList(int.Parse(this.ddlIssuer.SelectedValue));

                    if (cardResult != null)
                        UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue), cardResult.product_id);
                    else
                        UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue), null);

                    //If we're loading from maker/checker reject
                   
                }


            } catch(Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;

                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private bool validateForm(bool validateDocuments = false)
        {
      
            if (this.tbAccountNumber.Text.Length < 7)
            {
                if (this.lblErrorMessage.Text.Length > 0)
                    this.lblErrorMessage.Text += "<br />";

                this.lblErrorMessage.Text += GetLocalResourceObject("ValidationAccountNoShort").ToString();
            }

            var combinedExpiry = this.tbExpiry.Text + this.ddlExpMonth.SelectedValue;
            int ExpiryPeriod;
            String Month = DateTime.Now.Month.ToString();
            String Year = DateTime.Now.Year.ToString();

            if(DateTime.Now.Month < 10)
            {
                Month = "0" + Month;
            }

            var combinedCurrentPeriod = Year + Month;
            var IntPeriod = int.Parse(combinedCurrentPeriod);

            if (!int.TryParse(combinedExpiry, out ExpiryPeriod))
            {
                this.lblErrorMessage.Text += "Invalid Expiry Date Structure. Should be in the format YYYYMM";
            }
            else if (IntPeriod > ExpiryPeriod)
            {
                this.lblErrorMessage.Text += "Invalid Expiry Date. Date should be greater than " + combinedCurrentPeriod;
            }



                if (this.lblErrorMessage.Text.Length > 0)
                    return false;
                else
                    return true;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
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
                        //  int issuingScenario = int.Parse(this.ddlReasonForIssue.SelectedValue);
                        int issuingScenario = 1;

                        string AccNum =  this.tbAccountNumber.Text.Trim();

                        if (_customerCardIssuerService.GetAccountDetail(issuerId,
                                    int.Parse(this.ddlProduct.SelectedValue),
                                    issuingScenario,
                                    branchId, AccNum, out accountDetails, out messages))
                        {
                            Customer = AccountDetailsToCustomerDetails(accountDetails);

                            DisableAccountLookup();


                           // this.tbNameOnCard.Text = accountDetails.NameOnCard;

                            this.tbAccountNumber.Text = accountDetails.AccountNumber;
                            this.tbFirstName.Text = accountDetails.FirstName;
                            this.tbMiddleName.Text = accountDetails.MiddleName;
                            this.tbLastName.Text = accountDetails.LastName;
                            // this.tbIDNumber.Text = accountDetails.CustomerIDNumber;
#if DEBUG
                            this.tbContactnumber.Text = "+263772222696";
                              this.tbEmailAddress.Text = "test@email.com";
#else
                            this.tbContactnumber.Text = accountDetails.ContactNumber;
                              this.tbEmailAddress.Text = accountDetails.EmailAddress;
#endif

                            //if (IsPinAccountValidation)
                            //{
                            //    ValidatedAccountPIN = AccNum.Split('|')[1];
                            //    showPinValidationCapture.Visible = false;
                            //    this.tbAccountNumber.Text = AccNum.Split('|')[0];
                            //}

                            // this.ddlTitle.Enabled = true;

                            this.tbFirstName.ReadOnly = true;
                            this.tbMiddleName.ReadOnly = true;
                            this.tbLastName.ReadOnly = true;
                            this.tbEmailAddress.ReadOnly = true;
                            this.tbContactnumber.ReadOnly = true;


                            //if (accountDetails.AccountTypeId >= 0)
                            //{
                            //    this.ddlAccountType.Enabled = false;
                            //    this.ddlAccountType.SelectedValue = accountDetails.AccountTypeId.ToString();
                            //}
                            //this.ddlCurrency.Enabled = false;
                            //this.ddlCurrency.SelectedValue = accountDetails.CurrencyId.ToString();
                            //this.ddlResident.Enabled = true;
                            //this.ddlCustomerType.Enabled = true;

                            //this.tbCustomerID.Text = accountDetails.CmsID;
                            //if (String.IsNullOrWhiteSpace(accountDetails.CmsID))
                            //    this.tbCustomerID.Enabled = true;
                            //else
                            //    this.tbCustomerID.Enabled = false;

                            //this.tbInternalAccountNo.Text = accountDetails.CustomerId;

                            //this.tbNameOnCard.ReadOnly = true;  

                            this.pnlButtons.Visible = true;

                            //Depending on issuing scenario and if cards in list, display them
                            //If the card list is empty we assume the integration worked correctly but might not return cards for display
                            //Therefor is response was true the assume the call to integration account lookups is successful.
                            //if (issuingScenario > 1 && accountDetails.CMSCards != null && accountDetails.CMSCards.Length > 0)
                            //{
                            //    dlCardslist.DataSource = accountDetails.CMSCards;
                            //    dlCardslist.DataBind();
                            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "showCardDialog", "showCardDialog();", true);
                            //}


                            //LoadFeeDetails();
                            //BuildPrintFields(int.Parse(this.ddlProduct.SelectedValue), accountDetails.ProductFields);

                            //LoadRemoteDocuments();

                            //foreach (ProductField c in accountDetails.ProductFields)
                            //{
                            //    if (divPrintFields.FindControl("lbl_" + c.ProductPrintFieldId) != null)
                            //    {



                            //        switch (c.MappedName.ToUpper().Trim())
                            //        {
                            //            case "ADDR1":
                            //                ((TextBox)divPrintFields.FindControl("tb_" + c.ProductPrintFieldId)).Text = accountDetails.Address1;
                            //                break;
                            //            case "ADDR2":
                            //                ((TextBox)divPrintFields.FindControl("tb_" + c.ProductPrintFieldId)).Text = accountDetails.Address2;
                            //                break;
                            //            case "ADDR3":
                            //                ((TextBox)divPrintFields.FindControl("tb_" + c.ProductPrintFieldId)).Text = accountDetails.Address3;
                            //                break;
                            //        }

                            //    }
                            //}
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

        private void DisableAccountLookup()
        {
            AccountLookupControls(false);
        }

        private void AccountLookupControls(bool enable)
        {
            this.ddlIssuer.Enabled =
            this.ddlBranch.Enabled =
          //  this.ddlCardNumber.Enabled =
            this.ddlProduct.Enabled =
            this.ddlDomBranch.Enabled =
           // this.ddlDelBranch.Enabled =
           // this.ddlReasonForIssue.Enabled =
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

           // DisplayDocumentsGrid();
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
                    //ddlDelBranch.SelectedValue = ddlDomBranch.SelectedValue = this.ddlBranch.SelectedValue;
                }
            }
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
                    //if (branchList.Count > 0)//If there's only one branch then fetch cards.
                    //{
                    //    if (this.ddlProduct.Items.Count > 0)
                    //        UpdateCardList(int.Parse(this.ddlBranch.SelectedValue), int.Parse(this.ddlProduct.SelectedValue));

                    //    //TODO: Hide branch ddl if only one branch?
                    //}
                }
                else
                {

                    lblErrorMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "EmptyBrancheslistMessage").ToString();
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
                 //   DisplayDocumentsGrid();
                }
            }
        }


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
        //    ddlGender.Items.Clear();
        //    var genderList = _customerCardIssuerService.LangLookupGenderTypes();
        //    this.ddlAccountType.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));

        //    //Populate account type drop down.
        //    List<ListItem> genders = new List<ListItem>();
        //    foreach (var item in genderList)
        //    {
        //        genders.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
        //    }
        //    this.ddlGender.Items.AddRange(genders.OrderBy(m => m.Text).ToArray());
        }


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
                   // UpdateCardList(branchId, int.Parse(this.ddlProduct.SelectedValue));
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



#endregion

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

        private PinObject PinDetails
        {
            get
            {
                if (ViewState["PinDetails"] != null)
                    return (PinObject)ViewState["PinDetails"];
                else
                    return null; //new PinDetailsDetails { ProductFields = new ProductField[0] };
            }
            set
            {
                ViewState["PinDetails"] = value;
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

        protected void btnValidatePan_Click(object sender, EventArgs e)
        {

        }




        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                //Pass all validations, set to confirm.
                if (validateForm(true))
                {                   
                    ConfirmSave = true;
                    DisplayAllFields(false);
                    this.btnCancel.Enabled = this.btnCancel.Visible = true;
                    this.lblInfoMessage.Text = "";
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

        private void DisplayAllFields(bool showAll)
        {
            this.tbAccountNumber.Enabled = showAll;
           // this.ddlCardNumber.Enabled = showAll;
           // this.tbNameOnCard.Enabled = showAll;
            this.tbFirstName.Enabled = showAll;
            this.tbMiddleName.Enabled = showAll;
            this.tbLastName.Enabled = showAll;
          //  this.tbIDNumber.Enabled = showAll;
            this.tbContactnumber.Enabled = showAll;
          //  this.ddlAccountType.Enabled = showAll;
           // this.ddlReasonForIssue.Enabled = showAll;
           // this.ddlTitle.Enabled = showAll;
            this.tbEmailAddress.Enabled = showAll;
            this.ddlIssuer.Enabled = showAll;
            this.ddlBranch.Enabled = showAll;
          //  this.ddlCurrency.Enabled = showAll;
          //  this.ddlCustomerType.Enabled = showAll;
           // this.ddlResident.Enabled = showAll;
            this.ddlProduct.Enabled = showAll;
            this.ddlDomBranch.Enabled =
           // this.ddlDelBranch.Enabled =
            //this.ddlFeeType.Enabled =
           // this.tbCustomerID.Enabled =
            //this.reqCustomerID.Enabled =
           // this.tbContractNumber.Enabled =
           // this.chkOverrideFee.Enabled =
           // this.chkWaiveFee.Enabled =
            //this.reqContractNumber.Enabled = 
            this.pnlCustomerDetail.Enabled = showAll;

           // if (CardIssueMethodId == 0 || (CardIssueMethodId == 1 && Satellite_Branch_UserYN))
            //    this.ddlPriority.Enabled = showAll;

            this.btnSave.Visible = showAll;
            this.btnConfirm.Visible = showAll ? false : true;
           // btnDocumentLocal.Visible = showAll;
           // btnDocumentRemote.Visible = showAll;

           // DisplayDocumentsGrid();
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {

            try
            {
                this.lblInfoMessage.Text = "";
                this.lblErrorMessage.Text = "";

                var quest_status = int.Parse(Request.QueryString["status"]);
                string request_type = null;
                if (!String.IsNullOrWhiteSpace(quest_status.ToString()))
                {
                    if (quest_status == 0)
                    {
                        request_type = "NewIssue";
                    }
                    else if (quest_status == 1)
                    {
                        request_type = "ReIssue";
                    }
                    
                }
             

                var combinedExpiry = this.tbExpiry.Text + this.ddlExpMonth.SelectedValue;
                int ExpiryPeriod;
                int productId;
                string resultMessage;
                bool result;
                if (int.TryParse(combinedExpiry, out ExpiryPeriod))
                {
                    productId = int.Parse(this.ddlProduct.SelectedValue);
                    var product = _batchService.GetProduct(productId);

                    PinObject newPinDetails = new PinObject();
                    newPinDetails.IssuerId = int.Parse(this.ddlIssuer.SelectedValue);
                    newPinDetails.BranchId = int.Parse(this.ddlBranch.SelectedValue);
                    newPinDetails.DomBranchId = int.Parse(this.ddlDomBranch.SelectedValue);
                    newPinDetails.ProductId = int.Parse(this.ddlProduct.SelectedValue);
                    newPinDetails.LastFourDigitsOfPan = this.tbPan.Text;
                    newPinDetails.PinRequestStatus = "NewRequest";
                    newPinDetails.ExpiryPeriod = ExpiryPeriod;
                    newPinDetails.CustomerAccountNumber = this.tbAccountNumber.Text;
                    newPinDetails.CustomerContact = this.tbContactnumber.Text;
                    newPinDetails.CustomerEmail = this.tbEmailAddress.Text;
                    newPinDetails.ProductBin = product.Product.product_bin_code;
                    newPinDetails.PinRequestReference = String.Empty;
                    newPinDetails.PinRequestType = request_type;
                    newPinDetails.Channel = (this.rbEMAIL.Checked == true) ? "EMAIL" : (this.rbSMS.Checked == true) ? "SMS" : (this.rbUSSD.Checked == true) ? "USSD" : "SMS";

                    result = _customerCardIssuerService.createPinRequest(newPinDetails, out resultMessage);

                    if (result)
                    {
                        ConfirmSave = false;
                        this.lblInfoMessage.Text = resultMessage;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        this.btnCancel.Visible = this.btnCancel.Enabled = false;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = resultMessage;
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";
           
           if (ConfirmSave) //Cancelling confirm but we did use account lookup
            {
                //TODO: Enable controls
                ConfirmSave = false;
                DisplayAllFields(true);
                DisableAccountLookup();
            }
            else if (!ConfirmSave) //Cancel save and go back to account lookup
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
            this.ddlProduct.Enabled = true;
            this.tbFirstName.Text = String.Empty;
            this.tbLastName.Text = String.Empty;
            this.tbMiddleName.Text = String.Empty;
            this.tbContactnumber.Text = String.Empty;
            this.ddlIssuer.Enabled = true;
            this.ddlBranch.Enabled = true;
            this.tbAccountNumber.Enabled = true;
            this.tbAccountNumber.ReadOnly = false;
            this.btnCancel.Visible = false;    

        
        }

        private void EnableAccountLookup()
        {
            AccountLookupControls(true);
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
          //  hdnCMSAccounType.Value = accountDetails.CMSAccountTypeId;
           // hdnCBSAccounType.Value = accountDetails.CBSAccountTypeId;
            return rtn;
        }
    }


}