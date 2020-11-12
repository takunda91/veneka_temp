using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Web;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.load
{
    public partial class LoadBatchDetails : BasePage
    {
        private const string LoadBatchIdKey = "LoadBatchIdKey";
        private const string LoadPageLayoutKey = "LoadPageLayoutKey";
        private enum LoadPageLayout
        {
            READ,
            CONFIRM_APPROVE,
            CONFIRM_REJECT
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(LoadBatchDetails));

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private UserManagementService _userMan = new UserManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER, UserRole.CARD_PRODUCTION };

        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData(null);
                ViewState[LoadPageLayoutKey] = LoadPageLayout.READ;
            }
        }

        private void LoadPageData(long? loadBatchId)
        {
            try
            {
                //See if theres anything in the session.
                if (loadBatchId == null && SessionWrapper.loadBatchId != null)
                {
                    loadBatchId = SessionWrapper.loadBatchId;
                }

                LoadBatchResult loadBatch = null;
                if (loadBatchId != null)
                {
                    loadBatch = PopulatePage(loadBatchId.GetValueOrDefault());

                    if (loadBatch != null)
                    {
                        SetButtons(loadBatch);
                        ViewState[LoadBatchIdKey] = loadBatch.load_batch_id;
                    }
                    SessionWrapper.loadBatchId = null;
                }

                if (loadBatch == null)
                {
                    this.lblErrorMessage.Text = "No distribution batch to show.";
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
        #endregion

        #region Page Events
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CARD_PRODUCTION")]
        protected void btnApproveBatch_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                ViewState[LoadPageLayoutKey] = LoadPageLayout.CONFIRM_APPROVE;
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmApprove").ToString();

                this.btnPDFReport.Visible = false;

                this.btnApproveBatch.Enabled = false;
                this.btnApproveBatch.Visible = false;

                this.btnRejectBatch.Enabled = false;
                this.btnRejectBatch.Visible = false;

                this.tbStatusNote.Enabled = true;
                this.btnConfirm.Enabled = true;
                this.btnConfirm.Visible = true;
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

        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CARD_PRODUCTION")]
        protected void btnRejectBatch_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                ViewState[LoadPageLayoutKey] = LoadPageLayout.CONFIRM_REJECT;
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmReject").ToString();

                this.btnPDFReport.Visible = false;

                this.btnApproveBatch.Enabled = false;
                this.btnApproveBatch.Visible = false;

                this.btnRejectBatch.Enabled = false;
                this.btnRejectBatch.Visible = false;

                this.tbStatusNote.Enabled = true;
                this.btnConfirm.Enabled = true;
                this.btnConfirm.Visible = true;
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

        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CARD_PRODUCTION")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                if (ViewState[LoadPageLayoutKey] != null)
                {
                    LoadPageLayout loadPageLayout = (LoadPageLayout)ViewState[LoadPageLayoutKey];

                    if (loadPageLayout == LoadPageLayout.CONFIRM_APPROVE)
                    {
                        if (ViewState[LoadBatchIdKey] != null)
                        {
                            long loadBatchId = (long)ViewState[LoadBatchIdKey];

                            string response;
                            if (_batchService.ApproveLoadBatch(loadBatchId, this.tbStatusNote.Text.Trim(), out response))
                            {
                                this.lblInfoMessage.Text = response;
                                ViewState[LoadPageLayoutKey] = LoadPageLayout.READ;

                                this.btnPDFReport.Visible = true;
                                //this.btnApproveBatch.Enabled = true;
                                //this.btnRejectBatch.Enabled = true;
                                //this.btnApproveBatch.Visible = true;
                                //this.btnRejectBatch.Visible = true;

                                this.btnConfirm.Visible = false;
                                this.btnConfirm.Enabled = false;

                                this.tbStatusNote.Enabled = false;
                            }
                            else
                            {
                                this.lblErrorMessage.Text = response;
                            }

                            LoadPageData(loadBatchId);
                        }
                    }
                    else if (loadPageLayout == LoadPageLayout.CONFIRM_REJECT)
                    {
                        if (ViewState[LoadBatchIdKey] != null)
                        {
                            if (String.IsNullOrWhiteSpace(this.tbStatusNote.Text.Trim()))
                            {
                                this.lblErrorMessage.Text = GetLocalResourceObject("ValidationRejectReason").ToString();
                            }
                            else
                            {
                                long loadBatchId = (long)ViewState[LoadBatchIdKey];

                                string response;         

                                if (_batchService.RejectLoadBatch(loadBatchId, this.tbStatusNote.Text.Trim(), out response))
                                {
                                    this.lblInfoMessage.Text = response;
                                    ViewState[LoadPageLayoutKey] = LoadPageLayout.READ;

                                    this.btnPDFReport.Visible = true;
                                    //this.btnApproveBatch.Enabled = true;
                                    //this.btnRejectBatch.Enabled = true;
                                    //this.btnApproveBatch.Visible = true;
                                    //this.btnRejectBatch.Visible = true;

                                    this.btnConfirm.Visible = false;
                                    this.btnConfirm.Enabled = false;

                                    this.tbStatusNote.Enabled = false;
                                }
                                else
                                {
                                    this.lblErrorMessage.Text = response;
                                }
                                LoadPageData(loadBatchId);
                            }
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
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CARD_PRODUCTION")]
        protected void btnPDFReport_Click(object sender, EventArgs e)
        {
            try
            {
                var reportBytes = _batchService.GenerateLoadBatchReport((long)ViewState[LoadBatchIdKey]);

                string reportName = "Load_Report_" + ViewState[LoadBatchIdKey].ToString() + "_" +
                                        DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                Response.Clear();
                MemoryStream ms = new MemoryStream(reportBytes);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                //Response.End();
                //HttpContext.Current.ApplicationInstance.CompleteRequest();
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
            Server.Transfer("~\\webpages\\load\\LoadBatchList.aspx");
        }

        #endregion
        
        private LoadBatchResult PopulatePage(long loadBatchId)
        {
            var result = _batchService.GetLoadBatch(loadBatchId);

            if (result != null)
            {
                this.tbBatchLoadDate.Text = Convert.ToDateTime(result.load_date).ToString(DATETIME_FORMAT);
                this.tbBatchReference.Text = result.load_batch_reference;
                this.tbBatchStatus.Text = result.load_batch_status_name;
                this.tbNumberOfCards.Text = result.no_cards.ToString();
                this.tbStatusNote.Text = result.status_notes;

                if ((LoadBatchStatus)result.load_batch_statuses_id == LoadBatchStatus.LOADED)
                {
                    this.btnApproveBatch.Visible = true;
                    this.btnRejectBatch.Visible = true;

                    this.btnApproveBatch.Enabled = true;
                    this.btnRejectBatch.Enabled = true;
                    
                }
                else
                {
                    this.btnApproveBatch.Visible = false;
                    this.btnRejectBatch.Visible = false;
                    this.btnConfirm.Visible = false;
                    this.btnApproveBatch.Enabled = false;
                    this.btnRejectBatch.Enabled = false;

                }
            }
            else
            {
                this.lblInfoMessage.Text = "Load batch not found. Please try again.";
            }

            return result;
        }

        private void SetButtons(LoadBatchResult loadBatch)
        {
            //disable by default.
            btnApproveBatch.Enabled = false;
            btnRejectBatch.Enabled = false;
            btnApproveBatch.Visible = false;
            btnRejectBatch.Visible = false;
            btnConfirm.Visible = false;
            btnConfirm.Enabled = false;

            bool canRead;
            bool canUpdate;
            bool canCreate;
            if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_MANAGER, out canRead, out canUpdate, out canCreate))
            {
                if (loadBatch != null && canUpdate &&
                     (LoadBatchStatus)loadBatch.load_batch_statuses_id == LoadBatchStatus.LOADED)
                {
                    btnApproveBatch.Enabled = true;
                    btnRejectBatch.Enabled = true;
                    btnApproveBatch.Visible = true;
                    btnRejectBatch.Visible = true;
                }
            }
        }
    }
}
