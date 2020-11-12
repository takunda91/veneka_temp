using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using indigoCardIssuingWeb.SearchParameters;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;

namespace indigoCardIssuingWeb.webpages.card
{
    public partial class CardSearchForm : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CardSearchForm));
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly CustomerCardIssueService _cardservice = new CustomerCardIssueService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.PIN_OPERATOR,
                                                                        UserRole.AUDITOR,
                                                                        UserRole.BRANCH_PIN_OFFICER,
                                                                         UserRole.CARD_CENTRE_PIN_OFFICER

        };

        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
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

                BuildStatusDropDownList();


                if (issuersList.Count > 0)
                {
                    this.ddlIssuer.Visible = true;

                    //ddlIssuer.Items.Add(new ListItem("- ANY -", "-99"));
                    if (issuersList.ContainsKey(-1))
                        issuersList.Remove(-1);

                    ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                    this.ddlIssuer.SelectedIndex = 0;

                    if (SessionWrapper.CardSearchParams != null
                            && SessionWrapper.CardSearchParams.GetType() == typeof(CardSearchParameters))
                    {
                        var parms = (CardSearchParameters)SessionWrapper.CardSearchParams;

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

                        LoadCardIssueMethod(issuerId);
                        UpdateBranchList(issuerId);
                        UpdateProductList(issuerId);

                        if (parms.BranchId != null)
                            this.ddlBranch.SelectedValue = parms.BranchId.Value.ToString();

                        if (parms.LoadCardStatusId != null)
                            this.ddlStatus.SelectedValue = "0," + (parms.LoadCardStatusId.Value).ToString();

                        if (parms.DistCardStatusId != null)
                            this.ddlStatus.SelectedValue = "1," + (parms.DistCardStatusId.Value).ToString();

                        if (parms.BranchCardStatusId != null)
                            this.ddlStatus.SelectedValue = "2," + (parms.BranchCardStatusId.Value).ToString();

                        this.tbBatchReference.Text = parms.BatchReference ?? String.Empty;
                        this.tbCardNumber.Text = parms.CardNumber ?? String.Empty;
                        this.tbcardrefno.Text = parms.CardRefNumber ?? String.Empty;

                        if (parms.ProductId != null)
                        {
                            this.ddlProduct.SelectedValue = parms.ProductId.Value.ToString();
                            //LoadSubProducts(int.Parse(this.ddlIssuer.SelectedValue), parms.ProductId.Value);
                        }

                        if (parms.CardIssueMethodId != null)
                            this.ddlCardIssueMethod.SelectedValue = parms.CardIssueMethodId.Value.ToString();

                        if (parms.DateFrom != null)
                            this.tbDateFrom.Text = parms.DateFrom.Value.ToString(DATE_FORMAT);

                        if (parms.DateTo != null)
                            this.tbDateTo.Text = parms.DateTo.Value.ToString(DATE_FORMAT);


                        SessionWrapper.CardSearchParams = null;

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

                        //if (Issuers[issuerId].card_ref_preference)
                        //    this.tbCardNumber.Visible =
                        //        this.lblCardNumber.Visible = true;
                        //else
                        //    this.tbCardNumber.Visible =
                        //        this.lblCardNumber.Visible = false;

                        LoadCardIssueMethod(issuerId);
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

                var response = Issuers.Where(i => i.Value.allow_branches_to_search_cards == true && i.Value.issuer_id == issuerId);



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
                    //if (Issuers[issuerId].card_ref_preference)
                    //    this.tbCardNumber.Visible =
                    //        this.lblCardNumber.Visible = true;

                    LoadCardIssueMethod(issuerId);
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

            this.ddlStatus.SelectedIndex = 0;

            this.tbBatchReference.Text = "";
            this.tbCardNumber.Text = "";
            this.tbDateFrom.Text = "";
            this.tbDateTo.Text = "";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                string cardNumber = String.IsNullOrWhiteSpace(this.tbCardNumber.Text) ? null : this.tbCardNumber.Text.Trim().Replace('?', '%');
                string distBatchRef = String.IsNullOrWhiteSpace(this.tbBatchReference.Text) ? null : this.tbBatchReference.Text.Trim().Replace('?', '%');
                string cardRefNumber = String.IsNullOrWhiteSpace(this.tbcardrefno.Text) ? null : this.tbcardrefno.Text.Trim().Replace('?', '%');

                DateTime? dateToNull = null;
                DateTime? dateFromNull = null;
                DateTime dateFrom;
                //If date from is not empty, check that it's in correct format
                if (!String.IsNullOrWhiteSpace(this.tbDateFrom.Text))
                {
                    if (DateTime.TryParseExact(this.tbDateFrom.Text, DATE_FORMAT, null, DateTimeStyles.None, out dateFrom))
                    {
                        dateFromNull = dateFrom;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = "To Date not in correct format, please correct and try again.";
                        return;
                    }
                }

                DateTime dateTo;
                //If date to is not empty, check that it's in correct format
                if (!String.IsNullOrWhiteSpace(this.tbDateTo.Text))
                {
                    if (DateTime.TryParseExact(this.tbDateTo.Text, DATE_FORMAT, null, DateTimeStyles.None, out dateTo))
                    {
                        dateToNull = dateTo;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = "From Date not in correct format, please correct and try again.";
                        return;
                    }
                }


                int? loadCardStatus = null;
                int? distCardStatus = null;
                int? branchCardStatus = null;
                //Check which status has been selected. if "any" status chosed then just pass all nulls.
                string[] selectedCardStatus = this.ddlStatus.SelectedValue.Split(',');

                switch (int.Parse(selectedCardStatus[0]))
                {
                    case 0:
                        loadCardStatus = int.Parse(selectedCardStatus[1]);
                        break;
                    case 1:
                        distCardStatus = int.Parse(selectedCardStatus[1]);
                        break;
                    case 2:
                        branchCardStatus = int.Parse(selectedCardStatus[1]);
                        break;
                }


                int? userRole = null;
                //Check how many user roles the user has, if it's just one use that as user role.
                if (CurrentUserRoles.Count == 1)
                {
                    userRole = (int)CurrentUserRoles[0];
                }

                //int selectedBranchId;
                //int? branchId = null;
                //if (int.TryParse(this.ddlBranch.SelectedValue, out selectedBranchId))
                //{
                //    if (selectedBranchId > 0)
                //    {
                //        branchId = selectedBranchId;
                //    }
                //}

                //int selectedIssuerId;
                //int? issuerId = null;
                //if (int.TryParse(this.ddlIssuer.SelectedValue, out selectedIssuerId))
                //{
                //    if (selectedIssuerId > 0)
                //    {
                //        issuerId = selectedIssuerId;
                //    }
                //}

                int? branchId = GetDropDownId(this.ddlBranch.SelectedValue);
                int? issuerId = GetDropDownId(this.ddlIssuer.SelectedValue);

                int? productId = GetDropDownId(this.ddlProduct.SelectedValue);
                int? cardIssueMethodId = GetDropDownId(this.ddlCardIssueMethod.SelectedValue);
                int? priorityId = null;

                CardSearchParameters cardSearchParams = new CardSearchParameters(null, userRole, issuerId, branchId, cardNumber, null, cardRefNumber,
                                                                                 distBatchRef, loadCardStatus, distCardStatus,
                                                                                 branchCardStatus, null, dateFromNull, dateToNull, cardIssueMethodId,
                                                                                 productId, priorityId, 1, StaticDataContainer.ROWS_PER_PAGE);

                var results = _batchService.SearchForCards(cardSearchParams, 1, null);

                if (results.Count == 0)
                {
                    this.lblInfoMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                }
                else
                {
                    //Save both results and the search params to the session. results will be displayed on next page.
                    SessionWrapper.CardSearchResults = results;
                    SessionWrapper.CardSearchParams = cardSearchParams;
                    Server.Transfer("~\\webpages\\card\\CardList.aspx");
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

        private void BuildStatusDropDownList()
        {
            List<ListItem> cardStatusList = new List<ListItem>();
            cardStatusList.Add(new ListItem(Resources.ListItemLabels.ANY, "-1,-1"));

            //if (PageUtility.ValidateUserPageRole(new UserRole[] { UserRole.CENTER_MANAGER }))
            //{
            //    foreach (var status in _batchService.LangLookupLoadCardStatuses())
            //    {
            //        cardStatusList.Add(new ListItem("LOAD - " + status.language_text, "0," + status.lookup_id.ToString()));
            //    }
            //}

            //if (PageUtility.ValidateUserPageRole(new UserRole[] { UserRole.CENTER_MANAGER, 
            //                                                      UserRole.CENTER_OPERATOR, 
            //                                                      UserRole.BRANCH_CUSTODIAN }))
            //{
            //    foreach (var status in _batchService.LangLookupDistCardStatuses())
            //    {
            //        cardStatusList.Add(new ListItem("DIST - " + status.language_text, "1," + status.lookup_id.ToString()));
            //    }
            //}


            //if (PageUtility.ValidateUserPageRole(new UserRole[] { UserRole.BRANCH_CUSTODIAN, UserRole.BRANCH_OPERATOR, UserRole.PIN_OPERATOR }))
            //{
            foreach (var status in _batchService.LangLookupBranchCardStatuses())
            {
                cardStatusList.Add(new ListItem(status.language_text, "2," + status.lookup_id.ToString()));
            }
            //}

            this.ddlStatus.Items.AddRange(cardStatusList.ToArray());
            this.ddlStatus.SelectedIndex = 0;
            SessionWrapper.DateType = "DateFrom";
        }

        private void UpdateProductList(int issuerId)
        {
            this.ddlProduct.Items.Clear();

            if (issuerId > 0)
            {
                List<ProductValidated> products;
                string messages;

                int? cardIssueMethodId = null;
                if (this.ddlCardIssueMethod.Items.Count > 0 && ddlCardIssueMethod.SelectedValue != "-99")
                {
                    cardIssueMethodId = int.Parse(ddlCardIssueMethod.SelectedValue);
                }
                else
                {
                    cardIssueMethodId = CardIssueMethodId;
                }

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

        private void LoadCardIssueMethod(int issuerId)
        {
            //Populate reason for issue drop down.
            ddlCardIssueMethod.Items.Clear();
            this.ddlCardIssueMethod.Visible = false;
            this.lblCardIssueMethod.Visible = false;
            CardIssueMethodId = null;

            var selIssuer = Issuers[issuerId];

            if (selIssuer.instant_card_issue_YN && selIssuer.classic_card_issue_YN)
            {
                ddlCardIssueMethod.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
                foreach (var cardissuemethod in _cardservice.LangLookupCardIssueMethod())
                {
                    this.ddlCardIssueMethod.Items.Add(new ListItem(cardissuemethod.language_text, cardissuemethod.lookup_id.ToString()));
                }

                this.ddlCardIssueMethod.Visible = true;
                this.lblCardIssueMethod.Visible = true;
            }

            else if (selIssuer.instant_card_issue_YN)
                CardIssueMethodId = 1;
            else if (selIssuer.classic_card_issue_YN)
                CardIssueMethodId = 0;
        }

        #endregion

        #region Properites

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
