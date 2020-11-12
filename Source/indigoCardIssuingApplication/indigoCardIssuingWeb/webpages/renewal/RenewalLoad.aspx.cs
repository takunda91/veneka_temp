using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.renewal
{
    public partial class RenewalLoad : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RenewalLoad));
        private readonly RenewalService _cardRenewalService = new RenewalService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly BatchManagementService _batchService = new BatchManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR, UserRole.BRANCH_PRODUCT_OPERATOR };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        public Dictionary<int, RolesIssuerResult> IssuersList
        {
            get
            {
                if (ViewState["CardRenewalIssuersList"] != null)
                    return (Dictionary<int, RolesIssuerResult>)ViewState["CardRenewalIssuersList"];
                else
                    return new Dictionary<int, RolesIssuerResult>();
            }
            set
            {
                ViewState["CardRenewalIssuersList"] = value;
            }
        }

        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuerListItems = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuersList = new Dictionary<int, RolesIssuerResult>();
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerListItems, out issuersList);


                if (issuerListItems.ContainsKey(-1))
                {
                    issuerListItems.Remove(-1);
                    issuersList.Remove(-1);
                }

                this.ddlIssuer.Items.AddRange(issuerListItems.Values.OrderBy(m => m.Text).ToArray());


                IssuersList = issuersList; //save to ViewState property.
            }
            catch (Exception ex)
            {
                this.pnlButtons.Visible = false;

                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;

                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            btnConfirm.Visible = false;
            btnReject.Visible = false;

            if (fileRenewal.HasFile)
            {
                try
                {
                    if (fileRenewal.PostedFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        string filename = Path.GetFileName(fileRenewal.FileName);
                        //fileRenewal.SaveAs(Server.MapPath("~/") + filename);

                        // Get the length of the file.
                        int fileLen = fileRenewal.PostedFile.ContentLength;

                        // Display the length of the file in a label.
                        lblInfoMessage.Text = "The length of the file is " + fileLen.ToString() + " bytes.";

                        // Create a byte array to hold the contents of the file.
                        byte[] input = new byte[fileLen - 1];
                        input = fileRenewal.FileBytes;
                        RenewalFileSummary renewalCardSummary = null;
                        string messages;
                        int issuerId;
                        if (int.TryParse(ddlIssuer.SelectedValue, out issuerId))
                        {
                            if (_cardRenewalService.ExtractCardFile(issuerId, input, filename, out renewalCardSummary, out messages))
                            {
                                tbBranchCount.Text = renewalCardSummary.BranchCount.ToString();
                                tbNumberOfCards.Text = renewalCardSummary.CardCount.ToString();
                                hidRenewalId.Value = renewalCardSummary.RenewalId.ToString();
                                btnConfirm.Visible = true;
                                btnReject.Visible = true;
                                lblInfoMessage.Text = "Confirm you want to keep uploaded file?";
                            }
                            else
                            {
                                lblErrorMessage.Text = messages;
                            }
                        }
                        else
                        {
                            lblErrorMessage.Text = "Please select an issuer.";
                        }
                    }
                    else
                        lblErrorMessage.Text = "Upload status: Only Excel files are accepted!";
                }
                catch (Exception ex)
                {
                    lblErrorMessage.Text = "Upload status: The file could not be uploaded. The following error occured: " + ex.Message;
                }
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            btnReject.Visible = false;
            btnUpload.Visible = false;
            try
            {
                long cardRenewalId = 0;
                if (long.TryParse(hidRenewalId.Value, out cardRenewalId))
                {
                    RenewalFileSummary renewalCardSummary = null;
                    string messages;

                    if (_cardRenewalService.RejectLoad(cardRenewalId, out renewalCardSummary, out messages))
                    {
                        lblInfoMessage.Text = "File upload rejects.";
                    }
                    else
                    {
                        lblErrorMessage.Text = "File upload failed to reject.";
                        btnReject.Visible = true;
                        btnUpload.Visible = true;
                    }
                }
                else
                {
                    btnReject.Visible = true;
                    btnUpload.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "Upload status: The file could rejected. The following error occured: " + ex.Message;
                btnReject.Visible = true;
                btnUpload.Visible = true;
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            btnReject.Visible = false;
            btnUpload.Visible = false;
            try
            {
                long cardRenewalId = 0;
                if (long.TryParse(hidRenewalId.Value, out cardRenewalId))
                {
                    RenewalFileSummary renewalCardSummary = null;
                    string messages;

                    if (_cardRenewalService.ConfirmLoad(cardRenewalId, out renewalCardSummary, out messages))
                    {
                        lblInfoMessage.Text = "File upload confirmed.";
                        btnConfirm.Visible = false;
                    }
                    else
                    {
                        lblErrorMessage.Text = "File upload failed to confirm.";
                        btnReject.Visible = true;
                        btnUpload.Visible = true;
                    }
                }
                else
                {
                    btnReject.Visible = true;
                    btnUpload.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "Upload status: The file could confirmed. The following error occured: " + ex.Message;
                btnReject.Visible = true;
                btnUpload.Visible = true;
            }
        }

        protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}