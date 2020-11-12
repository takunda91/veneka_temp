using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class PinRequestList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PinRequestList));
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
        private readonly PINManagementService _pinService = new PINManagementService();

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
                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);


                if (issuersList.ContainsKey(-1))
                    issuersList.Remove(-1);

                this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;
                if (issuersList.Count > 0)
                {
                    PinRequestSearchParameters pinSearchParms = new PinRequestSearchParameters();

                    if (SessionWrapper.PinRequestSearchParams == null)
                    {
                        pinSearchParms = UpdateSearchParameters();
                    }
                    else
                    {
                        pinSearchParms = SessionWrapper.PinRequestSearchParams;
                        this.ddlIssuer.SelectedValue = pinSearchParms.IssuerId.ToString();

                        SessionWrapper.PinRequestSearchParams = null;
                    }
                    var quest_status = int.Parse(Request.QueryString["status"]);
                   
                    if (!String.IsNullOrWhiteSpace(quest_status.ToString()))
                    {
                        if(quest_status == 0)
                        {
                            pinSearchParms.PinRequestStatus = "NewRequest";
                        }
                        else if(quest_status == 1)
                        {
                            pinSearchParms.PinRequestStatus = "Sent";
                        }
                        else if(quest_status == 2)
                        {
                            pinSearchParms.PinRequestStatus = "Rejected";
                        }
                    }

                    var request_type = int.Parse(Request.QueryString["requestType"]);

                    if (!String.IsNullOrWhiteSpace(quest_status.ToString()))
                    {
                        pinSearchParms.PinReissueStatus = null;
                        pinSearchParms.PinApprovalStage = null;

                        if (request_type == 0)
                        {
                            pinSearchParms.PinRequestType = "NewIssue";
                      
                        }
                        else if (request_type == 1)
                        {
                            pinSearchParms.PinRequestType = "ReIssue";

                            List<RolesIssuerResult> issuers;
                            if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.CENTER_OPERATOR, out issuers))
                            {
                                pinSearchParms.PinReissueStatus = "Approved";
                                pinSearchParms.PinApprovalStage = "BRANCH_CUSTODIAN";
                                pinSearchParms.PinRequestStatus = "Approved";
                            }
                            else if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.CENTER_MANAGER, out issuers))
                            {
                                pinSearchParms.PinReissueStatus = "Approved";
                                pinSearchParms.PinApprovalStage = "CENTER_OPERATOR";
                                pinSearchParms.PinRequestStatus = "Approved";
                            }
                            else if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).TryGetValue(UserRole.PIN_OPERATOR, out issuers))
                            {
                                pinSearchParms.PinReissueStatus = "Approved";
                                pinSearchParms.PinApprovalStage = "CENTER_OPERATOR";
                                pinSearchParms.PinRequestStatus = "Approved";
                            }
                        }

                    }

                    DisplayResults(pinSearchParms, pinSearchParms.PageIndex, null);
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }
        #endregion

        #region SEARRCH PARAMS
        private PinRequestSearchParameters UpdateSearchParameters()
        {
            PinRequestSearchParameters pinSearchParms = new PinRequestSearchParameters();

            pinSearchParms.IssuerId = int.Parse(this.ddlIssuer.SelectedValue);

            pinSearchParms.PageIndex = 1;
            pinSearchParms.RowsPerPage = StaticDataContainer.ROWS_PER_PAGE;

            return pinSearchParms;
        }
        #endregion

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.dlCardList.DataSource = null;

            var pinSearchparms = (PinRequestSearchParameters)parms;
            this.SearchParameters = parms;

            if (results == null)
            {
               results = _pinService.GetPinRequestsForUser(pinSearchparms.IssuerId, pinSearchparms.PinRequestStatus, pinSearchparms.PinApprovalStage,
                                                    pinSearchparms.PinRequestType, pinSearchparms.RowsPerPage, pageIndex ).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlCardList.DataSource = results;
                TotalPages = (int)((CardIssuanceService.PinRequestList)results[0]).TOTAL_PAGES;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlCardList.DataBind();
        }

        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
           // this.lblInfoMessage.Text = "";

            try
            {
                DisplayResults(UpdateSearchParameters(), 1, null);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        protected void dlCardList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
        //    this.lblInfoMessage.Text = "";

            try
            {
                dlCardList.SelectedIndex = e.Item.ItemIndex;
                string cardDetails = ((LinkButton)dlCardList.SelectedItem.FindControl("lbtnPan")).Text;
                string productstr = ((Label)dlCardList.SelectedItem.FindControl("lblPinRequestID")).Text;

                int issuerId;
                long PinRequestId;
                if (int.TryParse(ddlIssuer.SelectedValue, out issuerId) &&
                    long.TryParse(productstr, out PinRequestId) &&
                    !String.IsNullOrWhiteSpace(cardDetails))
                {
                    var quest_status = int.Parse(Request.QueryString["status"]);
                    var request_type = int.Parse(Request.QueryString["requestType"]);
                    //SessionWrapper.IssuerID = issuerId;
                    SessionWrapper.PinRequestId = int.Parse(productstr);
                    SessionWrapper.PinRequestSearchParams = (PinRequestSearchParameters)this.SearchParameters;
                    SessionWrapper.PinRequestStatus = quest_status;
                    SessionWrapper.PinRequestType = request_type;

                   Response.Redirect("~\\webpages\\pin\\PinListView.aspx");
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


        #endregion

        protected void btnBack_OnClick(object sender, EventArgs e)
        {

        }
    }


}