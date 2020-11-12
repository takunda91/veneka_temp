using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Web.Security;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;

namespace indigoCardIssuingWeb.webpages.card
{
    public partial class CardList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CardList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.BRANCH_OPERATOR,
                                                                        UserRole.PIN_OPERATOR,
                                                                        UserRole.CARD_PRODUCTION,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.AUDITOR, UserRole.CREDIT_ANALYST, UserRole.CREDIT_MANAGER};

        private BatchManagementService _batchService = new BatchManagementService();
        private SystemAdminService _sysAdminService = new SystemAdminService();

        #region LOAD PAGE
        protected new void Page_Load(object sender, EventArgs e)
        {

            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private void DisplayListForUser()
        {
            CardStatus = null;

            if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
            {
                BranchCardStatus cardStatus = (BranchCardStatus)int.Parse(Request.QueryString["status"]);
                List<RolesIssuerResult> issuer;

                if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.BRANCH_CUSTODIAN, out issuer) && cardStatus == BranchCardStatus.ALLOCATED_TO_CUST)
                {
                    if (issuer.Count > 0)
                    {
                        //CardSearchParameters parm = new CardSearchParameters(null, (int)UserRole.BRANCH_CUSTODIAN, null, null, null, null, null, null,
                        //                                                     null, null, (int)BranchCardStatus.ALLOCATED_TO_CUST, null, null, null, null,null,null,null, 
                        //                                                     1, StaticDataContainer.ROWS_PER_PAGE);

                        BranchCardSearchParameters parm = new BranchCardSearchParameters(null, null, (int)UserRole.BRANCH_CUSTODIAN, null, null, null, null,
                                                                                            (int)BranchCardStatus.ALLOCATED_TO_CUST, null, 1, StaticDataContainer.ROWS_PER_PAGE);

                        DisplayResults(parm, parm.PageIndex, null);

                        SearchParameters = parm;
                        PageIndex = parm.PageIndex;
                    }
                }
                else if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.BRANCH_CUSTODIAN, out issuer) &&
                            (cardStatus == BranchCardStatus.PRINT_ERROR || cardStatus == BranchCardStatus.CMS_ERROR))
                {
                    if (issuer.Count > 0)
                    {
                        CardStatus = BranchCardStatus.PRINT_ERROR;
                        BranchCardSearchParameters parm = new BranchCardSearchParameters(null, null, null, null, null, null, null,
                                                                                            null, null, 1, StaticDataContainer.ROWS_PER_PAGE);

                        DisplayResults(parm, parm.PageIndex, null);

                        SearchParameters = parm;
                        PageIndex = parm.PageIndex;
                    }
                }
                else if ((Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.BRANCH_OPERATOR, out issuer) ||
                            Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.PIN_OPERATOR, out issuer))
                            && cardStatus == BranchCardStatus.APPROVED_FOR_ISSUE)
                {
                    if (issuer.Count > 0)
                    {
                        CardStatus = BranchCardStatus.APPROVED_FOR_ISSUE;

                        BranchCardSearchParameters parm = new BranchCardSearchParameters(null, null, null, null, null, null, null,
                                                                                            (int)BranchCardStatus.APPROVED_FOR_ISSUE, null, 1, StaticDataContainer.ROWS_PER_PAGE);

                        DisplayResults(parm, parm.PageIndex, null);

                        SearchParameters = parm;
                        PageIndex = parm.PageIndex;
                    }
                }
                else if ((Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.BRANCH_OPERATOR, out issuer))
                            && cardStatus == BranchCardStatus.MAKERCHECKER_REJECT)
                {
                    if (issuer.Count > 0)
                    {
                        CardStatus = BranchCardStatus.MAKERCHECKER_REJECT;

                        BranchCardSearchParameters parm = new BranchCardSearchParameters(null, null, null, null, null, null, null,
                                                                                            (int)BranchCardStatus.MAKERCHECKER_REJECT, null, 1, StaticDataContainer.ROWS_PER_PAGE);

                        DisplayResults(parm, parm.PageIndex, null);

                        SearchParameters = parm;
                        PageIndex = parm.PageIndex;
                    }
                }
                else if ((Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.BRANCH_OPERATOR, out issuer))
                            && cardStatus == BranchCardStatus.CREDIT_CONTRACT_CAPTURE)
                {
                    if (issuer.Count > 0)
                    {
                        CardStatus = BranchCardStatus.CREDIT_CONTRACT_CAPTURE;

                        BranchCardSearchParameters parm = new BranchCardSearchParameters(null, null, null, null, null, null, null,
                                                                                            (int)BranchCardStatus.CREDIT_CONTRACT_CAPTURE, null, 1, StaticDataContainer.ROWS_PER_PAGE);

                        DisplayResults(parm, parm.PageIndex, null);

                        SearchParameters = parm;
                        PageIndex = parm.PageIndex;
                    }
                }
                else if ((Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.CREDIT_ANALYST, out issuer))
                            && cardStatus == BranchCardStatus.CREDIT_READY_TO_ANALYZE)
                {
                    if (issuer.Count > 0)
                    {
                        CardStatus = BranchCardStatus.CREDIT_READY_TO_ANALYZE;
                        BranchCardSearchParameters parm = new BranchCardSearchParameters(null, null, null, null, null, null, null,
                                                                                            (int)BranchCardStatus.CREDIT_READY_TO_ANALYZE, null, 1, StaticDataContainer.ROWS_PER_PAGE);

                        DisplayResults(parm, parm.PageIndex, null);

                        SearchParameters = parm;
                        PageIndex = parm.PageIndex;
                    }
                }
                else if ((Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.CREDIT_MANAGER, out issuer))
                            && cardStatus == BranchCardStatus.CREDIT_READY_TO_APPROVE)
                {
                    if (issuer.Count > 0)
                    {
                        CardStatus = BranchCardStatus.CREDIT_READY_TO_APPROVE;
                        BranchCardSearchParameters parm = new BranchCardSearchParameters(null, null, null, null, null, null, null,
                                                                                            (int)BranchCardStatus.CREDIT_READY_TO_APPROVE, null, 1, StaticDataContainer.ROWS_PER_PAGE);

                        DisplayResults(parm, parm.PageIndex, null);

                        SearchParameters = parm;
                        PageIndex = parm.PageIndex;
                    }
                }
            }
        }

        private void LoadPageData()
        {
            try
            {

                if (SessionWrapper.CardViewMode != null)
                {
                    CardViewMode = SessionWrapper.CardViewMode;
                    SessionWrapper.CardViewMode = null;
                }

                //First we check if there are any results coming from card search page
                if (String.IsNullOrWhiteSpace(Request.QueryString["status"]) &&
                        SessionWrapper.CardSearchResults != null &&
                        SessionWrapper.CardSearchParams != null)
                {
                    var searchParams = SessionWrapper.CardSearchParams;
                    var results = SessionWrapper.CardSearchResults.ToArray();

                    SearchParameters = searchParams;

                    DisplayResults(searchParams, searchParams.PageIndex, results);
                    this.btnBack.Visible = true;
                    SessionWrapper.CardSearchResults = null;
                    SessionWrapper.CardSearchParams = null;
                }
                else if (String.IsNullOrWhiteSpace(Request.QueryString["status"]) &&
                            SessionWrapper.CardSearchParams != null)
                {
                    ISearchParameters parm;
                    parm = SessionWrapper.CardSearchParams;
                    //SessionWrapper.CardSearchParams = null;
                    bool DistBatchId = false, PinBatchId = false, exportBatchId = false, threedBatchId = false;
                    if (SessionWrapper.CardSearchParams.GetType() == typeof(CardSearchParameters))
                    {
                        if (((CardSearchParameters)SessionWrapper.CardSearchParams).PreviousSearchParameters != null)
                        {
                            if (((CardSearchParameters)SessionWrapper.CardSearchParams).PreviousSearchParameters.GetType() == typeof(DistBatchSearchParameters))
                            {
                                DistBatchId = true;
                                //parm = ((CardSearchParameters)SessionWrapper.CardSearchParams).PreviousSearchParameters;                         
                            }
                            else if (((CardSearchParameters)SessionWrapper.CardSearchParams).PreviousSearchParameters.GetType() == typeof(PinBatchSearchParameters))
                            {
                                PinBatchId = true;
                                //parm = ((CardSearchParameters)SessionWrapper.CardSearchParams).PreviousSearchParameters;                           
                            }
                            else if (((CardSearchParameters)SessionWrapper.CardSearchParams).PreviousSearchParameters.GetType() == typeof(ExportBatchSearchParameters))
                            {
                                exportBatchId = true;
                                //parm = ((CardSearchParameters)SessionWrapper.CardSearchParams).PreviousSearchParameters;                           
                            }
                            else if (((CardSearchParameters)SessionWrapper.CardSearchParams).PreviousSearchParameters.GetType() == typeof(ThreedBatchSearchParameters))
                            {
                                threedBatchId = true;
                                //parm = ((CardSearchParameters)SessionWrapper.CardSearchParams).PreviousSearchParameters;                           
                            }
                            parm.PreviousSearchParameters = ((CardSearchParameters)SessionWrapper.CardSearchParams).PreviousSearchParameters;
                            SessionWrapper.CardSearchParams = null;
                        }
                    }

                    SessionWrapper.CardSearchParams = null;
                    DisplayResults(parm, parm.PageIndex, null);

                    SearchParameters = parm;
                    PageIndex = parm.PageIndex;


                    if (DistBatchId || CardViewMode != null || PinBatchId || exportBatchId || threedBatchId)
                        this.btnBack.Visible = true;
                }
                else
                {
                    DisplayListForUser();
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

        #region LIST SELECT ACTION

        protected void btnBack_OnClick(Object sender, EventArgs e)
        {

            if (SearchParameters != null)
            {
                if (SearchParameters.GetType() == typeof(CardSearchParameters))
                {
                    CardSearchParameters prams = ((CardSearchParameters)SearchParameters);
                    SessionWrapper.CardSearchParams = new CardSearchParameters
                    {
                        DistBatchId = prams.DistBatchId,
                        PinbatchId = prams.PinbatchId,
                        //BatchReference = BatchResult.dist_batch_reference,
                        IssuerId = prams.IssuerId,
                        RowsPerPage = StaticDataContainer.ROWS_PER_PAGE,
                        PageIndex = 1,

                    };

                }


                if (SearchParameters.PreviousSearchParameters != null && SearchParameters.PreviousSearchParameters.GetType() == typeof(DistBatchSearchParameters))
                {
                    SessionWrapper.CardSearchParams.PreviousSearchParameters = (DistBatchSearchParameters)SearchParameters.PreviousSearchParameters;
                    //SessionWrapper.DistBatchId = DistBatchId;
                    Server.Transfer("~\\webpages\\dist\\DistBatchView.aspx");
                }
                else if (SearchParameters.PreviousSearchParameters != null && SearchParameters.PreviousSearchParameters.GetType() == typeof(PinBatchSearchParameters))
                {
                    SessionWrapper.CardSearchParams.PreviousSearchParameters = (PinBatchSearchParameters)SearchParameters.PreviousSearchParameters;

                    Server.Transfer("~\\webpages\\pin\\PinBatchView.aspx");
                }
                else if (SearchParameters.PreviousSearchParameters != null && SearchParameters.PreviousSearchParameters.GetType() == typeof(ExportBatchSearchParameters))
                {
                    SessionWrapper.CardSearchParams.PreviousSearchParameters = (PinBatchSearchParameters)SearchParameters.PreviousSearchParameters;

                    Server.Transfer("~\\webpages\\export\\ExportBatchView.aspx");
                }
                else if (SearchParameters.PreviousSearchParameters != null && SearchParameters.PreviousSearchParameters.GetType() == typeof(ThreedBatchSearchParameters))
                {
                    SessionWrapper.ThreedBatchId = ((CardSearchParameters)SearchParameters).ThreedBatchId;
                    Server.Transfer("~\\webpages\\ThreeDSecure\\ThreeDSecureView.aspx");

                }
                else
                {
                    if (CardViewMode != null && CardViewMode == "CUST")
                        Server.Transfer("~\\webpages\\card\\CustomerSearch.aspx");
                    else
                        Server.Transfer("~\\webpages\\card\\CardSearch.aspx");
                }
            }






        }

        protected void dlCardList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlCardList.SelectedIndex = e.Item.ItemIndex;
                int selectedIndex = dlCardList.SelectedIndex;

                string cardIdStr = ((Label)this.dlCardList.SelectedItem.FindControl("lblCardId")).Text;

                long selectedCardId;
                if (long.TryParse(cardIdStr, out selectedCardId))
                {
                    var result = SearchResults.Where(w => w.card_id == selectedCardId).First();
                    if (result.branch_card_statuses_id == 11)
                    {
                        SessionWrapper.CardSearchResultItem = result;

                        Server.Transfer("~\\webpages\\card\\CardCapture.aspx");
                    }
                    else
                    {
                        //if (DistBatchId != null)
                        //    SessionWrapper.DistBatchId = DistBatchId;

                        //if (SearchParameters != null && SearchParameters is DistBatchSearchParameters)
                        //    SessionWrapper.DistBatchSearchParams = (DistBatchSearchParameters)SearchParameters;

                        //if (PinBatchId != null)
                        //    SessionWrapper.PinBatchId = PinBatchId;

                        ISearchParameters searchParms = SearchParameters;
                        searchParms.PageIndex = PageIndex;
                        SessionWrapper.CardSearchParams = searchParms;

                        CardSearchResult card = new CardSearchResult();
                        card.card_id = selectedCardId;

                        SessionWrapper.CardSearchResultItem = card;

                        if (CardViewMode != null)
                            SessionWrapper.CardViewMode = CardViewMode;
                        else
                            SessionWrapper.CardViewMode = "List";

                        Response.Redirect("~\\webpages\\card\\CardView.aspx");
                        //Server.Transfer("~\\webpages\\card\\CardView.aspx");
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

        #endregion

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            int? totalRecords = 0;
            string messages;

            if (results == null)
            {
                if (CardStatus != null &&
                    (CardStatus == BranchCardStatus.PIN_CAPTURED || CardStatus == BranchCardStatus.APPROVED_FOR_ISSUE || CardStatus == BranchCardStatus.CARD_PRINTED))
                {
                    results = _batchService.GetOperatorCardsInProgress(null, parms.PageIndex, parms.RowsPerPage).ToArray();
                }

                else if (CardStatus != null &&
                    (CardStatus == BranchCardStatus.PRINT_ERROR || CardStatus == BranchCardStatus.CMS_ERROR))
                {
                    results = _batchService.GetCardsInError(null, parms.PageIndex, parms.RowsPerPage).ToArray();
                }
                else if (parms.GetType() == typeof(BranchCardSearchParameters))
                {
                    results = _batchService.SearchBranchCards((BranchCardSearchParameters)parms, pageIndex, parms.RowsPerPage, out messages).ToArray();
                }
                else if (parms.GetType() == typeof(CardSearchParameters))
                {
                    results = _batchService.SearchForCards((CardSearchParameters)parms, pageIndex, parms.RowsPerPage).ToArray();
                }

            }

            if (CardStatus == BranchCardStatus.CREDIT_READY_TO_APPROVE)
            {
                var issuers = _sysAdminService.GetIssuers(1, 100);

                List<ProductlistResult> products = new List<ProductlistResult>();
                //check for other entries that may have not need for separate approval
                foreach (var item in issuers)
                {
                    products.AddRange(_batchService.GetProductsList(item.issuer_id, null, null, 1, 100));
                }

                List<int> productIds = new List<int>();

                foreach (var item in products)
                {
                    if (item.parallel_approval.GetValueOrDefault() == true)
                    {
                        productIds.Add(item.product_id);
                    }
                }

                foreach (var productId in productIds)
                {
                    parms = new BranchCardSearchParameters(null, null, null, null, null, null, null,
                                                                                           (int)BranchCardStatus.CREDIT_READY_TO_ANALYZE, null, 1, StaticDataContainer.ROWS_PER_PAGE);
                    var additional_results = _batchService.SearchBranchCards((BranchCardSearchParameters)parms, pageIndex, parms.RowsPerPage, out messages).ToList();
                    List<CardSearchResult> moveMe = ((CardSearchResult[])results).ToList();
                    moveMe.AddRange(additional_results);
                    results = moveMe.ToArray();
                }
            }

            if (results.Length > 0)
            {
                dlCardList.DataSource = results;
                SearchResults = ((CardSearchResult[])results).ToList();
                TotalPages = ((CardSearchResult)results[0]).TOTAL_PAGES;
                totalRecords = ((CardSearchResult)results[0]).TOTAL_ROWS;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.lblTotalRecords2.Text = totalRecords.ToString();
            dlCardList.DataBind();
        }

        public List<CardSearchResult> SearchResults
        {
            get
            {
                if (ViewState["SearchResults"] != null)
                    return (List<CardSearchResult>)ViewState["SearchResults"];
                else
                    return null;
            }
            set
            {
                ViewState["SearchResults"] = value;
            }
        }

        public BranchCardStatus? CardStatus
        {
            get
            {
                if (ViewState["CardStatus"] != null)
                    return (BranchCardStatus)ViewState["CardStatus"];
                else
                    return null;
            }
            set
            {
                ViewState["CardStatus"] = value;
            }
        }

        public long? DistBatchId
        {
            get
            {
                if (ViewState["DistBatchId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["DistBatchId"].ToString());
            }
            set
            {
                ViewState["DistBatchId"] = value;
            }
        }

        public long? PinBatchId
        {
            get
            {
                if (ViewState["PinBatchId"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["PinBatchId"].ToString());
            }
            set
            {
                ViewState["PinBatchId"] = value;
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

        #endregion
    }
}
