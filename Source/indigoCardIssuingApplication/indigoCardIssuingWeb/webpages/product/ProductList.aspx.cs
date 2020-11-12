using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using indigoCardIssuingWeb.service;
using System.Web.Security;
using indigoCardIssuingWeb.SearchParameters;

namespace indigoCardIssuingWeb.webpages.product
{
    public partial class ProductList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductList));
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

                if (issuersList.ContainsKey(-1))
                    issuersList.Remove(-1);

                this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;
                if (issuersList.Count > 0)
                {
                    ProductSearchParameters prodSearchParms = new ProductSearchParameters();

                    if (SessionWrapper.ProductSearchParameter == null)
                    {
                        prodSearchParms = UpdateSearchParameters();                        
                    }
                    else
                    {
                        prodSearchParms = SessionWrapper.ProductSearchParameter;
                        this.ddlIssuer.SelectedValue = prodSearchParms.IssuerId.ToString();

                        if (prodSearchParms.DeletedYN == null)
                            this.ddlProductStatus.SelectedValue = "0";
                        else if (prodSearchParms.DeletedYN.Value)
                            this.ddlProductStatus.SelectedValue = "2";
                        else
                            this.ddlProductStatus.SelectedValue = "1";

                        SessionWrapper.ProductSearchParameter = null;
                    }

                    DisplayResults(prodSearchParms, prodSearchParms.PageIndex, null);                    
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

        private ProductSearchParameters UpdateSearchParameters()
        {
            ProductSearchParameters prodSearchParms = new ProductSearchParameters();

            prodSearchParms.IssuerId = int.Parse(this.ddlIssuer.SelectedValue);

            if (this.ddlProductStatus.SelectedValue == "1")
                prodSearchParms.DeletedYN = false;
            else if (this.ddlProductStatus.SelectedValue == "2")
                prodSearchParms.DeletedYN = true;

            prodSearchParms.PageIndex = 1;
            prodSearchParms.RowsPerPage = StaticDataContainer.ROWS_PER_PAGE;

            return prodSearchParms;
        }
        #endregion

        #region Page level Events
        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

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

        protected void ddlProductStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

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

        protected void dlproductlist_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlproductlist.SelectedIndex = e.Item.ItemIndex;
                string userDetails = ((LinkButton)dlproductlist.SelectedItem.FindControl("lnkProductName")).Text;
                string productstr = ((Label)dlproductlist.SelectedItem.FindControl("lblProductid")).Text;

                int issuerId;
                long userId;
                if (int.TryParse(ddlIssuer.SelectedValue, out issuerId) &&
                    long.TryParse(productstr, out userId) &&
                    !String.IsNullOrWhiteSpace(userDetails))
                {
                    //SessionWrapper.IssuerID = issuerId;
                    SessionWrapper.ProductID = int.Parse(productstr);
                    SessionWrapper.ProductSearchParameter = (ProductSearchParameters)this.SearchParameters;

                    Server.Transfer("~\\webpages\\product\\ProductAdminScreen.aspx");
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
            this.lblErrorMessage.Text = String.Empty;
            this.dlproductlist.DataSource = null;

            var prodSearchparms = (ProductSearchParameters)parms;
            this.SearchParameters = parms;           

            if (results == null)
            {
                results = _batchservice.GetProductsList(prodSearchparms.IssuerId, null, prodSearchparms.DeletedYN, pageIndex, prodSearchparms.RowsPerPage).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlproductlist.DataSource = results;
                TotalPages = (int)((ProductlistResult)results[0]).TOTAL_PAGES;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlproductlist.DataBind();
        }
        #endregion       
    }
}