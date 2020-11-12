using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Common.Logging;
using indigoCardIssuingWeb.Old_App_Code.security;
using NWebsec.Csp;

namespace indigoCardIssuingWeb
{
    public class Global : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Global));

        protected void Application_Start(object sender, EventArgs e)
        {
            if(System.Web.Configuration.WebConfigurationManager.AppSettings["InstrumentationKey"]!=null)
            Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = System.Web.Configuration.WebConfigurationManager.AppSettings["InstrumentationKey"];

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //Logic added so that eProcess security passes
            var app = sender as HttpApplication;
            bool xssProtect = false, crossDomain = false, securityPolicy = false;

            if (app != null && app.Context != null)
            {
                if (app.Context.Request.Url.AbsolutePath.ToLower().EndsWith(".js"))
                {
                    foreach (string header in app.Context.Response.Headers.AllKeys)
                    {
                        if (header.Equals("X-XSS-Protection", StringComparison.OrdinalIgnoreCase))
                            xssProtect = true;

                        if (header.Equals("x-permitted-cross-domain-policies", StringComparison.OrdinalIgnoreCase))
                            crossDomain = true;

                        if (header.Equals("content-security-policy", StringComparison.OrdinalIgnoreCase))
                            securityPolicy = true;
                    }

                    if (!xssProtect)
                        app.Context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                    if (!crossDomain)
                        app.Context.Response.Headers.Add("x-permitted-cross-domain-policies", "none");
                    if (!securityPolicy)
                        app.Context.Response.Headers.Add("content-security-policy", "default-src 'self';script-src 'self' 'unsafe-inline' 'unsafe-eval';object-src 'self';style-src 'self' 'unsafe-inline'; img - src 'self'; media - src 'self'; frame - src 'self'; font - src 'self'; connect - src 'self'; frame - ancestors 'self'; report - uri / WebResource.axd ? cspReport = true");
                }
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //// look if any security information exists for this request
            //if (HttpContext.Current.User != null)
            //{
            //    // see if this user is authenticated, any authenticated cookie (ticket) exists for this user
            //    if (HttpContext.Current.User.Identity.IsAuthenticated)
            //    {
            //        // see if the authentication is done using FormsAuthentication
            //        if (HttpContext.Current.User.Identity is FormsIdentity)
            //        {
            //            // Get the roles stored for this request from the ticket
            //            // get the identity of the user
            //            FormsIdentity identity = (FormsIdentity)HttpContext.Current.User.Identity;

            //            // get the forms authetication ticket of the user
            //            FormsAuthenticationTicket ticket = identity.Ticket;

            //            // get the roles stored as UserData into the ticket
            //            string[] roles = ticket.UserData.Split(',');

            //            // create generic principal and assign it to the current request
            //            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(identity, roles);
            //        }
            //    }
            //}
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (Request.IsAuthenticated)
                {
                    var identity = new IndigoIdentity(HttpContext.Current.User.Identity, authTicket.UserData);
                    var principal = new IndigoPrincipal(identity);
                    HttpContext.Current.User = principal;
                }
            }
        }

        protected void NWebsecHttpHeaderSecurityModule_CspViolationReported(object sender, CspViolationReportEventArgs e)
        {
            log.Warn(e.ViolationReport);
            //var report = e.ViolationReport;
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception err = Server.GetLastError();
                if (err is System.Security.SecurityException)
                {                    
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Unauthorized access by user {0}.", err, HttpContext.Current.User.Identity.Name);
                    Response.Redirect("~/UnauthorizedAccess.aspx");
                }
                else if (err is HttpException)
                {
                    // Get the error details
                    HttpException lastErrorWrapper = err as HttpException;
                    Exception lastError = lastErrorWrapper;
                    log.Error(lastError);
                }
                else
                {
                    log.Error(err);
                }                
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}