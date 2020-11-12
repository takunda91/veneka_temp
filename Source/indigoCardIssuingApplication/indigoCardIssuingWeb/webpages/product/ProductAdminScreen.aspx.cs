using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.utility;
using Common.Logging;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Web.Security;
using System.Security.Permissions;
using indigoCardIssuingWeb.SearchParameters;

namespace indigoCardIssuingWeb.webpages.product
{
    public partial class ProductAdminScreen : BasePage
    {
        private PageLayout pageLayout = PageLayout.READ;
        private const string PageLayoutKey = "PageLayout";
        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly BatchManagementService _batchservice = new BatchManagementService();
        private readonly DocumentManagementService _documents = new DocumentManagementService();

        private bool tableCopied = false;
        private DataTable originalDataTable;
        private bool LoadDataEmpty = false;
        private static readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.ISSUER_ADMIN };
        private static readonly ILog log = LogManager.GetLogger(typeof(ProductAdminScreen));
        List<ProductPrintFieldResult> result = new List<ProductPrintFieldResult>();

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (ViewState[PageLayoutKey] != null)
            {
                pageLayout = (PageLayout)ViewState[PageLayoutKey];
            }

            if (!IsPostBack)
            {
                hdnActiveTab.Value = "0";
                LoadPageData();
            }
        }

        #region Private Methods
        private void LoadPageData()
        {
            try
            {
                //chkCurrency.Items.Clear();

                Dictionary<int, ListItem> issuersList = Roles.Provider.ToIndigoRoleProvider()
                                                            .GetIssuersForRole(User.Identity.Name, userRolesForPage);

                List<Issuer_product_font> list = _batchservice.GetFontFamilyList();

                this.ProductSearchParameter = SessionWrapper.ProductSearchParameter;
                SessionWrapper.ProductSearchParameter = null;


                //List<ListItem> fonts = new List<ListItem>();
                //foreach (var item in list)
                //{
                //    fonts.Add(new ListItem(item.font_name, item.font_id.ToString()));
                //}
                //ddlfontdropdown.Items.AddRange(fonts.OrderBy(m => m.Text).ToArray());
                //ddlfontdropdown.SelectedIndex = 0;

                if (issuersList.ContainsKey(-1))
                    issuersList.Remove(-1);

                ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                ddlIssuer.SelectedIndex = 0;

                //Populate the product load type
                List<ListItem> productLoadTypes = new List<ListItem>();
                foreach (var item in _batchservice.LangLookupProductLoadTypes())
                {
                    productLoadTypes.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                }
                this.ddlLoadBatchType.Items.AddRange(productLoadTypes.OrderBy(m => m.Value).ToArray());
                this.ddlLoadBatchType.SelectedValue = "0";


                List<ListItem> pinlockitems = new List<ListItem>();
                foreach (var item in _batchservice.LookupPinBlockFormat())
                {
                    pinlockitems.Add(new ListItem(item.pin_block_format, item.pin_block_formatid.ToString()));
                }
                this.ddlPinblcok.Items.AddRange(pinlockitems.OrderBy(m => m.Value).ToArray());
                this.ddlPinblcok.SelectedValue = "0";

                //Populate service request values
                List<ServiceRequestCode> srcCode1 = _batchservice.GetServiceRequestCode1();
                List<ServiceRequestCode1> srcCode2 = _batchservice.GetServiceRequestCode2();
                List<ServiceRequestCode2> srcCode3 = _batchservice.GetServiceRequestCode3();
                if (srcCode1 != null)
                {
                    foreach (var item in srcCode1)
                    {
                        this.ddlSRC1.Items.Add(new ListItem(item.src1_id.ToString() + " - " + item.name, item.src1_id.ToString()));
                    }
                }
                if (srcCode2 != null)
                {
                    foreach (var item in srcCode2)
                    {
                        this.ddlSRC2.Items.Add(new ListItem(item.src2_id.ToString() + " - " + item.name, item.src2_id.ToString()));
                    }
                }
                if (srcCode3 != null)
                {
                    foreach (var item in srcCode3)
                    {
                        this.ddlSRC3.Items.Add(new ListItem(item.src3_id.ToString() + " - " + item.name, item.src3_id.ToString()));
                    }

                }

                //Populate Issuing Scenarios
                foreach (var reasonForIssue in _customerCardIssuerService.LangLookupCardIssueReasons().OrderBy(o => o.language_text))
                {
                    this.chklIssuingScenarios.Items.Add(new ListItem(reasonForIssue.language_text, reasonForIssue.lookup_id.ToString()));
                }

                //Populate account type drop down.
                List<ListItem> accountTypes = new List<ListItem>();
                foreach (var item in _customerCardIssuerService.LangLookupCustomerAccountTypes())
                {
                    accountTypes.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                }
                this.chklAccounts.Items.AddRange(accountTypes.OrderBy(m => m.Text).ToArray());

                if (ddlIssuer.Items.Count == 0)
                {
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "EmptyIssuerslistMessage").ToString() + "<br/>";
                }
                if (!string.IsNullOrEmpty(lblErrorMessage.Text))
                {
                    pnlButtons.Visible = false;
                    lblInfoMessage.Text = "";
                    lblErrorMessage.Text += GetGlobalResourceObject("CommonInfoMessages", "SaveActionMessage").ToString();
                }
                else
                {
                    // lblErrorMessage.Text = "";
                    pnlButtons.Visible = true;
                }

                //Fetch Connection Parameters
                //CBS
                List<ListItem> connList = new List<ListItem>();
                List<ListItem> connListCBS = new List<ListItem>();
                List<ListItem> connListPrepaid = new List<ListItem>();

                var connResult = _issuerMan.GetConnectionParameters();
                foreach (var item in connResult)
                {
                    if (item.connection_parameter_type_id != 2)
                    {
                        connList.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                        connListCBS.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                        connListPrepaid.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                    }
                }
                this.ddlIssueCoreBanking.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlIssueCoreBanking.Items.AddRange(connList.OrderBy(m => m.Text).ToArray());

                this.ddlFundsConnectionCBS.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlFundsConnectionCBS.Items.AddRange(connListCBS.OrderBy(m => m.Text).ToArray());

                this.ddlFundsConnectionPrepaid.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlFundsConnectionPrepaid.Items.AddRange(connListPrepaid.OrderBy(m => m.Text).ToArray());

                SetConnectionDropDowns(connResult, this.ddlProdCMS, new List<int> { 2 });

                SetConnectionDropDowns(connResult, this.ddlInterfaceThreed, new List<int> { 1 });

                SetConnectionDropDowns(connResult, this.ddlIssureThreedSecure, new List<int> { 2 });

                //List<ListItem> connList22 = new List<ListItem>();
                //foreach (var item in connResult)
                //{
                //    if (item.connection_parameter_type_id != 2)
                //        connList22.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                //}
                //this.ddlIssueCMS.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                //this.ddlIssueCMS.Items.AddRange(connList22.OrderBy(m => m.Text).ToArray());
                SetConnectionDropDowns(connResult, this.ddlIssueCMS, new List<int> { 2 });


                //Remote CMS
                SetConnectionDropDowns(connResult, this.ddlIssuerRemoteCMS, new List<int> { 2 });

                //CPS
                List<ListItem> connList4 = new List<ListItem>();
                foreach (var item in connResult)
                {
                    if (item.connection_parameter_type_id != 2)
                        connList4.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                }
                this.ddlProdCPS.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlProdCPS.Items.AddRange(connList4.OrderBy(m => m.Text).ToArray());

                //CardFileLoader
                List<ListItem> connFileLoaderList = new List<ListItem>();
                foreach (var item in connResult)
                {
                    if (item.connection_parameter_type_id == 1)
                        connFileLoaderList.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                }
                this.ddlFileLoader.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlFileLoader.Items.AddRange(connFileLoaderList.OrderBy(m => m.Text).ToArray());

                //ListItem[] connFileExportArray = new ListItem[connFileLoaderList.Count];
                //Array.Copy(connFileLoaderList.OrderBy(m => m.Text).ToArray(), connFileExportArray, connFileLoaderList.Count);

                List<ListItem> connFileExport = new List<ListItem>();
                foreach (var item in connResult)
                {
                    if (item.connection_parameter_type_id == 1)
                        connFileExport.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                }
                this.ddlFileExportSettings.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlFileExportSettings.Items.AddRange(connFileExport.OrderBy(m => m.Text).ToArray());


                //Fee Scheme
                List<ListItem> connFeeSchemeList = new List<ListItem>();
                foreach (var item in connResult)
                {
                    if (item.connection_parameter_type_id != 2)
                        connFeeSchemeList.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
                }
                this.ddlIssueFeeScheme.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
                this.ddlIssueFeeScheme.Items.AddRange(connFeeSchemeList.OrderBy(m => m.Text).ToArray());

                LoadAvailableInterfaces();

                LoadSchemes();

                PopulateFields();

                UpdateFormLayout(pageLayout);
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

        private void LoadDocuments(int productId)
        {
            var allDocuments = _documents.ProductDocumentAll(productId, false);
            this.grdDocuments.DataSource = ToDocumentDataTable(allDocuments);
            this.grdDocuments.DataBind();
        }

        private void SetConnectionDropDowns(List<ConnectionParamsResult> connResult, DropDownList dropDown, List<int> excludedConfigTypeIds)
        {
            List<ListItem> connList22 = new List<ListItem>();
            foreach (var item in connResult.Where(w => !excludedConfigTypeIds.Any(exc => exc == w.connection_parameter_type_id)))
            {
                connList22.Add(new ListItem(item.connection_name, item.connection_parameter_id.ToString()));
            }
            dropDown.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            dropDown.Items.AddRange(connList22.OrderBy(m => m.Text).ToArray());
        }

        private void BuildCurrencyTable()
        {
            //tblCurrency.Rows.Clear();

            var productCurrencies = new List<ProductCurrencyResult>();
            foreach (var currencyItem in _issuerMan.GetCurrencyList().OrderBy(o => o.currency_code))
            {
                productCurrencies.Add(new ProductCurrencyResult
                {
                    currency_id = currencyItem.currency_id,
                    currency_code = currencyItem.currency_code,
                    iso_4217_numeric_code = currencyItem.iso_4217_numeric_code,
                    currency_desc = currencyItem.currency_desc
                });
            }

            LoadCurrencyTable(productCurrencies.ToArray());
        }

        private void LoadCurrencyTable(ProductCurrencyResult[] productCurrency)
        {
            dlCurrency.DataSource = productCurrency.OrderBy(o => o.currency_code);
            dlCurrency.DataBind();
        }

        private ProductCurrencyResult[] PopulateCurrencyList()
        {
            var productCurrencies = new List<ProductCurrencyResult>();

            foreach (DataListItem row in dlCurrency.Items)
            {
                if (((CheckBox)row.FindControl("chkAllow")).Checked)
                {
                    var productCurrency = new ProductCurrencyResult();

                    productCurrency.currency_id = int.Parse(((Label)row.FindControl("chkCurrId")).Text);
                    productCurrency.is_base = ((CheckBox)row.FindControl("chkBase")).Checked;
                    productCurrency.usr_field_name_1 = ((TextBox)row.FindControl("tbFieldName1")).Text;
                    productCurrency.usr_field_val_1 = ((TextBox)row.FindControl("tbFieldVal1")).Text;

                    productCurrencies.Add(productCurrency);
                }
            }

            return productCurrencies.ToArray();
        }

        private ProductExternalSystemResult[] PopulateExternalSystemFields()
        {
            var external_system_fields = new List<ProductExternalSystemResult>();
            external_system_fields.AddRange(GetExternalSystemFields(dlExternalfieldsCBS));
            external_system_fields.AddRange(GetExternalSystemFields(dlExternalfieldsCPS));
            external_system_fields.AddRange(GetExternalSystemFields(dlExternalfieldsCMS));
            external_system_fields.AddRange(GetExternalSystemFields(dlExternalfieldsHSM));

            return external_system_fields.ToArray();
        }

        private List<ProductExternalSystemResult> GetExternalSystemFields(DataList dlExternalSystemFields)
        {
            List<ProductExternalSystemResult> externalsystemfields = new List<ProductExternalSystemResult>();
            foreach (DataListItem row in dlExternalSystemFields.Items)
            {

                var externalsystemfield = new ProductExternalSystemResult();
                string external_system_field_id = ((Label)row.FindControl("lblexternalsystemfieldid")).Text;
                externalsystemfield.external_system_field_id = int.Parse(external_system_field_id);
                externalsystemfield.field_name = ((Label)row.FindControl("tbFieldName")).Text;
                externalsystemfield.field_value = ((TextBox)row.FindControl("tbFieldValue")).Text;

                externalsystemfields.Add(externalsystemfield);

            }

            return externalsystemfields;
        }

        private void LoadAvailableInterfaces()
        {
            var availInterfaces = _issuerMan.ListAvailiableIntegrationInterfaces();

            var availExternalSystems = _issuerMan.GetExternalSystem(null, 1, 2000).ExternalSystems;
            //CBS
            List<ListItem> interfaces = new List<ListItem>();
            List<ListItem> interfacesFunds = new List<ListItem>();

            foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == 0))
            {
                interfaces.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
                interfacesFunds.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
            }
            this.ddlInterfaceCBS.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            this.ddlInterfaceCBS.Items.AddRange(interfaces.OrderBy(m => m.Text).ToArray());


            List<ListItem> externalsystem = new List<ListItem>();
            foreach (var item in availExternalSystems.Where(w => w.external_system_type_id == 0))
            {
                externalsystem.Add(new ListItem(item.system_name, item.external_system_id.ToString()));
            }
            this.ddlexternalsytemsCBS.Items.Add(new ListItem(Resources.ListItemLabels.NONE, "-99"));
            this.ddlexternalsytemsCBS.Items.AddRange(externalsystem.OrderBy(m => m.Text).ToArray());

            //CMS
            //List<ListItem> interfaces2 = new List<ListItem>();
            //foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == 1))
            //{
            //    interfaces2.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
            //}
            //this.ddlInterfaceCMS.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            //this.ddlInterfaceCMS.Items.AddRange(interfaces2.OrderBy(m => m.Text).ToArray());
            SetInterfaceDropDown(availInterfaces, this.ddlInterfaceCMS, 1);
            List<ListItem> threedsecure = new List<ListItem>();
            foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == 11))
            {
                threedsecure.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
            }
            this.ddlInterfaceThreed.Items.Add(new ListItem(Resources.ListItemLabels.NONE, "-99"));
            this.ddlInterfaceThreed.Items.AddRange(threedsecure.OrderBy(m => m.Text).ToArray());
            //REMOTE CMS
            SetInterfaceDropDown(availInterfaces, this.ddlInterfaceRemoteCMS, 9);

            List<ListItem> externalsystem1 = new List<ListItem>();
            foreach (var item in availExternalSystems.Where(w => w.external_system_type_id == 2))
            {
                externalsystem1.Add(new ListItem(item.system_name, item.external_system_id.ToString()));
            }
            this.ddlexternalsytemsCMS.Items.Add(new ListItem(Resources.ListItemLabels.NONE, "-99"));
            this.ddlexternalsytemsCMS.Items.AddRange(externalsystem1.OrderBy(m => m.Text).ToArray());

            //CPS
            List<ListItem> interfaces4 = new List<ListItem>();
            foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == 3))
            {
                interfaces4.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
            }
            this.ddlInterfaceCPS.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            this.ddlInterfaceCPS.Items.AddRange(interfaces4.OrderBy(m => m.Text).ToArray());


            List<ListItem> externalsystem2 = new List<ListItem>();
            foreach (var item in availExternalSystems.Where(w => w.external_system_type_id == 1))
            {
                externalsystem2.Add(new ListItem(item.system_name, item.external_system_id.ToString()));
            }
            this.ddlexternalsytemsCPS.Items.Add(new ListItem(Resources.ListItemLabels.NONE, "-99"));
            this.ddlexternalsytemsCPS.Items.AddRange(externalsystem2.OrderBy(m => m.Text).ToArray());

            //FileProcessing
            List<ListItem> interfaces5 = new List<ListItem>();
            foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == 4))
            {
                interfaces5.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
            }
            this.ddlInterfaceFileLoader.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            this.ddlInterfaceFileLoader.Items.AddRange(interfaces5.OrderBy(m => m.Text).ToArray());

            //Export
            List<ListItem> interfaces8 = new List<ListItem>();
            foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == 4))
            {
                interfaces8.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
            }
            this.ddlFileExport.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            this.ddlFileExport.Items.AddRange(interfaces8.OrderBy(m => m.Text).ToArray());

            //Fee Schemes
            List<ListItem> interfaces6 = new List<ListItem>();
            foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == 0))
            {
                interfaces6.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
            }
            this.ddlInterfaceFeeScheme.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            this.ddlInterfaceFeeScheme.Items.AddRange(interfaces6.OrderBy(m => m.Text).ToArray());

            // hsminterface
            List<ListItem> externalsystem3 = new List<ListItem>();
            foreach (var item in availExternalSystems.Where(w => w.external_system_type_id == 3))
            {
                externalsystem3.Add(new ListItem(item.system_name, item.external_system_id.ToString()));
            }
            this.ddlexternalsytemsHSM.Items.Add(new ListItem(Resources.ListItemLabels.NONE, "-99"));
            this.ddlexternalsytemsHSM.Items.AddRange(externalsystem3.OrderBy(m => m.Text).ToArray());


            //funds load tab
            this.ddlFundsCoreBanking.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            this.ddlFundsCoreBanking.Items.AddRange(interfacesFunds.OrderBy(m => m.Text).ToArray());

            SetInterfaceDropDown(availInterfaces, this.ddlFundsPrepaid, 12);

            // storing all the fields in viewstate
            ExternalSystemFields = _issuerMan.GetExternalSystemsFields(null, 1, 2000);
        }

        private void SetInterfaceDropDown(List<IntegrationInterface> availInterfaces, DropDownList dropDown, int interfaceTypeId)
        {
            List<ListItem> interfaces2 = new List<ListItem>();
            foreach (var item in availInterfaces.Where(w => w.InterfaceTypeId == interfaceTypeId))
            {
                interfaces2.Add(new ListItem(item.IntegrationName, item.IntegrationGUID));
            }
            dropDown.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-99"));
            dropDown.Items.AddRange(interfaces2.OrderBy(m => m.Text).ToArray());
        }

        private void LoadDistFlows(int CardIssueMethodId)
        {
            this.ddlProdFlow.Items.Clear();
            this.ddlDistFlow.Items.Clear();

            this.pnlButtons.Visible = false;

            var results = _batchservice.GetDistBatchFlows(CardIssueMethodId);

            List<ListItem> productionItems = new List<ListItem>();

            foreach (var result in results.Where(w => w.Dist_batch_type_id == 0))
            {
                productionItems.Add(new ListItem(result.dist_batch_status_flow_name, result.dist_batch_status_flow_id.ToString()));
            }

            this.ddlProdFlow.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-1"));
            this.ddlProdFlow.Items.AddRange(productionItems.OrderBy(o => o.Text).ToArray());

            this.pnlButtons.Visible = true;

            List<ListItem> distributionItems = new List<ListItem>();

            foreach (var result in results.Where(w => w.Dist_batch_type_id == 1))
            {
                distributionItems.Add(new ListItem(result.dist_batch_status_flow_name, result.dist_batch_status_flow_id.ToString()));
            }

            this.ddlDistFlow.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-1"));
            this.ddlDistFlow.Items.AddRange(distributionItems.OrderBy(o => o.Text).ToArray());

            this.ddlProdFlow.SelectedValue = this.ddlDistFlow.SelectedValue = "-1";

            /*************************************** Allow for instant  charge fee  for all products ***********/ 
            //if (CardIssueMethodId == 0)
            //{
            //    lblChargeFeeAtCardRequest.Visible = true;
            //    chkChargeFeeAtCardRequest.Visible = true;
            //}
            //else
            //{
            //    lblChargeFeeAtCardRequest.Visible = false;
            //    chkChargeFeeAtCardRequest.Visible = false;
            //}
            this.pnlButtons.Visible = true;
            LoadRenewalDistFlows();
        }

        private void LoadRenewalDistFlows()
        {
            int CardIssueMethodId = 0;
            this.ddlProdFlowRenewal.Items.Clear();
            this.ddlDistFlowRenewal.Items.Clear();

            this.pnlButtons.Visible = false;

            var results = _batchservice.GetDistBatchFlows(CardIssueMethodId);

            List<ListItem> productionItemsRenewal = new List<ListItem>();

            foreach (var result in results.Where(w => w.Dist_batch_type_id == 0))
            {
                productionItemsRenewal.Add(new ListItem(result.dist_batch_status_flow_name, result.dist_batch_status_flow_id.ToString()));
            }

            this.ddlProdFlowRenewal.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-1"));
            this.ddlProdFlowRenewal.Items.AddRange(productionItemsRenewal.OrderBy(o => o.Text).ToArray());

            this.pnlButtons.Visible = true;

            List<ListItem> distributionItemsRenewal = new List<ListItem>();

            foreach (var result in results.Where(w => w.Dist_batch_type_id == 1))
            {
                distributionItemsRenewal.Add(new ListItem(result.dist_batch_status_flow_name, result.dist_batch_status_flow_id.ToString()));
            }

            this.ddlDistFlowRenewal.Items.Add(new ListItem(Resources.ListItemLabels.SELECT, "-1"));
            this.ddlDistFlowRenewal.Items.AddRange(distributionItemsRenewal.OrderBy(o => o.Text).ToArray());

            this.ddlProdFlowRenewal.SelectedValue = this.ddlDistFlowRenewal.SelectedValue = "-1";
        }

        private void LoadSchemes()
        {
            this.ddlProductScheme.Items.Clear();
            this.pnlButtons.Visible = false;

            if (!String.IsNullOrWhiteSpace(this.ddlIssuer.SelectedValue))
            {
                var results = _batchservice.GetFeeSchemes(int.Parse(this.ddlIssuer.SelectedValue), 1, 1000);

                List<ListItem> items = new List<ListItem>();

                foreach (var result in results)
                {
                    items.Add(new ListItem(result.fee_scheme_name, result.fee_scheme_id.ToString()));
                }

                this.ddlProductScheme.Items.Add(new ListItem(Resources.ListItemLabels.NONE, "-1"));
                this.ddlProductScheme.Items.AddRange(items.OrderBy(o => o.Text).ToArray());
                this.pnlButtons.Visible = true;
            }
            else
            {
                this.lblErrorMessage.Text += "<br />No Issuers Avaialble";

            }
        }

        private void PopulateFields()
        {
            if (SessionWrapper.PreviewYN)
            {
                //if (SessionWrapper.PresentMode != null)
                //{
                pageLayout = (PageLayout)SessionWrapper.PresentMode;
                //}
                ViewState[PageLayoutKey] = pageLayout;
                if (SessionWrapper.ProductlistResult != null)
                {
                    ProductResult product = (ProductResult)SessionWrapper.ProductlistResult;
                    if (/*SessionWrapper.ProductID != null &&*/ SessionWrapper.ProductID != 0)
                    {
                        Productid = (int)SessionWrapper.ProductID;
                        product.Product.product_id = (int)Productid;

                        LoadPrintFields((int)Productid);
                    }

                    SetValuestoControls(product);
                }


                SessionWrapper.PreviewYN = false;
            }
            else
            {
                int productid = SessionWrapper.ProductID;
                if (productid != 0)
                {

                    ProductResult product = _batchservice.GetProduct(productid);

                    SetValuestoControls(product);
                    pageLayout = PageLayout.READ;
                    Productid = productid;

                    LoadCurrencyTable(product.Currency);
                    LoadPrintFields(productid);
                    BindGridView(product.AccountType_Mappings.ToList());
                    var result = from c in product.ExternalSystemFields
                                 group c by new
                                 {
                                     c.external_system_type_id,
                                     c.external_system_id
                                 } into grp
                                 select grp.First();
                    foreach (var item in result)
                    {
                        //CBS
                        switch (item.external_system_type_id)
                        {
                            case 0:
                                ddlexternalsytemsCBS.SelectedValue = item.external_system_id.ToString();
                                BindDataGrid(ddlexternalsytemsCBS, dlExternalfieldsCBS, false, product.ExternalSystemFields);
                                break;
                            case 1:
                                ddlexternalsytemsCPS.SelectedValue = item.external_system_id.ToString();
                                BindDataGrid(ddlexternalsytemsCPS, dlExternalfieldsCPS, false, product.ExternalSystemFields);
                                break;
                            case 2:
                                ddlexternalsytemsCMS.SelectedValue = item.external_system_id.ToString();
                                BindDataGrid(ddlexternalsytemsCMS, dlExternalfieldsCMS, false, product.ExternalSystemFields);
                                break;
                            case 3:
                                ddlexternalsytemsHSM.SelectedValue = item.external_system_id.ToString();
                                BindDataGrid(ddlexternalsytemsHSM, dlExternalfieldsHSM, false, product.ExternalSystemFields);
                                break;



                        }

                    }

                    //List<product_currency1> results = _batchservice.GetCurrencyByProduct(productid);
                    //if (results != null)
                    //{
                    //    foreach (var currency in results)
                    //    {
                    //        if (chkCurrency.Items.FindByValue(currency.currency_id.ToString()) != null)
                    //        {
                    //            int index = chkCurrency.Items.IndexOf(chkCurrency.Items.FindByValue(currency.currency_id.ToString()));
                    //            chkCurrency.Items[index].Selected = true;
                    //        }
                    //    }
                    //}
                }
                else
                {
                    //Populate currency
                    BuildCurrencyTable();

                    LoadDistFlows(int.Parse(this.ddlCardIssueMethod.SelectedValue));
                    BindGridView(null);
                    //No username in session, set page layout to create.
                    pageLayout = PageLayout.CREATE;
                    LoadPrintFields(0);
                }

                LoadDocuments(productid);

                ViewState[PageLayoutKey] = pageLayout;

                SessionWrapper.ProductID = 0;
            }
        }

        protected void SetValuestoControls(ProductResult product)
        {
            try
            {
                IsDeleted = product.Product.DeletedYN;

                tbProductname.Text = product.Product.product_name;
                tbProductCode.Text = product.Product.product_code;
                tbproductbin.Text = product.Product.product_bin_code;

                this.ddlPanLength.SelectedValue = product.Product.pan_length.ToString();
                this.tbSubProductCode.Text = product.Product.sub_product_code;
                this.ddlPinCalcMethod.SelectedValue = product.Product.pin_calc_method_id.ToString();
                this.chkAutoApproveBatch.Checked = product.Product.auto_approve_batch_YN;
                this.chkAccountValidation.Checked = product.Product.account_validation_YN;
                this.chkPinAccountValidation.Checked = product.Product.pin_account_validation_YN.GetValueOrDefault();

                this.ddlPVK1.SelectedValue = product.Product.PVKI;
                this.tbPVK.Text = product.Product.PVK;
                this.tbCVKA.Text = product.Product.CVKA;
                this.tbCVKB.Text = product.Product.CVKB;
                this.tbExpiryMonth.Text = product.Product.expiry_months.ToString();

                this.ddlSRC1.SelectedValue = product.Product.src1_id.ToString();
                this.ddlSRC2.SelectedValue = product.Product.src2_id.ToString();
                this.ddlSRC3.SelectedValue = product.Product.src3_id.ToString();

                this.chkPrintPIN.Checked = product.Product.pin_mailer_printing_YN;
                this.chkRePrintPin.Checked = product.Product.pin_mailer_reprint_YN;
                this.tbMinPinLength.Text = product.Product.min_pin_length.ToString();
                this.tbMaxPinLength.Text = product.Product.max_pin_length.ToString();

                this.chkRenewalActivate.Checked = product.Product.renewal_activate_card.GetValueOrDefault();
                this.chkRenewalCharge.Checked = product.Product.renewal_charge_card.GetValueOrDefault();

                this.chkCreditLimitCapture.Checked = product.Product.credit_limit_capture.GetValueOrDefault();
                this.chkCreditLimitUpdate.Checked = product.Product.credit_limit_update.GetValueOrDefault();
                this.chkCreditLimitApprove.Checked = product.Product.credit_limit_approve.GetValueOrDefault();

                this.chkEmailRequired.Checked = product.Product.email_required.GetValueOrDefault();
                this.chkGenerateReferenceNumber.Checked = product.Product.generate_contract_number.GetValueOrDefault();
                this.chkManualReferenceNumber.Checked = product.Product.manual_contract_number.GetValueOrDefault();
                this.chkParallelApproval.Checked = product.Product.parallel_approval.GetValueOrDefault();
                this.chkCenterOpsActivation.Checked = product.Product.activation_by_center_operator.GetValueOrDefault();

                this.tbCreditContractPrefix.Text = product.Product.credit_contract_prefix;
                this.tbCreditContractSuffixFormat.Text = product.Product.credit_contract_suffix_format;
                this.tbCreditContractLastSequence.Text = product.Product.credit_contract_last_sequence.GetValueOrDefault().ToString();

                if (product.Product.pin_block_formatid != null)
                    this.ddlPinblcok.SelectedValue = product.Product.pin_block_formatid.ToString();

                if (ddlPinCalcMethod.SelectedValue == "1")
                {
                    this.tbDecimalisation.Text = product.Product.decimalisation_table.ToString();
                    this.tbPinvalidation.Text = product.Product.pin_validation_data.ToString();
                }
                else
                {
                    this.tbDecimalisation.Text = string.Empty;
                    this.tbPinvalidation.Text = string.Empty;
                }
                this.ddlCardIssueMethod.SelectedValue = product.Product.card_issue_method_id.ToString();
              
                LoadDistFlows(product.Product.card_issue_method_id);
                this.ddlProdFlow.SelectedValue = product.Product.production_dist_batch_status_flow.ToString();
                this.ddlDistFlow.SelectedValue = product.Product.distribution_dist_batch_status_flow.ToString();
                this.ddlProdFlowRenewal.SelectedValue = product.Product.production_dist_batch_status_flow_renewal.ToString();
                this.ddlDistFlowRenewal.SelectedValue = product.Product.distribution_dist_batch_status_flow_renewal.ToString();

                this.chkbChargeToIssueBranch.Checked = product.Product.charge_fee_to_issuing_branch_YN;
                this.chkPrintIssueCard.Checked = product.Product.print_issue_card_YN;
                this.chkChargeFeeAtCardRequest.Checked = product.Product.charge_fee_at_cardrequest ?? false;
               
                this.allowM20Print.Checked = product.Product.Is_mtwenty_printed ?? false;


                this.chkbAllowManualUpload.Checked = product.Product.allow_manual_uploaded_YN;
                this.chkbAllowReupload.Checked = product.Product.allow_reupload_YN;

                this.ddlLoadBatchType.SelectedValue = product.Product.product_load_type_id.ToString();
                this.chkCMSExportable.Checked = product.Product.cms_exportable_YN;
                this.chkremoteCMSEnable.Checked = product.Product.remote_cms_update_YN;
                this.chkInstantPin.Checked = product.Product.enable_instant_pin_YN;
                this.chkInstantPinResissue.Checked = product.Product.enable_instant_pin_reissue_YN;

                if (product.CardIssueReasons != null)
                {
                    for (int i = 0; i < this.chklIssuingScenarios.Items.Count; i++)
                    {
                        if (product.CardIssueReasons.Contains(int.Parse(this.chklIssuingScenarios.Items[i].Value)))
                            this.chklIssuingScenarios.Items[i].Selected = true;
                    }
                }

                if (product.AccountTypes != null)
                {
                    for (int i = 0; i < this.chklAccounts.Items.Count; i++)
                    {
                        if (product.AccountTypes.Contains(int.Parse(this.chklAccounts.Items[i].Value)))
                            this.chklAccounts.Items[i].Selected = true;
                    }
                }

                if (product.Interfaces != null)
                {
                    //Production
                    foreach (var item in product.Interfaces.Where(w => w.interface_area == 0))
                    {
                        //CBS = 0
                        //CMS = 1
                        //HSM = 2
                        //CPS = 3
                        switch (item.interface_type_id)
                        {
                            case 0: //this.ddlCoreBanking.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlInterfaceCBS.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            case 1:
                                this.ddlProdCMS.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlInterfaceCMS.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            case 3:
                                this.ddlProdCPS.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlInterfaceCPS.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            case 4:
                                this.ddlFileLoader.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlInterfaceFileLoader.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            case 5:
                                this.ddlIssueFeeScheme.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlInterfaceFeeScheme.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            case 8:
                                this.ddlFileExportSettings.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlFileExport.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            case 12:
                                this.ddlFundsConnectionCBS.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlFundsCoreBanking.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            default: break;
                        }
                    }

                    //Issueing
                    foreach (var item in product.Interfaces.Where(w => w.interface_area == 1))
                    {
                        //CBS = 0
                        //CMS = 1
                        //HSM = 2
                        //CPS = 3
                        switch (item.interface_type_id)
                        {
                            case 0:
                                this.ddlIssueCoreBanking.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlInterfaceCBS.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            case 1:
                                this.ddlIssueCMS.SelectedValue = item.connection_parameter_id.ToString();
                                break;
                            case 3: //this.ddlProdCPS.SelectedValue = item.connection_parameter_id.ToString();
                                break;
                            case 5:
                                this.ddlIssueFeeScheme.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlInterfaceFeeScheme.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            case 9:
                                this.ddlIssuerRemoteCMS.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlInterfaceRemoteCMS.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            case 11:
                                this.ddlIssureThreedSecure.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlInterfaceThreed.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            case 12:
                                this.ddlFundsConnectionPrepaid.SelectedValue = item.connection_parameter_id.ToString();
                                this.ddlFundsPrepaid.SelectedValue = item.interface_guid ?? "-99";
                                break;
                            default: break;
                        }
                    }
                }

                ddlIssuer.SelectedIndex = ddlIssuer.Items.IndexOf(ddlIssuer.Items.FindByValue(product.Product.issuer_id.ToString()));
                LoadSchemes();

                if (product.Product.fee_scheme_id != null)
                    this.ddlProductScheme.SelectedValue = product.Product.fee_scheme_id.Value.ToString();

                //if (SessionWrapper.CurrencyList != null)
                //{
                //    List<int> results = SessionWrapper.CurrencyList;
                //    foreach (var currency in results)
                //    {

                //        if (chkCurrency.Items.FindByValue(currency.ToString()) != null)
                //        {
                //            int index = chkCurrency.Items.IndexOf(chkCurrency.Items.FindByValue(currency.ToString()));
                //            chkCurrency.Items[index].Selected = true;
                //        }
                //    }
                //    SessionWrapper.CurrencyList = null;
                //}

                grdPrintingFields.DataSource = null;
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

        private void UpdateProduct()
        {
            try
            {
                this.lblInfoMessage.Text =
                    this.lblErrorMessage.Text = String.Empty;

                ProductResult prod = GetvaluesfromControls();
                prod.Product.product_id = (int)Productid;
                prod.Currency = PopulateCurrencyList();
                prod.ExternalSystemFields = PopulateExternalSystemFields();
                prod.AccountType_Mappings = PopulateAccountTypes();

                string response;
                if (_batchservice.UpdateProduct(prod, out response))
                {
                    this.lblInfoMessage.Text = response;
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

        private ProductResult GetvaluesfromControls()
        {
            ProductResult prd = new ProductResult();

            ProductlistResult product = new ProductlistResult();
            product.issuer_id = int.Parse(ddlIssuer.SelectedValue);

            if (int.Parse(this.ddlProductScheme.SelectedValue) > 0)
                product.fee_scheme_id = int.Parse(this.ddlProductScheme.SelectedValue);
            
            product.product_name = tbProductname.Text.Trim();
            product.product_code = tbProductCode.Text.Trim();
            product.product_bin_code = tbproductbin.Text.Trim();

            product.pan_length = short.Parse(this.ddlPanLength.SelectedValue);
            product.sub_product_code = this.tbSubProductCode.Text;

            product.pin_calc_method_id = int.Parse(this.ddlPinCalcMethod.SelectedValue);
            product.auto_approve_batch_YN = this.chkAutoApproveBatch.Checked;
            product.account_validation_YN = this.chkAccountValidation.Checked;
            product.pin_account_validation_YN = this.chkPinAccountValidation.Checked;

            //if (this.rdbClassicCard.Checked)
            //    product.card_issue_method_id = 0;
            //else if (this.rdbInstantCard.Checked)
            //    product.card_issue_method_id = 1;

            product.product_load_type_id = int.Parse(this.ddlLoadBatchType.SelectedValue);
            product.cms_exportable_YN = this.chkCMSExportable.Checked;
            product.remote_cms_update_YN = this.chkremoteCMSEnable.Checked;
            product.enable_instant_pin_YN = this.chkInstantPin.Checked;
            product.enable_instant_pin_reissue_YN = this.chkInstantPinResissue.Checked;

            product.charge_fee_to_issuing_branch_YN = this.chkbChargeToIssueBranch.Checked;
            product.print_issue_card_YN = this.chkPrintIssueCard.Checked;

            product.production_dist_batch_status_flow = int.Parse(this.ddlProdFlow.SelectedValue);
            product.distribution_dist_batch_status_flow = int.Parse(this.ddlDistFlow.SelectedValue);

            product.production_dist_batch_status_flow_renewal = int.Parse(this.ddlProdFlowRenewal.SelectedValue);
            product.distribution_dist_batch_status_flow_renewal = int.Parse(this.ddlDistFlowRenewal.SelectedValue);

            product.allow_manual_uploaded_YN = this.chkbAllowManualUpload.Checked;
            product.allow_reupload_YN = this.chkbAllowReupload.Checked;

            product.renewal_charge_card = chkRenewalCharge.Checked;
            product.renewal_activate_card = chkRenewalActivate.Checked;

            product.credit_limit_capture = chkCreditLimitCapture.Checked;
            product.credit_limit_update = chkCreditLimitUpdate.Checked;
            product.credit_limit_approve = chkCreditLimitApprove.Checked;

            product.email_required = chkEmailRequired.Checked;
            product.generate_contract_number = chkGenerateReferenceNumber.Checked;
            product.manual_contract_number = chkManualReferenceNumber.Checked;
            product.parallel_approval = chkParallelApproval.Checked;
            product.activation_by_center_operator = chkCenterOpsActivation.Checked;

            long lastSequence;
            long.TryParse(tbCreditContractLastSequence.Text, out lastSequence);
            product.credit_contract_last_sequence = lastSequence;
            product.credit_contract_prefix = tbCreditContractPrefix.Text;
            product.credit_contract_suffix_format = tbCreditContractSuffixFormat.Text;

            decimal top, left;

            //Decimal.TryParse(tbnameoncardtop.Text.Trim(), NumberStyles.Any, new CultureInfo("en-US"), out top);
            //Decimal.TryParse(tbnameoncardleft.Text.Trim(), NumberStyles.Any, new CultureInfo("en-US"), out left);

            product.name_on_card_top = 0;
            product.name_on_card_left = 0;

            //product.font_id = int.Parse(ddlfontdropdown.SelectedValue);
            //product.font_name = ddlfontdropdown.SelectedItem.Text;


            //product.Name_on_card_font_size = int.Parse(ddlFontSize.SelectedValue);
            // _batchservice.InsertProduct();

            product.src1_id = int.Parse(ddlSRC1.SelectedValue);
            product.src2_id = int.Parse(ddlSRC2.SelectedValue);
            product.src3_id = int.Parse(ddlSRC3.SelectedValue);
            product.PVKI = ddlPVK1.SelectedValue;
            product.PVK = this.tbPVK.Text;
            product.CVKA = this.tbCVKA.Text;
            product.CVKB = this.tbCVKB.Text;

            product.pin_mailer_printing_YN = this.chkPrintPIN.Checked;
            product.pin_mailer_reprint_YN = this.chkRePrintPin.Checked;
            product.min_pin_length = int.Parse(this.tbMinPinLength.Text);
            product.max_pin_length = int.Parse(this.tbMaxPinLength.Text);

            product.decimalisation_table = this.tbDecimalisation.Text;
            product.pin_validation_data = this.tbPinvalidation.Text;
            int pin_block_formatid = 0;
            int.TryParse(this.ddlPinblcok.SelectedValue, out pin_block_formatid);
            product.pin_block_formatid = pin_block_formatid;

            //product.sub_product_id_length = int.Parse(this.ddlSubProductLength.SelectedValue);

            if (!String.IsNullOrWhiteSpace(this.tbExpiryMonth.Text))
                product.expiry_months = int.Parse(tbExpiryMonth.Text);
            else
                product.expiry_months = 0;

            prd.Product = product;

            prd.Product.card_issue_method_id = int.Parse(this.ddlCardIssueMethod.SelectedValue);

            //if (this.ddlCardIssueMethod.SelectedValue == "0")
                product.charge_fee_at_cardrequest = this.chkChargeFeeAtCardRequest.Checked;
                product.Is_mtwenty_printed = this.allowM20Print.Checked;
            //else
            //    product.charge_fee_at_cardrequest = false;


            //card_issue_reason
            List<int> issueReason = new List<int>();
            for (int i = 0; i < chklIssuingScenarios.Items.Count; i++)
            {
                if (chklIssuingScenarios.Items[i].Selected)
                {
                    issueReason.Add(int.Parse(chklIssuingScenarios.Items[i].Value));
                }
            }
            prd.CardIssueReasons = issueReason.ToArray();

            //account Types
            List<int> accountTypes = new List<int>();
            for (int i = 0; i < chklAccounts.Items.Count; i++)
            {
                if (chklAccounts.Items[i].Selected)
                {
                    accountTypes.Add(int.Parse(chklAccounts.Items[i].Value));
                }
            }
            prd.AccountTypes = accountTypes.ToArray();


            List<product_interface> interfaces = new List<product_interface>();

            if (!this.ddlProdCMS.SelectedValue.Equals("-99"))
            {
                product_interface cbInterface = new product_interface();
                cbInterface.connection_parameter_id = int.Parse(this.ddlProdCMS.SelectedValue);
                cbInterface.interface_area = 0;
                cbInterface.interface_type_id = 1;
                cbInterface.product_id = prd.Product.issuer_id;
                if (!this.ddlInterfaceCMS.SelectedValue.Equals("-99"))
                    cbInterface.interface_guid = this.ddlInterfaceCMS.SelectedValue;

                interfaces.Add(cbInterface);
            }
            if (!this.ddlIssureThreedSecure.SelectedValue.Equals("-99"))
            {
                product_interface cbInterface = new product_interface();
                cbInterface.connection_parameter_id = int.Parse(this.ddlIssureThreedSecure.SelectedValue);
                cbInterface.interface_area = 1;
                cbInterface.interface_type_id = 11;
                cbInterface.product_id = prd.Product.issuer_id;
                if (!this.ddlInterfaceThreed.SelectedValue.Equals("-99"))
                    cbInterface.interface_guid = this.ddlInterfaceThreed.SelectedValue;

                interfaces.Add(cbInterface);
            }

            if (!this.ddlProdCPS.SelectedValue.Equals("-99"))
            {
                product_interface cbInterface = new product_interface();
                cbInterface.connection_parameter_id = int.Parse(this.ddlProdCPS.SelectedValue);
                cbInterface.interface_area = 0;
                cbInterface.interface_type_id = 3;
                cbInterface.product_id = prd.Product.issuer_id;
                if (!this.ddlInterfaceCPS.SelectedValue.Equals("-99"))
                    cbInterface.interface_guid = this.ddlInterfaceCPS.SelectedValue;

                interfaces.Add(cbInterface);
            }

            if (!this.ddlFileLoader.SelectedValue.Equals("-99"))
            {
                product_interface fileLoaderInterface = new product_interface();
                fileLoaderInterface.connection_parameter_id = int.Parse(this.ddlFileLoader.SelectedValue);
                fileLoaderInterface.interface_area = 0;
                fileLoaderInterface.interface_type_id = 4;
                fileLoaderInterface.product_id = prd.Product.issuer_id;

                if (!this.ddlInterfaceFileLoader.SelectedValue.Equals("-99"))
                    fileLoaderInterface.interface_guid = this.ddlInterfaceFileLoader.SelectedValue;

                interfaces.Add(fileLoaderInterface);
            }

            product_interface accInterface = new product_interface();
            if (!this.ddlIssueCoreBanking.SelectedValue.Equals("-99"))
            {
                accInterface.connection_parameter_id = int.Parse(this.ddlIssueCoreBanking.SelectedValue);
                accInterface.interface_area = 1;
                accInterface.interface_type_id = 0;
                accInterface.product_id = prd.Product.issuer_id;
                if (!this.ddlInterfaceCBS.SelectedValue.Equals("-99"))
                    accInterface.interface_guid = this.ddlInterfaceCBS.SelectedValue;

                interfaces.Add(accInterface);
            }

            if (!this.ddlIssueCMS.SelectedValue.Equals("-99"))
            {
                product_interface cbInterface = new product_interface();
                cbInterface.connection_parameter_id = int.Parse(this.ddlIssueCMS.SelectedValue);
                cbInterface.interface_area = 1;
                cbInterface.interface_type_id = 1;
                cbInterface.product_id = prd.Product.issuer_id;
                if (!this.ddlInterfaceCMS.SelectedValue.Equals("-99"))
                    cbInterface.interface_guid = this.ddlInterfaceCMS.SelectedValue;

                interfaces.Add(cbInterface);
            }

            if (!this.ddlIssuerRemoteCMS.SelectedValue.Equals("-99"))
            {
                product_interface cbInterface = new product_interface();
                cbInterface.connection_parameter_id = int.Parse(this.ddlIssuerRemoteCMS.SelectedValue);
                cbInterface.interface_area = 1;
                cbInterface.interface_type_id = 9;
                cbInterface.product_id = prd.Product.issuer_id;
                if (!this.ddlInterfaceRemoteCMS.SelectedValue.Equals("-99"))
                    cbInterface.interface_guid = this.ddlInterfaceRemoteCMS.SelectedValue;

                interfaces.Add(cbInterface);
            }

            if (!this.ddlIssueFeeScheme.SelectedValue.Equals("-99"))
            {
                product_interface fsInterface = new product_interface();
                fsInterface.connection_parameter_id = int.Parse(this.ddlIssueFeeScheme.SelectedValue);
                fsInterface.interface_area = 1;
                fsInterface.interface_type_id = 5;
                fsInterface.product_id = prd.Product.issuer_id;
                if (!this.ddlInterfaceFeeScheme.SelectedValue.Equals("-99"))
                    fsInterface.interface_guid = this.ddlInterfaceFeeScheme.SelectedValue;

                interfaces.Add(fsInterface);
            }

            if (!this.ddlFileExportSettings.SelectedValue.Equals("-99"))
            {
                product_interface feInterface = new product_interface();
                feInterface.connection_parameter_id = int.Parse(this.ddlFileExportSettings.SelectedValue);
                feInterface.interface_area = 0;
                feInterface.interface_type_id = 8;
                feInterface.product_id = prd.Product.issuer_id;
                if (!this.ddlFileExport.SelectedValue.Equals("-99"))
                    feInterface.interface_guid = this.ddlFileExport.SelectedValue;

                interfaces.Add(feInterface);
            }

            if (!this.ddlFundsCoreBanking.SelectedValue.Equals("-99"))
            {
                product_interface feInterface = new product_interface();
                feInterface.interface_guid = this.ddlFundsCoreBanking.SelectedValue;
                feInterface.connection_parameter_id = int.Parse(ddlFundsConnectionCBS.SelectedValue);
                feInterface.interface_area = 0;
                feInterface.interface_type_id = 12;
                feInterface.product_id = prd.Product.issuer_id;
                interfaces.Add(feInterface);
            }

            if (!this.ddlFundsPrepaid.SelectedValue.Equals("-99"))
            {
                product_interface feInterface = new product_interface();
                feInterface.interface_guid = ddlFundsPrepaid.SelectedValue;
                feInterface.connection_parameter_id = int.Parse(ddlFundsConnectionPrepaid.SelectedValue);
                feInterface.interface_area = 1;
                feInterface.interface_type_id = 12;
                feInterface.product_id = prd.Product.issuer_id;

                interfaces.Add(feInterface);
            }

            prd.Interfaces = interfaces.ToArray();

            return prd;
        }

        private void CreateProduct()
        {
            try
            {
                this.lblInfoMessage.Text =
                    this.lblErrorMessage.Text = String.Empty;

                ProductResult prod = GetvaluesfromControls();
                prod.Currency = PopulateCurrencyList();
                prod.ExternalSystemFields = PopulateExternalSystemFields();
                prod.AccountType_Mappings = PopulateAccountTypes();
                long productId;
                string response = "";

                if (_batchservice.InsertProduct(prod, out productId, out response))
                {
                    Productid = productId;
                    this.lblInfoMessage.Text = response;
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

        private void DeleteProduct()
        {
            try
            {
                var response = _batchservice.DeleteProduct((int)Productid);
                if (String.IsNullOrWhiteSpace(response))
                {
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteSuccess").ToString();
                    pageLayout = PageLayout.CREATE;
                    Productid = 0;
                    this.tbProductname.Text = "";
                    this.tbProductCode.Text = "";
                    this.tbproductbin.Text = "";
                    this.ddlPanLength.SelectedValue = "16";
                    this.tbSubProductCode.Text = String.Empty;
                    this.ddlCardIssueMethod.SelectedValue = "1";
                    this.ddlPinCalcMethod.SelectedIndex = 0;
                    this.chkAutoApproveBatch.Checked = false;
                    this.chkAccountValidation.Checked = false;
                    this.chkPinAccountValidation.Checked = false;
                    this.chkCreditLimitApprove.Checked = false;
                    this.chkCreditLimitCapture.Checked = false;
                    this.chkCreditLimitUpdate.Checked = false;
                    this.chkRenewalActivate.Checked = false;
                    this.chkRenewalCharge.Checked = false;
                    this.ddlLoadBatchType.SelectedValue = "0";
                    this.chkCMSExportable.Checked = false;
                    this.chkremoteCMSEnable.Checked = false;
                    this.chkInstantPin.Checked = false;
                    this.chkInstantPinResissue.Checked = false;
                    this.ddlIssuer.SelectedIndex = 0;
                    this.ddlPinblcok.SelectedIndex = 0;
                    this.tbExpiryMonth.Text =
                    this.tbCVKB.Text =
                    this.tbCVKA.Text =
                    this.tbPVK.Text =
                    this.tbMinPinLength.Text =
                    this.tbMaxPinLength.Text = tbPinvalidation.Text = tbDecimalisation.Text = String.Empty;
                    this.ddlPVK1.SelectedIndex = 0;
                    this.ddlSRC1.SelectedIndex = 0;
                    this.ddlSRC2.SelectedIndex = 0;
                    this.ddlSRC3.SelectedIndex = 0;

                    this.ddlInterfaceCBS.SelectedIndex =
                    this.ddlInterfaceCMS.SelectedIndex =
                    this.ddlInterfaceRemoteCMS.SelectedIndex =
                    this.ddlInterfaceCPS.SelectedIndex =
                    this.ddlInterfaceFeeScheme.SelectedIndex =
                    this.ddlFileExportSettings.SelectedIndex =
                    this.ddlInterfaceFileLoader.SelectedIndex = 0;

                    this.ddlIssueCMS.SelectedIndex =
                    this.ddlInterfaceRemoteCMS.SelectedIndex =
                    this.ddlIssueCoreBanking.SelectedIndex =
                    this.ddlIssueFeeScheme.SelectedIndex =
                    this.ddlProdCMS.SelectedIndex =
                    this.ddlProdCPS.SelectedIndex =
                    this.ddlProductThreedSecure.SelectedIndex =
                    this.ddlInterfaceThreed.SelectedIndex =
                    this.ddlIssureThreedSecure.SelectedIndex =
                    this.ddlDistFlow.SelectedIndex =
                    this.ddlProdFlow.SelectedIndex =
                    this.ddlDistFlowRenewal.SelectedIndex =
                    this.ddlProdFlowRenewal.SelectedIndex =
                    this.ddlProductScheme.SelectedIndex =
                    this.ddlFileLoader.SelectedIndex = 0;
                    this.chkbChargeToIssueBranch.Checked = false;
                    this.chkPrintIssueCard.Checked = true;
                    this.chkChargeFeeAtCardRequest.Checked = false;
                    this.allowM20Print.Checked = false;
                    this.chkbAllowManualUpload.Checked = false;
                    this.chkbAllowReupload.Checked = false;
                    this.chkEmailRequired.Checked = false;
                    this.chkGenerateReferenceNumber.Checked = false;
                    this.chkManualReferenceNumber.Checked = false;
                    this.chkParallelApproval.Checked = false;
                    this.chkCenterOpsActivation.Checked = false;

                    this.tbCreditContractSuffixFormat.Text =
                    this.tbCreditContractPrefix.Text = string.Empty;
                    this.tbCreditContractLastSequence.Text = "0";

                    for (int i = 0; i < this.chklAccounts.Items.Count; i++)
                    {
                        this.chklAccounts.Items[i].Selected = false;
                    }

                    for (int i = 0; i < this.chklIssuingScenarios.Items.Count; i++)
                    {
                        this.chklIssuingScenarios.Items[i].Selected = false;
                    }

                    //foreach (ListItem item in this.chkCurrency.Items)
                    //{
                    //    item.Selected = false;
                    //}
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


        private void ActivateProduct()
        {
            try
            {
                var response = _batchservice.ActivateProduct((int)Productid);
                if (String.IsNullOrWhiteSpace(response))
                {
                    //this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteSuccess").ToString();
                    IsDeleted = false;
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
        #endregion

        #region Page Events
        protected void ddlIssuer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                LoadSchemes();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }

            }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState[PageLayoutKey] != null)
                {
                    pageLayout = (PageLayout)ViewState[PageLayoutKey];
                }
                SessionWrapper.PreviewYN = true;
                SessionWrapper.PresentMode = pageLayout;
                SessionWrapper.ProductlistResult = GetvaluesfromControls();
                if (Productid != null)
                {
                    SessionWrapper.ProductID = (int)Productid;
                    //SessionWrapper.CurrencyList = PopulateCurrencyList();
                    SessionWrapper.ProductlistResult.Product.product_id = (int)Productid;
                }
                Server.Transfer("~\\webpages\\product\\ProductReview.aspx");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }

            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {

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

        protected void btnActivate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                UpdateFormLayout(PageLayout.CONFIRM_ACTIVATE);
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (Validation())
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                if (Validation())
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ISSUER_ADMIN")]
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
                        CreateProduct();
                        UpdateGrid();
                        UpdateDocuments();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_DELETE:
                        DeleteProduct();
                        UpdateGrid();
                        UpdateDocuments();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_UPDATE:
                        UpdateProduct();
                        UpdateGrid();
                        UpdateDocuments();
                        UpdateFormLayout(pageLayout);
                        break;
                    case PageLayout.CONFIRM_ACTIVATE:
                        ActivateProduct();
                        UpdateGrid();
                        UpdateDocuments();
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

        private void UpdateDocuments()
        {
            if (Productid != null)
            {
                foreach (GridViewRow row in grdDocuments.Rows)
                {
                    CheckBox chkInc = (CheckBox)row.FindControl("chkInclude");
                    CheckBox chkReq = (CheckBox)row.FindControl("chkRequired");
                    HiddenField hidId = (HiddenField)row.FindControl("hidDocumentId");
                    HiddenField hidDocTypeId = (HiddenField)row.FindControl("hidDocumentTypeId");

                    ProductDocument toSave = new ProductDocument()
                    {
                        DocumentTypeId = Convert.ToInt32(hidDocTypeId.Value),
                        Id = Convert.ToInt32(hidId.Value),
                        IsActive = chkInc.Checked,
                        IsRequired = chkReq.Checked,
                        ProductId = Convert.ToInt32(Productid.GetValueOrDefault())
                    };

                    bool saveThis = !(toSave.Id == 0 && toSave.IsActive == false);
                    if (saveThis)
                    {
                        _documents.ProductDocumentSave(toSave);
                    }
                }
                LoadDocuments(Convert.ToInt32(Productid.GetValueOrDefault()));
            }
            else
            {
                log.Error("Product Id is null.");
            }
        }

        public long? Productid
        {
            get
            {
                if (ViewState["Productid"] == null)
                    return null;
                else
                    return Convert.ToInt64(ViewState["Productid"].ToString());
            }
            set
            {
                ViewState["Productid"] = value;
            }
        }

        public List<ExternalSystemFieldsResult> ExternalSystemFields
        {
            get
            {
                if (ViewState["ExternalSystemFields"] == null)
                    return null;
                else
                    return (List<ExternalSystemFieldsResult>)ViewState["ExternalSystemFields"];
            }
            set
            {
                ViewState["ExternalSystemFields"] = value;
            }
        }
        public bool? IsDeleted
        {
            get
            {
                if (ViewState["IsDeleted"] == null)
                    return null;
                else
                    return Convert.ToBoolean(ViewState["IsDeleted"].ToString());
            }
            set
            {
                ViewState["IsDeleted"] = value;
            }
        }

        public ProductSearchParameters ProductSearchParameter
        {
            get
            {
                if (ViewState["ProductSearchParameters"] == null)
                    return null;
                else
                    return (ProductSearchParameters)ViewState["ProductSearchParameters"];
            }
            set
            {
                ViewState["ProductSearchParameters"] = value;
            }
        }
        #endregion

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
                    this.btnActivate.Enabled = this.btnActivate.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    //this.btnPreview.Enabled = this.btnPreview.Visible = true;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.READ:
                    DisableFields(false, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnBack.Enabled = this.btnBack.Visible = true;

                    if (IsDeleted == null)
                    {
                        this.btnEdit.Enabled = this.btnEdit.Visible = true;
                        this.btnActivate.Enabled = this.btnActivate.Visible = false;
                        this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    }
                    else if (IsDeleted == true)
                    {
                        this.btnEdit.Enabled = this.btnEdit.Visible = false;
                        this.btnActivate.Enabled = this.btnActivate.Visible = true;
                        this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    }
                    else
                    {
                        this.btnEdit.Enabled = this.btnEdit.Visible = true;
                        this.btnActivate.Enabled = this.btnActivate.Visible = false;
                        this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    }

                    //this.btnDelete.Enabled = this.btnDelete.Visible = true;
                    //this.btnActivate.Enabled = this.btnActivate.Visible = false;
                    //this.btnPreview.Enabled = this.btnPreview.Visible = true;

                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
                    break;
                case PageLayout.UPDATE:
                    EnableFields(false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    //this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    //this.btnActivate.Enabled = this.btnActivate.Visible = false;
                    //this.btnPreview.Enabled = this.btnPreview.Visible = true;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = true;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = false;
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
                    //this.btnPreview.Enabled = this.btnPreview.Visible = true;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnActivate.Enabled = this.btnActivate.Visible = false;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                    //this.btnUpdate.Text = "Confirm";
                    break;
                case PageLayout.CONFIRM_DELETE:
                    DisableFields(true, false);

                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnActivate.Enabled = this.btnActivate.Visible = false;
                    this.btnActivate.Enabled = this.btnActivate.Visible = false;
                    //this.btnPreview.Enabled = this.btnPreview.Visible = false;
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmDeleteMessage").ToString();
                    //this.btnUpdate.Text = "Confirm";
                    break;
                case PageLayout.CONFIRM_ACTIVATE:
                    DisableFields(true, false);

                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnActivate.Enabled = this.btnActivate.Visible = false;
                    //this.btnPreview.Enabled = this.btnPreview.Visible = false;
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = "Confirm Activation of Product";
                    //this.btnUpdate.Text = "Confirm";
                    break;
                case PageLayout.CONFIRM_UPDATE:
                    DisableFields(true, false);
                    this.btnCreate.Enabled = this.btnCreate.Visible = false;
                    //this.btnPreview.Enabled = this.btnPreview.Visible = false;
                    this.btnEdit.Enabled = this.btnEdit.Visible = false;
                    this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
                    this.btnDelete.Enabled = this.btnDelete.Visible = false;
                    this.btnActivate.Enabled = this.btnActivate.Visible = false;
                    this.btnConfirm.Enabled = this.btnConfirm.Visible = true;
                    this.lblInfoMessage.Text = GetLocalResourceObject("ConfirmMessage").ToString();
                    //this.btnUpdate.Text = "Confirm";
                    break;
                default:
                    DisableFields(false, false);
                    //this.btnUpdate.Text = UtilityClass.UppercaseFirst("EDIT");
                    break;
            }

            ViewState[PageLayoutKey] = toPageLayout;
        }

        private bool Validation()
        {
            bool valid = true;
            //if (this.rdbInstantCard.Checked == false && this.rdbClassicCard.Checked == false)
            //{
            //    this.lblErrorMessage.Text = "Please select a card issue method.";
            //    return false;
            //}

            if (this.ddlDistFlow.SelectedValue == "-1")
            {
                valid = false;
                this.lblErrorMessage.Text += "<br/>Please select distribution flow.";
            }

            if (this.ddlProdFlow.SelectedValue == "-1")
            {
                valid = false;
                this.lblErrorMessage.Text += "<br/>Please select production flow.";
            }

            return valid;
        }

        private void EnableFields(bool isCreate)
        {
            this.chkPrintPIN.Enabled =
            this.chkRePrintPin.Enabled =
            this.ddlFileExport.Enabled =
            this.tbProductname.Enabled = true;
            this.tbProductCode.Enabled = true;
            this.tbproductbin.Enabled = true;
            this.ddlIssuer.Enabled = true;
            this.ddlProductScheme.Enabled = true;
            this.ddlDistFlow.Enabled =
            this.ddlProdFlow.Enabled =
            this.ddlDistFlowRenewal.Enabled =
            this.ddlProdFlowRenewal.Enabled =
            this.chkbChargeToIssueBranch.Enabled =
            this.chkPrintIssueCard.Enabled =
            this.chkChargeFeeAtCardRequest.Enabled =
            this.allowM20Print.Enabled =    
            this.chkbAllowManualUpload.Enabled =
            this.chkbAllowReupload.Enabled =
            this.ddlPanLength.Enabled =
            this.tbSubProductCode.Enabled =
            this.ddlCardIssueMethod.Enabled =
            this.ddlPinCalcMethod.Enabled =
            this.chkAutoApproveBatch.Enabled =
            this.chkAccountValidation.Enabled =
            this.chkPinAccountValidation.Enabled =
            this.chkRenewalCharge.Enabled =
            this.chkRenewalActivate.Enabled =
            this.chkCreditLimitUpdate.Enabled =
            this.chkCreditLimitApprove.Enabled =
            this.chkCreditLimitCapture.Enabled =
            this.chkParallelApproval.Enabled =
            this.chkCenterOpsActivation.Enabled =
            this.chkEmailRequired.Enabled =
            this.chkGenerateReferenceNumber.Enabled =
            this.chkManualReferenceNumber.Enabled =
            this.chklAccounts.Enabled =
            this.chklIssuingScenarios.Enabled =
            this.tbCreditContractLastSequence.Enabled =
            this.tbCreditContractPrefix.Enabled =
            this.tbCreditContractSuffixFormat.Enabled =
            this.ddlLoadBatchType.Enabled =
            this.chkCMSExportable.Enabled =
            this.chkremoteCMSEnable.Enabled =
            this.chkInstantPin.Enabled =
            this.chkInstantPinResissue.Enabled =
            this.dlCurrency.Enabled =
            this.tbPVK.Enabled =
            this.tbCVKA.Enabled =
            this.tbCVKB.Enabled =
            this.tbExpiryMonth.Enabled =
            this.ddlPVK1.Enabled =
            this.ddlSRC1.Enabled =
            this.ddlSRC2.Enabled =
            this.ddlSRC3.Enabled =
            this.tbMaxPinLength.Enabled =
            this.tbMinPinLength.Enabled =
            this.tbPinvalidation.Enabled =
            ddlPinblcok.Enabled =
            this.tbDecimalisation.Enabled = true;

            this.ddlInterfaceCBS.Enabled =
            this.ddlInterfaceCMS.Enabled =
            this.ddlInterfaceRemoteCMS.Enabled =
            this.ddlInterfaceCPS.Enabled =
            this.ddlInterfaceFeeScheme.Enabled =
            this.ddlFileExportSettings.Enabled =
           this.ddlInterfaceFileLoader.Enabled =
            this.ddlProductThreedSecure.Enabled =
            this.ddlInterfaceThreed.Enabled =
            this.ddlIssureThreedSecure.Enabled =
            this.ddlIssueCMS.Enabled =
            this.ddlIssuerRemoteCMS.Enabled =
            this.ddlIssueCoreBanking.Enabled =
            this.ddlIssueFeeScheme.Enabled =
            this.ddlProdCMS.Enabled =
            this.ddlProdCPS.Enabled =
            this.ddlFileLoader.Enabled =
            this.ddlProductScheme.Enabled =
            this.ddlexternalsytemsCBS.Enabled =
             this.ddlexternalsytemsCPS.Enabled =
              this.ddlexternalsytemsCMS.Enabled =
               this.ddlexternalsytemsHSM.Enabled =
               this.dlExternalfieldsCBS.Enabled =
               this.dlExternalfieldsCPS.Enabled =
               this.dlExternalfieldsCMS.Enabled =
               this.dlExternalfieldsHSM.Enabled =
               this.Grdaccounttype.Enabled =
               this.ddlFundsPrepaid.Enabled =
               this.ddlFundsCoreBanking.Enabled =
               this.ddlFundsConnectionPrepaid.Enabled =
            this.ddlFundsConnectionCBS.Enabled =
            this.grdDocuments.Enabled =
            this.grdPrintingFields.Enabled = true;

            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnEdit.Enabled = this.btnEdit.Visible = isCreate ? false : true;
            this.btnUpdate.Enabled = this.btnUpdate.Visible = false;
            //this.btnPreview.Enabled = this.btnPreview.Visible = false;

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
                case PageLayout.READ:
                    SessionWrapper.ProductSearchParameter = this.ProductSearchParameter;
                    Server.Transfer("~\\webpages\\product\\ProductList.aspx");
                    break;
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
            this.chkPrintPIN.Enabled =
            this.chkRePrintPin.Enabled =
            this.ddlFileExport.Enabled =
            this.tbProductname.Enabled = false;
            this.tbProductCode.Enabled = false;
            this.tbproductbin.Enabled = false;
            this.ddlIssuer.Enabled = false;
            this.ddlProductScheme.Enabled = false;
            this.ddlProdFlow.Enabled =
            this.ddlDistFlow.Enabled =
            this.ddlProdFlowRenewal.Enabled =
            this.ddlDistFlowRenewal.Enabled =
            this.chkbChargeToIssueBranch.Enabled =
            this.chkPrintIssueCard.Enabled =
            this.chkChargeFeeAtCardRequest.Enabled =
            this.allowM20Print.Enabled =
            this.chkbAllowManualUpload.Enabled =
            this.chkbAllowReupload.Enabled =
            this.ddlPanLength.Enabled =
            this.tbSubProductCode.Enabled =
            this.ddlCardIssueMethod.Enabled =
            this.ddlPinCalcMethod.Enabled =
            this.chkAutoApproveBatch.Enabled =
            this.chkAccountValidation.Enabled =
            this.chkPinAccountValidation.Enabled =
            this.chkRenewalCharge.Enabled =
            this.chkRenewalActivate.Enabled =
            this.chkCreditLimitUpdate.Enabled =
            this.chkCreditLimitApprove.Enabled =
            this.chkCreditLimitCapture.Enabled =
            this.chkParallelApproval.Enabled =
            this.chkCenterOpsActivation.Enabled =
            this.chkEmailRequired.Enabled =
            this.chkGenerateReferenceNumber.Enabled =
            this.chkManualReferenceNumber.Enabled =
            this.tbCreditContractLastSequence.Enabled =
            this.tbCreditContractPrefix.Enabled =
            this.tbCreditContractSuffixFormat.Enabled =
            this.chklAccounts.Enabled =
            this.chklIssuingScenarios.Enabled =

            this.ddlLoadBatchType.Enabled =
            this.chkCMSExportable.Enabled =
            this.chkremoteCMSEnable.Enabled =
            this.chkInstantPin.Enabled =
            this.chkInstantPinResissue.Enabled =
            this.dlCurrency.Enabled =
            this.tbPVK.Enabled =
            this.tbCVKA.Enabled =
            this.tbCVKB.Enabled =
            this.tbExpiryMonth.Enabled =
            this.ddlPVK1.Enabled =
            this.ddlSRC1.Enabled =
            this.ddlSRC2.Enabled =
            this.ddlSRC3.Enabled =
            this.tbMinPinLength.Enabled =
            this.ddlPinblcok.Enabled =
            this.tbMaxPinLength.Enabled = this.tbDecimalisation.Enabled = this.tbPinvalidation.Enabled = false;

            this.ddlInterfaceCBS.Enabled =
            this.ddlInterfaceCMS.Enabled =
            this.ddlInterfaceRemoteCMS.Enabled =
            this.ddlInterfaceCPS.Enabled =
            this.ddlInterfaceFeeScheme.Enabled =
            this.ddlFileExportSettings.Enabled =
           this.ddlInterfaceFileLoader.Enabled =
            this.ddlProductThreedSecure.Enabled =
            this.ddlInterfaceThreed.Enabled =
            this.ddlIssureThreedSecure.Enabled =
            this.ddlIssueCMS.Enabled =
            this.ddlIssuerRemoteCMS.Enabled =
            this.ddlIssueCoreBanking.Enabled =
            this.ddlIssueFeeScheme.Enabled =
            this.ddlProdCMS.Enabled =
            this.ddlProdCPS.Enabled =
            this.ddlFileLoader.Enabled =
            this.ddlProductScheme.Enabled =
             this.ddlexternalsytemsCBS.Enabled =
             this.ddlexternalsytemsCPS.Enabled =
              this.ddlexternalsytemsCMS.Enabled =
               this.ddlexternalsytemsHSM.Enabled =
               this.dlExternalfieldsCBS.Enabled =
               this.dlExternalfieldsCPS.Enabled =
               this.dlExternalfieldsCMS.Enabled =
               this.dlExternalfieldsHSM.Enabled =
               this.Grdaccounttype.Enabled =
               this.ddlFundsCoreBanking.Enabled =
            this.ddlFundsPrepaid.Enabled =
            this.ddlFundsConnectionPrepaid.Enabled =
            this.ddlFundsConnectionCBS.Enabled =
            this.grdDocuments.Enabled =
            this.grdPrintingFields.Enabled = false;

            this.btnBack.Visible = IsConfirm ? true : false;
            this.btnCreate.Enabled = this.btnCreate.Visible = isCreate;
            this.btnDelete.Enabled = this.btnDelete.Visible = isCreate;
            this.btnActivate.Enabled = this.btnActivate.Visible = false;
            //this.btnPreview.Enabled = this.btnPreview.Visible = IsConfirm;

            this.btnUpdate.Enabled = this.btnUpdate.Visible = isCreate ? false : true;
            this.btnConfirm.Enabled = this.btnConfirm.Visible = IsConfirm;



            ddlIssuer.Enabled = isCreate ? true : false;

        }

        #endregion

        #region Print Fields Methods

        /*  Load Print Fields Section */
        private void LoadPrintFields(int productId)
        {
            var results = _batchservice.GetProductPrintingFields(productId, null, null);


            var printResults = ToDataTable(results);

            if (results.Count == 0)
            {
                DataRow row = printResults.NewRow();
                row["field_name"] = "Name On Card";
                row["print_field_type_id"] = 0;
                row["print_field_name"] = "Text";
                row["X"] = 175;
                row["Y"] = 10;
                row["width"] = 0;
                row["height"] = 0;
                row["font"] = "Arial";
                row["font_size"] = 10;
                row["mapped_name"] = "ind_sys_noc";
                row["editable"] = false;
                row["deleted"] = false;
                row["label"] = string.Empty;
                row["max_length"] = 25;
                row["printable"] = false;
                row["printside"] = PrintSide.FRONT.ToString();
                row["product_field_id"] = -1;
                row["product_id"] = 0;
                printResults.Rows.Add(row);

                DataRow row1 = printResults.NewRow();
                row1["field_name"] = "CashLimit";
                row1["print_field_type_id"] = 0;
                row1["print_field_name"] = "Text";
                row1["X"] = 0;
                row1["Y"] = 0;
                row1["width"] = 0;
                row1["height"] = 0;
                row1["font"] = "Arial";
                row1["font_size"] = 10;
                row1["mapped_name"] = "CashLimit";
                row1["editable"] = false;
                row1["deleted"] = false;
                row1["label"] = string.Empty;
                row1["printable"] = false;
                row1["printside"] = PrintSide.NONE.ToString();
                row1["max_length"] = 25;
                row1["product_field_id"] = 0;
                row1["product_id"] = 0;
                printResults.Rows.Add(row1);
            }
            else
            {
                var field = results.FirstOrDefault(i => i.field_name.ToUpper() == "CASHLIMIT");
                if (field == null)
                {
                    DataRow row1 = printResults.NewRow();
                    row1["field_name"] = "CashLimit";
                    row1["print_field_type_id"] = 0;
                    row1["print_field_name"] = "Text";
                    row1["X"] = 0;
                    row1["Y"] = 0;
                    row1["width"] = 0;
                    row1["height"] = 0;
                    row1["font"] = "Arial";
                    row1["font_size"] = 10;
                    row1["mapped_name"] = "CashLimit";
                    row1["editable"] = false;
                    row1["deleted"] = false;
                    row1["label"] = string.Empty;
                    row1["max_length"] = 25;
                    row1["product_field_id"] = 0;
                    row1["product_id"] = 0;
                    row1["printable"] = false;
                    row1["printside"] = PrintSide.NONE.ToString();
                    printResults.Rows.Add(row1);
                }
            }
            ViewState["originalValuesDataTable"] = printResults;
            grdPrintingFields.DataSource = InsertRowAtEnd(printResults);
            grdPrintingFields.DataBind();

            if (!tableCopied)
            {
                originalDataTable = printResults;
                ViewState["originalValuesDataTable"] = printResults;
                tableCopied = true;
            }
        }


        public static DataTable ConvertToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static DataTable ToDataTable<ProductPrintFieldResult>(List<ProductPrintFieldResult> items)
        {
            DataTable dataTable = new DataTable(typeof(ProductPrintFieldResult).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(ProductPrintFieldResult).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (ProductPrintFieldResult item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static DataTable ToDocumentDataTable<ProductDocumentListModel>(List<ProductDocumentListModel> items)
        {
            DataTable dataTable = new DataTable(typeof(ProductDocumentListModel).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(ProductDocumentListModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (ProductDocumentListModel item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        private DataTable InsertRowAtEnd(DataTable dt)
        {

            Random random = new Random();
            int rowid = random.Next(10000000, 99999999);
            DataRow dr = dt.NewRow();
            dr["product_field_id"] = rowid;
            dr["deleted"] = "false";
            dr["editable"] = "false";
            dr["printable"] = false;
            dr["printside"] = PrintSide.NONE.ToString();
            dt.Rows.Add(dr);
            return dt;
        }

        protected void AddUpdateGrid(object sender, GridViewUpdateEventArgs e)
        {

            Control control = null;
            if (grdPrintingFields.FooterRow != null)
            {
                control = grdPrintingFields.FooterRow;
            }
            else
            {
                control = grdPrintingFields.Controls[0].Controls[0];
            }
            try
            {
                GridViewRow r = (GridViewRow)grdPrintingFields.Rows[e.RowIndex];



                //foreach (GridViewRow r in grdPrintingFields.Rows)
                //{
                if (r.RowType == DataControlRowType.DataRow)
                {

                    bool isChecked = r.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked;

                    if (((TextBox)r.FindControl("txt_mapped_name")).Text.ToUpper() == "IND_SYS_NOC" || ((TextBox)r.FindControl("txt_mapped_name")).Text.ToUpper() == "CASHLIMIT")
                        isChecked = true;

                    if (isChecked)
                    {
                        ProductPrintFieldResult field = new ProductPrintFieldResult();

                        //field.product_field_id = 0;
                        if (!string.IsNullOrEmpty(grdPrintingFields.DataKeys[r.RowIndex % grdPrintingFields.PageSize].Value.ToString()))
                        {
                            if (!string.IsNullOrEmpty(grdPrintingFields.DataKeys[r.RowIndex].Value.ToString()))
                            {
                                int product_field_id;
                                int.TryParse(grdPrintingFields.DataKeys[r.RowIndex].Value.ToString(), out product_field_id);
                                field.product_field_id = product_field_id;
                            }
                            else
                                field.product_field_id = string.IsNullOrEmpty(grdPrintingFields.DataKeys[r.RowIndex % grdPrintingFields.PageSize].Value.ToString()) ? 0 : Convert.ToInt32(grdPrintingFields.DataKeys[r.RowIndex % grdPrintingFields.PageSize].Value);

                        }
                        else
                            field.product_field_id = 0;
                        if (Productid != null)
                            field.product_id = (int)Productid;
                        else
                            field.product_id = 0;

                        field.field_name = ((TextBox)r.FindControl("txt_field_name")).Text;

                        int print_field_type_id;
                        int.TryParse(((DropDownList)r.FindControl("ddl_fieldtype")).SelectedValue, out print_field_type_id);
                        field.print_field_type_id = print_field_type_id;

                        field.X = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_X")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("txt_X")).Text);
                        field.Y = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_Y")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("txt_Y")).Text);

                        field.width = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_width")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("txt_width")).Text);
                        field.height = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_height")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("txt_height")).Text);

                        field.font = ((DropDownList)r.FindControl("ddl_font")).SelectedItem.Text;
                        field.font_size = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_font_size")).Text) ? 10 : int.Parse(((TextBox)r.FindControl("txt_font_size")).Text);

                        field.mapped_name = ((TextBox)r.FindControl("txt_mapped_name")).Text;

                        if (!chkAccountValidation.Checked)
                            field.editable = ((CheckBox)r.FindControl("txt_editable")).Checked;
                        else
                            field.editable = true;

                        field.printable = ((CheckBox)r.FindControl("txt_printable")).Checked;
                        field.printside = int.Parse(((DropDownList)r.FindControl("ddl_printside")).SelectedValue);

                        field.deleted = ((CheckBox)r.FindControl("txt_deleted")).Checked;

                        field.label = ((TextBox)r.FindControl("txt_label")).Text ?? string.Empty;
                        field.max_length = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_max_length")).Text) ? 10 : int.Parse(((TextBox)r.FindControl("txt_max_length")).Text);

                        result.Add(field);
                    }
                }
                //}


            }
            catch (Exception ex)
            {
                log.Error(ex);
                lblErrorMessage.Text = ex.Message;
            }
        }

        /* Update Print Fields Section */
        private void UpdateGrid()
        {
            if (Productid != null)
            {

                if (ViewState["originalValuesDataTable"] != null)
                    originalDataTable = (System.Data.DataTable)ViewState["originalValuesDataTable"];

                foreach (GridViewRow r in grdPrintingFields.Rows)
                    if (IsRowModified(r)) { grdPrintingFields.UpdateRow(r.RowIndex, false); }

                string messages;
                if (result.Count > 0)
                    if (_batchservice.UpdateProductPrintingFields(result, out messages))
                    {
                        lblInfoMessage.Text = messages;

                    }
                result.Clear();
                // Rebind the Grid to repopulate the original values table.
                tableCopied = false;
                LoadPrintFields((int)Productid);
            }
            else
            {
                log.Error("Product Id is null.");
            }

        }
        protected bool IsRowModified(GridViewRow r)
        {
            ProductPrintFieldResult field = new ProductPrintFieldResult();

            int product_field_id = 0;
            if (!string.IsNullOrEmpty(grdPrintingFields.DataKeys[r.RowIndex % grdPrintingFields.PageSize].Value.ToString()))
            {
                if (!string.IsNullOrEmpty(grdPrintingFields.DataKeys[r.RowIndex].Value.ToString()))
                {

                    int.TryParse(grdPrintingFields.DataKeys[r.RowIndex].Value.ToString(), out product_field_id);

                }
                else
                    product_field_id = string.IsNullOrEmpty(grdPrintingFields.DataKeys[r.RowIndex % grdPrintingFields.PageSize].Value.ToString()) ? 0 : Convert.ToInt32(grdPrintingFields.DataKeys[r.RowIndex % grdPrintingFields.PageSize].Value);

            }
            else
                product_field_id = 0;

            field.product_field_id = product_field_id;
            if (field.product_field_id == 0)
                return true;

            field.field_name = ((TextBox)r.FindControl("txt_field_name")).Text ?? string.Empty;
            int print_field_type_id;
            int.TryParse(((DropDownList)r.FindControl("ddl_fieldtype")).SelectedValue, out print_field_type_id);
            field.print_field_type_id = print_field_type_id;



            field.X = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_X")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("txt_X")).Text);
            field.Y = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_Y")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("txt_Y")).Text);

            field.width = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_width")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("txt_width")).Text);
            field.height = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_height")).Text) ? 0 : decimal.Parse(((TextBox)r.FindControl("txt_height")).Text);

            field.font = ((DropDownList)r.FindControl("ddl_font")).SelectedItem.Text ?? string.Empty;
            field.font_size = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_font_size")).Text) ? 10 : int.Parse(((TextBox)r.FindControl("txt_font_size")).Text);

            field.mapped_name = ((TextBox)r.FindControl("txt_mapped_name")).Text;

            field.editable = ((CheckBox)r.FindControl("txt_editable")).Checked;

            field.deleted = ((CheckBox)r.FindControl("txt_deleted")).Checked;

            field.printable = ((CheckBox)r.FindControl("txt_printable")).Checked;
            field.printside = int.Parse(((DropDownList)r.FindControl("ddl_printside")).SelectedValue);

            field.label = ((TextBox)r.FindControl("txt_label")).Text ?? string.Empty;
            field.max_length = string.IsNullOrEmpty(((TextBox)r.FindControl("txt_max_length")).Text) ? 10 : int.Parse(((TextBox)r.FindControl("txt_max_length")).Text);

            System.Data.DataRow[] rows = originalDataTable.Select("product_field_id ='" + field.product_field_id + "'");

            if (rows.Length > 0)
            {
                System.Data.DataRow row = rows[0];
                if (!field.field_name.Equals(row["field_name"].ToString())) { return true; }
                if (!field.print_field_type_id.Equals(row["print_field_type_id"].ToString())) { return true; }
                if (!field.X.Equals(row["X"].ToString())) { return true; }
                if (!field.Y.Equals(row["Y"].ToString())) { return true; }
                if (!field.width.Equals(row["width"].ToString())) { return true; }
                if (!field.height.Equals(row["height"].ToString())) { return true; }
                if (!field.font.Equals(row["font"].ToString())) { return true; }
                if (!field.font_size.Equals(row["font_size"].ToString())) { return true; }
                if (!field.mapped_name.Equals(row["mapped_name"].ToString())) { return true; }
                if (!field.editable.Equals(row["editable"].ToString())) { return true; }
                if (!field.editable.Equals(row["printable"].ToString())) { return true; }
                if (!field.editable.Equals(row["printside"].ToString())) { return true; }
                if (!field.label.Equals(row["label"].ToString())) { return true; }
                if (!field.max_length.Equals(row["max_length"].ToString())) { return true; }

            }

            return false;

        }

        /* Add Print Fields Section */
        //private void AddNewRow()
        //{
        //    int rowIndex = 0;

        //    if (ViewState["originalValuesDataTable"] != null)
        //    {
        //        DataTable dtCurrentTable = (DataTable)ViewState["originalValuesDataTable"];
        //        if (dtCurrentTable.Rows.Count > 0)
        //        {
        //            DataRow drCurrentRow = null;
        //            for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
        //            {
        //                TextBox txt_product_field_id = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("product_field_id");
        //                TextBox txt_field_name = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_field_name");
        //                TextBox txt_print_field_type_id = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_print_field_type_id");
        //                TextBox txt_X = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_X");
        //                TextBox txt_Y = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_Y");
        //                TextBox txt_width = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_width");
        //                TextBox txt_height = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_height");
        //                TextBox txt_font = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_font");
        //                TextBox txt_font_size = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_font_size");
        //                TextBox txt_mapped_name = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_mapped_name");
        //                TextBox txt_editable = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_editable");
        //                TextBox txt_deleted = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_deleted");
        //                TextBox txt_label = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_label");
        //                TextBox txt_max_length = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_max_length");
        //                TextBox txt_product_id = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("product_id");

        //                drCurrentRow = dtCurrentTable.NewRow();
        //                drCurrentRow["RowNumber"] = i + 1;

        //                dtCurrentTable.Rows[i - 1]["Col1"] = string.IsNullOrEmpty(txt_product_field_id.Text) ? string.Empty : txt_product_field_id.Text;
        //                dtCurrentTable.Rows[i - 1]["Col2"] = string.IsNullOrEmpty(txt_field_name.Text) ? string.Empty : txt_field_name.Text;
        //                dtCurrentTable.Rows[i - 1]["Col3"] = string.IsNullOrEmpty(txt_print_field_type_id.Text) ? string.Empty : txt_print_field_type_id.Text;
        //                dtCurrentTable.Rows[i - 1]["Col4"] = string.IsNullOrEmpty(txt_X.Text) ? string.Empty : txt_X.Text;
        //                dtCurrentTable.Rows[i - 1]["Col5"] = string.IsNullOrEmpty(txt_Y.Text) ? string.Empty : txt_Y.Text;
        //                dtCurrentTable.Rows[i - 1]["Col6"] = string.IsNullOrEmpty(txt_width.Text) ? string.Empty : txt_width.Text;
        //                dtCurrentTable.Rows[i - 1]["Col7"] = string.IsNullOrEmpty(txt_height.Text) ? string.Empty : txt_height.Text;
        //                dtCurrentTable.Rows[i - 1]["Col8"] = string.IsNullOrEmpty(txt_font.Text) ? string.Empty : txt_font.Text;
        //                dtCurrentTable.Rows[i - 1]["Col9"] = string.IsNullOrEmpty(txt_font_size.Text) ? string.Empty : txt_font_size.Text;
        //                dtCurrentTable.Rows[i - 1]["Col10"] = string.IsNullOrEmpty(txt_mapped_name.Text) ? string.Empty : txt_mapped_name.Text;
        //                dtCurrentTable.Rows[i - 1]["Col11"] = string.IsNullOrEmpty(txt_editable.Text) ? string.Empty : txt_editable.Text;
        //                dtCurrentTable.Rows[i - 1]["Col12"] = string.IsNullOrEmpty(txt_deleted.Text) ? string.Empty : txt_deleted.Text;
        //                dtCurrentTable.Rows[i - 1]["Col13"] = string.IsNullOrEmpty(txt_label.Text) ? string.Empty : txt_label.Text;
        //                dtCurrentTable.Rows[i - 1]["Col14"] = string.IsNullOrEmpty(txt_max_length.Text) ? string.Empty : txt_max_length.Text;
        //                dtCurrentTable.Rows[i - 1]["Col15"] = string.IsNullOrEmpty(Productid.ToString()) ? string.Empty : Productid.ToString();

        //                rowIndex++;
        //            }

        //            dtCurrentTable.Rows.Add(drCurrentRow);
        //            ViewState["CurrentTable"] = dtCurrentTable;

        //            grdPrintingFields.DataSource = dtCurrentTable;
        //            grdPrintingFields.DataBind();
        //        }
        //    }
        //    else
        //    {
        //        log.Error("ViewState is null");
        //    }

        //    SetPreviousData();
        //    //AddUpdateGrid();
        //}
        //private void SetPreviousData()
        //{
        //    int rowIndex = 0;
        //    if (ViewState["originalValuesDataTable"] != null)
        //    {
        //        DataTable dt = (DataTable)ViewState["originalValuesDataTable"];
        //        if (dt.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {

        //                TextBox txt_product_field_id = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("product_field_id");
        //                TextBox txt_field_name = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_field_name");
        //                TextBox txt_print_field_type_id = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_print_field_type_id");
        //                TextBox txt_X = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_X");
        //                TextBox txt_Y = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_Y");
        //                TextBox txt_width = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_width");
        //                TextBox txt_height = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_height");
        //                TextBox txt_font = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_font");
        //                TextBox txt_font_size = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_font_size");
        //                TextBox txt_mapped_name = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_mapped_name");
        //                TextBox txt_editable = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_editable");
        //                TextBox txt_deleted = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_deleted");
        //                TextBox txt_label = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_label");
        //                TextBox txt_max_length = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("txt_max_length");
        //                TextBox txt_product_id = (TextBox)grdPrintingFields.Rows[rowIndex].Cells[1].FindControl("product_id");

        //                txt_product_field_id.Text = dt.Rows[i]["Col1"].ToString();
        //                txt_field_name.Text = dt.Rows[i]["Col2"].ToString();
        //                txt_print_field_type_id.Text = dt.Rows[i]["Col3"].ToString();
        //                txt_X.Text = dt.Rows[i]["Col4"].ToString();
        //                txt_Y.Text = dt.Rows[i]["Col5"].ToString();
        //                txt_width.Text = dt.Rows[i]["Col6"].ToString();
        //                txt_height.Text = dt.Rows[i]["Col7"].ToString();
        //                txt_font.Text = dt.Rows[i]["Col8"].ToString();
        //                txt_font_size.Text = dt.Rows[i]["Col9"].ToString();
        //                txt_mapped_name.Text = dt.Rows[i]["Col10"].ToString();
        //                txt_editable.Text = dt.Rows[i]["Col11"].ToString();
        //                txt_deleted.Text = dt.Rows[i]["Col12"].ToString();
        //                txt_label.Text = dt.Rows[i]["Col13"].ToString();
        //                txt_max_length.Text = dt.Rows[i]["Col14"].ToString();
        //                txt_product_id.Text = Productid.ToString();
        //                rowIndex++;
        //            }
        //        }
        //    }
        //}
        //private void SetRowData()
        //{
        //    int rowIndex = 0;

        //    if (ViewState["originalValuesDataTable"] != null)
        //    {
        //        DataTable dtCurrentTable = (DataTable)ViewState["originalValuesDataTable"];
        //        DataRow drCurrentRow = null;
        //        if (dtCurrentTable.Rows.Count > 0)
        //        {
        //            for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
        //            {
        //                //TextBox txt_product_field_id = grdPrintingFields.FindControl("txt_product_field_id") as TextBox;
        //                TextBox txt_field_name = grdPrintingFields.FindControl("txt_field_name") as TextBox;
        //                TextBox txt_print_field_type_id = grdPrintingFields.FindControl("txt_print_field_type_id") as TextBox;
        //                TextBox txt_X = grdPrintingFields.FindControl("txt_X") as TextBox;
        //                TextBox txt_Y = grdPrintingFields.FindControl("txt_Y") as TextBox;
        //                TextBox txt_width = grdPrintingFields.FindControl("txt_width") as TextBox;
        //                TextBox txt_height = grdPrintingFields.FindControl("txt_height") as TextBox;
        //                TextBox txt_font = grdPrintingFields.FindControl("txt_font") as TextBox;
        //                TextBox txt_font_size = grdPrintingFields.FindControl("txt_font_size") as TextBox;
        //                TextBox txt_mapped_name = grdPrintingFields.FindControl("txt_mapped_name") as TextBox;
        //                TextBox txt_editable = grdPrintingFields.FindControl("txt_editable") as TextBox;
        //                TextBox txt_deleted = grdPrintingFields.FindControl("txt_deleted") as TextBox;
        //                TextBox txt_label = grdPrintingFields.FindControl("txt_label") as TextBox;
        //                TextBox txt_max_length = grdPrintingFields.FindControl("txt_max_length") as TextBox;


        //                drCurrentRow = dtCurrentTable.NewRow();
        //                drCurrentRow["RowNumber"] = i + 1;

        //                //dtCurrentTable.Rows[i - 1]["Col1"] = txt_product_field_id.Text;
        //                dtCurrentTable.Rows[i - 1]["Col2"] = txt_field_name.Text;
        //                dtCurrentTable.Rows[i - 1]["Col3"] = txt_print_field_type_id.Text;
        //                dtCurrentTable.Rows[i - 1]["Col4"] = txt_X.Text;
        //                dtCurrentTable.Rows[i - 1]["Col5"] = txt_Y.Text;

        //                dtCurrentTable.Rows[i - 1]["Col1"] = txt_width.Text;
        //                dtCurrentTable.Rows[i - 1]["Col2"] = txt_height.Text;
        //                dtCurrentTable.Rows[i - 1]["Col3"] = txt_font.Text;
        //                dtCurrentTable.Rows[i - 1]["Col4"] = txt_font_size.Text;
        //                dtCurrentTable.Rows[i - 1]["Col5"] = txt_mapped_name.Text;

        //                dtCurrentTable.Rows[i - 1]["Col1"] = txt_editable.Text;
        //                dtCurrentTable.Rows[i - 1]["Col2"] = txt_deleted.Text;
        //                dtCurrentTable.Rows[i - 1]["Col3"] = txt_label.Text;
        //                dtCurrentTable.Rows[i - 1]["Col4"] = txt_max_length.Text;
        //                dtCurrentTable.Rows[i - 1]["Col5"] = Productid.ToString();

        //                rowIndex++;
        //            }

        //            ViewState["originalValuesDataTable"] = dtCurrentTable;
        //        }
        //    }
        //    else
        //    {
        //        log.Error("ViewState is null");
        //    }
        //}

        #endregion

        #region Print Field Events

        protected void grdPrintingFields_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grdPrintingFields.EditIndex = e.NewEditIndex;
            LoadPrintFields((int)Productid);
        }

        protected void grdPrintingFields_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grdPrintingFields.EditIndex = -1;
            LoadPrintFields((int)Productid);
        }

        protected void grdPrintingFields_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            AddUpdateGrid(sender, e);
        }

        protected void grdPrintingFields_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {



                //if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    DropDownList ddl_font = (DropDownList)e.Row.FindControl("ddl_font");
                    List<ListItem> fonts = new List<ListItem>();
                    foreach (var item in _batchservice.GetFontFamilyList())
                    {
                        fonts.Add(new ListItem(item.font_name, item.font_id.ToString()));
                    }
                    ddl_font.Items.AddRange(fonts.OrderBy(m => m.Text).ToArray());
                    ddl_font.SelectedIndex = 0;


                    DropDownList ddl_fieldtype = (DropDownList)e.Row.FindControl("ddl_fieldtype");

                    List<ListItem> list = new List<ListItem>();
                    foreach (var item in _batchservice.LookupPrintFieldTypes())
                    {
                        list.Add(new ListItem(item.language_text, item.lookup_id.ToString()));
                    }
                    ddl_fieldtype.Items.AddRange(list.OrderBy(m => m.Value).ToArray());
                    ddl_fieldtype.SelectedIndex = 0;

                    DropDownList ddl_printside = (DropDownList)e.Row.FindControl("ddl_printside");

                    list = new List<ListItem>();
                    foreach (var item in Enum.GetValues(typeof(PrintSide)))
                    {
                        list.Add(new ListItem(Enum.GetName(typeof(PrintSide), item), ((int)item).ToString()));
                    }
                    ddl_printside.Items.AddRange(list.OrderBy(m => m.Value).ToArray());
                    ddl_printside.SelectedIndex = 0;

                    Label lbl_printside = (Label)e.Row.FindControl("lbl_printside");

                    DataRowView dr = e.Row.DataItem as DataRowView;
                    //ddList.SelectedItem.Text = dr["category_name"].ToString();
                    ddl_font.SelectedValue = dr.Row.ItemArray[6].ToString();
                    ddl_fieldtype.SelectedValue = dr.Row.ItemArray[1].ToString();
                    ddl_printside.SelectedValue = dr.Row.ItemArray[18].ToString();
                    if (lbl_printside != null)
                    {

                        PrintSide printSide = (PrintSide)Enum.Parse(typeof(PrintSide), dr.Row.ItemArray[18].ToString());
                        lbl_printside.Text = printSide.ToString();

                    }
                }
            }
        }

        protected void Unnamed_CheckedChanged(object sender, EventArgs e)
        {
            bool isUpdateVisible = false;
            CheckBox chk = (sender as CheckBox);

            if (chk.ID == "chkAll")
            {
                foreach (GridViewRow row in grdPrintingFields.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked = chk.Checked;
                    }
                }
            }

            CheckBox chkAll = (grdPrintingFields.HeaderRow.FindControl("chkAll") as CheckBox);
            chkAll.Checked = true;

            foreach (GridViewRow row in grdPrintingFields.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    bool isChecked = row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked;

                    for (int i = 1; i < row.Cells.Count; i++)
                    {
                        row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Visible = !isChecked;

                        if (row.Cells[i].Controls.OfType<TextBox>().ToList().Count > 0)
                        {

                            row.Cells[i].Controls.OfType<TextBox>().FirstOrDefault().Visible = isChecked;
                            row.Cells[i].Controls.OfType<TextBox>().FirstOrDefault().Enabled = isChecked;


                            row.Cells[i].Controls.OfType<Label>().FirstOrDefault().Visible = !isChecked;
                            row.Cells[i].Controls.OfType<Label>().FirstOrDefault().Enabled = !isChecked;
                        }
                        if (row.Cells[i].Controls.OfType<CheckBox>().ToList().Count > 0)
                        {

                            ((CheckBox)row.Cells[i].FindControl("chk_editable")).Visible = !isChecked;
                            ((CheckBox)row.Cells[i].FindControl("chk_deleted")).Visible = !isChecked;
                            ((CheckBox)row.Cells[i].FindControl("chk_printable")).Visible = !isChecked;

                            ((CheckBox)row.Cells[i].FindControl("chk_editable")).Enabled = !isChecked;
                            ((CheckBox)row.Cells[i].FindControl("chk_deleted")).Enabled = !isChecked;
                            ((CheckBox)row.Cells[i].FindControl("chk_printable")).Enabled = !isChecked;


                            ((CheckBox)row.Cells[i].FindControl("txt_deleted")).Visible = isChecked;
                            ((CheckBox)row.Cells[i].FindControl("txt_editable")).Visible = isChecked;
                            ((CheckBox)row.Cells[i].FindControl("txt_printable")).Visible = isChecked;

                            ((CheckBox)row.Cells[i].FindControl("txt_deleted")).Enabled = isChecked;
                            ((CheckBox)row.Cells[i].FindControl("txt_editable")).Enabled = isChecked;
                            ((CheckBox)row.Cells[i].FindControl("txt_printable")).Enabled = isChecked;
                            //row.Cells[i].Controls.OfType<CheckBox>().FirstOrDefault().Enabled = isChecked;
                            //row.Cells[i].Controls.OfType<Label>().FirstOrDefault().Visible = !isChecked;
                            //row.Cells[i].Controls.OfType<Label>().FirstOrDefault().Enabled = !isChecked;

                        }
                        if (row.Cells[i].Controls.OfType<DropDownList>().ToList().Count > 0)
                        {

                            row.Cells[i].Controls.OfType<DropDownList>().FirstOrDefault().Visible = isChecked;
                            row.Cells[i].Controls.OfType<DropDownList>().FirstOrDefault().Enabled = isChecked;


                            row.Cells[i].Controls.OfType<Label>().FirstOrDefault().Visible = !isChecked;
                            row.Cells[i].Controls.OfType<Label>().FirstOrDefault().Enabled = !isChecked;
                        }
                        if (isChecked && !isUpdateVisible)
                        {
                            isUpdateVisible = true;
                        }

                        if (!isChecked)
                        {
                            chkAll.Checked = false;
                        }
                    }
                }
            }

            UpdateFormLayout(PageLayout.UPDATE);
        }

        protected void ddlCardIssueMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDistFlows(int.Parse(this.ddlCardIssueMethod.SelectedValue));
        }

        protected void ddlexternalsytemsHSM_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDataGrid(ddlexternalsytemsHSM, dlExternalfieldsHSM, true, null);

        }

        protected void ddlexternalsytemsCMS_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDataGrid(ddlexternalsytemsCMS, dlExternalfieldsCMS, true, null);

        }

        protected void ddlexternalsytemsCBS_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDataGrid(ddlexternalsytemsCBS, dlExternalfieldsCBS, true, null);

        }

        protected void ddlexternalsytemsCPS_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDataGrid(ddlexternalsytemsCPS, dlExternalfieldsCPS, true, null);
        }
        protected void BindDataGrid(DropDownList ddlexternalsystem, DataList dlexternalfields, bool autopostback, ProductExternalSystemResult[] productExternalSystemFields)
        {
            int external_system_id = 0;

            int.TryParse(ddlexternalsystem.SelectedValue, out external_system_id);
            if (external_system_id > 0)
            {
                if (Productid > 0 && !autopostback && productExternalSystemFields != null)
                {

                    var ExternalSystemFieldsList = productExternalSystemFields.Where(i => i.external_system_id == external_system_id);

                    dlexternalfields.DataSource = ExternalSystemFieldsList.ToList();
                    dlexternalfields.DataBind();
                    dlexternalfields.Visible = true;
                }
                else if (ExternalSystemFields != null)
                {
                    List<ExternalSystemFieldsResult> ExternalSystemFieldsList = ExternalSystemFields.Where(i => i.external_system_id == external_system_id).ToList();
                    DataTable dt = ConvertToDataTable<ExternalSystemFieldsResult>(ExternalSystemFieldsList);
                    dt.Columns.Add("field_value", typeof(string));
                    dlexternalfields.DataSource = dt;
                    dlexternalfields.DataBind();
                    dlexternalfields.Visible = true;
                }

            }
            else
            {
                dlexternalfields.DataSource = null;
                dlexternalfields.DataBind();
                dlexternalfields.Visible = false;
            }
        }
        //protected void btnAddRow_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        AddNewRow();
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        lblErrorMessage.Text = ex.Message;
        //    }
        //}

        #endregion

        #region "GRID EVENTS"



        protected void Grdaccounttype_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && LoadDataEmpty)
            {
                e.Row.Visible = false;
            }
        }
        private void BindGridView(List<ProductAccountTypeMapping> accounttypes_Mappings)
        {
            if (accounttypes_Mappings == null)
            {
                accounttypes_Mappings = new List<ProductAccountTypeMapping>();

            }

            //Declare a datatable for the gridview
            DataTable dt = ToDataTable<ProductAccountTypeMapping>(accounttypes_Mappings);
            dt.Columns.Add("Id", typeof(int));
            foreach (DataRow row in dt.Rows)
            {
                row["Id"] = 1;

            }
            if (dt.Rows.Count == 0)
            {
                dt.Rows.Add(dt.NewRow());
                LoadDataEmpty = true;
            }

            Grdaccounttype.DataSource = dt;
            Grdaccounttype.DataBind();
            //Now hide the extra row of the grid view
            //Grdaccounttype.Rows[0].Visible = false;
            //Delete row 0 from the datatable

            //View the datatable to the viewstate
            AccounTypes = dt;
        }
        protected void EditRow(object sender, GridViewEditEventArgs e)
        {

            Grdaccounttype.EditIndex = e.NewEditIndex;
            //Now bind the gridview
            Grdaccounttype.DataSource = AccounTypes;
            Grdaccounttype.DataBind();
        }

        public DataTable AccounTypes
        {
            get
            {
                if (ViewState["AccounTypes"] == null)
                    return null;
                else
                    return (DataTable)(ViewState["AccounTypes"]);
            }
            set
            {
                ViewState["AccounTypes"] = value;
            }
        }

        protected void DeleteRow(object sender, GridViewDeleteEventArgs e)
        {
            AccounTypes.Rows[e.RowIndex].Delete();
            AccounTypes.AcceptChanges();

            if (AccounTypes.Rows.Count == 0)
            {
                AccounTypes.Rows.Add(AccounTypes.NewRow());
                LoadDataEmpty = true;
            }


            Grdaccounttype.DataSource = AccounTypes;
            Grdaccounttype.DataBind();

        }

        protected void Grdaccounttype_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToUpper() == "CREATE")
            {
                GridViewRow row = Grdaccounttype.FooterRow as GridViewRow;
                TextBox tbcbs_account_type = row.FindControl("tbCBSAccountType") as TextBox;
                TextBox tbcms_accounttype = row.FindControl("tbCMSAccountType") as TextBox;
                TextBox tbIndigoAccountType = row.FindControl("tbIndigoAccountType") as TextBox;

                if (tbcbs_account_type.Text != string.Empty)
                {


                    Random random = new Random();
                    int rowid = random.Next(10000000, 99999999);

                    DataRow dr = AccounTypes.NewRow();
                    dr["Id"] = rowid;
                    dr["CbsAccountType"] = tbcbs_account_type.Text;
                    dr["CmsAccountType"] = tbcms_accounttype.Text;
                    dr["IndigoAccountTypeId"] = tbIndigoAccountType.Text;

                    AccounTypes.Rows.Add(dr);
                    foreach (DataRow dtrow in AccounTypes.Rows)
                    {
                        if (dtrow["Id"] == null || dtrow["Id"].ToString() == string.Empty)
                        {
                            dtrow.Delete();
                            break;
                        }
                    }
                    AccounTypes.AcceptChanges();

                    Grdaccounttype.DataSource = AccounTypes;
                    Grdaccounttype.DataBind();
                }
                else
                {
                    lblErrorMessage.Text = "Field Name Required.";
                }
            }

        }

        protected void CancelEditRow(object sender, GridViewCancelEditEventArgs e)
        {

            Grdaccounttype.EditIndex = -1;
            Grdaccounttype.DataSource = AccounTypes;
            Grdaccounttype.DataBind();

        }

        protected void UpdateRow(object sendedr, GridViewUpdateEventArgs e)
        {

            var external_system_field_id = Grdaccounttype.DataKeys[e.RowIndex].Value;

            GridViewRow row = Grdaccounttype.Rows[e.RowIndex] as GridViewRow;

            TextBox tbCBSAccountType = row.FindControl("tbCBSAccountType") as TextBox;
            TextBox tbCMSAccountType = row.FindControl("tbCMSAccountType") as TextBox;
            TextBox tbIndigoAccountType = row.FindControl("tbIndigoAccountType") as TextBox;

            if (tbCBSAccountType.Text != string.Empty)
            {

                DataRow dr = AccounTypes.Rows[e.RowIndex];

                dr.BeginEdit();
                dr["CbsAccountType"] = tbCBSAccountType.Text;
                dr["CmsAccountType"] = tbCMSAccountType.Text;
                dr["IndigoAccountTypeId"] = tbIndigoAccountType.Text;

                dr.EndEdit();
                dr.AcceptChanges();
                Grdaccounttype.EditIndex = -1;
                //Now bind the datatable to the gridview
                Grdaccounttype.DataSource = AccounTypes;
                Grdaccounttype.DataBind();
            }
            else
            {
                lblErrorMessage.Text = "Field Name Required.";
            }


        }
        protected void ChangePage(object sender, GridViewPageEventArgs e)
        {

            Grdaccounttype.EditIndex = -1;
            Grdaccounttype.DataSource = AccounTypes;
            Grdaccounttype.DataBind();

        }

        private ProductAccountTypeMapping[] PopulateAccountTypes()
        {
            List<ProductAccountTypeMapping> account_type_mappings_list = new List<ProductAccountTypeMapping>();
            //foreach (GridViewRow row in Grdaccounttype.Rows)
            //{

            //    ProductAccountTypeMapping account_type_mapping = new ProductAccountTypeMapping();

            //    account_type_mapping.CbsAccountType = row.Cells[0].Text.ToString();
            //    if(string.IsNullOrEmpty(row.Cells[1].ToString()))
            //    account_type_mapping.IndigoAccountTypeId = int.Parse( row.Cells[1].Text.ToString());

            //    account_type_mapping.CmsAccountType = row.Cells[2].Text.ToString();


            //    account_type_mappings_list.Add(account_type_mapping);
            //}
            foreach (DataRow row in AccounTypes.Rows)
            {

                ProductAccountTypeMapping account_type_mapping = new ProductAccountTypeMapping();

                account_type_mapping.CbsAccountType = row["CbsAccountType"].ToString();
                if (!string.IsNullOrEmpty(row["IndigoAccountTypeId"].ToString()))
                    account_type_mapping.IndigoAccountTypeId = int.Parse(row["IndigoAccountTypeId"].ToString());
                else
                    account_type_mapping.IndigoAccountTypeId = -1;

                account_type_mapping.CmsAccountType = row["CmsAccountType"].ToString();


                account_type_mappings_list.Add(account_type_mapping);
            }


            return account_type_mappings_list.ToArray();
        }
        #endregion

        protected void grdDocuments_RowEditing(object sender, GridViewEditEventArgs e)
        {
            grdDocuments.EditIndex = e.NewEditIndex;
            LoadDocuments((int)Productid);
        }

        protected void grdDocuments_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grdDocuments.EditIndex = -1;
            LoadDocuments((int)Productid);
        }

        protected void grdDocuments_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            AddUpdateGrid(sender, e);
        }

        protected void grdDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

            }
        }
    }
}