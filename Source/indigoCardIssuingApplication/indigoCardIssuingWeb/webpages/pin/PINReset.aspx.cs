using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Veneka.Indigo.UX.NativeAppAPI;
using indigoCardIssuingWeb.security;
using System.Security.Principal;
using indigoCardIssuingWeb.Old_App_Code.service;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class PINReset : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PINReset));
        private readonly UserRole[] userRolesForPage = new UserRole[]  { UserRole.PIN_OPERATOR };

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly CustomerCardIssueService _cardService = new CustomerCardIssueService();
        private readonly PINManagementService _pinService = new PINManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                //Coming back to this page, make sure that the pin or pvv is cleared out.
                TerminalService.ClearPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey);
                SessionWrapper.TerminalIssuerId = 0;
                SessionWrapper.TerminalProductId = null;
                SessionWrapper.ReissueBranchId = null;
                SessionWrapper.PinIssueCardId = null;

                TerminalService.ClearPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey);
                LoadPageData();
            }
            else
            {
                if (Request["__EVENTTARGET"].Equals("ClosePinApp"))
                {
                    var issuer = Issuers[int.Parse(this.ddlIssuer.SelectedValue)];

                    PINResponse pinresp1, pinresp2;
                    if (TerminalService.GetPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey, out  pinresp1) &&
                        issuer.enable_instant_pin_YN &&
                        issuer.authorise_pin_reissue_YN &&
                        //!issuer.back_office_pin_auth_YN &&
                        PinAuthorised == false)
                    {
                        PinAuth();
                    }
                    //else if (SessionWrapper.PinIndex != null &&
                    //        issuer.enable_instant_pin_YN &&
                    //        issuer.authorise_pin_reissue_YN &&
                    //        issuer.back_office_pin_auth_YN)
                    //{
                    //    ProcessBackOffice();
                    //}
                    else if (TerminalService.GetPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey, out  pinresp2) &&
                                issuer.enable_instant_pin_YN &&
                                !issuer.authorise_pin_reissue_YN)
                    {
                        ProcessPIN();
                    }
                }
            }
        }

        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuerListItems = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuersList = new Dictionary<int, RolesIssuerResult>();
                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerListItems, out issuersList);

                if (issuerListItems.ContainsKey(-1))
                {
                    issuerListItems.Remove(-1);
                    issuersList.Remove(-1);
                }

                List<ListItem> listItems = new List<ListItem>();
                foreach (var issuer in issuersList.Values)
                {
                    listItems.Add(issuerListItems[issuer.issuer_id]);
                }
                //TEST2
                //Issuers = issuersList.Where(w => w.Value.enable_instant_pin_YN == true).ToDictionary(k => k.Key, v => v.Value);

                Issuers = issuersList.ToDictionary(k => k.Key, v => v.Value);
                this.ddlIssuer.Items.AddRange(listItems.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;

                if (this.ddlIssuer.Items.Count > 0)
                {
                    int issuerId = int.Parse(this.ddlIssuer.SelectedValue);
                    UpdateBranchList(issuerId);
                    //UpdateProductList(issuerId);
                }
                else
                {
                    this.lblInfoMessage.Text = "There are no issues that have instant PIN enabled.";
                }
            }
            catch (Exception ex)
            {
                this.pnlButtons.Visible = false;
                this.pnlDisable.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private void UpdateBranchList(int issuerId)
        {
            if (issuerId >= 0)
            {
                this.ddlBranch.Items.Clear();
                Dictionary<int, ListItem> branchList = new Dictionary<int, ListItem>();

                //if (!useCache /*|| BranchList.Count == 0*/) //should I use cache? if yes check that there is something in page cache.
                //{
                var branches = _userMan.GetBranchesForUser(issuerId, userRolesForPage[0], null);

                foreach (var item in branches)//Convert branches in item list.
                {
                    branchList.Add(item.branch_id, utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
                }

                if (branchList.Count > 0)
                {
                    ddlBranch.Items.AddRange(branchList.Values.OrderBy(m => m.Text).ToArray());
                    ddlBranch.SelectedIndex = 0;

                    this.lblBranch.Visible = true;
                    this.ddlBranch.Visible = true;
                    this.btnPinCapture.Visible = true;

                }
                else
                {
                    this.btnPinCapture.Visible = false;
                    lblErrorMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "EmptyBrancheslistMessage").ToString();
                }
            }
        }

        private void PinAuth()
        {
            this.lblErrorMessage.Text = "Supervisor PIN authorisation needed.";
            log.Warn(w => w("Supervisor PIN authorisation needed."));

            this.tbPinAuthPasscode.Text =
            this.tbPinAuthUser.Text =
            this.tbComments.Text = String.Empty;

            this.tbPinAuthUser.Enabled =
            this.tbPinAuthPasscode.Enabled =
            this.tbComments.Enabled = true;

            this.btnApprove.Visible =
                this.btnReject.Visible = true;
            

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showPinAuth", "showPinAuth();", true);
        }

        private void ProcessBackOffice()
        {
            this.lblInfoMessage.Text = "PIN Captured, please have manager approve PIN request.";
            this.btnPinCapture.Visible = false;
            SessionWrapper.TerminalProductId = null;
            SessionWrapper.ReissueBranchId = null;
            SessionWrapper.PinIssueCardId = null;
        }

        private void ProcessPIN()
        {
            this.lblInfoMessage.Text = "Please click confirm to upload PIN to CMS to complete Process.";
            //this.btnContinue.Visible = false;
            this.btnPinCapture.Visible = false;
            //this.btnPinCapture.Visible = false;
            this.btnConfirm.Visible = true;
            //Update workflow to indicate the custer has selected a pin
            //string responseMessage;
            //if (_cardService.IssueCardPinCaptured(CardId.Value, PinAuthUserId, out responseMessage))
            //{
            //    this.lblInfoMessage.Text = responseMessage;

            //    //this.btnApprove.Enabled = this.btnApprove.Visible = false;
            //    //this.btnReject.Enabled = this.btnReject.Visible = false;
            //    //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
            //    //this.btnPrint.Enabled = this.btnPrint.Visible = false;
            //    //this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
            //    //this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
            //    //this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
            //    //FetchCardDetails(CardId);
            //}
            //else
            //{
            //    this.lblErrorMessage.Text = responseMessage;
            //}
        }

        #region Page Events
        protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                {
                    //UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue));
                    UpdateBranchList(issuerId);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
        protected void btnPinCapture_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                //Pin Logic, if no PIN or PVV present have the PIN operator start PIN selection process.

                TerminalService.ClearPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey);
                SessionWrapper.PINReissue = true;
                PinAuthorised = false;
                SessionWrapper.TerminalIssuerId = int.Parse(this.ddlIssuer.SelectedValue);
                SessionWrapper.ReissueBranchId = int.Parse(this.ddlBranch.SelectedValue);
                this.lblInfoMessage.Text = "Starting Indigo Native App.";

                var sessionGuid = NativeAPIController.CreateStatusSession();
                hdnGuid.Value = sessionGuid.ToString();

                var token = NativeAPIController.CreateToken(sessionGuid, User.ToIndigoPrincipal().IndigoIdentity, Veneka.Indigo.UX.NativeAppAPI.Action.PINSelect, -1, int.Parse(this.ddlBranch.SelectedValue),"-1", true,string.Empty);
                //string token = "1234";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showPinApp", String.Format("showPinApplet('{0},{1}');", token,1), true);

                //btnContinue.Visible = true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }        

        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
        protected void btnReject_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";
            this.lblAuthInfo.Text = "";
            this.lblAuthError.Text = "";
            this.tbPinAuthPasscode.Enabled = true;
            this.tbPinAuthUser.Enabled = true;
            //this.btnSubmitPasscode.Visible = true;

            try
            {
                int branchId;

                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId))
                {
                    string responseMessage;
                    PINResponse pinResp;
                    TerminalService.GetPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey, out  pinResp);

                    if (_pinService.AuthorisationPinReject(this.tbPinAuthUser.Text, this.tbPinAuthPasscode.Text, branchId, pinResp.PinReissueId.Value, tbComments.Text, out responseMessage))
                    {
                        PinAuthorised = false;                        
                        TerminalService.ClearPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey);
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "showPinAuth", "showPinAuth();", true);
                        this.tbPinAuthPasscode.Enabled =
                        this.tbPinAuthUser.Enabled =
                        this.tbComments.Enabled = false;
                        //this.btnSubmitPasscode.Visible = true;
                        this.btnApprove.Visible =
                        this.btnReject.Visible = false;
                        this.btnConfirm.Visible = false;

                        this.lblAuthInfo.Text = responseMessage;
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "showPinAuth", "showPinAuth();", true);
                        this.lblAuthError.Text = responseMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";
            this.lblAuthInfo.Text = "";
            this.lblAuthError.Text = "";
            this.tbPinAuthPasscode.Enabled = true;
            this.tbPinAuthUser.Enabled = true;
            //this.btnSubmitPasscode.Visible = true;

            try
            {
                int branchId;

                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId))
                {                    
                    string responseMessage;

                    PINResponse pinResp;
                    TerminalService.GetPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey, out  pinResp);

                    if (_pinService.AuthorisationPinApprove(this.tbPinAuthUser.Text, this.tbPinAuthPasscode.Text, branchId, pinResp.PinReissueId.Value, tbComments.Text, out responseMessage))
                    {
                        PinAuthorised = true;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "showPinAuth", "showPinAuth();", true);
                        this.tbPinAuthPasscode.Enabled =
                        this.tbPinAuthUser.Enabled = 
                        this.tbComments.Enabled = false;
                        //this.btnSubmitPasscode.Visible = true;
                        this.btnApprove.Visible =
                        this.btnReject.Visible = false;

                        this.lblAuthInfo.Text = responseMessage;
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "showPinAuth", "showPinAuth();", true);
                        this.lblAuthError.Text = responseMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
        protected void btnPasscodeClose_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "hidePinAuth", "hidePinAuth();", true);

            if (PinAuthorised)
            {
                ProcessPIN();
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
        protected void btnConfirm_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                var issuer = Issuers[int.Parse(this.ddlIssuer.SelectedValue)];
                int branchId;
                //int productId;

                //Check instant pin is on, grab pin or pvv from the session and send through with link command
                PINResponse pinresp;
                if (!TerminalService.GetPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey, out  pinresp))
                    throw new ArgumentException("PIN has not been selected. Please have customer select PIN.");

                if (pinresp != null &&
                    issuer.authorise_pin_reissue_YN &&
                    PinAuthorised == false)
                    throw new ArgumentException("Supervisor authorisation needed. Please have supervisor authorise PIN issue.");


                string responseMessage;

                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId))
                {
                    // PIN request should have already been done.
                    // If the passcode for authoristation was successful then the status of approved for the pin reissue has been set.
                    // Branch custodian could have approved via back end too... might need a check button
                    // Complete the reissue process
                    PinReissueWSResult result;
                    
                                        
                    //if (_batchService.PINReissue(issuer.issuer_id, branchId, SessionWrapper.TerminalProductId.Value, PinAuthUserId, SessionWrapper.PinIndex, out responseMessage))
                    if(_pinService.CompletePINReissue(pinresp.PinReissueId.Value, "Complete", out result, out responseMessage))
                    {
                        PinAuthorised = false;
                        
                        TerminalService.ClearPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey);
                        SessionWrapper.TerminalProductId = null;
                        this.lblInfoMessage.Text = responseMessage;
                        this.lblInfoMessage.Text = "PIN successfully reset.";
                        this.btnConfirm.Visible = false;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = responseMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.pnlButtons.Visible = false;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }
        #endregion

        private bool PinAuthorised
        {
            get
            {
                if (ViewState["PinAuthorised"] == null)
                    return false;
                else
                    return (bool)ViewState["PinAuthorised"];
            }
            set
            {
                ViewState["PinAuthorised"] = value;
            }
        }

        private Dictionary<int, RolesIssuerResult> Issuers
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

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            var issuer = Issuers[int.Parse(this.ddlIssuer.SelectedValue)];

            PINResponse pinresp1, pinresp2;
            if (TerminalService.GetPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey, out  pinresp1) &&
                        issuer.enable_instant_pin_YN &&
                        issuer.authorise_pin_reissue_YN &&
                        //!issuer.back_office_pin_auth_YN &&
                        PinAuthorised == false)
            {
                PinAuth();
            }
            else if (TerminalService.GetPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey, out  pinresp2) &&
                        issuer.enable_instant_pin_YN &&
                        !issuer.authorise_pin_reissue_YN)
            {
                ProcessPIN();
            }
            else
            {
                lblInfoMessage.Text = "PIN reset on terminal not complete";
            }
        }
    }
}