using System;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using System.Web.Security;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.issuer
{
    public partial class IssuerManagement : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IssuerManagement));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ISSUER_ADMIN };

        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly UserManagementService userMan = new UserManagementService();

        #region PAGE LOAD
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadIssuerDetails();
            }
        }

        private void LoadIssuerDetails()
        {
            try
            {
                List<ListItem> issuerStatusList = new List<ListItem>();

                foreach (var status in _issuerMan.LangLookupIssuerStatuses())
                {
                    issuerStatusList.Add(new ListItem(status.language_text, status.lookup_id.ToString()));
                }
                this.ddlIssuerStatus.Items.AddRange(issuerStatusList.OrderBy(m => m.Text).ToArray());

                List<ListItem> issuerLanguageList = new List<ListItem>();
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

                    issuerLanguageList.Add(new ListItem(text, id));
                }
                this.ddlIssuerLanguage.Items.AddRange(issuerLanguageList.OrderBy(m => m.Text).ToArray());

                //Fetch List of countries
                var countries = _issuerMan.GetCountries();
                List<ListItem> issuercountriesList = new List<ListItem>();
                foreach (var item in countries)
                {
                    issuercountriesList.Add(new ListItem(item.country_name, item.country_id.ToString()));
                }
                this.ddlIssuerCountry.Items.AddRange(issuercountriesList.OrderBy(m => m.Text).ToArray());

                //Fetch Available Interfaces
                LoadAvailableInterfaces();


                //Fetch Connection Parameters
                var connResult = _issuerMan.GetConnectionParameters();

                //HSM
                List<ListItem> connList3 = new List<ListItem>();
                foreach (var item in connResult)
                {
                    if (item.connection_parameter_type_id == 2 || item.connection_parameter_type_id == 0)
                        connList3.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                }
                this.ddlProdHSM.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlProdHSM.Items.AddRange(connList3.OrderBy(m => m.Text).ToArray());

                List<ListItem> connList33 = new List<ListItem>();
                foreach (var item in connResult)
                {
                    if (item.connection_parameter_type_id == 2)
                        connList33.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                }
                this.ddlIssueHSM.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlIssueHSM.Items.AddRange(connList33.OrderBy(m => m.Text).ToArray());

                List<ListItem> connList4 = new List<ListItem>();
                foreach (var item in connResult)
                {
                    if (item.connection_parameter_type_id == 0)
                        connList4.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                }
                this.ddlprodNotification.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlprodNotification.Items.AddRange(connList4.OrderBy(m => m.Text).ToArray());

                List<ListItem> connList5 = new List<ListItem>();
                foreach (var item in connResult)
                {
                    if (item.connection_parameter_type_id == 0)
                        connList5.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                }
                this.ddlIssuerNotification.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlIssuerNotification.Items.AddRange(connList5.OrderBy(m => m.Text).ToArray());

                if (SessionWrapper.ManageIssuerID == null) //Create new issuer
                {
                    CurrentPageLayout = PageLayout.CREATE;

                    this.chkEnableInstant.Checked =
                    this.chkEnableCentralised.Checked = false;
                    this.tbIssuerName.Text = String.Empty;
                    this.tbIssuerCode.Text = String.Empty;
                    this.ddlIssuerStatus.SelectedValue = ((int)InstitutionStatus.ACTIVE).ToString();

                    this.chkMakerChecker.Checked = false;
                    this.chkShowsCard.Checked = false;
                    this.pnlPinCard.Visible = false;
                    this.chkDeletePinFile.Checked = false;

                    this.chkEnableInstantPin.Checked = false;
                    this.chkAuthorisePinIssue.Checked = false;
                    this.chkBackOfficeAuth.Checked = false;
                    this.chkAuthorisePinReissue.Checked = false;
                    this.chkAllowBranches.Checked = false;

                    this.ddlProdHSM.SelectedValue =
                    this.ddlIssueHSM.SelectedValue = "-99";
                    this.chkEnableToken.Checked = false;
                    this.tbexpirydate.Text = string.Empty;
                    this.tbMessageBody.Text = string.Empty;
                    this.tbMaxNumOfPinSend.Text = string.Empty;
                    this.tbDeletePinBlockAfter.Text = string.Empty;
                    this.tbremotetoken.Text = Guid.NewGuid().ToString();
                }
                else //Edit issuer
                {
                    int issuerId = SessionWrapper.ManageIssuerID.Value;
                    SessionWrapper.ManageIssuerID = null;

                    if (issuerId > -99)
                    {
                        CurrentPageLayout = PageLayout.READ;
                        IssuerId = issuerId;
                        var issuer = _issuerMan.GetIssuer(issuerId);
                        var pin_message = _issuerMan.GetIssuerPinMessage(issuerId);

                        this.chkEnableInstant.Checked = issuer.Issuer.instant_card_issue_YN;
                        this.chkEnableCentralised.Checked = issuer.Issuer.classic_card_issue_YN;
                        this.tbIssuerName.Text = issuer.Issuer.issuer_name;
                        this.tbIssuerCode.Text = issuer.Issuer.issuer_code;
                        //this.tbCardsFileLocation.Text = issuer.Issuer.cards_file_location;
                        this.ddlIssuerStatus.SelectedValue = issuer.Issuer.issuer_status_id.ToString();
                        this.ddlIssuerLanguage.SelectedValue = issuer.Issuer.language_id.ToString();
                        this.ddlIssuerCountry.SelectedValue = issuer.Issuer.country_id.ToString();
                        this.tbremotetoken.Text = issuer.Issuer.remote_token.ToString();
                        this.tbMessageBody.Text = pin_message.pin_notification_message;
                        this.tbDeletePinBlockAfter.Text = pin_message.pin_block_delete_days.ToString();
                        this.tbMaxNumOfPinSend.Text = pin_message.max_number_of_pin_send.ToString();
                        if (issuer.Issuer.remote_token_expiry != null)
                        {
                            chkEnableToken.Checked = true;
                            this.tbexpirydate.Text = issuer.Issuer.remote_token_expiry.Value.ToString(DATE_FORMAT.ToString());
                        }
                        //Production
                        foreach (var item in issuer.ProdInterfaces)
                        {
                            //CBS = 0
                            //CMS = 1
                            //HSM = 2
                            //CPS = 3
                            switch (item.interface_type_id)
                            {
                                case 2:
                                    this.ddlProdHSM.SelectedValue = item.connection_parameter_id.ToString();
                                    this.ddlInterfaceHSM.SelectedValue = item.interface_guid ?? "-99";
                                    break;
                                case 7:
                                    this.ddlprodNotification.SelectedValue = item.connection_parameter_id.ToString();
                                    this.ddlInterfaceNotification.SelectedValue = item.interface_guid ?? "-99";
                                    break;
                                default: break;
                            }
                        }

                        //Issueing
                        foreach (var item in issuer.IssueInterfaces)
                        {
                            //CBS = 0
                            //CMS = 1
                            //HSM = 2
                            //CPS = 3
                            switch (item.interface_type_id)
                            {
                                case 2:
                                    this.ddlIssueHSM.SelectedValue = item.connection_parameter_id.ToString();
                                    break;
                                case 7:
                                    this.ddlIssuerNotification.SelectedValue = item.connection_parameter_id.ToString();
                                    break;
                                default: break;
                            }
                        }

                        this.chkEnableInstantPin.Checked = issuer.Issuer.enable_instant_pin_YN;
                        this.chkBackOfficeAuth.Checked = issuer.Issuer.back_office_pin_auth_YN;
                        this.chkAuthorisePinIssue.Checked = issuer.Issuer.authorise_pin_issue_YN;
                        this.chkAuthorisePinReissue.Checked = issuer.Issuer.authorise_pin_reissue_YN;
                        this.chkAllowBranches.Checked = issuer.Issuer.allow_branches_to_search_cards??false ;
                        this.chkMakerChecker.Checked = issuer.Issuer.maker_checker_YN;
                        this.chkShowsCard.Checked = issuer.Issuer.card_ref_preference;
                        //this.chkDeletePinFile.Checked = issuer.Issuer.delete_pin_file_YN;
                    }
                }

                SetControls(false);
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }
        private void SetConnectionDropDowns(List<ConnectionParamsResult> connResult, DropDownList dropDown, List<int> excludedConfigTypeIds)
        {
            List<ListItem> connList22 = new List<ListItem>();
            foreach (var item in connResult.Where(w => !excludedConfigTypeIds.Any(exc => exc == w.connection_parameter_type_id)))
            {
                connList22.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
            }
            dropDown.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            dropDown.Items.AddRange(connList22.OrderBy(m => m.Text).ToArray());
        }
        private void LoadAvailableInterfaces()
        {
            var availInterfaces = _issuerMan.ListAvailiableIntegrationInterfaces();

            //HSM
            List<ListItem> interfaces3 = new List<ListItem>();
            foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == 2))
            {
                interfaces3.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
            }
            this.ddlInterfaceHSM.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            this.ddlInterfaceHSM.Items.AddRange(interfaces3.OrderBy(m => m.Text).ToArray());



            //HSM
            List<ListItem> interfaces4 = new List<ListItem>();
            foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == 7))
            {
                interfaces4.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
            }
            this.ddlInterfaceNotification.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            this.ddlInterfaceNotification.Items.AddRange(interfaces4.OrderBy(m => m.Text).ToArray());
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Helper method used to set fields based on PageLaout.
        /// </summary>
        /// <param name="hideAll"></param>
        private void SetControls(bool hideAll)
        {
            this.btnEdit.Visible = this.btnEdit.Enabled = false;
            this.btnCreate.Visible = this.btnCreate.Enabled = false;
            this.btnUpdateIssuer.Visible = this.btnUpdateIssuer.Enabled = false;
            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
            this.btnCancel.Visible = this.btnCancel.Enabled = false;
            this.btnBack.Visible = this.btnBack.Enabled = true;

            bool enabled = false;
            bool visable = hideAll ? false : true;

            bool hasRead;
            bool hasUpdate;
            bool hasCreate;
            if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.ISSUER_ADMIN, out hasRead, out hasUpdate, out hasCreate))
            {
                if (IssuerId != null && IssuerId < 0)
                {
                    this.btnEdit.Visible = this.btnEdit.Enabled = false;
                    this.btnCreate.Visible = this.btnCreate.Enabled = false;
                    this.btnUpdateIssuer.Visible = this.btnUpdateIssuer.Enabled = false;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                    this.btnCancel.Visible = this.btnCancel.Enabled = false;
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
                                this.btnUpdateIssuer.Visible = this.btnUpdateIssuer.Enabled = false;
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
                                this.btnUpdateIssuer.Visible = this.btnUpdateIssuer.Enabled = false;
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
                                this.btnUpdateIssuer.Visible = this.btnUpdateIssuer.Enabled = false;
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
                                this.btnUpdateIssuer.Visible = this.btnUpdateIssuer.Enabled = true;
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
                                this.btnUpdateIssuer.Visible = this.btnUpdateIssuer.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                                //this.btnCancel.Visible = this.btnCancel.Enabled = false;
                                this.btnBack.Visible = this.btnBack.Enabled = true;
                            }
                            break;
                    }
                }
            }

            this.tbIssuerName.Enabled =
                this.tbremotetoken.Enabled =
                this.tbexpirydate.Enabled =
                this.chkEnableToken.Enabled =
                    this.chkEnableCentralised.Enabled =
                    this.chkEnableInstant.Enabled =
                    this.tbIssuerCode.Enabled =
                    this.chkShowsCard.Enabled =
                    this.chkDeletePinFile.Enabled =
                    this.chkMakerChecker.Enabled =
                    this.chkEnableInstantPin.Enabled =
                    this.chkAuthorisePinIssue.Enabled =
                    this.chkBackOfficeAuth.Enabled =
                    this.chkAuthorisePinReissue.Enabled =
                    this.chkAllowBranches.Enabled =
                    this.ddlProdHSM.Enabled =
                    this.ddlIssueHSM.Enabled =
                    this.ddlIssuerStatus.Enabled =
                    this.ddlIssuerLanguage.Enabled =
                    this.ddlIssuerCountry.Enabled =
                    this.ddlInterfaceHSM.Enabled =
                    this.ddlInterfaceNotification.Enabled =
                    this.ddlIssuerNotification.Enabled =
                    this.ddlprodNotification.Enabled =
                    this.reqIssuerCode.Enabled =
                    this.reqIssuerName.Enabled =
                    this.tbMessageBody.Enabled =
                    this.tbMaxNumOfPinSend.Enabled = 
                    this.tbDeletePinBlockAfter.Enabled = enabled;

            this.tbIssuerName.Visible =
                 this.tbremotetoken.Visible =
                this.tbexpirydate.Visible =
                this.chkEnableToken.Visible =
                    this.chkEnableCentralised.Visible =
                    this.chkEnableInstant.Visible =
                    this.tbIssuerCode.Visible =
                    this.chkShowsCard.Visible =
                    this.chkDeletePinFile.Visible =
                    this.chkEnableInstantPin.Visible =
                    this.chkAuthorisePinIssue.Visible =
                    this.chkBackOfficeAuth.Visible =
                    this.chkAuthorisePinReissue.Visible =
                    this.chkAllowBranches.Visible =
                    this.chkMakerChecker.Visible =
                    this.ddlProdHSM.Visible =
                    this.ddlIssueHSM.Visible =
                    this.ddlInterfaceHSM.Visible =
                     this.ddlInterfaceNotification.Visible =
                    this.ddlIssuerNotification.Visible =
                    this.ddlprodNotification.Visible =
                    this.ddlIssuerStatus.Visible =
                    this.ddlIssuerLanguage.Visible =
                    this.tbMessageBody.Visible =
                    this.tbMaxNumOfPinSend.Visible =
                    this.tbDeletePinBlockAfter.Visible =
                    this.ddlIssuerCountry.Visible = visable;

        }

        /// <summary>
        /// Generate an issuer object from fields.
        /// </summary>
        /// <returns></returns>
        private IssuerResult BuildIssuer()
        {
            IssuerResult rtnIssuer = new IssuerResult();
            CardIssuanceService.issuer issuer = new CardIssuanceService.issuer();
            if (IssuerId != null)
            {
                issuer.issuer_id = IssuerId.Value;
            }

            issuer.classic_card_issue_YN = this.chkEnableCentralised.Checked;
            issuer.instant_card_issue_YN = this.chkEnableInstant.Checked;
            issuer.issuer_code = this.tbIssuerCode.Text;
            issuer.issuer_name = this.tbIssuerName.Text;
            issuer.issuer_status_id = int.Parse(this.ddlIssuerStatus.SelectedValue);
            issuer.maker_checker_YN = this.chkMakerChecker.Checked;
            issuer.language_id = int.Parse(this.ddlIssuerLanguage.SelectedValue);
            issuer.country_id = int.Parse(this.ddlIssuerCountry.SelectedValue);
            issuer.card_ref_preference = this.chkShowsCard.Checked;
            issuer.enable_instant_pin_YN = this.chkEnableInstantPin.Checked;
            issuer.authorise_pin_issue_YN = this.chkAuthorisePinIssue.Checked;
            issuer.back_office_pin_auth_YN = this.chkBackOfficeAuth.Checked;
            issuer.authorise_pin_reissue_YN = this.chkAuthorisePinReissue.Checked;
            issuer.allow_branches_to_search_cards = this.chkAllowBranches.Checked;
            

            rtnIssuer.Issuer = issuer;
            

            List<issuer_interface> prodInterfaces = new List<issuer_interface>();
            if (!this.ddlProdHSM.SelectedValue.Equals("-99"))
            {
                issuer_interface cbInterface = new issuer_interface();
                cbInterface.connection_parameter_id = int.Parse(this.ddlProdHSM.SelectedValue);
                cbInterface.interface_type_id = 2;
                cbInterface.issuer_id = rtnIssuer.Issuer.issuer_id;
                if (!this.ddlInterfaceHSM.SelectedValue.Equals("-99"))
                    cbInterface.interface_guid = this.ddlInterfaceHSM.SelectedValue;

                prodInterfaces.Add(cbInterface);
            }

            List<issuer_interface> issueInterfaces = new List<issuer_interface>();

            if (!this.ddlIssueHSM.SelectedValue.Equals("-99"))
            {
                issuer_interface cbInterface = new issuer_interface();
                cbInterface.connection_parameter_id = int.Parse(this.ddlIssueHSM.SelectedValue);
                cbInterface.interface_type_id = 2;
                cbInterface.issuer_id = rtnIssuer.Issuer.issuer_id;
                if (!this.ddlInterfaceHSM.SelectedValue.Equals("-99"))
                    cbInterface.interface_guid = this.ddlInterfaceHSM.SelectedValue;

                issueInterfaces.Add(cbInterface);
            }

            if (!this.ddlprodNotification.SelectedValue.Equals("-99"))
            {
                issuer_interface cbInterface = new issuer_interface();
                cbInterface.connection_parameter_id = int.Parse(this.ddlprodNotification.SelectedValue);
                cbInterface.interface_type_id = 7;
                cbInterface.issuer_id = rtnIssuer.Issuer.issuer_id;
                if (!this.ddlInterfaceNotification.SelectedValue.Equals("-99"))
                    cbInterface.interface_guid = this.ddlInterfaceNotification.SelectedValue;

                prodInterfaces.Add(cbInterface);
            }


            if (!this.ddlIssuerNotification.SelectedValue.Equals("-99"))
            {
                issuer_interface cbInterface = new issuer_interface();
                cbInterface.connection_parameter_id = int.Parse(this.ddlIssuerNotification.SelectedValue);
                cbInterface.interface_type_id = 7;
                cbInterface.issuer_id = rtnIssuer.Issuer.issuer_id;
                if (!this.ddlInterfaceNotification.SelectedValue.Equals("-99"))
                    cbInterface.interface_guid = this.ddlInterfaceNotification.SelectedValue;

                issueInterfaces.Add(cbInterface);
            }
            rtnIssuer.Issuer.remote_token = Guid.Parse(tbremotetoken.Text);
            if (!chkEnableToken.Checked)
            {
                rtnIssuer.Issuer.remote_token_expiry = null;
            }
            else
            {
                rtnIssuer.Issuer.remote_token_expiry = DateTime.ParseExact(this.tbexpirydate.Text, DATE_FORMAT, null, DateTimeStyles.None);

            }
            rtnIssuer.ProdInterfaces = prodInterfaces.ToArray();
            rtnIssuer.IssueInterfaces = issueInterfaces.ToArray();
            rtnIssuer.max_number_of_pin_send = int.Parse(this.tbMaxNumOfPinSend.Text);
            rtnIssuer.pin_block_delete_days = int.Parse(this.tbDeletePinBlockAfter.Text);
            //rtnIssuer.no = notificationprodInterfaces.ToArray();
            //rtnIssuer.IssueInterfaces = notificationissueInterfaces.ToArray();

            return rtnIssuer;
        }

        /// <summary>
        /// Makes sure that all the settings have been set correctly.
        /// </summary>
        /// <returns>True of all validations have passed.</returns>
        private bool ValidateSettings()
        {
            bool rtnVal = true;
            int number_of_send_req;
            int pin_block_delete;

            
            if (int.TryParse(this.tbDeletePinBlockAfter.Text, out pin_block_delete) &&
                int.TryParse(this.tbMaxNumOfPinSend.Text, out number_of_send_req))
            {
                rtnVal = true;
            }
            else
            {
           
                this.lblErrorMessage.Text = "Max number of times pin can be sent and pin block delete days must be integer numbers..";
                this.lblInfoMessage.Text = string.Empty;
            
            rtnVal = false;
            }

                return rtnVal;
        }

        #endregion

        #region Page Events
        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnCreate_Click(object sender, EventArgs e)
        {
          
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                this.lblErrorMessage.Text = "";
                CurrentPageLayout = PageLayout.CONFIRM_CREATE;
                SetControls(false);
           
          
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";
            CurrentPageLayout = PageLayout.UPDATE;
            SetControls(false);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnUpdateIssuer_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
            this.lblErrorMessage.Text = "";
            CurrentPageLayout = PageLayout.CONFIRM_UPDATE;
            SetControls(false);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";
            try
            {
                if (ValidateSettings())
                {
                    IssuerResult issuerResult = null;
                    string result = "";
                    string pin_notification_message = this.tbMessageBody.Text;
                    if(pin_notification_message == null || pin_notification_message == "")
                    {
                        pin_notification_message = "Dear @customername, your pin request for account @account has been processed. Your new pin is @pin.";
                    }

                    if (CurrentPageLayout == PageLayout.CONFIRM_CREATE)
                    {
                        if (_issuerMan.CreateIssuer(BuildIssuer(), pin_notification_message,  out issuerResult, out result))
                        {
                            IssuerId = issuerResult.Issuer.issuer_id;
                            this.lblInfoMessage.Text = "Issuer Created Successfully";
                            CurrentPageLayout = PageLayout.READ;
                            SetControls(false);
                        }
                        else
                        {
                            this.lblErrorMessage.Text = result;
                        }
                    }
                    else if (CurrentPageLayout == PageLayout.CONFIRM_UPDATE)
                    {
                        if (_issuerMan.UpdateIssuer(BuildIssuer(), pin_notification_message, out result))
                        {
                            this.lblInfoMessage.Text = "Issuer Updated Successfully";
                            CurrentPageLayout = PageLayout.READ;
                            SetControls(false);
                        }
                        else
                        {
                            this.lblErrorMessage.Text = result;
                        }
                    }
                }


                //SetControls(false);
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
                if (CurrentPageLayout == PageLayout.CONFIRM_CREATE)
                {
                    CurrentPageLayout = PageLayout.CREATE;
                }
                else if (CurrentPageLayout == PageLayout.CONFIRM_UPDATE)
                {
                    CurrentPageLayout = PageLayout.UPDATE;
                }
                else if (CurrentPageLayout == PageLayout.UPDATE)
                {
                    CurrentPageLayout = PageLayout.READ;
                }

                SetControls(false);
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            switch (CurrentPageLayout)
            {
                case PageLayout.READ:
                    Server.Transfer("~\\webpages\\issuer\\IssuerList.aspx");
                    break;
                case PageLayout.UPDATE:
                    CurrentPageLayout = PageLayout.READ;
                    SetControls(false);
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    CurrentPageLayout = PageLayout.UPDATE;
                    SetControls(false);
                    break;
                case PageLayout.CONFIRM_CREATE:
                    CurrentPageLayout = PageLayout.CREATE;
                    SetControls(false);
                    break;
            }
        }
        #endregion

        #region private_fields

        private void GenerateErrorMessage(string message)
        {
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Text += message + "<br/>";
        }

        #endregion

        #region Page Properties
        private PageLayout CurrentPageLayout
        {
            get
            {
                if (ViewState["CurrentPagelayout"] != null)
                {
                    return (PageLayout)ViewState["CurrentPagelayout"];
                }

                return PageLayout.CREATE;
            }
            set
            {
                ViewState["CurrentPagelayout"] = value;
            }
        }

        private int? IssuerId
        {
            get
            {
                if (ViewState["IssuerId"] != null)
                {
                    return (int?)ViewState["IssuerId"];
                }

                return null;
            }
            set
            {
                ViewState["IssuerId"] = value;
            }
        }
        #endregion

        protected void btngenerateremotetoken_Click(object sender, EventArgs e)
        {
            tbremotetoken.Text = Guid.NewGuid().ToString();
        }

        protected void chkEnableToken_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkEnableToken.Checked)
            {
                tbexpirydate.Text = string.Empty;
            }
        }
    }
}