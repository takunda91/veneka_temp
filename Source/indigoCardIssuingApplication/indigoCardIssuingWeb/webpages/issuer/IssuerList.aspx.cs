using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Web.Security;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;

namespace indigoCardIssuingWeb.webpages.issuer
{
    public partial class IssuerList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IssuerList));
        private readonly SystemAdminService sysAdminService = new SystemAdminService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ISSUER_ADMIN };

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                try
                {
                    DisplayResults(new IssuerSearchParameters { PageIndex = 1, RowsPerPage = StaticDataContainer.ROWS_PER_PAGE }, 1, null);
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
        }       

        #region Page Events
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.dlIssuersList.DataSource = null;
            this.lblErrorMessage.Text = String.Empty;
            SearchParameters = parms;

            try
            {
                if (results == null)
                {
                    results = sysAdminService.GetIssuers(PageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
                }

                if (results.Length > 0)
                {
                    this.dlIssuersList.DataSource = results;

                    TotalPages = (int)((issuers_Result)results[0]).TOTAL_PAGES;
                }
                else
                {
                    TotalPages = 0;
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                }

                this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
                this.dlIssuersList.DataBind();
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

        protected void dlIssuersList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            dlIssuersList.SelectedIndex = e.Item.ItemIndex;
            string issuerID = ((Label)dlIssuersList.SelectedItem.FindControl("lblIssuerID")).Text;
            int id = Int16.Parse(issuerID);
            SessionWrapper.ManageIssuerID = id;
            Server.Transfer("~\\webpages\\issuer\\IssuerManagement.aspx");
        }
        #endregion

        #region Properties
        public int issuerid
        {
            get
            {
                if (ViewState["issuerid"] != null)
                    return (int)ViewState["issuerid"];
                else
                    return 0;
            }
            set
            {
                ViewState["issuerid"] = value;
            }
        }
        #endregion
    }
}