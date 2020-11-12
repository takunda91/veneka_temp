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
using indigoCardIssuingWeb.SearchParameters;
using System.Web.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Web;
using System.Collections.ObjectModel;

namespace indigoCardIssuingWeb.webpages.users
{
    public partial class ManageUser : BasePage
    {
        //private const string PageLayoutKey = "PageLayout";        
        //private const string PasswordKey = "PasswordKey";

        private static readonly ILog log = LogManager.GetLogger(typeof(ManageUser));
        //Standardise look and feel of the Website across all Web Browsers
        //private readonly SystemAdminService sysAdminService = new SystemAdminService();
        private readonly UserManagementService userMan = new UserManagementService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.USER_ADMIN, UserRole.USER_AUDIT };
        private PageLayout pageLayout = PageLayout.READ;
        private static Random r = new Random();

        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (CurrentPageLayout != null)
            {
                pageLayout = CurrentPageLayout.Value;
            }

            if (!IsPostBack)
            {
                LoadPageData();
                UpdateFormLayout(null);
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(this.tbPassword.Text))
                {
                    Password = this.tbPassword.Text;
                }

                if (!String.IsNullOrWhiteSpace(Password))
                {
                    this.tbPassword.Attributes["value"] = Password;
                    this.tbConfirmPassword.Attributes["value"] = Password;
                }
            }
        }

        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                if (SessionWrapper.SearchUserMode)
                {
                    SearchMode = SessionWrapper.SearchUserMode;
                    SessionWrapper.SearchUserMode = false;
                }
                if (SessionWrapper.UserSearchParameters != null)
                {
                    UserSearchParams = SessionWrapper.UserSearchParameters;
                    SessionWrapper.UserSearchParameters = null;
                }


                ddlAuthConfig.Items.Clear();
                List<ListItem> authconfigList = new List<ListItem>();
                foreach (var config in userMan.GetAuthConfigurationList(1, 2000))
                {
                    authconfigList.Add(new ListItem(config.authentication_configuration, config.authentication_configuration_id.ToString()));
                }
                ddlAuthConfig.Items.AddRange(authconfigList.OrderBy(m => m.Text).ToArray());
                ddlAuthConfig.SelectedIndex = -1;

                ddlUserStatus.Items.Clear();
                List<ListItem> userStatusList = new List<ListItem>();
                foreach (var userStatus in userMan.LangLookupUserStatuses())
                {
                    userStatusList.Add(new ListItem(userStatus.language_text, userStatus.lookup_id.ToString()));
                }
                ddlUserStatus.Items.AddRange(userStatusList.OrderBy(m => m.Text).ToArray());
                ddlUserStatus.SelectedValue = "0";

                ddlUserLanguage.Items.Clear();
                List<ListItem> userLanguagesList = new List<ListItem>();
                foreach (var item in userMan.GetLanguages())
                {
                    string text = "";
                    string id = item.id.ToString();
                    switch (SessionWrapper.UserLanguage)
                    {
                        case 0:
                            text = item.language_name;
                            break;
                        case 1:
                            text = item.language_name_fr;
                            break;
                        case 2:
                            text = item.language_name_pt;
                            break;
                        case 3:
                            text = item.language_name_sp;
                            break;
                        default:
                            text = item.language_name;
                            break;
                    }

                    userLanguagesList.Add(new ListItem(text, id));
                }
                ddlUserLanguage.Items.AddRange(userLanguagesList.OrderBy(m => m.Text).ToArray());


                this.ddlIssuer.Items.Clear();
                this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());

                ddlRole.Items.Clear();
                List<ListItem> userRolesList = new List<ListItem>();
                int? isenterprise = null;
                //only the enterprise user can create useradmin
                if (!issuersList.ContainsKey(-1))
                {
                    isenterprise = 0;

                }
                foreach (var userRole in userMan.GetLangUserRoles(isenterprise))
                {

                    userRolesList.Add(new ListItem(userRole.language_text, userRole.lookup_id.ToString()));
                }


                ddlRole.Items.AddRange(userRolesList.OrderBy(m => m.Text).ToArray());

                try
                {


                    this.ddlTimeZone.Items.Clear();
                    ddlTimeZone.DataSource = TimeZoneInfo.GetSystemTimeZones();
                    ddlTimeZone.DataValueField = "Id";
                    ddlTimeZone.DataTextField = "DisplayName";
                    ddlTimeZone.DataBind();

                   
                    PopulateFields();
                    UpdateFormForLdapLookup(true);
                }
                catch (Exception ex)
                {
                    this.pnlButtons.Visible = false;
                    log.Error(ex);
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                    if (log.IsDebugEnabled || log.IsTraceEnabled)
                    {
                        this.lblErrorMessage.Text = ex.ToString();
                    }
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

        private void LoadEditData()
        {
            try
            {
                this.pnlUserGroups.Visible = true;
                this.pnlViewUserGroups.Visible = false;
                this.dlGroupList.Enabled = false;

                updateBranchList(int.Parse(this.ddlIssuer.SelectedValue));
            }
            catch (Exception ex)
            {
                this.pnlButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private void LoadViewData(bool isConfirm)
        {
            try
            {
                this.pnlUserGroups.Visible = false;
                this.pnlViewUserGroups.Visible = true;
                this.dlGroupList.Enabled = true;

                List<GroupsRolesResult> results = new List<GroupsRolesResult>();

                if (isConfirm)
                {
                    if (GroupsViewList != null)
                    {
                        results = GroupsViewList.Values.ToList();
                    }
                }
                else if (UserId != null || PendingUserId != null)
                {
                    results = PendingUserId == null ? userMan.GetGroupRolesForUser(UserId.Value) : userMan.GetGroupRolesForPendingUser(PendingUserId.Value);
                    Dictionary<int, GroupsRolesResult> groups = new Dictionary<int, GroupsRolesResult>();
                    foreach (var item in results)
                    {
                        groups.Add(item.user_group_id, item);
                    }
                    GroupsViewList = groups;
                }

                this.dlGroupList.DataSource = results.OrderBy(m => m.issuer_code)
                                                     .ThenBy(m => m.user_role)
                                                     .ThenBy(m => m.user_group_name);
                this.dlGroupList.DataBind();
            }
            catch (Exception ex)
            {
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
                long? userId = SessionWrapper.SelectedUserId;
                long? pendinguserId = SessionWrapper.SelectedPendingUserId;

                if (userId != null || pendinguserId != null)
                {
                    //get the user
                    decrypted_user userResult = pendinguserId == null ? userMan.GetUserByUserId(userId.GetValueOrDefault())
                                                : userMan.GetPendingUserByUserId(pendinguserId.GetValueOrDefault());

                    tbFirstName.Text = userResult.first_name;
                    tbLastName.Text = userResult.last_name;
                    tbUserName.Text = userResult.username;
                    tbEmail.Text = userResult.user_email;

                    ddlUserStatus.SelectedValue = userResult.user_status_id.ToString();
                    ddlUserLanguage.SelectedValue = userResult.language_id.ToString() ?? "0";
                    if (userResult.time_zone_Id != null)
                        ddlTimeZone.SelectedValue = userResult.time_zone_Id.ToString() ?? "0";

                    ddlAuthConfig.SelectedValue = userResult.authentication_configuration_id.ToString();


                    LoadGroupsForUser(userId, pendinguserId);
                    


                    UserId = userId;
                    PendingUserId = pendinguserId;
                    pageLayout = PageLayout.READ;
                }
                else
                {
                    //No username in session, set page layout to create.
                    pageLayout = PageLayout.CREATE;
                }

                CurrentPageLayout = pageLayout;

                bool canUpdate;
                bool canRead;
                bool canCreate;
                SessionWrapper.SelectedUserName = null;
                SessionWrapper.SelectedUserId = null;
                SessionWrapper.SelectedPendingUserId = null;

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

        private void updateBranchList(int issuerId)
        {
            this.ddlBranch.Items.Clear();

            var branches = userMan.GetBranchesForUser(issuerId, userRolesForPage[0], null);

            List<ListItem> branchList = new List<ListItem>();

            foreach (var item in branches)//Convert branches in item list.
            {
                ListItem listItem = UtilityClass.FormatListItem(item.branch_name, item.branch_code, item.branch_id);

                branchList.Add(listItem);
            }

            this.ddlBranch.Items.Add(new ListItem(Resources.ListItemLabels.ALL, "-99"));
            if (branchList.Count > 0)
            {
                this.ddlBranch.Items.AddRange(branchList.OrderBy(m => m.Text).ToArray());
            }

            LoadGroupsForUser(UserId, PendingUserId);

        }

        #endregion

        #region Page Flow Methods
        private void UpdateFormLayout(PageLayout? toPageLayout)
        {
            this.btnCreate.Enabled = this.btnCreate.Visible = false;
            this.btnEdit.Enabled = this.btnEdit.Visible = false;
            this.btnApprove.Enabled = this.btnApprove.Visible = false;
            this.btnReject.Enabled = this.btnReject.Visible = false;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
            this.btnResetPassword.Enabled = this.btnResetPassword.Visible = false;
            this.btnResetUserLogin.Enabled = this.btnResetUserLogin.Visible = false;

            if (toPageLayout == null)
            {
                if (CurrentPageLayout != null)
                {
                    pageLayout = CurrentPageLayout.Value;
                }
                toPageLayout = pageLayout;
            }

            bool canCreate;
            bool canUpdate;
            bool canRead;
            long? UserId = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.UserId;
            if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.USER_ADMIN, out canRead, out canUpdate, out canCreate) || PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.USER_AUDIT, out canRead, out canUpdate, out canCreate))
            {
                switch (toPageLayout)
                {
                    case PageLayout.CREATE:
                        EnableFields(true);
                        UpdateFormForLdapLookup(true);
                        LoadEditData();
                        if (canCreate)
                        {
                            this.btnCreate.Enabled = this.btnCreate.Visible = true;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnApprove.Enabled = this.btnApprove.Visible = false;

                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                            this.btnResetPassword.Enabled = this.btnResetPassword.Visible = false;
                            this.btnResetUserLogin.Enabled = this.btnResetUserLogin.Visible = false;
                        }
                        break;
                    case PageLayout.READ:
                        DisableFields(false, false);
                        LoadViewData(false);
                        if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.USER_AUDIT, out canRead, out canUpdate, out canCreate))
                        {
                            if (!(PendingUserId == null))
                            {
                                this.btnApprove.Enabled = this.btnApprove.Visible = true;
                                this.btnReject.Enabled = this.btnReject.Visible = true;

                            }
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;

                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                            this.btnBack.Enabled = this.btnBack.Visible = true;
                            this.btnResetPassword.Visible = this.btnResetPassword.Enabled = false;
                            this.btnResetUserLogin.Visible = this.btnResetUserLogin.Enabled = false;
                        }
                        else
                        if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.USER_ADMIN, out canRead, out canUpdate, out canCreate))
                        {
                            if (canUpdate && this.UserId != UserId)
                            {
                                this.btnCreate.Enabled = this.btnCreate.Visible = false;
                                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                                this.btnReject.Enabled = this.btnReject.Visible = false;

                                if (this.UserId != null && PendingUserId == null)
                                {
                                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                                    this.btnResetPassword.Visible = this.btnResetPassword.Enabled =
                                               IsIndiogUser();
                                    this.btnResetUserLogin.Visible = this.btnResetUserLogin.Enabled = true;
                                }
                                this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                                this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                                this.btnBack.Enabled = this.btnBack.Visible = true;


                            }

                        }
                        break;
                    case PageLayout.UPDATE:
                        EnableFields(false);
                        LoadEditData();
                        if (canUpdate)
                        {
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnApprove.Enabled = this.btnApprove.Visible = false;

                            this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                            this.btnResetPassword.Enabled = this.btnResetPassword.Visible = false;
                            this.btnResetUserLogin.Enabled = this.btnResetUserLogin.Visible = false;
                        }
                        break;
                    case PageLayout.DELETE:
                        //this.btnUpdate.Text = UtilityClass.UppercaseFirst(PageLayout.DELETE.ToString());
                        break;
                    case PageLayout.CONFIRM_CREATE:
                        DisableFields(true, true);
                        LoadViewData(true);
                        if (canCreate)
                        {
                            this.btnLookupUser.Enabled = this.btnLookupUser.Visible = false;
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnApprove.Enabled = this.btnApprove.Visible = false;

                            this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                            this.btnResetPassword.Enabled = this.btnResetPassword.Visible = false;
                            this.btnResetUserLogin.Enabled = this.btnResetUserLogin.Visible = false;
                        }
                        //this.btnUpdate.Text = "Confirm";
                        break;

                    case PageLayout.CONFIRM_APPROVE:
                    case PageLayout.CONFIRM_REJECT:

                        DisableFields(true, true);
                        LoadViewData(true);
                        if (canCreate)
                        {
                            this.btnLookupUser.Enabled = this.btnLookupUser.Visible = false;
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnApprove.Enabled = this.btnApprove.Visible = false;

                            this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                            this.btnResetPassword.Enabled = this.btnResetPassword.Visible = false;
                            this.btnResetUserLogin.Enabled = this.btnResetUserLogin.Visible = false;
                        }
                        //this.btnUpdate.Text = "Confirm";
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        DisableFields(true, false);
                        this.btnLookupUser.Enabled = this.btnLookupUser.Visible = false;
                        this.btnCreate.Enabled = this.btnCreate.Visible = false;
                        this.btnEdit.Enabled = this.btnEdit.Visible = false;
                        this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                        this.btnApprove.Enabled = this.btnApprove.Visible = false;

                        this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                        this.btnResetPassword.Enabled = this.btnResetPassword.Visible = false;
                        this.btnResetUserLogin.Enabled = this.btnResetUserLogin.Visible = false;
                        //this.lblInfoMessage.Text = Resources.CommonInfoMessages.ReviewCommitMessage;
                        //this.btnUpdate.Text = "Confirm";
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        DisableFields(true, false);
                        LoadViewData(true);
                        if (canUpdate)
                        {
                            this.btnLookupUser.Enabled = this.btnLookupUser.Visible = false;
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnApprove.Enabled = this.btnApprove.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                            this.btnResetPassword.Enabled = this.btnResetPassword.Visible = false;
                            this.btnResetUserLogin.Enabled = this.btnResetUserLogin.Visible = false;
                            //this.lblInfoMessage.Text = Resources.CommonInfoMessages.ReviewCommitMessage;
                        }
                        //this.btnUpdate.Text = "Confirm";
                        break;
                    default:
                        DisableFields(false, false);
                        this.btnLookupUser.Enabled = this.btnLookupUser.Visible = false;
                        this.btnCreate.Enabled = this.btnCreate.Visible = false;
                        this.btnEdit.Enabled = this.btnEdit.Visible = false;
                        this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                        this.btnApprove.Enabled = this.btnApprove.Visible = false;

                        this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                        this.btnResetPassword.Enabled = this.btnResetPassword.Visible = false;
                        this.btnResetUserLogin.Enabled = this.btnResetUserLogin.Visible = false;
                        //this.btnUpdate.Text = UtilityClass.UppercaseFirst("EDIT");
                        break;
                }
            }

            CurrentPageLayout = toPageLayout;
        }

        private void EnableFields(bool isCreate)
        {
            this.ddlAuthConfig.Enabled = true;
            this.tbFirstName.Enabled = true;
            this.tbLastName.Enabled = true;
            this.tbUserName.Enabled = isCreate;
            this.tbEmail.Enabled = true;
            this.ddlUserStatus.Enabled = true;
            this.ddlUserLanguage.Enabled = true;
            this.ddlTimeZone.Enabled = true;

            this.lblPassword.Visible = isCreate;
            this.tbPassword.Visible = isCreate;
            this.tbPassword.Enabled = isCreate;
            this.revPassword.Enabled = isCreate;
            this.revPassword.Visible = isCreate;

            this.lblConfirmPassword.Visible = isCreate;
            this.tbConfirmPassword.Visible = isCreate;
            this.tbConfirmPassword.Enabled = isCreate;
            this.cvPasswordsComparator.Enabled = isCreate;
            this.cvPasswordsComparator.Visible = isCreate;

            this.cblGroups.Enabled = true;

            //this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            //this.btnEdit.Enabled = this.btnEdit.Visible = isCreate ? false : true;
            //this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            //this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
            this.btnBack.Visible = isCreate ? false : true;
            //this.btnResetUserLogin.Visible = false;
            //this.btnBackToIssuers.Visible = false;
            //this.btnBackToUsers.Visible = false;
            //this.btnResetPassword.Visible = false;          
        }

        private void DisableFields(bool IsConfirm, bool isCreate)
        {
            this.tbFirstName.Enabled = false;
            this.tbLastName.Enabled = false;
            this.tbUserName.Enabled = false;
            this.tbEmail.Enabled = false;
            this.ddlUserStatus.Enabled = false;
            this.ddlAuthConfig.Enabled = false;
            this.ddlUserLanguage.Enabled = false;
            this.ddlTimeZone.Enabled = false;

            this.lblPassword.Visible = isCreate ? true : false;
            this.tbPassword.Visible = isCreate ? true : false;
            this.tbPassword.Enabled = false;
            //this.tbPassword.ReadOnly = true;
            this.revPassword.Enabled = false;
            this.revPassword.Visible = false;

            this.lblConfirmPassword.Visible = isCreate ? true : false;
            this.tbConfirmPassword.Visible = isCreate ? true : false;
            this.tbConfirmPassword.Enabled = false;
            this.cvPasswordsComparator.Enabled = false;
            this.cvPasswordsComparator.Visible = false;

            this.cblGroups.Enabled = false;

            this.btnBack.Visible = IsConfirm ? true : false;
            //this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            //this.btnUpdate.Enabled = this.btnUpdate.Visible = isCreate ? false : true;
            //this.btnConfirm.Enabled = this.btnConfirm.Visible = IsConfirm;
            //this.btnResetUserLogin.Visible = IsConfirm ? false : true;

            //if (int.Parse(this.ddlLoginIssuer.SelectedValue) == 0)
            //{
            //    this.btnResetPassword.Visible = IsConfirm ? false : true;
            //}
            //else
            //{
            //    this.btnResetPassword.Visible = false;
            //}
        }

        private void enablePasswordResset()
        {
            this.lblConfirmPassword.Visible = true;
            this.lblPassword.Visible = true;
            this.tbPassword.Enabled = true;
            this.tbConfirmPassword.Enabled = true;
            this.tbPassword.Visible = true;
            this.tbConfirmPassword.Visible = true;
            this.revPassword.Enabled = true;
            this.cvPasswordsComparator.Enabled = true;
            //this.btnUpdate.Text = "Save Password";            
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Grabs info off the screen and populates a user object.
        /// </summary>
        /// <returns></returns>
        private user PopulateUserObject()
        {
            int userStatusId;
            if (int.TryParse(this.ddlUserStatus.SelectedValue, out userStatusId))
            {
                user userObj = new user();
                userObj.username = utility.UtilityClass.GetBytes(this.tbUserName.Text.Trim());

                if (Password != null)
                {
                    userObj.password = utility.UtilityClass.GetBytes(Password);
                }


                //ViewState[PasswordKey] = this.tbPassword.Text;
                userObj.first_name = utility.UtilityClass.GetBytes(this.tbFirstName.Text.Trim());
                userObj.last_name = utility.UtilityClass.GetBytes(this.tbLastName.Text.Trim());
                userObj.employee_id = utility.UtilityClass.GetBytes("");
                userObj.user_email = this.tbEmail.Text.Trim();
                userObj.user_status_id = userStatusId;
                int authentication_configuration_id = 0;

                int.TryParse(ddlAuthConfig.SelectedValue, out authentication_configuration_id);


                userObj.authentication_configuration_id = authentication_configuration_id;
                if (!IsIndiogUser())
                {
                    //Add random password
                    userObj.password = utility.UtilityClass.GetBytes("R@nd" + r.Next(999).ToString() + DateTime.Now.Second.ToString());
                }

                int userLanguageId;
                if (int.TryParse(this.ddlUserLanguage.SelectedValue, out userLanguageId))
                {
                    userObj.language_id = userLanguageId;
                }

                if (UserId != null)
                {
                    userObj.user_id = UserId.GetValueOrDefault();
                }
                else
                {
                    userObj.user_id = 0;
                    if (String.IsNullOrWhiteSpace(utility.UtilityClass.GetString(userObj.password)))
                        throw new ArgumentNullException("Password is empty");
                }



                TimeZoneInfo timeinfo = TimeZoneInfo.FindSystemTimeZoneById(ddlTimeZone.SelectedValue);

                userObj.time_zone_utcoffset = (timeinfo.BaseUtcOffset.Hours.ToString("00").Length > 2 ? timeinfo.BaseUtcOffset.Hours.ToString("00") : "+" + timeinfo.BaseUtcOffset.Hours.ToString("00")) + ":" + timeinfo.BaseUtcOffset.Minutes.ToString("00");
                userObj.time_zone_id = ddlTimeZone.SelectedValue;

                return userObj;
            }

            return null;
        }

        public bool IsIndiogUser()
        {
            int authentication_configuration_id = 0;

            int.TryParse(ddlAuthConfig.SelectedValue, out authentication_configuration_id);
            AuthConfigResult authresult = userMan.GetAuthConfiguration(authentication_configuration_id);

            var config = authresult.AuthConfigConnectionParams;

            bool indigoUser = config.Any(i => i.connection_parameter_id == null && i.auth_type_id == null);
            return indigoUser;
        }
        private List<int> PopulateUserGroupList()
        {
            List<int> userGroupList = new List<int>();

            if (GroupsViewList != null)
            {
                Dictionary<int, GroupsRolesResult> selectedUserGroups = new Dictionary<int, GroupsRolesResult>();
                selectedUserGroups = GroupsViewList;

                foreach (int items in selectedUserGroups.Keys)
                {
                    userGroupList.Add(items);
                }
            }

            return userGroupList;
        }

        /// <summary>
        /// Loops through the check box list and removes any groups from the view state that have not been checked.
        /// These selected groups are used when persisting to the database during Create/Update
        /// </summary>
        private void UpdateSelectedGroupsList()
        {
            //Before clearing out the list, check for changes then move on and load the list
            foreach (ListItem item in this.cblGroups.Items)
            {
                if (GroupsViewList != null && GroupsViewList.ContainsKey(int.Parse(item.Value)))
                {
                    if (!item.Selected)
                    {
                        GroupsViewList.Remove(int.Parse(item.Value));
                    }
                }
            }
        }

        /// <summary>
        /// Fetches groups based on issuer and role, and if branch is selected, then also branch.
        /// The groups are saved into the ViewState to be examined later if the checked status has changed.
        /// </summary>
        /// <param name="userId"></param>
        private void LoadGroupsForUser(long? userId, long? userpendingId)
        {
            UpdateSelectedGroupsList();
            this.cblGroups.Items.Clear();

            if (userId == null && UserId != null)
            {
                userId = UserId;
            }


            int issuerId;
            int roleTypeId;
            int branchId;
            if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) &&
                    int.TryParse(this.ddlRole.SelectedValue, out roleTypeId) &&
                    int.TryParse(this.ddlBranch.SelectedValue, out branchId))
            {
                int? selBranchId = branchId;
                if (branchId == -99)
                {
                    selBranchId = null;
                }

                Dictionary<int, List<UserGroupAdminResult>> results;

                results = userpendingId == null ? userMan.GetDictionaryUserGroupForUserAdmin(userId.GetValueOrDefault(), roleTypeId,
                                                                        issuerId,
                                                                        selBranchId) :
                                                                        userMan.GetUserGroupForPendingUserAdmin(userpendingId.GetValueOrDefault(),
                                                                        roleTypeId,
                                                                        issuerId,
                                                                        selBranchId);

                List<ListItem> displayGroups = new List<ListItem>();
                //List<UserGroupAdminResult> selectedGroups = new List<UserGroupAdminResult>();

                if (GroupsViewList != null)
                {
                    if (results.Count > 0)
                    {
                        foreach (var item in results[issuerId])
                        {
                            GroupsRolesResult groupRole = new GroupsRolesResult();
                            groupRole.user_group_name = item.user_group_name;
                            groupRole.user_role = this.ddlRole.SelectedItem.Text;
                            groupRole.issuer_code = this.ddlIssuer.SelectedItem.Text;
                            groupRole.issuer_name = "";

                            ListItem groupItem = new ListItem(item.user_group_name, item.user_group_id.ToString());
                            groupItem.Selected = item.is_in_group == 1 ? true : false;


                            if (GroupsViewList.ContainsKey(item.user_group_id))
                            {
                                groupItem.Selected = true;
                            }
                            else
                            {
                                GroupsViewList.Add(item.user_group_id, groupRole);
                            }

                            displayGroups.Add(groupItem);
                        }
                    }
                }
                else
                {
                    if (GroupsViewList == null)
                    {
                        GroupsViewList = new Dictionary<int, GroupsRolesResult>();
                    }
                    //Dictionary<int, GroupsRolesResult>  groupsViewList = new Dictionary<int, GroupsRolesResult>();
                    List<UserGroupAdminResult> groups = new List<UserGroupAdminResult>();

                    if (results.TryGetValue(issuerId, out groups))
                    {
                        foreach (var group in groups.Where(m => m.user_role_id == roleTypeId).OrderBy(m => m.user_group_name).ToList())
                        {
                            GroupsRolesResult groupRole = new GroupsRolesResult();
                            groupRole.user_group_name = group.user_group_name;
                            groupRole.user_role = this.ddlRole.SelectedItem.Text;
                            groupRole.issuer_code = this.ddlIssuer.SelectedItem.Text;
                            groupRole.issuer_name = "";
                            ListItem groupItem = new ListItem(group.user_group_name, group.user_group_id.ToString());

                            if (group.is_in_group == 1)
                            {
                                groupItem.Selected = true;
                            }

                            if (!GroupsViewList.ContainsKey(group.user_group_id))
                            {
                                GroupsViewList.Add(group.user_group_id, groupRole);
                            }
                            displayGroups.Add(groupItem);
                        }
                    }
                }

                this.cblGroups.Items.AddRange(displayGroups.OrderBy(m => m.Text).ToArray());
            }
        }

        private bool GroupsListValidation()
        {

            try
            {


                if (GroupsViewList.Count == 0)
                {
                    lblErrorMessage.Text = GetLocalResourceObject("ValidationGroups").ToString();
                    return false;
                }
                else
                {
                    return true;
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


        #endregion

        #region DB Persisting Methods
        /// <summary>
        /// Populate a user object and then persist to the database
        /// </summary>
        private void CreateUser()
        {
            try
            {
                user createUser = PopulateUserObject();

                if (createUser != null && pageLayout == PageLayout.CONFIRM_CREATE)
                {
                    string response = "";
                    long? newUserId;
                    if (userMan.CreateUser(createUser, UserId, PopulateUserGroupList(), out response, out newUserId))
                    {
                        //UserId = newUserId;
                        PendingUserId = newUserId;
                        //ViewState[PasswordKey] = null;
                        Password = null;
                        this.lblInfoMessage.Text = "useraudit should approve the record to save changes";

                        pageLayout = PageLayout.READ;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = response;
                    }
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

        /// <summary>
        /// Populate a user object and persist changes to the database
        /// </summary>
        private void UpdateUser()
        {
            try
            {
                user updateUser = PopulateUserObject();

                if (updateUser != null && pageLayout == PageLayout.CONFIRM_UPDATE)
                {
                    string response; long? newUserId;
                    if (userMan.UpdateUser(updateUser, PopulateUserGroupList(), out response, out newUserId))
                    {
                        Roles.Provider.ToIndigoRoleProvider().ClearRoles(utility.UtilityClass.GetString(updateUser.username));
                        this.lblInfoMessage.Text = "useraudit should approve the record to save changes";
                        PendingUserId = newUserId;
                        pageLayout = PageLayout.READ;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = response;
                    }
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

        /// <summary>
        /// Persist new password to database.
        /// </summary>
        private bool ResetPassword()
        {
            try
            {
                if (UserId != null)
                {
                    long userId = UserId.GetValueOrDefault();

                    if (pageLayout == PageLayout.RESET)
                    {

                        string response;
                        if (userMan.ResetUserPassword(userId, this.tbPassword.Text, out response))
                        {
                            lblInfoMessage.Text = "Password Updated Successfully.";
                            pageLayout = PageLayout.READ;
                            return true;
                        }
                        else
                        {
                            this.lblErrorMessage.Text = response;
                        }
                    }

                }
                else
                {
                    this.lblErrorMessage.Text = "No User Selected";
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

        /// <summary>
        /// Sets screen controls based on of an LDAP issuer has been selected.
        /// </summary>
        private void UpdateFormForLdapLookup(bool isBackButton)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";
            if (!isBackButton)
            {
                this.tbFirstName.Text = "";
                this.tbLastName.Text = "";
                this.tbEmail.Text = "";
            }
            int ldapConnectionId;
            if (int.TryParse(this.ddlAuthConfig.SelectedValue, out ldapConnectionId))
            {

                if (IsIndiogUser())
                {
                    this.btnLookupUser.Visible = this.btnLookupUser.Enabled = false;
                    this.btnUpdate.Enabled =
                        this.btnCreate.Enabled =
                        this.btnEdit.Enabled = true;
                    this.tbConfirmPassword.Enabled = this.tbConfirmPassword.Visible = this.lblPassword.Visible = true;
                    this.tbPassword.Enabled = this.tbPassword.Visible = this.lblConfirmPassword.Visible = true;
                    this.tbFirstName.Enabled = this.tbLastName.Enabled = this.tbEmail.Enabled = true;
                }
                else
                {
                    this.btnLookupUser.Visible = this.btnLookupUser.Enabled = true;
                    this.btnUpdate.Enabled =
                        this.btnCreate.Enabled =
                        this.btnEdit.Enabled = false;
                    this.tbConfirmPassword.Enabled = this.tbConfirmPassword.Visible = this.lblPassword.Visible = false;
                    this.tbPassword.Enabled = this.tbPassword.Visible = this.lblConfirmPassword.Visible = false;
                    this.tbFirstName.Enabled = this.tbLastName.Enabled = this.tbEmail.Enabled = false;


                }
            }
        }

        #endregion

        #region Page Events

        protected bool FindConnectiontype(int ldapConnectionId)
        {
            var ldapResult = _issuerMan.GetConnectionParameters().Where(i => i.connection_parameter_id == ldapConnectionId).SingleOrDefault();
            if (ldapResult != null && ldapResult.connection_parameter_type_id == 0)
                return true;
            else
                return false;

        }
        protected void ddlAuth_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormForLdapLookup(false);
        }

        protected void btnLookupUser_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";
            this.tbFirstName.Text = "";
            this.tbLastName.Text = "";
            this.tbEmail.Text = "";
            this.btnUpdate.Enabled = false;
            this.btnEdit.Enabled = false;
            this.btnCreate.Enabled = false;

            try
            {
                if (String.IsNullOrWhiteSpace(this.tbUserName.Text))
                {
                    this.lblInfoMessage.Text = GetLocalResourceObject("ValidationLDAPUsernameEmpty").ToString();
                }
                else
                {

                    int authConfigId;
                    if (int.TryParse(this.ddlAuthConfig.SelectedValue, out authConfigId))
                    {
                        string message;


                        var response = userMan.LdapLookup(this.tbUserName.Text.Trim(), "", "", authConfigId, out message);


                        if (!String.IsNullOrWhiteSpace(message))
                        {
                            this.lblErrorMessage.Text = message;
                        }
                        else
                        {
                            if (response != null)
                            {
                                this.lblInfoMessage.Text = GetLocalResourceObject("InfoLdapUserFound").ToString();

                                SetTextBox(tbFirstName, response.first_name);
                                SetTextBox(tbLastName, response.last_name);
                                SetTextBox(tbEmail, response.user_email);

                                if (CurrentPageLayout != null && CurrentPageLayout.Value == PageLayout.CREATE)
                                    this.btnCreate.Enabled = true;
                                else
                                    this.btnUpdate.Enabled = true;
                            }
                            else
                            {
                                this.lblInfoMessage.Text = GetLocalResourceObject("InfoLdapUserNotFound").ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                this.lblErrorMessage.Text = ex.Message;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_ADMIN")]
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (String.IsNullOrWhiteSpace(this.tbPassword.Text) && int.Parse(this.ddlAuthConfig.SelectedValue) < 0)
                    this.lblErrorMessage.Text = GetLocalResourceObject("ValidationNoPassword").ToString();
                else
                {

                    UpdateSelectedGroupsList();
                    if (GroupsListValidation())
                    {
                        lblErrorMessage.Text = string.Empty;
                        UpdateFormLayout(PageLayout.CONFIRM_CREATE);
                        this.lblInfoMessage.Text = Resources.CommonInfoMessages.ReviewCommitMessage;
                    }

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

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_ADMIN")]
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                var result = userMan.GetUserPendingForApproval(null, null, BranchStatus.ACTIVE, 0, null, tbUserName.Text, string.Empty, string.Empty, 1, 2000);
                if (result.Count > 0)
                {
                    this.lblErrorMessage.Text = "pending change needs approval for this user.";
                }
                else
                {
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    UpdateFormLayout(PageLayout.UPDATE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_AUDIT")]
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                UpdateFormLayout(PageLayout.CONFIRM_APPROVE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_AUDIT")]

        protected void btnReject_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                this.btnReject.Enabled = this.btnReject.Visible = false;

                UpdateFormLayout(PageLayout.CONFIRM_REJECT);
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
        [PrincipalPermission(SecurityAction.Demand, Role = "USER_ADMIN")]
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                UpdateSelectedGroupsList();

                if (GroupsListValidation())
                {
                    lblErrorMessage.Text = string.Empty;
                    UpdateFormLayout(PageLayout.CONFIRM_UPDATE);
                    this.lblInfoMessage.Text = Resources.CommonInfoMessages.ReviewCommitMessage;
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

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_ADMIN")]
        [PrincipalPermission(SecurityAction.Demand, Role = "USER_AUDIT")]
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
                        CreateUser();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        UpdateUser();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_APPROVE:
                        ApproveUser();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_REJECT:
                        RejectUser();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.RESET:
                        if (ResetPassword())
                        {
                            UpdateFormLayout(PageLayout.READ);
                        }
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

        private void RejectUser()
        {
            try
            {
                if (pageLayout == PageLayout.CONFIRM_REJECT)
                {
                    string response;
                    if (userMan.RejectUserRequest((long)PendingUserId, out response))
                    {
                        //Roles.Provider.ToIndigoRoleProvider().ClearRoles(utility.UtilityClass.GetString(updateUser.username));
                        this.lblInfoMessage.Text = "User Rejected Successfully.";
                        pageLayout = PageLayout.READ;

                        PendingUserId = null;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = response;
                    }
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

        private void ApproveUser()
        {
            try
            {
                if (pageLayout == PageLayout.CONFIRM_APPROVE)
                {
                    string response;
                    if (userMan.ApproveUser((long)PendingUserId, out response))
                    {
                        //Roles.Provider.ToIndigoRoleProvider().ClearRoles(utility.UtilityClass.GetString(updateUser.username));
                        this.lblInfoMessage.Text = "User Approved Successfully.";
                        pageLayout = PageLayout.READ;

                        PendingUserId = null;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = response;
                    }
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

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_ADMIN")]
        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            enablePasswordResset();
            CurrentPageLayout = PageLayout.RESET;

            this.revPassword.Enabled = true;
            this.cvPasswordsComparator.Enabled = true;
            this.btnCreate.Enabled = this.btnCreate.Visible = false;
            this.btnEdit.Enabled = this.btnEdit.Visible = false;
            this.btnApprove.Enabled = this.btnApprove.Visible = false;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
            this.btnResetPassword.Visible = false;
            this.btnResetUserLogin.Visible = false;
            this.btnBack.Visible = true;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_ADMIN")]
        protected void btnResetUserLogin_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblInfoMessage.Text = "";
                this.lblErrorMessage.Text = "";

                if (UserId != null)
                {
                    userMan.ResetUserLoginStatus(UserId.GetValueOrDefault());
                    this.lblInfoMessage.Text = GetLocalResourceObject("InfoResetSuccess").ToString();
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

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_AUDIT")]
        [PrincipalPermission(SecurityAction.Demand, Role = "USER_ADMIN")]
        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            if (CurrentPageLayout != null)
            {
                pageLayout = CurrentPageLayout.Value;
            }

            switch (pageLayout)
            {
                case PageLayout.RESET:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.UPDATE:
                    //LoadRestoreGroup();
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.DELETE:
                case PageLayout.CONFIRM_APPROVE:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.CONFIRM_CREATE:
                    UpdateFormLayout(PageLayout.CREATE);
                    UpdateFormForLdapLookup(true);
                    break;
                case PageLayout.CONFIRM_DELETE:
                    UpdateFormLayout(PageLayout.DELETE);
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    UpdateFormLayout(PageLayout.UPDATE);
                    UpdateFormForLdapLookup(true);
                    break;
                case PageLayout.READ:
                    if (SearchMode)
                    {
                        SessionWrapper.SearchUserMode = SearchMode;
                    }
                    SessionWrapper.UserSearchParameters = UserSearchParams;
                    if (this.UserId != null)
                        Response.Redirect("~\\webpages\\users\\UserList.aspx");
                    else
                        Response.Redirect("~\\webpages\\users\\UserPendingList.aspx");

                    break;
               

                default:
                    break;
            }
        }

        protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                int issuerId;

                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                {
                    updateBranchList(issuerId);
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

        protected void ddlBranch_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                LoadGroupsForUser(null, null);
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

        protected void ddlRole_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                LoadGroupsForUser(null, null);
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

        protected void cblGroups_OnSelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion

        private PageLayout? CurrentPageLayout
        {
            get
            {
                if (ViewState["CurrentPageLayout"] == null)
                    return null;
                else
                    return (PageLayout)ViewState["CurrentPageLayout"];

            }
            set
            {
                ViewState["CurrentPageLayout"] = value;
            }
        }

        private Dictionary<int, GroupsRolesResult> GroupsViewList
        {
            get
            {
                if (ViewState["GroupsViewList"] == null)
                    return null;
                else
                    return (Dictionary<int, GroupsRolesResult>)ViewState["GroupsViewList"];

            }
            set
            {
                ViewState["GroupsViewList"] = value;
            }
        }


        public long? PendingUserId
        {
            get
            {
                if (ViewState["PendingUserId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["PendingUserId"].ToString());
            }
            set
            {
                ViewState["PendingUserId"] = value;
            }
        }
        public long? UserId
        {
            get
            {
                if (ViewState["UserId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["UserId"].ToString());
            }
            set
            {
                ViewState["UserId"] = value;
            }
        }

        public string Password
        {
            get
            {
                if (ViewState["UserPassword"] == null)
                    return null;
                else
                    return ViewState["UserPassword"].ToString();
            }
            set
            {
                ViewState["UserPassword"] = value;
            }
        }
        public bool SearchMode
        {
            get
            {
                if (ViewState["SearchMode"] == null)
                    return false;
                else
                    return (bool)ViewState["SearchMode"];
            }
            set
            {
                ViewState["SearchMode"] = value;
            }
        }
        public UserSearchParameters UserSearchParams
        {
            get
            {
                if (ViewState["UserSearchParams"] != null)
                    return (UserSearchParameters)ViewState["UserSearchParams"];
                else
                    return null;
            }
            set
            {
                ViewState["UserSearchParams"] = value;
            }
        }


    }
}
