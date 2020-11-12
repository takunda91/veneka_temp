using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.CardIssuanceService;
using Common.Logging;
using indigoCardIssuingWeb.SearchParameters;
using System.Threading;
using System.Globalization;
using indigoCardIssuingWeb.utility;

namespace indigoCardIssuingWeb.webpages.cardmanagement
{
    public partial class CardSearch: BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CardSearch));
        private readonly UserManagementService _userMan = new UserManagementService();
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();
        private readonly CustomerCardIssueService _cardservice = new CustomerCardIssueService();

        private readonly UserRole[] userRolesForPage = new UserRole[] { UserRole.BRANCH_OPERATOR,
                                                                        UserRole.BRANCH_CUSTODIAN,
                                                                        UserRole.CENTER_MANAGER,
                                                                        UserRole.CENTER_OPERATOR,
                                                                        UserRole.PIN_OPERATOR,
                                                                        UserRole.AUDITOR,
                                                                        UserRole.BRANCH_PIN_OFFICER,
                                                                         UserRole.CARD_CENTRE_PIN_OFFICER

        };

        #region LOAD PAGE
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
                List<UserRole> currentUserRoles = new List<UserRole>();
                Dictionary<int, ListItem> issuersList = new Dictionary<int, ListItem>();
                Dictionary<int, RolesIssuerResult> issuers = new Dictionary<int, RolesIssuerResult>();
                //Check users roles to make sure he didnt try and get to the page by typing in the address of this page
                PageUtility.ValidateUserPageRole(User.Identity.Name, userRolesForPage, out issuersList, out issuers);

                Issuers = issuers;
                CurrentUserRoles = currentUserRoles;
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
        #endregion

        #region BUTTON CLICK METHODS
       
        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            this.tbCardNumber.Text = "";
            this.tbAccountNumber.Text = string.Empty;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.lblErrorMessage.Text = "";
            this.lblInfoMessage.Text = "";

            try
            {
                string cardNumber = String.IsNullOrWhiteSpace(this.tbCardNumber.Text) ? null : this.tbCardNumber.Text.Trim().Replace('?', '%');
                string accountNumber = String.IsNullOrWhiteSpace(this.tbAccountNumber.Text) ? null : this.tbAccountNumber.Text.Trim().Replace('?', '%');


                int? userRole = null;
                //Check how many user roles the user has, if it's just one use that as user role.
                if (CurrentUserRoles.Count == 1)
                {
                    userRole = (int)CurrentUserRoles[0];
                }

                CardSearchParameters cardSearchParams = new CardSearchParameters(null, userRole, null, null, cardNumber, null, null,
                                                                                 null, null, null,
                                                                                 null, accountNumber, null, null, null,
                                                                                 null, null, 1, StaticDataContainer.ROWS_PER_PAGE);

                var results = _batchService.SearchForCards(cardSearchParams, 1, null);

                if (results.Count == 0)
                {
                    this.lblInfoMessage.Text = Resources.DefaultExceptions.NoResultsMessage;
                }
                else
                {
                    //Save both results and the search params to the session. results will be displayed on next page.
                    SessionWrapper.CardSearchResults = results;
                    SessionWrapper.CardSearchParams = cardSearchParams;
                    Server.Transfer("~\\webpages\\cardmanagement\\CardList.aspx");
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

        private int? GetDropDownId(string value)
        {
            int selectedId;
            int? Id = null;
            if (int.TryParse(value, out selectedId))
            {
                if (selectedId > 0)
                {
                    Id = selectedId;
                }
            }

            return Id;
        }
        #endregion

        #region PRIVATE METHODS

        #endregion

        #region Properites

        /// <summary>
        /// Roles currently assigned to the user.
        /// </summary>
        public List<UserRole> CurrentUserRoles
        {
            get
            {
                if (ViewState["CurrentUserRoles"] != null)
                {
                    return (List<UserRole>)ViewState["CurrentUserRoles"];
                }
                else
                {
                    return new List<UserRole>();
                }
            }
            set
            {
                ViewState["CurrentUserRoles"] = value;
            }
        }

        public Dictionary<int, RolesIssuerResult> Issuers
        {
            get
            {
                if (ViewState["Issuers"] != null)
                {
                    return (Dictionary<int, RolesIssuerResult>)ViewState["Issuers"];
                }
                else
                {
                    return new Dictionary<int, RolesIssuerResult>();
                }
            }
            set
            {
                ViewState["Issuers"] = value;
            }
        }

        #endregion


    }
}