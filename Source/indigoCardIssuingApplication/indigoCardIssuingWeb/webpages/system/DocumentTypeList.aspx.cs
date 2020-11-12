using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class DocumentTypeList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentTypeList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR};

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private SystemAdminService sysAdminService = new SystemAdminService();
        private bool showcheckbox = false;
        private readonly DocumentManagementService _cardRenewalService = new DocumentManagementService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private void LoadPageData()
        {
            DisplayBatchListForUser();
        }

        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = "";
            this.dlDocumentTypeList.DataSource = null;

            lblCardRenewalList.Text = Resources.CommonLabels.CardRenewalList;

            if (results == null)
            {
                results = _cardRenewalService.DocumentTypeAll(false).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlDocumentTypeList.DataSource = results;
                TotalPages = results.Length / 10;
                List<DocumentType> list = results.Cast<DocumentType>().ToList();
            }
            else
            {
                TotalPages = 1;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                pnlremarks.Visible = false;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlDocumentTypeList.DataBind();
        }

        private void DisplayBatchListForUser()
        {
            dlDocumentTypeList.DataSource = null;

            try
            {
                DisplayResults(null, 1, null);
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

        protected void dlDocumentTypeList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            try
            {
                dlDocumentTypeList.SelectedIndex = e.Item.ItemIndex;
                int selectedIndex = dlDocumentTypeList.SelectedIndex;

                string CardRenewalIdStr = ((Label)this.dlDocumentTypeList.SelectedItem.FindControl("lblDocumentTypeId")).Text;

                long selectedCardRenewalId;
                if (long.TryParse(CardRenewalIdStr, out selectedCardRenewalId))
                {
                    string redirectURL = string.Format("~\\webpages\\system\\DocumentTypeEdit.aspx?id={0}", selectedCardRenewalId);
                    Response.Redirect(redirectURL);
                }
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

        protected void dlDocumentTypeList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType== ListItemType.Item || e.Item.ItemType== ListItemType.AlternatingItem)
            {
                CheckBox check = (CheckBox)e.Item.FindControl("chksel");
                DocumentType data = (DocumentType)e.Item.DataItem;
                check.Enabled = false;
                check.Checked = data.IsActive;
            }
        }

        protected void dlDocumentTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}