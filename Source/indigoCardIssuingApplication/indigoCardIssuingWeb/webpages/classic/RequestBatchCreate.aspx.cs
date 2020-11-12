using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Org.BouncyCastle.Bcpg.Sig;

namespace indigoCardIssuingWeb.webpages.classic
{
    public partial class RequestBatchCreate : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RequestBatchCreate));
        private BatchManagementService _batchService = new BatchManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_OPERATOR, UserRole.BRANCH_PRODUCT_OPERATOR };

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
                Dictionary<int, ListItem> issuerListItems = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuersList = new Dictionary<int, RolesIssuerResult>();

                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerListItems, out issuersList);

                //var priorities = _batchService.GetCardPriorityList();

                //List<ListItem> priorityList = new List<ListItem>();
                //int cardPrioritySelValue = 0;
                //foreach (var priority in priorities.OrderBy(m => m.card_priority_order))
                //{
                //    if (priority.default_selection)
                //        cardPrioritySelValue = priority.card_priority_id;

                //    priorityList.Add(new ListItem(priority.card_priority_name, priority.card_priority_id.ToString()));
                //}
                if (!String.IsNullOrWhiteSpace(Request.QueryString["issueMethod"]))
                {
                    cardIssueMethodId = int.Parse(Request.QueryString["issueMethod"]);
                }
                if (cardIssueMethodId == 1)
                {
                    ddlBranch.Visible = true;
                }
                else
                {
                    ddlBranch.Visible = false;
                }
                this.ddlIssuer.Items.AddRange(issuerListItems.Values.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;
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
                    UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue));

                }
                if (cardIssueMethodId == 1)
                {
                    divstock.Attributes.Add("Style", "display:block");
                }
                else
                {
                    divstock.Attributes.Add("Style", "display:none");

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
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        /// <summary>
        /// Populates the branch drop down list.
        /// </summary>
        /// <param name="issuerId"></param>
        private void UpdateBranchList(int issuerId)
        {
            this.ddlBranch.Items.Clear();
            Dictionary<int, ListItem> branchList = new Dictionary<int, ListItem>();


            var branches = _userMan.GetBranchesForUser(issuerId, userRolesForPage[0], null);

            foreach (var item in branches)//Convert branches in item list.
            {
                branchList.Add(item.branch_id, utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
            }


            //this.ddlBranch.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));

            if (branchList.Count > 0)
            {
                ddlBranch.Items.AddRange(branchList.Values.OrderBy(m => m.Text).ToArray());

            }

            ddlBranch.SelectedIndex = 0;
            GetStockInBranch();
            DisplayResults(null, 1, null);
        }

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = "";
            this.dlBatchList.DataSource = null;

            if (results == null)
            {
                int? branchId = null;
                if (cardIssueMethodId != 0 && cardIssueMethodId != 2)
                {
                    if (this.ddlBranch.SelectedIndex > -1)
                        branchId = int.Parse(this.ddlBranch.SelectedValue);
                }
                int issuerId;
                int.TryParse(this.ddlIssuer.SelectedValue, out issuerId);

                bool canRead;
                bool canUpdate;
                bool canCreate;
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_OPERATOR, issuerId, out canRead, out canUpdate, out canCreate))
                {
                    results = _batchService.GetCardRequestList(int.Parse(ddlIssuer.SelectedValue), branchId, pageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
                }
                else if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_PRODUCT_OPERATOR, issuerId, out canRead, out canUpdate, out canCreate))

                {
                    results = _batchService.GetHybridRequestList(int.Parse(ddlIssuer.SelectedValue), branchId, null, string.Empty, null, pageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
                }
            }

            if (results.Length > 0)
            {
                this.dlBatchList.DataSource = results;
                if (results[0] is card_request_result)
                    TotalPages = ((card_request_result)results[0]).TOTAL_PAGES;
                else
                    TotalPages = ((hybrid_request_result)results[0]).TOTAL_PAGES;

            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlBatchList.SelectedIndex = -1;
            this.dlBatchList.DataBind();
        }
        #endregion
        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            GetStockInBranch();
            DisplayResults(null, 1, null);
        }

        protected void GetStockInBranch()
        {
            int? branchId = null, issuerId = null, productId = null;
            if (this.ddlBranch.SelectedIndex > -1)
                branchId = int.Parse(this.ddlBranch.SelectedValue);
            if (this.ddlIssuer.SelectedIndex > -1)
                issuerId = int.Parse(this.ddlIssuer.SelectedValue);

            var result = _batchService.GetStockinBranch((int)issuerId, branchId, productId, 1);
            lblstockvalue.Text = result.ToString();
        }

        #region Page Events
        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                {
                    UpdateBranchList(issuerId);
                    //  UpdateProductList(issuerId);
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

        #endregion
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_PRODUCT_OPERATOR")]

        protected void dlBatchList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            this.lblInfoMessage.Text = String.Empty;
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                dlBatchList.SelectedIndex = e.Item.ItemIndex;
                string productid = ((Label)dlBatchList.SelectedItem.FindControl("lblproduct_id")).Text;
                string card_priority_id = ((Label)dlBatchList.SelectedItem.FindControl("lblcard_priority_id")).Text;

                this.tbProductName.Text = ((Label)dlBatchList.SelectedItem.FindControl("lblProduct")).Text;
                this.tbPriority.Text = ((Label)dlBatchList.SelectedItem.FindControl("lblPriority")).Text;
                this.tbNumberOfCards.Text = ((Label)dlBatchList.SelectedItem.FindControl("lblnoofcards")).Text;

                ProductId = int.Parse(productid);
                PriorityId = int.Parse(card_priority_id);

                this.lblInfoMessage.Text = "Please Confirm you wish to create a production batch for the details above";
                this.pnlBatchCreate.Visible = true;
                this.pnlButtons.Visible = true;
                this.pnlRequestTable.Visible = false;
                this.dlBatchList.Enabled = false;
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
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_PRODUCT_OPERATOR")]
   
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = String.Empty;
            this.lblErrorMessage.Text = String.Empty;

            try
            {

                int? branchId = null;
                if (cardIssueMethodId != 0 && cardIssueMethodId != 2)
                {
                    if (this.ddlBranch.SelectedIndex > -1)
                        branchId = int.Parse(this.ddlBranch.SelectedValue);
                }
                CardRequestBatchResponse resp;
                string messages = string.Empty;
                int issuerId;
                int.TryParse(this.ddlIssuer.SelectedValue, out issuerId);

                bool canRead;
                bool canUpdate;
                bool canCreate;
                bool status = false;
                HybridRequestBatchResponse response;
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_OPERATOR, issuerId, out canRead, out canUpdate, out canCreate))
                {
                    status = _batchService.CreateCardRequestBatch(cardIssueMethodId, int.Parse(this.ddlIssuer.SelectedValue), branchId, ProductId.Value, PriorityId.Value,
                                                            out resp, out messages);
                }
                else if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_PRODUCT_OPERATOR, issuerId, out canRead, out canUpdate, out canCreate))

                {
                    status = _batchService.CreateHybridRequestBatch(1, int.Parse(this.ddlIssuer.SelectedValue), branchId, ProductId.Value, PriorityId.Value,
                                                            out response, out messages);
                }
                if (status)
                {
                    PriorityId = null;
                    ProductId = null;

                    this.pnlButtons.Visible = false;
                    this.pnlBatchCreate.Visible = false;
                    this.pnlRequestTable.Visible = true;
                    this.dlBatchList.Enabled = true;

                    DisplayResults(null, PageIndex, null);

                    this.lblInfoMessage.Text = messages;
                }
                else
                {
                    this.lblErrorMessage.Text = messages;
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
            this.lblInfoMessage.Text = String.Empty;
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                PriorityId = null;
                ProductId = null;

                this.pnlButtons.Visible = false;
                this.pnlBatchCreate.Visible = false;
                this.pnlRequestTable.Visible = true;
                this.dlBatchList.Enabled = true;

                DisplayResults(null, PageIndex, null);
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
        private int? ProductId
        {
            get
            {
                if (ViewState["ProductId"] != null)
                    return (int)ViewState["ProductId"];
                else
                    return null;
            }
            set
            {
                ViewState["ProductId"] = value;
            }
        }
        private int cardIssueMethodId
        {
            get
            {
                if (ViewState["cardIssueMethodId"] != null)
                    return (int)ViewState["cardIssueMethodId"];
                else
                    return 0;
            }
            set
            {
                ViewState["cardIssueMethodId"] = value;
            }
        }

        private int? PriorityId
        {
            get
            {
                if (ViewState["PriorityId"] != null)
                    return (int)ViewState["PriorityId"];
                else
                    return null;
            }
            set
            {
                ViewState["PriorityId"] = value;
            }
        }
    }
}