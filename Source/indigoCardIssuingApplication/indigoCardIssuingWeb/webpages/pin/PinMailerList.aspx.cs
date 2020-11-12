using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class PinMailerList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PinMailerList));
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
        protected void Page_Load(object sender, EventArgs e)
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
                        if (quest_status == 0)
                        {
                            pinSearchParms.PinRequestStatus = String.Empty;
                        }
                        else if (quest_status == 1)
                        {
                            pinSearchParms.PinRequestStatus = "Approved";
                        }
                        else if (quest_status == 2)
                        {
                            pinSearchParms.PinRequestStatus = "Rejected";
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
                results = _pinService.GetPinBacthForUser(pinSearchparms.IssuerId, pinSearchparms.PinRequestStatus, pinSearchparms.RowsPerPage, pageIndex).ToArray();
         
            }

            if (results.Length > 0)
            {
                this.dlCardList.DataSource = results;
                TotalPages = (int)((CardIssuanceService.TerminalCardData)results[0]).TOTAL_PAGES;
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
                string cardDetails = ((LinkButton)dlCardList.SelectedItem.FindControl("lblReference")).Text;
                string productstr = ((Label)dlCardList.SelectedItem.FindControl("lblHeaderId")).Text;

                int issuerId;
                long PinRequestId;
                if (int.TryParse(ddlIssuer.SelectedValue, out issuerId) &&
                    long.TryParse(productstr, out PinRequestId) &&
                    !String.IsNullOrWhiteSpace(cardDetails))
                {
                    //SessionWrapper.IssuerID = issuerId;
                    SessionWrapper.PinRequestId = int.Parse(productstr);
                    SessionWrapper.PinRequestSearchParams = (PinRequestSearchParameters)this.SearchParameters;

                    Response.Redirect("~\\webpages\\pin\\PinMailerView.aspx");
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