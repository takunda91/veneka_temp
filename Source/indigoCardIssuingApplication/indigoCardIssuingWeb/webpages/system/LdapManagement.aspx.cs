using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.Web.Security;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class LdapDetail : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LdapDetail));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();



        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
            }
            tbPassword.Attributes.Add("value", Password);
        }

        #region Helper Methods

        private void LoadLdapSettings()
        {
            var results = _issuerMan.GetLdapSettings();
            LdapSettings = results;
            LoadLdapDetails();
        }

        private void LoadPageData()
        {
            try
            {
                if (SessionWrapper.LDAPID.HasValue)
                {
                    LdapSettingId = SessionWrapper.LDAPID.Value;
                    SessionWrapper.LDAPID = null;
                }
                else
                {
                    LdapSettingId = null;
                }
                var availInterfaces = _issuerMan.ListAvailiableIntegrationInterfacesByInterfaceTypeId(6);
                //CBS
                List<ListItem> interfaces = new List<ListItem>();
                foreach (var item in availInterfaces)
                {
                    interfaces.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
                }
                this.ddlInterface.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlInterface.Items.AddRange(interfaces.OrderBy(m => m.Text).ToArray());

                var authenticationtypes = _issuerMan.GetAuthenticationTypes();
                //CBS
                List<ListItem> lsauthenticationtypes = new List<ListItem>();
                foreach (var item in authenticationtypes)
                {
                    lsauthenticationtypes.Add(new ListItem(item.auth_type_name, item.auth_type_id.ToString()));
                }
                this.ddltype.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddltype.Items.AddRange(lsauthenticationtypes.OrderBy(m => m.Text).ToArray());

                LoadLdapSettings();
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

        private LdapSettingsResult ConstructConnectionObject()
        {
            LdapSettingsResult rtnLdap = new LdapSettingsResult();
            if (LdapSettingId != null && LdapSettingId != 0)
            {
                rtnLdap.ldap_setting_id = LdapSettingId.Value;
            }

            rtnLdap.domain_name = this.tbDomain.Text;
            rtnLdap.ldap_setting_name = this.tbLdapName.Text.Trim();
            rtnLdap.password = Password;
            rtnLdap.hostname_or_ip = this.tbHostnameOrIp.Text.Trim();
            rtnLdap.path = this.tbPath.Text.Trim();
            rtnLdap.username = this.tbUsername.Text;
            if (this.ddltype.SelectedValue != "-99")
                rtnLdap.auth_type_id = int.Parse(this.ddltype.SelectedValue);
            else
                rtnLdap.auth_type_id = null;
            if (this.ddlInterface.SelectedValue != "-99")
                rtnLdap.external_inteface_id = this.ddlInterface.SelectedValue;
            else
                rtnLdap.external_inteface_id = null;

            return rtnLdap;
        }

        private void FormElements(bool enable, bool hide)
        {
            this.tbDomain.Enabled =
                this.tbLdapName.Enabled =
                this.tbPassword.Enabled =
                this.tbHostnameOrIp.Enabled =
                this.tbPath.Enabled =
                this.ddlInterface.Enabled=
                this.ddltype.Enabled=
                this.tbUsername.Enabled = enable;
                //this.ddlLdapSettings.Enabled

            this.divInputs.Visible = hide ? false : true;
            this.btnCancel.Visible = this.btnCancel.Enabled =
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnCreate.Visible = this.btnCreate.Enabled =
                this.btnUpdate.Visible = this.btnUpdate.Enabled = this.btnDelete.Visible = this.btnDelete.Enabled = hide ? false : true;
            tbPassword.Attributes.Add("value", Password);
        }

        protected void ClearControls()
        {
            this.tbDomain.Text =
               this.tbLdapName.Text =
               this.tbPassword.Text =
               this.tbHostnameOrIp.Text =
               this.tbPath.Text =
                this.tbUsername.Text = "";
                this.ddlInterface.SelectedIndex = 0;
                this.ddltype.SelectedIndex = 0;
              
            //this.ddlLdapSettings.SelectedIndex = 0;
        }

        #endregion

        #region Page Events

        protected void LoadLdapDetails()
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";
            try
            {
                this.tbDomain.Text = "";
                this.tbHostnameOrIp.Text = "";
                this.tbPath.Text = "";
                this.tbLdapName.Text = "";

                this.tbUsername.Text = "";
                this.tbPassword.Text = "";

                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                this.btnDelete.Visible = this.btnDelete.Enabled = false;


                //LdapSettingId = null; // Clear out the connection ID. this is important for creating new connection.

                bool canRead;
                bool canUpdate;
                bool canCreate;
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.ADMINISTRATOR, out canRead, out canUpdate, out canCreate))
                {
                    int connectionId;
                    if (int.TryParse(LdapSettingId.ToString(), out connectionId))
                    {
                        if (connectionId != 0)
                        {
                            LdapSettingsResult conn = LdapSettings.Where(w => w.ldap_setting_id == connectionId).First();

                            this.tbDomain.Text = conn.domain_name;
                            this.tbHostnameOrIp.Text = conn.hostname_or_ip;
                            this.tbPath.Text = conn.path;
                            this.tbLdapName.Text = conn.ldap_setting_name;

                            this.tbUsername.Text = conn.username;

                            if (conn.auth_type_id!=null)
                            this.ddltype.SelectedValue = conn.auth_type_id.ToString();
                            if (conn.external_inteface_id!=null)
                            this.ddlInterface.SelectedValue = conn.external_inteface_id;

                            if (ddltype.SelectedValue == "2")
                            {
                                divInterface.Style.Add("display", "block");
                            }
                            else
                            {
                                divInterface.Style.Add("display", "none");

                            }
                           // this.tbPassword.Text = conn.password;
                            Password = conn.password;                        
                            FormElements(false, false);
                            //this.ddlLdapSettings.Enabled = true;

                            if (canUpdate)
                            {
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnEdit.Visible = this.btnEdit.Enabled = true;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                                this.btnDelete.Visible = this.btnDelete.Enabled = true;
                                this.btnCancel.Visible = this.btnCancel.Enabled = false;

                            }
                            pageLayout = PageLayout.READ;
                            LdapSettingId = conn.ldap_setting_id;
                        }
                        else
                        {
                            Password = null;
                            FormElements(true, false);
                          
                            if (canCreate)
                            {
                                this.btnCreate.Visible = this.btnCreate.Enabled = true;
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                                this.btnDelete.Visible = this.btnDelete.Enabled = false;
                                this.btnCancel.Visible = this.btnCancel.Enabled = false;
                            }
                            pageLayout = PageLayout.CREATE;
                        }
                    }
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteMessage").ToString();
                FormElements(false, false);
                pageLayout = PageLayout.DELETE;

                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                this.btnDelete.Visible = this.btnDelete.Enabled = false;
                this.btnCancel.Visible = this.btnCancel.Enabled = true;
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                Password = tbPassword.Text;
                FormElements(false, false);
                pageLayout = PageLayout.CONFIRM_CREATE;              
                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                this.btnDelete.Visible = this.btnDelete.Enabled = false;
                this.btnCancel.Visible = this.btnCancel.Enabled = true;
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
               
                FormElements(true, false);
                //this.ddlLdapSettings.Enabled = false;
                pageLayout = PageLayout.UPDATE;
                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                this.btnDelete.Visible = this.btnDelete.Enabled = false;
                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                this.btnUpdate.Visible = this.btnUpdate.Enabled = true;
                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                this.btnCancel.Visible = this.btnCancel.Enabled = true;
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                Password = tbPassword.Text;
                FormElements(false, false);
                pageLayout = PageLayout.CONFIRM_UPDATE;             
                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                this.btnDelete.Visible = this.btnDelete.Enabled = false;
                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                this.btnCancel.Visible = this.btnCancel.Enabled = true;
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                if (pageLayout != null)
                {
                    string messages;
                    LdapSettingsResult connParms = ConstructConnectionObject();
                    if (pageLayout == PageLayout.CONFIRM_CREATE)
                    {                        
                        if (_issuerMan.CreateLdapSetting(ref connParms, out messages))
                        {
                            //this.ddlLdapSettings.Items.Add(new ListItem(connParms.ldap_setting_name, connParms.ldap_setting_id.ToString()));
                            //this.ddlLdapSettings.SelectedValue = connParms.ldap_setting_id.ToString();
                            //LdapSettingId = connParms.ldap_setting_id;
                            //LdapSettings.Add(connParms);
                            LoadLdapSettings();
                            //this.ddlLdapSettings.SelectedValue = connParms.ldap_setting_id.ToString();
                            LdapSettingId = connParms.ldap_setting_id;
                            this.lblInfoMessage.Text = messages;
                            FormElements(false, false);                          
                            //this.ddlLdapSettings.Enabled = true;
                            this.btnCreate.Visible = this.btnCreate.Enabled = false;
                            this.btnEdit.Visible = this.btnEdit.Enabled = true;
                            this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                            this.btnCancel.Visible = this.btnCancel.Enabled = false;
                        }
                        else
                        {
                            this.lblErrorMessage.Text = messages;
                        }
                    }
                    else if (pageLayout == PageLayout.CONFIRM_UPDATE)
                    {
                        if (_issuerMan.UpdateLdapSetting(connParms, out messages))
                        {
                            //LdapSettings.Remove(connParms);
                            //LdapSettings.Add(connParms);
                            LoadLdapSettings();
                            //this.ddlLdapSettings.SelectedValue = connParms.ldap_setting_id.ToString();
                            LdapSettingId = connParms.ldap_setting_id;
                            this.lblInfoMessage.Text = messages;
                            pageLayout = PageLayout.UPDATE;
                            FormElements(false, false);                     
                            //this.ddlLdapSettings.Enabled = true;
                            this.btnCreate.Visible = this.btnCreate.Enabled = false;
                            this.btnEdit.Visible = this.btnEdit.Enabled = true;
                            this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                            this.btnCancel.Visible = this.btnCancel.Enabled = false;
                        }
                        else
                        {
                            this.lblErrorMessage.Text = messages;
                        }
                    }
                    else if (pageLayout == PageLayout.DELETE)
                    {
                        string response = _issuerMan.DeleteLdapSetting((int)LdapSettingId);
                        LdapSettingId = 0;
                        //ddlLdapSettings.Items.Clear();
                        LoadLdapSettings();
                        this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteSuccess").ToString();
                        pageLayout = PageLayout.READ;
                        ClearControls();
                        Password = null;
                        FormElements(true, false);
                        this.btnDelete.Visible = this.btnDelete.Enabled = false;
                        this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        this.btnCancel.Visible = this.btnCancel.Enabled = false;
                    }
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                this.btnCancel.Visible = this.btnCancel.Enabled = false;

                if (pageLayout == PageLayout.CREATE)
                {
                    FormElements(true, false);

                    this.btnCreate.Visible = this.btnCreate.Enabled = true;
                    this.btnDelete.Visible = this.btnDelete.Enabled = false;
                    this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                    this.btnEdit.Visible = this.btnEdit.Enabled = false;
                    this.btnCancel.Visible = this.btnCancel.Enabled = false;
                }
                else if (pageLayout == PageLayout.UPDATE)
                {

                    foreach (var item in LdapSettings)
                    {
                        if (item.ldap_setting_id == int.Parse(LdapSettingId.Value.ToString()))
                        {
                            this.tbDomain.Text = item.domain_name;
                            this.tbHostnameOrIp.Text = item.hostname_or_ip;
                            this.tbPath.Text = item.path;
                            this.tbLdapName.Text = item.ldap_setting_name;

                            this.tbUsername.Text = item.username;
                            this.tbPassword.Text = item.password;
                        }
                    }

                    this.pageLayout = PageLayout.READ;
                    FormElements(false, false);
                    //this.ddlLdapSettings.Enabled = true;
                    this.btnCancel.Visible = this.btnCancel.Enabled = false;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                    this.btnCreate.Visible = this.btnCreate.Enabled = false;
                    this.btnDelete.Visible = this.btnDelete.Enabled = true;
                    this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                    this.btnEdit.Visible = this.btnEdit.Enabled = true;
                }
                else if (pageLayout == PageLayout.CONFIRM_CREATE)
                {
                    this.pageLayout = PageLayout.CREATE;
                    FormElements(true, false);
                    this.btnCancel.Visible = this.btnCancel.Enabled = false;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                    this.btnCreate.Visible = this.btnCreate.Enabled = true;
                    this.btnDelete.Visible = this.btnDelete.Enabled = false;
                    this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                    this.btnEdit.Visible = this.btnEdit.Enabled = false;
                }
                else if (pageLayout == PageLayout.CONFIRM_UPDATE)
                {
                    this.pageLayout = PageLayout.UPDATE;
                    FormElements(true, false);
                    //this.ddlLdapSettings.Enabled = false;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                    this.btnCreate.Visible = this.btnCreate.Enabled = false;
                    this.btnDelete.Visible = this.btnDelete.Enabled = false;
                    this.btnUpdate.Visible = this.btnUpdate.Enabled = true;
                    this.btnEdit.Visible = this.btnEdit.Enabled = false;
                }
                else if (pageLayout == PageLayout.DELETE)
                {
                    this.pageLayout = PageLayout.DELETE;
                    FormElements(false, false);
                    //this.ddlLdapSettings.Enabled = true;
                    this.btnCreate.Visible = this.btnCreate.Enabled = false;
                    this.btnEdit.Visible = this.btnEdit.Enabled = true;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                    this.btnUpdate.Visible = this.btnUpdate.Enabled = false;
                    this.btnDelete.Visible = this.btnDelete.Enabled = true;
                    this.btnCancel.Visible = this.btnCancel.Enabled = false;
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

        protected void chkmaskpassword_CheckedChanged(object sender, EventArgs e)
        {
            if (Password == null)
            {
                Password = tbPassword.Text;
            }
            if (chkmaskpassword.Checked)
            {
                tbPassword.TextMode = TextBoxMode.Password;
            }
            else
            {
                tbPassword.TextMode = TextBoxMode.SingleLine;
            }
        }

        #endregion

        #region Page Properties

        private List<LdapSettingsResult> LdapSettings
        {
            get
            {
                if (ViewState["LdapSettings"] != null)
                {
                    return (List<LdapSettingsResult>)ViewState["LdapSettings"];
                }
                return null;
            }

            set
            {
                ViewState["LdapSettings"] = value;
            }
        }

        private int? LdapSettingId
        {
            get
            {
                if (ViewState["LdapSettingId"] != null)
                {
                    return (int)ViewState["LdapSettingId"];
                }
                return null;
            }

            set
            {
                ViewState["LdapSettingId"] = value;
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

        private PageLayout? pageLayout
        {
            get
            {
                if (ViewState["pageLayout"] != null)
                {
                    return (PageLayout)ViewState["pageLayout"];
                }
                return null;
            }
            set
            {
                ViewState["pageLayout"] = value;
            }
        }

        #endregion

        protected void ddltype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddltype.SelectedValue=="2")
            {
                divInterface.Style.Add("display", "block");
            }
            else
            {
                divInterface.Style.Add("display", "none");

            }
        }
    }
}
