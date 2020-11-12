using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
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
    public partial class PINResetPOS : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PINReset));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.PIN_OPERATOR };

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly CustomerCardIssueService _cardService = new CustomerCardIssueService();
        private readonly PINManagementService _pinService = new PINManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)            
                LoadPageData();

        }

        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuerListItems = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuersList = new Dictionary<int, RolesIssuerResult>();
                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerListItems, out issuersList);

                if (issuerListItems.ContainsKey(-1))
                {
                    issuerListItems.Remove(-1);
                    issuersList.Remove(-1);
                }

                List<ListItem> listItems = new List<ListItem>();
                foreach (var issuer in issuersList.Values.Where(w => w.enable_instant_pin_YN == true))
                {
                    listItems.Add(issuerListItems[issuer.issuer_id]);
                }

                Issuers = issuersList.Where(w => w.Value.enable_instant_pin_YN == true).ToDictionary(k => k.Key, v => v.Value);

                this.ddlIssuer.Items.AddRange(listItems.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;

                if (this.ddlIssuer.Items.Count > 0)
                {
                    int issuerId = int.Parse(this.ddlIssuer.SelectedValue);
                    UpdateBranchList(issuerId);
                    //UpdateProductList(issuerId);
                }
                else
                {
                    this.lblInfoMessage.Text = "There are no issues that have instant PIN enabled.";
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

        private void UpdateBranchList(int issuerId)
        {
            if (issuerId >= 0)
            {
                this.ddlBranch.Items.Clear();
                Dictionary<int, ListItem> branchList = new Dictionary<int, ListItem>();

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
                    this.btnGenPinSessionKey.Visible = true;
                }
                else
                {
                    this.btnGenPinSessionKey.Visible = false;
                    lblErrorMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "EmptyBrancheslistMessage").ToString();
                }
            }
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
                    //UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue));
                    UpdateBranchList(issuerId);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
        protected void btnGenPinSessionKey_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = this.lblAuthInfo.Text = "Session key generated, please keep it safe.";
                this.tbPinSessionKey.Text = _pinService.GetOperatorSessionKey();
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showPinAuth", "showPinAuth();", true);
                return;           
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

        private Dictionary<int, RolesIssuerResult> Issuers
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
    }
}