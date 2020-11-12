using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class ReprintBatchCreate : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReprintBatchCreate));
       // private BatchManagementService _batchService = new BatchManagementService();
        private PINManagementService _pinService = new PINManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_OPERATOR };

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

                this.ddlIssuer.Items.AddRange(issuerListItems.Values.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;

                if (issuerListItems.Count > 0)
                {
                    UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue));

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


            this.ddlBranch.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));

            if (branchList.Count > 0)
            {
                ddlBranch.Items.AddRange(branchList.Values.OrderBy(m => m.Text).ToArray());

            }

            ddlBranch.SelectedIndex = 0;
            DisplayResults(null, 1, null);
        }

        #region Pagination
        protected override void DisplayResults(SearchParameters.ISearchParameters parms, int pageIndex, object[] results)
        {            
            this.lblErrorMessage.Text = "";
            this.dlBatchList.DataSource = null;

            if (results == null)
            {
                //int? branchId = int.Parse(this.ddlBranch.SelectedValue);
                //if (branchId < 0)
                //    branchId = null;

                results = _pinService.PinMailerReprintList(int.Parse(ddlIssuer.SelectedValue), null, pageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlBatchList.DataSource = results;
                TotalPages = ((PinMailerReprintRequestResult)results[0]).TOTAL_PAGES;

            }
            else
            {
                TotalPages = 1;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlBatchList.SelectedIndex = -1;
            this.dlBatchList.DataBind();
        }
        #endregion

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayResults(null, 1, null);
        }

        #region Page Events
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
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
        protected void dlBatchList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            this.lblInfoMessage.Text = String.Empty;
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                dlBatchList.SelectedIndex = e.Item.ItemIndex;
                string productid = ((Label)dlBatchList.SelectedItem.FindControl("lblproduct_id")).Text;
                //string card_priority_id = ((Label)dlBatchList.SelectedItem.FindControl("lblcard_priority_id")).Text;

                this.tbProductName.Text = ((Label)dlBatchList.SelectedItem.FindControl("lblProduct")).Text;
                //this.tbPriority.Text = ((Label)dlBatchList.SelectedItem.FindControl("lblPriority")).Text;
                this.tbNumberOfCards.Text = ((Label)dlBatchList.SelectedItem.FindControl("lblnoofcards")).Text;

                ProductId = int.Parse(productid);
                //PriorityId = int.Parse(card_priority_id);

                this.lblInfoMessage.Text = "Please Confirm you wish to create a PIN re-print batch for the details above";
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

        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = String.Empty;
            this.lblErrorMessage.Text = String.Empty;

            try
            {
                int? branchid = null;

                //CardRequestBatchResponse resp;
                int pinBatchId;
                string messages;
                if (_pinService.PinMailerReprintBatchCreate(0, int.Parse(this.ddlIssuer.SelectedValue), branchid, ProductId.Value, out pinBatchId, out messages))
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