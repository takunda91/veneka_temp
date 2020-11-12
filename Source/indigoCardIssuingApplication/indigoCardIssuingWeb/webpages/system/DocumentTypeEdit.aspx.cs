using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class DocumentTypeEdit : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentTypeEdit));
        private readonly DocumentManagementService _documentTypeService = new DocumentManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };

        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private void LoadPageData()
        {
            try
            {
                int entityId = 0;
                if (!String.IsNullOrWhiteSpace(Request.QueryString["id"]))
                {
                    entityId = int.Parse(Request.QueryString["id"]);
                }
                if (entityId != 0)
                {
                    DocumentType entity = _documentTypeService.DocumentTypeGet(entityId);
                    if (entity != null)
                    {
                        txtDescription.Text = entity.Description;
                        txtName.Text = entity.Name;
                        chkIsActive.Checked = entity.IsActive;
                    }
                    else
                    {
                        entityId = 0;
                    }
                }
                documentTypeId.Value = entityId.ToString();

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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = string.Empty;

            if (btnConfirm.Visible == true)
            {
                btnSave.Visible = true;
                btnConfirm.Visible = false;
            }
            else
            {
                Server.TransferRequest("~\\webpages\\system\\DocumentTypeList.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                this.btnCancel.Enabled = this.btnCancel.Visible = true;
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDocumentSave").ToString();
                btnSave.Visible = false;
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
            try
            {
                lblInfoMessage.Text = "";
                lblErrorMessage.Text = "";

                string resultMessage = string.Empty;

                DocumentType entity = new DocumentType()
                {
                    Description = txtDescription.Text,
                    Id = Convert.ToInt32(documentTypeId.Value),
                    IsActive = chkIsActive.Checked,
                    Name = txtName.Text
                };

                int entityId = _documentTypeService.DocumentTypeSave(entity);
                if (entityId != 0)
                {
                    documentTypeId.Value = entityId.ToString();
                    lblInfoMessage.Text = resultMessage;
                    btnConfirm.Visible = this.btnConfirm.Enabled = false;
                }
                else
                {
                    lblErrorMessage.Text = resultMessage;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException);
                lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;

                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    lblErrorMessage.Text = ex.ToString();
                }
            }
        }
    }
}