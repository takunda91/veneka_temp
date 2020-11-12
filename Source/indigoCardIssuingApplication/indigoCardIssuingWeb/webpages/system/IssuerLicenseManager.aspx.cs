using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CCO;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class IssuerLicenseManager : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IssuerLicenseManager));
        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };
        private readonly SystemAdminService _systemAdmin = new SystemAdminService();

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadControls();
            }
        }
        protected void LoadControls()
        {
            try
            {
                var machineId = _systemAdmin.GetMachineId();
                this.tbMachineId.Text = machineId;

                if (SessionWrapper.IndigoComponentLicense != null)
                {
                    IndigoComponentLicense indigolicense = SessionWrapper.IndigoComponentLicense;

                    this.tbIssuerName.Text = indigolicense.IssuerName;
                    this.tbIssuerCode.Text = indigolicense.IssuerCode;
                    DateTime dt = new DateTime(0001, 1, 1);
                    if (indigolicense.IssueDate != dt)
                    {
                        this.tbIssueDate.Text = indigolicense.IssueDate.ToShortDateString();
                    }
                    if (indigolicense.ExpiryDate != dt)
                    {
                        this.tbExpiryDate.Text = indigolicense.ExpiryDate.ToShortDateString();
                    }

                    List<ListItem> binCodes = new List<ListItem>();

                    if (indigolicense.LicensedBinCodes != null)
                    {
                        foreach (var bin in indigolicense.LicensedBinCodes)
                        {
                            binCodes.Add(new ListItem(bin));
                        }

                        this.lbLicensedBins.Items.AddRange(binCodes.OrderBy(m => m.Value).ToArray());

                    }

                    SessionWrapper.IndigoComponentLicense = null;
                }
            }
            catch (Exception ex)
            {
                this.pnlDisable.Visible = false;
                this.pnlButtons.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ADMINISTRATOR")]
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            try
            {
                if (this.fuUploadLicense.HasFile)
                {
                    if (Path.GetExtension(this.fuUploadLicense.FileName).ToUpper().Equals(".XML"))
                    {
                        IndigoComponentLicense licenseInfo;
                        string messages;

                        if (_systemAdmin.LoadIssuerLicenseFile(this.fuUploadLicense.FileBytes, out licenseInfo, out messages))
                        {

                            this.lblInfoMessage.Text = "License loaded successfully.";

                            this.tbIssuerName.Text = licenseInfo.IssuerName;
                            this.tbIssuerCode.Text = licenseInfo.IssuerCode;
                            this.tbIssueDate.Text = licenseInfo.IssueDate.ToShortDateString();
                            this.tbExpiryDate.Text = licenseInfo.ExpiryDate.ToShortDateString();

                            List<ListItem> binCodes = new List<ListItem>();

                            foreach (var bin in licenseInfo.LicensedBinCodes)
                            {
                                binCodes.Add(new ListItem(bin));
                            }

                            this.lbLicensedBins.Items.AddRange(binCodes.OrderBy(m => m.Value).ToArray());
                        }
                        else
                        {
                            this.lblErrorMessage.Text = messages;
                        }
                    }
                    else
                    {
                        this.lblErrorMessage.Text = "Incorrect file type uploaded, license file must be xml";
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblErrorMessage.Text = ex.ToString();
            }
        }       
    }
}