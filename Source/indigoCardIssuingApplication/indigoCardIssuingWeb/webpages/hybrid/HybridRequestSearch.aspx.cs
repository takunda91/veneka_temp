using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.hybrid
{
    public partial class HybridRequestSearch : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HybridRequestSearch));

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly IssuerManagementService _issuerman = new IssuerManagementService();

        private readonly UserManagementService _userMan = new UserManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_PRODUCT_MANAGER,
                                                                        UserRole.BRANCH_PRODUCT_OPERATOR,
                                                                        UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR
                                                                        };

        #region Load Page
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
                BuildStatusDropDownList();

                this.ddlIssuer.Items.Clear();
                this.ddlBranch.Items.Clear();

                //Grab all the distinct issuers from the groups the user belongs to that match the role for this page.
                Dictionary<int, ListItem> issuersList = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuers = new Dictionary<int, RolesIssuerResult>();
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuersList, out issuers);

                try
                {
                    Issuers = issuers;


                    this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());

                    if (ddlIssuer.Items.FindByValue("-1") != null)
                    {
                        ddlIssuer.Items.RemoveAt(ddlIssuer.Items.IndexOf(ddlIssuer.Items.FindByValue("-1")));
                    }

                    if (ddlIssuer.Items.Count > 1)
                    {
                        ddlIssuer.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                    }
                    this.ddlIssuer.SelectedIndex = 0;

                    int issuerId;
                    if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                    {
                        UpdateIssueMethod(issuerId);
                        UpdateBranchList(issuerId);
                        UpdateProductList(issuerId);
                    }

                    if (SessionWrapper.RequestSearchParam != null)
                    {
                        RequestSearchParamters searchParms = (RequestSearchParamters)SessionWrapper.RequestSearchParam;
                        tbrequestrefno.Text = searchParms.BatchReference;
                        ddlStatus.SelectedValue = searchParms.HybridRequestStatusId != null ? (searchParms.HybridRequestStatusId.Value).ToString() : "-1";
                        ddlIssuer.SelectedValue = searchParms.IssuerId != null ? searchParms.IssuerId.ToString() : "-99";




                        if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                        {
                            UpdateIssueMethod(issuerId);
                            UpdateBranchList(issuerId);
                            UpdateProductList(issuerId);
                        }
                        ddlBranch.SelectedValue = searchParms.BranchId != null ? searchParms.BranchId.ToString() : "-99";

                        if (this.ddlIssueMethod.Items.Count > 0)
                            this.ddlIssueMethod.SelectedValue = searchParms.CardIssueMethodId != null ? searchParms.CardIssueMethodId.Value.ToString() : "-99";

                        

                        SessionWrapper.RequestSearchParam = null;
                    }
                    if (ddlIssuer.Items.Count == 0)
                    {
                        lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                    }
                    if (ddlBranch.Visible == true && ddlBranch.Items.Count == 0)
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
                    log.Error(ex);
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                    if (log.IsDebugEnabled || log.IsTraceEnabled)
                    {
                        this.lblErrorMessage.Text = ex.Message;
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
        private void UpdateProductList(int issuerId)
        {
            this.ddlProduct.Items.Clear();

            if (issuerId > 0)
            {
                List<ProductValidated> products;
                string messages;

                int? cardIssueMethodId = null;
                if (this.ddlIssueMethod.Items.Count > 0 && ddlIssueMethod.SelectedValue != "-99")
                {
                    cardIssueMethodId = int.Parse(ddlIssueMethod.SelectedValue);
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
        #endregion        

        #region Page Events
        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && User.Identity.Name != null)
                {
                    UpdateIssueMethod(issuerId);
                    UpdateBranchList(issuerId);
                    UpdateProductList(issuerId);
                }
                if (ddlBranch.Visible == true && ddlBranch.Items.Count > 0)
                {
                    lblErrorMessage.Text = "";
                    pnlButtons.Visible = true;

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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text =
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                DateTime? dateToNull = null;
                DateTime? dateFromNull = null;

                int requestBatchStatusInt=0;
                int? issuerid, branchId = null, productId = null;

                if (ddlIssuer.SelectedValue != "-99")
                {
                    issuerid = int.Parse(ddlIssuer.SelectedValue);
                }
                else
                {
                    issuerid = null;
                }
                if (ddlBranch.SelectedValue != "-99" && ddlBranch.SelectedValue != "")
                {
                    branchId = int.Parse(ddlBranch.SelectedValue);
                }
                else
                {
                    branchId = null;
                }
                if (ddlProduct.SelectedValue != "-99" && ddlProduct.SelectedValue != "")
                {
                    productId = int.Parse(ddlProduct.SelectedValue);
                }
                else
                {
                    productId = null;
                }

                //If a valid distbatches status use it, else pass null to not filter by status.
                int? requestBatchStatus = null;
                //if (requestBatchStatusInt >= 0)
                //    requestBatchStatus = requestBatchStatusInt;
                if (ddlStatus.SelectedIndex>0)
                {
                    requestBatchStatus = int.Parse(ddlStatus.SelectedValue);
                }
                else
                {
                    requestBatchStatus = null;
                }
                int? cardIssueMethod = null;
                if (this.ddlIssueMethod.Items.Count > 0 && this.ddlIssueMethod.SelectedValue != "-99")
                    cardIssueMethod = int.Parse(this.ddlIssueMethod.SelectedValue);


                string RequestBatchRef = String.IsNullOrWhiteSpace(this.tbrequestrefno.Text) ? null : this.tbrequestrefno.Text;

                RequestSearchParamters searchParms = new RequestSearchParamters(issuerid,branchId,RequestBatchRef, requestBatchStatus, cardIssueMethod, productId,  1,StaticDataContainer.ROWS_PER_PAGE);

                var results = _batchService.SearchHybridRequests((int)issuerid, branchId,productId,RequestBatchRef, requestBatchStatus, cardIssueMethod, true, searchParms.PageIndex,StaticDataContainer.ROWS_PER_PAGE);

                if (results.Count == 0)
                {
                    this.lblInfoMessage.Text = GetLocalResourceObject("InfoNoRecordsFound").ToString();
                }
                else
                {
                    SessionWrapper.HybridRequestSearchResult = results;
                    SessionWrapper.RequestSearchParam = searchParms;
                    Server.Transfer("~\\webpages\\hybrid\\HybridRequestList.aspx");
                }
            }

            catch (Exception ex)
            {
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }
    
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        this.lblErrorMessage.Text =
            this.lblInfoMessage.Text = String.Empty;

        try
        {
            ddlIssuer.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            ddlBranch.Items.Clear();
            tbrequestrefno.Text = "";
           
            int issuerId;
            if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && User.Identity.Name != null)
            {
                UpdateBranchList(issuerId);
                    UpdateProductList(issuerId);
            }
            SessionWrapper.RequestSearchParam = null;
        }
        catch (Exception ex)
        {
            this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
            if (log.IsTraceEnabled || log.IsDebugEnabled)
            {
                this.lblErrorMessage.Text = ex.ToString();
            }
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Populates Batch status drop down the items from DisttributionBatchStatus enum.
    /// </summary>
    private void BuildStatusDropDownList()
    {
        this.ddlStatus.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-1"));

        foreach (var batchStatus in _batchService.LangLookupHybridRequestStatues().OrderBy(o => o.language_text))
        {
            this.ddlStatus.Items.Add(new ListItem(batchStatus.language_text, batchStatus.lookup_id.ToString()));
        }
        this.ddlStatus.SelectedIndex = 0;
    }

    private void UpdateIssueMethod(int issuerId)
    {
        this.ddlIssueMethod.Items.Clear();
        this.ddlIssueMethod.Visible =
                this.lblIssueMethod.Visible = true;

        if (issuerId > 0)
        {
            var issuer = Issuers[issuerId];

            if (issuer.classic_card_issue_YN)
                this.ddlIssueMethod.Items.Add(new ListItem("Classic", "0"));

            if (issuer.instant_card_issue_YN)
                this.ddlIssueMethod.Items.Add(new ListItem("Instant", "1"));
        }
        else
        {
            if (Issuers.Values.Where(w => w.classic_card_issue_YN == true).Count() > 0)
                this.ddlIssueMethod.Items.Add(new ListItem("Classic", "0"));

            if (Issuers.Values.Where(w => w.instant_card_issue_YN == true).Count() > 0)
                this.ddlIssueMethod.Items.Add(new ListItem("Instant", "1"));
        }


        if (this.ddlIssueMethod.Items.Count > 1)
            this.ddlIssueMethod.Items.Insert(0, new ListItem(Resources.ListItemLabels.ANY, "-99"));
        else if (this.ddlIssueMethod.Items.Count == 0)
        {
            this.ddlIssueMethod.Visible =
                this.lblIssueMethod.Visible = false;
        }

    }

    private void UpdateBranchList(int issuerId)
    {
        this.ddlBranch.Items.Clear();

        List<int> rolesList = new List<int>();
        foreach (var role in userRolesForPage)
        {
            rolesList.Add((int)role);
        }

        var branches = _userMan.GetBranchesForUserroles(issuerId, rolesList, null);

        if (branches.Count > 0)
        {
            List<ListItem> branchList = new List<ListItem>();

            foreach (var item in branches)//Convert branches in item list.
            {
                branchList.Add(utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
            }

            if (branchList.Count > 0)
            {

                ddlBranch.Items.AddRange(branchList.OrderBy(m => m.Value).ToArray());

                if (ddlBranch.Items.Count > 1)
                {
                    this.ddlBranch.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                }
                ddlBranch.SelectedIndex = 0;
            }
        }
        if (ddlIssuer.SelectedValue != "-99")
        {
            lblBranchAllocated.Visible = true;
            ddlBranch.Visible = true;
        }
        else
        {
            lblBranchAllocated.Visible = false;
            ddlBranch.Visible = false;
        }
    }
    #endregion

    #region ViewState Properties


    public Dictionary<int, RolesIssuerResult> Issuers
    {
        get
        {
            if (ViewState["Issuers"] == null)
                return null;
            else
                return (Dictionary<int, RolesIssuerResult>)ViewState["Issuers"];
        }
        set
        {
            ViewState["Issuers"] = value;
        }
    }
    #endregion

    
}
}

