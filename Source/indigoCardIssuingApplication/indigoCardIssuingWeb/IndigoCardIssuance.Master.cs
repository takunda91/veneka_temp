using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Security.Principal;
using System.Configuration;
using indigoCardIssuingWeb.Old_App_Code.security;
using indigoCardIssuingWeb.utility;

namespace indigoCardIssuingWeb
{
    public partial class IndigoCardIssuance : MasterPage
    {
        //private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        //private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        //private string _antiXsrfTokenValue;

        private static readonly ILog log = LogManager.GetLogger(typeof(IndigoCardIssuance));
        private readonly SystemAdminService _systemAdmin = new SystemAdminService();

        private MenuContainer menuContainer = new MenuContainer();
        //private static bool _logoLoaded = false;
        //private static string _logoSrc = null;
        private static readonly object _lockObject = new object();

        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;

        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                //log.Trace(t=>t("Validating Anti-XSRF token."));
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    log.Warn("Validation of Anti-XSRF token failed.");
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }

            }
        }


        //protected void Page_PreInit(object sender, EventArgs e)
        //{
        //    if (Request.ServerVariables["http_user_agent"].IndexOf("Chrome", StringComparison.CurrentCultureIgnoreCase) != -1)
        //        Page.ClientTarget = "uplevel";
        //    if (Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) != -1)
        //        Page.ClientTarget = "uplevel";

        //    //First, check for the existence of the Anti-XSS cookie
        //    var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        //    Guid requestCookieGuidValue;

        //    //If the CSRF cookie is found, parse the token from the cookie.
        //    //Then, set the global page variable and view state user
        //    //key. The global variable will be used to validate that it matches in the view state form field in the Page.PreLoad
        //    //method.
        //    if (requestCookie != null
        //    && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        //    {
        //        //Set the global token variable so the cookie value can be
        //        //validated against the value in the view state form field in
        //        //the Page.PreLoad method.
        //        _antiXsrfTokenValue = requestCookie.Value;

        //        //Set the view state user key, which will be validated by the
        //        //framework during each request
        //        Page.ViewStateUserKey = _antiXsrfTokenValue;
        //    }
        //    //If the CSRF cookie is not found, then this is a new session.
        //    else
        //    {
        //        //Generate a new Anti-XSRF token
        //        _antiXsrfTokenValue = Guid.NewGuid().ToString("N");

        //        //Set the view state user key, which will be validated by the
        //        //framework during each request
        //        Page.ViewStateUserKey = _antiXsrfTokenValue;

        //        //Create the non-persistent CSRF cookie
        //        var responseCookie = new HttpCookie(AntiXsrfTokenKey)
        //        {
        //            //Set the HttpOnly property to prevent the cookie from
        //            //being accessed by client side script
        //            HttpOnly = true,

        //            //Add the Anti-XSRF token to the cookie value
        //            Value = _antiXsrfTokenValue
        //        };

        //        //If we are using SSL, the cookie should be set to secure to
        //        //prevent it from being sent over HTTP connections
        //        if (FormsAuthentication.RequireSSL &&
        //        Request.IsSecureConnection)
        //            responseCookie.Secure = true;

        //        //Add the CSRF cookie to the response
        //        Response.Cookies.Set(responseCookie);
        //    }

        //    Page.PreLoad += master_Page_PreLoad;
        //}

        //protected void master_Page_PreLoad(object sender, EventArgs e)
        //{
        //    //During the initial page load, add the Anti-XSRF token and user
        //    //name to the ViewState
        //    if (!IsPostBack)
        //    {
        //        //Set Anti-XSRF token
        //        ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;

        //        //If a user name is assigned, set the user name
        //        ViewState[AntiXsrfUserNameKey] =
        //        Context.User.Identity.Name ?? String.Empty;
        //    }
        //    //During all subsequent post backs to the page, the token value from
        //    //the cookie should be validated against the token in the view state
        //    //form field. Additionally user name should be compared to the
        //    //authenticated users name
        //    else
        //    {
        //        //Validate the Anti-XSRF token
        //        if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
        //        || (string)ViewState[AntiXsrfUserNameKey] !=
        //        (Context.User.Identity.Name ?? String.Empty))
        //        {
        //            throw new InvalidOperationException("Validation of Anti - XSRF token failed.");
        //        }
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                //Response.Cache.SetNoStore();
                //Response.Cache.SetNoServerCaching();
                //Response.AddHeader("Pragma", "No-Cache");

                Response.AddHeader("x-permitted-cross-domain-policies", "none");

                //string selectedLang = Request.Form["ctl00$cphLogin$ddlMasterLangueges"];
                //if (selectedLang != null)
                //{
                //    Thread.CurrentThread.CurrentUICulture = new CultureInfo(UtilityClass.GetLang(int.Parse(selectedLang)));
                //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(UtilityClass.GetLang(int.Parse(selectedLang)));
                //}

                if (String.IsNullOrWhiteSpace(SessionWrapper.ClientAddress))
                {
                    try
                    {
                        //First check HTTP_X_FORWARDED_FOR
                        string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                        if (log.IsDebugEnabled)
                            log.Debug("HTTP_X_FORWARDED_FOR=" + (ipAddress ?? "empty"));

                        if (String.IsNullOrWhiteSpace(ipAddress))
                            ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

                        if (String.IsNullOrWhiteSpace(ipAddress))
                            throw new Exception("IP Address of remote host uknown.");
                        else if (log.IsDebugEnabled)
                            log.Debug("REMOTE_HOST=" + (ipAddress ?? "empty"));

                        try
                        {
                            SessionWrapper.ClientAddress = Dns.GetHostEntry(ipAddress).HostName;
                        }
                        catch(System.Net.Sockets.SocketException sex)
                        {
                            log.Warn(sex);
                            SessionWrapper.ClientAddress = ipAddress;
                        }
                        //LoadLanguageSettings();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                //if (!IsPostBack)
                //{
                    menuContainer.ClearMenu();
                //}

                if (Request.IsAuthenticated)
                {
                    //if (Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name) != null && Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name).Count > 0 && tt == "")
                    if (Roles.GetRolesForUser(HttpContext.Current.User.Identity.Name) != null)
                    {
                        //PopulateUserMenu(Roles.Provider.ToIndigoRoleProvider().GetRolesDictForUser(User.Identity.Name), SessionWrapper.LdapUser, SessionWrapper.StatusFlow);
                        var _listroles = ((IndigoRoleProvider)Roles.Provider).GetRolesDictForUser(HttpContext.Current.User.Identity.Name);
                        PopulateUserMenu(_listroles, 
                                            HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.IsDomainUser,
                                            ((IndigoRoleProvider)Roles.Provider).GetStatusFlowRole(HttpContext.Current.User.Identity.Name));

                        List<IndigoComponentLicense> Licenses = _systemAdmin.GetLicenseListIssuers(null);
                        lblnotificationtext.Text="";
                        if (Licenses != null)
                        {
                            int configdays=int.Parse(ConfigurationManager.AppSettings["LicenseNotificationDur(indays)"].ToString());
                            Licenses = Licenses.FindAll(i=>i.ExpiryDate <=DateTime.Now.AddDays(-configdays));
                            if (_listroles != null && _listroles.Count > 0 && Licenses != null)
                            {
                                foreach (var role in _listroles)
                                {
                                    if(role.Key==UserRole.CARD_CENTRE_PIN_OFFICER 
                                        || role.Key==UserRole.CENTER_MANAGER|| role.Key==UserRole.CENTER_OPERATOR || role.Key==UserRole.CMS_OPERATOR
                                        || role.Key==UserRole.USER_ADMIN || role.Key == UserRole.USER_AUDIT)
                                    foreach (var issuer in role.Value)
                                    {
                                      var result=  Licenses.FindAll(i=>i.IssuerName==issuer.issuer_name);
                                        if(result!=null&& result.Count>0&& issuer.issuer_id!=-1)
                                        {
                                            lblnotificationtext.Text= "license is about to expiry in the "+(result[0].ExpiryDate-DateTime.Now).Days.ToString()+" for the "+ issuer.issuer_name;
                                                break;
                                            }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {                        
                        FormsAuthentication.SignOut();
                        HttpContext.Current.User =
                            new GenericPrincipal(new GenericIdentity(string.Empty), null);
                        FormsAuthentication.RedirectToLoginPage();
                    }
                }
            }
            catch(Exception ex)
            {
                log.Fatal(ex);
                throw;
            }
        }

        protected void LoginStatus1_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            try
            {
                menuContainer.ClearMenu();
                UserManagementService userMan = new UserManagementService();
                userMan.LogOut();                
            }
            catch (Exception ex)
            {
                log.Error(ex);                
            }
            finally
            {
                Roles.Provider.ToIndigoRoleProvider().ClearRoles(HttpContext.Current.User.Identity.Name);
                Session.Abandon();
                FormsAuthentication.SignOut();
                FormsAuthentication.RedirectToLoginPage();
                

                //HttpCookie myCookie = new HttpCookie("ASP.NET_SessionId");
                //myCookie.Expires = DateTime.Now.AddDays(-1d);
                //myCookie.HttpOnly = true;
                //myCookie.Secure = true;
                //Response.Cookies.Add(myCookie);
            }
        }

        protected void PopulateUserMenu(Dictionary<UserRole, List<RolesIssuerResult>> userRoles, bool ldapUser, List<StatusFlowRole> statusFlow)
        {
            if(lgLogInView.FindControl("mnNavigationMenu") == null)
                return;

            ((Menu)lgLogInView.FindControl("mnNavigationMenu")).Items.Clear();
            //mnNavigationMenu.Items.Clear();
            var mainMenus = menuContainer.GetMenuForRoles(userRoles, ldapUser, statusFlow);

            if (mainMenus != null)
            {
                foreach (DisplayMenu menu in mainMenus)
                {
                    MenuItem item = ConstructMenuItem(menu);
                    if (menu.HasSubmenus())
                    {
                        item.Selectable = false;
                        foreach (DisplayMenu subMen in menu.GetSubmenus())
                        {
                            item.ChildItems.Add(ConstructMenuItem(subMen));
                        }
                    }
                    ((Menu)lgLogInView.FindControl("mnNavigationMenu")).Items.Add(item);
                    //mnNavigationMenu.Items.Add(item);
                }
            }
        }

        protected MenuItem ConstructMenuItem(DisplayMenu disMenu)
        {
            var item = new MenuItem(disMenu.MenuName, disMenu.MenuValue);
            item.NavigateUrl = disMenu.NavigateUrl;
            item.Selectable = disMenu.Selectable;
            
            return item;
        }

        //private void LoadLanguageSettings()
        //{
        //    List<ListItem> languages = new List<ListItem>();
        //    foreach (var lang in Enum.GetValues(typeof(IndigoLanguages)))
        //    {
        //        languages.Add(new ListItem
        //        {
        //            Text = lang.ToString(),
        //            Value = (((int)lang) + 1).ToString()
        //        });
        //    }// end foreach (var lang in Enum.GetValues(typeof(IndigoLanguages)))
        //    ddlMasterLangueges.Items.AddRange(languages.ToArray());
        //}

        //protected override void InitializeCulture()
        //{

        //    string selectedLang = Request.Form["ctl00$cphLogin$ddlMasterLangueges"];
        //    if (selectedLang != null)
        //    {
        //        Thread.CurrentThread.CurrentUICulture = new CultureInfo(UtilityClass.GetLang(int.Parse(selectedLang)));
        //        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(UtilityClass.GetLang(int.Parse(selectedLang)));
        //    }
        //}
    }
}