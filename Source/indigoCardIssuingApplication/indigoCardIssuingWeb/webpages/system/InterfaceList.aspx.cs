using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
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
    public partial class InterfaceList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InterfaceList));
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

        #region Methods
        private void LoadPageData()
        {
            List<ListItem> connections = new List<ListItem>();
            var results = _issuerMan.GetConnectionParameters();

            ConnectionParams = results;

            DisplayResults(new InterfaceSearchParameters { PageIndex = 1, RowsPerPage = StaticDataContainer.ROWS_PER_PAGE }, 1, new ConnectionParamsResult[0]);
        }

        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {           
            this.dlInterfaceList.DataSource = null;
            this.SearchParameters = parms;

            try
            {
                if (ConnectionParams == null)
                {
                    ConnectionParams = _issuerMan.GetConnectionParameters(); 
                }

                if (ConnectionParams.Count > 0)
                {
                    ConnectionParams = ConnectionParams.OrderBy(o => o.connection_name).ToList();
                    this.dlInterfaceList.DataSource = ConnectionParams;
                }
                else
                {
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                }
                
                this.dlInterfaceList.DataBind();
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

        #region Page Properties
        private List<ConnectionParamsResult> ConnectionParams
        {
            get
            {
                if (ViewState["ConnectionParams"] != null)
                {
                    return (List<ConnectionParamsResult>)ViewState["ConnectionParams"];
                }
                return null;
            }
            set
            {
                ViewState["ConnectionParams"] = value;
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

        #region Page Events
        protected void dlInterfaceList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            dlInterfaceList.SelectedIndex = e.Item.ItemIndex;
            string ldapSettingId = ((Label)dlInterfaceList.SelectedItem.FindControl("lblConnectionParameterId")).Text;
            int id = Int16.Parse(ldapSettingId);
            SessionWrapper.InterfaceConnectionID = id;
            Server.Transfer("~\\webpages\\system\\InterfaceManagement.aspx");
        }
        #endregion
    }
}