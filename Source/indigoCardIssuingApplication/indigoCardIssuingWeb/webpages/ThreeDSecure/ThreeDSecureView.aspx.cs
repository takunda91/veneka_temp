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
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;

namespace indigoCardIssuingWeb.webpages.ThreeDSecure
{
    public partial class ThreeDSecureView : BasePage
    {
        private const string ThreedBatchIdKey = "ThreedBatchIdKey";
        private const string ThreedPageLayoutKey = "ThreedPageLayoutKey";
        private enum LoadPageLayout
        {
            READ,
            CONFIRM_RECREATE,
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(ThreeDSecureView));

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private UserManagementService _userMan = new UserManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER, UserRole.CENTER_OPERATOR };

        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData(null);
                ViewState[ThreedPageLayoutKey] = LoadPageLayout.READ;
            }
        }

        private void LoadPageData(long? threeBatchId)
        {
            try
            {
                //See if theres anything in the session.
                if (threeBatchId == null && SessionWrapper.ThreedBatchId != null)
                {
                    threeBatchId = SessionWrapper.ThreedBatchId;
                }

                ThreedBatchResult ThreedBatch = null;
                if (ThreedBatch == null)
                {
                    
                    ThreedBatch = PopulatePage(threeBatchId.GetValueOrDefault());
                    BatchResult = ThreedBatch;
                    if (ThreedBatch != null)
                    {
                       // SetButtons(ThreedBatch);
                        ViewState[ThreedBatchIdKey] = ThreedBatch.threed_batch_id;
                    }
                    SessionWrapper.ThreedBatchId = null;
                }

                if (ThreedBatch == null)
                {
                    this.lblErrorMessage.Text = "No Threed batch to show.";
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
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        protected void btnReCreate_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                ViewState[ThreedPageLayoutKey] = LoadPageLayout.CONFIRM_RECREATE;
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoConfirmApprove").ToString();

              

                this.btnRecreateBatch.Enabled = false;
                this.btnRecreateBatch.Visible = false;

               

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
        private ThreedBatchResult BatchResult
        {
            get
            {
                if (ViewState["BatchResult"] == null)
                    return null;
                else
                    return (ThreedBatchResult)ViewState["BatchResult"];
            }
            set
            {
                ViewState["BatchResult"] = value;
            }
        }


        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                if (ViewState[ThreedPageLayoutKey] != null)
                {
                    LoadPageLayout loadPageLayout = (LoadPageLayout)ViewState[ThreedPageLayoutKey];

                    if (loadPageLayout == LoadPageLayout.CONFIRM_RECREATE)
                    {
                        if (ViewState[ThreedBatchIdKey] != null)
                        {
                            long ThreedBatchId = (long)ViewState[ThreedBatchIdKey];

                            string response;
                            if (_batchService.RecreateThreedBatch(ThreedBatchId, this.tbStatusNote.Text.Trim(), out response))
                            {
                                this.lblInfoMessage.Text = response;
                                ViewState[ThreedPageLayoutKey] = LoadPageLayout.READ;

                                this.btnDisplayCards.Visible = true;
                                

                                this.btnConfirm.Visible = false;
                                this.btnConfirm.Enabled = false;

                                this.tbStatusNote.Enabled = false;
                            }
                            else
                            {
                                this.lblErrorMessage.Text = response;
                            }

                            LoadPageData(ThreedBatchId);
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

        protected void btnDisplayCards_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                //SessionWrapper.BackURL = "~\\webpages\\card\\";
                //SessionWrapper.DistBatchId = BatchResult.dist_batch_id;
                SessionWrapper.CardSearchParams = new CardSearchParameters
                {
                    ThreedBatchId = this.BatchResult.threed_batch_id,
                    //BatchReference = BatchResult.dist_batch_reference,
                    IssuerId = BatchResult.issuer_id,
                    RowsPerPage = StaticDataContainer.ROWS_PER_PAGE,
                    PageIndex = 1,
                    PreviousSearchParameters = new ThreedBatchSearchParameters(),
                };
                //SessionWrapper.DistBatchSearchParams = SearchParam;
                //Response.Redirect("~\\webpages\\card\\CardList.aspx");

                Server.Transfer("~\\webpages\\card\\CardList.aspx", false);
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
            Server.Transfer("~\\webpages\\ThreeDSecure\\ThreeDSecureList.aspx");
        }

        #endregion

        private ThreedBatchResult PopulatePage(long ThreedBatchId)
        {
            var result = _batchService.GetThreedBatch(ThreedBatchId);

            if (result != null)
            {
                this.tbBatchLoadDate.Text = Convert.ToDateTime(result.date_created).ToString(DATETIME_FORMAT);
                this.tbBatchReference.Text = result.batch_reference;
                this.tbBatchStatus.Text = result.batch_status;
                this.tbNumberOfCards.Text = result.no_cards.ToString();
                this.tbStatusNote.Text = result.status_note;

                    this.btnRecreateBatch.Visible = true;
                    this.btnRecreateBatch.Visible = true;
                    this.btnConfirm.Visible = false;
                   

                
            }
            else
            {
                this.lblInfoMessage.Text = "Threed batch not found. Please try again.";
            }

            return result;
        }

        private void SetButtons(ThreedBatchResult ThreedBatch)
        {
            //disable by default.
          
            btnConfirm.Visible = false;
            btnConfirm.Enabled = false;

            bool canRead;
            bool canUpdate;
            bool canCreate;
            if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_MANAGER, out canRead, out canUpdate, out canCreate)
                || PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CENTER_OPERATOR, out canRead, out canUpdate, out canCreate))
                {
                    btnRecreateBatch.Enabled = true;
                    btnRecreateBatch.Enabled = true;
                }
            }
        }
    
}
