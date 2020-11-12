using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using System.Threading;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.service;
using System.Web.Security;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;

namespace indigoCardIssuingWeb.webpages.product
{
    public partial class ProductFeeList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductFeeList));
        private readonly BatchManagementService _batchservice = new BatchManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ISSUER_ADMIN };

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

                if (!String.IsNullOrWhiteSpace(Request.QueryString["delete"]))
                {
                    var deleted = int.Parse(Request.QueryString["delete"]);
                    if (deleted == 1)
                    {
                        this.lblInfoMessage.Text = "Fee scheme deleted successfully.";
                    }
                }

                if (issuersList.ContainsKey(-1))
                    issuersList.Remove(-1);

                this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;
                if (issuersList.Count > 0)
                {
                    IssuerId = int.Parse(this.ddlIssuer.SelectedValue);
                    DisplayResults(new ProductFeeSearchParameters { PageIndex = 1, RowsPerPage = StaticDataContainer.ROWS_PER_PAGE }, 1, null);                    
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

        #region Page level Events
        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                IssuerId = int.Parse(ddlIssuer.SelectedValue);
                DisplayResults(null, 1, null);
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

        protected void dlFeeSchemeList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlFeeSchemeList.SelectedIndex = e.Item.ItemIndex;
                string userDetails = ((LinkButton)dlFeeSchemeList.SelectedItem.FindControl("lnkFeeSchemeName")).Text;
                string productstr = ((Label)dlFeeSchemeList.SelectedItem.FindControl("lblFeeSchemeId")).Text;

                int issuerId;
                int userId;
                if (int.TryParse(ddlIssuer.SelectedValue, out issuerId) &&
                    int.TryParse(productstr, out userId) &&
                    !String.IsNullOrWhiteSpace(userDetails))
                {
                    SessionWrapper.ProductFeeSchemeId = int.Parse(productstr);

                    Server.Transfer("~\\webpages\\product\\ProductFeeDetails.aspx");
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

        #region Pagination
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {            
            this.lblErrorMessage.Text = "";
            this.dlFeeSchemeList.DataSource = null;
            SearchParameters = parms;

            if (results == null)
            {
                results = _batchservice.GetFeeSchemes(IssuerId, pageIndex, StaticDataContainer.ROWS_PER_PAGE).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlFeeSchemeList.DataSource = results;
                TotalPages = (int)((FeeSchemeResult)results[0]).TOTAL_PAGES;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlFeeSchemeList.DataBind();
        }

        public int IssuerId
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