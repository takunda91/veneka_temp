using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.Security.Permissions;
using indigoCardIssuingWeb.SearchParameters;

namespace indigoCardIssuingWeb.webpages.dist
{
    public partial class DistributionBatchDetails : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DistributionBatchDetails));

        private readonly SystemAdminService _adminServ = new SystemAdminService();
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private UserManagementService _userMan = new UserManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN };

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                //LoadCardsInfo();
                LoadPageData();
                //PopulateBranches();    
                tbRefFrom.Enabled = false;
                tbRefTo.Enabled = false;
                rdbCardStock.Checked = true;
                //  tbRefFrom.Attributes.Add("disabled", "true");
                // tbRefTo.Attributes.Add("disabled", "true");
            }

            rdbOption1.Attributes.Add("onclick", "radioButtons(this);");
            rdbOption2.Attributes.Add("onclick", "radioButtons(this);");

            rdbCardStock.Attributes.Add("onclick", "radioButtonsCardStock(this);");
            rdbdistBatch.Attributes.Add("onclick", "radioButtonsCardStock(this);");
            //rdbOption3.Attributes.Add("onclick", "radioButtons(this);");
        }

        #region Helper Methods

        private void LoadPageData()
        {
            this.ddlIssuer.Items.Clear();
            this.ddlBranch.Items.Clear();

            try
            {
                Dictionary<int, ListItem> issuersList = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuersRoleList = new Dictionary<int, RolesIssuerResult>();
                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuersRoleList);

                //TODO: Determin which way the dist batch is being sent. CC -> Branch or Branch -> CC
                if (!String.IsNullOrWhiteSpace(Request.QueryString["direction"]))
                    BatchDirection = int.Parse(Request.QueryString["direction"]);
                //else if()

                
                //remove issuers from the drop down list who are issuers that have auto create dist batch.                
                //var manualBatchCreate = issuersRoleList.Values.Where(m => m.auto_create_dist_batch == false).ToList();
                var manualBatchCreate = issuersRoleList.Values.ToList();

                foreach (var issuer in manualBatchCreate)
                {
                    issuersList.Add(issuer.issuer_id, utility.UtilityClass.FormatListItem<int>(issuer.issuer_name, issuer.issuer_code, issuer.issuer_id));
                }

                this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;

                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && User.Identity.Name != null)
                {
                    UpdateBranchList(issuerId);
                    UpdateToBranchList(issuerId);
                    UpdateProductList(issuerId);
                    int branchId;
                    int.TryParse(this.ddlBranch.SelectedValue, out branchId);
                    //UpdateDistBatchList(issuerId);
                    
                }
                if (ddlIssuer.Items.Count == 0)
                {
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                }
                //if (ddlBranch.Items.Count == 0)
                //{
                //    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyBrancheslistMessage").ToString() + "<br/>";
                //}
                if (ddlProduct.Items.Count == 0)
                {
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyProductsListMessage").ToString() + "<br/>";
                }

                if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    pnlButtons.Visible = false;
                    lblInfoMessage.Text = "";
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SaveActionMessage").ToString();
                }
                else
                {
                    UpdateCardCount();
                    //UpdateDistBatchCardCount();
                    // lblErrorMessage.Text = "";
                    pnlButtons.Visible = true;
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
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
                    this.pnlButtons.Visible = true;
                    return true;
                }
                else
                {
                    this.pnlButtons.Visible = false;
                }
            }

            return false;
        }

        /// <summary>
        /// Populates the branch drop down list.
        /// </summary>
        /// <param name="issuerId"></param>
        private void UpdateBranchList(int issuerId)
        {
            if (issuerId >= 0)
            {
                this.ddlBranch.Items.Clear();
                Dictionary<int, ListItem> branchList = new Dictionary<int, ListItem>();

                //var branches = _userMan.GetBranchesForUser(issuerId, userRolesForPage[0], BatchDirection == 0 ? true : false);
               bool? cardcenter=null;
                if (BatchDirection == 0) cardcenter= false; else cardcenter=null;

                var branches = _userMan.GetBranchesForUserroles(issuerId, Array.ConvertAll(userRolesForPage, value => (int)value).ToList(), cardcenter);

                foreach (var item in branches)//Convert branches in item list.
                {
                    branchList.Add(item.branch_id, utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
                }

                if (branchList.Count > 0)
                {
                    ddlBranch.Items.AddRange(branchList.Values.OrderBy(m => m.Text).ToArray());
                    ddlBranch.SelectedIndex = 0;
                }
                else
                {
                    lblErrorMessage.Text += "Unable to find Branch to Distribution from.<br />";
                }
            }
        }

        private void UpdateCardCount()
        {
            if (rdbCardStock.Checked)
                UpdateCardStockCount();
            else if (rdbdistBatch.Checked)
                UpdateDistBatchList(int.Parse(ddlIssuer.SelectedValue));
            else
            {
                this.tbNumberOfCardsAvailabe.Text = "0";
                this.lblErrorMessage.Text = "Uknown card count option";
            }
        }

        private void UpdateDistBatchList(int issuerId)
        {
            if (issuerId >= 0)
            {
                this.ddldistBatch.Items.Clear();

                Dictionary<long, ListItem> distbatchlist = new Dictionary<long, ListItem>();
                int branchId, productId, cardIssueTypeId;

                int? distBatchType = null;
                int distBatchStatusId = 14;

                cardIssueTypeId = 1;
                int.TryParse(this.ddlBranch.SelectedValue, out branchId);
                int.TryParse(this.ddlProduct.SelectedValue, out productId);

                if (BatchDirection == 1)
                {
                    distBatchType = 1;
                    distBatchStatusId = 3;
                }

                var batches = _batchService.GetDistBatchesForStatus(null, issuerId, null, distBatchStatusId, branchId, cardIssueTypeId, distBatchType, null, null, true, 100, 1);

                //get any rejected batches
                batches.AddRange(_batchService.GetDistBatchesForStatus(null, issuerId, null, 8, branchId, cardIssueTypeId, distBatchType, null, null, true, 100, 1));

                foreach (var item in batches)//Convert branches in item list.
                {
                    //int count = _batchService.GetDistBatchCount(item.dist_batch_id, branchId, productId, cardIssueTypeId);

                    if (item.cards_count.HasValue && item.cards_count > 0)
                        distbatchlist.Add(item.dist_batch_id, new ListItem(item.dist_batch_reference.ToString(), item.dist_batch_id.ToString()));
                }

                if (distbatchlist.Count > 0)
                {
                    pnlButtons.Visible = true;
                    ddldistBatch.Items.AddRange(distbatchlist.Values.OrderBy(m => m.Text).ToArray());
                    ddldistBatch.SelectedIndex = 0;

                    UpdateDistBatchCardCount();
                }
                else
                {
                    pnlButtons.Visible = false;
                    lblErrorMessage.Text += "No distribution batches found to distribute from.<br />";
                }
            }
        }
        private void UpdateToBranchList(int issuerId)
        {
            if (issuerId >= 0)
            {
                this.ddlToBranch.Items.Clear();
                Dictionary<int, ListItem> branchList = new Dictionary<int, ListItem>();
                //List<BranchesResult> branches;

                var branches = _userMan.GetBranchesForIssuer(issuerId, null);

                //if (BatchDirection == 0)
                //    branches = _userMan.GetBranchesForUserroles(issuerId, Array.ConvertAll(userRolesForPage, value => (int)value).ToList(), BatchDirection == 0 ? false : true);
                //else// for re-distrubation we need to load cardcenter and branches.
                //    branches = _userMan.GetBranchesForUserroles(issuerId, Array.ConvertAll(userRolesForPage, value => (int)value).ToList(), null);
                ////var branches = _userMan.GetBranchesForUser(issuerId, userRolesForPage[0], null);

                foreach (var item in branches)//Convert branches in item list.
                {
                    branchList.Add(item.branch_id, utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
                }

                if (branchList.Count > 0)
                {
                    ddlToBranch.Items.AddRange(branchList.Values.OrderBy(m => m.Text).ToArray());
                    ddlToBranch.SelectedIndex = 0;
                }
                else
                {
                    lblErrorMessage.Text += "No branches found to distribute to.<br />";
                }
            }
        }

        /// <summary>
        /// Updates the products dropdown with the issuers products
        /// </summary>
        /// <param name="issuerId"></param>
        private void UpdateProductList(int issuerId)
        {
            this.ddlProduct.Items.Clear();

            if (issuerId > 0)
            {
                List<ProductValidated> products;
                string messages;
                if (_batchService.GetProductsListValidated(issuerId, 1, 1, 1000, out products, out messages))
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
                        this.ddlProduct.SelectedIndex = 0;
                    }
                }
            }
        }

        private void UpdateCardStockCount()
        {
            this.tbNumberOfCardsAvailabe.Text = "0";
            this.ddldistBatch.Items.Clear();

            int branchId, productId;

            if (int.TryParse(this.ddlBranch.SelectedValue, out branchId) && int.TryParse(this.ddlProduct.SelectedValue, out productId))
            {
                var cardCount = _batchService.GetBranchCardCount(branchId, productId, 1);
                this.tbNumberOfCardsAvailabe.Text = cardCount.ToString();
            }
        }

        private void UpdateDistBatchCardCount()
        {
            this.tbNumberOfCardsAvailabe.Text = "0";

            int distBatchId = int.Parse(this.ddldistBatch.SelectedValue);
            int productId = int.Parse(this.ddlProduct.SelectedValue);

            var cardCount = _batchService.GetDistBatchCardCountForRedist(distBatchId, productId);
            this.tbNumberOfCardsAvailabe.Text = cardCount.ToString();

            if (cardCount > 0)
                pnlButtons.Visible = true;
            else
                pnlButtons.Visible = false;
        }

        private void UpdatePageControls(bool enable)
        {            
            this.ddlIssuer.Enabled =
            this.ddlBranch.Enabled =
            this.ddlToBranch.Enabled =
            this.ddlProduct.Enabled =
            this.rdbOption1.Enabled =
            this.rdbOption2.Enabled = enable;

            if (enable)
            {

                if (rdbOption1.Checked)
                {
                    this.tbNumberOfCards.Enabled = enable;
                    this.tbRefFrom.Enabled =
                    this.tbRefTo.Enabled = false;
                }
                else if (rdbOption2.Checked)
                {
                    this.tbNumberOfCards.Enabled = false;
                    this.tbRefFrom.Enabled =
                    this.tbRefTo.Enabled = enable;
                }

                if (rdbdistBatch.Checked)
                    this.ddldistBatch.Enabled = enable;
            }
            else
            {
                this.ddldistBatch.Enabled =
                this.tbNumberOfCards.Enabled =
                this.tbRefFrom.Enabled =
                this.tbRefTo.Enabled = enable;
            }

            //tbRefFrom.Attributes.Add("disabled", (!enable).ToString().ToLower());
            //tbRefTo.Attributes.Add("disabled", (!enable).ToString().ToLower());

            this.btnCreateBatch.Visible = enable;
            this.btnCreateBatch.Enabled = enable;

            this.btnConfirm.Visible = enable ? false : true;
            this.btnConfirm.Enabled = enable ? false : true;

            this.btnBack.Visible = enable ? false : true;
            this.btnBack.Enabled = enable ? false : true;
        }

        #endregion

        #region Page Events

        protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue));
                UpdateToBranchList(int.Parse(this.ddlIssuer.SelectedValue));
                int branchId;
                int.TryParse(this.ddlBranch.SelectedValue, out branchId);

                UpdateCardCount();
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

        protected void ddlBranch_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {

                int branchId;
                int.TryParse(this.ddlBranch.SelectedValue, out branchId);
                UpdateCardCount();
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

        protected void ddlProduct_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {                 
                UpdateCardCount();                
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

        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        protected void btnCreateBatch_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";


            try
            {
                int cardsAtBranch = 0;
                int fromBranchId, toBranchId;

                if (!String.IsNullOrWhiteSpace(this.tbNumberOfCardsAvailabe.Text))
                    cardsAtBranch = int.Parse(this.tbNumberOfCardsAvailabe.Text);


                if (cardsAtBranch == 0)
                {
                    this.lblErrorMessage.Text = "No Cards Available";
                    return;
                }

                fromBranchId = int.Parse(ddlBranch.SelectedValue);
                toBranchId = int.Parse(ddlToBranch.SelectedValue);

                if(fromBranchId == toBranchId)
                {
                    this.lblErrorMessage.Text = "From and To Branches cannot be the same.";
                    return;
                }

                if (this.rdbOption1.Checked)
                {
                    int cardSizeForBatch = 0;

                    if (!String.IsNullOrWhiteSpace(this.tbNumberOfCards.Text))
                        cardSizeForBatch = int.Parse(this.tbNumberOfCards.Text);


                    //validate we have atleast one card.
                    if (cardSizeForBatch < 1)
                    {
                        this.lblErrorMessage.Text = GetLocalResourceObject("ValidationMinBatchSize").ToString();
                        return;
                    }
                    else if (cardSizeForBatch > cardsAtBranch)
                    {
                        this.lblErrorMessage.Text = string.Format(GetLocalResourceObject("ValidationMoreCardsThanAvailable").ToString(), new object[] { cardSizeForBatch, cardsAtBranch });
                        return;
                    }
                }
                else if (this.rdbOption2.Checked)
                {
                    if (String.IsNullOrWhiteSpace(this.tbRefFrom.Text) ||
                        String.IsNullOrWhiteSpace(this.tbRefTo.Text))
                    {
                        this.lblErrorMessage.Text = "Please fill in reference number from and to.";
                        return;
                    }
                }
                //else if (this.rdbOption3.Checked)
                //{
                //    return;
                //}


                //Move to confirm.
                UpdatePageControls(false);
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmCreate").ToString();
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

        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        protected void btnConfirm_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            //Create the new distribution batch.
            try
            {
                int branchId;
                int toBranchId;
                //int productId;
                //int? subProductId = null;
                int batchCardSize = 0;
                int createBatchOption = 0;

                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId) &&
                    int.TryParse(this.ddlToBranch.SelectedValue, out toBranchId))
                {
                    if (!String.IsNullOrWhiteSpace(this.tbNumberOfCards.Text))
                        batchCardSize = int.Parse(this.tbNumberOfCards.Text);

                    string response;
                    string dist_batch_ref;
                    int dist_batch_id;

                    if (this.rdbOption1.Checked)
                        createBatchOption = 1;
                    if (this.rdbOption2.Checked)
                        createBatchOption = 2;

                    long? fromdist_batch_id = null;
                    if (this.rdbdistBatch.Checked)
                    {
                        if (ddldistBatch.SelectedIndex > -1)
                        {
                            fromdist_batch_id = long.Parse(ddldistBatch.SelectedValue);
                        }
                    }

                    if (_batchService.CreateDistributionBatch(int.Parse(this.ddlIssuer.SelectedValue),
                                                                            branchId, toBranchId, 1,
                                                                            int.Parse(this.ddlProduct.SelectedValue),
                                                                            batchCardSize, createBatchOption,
                                                                            this.tbRefFrom.Text,
                                                                            this.tbRefTo.Text, fromdist_batch_id,
                                                                            out response, out dist_batch_id, out dist_batch_ref))
                    {

                        UpdatePageControls(true);
                        if (rdbCardStock.Checked)
                        {
                            UpdateCardStockCount();
                        }
                        else
                        {
                            UpdateDistBatchCardCount();
                        }
                        //UpdateDistBatchList(int.Parse(this.ddlIssuer.SelectedValue));
                        this.tbNumberOfCards.Text = String.Empty;
                        this.tbRefFrom.Text = String.Empty;
                        this.tbRefTo.Text = String.Empty;
                        SessionWrapper.DistBatchId = dist_batch_id;

                        if (BatchDirection != null & BatchDirection == 1)
                            this.lblInfoMessage.Text = "Distribution batch created successfully. ref :" + " <a href='DistBatchView.aspx?page=DBC1'>" + dist_batch_ref + "</a>";
                        else
                            this.lblInfoMessage.Text = "Distribution batch created successfully. ref :" + " <a href='DistBatchView.aspx?page=DBC0'>" + dist_batch_ref + "</a>";
                    }
                    else
                    {
                        this.lblErrorMessage.Text = response;
                    }
                }
                else
                {
                    this.lblInfoMessage.Text = "There seems to be an issue with the batch details, Please check details and try again.";
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

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            UpdatePageControls(true);
        }

        #endregion

        #region ViewState Properties
        private int? BatchDirection
        {
            get
            {
                if (ViewState["BatchDirection"] != null)
                    return (int)ViewState["BatchDirection"];
                else
                    return null;
            }
            set
            {
                ViewState["BatchDirection"] = value;
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

        private Dictionary<int, int> BranchCardCount
        {
            get
            {
                if (ViewState["BranchCardCount"] == null)
                    return new Dictionary<int, int>();
                else
                    return (Dictionary<int, int>)ViewState["BranchCardCount"];
            }
            set
            {
                ViewState["BranchCardCount"] = value;
            }
        }


        #endregion

        protected void ddldistBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            { 
                UpdateDistBatchCardCount();
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

        protected void rdbCardStock_CheckedChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";
            try
            {
                UpdateCardCount();
                ddldistBatch.Enabled = false;
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

        protected void rdbdistBatch_CheckedChanged(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                ddldistBatch.Enabled = true;

                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && User.Identity.Name != null)
                {
                    UpdateDistBatchList(issuerId);
                }
                else
                    lblErrorMessage.Text += "Cannot get issuer from issuers list.";
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
    }
}
