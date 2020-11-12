using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using System.Web.Security;


namespace indigoCardIssuingWeb.webpages.issuer
{
    public partial class BranchList : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BranchList));
        private readonly UserManagementService userMan = new UserManagementService();
        private readonly SystemAdminService sysAdminService = new SystemAdminService();

        private readonly UserRole[] userRolesForPage = new UserRole[]{ UserRole.BRANCH_ADMIN };

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadBranches();
            }
        }

        protected void dlBranchList_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlBranchList.SelectedIndex = e.Item.ItemIndex;
                string strBranchId = ((Label)dlBranchList.SelectedItem.FindControl("lblBranchID")).Text;

                int branchId = 0;
                if (int.TryParse(strBranchId, out branchId))
                {
                    SessionWrapper.BranchID = branchId;
                 int issuerId, branchstatusid;
                 if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && int.TryParse(this.ddlBranchStatus.SelectedValue, out branchstatusid))
                 {
                     SessionWrapper.IssuerID = issuerId;
                     SessionWrapper.BranchStatusId = branchstatusid;
                 }
                    Server.Transfer("ManageBranch.aspx");
                }
                else
                {
                    throw new Exception("Issue trying to set the branch Id. Cannot process further.");
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

        #region private_ fields
        private void LoadBranches()
        {
            Dictionary<int, ListItem> issuerList = Roles.Provider.ToIndigoRoleProvider()
                                                        .GetIssuersForRole(User.Identity.Name, userRolesForPage);
            try
            {
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

                if (SessionWrapper.BranchStatusId != null)
                {
                    ddlIssuer.SelectedValue = SessionWrapper.IssuerID.ToString();
                    ddlBranchStatus.SelectedValue = SessionWrapper.BranchStatusId.ToString();
                    SessionWrapper.IssuerID = null;
                    SessionWrapper.BranchStatusId = null;
                }
                int issuerId, branchstatusid;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && int.TryParse(this.ddlBranchStatus.SelectedValue, out branchstatusid))
                {
                    getBranchesForIssuer(issuerId, branchstatusid);
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.Message;
                }
            }
        }

        private void getBranchesForIssuer(int issuerId, int branchstatusid)
        {
            this.dlBranchList.DataSource = null;
            this.dlBranchList.DataBind();

            var branches = userMan.GetBranchesForUserAdmin(issuerId, branchstatusid);

            if (branches.Count > 0)
            {
                this.dlBranchList.DataSource = branches;
                this.dlBranchList.DataBind();
            }
            else
            {
                lblErrorMessage.Text = GetLocalResourceObject("InfoNoRecordsFound").ToString();
            }
        }
        #endregion

        protected void ddlBranchStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                int issuerId, branchstatusid;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && int.TryParse(this.ddlBranchStatus.SelectedValue, out branchstatusid))
                {
                    getBranchesForIssuer(issuerId, branchstatusid);
                }
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
        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                int issuerId, branchstatusid;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId) && int.TryParse(this.ddlBranchStatus.SelectedValue, out branchstatusid))
                {
                    getBranchesForIssuer(issuerId, branchstatusid);
                }
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
    }
}