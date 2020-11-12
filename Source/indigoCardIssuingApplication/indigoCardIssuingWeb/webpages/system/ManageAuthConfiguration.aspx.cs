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

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class ManageAuthConfiguration : BasePage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";
        private static readonly ILog log = LogManager.GetLogger(typeof(MasterkeyManager));
        private UserManagementService _userMan = new UserManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();

        private bool LoadDataEmpty = false;
        #region Page Load


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



        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                //this.txtMasterkeyName.Enabled =
                this.tbAuthConfigurationName.Enabled =true;

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
                
                    UpdateFormLayout(PageLayout.CONFIRM_CREATE);
                
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
            string responsemessage = string.Empty;
            try
            {
                if (CurrentPageLayout != null)
                {
                    pageLayout = CurrentPageLayout.Value;
                }

                switch (pageLayout)
                {
                    case PageLayout.CONFIRM_CREATE:
                        if (CreateAuthenticationConfguration(out responsemessage))
                        {
                            this.lblInfoMessage.Text = responsemessage;

                        }
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        DeleteAuthenticationConfguration(out responsemessage);
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        if (updateAuthenticationConfiguration(out responsemessage))
                        {
                            this.lblInfoMessage.Text = responsemessage;
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
                    Server.Transfer("~\\webpages\\system\\AuthConfigurationList.aspx");
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

        public int? AuthConfigId
        {
            get
            {
                if (ViewState["AuthConfigId"] == null)
                    return null;
                else
                    return Convert.ToInt32(ViewState["AuthConfigId"].ToString());
            }
            set
            {
                ViewState["AuthConfigId"] = value;
            }
        }
        #endregion

        #region Page Load Methods
        private void LoadPageData()
        {
            try
            {

                List<ListItem> externaltypes = new List<ListItem>();


                this.ddlInterface.Items.Clear();
                this.ddlInterface.Items.Add(new ListItem(GetLocalResourceObject("ListItemIndigoUser").ToString(), "-99"));
              
                List<ListItem> ldapList = new List<ListItem>();
                var ldapResult = _issuerMan.GetConnectionParameters();
                foreach (var item in ldapResult.Where(i => i.connection_parameter_type_id == 4 || (bool)i.is_external_auth == true))
                {
                    ldapList.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                }
                this.ddlInterface.Items.AddRange(ldapList.OrderBy(m => m.Text).ToArray());
                this.ddlInterface.SelectedIndex = 0;

                var availInterfaces = _issuerMan.ListAvailiableIntegrationInterfaces();
                this.ddlMultiFactorInterface.Items.Clear();
                //HSM
                List<ListItem> interfaces3 = new List<ListItem>();
                foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == 10))
                {
                    interfaces3.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
                }
                this.ddlMultiFactorInterface.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlMultiFactorInterface.Items.AddRange(interfaces3.OrderBy(m => m.Text).ToArray());

                this.ddlexternalInterface.Items.Clear();

                List<ListItem> interfaces = new List<ListItem>();
                foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == 6))
                {
                    interfaces.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
                }
                this.ddlexternalInterface.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlexternalInterface.Items.AddRange(interfaces.OrderBy(m => m.Text).ToArray());
                List<ListItem> configlist = new List<ListItem>();
                foreach (var item in ldapResult.Where(i => i.connection_parameter_type_id == 0))
                {
                    configlist.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                }
                this.ddlInterfaceConfig.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));

                this.ddlInterfaceConfig.Items.AddRange(configlist.OrderBy(m => m.Text).ToArray());
                this.ddlInterfaceConfig.SelectedIndex = 0;

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
                PopulateFields();

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
                if (SessionWrapper.AuthConfigId > 0)
                {
                    AuthConfigId = SessionWrapper.AuthConfigId;
                    SessionWrapper.AuthConfigId = 0;
                    if (AuthConfigId != 0)
                    {
                        var authConfig = _userMan.GetAuthConfiguration((int)AuthConfigId);

                        if (authConfig != null)
                        {
                            tbAuthConfigurationName.Text = authConfig.AuthConfig.authentication_configuration;
                        }

                         foreach(var item in authConfig.AuthConfigConnectionParams.Where(i=>i.auth_type_id== (int)AuthTypes.ExternalAuth))
                        {
                            ddlexternalInterface.SelectedValue = item.external_interface_id == null ? "-99" : item.external_interface_id;
                            ddlInterface.SelectedValue = item.connection_parameter_id == null ? "-99" : item.connection_parameter_id.ToString();
                        }
                        foreach (var item in authConfig.AuthConfigConnectionParams.Where(i => i.auth_type_id == (int)AuthTypes.MultiFactor))
                        {
                            ddlMultiFactorInterface.SelectedValue = item.interface_guid == null ? "-99" : item.interface_guid.ToString();
                            ddlInterfaceConfig.SelectedValue = item.connection_parameter_id == null ? "-99" : item.connection_parameter_id.ToString();

                        }


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
            this.tbAuthConfigurationName.Enabled =
                this.ddlInterface.Enabled=
                this.ddlInterfaceConfig.Enabled=
                this.ddlMultiFactorInterface.Enabled=
                this.ddlexternalInterface.Enabled = true;

            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnEdit.Enabled = this.btnEdit.Visible = isCreate ? false : true;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
            this.btnBack.Visible = this.btnBack.Enabled = isCreate ? false : true;
        }

        private void DisableFields(bool IsConfirm, bool isCreate)
        {
            this.tbAuthConfigurationName.Enabled =
               this.ddlInterface.Enabled =
                this.ddlInterfaceConfig.Enabled =
                this.ddlMultiFactorInterface.Enabled = false;

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

            this.tbAuthConfigurationName.Enabled = enabled;

            this.tbAuthConfigurationName.Visible = visable;
        }
        #endregion

        #region Helper Methods

       
        private AuthConfigResult ReadControlValues()
        {

            AuthConfigResult obj = new AuthConfigResult();

            auth_configuration_result authconfig = new auth_configuration_result();
            if (AuthConfigId != 0 && AuthConfigId != null)
            {
                authconfig.authentication_configuration_id = (int)AuthConfigId;
            }
            authconfig.authentication_configuration = tbAuthConfigurationName.Text;
            obj.AuthConfig = authconfig;

            List<auth_configuration_connectionparams_result> _list = new List<auth_configuration_connectionparams_result>();
            if (ddlInterface.SelectedIndex > -1)
            {
                auth_configuration_connectionparams_result _item;
                _item = new auth_configuration_connectionparams_result();
                if (AuthConfigId != 0 && AuthConfigId != null)
                {
                    _item.authentication_configuration_id = (int)AuthConfigId;
                }
                int? connection_paramter_id = null;
                if (ddlInterface.SelectedIndex > 0)
                {
                    _item.auth_type_id = (int)AuthTypes.ExternalAuth;
                    connection_paramter_id = int.Parse(ddlInterface.SelectedValue);
                }
                _item.connection_parameter_id = connection_paramter_id;

                _item.external_interface_id = ddlexternalInterface.SelectedValue == "-99" ? null : ddlexternalInterface.SelectedValue;

                _list.Add(_item);
            }
            if (ddlMultiFactorInterface.SelectedIndex>0)
            {
                auth_configuration_connectionparams_result _item;
                _item = new auth_configuration_connectionparams_result();
                if (AuthConfigId != 0 && AuthConfigId != null)
                {
                    _item.authentication_configuration_id = (int)AuthConfigId;
                }
                int? connection_paramter_id = null;
                if (ddlInterfaceConfig.SelectedIndex > 0)
                    connection_paramter_id = int.Parse(ddlInterfaceConfig.SelectedValue);
                _item.connection_parameter_id = connection_paramter_id;
                _item.auth_type_id = (int)AuthTypes.MultiFactor;
                _item.interface_guid = ddlMultiFactorInterface.SelectedIndex > 0 ? ddlMultiFactorInterface.SelectedValue:null;

                _list.Add(_item);
            }
            obj.AuthConfigConnectionParams = _list.ToArray();
            
            
            //ExternalSystemsResult externalsystem = new ExternalSystemsResult();

            //int external_system_typeid = Convert.ToInt32(this.ddlexternalsytemtype.SelectedValue);
            //externalsystem.system_name = txtExternalSystemName.Text;
            //externalsystem.external_system_type_id = external_system_typeid;

            //obj.ExternalSystem = externalsystem;
            //obj.ExternalSystemFields = PopulateExternalSystemFields();
            return obj;
        }

        private bool CreateAuthenticationConfguration(out string responsemessage)
        {
            responsemessage = string.Empty;
            try
            {

               
                 int? authConfigId;
                if (_userMan.InsertAuthenticationConfiguration(ReadControlValues(), out authConfigId, out responsemessage))
                {
                    AuthConfigId = authConfigId;
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

        private bool DeleteAuthenticationConfguration(out string responsemessage)
        {
            responsemessage = string.Empty;
            try
            {


                if (_userMan.DeleteAuthConfiguration( (int)AuthConfigId, out responsemessage))
                {
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

        private bool updateAuthenticationConfiguration(out string responsemessage)
        {

             responsemessage = string.Empty;

            if (_userMan.updateAuthenticationConfiguration(ReadControlValues(), out responsemessage))
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