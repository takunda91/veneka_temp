using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.service;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.remote
{
    public partial class RemoteCardUpdateList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RemoteCardUpdateList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.AUDITOR};

        private RemoteService _remoteService = new RemoteService();


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
                var remoteStatus = int.Parse(Request.QueryString["status"]);
                List<RolesIssuerResult> issuer;
                RemoteCardUpdateSearchParameters parm = new RemoteCardUpdateSearchParameters(null, remoteStatus, null, null, null, null, null, 1, StaticDataContainer.ROWS_PER_PAGE);
                parm.IsSearch = false;
                DisplayResults(parm, parm.PageIndex, null);
                SetResend(remoteStatus);

                SearchParameters = parm;
                PageIndex = parm.PageIndex;
            }
        }

        private void SetResend(int? remoteStatus)
        {
            bool canUpdate = false;
            bool canRead;
            bool canCreate;

            if (remoteStatus == null)
            {
                this.btnResend.Visible = false;
            }
            else if ((remoteStatus != 0 && remoteStatus != 2 && remoteStatus != 3)
                        && PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.CMS_OPERATOR, out canRead, out canUpdate, out canCreate))
            {
                this.btnResend.Visible = canUpdate;
            }
            else
            {
                this.btnResend.Visible = false;
            }

        }

        private void LoadPageData()
        {
            try
            {
                //First we check if there are any results coming from card search page
                if (String.IsNullOrWhiteSpace(Request.QueryString["status"]) &&
                        SessionWrapper.RemoteCardUpdateSearchParams != null)
                {
                    var searchParams = SessionWrapper.RemoteCardUpdateSearchParams;

                    SearchParameters = searchParams;

                    DisplayResults(searchParams, searchParams.PageIndex, null);

                    this.btnBack.Visible = searchParams.IsSearch;
                    SetResend(((RemoteCardUpdateSearchParameters)searchParams).RemoteUpdateStatusesId);


                    SessionWrapper.RemoteCardUpdateSearchParams = null;
                }
                else if (String.IsNullOrWhiteSpace(Request.QueryString["status"]) &&
                    SessionWrapper.RemoteCardUpdateSearchParams != null &&
                    SessionWrapper.RemoteCardUpdateSearchResults != null)
                {
                    var results = SessionWrapper.RemoteCardUpdateSearchResults.ToArray();
                    SearchParameters = SessionWrapper.RemoteCardUpdateSearchParams;

                    DisplayResults(SearchParameters, SearchParameters.PageIndex, results);

                    this.btnBack.Visible = SearchParameters.IsSearch;
                    SetResend(((RemoteCardUpdateSearchParameters)SearchParameters).RemoteUpdateStatusesId);

                    SessionWrapper.RemoteCardUpdateSearchParams = null;
                    SessionWrapper.RemoteCardUpdateSearchResults = null;
                }
                else
                {
                    DisplayListForUser();
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                //this.pnlButtons.Visible = false;
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

        protected void dlCardUpdateList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlCardUpdateList.SelectedIndex = e.Item.ItemIndex;
                int selectedIndex = dlCardUpdateList.SelectedIndex;

                string cardIdStr = ((Label)this.dlCardUpdateList.SelectedItem.FindControl("lblCardId")).Text;

                long selectedCardId;
                if (long.TryParse(cardIdStr, out selectedCardId))
                {
                    ISearchParameters searchParms = SearchParameters;
                    searchParms.PageIndex = PageIndex;
                    SessionWrapper.RemoteCardUpdateSearchParams = searchParms;
                    SessionWrapper.RemoteCardId = selectedCardId;

                    Response.Redirect("~\\webpages\\remote\\RemoteCardUpdateView.aspx");
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

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            SessionWrapper.RemoteCardUpdateSearchParams = SearchParameters;
            Server.Transfer("~\\webpages\\remote\\RemoteCardUpdateSearch.aspx");
        }

        protected void btnResend_OnClick(object sender, EventArgs e)
        {
            List<long> cardIds = new List<long>();

            foreach(DataListItem item in dlCardUpdateList.Items)
            {
                var chk = (CheckBox)item.FindControl("chksel");
                if (chk.Checked)
                {
                    string cardIdStr = ((Label)item.FindControl("lblCardId")).Text;
                    long selectedCardId;
                    if (long.TryParse(cardIdStr, out selectedCardId))
                    {
                        cardIds.Add(selectedCardId);
                    }
                }
            }

            if(cardIds.Count > 0)
            {
                _remoteService.ChangeRemoteCardsStatus(cardIds, 3, String.Empty);                
                DisplayResults(SearchParameters, 1, null);
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
                results = _remoteService.SearchRemoteCardUpdates((RemoteCardUpdateSearchParameters)parms, pageIndex, parms.RowsPerPage).ToArray();
            }

            if (results.Length > 0)
            {
                SearchParameters = parms;
                dlCardUpdateList.DataSource = results;
                SearchResults = ((RemoteCardUpdateSearchResult[])results).ToList();
                TotalPages = ((RemoteCardUpdateSearchResult)results[0]).TOTAL_PAGES;
                totalRecords = ((RemoteCardUpdateSearchResult)results[0]).TOTAL_ROWS;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.lblTotalRecords2.Text = totalRecords.ToString();
            dlCardUpdateList.DataBind();
        }

        public List<RemoteCardUpdateSearchResult> SearchResults
        {
            get
            {
                if (ViewState["SearchResults"] != null)
                    return (List<RemoteCardUpdateSearchResult>)ViewState["SearchResults"];
                else
                    return null;
            }
            set
            {
                ViewState["SearchResults"] = value;
            }
        }
        #endregion
    }
}