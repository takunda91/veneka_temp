using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class ExternalSystemFieldsManitanance : ListPage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";
        private static readonly ILog log = LogManager.GetLogger(typeof(MasterkeyList));
        private readonly IssuerManagementService _extenalservice = new IssuerManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };


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


                DisplayResults(null, 1, null);

                if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    lblInfoMessage.Text = "";
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SaveActionMessage").ToString();
                }
                else
                {
                    // lblErrorMessage.Text = "";
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


        protected void dlExternalSystemsfieldsList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlExternalSystemsfieldsList.SelectedIndex = e.Item.ItemIndex;
                string external_system_field_id = ((Label)dlExternalSystemsfieldsList.SelectedItem.FindControl("lblExternalSystemId")).Text;
                int externalsystemfieldid = 0;
                int.TryParse(external_system_field_id, out externalsystemfieldid);
                if (!string.IsNullOrWhiteSpace(external_system_field_id))
                {
                    SessionWrapper.ExternalSystemId = externalsystemfieldid;
                    Server.Transfer("~\\webpages\\system\\ExternalSystemFieldsViewForm.aspx");
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

        #region Methods
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = string.Empty;
            this.dlExternalSystemsfieldsList.DataSource = null;
            SearchParameters = parms;

            if (results == null)
            {
                results = _extenalservice.GetExternalSystemsFields(null, pageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
            }

            if (results.Length > 0)
            {
                TotalPages = (int)((ExternalSystemFieldsResult)results[0]).TOTAL_PAGES;
                this.dlExternalSystemsfieldsList.DataSource = results;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlExternalSystemsfieldsList.DataBind();
        }
        #endregion
    }
}