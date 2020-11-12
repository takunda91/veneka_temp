using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.service;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class TerminalManager : BasePage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";
        private static readonly ILog log = LogManager.GetLogger(typeof(TerminalManager));
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly TerminalManagementService _terminalService = new TerminalManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };

        #region Page Load

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this.ddlIssuer.Items.Count == 0)
            {
                this.lblIssuer.Visible = this.ddlIssuer.Visible = this.rfvIssuer.Visible = false;
            }
            else
            {
                this.lblIssuer.Visible = this.ddlIssuer.Visible = this.rfvIssuer.Visible = true;
            }

            if (this.ddlBranch.Items.Count == 0)
            {
                this.lblBranch.Visible = this.ddlBranch.Visible = this.rfvBranch.Visible = false;
            }
            else
            {
                this.lblBranch.Visible = this.ddlBranch.Visible = this.rfvBranch.Visible = true;
            }

            if (this.ddlTerminalMasterKey.Items.Count == 0)
            {
                this.lblMasterKey.Visible = this.ddlTerminalMasterKey.Visible = this.rfvTerminalMasterKey.Visible = false;
            }
            else
            {
                this.lblMasterKey.Visible = this.ddlTerminalMasterKey.Visible = this.rfvTerminalMasterKey.Visible = true;

            }
        }

        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);

                if (CurrentPageLayout != null)
                {
                    pageLayout = CurrentPageLayout.Value;
                }

                if (!IsPostBack)
                {
                    LoadPageData();
                    PopulateFields();
                    UpdateFormLayout(pageLayout);
                }
                tbPassword.Attributes.Add("value", Password);
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

        #endregion

        #region Page Events

        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                int issuerId = int.Parse(ddlIssuer.SelectedValue);

                PopulateIssuerFields(issuerId);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                this.btnEdit.Enabled = this.btnEdit.Visible = false;
                tbPassword.Attributes.Add("value", Password);
                UpdateFormLayout(PageLayout.UPDATE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {

                UpdateFormLayout(PageLayout.CONFIRM_DELETE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (CurrentPageLayout != null)
                {
                    pageLayout = CurrentPageLayout.Value;
                }
                switch (pageLayout)
                {
                    case PageLayout.CONFIRM_CREATE:
                        if (CreateTerminal())
                        {
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessCreate").ToString();

                        }
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        if (DeleteTerminal())
                            UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        if (UpdateTerminal())
                        {
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessUpdate").ToString();
                        }
                        UpdateFormLayout(pageLayout);
                        break;

                    default:
                        break;
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            switch (CurrentPageLayout)
            {
                case PageLayout.READ:
                case PageLayout.CREATE:
                case PageLayout.DELETE:
                    if (TerminalSearchParms != null)
                        SessionWrapper.TerminalSearchParms = TerminalSearchParms;
                    Server.Transfer("~\\webpages\\system\\TerminalList.aspx");
                    break;
                case PageLayout.UPDATE:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    UpdateFormLayout(PageLayout.UPDATE);
                    break;
                case PageLayout.CONFIRM_CREATE:
                    UpdateFormLayout(PageLayout.CREATE);
                    break;
                case PageLayout.CONFIRM_DELETE:
                    UpdateFormLayout(PageLayout.DELETE);
                    break;
                //case PageLayout.DELETE:
                //    UpdateFormLayout(PageLayout.READ);
                //    break;
                default:
                    break;
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            Password = tbPassword.Text;
            try
            {
                if (Validation())
                {
                    UpdateFormLayout(PageLayout.CONFIRM_CREATE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (Validation())
                {
                    UpdateFormLayout(PageLayout.CONFIRM_UPDATE);
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

        #region Page Flow Methods
        private bool DeleteTerminal()
        {
            try
            {
                var response = _terminalService.DeleteTerminal(TerminalId);
                if (String.IsNullOrWhiteSpace(response))
                {

                    Server.Transfer("~\\webpages\\system\\TerminalList.aspx?delete=" + this.ddlIssuer.SelectedValue);
                    //this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessDelete").ToString();
                    //pageLayout = PageLayout.CREATE;
                    //TerminalId = 0;
                    //this.txtDeviceId.Text = "";
                    //this.txtTerminalModel.Text = "";
                    //this.txtTerminalName.Text = "";
                    //ddlBranch.SelectedIndex = -1;
                    //ddlIssuer.SelectedIndex = -1;
                    //this.ddlBranch.SelectedValue="-99";
                    //this.ddlIssuer.SelectedValue = "-99";

                    //return true;

                }
                else
                {
                    this.lblInfoMessage.Text = string.Empty;
                    this.lblErrorMessage.Text = response;
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

            return false;
        }
        private void PopulateIssuerFields(int issuerId)
        {
            ddlBranch.Items.Clear();
            ddlTerminalMasterKey.Items.Clear();

            var branches = _userMan.GetBranchesForIssuer(issuerId, null);
            var masterkeys = _terminalService.GetTMKByIssuer(issuerId, 1, StaticDataContainer.ROWS_PER_PAGE);

            List<ListItem> masterkeyList = new List<ListItem>();
            foreach (var item in masterkeys)
            {
                masterkeyList.Add(new ListItem(item.masterkey_name, item.masterkey_id.ToString()));
            }

            List<ListItem> branchList = new List<ListItem>();
            foreach (var item in branches)
            {
                branchList.Add(utility.UtilityClass.FormatListItem(item.branch_name, item.branch_code, item.branch_id));
            }

            this.ddlBranch.Items.AddRange(branchList.OrderBy(b => b.Text).ToArray());
            this.ddlTerminalMasterKey.Items.AddRange(masterkeyList.OrderBy(b => b.Text).ToArray());

            ddlBranch.Visible = ddlTerminalMasterKey.Visible = true;
        }

        private void PopulateFields()
        {
            try
            {
                if (SessionWrapper.TerminalId != null)
                {
                    TerminalId = SessionWrapper.TerminalId;
                    SessionWrapper.TerminalId = 0;


                    if (TerminalId != 0)
                    {

                        var terminal = _terminalService.GetTerminals((int)TerminalId, 1, 1);
                        ddlIssuer.SelectedValue = terminal.issuer_id.ToString();
                        int issuerId = int.Parse(ddlIssuer.SelectedValue);
                        PopulateIssuerFields(issuerId);
                        ddlBranch.SelectedValue = terminal.branch_id.ToString();
                        ddlTerminalMasterKey.SelectedValue = terminal.masterkey_id.ToString();

                        txtDeviceId.Text = terminal.device_Id;
                        txtTerminalModel.Text = terminal.terminal_model;
                        txtTerminalName.Text = terminal.terminal_name;
                        if (terminal.password != null)
                        {
                            tbPassword.Text = terminal.password;
                            Password = terminal.password;
                        }
                        else
                        {
                            Password = null;

                        }
                        if (terminal.IsMacUsed != null)
                        {
                            chkMac.Checked = (bool)terminal.IsMacUsed;
                        }
                        tbPassword.Attributes.Add("value", Password);
                        
                        pageLayout = PageLayout.READ;

                    }
                    else
                    {
                        //No username in session, set page layout to create.
                        pageLayout = PageLayout.CREATE;
                        Password = null;
                    }
                }
                else
                {
                    //No username in session, set page layout to create.
                    pageLayout = PageLayout.CREATE;
                    Password = null;
                }
                CurrentPageLayout = pageLayout;
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

        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                if (issuersList.ContainsKey(-1))
                    issuersList.Remove(-1);

                ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                ddlIssuer.SelectedIndex = 0;

                PopulateIssuerFields(Convert.ToInt32(ddlIssuer.SelectedValue));

                if (ddlIssuer.Items.Count == 0)
                {
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                }
                if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    pnlButtons.Visible = false;
                    lblInfoMessage.Text = "";
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SaveActionMessage").ToString();
                }
                else
                {
                    // lblErrorMessage.Text = "";
                    pnlButtons.Visible = true;
                }

                UpdateFormLayout(pageLayout);

                if (SessionWrapper.TerminalSearchParms != null)
                {
                    TerminalSearchParms = SessionWrapper.TerminalSearchParms;
                    SessionWrapper.TerminalSearchParms = null;
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
        private string Password
        {
            get
            {
                if (ViewState["Password"] != null)
                {
                    return ViewState["Password"].ToString();
                }
                return null;
            }

            set
            {
                ViewState["Password"] = value;
            }
        }
        private void UpdateFormLayout(PageLayout? toPageLayout)
        {
            if (toPageLayout == null)
            {
                if (CurrentPageLayout != null)
                {
                    pageLayout = CurrentPageLayout.Value;
                }
                toPageLayout = pageLayout;
            }

            switch (toPageLayout)
            {
                case PageLayout.CREATE:
                    EnableFields(true);
                    this.btnCreate.Enabled = this.btnCreate.Visible = true;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnBack.Enabled = this.btnBack.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.READ:
                    DisableFields(false, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                    this.btnBack.Enabled = this.btnBack.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.UPDATE:
                    EnableFields(false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.DELETE:
                    DisableFields(false, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                    this.btnBack.Enabled = this.btnBack.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.CONFIRM_CREATE:
                    DisableFields(true, true);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmUpdate").ToString();
                    break;
                case PageLayout.CONFIRM_DELETE:
                    DisableFields(true, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteMessage").ToString();
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    DisableFields(true, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmUpdate").ToString();
                    break;
                default:
                    DisableFields(false, false);
                    break;
            }

            CurrentPageLayout = toPageLayout;
        }

        private void EnableFields(bool isCreate)
        {
            this.txtDeviceId.Enabled =
            this.txtTerminalName.Enabled =
            this.txtTerminalModel.Enabled =
            this.tbPassword.Enabled =
            this.chkMac.Enabled =
            this.ddlIssuer.Enabled =
            this.ddlBranch.Enabled =
            this.ddlTerminalMasterKey.Enabled = true;
            this.btnDelete.Enabled = this.btnDelete.Visible = false;
            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnEdit.Enabled = this.btnEdit.Visible = isCreate ? false : true;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
            this.btnBack.Visible = this.btnBack.Enabled = isCreate ? false : true;
            tbPassword.Attributes.Add("value", Password);

        }

        private void DisableFields(bool IsConfirm, bool isCreate)
        {
            this.txtDeviceId.Enabled =
            this.txtTerminalName.Enabled =
            this.txtTerminalModel.Enabled =
             this.tbPassword.Enabled =
            this.chkMac.Enabled =
            this.ddlIssuer.Enabled =
            this.ddlBranch.Enabled =
            this.ddlTerminalMasterKey.Enabled = false;
            this.btnDelete.Enabled = this.btnDelete.Visible = isCreate;
            this.btnBack.Visible = this.btnBack.Enabled = IsConfirm ? true : false;
            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = isCreate ? false : true;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = IsConfirm;
            tbPassword.Attributes.Add("value", Password);

        }

        /// <summary>
        /// Helper method used to set fields based on PageLaout.
        /// </summary>
        /// <param name="hideAll"></param>
        private void SetControls(bool hideAll)
        {
            this.btnEdit.Visible = this.btnEdit.Enabled = false;
            this.btnCreate.Visible = this.btnCreate.Enabled = false;
            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
            this.btnBack.Visible = this.btnBack.Enabled = true;

            bool enabled = false;
            bool visable = hideAll ? false : true;

            bool hasRead;
            bool hasUpdate;
            bool hasCreate;

            if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.ADMINISTRATOR, out hasRead, out hasUpdate, out hasCreate))
            {
                if (TerminalId != 0 && TerminalId < 0)
                {
                    this.btnEdit.Visible = this.btnEdit.Enabled = false;
                    this.btnCreate.Visible = this.btnCreate.Enabled = false;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                    this.btnBack.Visible = this.btnBack.Enabled = true;
                }
                else
                {
                    switch (CurrentPageLayout)
                    {
                        case PageLayout.READ:
                            enabled = false;
                          
                            if (hasUpdate)
                            {
                                this.btnEdit.Visible = this.btnEdit.Enabled = true;
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                //this.btnCancel.Visible = this.btnCancel.Enabled = false;
                                this.btnBack.Visible = this.btnBack.Enabled = true;
                            }
                            break;
                        case PageLayout.CREATE:
                            enabled = true;
                            if (hasCreate)
                            {
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnCreate.Visible = this.btnCreate.Enabled = true;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                //this.btnCancel.Visible = this.btnCancel.Enabled = false;
                                this.btnBack.Visible = this.btnBack.Enabled = false;
                            }
                            break;
                        case PageLayout.CONFIRM_CREATE:
                            enabled = false;
                            if (hasCreate)
                            {
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                                //this.btnCancel.Visible = this.btnCancel.Enabled = true;
                                this.btnBack.Visible = this.btnBack.Enabled = true;
                            }
                            break;
                        case PageLayout.UPDATE:
                            enabled = true;
                            if (hasUpdate)
                            {
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                //this.btnCancel.Visible = this.btnCancel.Enabled = false;
                                this.btnBack.Visible = this.btnBack.Enabled = true;
                            }
                            break;
                        case PageLayout.CONFIRM_UPDATE:
                            enabled = false;
                            if (hasUpdate)
                            {
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                                //this.btnCancel.Visible = this.btnCancel.Enabled = false;
                                this.btnBack.Visible = this.btnBack.Enabled = true;
                            }
                            break;
                    }
                }
            }

                this.txtTerminalName.Enabled =
                this.txtTerminalModel.Enabled =
                this.txtDeviceId.Enabled =
                 this.tbPassword.Enabled =
                this.chkMac.Enabled =
                this.ddlIssuer.Enabled =
                this.ddlBranch.Enabled =
                this.ddlTerminalMasterKey.Enabled = enabled;

                this.txtTerminalName.Visible =
                this.txtTerminalModel.Visible =
                this.txtDeviceId.Visible =
                 this.tbPassword.Visible =
                this.chkMac.Visible =
                this.ddlIssuer.Visible =
                this.ddlBranch.Visible =
                this.ddlTerminalMasterKey.Visible = visable;
        }

        #endregion

        #region Page Properties
        public TerminalSearchParams TerminalSearchParms
        {
            get
            {
                if (ViewState["TerminalSearchParms"] != null)
                    return (TerminalSearchParams)ViewState["TerminalSearchParms"];
                else
                    return null;
            }
            set
            {
                ViewState["TerminalSearchParms"] = value;
            }
        }
        private PageLayout? CurrentPageLayout
        {
            get
            {
                if (ViewState["CurrentPageLayout"] == null)
                    return PageLayout.CREATE;
                else
                    return (PageLayout)ViewState["CurrentPageLayout"];

            }
            set
            {
                ViewState["CurrentPageLayout"] = value;
            }
        }


        public int? TerminalId
        {
            get
            {
                if (ViewState["TerminalId"] == null)
                    return null;
                else
                    return Convert.ToInt32(ViewState["TerminalId"].ToString());
            }
            set
            {
                ViewState["TerminalId"] = value;
            }
        }

        #endregion

        #region Helper Methods

        private bool Validation()
        {
            bool flag = false;
            if (ddlIssuer.SelectedIndex < 0)
            {
                lblErrorMessage.Text = "Issuer Required.";
            }
            else
                if (ddlBranch.SelectedIndex < 0)
                {
                    lblErrorMessage.Text = "Branch Required.";
                }
                else if (ddlTerminalMasterKey.SelectedIndex < 0)
                {
                    lblErrorMessage.Text = "Master Key Required.";
                }

            if (string.IsNullOrEmpty(lblErrorMessage.Text.Trim()))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
            return flag;
        }


        private bool CreateTerminal()
        {
            lblErrorMessage.Text = string.Empty;

            try
            {
                int branchid = 0;
                int.TryParse(ddlBranch.SelectedValue, out branchid);

                int TerminalMasterKey = 0;
                int.TryParse(ddlTerminalMasterKey.SelectedValue, out TerminalMasterKey);

                string responsemessage; int terminalid;
                if (_terminalService.CreateTerminal(txtTerminalName.Text, txtTerminalModel.Text,
                     txtDeviceId.Text, branchid, TerminalMasterKey,Password,chkMac.Checked, out terminalid, out responsemessage))
                {
                    TerminalId = terminalid;
                    pageLayout = PageLayout.READ;
                    return true;
                }
                else
                {
                    lblErrorMessage.Text = responsemessage;
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }


        }

        private bool UpdateTerminal()
        {

            int branchid = 0;
            int.TryParse(ddlBranch.SelectedValue, out branchid);

            int TerminalMasterKey = 0;
            int.TryParse(ddlTerminalMasterKey.SelectedValue, out TerminalMasterKey);

            string responsemessage = string.Empty;


            if (_terminalService.UpdateTerminal((int)TerminalId, txtTerminalName.Text, txtTerminalModel.Text,
                txtDeviceId.Text, branchid, TerminalMasterKey,Password,chkMac.Checked, out responsemessage))
            {
                pageLayout = PageLayout.READ;
                return true;
            }
            else
            {
                lblErrorMessage.Text = responsemessage;
                return false;
            }

            return false;
        }



        #endregion
    }
}