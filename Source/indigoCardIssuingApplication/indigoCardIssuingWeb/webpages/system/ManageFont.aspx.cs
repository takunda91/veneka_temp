using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using Common.Logging;
using System.Threading;
using System.Globalization;
using System.IO;

namespace indigoCardIssuingWeb.webpages.system
{
    public partial class ManageFont : System.Web.UI.Page
    {

        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly BatchManagementService _batchservice = new BatchManagementService();


        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ADMINISTRATOR };
        private static readonly ILog log = LogManager.GetLogger(typeof(ManageFont));

        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                //if (DateTime.Now.Subtract(SessionWrapper.PasswordExpiryDate).TotalDays >= 60)
                //{
                //    Response.Redirect("~\\webpages\\users\\UserPasswordMaintainance.aspx");
                //}
                //else
                //{
                    if (ViewState[PageLayoutKey] != null)
                    {
                        pageLayout = (PageLayout)ViewState[PageLayoutKey];
                    }

                    if (!IsPostBack)
                    {
                        LoadPageData();
                        PopulateFields();
                        UpdateFormLayout(pageLayout);
                    }
                    else
                    {
                        MainFileupload();
                    }
                //}

            }
            else
            {
                Response.Redirect("~\\Default.aspx");
            }
        }

        #region Private Methods
        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuersList = new Dictionary<int, ListItem>();
                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                if (PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuersList))
                {
                   
                    
                }
                else
                {
                    this.pnlDisable.Visible = false;
                    this.pnlButtons.Visible = false;
                    if (!log.IsTraceEnabled)
                    {
                        log.Warn("A user tried to access a page that he/she does not have access to.");
                    }
                    log.Trace(m => m(String.Format("User {0} tried to access a page he/she does not have access to.", User.Identity.Name)));
                    lblErrorMessage.Text = Resources.DefaultExceptions.UnauthorisedPageAccessMessage;
                    return;
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

        private void PopulateFields()
        {
            try
            {

                int fontid = SessionWrapper.FontId;
                    if (fontid != 0)
                    {
                        FontResult product = _batchservice.GetFont(fontid);
                        lblfilename.Text = Path.GetFileName(product.resource_path);
                        SetValuestoControls(product);
                        pageLayout = PageLayout.READ;
                        Fontid = fontid;
                        SessionWrapper.FontId = 0;
                    }
                    else
                    {
                        //No username in session, set page layout to create.
                        pageLayout = PageLayout.CREATE;
                    }

                    ViewState[PageLayoutKey] = pageLayout;



                    SessionWrapper.ProductID = 0;
                    
                
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
        protected void SetValuestoControls(FontResult font)
        {
            try
            {
                tbFontname.Text = font.font_name;
               // furesourcepath= font.resource_path;
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
        private void UpdateFont()
        {
            try
            {
                FontResult fontresult = GetvaluesfromControls();
               
                fontresult.font_id = (int)Fontid;
                string response = _batchservice.UpdateFont(fontresult);
                if (String.IsNullOrWhiteSpace(response))
                {

                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessUpdate").ToString();
                    pageLayout = PageLayout.READ;
                    SessionWrapper.FontId = 0;
                }
                else
                {
                    this.lblErrorMessage.Text = response;
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

        private FontResult GetvaluesfromControls()
        {
            FontResult fontresult = new FontResult();

            fontresult.font_name = tbFontname.Text;
            if (furesourcepath.HasFile)
            {
                string filepath = Server.MapPath("..//..//Fonts//");
              string resourecpath=filepath + Path.GetFileName(furesourcepath.PostedFile.FileName);
              furesourcepath.PostedFile.SaveAs(resourecpath);
              fontresult.resource_path = "..//..//Fonts//" + Path.GetFileName(furesourcepath.PostedFile.FileName);
            }
            else
            {
                fontresult.resource_path = null;
            }
            // _batchservice.InsertProduct();

            return fontresult;
        }
        private void CreateFont()
        {
            try
            {
                FontResult fontresult = GetvaluesfromControls();
               
                string response = "";
                Fontid = _batchservice.InsertFont(fontresult, out response);
                if (String.IsNullOrWhiteSpace(response))
                {

                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessCreate").ToString();
                    SessionWrapper.UploadControl = null;
                    pageLayout = PageLayout.READ;
                }
                else
                {
                    this.lblErrorMessage.Text = response;
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
        private void DeleteFont()
        {
            try
            {


                var response = _batchservice.DeleteFont((int)Fontid);
                if (String.IsNullOrWhiteSpace(response))
                {


                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessDelete").ToString();
                    pageLayout = PageLayout.CREATE;
                    Fontid = 0;
                    this.tbFontname.Text = "";
                   // this.furesourcepath.Text = "";
                  
                }
                else
                {
                    this.lblErrorMessage.Text = response;
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
        #endregion

        #region Page Events
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.ServerVariables["http_user_agent"].IndexOf("Chrome", StringComparison.CurrentCultureIgnoreCase) != -1)
                Page.ClientTarget = "uplevel";
            if (Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) != -1)
                Page.ClientTarget = "uplevel";
        }

        protected override void InitializeCulture()
        {

            int lang = SessionWrapper.UserLanguage;

                Thread.CurrentThread.CurrentUICulture = new CultureInfo(UtilityClass.GetLang(lang));
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(UtilityClass.GetLang(lang));
            
        }

       
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteMessage").ToString();
                UpdateFormLayout(PageLayout.CONFIRM_DELETE);
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


        protected void btnCreate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
               
                UpdateFormLayout(PageLayout.CONFIRM_CREATE);
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
        protected void MainFileupload()
        {
            if (SessionWrapper.UploadControl== null && furesourcepath.HasFile)
            {
                Session["UploadControl"] = furesourcepath;
                lblfilename.Text = furesourcepath.FileName;
            }
            else if (SessionWrapper.UploadControl != null && (!furesourcepath.HasFile))
            {
                furesourcepath = (FileUpload)SessionWrapper.UploadControl;
                 lblfilename.Text = furesourcepath.FileName;
            }
            else if (furesourcepath.HasFile)
            {
                SessionWrapper.UploadControl = furesourcepath;
                lblfilename.Text = furesourcepath.FileName;
            } 
        }
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                this.btnEdit.Enabled = this.btnEdit.Visible = false;
                UpdateFormLayout(PageLayout.UPDATE);
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

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
               
                UpdateFormLayout(PageLayout.CONFIRM_UPDATE);
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
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (ViewState[PageLayoutKey] != null)
                {
                    pageLayout = (PageLayout)ViewState[PageLayoutKey];
                }

                switch (pageLayout)
                {
                    case PageLayout.CONFIRM_CREATE:
                        CreateFont();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        DeleteFont();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        UpdateFont();
                        UpdateFormLayout(pageLayout);
                        break;

                    default:
                        break;
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

        #endregion

        public long? Fontid
        {
            get
            {
                if (ViewState["Fontid"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["Fontid"].ToString());
            }
            set
            {
                ViewState["Fontid"] = value;
            }
        }

        #region Page Flow Methods
        private void UpdateFormLayout(PageLayout? toPageLayout)
        {
            if (toPageLayout == null)
            {
                if (ViewState[PageLayoutKey] != null)
                {
                    pageLayout = (PageLayout)ViewState[PageLayoutKey];
                }
                toPageLayout = pageLayout;
            }

            switch (toPageLayout)
            {
                case PageLayout.CREATE:
                    EnableFields(true);
                    this.btnCreate.Enabled = this.btnCreate.Visible = true;                   
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    divfileupload.Visible = true;
                    divFileuploadresult.Visible = false;
                    break;
                case PageLayout.READ:
                    DisableFields(false, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    divfileupload.Visible = false;
                    divFileuploadresult.Visible = true;
                    break;
                case PageLayout.UPDATE:
                    EnableFields(false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                      divfileupload.Visible = false;
                    divFileuploadresult.Visible = true;
                    break;
                case PageLayout.DELETE:

                    // this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    //this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    //this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    //this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                    //this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.CONFIRM_CREATE:
                    DisableFields(true, true);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmUpdate").ToString();
                      divfileupload.Visible = false;
                    divFileuploadresult.Visible = true;
                    //this.btnUpdate.Text = "Confirm";
                    break;
                case PageLayout.CONFIRM_DELETE:
                    DisableFields(true, false);

                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteMessage").ToString();
                      divfileupload.Visible = false;
                    divFileuploadresult.Visible = true;
                    //this.btnUpdate.Text = "Confirm";
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    DisableFields(true, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmUpdate").ToString();
                      divfileupload.Visible = false;
                    divFileuploadresult.Visible = true;
                    //this.btnUpdate.Text = "Confirm";
                    break;
                default:
                    DisableFields(false, false);
                    //this.btnUpdate.Text = UtilityClass.UppercaseFirst("EDIT");
                    break;
            }

            ViewState[PageLayoutKey] = toPageLayout;
        }

        private void EnableFields(bool isCreate)
        {
            this.tbFontname.Enabled = true;
            this.furesourcepath.Enabled = true;

            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnEdit.Enabled = this.btnEdit.Visible = isCreate ? false : true;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
          

            this.btnDelete.Enabled = this.btnDelete.Visible = false;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
            this.btnBack.Visible = isCreate ? false : true;


        }
        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            if (ViewState[PageLayoutKey] != null)
            {
                pageLayout = (PageLayout)ViewState[PageLayoutKey];
            }

            switch (pageLayout)
            {
                case PageLayout.RESET:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.UPDATE:
                    //LoadRestoreGroup();
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.DELETE:
                    UpdateFormLayout(PageLayout.READ);
                    break;
                case PageLayout.CONFIRM_CREATE:
                    UpdateFormLayout(PageLayout.CREATE);

                    break;
                case PageLayout.CONFIRM_DELETE:
                    UpdateFormLayout(PageLayout.DELETE);
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    UpdateFormLayout(PageLayout.UPDATE);

                    break;
                default:
                    break;
            }
        }
        private void DisableFields(bool IsConfirm, bool isCreate)
        {
            this.tbFontname.Enabled = false;
            this.furesourcepath.Enabled = false;
            

            this.btnBack.Visible = IsConfirm ? true : false;
            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnDelete.Enabled = this.btnDelete.Visible = isCreate;
          

            this.btnUpdate.Enabled = this.btnUpdate.Visible = isCreate ? false : true;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = IsConfirm;
        }


        #endregion
    }
}