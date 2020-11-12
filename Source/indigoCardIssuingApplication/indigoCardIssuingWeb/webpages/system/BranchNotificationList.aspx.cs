using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
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


namespace indigoCardIssuingWeb.webpages.system
{
    public partial class BranchNotificationList : ListPage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private UserManagementService _userMan = new UserManagementService();
        private const string PageLayoutKey = "PageLayout";
        private static readonly ILog log = LogManager.GetLogger(typeof(BranchNotificationList));
        private readonly NotificationService _notificationservice = new NotificationService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };

        #region Page Events

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (ViewState[PageLayoutKey] != null)
            {
                pageLayout = (PageLayout)ViewState[PageLayoutKey];
            }

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
                    DisplayResults(null, 1, null);

                }


                if (!String.IsNullOrWhiteSpace(Request.QueryString["delete"]))
                {
                    var deleted = int.Parse(Request.QueryString["delete"]);
                    this.ddlIssuer.SelectedValue = deleted.ToString();
                    this.lblInfoMessage.Text = "Deleted successfully.";
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


        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = String.Empty;
            this.lblInfoMessage.Text = String.Empty;

            try
            {
                int issuerId;

                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && User.Identity.Name != null)
                {

                    DisplayResults(null, 1, null);
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

        protected void dlnotificationlist_ItemCommand(object source, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlnotificationlist.SelectedIndex = e.Item.ItemIndex;
                string issuerid = ((Label)dlnotificationlist.SelectedItem.FindControl("lblissuerid")).Text;
                string cardissuemethodid = ((Label)dlnotificationlist.SelectedItem.FindControl("lblcardissuemethodid")).Text;
                string branchcardstatusid = ((Label)dlnotificationlist.SelectedItem.FindControl("lblbranchcardstatusid")).Text;
                string channelid = ((Label)dlnotificationlist.SelectedItem.FindControl("lblchannelid")).Text;


                if (!string.IsNullOrWhiteSpace(branchcardstatusid))
                {
                    NotificationMessages message = new NotificationMessages();

                    message.issuerid = int.Parse(issuerid);
                    message.cardissuemethodid = int.Parse(cardissuemethodid);
                    message.branchcardstatusesid = int.Parse(branchcardstatusid);
                    message.channel_id = int.Parse(channelid);
                    SessionWrapper.Message = message;
                    Server.Transfer("~\\webpages\\system\\ManageBranchNotification.aspx");
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

        #region Methods

        private int GetDropDownValue(string value)
        {
            int selectedId;
            int Id = 0;

            if (int.TryParse(value, out selectedId))
            {
                if (selectedId > 0)
                {
                    Id = selectedId;
                }
            }

            return Id;
        }

        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = string.Empty;
            this.lblErrorMessage.Text = "";
            this.dlnotificationlist.DataSource = null;

            if (results == null && parms == null)
            {
                int issuerId = GetDropDownValue(this.ddlIssuer.SelectedValue);

                results = _notificationservice.ListNotificationBranches(issuerId, StaticDataContainer.ROWS_PER_PAGE, pageIndex).ToArray();
            }


            if (results.Length > 0)
            {
                this.dlnotificationlist.DataSource = results;
                TotalPages = (int)((notification_branch_ListResult)results[0]).TOTAL_PAGES;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            SearchParameters = parms;

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlnotificationlist.DataBind();
        }
        public int? TotalPages
        {
            get
            {
                if (ViewState["TotalPages"] == null)
                    return 1;
                else
                    return Convert.ToInt32(ViewState["TotalPages"].ToString());
            }
            set
            {
                ViewState["TotalPages"] = value;
            }
        }

        public ISearchParameters SearchParameters
        {
            get
            {
                if (ViewState["SearchParameters"] != null)
                    return (ISearchParameters)ViewState["SearchParameters"];
                else
                    return null;
            }
            set
            {
                ViewState["SearchParameters"] = value;
            }
        }
        #endregion
    }
}