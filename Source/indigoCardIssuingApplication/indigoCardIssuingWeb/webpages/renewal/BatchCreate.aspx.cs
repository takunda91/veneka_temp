using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.renewal
{
    public partial class BatchCreate : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RenewalApprove));
        private readonly RenewalService _cardRenewalService = new RenewalService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR, UserRole.BRANCH_PRODUCT_OPERATOR };
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DisplayResults(1);
            }
        }

        public int? TotalPages
        {
            get
            {
                if (ViewState["TotalPages"] == null)
                    return 1;
                else
                    return Convert.ToInt32(ViewState["TotalPages"].ToString());
            }
            set
            {
                ViewState["TotalPages"] = value;
            }
        }

        private void DisplayResults(int pageIndex)
        {
            this.lblErrorMessage.Text = "";
            this.dlCardRenewalList.DataSource = null;
            List<RenewalDetailListModel> results = _cardRenewalService.ListRenewalApprovedCards();
            batchNumber.Visible = false;
            if (results.Count > 0)
            {
                this.dlCardRenewalList.DataSource = results;
                TotalPages = results.Count/ 10;
            }
            else
            {
                TotalPages = 1;
                this.lblErrorMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
            }

            this.lblPageIndex.Text = String.Format(Resources.CommonLabels.PageinationPageIndex, pageIndex, TotalPages);
            this.dlCardRenewalList.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";
            btnSave.Visible = false;
            try
            {
                this.lblInfoMessage.Text = "Are you sure you want to create a batch?";
                btnConfirm.Visible = true;
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

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            btnConfirm.Visible = false;
            btnSave.Visible = false;
            try
            {
                var newBatches = _cardRenewalService.CreateRenewalBatch();
                RenewalBatches = newBatches;
                if (newBatches!=null && newBatches.Count>0)
                {
                    batchNumber.Visible = true;
                    string generatedBatches = string.Join(",", newBatches.Select(p => p.CalculatedBatchNumber.ToString()));

                    txtBatchId.Text = generatedBatches;
                    lblInfoMessage.Text = "Card Renewal batch(es) created.";
                    btnPrint.Visible = true;
                }
                else
                {
                    lblErrorMessage.Text = "Failed to create a card renewal batch.";
                    btnSave.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "Upload status: The file could not be approved. The following error occured: " + ex.Message;
                btnSave.Visible = true;
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {

                var reportBytes = _cardRenewalService.GenerateRenewalNewBatches(RenewalBatches.Select(p=>p.RenewalBatchId).ToList());
                string reportName = String.Empty;
                reportName = $"Renewal_New_Batches_{DateTime.Now.ToString("ddd_dd_MMMM_yyyy") }.pdf";
                Response.Clear();
                Response.ClearHeaders();
                MemoryStream ms = new MemoryStream(reportBytes);
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.Flush();
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
        private List<RenewalBatch> RenewalBatches
        {
            get
            {
                if (ViewState["RenewalBatches"] != null)
                    return (List<RenewalBatch>)ViewState["RenewalBatches"];
                else
                    return new List<RenewalBatch>();
            }
            set
            {
                ViewState["RenewalBatches"] = value;
            }
        }

        protected void lnkFirst_Click(object sender, EventArgs e)
        {

        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {

        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {

        }

        protected void lnkLast_Click(object sender, EventArgs e)
        {

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~\\webpages\\renewal\\BatchCreate.aspx");
        }
    }
}