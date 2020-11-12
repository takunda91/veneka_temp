using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CCO;
using System.Threading;
using System.Globalization;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class FontList : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FontList));
        private readonly BatchManagementService _batchservice = new BatchManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };


        #region LOAD PAGE

        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                //if (DateTime.Now.Subtract(SessionWrapper.PasswordExpiryDate).TotalDays >= 60)
                //{
                //    Response.Redirect("~\\webpages\\users\\UserPasswordMaintainance.aspx");
                //}
                //else
                //{
                    if (!IsPostBack)
                    {
                        this.lblInfoMessage.Text = "";
                        this.lblErrorMessage.Text = "";

                        try
                        {
                            Dictionary<int, string> issuers = new Dictionary<int, string>();

                            //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                            if (PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage))
                            {
                                DisplayResults(1, null);
                            }
                            else
                            {
                                this.pnlDisable.Visible = false;
                                //this.pnlButtons.Visible = false;
                                if (!log.IsTraceEnabled)
                                {
                                    log.Warn("A user tried to access a page that he/she does not have access to.");
                                }
                                log.Trace(m => m(String.Format("User {0} tried to access a page he/she does not have access to.", User.Identity.Name)));
                                lblErrorMessage.Text = Resources.DefaultExceptions.UnauthorisedPageAccessMessage;
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            this.pnlDisable.Visible = false;
                            log.Error(ex);
                            this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                            if (log.IsDebugEnabled || log.IsTraceEnabled)
                            {
                                this.lblErrorMessage.Text = ex.ToString();
                            }
                        }
                    //}
                }
            }
            else
            {
                Response.Redirect("~\\Default.aspx");
            }
        }
     
        #endregion

        #region Page level Events

        protected void dlfontlist_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            try
            {
                dlfontlist.SelectedIndex = e.Item.ItemIndex;
                string fontname = ((LinkButton)dlfontlist.SelectedItem.FindControl("lnkFontName")).Text;
                string fontid = ((Label)dlfontlist.SelectedItem.FindControl("lblfontid")).Text;


                long fontId;
                if (long.TryParse(fontid, out fontId))
                {

                    SessionWrapper.FontId = (int)fontId;

                    Server.Transfer("~\\webpages\\system\\ManageFont.aspx");
                }
                else
                {
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.BadSelectionMessage;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);

            }
        }
        #endregion


        protected override void InitializeCulture()
        {

            int lang = SessionWrapper.UserLanguage;

                Thread.CurrentThread.CurrentUICulture = new CultureInfo(UtilityClass.GetLang(lang));
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(UtilityClass.GetLang(lang));
            
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (
                Request.ServerVariables["http_user_agent"].IndexOf("Chrome", StringComparison.CurrentCultureIgnoreCase) !=
                -1)
                Page.ClientTarget = "uplevel";
            if (
                Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) !=
                -1)
                Page.ClientTarget = "uplevel";
        }



        #region Pagination
        private void ChangePage(ResultNavigation resultNav)
        {
            this.lblErrorMessage.Text = "";
            this.dlfontlist.DataSource = null;

            switch (resultNav)
            {
                case ResultNavigation.FIRST:
                    PageIndex = 1;
                    break;
                case ResultNavigation.NEXT:
                    if (PageIndex < TotalPages)
                    {
                        PageIndex = PageIndex + 1;
                    }
                    break;
                case ResultNavigation.PREVIOUS:
                    if (PageIndex > 1)
                    {
                        PageIndex = PageIndex - 1;
                    }
                    break;
                case ResultNavigation.LAST:
                    PageIndex = TotalPages.GetValueOrDefault();
                    break;
                default:
                    break;
            }

            DisplayResults(PageIndex, null);
        }

        private void DisplayResults( int pageIndex, List<FontResult> results)
        {


            if (results == null)
            {
                results = _batchservice.GetFontlist(pageIndex, StaticDataContainer.ROWS_PER_PAGE);
            }

            if (results.Count > 0)
            {
                this.dlfontlist.DataSource = results;
                TotalPages = (int)results[0].TOTAL_PAGES;
            }
            else
            {
                TotalPages = 0;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlfontlist.DataBind();
        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.NEXT);
        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.PREVIOUS);
        }

        protected void lnkFirst_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.FIRST);
        }
        protected void lnkLast_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.LAST);
        }

        public int issuerid
        {
            get
            {
                if (ViewState["issuerid"] != null)
                    return (int)ViewState["issuerid"];
                else
                    return 0;
            }
            set
            {
                ViewState["issuerid"] = value;
            }
        }

        public int PageIndex
        {
            get
            {
                if (ViewState["PageIndex"] == null)
                    return 1;
                else
                    return Convert.ToInt32(ViewState["PageIndex"].ToString());
            }
            set
            {
                ViewState["PageIndex"] = value;
            }
        }

        public int? TotalPages
        {
            get
            {
                if (ViewState["TotalPages"] == null)
                    return 1;
                else
                    return Convert.ToInt32(ViewState["TotalPages"].ToString());
            }
            set
            {
                ViewState["TotalPages"] = value;
            }
        }
        #endregion
    }
}