using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.card
{
	public partial class CardStockOrdering : BasePage
	{
        private static readonly ILog log = LogManager.GetLogger(typeof(CardStockOrdering));

		private readonly SystemAdminService _adminServ = new SystemAdminService();
		private readonly BatchManagementService _batchservice = new BatchManagementService();
		private UserManagementService _userMan = new UserManagementService();

		private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.CENTER_OPERATOR };
		
		//int TotalCards = 0;        
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }


        private void LoadPageData()
        {
            try
            {
                Dictionary<int, ListItem> issuerListItems = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuersList = new Dictionary<int, RolesIssuerResult>();
                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuerListItems, out issuersList);

                this.ddlIssuer.Items.AddRange(issuerListItems.Values.OrderBy(m => m.Text).ToArray());
                this.ddlIssuer.SelectedIndex = 0;
                if (issuerListItems.Count > 0)
                {
                    UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue), true);
                    UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue));
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
		
		private Dictionary<int, ProductValidated> Products
		{
			get
			{
				if (ViewState["Products"] != null)
					return (Dictionary<int, ProductValidated>)ViewState["Products"];
				else
					return new Dictionary<int, ProductValidated>();
			}
			set
			{
				ViewState["Products"] = value;
			}
		}

		/// <summary>
		/// Populates the branch drop down list.
		/// </summary>
		/// <param name="issuerId"></param>
		private void UpdateBranchList(int issuerId, bool useCache)
		{
			if (issuerId >= 0)
			{
				this.ddlBranch.Items.Clear();
				Dictionary<int, ListItem> branchList = new Dictionary<int, ListItem>();

				var branches = _userMan.GetBranchesForUser(issuerId, userRolesForPage[0], 0);

				foreach (var item in branches)//Convert branches in item list.
				{
					branchList.Add(item.branch_id, utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
				}

				if (branchList.Count > 0)
				{
					ddlBranch.Items.AddRange(branchList.Values.OrderBy(m => m.Text).ToArray());
					ddlBranch.SelectedIndex = 0;
				}
			}
		}

		/// <summary>
		/// Updates the products dropdown with the issuers products
		/// </summary>
		/// <param name="issuerId"></param>
		private void UpdateProductList(int issuerId)
		{
			this.ddlProduct.Items.Clear();

			if (issuerId > 0)
			{
				List<ProductValidated> products;
				string messages;
				if (_batchservice.GetProductsListValidated(issuerId, 1, 1, 1000, out products, out messages))
				{
					List<ListItem> productsList = new List<ListItem>();
					Dictionary<int, ProductValidated> productDict = new Dictionary<int, ProductValidated>();

					foreach (var product in products)
					{
						if (!productDict.ContainsKey(product.ProductId))
							productDict.Add(product.ProductId, product);

						productsList.Add(utility.UtilityClass.FormatListItem<int>(product.ProductName, product.ProductCode, product.ProductId));
					}

					Products = productDict;

					if (productsList.Count > 0)
					{
						this.ddlProduct.Items.AddRange(productsList.OrderBy(m => m.Text).ToArray());
						this.ddlProduct.SelectedIndex = 0;

						ValidateLicence(issuerId, productDict);
					}
				}
				else
				{
                    this.lblErrorMessage.Text = messages;
				}
			}
		}

		private void ValidateLicence(int issuerId, Dictionary<int, ProductValidated> productDict)
		{
			if (productDict[int.Parse(this.ddlProduct.SelectedValue)].ValidLicence)
			{
				this.pnlButtons.Visible = true;				
			}
			else
			{
				this.pnlButtons.Visible = false;
				this.lblErrorMessage.Text = productDict[int.Parse(this.ddlProduct.SelectedValue)].Messages;
			}
		}

		private Dictionary<int, CardStock> ListOfRows
		{
			get
			{
				if (ViewState["TableRow"] != null)
					return (Dictionary<int, CardStock>)ViewState["TableRow"];
				else
					return new Dictionary<int, CardStock>();
			}
			set
			{
				ViewState["TableRow"] = value;
			}
		}

		//protected void btnAddStockItem_Click(object sender, EventArgs e)
		//{
		//    CardStock OrderDTO = new CardStock();
		//    List<CardStock> TableRowList = new List<CardStock>();

		//    OrderDTO.Issuer = ddlIssuer.SelectedItem.Text;
		//    OrderDTO.Branch = ddlBranch.SelectedItem.Text;
		//    OrderDTO.Product = ddlProduct.SelectedItem.Text;
		//    OrderDTO.NumberOfCards = this.tbNumberOfCards.Text;
						
		//    if (ViewState["TableRow"] == null)
		//    {
		//        TableRowList.Add(OrderDTO);
		//        ViewState["TableRow"] = TableRowList.ToArray();
		//    }
		//    else
		//    {
		//        CardStock[] newList = (CardStock[]) ViewState["TableRow"];
		//        List<CardStock> UpdatedList = new List<CardStock>(newList);
		//        TableRowList = UpdatedList;
		//        TableRowList.Add(OrderDTO);
		//        ViewState["TableRow"] = TableRowList.ToArray();
		//    }

		//    int rowCount = 1;
			
		//    foreach (var item in TableRowList)
		//    {               
		//        TableRow rowcount = new TableRow();
		//        TableCell Issuer = new TableCell();
		//        TableCell Branch = new TableCell();
		//        TableCell Product = new TableCell();
		//        TableCell NumberOfCards = new TableCell();

		//        Issuer.Text = item.Issuer;
		//        Branch.Text = item.Branch;
		//        Product.Text = item.Product;
		//        NumberOfCards.Text = item.NumberOfCards;
				
		//        rowcount.Cells.Add(Issuer);
		//        rowcount.Cells.Add(Branch);
		//        rowcount.Cells.Add(Product);
		//        rowcount.Cells.Add(NumberOfCards);
		//        tbOrderlist.Rows.AddAt(rowCount, rowcount);
		//        rowCount++;
		//    }
			
		//    //check if user made input
			
		//    int currentValue = 0;
		//    currentValue =  int.Parse(this.tbNumberOfCards.Text);
		//    if (ViewState["TotalCards"] == null)
		//    {
		//        TotalCards += currentValue;
		//        ViewState["TotalCards"] = TotalCards;
		//    }
		//    else
		//    {                
		//        TotalCards = int.Parse(ViewState["TotalCards"].ToString());
		//        TotalCards += currentValue;
		//        ViewState["TotalCards"] = TotalCards;
		//    }            
		//    this.lblTotalCards.Text = TotalCards.ToString();
		//}

		protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			this.lblInfoMessage.Text = "";
			this.lblErrorMessage.Text = "";

			try
			{
				UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue), false);
				UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue));
			}
			catch (Exception ex)
			{
				log.Error(ex);
				this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
				if (log.IsDebugEnabled || log.IsTraceEnabled)
				{
					this.lblErrorMessage.Text = ex.Message;
				}
			}
		}

		protected void ddlProduct_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			this.lblInfoMessage.Text = "";
			this.lblErrorMessage.Text = "";

			try
			{
				ValidateLicence(int.Parse(this.ddlIssuer.SelectedValue), Products);
			}
			catch (Exception ex)
			{
				log.Error(ex);
				this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
				if (log.IsDebugEnabled || log.IsTraceEnabled)
				{
					this.lblErrorMessage.Text = ex.Message;
				}
			}
		}

        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
		protected void btnCreate_Click(object sender, EventArgs e)
		{
			this.lblInfoMessage.Text = "";
			this.lblErrorMessage.Text = "";

			try
			{
				int cardsToCreate;
				if (int.TryParse(this.tbNumberOfCards.Text, out cardsToCreate))
				{
					if (cardsToCreate <= 0)
					{
						this.lblErrorMessage.Text = "Number of cards to create must be greater than 0.";
						return;
					}

					if(cardsToCreate > 100000)
					{
						this.lblErrorMessage.Text = "Number of cards to create must be less than or equal to 100,000.";
						return;
					}


					this.ddlIssuer.Enabled =
					this.ddlBranch.Enabled =
					this.ddlProduct.Enabled =
					this.tbNumberOfCards.Enabled = false;


					this.btnCreate.Visible = false;
					this.btnConfirm.Visible = true;
					this.btnCancel.Visible = true;


					this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmInfoMessage").ToString();
				}
                else
                {
                    this.lblErrorMessage.Text = "Invalid value entered for number of cards, please enter a value between 1 and 100,000.";
                }
			}
			catch (Exception ex)
			{
				log.Error(ex);
				this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
				if (log.IsDebugEnabled || log.IsTraceEnabled)
				{
					this.lblErrorMessage.Text = ex.Message;
				}
			}
		}

        [PrincipalPermission(SecurityAction.Demand, Role = "CENTER_OPERATOR")]
		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			this.lblInfoMessage.Text = "";
			this.lblErrorMessage.Text = "";

			//Create the new distribution batch.
			try
			{
				int branchId = int.Parse(ddlBranch.SelectedValue);
				int issuerId = int.Parse(ddlIssuer.SelectedValue);
				int productId = int.Parse(ddlProduct.SelectedValue);

				int batchCardSize = int.Parse(this.tbNumberOfCards.Text);

				CardRequestBatchResponse cardreq;

				if (int.TryParse(this.ddlBranch.SelectedValue, out branchId) &&
				   int.TryParse(this.tbNumberOfCards.Text, out batchCardSize))
				{
					string response;
					if(_batchservice.CreateCardStockRequest(issuerId, branchId, productId, 1, 0, batchCardSize,out cardreq, out response))
					{
						this.btnConfirm.Visible =
						this.btnCancel.Visible = false;

						this.btnCreate.Visible = false;

						this.ddlIssuer.Enabled =
						this.ddlBranch.Enabled =
						this.ddlProduct.Enabled =
						this.tbNumberOfCards.Enabled = true;

						this.tbNumberOfCards.Text = String.Empty;

						SessionWrapper.DistBatchId = cardreq.DistBatchId;

						this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmSuccessCreate").ToString() + " <a href='../dist/DistBatchView.aspx?page=CSO'>" + cardreq.DistBatchRef + "</a>";
					}
					else
					{
						this.lblErrorMessage.Text = response;
					}
				}
				else
				{
					this.lblInfoMessage.Text = "There seems to be an issue with the batch details, Please check details and try again.";
				}
			}
			catch (Exception ex)
			{
				log.Error(ex);
				this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
				if (log.IsDebugEnabled || log.IsTraceEnabled)
				{
					this.lblErrorMessage.Text = ex.Message;
				}
			}
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			this.lblInfoMessage.Text = "";
			this.lblErrorMessage.Text = "";

			try
			{

				this.ddlIssuer.Enabled =
				this.ddlBranch.Enabled =
				this.ddlProduct.Enabled =
				this.tbNumberOfCards.Enabled = true;


				this.btnCreate.Visible = true;
				this.btnConfirm.Visible = false;
				this.btnCancel.Visible = false;
			}
			catch (Exception ex)
			{
				log.Error(ex);
				this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
				if (log.IsDebugEnabled || log.IsTraceEnabled)
				{
					this.lblErrorMessage.Text = ex.Message;
				}
			}
		}
	}	
}