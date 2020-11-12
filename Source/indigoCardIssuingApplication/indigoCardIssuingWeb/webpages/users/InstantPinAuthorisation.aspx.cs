using System;
using System.Collections.Generic;
using System.Linq;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Text;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.users
{
    public partial class InstantPinAuthorisation : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InstantPinAuthorisation));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_CUSTODIAN, UserRole.PIN_OPERATOR };

        private readonly UserManagementService userMan = new UserManagementService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
        protected void btnSubmitPin_Click(object sender, EventArgs e)
        {
            string username = tbCustodianUsername.Text;
            string encryptedPin = tbCustodianAuthPin.Text;

            var userid = Convert.ToInt64(userMan.GetUserByUsername(username));

            if (!string.IsNullOrEmpty(userid.ToString()))
            {
                if (userMan.GetUserAuthorisationPin(userid, encryptedPin))
                {
                    Response.Redirect("~\\webpages\\cards\\CardView.aspx");
                }
                else
                {
                    lblErrorMessage.Text = "The Authorisation Pin captured is incorrect. Please try again.";
                }
            }
        }

        private void LoadPageData()
        {
            try
            {
                this.btnSubmitPin.Enabled = this.btnSubmitPin.Visible = false;


                if (SessionWrapper.CardSearchResultItem != null)
                {
                    var card = SessionWrapper.CardSearchResultItem;
                }
                else
                {
                    this.lblInfoMessage.Text = "Sorry there is no card information to display.";
                }

                if (Cache["custsettings"] != null)
                {
                    List<ConfigurationSettings> elements = (List<ConfigurationSettings>)Cache["custsettings"];
                    elements = elements.FindAll(i => i.PageName == "cardcapture").ToList();

                    ContentPlaceHolder myContent = (ContentPlaceHolder)this.Master.FindControl("MainContent");
                    HtmlGenericControl divcontrol = null;
                    foreach (var element in elements)
                    {
                        divcontrol = myContent.FindControl("div" + element.Key) as HtmlGenericControl;
                        if (divcontrol != null)
                        {
                            if (element.Visibility)
                            {
                                divcontrol.Attributes.Add("style", "display:block");

                            }
                            else
                            {
                                divcontrol.Attributes.Add("style", "display:none");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.pnlPinAuthorisation.Visible = false;
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }
    }
}