using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.Security.Permissions;
using System.Web.Security;

namespace indigoCardIssuingWeb.webpages.users
{
    public partial class UserGroupViewForm : BasePage
    {
        private const string PageLayoutKey = "PageLayout";
        private const string RestoreUserGroupKey = "RestoreUserGroupKey";

        private static readonly ILog log = LogManager.GetLogger(typeof(UserGroupViewForm));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.USER_GROUP_ADMIN };
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
                List<ListItem> UserRolesLists = new List<ListItem>();
                foreach (var role in _userMan.LangLookupUserRoles())
                {
                    UserRolesLists.Add(new ListItem(role.language_text, role.lookup_id.ToString()));
                }
                ddlUserRoles.Items.AddRange(UserRolesLists.OrderBy(m => m.Text).ToArray());

                InitialisePage();

                //If this is not null then we've the page has been called from a search, load the data.
                if (SessionWrapper.SelectedUserGroupId != null)
                {
                    pageLayout = PageLayout.READ;
                    isView = true;

                    var result = _userMan.GetUserGroup(SessionWrapper.SelectedUserGroupId.GetValueOrDefault());

                    UserGroupId = result.user_group_id;
                    ViewState[RestoreUserGroupKey] = result;
                    this.tbUserGroupName.Text = result.user_group_name;

                    this.chkAllowCreate.Checked = result.can_create;
                    this.chkAllowUpdate.Checked = result.can_update;

                    this.chkMaskScreenPAN.Checked = result.mask_screen_pan;
                    this.chkMaskReportPAN.Checked = result.mask_report_pan;

                    this.ddlUserRoles.SelectedValue = result.user_role_id.ToString();
                    this.ddlIssuer.SelectedValue = result.issuer_id.ToString();
                    ValidateIssuerBranches(result.issuer_id, result.user_group_id);

                    if (result.all_branch_access)
                    {
                        this.rbtnlBranchAccess.SelectedValue = "1";
                    }
                    else
                    {
                        this.rbtnlBranchAccess.SelectedValue = "2";
                        this.pnlBranchPanel.Enabled = true;
                        this.pnlBranchPanel.Visible = true;
                    }
                }
                else//No data therefor must be a create.
                {
                    pageLayout = PageLayout.CREATE;
                    isView = false;
                }

                ViewState[PageLayoutKey] = pageLayout;
                SessionWrapper.SelectedUserGroupId = null;
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

        private void InitialisePage()
        {
            this.ddlIssuer.Items.Clear();
            this.chkblBranchList.Items.Clear();
            // this.ddlBranch.Items.Clear();

            Dictionary<int, ListItem> issuerList = Roles.Provider.ToIndigoRoleProvider()
                                                     .GetIssuersForRole(User.Identity.Name, userRolesForPage);

            try
            {
                this.ddlIssuer.Items.AddRange(issuerList.Values.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;

                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && User.Identity.Name != null)
                {
                    //ValidateIssuerBranches(issuerId, null);
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
            this.btnDelete.Enabled = this.btnDelete.Visible = false;

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
            if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.USER_GROUP_ADMIN, out hasRead, out hasUpdate, out hasCreate))
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
                            this.btnBack.Enabled = this.btnBack.Visible = false;
                            this.btnDelete.Enabled = this.btnDelete.Visible = false;
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
                            this.btnDelete.Enabled = this.btnDelete.Visible = true;
                        }
                        break;
                    case PageLayout.UPDATE:
                        EnableFields(false);
                        if (hasUpdate)
                        {
                            updateBranchList(int.Parse(this.ddlIssuer.SelectedValue), null);
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                            this.btnBack.Enabled = this.btnBack.Visible = true;
                            this.btnDelete.Enabled = this.btnDelete.Visible = false;
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
                            this.btnDelete.Enabled = this.btnDelete.Visible = false;
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                        }                       
                        
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        DisableFields(true);
                        if (hasUpdate)
                        {
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                            this.btnBack.Enabled = this.btnBack.Visible = true;
                            this.btnDelete.Enabled = this.btnDelete.Visible = false;
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDelete").ToString();
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
                            this.btnDelete.Enabled = this.btnDelete.Visible = false;
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
                        this.btnDelete.Enabled = this.btnDelete.Visible = false;
                        break;
                }
            }

            ViewState[PageLayoutKey] = toPageLayout;
        }

        private void ClearFields()
        {
            this.tbUserGroupName.Text = String.Empty;
            this.ddlUserRoles.SelectedIndex = 0;
            this.ddlIssuer.SelectedIndex = 0;
            this.rbtnlBranchAccess.SelectedValue = "1";            
            this.pnlBranchPanel.Enabled = false;
            this.chkMaskReportPAN.Checked = 
            this.chkMaskScreenPAN.Checked =
            this.chkAllowCreate.Checked = false;
            this.chkAllowUpdate.Checked = false; 
        }

        private void EnableFields(bool isCreate)
        {
            this.tbUserGroupName.Enabled = true;
            this.ddlUserRoles.Enabled = true;
            this.ddlIssuer.Enabled = true;
            this.rbtnlBranchAccess.Enabled = true;
            this.pnlBranchPanel.Enabled = true;
            this.chkMaskReportPAN.Enabled =
            this.chkMaskScreenPAN.Enabled =
            this.chkAllowCreate.Enabled = true;
            this.chkAllowUpdate.Enabled = true;            
        }

        private void DisableFields(bool IsConfirm)
        {
            this.tbUserGroupName.Enabled = false;
            this.ddlUserRoles.Enabled = false;
            this.ddlIssuer.Enabled = false;
            this.rbtnlBranchAccess.Enabled = false;
            this.pnlBranchPanel.Enabled = false;
            this.chkMaskReportPAN.Enabled =
            this.chkMaskScreenPAN.Enabled =
            this.chkAllowCreate.Enabled = false;
            this.chkAllowUpdate.Enabled = false; 
        }        

        /// <summary>
        /// Use this to restore a usergroup from view state.
        /// Usefull when user doesnt want to save changes they have made and clicks "back"
        /// </summary>
        private void LoadRestoreGroup()
        {
            if (RestoreUserGroup != null)
            {
                user_group group = RestoreUserGroup;

                this.tbUserGroupName.Text = group.user_group_name;
                this.ddlUserRoles.SelectedValue = group.user_role_id.ToString();
                this.ddlIssuer.SelectedValue = group.issuer_id.ToString();
                this.chkAllowCreate.Checked = group.can_create;
                this.chkAllowUpdate.Checked = group.can_update;
                this.chkMaskReportPAN.Checked = group.mask_report_pan;
                this.chkMaskScreenPAN.Checked = group.mask_screen_pan;
                ValidateIssuerBranches(group.issuer_id, group.user_group_id);

                if (group.all_branch_access)
                {
                    this.rbtnlBranchAccess.SelectedValue = "1";
                }
                else
                {
                    this.rbtnlBranchAccess.SelectedValue = "2";
                    this.pnlBranchPanel.Enabled = true;
                    this.pnlBranchPanel.Visible = true;
                }
            }
        }

        /// <summary>
        /// Fills in the branch list based on the issuer. If the user group object is populated from the DB it will also check
        /// the branches the user group belong to.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="userGroupId"></param>
        private void updateBranchList(int issuerId, int? userGroupId)
        {
            this.chkblBranchList.Items.Clear();

            var branches = _userMan.GetBranchesForUser(issuerId, userRolesForPage[0], null);

            List<BranchIdResult> selectedBranches = new List<BranchIdResult>();
            if (userGroupId != null)
            {
                selectedBranches = _userMan.GetBranchesForUserGroup(userGroupId.GetValueOrDefault());
            }
            else if (UserGroupId != null)
            {
                selectedBranches = _userMan.GetBranchesForUserGroup(UserGroupId.Value);
            }

            if (branches.Count > 0)
            {
                List<ListItem> branchList = new List<ListItem>();

                foreach (var item in branches)//Convert branches in item list.
                {
                    ListItem listItem = new ListItem(string.Format("{0} {1} ", new object[] { item.branch_code, item.branch_name }), item.branch_id.ToString());

                    foreach (var branch in selectedBranches)//Check those that have been selected from the DB.
                    {
                        if (branch.branch_id == item.branch_id)
                        {
                            listItem.Selected = true;
                            break;
                        }
                    }

                    if (isView && listItem.Selected)
                    {
                        branchList.Add(listItem);
                    }
                    else if (!isView)
                    {
                        branchList.Add(listItem);
                    }
                }

                if (branchList.Count > 0)
                {
                    chkblBranchList.Items.AddRange(branchList.OrderBy(m=>m.Text).ToArray());
                }
            }
        }

        /// <summary>
        /// Depending on the issuer certain compnenets on the page will be enabled/disabled/visible/hidden.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        private bool ValidateIssuerBranches(int issuerId, int? userGroupId)
        {
            this.rbtnlBranchAccess.SelectedValue = "1";

            if (issuerId == -1)
            {
                this.rbtnlBranchAccess.Enabled = false;
                this.pnlBranchPanel.Enabled = false;
                this.pnlBranchPanel.Enabled = false;
                return false;
            }
            else
            {
                //this.rbtnlBranchAccess.Visible = true;
                updateBranchList(issuerId, userGroupId);
                return true;
            }
        }
        #endregion

        #region Helper Methods for Persisting Methods
        /// <summary>
        /// Populate a user group object from info on page.
        /// </summary>
        /// <returns>If null means something went wrong</returns>
        private user_group PopulateUserGroup()
        {
            int issuerId;
            int userRoleId;

            if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) &&
                 int.TryParse(this.ddlUserRoles.SelectedValue, out userRoleId))
            {
                user_group userGroup = new user_group();

                if (UserGroupId != null)
                {
                    userGroup.user_group_id = UserGroupId.Value;
                }

                userGroup.user_group_name = this.tbUserGroupName.Text;
                userGroup.issuer_id = issuerId;
                userGroup.user_role_id = userRoleId;
                userGroup.can_read = true;
                userGroup.can_create = this.chkAllowCreate.Checked;
                userGroup.can_update = this.chkAllowUpdate.Checked;

                userGroup.mask_report_pan = this.chkMaskReportPAN.Checked;
                userGroup.mask_screen_pan = this.chkMaskScreenPAN.Checked;


                if (this.rbtnlBranchAccess.SelectedValue.Equals("1"))
                {

                    userGroup.all_branch_access = true;
                }
                else
                {
                    userGroup.all_branch_access = false;
                }

                return userGroup;
            }

            return null;
        }

        /// <summary>
        /// Gets a list of selected branches from the page.
        /// </summary>
        /// <returns></returns>
        private List<int> PopulateBranchList()
        {
            List<int> branchIdList = new List<int>();

            if (this.rbtnlBranchAccess.SelectedValue.Equals("2"))
            {
                foreach (ListItem item in this.chkblBranchList.Items)
                {
                    if (item.Selected)
                    {
                        int branchId;

                        if (int.TryParse(item.Value, out branchId))
                        {
                            branchIdList.Add(branchId);
                        }
                        else
                        {
                            throw new ArgumentException("Branch ID is not a valid integer value");
                        }
                    }
                }
            }

            return branchIdList;
        }
        #endregion

        #region DB Persisting Methods
        /// <summary>
        /// Persists new user group to the DB
        /// </summary>
        /// <returns></returns>
        private bool CreateUserGroup()
        {
            user_group userGroup = PopulateUserGroup();

            if (userGroup != null)
            {
                int userGroupId;
                string result;
                if (_userMan.CreateUserGroup(userGroup, PopulateBranchList(), out userGroupId, out result))
                {
                    UserGroupId = userGroupId;
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
            user_group userGroup = PopulateUserGroup();

            if (userGroup != null)
            {
                string result;

                if (_userMan.UpdateUserGroup(userGroup, PopulateBranchList(), out result))
                {
                    RestoreUserGroup = null;
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

        private bool DeleteUserGroup()
        {
            user_group userGroup = PopulateUserGroup();

            if (UserGroupId != null)
            {
                string responseMessage = String.Empty;
                if (_userMan.DeleteUserGroup(UserGroupId.Value, out responseMessage))
                {
                    ClearFields();
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteSuccess").ToString();
                    return true;
                }
                else
                {
                    this.lblErrorMessage.Text = responseMessage;
                }
            }

            return false;
        }
        #endregion

        #region PAGE EVENTS
        [PrincipalPermission(SecurityAction.Demand, Role = "USER_GROUP_ADMIN")]
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

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_GROUP_ADMIN")]
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                isView = false;
                RestoreUserGroup = PopulateUserGroup();
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

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_GROUP_ADMIN")]
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

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_GROUP_ADMIN")]
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

        [PrincipalPermission(SecurityAction.Demand, Role = "USER_GROUP_ADMIN")]
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
                        if(CreateUserGroup())
                            UpdateFormLayout(PageLayout.READ);
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        if(DeleteUserGroup())
                            UpdateFormLayout(PageLayout.CREATE);
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        if(UpdateUserGroup())
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
                    LoadRestoreGroup();
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

        protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            int issuerId;
            if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
            {
                ValidateIssuerBranches(issuerId, null);
            }
        }

        protected void rbtnlBranchAccess_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.rbtnlBranchAccess.SelectedValue.Equals("1"))
            {
                this.pnlBranchPanel.Enabled = false;
                this.pnlBranchPanel.Visible = false;
            }
            else
            {
                this.pnlBranchPanel.Enabled = true;
                this.pnlBranchPanel.Visible = true;

                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                {
                    updateBranchList(issuerId, UserGroupId);
                }
            }
        }

        protected void btnRemoveUserGroup_Click(object sender, EventArgs e)
        {

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

        private int? UserGroupId
        {
            get
            {
                if (ViewState["UserGroupId"] == null)
                    return null;
                else
                    return (int)ViewState["UserGroupId"];
            }
            set
            {
                ViewState["UserGroupId"] = value;
            }
        }

        private user_group RestoreUserGroup
        {
            get
            {
                if (ViewState["RestoreUserGroup"] == null)
                    return null;
                else
                    return (user_group)ViewState["RestoreUserGroup"];
            }
            set
            {
                ViewState["RestoreUserGroup"] = value;
            }
        }
    }
}