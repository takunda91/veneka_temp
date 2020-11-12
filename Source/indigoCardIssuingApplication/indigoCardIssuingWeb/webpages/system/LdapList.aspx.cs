using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
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

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class LdapList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LdapList));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        #region Helper Methods
        private void LoadPageData()
        {
            DisplayResults(new AuthenticationSearchParameters { PageIndex = 1, RowsPerPage = StaticDataContainer.ROWS_PER_PAGE }, 1, null);
        }

        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = "";
            this.dlLDAPList.DataSource = null;
            SearchParameters = parms;

            try
            {
                if (LdapSettings == null)
                {
                    LdapSettings = _issuerMan.GetLdapSettings(); //PageIndex, StaticDataContainer.ROWS_PER_PAGE);
                }

                if (LdapSettings.Count > 0)
                {
                    this.dlLDAPList.DataSource = LdapSettings;

                    //TotalPages = (int)LdapSettings[0].TOTAL_PAGES;
                }
                else
                {
                    //TotalPages = 0;
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                }

                //this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
                this.dlLDAPList.DataBind();
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

        #region Page Events

        protected void dlLDAPList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            dlLDAPList.SelectedIndex = e.Item.ItemIndex;
            string ldapSettingId = ((Label)dlLDAPList.SelectedItem.FindControl("lblLdapID")).Text;
            int id = Int16.Parse(ldapSettingId);
            SessionWrapper.LDAPID = id;
            Server.Transfer("~\\webpages\\system\\LdapManagement.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Page Properties
        private List<LdapSettingsResult> LdapSettings
        {
            get
            {
                if (ViewState["LdapSettings"] != null)
                {
                    return (List<LdapSettingsResult>)ViewState["LdapSettings"];
                }

                return null;
            }

            set
            {
                ViewState["LdapSettings"] = value;
            }
        }

        private int? LdapSettingId
        {
            get
            {
                if (ViewState["LdapSettingId"] != null)
                {
                    return (int)ViewState["LdapSettingId"];
                }

                return null;
            }

            set
            {
                ViewState["LdapSettingId"] = value;
            }
        }

        private string Password
        {
            get
            {
                if (ViewState["Password"] != null)
                {
                    return ViewState["Password"].ToString();
                }

                return null;
            }

            set
            {
                ViewState["Password"] = value;
            }
        }

        private PageLayout? pageLayout
        {
            get
            {
                if (ViewState["pageLayout"] != null)
                {
                    return (PageLayout)ViewState["pageLayout"];
                }

                return null;
            }
            set
            {
                ViewState["pageLayout"] = value;
            }
        }
        #endregion
    }
}