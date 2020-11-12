using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.utility;
using System.Globalization;
using System.Threading;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.App_Code.SearchParameters;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.load
{
    public partial class FileLoadList : ListPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FileLoadList));
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

                //if (SessionWrapper.FileLoadSearchResult != null && SessionWrapper.FileLoadSearchParams != null)
                //{
                //    var results = SessionWrapper.FileLoadSearchResult;
                //    FileDetailsParams = SessionWrapper.FileLoadSearchParams;

                //    DisplayResults(FileDetailsParams, FileDetailsParams.PageIndex, results);

                //    // SessionWrapper.FileLoadSearchParams = null;
                //    SessionWrapper.FileLoadSearchResult = null;
                //}
                //else if (SessionWrapper.FileLoadSearchParams != null)
                //{
                //    //FileLoadSearchParamerts
                //    FileDetailsSearch searchParms = SessionWrapper.FileLoadSearchParams;
                //    FileDetailsParams = searchParms;

                //    DisplayResults(searchParms, searchParms.PageIndex, null);

                //    // SessionWrapper.FileLoadSearchParams = null;
                //}
                //else
                //{
                //    // DisplayResults(null,1,null);
                //}

                if (SessionWrapper.FileLoadParams != null)
                {
                    FileLoadSearchParameters searchParms = SessionWrapper.FileLoadParams;
                    SessionWrapper.FileLoadParams = null;

                    this.tbDateFrom.Text = searchParms.DateFrom.ToString(DATE_FORMAT);
                    this.tbDateTo.Text = searchParms.DateTo.ToString(DATE_FORMAT);

                    DisplayResults(searchParms, searchParms.PageIndex, null);
                }
                else
                {
                    this.tbDateFrom.Text = DateTime.Now.ToString(DATE_FORMAT);
                    this.tbDateTo.Text = DateTime.Now.ToString(DATE_FORMAT);
                    FileLoadSearchParameters searchParms = new FileLoadSearchParameters(DateTime.Now.Date, DateTime.Now.Date, StaticDataContainer.ROWS_PER_PAGE, 1);
                    DisplayResults(searchParms, searchParms.PageIndex, null);
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
        }

        protected void dlFileloadlist_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                dlFileloadlist.SelectedIndex = e.Item.ItemIndex;
                string fileLoadIdStr = ((Label)dlFileloadlist.SelectedItem.FindControl("lblFileLoadId")).Text;
                
                int fileLoadId;
                if (int.TryParse(fileLoadIdStr, out fileLoadId))
                {
                    SessionWrapper.FileLoadParams = (FileLoadSearchParameters)SearchParameters;
                    SessionWrapper.FileLoadId = fileLoadId;
                    Server.Transfer("~\\webpages\\load\\FileLoaderLogView.aspx");
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

        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_MANAGER")]
        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "AUDITOR")]
        [PrincipalPermission(SecurityAction.Demand, Role = "CARD_PRODUCTION")]
        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            try
            {
                DateTime dateFrom;
                DateTime dateTo;
                if (DateTime.TryParseExact(this.tbDateFrom.Text, DATE_FORMAT, null, DateTimeStyles.None, out dateFrom) &&
                    DateTime.TryParseExact(this.tbDateTo.Text, DATE_FORMAT, null, DateTimeStyles.None, out dateTo))
                {
                    FileLoadSearchParameters searchParms = new FileLoadSearchParameters(dateFrom, dateTo, StaticDataContainer.ROWS_PER_PAGE, 1);
                    DisplayResults(searchParms, searchParms.PageIndex, null);
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
        
        #region Pagination

        //protected void btnBack_Click(object sender, EventArgs e)
        //{
        //    SessionWrapper.FileLoadSearchParams = FileDetailsParams;
        //    Server.Transfer("~\\webpages\\system\\FileLoaderLogSearch.aspx");
        //}
        
        #region PrivateMethods
        protected override void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {            
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            this.dlFileloadlist.DataSource = null;

            try
            {
                var fileloadSearchParms = (FileLoadSearchParameters)parms;
                SearchParameters = parms;

                if (results == null)
                {
                    parms.PageIndex = pageIndex;
                    results = sysAdService.GetFileLoadList(fileloadSearchParms).ToArray();
                }

                if (results.Length > 0)
                {
                    this.dlFileloadlist.DataSource = results;
                    pnlpage.Visible = true;
                    TotalPages = ((FileLoadResult)results[0]).TOTAL_PAGES;
                }
                else
                {
                    TotalPages = 0;
                    pnlpage.Visible = false;
                    this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                }

                this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
                this.dlFileloadlist.DataBind();
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
        #endregion
    }
}