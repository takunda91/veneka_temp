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
using System.Data;
using System.Reflection;

namespace indigoCardIssuingWeb.webpages.product
{
    public partial class ProductFeeCharges : BasePage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";
        private bool tableCopied = false;

        private bool LoadDataEmpty = false;
        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly BatchManagementService _batchservice = new BatchManagementService();
        List<FeeChargeResult> result = new List<FeeChargeResult>();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ISSUER_ADMIN };
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductFeeCharges));

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
                    pnlButtons.Visible = true;


                    LoadSchemes();
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

        #region Page Events
        protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                LoadSchemes();
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

        protected void ddlProductScheme_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                LoadSchemeDetails();
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

        protected void ddlFeeType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                LoadCharges();
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

        protected void ddlIssuingScenario_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                LoadCharges();
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
                //UpdateFormLayout(PageLayout.CONFIRM_DELETE);
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

                this.btnEdit.Enabled = this.btnEdit.Visible = false;
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

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            if (ViewState[PageLayoutKey] != null)
            {
                pageLayout = (PageLayout)ViewState[PageLayoutKey];
            }

            switch (pageLayout)
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

         public DataTable originalDataTable
        {
            get
            {
                if (ViewState["originalDataTable"] == null)
                    return null;
                else
                    return (DataTable)(ViewState["originalDataTable"]);
            }
            set
            {
                ViewState["originalDataTable"] = value;
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (ViewState[PageLayoutKey] != null)
                {
                    pageLayout = (PageLayout)ViewState[PageLayoutKey];
                }

                string messages;

                switch (pageLayout)
                {
                    case PageLayout.CONFIRM_CREATE:
                        //CreateProduct();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        //DeleteProduct();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        if (_batchservice.UpdateFeeCharges(int.Parse(this.ddlFeeType.SelectedValue),
                                                            PopulateFeeCharge(), out messages))
                        {
                            UpdateFormLayout(PageLayout.READ);
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

        #endregion

        #region Properties
        public long? Productid
        {
            get
            {
                if (ViewState["Productid"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["Productid"].ToString());
            }
            set
            {
                ViewState["Productid"] = value;
            }
        }
        #endregion

        #region Private Methods

        protected void grdCharges_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToUpper() == "CREATE")
            {
                GridViewRow r = grdCharges.FooterRow as GridViewRow;


                int IssuingScenarioId, currencyId;
                int.TryParse(((DropDownList)r.FindControl("ddlIssuingScenario")).SelectedValue, out IssuingScenarioId);

                int.TryParse(((DropDownList)r.FindControl("ddlCurrencycode")).SelectedValue, out currencyId);


                if (IssuingScenarioId > -1 && currencyId > -1)
                {


                    Random random = new Random();
                    int rowid = random.Next(10000000, 99999999);

                    DataRow dr = originalDataTable.NewRow();
                    dr["Id"] = rowid;
                    dr["card_issue_reason_id"] = IssuingScenarioId;
                    dr["currency_id"] = currencyId;
                    dr["fee_charge"] = string.IsNullOrEmpty(((TextBox)r.FindControl("tbFeecharge")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("tbFeecharge")).Text);
                    dr["vat"] = string.IsNullOrEmpty(((TextBox)r.FindControl("tbVatRate")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("tbVatRate")).Text); ;
                    dr["cbs_account_type"] = ((TextBox)r.FindControl("tbAccountType")).Text;

                    originalDataTable.Rows.Add(dr);
                    foreach (DataRow dtrow in originalDataTable.Rows)
                    {
                        if (dtrow["Id"] == null || dtrow["Id"].ToString() == string.Empty)
                        {
                            dtrow.Delete();
                            break;
                        }
                    }
                    originalDataTable.AcceptChanges();

                    grdCharges.DataSource = originalDataTable;
                    grdCharges.DataBind();
                }
                else
                {
                    lblErrorMessage.Text = "Please Select IssuingScenario";
                }
            }

        }
        private void LoadSchemes()
        {
            this.ddlProductScheme.Items.Clear();
            this.pnlButtons.Visible = false;

            if (!String.IsNullOrWhiteSpace(this.ddlIssuer.SelectedValue))
            {
                var results = _batchservice.GetFeeSchemes(int.Parse(this.ddlIssuer.SelectedValue), 1, 1000);

                List<ListItem> items = new List<ListItem>();

                foreach (var result in results)
                {
                    items.Add(new ListItem(result.fee_scheme_name, result.fee_scheme_id.ToString()));
                }

                this.ddlProductScheme.Items.AddRange(items.OrderBy(o => o.Text).ToArray());
                this.pnlButtons.Visible = true;
            }
            else
            {
                this.lblErrorMessage.Text += "<br />No Issuers Avaialble";

            }

            LoadSchemeDetails();
        }

        private void LoadSchemeDetails()
        {
            this.ddlFeeType.Items.Clear();
            this.pnlButtons.Visible = false;

            if (!String.IsNullOrWhiteSpace(this.ddlProductScheme.SelectedValue))
            {
                var results = _batchservice.GetFeeDetails(int.Parse(this.ddlProductScheme.SelectedValue), 1, 1000);

                List<ListItem> items = new List<ListItem>();

                foreach (var result in results)
                {
                    items.Add(new ListItem(result.fee_detail_name, result.fee_detail_id.ToString()));
                }

                this.ddlFeeType.Items.AddRange(items.OrderBy(o => o.Text).ToArray());
                this.pnlButtons.Visible = true;
            }
            else
            {
                this.lblErrorMessage.Text += "<br />No Fee Schemes Available";
            }

            LoadCharges();
        }


        public static DataTable ToDataTable<ProductPrintFieldResult>(List<ProductPrintFieldResult> items)
        {
            DataTable dataTable = new DataTable(typeof(ProductPrintFieldResult).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(ProductPrintFieldResult).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (ProductPrintFieldResult item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        private void LoadCharges()
        {
            int feeDetailId;
            int.TryParse(ddlFeeType.SelectedValue, out feeDetailId);
            var results = _batchservice.GetFeeCharges(feeDetailId);


            DataTable chargeResults = ToDataTable(results);

            chargeResults.Columns.Add("Id", typeof(int));
            int i = 0;
            foreach (DataRow row in chargeResults.Rows)
            {
                row["Id"] = i++;

            }
            if (chargeResults.Rows.Count == 0)
            {
                chargeResults.Rows.Add(chargeResults.NewRow());
                LoadDataEmpty = true;
            }
            originalDataTable = chargeResults;

            grdCharges.DataSource = (chargeResults);


            grdCharges.DataBind();

           
        }

        private DataTable InsertRowAtEnd(DataTable dt)
        {

            Random random = new Random();

            DataRow dr = dt.NewRow();
            dr["currency_id"] = "0";
            dr["card_issue_reason_id"] = "0";
            dr["fee_charge"] = "0.0";
            dr["vat"] = "0.0";
            dr["cbs_account_type"] = "";
            dt.Rows.Add(dr);
            return dt;
        }
        public static DataTable ConvertToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


        #endregion

        #region Page Flow Methods
        private void UpdateFormLayout(PageLayout? toPageLayout)
        {
            if (toPageLayout == null)
            {
                if (ViewState[PageLayoutKey] != null)
                {
                    pageLayout = (PageLayout)ViewState[PageLayoutKey];
                }
                toPageLayout = pageLayout;
            }

            switch (toPageLayout)
            {
                case PageLayout.CREATE:
                    EnableFields(true);
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.READ:
                    DisableFields(false, false);
                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    this.btnBack.Enabled = this.btnBack.Visible = false;
                    break;
                case PageLayout.UPDATE:
                    EnableFields(false);
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    this.btnBack.Enabled = this.btnBack.Visible = true;
                    break;
                case PageLayout.DELETE:

                    // this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    //this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    //this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    //this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                    //this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.CONFIRM_CREATE:
                    DisableFields(true, true);
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    //this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                    //this.btnUpdate.Text = "Confirm";
                    break;
                case PageLayout.CONFIRM_DELETE:
                    DisableFields(true, false);

                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    //this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteMessage").ToString();
                    //this.btnUpdate.Text = "Confirm";
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    DisableFields(true, false);
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.btnBack.Enabled = this.btnBack.Visible = true;
                    this.lblInfoMessage.Text = Resources.CommonInfoMessages.ReviewCommitMessage;

                    //this.btnUpdate.Text = "Confirm";
                    break;
                default:
                    DisableFields(false, false);
                    //this.btnUpdate.Text = UtilityClass.UppercaseFirst("EDIT");
                    break;
            }

            ViewState[PageLayoutKey] = toPageLayout;
        }

        private bool Validation()
        {
            return true;
        }

        private void DisableFields(bool IsConfirm, bool isCreate)
        {
            this.ddlIssuer.Enabled =
            this.ddlFeeType.Enabled =
            this.ddlProductScheme.Enabled = true;
            this.grdCharges.Enabled = false;


            this.btnUpdate.Enabled = this.btnUpdate.Visible = isCreate ? false : true;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = IsConfirm;
        }

        private void EnableFields(bool isCreate)
        {
            this.ddlIssuer.Enabled =
            this.ddlFeeType.Enabled =
            //this.ddlIssuingScenario.Enabled =
            this.ddlProductScheme.Enabled = false;
            this.grdCharges.Enabled = true;

            this.btnEdit.Enabled = this.btnEdit.Visible = isCreate ? false : true;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;

            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
        }
        #endregion

        #region GridEvents
        protected void grdCharges_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grdCharges.EditIndex = -1;
            grdCharges.DataSource = originalDataTable;
            grdCharges.DataBind();
        }
        protected void grdCharges_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grdCharges.EditIndex = e.NewEditIndex;
            //Now bind the gridview
            grdCharges.DataSource = originalDataTable;
            grdCharges.DataBind();
        }
        protected void grdCharges_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            AddUpdateGrid(sender, e);
        }

        protected void AddUpdateGrid(object sender, GridViewUpdateEventArgs e)
        {

            try
            {
                var Id = grdCharges.DataKeys[e.RowIndex].Value;

                GridViewRow r = grdCharges.Rows[e.RowIndex] as GridViewRow;
                int IssuingScenarioId, currencyId;
                int.TryParse(((DropDownList)r.FindControl("ddlIssuingScenario")).SelectedValue, out IssuingScenarioId);

                int.TryParse(((DropDownList)r.FindControl("ddlCurrencycode")).SelectedValue, out currencyId);
                if (IssuingScenarioId > -1 && currencyId > -1)
                {
                    DataRow dr = originalDataTable.Rows[e.RowIndex];

                    dr.BeginEdit();
                    dr["card_issue_reason_id"] = IssuingScenarioId;
                    dr["currency_id"] = currencyId;
                    dr["fee_charge"] = string.IsNullOrEmpty(((TextBox)r.FindControl("tbFeecharge")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("tbFeecharge")).Text);
                    dr["vat"] = string.IsNullOrEmpty(((TextBox)r.FindControl("tbVatRate")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("tbVatRate")).Text); ;
                    dr["cbs_account_type"] = ((TextBox)r.FindControl("tbAccountType")).Text;
                    dr.EndEdit();
                    dr.AcceptChanges();
                    grdCharges.EditIndex = -1;
                    //Now bind the datatable to the gridview
                    grdCharges.DataSource = originalDataTable;
                    grdCharges.DataBind();
                }
                else
                {
                    lblErrorMessage.Text = "Currency and  IssuingScenario Required.";
                }


            }
            catch (Exception ex)
            {
                log.Error(ex);
                lblErrorMessage.Text = ex.Message;
            }
        }
        protected void DeleteRow(object sender, GridViewDeleteEventArgs e)
        {
            originalDataTable.Rows[e.RowIndex].Delete();
            originalDataTable.AcceptChanges();

            if (grdCharges.Rows.Count > 0)
            {

                grdCharges.DataSource = originalDataTable;
                grdCharges.DataBind();
            }


        }
        /* Update Print Fields Section */
        private void UpdateGrid()
        {
            if (ddlFeeType.SelectedIndex > -1)
            {

             

                string messages;
                if (result.Count > 0)
                    if (_batchservice.UpdateFeeCharges(int.Parse(ddlFeeType.SelectedValue), result, out messages))
                    {
                        lblInfoMessage.Text = messages;

                    }
                result.Clear();
                // Rebind the Grid to repopulate the original values table.
                tableCopied = false;
                LoadCharges();
            }
            else
            {
                log.Error("fee Type is null.");
            }

        }
        private List<FeeChargeResult> PopulateFeeCharge()
        {
            List<FeeChargeResult> feechargelist = new List<FeeChargeResult>();
            foreach (DataRow row in originalDataTable.Rows)
            {

                FeeChargeResult fee = new FeeChargeResult();

                fee.card_issue_reason_id = int.Parse(row["card_issue_reason_id"].ToString());
                fee.currency_id = int.Parse(row["currency_id"].ToString());
                fee.fee_charge = decimal.Parse(row["fee_charge"].ToString());
                fee.vat = decimal.Parse(row["vat"].ToString());
                fee.cbs_account_type = row["cbs_account_type"].ToString();


                feechargelist.Add(fee);
            }


            return feechargelist;
        }
        protected void grdCharges_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Footer)
            {
                if (LoadDataEmpty&& e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Visible = false;
                }


                //if ((e.Row.RowState & DataControlRowState.Edit) > 0)

                DropDownList ddlCurrencycode = (DropDownList)e.Row.FindControl("ddlCurrencycode");
                List<ListItem> currencies = new List<ListItem>();
                foreach (var currencyItem in _issuerMan.GetCurrencyList().OrderBy(o => o.currency_code))
                {
                    currencies.Add(new ListItem(currencyItem.currency_code, currencyItem.currency_id.ToString()));
                }
                ddlCurrencycode.Items.AddRange(currencies.OrderBy(m => m.Text).ToArray());
                ddlCurrencycode.SelectedIndex = 0;


                DropDownList ddlIssuingScenario = (DropDownList)e.Row.FindControl("ddlIssuingScenario");

                foreach (var reasonForIssue in _customerCardIssuerService.LangLookupCardIssueReasons())
                {
                    ddlIssuingScenario.Items.Add(new ListItem(reasonForIssue.language_text, reasonForIssue.lookup_id.ToString()));
                }

                ddlIssuingScenario.SelectedIndex = 0;
                //TO DO
                if (e.Row.RowType == DataControlRowType.DataRow )
                {

                    DataRow dr =  originalDataTable.Rows[e.Row.RowIndex];
                    if (dr != null)
                    {
                        //ddList.SelectedItem.Text = dr["category_name"].ToString();
                        ddlCurrencycode.SelectedValue = dr["currency_id"].ToString();
                        ddlIssuingScenario.SelectedValue = dr["card_issue_reason_id"].ToString();
                    }

                }

            }
        }
        #endregion
    }
}