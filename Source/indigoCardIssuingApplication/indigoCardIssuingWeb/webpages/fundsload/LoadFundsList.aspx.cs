using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Linq;
using System.Web.Security;
using System.Configuration;

namespace indigoCardIssuingWeb.webpages.fundsload
{
    public partial class LoadFundsList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoadFundsList));

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.CMS_OPERATOR,
                                                                        UserRole.CARD_PRODUCTION,
                                                                        UserRole.PIN_PRINTER_OPERATOR,
                                                                        UserRole.AUDITOR};

        private readonly BatchManagementService _batchService = new BatchManagementService();
        private SystemAdminService sysAdminService = new SystemAdminService();
        private bool showcheckbox = false;
        private readonly FundsLoadService _fundsLoadService = new FundsLoadService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private void LoadPageData()
        {
            if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
            {
                StatusType = (FundsLoadStatusType)int.Parse(Request.QueryString["status"]);
            }
            DisplayBatchListForUser();
        }

        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = "";
            this.dlFundsLoadList.DataSource = null;

            var searchParm = (FundsLoadSearchParameters)parms;

            searchParm.Status = StatusType;

            lblFundsLoadList.Text = Resources.CommonLabels.FundsLoadList;

            if (results == null)
            {
                results = _fundsLoadService.ListFundsLoads(searchParm.Status, searchParm.IssuerId.GetValueOrDefault(), searchParm.BranchId.GetValueOrDefault()).ToArray();
            }

            if (results.Length > 0)
            {
                this.dlFundsLoadList.DataSource = results;
                TotalPages = results.Length / 10;
                List<FundsLoadListModel> list = results.Cast<FundsLoadListModel>().ToList();
                EnableButton(list);
            }
            else
            {
                TotalPages = 1;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                btnApprove.Visible = false;
                btnReject.Visible = false;
                btnConfirm.Visible = false;
                pnlremarks.Visible = false;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlFundsLoadList.DataBind();
        }

        private void SetButtons(FundsLoadListModel distBatch)
        {
            this.btnApprove.Text = GetLocalResourceObject("FundsLoadApproveButton").ToString();
            if (StatusType== FundsLoadStatusType.Approved)
            {
                this.btnApprove.Text = GetLocalResourceObject("FundsLoadLoadButton").ToString();
            }
            this.btnApprove.Visible = true;
            pnlremarks.Visible = true;
            showcheckbox = true;
        }

        private void DisplayBatchListForUser()
        {
            dlFundsLoadList.DataSource = null;

            try
            {
                FundsLoadStatusType searchStatus = FundsLoadStatusType.Created;

                if (!String.IsNullOrWhiteSpace(Request.QueryString["status"]))
                {
                    searchStatus = (FundsLoadStatusType)int.Parse(Request.QueryString["status"]);
                }

                FundsLoadSearchParameters searchParams = new FundsLoadSearchParameters(null, searchStatus, 1);
                SearchParameters = searchParams;

                DisplayResults(searchParams, searchParams.PageIndex, null);

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

        private void EnableButton(List<FundsLoadListModel> list)
        {

            btnApprove.Visible = false;
            pnlremarks.Visible = false;

            if (list.Count() > 0)
            {
                SetButtons(list[0]);
            }
        }

        protected void dlFundsLoadList_ItemCommand(object source, DataListCommandEventArgs e)
        {
            try
            {
                dlFundsLoadList.SelectedIndex = e.Item.ItemIndex;
                int selectedIndex = dlFundsLoadList.SelectedIndex;

                string fundsLoadIdStr = ((Label)this.dlFundsLoadList.SelectedItem.FindControl("lblFundsLoadId")).Text;

                long selectedFundsLoadId;
                if (long.TryParse(fundsLoadIdStr, out selectedFundsLoadId))
                {
                    string redirectURL = string.Format("~\\webpages\\fundsload\\LoadFundsApprove.aspx?id={0}&status={1}", selectedFundsLoadId, StatusType);
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

        protected void dlFundsLoadList_ItemDataBound(object sender, DataListItemEventArgs e)
        {

        }

        protected void dlFundsLoadList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            List<long> selectedItems = new List<long>();
            lblErrorMessage.Text = string.Empty;
            foreach (DataListItem item in dlFundsLoadList.Items)
            {
                CheckBox chk = item.FindControl("chksel") as CheckBox;

                string distBatchIdStr = ((Label)item.FindControl("lblFundsLoadId")).Text;

                if (chk.Checked)
                {
                    long fundsLoadId;
                    long.TryParse(distBatchIdStr, out fundsLoadId);
                    selectedItems.Add(fundsLoadId);
                }

            }
            if (selectedItems.Count > 0)
            {
                btnConfirm.Visible = true;
                ActionIsConfirm = true;
                btnReject.Visible = false;
                btnApprove.Visible = false;
            }
            else
            {
                lblErrorMessage.Text = "Select at least one funds load to approve.";
            }
        }

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            btnConfirm.Visible = false;
            ActionIsConfirm = null;
            //btnReject.Visible = true;
            //btnApprove.Visible = true;
        }

        private FundsLoadStatusType StatusType
        {
            get
            {
                if (ViewState["StatusType"] != null)
                    return (FundsLoadStatusType)ViewState["StatusType"];
                else
                    return FundsLoadStatusType.Created;
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
            foreach (DataListItem item in dlFundsLoadList.Items)
            {
                CheckBox chk = item.FindControl("chksel") as CheckBox;

                string distBatchIdStr = ((Label)item.FindControl("lblFundsLoadId")).Text;

                if (chk.Checked)
                {
                    long fundsLoadId;
                    long.TryParse(distBatchIdStr, out fundsLoadId);
                    selectedItems.Add(fundsLoadId);
                }

            }
            if (selectedItems.Count > 0)
            {
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
                    string notes = tbStatusNote.Text;
                    foreach (DataListItem item in dlFundsLoadList.Items)
                    {
                        CheckBox chk = item.FindControl("chksel") as CheckBox;

                        string distBatchIdStr = ((Label)item.FindControl("lblFundsLoadId")).Text;

                        if (chk.Checked)
                        {
                            long fundsLoadId;
                            long.TryParse(distBatchIdStr, out fundsLoadId);
                            selectedItems.Add(fundsLoadId);
                        }

                    }
                    if (selectedItems.Count > 0)
                    {
                        string messages = string.Empty;
                        bool actionResult = false;
                        if (isConfirm)
                        {
                            actionResult = _fundsLoadService.ApproveBulk(selectedItems, StatusType, out messages);
                        }
                        else
                        {
                            actionResult = _fundsLoadService.RejectBulk(selectedItems, StatusType, out messages);
                        }
                        if (actionResult)
                        {
                            DisplayResults(SearchParameters, SearchParameters.PageIndex, null);
                            lblInfoMessage.Text = messages;
                            tbStatusNote.Text = string.Empty;
                        }
                        else
                        {
                            DisplayResults(SearchParameters, SearchParameters.PageIndex, null);
                            lblErrorMessage.Text = messages;
                            tbStatusNote.Text = string.Empty;
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