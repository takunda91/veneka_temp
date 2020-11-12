using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using System.Threading;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CCO.objects;
using System.Drawing;
using System.Web.Security;
using System.Security.Permissions;
using indigoCardIssuingWeb.SearchParameters;

namespace indigoCardIssuingWeb.webpages.load
{
    public partial class FileLoaderLogSearch : BasePage
    {
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER,
                                                                        UserRole.ADMINISTRATOR,
                                                                        UserRole.AUDITOR,
                                                                        UserRole.CARD_PRODUCTION };
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private static readonly ILog log = LogManager.GetLogger(typeof(FileLoaderLogSearch));
        private readonly SystemAdminService sysAdService = new SystemAdminService();

        #region LOAD PAGE
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
            try
            {
                //preset date
                this.tbDateFrom.Text = DateTime.Today.ToString(DATE_FORMAT);
                this.tbDateTo.Text = DateTime.Today.ToString(DATE_FORMAT);

                Dictionary<int, ListItem> issuerList = Roles.Provider.ToIndigoRoleProvider()
                                                        .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                this.ddlIssuer.Items.AddRange(issuerList.Values.OrderBy(m => m.Text).ToArray());

                if (ddlIssuer.Items.FindByValue("-1") != null)
                {
                    ddlIssuer.Items.RemoveAt(ddlIssuer.Items.IndexOf(ddlIssuer.Items.FindByValue("-1")));
                }
                if (ddlIssuer.Items.Count > 1)
                {
                    ddlIssuer.Items.Insert(0, new ListItem(Resources.ListItemLabels.ALL, "-99"));
                }
                this.ddlIssuer.SelectedIndex = 0;
                if (SessionWrapper.FileLoadSearchParams != null)
                {
                    FileDetailsSearch fileloadsearch = SessionWrapper.FileLoadSearchParams;
                    if (fileloadsearch.IssuerId != null)
                        this.ddlIssuer.SelectedValue = fileloadsearch.IssuerId.ToString();

                    if (fileloadsearch.FileName != null)
                        this.tbKeyWord.Text = fileloadsearch.FileName.ToString();

                    this.tbDateFrom.Text = fileloadsearch.DateFrom.GetValueOrDefault().ToString(DATE_FORMAT);
                    this.tbDateTo.Text = fileloadsearch.DateTo.GetValueOrDefault().ToString(DATE_FORMAT);

                    SessionWrapper.FileLoadSearchParams = null;
                }
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

        #endregion

        #region BUTTON CLICKS
        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "AUDITOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CARD_PRODUCTION")]
        protected void btnSearch_Click(object sender, EventArgs e)   
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            
            try
            { //check date field entered
                var FileDetailsParams = new FileDetailsSearch();
                //reset error message

                if (tbDateFrom.Text.Trim() == "" )
                {
                    GenerateErrorMessage(GetLocalResourceObject("ValidationLoadDateEmpty").ToString());
                }
                else
                {
                    FileDetailsParams.FileName = String.IsNullOrWhiteSpace(this.tbKeyWord.Text) ? null : this.tbKeyWord.Text.Trim();
                    FileDetailsParams.FileLoaderStatus = null;
                    FileDetailsParams.DateFrom = DateTime.ParseExact(this.tbDateFrom.Text, DATE_FORMAT, null, DateTimeStyles.None);
                    FileDetailsParams.DateTo = DateTime.ParseExact(this.tbDateTo.Text, DATE_FORMAT, null, DateTimeStyles.None);
                    if (this.ddlIssuer.SelectedValue != "-99")
                    {
                        FileDetailsParams.IssuerId = int.Parse(this.ddlIssuer.SelectedValue);
                    }
                    else
                    {
                        FileDetailsParams.IssuerId = null;
                    }
                    FileDetailsParams.PageIndex = 1;
                    FileDetailsParams.RowsPerPage = StaticDataContainer.ROWS_PER_PAGE;

                    SessionWrapper.FileLoadSearchParams = FileDetailsParams;
                    List<GetFileLoderLog_Result> results = sysAdService.GetLoadFileLog(FileDetailsParams);
                    if (results.Count > 0)
                    {
                        SessionWrapper.FileLoadSearchResult = results;
                        Server.Transfer("~\\webpages\\load\\FileLoaderLogView.aspx");
                    }
                    else
                    {
                        this.lblInfoMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                    }
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
        
        public int? issuerid
        {
            get
            {
                if (ViewState["issuerid"] == null)
                    return 1;
                else
                    return Convert.ToInt32(ViewState["issuerid"].ToString());
            }
            set
            {
                ViewState["issuerid"] = value;
            }
        }
        private void GenerateErrorMessage(string strResponse)
        {
            lblErrorMessage.ForeColor = Color.Red;

            if (!lblErrorMessage.Text.Trim().Equals(""))
                lblErrorMessage.Text = lblErrorMessage.Text + "<br/>" + strResponse;
            else
                lblErrorMessage.Text = strResponse;
        }
    }
}