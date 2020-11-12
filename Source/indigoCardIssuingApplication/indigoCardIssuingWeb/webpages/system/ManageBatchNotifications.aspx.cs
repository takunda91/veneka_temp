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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class ManageBatchNotifications : BasePage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";
        private static readonly ILog log = LogManager.GetLogger(typeof(ManageBatchNotifications));
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly NotificationService _notificationservice = new NotificationService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };
        private readonly BatchManagementService _batchService = new BatchManagementService();
        int rowcount = 0;
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



            if (this.ddlStatus.Items.Count == 0)
            {
                this.lblBatchStatus.Visible = this.ddlStatus.Visible = this.rfvStatus.Visible = false;
            }
            else
            {
                this.lblBatchStatus.Visible = this.ddlStatus.Visible = this.rfvStatus.Visible = true;

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



        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
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
                        if (CreateNotification())
                        {
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessCreate").ToString();

                        }
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        if (DeleteNotification())
                            UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        if (UpdateNotification())
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

                    Server.Transfer("~\\webpages\\system\\BatchNotificationList.aspx");
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
        private bool DeleteNotification()
        {
            try
            {
                
                string responsemessage;
                bool response = _notificationservice.DeleteNotificationBatch(ReadControls(), out responsemessage);
                if (response)
                {

                    Server.Transfer("~\\webpages\\system\\BatchNotificationList.aspx?delete=" + this.ddlIssuer.SelectedValue);
                 

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


        private void PopulateFields()
        {
            try
            {
                if (SessionWrapper.Message !=null)
                {
                    NotificationMessages _message = (NotificationMessages)SessionWrapper.Message;
                    Mode = "Edit";

                    ddlIssuer.SelectedValue = _message.issuerid.ToString();
                    ddlStatus.SelectedValue = _message.distbatchstatusesid.ToString();
                    ddlBatchType.SelectedValue = _message.distbatchtypeid.ToString();
                    ddlchannel.SelectedValue = _message.channel_id.ToString();
                   
                  var notifications = _notificationservice.GetNotificationBatch(ReadControls());

                    pageLayout = PageLayout.READ;
                    ddlIssuer.Enabled =
                   ddlStatus.Enabled =
                   ddlBatchType.Enabled =
                   ddlchannel.Enabled = false;
                    BuildEmptyNotifications();
                    foreach (GridViewRow row in GrdNotificationFields.Rows)
                    {
                        Label lbllanguage = (Label)row.FindControl("lbllanguage");
                        Label lbllanguageid = (Label)row.FindControl("lbllanguageid");
                        TextBox txtnotificationtext = (TextBox)row.FindControl("txtnotificationtext");
                        TextBox txtSubjectText = (TextBox)row.FindControl("txtSubjectText");
                        TextBox txtfromaddress = (TextBox)row.FindControl("txtfromaddress");

                        var item = notifications.Where(i=>i.language_id == int.Parse(lbllanguageid.Text)).FirstOrDefault();
                        if (item != null)
                        {
                            lbllanguage.Text = item.language_name;
                            txtnotificationtext.Text = (item.notification_text);
                            txtSubjectText.Text = (item.subject_text);
                            txtfromaddress.Text = (item.from_address);
                        }

                    }
                    SessionWrapper.Message = null;
                }
                else
                {
                    //No username in session, set page layout to create.
                    pageLayout = PageLayout.CREATE;
                    BuildEmptyNotifications();
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
        private void BuildStatusDropDownList()
        {
            this.ddlStatus.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-1"));

            foreach (var batchStatus in _batchService.LangLookupDistBatchStatuses().OrderBy(o => o.language_text))
            {
                this.ddlStatus.Items.Add(new ListItem(batchStatus.language_text, batchStatus.lookup_id.ToString()));
            }
            this.ddlStatus.SelectedIndex = 0;


            ddlchannel.Items.Add(new ListItem("Email", "0"));
            ddlchannel.Items.Add(new ListItem("SMS", "1"));
            this.ddlchannel.SelectedIndex = 0;
        }
        private void LoadPageData()
        {
            BuildStatusDropDownList();

            this.ddlIssuer.Items.Clear();


            //Grab all the distinct issuers from the groups the user belongs to that match the role for this page.
            Dictionary<int, ListItem> issuersList = new Dictionary<int, ListItem>();
            Dictionary<int, RolesIssuerResult> issuers = new Dictionary<int, RolesIssuerResult>();
            PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuersList, out issuers);

            try
            {



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
            this.ddlBatchType.Enabled =
            this.ddlIssuer.Enabled =
            this.ddlStatus.Enabled = 
            this.ddlchannel.Enabled=
            GrdNotificationFields.Enabled= true;
            this.btnDelete.Enabled = this.btnDelete.Visible = false;
            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnEdit.Enabled = this.btnEdit.Visible = isCreate ? false : true;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
            this.btnBack.Visible = this.btnBack.Enabled = isCreate ? false : true;

        }

        private void DisableFields(bool IsConfirm, bool isCreate)
        {
            this.ddlBatchType.Enabled =
           this.ddlIssuer.Enabled =
           this.ddlStatus.Enabled =
           this.ddlchannel.Enabled=
           GrdNotificationFields.Enabled = false;
            this.btnDelete.Enabled = this.btnDelete.Visible = isCreate;
            this.btnBack.Visible = this.btnBack.Enabled = IsConfirm ? true : false;
            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = isCreate ? false : true;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = IsConfirm;

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
                if (Mode.ToUpper() == "Edit")
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

            this.ddlBatchType.Enabled =
            this.ddlIssuer.Enabled =
            this.ddlStatus.Enabled =
            this.ddlchannel.Enabled=
            this.GrdNotificationFields.Enabled = enabled;

            this.ddlBatchType.Visible =
            this.ddlIssuer.Visible =
            this.ddlStatus.Visible =
            this.ddlchannel.Enabled=
             this.GrdNotificationFields.Visible = visable;
        }

        #endregion

        #region Page Properties

        private string Mode
        {
            get;
            set;
        }
        public PageLayout? CurrentPageLayout
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
                if (ddlIssuer.SelectedIndex < 0)
            {
                lblErrorMessage.Text = "Status Required.";
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
        private void PopulateNotifications(List<notification_batchResult> result)
        {

            foreach (GridViewRow row in GrdNotificationFields.Rows)
            {
                DropDownList ddllanguage = (DropDownList)row.FindControl("ddllanguage");
                DropDownList ddlchannel = (DropDownList)row.FindControl("ddlchannel");
                TextBox txtnotificationtext = (TextBox)row.FindControl("txtnotificationtext");
                TextBox txtSubjectText = (TextBox)row.FindControl("txtSubjectText");
                TextBox txtfromaddress = (TextBox)row.FindControl("txtfromaddress");

            }
        }

        private NotificationMessages GetNotifications()
        {
            NotificationMessages obj = new NotificationMessages();
            obj = ReadControls();
            List<NotificationMessage> _list = new List<NotificationMessage>();
            foreach (GridViewRow row in GrdNotificationFields.Rows)
            {
                Label lbllanguageid = (Label)row.FindControl("lbllanguageid");
                TextBox txtnotificationtext = (TextBox)row.FindControl("txtnotificationtext");
                TextBox txtSubjectText = (TextBox)row.FindControl("txtSubjectText");
                TextBox txtfromaddress = (TextBox)row.FindControl("txtfromaddress");

                NotificationMessage messasge = new NotificationMessage();
                messasge.language_id = int.Parse(lbllanguageid.Text);
                messasge.notification_text = (txtnotificationtext.Text);
                messasge.subject_text =(txtSubjectText.Text);
                messasge.from_address = (txtfromaddress.Text);
                _list.Add(messasge);


            }

            obj.messages = _list.ToArray();
            return obj;
        }
        private bool CreateNotification()
        {
            lblErrorMessage.Text = string.Empty;

            try
            {


                string responsemessage = string.Empty;


                if (_notificationservice.InsertNotificationforBatch(GetNotifications(), out responsemessage))
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

        private bool UpdateNotification()
        {

           

            string responsemessage = string.Empty;


            if (_notificationservice.UpdateNotificationforBatch(GetNotifications(), out responsemessage))
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

        protected void BuildEmptyNotifications()
        {
            List<NotificationMessage> _list = new List<NotificationMessage>();

            
            foreach (var item in _userMan.GetLanguages())
            {
                NotificationMessage messasge = new NotificationMessage();
                messasge.language_id = item.id;
                messasge.notification_text = "";
                messasge.subject_text = "";

                switch (SessionWrapper.UserLanguage)
                {
                    case 0:
                        messasge.language_name= item.language_name;
                        break;
                    case 1:
                        messasge.language_name = item.language_name_fr;
                        break;
                    case 2:
                        messasge.language_name = item.language_name_pt;
                        break;
                    case 3:
                        messasge.language_name = item.language_name_sp;
                        break;
                    default:
                        messasge.language_name = item.language_name;
                        break;
                }
                _list.Add(messasge);
            }
            GrdNotificationFields.DataSource = _list;
            GrdNotificationFields.DataBind();
        }


        protected NotificationMessages ReadControls()
        {
            NotificationMessages message = new NotificationMessages();
            int issuerid = 0;
            int.TryParse(ddlIssuer.SelectedValue, out issuerid);
            message.issuerid = issuerid;
            int distbatchtypeid = 0;
            int.TryParse(ddlBatchType.SelectedValue, out distbatchtypeid);
            message.distbatchtypeid = distbatchtypeid;

            int distbatchstatusesid = 0;
            int.TryParse(ddlStatus.SelectedValue, out distbatchstatusesid);
            message.distbatchstatusesid = distbatchstatusesid;

            int channelid = 0;
            int.TryParse(ddlchannel.SelectedValue, out channelid);
            message.channel_id = channelid;


            return message;

        }
        #endregion

        protected void GrdNotificationFields_RowDataBound(object sender, GridViewRowEventArgs e)
        {

                    }
    }
}