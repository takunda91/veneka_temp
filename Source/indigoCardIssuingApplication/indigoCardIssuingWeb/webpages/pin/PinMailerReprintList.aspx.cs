using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class PinMailerReprintList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PinMailerReprintList));
        private readonly PINManagementService _pinService = new PINManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.PIN_PRINTER_OPERATOR,
                                                                        UserRole.CARD_CENTRE_PIN_OFFICER,
                                                                        UserRole.BRANCH_PIN_OFFICER };

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
            if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
            {
                int cardStatus = int.Parse(Request.QueryString["status"]);
                List<RolesIssuerResult> issuer;

                if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.BRANCH_CUSTODIAN, out issuer))
                {
                    if (issuer.Count > 0)
                    {
                        PinMailerReprintSearchParameters parm = new PinMailerReprintSearchParameters(null, null, null, cardStatus, 1, StaticDataContainer.ROWS_PER_PAGE);

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
                if (SessionWrapper.CardSearchParams != null &&
                        SessionWrapper.CardSearchParams.GetType() == typeof(PinMailerReprintSearchParameters))
                {
                    PinMailerReprintSearchParameters parm = (PinMailerReprintSearchParameters)SessionWrapper.CardSearchParams;
                    SessionWrapper.CardSearchParams = null;

                    DisplayResults(parm, parm.PageIndex, null);

                    SearchParameters = parm;
                    PageIndex = parm.PageIndex;
                }
                else
                {
                    DisplayListForUser();
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
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

                    Server.Transfer("~\\webpages\\card\\CardView.aspx");
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
            this.lblErrorMessage.Text = "";
            dlCardList.DataSource = null;

            int? totalRecords = 0;
            string messages;
            // List<SearchBranchCardsResult> branchResults = new List<SearchBranchCardsResult>();

            if (results == null)
            {

                results = _pinService.SearchPinMailerReprint((PinMailerReprintSearchParameters)parms, pageIndex, parms.RowsPerPage).ToArray();
                
            }

            if (results.Length > 0)
            {
                dlCardList.DataSource = results;
                SearchResults = ((PinMailerReprintResult[])results).ToList();
                TotalPages = ((PinMailerReprintResult)results[0]).TOTAL_PAGES;
                totalRecords = ((PinMailerReprintResult)results[0]).TOTAL_ROWS;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            //this.lblPageIndex.Text = "Page " + pageIndex + " of " + TotalPages;
            this.lblTotalRecords2.Text = totalRecords.ToString();
            dlCardList.DataBind();
        }

        public List<PinMailerReprintResult> SearchResults
        {
            get
            {
                if (ViewState["SearchResults"] != null)
                    return (List<PinMailerReprintResult>)ViewState["SearchResults"];
                else
                    return null;
            }
            set
            {
                ViewState["SearchResults"] = value;
            }
        }

        public int? CardStatus
        {
            get
            {
                if (ViewState["CardStatus"] != null)
                    return (int?)ViewState["CardStatus"];
                else
                    return null;
            }
            set
            {
                ViewState["CardStatus"] = value;
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