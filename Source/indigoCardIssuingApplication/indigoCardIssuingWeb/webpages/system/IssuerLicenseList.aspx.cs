using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using System.Data;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class IssuerLicenseList : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IssuerLicenseList));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };
        private readonly SystemAdminService _sysAdmin = new SystemAdminService();

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
                var results = _sysAdmin.GetLicenseListIssuers(null);
                DateTime dt = new DateTime(0001, 1, 1);

                IndigoComponentLicense = results;
                dlLicenselist.DataSource = results;
                dlLicenselist.DataBind();
            }
            catch (Exception ex)
            {
                //this.pnlButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }


        protected void dlLicenselist_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlLicenselist.SelectedIndex = e.Item.ItemIndex;
                string IssuerName = ((LinkButton)dlLicenselist.SelectedItem.FindControl("lnkIssuerName")).Text;
                IndigoComponentLicense result = IndigoComponentLicense.FirstOrDefault(i=>i.IssuerName==IssuerName);
         

              
                if (!String.IsNullOrWhiteSpace(IssuerName))
                {
                    SessionWrapper.IndigoComponentLicense = result;
                    Server.Transfer("~\\webpages\\system\\IssuerLicenseManager.aspx");
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

        public List<IndigoComponentLicense> IndigoComponentLicense
        {
            get
            {
                if (ViewState["IndigoComponentLicense"] == null)
                    return null;
                else
                    return (List<IndigoComponentLicense>)ViewState["IndigoComponentLicense"];  
            }
            set
            {
                ViewState["IndigoComponentLicense"] = value;
            }
        }       

        protected void dlLicenselist_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
           e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblExpiry = ((Label)e.Item.FindControl("lblExpiry"));

                if (((IndigoComponentLicense)e.Item.DataItem).ExpiryDate.ToString().Contains("1/1/0001"))
                {
                    lblExpiry.Text = "No Valid License";
                }
            }
        }
    }
}