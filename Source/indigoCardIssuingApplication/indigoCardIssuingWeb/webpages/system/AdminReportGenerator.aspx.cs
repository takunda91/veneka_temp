using System;
using System.Web.UI;
using indigoCardIssuingWeb.CCO;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using System.Web.Security;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class AdminReportGenerator : Page
    {
        //Standardise look and feel of the Website across all Web Browsers
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
                FormsAuthentication.RedirectToLoginPage();

          
        }

        protected override void InitializeCulture()
        {
            int lang = SessionWrapper.UserLanguage;

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(UtilityClass.GetLang(lang));
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(UtilityClass.GetLang(lang));
        }
    }
}