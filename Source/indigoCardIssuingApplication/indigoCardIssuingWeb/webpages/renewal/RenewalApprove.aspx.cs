using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.renewal
{
    public partial class RenewalApprove : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RenewalApprove));
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private SystemAdminService sysAdminService = new SystemAdminService();
        private readonly RenewalService _cardRenewalService = new RenewalService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR, UserRole.BRANCH_PRODUCT_OPERATOR };

        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private bool IsConfirmation
        {
            get
            {
                if (ViewState["IsConfirmation"] != null)
                    return (bool)ViewState["IsConfirmation"];
                else
                    return false;
            }
            set
            {
                ViewState["IsConfirmation"] = value;
            }
        }

        private long RenewalId
        {
            get
            {
                if (ViewState["RenewalId"] != null)
                    return (long)ViewState["RenewalId"];
                else
                    return 0;
            }
            set
            {
                ViewState["RenewalId"] = value;
            }
        }


        private RenewalStatusType StatusType
        {
            get
            {
                if (ViewState["StatusType"] != null)
                    return (RenewalStatusType)ViewState["StatusType"];
                else
                    return RenewalStatusType.LoadConfirmed;
            }
            set
            {
                ViewState["StatusType"] = value;
            }
        }

        private void LoadPageData()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(Request.QueryString["id"]))
                {
                    RenewalId = long.Parse(Request.QueryString["id"]);
                }

                if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
                {
                    StatusType = (RenewalStatusType)Enum.Parse(typeof(RenewalStatusType), Request.QueryString["status"]);
                }

                RenewalFileViewModel model = _cardRenewalService.Retrieve(RenewalId);
                txtFileName.Text = model.FileName;
                txtUploaded.Text = model.DateUploaded.ToShortDateString();
                txtStatus.Text = model.Status.ToString();
                txtCreatedBy.Text = model.CreatedByName;
                txtCreatedDate.Text = model.CreateDate.ToString();
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

        protected void btnReject_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("RejectRenewalInfoMessage").ToString();
                btnApprove.Visible = false;
                btnReject.Visible = false;
                btnConfirm.Visible = true;
                IsConfirmation = false;
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

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            btnReject.Visible = false;
            btnApprove.Visible = false;
            try
            {
                long cardRenewalId = RenewalId;
                if (cardRenewalId != 0)
                {
                    string messages;
                    if (IsConfirmation)
                    {
                        if (_cardRenewalService.Approve(cardRenewalId, out messages))
                        {
                            lblInfoMessage.Text = "File upload approved.";
                            btnConfirm.Visible = false;
                            btnPrint.Visible = true;
                        }
                        else
                        {
                            lblErrorMessage.Text = "File upload failed to approve.";
                            btnReject.Visible = true;
                            btnApprove.Visible = true;
                        }
                    }
                    else
                    {
                        if (_cardRenewalService.Reject(cardRenewalId, out messages))
                        {
                            lblInfoMessage.Text = "File upload rejected.";
                            btnConfirm.Visible = false;
                        }
                        else
                        {
                            lblErrorMessage.Text = "File upload failed to reject.";
                            btnReject.Visible = true;
                            btnApprove.Visible = true;
                        }
                    }

                }
                else
                {
                    btnReject.Visible = true;
                    btnApprove.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "Upload status: The file could not be approved. The following error occured: " + ex.Message;
                btnReject.Visible = true;
                btnApprove.Visible = true;
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmRenewalInfoMessage").ToString();
                btnApprove.Visible = false;
                btnReject.Visible = false;
                btnConfirm.Visible = true;
                btnPrint.Visible = false;
                IsConfirmation = true;
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
            Server.Transfer("\\webpages\\renewal\\RenewalList.aspx?status=" + Convert.ToInt32(StatusType).ToString());
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {

                var reportBytes = _cardRenewalService.GenerateRenewalFileReport(RenewalId);
                string reportName = String.Empty;
                reportName = $"Renewal_File_Report_{RenewalId}_{DateTime.Now.ToString("ddd_dd_MMMM_yyyy") }.pdf";
                Response.Clear();
                Response.ClearHeaders();
                MemoryStream ms = new MemoryStream(reportBytes);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.Flush();
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



        //protected void btnValidatePrepaid_Click(object sender, EventArgs e)
        //{
        //    ValidateFundsLoad();
        //}

        //protected void btnCancel_Click(object sender, EventArgs e)
        //{
        //    int statusTypeId = (int)StatusType;
        //    string newURL = string.Format("~\\webpages\\fundsload\\LoadFundsList.aspx?status={0}", statusTypeId);
        //    Server.Transfer(newURL);
        //}

        //[PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        //[PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        //[PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    this.lblErrorMessage.Text = "";
        //    this.lblInfoMessage.Text = "";

        //    try
        //    {
        //        //Pass all validations, set to confirm.
        //        if (FormStillValid())
        //        {
        //            this.btnCancel.Enabled = this.btnCancel.Visible = true;
        //            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmAccountInfoMessage").ToString();
        //            btnSave.Visible = false;
        //            btnReject.Visible = false;
        //            ActionIsConfirm = true;
        //            btnConfirm.Visible = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;

        //        if (log.IsDebugEnabled || log.IsTraceEnabled)
        //        {
        //            this.lblErrorMessage.Text = ex.ToString();
        //        }
        //    }
        //}

        //private bool? ActionIsConfirm
        //{
        //    get
        //    {
        //        if (ViewState["IsConfirm"] != null)
        //            return (bool)ViewState["IsConfirm"];
        //        else
        //            return null;
        //    }
        //    set
        //    {
        //        ViewState["IsConfirm"] = value;
        //    }
        //}

        //private bool FormStillValid()
        //{
        //    //TODO: 
        //    return true;
        //}

        //[PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        //[PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        //[PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        //protected void btnConfirm_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        this.lblInfoMessage.Text = "";
        //        this.lblErrorMessage.Text = "";

        //        string resultMessage = string.Empty;
        //        bool result;

        //        if (ActionIsConfirm.GetValueOrDefault())
        //        {
        //            FundsLoadListModel model = _fundsLoadService.Retrieve(CardRenewalId);

        //            var product = _batchService.GetProduct(model.ProductId);
        //            int cardIssueReasonId = product.CardIssueReasons[0];

        //            result = _fundsLoadService.Approve(CardRenewalId, StatusType, cardIssueReasonId, out resultMessage);
        //        }
        //        else
        //        {
        //            result = _fundsLoadService.Reject(CardRenewalId, StatusType, out resultMessage);
        //        }
        //        if (result)
        //        {
        //            this.lblInfoMessage.Text = resultMessage;
        //            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
        //            this.btnCancel.Visible = this.btnCancel.Enabled = false;
        //        }
        //        else
        //        {
        //            this.lblErrorMessage.Text = resultMessage;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.InnerException);
        //        this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;

        //        if (log.IsDebugEnabled || log.IsTraceEnabled)
        //        {
        //            this.lblErrorMessage.Text = ex.ToString();
        //        }
        //    }
        //}

        //public Dictionary<int, RolesIssuerResult> IssuersList
        //{
        //    get
        //    {
        //        if (ViewState["FundsLoadIssuersList"] != null)
        //            return (Dictionary<int, RolesIssuerResult>)ViewState["FundsLoadIssuersList"];
        //        else
        //            return new Dictionary<int, RolesIssuerResult>();
        //    }
        //    set
        //    {
        //        ViewState["FundsLoadIssuersList"] = value;
        //    }
        //}

        //[PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        //[PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        //[PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        //protected void btnReject_Click(object sender, EventArgs e)
        //{
        //    this.lblErrorMessage.Text = "";
        //    this.lblInfoMessage.Text = "";

        //    try
        //    {
        //        //Pass all validations, set to confirm.

        //        this.btnCancel.Enabled = this.btnCancel.Visible = true;
        //        this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmRejectInfoMessage").ToString();
        //        btnSave.Visible = false;
        //        btnReject.Visible = false;
        //        ActionIsConfirm = false;
        //        btnConfirm.Visible = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;

        //        if (log.IsDebugEnabled || log.IsTraceEnabled)
        //        {
        //            this.lblErrorMessage.Text = ex.ToString();
        //        }
        //    }
        //}
    }
}