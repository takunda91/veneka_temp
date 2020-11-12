using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CCO;
using Common.Logging;
using System.Web.Security;
using System.Security.Principal;

namespace indigoCardIssuingWeb
{
    public partial class Logout : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logout));
        private readonly UserManagementService userMan = new UserManagementService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            try
            {
                log.Debug(m => m("Auto logging out user " + User.Identity.Name ?? "Unkown"));
                try
                {
                    userMan.LogOut();
                }
                catch(Exception ex)
                {
                    log.Error(ex);
                }

                Roles.Provider.ToIndigoRoleProvider().ClearRoles(User.Identity.Name);
                FormsAuthentication.SignOut();
                HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

                SessionWrapper.ClearSession();
                Session.Abandon();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                FormsAuthentication.RedirectToLoginPage();
            }
        }
    }
}