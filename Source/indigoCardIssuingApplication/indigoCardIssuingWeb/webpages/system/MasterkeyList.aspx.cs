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

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class MasterkeyList : ListPage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";
        private static readonly ILog log = LogManager.GetLogger(typeof(MasterkeyList));
        private readonly TerminalManagementService _terminalService = new TerminalManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this.ddlIssuer.Items.Count == 0)
            {
                this.lblIssuer.Visible =
                    this.ddlIssuer.Visible = false;
            }
        }

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

                ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                ddlIssuer.SelectedIndex = 0;

                if (!String.IsNullOrWhiteSpace(Request.QueryString["delete"]))
                {
                    var deleted = int.Parse(Request.QueryString["delete"]);
                    this.ddlIssuer.SelectedValue = deleted.ToString();
                    this.lblInfoMessage.Text = "Master key deleted successfully.";
                }

                DisplayResults(new MasterKeySearchParameters { PageIndex = 1, RowsPerPage = StaticDataContainer.ROWS_PER_PAGE }, 1, null);
                if (ddlIssuer.Items.Count == 0)
                {
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                }

                //if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                //{
                //    lblInfoMessage.Text = "";
                //    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SaveActionMessage").ToString();
                //}
                //else
                //{
                //    // lblErrorMessage.Text = "";
                //}
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
            DisplayResults(null, 1, null);
        }

        protected void dlMasterkeyList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlMasterkeyList.SelectedIndex = e.Item.ItemIndex;
                string masterkey = ((LinkButton)dlMasterkeyList.SelectedItem.FindControl("lnkMasterkey")).Text;
                string masterkeyId = ((Label)dlMasterkeyList.SelectedItem.FindControl("lblMasterkeyId")).Text;

                if (!string.IsNullOrWhiteSpace(masterkeyId))
                {
                    SessionWrapper.MasterkeyId = int.Parse(masterkeyId);
                    Server.Transfer("~\\webpages\\system\\MasterkeyManager.aspx");
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
            this.dlMasterkeyList.DataSource = null;
            SearchParameters = parms;

            if (results == null)
            {
                results = _terminalService.GetTMKByIssuer(int.Parse(ddlIssuer.SelectedValue), pageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
            }

            if (results.Length > 0)
            {
                TotalPages = (int)((TerminalTMKIssuerResult)results[0]).TOTAL_PAGES;
                this.dlMasterkeyList.DataSource = results;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlMasterkeyList.DataBind();
        }
        #endregion
    }
}