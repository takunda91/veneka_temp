using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class ExternalSystemsViewForm : BasePage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";
        private static readonly ILog log = LogManager.GetLogger(typeof(MasterkeyManager));
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };
        private bool LoadDataEmpty = false;
        #region Page Load


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

                PopulateFields();
                UpdateFormLayout(pageLayout);
            }
        }
        #endregion

        #region Events
        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
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



        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                //this.txtMasterkeyName.Enabled =
                this.txtExternalSystemName.Enabled =
                this.ddlexternalsytemtype.Enabled = true;

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

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (ValidateExternalFields())
                {
                    UpdateFormLayout(PageLayout.CONFIRM_UPDATE);
                }
                else
                {
                    lblErrorMessage.Text = "No External System Fields added to this External System.";
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (ValidateExternalFields())
                {
                    UpdateFormLayout(PageLayout.CONFIRM_CREATE);
                }
                else
                {
                    lblErrorMessage.Text = "No External System Fields added to this External System.";
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
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

                switch (pageLayout)
                {
                    case PageLayout.CONFIRM_CREATE:
                        if (CreateExternalSystem())
                        {
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessCreate").ToString();

                        }
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        DeleteExternalSystem();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        if (UpdateExternalSystem())
                        {
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessUpdate").ToString();
                        }
                        UpdateFormLayout(pageLayout);
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            switch (CurrentPageLayout)
            {
                case PageLayout.READ:
                case PageLayout.CREATE:
                case PageLayout.DELETE:
                    Server.Transfer("~\\webpages\\system\\ExternalSystemsMaintanance.aspx");
                    break;
                case PageLayout.UPDATE:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    UpdateFormLayout(PageLayout.UPDATE);
                    break;
                case PageLayout.CONFIRM_CREATE:
                    UpdateFormLayout(PageLayout.CREATE);
                    break;
                case PageLayout.CONFIRM_DELETE:
                    UpdateFormLayout(PageLayout.DELETE);
                    break;
                //case PageLayout.DELETE:
                //    UpdateFormLayout(PageLayout.READ);
                //    break;
            }
        }
        #endregion

        #region Page Properties
        private PageLayout? CurrentPageLayout
        {
            get
            {
                if (ViewState["CurrentPageLayout"] == null)
                    return PageLayout.CREATE;
                else
                    return (PageLayout)ViewState["CurrentPageLayout"];
            }
            set
            {
                ViewState["CurrentPageLayout"] = value;
            }
        }

        public int? ExternalSystemId
        {
            get
            {
                if (ViewState["ExternalSystemId"] == null)
                    return null;
                else
                    return Convert.ToInt32(ViewState["ExternalSystemId"].ToString());
            }
            set
            {
                ViewState["ExternalSystemId"] = value;
            }
        }
        #endregion

        #region Page Load Methods
        private void LoadPageData()
        {
            try
            {

                List<ListItem> externaltypes = new List<ListItem>();
                foreach (var role in _issuerMan.LangLookupExternalSystems())
                {
                    externaltypes.Add(new ListItem(role.language_text, role.lookup_id.ToString()));
                }
                ddlexternalsytemtype.Items.AddRange(externaltypes.OrderBy(m => m.Text).ToArray());

                if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    pnlButtons.Visible = false;
                    lblInfoMessage.Text = "";
                    //lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SaveActionMessage").ToString();
                }
                else
                {
                    lblErrorMessage.Text = "";
                    pnlButtons.Visible = true;
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

        private void PopulateFields()
        {
            try
            {
                if (SessionWrapper.ExternalSystemId > 0)
                {
                    ExternalSystemId = SessionWrapper.ExternalSystemId;
                    SessionWrapper.ExternalSystemId = 0;
                    if (ExternalSystemId != 0)
                    {
                        var externalsystem = _issuerMan.GetExternalSystem((int)ExternalSystemId, 1, 2000);

                        if (externalsystem != null && externalsystem.ExternalSystems.Length > 0)
                        {
                            txtExternalSystemName.Text = externalsystem.ExternalSystems[0].system_name;
                            ddlexternalsytemtype.SelectedValue = externalsystem.ExternalSystems[0].external_system_type_id.ToString();
                        }
                        BindGridView(externalsystem.ExternalSystemFields.ToList());

                        pageLayout = PageLayout.READ;
                    }
                    else
                    {
                        //No username in session, set page layout to create.
                        pageLayout = PageLayout.CREATE;
                    }
                }
                else
                {
                    //No username in session, set page layout to create.
                    pageLayout = PageLayout.CREATE;
                    BindGridView(null);
                }
                CurrentPageLayout = pageLayout;
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

        #region Page Flow Methods

        private bool DeleteExternalSystem()
        {
            try
            {
                string responsemessage = string.Empty;
                bool response = _issuerMan.DeleteExternalSystem(ExternalSystemId, out responsemessage);
                if (response)
                {

                    Server.Transfer("~\\webpages\\system\\ExternalSystemsMaintanance.aspx?delete=" + txtExternalSystemName.Text);

                    //this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessDelete").ToString();
                    //pageLayout = PageLayout.CREATE;

                    //MasterkeyId = 0;
                    //this.txtMasterkey.Text = "";
                    //this.txtMasterkeyName.Text = "";

                    //return true;

                }
                else
                {
                    this.lblInfoMessage.Text = string.Empty;
                    this.lblErrorMessage.Text = responsemessage;
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

            return false;
        }
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
                    this.btnBack.Enabled = this.btnBack.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.READ:
                    DisableFields(false, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnBack.Enabled = this.btnBack.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.DELETE:
                    DisableFields(false, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                    this.btnBack.Enabled = this.btnBack.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.UPDATE:
                    EnableFields(false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.CONFIRM_CREATE:
                    DisableFields(true, true);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmUpdate").ToString();
                    break;
                case PageLayout.CONFIRM_DELETE:
                    DisableFields(true, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;

                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteMessage").ToString();
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    DisableFields(true, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmUpdate").ToString();
                    break;
                default:
                    DisableFields(false, false);
                    break;
            }

            CurrentPageLayout = toPageLayout;
        }

        private void EnableFields(bool isCreate)
        {
            this.txtExternalSystemName.Enabled =
                this.GrdExternalSystemFields.Enabled =
            this.ddlexternalsytemtype.Enabled = true;

            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnEdit.Enabled = this.btnEdit.Visible = isCreate ? false : true;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
            this.btnBack.Visible = this.btnBack.Enabled = isCreate ? false : true;
        }

        private void DisableFields(bool IsConfirm, bool isCreate)
        {
            this.txtExternalSystemName.Enabled =
                GrdExternalSystemFields.Enabled =
            this.ddlexternalsytemtype.Enabled = false;

            this.btnBack.Visible = this.btnBack.Enabled = IsConfirm ? true : false;
            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = isCreate ? false : true;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = IsConfirm;
        }

        private void SetControls(bool hideAll)
        {
            this.btnEdit.Visible = this.btnEdit.Enabled = false;
            this.btnCreate.Visible = this.btnCreate.Enabled = false;
            this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
            this.btnBack.Visible = this.btnBack.Enabled = true;

            bool enabled = false;
            bool visable = hideAll ? false : true;

            bool hasRead;
            bool hasUpdate;
            bool hasCreate;

            if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.ADMINISTRATOR, out hasRead, out hasUpdate, out hasCreate))
            {
                if (SessionWrapper.TerminalId < 0)
                {
                    this.btnEdit.Visible = this.btnEdit.Enabled = false;
                    this.btnCreate.Visible = this.btnCreate.Enabled = false;
                    this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                    this.btnBack.Visible = this.btnBack.Enabled = true;
                }
                else
                {
                    switch (CurrentPageLayout)
                    {
                        case PageLayout.READ:
                            enabled = false;
                            if (hasUpdate)
                            {
                                this.btnEdit.Visible = this.btnEdit.Enabled = true;
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                //this.btnCancel.Visible = this.btnCancel.Enabled = false;
                                this.btnBack.Visible = this.btnBack.Enabled = true;
                            }
                            break;
                        case PageLayout.CREATE:
                            enabled = true;
                            if (hasCreate)
                            {
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnCreate.Visible = this.btnCreate.Enabled = true;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                //this.btnCancel.Visible = this.btnCancel.Enabled = false;
                                this.btnBack.Visible = this.btnBack.Enabled = false;
                            }
                            break;
                        case PageLayout.CONFIRM_CREATE:
                            enabled = false;
                            if (hasCreate)
                            {
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                                //this.btnCancel.Visible = this.btnCancel.Enabled = true;
                                this.btnBack.Visible = this.btnBack.Enabled = true;
                            }
                            break;
                        case PageLayout.UPDATE:
                            enabled = true;
                            if (hasUpdate)
                            {
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = false;
                                //this.btnCancel.Visible = this.btnCancel.Enabled = false;
                                this.btnBack.Visible = this.btnBack.Enabled = true;
                            }
                            break;
                        case PageLayout.CONFIRM_UPDATE:
                            enabled = false;
                            if (hasUpdate)
                            {
                                this.btnEdit.Visible = this.btnEdit.Enabled = false;
                                this.btnCreate.Visible = this.btnCreate.Enabled = false;
                                this.btnConfirm.Visible = this.btnConfirm.Enabled = true;
                                //this.btnCancel.Visible = this.btnCancel.Enabled = false;
                                this.btnBack.Visible = this.btnBack.Enabled = true;
                            }
                            break;
                    }
                }
            }

            this.txtExternalSystemName.Enabled =
                this.ddlexternalsytemtype.Enabled = enabled;

            this.txtExternalSystemName.Visible =
                this.ddlexternalsytemtype.Visible = visable;
        }
        #endregion

        #region Helper Methods

        private bool ValidateExternalFields()
        {
            if (GrdExternalSystemFields.Rows.Count <= 0)
            {
                return false;
            }
            else if (GrdExternalSystemFields.Rows.Count == 1)
            {

                if (GrdExternalSystemFields.DataKeys[0].Value.ToString() == "")
                {
                    return false;
                }
            }
            return true;
        }
        private ExternalSystemFieldResult ReadControlValues()
        {

            ExternalSystemFieldResult obj = new ExternalSystemFieldResult();

            ExternalSystemsResult externalsystem = new ExternalSystemsResult();
            if (ExternalSystemId != 0 && ExternalSystemId != null)
            {
                externalsystem.external_system_id = (int)ExternalSystemId;
            }
            int external_system_typeid = Convert.ToInt32(this.ddlexternalsytemtype.SelectedValue);
            externalsystem.system_name = txtExternalSystemName.Text;
            externalsystem.external_system_type_id = external_system_typeid;

            obj.ExternalSystem = externalsystem;
            obj.ExternalSystemFields = PopulateExternalSystemFields();
            return obj;
        }

        private bool CreateExternalSystem()
        {
            try
            {


                string responsemessage; int external_system_id;
                if (_issuerMan.CreateExternalSystem(ReadControlValues(), out external_system_id, out responsemessage))
                {
                    ExternalSystemId = external_system_id;
                    pageLayout = PageLayout.READ;
                    return true;
                }
                else
                {
                    lblErrorMessage.Text = responsemessage;
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        private bool UpdateExternalSystem()
        {

            string responsemessage = string.Empty;

            if (_issuerMan.UpdateExternalSystem(ReadControlValues(), out responsemessage))
            {
                pageLayout = PageLayout.READ;

                return true;
            }
            else
            {
                lblErrorMessage.Text = responsemessage;
            }

            return false;
        }
        #endregion

        #region Grid Commands

        public static DataTable ToDataTable<T>(List<T> items)
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
        private ExternalSystemFieldsResult[] PopulateExternalSystemFields()
        {
            List<ExternalSystemFieldsResult> fields = new List<ExternalSystemFieldsResult>();
            foreach (DataRow row in ExternalSystemFields.Rows)
            {

                ExternalSystemFieldsResult externalsystem = new ExternalSystemFieldsResult();
                if (Convert.ToBoolean(row["Status"]))
                {
                    externalsystem.external_system_field_id = 0;
                }
                else
                {
                    externalsystem.external_system_field_id = int.Parse(row["external_system_field_id"].ToString());

                }
                externalsystem.field_name = row["field_name"].ToString();

                fields.Add(externalsystem);
            }


            return fields.ToArray();
        }
        protected void GrdExternalSystemFields_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && LoadDataEmpty)
            {
                e.Row.Visible = false;
            }
        }
        private void BindGridView(List<ExternalSystemFieldsResult> externalsystemfields)
        {
            if (externalsystemfields == null)
            {
                externalsystemfields = new List<ExternalSystemFieldsResult>();

            }

            //Declare a datatable for the gridview
            DataTable dt = ToDataTable<ExternalSystemFieldsResult>(externalsystemfields);
            dt.Columns.Add("Status", typeof(bool));
            foreach (DataRow row in dt.Rows)
            {
                row["Status"] = false;

            }
            if (dt.Rows.Count == 0)
            {
                dt.Rows.Add(dt.NewRow());
                LoadDataEmpty = true;
            }

            GrdExternalSystemFields.DataSource = dt;
            GrdExternalSystemFields.DataBind();
            //Now hide the extra row of the grid view
            //GrdExternalSystemFields.Rows[0].Visible = false;
            //Delete row 0 from the datatable

            //View the datatable to the viewstate
            ExternalSystemFields = dt;
        }
        protected void EditRow(object sender, GridViewEditEventArgs e)
        {

            GrdExternalSystemFields.EditIndex = e.NewEditIndex;
            //Now bind the gridview
            GrdExternalSystemFields.DataSource = ExternalSystemFields;
            GrdExternalSystemFields.DataBind();
        }



        protected void CancelEditRow(object sender, GridViewCancelEditEventArgs e)
        {

            GrdExternalSystemFields.EditIndex = -1;
            GrdExternalSystemFields.DataSource = ExternalSystemFields;
            GrdExternalSystemFields.DataBind();

        }

        protected void UpdateRow(object sendedr, GridViewUpdateEventArgs e)
        {

            var external_system_field_id = GrdExternalSystemFields.DataKeys[e.RowIndex].Value;

            GridViewRow row = GrdExternalSystemFields.Rows[e.RowIndex] as GridViewRow;

            TextBox tbfield_name = row.FindControl("etfieldname") as TextBox;
            if (tbfield_name.Text != string.Empty)
            {

                DataRow dr = ExternalSystemFields.Rows[e.RowIndex];

                dr.BeginEdit();
                dr["field_name"] = tbfield_name.Text;

                dr.EndEdit();
                dr.AcceptChanges();
                GrdExternalSystemFields.EditIndex = -1;
                //Now bind the datatable to the gridview
                GrdExternalSystemFields.DataSource = ExternalSystemFields;
                GrdExternalSystemFields.DataBind();
            }
            else
            {
                lblErrorMessage.Text = "Field Name Required.";
            }


        }
        protected void ChangePage(object sender, GridViewPageEventArgs e)
        {

            GrdExternalSystemFields.EditIndex = -1;
            GrdExternalSystemFields.DataSource = ExternalSystemFields;
            GrdExternalSystemFields.DataBind();

        }

        public DataTable ExternalSystemFields
        {
            get
            {
                if (ViewState["ExternalSystemFields"] == null)
                    return null;
                else
                    return (DataTable)(ViewState["ExternalSystemFields"]);
            }
            set
            {
                ViewState["ExternalSystemFields"] = value;
            }
        }

        protected void DeleteRow(object sender, GridViewDeleteEventArgs e)
        {
            ExternalSystemFields.Rows[e.RowIndex].Delete();
            ExternalSystemFields.AcceptChanges();

            if (ExternalSystemFields.Rows.Count > 0)
            {

                GrdExternalSystemFields.DataSource = ExternalSystemFields;
                GrdExternalSystemFields.DataBind();
            }


        }
        #endregion

        protected void GrdExternalSystemFields_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToUpper() == "CREATE")
            {
                GridViewRow row = GrdExternalSystemFields.FooterRow as GridViewRow;
                TextBox tbfield_name = row.FindControl("ftfieldname") as TextBox;

                if (tbfield_name.Text != string.Empty)
                {


                    Random random = new Random();
                    int rowid = random.Next(10000000, 99999999);

                    DataRow dr = ExternalSystemFields.NewRow();
                    dr["external_system_field_id"] = rowid;
                    dr["Status"] = false;
                    dr["field_name"] = tbfield_name.Text;
                    ExternalSystemFields.Rows.Add(dr);
                    foreach (DataRow dtrow in ExternalSystemFields.Rows)
                    {
                        if (dtrow["external_system_field_id"] == null || dtrow["external_system_field_id"].ToString() == string.Empty)
                        {
                            dtrow.Delete();
                            break;
                        }
                    }
                    ExternalSystemFields.AcceptChanges();

                    GrdExternalSystemFields.DataSource = ExternalSystemFields;
                    GrdExternalSystemFields.DataBind();
                }
                else
                {
                    lblErrorMessage.Text = "Field Name Required.";
                }
            }

        }







    }
}