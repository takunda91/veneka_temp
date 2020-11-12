using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using System.Threading;
using System.Globalization;
using System.Web.Security;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.product
{
    public partial class ProductFeeDetails : BasePage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";

        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly BatchManagementService _batchservice = new BatchManagementService();


        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ISSUER_ADMIN };
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductFeeDetails));

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (CurrentPageLayout != null)
            {
                pageLayout = (PageLayout)CurrentPageLayout;
            }

            if (!IsPostBack)
            {
                LoadPageData();
            }
            else
            {
                string parameter = Request["__EVENTARGUMENT"];
                if (!String.IsNullOrWhiteSpace(parameter) && parameter == "btnAddDetailDone")
                    btnAddDetailDone_OnClick(sender, e);
            }
        }

        #region Private Methods
        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                List<Issuer_product_font> list = _batchservice.GetFontFamilyList();

                if (issuersList.ContainsKey(-1))
                    issuersList.Remove(-1);

                ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                ddlIssuer.SelectedIndex = 0;

                if (ddlIssuer.Items.Count == 0)
                {
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                }
                if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    pnlButtons.Visible = false;
                    lblInfoMessage.Text = "";
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SaveActionMessage").ToString();
                }
                else
                {

                    //List<ListItem> accountingList = new List<ListItem>();
                    //foreach (var item in _batchservice.GetFeeAccountingList(int.Parse(ddlIssuer.SelectedValue), 1, 100))
                    //{
                    //    accountingList.Add(new ListItem(item.fee_accounting_name, item.fee_accounting_id.ToString()));
                    //}

                    //if (accountingList.Count == 0)
                    //{
                    //    this.pnlButtons.Visible = false;
                    //    this.lblErrorMessage.Text = "Fee accounting list is empty. Please create fee accounting first.";
                    //    return;
                    //}

                    //this.ddlFeeAccounting.Items.AddRange(accountingList.OrderBy(m => m.Text).ToArray());

                    pnlButtons.Visible = true;

                    FeeSchemeId = SessionWrapper.ProductFeeSchemeId;
                    SessionWrapper.ProductFeeSchemeId = null;

                    if (FeeSchemeId == null)
                    {
                        pageLayout = PageLayout.CREATE;
                        try
                        {
                            FeeAccountingList(int.Parse(ddlIssuer.SelectedValue));
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            this.pnlButtons.Visible = false;
                            this.lblErrorMessage.Text = ex.Message.ToString();
                            return;
                        }
                    }
                    else
                    {
                        var result = _batchservice.GetFeeSchemeDetails(FeeSchemeId.Value);

                        this.ddlIssuer.SelectedValue = result.issuer_id.ToString();

                        try
                        {
                            FeeAccountingList(result.issuer_id);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            this.pnlButtons.Visible = false;
                            this.lblErrorMessage.Text = ex.Message.ToString();
                            return;
                        }

                        this.tbSchemeName.Text = result.fee_scheme_name;
                        this.ddlFeeAccounting.SelectedValue = result.fee_accounting_id.ToString();

                        this.dlFeeDetails.DataSource = result.FeeDetails;
                        this.dlFeeDetails.DataBind();

                        FeeDetailResults = result.FeeDetails.ToDictionary(v => v.fee_detail_id);
                    }
                }

                UpdateFormLayout(pageLayout);
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private FeeSchemeDetails GetValuesFromControls()
        {
            FeeSchemeDetails feeSchemeDetails = new FeeSchemeDetails();

            if (FeeSchemeId != null)
                feeSchemeDetails.fee_scheme_id = FeeSchemeId.Value;

            feeSchemeDetails.issuer_id = int.Parse(this.ddlIssuer.SelectedValue);
            feeSchemeDetails.fee_scheme_name = this.tbSchemeName.Text;
            feeSchemeDetails.fee_accounting_id = int.Parse(this.ddlFeeAccounting.SelectedValue);

            feeSchemeDetails.FeeDetails = GetDetailsFromList().Values.ToArray();

            return feeSchemeDetails;
        }
        #endregion

        #region Page Events
        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnAddDetailDone_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                int feeDetailsId = -1;
                Dictionary<int, FeeDetailResult> details = new Dictionary<int, FeeDetailResult>();


                FeeDetailResults = GetDetailsFromList();

                if (FeeDetailResults != null && FeeDetailResults.Count > 0)
                {
                    details = FeeDetailResults;

                    if (details.Keys.Min() < 0)
                    {
                        feeDetailsId = details.Keys.Where(w => w < 0).Min();
                        feeDetailsId--;
                    }
                }

                FeeDetailResult feeDetails = new FeeDetailResult
                {
                    fee_detail_id = feeDetailsId,
                    fee_detail_name = this.tbFeeDetailName.Text,
                    fee_waiver_YN = this.chkbFeeWaiver.Checked,
                    fee_editable_YN = this.chkbFeeEditable.Checked
                };

                if (FeeSchemeId != null)
                    feeDetails.fee_scheme_id = FeeSchemeId.Value;

                details.Add(feeDetails.fee_detail_id, feeDetails);

                FeeDetailResults = details;

                this.tbFeeDetailName.Text = String.Empty;
                this.chkbFeeWaiver.Checked = false;
                this.chkbFeeEditable.Checked = false;

                this.dlFeeDetails.DataSource = details.Values;
                this.dlFeeDetails.DataBind();
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                UpdateFormLayout(PageLayout.CONFIRM_DELETE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                //if (Validation())
                UpdateFormLayout(PageLayout.CONFIRM_CREATE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {

                //this.btnEdit.Enabled = this.btnEdit.Visible = false;
                UpdateFormLayout(PageLayout.UPDATE);

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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                //if (Validation())
                UpdateFormLayout(PageLayout.CONFIRM_UPDATE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (CurrentPageLayout != null)
                {
                    pageLayout = CurrentPageLayout.Value;
                }
                string messages;
                FeeSchemeDetails details;

                switch (pageLayout)
                {
                    case PageLayout.CONFIRM_CREATE:
                        details = GetValuesFromControls();
                        FeeSchemeDetails feeschemedetails;
                        if (_batchservice.InsertFeeScheme(details, out messages, out feeschemedetails))
                        {
                            FeeDetailResults = feeschemedetails.FeeDetails.ToDictionary(v => v.fee_detail_id);
                            FeeSchemeId = feeschemedetails.fee_scheme_id;
                            pageLayout = PageLayout.READ;
                            UpdateFormLayout(pageLayout);
                            this.lblInfoMessage.Text = messages;
                            this.dlFeeDetails.DataSource = details.FeeDetails;
                            this.dlFeeDetails.DataBind();
                        }
                        else
                        {
                            this.lblErrorMessage.Text = messages;
                        }
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        if(_batchservice.DeleteFeeScheme(FeeSchemeId.Value, out messages))
                        {
                            //redirect to list page, display message.
                            this.lblInfoMessage.Text = messages;
                            //UpdateFormLayout(pageLayout);
                            Server.Transfer("~\\webpages\\product\\ProductFeeList.aspx?delete=1");
                        }
                        else
                        {
                            this.lblErrorMessage.Text = messages;
                        }                        
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        details = GetValuesFromControls();
                        if (_batchservice.UpdateFeeScheme(details, out messages))
                        {
                            FeeDetailResults = details.FeeDetails.ToDictionary(v => v.fee_detail_id);
                            pageLayout = PageLayout.READ;
                            UpdateFormLayout(pageLayout);
                            this.lblInfoMessage.Text = messages;

                        }
                        else
                        {
                            this.lblErrorMessage.Text = messages;
                        }
                        break;
                    default:
                        break;
                }
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void dlFeeDetails_ItemCommand(object source, DataListCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "remove")
                {
                    int detailsId = int.Parse(((Label)(e.Item.FindControl("lblDlFeeDetailsId"))).Text);

                    var values = GetDetailsFromList();

                    values.Remove(detailsId);

                    FeeDetailResults = values;

                    this.dlFeeDetails.DataSource = values.Values;
                    this.dlFeeDetails.DataBind();
                }
                else if (e.CommandName == "edit")
                {
                    ViewState["FeeDetails"] = GetDetailsFromList().Values.ToArray();
                    dlFeeDetails.EditItemIndex = e.Item.ItemIndex;

                    this.dlFeeDetails.DataSource = (FeeDetailResult[])ViewState["FeeDetails"];
                    this.dlFeeDetails.DataBind();

                }
                else if (e.CommandName == "update")
                {

                    string feedetailsid = ((Label)(e.Item.FindControl("lblFeeDetailsId"))).Text;
                    string bandname = ((TextBox)(e.Item.FindControl("tbFeeDetailsName"))).Text;
                    bool feeeditable = ((CheckBox)(e.Item.FindControl("chkFeeEditable"))).Checked;
                    bool feewaiver = ((CheckBox)(e.Item.FindControl("chkFeeWaiver"))).Checked;

                    if (ViewState["FeeDetails"] != null)
                    {
                        dlFeeDetails.EditItemIndex = -1;
                        this.dlFeeDetails.DataSource = (FeeDetailResult[])ViewState["FeeDetails"];
                        this.dlFeeDetails.DataBind();
                        foreach (DataListItem item in dlFeeDetails.Items)
                        {
                            if (((Label)item.FindControl("lblDlFeeDetailsId")).Text == feedetailsid)
                            {
                                ((Label)(item.FindControl("lblDlFeeDetailsName"))).Text = bandname;
                                ((CheckBox)(item.FindControl("chkbFeeEditable"))).Checked = feeeditable;
                                ((CheckBox)(item.FindControl("chkbFeeWaiver"))).Checked = feewaiver;
                                break;
                            }
                            
                        }
                    }


                }
                else if (e.CommandName == "cancel")
                {
                    dlFeeDetails.EditItemIndex = -1;
                    if (ViewState["FeeDetails"] != null)
                    {
                        this.dlFeeDetails.DataSource = (FeeDetailResult[])ViewState["FeeDetails"];
                        this.dlFeeDetails.DataBind();
                    }
                    ViewState["FeeDetails"] = null;
                }
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
        #endregion

        public int? FeeSchemeId
        {
            get
            {
                if (ViewState["FeeSchemeId"] == null)
                    return null;
                else
                    return Convert.ToInt32(ViewState["FeeSchemeId"].ToString());
            }
            set
            {
                ViewState["FeeSchemeId"] = value;
            }
        }
        private PageLayout? CurrentPageLayout
        {
            get
            {
                if (ViewState["CurrentPageLayout"] == null)
                    return null;
                else
                    return (PageLayout)ViewState["CurrentPageLayout"];

            }
            set
            {
                ViewState["CurrentPageLayout"] = value;
            }
        }
        public Dictionary<int, FeeDetailResult> FeeDetailResults
        {
            get
            {
                if (ViewState["FeeDetailResults"] == null)
                    return null;
                else
                    return (Dictionary<int, FeeDetailResult>)ViewState["FeeDetailResults"];
            }
            set
            {
                ViewState["FeeDetailResults"] = value;
            }
        }

        #region Page Flow Methods
        private void UpdateFormLayout(PageLayout? toPageLayout)
        {
            if (toPageLayout == null)
            {
                if (CurrentPageLayout != null)
                {
                    pageLayout = CurrentPageLayout.Value;
                }
                toPageLayout = pageLayout;
            }

            switch (toPageLayout)
            {
                case PageLayout.CREATE:
                    EnableFields(true);
                    this.btnCreate.Enabled = this.btnCreate.Visible = true;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.READ:
                    DisableFields(false, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = true;

                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.UPDATE:
                    EnableFields(false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.DELETE:
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    this.btnBack.Visible = false;
                    break;
                case PageLayout.CONFIRM_CREATE:
                    DisableFields(true, true);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.lblInfoMessage.Text = Resources.CommonInfoMessages.ReviewCommitMessage;
                    break;
                case PageLayout.CONFIRM_DELETE:
                    DisableFields(true, false);

                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = Resources.CommonInfoMessages.ReviewCommitMessage;
                    //this.btnUpdate.Text = "Confirm";
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    DisableFields(true, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = Resources.CommonInfoMessages.ReviewCommitMessage;
                    //this.btnUpdate.Text = "Confirm";
                    break;
                default:
                    DisableFields(false, false);
                    //this.btnUpdate.Text = UtilityClass.UppercaseFirst("EDIT");
                    break;
            }

            CurrentPageLayout = toPageLayout;
        }

        private bool Validation()
        {
            return true;
        }

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";


            switch (CurrentPageLayout)
            {
                case PageLayout.RESET:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.UPDATE:
                    //LoadRestoreGroup();
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.DELETE:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.CONFIRM_CREATE:
                    UpdateFormLayout(PageLayout.CREATE);

                    break;
                case PageLayout.CONFIRM_DELETE:
                    UpdateFormLayout(PageLayout.DELETE);
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    UpdateFormLayout(PageLayout.UPDATE);

                    break;
                default:
                    break;
            }
        }

        private void EnableFields(bool isCreate)
        {
            this.ddlIssuer.Enabled = isCreate;

            this.tbSchemeName.Enabled = true;
            this.ddlFeeAccounting.Enabled = true;
            this.dlFeeDetails.Enabled = true;

            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnEdit.Enabled = this.btnEdit.Visible = isCreate ? false : true;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;

            this.btnAddFeeDetail.Visible = true;

            this.btnDelete.Enabled = this.btnDelete.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
            this.btnBack.Visible = isCreate ? false : true;


        }

        private void DisableFields(bool IsConfirm, bool isCreate)
        {
            this.ddlIssuer.Enabled = false;

            this.tbSchemeName.Enabled = false;
            this.ddlFeeAccounting.Enabled = false;
            this.dlFeeDetails.Enabled = false;

            this.btnBack.Visible = IsConfirm ? true : false;

            this.btnAddFeeDetail.Visible = false;

            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnDelete.Enabled = this.btnDelete.Visible = isCreate;

            this.btnUpdate.Enabled = this.btnUpdate.Visible = isCreate ? false : true;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = IsConfirm;

            ddlIssuer.Enabled = isCreate ? true : false;
        }

        private Dictionary<int, FeeDetailResult> GetDetailsFromList()
        {
            Dictionary<int, FeeDetailResult> response = new Dictionary<int, FeeDetailResult>();


            foreach (DataListItem item in this.dlFeeDetails.Items)
            {
                if (((Label)(item.FindControl("lblDlFeeDetailsId"))) != null)
                {
                    response.Add(int.Parse(((Label)(item.FindControl("lblDlFeeDetailsId"))).Text),
                        new FeeDetailResult
                        {
                            fee_detail_id = int.Parse(((Label)(item.FindControl("lblDlFeeDetailsId"))).Text),
                            fee_detail_name = ((Label)(item.FindControl("lblDlFeeDetailsName"))).Text,
                            fee_editable_YN = ((CheckBox)(item.FindControl("chkbFeeEditable"))).Checked,
                            fee_waiver_YN = ((CheckBox)(item.FindControl("chkbFeeWaiver"))).Checked
                        });
                }
            }

            return response;
        }
        #endregion

        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = String.Empty;
                this.ddlFeeAccounting.Items.Clear();
                

                this.pnlButtons.Visible = true;

                if (FeeSchemeId == null)
                {
                    pageLayout = PageLayout.CREATE;
                    FeeAccountingList(int.Parse(this.ddlIssuer.SelectedValue));
                }
                else
                {
                    var result = _batchservice.GetFeeSchemeDetails(FeeSchemeId.Value);

                    this.ddlIssuer.SelectedValue = result.issuer_id.ToString();

                    FeeAccountingList(result.issuer_id);

                    this.tbSchemeName.Text = result.fee_scheme_name;
                    this.ddlFeeAccounting.SelectedValue = result.fee_accounting_id.ToString();

                    this.dlFeeDetails.DataSource = result.FeeDetails;
                    this.dlFeeDetails.DataBind();

                    FeeDetailResults = result.FeeDetails.ToDictionary(v => v.fee_detail_id);
                }



                UpdateFormLayout(pageLayout);
            }
            catch (Exception ex)
            {
                //this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private void FeeAccountingList(int issuerId)
        {
            List<ListItem> accountingList = new List<ListItem>();
            foreach (var item in _batchservice.GetFeeAccountingList(issuerId, 1, 100))
            {
                accountingList.Add(new ListItem(item.fee_accounting_name, item.fee_accounting_id.ToString()));
            }

            if (accountingList.Count == 0)
                throw new Exception("Fee accounting list is empty. Please create fee accounting first.");

            this.ddlFeeAccounting.Items.AddRange(accountingList.OrderBy(m => m.Text).ToArray());
        }
    }
}