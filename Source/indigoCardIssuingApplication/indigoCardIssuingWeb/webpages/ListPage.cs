using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb
{
    /// <summary>
    /// Default implementation for List pages
    /// </summary>
    public class ListPage : BasePage
    {
        #region ResultsNavigation
        private void ChangePage(ResultNavigation resultNavigation)
        {

            ////Clear error messages
            //var errorLabel = FindControl("lblErrorMessage");
            //if(errorLabel != null && errorLabel is Label)
            //    ((Label)errorLabel).Text = String.Empty;

            switch (resultNavigation)
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

            if (SearchParameters != null)
                SearchParameters.PageIndex = PageIndex;

            DisplayResults(SearchParameters, PageIndex, null);
        }

        /// <summary>
        /// Implment this for results pagination
        /// </summary>
        /// <param name="pageIndex"></param>
        protected virtual void DisplayResults(ISearchParameters parms, int pageIndex, object[] results)
        {
            throw new NotImplementedException();
        }

        protected void lnkFirst_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.FIRST);
        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.PREVIOUS);
        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.NEXT);
        }

        protected void lnkLast_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.LAST);
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

        public ISearchParameters SearchParameters
        {
            get
            {
                if (ViewState["SearchParameters"] != null)
                    return (ISearchParameters)ViewState["SearchParameters"];
                else
                    return null;
            }
            set
            {
                ViewState["SearchParameters"] = value;
            }
        }
        #endregion
    }
}