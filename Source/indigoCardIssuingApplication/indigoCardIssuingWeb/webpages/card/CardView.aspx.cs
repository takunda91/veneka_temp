using System;
using System.Drawing;
using System.Web.UI;
using System.Linq;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Text;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using System.Web.UI.HtmlControls;
using System.IO;
using indigoCardIssuingWeb.Old_App_Code.service;
using System.Web.Services;
using System.Web;
using System.Security.Principal;
using Veneka.Indigo.UX.NativeAppAPI;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
//using System.Configuration;

namespace indigoCardIssuingWeb.webpages.card
{
    public partial class CardView : BasePage
    {

        //[DllImport("CPSE.dll", EntryPoint = "_CPSC1900Connect@8", CallingConvention = CallingConvention.StdCall)]
        //public static extern byte CPSC1900Connect(byte port);

        private static readonly ILog log = LogManager.GetLogger(typeof(CardView));
        private readonly UserRole[] userRolesForPage = new UserRole[]  { UserRole.CENTER_MANAGER, UserRole.CENTER_OPERATOR,
                                                                         UserRole.BRANCH_CUSTODIAN, UserRole.BRANCH_OPERATOR,
                                                                         UserRole.PIN_OPERATOR, UserRole.AUDITOR,
                                                                         UserRole.PIN_PRINTER_OPERATOR, UserRole.CMS_OPERATOR,
                                                                         UserRole.CARD_PRODUCTION, UserRole.CARD_CENTRE_PIN_OFFICER,
                                                                         UserRole.BRANCH_PIN_OFFICER,
                                                                         UserRole.CREDIT_ANALYST, UserRole.CREDIT_MANAGER};

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly CustomerCardIssueService _cardService = new CustomerCardIssueService();
        private readonly PINManagementService _pinService = new PINManagementService();
        private readonly DocumentManagementService _documentMan = new DocumentManagementService();
        private readonly RenewalService _renewalService = new RenewalService();
        //private readonly string _printerPath = System.Configuration.ConfigurationManager.AppSettings["sdkPrinterPath"].ToString();



        private bool spoilcardflag = true;
        private enum CardConfirm
        {
            CONFIRM_APPROVE,
            CONFIRM_REJECT,
            CONFIRM_SPOIL,
            CONFIRM_PRINT_SUCCESS,
            CONFIRM_PRINT_FAIL,
            CONFIRM_MANUAL_LINK,
            CONFIRM_PIN_MAILER_REPRINT,
            CONFIRM_PIN_MAILER_REPRINT_APPROVE,
            CONFIRM_PIN_MAILER_REPRINT_REJECT,
            CONFIRM_CREDIT_ANALYST,
            CONFIRM_CREDIT_MANAGER,
            CONFIRM_CREDIT_CONTRACT
        }

        #region Page Load
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                //Coming back to this page, make sure that the pin or pvv is cleared out.
                //SessionWrapper.PinIndex = null;
                //TerminalService.ClearPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey);
                //SessionWrapper.TerminalIssuerId = 0;
                //SessionWrapper.PinIssueCardId = null;
                LoadPageData();
            }
            //else
            //{
            //if (Request["__EVENTTARGET"].Equals("ClosePinApp"))
            //{
            //    //ProcessPIN();
            //    //TODO: Look at putting some of these into session.
            //    var cardDetails = _batchService.GetCardDetails(CardId.Value);

            //    var gotIndex = TerminalService.GetPINIndex(User.ToIndigoPrincipal().IndigoIdentity.SessionKey, out PINResponse pinresp);


            //    if (gotIndex &&
            //        cardDetails.authorise_pin_issue_YN &&
            //        !cardDetails.back_office_pin_auth_YN &&
            //        PinAuthUserId == null)
            //    {
            //        PinAuth();
            //    }
            //    else if (gotIndex &&
            //            !cardDetails.authorise_pin_issue_YN)
            //    {
            //        ProcessPIN();
            //    }
            //}
            //}
        }

        private void LoadPageData()
        {
            try
            {
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                Dictionary<int, ListItem> issuerListItems = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuersList = new Dictionary<int, RolesIssuerResult>();
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerListItems, out issuersList);
                if (issuersList.Any(a => a.Value.SatelliteBranch_YN == true))
                {
                    Satellite_Branch_UserYN = true;
                }
                if (issuersList.Any(a => a.Value.MainBranch_YN == true))
                {
                    Main_Branch_UserYN = true;
                }

                if (SessionWrapper.CardSearchResultItem != null)
                {
                    var card = SessionWrapper.CardSearchResultItem;

                    FetchCardDetails(card.card_id);
                    CardId = card.card_id;

                    SessionWrapper.CardSearchResultItem = null; //Clear item out of the session.
                    if (SessionWrapper.CardViewMode != null)
                    {
                        CardViewMode = SessionWrapper.CardViewMode;
                        SessionWrapper.CardViewMode = null;
                    }

                    if (SessionWrapper.CardSearchParams != null
                            && SessionWrapper.CardSearchParams.GetType() == typeof(CardSearchParameters))
                    {
                        CardSearchParametersParams = (CardSearchParameters)SessionWrapper.CardSearchParams;
                        SessionWrapper.CardSearchParams = null;
                    }
                    else if (SessionWrapper.CardSearchParams != null
                          && SessionWrapper.CardSearchParams.GetType() == typeof(PinMailerReprintSearchParameters))
                    {
                        CardSearchParametersParams = (PinMailerReprintSearchParameters)SessionWrapper.CardSearchParams;
                        SessionWrapper.CardSearchParams = null;
                    }
                    else if (SessionWrapper.CardSearchParams != null
                          && SessionWrapper.CardSearchParams.GetType() == typeof(BranchCardSearchParameters))
                    {
                        CardSearchParametersParams = (BranchCardSearchParameters)SessionWrapper.CardSearchParams;
                        SessionWrapper.CardSearchParams = null;
                    }
                }
                else
                {
                    this.lblInfoMessage.Text = "Sorry there is no card information to display.";
                }

                if (Cache["custsettings"] != null)
                {
                    List<ConfigurationSettings> elements = (List<ConfigurationSettings>)Cache["custsettings"];
                    elements = elements.FindAll(i => i.PageName == "cardcapture").ToList();

                    ContentPlaceHolder myContent = (ContentPlaceHolder)this.Master.FindControl("MainContent");
                    HtmlGenericControl divcontrol = null;
                    foreach (var element in elements)
                    {
                        divcontrol = myContent.FindControl("div" + element.Key) as HtmlGenericControl;
                        if (divcontrol != null)
                        {
                            if (element.Visibility)
                            {
                                divcontrol.Attributes.Add("style", "display:block");

                            }
                            else
                            {
                                divcontrol.Attributes.Add("style", "display:none");

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.pnlButtons.Visible = false;
                this.pnlCmsButtons.Visible = false;
                //this.pnlPinButtons.Visible = false;
                this.pnlPrintButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private void LoadRemoteDocuments()
        {
            documentPanel.Visible = false;
            if (DocumentStructure != null)
            {
                if (DocumentStructure.StorageType == DocumentStorageType.Remote)
                {
                    var remoteDocuments = _documentMan.CardDocumentsRemote(tbAccountNumber.Text);
                    panelRemoteDocuments.Visible = true;
                    DocumentTable = ToDocumentDataTable(remoteDocuments);
                    grdRemoteDocuments.DataSource = DocumentTable;
                    grdRemoteDocuments.DataBind();
                }
            }
        }

        private void ClearControls()
        {
            this.tbAccountNumber.Text = String.Empty;
            this.tbDomicileBranch.Text =
            this.tbAccountType.Text = String.Empty;
            this.tbBranchCardStatus.Text = String.Empty;
            this.tbProductName.Text = String.Empty;
            this.tbCardNumber.Text = String.Empty;
            this.tbContractId.Text = String.Empty;
            this.tbContractNumber.Text = String.Empty;
            this.tbCurrency.Text = String.Empty;
            this.tbCustomerType.Text = String.Empty;
            this.tbStatusDate.Text =
            this.tbDateIssued.Text = String.Empty;
            this.tbFirstName.Text = String.Empty;
            this.tbIssuedBy.Text = String.Empty;
            this.tbIssuer.Text = String.Empty;
            this.tbIssuingBranch.Text = String.Empty;
            this.tbLastName.Text = String.Empty;
            this.tbIdNumber.Text = String.Empty;
            this.tbContactNumber.Text = String.Empty;
            this.tbMiddleName.Text = String.Empty;
            this.tbReasonForIssue.Text = String.Empty;
            this.tbResident.Text = String.Empty;
            this.tbSpoilComments.Text = String.Empty;
            this.tbTitle.Text = String.Empty;
            //this.tbNameOnCard.Text = String.Empty;

            this.tbFee.Text = String.Empty;
            this.tbFeeRefNo.Text = String.Empty;
            this.tbFeeRevNo.Text = String.Empty;
            this.chkFeeOverridden.Checked = false;
            this.chkFeeWaived.Checked = false;

            this.tbCardReference.Text = String.Empty;
            this.tbCardPriority.Text = String.Empty;
            this.tbCardIssueMethod.Text = String.Empty;
            if (DocumentTable != null)
            {
                DocumentTable.Rows.Clear();
            }
        }

        #endregion

        #region Page Methods

        private CardDetails FetchCardDetails(long? cardId)
        {
            ClearControls();
            bool renewalCard = false;

            if (cardId != null)
            {
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.ddlSpoilReason.Enabled = this.ddlSpoilReason.Visible = false;
                this.tbSpoilComments.Enabled = this.tbSpoilComments.Visible = false;
                this.btnManualLink.Enabled = this.btnManualLink.Visible = false;
                this.btnReuploadCard.Enabled = this.btnReuploadCard.Visible = false;
                this.lblSpoilComments.Visible = false;
                this.lblSpoilReason.Visible = false;
                this.btnReprintPinMailer.Visible = false;
                this.btnReprintPinMailerApprove.Visible = false;
                this.btnReprintPinMailerReject.Visible = false;

                var cardDetails = _batchService.GetCardDetails(cardId.GetValueOrDefault());
              

                if (cardDetails != null)
                {
                    if (IsRenewal(cardDetails.card_id))
                    {
                        renewalCard = true;
                    }
                    if (cardDetails.ProductFields != null)
                        AdditionalFields = cardDetails.ProductFields.ToList();
                    //CardDetails = cardDetails;
                    var product_id = cardDetails.product_id;
                    ProductResult product = _batchService.GetProduct(product_id);
                  
                        //this.hdnCardPrintStyle.Value = ""; // printStyle.ToString();
                        this.hdnNameOnCard.Value =
                                     PrintingService.Instance.SetCardPrinting(cardDetails.card_id.ToString() + cardDetails.customer_account_id.ToString(),
                                                 String.Join(" ", cardDetails.ProductFields.Select(s => s.GetHTML()).ToArray()));


                    //Card Details
                    this.tbCardNumber.Text = utility.UtilityClass.FormatPAN(cardDetails.card_number) ?? "";
                    this.tbProductName.Text = cardDetails.product_name;
                    this.tbIssuer.Text = base.FormatNameAndCode(cardDetails.issuer_name, cardDetails.issuer_code);
                    this.tbIssuingBranch.Text = base.FormatNameAndCode(cardDetails.branch_name, cardDetails.branch_code);
                    this.tbCardIssueMethod.Text = cardDetails.card_issue_method_name;
                    this.tbCardPriority.Text = cardDetails.card_priority_name;
                    this.tbCardReference.Text = cardDetails.card_request_reference;

                    this.tbFee.Text = cardDetails.total_charged == null ? String.Empty : cardDetails.total_charged.Value.ToString();
                    this.tbFeeRefNo.Text = cardDetails.fee_reference_number;
                    this.tbFeeRevNo.Text = cardDetails.fee_reversal_ref_number;
                    this.chkFeeOverridden.Checked = cardDetails.fee_overridden_YN == null ? false : cardDetails.fee_overridden_YN.Value;
                    this.chkFeeWaived.Checked = cardDetails.fee_waiver_YN == null ? false : cardDetails.fee_waiver_YN.Value;

                    if (!String.IsNullOrWhiteSpace(cardDetails.branch_card_statuses_name))
                    {
                        this.tbBranchCardStatus.Text = cardDetails.branch_card_statuses_name;
                    }
                    else
                    {
                        this.tbBranchCardStatus.Text = !String.IsNullOrWhiteSpace(cardDetails.dist_card_status_name) ? cardDetails.dist_card_status_name : "";
                    }

                    //Customer account details
                    if (!String.IsNullOrWhiteSpace(cardDetails.customer_account_number))
                    {
                        //this.hdnNameOnCard.Value = cardDetails.name_on_card;
                        this.tbStatusDate.Text = cardDetails.status_date != null ? cardDetails.status_date.ToString() : "N/A";
                        this.tbAccountNumber.Text = cardDetails.customer_account_number;
                        this.tbDomicileBranch.Text = utility.UtilityClass.FormatNameAndCode(cardDetails.domicile_branch_name, cardDetails.domicile_branch_code);
                        this.tbAccountType.Text = cardDetails.customer_account_type_name;
                        this.tbDateIssued.Text = cardDetails.date_issued != null ? cardDetails.date_issued.ToString() : "N/A";
                        this.tbFirstName.Text = cardDetails.customer_first_name;
                        this.tbMiddleName.Text = cardDetails.customer_middle_name;
                        this.tbIssuedBy.Text = cardDetails.username;// cardDetails.user_first_name + " " + cardDetails.user_last_name;
                        this.tbLastName.Text = cardDetails.customer_last_name;
                        this.tbReasonForIssue.Text = cardDetails.card_issuer_reason_name;
                        this.tbTitle.Text = cardDetails.customer_title_name;
                        this.tbCurrency.Text = String.Format("{0}/{1}", cardDetails.currency_code, cardDetails.iso_4217_numeric_code);
                        this.tbResident.Text = cardDetails.customer_residency_name;
                        this.tbCustomerType.Text = cardDetails.customer_type_name;
                        this.tbContractNumber.Text = cardDetails.contract_number ?? "";
                        this.tbContractId.Text = cardDetails.cms_id ?? "";
                        this.tbContactNumber.Text = cardDetails.contract_number;
                        this.tbIdNumber.Text = cardDetails.id_number;
                        this.tbContactNumber.Text = cardDetails.contact_number;
                        this.tbEmailAddress.Text = cardDetails.customer_email;

                        string firstName = String.IsNullOrEmpty(cardDetails.customer_first_name) ? "" : cardDetails.customer_first_name;
                        string middleName = String.IsNullOrEmpty(cardDetails.customer_middle_name) ? "" : cardDetails.customer_middle_name;
                        string initializedMiddleName = String.IsNullOrEmpty(GetInitializedMiddleName(cardDetails.customer_middle_name)) ? "" : GetInitializedMiddleName(cardDetails.customer_middle_name);
                        string lastName = String.IsNullOrEmpty(cardDetails.customer_last_name) ? "" : cardDetails.customer_last_name;
                        //this.tbNameOnCard.Text = String.IsNullOrEmpty(cardDetails.name_on_card) ? "" : cardDetails.name_on_card;
                    }

                    bool canRead;
                    bool canUpdate;
                    bool canCreate;
                    if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_MANAGER, cardDetails.issuer_id, out canRead, out canUpdate, out canCreate))
                    {
                        if (cardDetails.branch_card_statuses_id == null)
                        {

                        }
                        else
                        {
                            if (canUpdate)
                            {
                                EnableSpoilCard();
                            }
                        }

                    }

                    if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CREDIT_ANALYST, cardDetails.issuer_id, out canRead, out canUpdate, out canCreate) ||
                        PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CREDIT_MANAGER, cardDetails.issuer_id, out canRead, out canUpdate, out canCreate))
                    {
                        BranchCardStatus currentCardStatus = (BranchCardStatus)cardDetails.branch_card_statuses_id;
                        if (currentCardStatus == BranchCardStatus.CREDIT_READY_TO_ANALYZE || currentCardStatus == BranchCardStatus.CREDIT_READY_TO_APPROVE)
                        {
                            this.lblSpoilComments.Enabled = this.lblSpoilComments.Visible = true;
                            this.tbSpoilComments.Enabled = this.tbSpoilComments.Visible = true;
                        }
                    }

                    if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_CUSTODIAN, cardDetails.issuer_id, out canRead, out canUpdate, out canCreate))
                    {
                        if (cardDetails.branch_card_statuses_id == null)
                        {

                        }
                        else
                        {
                            if (canUpdate)
                            {
                                EnableSpoilCard();
                            }

                            if (cardDetails.spoil_only != null && !cardDetails.spoil_only.Value)
                            {
                                this.btnManualLink.Enabled = this.btnManualLink.Visible = true;
                            }

                            if (BranchCardStatus.ALLOCATED_TO_CUST == (BranchCardStatus)cardDetails.branch_card_statuses_id &&
                                cardDetails.maker_checker_YN &&
                                canUpdate)
                            {
                                this.btnApprove.Enabled = this.btnApprove.Visible = true;
                                this.btnReject.Enabled = this.btnReject.Visible = true;
                                this.lblSpoilComments.Enabled = this.lblSpoilComments.Visible = true;
                                this.tbSpoilComments.Enabled = this.tbSpoilComments.Visible = true;
                            }
                            else if (BranchCardStatus.CMS_ERROR == (BranchCardStatus)cardDetails.branch_card_statuses_id && canUpdate)
                            {
                                if (cardDetails.allow_manual_uploaded_YN)
                                {
                                    this.pnlCmsButtons.Visible =
                                    this.btnManualLink.Enabled = this.btnManualLink.Visible = true;
                                }

                                if (cardDetails.allow_reupload_YN)
                                {
                                    this.pnlCmsButtons.Visible =
                                    this.btnReuploadCard.Enabled = this.btnReuploadCard.Visible = true;
                                }
                            }
                            else if (!(cardDetails.branch_card_statuses_id == (int)BranchCardStatus.ALLOCATED_TO_CUST ||
                                        cardDetails.branch_card_statuses_id == (int)BranchCardStatus.APPROVED_FOR_ISSUE ||
                                        cardDetails.branch_card_statuses_id == (int)BranchCardStatus.REQUESTED) &&
                                        canUpdate &&
                                        cardDetails.pin_mailer_printing_YN &&
                                        cardDetails.pin_mailer_reprint_YN &&
                                        cardDetails.pin_mailer_reprint_status_id == 0 &&
                                        cardDetails.maker_checker_YN)
                            {
                                this.lblSpoilComments.Enabled = this.lblSpoilComments.Visible =
                                this.tbSpoilComments.Enabled = this.tbSpoilComments.Visible =
                                this.btnReprintPinMailerApprove.Visible =
                                this.btnReprintPinMailerReject.Visible = true;
                            }
                        }
                    }
                    long? UserId = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.UserId;

                    if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_OPERATOR, cardDetails.issuer_id, out canRead, out canUpdate, out canCreate))
                    {

                        if (cardDetails.branch_card_statuses_id == null)
                        {

                        }
                        else
                        {

                            if (BranchCardStatus.APPROVED_FOR_ISSUE == (BranchCardStatus)cardDetails.branch_card_statuses_id &&
                                 //cardDetails.maker_checker_YN &&
                                 ((cardDetails.user_id == UserId && cardDetails.card_issue_method_id == 1)) &&
                                 canUpdate)
                            {

                                this.hdnNameOnCard.Value =
                                    PrintingService.Instance.SetCardPrinting(cardDetails.card_id.ToString() + cardDetails.customer_account_id.ToString(),
                                                String.Join(" ", cardDetails.ProductFields.Select(s => s.GetHTML()).ToArray()));

                                if ((bool)product.Product.Is_mtwenty_printed)
                                {
                                    this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = true;
                                }
                                else
                                {
                                    this.btnPrint.Enabled = this.btnPrint.Visible = true;
                                }
                               
                                
                                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = true;
                                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = true;
                                this.pnlPrintButtons.Visible = true;
                                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmPrint").ToString();
                            }
                            else if (BranchCardStatus.ALLOCATED_TO_CUST == (BranchCardStatus)cardDetails.branch_card_statuses_id &&
                                !cardDetails.maker_checker_YN &&
                                cardDetails.user_id == UserId &&
                                cardDetails.card_issue_method_id == 1 &&
                                canUpdate)
                            {
                                if ((bool)product.Product.Is_mtwenty_printed)
                                {
                                    this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = true;
                                }
                                else
                                {
                                    this.btnPrint.Enabled = this.btnPrint.Visible = true;
                                }
                               
                                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = true;
                                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = true;
                                this.pnlPrintButtons.Visible = true;
                                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmPrint").ToString();
                            }
                            else if ((BranchCardStatus.CARD_PRINTED == (BranchCardStatus)cardDetails.branch_card_statuses_id ||
                                        BranchCardStatus.CMS_REUPLOAD == (BranchCardStatus)cardDetails.branch_card_statuses_id) &&
                                        canUpdate &&
                                        ((cardDetails.user_id == UserId && cardDetails.card_issue_method_id == 1) ||
                                        (cardDetails.card_issue_method_id == 0)))
                            {
                                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = true;
                            }
                            else if (BranchCardStatus.CHECKED_IN == (BranchCardStatus)cardDetails.branch_card_statuses_id &&
                                    canUpdate)
                            {
                                if (renewalCard)
                                {
                                    this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = true;
                                }
                                else if (cardDetails.card_issue_method_id == 0)
                                {
                                    this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = true;
                                }
                                else
                                {
                                    if (cardDetails.print_issue_card_YN)
                                    {
                                        var token = NativeAPIController.CreateToken(Guid.NewGuid(), HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity, Veneka.Indigo.UX.NativeAppAPI.Action.PrintCard, cardDetails.card_id, cardDetails.branch_id, "-1", false, string.Empty);

                                        if ((bool)product.Product.Is_mtwenty_printed)
                                        {
                                            this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = true;
                                        }
                                        else
                                        {
                                            this.btnPrint.Enabled = this.btnPrint.Visible = true;
                                        }
                                        
                                        this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = true;
                                        this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = true;
                                        this.pnlPrintButtons.Visible = true;
                                        this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmPrint").ToString();
                                    }
                                    else
                                    {
                                        this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = true;
                                    }
                                }
                            }
                            else if (!(cardDetails.branch_card_statuses_id == (int)BranchCardStatus.CHECKED_IN ||
                                    cardDetails.branch_card_statuses_id == (int)BranchCardStatus.ALLOCATED_TO_CUST ||
                                    cardDetails.branch_card_statuses_id == (int)BranchCardStatus.APPROVED_FOR_ISSUE ||
                                    cardDetails.branch_card_statuses_id == (int)BranchCardStatus.REQUESTED) &&
                                    canUpdate &&
                                    cardDetails.pin_mailer_printing_YN &&
                                    cardDetails.pin_mailer_reprint_YN &&
                                    (cardDetails.pin_mailer_reprint_status_id == null || cardDetails.pin_mailer_reprint_status_id == 3
                                        || cardDetails.pin_mailer_reprint_status_id == 4))
                            {
                                //enable pin mailer reprint button
                                this.btnReprintPinMailer.Visible = true;
                            }
                            else if (BranchCardStatus.CHECKED_IN == (BranchCardStatus)cardDetails.branch_card_statuses_id &&
                                    canUpdate &&
                                    cardDetails.card_issue_method_id == 1 && Satellite_Branch_UserYN)
                            {
                                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = true;

                            }

                        }
                    }

                    ProductDocumentStructure docStructure = _documentMan.GetProductDocumentStructure(cardDetails.product_id);
                    log.Trace("Document Structure Retrieved");
                    documentPanel.Visible = false;
                    panelRemoteDocuments.Visible = false;

                    DocumentStructure = docStructure;

                    var savedDocuments = _documentMan.CardDocumentAll(cardDetails.card_id, true);

                    if (docStructure.StorageType != DocumentStorageType.Unknown)
                    {
                        DocumentTable = ToDocumentDataTable(savedDocuments);
                        if (savedDocuments.Count > 0)
                        {
                            documentPanel.Visible = true;
                            grdDocuments.DataSource = DocumentTable;
                            grdDocuments.DataBind();
                        }
                    }

                    SetLimitControls(cardDetails);
                    SetActivationControls(cardDetails);
                }
                else
                {
                    //AdditionalFields = null;
                    this.lblInfoMessage.Text = "There are no card details to show.";
                }

                return cardDetails;
            }
            else
            {
                log.Error("Card Id is null, unable to fetch card details.");
                this.lblErrorMessage.Text = "There was an issue retrieving the card details. Please try reload the page.";
            }

            return null;
        }

        private bool IsRenewal(long card_id)
        {
            var renewalEntry = _renewalService.GetRenewalDetailCard(card_id);
            if (renewalEntry != null && renewalEntry.CardId.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CreditApproval
        {
            get
            {
                if (ViewState["CreditApproval"] != null)
                    return (bool)ViewState["CreditApproval"];
                else
                    return false;
            }
            set
            {
                ViewState["CreditApproval"] = value;
            }
        }

        private void SetLimitControls(CardDetails cardDetails)
        {
            if (cardDetails != null)
            {
                var product = _batchService.GetProduct(cardDetails.product_id);
                if (product != null)
                {
                    if (product.Product.credit_limit_capture.GetValueOrDefault() == true)
                    {
                        CreditApproval = true;
                        bool canRead;
                        bool canUpdate;
                        bool canCreate;

                        CardLimitModel accountLimit = _cardService.CardLimitGet(cardDetails.card_id);

                        if (accountLimit != null)
                        {
                            creditDetailPanel.Visible = true;

                            divCreditLimit.Visible = true;
                            tbCreditLimit.Enabled = false;
                            tbCreditLimit.Text = accountLimit.Limit.ToString("#,##0.00");

                            divCreditContractNumber.Visible = true;
                            tbCreditContractNumber.Text = accountLimit.ContractNumber;
                            tbCreditContractNumber.Enabled = false;
                            reqCreditContractNumber.Enabled = false;

                            if (product.Product.credit_limit_update.GetValueOrDefault())
                            {
                                divCreditLimitUpdate.Visible = true;
                            }
                            else
                            {
                                divCreditLimitUpdate.Visible = false;
                            }
                            tbCreditLimitUpdate.Text = accountLimit.LimitApproved.GetValueOrDefault().ToString("#,##0.00");
                            tbCreditLimitUpdate.Enabled = false;
                            reqCreditLimitUpdate.Enabled = false;
                            this.rangeValidatorCreditLimit.Enabled = false;
                        }

                        if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CREDIT_ANALYST, cardDetails.issuer_id, out canRead, out canUpdate, out canCreate))
                        {
                            bool allowReview = cardDetails.branch_card_statuses_id == Convert.ToInt32(BranchCardStatus.CREDIT_READY_TO_ANALYZE);

                            if (allowReview)
                            {
                                if (accountLimit != null)
                                {
                                    if (accountLimit.CreditAnalystId.GetValueOrDefault() == 0)
                                    {
                                        btnCreditAnalyst.Visible = true;
                                        btnReject.Visible = true;
                                        btnReject.Enabled = true;
                                    }
                                    if (product.Product.credit_limit_update.GetValueOrDefault() == true)
                                    {
                                        divCreditLimitUpdate.Visible = true;
                                        tbCreditLimitUpdate.Enabled = true;
                                        this.rangeValidatorCreditLimit.Enabled = true;
                                        reqCreditLimitUpdate.Enabled = true;
                                    }
                                }
                            }
                        }

                        if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CREDIT_MANAGER, cardDetails.issuer_id, out canRead, out canUpdate, out canCreate))
                        {
                            bool allowApprove = cardDetails.branch_card_statuses_id == Convert.ToInt32(BranchCardStatus.CREDIT_READY_TO_APPROVE);
                            if (product.Product.parallel_approval.GetValueOrDefault() == true)
                            {
                                if (cardDetails.branch_card_statuses_id == Convert.ToInt32(BranchCardStatus.CREDIT_READY_TO_ANALYZE))
                                {
                                    allowApprove = cardDetails.branch_card_statuses_id == Convert.ToInt32(BranchCardStatus.CREDIT_READY_TO_ANALYZE);
                                }
                            }

                            if (allowApprove)
                            {
                                if (accountLimit != null)
                                {
                                    if (btnCreditAnalyst.Visible == false)
                                    {
                                        if (accountLimit.CreditManagerId.GetValueOrDefault() == 0)
                                        {
                                            btnCreditManager.Visible = true;
                                            btnReject.Enabled = true;
                                            btnReject.Visible = true;
                                        }
                                    }

                                    if (product.Product.credit_limit_update.GetValueOrDefault() == true)
                                    {
                                        tbCreditLimitUpdate.Enabled = false;
                                    }
                                }
                            }
                        }

                        if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_OPERATOR, cardDetails.issuer_id, out canRead, out canUpdate, out canCreate))
                        {
                            bool allowApprove = cardDetails.branch_card_statuses_id == Convert.ToInt32(BranchCardStatus.CREDIT_CONTRACT_CAPTURE);

                            if (allowApprove)
                            {
                                if (accountLimit != null)
                                {
                                    if (btnCreditAnalyst.Visible == false && btnCreditContractNumber.Visible == false)
                                    {
                                        btnCreditContractNumber.Visible = true;
                                    }

                                    if (product.Product.manual_contract_number.GetValueOrDefault() == true)
                                    {
                                        tbCreditContractNumber.Enabled = true;
                                        reqCreditContractNumber.Enabled = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetActivationControls(CardDetails cardDetails)
        {
            if (cardDetails != null)
            {
                var product = _batchService.GetProduct(cardDetails.product_id);

                if (product != null)
                {
                    if (product.Product.activation_by_center_operator.GetValueOrDefault() == true)
                    {
                        CreditApproval = true;
                        bool canRead;
                        bool canUpdate;
                        bool canCreate;

                        if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_OPERATOR, cardDetails.issuer_id, out canRead, out canUpdate, out canCreate))
                        {
                            if (cardDetails.branch_card_statuses_id == 6) //issued card so can issue
                            {
                                pnlActivationButtons.Visible = true;
                                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = true;
                            }
                            else
                            {
                                pnlActivationButtons.Visible = false;
                            }
                        }
                    }
                }
            }
        }

        private ProductDocumentStructure DocumentStructure
        {
            get
            {
                if (ViewState["ProductDocumentStructure"] != null)
                    return (ProductDocumentStructure)ViewState["ProductDocumentStructure"];
                else
                    return new ProductDocumentStructure();
            }
            set
            {
                ViewState["ProductDocumentStructure"] = value;
            }
        }

        private DataTable DocumentTable
        {
            get
            {
                if (ViewState["DocumentTable"] != null)
                    return (DataTable)ViewState["DocumentTable"];
                else
                    return null;
            }
            set
            {
                ViewState["DocumentTable"] = value;
            }
        }

        public DataTable ToDocumentDataTable<CardDocument>(List<CardDocument> items)
        {
            DataTable dataTable = new DataTable(typeof(CardDocument).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(CardDocument).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            dataTable.Columns.Add("ShortName");

            int lastId = 0;
            if (DocumentTable != null)
            {
                dataTable = DocumentTable;
                DataRow lastRow = dataTable.Select().LastOrDefault();
                if (lastRow != null)
                {
                    lastId = Math.Abs(Convert.ToInt32(lastRow["Id"]));
                }
            }

            foreach (CardDocument item in items)
            {
                var values = new object[dataTable.Columns.Count];
                string location = string.Empty;
                string comment = string.Empty;
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                    if (Props[i].Name == "Location")
                    {
                        location = values[i].ToString();
                    }
                    if (Props[i].Name == "Id")
                    {
                        values[i] = ((lastId + 1) * -1).ToString();
                    }
                    if (Props[i].Name == "Comment")
                    {
                        comment = values[i].ToString();
                    }
                }
                lastId++;
                if (DocumentStructure.StorageType == DocumentStorageType.Local)
                {
                    values[dataTable.Columns.Count - 1] = Path.GetFileName(location);
                }
                else
                {
                    values[dataTable.Columns.Count - 1] = comment;
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        private string GetInitializedMiddleName(string middlename)
        {
            string rValue = "";
            if (!String.IsNullOrEmpty(middlename))
            {
                string[] parts = middlename.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in parts)
                {
                    rValue += s[0] + ". ";
                }

            }
            return rValue;
        }

        private void RejectCard()
        {
            string result;

            if (_cardService.MakerChecker(CardId.Value, false, this.tbSpoilComments.Text, out result))
            {
                this.lblInfoMessage.Text = result;
                spoilcardflag = false;
                FetchCardDetails(CardId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible =
                this.pnlPrintButtons.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;

                CardConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = result;
            }
        }

        private void ApproveCard()
        {
            string result;

            if (_cardService.MakerChecker(CardId.Value, true, this.tbSpoilComments.Text, out result))
            {
                this.lblInfoMessage.Text = result;
                spoilcardflag = false;
                FetchCardDetails(CardId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible =
                    this.pnlPrintButtons.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                CardConfirmCurrent = null;
                if (CreditApproval == true)
                {
                    log.Trace("Call: _cardService.CardSetReviewStatus");
                    _cardService.CardSetReviewStatus(CardId.GetValueOrDefault(), Convert.ToInt32(BranchCardStatus.CREDIT_READY_TO_ANALYZE));
                }
            }
            else
            {
                this.lblErrorMessage.Text = result;
            }
        }

        private void PrintSuccess()
        {
            string result;
            if (_cardService.IssueCardPrinted(CardId.Value, out result))
            {
                this.lblInfoMessage.Text = result;
                spoilcardflag = false;
                var cardDetails = FetchCardDetails(CardId);

                //TODO: Quick fix
                if (cardDetails.enable_instant_pin_YN && cardDetails.enable_instant_pin_YN_issuer)
                {
                    bool canRead, canUpdate, canCreate;
                    if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.PIN_OPERATOR, cardDetails.issuer_id, out canRead, out canUpdate, out canCreate))
                        this.lblInfoMessage.Text = "Card has been marked as PRINTED.<br/>Please click 'Capture PIN' to have customer select PIN.";
                    else
                        this.lblInfoMessage.Text = "Card has been marked as PRINTED.<br/>Please have PIN Operator do PIN selection.";
                }


                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                CardConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = result;
            }
        }

        private void PrintFailed()
        {
            string result;

            if (_cardService.IssueCardPrintError(CardId.Value, out result))
            {
                this.lblInfoMessage.Text = result;
                spoilcardflag = false;
                FetchCardDetails(CardId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                CardConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = result;
            }
        }

        private void SpoilCard()
        {
            string result;
            if (_cardService.IssueCardSpoil(CardId.Value, int.Parse(this.ddlSpoilReason.SelectedValue),
                                             this.tbSpoilComments.Text, out result))
            {
                this.lblInfoMessage.Text = result;
                spoilcardflag = false;
                FetchCardDetails(CardId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                CardConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = result;
            }
        }

        private void ManuallyLinkCard()
        {
            string result;
            if (_batchService.IssueCardComplete(CardId.Value, out result))
            {
                spoilcardflag = false;
                this.lblInfoMessage.Text = result;
                FetchCardDetails(CardId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.btnManualLink.Enabled = this.btnManualLink.Visible = false;
                this.btnReuploadCard.Enabled = this.btnReuploadCard.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                CardConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = result;
            }
        }

        private void PinMailerReprint()
        {
            string result;
            if (_pinService.PinMailerReprintRequest(CardId.Value, "", out result))
            {
                spoilcardflag = false;
                this.lblInfoMessage.Text = result;
                FetchCardDetails(CardId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.btnManualLink.Enabled = this.btnManualLink.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                CardConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = result;
            }
        }

        private void PinMailerReprintApprove()
        {
            string result;
            if (_pinService.PinMailerReprintApprove(CardId.Value, "", out result))
            {
                spoilcardflag = false;
                this.lblInfoMessage.Text = result;
                FetchCardDetails(CardId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.btnManualLink.Enabled = this.btnManualLink.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                CardConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = result;
            }
        }

        private void PinMailerReprintReject()
        {
            string result;
            if (_pinService.PinMailerReprintReject(CardId.Value, "", out result))
            {
                spoilcardflag = false;
                this.lblInfoMessage.Text = result;
                FetchCardDetails(CardId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.btnManualLink.Enabled = this.btnManualLink.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                CardConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = result;
            }
        }

        //private void PinAuth()
        //{
        //    this.lblErrorMessage.Text = "Supervisor PIN authorisation needed.";
        //    log.Warn(w => w("Supervisor PIN authorisation needed."));
        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "showPinAuth", "showPinAuth();", true);
        //}

        //private void ProcessPIN()
        //{
        //    //Update workflow to indicate the custer has selected a pin
        //    string responseMessage;
        //    if (_cardService.IssueCardPinCaptured(CardId.Value, PinAuthUserId, out responseMessage))
        //    {
        //        this.lblInfoMessage.Text = responseMessage;
        //        this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfrimPincapturedsuccess").ToString();
        //        this.btnApprove.Enabled = this.btnApprove.Visible = false;
        //        this.btnReject.Enabled = this.btnReject.Visible = false;
        //        //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
        //        spoilcardflag = false;
        //        this.btnPrint.Enabled = this.btnPrint.Visible = false;
        //        this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
        //        this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
        //        this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
        //        FetchCardDetails(CardId);
        //    }
        //    else
        //    {
        //        this.lblErrorMessage.Text = responseMessage;
        //    }
        //}

        #endregion

        #region Page Events

        protected void btnRefresh_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                FetchCardDetails(CardId);
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoRefresh").ToString();
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
        protected void EnableSpoilCard()
        {
            if (spoilcardflag)
            {
                var reasons = _cardService.ListBranchCardCodes(1, true);
                List<ListItem> reasonsList = new List<ListItem>();
                foreach (var item in reasons)
                {
                    reasonsList.Add(new ListItem(item.branch_card_code_name, item.branch_card_code_id.ToString()));
                }
                this.ddlSpoilReason.Items.AddRange(reasonsList.OrderBy(m => m.Text).ToArray());

                this.btnSpoil.Enabled = this.btnSpoil.Visible = true;
                this.ddlSpoilReason.Enabled = this.ddlSpoilReason.Visible = true;
                this.tbSpoilComments.Enabled = this.tbSpoilComments.Visible = true;
                this.lblSpoilComments.Visible = true;
                this.lblSpoilReason.Visible = true;
            }
            else
            {
                this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                this.ddlSpoilReason.Enabled = this.ddlSpoilReason.Visible = false;
                this.tbSpoilComments.Enabled = this.tbSpoilComments.Visible = false;
                this.lblSpoilComments.Visible = false;
                this.lblSpoilReason.Visible = false;
            }
        }

        //protected void btnSubmitPasscode_Click(object sender, EventArgs e)
        //{
        //    this.lblInfoMessage.Text = "";
        //    this.lblErrorMessage.Text = "";
        //    this.lblAuthInfo.Text = "";
        //    this.lblAuthError.Text = "";
        //    this.tbPinAuthPasscode.Enabled = true;
        //    this.tbPinAuthUser.Enabled = true;
        //    this.btnSubmitPasscode.Visible = true;

        //    try
        //    {
        //        PinAuthUserId = null;

        //        //TODO: Look at putting some of these into session.
        //        var cardDetails = _batchService.GetCardDetails(CardId.Value);

        //        long? authUserId = _pinService.GetUserAuthorisationPin(this.tbPinAuthUser.Text, this.tbPinAuthPasscode.Text, cardDetails.branch_id);

        //        if (authUserId != null && authUserId > 0)
        //        {
        //            PinAuthUserId = authUserId;
        //            Page.ClientScript.RegisterStartupScript(this.GetType(), "showPinAuth", "showPinAuth();", true);
        //            this.tbPinAuthPasscode.Enabled =
        //            this.tbPinAuthUser.Enabled =
        //            this.btnSubmitPasscode.Visible = false;
        //            this.lblAuthInfo.Text = "Passcode Correct.";
        //        }
        //        else
        //        {
        //            Page.ClientScript.RegisterStartupScript(this.GetType(), "showPinAuth", "showPinAuth();", true);
        //            this.lblAuthError.Text = "Passcode Incorrect.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
        //        if (log.IsTraceEnabled || log.IsDebugEnabled)
        //        {
        //            this.lblErrorMessage.Text = ex.ToString();
        //        }
        //    }
        //}

        //protected void btnPasscodeClose_Click(object sender, EventArgs e)
        //{
        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "hidePinAuth", "hidePinAuth();", true);

        //    if (PinAuthUserId != null && PinAuthUserId > 0)
        //    {
        //        ProcessPIN();
        //    }
        //}

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmApprove").ToString();

                CardConfirmCurrent = CardConfirm.CONFIRM_APPROVE;
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnBack.Visible = this.btnBack.Enabled = true;
                this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                this.tbSpoilComments.Enabled = false;

                this.btnApprove.Visible = this.btnApprove.Enabled =
                     this.btnSpoil.Enabled = this.btnSpoil.Visible =
                      this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                    this.btnReject.Visible = this.btnReject.Enabled =
                    this.btnRefresh.Visible = this.btnRefresh.Enabled = false;
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

        protected void btnReject_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                if (String.IsNullOrWhiteSpace(this.tbSpoilComments.Text))
                {
                    this.lblErrorMessage.Text = "Please provide comments for rejecting.";
                }
                else
                {
                    this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmReject").ToString();

                    CardConfirmCurrent = CardConfirm.CONFIRM_REJECT;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled =
                    this.btnBack.Visible = this.btnBack.Enabled = true;
                    this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                    this.tbSpoilComments.Enabled = false;

                    this.btnApprove.Visible = this.btnApprove.Enabled =
                        this.btnSpoil.Enabled = this.btnSpoil.Visible =
                        this.btnSpoil.Enabled = this.btnSpoil.Visible =
                        this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                        this.btnReject.Visible = this.btnReject.Enabled =
                        this.btnCreditAnalyst.Visible = this.btnCreditAnalyst.Enabled =
                        this.btnCreditManager.Visible = this.btnCreditManager.Enabled =
                        this.btnRefresh.Visible = this.btnRefresh.Enabled = false;

                    this.tbSpoilComments.Enabled = false;
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

        protected void btnSpoil_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmSpoil").ToString();

                this.ddlSpoilReason.Enabled = false;
                this.tbSpoilComments.Enabled = false;
                CardConfirmCurrent = CardConfirm.CONFIRM_SPOIL;
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnBack.Visible = this.btnBack.Enabled = true;
                this.tbSpoilComments.Visible = this.lblSpoilComments.Visible =
                     this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = true;

                this.tbSpoilComments.Enabled = this.ddlSpoilReason.Enabled = false;

                this.btnManualLink.Enabled = this.btnManualLink.Visible =
                        this.btnReuploadCard.Enabled = this.btnReuploadCard.Visible =
                        this.btnSpoil.Enabled = this.btnSpoil.Visible =
                      this.btnApprove.Visible = this.btnApprove.Enabled =
                      this.btnReject.Visible = this.btnReject.Enabled =
                        this.btnRefresh.Visible = this.btnRefresh.Enabled = false;
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

        protected void btnPrint_OnClick(object sender, EventArgs e)
        {

            //StringWriter stringWriter = new StringWriter();

            // // Put HtmlTextWriter in using block because it needs to call Dispose.
            //using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            //{
            //    //printWindow.document.write('<html><head><title>PRINT Card</title>');
            //    //printWindow.document.write('<style type="text/css"> .custName{font-weight:bold;position:absolute;display:block;text-transform:uppercase;' + cardPrintStyle.value + '}</style>');
            //    //printWindow.document.write('</head><body style="height:206px; width:325px;">');
            //    //printWindow.document.write('<span class="custName">');
            //    //printWindow.document.write(nameOnCard);
            //    //printWindow.document.write('</span>');
            //    //printWindow.document.write('</body></html>');

            //    writer.AddAttribute(HtmlTextWriterAttribute.Class, "custName");
            //    writer.RenderBeginTag(HtmlTextWriterTag.Span); // Name on Card
            //    writer.Write("NAME ON CARD");
            //    writer.RenderEndTag();

            //    writer.AddStyleAttribute(HtmlTextWriterStyle.Top, "10");
            //    writer.AddStyleAttribute(HtmlTextWriterStyle.Left, "10");
            //    writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, "Arial");
            //    writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "12");
            //    writer.RenderBeginTag(HtmlTextWriterTag.Span); //Institute
            //    writer.Write("INSTITUTE");
            //    writer.RenderEndTag();

            //    //writer.AddAttribute(HtmlTextWriterAttribute.Src, "data:image/jpg;base64," + Convert.ToBase64String(new byte[0]));
            //    //writer.AddAttribute(HtmlTextWriterAttribute.Width, "60");
            //    //writer.AddAttribute(HtmlTextWriterAttribute.Height, "60");
            //    //writer.AddAttribute(HtmlTextWriterAttribute.Alt, "");
            //    //writer.RenderBeginTag(HtmlTextWriterTag.Img); 
            //    //writer.RenderEndTag();
            //}

            //// Return the result.
            //hdnNameOnCard.Value = stringWriter.ToString();

            //Page.ClientScript.RegisterStartupScript(this.GetType(), "showCardPrint", "PrintPanel();", true);

           this.lblInfoMessage.Text = "Card printed successfully";
        }

        protected void btnPrintSuccess_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmPrintSuccessful").ToString();
                PrintingService.Instance.Clear(hdnNameOnCard.Value);
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                CardConfirmCurrent = CardConfirm.CONFIRM_PRINT_SUCCESS;
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnBack.Visible = this.btnBack.Enabled = true;
                this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                this.tbSpoilComments.Enabled = false;

                this.btnPrintFailed.Visible = this.btnPrintFailed.Enabled =
                    this.btnPrintSuccess.Visible = this.btnPrintSuccess.Enabled =
                       this.btnSpoil.Enabled = this.btnSpoil.Visible =
                      this.btnSpoil.Enabled = this.btnSpoil.Visible =
                      this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                    this.btnRefresh.Visible = this.btnRefresh.Enabled = this.pnlPrintButtons.Visible = false;
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

        protected void btnPrintFailed_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmPrintFailed").ToString();

                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                CardConfirmCurrent = CardConfirm.CONFIRM_PRINT_FAIL;
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnBack.Visible = this.btnBack.Enabled = true;
                this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                this.tbSpoilComments.Enabled = false;

                this.btnPrintFailed.Visible = this.btnPrintFailed.Enabled =
                    this.btnPrintSuccess.Visible = this.btnPrintSuccess.Enabled =
                       this.btnSpoil.Enabled = this.btnSpoil.Visible =
                        this.btnSpoil.Enabled = this.btnSpoil.Visible =
                      this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                    this.btnRefresh.Visible = this.btnRefresh.Enabled = this.pnlPrintButtons.Visible = false;
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


        private void UploadCardDetails()
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                if (CardId != null)
                {
                    //TODO: Look at putting some of these into session.
                    var cardDetails = _batchService.GetCardDetails(CardId.Value);
                    
                    string responseMessage;
                    if (_batchService.LinkCardToAccount(CardId.Value, out responseMessage))
                    {
                        PinAuthUserId = null;
                        if (!String.IsNullOrWhiteSpace(responseMessage))
                        {
                            if (responseMessage.ToUpper() != "SUCCESS" && responseMessage.ToUpper() != "SUCCESSFUL_ACTIVATION" && responseMessage.ToUpper() != "SUCCESSFUL_ISSUE")
                            {
                                this.lblErrorMessage.Text = responseMessage;
                            }
                        }
                        spoilcardflag = false;
                        this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmuploadcardsuccess").ToString();
                        if (responseMessage.ToUpper() == "SUCCESSFUL_ISSUE")
                        {
                            this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmuploadIssueSuccess").ToString();
                        }

                        if (responseMessage.ToUpper() == "SUCCESSFUL_ACTIVATION")
                        {
                            this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmuploadActivationSuccess").ToString();
                        }

                        FetchCardDetails(CardId);
                        this.btnApprove.Enabled = this.btnApprove.Visible = false;
                        this.btnReject.Enabled = this.btnReject.Visible = false;
                        this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled = false;
                        this.btnSpoil.Enabled = this.btnSpoil.Visible =
                        this.btnSpoil.Enabled = this.btnSpoil.Visible =
                        this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                        //this.btnSpoil.Enabled = this.btnSpoil.Visible = false;
                        this.btnPrint.Enabled = this.btnPrint.Visible = false;
                        this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                        this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                        this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
                        this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                        this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = responseMessage;
                        FetchCardDetails(CardId);
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

        protected void btnLinkCardCenter_OnClick(object sender, EventArgs e)
        {
            UploadCardDetails();
        }

        protected void btnLinkCard_OnClick(object sender, EventArgs e)
        {
            UploadCardDetails();
        }

        protected void btnReuploadCard_OnClick(object sender, EventArgs e)
        {
            UploadCardDetails();
        }

        protected void btnManualLink_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmPrintFailed").ToString();

                CardConfirmCurrent = CardConfirm.CONFIRM_MANUAL_LINK;
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnBack.Visible = this.btnBack.Enabled = true;

                this.btnManualLink.Enabled = this.btnManualLink.Visible =
                    this.btnReuploadCard.Enabled = this.btnReuploadCard.Visible =
                this.btnSpoil.Enabled = this.btnSpoil.Visible =
                  this.ddlSpoilReason.Visible = this.tbSpoilComments.Visible =
                  this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled =
                    this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                        this.ddlSpoilReason.Enabled = this.tbSpoilComments.Enabled =
                        this.btnRefresh.Visible = this.btnRefresh.Enabled = false;
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

        protected void btnReprintPinMailer_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmRePrintPinMailer").ToString();

                this.btnReprintPinMailer.Visible = false;

                CardConfirmCurrent = CardConfirm.CONFIRM_PIN_MAILER_REPRINT;
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnBack.Visible = this.btnBack.Enabled = true;
                this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                this.tbSpoilComments.Enabled = false;

                this.btnPrintFailed.Visible = this.btnPrintFailed.Enabled =
                this.btnPrintSuccess.Visible = this.btnPrintSuccess.Enabled =
                this.btnSpoil.Enabled = this.btnSpoil.Visible =
                this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                this.btnRefresh.Visible = this.btnRefresh.Enabled = this.pnlPrintButtons.Visible = false;
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

        protected void btnReprintPinMailerApprove_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmRePrintPinMailer").ToString();

                this.btnReprintPinMailerReject.Visible =
                this.btnReprintPinMailerApprove.Visible = false;

                CardConfirmCurrent = CardConfirm.CONFIRM_PIN_MAILER_REPRINT_APPROVE;
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnBack.Visible = this.btnBack.Enabled = true;
                this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                this.tbSpoilComments.Enabled = false;

                this.btnSpoil.Enabled = this.btnSpoil.Visible =
                this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                this.btnPrintFailed.Visible = this.btnPrintFailed.Enabled =
                    this.btnPrintSuccess.Visible = this.btnPrintSuccess.Enabled =
                    this.btnRefresh.Visible = this.btnRefresh.Enabled = this.pnlPrintButtons.Visible = false;
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

        protected void btnReprintPinMailerReject_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                if (String.IsNullOrWhiteSpace(this.tbSpoilComments.Text))
                {
                    this.lblErrorMessage.Text = "Please provide comments for rejecting.";
                }
                else
                {
                    this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmRePrintPinMailerReject").ToString();

                    this.btnReprintPinMailerApprove.Visible =
                    this.btnReprintPinMailerReject.Visible = false;

                    CardConfirmCurrent = CardConfirm.CONFIRM_PIN_MAILER_REPRINT_REJECT;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled =
                    this.btnBack.Visible = this.btnBack.Enabled = true;
                    this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                    this.tbSpoilComments.Enabled = false;

                    this.btnSpoil.Enabled = this.btnSpoil.Visible =
                    this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                    this.btnPrintFailed.Visible = this.btnPrintFailed.Enabled =
                    this.btnPrintSuccess.Visible = this.btnPrintSuccess.Enabled =
                  this.btnRefresh.Visible = this.btnRefresh.Enabled = this.pnlPrintButtons.Visible = false;
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

        protected void btnConfirm_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                switch (CardConfirmCurrent)
                {
                    case CardConfirm.CONFIRM_APPROVE:
                        ApproveCard();
                        break;
                    case CardConfirm.CONFIRM_REJECT:
                        RejectCard();
                        break;
                    case CardConfirm.CONFIRM_SPOIL:
                        SpoilCard();
                        break;
                    case CardConfirm.CONFIRM_PRINT_SUCCESS:
                        PrintSuccess();
                        break;
                    case CardConfirm.CONFIRM_PRINT_FAIL:
                        PrintFailed();
                        break;
                    case CardConfirm.CONFIRM_MANUAL_LINK:
                        ManuallyLinkCard();
                        break;
                    case CardConfirm.CONFIRM_PIN_MAILER_REPRINT:
                        PinMailerReprint();
                        break;
                    case CardConfirm.CONFIRM_PIN_MAILER_REPRINT_APPROVE:
                        PinMailerReprintApprove();
                        break;
                    case CardConfirm.CONFIRM_PIN_MAILER_REPRINT_REJECT:
                        PinMailerReprintReject();
                        break;
                    case CardConfirm.CONFIRM_CREDIT_ANALYST:
                        ApproveCreditAnalyst();
                        break;
                    case CardConfirm.CONFIRM_CREDIT_MANAGER:
                        ApproveCreditManager();
                        break;
                    case CardConfirm.CONFIRM_CREDIT_CONTRACT:
                        SetCreditContractNumber();
                        break;
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

        private void ApproveCreditManager()
        {
            string result;

            if (_cardService.CardLimitApproveManager(CardId.Value))
            {
                this.lblInfoMessage.Text = "Credit Limit Approved.";
                spoilcardflag = false;
                FetchCardDetails(CardId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                CardConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = "Failed to approve credit limit.";
            }
        }

        private void SetCreditContractNumber()
        {
            string result;

            if (_cardService.CardLimitContractNumber(CardId.Value, tbCreditContractNumber.Text))
            {
                this.lblInfoMessage.Text = "Credit Contract Number Set.";
                spoilcardflag = false;
                FetchCardDetails(CardId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                CardConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = "Failed to set credit contract number.";
            }
        }

        private void ApproveCreditAnalyst()
        {
            string result;
            decimal updateLimit;
            if (tbCreditLimitUpdate.Visible)
            {
                decimal.TryParse(tbCreditLimitUpdate.Text, out updateLimit);
            }
            else
            {
                decimal.TryParse(tbCreditLimit.Text, out updateLimit);
            }

            if (_cardService.CardLimitApprove(CardId.Value, updateLimit))
            {
                this.lblInfoMessage.Text = "Credit Limit Approved.";
                spoilcardflag = false;
                FetchCardDetails(CardId);
                this.btnApprove.Enabled = this.btnApprove.Visible = false;
                this.btnReject.Enabled = this.btnReject.Visible = false;
                this.tbCreditLimitUpdate.Text = updateLimit.ToString("#,##0.00");
                this.tbCreditLimitUpdate.Enabled = false;
                this.rangeValidatorCreditLimit.Enabled = false;
                this.btnPrint.Enabled = this.btnPrint.Visible = false;
                this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = false;
                this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = false;
                this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible = this.pnlPrintButtons.Visible = false;
                this.btnLinkCard.Enabled = this.btnLinkCard.Visible = this.pnlCmsButtons.Visible = false;
                this.btnLinkCardCenter.Enabled = this.btnLinkCardCenter.Visible = this.pnlActivationButtons.Visible = false;
                this.btnConfirm.Visible = false;
                this.btnBack.Visible = false;
                this.btnRefresh.Visible = this.btnRefresh.Enabled = true;

                CardConfirmCurrent = null;
            }
            else
            {
                this.lblErrorMessage.Text = "Failed to approve credit limit.";
            }
        }

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.btnRefresh.Enabled = this.btnRefresh.Visible = true;

                switch (CardConfirmCurrent)
                {
                    case CardConfirm.CONFIRM_APPROVE:
                        this.btnApprove.Enabled = this.btnApprove.Visible =
                            this.btnReject.Enabled = this.btnReject.Visible = true;
                        this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled =
                         this.btnSpoil.Enabled = this.btnSpoil.Visible =
                         this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled = true;
                        this.btnBack.Visible = this.btnBack.Enabled = true;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        CardConfirmCurrent = null;
                        break;
                    case CardConfirm.CONFIRM_REJECT:
                        this.btnApprove.Enabled = this.btnApprove.Visible =
                            this.btnReject.Enabled = this.btnReject.Visible = true;
                        this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled =
                         this.btnSpoil.Enabled = this.btnSpoil.Visible =
                         this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled = true;
                        this.btnBack.Visible = this.btnBack.Enabled = true;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        this.tbSpoilComments.Enabled = true;
                        CardConfirmCurrent = null;
                        break;
                    case CardConfirm.CONFIRM_SPOIL:

                        this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled =
                        this.btnSpoil.Enabled = this.btnSpoil.Visible =
                        this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled = true;
                        this.btnRefresh.Enabled = this.btnRefresh.Visible = true;
                        this.btnApprove.Enabled = this.btnApprove.Visible =
                          this.btnReject.Enabled = this.btnReject.Visible = true;
                        //TODO: Fix manual link
                        //if (CardDetails.branch_card_statuses_id == 9)
                        //    this.btnManualLink.Enabled = this.btnManualLink.Visible = true;

                        this.btnBack.Visible = this.btnBack.Enabled = true;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        CardConfirmCurrent = null;
                        break;
                    case CardConfirm.CONFIRM_PRINT_SUCCESS:
                        this.btnPrint.Enabled = this.btnPrint.Visible =
                            this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible = 
                        this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible =
                            this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = this.pnlPrintButtons.Visible = true;
                        //this.btnBack.Visible = this.btnBack.Enabled =
                        this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled =
                         this.btnSpoil.Enabled = this.btnSpoil.Visible =
                         this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled = true;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        break;
                    case CardConfirm.CONFIRM_PRINT_FAIL:
                        this.btnPrint.Enabled = this.btnPrint.Visible =
                            this.btnSdkPrint.Enabled = this.btnSdkPrint.Visible =
                            this.btnPrintFailed.Enabled = this.btnPrintFailed.Visible =
                            this.btnPrintSuccess.Enabled = this.btnPrintSuccess.Visible = this.pnlPrintButtons.Visible = true;
                        //this.btnBack.Visible = this.btnBack.Enabled =
                        this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled =
                        this.btnSpoil.Enabled = this.btnSpoil.Visible =
                        this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled = true;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        break;
                    case CardConfirm.CONFIRM_MANUAL_LINK:
                        this.btnManualLink.Enabled = this.btnManualLink.Visible =
                            this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled =
                        this.btnSpoil.Enabled = this.btnSpoil.Visible =
                        this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled = true;
                        // this.btnBack.Visible = this.btnBack.Enabled =
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        this.btnRefresh.Enabled = this.btnRefresh.Visible = true;
                        break;
                    case CardConfirm.CONFIRM_PIN_MAILER_REPRINT_APPROVE:
                        this.btnReprintPinMailerApprove.Enabled = this.btnReprintPinMailerApprove.Visible =
                            this.btnReprintPinMailerReject.Enabled = this.btnReprintPinMailerReject.Visible = true;
                        this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled =
                        this.btnSpoil.Enabled = this.btnSpoil.Visible =
                        this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled = true;
                        this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                        this.btnRefresh.Enabled = this.btnRefresh.Visible = true;
                        CardConfirmCurrent = null;
                        break;
                    case CardConfirm.CONFIRM_PIN_MAILER_REPRINT_REJECT:
                        this.btnReprintPinMailerApprove.Enabled = this.btnReprintPinMailerApprove.Visible =
                            this.btnReprintPinMailerReject.Enabled = this.btnReprintPinMailerReject.Visible = true;
                        this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = this.tbSpoilComments.Enabled =
                       this.btnSpoil.Enabled = this.btnSpoil.Visible =
                       this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled = true;
                        this.btnRefresh.Enabled = this.btnRefresh.Visible = true;
                        CardConfirmCurrent = null;
                        break;
                    default:
                        //if (DistBatchId != null)
                        //{
                        //    SessionWrapper.DistBatchId = DistBatchId;

                        //    if (SearchParam != null)
                        //        SessionWrapper.DistBatchSearchParams = SearchParam;
                        //}
                        //if (PinBatchId != null)
                        //{
                        //    SessionWrapper.PinBatchId = PinBatchId;
                        //    SessionWrapper.PinBatchSearchParams = PinBatchSearchParameters;

                        //}
                        SessionWrapper.CardSearchParams = CardSearchParametersParams;
                        if (CardSearchParametersParams != null && CardSearchParametersParams.GetType() == typeof(PinMailerReprintSearchParameters))
                        {

                            Server.Transfer("~\\webpages\\pin\\PinMailerReprintList.aspx");
                        }
                        else
                        {
                            if (CardViewMode != null && CardViewMode.ToUpper() != "SEARCH")
                            {
                                SessionWrapper.CardViewMode = CardViewMode;
                                if (CardSearchParametersParams.GetType() == typeof(BranchCardSearchParameters))
                                {
                                    var searchparams = (BranchCardSearchParameters)CardSearchParametersParams;
                                    string status = searchparams.BranchCardStatusId.ToString();
                                    Server.Transfer("~\\webpages\\card\\CardList.aspx?status=" + status);
                                }
                                else
                                    Server.Transfer("~\\webpages\\card\\CardList.aspx");
                            }
                            else
                            {
                                Server.Transfer("~\\webpages\\card\\CustomerCardSearch.aspx");
                            }
                        }

                        break;
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

        #endregion

        #region Page Properties

        private DistBatchSearchParameters SearchParam
        {
            get
            {
                if (ViewState["SearchParam"] == null)
                    return null;
                else
                    return (DistBatchSearchParameters)ViewState["SearchParam"];
            }
            set
            {
                ViewState["SearchParam"] = value;
            }
        }

        public long? CardId
        {
            get
            {
                if (ViewState["CardId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["CardId"].ToString());
            }
            set
            {
                ViewState["CardId"] = value;
            }
        }

        //TODO: Temp fix, must be removed and call DB
        public List<ProductField> AdditionalFields
        {
            get
            {
                if (ViewState["AdditionalFields"] == null)
                    return null;
                else
                    return (List<ProductField>)ViewState["AdditionalFields"];
            }
            set
            {
                ViewState["AdditionalFields"] = value;
            }
        }



        public String CardViewMode
        {
            get
            {
                if (ViewState["CardViewMode"] == null)
                    return null;
                else
                    return ViewState["CardViewMode"].ToString();
            }
            set
            {
                ViewState["CardViewMode"] = value;
            }
        }



        public ISearchParameters CardSearchParametersParams
        {
            get
            {
                if (ViewState["CardSearchParametersParams"] != null)
                    return (ISearchParameters)ViewState["CardSearchParametersParams"];
                else
                    return null;
            }
            set
            {
                ViewState["CardSearchParametersParams"] = value;
            }
        }

        //public PinMailerReprintSearchParameters PinMailerSearchParametersParams
        //{
        //    get
        //    {
        //        if (ViewState["PinMailerSearchParametersParams"] != null)
        //            return (PinMailerReprintSearchParameters)ViewState["PinMailerSearchParametersParams"];
        //        else
        //            return null;
        //    }
        //    set
        //    {
        //        ViewState["PinMailerSearchParametersParams"] = value;
        //    }
        //}

        //private CardDetailResult CardDetails
        //{
        //    get
        //    {
        //        if (ViewState["CardDetails"] == null)
        //            return null;
        //        else
        //            return (CardDetailResult)ViewState["CardDetails"];
        //    }
        //    set
        //    {
        //        ViewState["CardDetails"] = value;
        //    }
        //}

        private CardConfirm? CardConfirmCurrent
        {
            get
            {
                if (ViewState["CardConfirm"] == null)
                    return null;
                else
                    return (CardConfirm)ViewState["CardConfirm"];
            }
            set
            {
                ViewState["CardConfirm"] = value;
            }
        }

        private long? PinAuthUserId
        {
            get
            {
                if (ViewState["PinAuthUserId"] == null)
                    return null;
                else
                    return (long)ViewState["PinAuthUserId"];
            }
            set
            {
                ViewState["PinAuthUserId"] = value;
            }
        }

        public object ListItems { get; set; }

        #endregion

        #region dlgCardStatus

        private void LoadAdditionalFields()
        {
            dlFields.DataSource = null;
            dlFields.DataBind();

            if (AdditionalFields != null)
            {
                dlFields.DataSource = AdditionalFields;
                dlFields.DataBind();
            }
            else
            {
                log.Warn(w => w("AdditionalFields empty."));
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showAddFields", "showAddFields();", true);
        }

        private void LoadCardStatus()
        {
            dlCardStatus.DataSource = null;
            dlCardStatus.DataBind();

            if (CardId == null)
            {
                this.lblReferenceError.Text = "Card Details are not valid.";
                log.Warn(w => w("Null CardId"));
            }
            else
            {
                var results = _cardService.GetCardHistory(CardId.Value).ToList();
                dlCardStatus.DataSource = results;
                dlCardStatus.DataBind();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showStatus", "showStatus();", true);
        }

        protected void dlCardStatus_ItemCommand(object source, DataListCommandEventArgs e)
        {

        }

        protected void btnAddFields_Click(object sender, EventArgs e)
        {
            LoadAdditionalFields();
        }

        protected void btnCloseAddFields_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "hideStatus", "hideStatus();", true);
        }

        protected void btnCardStatus_Click(object sender, EventArgs e)
        {
            LoadCardStatus();
        }

        protected void btnCloseStatus_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "hideStatus", "hideStatus();", true);
        }

        #endregion

        #region dlgBatchReference

        private void LoadBatchReference()
        {
            dlBatchReference.DataSource = null;
            dlBatchReference.DataBind();

            if (CardId == null)
            {
                this.lblStatusError.Text = "Card Details are not valid.";
                log.Warn(w => w("Null CardId"));
            }
            else
            {
                var results = _cardService.GetCardStatus(CardId.Value).ToList();
                dlBatchReference.DataSource = results;
                dlBatchReference.DataBind();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showReference", "showReference();", true);
        }

        protected void dlBatchReference_ItemCommand(object source, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlBatchReference.SelectedIndex = e.Item.ItemIndex;
                string distbatchref = ((LinkButton)dlBatchReference.SelectedItem.FindControl("lblBatchRefeferenceField")).Text;
                string distbatchstr = ((Label)dlBatchReference.SelectedItem.FindControl("lblDistBatchId")).Text;

                if (!string.IsNullOrWhiteSpace(distbatchref))
                {
                    SessionWrapper.DistBatchId = int.Parse(distbatchstr);
                    Server.Transfer("~\\webpages\\system\\DistBatchView.aspx");
                }
                else
                {
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.BadSelectionMessage;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected void btnBatchReference_Click(object sender, EventArgs e)
        {
            LoadBatchReference();
        }

        protected void btnCloseReference_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "hideReference", "hideReference();", true);
        }
        #endregion
        private bool Satellite_Branch_UserYN
        {
            get
            {
                if (ViewState["Satellite_Branch_UserYN"] != null)
                    return (bool)ViewState["Satellite_Branch_UserYN"];
                else
                    return false;
            }
            set
            {
                ViewState["Satellite_Branch_UserYN"] = value;
            }
        }

        private bool Main_Branch_UserYN

        {
            get
            {
                if (ViewState["Main_Branch_UserYN"] != null)
                    return (bool)ViewState["Main_Branch_UserYN"];
                else
                    return false;
            }
            set
            {
                ViewState["Main_Branch_UserYN"] = value;
            }
        }
        #region WebMethods
        [WebMethod]
        public static string GetPrintingHtml(string token)
        {
            try
            {
                return PrintingService.Instance.GetCardPrinting(token);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.StatusCode = 500;
                throw new HttpException(500, ex.Message);
            }
        }
        #endregion

        protected void btnContinue_Click(object sender, EventArgs e)
        {

        }

        protected void grdDocuments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("View"))
            {
                string fileName = e.CommandArgument.ToString();
                if (DocumentStructure.StorageType == DocumentStorageType.Local)
                {
                    if (File.Exists(fileName))
                    {
                        Response.Clear();
                        Response.AddHeader("content-disposition", "attachment;filename=" + Path.GetFileName(fileName));
                        Response.WriteFile(fileName);
                        Response.End();
                    }
                }
                else
                {
                    Response.Clear();
                    GridViewRow gvRow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    HiddenField hidDocumentTypeId = (HiddenField)gvRow.FindControl("hidDocumentTypeId");
                    int documentTypeId;
                    int.TryParse(hidDocumentTypeId.Value, out documentTypeId);
                    string downloadFileName = $"{tbAccountNumber.Text}_{DateTime.Now.ToString("yyyyMMddhhmmss")}.pdf";
                    if (documentTypeId > 0)
                    {
                        var documentType = DocumentStructure.ProductDocuments.Where(p => p.DocumentTypeId == documentTypeId).FirstOrDefault();
                        if (documentType != null)
                        {
                            downloadFileName = $"{documentType.DocumentTypeName.Replace("-","_").Replace(" ","")}_{downloadFileName}";
                        }
                    }
                    byte[] content = _documentMan.DownloadRemoteDocument(fileName);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + downloadFileName);
                    Response.BinaryWrite(content);
                    Response.End();
                }
            }
        }

        protected void grdDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                {
                    List<ListItem> documentTypes = new List<ListItem>();
                    foreach (var item in DocumentStructure.ProductDocuments)
                    {
                        documentTypes.Add(new ListItem()
                        {
                            Text = item.DocumentTypeName,
                            Value = item.DocumentTypeId.ToString()
                        });
                    }

                    DropDownList ddlDocs = (DropDownList)e.Row.FindControl("ddlDocumentType");

                    ddlDocs.Items.AddRange(documentTypes.OrderBy(m => m.Text).ToArray());

                    HiddenField hidId = (HiddenField)e.Row.FindControl("hidDocumentTypeId");
                    long documentId = 0;
                    long.TryParse(hidId.Value, out documentId);
                    if (documentId > 0)
                    {
                        ddlDocs.SelectedValue = documentId.ToString();
                    }
                }
            }
        }

        protected void btnCreditAnalyst_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmApprove").ToString();

                CardConfirmCurrent = CardConfirm.CONFIRM_CREDIT_ANALYST;
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnBack.Visible = this.btnBack.Enabled = true;
                this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                this.tbSpoilComments.Enabled = false;
                this.tbCreditLimitUpdate.Enabled = false;
                this.rangeValidatorCreditLimit.Enabled = false;

                this.btnApprove.Visible = this.btnApprove.Enabled =
                    this.btnSpoil.Enabled = this.btnSpoil.Visible =
                    this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                    this.btnReject.Visible = this.btnReject.Enabled =
                    this.btnCreditAnalyst.Visible = this.btnCreditAnalyst.Enabled =
                    this.btnCreditManager.Visible = this.btnCreditManager.Enabled =
                    this.btnCreditContractNumber.Visible = this.btnCreditContractNumber.Enabled =
                    this.btnRefresh.Visible = this.btnRefresh.Enabled = false;
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

        protected void btnCreditManager_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmApprove").ToString();

                CardConfirmCurrent = CardConfirm.CONFIRM_CREDIT_MANAGER;
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnBack.Visible = this.btnBack.Enabled = true;
                this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                this.tbSpoilComments.Enabled = false;

                this.btnApprove.Visible = this.btnApprove.Enabled =
                    this.btnSpoil.Enabled = this.btnSpoil.Visible =
                    this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                    this.btnCreditAnalyst.Visible = this.btnCreditAnalyst.Enabled =
                    this.btnCreditManager.Visible = this.btnCreditManager.Enabled =
                    this.btnCreditContractNumber.Visible = this.btnCreditContractNumber.Enabled =
                    this.btnReject.Visible = this.btnReject.Enabled =
                    this.btnRefresh.Visible = this.btnRefresh.Enabled = false;
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

        protected void btnCreditContractNumber_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmApprove").ToString();

                CardConfirmCurrent = CardConfirm.CONFIRM_CREDIT_CONTRACT;
                this.btnConfirm.Visible = this.btnConfirm.Enabled =
                this.btnBack.Visible = this.btnBack.Enabled = true;
                this.tbSpoilComments.Visible = this.lblSpoilComments.Visible = true;
                this.tbSpoilComments.Enabled = false;

                this.btnApprove.Visible = this.btnApprove.Enabled =
                    this.btnSpoil.Enabled = this.btnSpoil.Visible =
                    this.ddlSpoilReason.Visible = this.lblSpoilReason.Visible = this.ddlSpoilReason.Enabled =
                    this.btnCreditAnalyst.Visible = this.btnCreditAnalyst.Enabled =
                    this.btnCreditManager.Visible = this.btnCreditManager.Enabled =
                    this.btnCreditContractNumber.Visible = this.btnCreditContractNumber.Enabled =
                    this.btnReject.Visible = this.btnReject.Enabled =
                    this.btnRefresh.Visible = this.btnRefresh.Enabled = false;
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

   

        protected async void btnSdkPrint_OnClick(object sender, EventArgs e)
        {

           

                                    //Response.Clear();
                                    //Response.Buffer = true;
                                    //Response.ContentType = "text/plain";
                                    //Response.AddHeader("Content-Disposition", "attachment;filename=cardname.txt");
                                    //Response.Charset = "";
                                    //this.EnableViewState = true;
                                    //System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
                                    //Response.Write(customer_name);
                                    //Response.Flush();
                                    //Response.End();
            await System.Threading.Tasks.Task.Run(() => {

                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                string customer_name = "";

                if (AdditionalFields != null)
                {
                    customer_name = AdditionalFields[0].TextValue;
                }
                else
                {
                    customer_name = "No Name";
                }
                Response.ClearContent();
                Response.Clear();
                Response.ContentType = "text/txt";
                Response.AddHeader("Content-Disposition", "attachment; filename=cardname.txt;");
                Response.Write(customer_name);
                Response.FlushAsync();
                Response.End();
            

               }).ConfigureAwait(false);
            //string pathFinal = Path.Combine(_printerPath + "cardname.txt");
            //TextWriter txt = new StreamWriter(pathFinal);
            //txt.Write(customer_name);
            //txt.Close();

            //this.lblInfoMessage.Text = "File has been generated for print. The printer should start printing if the printing service is running.";

           



        }
    }
}
