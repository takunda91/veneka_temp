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
    public partial class RenewalList : ListPage
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
            if (!String.IsNullOrWhiteSpace(Request.QueryString["batchstatus"]))
            {
                StatusType = (RenewalStatusType)int.Parse(Request.QueryString["batchstatus"]);
            }
            DisplayBatchListForUser();
        }
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            base.DisplayResults(parms, pageIndex, results);
        }

        private void FilterRecords()
        {
            this.lblErrorMessage.Text = "";
            this.dlCardRenewalList.DataSource = null;

            lblCardRenewalList.Text = Resources.CommonLabels.CardRenewalList;


            var results = _cardRenewalService.ListRenewals(StatusType).ToArray();


            if (results.Length > 0)
            {
                this.dlCardRenewalList.DataSource = results;
                TotalPages = results.Length / 10;
                List<RenewalFileListModel> list = results.Cast<RenewalFileListModel>().ToList();
                EnableButton(list);
            }
            else
            {
                TotalPages = 1;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                btnApprove.Visible = false;
                btnReject.Visible = false;
                btnConfirm.Visible = false;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, 1, TotalPages);
            this.dlCardRenewalList.DataBind();
        }

        private void SetButtons(RenewalFileListModel distBatch)
        {
            btnApprove.Text = GetLocalResourceObject("CardRenewalApproveButton").ToString();
            if (StatusType == RenewalStatusType.Approved)
            {
                this.btnApprove.Text = GetLocalResourceObject("CardRenewalLoadButton").ToString();
            }
            this.btnApprove.Visible = true;
            showcheckbox = true;
        }

        private void DisplayBatchListForUser()
        {
            dlCardRenewalList.DataSource = null;

            try
            {
                FilterRecords();
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

        private void EnableButton(List<RenewalFileListModel> list)
        {

            btnApprove.Visible = false;

            if (list.Count() > 0)
            {
                SetButtons(list[0]);
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
                    string redirectURL = string.Format("~\\webpages\\renewal\\RenewalApprove.aspx?id={0}&status={1}", selectedCardRenewalId, StatusType);
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
                btnConfirm.Visible = true;
                ActionIsConfirm = true;
                btnReject.Visible = false;
                btnApprove.Visible = false;
            }
            else
            {
                lblErrorMessage.Text = "Select at least one renewal batch load to approve.";
            }
        }

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            btnConfirm.Visible = false;
            ActionIsConfirm = null;
            btnReject.Visible = true;
            btnApprove.Visible = true;
        }

        private RenewalStatusType StatusType
        {
            get
            {
                if (ViewState["StatusType"] != null)
                    return (RenewalStatusType)ViewState["StatusType"];
                else
                    return RenewalStatusType.Loaded;
            }
            set
            {
                ViewState["StatusType"] = value;
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
                btnConfirm.Visible = true;
                ActionIsConfirm = false;
                btnReject.Visible = false;
                btnApprove.Visible = false;
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
                        FilterRecords();
                        if (actionResult)
                        {
                            lblInfoMessage.Text = messages;
                        }
                        else
                        {
                            lblErrorMessage.Text = messages;
                        }
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