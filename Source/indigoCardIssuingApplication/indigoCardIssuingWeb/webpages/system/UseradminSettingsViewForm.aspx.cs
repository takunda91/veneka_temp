using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;

namespace indigoCardIssuingWeb.webpages.users
{
    public partial class UseradminSettingsViewForm : BasePage
    {
        private const string PageLayoutKey = "PageLayout";

        private static readonly ILog log = LogManager.GetLogger(typeof(UserGroupViewForm));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };
        private PageLayout pageLayout = PageLayout.READ;

        private readonly UserManagementService _userMan = new UserManagementService();

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

        #region Helper methods for page construction/Display
        private void LoadPageData()
        {
            try
            {


                //If this is not null then we've the page has been called from a search, load the data.

                pageLayout = PageLayout.READ;
                isView = true;

                var result = _userMan.GetUseradminSettings();

                UserAdminSettingsID = result.user_admin_id;

                tbPasswordMaxlength.Text = result.PasswordMaxLength.ToString();
                tbPasswordminlength.Text = result.PasswordMinLength.ToString();
                tbpasswordvalidation.Text = result.PasswordValidPeriod.ToString();
                tbPreviousPasswordsCount.Text = result.PreviousPasswordsCount.ToString();
                tbmaxnofpasswordattempts.Text = result.maxInvalidPasswordAttempts.ToString();
                tbPasswordAttemptLockoutDuration.Text = result.PasswordAttemptLockoutDuration.ToString();



                ViewState[PageLayoutKey] = pageLayout;
                SessionWrapper.UserAdminSettingsID = null;
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
        /// Use this to update form components depending on layout.
        /// </summary>
        /// <param name="toPageLayout"></param>
        private void UpdateFormLayout(PageLayout? toPageLayout)
        {
            this.btnCreate.Enabled = this.btnCreate.Visible = false;
            this.btnEdit.Enabled = this.btnEdit.Visible = false;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
            this.btnBack.Enabled = this.btnBack.Visible = false;

            if (toPageLayout == null)
            {
                if (ViewState[PageLayoutKey] != null)
                {
                    pageLayout = (PageLayout)ViewState[PageLayoutKey];
                }
                toPageLayout = pageLayout;
            }

            bool hasRead;
            bool hasUpdate;
            bool hasCreate;
            if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.ADMINISTRATOR, out hasRead, out hasUpdate, out hasCreate))
            {
                switch (toPageLayout)
                {
                    case PageLayout.CREATE:
                        EnableFields(true);
                        if (hasCreate)
                        {
                            this.btnCreate.Enabled = this.btnCreate.Visible = true;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                            this.btnBack.Enabled = this.btnBack.Visible = false; ;
                        }
                        break;
                    case PageLayout.READ:
                        DisableFields(false);
                        if (hasUpdate)
                        {
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = true;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                            this.btnBack.Enabled = this.btnBack.Visible = false;
                        }
                        break;
                    case PageLayout.UPDATE:
                        EnableFields(false);
                        if (hasUpdate)
                        {
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                            this.btnBack.Enabled = this.btnBack.Visible = true;
                        }
                        break;
                    case PageLayout.DELETE:

                        break;
                    case PageLayout.CONFIRM_CREATE:
                        DisableFields(true);
                        if (hasCreate)
                        {
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                            this.btnBack.Enabled = this.btnBack.Visible = true;
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                        }

                        break;

                    case PageLayout.CONFIRM_UPDATE:
                        DisableFields(true);
                        if (hasUpdate)
                        {
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                            this.btnBack.Enabled = this.btnBack.Visible = true;
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                        }


                        break;
                    default:
                        DisableFields(false);
                        this.btnCreate.Enabled = this.btnCreate.Visible = false;
                        this.btnEdit.Enabled = this.btnEdit.Visible = false;
                        this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                        this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                        this.btnBack.Enabled = this.btnBack.Visible = false;
                        break;
                }
            }

            ViewState[PageLayoutKey] = toPageLayout;
        }

        private void ClearFields()
        {
            tbPasswordMaxlength.Text = string.Empty;
            tbPasswordminlength.Text = string.Empty;
            tbpasswordvalidation.Text = string.Empty;
            tbPreviousPasswordsCount.Text = string.Empty;
            tbmaxnofpasswordattempts.Text = string.Empty;
            tbPasswordAttemptLockoutDuration.Text = string.Empty;
        }

        private void EnableFields(bool isCreate)
        {

            this.tbPasswordMaxlength.Enabled = true;
            this.tbPasswordminlength.Enabled = true;
            this.tbpasswordvalidation.Enabled = true;
            this.tbPreviousPasswordsCount.Enabled = true;
            this.tbmaxnofpasswordattempts.Enabled = true;
            this.tbPasswordAttemptLockoutDuration.Enabled = true;
        }

        private void DisableFields(bool IsConfirm)
        {
            this.tbPasswordMaxlength.Enabled = false;
            this.tbPasswordminlength.Enabled = false;
            this.tbpasswordvalidation.Enabled = false;
            this.tbPreviousPasswordsCount.Enabled = false;
            this.tbmaxnofpasswordattempts.Enabled = false;
            this.tbPasswordAttemptLockoutDuration.Enabled = false;
        }



        #endregion

        #region Helper Methods for Persisting Methods
        /// <summary>
        /// Populate a user group object from info on page.
        /// </summary>
        /// <returns>If null means something went wrong</returns>
        private useradminsettingslist PopulateUserSettings()
        {
            useradminsettingslist item = new useradminsettingslist();

            if (UserAdminSettingsID != null)
            {
                item.user_admin_id = UserAdminSettingsID.Value;
            }
            int PasswordValidPeriod = 0, PasswordMinLength = 0, PasswordMaxLength = 0, PreviousPasswordsCount = 0, maxnofpasswordattempts = 0, PasswordAttemptLockoutDuration = 0;
            int.TryParse(tbPreviousPasswordsCount.Text, out PreviousPasswordsCount);
            int.TryParse(tbpasswordvalidation.Text, out PasswordValidPeriod);
            int.TryParse(tbPasswordminlength.Text, out PasswordMinLength);
            int.TryParse(tbPasswordMaxlength.Text, out PasswordMaxLength);
            int.TryParse(tbmaxnofpasswordattempts.Text, out maxnofpasswordattempts);
            int.TryParse(tbPasswordAttemptLockoutDuration.Text, out PasswordAttemptLockoutDuration);


            item.PasswordValidPeriod = PasswordValidPeriod;
            item.PasswordMinLength = PasswordMinLength;
            item.PreviousPasswordsCount = PreviousPasswordsCount;
            item.PasswordMaxLength = PasswordMaxLength;
            item.maxInvalidPasswordAttempts = maxnofpasswordattempts;
            item.PasswordAttemptLockoutDuration = PasswordAttemptLockoutDuration;

            return item;

        }

        #endregion

        #region DB Persisting Methods
        /// <summary>
        /// Persists new user group to the DB
        /// </summary>
        /// <returns></returns>
        private bool CreateUserGroup()
        {
            useradminsettingslist item = PopulateUserSettings();

            if (item != null)
            {
                int? user_admin_id;
                string result;
                if (_userMan.CreateUseradminSettings(item, out user_admin_id, out result))
                {
                    UserAdminSettingsID = user_admin_id;
                    this.lblInfoMessage.Text = result;
                    return true;
                }
                else
                {
                    this.lblErrorMessage.Text = result;
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Persist changes to the DB.
        /// </summary>
        /// <returns></returns>
        private bool UpdateUserGroup()
        {

            useradminsettingslist item = PopulateUserSettings();

            if (item != null)
            {
                string result;

                if (_userMan.UpdateUseradminSettings(item, out result))
                {

                    this.lblInfoMessage.Text = result;
                    return true;
                }
                else
                {
                    this.lblErrorMessage.Text = result;
                    return false;
                }
            }

            return false;
        }


        #endregion

        #region PAGE EVENTS
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
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                isView = false;
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
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (ViewState[PageLayoutKey] != null)
                {
                    pageLayout = (PageLayout)ViewState[PageLayoutKey];
                }

                switch (pageLayout)
                {
                    case PageLayout.CREATE:
                        UpdateFormLayout(PageLayout.CONFIRM_CREATE);
                        break;
                    case PageLayout.READ:
                        //SaveRestoreGroup(PopulateUserGroup());
                        UpdateFormLayout(PageLayout.UPDATE);
                        break;
                    case PageLayout.UPDATE:
                        UpdateFormLayout(PageLayout.CONFIRM_UPDATE);
                        break;
                    case PageLayout.DELETE:
                        UpdateFormLayout(PageLayout.CONFIRM_DELETE);
                        break;
                    case PageLayout.CONFIRM_CREATE:
                        if (CreateUserGroup())
                            UpdateFormLayout(PageLayout.READ);
                        break;

                    case PageLayout.CONFIRM_UPDATE:
                        if (UpdateUserGroup())
                            UpdateFormLayout(PageLayout.READ);
                        break;
                    default:
                        UpdateFormLayout(PageLayout.READ);
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

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            if (ViewState[PageLayoutKey] != null)
            {
                pageLayout = (PageLayout)ViewState[PageLayoutKey];
            }

            switch (pageLayout)
            {
                case PageLayout.UPDATE:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.DELETE:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.CONFIRM_CREATE:
                    UpdateFormLayout(PageLayout.CREATE);
                    break;
                case PageLayout.CONFIRM_DELETE:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    UpdateFormLayout(PageLayout.UPDATE);
                    break;
                default:
                    break;
            }
        }


        #endregion

        private bool isView
        {
            get
            {
                if (ViewState["isView"] == null)
                    return true;
                else
                    return (bool)ViewState["isView"];
            }

            set
            {
                ViewState["isView"] = value;
            }
        }

        private int? UserAdminSettingsID
        {
            get
            {
                if (ViewState["UserAdminSettingsID"] == null)
                    return null;
                else
                    return (int)ViewState["UserAdminSettingsID"];
            }
            set
            {
                ViewState["UserAdminSettingsID"] = value;
            }
        }



    }
}