using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class PinSendSearch : BasePage
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(PinSendSearch));
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly CustomerCardIssueService _cardservice = new CustomerCardIssueService();
        private readonly PINManagementService _pinService = new PINManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { //UserRole.BRANCH_OPERATOR,
                                                                        //UserRole.BRANCH_CUSTODIAN,
                                                                        //UserRole.CENTER_MANAGER,
                                                                        //UserRole.CENTER_OPERATOR,
                                                                        UserRole.PIN_OPERATOR,
                                                                        //UserRole.AUDITOR,
                                                                        //UserRole.BRANCH_PIN_OFFICER,
                                                                        // UserRole.CARD_CENTRE_PIN_OFFICER

        };

        #region LOAD PAGE
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private void LoadPageData()
        {
            try
            {
                List<UserRole> currentUserRoles = new List<UserRole>();
                Dictionary<int, ListItem> issuersList = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuers = new Dictionary<int, RolesIssuerResult>();
                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuersList, out issuers);

                Issuers = issuers;
                CurrentUserRoles = currentUserRoles;

                if (issuersList.Count > 0)
                {
                    this.ddlIssuer.Visible = true;

                    //ddlIssuer.Items.Add(new ListItem("- ANY -", "-99"));
                    if (issuersList.ContainsKey(-1))
                        issuersList.Remove(-1);

                    ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                    this.ddlIssuer.SelectedIndex = 0;

                    if (SessionWrapper.PinRequestSearchParams != null
                            && SessionWrapper.PinRequestSearchParams.GetType() == typeof(PinRequestSearchParameters))
                    {
                        var parms = (PinRequestSearchParameters)SessionWrapper.PinRequestSearchParams;

                        if (parms.IssuerId != null)
                        {
                            ddlIssuer.SelectedValue = parms.IssuerId.Value.ToString();
                        }

                        int issuerId = int.Parse(this.ddlIssuer.SelectedValue);

                        //if (Issuers[issuerId].card_ref_preference)
                        //    this.tbCardNumber.Visible =
                        //        this.lblCardNumber.Visible = true;
                        //else
                        //    this.tbCardNumber.Visible =
                        //        this.lblCardNumber.Visible = false;

                        //LoadCardIssueMethod(issuerId);
                        UpdateBranchList(issuerId);
                        UpdateProductList(issuerId);

                        if (parms.BranchId != null)
                            this.ddlBranch.SelectedValue = parms.BranchId.Value.ToString();

                        this.tbrequestrefno.Text = parms.PinRequestReference ?? String.Empty;
                        this.tbCardNumber.Text = parms.ProductBin ?? String.Empty;
                        this.tbLastFourDigits.Text = parms.LastFourDigits ?? String.Empty;
                        this.tbCustomerAccount.Text = parms.PinCustomerAccount ?? String.Empty;

                        if (parms.ProductId != null)
                        {
                            this.ddlProduct.SelectedValue = parms.ProductId.Value.ToString();
                            //LoadSubProducts(int.Parse(this.ddlIssuer.SelectedValue), parms.ProductId.Value);
                        }

                        SessionWrapper.PinRequestSearchParams = null;

                        if (ddlIssuer.Items.Count == 0)
                        {
                            lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                        }
                        if (ddlBranch.Items.Count == 0)
                        {
                            lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyBrancheslistMessage").ToString() + "<br/>";
                        }
                        if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                        {
                            pnlButtons.Visible = false;
                            lblInfoMessage.Text = "";
                            lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SearchActionMessage").ToString();
                        }
                        else
                        {
                            // lblErrorMessage.Text = "";
                            pnlButtons.Visible = true;
                        }
                    }
                    else
                    {
                        int issuerId = int.Parse(this.ddlIssuer.SelectedValue);

                        // LoadCardIssueMethod(issuerId);
                        UpdateBranchList(issuerId);
                        UpdateProductList(issuerId);
                    }
                }
                else
                {
                    this.ddlIssuer.Visible = false;
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private void UpdateBranchList(int issuerId)
        {
            this.ddlBranch.Items.Clear();
            if (issuerId != -99)
            {
                Dictionary<int, ListItem> branchList = new Dictionary<int, ListItem>();

                var response = Issuers.Where(i => i.Value.issuer_id == issuerId);



                if (response != null && response.Count() > 0)
                {
                    var branches = _userMan.GetBranchesForIssuer(issuerId, null);

                    if (branches.Count > 1)
                    {
                        this.ddlBranch.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    }

                    foreach (var result in branches)
                    {
                        if (!branchList.ContainsKey(result.branch_id))
                        {
                            branchList.Add(result.branch_id, utility.UtilityClass.FormatListItem<int>(result.branch_name, result.branch_code, result.branch_id));
                        }
                    }

                }
                else
                {
                    var results = _userMan.GetBranchesForUser(issuerId, null, null);

                    if (results.Count > 1)
                    {
                        this.ddlBranch.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    }

                    foreach (var result in results)
                    {
                        if (!branchList.ContainsKey(result.branch_id))
                        {
                            branchList.Add(result.branch_id, utility.UtilityClass.FormatListItem<int>(result.branch_name, result.branch_code, result.branch_id));
                        }
                    }
                }


                this.ddlBranch.Items.AddRange(branchList.Values.OrderBy(m => m.Text).ToArray());
            }
        }

        #endregion

        #region BUTTON CLICK METHODS
        protected void ddlCardIssueMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            int issuerId;
            int.TryParse(ddlIssuer.SelectedValue, out issuerId);
            UpdateProductList(issuerId);
        }
        protected void ddlProduct_SelectedIndexChanged(object sender, EventArgs e)
        {

            int issuerId;
            int product_id;
            if (int.TryParse(ddlIssuer.SelectedValue, out issuerId) &&
                int.TryParse(ddlProduct.SelectedValue, out product_id))
            {
                //LoadSubProducts(issuerId, product_id);
            }
        }

        protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.ddlBranch.Items.Clear();
                this.tbCardNumber.Visible =
                            this.lblCardNumber.Visible = false;

                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                {

                    UpdateBranchList(issuerId);
                    UpdateProductList(issuerId);
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

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            this.ddlIssuer.SelectedIndex = 0;
            UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue));


            this.tbrequestrefno.Text = "";
            this.tbCardNumber.Text = "";
            this.tbLastFourDigits.Text = "";
            this.tbCustomerAccount.Text = "";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                string productBin = String.IsNullOrWhiteSpace(this.tbCardNumber.Text) ? null : this.tbCardNumber.Text.Trim().Replace('?', '%');
                string lastFourDigits = String.IsNullOrWhiteSpace(this.tbLastFourDigits.Text) ? null : this.tbLastFourDigits.Text.Trim().Replace('?', '%');
                string request_reference = String.IsNullOrWhiteSpace(this.tbrequestrefno.Text) ? null : this.tbrequestrefno.Text.Trim().Replace('?', '%');
                string customer_account = String.IsNullOrWhiteSpace(this.tbCustomerAccount.Text) ? null : this.tbCustomerAccount.Text.Trim().Replace('?', '%');



                int? userRole = null;
                //Check how many user roles the user has, if it's just one use that as user role.
                if (userRolesForPage.Length == 1)
                {
                    userRole = (int)userRolesForPage[0];
                }



                int? branchId = GetDropDownId(this.ddlBranch.SelectedValue);
                int? issuerId = GetDropDownId(this.ddlIssuer.SelectedValue);

                int? productId = GetDropDownId(this.ddlProduct.SelectedValue);
                int? cardIssueMethodId = 0;
                int? priorityId = null;

                PinRequestSearchParameters pinSearchParms = new PinRequestSearchParameters
                {
                    IssuerId = issuerId,
                    BranchId = branchId,
                    ProductId = productId,
                    ProductBin = productBin,
                    LastFourDigits = lastFourDigits,
                    PinCustomerAccount = customer_account,
                    PinRequestReference = request_reference,
                    PageIndex = 1,
                    RowsPerPage = StaticDataContainer.ROWS_PER_PAGE
                };

                var results = _pinService.SearchForPinReIssue(pinSearchParms);

                if (results.Count == 0)
                {
                    this.lblInfoMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                }
                else
                {
                    //Save both results and the search params to the session. results will be displayed on next page.
                    SessionWrapper.PinRequestSearchResult = results;
                    SessionWrapper.PinRequestSearchParams = pinSearchParms;
                    //Server.Transfer("~\\webpages\\pin\\PinReIssueList.aspx");
                    Response.Redirect("~\\webpages\\pin\\PinReIssueList.aspx");
                  
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

        private int? GetDropDownId(string value)
        {
            int selectedId;
            int? Id = null;
            if (int.TryParse(value, out selectedId))
            {
                if (selectedId > 0)
                {
                    Id = selectedId;
                }
            }

            return Id;
        }
        #endregion

        #region PRIVATE METHODS


        private void UpdateProductList(int issuerId)
        {
            this.ddlProduct.Items.Clear();

            if (issuerId > 0)
            {
                List<ProductValidated> products;
                string messages;

                int? cardIssueMethodId = null;
                //if (this.ddlCardIssueMethod.Items.Count > 0 && ddlCardIssueMethod.SelectedValue != "-99")
                //{
                //    cardIssueMethodId = int.Parse(ddlCardIssueMethod.SelectedValue);
                //}
                //else
                //{
                //    cardIssueMethodId = CardIssueMethodId;
                //}

                cardIssueMethodId = 0;

                if (_batchService.GetProductsListValidated(issuerId, cardIssueMethodId, 1, 1000, out products, out messages))
                {
                    List<ListItem> productsList = new List<ListItem>();
                    Dictionary<int, ProductValidated> productDict = new Dictionary<int, ProductValidated>();

                    foreach (var product in products)
                    {
                        if (!productDict.ContainsKey(product.ProductId))
                            productDict.Add(product.ProductId, product);

                        productsList.Add(utility.UtilityClass.FormatListItem<int>(product.ProductName, product.ProductCode, product.ProductId));
                    }

                    if (productsList.Count > 0)
                    {
                        this.ddlProduct.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
                        this.ddlProduct.Items.AddRange(productsList.OrderBy(m => m.Text).ToArray());
                        this.ddlProduct.SelectedIndex = 0;
                        this.ddlProduct.Visible = true;
                        this.lblCardProduct.Visible = true;
                        this.pnlButtons.Visible = true;

                    }
                }
                else
                {
                    this.lblErrorMessage.Text = messages;
                    this.pnlButtons.Visible = false;
                }
            }
        }


        #endregion


        #region CLASS PROPERTIES
        private int? CardIssueMethodId
        {
            get
            {
                if (ViewState["CardIssueMethodId"] != null)
                    return (int?)ViewState["CardIssueMethodId"];
                else
                    return 0;
            }
            set
            {
                ViewState["CardIssueMethodId"] = value;
            }
        }

        /// <summary>
        /// Roles currently assigned to the user.
        /// </summary>
        public List<UserRole> CurrentUserRoles
        {
            get
            {
                if (ViewState["CurrentUserRoles"] != null)
                {
                    return (List<UserRole>)ViewState["CurrentUserRoles"];
                }
                else
                {
                    return new List<UserRole>();
                }
            }
            set
            {
                ViewState["CurrentUserRoles"] = value;
            }
        }

        public Dictionary<int, RolesIssuerResult> Issuers
        {
            get
            {
                if (ViewState["Issuers"] != null)
                {
                    return (Dictionary<int, RolesIssuerResult>)ViewState["Issuers"];
                }
                else
                {
                    return new Dictionary<int, RolesIssuerResult>();
                }
            }
            set
            {
                ViewState["Issuers"] = value;
            }
        }
        #endregion
    }
}