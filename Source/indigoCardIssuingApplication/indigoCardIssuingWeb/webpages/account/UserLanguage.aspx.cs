using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.account
{
    public partial class UserLanguage : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserLanguage));
        private readonly UserManagementService userMan = new UserManagementService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadLanguageSettings();
            }            
        }

        private void LoadLanguageSettings()
        {
            try
            {
                List<ListItem> languages = new List<ListItem>();
                foreach (var lang in userMan.GetLanguages())
                {
                    string text = "";
                    string id = lang.id.ToString();
                    switch (SessionWrapper.UserLanguage)
                    {
                        case 0:
                            text = lang.language_name;
                            break;
                        case 1:
                            text = lang.language_name_fr;
                            break;
                        case 2:
                            text = lang.language_name_pt;
                            break;
                        case 3:
                            text = lang.language_name_sp;
                            break;
                        default:
                            text = lang.language_name;
                            break;
                    }


                    languages.Add(new ListItem
                    {
                        Text = text,
                        Value = id
                    });
                }// end foreach (var lang in Enum.GetValues(typeof(IndigoLanguages)))
                ddlMasterLangueges.Items.Clear();
                ddlMasterLangueges.Items.AddRange(languages.OrderBy(m => m.Text).ToArray());
                ddlMasterLangueges.SelectedValue = SessionWrapper.UserLanguage.ToString();
            }
            catch (Exception ex)
            {
                //this.pnlDisable.Visible = false;
                //this.pnlButtons.Visible = false;
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsTraceEnabled || log.IsDebugEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        protected void btnChangeLang_Click(object sender, EventArgs e)
        {
            try
            {
              
                lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                this.ddlMasterLangueges.Enabled = false;
                this.btnChangeLang.Visible = false;
                this.btnConfirm.Visible = true;
                this.btnCancel.Visible = true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            if (userMan.UpdateUserLanguage(int.Parse(this.ddlMasterLangueges.SelectedValue)))
            {
                SessionWrapper.UserLanguage = int.Parse(this.ddlMasterLangueges.SelectedValue);

                Thread.CurrentThread.CurrentUICulture = new CultureInfo(UtilityClass.GetLang(int.Parse(this.ddlMasterLangueges.SelectedValue)));
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(UtilityClass.GetLang(int.Parse(this.ddlMasterLangueges.SelectedValue)));

                lblInfoMessage.Text = GetLocalResourceObject("ConfirmChangeSuccess").ToString();

                this.btnChangeLang.Visible = true;
                this.btnConfirm.Visible = false;
                this.btnCancel.Visible = false;
                this.ddlMasterLangueges.Enabled = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RefreshPage", "Refresh()",
true);
                LoadLanguageSettings();
               // Page.RegisterRequiresPostBack(this.Page);
            }
            else
            {
                lblErrorMessage.Text = GetLocalResourceObject("ConfirmChangeFailed").ToString();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            this.ddlMasterLangueges.Enabled = true;
            ddlMasterLangueges.SelectedValue = SessionWrapper.UserLanguage.ToString();

            this.btnChangeLang.Visible = true;
            this.btnConfirm.Visible = false;
            this.btnCancel.Visible = false;
        }

    }
}