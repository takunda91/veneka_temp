using System;
using System.Drawing;
using System.Web.UI;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using Common.Logging;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CardIssuanceService;
using System.Web.Security;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.account
{
    public partial class UserPinAuthorisationMaintainance : BasePage
    {
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_CUSTODIAN };
        private static readonly ILog log = LogManager.GetLogger(typeof(UserPinAuthorisationMaintainance));
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

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        protected void btnChangeAdminPin_Click(object sender, EventArgs e)
        {
            string newPin = tbNewPin.Text;
            try
            {
                string result;
                if (userMan.UpdateUserAuthorisationPin(null, newPin, out result))
                {
                    ClearAllFields();
                    lblInfoMessage.Text = "Authorisation Pin created successfully.";//GetLocalResourceObject("ConfirmUpdateSuccess").ToString();
                }
                else
                {
                    lblErrorMessage.Text = result;
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

        private void ClearAllFields()
        {
            tbNewPin.Text = "";
            tbConfirmPin.Text = "";
        }
    }
}