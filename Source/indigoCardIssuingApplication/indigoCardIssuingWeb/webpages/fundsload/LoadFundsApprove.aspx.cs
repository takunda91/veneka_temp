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

namespace indigoCardIssuingWeb.webpages.fundsload
{
    public partial class LoadFundsApprove : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoadFunds));
        private readonly FundsLoadService _fundsLoadService = new FundsLoadService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly BatchManagementService _batchService = new BatchManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR, UserRole.BRANCH_PRODUCT_OPERATOR };

        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private long FundsLoadId
        {
            get
            {
                if (ViewState["FundsLoadId"] != null)
                    return (long)ViewState["FundsLoadId"];
                else
                    return 0;
            }
            set
            {
                ViewState["FundsLoadId"] = value;
            }
        }


        private FundsLoadStatusType StatusType
        {
            get
            {
                if (ViewState["StatusType"] != null)
                    return (FundsLoadStatusType)ViewState["StatusType"];
                else
                    return FundsLoadStatusType.Created;
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
                    FundsLoadId = long.Parse(Request.QueryString["id"]);
                }

                if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
                {
                    StatusType = (FundsLoadStatusType)Enum.Parse(typeof(FundsLoadStatusType), Request.QueryString["status"]);
                }

                FundsLoadListModel model = _fundsLoadService.Retrieve(FundsLoadId);
                txtBranch.Text = model.BranchName;
                txtIssuer.Text = model.IssuerName;
                txtPrepaidCard.Text = model.PrepaidCardNo;
                txtAccountNumber.Text = model.BankAccountNo;
                txtAmount.Text = model.Amount.ToString();
                txtCreatedBy.Text = model.CreatedBy;
                txtReviewedBy.Text = model.ReviewedBy;
                txtApprovedBy.Text = model.ApprovedBy;
                txtLoadedBy.Text = model.LoadedBy;

                txtCreatedDate.Text = model.Created.ToString();
                txtReviewedDate.Text = (model.Reviewed.HasValue ? model.Reviewed.GetValueOrDefault().ToString() : string.Empty);
                txtApprovedDate.Text = (model.Approved.HasValue ? model.Approved.GetValueOrDefault().ToString() : string.Empty);
                txtLoadedDate.Text = (model.Loaded.HasValue ? model.Loaded.GetValueOrDefault().ToString() : string.Empty);

                tbFirstName.Text = model.Firstname;
                tbLastName.Text = model.LastName;
                txtAddress.Text = model.Address;
                txtPrepaidAccountNumber.Text = model.PrepaidAccountNo;

                if (StatusType == FundsLoadStatusType.Approved)
                {
                    btnSave.Text = GetLocalResourceObject("FundsLoadLoadButton").ToString();
                }
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

        private void ValidateFundsLoad()
        {
            if (panelPrepaid.Visible)
            {
                pnlButtons.Visible = true;
                btnCancel.Enabled = btnCancel.Visible = true;
                btnSave.Enabled = btnSave.Visible = true;
            }
            else
            {
                panelPrepaid.Visible = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            int statusTypeId = (int)StatusType;
            string newURL = string.Format("~\\webpages\\fundsload\\LoadFundsList.aspx?status={0}", statusTypeId);
            Server.Transfer(newURL);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                //Pass all validations, set to confirm.
                if (FormStillValid())
                {
                    this.btnCancel.Enabled = this.btnCancel.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmAccountInfoMessage").ToString();
                    btnSave.Visible = false;
                    btnReject.Visible = false;
                    ActionIsConfirm = true;
                    btnConfirm.Visible = true;
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

        private bool? ActionIsConfirm
        {
            get
            {
                if (ViewState["IsConfirm"] != null)
                    return (bool)ViewState["IsConfirm"];
                else
                    return null;
            }
            set
            {
                ViewState["IsConfirm"] = value;
            }
        }

        private bool FormStillValid()
        {
            //TODO: 
            return true;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblInfoMessage.Text = "";
                this.lblErrorMessage.Text = "";

                string resultMessage = string.Empty;
                bool result;

                if (ActionIsConfirm.GetValueOrDefault())
                {
                    FundsLoadListModel model = _fundsLoadService.Retrieve(FundsLoadId);

                    var product = _batchService.GetProduct(model.ProductId);
                    int cardIssueReasonId = product.CardIssueReasons[0];

                    result = _fundsLoadService.Approve(FundsLoadId, StatusType, cardIssueReasonId, out resultMessage);
                }
                else
                {
                    result = _fundsLoadService.Reject(FundsLoadId, StatusType, out resultMessage);
                }
                if (result)
                {
                    this.lblInfoMessage.Text = resultMessage;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                    this.btnCancel.Visible = this.btnCancel.Enabled = false;
                }
                else
                {
                    this.lblErrorMessage.Text = resultMessage;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;

                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        public Dictionary<int, RolesIssuerResult> IssuersList
        {
            get
            {
                if (ViewState["FundsLoadIssuersList"] != null)
                    return (Dictionary<int, RolesIssuerResult>)ViewState["FundsLoadIssuersList"];
                else
                    return new Dictionary<int, RolesIssuerResult>();
            }
            set
            {
                ViewState["FundsLoadIssuersList"] = value;
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_OPERATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        protected void btnReject_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                //Pass all validations, set to confirm.

                this.btnCancel.Enabled = this.btnCancel.Visible = true;
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmRejectInfoMessage").ToString();
                btnSave.Visible = false;
                btnReject.Visible = false;
                ActionIsConfirm = false;
                btnConfirm.Visible = true;
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
    }
}