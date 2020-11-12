using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.utility;
using System.Web.Security;
using System.Security.Permissions;


namespace indigoCardIssuingWeb.webpages.card
{
    public partial class RenewalCardSearch : ListPage
    {
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR };
        private static readonly ILog log = LogManager.GetLogger(typeof(CustomerCardSearch));
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly CustomerCardIssueService _cardservice = new CustomerCardIssueService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        protected void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);
                lblErrorMessage.Text = "";

                if (issuersList.ContainsKey(-1))
                {
                    issuersList.Remove(-1);
                    issuersList.Remove(-1);
                }

                try
                {
                    this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                    this.ddlIssuer.SelectedIndex = 0;

                    this.ddlPriority.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    int selectedValue;
                    var priorities = _batchService.GetCardPriorityList(out selectedValue);

                    this.ddlPriority.Items.AddRange(priorities.ToArray());
                    this.ddlPriority.SelectedIndex = 0;

                    PopulateIssueReasonDropDown();

                    if (SessionWrapper.CardSearchParams != null &&
                            SessionWrapper.CardSearchParams.GetType() == typeof(CardSearchParameters))
                    {
                        var searchParms = (CardSearchParameters)SessionWrapper.CardSearchParams;
                        SessionWrapper.CardSearchParams = null;
                        tbaccountno.Text = searchParms.AccountNumber;
                        tbcardrefno.Text = searchParms.CardNumber;
                        if (searchParms.IssuerId != null)
                        {
                            ddlIssuer.SelectedValue = searchParms.IssuerId.ToString();
                            UpdateBranchList(int.Parse(ddlIssuer.SelectedValue));
                            UpdateProductList(int.Parse(ddlIssuer.SelectedValue));
                        }

                        if (searchParms.BranchId != null)
                        {
                            ddlBranch.SelectedValue = searchParms.IssuerId.ToString();
                        }
                        if (searchParms.ProductId != null)
                        {
                            ddlProduct.SelectedValue = searchParms.IssuerId.ToString();
                        }
                        if (searchParms.PriorityId != null)
                        {
                            ddlPriority.SelectedValue = searchParms.PriorityId.ToString();
                        }

                        if (searchParms.CardIssueMethodId != null)
                        {
                            ddlCardIssueMethod.SelectedValue = searchParms.CardIssueMethodId.ToString();
                        }
                        DisplayResults(searchParms, 1, null);
                    }
                    else
                    {
                        int? issuer = null;
                        issuer = int.Parse(ddlIssuer.SelectedValue);
                        UpdateBranchList((int)issuer);
                        UpdateProductList((int)issuer);

                    }


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
                catch (Exception ex)
                {
                    this.pnlButtons.Visible = false;
                    this.pnlDisable.Visible = false;

                    log.Error(ex);
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                    if (log.IsTraceEnabled || log.IsDebugEnabled)
                    {
                        this.lblErrorMessage.Text = ex.ToString();
                    }
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

        private void PopulateIssueReasonDropDown()
        {
            //Populate reason for issue drop down.
            ddlCardIssueMethod.Items.Clear();
            ddlCardIssueMethod.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
            foreach (var cardissuemethod in _cardservice.LangLookupCardIssueMethod())
            {

                this.ddlCardIssueMethod.Items.Add(new ListItem(cardissuemethod.language_text, cardissuemethod.lookup_id.ToString()));
            }
        }
        private void UpdateProductList(int issuerId)
        {
            this.ddlProduct.Items.Clear();

            if (issuerId > 0)
            {
                List<ProductValidated> products;
                string messages;
                //if (ddlCardIssueMethod.SelectedValue != "-99")
                //{
                //    CardIssueMethodId = int.Parse(ddlCardIssueMethod.SelectedValue);
                //}
                //else
                //{
                //    CardIssueMethodId = 0;
                //}
                if (_batchService.GetProductsListValidated(issuerId, null, 1, 1000, out products, out messages))
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
                        this.ddlProduct.Items.AddRange(productsList.OrderBy(m => m.Text).ToArray());
                        this.ddlProduct.SelectedIndex = 0;
                    }

                    this.ddlProduct.Visible = true;
                    this.lblCardProduct.Visible = true;

                }
                else
                {
                    this.lblErrorMessage.Text = messages;
                    this.pnlButtons.Visible = false;
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
        private void UpdateBranchList(int issuerId)
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

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                lblErrorMessage.Text = "";
                int? issuerid, branchid = null, productid = null, priorityid = null, cardissuemethodid = null;
                issuerid = int.Parse(ddlIssuer.SelectedValue);
                if (ddlBranch.SelectedValue != "-99")
                {
                    branchid = int.Parse(ddlBranch.SelectedValue);
                }
                else
                {
                    branchid = null;
                }
                if (ddlProduct.SelectedValue != "-99")
                {
                    productid = int.Parse(ddlProduct.SelectedValue);
                }
                else
                {
                    productid = null;
                }
                if (ddlPriority.SelectedValue != "-99")
                {
                    priorityid = int.Parse(ddlPriority.SelectedValue);
                }
                else
                {
                    priorityid = null;
                }

                //if (ddlCardIssueMethod.SelectedValue != "-99")
                //{
                //    CardIssueMethodId = int.Parse(ddlCardIssueMethod.SelectedValue);
                //}
                //else
                //{
                //    CardIssueMethodId = 1;
                //}
                CardSearchParameters cardSearchParams = new CardSearchParameters(branchid, cardissuemethodid, productid, issuerid, priorityid, tbcardrefno.Text, tbaccountno.Text, PageIndex, StaticDataContainer.ROWS_PER_PAGE);
                cardSearchParams.IsRenewalSearch = true;
                SearchParameters = cardSearchParams;

                DisplayResults(cardSearchParams, 1, null);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = "";
            this.dlBatchList.DataSource = null;

            if (results == null)
            {
                results = _batchService.SearchCustomerCardsList((CardSearchParameters)parms, PageIndex).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlBatchList.DataSource = results;
                TotalPages = ((CustomercardsearchResult)results[0]).TOTAL_PAGES;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text += Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            divPaginationPanel.Visible = true;
            this.dlBatchList.DataBind();
        }

        protected void dlBatchList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlBatchList.SelectedIndex = e.Item.ItemIndex;
                string CARDID = ((Label)dlBatchList.SelectedItem.FindControl("lblcustomeraccountid")).Text;

                long _cardid;
                if (long.TryParse(CARDID, out _cardid))
                {
                    CardSearchResult cardSearchResult = new CardSearchResult();
                    cardSearchResult.card_id = _cardid;
                    SessionWrapper.CardViewMode = "Search";
                    SessionWrapper.CardSearchParams = SearchParameters;
                    SessionWrapper.CardSearchResultItem = cardSearchResult;
                    Server.Transfer(@"~\webpages\card\CardView.aspx");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion
    }
}