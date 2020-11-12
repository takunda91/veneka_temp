using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.CardIssuanceService;
using System.Reflection;
using indigoCardIssuingWeb.SearchParameters;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.classic
{
    public partial class RejectBatchCards : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RejectBatchCards));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR };

        private BatchManagementService _batchService = new BatchManagementService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private void LoadPageData()
        {
            try
            {
                if (SessionWrapper.DistBatchSearchParams != null)
                {
                    SearchParameters = SessionWrapper.DistBatchSearchParams;
                    SessionWrapper.DistBatchSearchParams = null;
                }

                long distBatchId = SessionWrapper.DistBatchId.Value;

                BatchRejectComments = SessionWrapper.RejectComments;
                BatchId = distBatchId;

                SessionWrapper.DistBatchId = null;
                SessionWrapper.RejectComments = null;

                BatchCardInfoSearchParameters searchParams = new BatchCardInfoSearchParameters
                {
                    DistBatchId = distBatchId,
                    PageIndex = 1
                };

                SearchParameters = searchParams;

                DisplayResults(searchParams, PageIndex, null);
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


        #region ViewState Properties
        protected long? BatchId
        {
            get
            {
                if (ViewState["BatchId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["BatchId"].ToString());
            }
            set
            {
                ViewState["BatchId"] = value;
            }
        }

        protected string BatchRejectComments
        {
            get
            {
                if (ViewState["BatchRejectComments"] == null)
                    return null;
                else
                    return ViewState["BatchRejectComments"].ToString();
            }
            set
            {
                ViewState["BatchRejectComments"] = value;
            }
        }

        #endregion

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {            
            this.lblErrorMessage.Text = "";
            dlCardRequests.DataSource = null;

            int? totalRecords = 0;

            if (results == null)
            {
                results = _batchService.GetBatchCardInfoPaged((BatchCardInfoSearchParameters)parms, StaticDataContainer.ROWS_PER_PAGE).ToArray(); //parms, pageIndex, parms.RowsPerPage);
                CardsInBatch = ((BatchCardInfo[])results).ToList();
            }

            if (results.Length > 0)
            {
                dlCardRequests.DataSource = results;
                //SearchResults = results;
                TotalPages = ((BatchCardInfo)results[0]).TOTAL_PAGES;
                totalRecords = ((BatchCardInfo)results[0]).TOTAL_ROWS;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            //this.lblPageIndex.Text = "Page " + pageIndex + " of " + TotalPages;
            this.lblTotalRecords2.Text = totalRecords.ToString();
            dlCardRequests.DataBind();
        }

        private void UpdateRejectedDataList(List<RejectBatchCardInfo> rejectedCards)
        {
            this.dlRejectedCardRequests.DataSource = rejectedCards;
            this.dlRejectedCardRequests.DataBind();
        }

        protected List<BatchCardInfo> CardsInBatch
        {
            get
            {
                if (ViewState["CardsInBatch"] != null)
                    return (List<BatchCardInfo>)ViewState["CardsInBatch"];
                else
                    return null;
            }
            set
            {
                ViewState["CardsInBatch"] = value;
            }
        }

        protected List<RejectBatchCardInfo> CardsRejected
        {
            get
            {
                if (ViewState["CardsRejected"] != null)
                    return (List<RejectBatchCardInfo>)ViewState["CardsRejected"];
                else
                    return new List<RejectBatchCardInfo>();
            }
            set
            {
                ViewState["CardsRejected"] = value;
            }
        }
        #endregion

        protected void dlCardRequests_ItemCommand(object sender, EventArgs e)
        {
            
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        protected void btnRejectCard_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                Button btn = (Button)sender;
                DataListItem item = (DataListItem)btn.NamingContainer;
                Label lbl = (Label)item.FindControl("lblCardId");

                TextBox txt = (TextBox)item.FindControl("tbReason");

                if (String.IsNullOrWhiteSpace(txt.Text))
                {
                    this.lblErrorMessage.Text = "Please enter reason for rejecting.";
                }
                else
                {

                    var card = CardsInBatch.Single(w => w.CardId == long.Parse(lbl.Text));

                    if (CardsRejected.Where(w => w.CardId == long.Parse(lbl.Text)).Count() > 0)
                    {
                        this.lblErrorMessage.Text = "Card has already been added to reject list";
                    }
                    else
                    {
                        List<RejectBatchCardInfo> rejected = CardsRejected;

                        rejected.Add(new RejectBatchCardInfo(card) { Comment = txt.Text});

                        txt.Text = String.Empty;

                        UpdateRejectedDataList(rejected);
                        CardsRejected = rejected;
                    }
                }
            }
            catch(Exception ex)
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
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        protected void btnRemoveRejectedCard_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                Button btn = (Button)sender;
                DataListItem item = (DataListItem)btn.NamingContainer;
                Label lbl = (Label)item.FindControl("lblRejectCardId");

                string sss = lbl.Text;

                var card = CardsRejected.Single(w => w.CardId == long.Parse(lbl.Text));
                List<RejectBatchCardInfo> rejected = CardsRejected;

                rejected.Remove(rejected.Single(r => r.CardId == card.CardId));


                UpdateRejectedDataList(rejected);
                CardsRejected = rejected;
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
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        protected void btnReject_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ReviewCommitMessage").ToString();
                this.pnlCardsInBatch.Enabled = false;
                this.pnlRejectedCards.Enabled = false;
                this.btnCancel.Visible =
                    this.btnConfirm.Visible = true;

                this.btnReject.Visible =
                    this.btnBack.Visible = false;
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
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                DistBatchResult result;
                string messages;
                List<RejectCardInfo> rejectList = new List<RejectCardInfo>();

                foreach(var card in CardsRejected)
                {
                    rejectList.Add(new RejectCardInfo() { CardId = card.CardId, Comments = card.Comment });
                }

                if (!_batchService.RejectProductionBatch(BatchId.Value, BatchRejectComments, rejectList, out result, out messages))
                    this.lblErrorMessage.Text = messages;
                else
                {
                    if (!String.IsNullOrWhiteSpace(messages))
                        this.lblInfoMessage.Text = messages;
                    else
                        this.lblInfoMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "ActionSuccesful").ToString();

                    this.btnConfirm.Visible = false;
                    this.btnCancel.Visible = false;
                    this.btnBack.Visible = true;
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
            this.lblErrorMessage.Text = String.Empty;
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                SessionWrapper.DistBatchSearchParams = (DistBatchSearchParameters)SearchParameters;
                SessionWrapper.DistBatchId = BatchId;
                //SessionWrapper.RejectComments = BatchRejectComments;

                Server.Transfer("~\\webpages\\dist\\DistBatchView.aspx");
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                this.pnlCardsInBatch.Enabled = 
                this.pnlRejectedCards.Enabled = true;

                this.btnCancel.Visible =
                    this.btnConfirm.Visible = false;

                this.btnReject.Visible =
                    this.btnBack.Visible = true;
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

        [Serializable]
        protected class RejectBatchCardInfo : BatchCardInfo
        {
            public bool Checked { get; set; }
            public string Comment { get; set; }

            public RejectBatchCardInfo()
            {     }

            public RejectBatchCardInfo(BatchCardInfo cardInfo)
            {
                //Type baseType = this.GetType();
                Type type = cardInfo.GetType();

                BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                PropertyInfo[] properties = type.GetProperties(flags);

                foreach(var property in properties)
                {
                    this.GetType()
                        .GetProperties(flags)
                        .Where(w=> w.Name == property.Name)
                        .First()                        
                        .SetValue(this, property.GetValue(cardInfo, null), null);
                }
            }
        }
    }
}