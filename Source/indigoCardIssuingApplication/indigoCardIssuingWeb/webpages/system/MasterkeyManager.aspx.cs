using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
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
    public partial class MasterkeyManager : BasePage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";
        private static readonly ILog log = LogManager.GetLogger(typeof(MasterkeyManager));
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly TerminalManagementService _terminalService = new TerminalManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };

        #region Page Load
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this.ddlIssuer.Items.Count == 0)
            {
                this.lblIssuer.Enabled =
                    this.ddlIssuer.Visible = false;
            }
        }

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (ViewState[PageLayoutKey] != null)
            {
                pageLayout = (PageLayout)ViewState[PageLayoutKey];
            }

            if (!IsPostBack)
            {
                LoadPageData();
                PopulateFields();
                UpdateFormLayout(pageLayout);
            }
        }
        #endregion

        #region Events
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

        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                int issuerId = int.Parse(ddlIssuer.SelectedValue);
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
                //this.txtMasterkeyName.Enabled =
                this.txtMasterkey.Enabled =
                this.ddlIssuer.Enabled = true;

                this.btnEdit.Enabled = this.btnEdit.Visible = false;
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
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                UpdateFormLayout(PageLayout.CONFIRM_UPDATE);
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
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (Validation())
                {
                    UpdateFormLayout(PageLayout.CONFIRM_CREATE);
                }
                else
                {
                    this.lblErrorMessage.Text = "Invalid Issuer / Branch selection";
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
                        if (CreateMasterkey())
                        {
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessCreate").ToString();

                        }
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        DeleteMasterKey();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        if (UpdateMasterkey())
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
                    Server.Transfer("~\\webpages\\system\\MasterkeyList.aspx");
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
            }
        }
        #endregion

        #region Page Properties
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

        public int? MasterkeyId
        {
            get
            {
                if (ViewState["MasterkeyId"] == null)
                    return null;
                else
                    return Convert.ToInt32(ViewState["MasterkeyId"].ToString());
            }
            set
            {
                ViewState["MasterkeyId"] = value;
            }
        }
        #endregion

        #region Page Load Methods
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

                if (ddlIssuer.Items.Count == 0)
                {
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                }
                if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    pnlButtons.Visible = false;
                    lblInfoMessage.Text = "";
                    //lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SaveActionMessage").ToString();
                }
                else
                {
                    lblErrorMessage.Text = "";
                    pnlButtons.Visible = true;
                }

                UpdateFormLayout(pageLayout);
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

        private void PopulateFields()
        {
            try
            {
                if (SessionWrapper.MasterkeyId > 0)
                {
                    MasterkeyId = SessionWrapper.MasterkeyId;
                    SessionWrapper.MasterkeyId = 0;
                    if (MasterkeyId != 0)
                    {
                        var masterkey = _terminalService.GetMasterkey((int)MasterkeyId);

                        txtMasterkey.Text = masterkey.masterkey;
                        txtMasterkeyName.Text = masterkey.masterkey_name;
                        ddlIssuer.SelectedValue = masterkey.issuer_id.ToString();

                        pageLayout = PageLayout.READ;
                    }
                    else
                    {
                        //No username in session, set page layout to create.
                        pageLayout = PageLayout.CREATE;
                    }
                }
                else
                {
                    //No username in session, set page layout to create.
                    pageLayout = PageLayout.CREATE;
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
        #endregion

        #region Page Flow Methods
        private bool DeleteMasterKey()
        {
            try
            {
                string responsemessage = string.Empty;
                bool response = _terminalService.DeleteMasterKey(MasterkeyId, out responsemessage);
                if (response)
                {

                    Server.Transfer("~\\webpages\\system\\MasterKeyList.aspx?delete=" + this.ddlIssuer.SelectedValue);

                    //this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessDelete").ToString();
                    //pageLayout = PageLayout.CREATE;

                    //MasterkeyId = 0;
                    //this.txtMasterkey.Text = "";
                    //this.txtMasterkeyName.Text = "";

                    //return true;

                }
                else
                {
                    this.lblInfoMessage.Text = string.Empty;
                    this.lblErrorMessage.Text = responsemessage;
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
                    this.btnBack.Enabled = this.btnBack.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
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
                case PageLayout.UPDATE:
                    EnableFields(false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
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
            this.txtMasterkeyName.Enabled =
            this.txtMasterkey.Enabled =
            this.ddlIssuer.Enabled = true;

            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnEdit.Enabled = this.btnEdit.Visible = isCreate ? false : true;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
            this.btnBack.Visible = this.btnBack.Enabled = isCreate ? false : true;
        }

        private void DisableFields(bool IsConfirm, bool isCreate)
        {
            this.txtMasterkeyName.Enabled =
            this.txtMasterkey.Enabled =
            this.ddlIssuer.Enabled = false;

            this.btnBack.Visible = this.btnBack.Enabled = IsConfirm ? true : false;
            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = isCreate ? false : true;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = IsConfirm;
        }

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
                if (SessionWrapper.TerminalId < 0)
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

            this.txtMasterkeyName.Enabled =
            this.txtMasterkey.Enabled =
                this.ddlIssuer.Enabled = enabled;

            this.txtMasterkeyName.Visible =
            this.txtMasterkey.Visible =
                this.ddlIssuer.Visible = visable;
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

        private bool CreateMasterkey()
        {
            try
            {
                int issuerId = Convert.ToInt32(this.ddlIssuer.SelectedValue);
                string masterkey = txtMasterkey.Text;
                string masterkeyName = txtMasterkeyName.Text;

                string responsemessage; int masterkeyid;
                if (_terminalService.CreateMasterkey(issuerId, masterkey, masterkeyName, out masterkeyid, out responsemessage))
                {
                    MasterkeyId = masterkeyid;
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

        private bool UpdateMasterkey()
        {
            int issuerId = Convert.ToInt32(this.ddlIssuer.SelectedValue);
            string masterkey = txtMasterkey.Text;
            string masterkeyName = txtMasterkeyName.Text;
            string responsemessage = string.Empty;

            if (_terminalService.UpdateMasterkey((int)MasterkeyId, masterkeyName, issuerId, masterkey, out responsemessage))
            {
                pageLayout = PageLayout.READ;
                return true;
            }
            else
            {
                lblErrorMessage.Text = responsemessage;
            }
            return false;
        }
        #endregion
    }
}