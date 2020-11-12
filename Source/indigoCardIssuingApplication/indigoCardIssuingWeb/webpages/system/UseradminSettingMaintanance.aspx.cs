using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
namespace indigoCardIssuingWeb.webpages.users
{
    public partial class UseradminSettingMaintanance : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserGroupCreation));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };
        private readonly SystemAdminService sysAdminService = new SystemAdminService();
        private readonly UserManagementService userMan = new UserManagementService();

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
            dlUseradminSettings.DataSource = null;
            dlUseradminSettings.DataBind();
            try
            {
                BindGrid();
                if (dlUseradminSettings.Items.Count > 0)
                    pnlButtons.Visible = false;
            }
            catch (Exception ex)
            {

                this.pnlButtons.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        protected void dlUseradminSettings_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlUseradminSettings.SelectedIndex = e.Item.ItemIndex;
                string usersettingsIdStr = ((Label)dlUseradminSettings.SelectedItem.FindControl("lbluseradminsettingsID")).Text;

                int UserAdminSettingsId;
                if (int.TryParse(usersettingsIdStr, out UserAdminSettingsId))
                {
                    SessionWrapper.UserAdminSettingsID = UserAdminSettingsId;
                    Server.Transfer("~\\webpages\\system\\UseradminSettingsViewForm.aspx");
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
        protected void btnCreateUserAdminSettings_Click(object sender, EventArgs e)
        {
            SessionWrapper.SelectedUserGroupId = null;
            Server.Transfer("~\\webpages\\system\\UseradminSettingsViewForm.aspx");
        }



        #region Pagination

        protected void BindGrid()
        {
            this.lblErrorMessage.Text = "";
            this.dlUseradminSettings.DataSource = null;
            List<useradminsettingslist> results = new List<useradminsettingslist>();

            results.Add(userMan.GetUseradminSettings());


            if (results.Count > 0)
            {
                this.dlUseradminSettings.DataSource = results;
                this.dlUseradminSettings.DataBind();
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }



        }
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {

        }
        #endregion
    }
}