using System;
using System.Net.Security;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.utility;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using indigoCardIssuingWeb.CardIssuanceService;
using System.Web.Caching;
using System.Xml.Linq;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using indigoCardIssuingWeb.CCO.constants;
using System.Web;
using System.Security.Principal;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.Old_App_Code.security;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Runtime.CompilerServices;

namespace indigoCardIssuingWeb
{
    public partial class Default : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Default));
        static UserManagementService userMan = new UserManagementService();

        protected override void InitializeCulture()
        {
            string selectedLang = Request.Form["ctl00$cphLogin$ddlMasterLangueges"];
            if (selectedLang != null)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(UtilityClass.GetLang(int.Parse(selectedLang)));
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(UtilityClass.GetLang(int.Parse(selectedLang)));
            }
        }



        protected new void Page_Load(object sender, EventArgs e)
        {

            //if (!String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            //    Response.Redirect("Default.aspx");



            if (Request.IsAuthenticated && !String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            {
                // This is an unauthorized, authenticated request...
                if (log.IsWarnEnabled)
                    log.WarnFormat("Unauthorized access to page {1} by user {0}.", HttpContext.Current.User.Identity.Name, Request.QueryString["ReturnUrl"]);

                Response.Redirect("~/UnauthorizedAccess.aspx");
            }
            else if (!String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                Response.Redirect("Default.aspx");


        }

        public static bool IgnoreCertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            try
            {
                //SetLoginView(false);

                //short term solution to sql injection, ideally get time
                //to develop a regular expression which is much more readable
                //if (HasInvalidChars(Login1.UserName) || HasInvalidChars(Login1.Password))
                //{

                //    Login1.FailureText = "Please use only valid characters in username and password";
                //    return;
                //}

                //Membership.ValidatingPassword += Membership_ValidatingPassword;

                //Allow funny characters for the user.
                string userName = Login1.UserName; //Server.HtmlEncode(Login1.UserName);
                string password = Login1.Password;//Server.HtmlEncode(Login1.Password);
                int passwordMinlength = 0, passwordExpiresDays = 0, PasswordAttemptLockoutDuration = 0, maxInvalidPasswordAttempts = 0;


                if (String.IsNullOrWhiteSpace(userName) && String.IsNullOrWhiteSpace(password))
                {
                    Login1.FailureText = "Please enter your username and password";
                }
                else if (String.IsNullOrWhiteSpace(password))
                {
                    Login1.FailureText = "Please enter your password";
                }
                else if (String.IsNullOrWhiteSpace(userName))
                {
                    Login1.FailureText = "Please enter your username";
                }
                else if (String.IsNullOrWhiteSpace(SessionWrapper.ClientAddress))
                {
                    Login1.FailureText = "Unknown Remote Host";
                }
                else if (Membership.ValidateUser(userName.ToLower(), password))
                {
                    var indigoUser = User.ToIndigoPrincipal();

                    lblname.Text = userName;
                    lblsessionkey.Text = indigoUser.IndigoIdentity.SessionKey;
                    lblauthId.Text = indigoUser.IndigoIdentity.AuthConfigId.ToString();
                    //Clear out any previous roles
                    Roles.Provider.ToIndigoRoleProvider().ClearRoles(userName);

                    //MembershipUser u = Membership.GetUser(Login1.UserName, false);                    

                    //IndigoMembershipUser m = (IndigoMembershipUser)u;
                    //userId = m.UserId;
                    if (Cache["PasswordMinlength"] == null)
                    {

                        var usersettings = userMan.GetUseradminSettings();
                        if (usersettings != null)
                        {
                            passwordMinlength = (int)usersettings.PasswordMinLength;
                            passwordExpiresDays = (int)usersettings.PasswordValidPeriod;
                            maxInvalidPasswordAttempts = (int)usersettings.maxInvalidPasswordAttempts;
                            PasswordAttemptLockoutDuration = (int)usersettings.PasswordAttemptLockoutDuration;

                        }
                        Cache.Insert("PasswordMinlength", passwordMinlength.ToString());
                        Cache.Insert("passwordExpiresDays", passwordExpiresDays.ToString());
                        Cache.Insert("maxInvalidPasswordAttempts", maxInvalidPasswordAttempts.ToString());
                        Cache.Insert("PasswordAttemptLockoutDuration", PasswordAttemptLockoutDuration.ToString());
                    }
                    else
                    {
                        int.TryParse(Cache["passwordExpiresDays"].ToString(), out passwordExpiresDays);
                        int.TryParse(Cache["PasswordMinlength"].ToString(), out passwordMinlength);


                    }
                    ReadCacheValues();

                    //if (u.LastPasswordChangedDate.AddDays((int)passwordExpiresDays) < DateTime.Now && userId > 0)
                    if (indigoUser.IndigoIdentity.MustChangePassword)
                    {
                        userId = indigoUser.IndigoIdentity.UserId;
                        revPassword.ValidationExpression = @"^.*(?=.{" + passwordMinlength + @",})(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z0-9!@#$%~`&+-=_*]+$";

                        lblMessage.Text = "Your password has expired. Please change your password to a new value.";
                        lblUserName.Text = Login1.UserName;
                        ChangePasswordPanel.Visible = true;
                        Login1.Visible = false;
                    }
                    else if (indigoUser.IndigoIdentity.ISMultiFactorenabled)
                    {

                        lblInfoverificationmessage.Text = "Please enter verification code.";
                        pnlVerification.Visible = true;
                        Login1.Visible = false;
                    }
                    else
                    {

                        ReadCacheValues();
                        Response.SetAuthCookie(userName, false, HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey);

                        Response.Redirect("~/Default.aspx", false);
                    }

                    //FormsAuthentication.RedirectFromLoginPage(userName, false);
                }
                else
                {

                    //if (mLoginAttempt < maxInvalidPasswordAttempts)
                    //{
                    //    mLoginAttempt = mLoginAttempt + 1;
                    //    if (mLoginAttempt == maxInvalidPasswordAttempts)
                    //    {
                    //        Login1.FailureText = "Lock Warning";
                    //    }
                    //    else
                    //    {
                    //        Login1.FailureText = "Account Locked";
                    //    }
                    //}


                    Login1.FailureText += "Username and/or Password incorrect. Please try again";
                    e.Authenticated = false;
                }
            }
            catch (Exception ex)
            {
                //no IIS error logging is coming because tracing and debugging has been disabled in IIS web tools
                //LogFileWriter.WriteException(ToString(), ex);
                log.Error(ex);

                //Login1.FailureText = "NETWORK / APPLICATION ERROR PLEASE TRY AGAIN.<br/>IF ERROR PERSISTS INFORM I.T DEPT. SORRY FOR THE INCONVENIENCE CAUSED BY THIS.";
                //if (log.IsDebugEnabled || log.IsTraceEnabled)
                //{
                Login1.FailureText = ex.Message;
                //}

                //Login1.FailureText = "Debug version - <br/>" + ex.Message + " <br/>" +ex.StackTrace;
                //Login1.FailureText = "Username or Password incorrect. Please try again";
                e.Authenticated = false;
            }
        }
        public void ChangePassword_OnClick(object sender, EventArgs args)
        {
            // Update the password.

            //MembershipUser u = Membership.GetUser(Login1.UserName, true);

            //if (u.ChangePassword(tbOldPassword.Text, tbNewPassword.Text))
            string result;
            if (userMan.UpdateUserPassword(userId, tbOldPassword.Text, tbNewPassword.Text, out result))
            {
                lblMessage.Text = "Password changed.";
                ChangePasswordPanel.Visible = false;
                Login1.Visible = true;


            }
            else
            {
                lblMessage.Text = result;
            }
        }
        /// <summary>
        /// Read XML for card capture screen controls.
        /// </summary>
        private void ReadCacheValues()
        {
            #region "Reading Xml and Cacheing"
            if (Cache["custsettings"] == null)
            {
                List<ConfigurationSettings> objlist = new List<ConfigurationSettings>();
                ConfigurationSettings config = null;

                XElement allData = XElement.Load(Server.MapPath(ApplicationUsedPath.ConfigurationSettings));
                if (allData != null)
                {
                    IEnumerable<XElement> Elements = allData.Descendants("Page");
                    foreach (XElement element in Elements)
                    {
                        XName url = XName.Get("column");

                        foreach (var Element in Elements.Elements(url))
                        {
                            config = new ConfigurationSettings();
                            config.PageName = element.Attribute("Name").Value;
                            config.Key = Element.Attribute("key").Value;
                            config.Visibility = Boolean.Parse(Element.Attribute("visible").Value);
                            config.ColumnName = Element.Attribute("name").Value;
                            config.Length = int.Parse(Element.Attribute("length").Value);
                            config.Required = Boolean.Parse(Element.Attribute("required").Value);
                            config.Values = Element.Attribute("values").Value;
                            config.type = Element.Attribute("type").Value;
                            objlist.Add(config);
                            
                        }
                    }
                }


                Cache.Insert("custsettings", objlist, new
                        System.Web.Caching.CacheDependency(Server.MapPath(ApplicationUsedPath.ConfigurationSettings)));
            }
            #endregion
        }
        private long userId
        {
            get
            {
                if (ViewState["userId"] == null)
                    return 0;
                else
                    return (long)ViewState["userId"];
            }

            set
            {
                ViewState["userId"] = value;
            }
        }
        private int? mLoginAttempt
        {
            get
            {
                if (ViewState["mLoginAttempt"] == null)
                    return 0;
                else
                    return (int?)ViewState["mLoginAttempt"];
            }

            set
            {
                ViewState["mloginAttempts"] = value;
            }
        }



        private bool HasInvalidChars(String text)
        {
            var inValidChars = new[]
                {
                    '!', '@', '#', '$', '%','-',
                    '^', '&', '*', '(', ')', '\'', '\"', '{', '}', '<', '>',
                    '/', '\\', ':', ';', '[', '}'
                };


            foreach (char c in inValidChars)
            {
                if (text.Contains(c.ToString()))
                    return true;
            }
            return false;
        }

        [WebMethod]

        public static string Resend(string authId, string name)
        {
            string responseMessage;
            try
            {

                userMan.SendChallenge(int.Parse(authId), name, out responseMessage);


                return responseMessage;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.StatusCode = 500;
                throw new HttpException(500, ex.Message);
            }

        }
        [WebMethod]
        public static string Verify(string Code, string authId, string name, string sessionKey)
        {
            string responseMessage;
            log.Info("Test Call");

            try
            {
                
                log.InfoFormat("Inside the verify method with code : {0}", Code);
                if (userMan.VerifyChallenge(int.Parse(authId), name, Code, out responseMessage))

                {
                    HttpContext.Current.Response.SetAuthCookie(name, false, sessionKey);

                    //HttpContext.Current.Response.Redirect("~/Default.aspx", false);

                    return "SUCCESS";

                }


                return responseMessage;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                //HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                //HttpContext.Current.Response.Clear();
                //HttpContext.Current.Response.StatusCode = 500;
                //throw new HttpException(500, ex.Message);
                return ex.Message;

            }

        }

        public void VerifyAuthCode(object sender, EventArgs args)
        {
            string responseMessage;
           // log.Info("Test Call");

            try
            {
                var Code = this.tbCode.Text;
                var authId = this.lblauthId.Text;
                var name = this.lblname.Text;
                var sessionKey = lblsessionkey.Text;

              //  log.InfoFormat("Inside the verify method with code : {0}", Code);
                if (userMan.VerifyChallenge(int.Parse(authId), name, Code, out responseMessage))

                {
                    HttpContext.Current.Response.SetAuthCookie(name, false, sessionKey);

                    //HttpContext.Current.Response.Redirect("~/Default.aspx", false);

                    responseMessage = "SUCCESS";
                    Response.Redirect("~/HomePage.aspx");


                }


                log.Info(responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                //HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                //HttpContext.Current.Response.Clear();
                //HttpContext.Current.Response.StatusCode = 500;
                //throw new HttpException(500, ex.Message);
               // return ex.Message;

            }
        }



        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {


                pnlVerification.Visible = false;
                Login1.Visible = true;
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

            }
        }
    }
}