using System;
using System.Drawing;
using System.Web.UI;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using Common.Logging;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using System.Web.Security;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.account
{
    public partial class UserPasswordMaintainance : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserPasswordMaintainance));
        private readonly UserManagementService userMan = new UserManagementService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
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

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        protected void btnChangeAdminPass_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Text = "";
            string oldPassword = tbOldPassword.Text;
            string newPassword = tbNewPassword.Text;
            try
            {
                //if (HasInvalidChars(tbNewPassword.Text) || HasInvalidChars(tbConfirmPassword.Text) || HasInvalidChars(tbOldPassword.Text))
                //{
                //    lblErrorMessage.Text = GetLocalResourceObject("ValidationPasswordChars").ToString();
                //}
                //else
                //{ 
               //int PasswordMinlength = 0;
               // if (Cache["PasswordMinlength"] == null)
               // {
               //     int value;
               //     int.TryParse(Cache["PasswordMinlength"].ToString(), out value);
               //     PasswordMinlength = value;
               // }

                
                    string result;
                    if (userMan.UpdateUserPassword(null, oldPassword, newPassword, out result))
                    {
                        ClearAllFields();
                        lblInfoMessage.Text = GetLocalResourceObject("ConfirmUpdateSuccess").ToString();
                    }
                    else
                    {
                        lblErrorMessage.Text = result;
                    }

                //}
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

        private void ClearAllFields()
        {
            tbNewPassword.Text = "";
            tbConfirmPassword.Text = "";
            tbOldPassword.Text = "";
        }
    }
}