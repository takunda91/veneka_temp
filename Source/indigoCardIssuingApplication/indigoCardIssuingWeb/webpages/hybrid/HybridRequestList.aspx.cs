using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.hybrid
{
    public partial class HybridRequestList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HybridRequestList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.BRANCH_OPERATOR,
                                                                        UserRole.PIN_OPERATOR,
                                                                        UserRole.CARD_PRODUCTION,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.AUDITOR};

        private BatchManagementService _batchService = new BatchManagementService();

        #region LOAD PAGE
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


                RequestStatus = null;

                if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
                {
                    RequestStatus = ((HybridRequestStatuses)int.Parse(Request.QueryString["status"]));
                    List<RolesIssuerResult> issuer;

                    if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.BRANCH_CUSTODIAN, out issuer))
                    {
                        if (issuer.Count > 0)
                        {
                           RequestSearchParamters parm = new RequestSearchParamters(issuer[0].issuer_id, null,  null, (int)RequestStatus, 1, null,1, 20);

                            DisplayResults(parm, parm.PageIndex, null);

                            SearchParameters = parm;
                            PageIndex = parm.PageIndex;
                        }
                    }
                }
                else if (SessionWrapper.RequestSearchParam != null)
                {
                    RequestSearchParamters parm =(RequestSearchParamters)SessionWrapper.RequestSearchParam;
                    DisplayResults(parm, parm.PageIndex, null);

                    SearchParameters = parm;
                    PageIndex = parm.PageIndex;
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


        }

       
        protected void dlRequestList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlRequestList.SelectedIndex = e.Item.ItemIndex;
                int selectedIndex = dlRequestList.SelectedIndex;

                string cardIdStr = ((Label)this.dlRequestList.SelectedItem.FindControl("lblRequestId")).Text;

                long selectedCardId;
                if (long.TryParse(cardIdStr, out selectedCardId))
                {
                    var result = SearchResults.Where(w => w.request_id == selectedCardId).First();
                  
                        ISearchParameters searchParms = SearchParameters;
                        searchParms.PageIndex = PageIndex;
                    
                        SessionWrapper.RequestSearchParam = searchParms;
                    
                        //RequestDetailResult request = new RequestDetailResult();
                        //card.request_id = selectedCardId;

                        //SessionWrapper.CardSearchResultItem = request;

                        if (RequestViewMode != null)
                            SessionWrapper.RequestViewMode = RequestViewMode;
                        else
                            SessionWrapper.RequestViewMode = "List";

                        SessionWrapper.RequestId=result.request_id;

                    Server.Transfer("HybridRequestView.aspx");
                    

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

            RequestSearchParamters Searchparam = (RequestSearchParamters)parms;
            if (results == null)
            {
                
                    if(Searchparam.printbatchId==null)
                    //results = _batchService.GetOperatorHybridRequestsInProgress(Searchparam.HybridRequestStatusId,null, Searchparam.PageIndex, Searchparam.RowsPerPage).ToArray();
                    results = _batchService.SearchHybridRequests((int)Searchparam.IssuerId, Searchparam.BranchId, Searchparam.ProductId, Searchparam.RequestReferenceNumber, Searchparam.HybridRequestStatusId,1, true, Searchparam.PageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();


                else
                    results = _batchService.GetRequestsByPrintBatch((long)Searchparam.printbatchId, Searchparam.PageIndex, Searchparam.RowsPerPage).ToArray();

                


            }

            if (results.Length > 0)
            {
                dlRequestList.DataSource = results;
                SearchResults = ((HybridRequestResult[])results).ToList();
                TotalPages = ((HybridRequestResult)results[0]).TOTAL_PAGES;
                totalRecords = ((HybridRequestResult)results[0]).TOTAL_ROWS;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.lblTotalRecords2.Text = totalRecords.ToString();
            dlRequestList.DataBind();
        }

        

        public List<HybridRequestResult> SearchResults
        {
            get
            {
                if (ViewState["SearchResults"] != null)
                    return (List<HybridRequestResult>)ViewState["SearchResults"];
                else
                    return null;
            }
            set
            {
                ViewState["SearchResults"] = value;
            }
        }

        public HybridRequestStatuses? RequestStatus
        {
            get
            {
                if (ViewState["RequestStatus"] != null)
                    return (HybridRequestStatuses)ViewState["RequestStatus"];
                else
                    return null;
            }
            set
            {
                ViewState["RequestStatus"] = value;
            }
        }

        //private DistBatchSearchParameters SearchParam
        //{
        //    get
        //    {
        //        if (ViewState["SearchParam"] == null)
        //            return null;
        //        else
        //            return (DistBatchSearchParameters)ViewState["SearchParam"];
        //    }
        //    set
        //    {
        //        ViewState["SearchParam"] = value;
        //    }
        //}



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

        public String RequestViewMode
        {
            get
            {
                if (ViewState["RequestViewMode"] == null)
                    return null;
                else
                    return ViewState["RequestViewMode"].ToString();
            }
            set
            {
                ViewState["RequestViewMode"] = value;
            }
        }

        #endregion
    }
}
