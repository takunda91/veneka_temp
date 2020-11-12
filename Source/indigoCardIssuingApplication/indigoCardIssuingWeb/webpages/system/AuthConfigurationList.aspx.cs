using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class AuthConfigurationList : ListPage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private UserManagementService _userMan = new UserManagementService();
        private const string PageLayoutKey = "PageLayout";
        private static readonly ILog log = LogManager.GetLogger(typeof(BatchNotificationList));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };

        #region PAGE_LOAD
        protected void Page_Load(object sender, EventArgs e)
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

        #endregion

        #region PRIVATE METHODS
        private void LoadPageData()
        {
            DisplayResults(null, 1, null);

        }
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = string.Empty;
            this.lblErrorMessage.Text = "";
            this.dlauthconfiguration.DataSource = null;

            if (results == null && parms == null)
            {

                results = _userMan.GetAuthConfigurationList( pageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
            }


            if (results.Length > 0)
            {
                this.dlauthconfiguration.DataSource = results;
                TotalPages = 0;
                //TotalPages = (int)((auth_configuration_result)results[0]);
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }


            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlauthconfiguration.DataBind();
        }
        #endregion

        #region PROPERTIES
        public int PageIndex
        {
            get
            {
                if (ViewState["PageIndex"] == null)
                    return 1;
                else
                    return Convert.ToInt32(ViewState["PageIndex"].ToString());
            }
            set
            {
                ViewState["PageIndex"] = value;
            }
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

        #endregion

        #region EVENTS
        protected void dlauthconfiguration_ItemCommand(object source, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlauthconfiguration.SelectedIndex = e.Item.ItemIndex;
                string authConfigId = ((Label)dlauthconfiguration.SelectedItem.FindControl("lblauthConfigId")).Text;
                SessionWrapper.AuthConfigId = int.Parse(authConfigId);

                Response.Redirect("~\\webpages\\system\\ManageAuthConfiguration.aspx");

            }
            catch (Exception ex)
            {
                log.Error(ex);

            }
        }
        #endregion

    }
}