using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Security.Permissions;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.SearchParameters;
using System.Security.Principal;
using indigoCardIssuingWeb.service;

namespace indigoCardIssuingWeb
{
    /// <summary>
    /// Base class for commonly implemented items for pages.
    /// </summary>
    /// 
    public class BasePage : System.Web.UI.Page
    {
        protected const string DATETIME_ASPX_FORMAT = "{0:dd/MM/yyyy HH:mm:ss}";
        protected const string DATE_ASPX_FORMAT = "{0:dd/MM/yyyy}";
        protected const string DATETIME_FORMAT = "dd/MM/yyyy HH:mm:ss";
        protected const string DATE_FORMAT = "dd/MM/yyyy";
        protected const string DATEPICKER_FORMAT = "dd/mm/yy";
        protected const string LISTITEM_ALL_ID = "-99";

        private readonly UserManagementService userMan = new UserManagementService();

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.ServerVariables["http_user_agent"].IndexOf("Chrome", StringComparison.CurrentCultureIgnoreCase) != -1)
                Page.ClientTarget = "uplevel";
            if (Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) != -1)
                Page.ClientTarget = "uplevel";
        }

        protected override void InitializeCulture()
        {
            int lang = SessionWrapper.UserLanguage;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(UtilityClass.GetLang(lang));
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(UtilityClass.GetLang(lang));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
                FormsAuthentication.RedirectToLoginPage();
          // TO DO: working on this

            //DateTime ExpiryDate = new DateTime();
            //DateTime.TryParse(SessionWrapper.PasswordExpiryDate, out ExpiryDate);
            //if (DateTime.Compare(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.ExpiryDate, DateTime.Now) < 0)        
            //   Response.Redirect("~\\webpages\\account\\UserPasswordMaintainance.aspx", true);
        }

        
        /// <summary>
        /// Formats a list itme text as code - name.
        /// </summary>
        /// <param name="name">Name of item example: issuer name</param>
        /// <param name="code">Code of item example: issuer code</param>
        /// <param name="Id">a unique id for the item.</param>
        /// <returns></returns>
        protected ListItem FormatListItem(string name, string code, string Id)
        {
            return UtilityClass.FormatListItem(name, code, Id);
        }

        /// <summary>
        /// Formats Name and Code pairs, for example branch name and code.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected string FormatNameAndCode(string name, string code)
        {
            return UtilityClass.FormatNameAndCode(name, code);
        }

        /// <summary>
        /// Sets calue of textbox. If value is empty will set textbox as editable.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="value"></param>
        protected void SetTextBox(TextBox textBox, string value)
        {
            textBox.Text = value;

            if (String.IsNullOrWhiteSpace(value))
            {
                textBox.Enabled = true;
                textBox.ReadOnly = false;
            }
        }

        protected void SetAccountInputRestrictions(TextBox textBox)
        {
            bool accountHyphen = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AllowAccountHyphen"].ToString());

            if (accountHyphen)
            {
                textBox.Attributes.Add("onkeypress", "javascript:return isNumberKeyWithhyphen(event);");
            }
            else
            {
                textBox.Attributes.Add("onkeypress", "javascript:return isNumberKey(event);");
            }
        }

        protected bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}