using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;

namespace indigoCardIssuingWeb.webpages.pin
{
    public partial class E_PINRequest : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(E_PINRequest));

        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly UserManagementService _userMan = new UserManagementService();
        private static readonly AudiControllService _auditservice = new AudiControllService();

        private readonly PINManagementService _pinMan = new PINManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.PIN_OPERATOR };
        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        #region Page Load
        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuerListItems = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuersList = new Dictionary<int, RolesIssuerResult>();
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerListItems, out issuersList);

                
                this.ddlIssuer.Items.AddRange(issuerListItems.Values.OrderBy(m => m.Text).ToArray());

                this.ddlIssuer.SelectedIndex = 0;



                if (issuerListItems.Count > 0)
                {

                    UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue), true);
                    UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue), null);
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

        #endregion

        #region private methods
        private void UpdateProductList(int issuerId, int? productId)
        {
            this.ddlProduct.Items.Clear();

            if (issuerId > 0)
            {
                List<ProductValidated> products;
                string messages;
                if (_batchService.GetProductsListValidated(issuerId, null, 1, 1000, out products, out messages))
                {
                    List<ListItem> productsList = new List<ListItem>();
                    Dictionary<int, ProductValidated> productDict = new Dictionary<int, ProductValidated>();

                    foreach (var product in products.Where(i=>i.ePinRequest==true))
                    {
                        if (!productDict.ContainsKey(product.ProductId))
                            productDict.Add(product.ProductId, product);

                        productsList.Add(utility.UtilityClass.FormatListItem<int>(product.ProductName, product.ProductCode, product.ProductId));
                    }



                    if (productsList.Count > 0)
                    {
                        this.ddlProduct.Items.AddRange(productsList.OrderBy(m => m.Text).ToArray());

                        if (productId != null)
                            this.ddlProduct.SelectedValue = productId.Value.ToString();
                        else
                            this.ddlProduct.SelectedIndex = 0;
                    }

                    this.ddlProduct.Visible = true;
                    this.lblCardProduct.Visible = true;

                }
                else
                {
                    this.lblErrorMessage.Text = messages;
                    this.pnlAccountNumber.Visible = false;
                    this.pnlButtons.Visible = false;
                }
            }
        }
        private string MaskAccountnumber(string messageField,int masklength,int rightpad)
        {
            int LeftPad = messageField.Length- rightpad;
            string righttext=messageField.Substring(LeftPad, rightpad);
            string Lefttext= messageField.Substring(0,(LeftPad- masklength));

            return Lefttext+"".PadLeft(masklength, '*')
                        + righttext;

            
        }
        //public string StrSubstring(this string value, int startIndex, int length)
        //{
        //    return new string((value ?? string.Empty).Skip(startIndex).Take(length).ToArray());
        //}
        
        private  string Contactnumber
        {
            get
            {
                if (ViewState["Contactnumber"] != null)
                    return (string)ViewState["Contactnumber"];
                else
                    return null;
            }
            set
            {
                ViewState["Contactnumber"] = value;
            }
        }
        private void UpdateBranchList(int issuerId, bool useCache)
        {
            if (issuerId >= 0)
            {
                this.ddlBranch.Items.Clear();
                Dictionary<int, ListItem> branchList = new Dictionary<int, ListItem>();

                //if (!useCache /*|| BranchList.Count == 0*/) //should I use cache? if yes check that there is something in page cache.
                //{
                var branches = _userMan.GetBranchesForUser(issuerId, userRolesForPage[0], false);

                foreach (var item in branches)//Convert branches in item list.
                {
                    branchList.Add(item.branch_id, utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
                }

                if (branchList.Count > 0)
                {
                    ddlBranch.Items.AddRange(branchList.Values.OrderBy(m => m.Text).ToArray());
                    ddlBranch.SelectedIndex = 0;

                    this.lblBranch.Visible = true;
                    this.ddlBranch.Visible = true;

                }
                else
                {

                    lblErrorMessage.Text = GetGlobalResourceObject("CommonInfoMessages", "EmptyBrancheslistMessage").ToString();
                }
            }
        }
        #endregion


        #region Page Events
        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
        protected void btnValidateAccount_Click(object sender, EventArgs e)
        {
            int issuerId;
            int branchId = int.Parse(this.ddlBranch.SelectedValue);
            if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
            {
                string messages;
                AccountDetails accountDetails;
                if (_customerCardIssuerService.GetAccountDetail(issuerId,
                           int.Parse(ddlProduct.SelectedValue),
                            0,
                            branchId, this.tbAccountNumber.Text.Trim(), out accountDetails, out messages))
                {
                   
                    hdnContactnumber.Value= accountDetails.ContactNumber;
                    tbContactnumber.Text = MaskAccountnumber(accountDetails.ContactNumber,6,4);

                    divContactnumber.Visible = true;
                    lblErrorMessage.Text = string.Empty;
                    dlCardList.DataSource = accountDetails.CMSCards;
                    dlCardList.DataBind();
                    DisplayAllFields(true);
                }
                else
                {
                    lblErrorMessage.Text = messages;
                    divContactnumber.Visible = false;
                }

            }
        }
        protected void btnConfirm_OnClick(object sender, EventArgs e)
        {
            try

            {

                int branchId, issuerId, productId;
                int.TryParse(this.ddlBranch.SelectedValue, out branchId);
                int.TryParse(this.ddlIssuer.SelectedValue, out issuerId);
                int.TryParse(this.ddlProduct.SelectedValue, out productId);

                string responseMessage, PRRN, pan = string.Empty;
                foreach (DataListItem item in dlCardList.Items)
                {
                    RadioButton chk = item.FindControl("radCardNumber") as RadioButton;
                    Label hdncardnumber = item.FindControl("hdncardnumber") as Label;
                    pan = hdncardnumber.Text;
                }
                lblErrorMessage.Text = string.Empty;
                if (_pinMan.EPinRequest(issuerId, branchId, productId, hdnContactnumber.Value, pan, out responseMessage, out PRRN)) 
                {
                    lblInfoMessage.Text = "PRRN Number for the Request :" + PRRN;
                }
                DisplayAllFields(true);
                ClearControls();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;

                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }
            }
        }
        protected void radCardNumber_CheckedChanged(object sender, System.EventArgs e)
        {
            //Clear the existing selected row 
            foreach (DataListItem oldrow in dlCardList.Items)
            {
                ((RadioButton)oldrow.FindControl("radCardNumber")).Checked = false;
            }

            //Set the new selected row
            RadioButton rb = (RadioButton)sender;
            DataListItem row = (DataListItem)rb.NamingContainer;
            ((RadioButton)row.FindControl("radCardNumber")).Checked = true;
        }

        protected void ClearControls()
        {
            tbAccountNumber.Text=string.Empty;
            tbContactnumber.Text=string.Empty;
            btnValidateAccount.Visible=true;
            dlCardList.Visible=false;
            btnPinRequest.Visible=false;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            DisplayAllFields(true);
        }
        protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                int issuerId;
                if (int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                {
                    //UpdateAccountValidationControls();
                    UpdateBranchList(issuerId, true);
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
        private void DisplayAllFields(bool showAll)
        {
            this.tbAccountNumber.Enabled = showAll;
            this.btnValidateAccount.Enabled = showAll;
            this.tbContactnumber.Enabled = showAll;

            this.ddlIssuer.Enabled = showAll;
            this.ddlBranch.Enabled = showAll;

            this.ddlProduct.Enabled = showAll;

            //this.reqCustomerID.Enabled =


            this.btnPinRequest.Visible = showAll;
            this.btnConfirm.Visible = showAll ? false : true;
            this.btnCancel.Visible = showAll ? false : true;

        }
        protected void dlCardList_ItemDataBound(object sender, DataListItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item ||
             e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label tb = e.Item.FindControl("lblcardnumber") as Label;
                Label hdncardnumber = e.Item.FindControl("hdncardnumber") as Label;



                if (tb != null)
                {
                    tb.Text = MaskAccountnumber(hdncardnumber.Text,6,4);
                }
            }
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "PIN_OPERATOR")]
        protected void btnPinRequest_OnClick(object sender, EventArgs e)
        {


            DisplayAllFields(false);
            this.btnCancel.Enabled = this.btnCancel.Visible = true;
            this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
        }

        #endregion

        #region WebMethods
        [WebMethod]
        public static string InsertAuditRequest(string accountnumber,string issuerId,string Contactnumber)
        {
            try
            {
                string description= "moblie number : " + Contactnumber + " incorrect for acc :" + accountnumber;
                 _auditservice.InsertAudit(10, description,int.Parse(issuerId));
                return "success";
            }
            catch (Exception ex)
            {
                log.Error(ex);
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.StatusCode = 500;
                throw new HttpException(500, ex.Message);
            }
        }
        #endregion


    }
}