using System;
using System.Drawing;
using System.Web.UI;
using System.Linq;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using System.Web.Security;
using System.Security.Permissions;
using System.Data;
using System.Reflection;

namespace indigoCardIssuingWeb.webpages.issuer
{
    public partial class ManageBranch : BasePage
    {
        private const string PageLayoutKey = "PageLayout";

        private static readonly ILog log = LogManager.GetLogger(typeof(ManageBranch));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_ADMIN };
        private readonly SystemAdminService sysAdminService = new SystemAdminService();
        private readonly UserManagementService userMan = new UserManagementService();

        private PageLayout pageLayout = PageLayout.READ;

        #region PAGE LOAD

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (ViewState[PageLayoutKey] != null)
            {
                pageLayout = (PageLayout)ViewState[PageLayoutKey];
            }

            if (!IsPostBack)
            {
                LoadFormData();
            }
        }

        private void LoadFormData()
        {
            try
            {
                this.ddlIssuer.Items.Clear();

                Dictionary<int, ListItem> issuerList = Roles.Provider.ToIndigoRoleProvider()
                                                        .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                try
                {
                    if (SessionWrapper.IssuerID != null)
                    {
                        issuerid = SessionWrapper.IssuerID;
                        BranchStatusId = SessionWrapper.BranchStatusId;
                        SessionWrapper.IssuerID = 0;
                        SessionWrapper.BranchStatusId = 0;
                    }

                    this.ddlIssuer.Items.AddRange(issuerList.Values.OrderBy(m => m.Text).ToArray());
                    this.ddlIssuer.SelectedIndex = 0;

                    //Load branch statuses into drop down list
                    List<ListItem> statusItems = new List<ListItem>();
                    foreach (var item in sysAdminService.LangLookupBranchStatuses())
                    {
                        statusItems.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                    }

                    ddlBranchStatus.Items.AddRange(statusItems.OrderBy(m => m.Text).ToArray());
                    ddlBranchStatus.SelectedIndex = 0;


                    //Load branch statuses into drop down list
                    List<ListItem> branchtypes = new List<ListItem>();
                    foreach (var item in sysAdminService.LangLookupBranchTypes())
                    {
                        branchtypes.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                    }

                    ddlBranchType.Items.AddRange(branchtypes.OrderBy(m => m.Text).ToArray());
                    ddlBranchType.SelectedIndex = 0;

                    LoadBranchDetails();
                    UpdateFormLayout(null);



                    if (ddlIssuer.Items.Count == 0)
                    {
                        lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                    }
                    if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                    {
                        pnlButtons.Visible = false;
                        lblInfoMessage.Text = "";
                        lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SearchActionMessage").ToString();
                    }
                    else
                    {
                        // lblErrorMessage.Text = "";
                        pnlButtons.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                    if (log.IsDebugEnabled || log.IsTraceEnabled)
                    {
                        this.lblErrorMessage.Text = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        private void LoadBranchDetails()
        {
            try
            {
                if (SessionWrapper.BranchID != null)
                {
                    int branchId = SessionWrapper.BranchID.GetValueOrDefault();

                    branch branchResult = sysAdminService.GetBranchById(branchId);

                    this.tbLocation.Text = branchResult.location;
                    this.tbContactEmail.Text = branchResult.contact_email;
                    this.tbContactPerson.Text = branchResult.contact_person;
                    this.tbBranchCode.Text = branchResult.branch_code;
                    this.tbEmpBranchCode.Text = branchResult.emp_branch_code;

                    this.tbBranchName.Text = branchResult.branch_name;
                    //this.chkCardCentreBranch.Checked = branchResult.card_centre_branch_YN;

                    this.ddlBranchStatus.SelectedValue = branchResult.branch_status_id.ToString();
                    this.ddlIssuer.SelectedValue = branchResult.issuer_id.ToString();

                    this.ddlBranchType.SelectedValue = branchResult.branch_type_id.ToString();
                    
                    SessionWrapper.BranchID = null;
                    BranchId = branchId;
                    LoadBranches(branchResult.main_branch_id);
                    ViewState[PageLayoutKey] = PageLayout.READ;
                }
                else
                {
                    ViewState[PageLayoutKey] = PageLayout.CREATE;
                    LoadSatelliteBranches();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        #endregion       

        #region Page Flow Methods        
        private void UpdateFormLayout(PageLayout? toPageLayout)
        {
            this.btnCreate.Enabled = this.btnCreate.Visible = false;
            this.btnEdit.Enabled = this.btnEdit.Visible = false;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;

            if (toPageLayout == null)
            {
                if (ViewState[PageLayoutKey] != null)
                {
                    pageLayout = (PageLayout)ViewState[PageLayoutKey];
                }
                toPageLayout = pageLayout;
            }

            bool canRead;
            bool canUpdate;
            bool canCreate;
            if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_ADMIN, out canRead, out canUpdate, out canCreate))
            {
                switch (toPageLayout)
                {
                    case PageLayout.CREATE:
                        EnableFields(true);
                        if (canCreate)
                        {
                            this.btnCreate.Enabled = this.btnCreate.Visible = true;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                        }
                        break;
                    case PageLayout.READ:
                        DisableFields(false, false);
                        if (canUpdate)
                        {
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = true;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                            this.btnBack.Enabled = this.btnBack.Visible = true;

                        }
                        break;
                    case PageLayout.UPDATE:
                        EnableFields(false);
                        if (canUpdate)
                        {
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                        }
                        break;
                    case PageLayout.DELETE:
                        break;
                    case PageLayout.CONFIRM_CREATE:
                        DisableFields(true, true);
                        if (canCreate)
                        {
                            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                        }
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        DisableFields(true, false);
                        this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        DisableFields(true, false);
                        if (canUpdate)
                        {
                            this.lblInfoMessage.Text = "Please review information then click confirm to commit changes.";
                            this.btnCreate.Enabled = this.btnCreate.Visible = false;
                            this.btnEdit.Enabled = this.btnEdit.Visible = false;
                            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                            this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                        }
                        break;
                    default:
                        DisableFields(false, false);
                        this.btnCreate.Enabled = this.btnCreate.Visible = false;
                        this.btnEdit.Enabled = this.btnEdit.Visible = false;
                        this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                        this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                        break;
                }
            }

            ViewState[PageLayoutKey] = toPageLayout;
        }

        private void EnableFields(bool isCreate)
        {
            this.tbBranchName.Enabled = true;
            this.tbBranchCode.Enabled = true;
            this.tbContactEmail.Enabled = true;
            this.tbContactPerson.Enabled = true;
            this.tbLocation.Enabled = true;
            this.ddlBranchStatus.Enabled = true;
            this.tbEmpBranchCode.Enabled = true;
            this.ddlIssuer.Enabled = true;
            this.ddlBranchType.Enabled = true;
            this.ddlBranch.Enabled = true;
            this.pnlsatellitebranches.Enabled=true;
            this.btnBack.Visible = isCreate ? false : true;
        }

        private void DisableFields(bool IsConfirm, bool isCreate)
        {
            this.tbBranchName.Enabled = false;
            this.tbBranchCode.Enabled = false;
            this.tbEmpBranchCode.Enabled = false;
            this.tbContactEmail.Enabled = false;
            this.tbContactPerson.Enabled = false;
            this.tbLocation.Enabled = false;
            this.ddlBranchStatus.Enabled = false;
            this.ddlIssuer.Enabled = false;
            this.ddlBranchType.Enabled = false;
            this.ddlBranch.Enabled = false;
            this.pnlsatellitebranches.Enabled = false;
            this.btnBack.Visible = IsConfirm ? true : false;
        }

        #endregion

        #region Helper Methods

        private branch PopulateBranchObject()
        {
            int issuerId;
            int branchStatusId;

            if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) &&
                   int.TryParse(this.ddlBranchStatus.SelectedValue, out branchStatusId))
            {
                branch createBranch = new branch();

                createBranch.issuer_id = issuerId;
                createBranch.branch_status_id = branchStatusId;

                createBranch.branch_code = this.tbBranchCode.Text;
                createBranch.emp_branch_code = this.tbEmpBranchCode.Text;
                createBranch.branch_name = this.tbBranchName.Text;
                createBranch.contact_email = this.tbContactEmail.Text;
                createBranch.contact_person = this.tbContactPerson.Text;
                createBranch.location = this.tbLocation.Text;
                createBranch.branch_type_id = int.Parse(ddlBranchType.SelectedValue);
                int branchid = -1;
                int.TryParse(this.ddlBranch.SelectedValue, out branchid);
                if (branchid > 0)
                {
                    createBranch.main_branch_id = branchid;
                }
                createBranch.main_branch_id = (branchid==0)?-1: branchid;
                //createBranch.card_centre_branch_YN = this.chkCardCentreBranch.Checked;

                if (BranchId != null)
                {
                    createBranch.branch_id = BranchId.Value;
                }

                return createBranch;
            }

            return null;
        }

        private List<int> Populatesatellitebranch()
        {
            List<int> _branches=new List<int>();
            DataTable dt =new DataTable();
            if (ViewState["SatelliteBranches"] != null)
            {
                 dt = (DataTable)ViewState["SatelliteBranches"];

            }
            foreach (DataRow row in dt.Rows)
            {
                int branchid = Convert.ToInt32(row["branch_id"]);

                _branches.Add(branchid);
            }
            //foreach (GridViewRow row in grdsatellite.Rows)
            //{
            //   int branchid = Convert.ToInt32(grdsatellite.DataKeys[row.RowIndex].Value);

            //    _branches.Add(branchid);
            //}
            return _branches;
        }

        private bool CreateBranch()
        {
            bool rtn = false;
            try
            {
                branch createBranch = PopulateBranchObject();

                if (createBranch != null && pageLayout == PageLayout.CONFIRM_CREATE)
                {
                    int branchId;
                    string response;
                    rtn = sysAdminService.CreateBranch(createBranch, Populatesatellitebranch(),out branchId, out response);

                    if (rtn)
                    {
                        BranchId = branchId;
                        this.lblInfoMessage.Text = response;
                        pageLayout = PageLayout.READ;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = response;
                    }
                }

                return rtn;
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

        private bool UpdateBranch()
        {
            bool rtn = false;
            try
            {
                branch updateBranch = PopulateBranchObject();

                if (updateBranch != null && pageLayout == PageLayout.CONFIRM_UPDATE)
                {
                    string response;
                    rtn = sysAdminService.UpdateBranch(updateBranch, Populatesatellitebranch(), out response);

                    if (rtn)
                    {
                        this.lblInfoMessage.Text = response;
                        pageLayout = PageLayout.READ;
                    }
                    else
                    {
                        this.lblErrorMessage.Text = response;
                    }
                }

                return rtn;
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
        #endregion

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_ADMIN")]
        protected void btnCreate_OnClick(object sender, EventArgs e)
        {
            UpdateFormLayout(PageLayout.CONFIRM_CREATE);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_ADMIN")]
        protected void btnEdit_OnClick(object sender, EventArgs e)
        {
            UpdateFormLayout(PageLayout.UPDATE);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_ADMIN")]
        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            UpdateFormLayout(PageLayout.CONFIRM_UPDATE);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_ADMIN")]
        protected void btnConfirm_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (ViewState[PageLayoutKey] != null)
                {
                    pageLayout = (PageLayout)ViewState[PageLayoutKey];
                }

                switch (pageLayout)
                {
                    case PageLayout.CONFIRM_CREATE:
                        if (CreateBranch())
                            UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        if (UpdateBranch())
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
        protected void ddlBranchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBranches(null);
        }
        private void LoadSatelliteBranches()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[2] { new DataColumn("branch_id"), new DataColumn("branch_name") });
            ViewState["SatelliteBranches"] = dt;
        }

        protected void BindGrid()
        {
            if (ViewState["SatelliteBranches"] == null)

            {
                LoadSatelliteBranches();
            }
    DataTable dt = (DataTable)ViewState["SatelliteBranches"];
                if (dt.Rows.Count == 0)
                {
                    DataTable dt_empty = dt.Clone();
                    DataRow row = dt_empty.NewRow();

                    dt_empty.Rows.Add(row);
                    grdsatellite.DataSource = dt_empty;
                    grdsatellite.DataBind();
                }
                else
                {
                    grdsatellite.DataSource = dt;
                    grdsatellite.DataBind();
                }
            
        }
        protected void grdsatellite_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                
                    DropDownList ddList = (DropDownList)e.Row.FindControl("ddlBranch");

                    //bind dropdown-list
                    ddList.DataSource = GetBranches().Where(i=>i.branch_type_id!=(int)BranchTypes.CARD_CENTER);
                    ddList.DataTextField = "branch_name";
                    ddList.DataValueField = "branch_id";
                    ddList.DataBind();
                if (BranchId == null)
                    ddList.SelectedIndex = 0;
                else
                {
                    ddList.Items.Remove(ddList.Items.FindByValue(BranchId.ToString()));
                }
                DataRowView dr = e.Row.DataItem as DataRowView;
                    //ddList.SelectedItem.Text = dr["category_name"].ToString();
                    //ddList.SelectedValue = dr["branch_id"].ToString();
                
            }
        }
        protected void grdsatellite_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            this.BindGrid();
            grdsatellite.PageIndex = e.NewPageIndex;

            grdsatellite.DataBind();

        }


        protected void grdsatellite_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToUpper() == "DELETE")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                int branchid= Convert.ToInt32(grdsatellite.DataKeys[rowIndex].Value);
                DataTable dt = (DataTable)ViewState["SatelliteBranches"];
                DataRow[] drr = dt.Select("branch_id ="+ branchid.ToString());
                for (int i = 0; i < drr.Length; i++)
                    drr[i].Delete();
                dt.AcceptChanges();
                ViewState["SatelliteBranches"] = dt;
            }
            else if (e.CommandName.ToUpper() == "ADD")
            {
                DropDownList branch = ((DropDownList)grdsatellite.FooterRow.FindControl("ddlBranch"));
                DataTable dt = (DataTable)ViewState["SatelliteBranches"];
                dt.Rows.Add(branch.SelectedValue, branch.SelectedItem.Text);
                ViewState["SatelliteBranches"] = dt;
                
            }
            this.BindGrid();
        }
        protected List<BranchesResult> GetBranches()
        {
            int issuerId;
            List<BranchesResult> branches = null;
            if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
            {
                branches = userMan.GetBranchesForUserAdmin(issuerId, ((int)BranchStatus.ACTIVE));
            }
            return branches;
        }
            protected void LoadBranches(int? branchId)
        {
            divbranchtype.Attributes.Add("style", "display:none");
            pnlsatellitebranches.Visible=false;
          
            if (ddlBranchType.SelectedValue == ((int)BranchTypes.SATELLITE).ToString())
            {
                divbranchtype.Attributes.Add("style", "display:block");
               
                List<ListItem> lst_branches = new List<ListItem>();
                List<BranchesResult> branches=GetBranches();
                ddlBranch.Items.Clear();


                foreach (var item in branches.Where(i => i.branch_type_id == ((int)BranchTypes.MAIN)))
                {
                    if (BranchId != item.branch_id)
                        lst_branches.Add(new ListItem(item.branch_name, item.branch_id.ToString()));
                }

                ddlBranch.Items.AddRange(lst_branches.OrderBy(m => m.Text).ToArray());
                ddlBranch.Items.Insert(0, new ListItem(Resources.ListItemLabels.SELECT, "-1"));

                if (branchId == null)
                    ddlBranch.SelectedIndex = 0;
                else
                {
                    ddlBranch.Items.Remove(ddlBranch.Items.FindByValue(BranchId.ToString()));
                    ddlBranch.SelectedValue = branchId.ToString();

                }
            }
            else if (ddlBranchType.SelectedValue == ((int)BranchTypes.MAIN).ToString())
            {
                pnlsatellitebranches.Visible = true;
                List<BranchesResult> branches_result = GetBranches();
                List <BranchesResult> _branches=null;
                DataTable newTable =new DataTable();
                if (BranchId != null)
                {
                    _branches = branches_result
                                       .Distinct()
                                       .Where(i => i.main_branch_id == BranchId)
                                       .OrderByDescending(i => i.branch_name).ToList();
                    DataTable dt = ToDataTable(_branches);

                     newTable = dt.DefaultView.ToTable(true, "branch_id", "branch_name");
                }
                else
                {
                    newTable=(DataTable)ViewState["SatelliteBranches"];
                }
                
                ViewState["SatelliteBranches"]= newTable;              
                BindGrid();
            }

            
        }
        public static DataTable ToDataTable<BranchesResult>(List<BranchesResult> items)
        {
            DataTable dataTable = new DataTable(typeof(BranchesResult).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(BranchesResult).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (BranchesResult item in items)
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
                case PageLayout.READ:
                    SessionWrapper.IssuerID = issuerid;
                    SessionWrapper.BranchStatusId = BranchStatusId;
                    Response.Redirect("~\\webpages\\issuer\\BranchList.aspx");
                    break;
                default:
                    break;
            }
        }

        private int? BranchId
        {
            get
            {
                if (ViewState["BranchIdKey"] != null)
                    return (int)ViewState["BranchIdKey"];
                else
                    return null;
            }
            set
            {
                ViewState["BranchIdKey"] = value;
            }
        }
        private int? issuerid
        {
            get
            {
                if (ViewState["issuerid"] != null)
                    return (int)ViewState["issuerid"];
                else
                    return null;
            }
            set
            {
                ViewState["issuerid"] = value;
            }
        }
        private int? BranchStatusId
        {
            get
            {
                if (ViewState["BranchStatusId"] != null)
                    return (int)ViewState["BranchStatusId"];
                else
                    return null;
            }
            set
            {
                ViewState["BranchStatusId"] = value;
            }
        }

        protected void grdsatellite_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }
    }
}