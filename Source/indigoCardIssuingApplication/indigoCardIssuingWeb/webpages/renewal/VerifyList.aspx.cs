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

namespace indigoCardIssuingWeb.webpages.renewal
{
    public partial class VerifyList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RenewalList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR,
                                                                        UserRole.BRANCH_PRODUCT_OPERATOR,
                                                                        UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.CARD_PRODUCTION,
                                                                        UserRole.PIN_PRINTER_OPERATOR,
                                                                        UserRole.AUDITOR};

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private SystemAdminService sysAdminService = new SystemAdminService();
        private bool showcheckbox = false;
        private readonly RenewalService _cardRenewalService = new RenewalService();

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
            this.dlCardRenewalList.DataSource = null;

            lblCardRenewalList.Text = Resources.CommonLabels.CardRenewalList;

            if (results == null)
            {
                results = _cardRenewalService.ListRenewals(StatusType).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlCardRenewalList.DataSource = results;
                TotalPages = results.Length / 10;
                List<RenewalFileListModel> list = results.Cast<RenewalFileListModel>().ToList();
            }
            else
            {
                TotalPages = 1;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                pnlremarks.Visible = false;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlCardRenewalList.DataBind();
        }

        private void SetButtons(RenewalFileListModel distBatch)
        {
            pnlremarks.Visible = true;
            showcheckbox = true;
        }

        private void DisplayBatchListForUser()
        {
            dlCardRenewalList.DataSource = null;

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

        protected void dlCardRenewalList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            try
            {
                dlCardRenewalList.SelectedIndex = e.Item.ItemIndex;
                int selectedIndex = dlCardRenewalList.SelectedIndex;

                string CardRenewalIdStr = ((Label)this.dlCardRenewalList.SelectedItem.FindControl("lblCardRenewalId")).Text;

                long selectedCardRenewalId;
                if (long.TryParse(CardRenewalIdStr, out selectedCardRenewalId))
                {
                    string redirectURL = string.Format("~\\webpages\\renewal\\VerifyBatch.aspx?id={0}", selectedCardRenewalId, StatusType);
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

        protected void dlCardRenewalList_ItemDataBound(object sender, DataListItemEventArgs e)
        {

        }

        protected void dlCardRenewalList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            List<long> selectedItems = new List<long>();
            lblErrorMessage.Text = string.Empty;
            foreach (DataListItem item in dlCardRenewalList.Items)
            {
                CheckBox chk = item.FindControl("chksel") as CheckBox;

                string distBatchIdStr = ((Label)item.FindControl("lblCardRenewalId")).Text;

                if (chk.Checked)
                {
                    long CardRenewalId;
                    long.TryParse(distBatchIdStr, out CardRenewalId);
                    selectedItems.Add(CardRenewalId);
                }

            }
            if (selectedItems.Count > 0)
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmRenewalInfoMessage").ToString();
            }
            else
            {
                lblErrorMessage.Text = "Select at least one funds load to approve.";
            }
        }

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            ActionIsConfirm = null;
        }

        private RenewalStatusType StatusType
        {
            get
            {
                return RenewalStatusType.Approved;
            }
        }

        private bool? ActionIsConfirm
        {
            get
            {
                if (ViewState["IsConfirm"] != null)
                    return (bool)ViewState["IsConfirm"];
                else
                    return null;
            }
            set
            {
                ViewState["IsConfirm"] = value;
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Text = string.Empty;
            List<long> selectedItems = new List<long>();
            foreach (DataListItem item in dlCardRenewalList.Items)
            {
                CheckBox chk = item.FindControl("chksel") as CheckBox;

                string distBatchIdStr = ((Label)item.FindControl("lblCardRenewalId")).Text;

                if (chk.Checked)
                {
                    long CardRenewalId;
                    long.TryParse(distBatchIdStr, out CardRenewalId);
                    selectedItems.Add(CardRenewalId);
                }

            }
            if (selectedItems.Count > 0)
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("RejectRenewalInfoMessage").ToString();
                ActionIsConfirm = false;
            }
            else
            {
                lblErrorMessage.Text = "Select at least one funds load to reject.";
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (ActionIsConfirm != null)
                {
                    bool isConfirm = ActionIsConfirm.Value;

                    List<long> selectedItems = new List<long>();
                    string notes = tbStatusNote.Text;
                    foreach (DataListItem item in dlCardRenewalList.Items)
                    {
                        CheckBox chk = item.FindControl("chksel") as CheckBox;

                        string distBatchIdStr = ((Label)item.FindControl("lblCardRenewalId")).Text;

                        if (chk.Checked)
                        {
                            long CardRenewalId;
                            long.TryParse(distBatchIdStr, out CardRenewalId);
                            selectedItems.Add(CardRenewalId);
                        }

                    }
                    if (selectedItems.Count > 0)
                    {
                        string messages = string.Empty;
                        bool actionResult = false;
                        if (isConfirm)
                        {
                            actionResult = _cardRenewalService.ApproveBulk(selectedItems, StatusType, out messages);
                        }
                        else
                        {
                            actionResult = _cardRenewalService.RejectBulk(selectedItems, StatusType, out messages);
                        }
                        if (actionResult)
                        {
                            DisplayResults(SearchParameters, SearchParameters.PageIndex, null);
                        }
                        lblInfoMessage.Text = messages;
                        tbStatusNote.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

    }
}