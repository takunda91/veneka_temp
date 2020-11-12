using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.Old_App_Code.service;
using System.Globalization;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class TerminalSearch : BasePage
    {
        private UserManagementService _userMan = new UserManagementService();
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";
        private static readonly ILog log = LogManager.GetLogger(typeof(TerminalList));
        private readonly TerminalManagementService _terminalService = new TerminalManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };

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

                this.ddlIssuer.Items.Clear();
                this.ddlBranch.Items.Clear();

                //Grab all the distinct issuers from the groups the user belongs to that match the role for this page.
                Dictionary<int, ListItem> issuersList = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuers = new Dictionary<int, RolesIssuerResult>();
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuersList, out issuers))
                {
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
                            UpdateBranchList(issuerId);
                        }

                        if (SessionWrapper.TerminalSearchParms != null)
                        {

                            var searchParms = SessionWrapper.TerminalSearchParms;
                            ddlIssuer.SelectedValue = searchParms.IssuerId != null ? searchParms.IssuerId.ToString() : "-99";

                            if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                            {
                                UpdateBranchList(issuerId);
                            }
                            ddlBranch.SelectedValue = searchParms.BranchId != null ? searchParms.BranchId.ToString() : "-99";
                            tbdevicenumber.Text = searchParms.DeviceId;
                            tbmodelname.Text = searchParms.Terminalname;
                            tbterminalname.Text = searchParms.TerminalModel;


                            SessionWrapper.TerminalSearchParms = null;
                            SessionWrapper.TerminalSearchParmsResult = null;
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
                        if (log.IsDebugEnabled)
                        {
                            this.lblErrorMessage.Text = ex.Message;
                        }
                    }
                }
                else
                {
                    this.pnlButtons.Visible = false;
                    this.pnlDisable.Visible = false;

                    if (!log.IsTraceEnabled)
                    {
                        log.Warn("A user tried to access a page that he/she does not have access to.");
                    }
                    log.Trace(m => m(String.Format("User {0} tried to access a page he/she does not have access to.", User.Identity.Name)));
                    lblErrorMessage.Text = Resources.DefaultExceptions.UnauthorisedPageAccessMessage;
                    return;
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            int? branchId = GetDropDownId(this.ddlBranch.SelectedValue);
            int? issuerId = GetDropDownId(this.ddlIssuer.SelectedValue);

            TerminalSearchParams parms = new TerminalSearchParams(issuerId, branchId, tbterminalname.Text, tbmodelname.Text, tbdevicenumber.Text);

            var results = _terminalService.SearchTerminal(parms,1,20);

            if (results.Count == 0)
            {
                this.lblInfoMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }
            else
            {
                //Save both results and the search params to the session. results will be displayed on next page.
                SessionWrapper.TerminalSearchParms = parms;
                SessionWrapper.TerminalSearchParmsResult = results;
                Server.Transfer("TerminalList.aspx");
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
                lblBranch.Visible = true;
                ddlBranch.Visible = true;
            }
            else
            {
                lblBranch.Visible = false;
                ddlBranch.Visible = false;
            }
        }
        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && User.Identity.Name != null)
                {
                    UpdateBranchList(issuerId);
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            this.ddlIssuer.SelectedIndex = 0;
            UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue));

           

            this.tbdevicenumber.Text = "";
            this.tbmodelname.Text = "";
            this.tbterminalname.Text = "";
                
        }
    }
}