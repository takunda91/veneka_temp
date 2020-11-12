using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.SearchParameters;
using Newtonsoft.Json;
using Common.Logging;
using System.Text;
using indigoCardIssuingWeb.App_Code.utility.Objects;
using indigoCardIssuingWeb.utility;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Security.Permissions;

namespace indigoCardIssuingWeb.webpages.card
{
    public partial class CardCheckInOut : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CardCheckInOut));
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_CUSTODIAN };

        private enum PickListNames
        {
            CheckedInCards,
            CheckedOutCards
        }

        private List<HtmlSelectOption> cardsInOpt;
        private List<HtmlSelectOption> cardsOutOpt;
        private List<SearchBranchCardsResult> successCheckedOutCards;

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!IsPostBack)
            {
                LogGuid = Guid.NewGuid().ToString();
                this.pnlPage.Visible = false;
                this.btnUpdate.Attributes.Add("style", "display:none");
                this.btnConfirm.Attributes.Add("style", "display:none");
                this.btnCancel.Attributes.Add("style", "display:none");
                this.hdnConfirmMsg.Value = Resources.CommonInfoMessages.ReviewCommitMessage;
                LoadPageData();
            }
        }

        #region Helper Methods
        private void LoadPageData()
        {
            try
            {
                this.ddlIssuer.Items.Clear();
                this.ddlBranch.Items.Clear();
                this.ddlOperator.Items.Clear();

                Dictionary<int, ListItem> issuersList = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> roleIssuerList = new Dictionary<int, RolesIssuerResult>();
                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuersList, out roleIssuerList);

                hdnsourcecaption.Value = GetLocalResourceObject("SourceCaption").ToString();
                hdntargetcaption.Value = GetLocalResourceObject("TargetCaption").ToString();

                RoleIssuerList = roleIssuerList;

                try
                {
                    this.ddlIssuer.Items.AddRange(issuersList.Values.OrderBy(m => m.Text).ToArray());
                    this.ddlIssuer.SelectedIndex = 0;

                    UpdateBranchList(int.Parse(this.ddlIssuer.SelectedValue));
                    UpdateProductList(int.Parse(this.ddlIssuer.SelectedValue));

                    UpdateCheckedInList(null, null);
                    UpdateCheckedoutList(null, null);
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
            catch (Exception ex)
            {
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

        /// <summary>
        /// This method populates the branch drop down based on the selected issuer id.
        /// </summary>
        /// <param name="issuerId"></param>
        private void UpdateBranchList(int issuerId)
        {
            this.ddlBranch.Items.Clear();
            this.ddlOperator.Items.Clear();

            var branches = _userMan.GetBranchesForUser(issuerId, userRolesForPage[0], null);

            if (branches.Count > 0)
            {
                List<ListItem> branchList = new List<ListItem>();

                foreach (var item in branches)//Convert branches in item list.
                {
                    branchList.Add(utility.UtilityClass.FormatListItem<int>(item.branch_name, item.branch_code, item.branch_id));
                }

                if (branchList.Count > 0)
                {
                    ddlBranch.Items.AddRange(branchList.OrderBy(m => m.Text).ToArray());
                    ddlBranch.SelectedIndex = 0;

                    UpdateOperatorsList(int.Parse(this.ddlBranch.SelectedValue));                    
                }
            }
            else
            {
                this.lblInfoMessage.Text = GetLocalResourceObject("InfoNoBranches").ToString();
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
                if (_batchService.GetProductsListValidated(issuerId, 1, 1, 1000, out products, out messages))
                {
                    List<ListItem> productsList = new List<ListItem>();
                    Dictionary<int, ProductValidated> productDict = new Dictionary<int, ProductValidated>();

                    foreach (var product in products)
                    {
                        if (!productDict.ContainsKey(product.ProductId))
                            productDict.Add(product.ProductId, product);

                        productsList.Add(utility.UtilityClass.FormatListItem<int>(product.ProductName, product.ProductCode, product.ProductId));
                    }

                    //Products = productDict;

                    if (productsList.Count > 0)
                    {
                        this.ddlProduct.Items.AddRange(productsList.OrderBy(m => m.Text).ToArray());
                        this.ddlProduct.SelectedIndex = 0;

                        this.ddlProduct.Visible = true;
                        this.lblCardProduct.Visible = true;

                        //Update the card list based on the product
                        //if (ValidateProductLicenced() && this.ddlBranch.Items.Count > 0)
                        //    UpdateSubProductList(int.Parse(this.ddlIssuer.SelectedValue), int.Parse(this.ddlProduct.SelectedValue));

                        //UpdateCardList(int.Parse(this.ddlBranch.SelectedValue), int.Parse(this.ddlProduct.SelectedValue));

                        //this.pnlButtons.Visible = true;
                        //this.pnlCustomerDetail.Visible = true;
                    }
                }
                else
                {
                    this.lblErrorMessage.Text = messages;
                    this.pnlButtons.Visible = false;
                }
            }
        }

        /// <summary>
        /// This method populates the Operator drop down list.
        /// </summary>
        /// <param name="branchId"></param>
        private void UpdateOperatorsList(int branchId)
        {
            this.ddlOperator.Items.Clear();
            //CardCheckedOut = null;
            CardCheckedIn = null;
            //UpdateCheckedInList(null, null);
            //UpdateCheckedoutList(null, null);            

            if (branchId >= 0)
            {
                var operators = _userMan.GetUsersByBranch(null, branchId, BranchStatus.ACTIVE, 0, UserRole.BRANCH_OPERATOR, null, null, null, 1, 1000);

                if (operators.Count > 0)
                {
                    List<ListItem> operatorList = new List<ListItem>();

                    foreach (var item in operators)//Convert to item list.
                    {
                        operatorList.Add(new ListItem(item.username, item.user_id.ToString()));
                    }

                    if (operatorList.Count > 0)
                    {
                        this.ddlOperator.Items.AddRange(operatorList.OrderBy(m => m.Text).ToArray());
                        ddlOperator.SelectedIndex = 0;

                        //UpdateCheckedInList(null);
                        //UpdateCheckedoutList(int.Parse(this.ddlBranch.SelectedValue), long.Parse(this.ddlOperator.SelectedValue));
                    }
                }
            }
        }

        /// <summary>
        /// This method populates the check in card list.
        /// </summary>
        /// <param name="branchId"></param>
        private void UpdateCheckedInList(int? branchId, List<long> exclusionList, List<HtmlSelectOption> persistCheckIn = null)
        {
            if (exclusionList == null)
                exclusionList = new List<long>();

            if (persistCheckIn == null)
                persistCheckIn = new List<HtmlSelectOption>();

            List<CardSearchResult> cards = new List<CardSearchResult>();
            int productId;

            //if (CardCheckedIn != null && CardCheckedIn.Count > 0)
            //{
            //    cards = CardCheckedIn.Values.ToList();
            //}


            if (branchId != null &&
                    branchId.GetValueOrDefault() >= 0 &&
                    int.TryParse(this.ddlProduct.SelectedValue, out productId))
            {
                string cardNumber = null;
                if (!String.IsNullOrWhiteSpace(this.txbCardFilter.Text))
                    cardNumber = this.txbCardFilter.Text.Trim().Replace("?", "%");

                string messages;
                cards = _batchService.SearchBranchCards(null, branchId.GetValueOrDefault(), null, productId, null, null, cardNumber, (int)BranchCardStatus.CHECKED_IN, null, PageIndex, RowsPerPage, out messages);

                if (!String.IsNullOrWhiteSpace(messages))
                {
                    this.lblErrorMessage.Text = messages;
                    //return;
                }

                if (cards.Count > 0)
                {
                    this.pnlPage.Visible = true;
                    TotalPages = cards[0].TOTAL_PAGES;
                    lblPageIndex.Text = String.Format("{0}/{1}", PageIndex, TotalPages);
                }
                else
                {
                    this.pnlPage.Visible = false;
                    lblPageIndex.Text = String.Format("{0}/{1}", 0, 0);
                }

                //Decide wheather to show card or reference number
                //var issuer = _issuerMan.GetIssuer(int.Parse(this.ddlIssuer.SelectedValue));
                //foreach (var item in cards)
                //{
                //    if (issuer.Issuer.card_ref_preference == false)
                //    {
                //        item.card_number = item.card_number;
                //    }
                //    else if (issuer.Issuer.card_ref_preference == true)
                //    {
                //        item.card_number = item.card_request_reference;
                //    }
                //}
            }

            Dictionary<long, string> cardList = new Dictionary<long, string>();
            this.sourceSel.Value = "";

            cardsInOpt = new List<HtmlSelectOption>();

            foreach (var card in cards)
            {
                //Dont show cards that have been moved to checked out
                if (exclusionList != null && exclusionList.Exists(e => e == card.card_id))
                    continue;

                if (RoleIssuerList[int.Parse(this.ddlIssuer.SelectedValue)].card_ref_preference)
                    cardsInOpt.Add(new HtmlSelectOption(utility.UtilityClass.FormatPAN(card.card_number), card.card_id.ToString()));
                else
                    cardsInOpt.Add(new HtmlSelectOption(card.card_request_reference, card.card_id.ToString()));

                cardList.Add(card.card_id, card.card_request_reference);
            }

            if (persistCheckIn.Count > 0)
            {
                foreach (var perCard in persistCheckIn)
                    if (!cardsInOpt.Exists(e => e.value == perCard.value))
                        cardsInOpt.Add(perCard);
            }

            CardCheckedIn = cardList;

            //if(cardsInOpt.Count == 0)
            //{
            //    cardsInOpt.Add(new HtmlSelectOption("No cards to display or already in checked out list.", "-1"));
            //}

            //BuildDataForPickList(PickListNames.CheckedInCards, cardsInOpt);
        }        

        /// <summary>
        /// This method populates the checked out card list.
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="userId"></param>
        private void UpdateCheckedoutList(int? branchId, long? userId)
        {
            List<CardSearchResult> cards = new List<CardSearchResult>();
            int productId;

            //if (CardCheckedOut != null && CardCheckedOut.Count > 0)
            //{
            //    cards = CardCheckedOut.Values.ToList();
            //}

            if (branchId != null && branchId >= 0 && 
                    userId.GetValueOrDefault() >= 0 &&
                    int.TryParse(this.ddlProduct.SelectedValue, out productId))
            {
                string messages;
                cards = _batchService.SearchBranchCards(null, branchId.GetValueOrDefault(), null, productId, null, null, null, (int)BranchCardStatus.AVAILABLE_FOR_ISSUE, userId, 1, 1000, out messages);

                if (!String.IsNullOrWhiteSpace(messages))
                {
                    this.lblErrorMessage.Text = messages;
                    //return;
                }
            }

            Dictionary<long, CardSearchResult> cardList = new Dictionary<long, CardSearchResult>();
            cardsOutOpt = new List<HtmlSelectOption>();

            foreach (var card in cards)
            {
                if (RoleIssuerList[int.Parse(this.ddlIssuer.SelectedValue)].card_ref_preference)
                    cardsOutOpt.Add(new HtmlSelectOption(utility.UtilityClass.FormatPAN(card.card_number), card.card_id.ToString()));
                else
                    cardsOutOpt.Add(new HtmlSelectOption(card.card_request_reference, card.card_id.ToString()));
                //cardsOutOpt.Add(new HtmlSelectOption(utility.UtilityClass.FormatPAN(card.card_number), card.card_id.ToString()));
                cardList.Add(card.card_id, card);
            }
            //CardCheckedOut = cardList;

            //BuildDataForPickList(PickListNames.CheckedInCards, cardsInOpt);

            //string json = JsonConvert.SerializeObject(cardsOutOpt);
            //StringBuilder script = new StringBuilder();
            //script.AppendLine("var CheckedOutCards = ");
            //script.AppendLine(json);
            //script.AppendLine(";");
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "dataCheckedOutCards", script.ToString(), true);
        }

        private void BuildDataForSummary()
        {
            string arrayName = "summaryResults";
            StringBuilder script = new StringBuilder();
            string emptyList = String.Format("var {0} = new Array();", arrayName);

            try
            {
                if (successCheckedOutCards != null && successCheckedOutCards.Count > 0)
                {
                    try
                    {
                        script.AppendLine(String.Format("var {0} = ", arrayName));
                        script.AppendLine(JsonConvert.SerializeObject(successCheckedOutCards));
                        script.AppendLine(";");
                    }
                    catch
                    {
                        script.Append(emptyList);
                        throw;
                    }
                }
                else
                {
                    script.Append(emptyList);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "dataSummary", script.ToString(), true);
            }
        }

        private void BuildDataForPickList(PickListNames picklist, List<HtmlSelectOption> cardsList)
        {
            StringBuilder script = new StringBuilder();
            string emptyList = String.Format("var {0} = new Array();", picklist.ToString());

            try
            {
                if (cardsList != null && cardsList.Count > 0)
                {
                    try
                    {
                        script.Append(String.Format("var {0} = ", picklist.ToString()));
                        script.Append(JsonConvert.SerializeObject(cardsList));
                        script.Append(";");
                    }
                    catch
                    {
                        script.Append(emptyList);
                        throw;
                    }
                }
                else
                {
                    script.Append(emptyList);
                }
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                //added debugging
                //log.Debug(String.Format("{0}<>{1} => {2}", LogGuid, picklist.ToString(), script.ToString()));
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "data" + picklist.ToString(), script.ToString().Trim(), true);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            BuildDataForPickList(PickListNames.CheckedInCards, cardsInOpt);
            BuildDataForPickList(PickListNames.CheckedOutCards, cardsOutOpt);

            BuildDataForSummary();
        }

        private void ClearCheckInOut()
        {            
            cardsInOpt = null;
            cardsOutOpt = null;

            hdnHasChanges.Value = "false";

            //StringBuilder script = new StringBuilder();
            //script.AppendLine("var CheckedInCards = new Array();");
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "dataCheckedInCards", script.ToString(), true);

            //StringBuilder script2 = new StringBuilder();
            //script2.AppendLine("var CheckedOutCards = new Array();");
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "dataCheckedOutCards", script2.ToString(), true);

            PageIndex = 1;
            lblPageIndex.Text = String.Format("{0}/{1}", 0, 0);
        }
        #endregion

        #region Page Events
        protected void ddlIssuer_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";
                ClearCheckInOut();

                this.btnCheckedInReport.Visible =
                    this.btnCheckedOutReport.Visible = false;

                int issuerId;
                if(int.TryParse(this.ddlIssuer.SelectedValue, out issuerId))
                {
                    UpdateBranchList(issuerId);
                    UpdateProductList(issuerId);
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

        protected void ddlBranch_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";
                ClearCheckInOut();

                this.btnCheckedInReport.Visible =
                    this.btnCheckedOutReport.Visible = false;

                int branchId;
                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId))
                {
                    UpdateOperatorsList(branchId);                    
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

        protected void ddlProduct_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";
                ClearCheckInOut();

                int branchId;
                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId))
                {                    
                        //UpdateSubProductList(int.Parse(this.ddlIssuer.SelectedValue),
                        //                        int.Parse(this.ddlProduct.SelectedValue));                     
                }
                else
                {
                    this.lblErrorMessage.Text = "Error with product id";
                }

                this.btnCheckedInReport.Visible =
                    this.btnCheckedOutReport.Visible = false;

                //long userId;
                //if (int.TryParse(this.ddlBranch.SelectedValue, out branchId) &&
                //     long.TryParse(this.ddlOperator.SelectedValue, out userId))
                //{
                //    UpdateCheckedInList(null, null);
                //    UpdateCheckedoutList(null, null);
                //}

                
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

        protected void ddlSubProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                this.btnCheckedInReport.Visible =
                    this.btnCheckedOutReport.Visible = false;

                int branchId;
                long userId;
                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId) &&
                     long.TryParse(this.ddlOperator.SelectedValue, out userId))
                {
                    UpdateCheckedInList(null, null);
                    UpdateCheckedoutList(null, null);
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

        protected void ddlOperator_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";
                ClearCheckInOut();

                this.btnCheckedInReport.Visible =
                    this.btnCheckedOutReport.Visible = false;
                  

                //int branchId;
                //long userId;
                //if (int.TryParse(this.ddlBranch.SelectedValue, out branchId) && 
                //     long.TryParse(this.ddlOperator.SelectedValue, out userId))
                //{
                //    UpdateCheckedInList(null, null);
                //    UpdateCheckedoutList(null, null);                    
                //}
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


        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        protected void btnLoadCards_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";

                this.btnCheckedInReport.Visible =
                    this.btnCheckedOutReport.Visible = false;

                int branchId;
                int userId;
                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId) &&
                    int.TryParse(this.ddlOperator.SelectedValue, out userId))
                {
                    UpdateCheckedInList(branchId, null);
                    UpdateCheckedoutList(branchId, userId);

                    bool canRead;
                    bool canUpdate;
                    bool canCreate;
                    if (PageUtility.ValidateUserPageRole(User.Identity.Name, UserRole.BRANCH_CUSTODIAN, int.Parse(this.ddlIssuer.SelectedValue), out canRead, out canUpdate, out canCreate))
                    {
                        if (canUpdate)
                        {
                            this.btnUpdate.Enabled = true;
                        }
                    }
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

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            this.btnConfirm.Visible = true;
            this.btnCancel.Visible = true;
            this.btnUpdate.Visible = false;

            this.pnlPage.Visible = false;

            //var sourceList = JsonConvert.DeserializeObject<List<HtmlSelectOption>>(this.sourceSel.Value).Select(m => long.Parse(m.value)).ToList();
            //var targetList = JsonConvert.DeserializeObject<List<HtmlSelectOption>>(this.targetSel.Value).Select(m => long.Parse(m.value)).ToList();


        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.btnConfirm.Visible = false;
            this.btnCancel.Visible = false;
            this.btnUpdate.Visible = true;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblErrorMessage.Text = "";
                this.lblInfoMessage.Text = "";
                this.lblCheckOutSummary.Text = "";

                //log.Debug(String.Format("{0}<>{1}", LogGuid, hdnlog.Value));

                var targetList = JsonConvert.DeserializeObject<List<HtmlSelectOption>>(this.targetSel.Value).Select(m => long.Parse(m.value)).ToList();
                var successCheckedOutCards = _batchService.CheckInOutCards(long.Parse(this.ddlOperator.SelectedValue), int.Parse(this.ddlBranch.SelectedValue), int.Parse(this.ddlProduct.SelectedValue), targetList, new List<long>());


                //reports
                this.tblSummary.Visible = false;

                int checkedOut = 0;
                List<structCard> checkedInCards = new List<structCard>();

                foreach (var scard in successCheckedOutCards)
                {
                    if (scard.branch_card_statuses_id == 0)
                        checkedInCards.Add(new structCard
                        {
                            card_number = scard.card_number,
                            card_reference_number = scard.card_request_reference
                        });
                    else if (scard.branch_card_statuses_id == 1)
                        checkedOut++;
                }

                CardCheckedInReportList = checkedInCards;                

                if(checkedInCards.Count > 0)
                    this.btnCheckedInReport.Visible = true;

                if(checkedOut > 0)
                    this.btnCheckedOutReport.Visible = true;

                this.lblInfoMessage.Text = String.Format(GetLocalResourceObject("InfoUpdateSummary").ToString(),
                    checkedOut, checkedOut, CardCheckedInReportList.Count, CardCheckedInReportList.Count
                                                            //(targetList.Count - unsucessfulCheckOut),
                                                            // targetList.Count,
                                                            // (processCheckInList.Count - unsucessfulCheckIn),
                                                            // processCheckInList.Count
                                                            );

                //string json = JsonConvert.SerializeObject(successCheckedOutCards);
                //StringBuilder script = new StringBuilder();
                //script.AppendLine("var summaryResults = ");
                //script.AppendLine(json);
                //script.AppendLine(";");

                //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "dataSummary", script.ToString(), true);


                int branchId;
                int userId;
                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId) &&
                    int.TryParse(this.ddlOperator.SelectedValue, out userId))
                {
                    CardCheckedIn = null;      
                    this.PageIndex = 1;
                    UpdateCheckedInList(branchId, null);
                    UpdateCheckedoutList(branchId, userId);
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

        //[PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        //protected void btnConfirm_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        this.lblErrorMessage.Text = "";
        //        this.lblInfoMessage.Text = "";
        //        this.lblCheckOutSummary.Text = "";

        //        var sourceList = JsonConvert.DeserializeObject<List<HtmlSelectOption>>(this.sourceSel.Value).Select(m => long.Parse(m.value)).ToList();
        //        var targetList = JsonConvert.DeserializeObject<List<HtmlSelectOption>>(this.targetSel.Value).Select(m => long.Parse(m.value)).ToList();

        //        List<long> processCheckInList = new List<long>();

        //        //Loop through the previous checked out list... doing this becuse it should be the shorter of the two lists.
        //        foreach (KeyValuePair<long, CardSearchResult> item in CardCheckedOut)
        //        {
        //            //check if the card is still in the list. If it isnt then it must be checked in, 
        //            //if it is then remove it from the check out list so that it isnt updated to checked out again and only newly checked out
        //            //cards are processed.
        //            if (!targetList.Contains(item.Key))
        //            {
        //                processCheckInList.Add(item.Key); //Cannot find originally checked out card in new checked out list, go an check card in.
        //            }
        //            else
        //            {
        //                targetList.Remove(item.Key); //New to the check out list, go an check it out.                
        //            }
        //        }

        //        if (targetList.Count == 0 && processCheckInList.Count == 0)
        //        {
        //            this.lblInfoMessage.Text = GetLocalResourceObject("InfoNoCardsUpdated").ToString();
        //            UpdateCheckedInList(int.Parse(this.ddlBranch.SelectedValue), null);
        //            UpdateCheckedoutList(int.Parse(this.ddlBranch.SelectedValue), int.Parse(this.ddlOperator.SelectedValue));
        //        }
        //        else
        //        {
        //            var successCheckedOutCards = _batchService.CheckInOutCards(long.Parse(this.ddlOperator.SelectedValue), int.Parse(this.ddlBranch.SelectedValue), targetList, processCheckInList);

        //            var targetList2 = JsonConvert.DeserializeObject<List<HtmlSelectOption>>(this.targetSel.Value).Select(m => long.Parse(m.value)).ToList();

        //            int unsucessfulCheckOut = 0;
        //            int unsucessfulCheckIn = 0;
        //            if (successCheckedOutCards.Count > 0)
        //            {
        //                foreach (var item in successCheckedOutCards)
        //                {
        //                    if (targetList.IndexOf(item.card_id) != -1)
        //                    {
        //                        unsucessfulCheckOut++;
        //                    }
        //                    else
        //                    {
        //                        unsucessfulCheckIn++;
        //                    }
        //                }

        //                this.tblSummary.Visible = true;
        //                this.lblCheckOutSummary.Text = GetLocalResourceObject("InfoCardsUpdateIssue").ToString();
        //            }

        //            this.tblSummary.Visible = false;

        //            CardCheckedInReportList = null;
        //            if (processCheckInList.Count > 0)
        //            {
        //                List<structCard> cardnos = new List<structCard>();

        //                foreach (var cardId in processCheckInList)
        //                {
        //                     structCard card=new structCard();

        //                    if(CardCheckedIn.ContainsKey(cardId))
        //                    {
        //                            card.card_number=CardCheckedIn[cardId].card_number;
        //                            card.card_reference_number=CardCheckedIn[cardId].card_request_reference;

        //                            cardnos.Add(card);
        //                    }
        //                    else if (CardCheckedOut.ContainsKey(cardId))
        //                    {
        //                        card.card_number = CardCheckedOut[cardId].card_number;
        //                        card.card_reference_number = CardCheckedOut[cardId].card_request_reference;
        //                        cardnos.Add(card);
        //                    }
        //                }

        //                CardCheckedInReportList = cardnos;
        //                this.btnCheckedInReport.Visible = true;
        //            }

        //            if(targetList.Count > 0)
        //                this.btnCheckedOutReport.Visible = true;

        //            this.lblInfoMessage.Text = String.Format(GetLocalResourceObject("InfoUpdateSummary").ToString(), 
        //                                                        (targetList.Count - unsucessfulCheckOut), 
        //                                                        targetList.Count, 
        //                                                        (processCheckInList.Count - unsucessfulCheckIn), 
        //                                                        processCheckInList.Count);
        //            //this.lblInfoMessage.Text = "" + (targetList.Count - unsucessfulCheckOut) + " out of " + targetList.Count + " Card/s checked out successfully <br />" +
        //            //                                (processCheckInList.Count - unsucessfulCheckIn) + " out of " + processCheckInList.Count + " Card/s checked in successfully";


        //            //this.targetSel.Value = utility.UtilityClass.PrimeUIPickListKeyValue<long, string>(cardOutList);

        //            //Page.ClientScript.RegisterArrayDeclaration("summaryResults", json);
        //            string json = JsonConvert.SerializeObject(successCheckedOutCards);
        //            StringBuilder script = new StringBuilder();
        //            script.AppendLine("var summaryResults = ");
        //            script.AppendLine(json);
        //            script.AppendLine(";");

        //            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "dataSummary", script.ToString(), true);
        //            //Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowDlg();", true);

        //           // this.pnlDisable.Visible = false;
        //           // this.pnlButtons.Visible = false;
        //          //  this.dlgCheckOutStatus.Visible = true;


        //            UpdateCheckedInList(int.Parse(this.ddlBranch.SelectedValue), null);
        //            UpdateCheckedoutList(int.Parse(this.ddlBranch.SelectedValue), long.Parse(this.ddlOperator.SelectedValue));  
        //        }                
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;

        //        if (log.IsDebugEnabled || log.IsTraceEnabled)
        //        {
        //            this.lblErrorMessage.Text = ex.ToString();
        //        }
        //    }
        //}

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        protected void btnCheckedInReport_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {
                if (CardCheckedInReportList != null && CardCheckedInReportList.Count > 0)
                {
                    var reportBytes = _batchService.GenerateCheckedInCardsReport(int.Parse(this.ddlBranch.SelectedValue), CardCheckedInReportList, this.ddlOperator.SelectedItem.Text);

                    string reportName = "Cards_CheckedIn_Report_" +
                                            DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(reportBytes);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.End();
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

        [PrincipalPermission(SecurityAction.Demand, Role = "BRANCH_CUSTODIAN")]
        protected void btnCheckedOutReport_OnClick(object sender, EventArgs e)
        {
            this.lblInfoMessage.Text = "";
            this.lblErrorMessage.Text = "";

            try
            {

                var reportBytes = _batchService.GenerateCheckedOutCardsReport(int.Parse(this.ddlOperator.SelectedValue),
                                                                              int.Parse(this.ddlBranch.SelectedValue));

                    string reportName = "Cards_CheckedOut_Report_" + 
                                            DateTime.Now.ToString("ddd_dd_MMMM_yyyy") + ".pdf";

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(reportBytes);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + reportName);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.End();
                
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

        #region Page Properties
        private Dictionary<long, string> CardCheckedIn
        {
            get
            {
                if (ViewState["CardCheckedIn"] == null)
                    return new Dictionary<long, string>();
                else
                    return (Dictionary<long, string>)ViewState["CardCheckedIn"];
            }
            set
            {
                ViewState["CardCheckedIn"] = value;
            }
        }

        //private Dictionary<long, CardSearchResult> CardCheckedOut
        //{
        //    get
        //    {
        //        if (ViewState["CardCheckedOut"] == null)
        //            return new Dictionary<long, CardSearchResult>();
        //        else
        //            return (Dictionary<long, CardSearchResult>)ViewState["CardCheckedOut"];
        //    }
        //    set
        //    {
        //        ViewState["CardCheckedOut"] = value;
        //    }
        //}

        private List<structCard> CardCheckedInReportList
        {
            get
            {
                if (ViewState["CardCheckedInReportList"] == null)
                    return new List<structCard>();
                else
                    return (List<structCard>)ViewState["CardCheckedInReportList"];
            }
            set
            {
                ViewState["CardCheckedInReportList"] = value;
            }
        }

        private Dictionary<int, RolesIssuerResult> RoleIssuerList
        {
            get
            {
                if (ViewState["RoleIssuerList"] == null)
                    return new Dictionary<int, RolesIssuerResult>();
                else
                    return (Dictionary<int, RolesIssuerResult>)ViewState["RoleIssuerList"];
            }
            set
            {
                ViewState["RoleIssuerList"] = value;
            }
        }
        #endregion

        public int RowsPerPage
        {
            get
            {
                if (ViewState["RowsPerPage"] == null)
                    return 100;
                else
                    return Convert.ToInt32(ViewState["RowsPerPage"].ToString());
            }
            set
            {
                ViewState["RowsPerPage"] = value;
            }
        }

        public int PageIndex
        {
            get
            {
                if (ViewState["PageIndex"] == null)
                    return 1;
                else
                    return Convert.ToInt32(ViewState["PageIndex"].ToString());
            }
            set
            {
                ViewState["PageIndex"] = value;
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
        
        protected void lnkNext_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.NEXT);
        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.PREVIOUS);
        }

        protected void lnkFirst_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.FIRST);
        }
        protected void lnkLast_Click(object sender, EventArgs e)
        {
            ChangePage(ResultNavigation.LAST);
        }

        protected void ddlPerPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int perPage;
            if (int.TryParse(this.ddlPerPage.SelectedValue, out perPage))
            {
                PageIndex = 1;
                RowsPerPage = perPage;

                DoPagination();
            }
        }

        #region Pagination
        private void ChangePage(ResultNavigation resultNav)
        {
            this.lblErrorMessage.Text = "";
            //this.dlBatchList.DataSource = null;

            switch (resultNav)
            {
                case ResultNavigation.FIRST:
                    PageIndex = 1;
                    break;
                case ResultNavigation.NEXT:
                    if (PageIndex < TotalPages)
                    {
                        PageIndex = PageIndex + 1;
                    }
                    break;
                case ResultNavigation.PREVIOUS:
                    if (PageIndex > 1)
                    {
                        PageIndex = PageIndex - 1;
                    }
                    break;
                case ResultNavigation.LAST:
                    PageIndex = TotalPages.GetValueOrDefault();
                    break;
                default:
                    break;
            }

            DoPagination();
        }

        private void DoPagination()
        {
            try
            {
                //log.Debug(String.Format("{0}<>{1}", LogGuid, hdnlog.Value));

                //Persist checked out cards
                cardsOutOpt = JsonConvert.DeserializeObject<List<HtmlSelectOption>>(this.targetSel.Value) ?? new List<HtmlSelectOption>();

                //cardsOutOpt[0].value = "a123";
                //check target list
                //var badList1 = cardsOutOpt.Where(w => !w.value.All(Char.IsDigit));
                //if(badList1.Count() > 0)
                //{
                //    //cardsOutOpt = cardsOutOpt.Where(w => !badList1.Any(p => p.value == w.value)).ToList();
                //    StringBuilder sb = new StringBuilder();
                //    sb.AppendLine("Bad Target:");
                //    foreach (var badItem in badList1)
                //        sb.AppendLine(String.Format("{0}<>{1}-{2}", LogGuid, badItem.label, badItem.value));

                //    log.Warn(sb.ToString());
                //}

                //whats been moved from checked-out to checked in?
                var sourceList = JsonConvert.DeserializeObject<List<HtmlSelectOption>>(this.sourceSel.Value) ?? new List<HtmlSelectOption>();

                //sourceList[0].value = "a123";
                //var badList2 = sourceList.Where(w => !w.value.All(Char.IsDigit));
                //if (badList2.Count() > 0)
                //{
                //    //cardsOutOpt = cardsOutOpt.Where(w => !badList1.Any(p => p.value == w.value)).ToList();
                //    StringBuilder sb = new StringBuilder();
                //    sb.AppendLine("Bad Source:");
                //    foreach (var badItem in badList2)
                //        sb.AppendLine(String.Format("{0}<>{1}-{2}", LogGuid,badItem.label, badItem.value));

                //    log.Warn(sb.ToString());
                //}

                var result = sourceList.Where(p => !CardCheckedIn.Keys.Any(p2 => p2 == long.Parse(p.value)));

                //Load checked in cards
                int branchId;
                int userId;
                if (int.TryParse(this.ddlBranch.SelectedValue, out branchId) &&
                    int.TryParse(this.ddlOperator.SelectedValue, out userId))
                {
                    UpdateCheckedInList(branchId, cardsOutOpt.Select(m => long.Parse(m.value)).ToList(), result.ToList());
                }
            }
            catch(Exception ex)
            {                
                log.Error(ex);
                this.lblErrorMessage.Text = Resources.DefaultExceptions.UnknownExceptionMessage;
                if (log.IsDebugEnabled || log.IsTraceEnabled)
                {
                    this.lblErrorMessage.Text = ex.ToString();
                }

                ClearCheckInOut();
            }        
        }

        #endregion

        private string LogGuid
        {
            get
            {
                if (ViewState["CardCheckedIn"] == null)
                    return "GuidEmpty";
                else
                    return ViewState["LogGuid"].ToString();
            }
            set
            {
                ViewState["LogGuid"] = value;
            }
        }
    }
}