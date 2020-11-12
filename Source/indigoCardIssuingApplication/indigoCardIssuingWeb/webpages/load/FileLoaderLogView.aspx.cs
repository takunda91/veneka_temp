using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using indigoCardIssuingWeb.App_Code.SearchParameters;
using System.Web.Security;
using indigoCardIssuingWeb.SearchParameters;

namespace indigoCardIssuingWeb.webpages.load
{
    public partial class FileLoaderLogView : ListPage
    {
        //Standardise look and feel of the Website across all Web Browsers
        private static readonly ILog log = LogManager.GetLogger(typeof(FileLoaderLogView));
        private readonly SystemAdminService sysAdService = new SystemAdminService();
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_MANAGER, 
                                                                        UserRole.ADMINISTRATOR, 
                                                                        UserRole.AUDITOR,
                                                                        UserRole.CARD_PRODUCTION };
        #region LOAD PAGE

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                if (SessionWrapper.FileLoadSearchResult != null && SessionWrapper.FileLoadSearchParams != null)
                {
                    var results = SessionWrapper.FileLoadSearchResult.ToArray();
                    SearchParameters = SessionWrapper.FileLoadSearchParams;

                    DisplayResults(SearchParameters, SearchParameters.PageIndex, results);

                    // SessionWrapper.FileLoadSearchParams = null;
                    SessionWrapper.FileLoadSearchResult = null;
                }
                else if (SessionWrapper.FileLoadSearchParams != null)
                {
                    FileDetailsSearch searchParms = SessionWrapper.FileLoadSearchParams;
                    SearchParameters = searchParms;

                    DisplayResults(searchParms, searchParms.PageIndex, null);

                    SessionWrapper.FileLoadSearchParams = null;
                }
                else if (SessionWrapper.FileLoadId != null)
                {
                    FileLoadSearchParams = SessionWrapper.FileLoadParams;

                    FileDetailsSearch searchParms = new FileDetailsSearch(SessionWrapper.FileLoadId, null, null, null, null, null, 1, StaticDataContainer.ROWS_PER_PAGE);
                    SearchParameters = searchParms;

                    SessionWrapper.FileLoadId = null;
                    SessionWrapper.FileLoadParams = null;

                    DisplayResults(searchParms, searchParms.PageIndex, null);
                }
                else
                {
                    // DisplayResults(null,1,null);
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

        #region Pagination

        protected void btnBack_Click(object sender, EventArgs e)
        {
            SessionWrapper.FileLoadSearchParams = (FileDetailsSearch)SearchParameters;

            if (((FileDetailsSearch)SearchParameters).FileLoadId == null)
                Server.Transfer("~\\webpages\\load\\FileLoaderLogSearch.aspx");
            else
            {
                SessionWrapper.FileLoadParams = FileLoadSearchParams;
                Server.Transfer("~\\webpages\\load\\FileLoadList.aspx");
            }
        }

        #region PrivateMethods
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                var fileSearchParms = (FileDetailsSearch)parms;
                SearchParameters = fileSearchParms;

                if (results == null)
                {
                    parms.PageIndex = pageIndex;
                    results = sysAdService.GetLoadFileLog(fileSearchParms).ToArray();
                }

                if (results.Length > 0)
                {
                    foreach (var item in results)
                    {
                        ((GetFileLoderLog_Result)item).file_load_comments = ((GetFileLoderLog_Result)item).file_load_comments.Replace("\n", "<br />");
                    }

                    this.dlFileloaderloglist.DataSource = results;
                    pnlpage.Visible = true;
                    TotalPages = ((GetFileLoderLog_Result)results[0]).TOTAL_PAGES;
                }
                else
                {
                    TotalPages = 0;
                    pnlpage.Visible = false;
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                }

                this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
                this.dlFileloaderloglist.DataBind();
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

        public FileLoadSearchParameters FileLoadSearchParams
        {
            get
            {
                if (ViewState["FileLoadSearchParams"] != null)
                    return (FileLoadSearchParameters)ViewState["FileLoadSearchParams"];
                else
                    return null;
            }
            set
            {
                ViewState["FileLoadSearchParams"] = value;
            }
        }
        #endregion
    }
}