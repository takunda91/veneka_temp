using System.Collections.Generic;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.CardIssuanceService;
using System;
using System.Linq;
using System.Resources;

namespace indigoCardIssuingWeb.CCO
{
    public class MenuContainer
    {
        //private static readonly Dictionary<UserRole, List<DisplayMenu>> _roleMenuLists = new Dictionary<UserRole, List<DisplayMenu>>();
        private readonly List<DisplayMenu> _builtMenuItems = new List<DisplayMenu>();

        private bool _initiated;
        private bool LdapUser;
        private bool BranchCustodian;

        private enum MenuHeader
        {

            MY_ACCOUNT_MENU,
            USER_ADMIN_MENU,
            USER_GROUP_ADMIN_MENU,
            ISSUER_ADMIN_MENU,
            BRANCH_ADMIN_MENU,
            LOAD_BATCH_MENU,
            EXPORT_BATCH_MENU,
            DIST_BATCH_MENU,
            CARD_REQUEST_MENU,
            CARD_STOCK_MENU,
            CARD_PRODUCTION_MENU,
            CARD_FEES_MENU,
            MAIN_MENU_1,
            MAIN_MENU_2,
            MAIN_MENU_3,
            MAIN_MENU_4,
            MAIN_MENU_5,
            MAIN_MENU_6,
            MAIN_MENU_7,
            MAIN_MENU_8,
            MAIN_MENU_9,
            MAIN_MENU_10,
            PIN_MENU,
            PIN_RESET_MENU,
            VAULT_MENU,
            ISSUE_CARD_MENU,
            SEARCH_MENU,
            REPORTS_MENU,
            AUDITOR_MENU,
            FILE_LOAD_MENU,
            LICENSE_MENU,
            ADMIN_MENU,
            PRODUCT_MENU,
            TERMINAL_MENU,

            REMOTE_MENU,
            USER_AUDIT_MENU,
            PRINT_MENU,
            THREEDSECURE_MENU,
            FUNDS_LOAD_MENU,
            RENEWALS_MENU,
            //CARD_MANAGEMENT_MENU,
        }

        private Dictionary<MenuHeader, DisplayMenu> parentMenuItems = new Dictionary<MenuHeader, DisplayMenu>();
        private Dictionary<MenuHeader, Dictionary<UserRole, Dictionary<int, DisplayMenu>>> menuItems = new Dictionary<MenuHeader, Dictionary<UserRole, Dictionary<int, DisplayMenu>>>();
        private Dictionary<UserRole, Dictionary<int, DisplayMenu>> roleItems;
        private Dictionary<int, DisplayMenu> items; //The int will indicate the menu item id, this is so we dont build two of the same submenu items.

        public DisplayMenu[] GetMenuForRoles(Dictionary<UserRole, List<RolesIssuerResult>> roles, bool ldapUser, List<StatusFlowRole> statusFlow)
        {
            this.LdapUser = ldapUser;
            if (roles != null && roles.Count > 0)
            {
                if (_initiated == false)
                    Init(statusFlow);

                return GetBuiltMenuItems(roles).ToArray();
            }

            return null;
        }

        public void ClearMenu()
        {
            _builtMenuItems.Clear();
        }

        private List<DisplayMenu> GetBuiltMenuItems(Dictionary<UserRole, List<RolesIssuerResult>> roles)
        {
            //_builtMenuItems.Clear();
            if (_builtMenuItems.Count == 0)
            {
                //Loop through all the Parent menu items (headers), then loop through each role building up the submenu for the parent.
                //Making sure that the submenu item has not already been added (This might happen if the users belongs to more than one
                //user group and those roles have able to see the same submenu item.
                //There is also a menu item exclusion that is checked, this removes menu items that should not be shown based on a runtime
                // parameter.

                foreach (MenuHeader header in System.Enum.GetValues(typeof(MenuHeader)))
                {
                    Dictionary<UserRole, Dictionary<int, DisplayMenu>> children;
                    DisplayMenu parent;
                    if (menuItems.TryGetValue(header, out children) && parentMenuItems.TryGetValue(header, out parent)) //Check that there are submenu's for the header menu.
                    {
                        parent.DeleteAllSubmenus();
                        List<int> builtItems;

                        foreach (UserRole role in roles.Keys)
                        {
                            builtItems = new List<int>();
                            Dictionary<int, DisplayMenu> subMenuItems;
                            // || (header == MenuHeader.DIST_BATCH_MENU && role == UserRole.BRANCH_PRODUCT_MANAGER
                            if (children.TryGetValue(role, out subMenuItems))
                            {
                                //if (header == MenuHeader.DIST_BATCH_MENU && role == UserRole.BRANCH_PRODUCT_MANAGER)
                                //{
                                //    children.TryGetValue(UserRole.CENTER_OPERATOR, out subMenuItems);
                                //}


                                foreach (KeyValuePair<int, DisplayMenu> item in subMenuItems)
                                {
                                    if (!builtItems.Contains(item.Key) && !excludeFromMenu(header, item.Key, role, roles))
                                    {
                                        if (item.Value.IssueMethodId != null &&
                                            item.Value.IssueMethodId == 1 &&
                                            roles[role].Where(w => w.instant_card_issue_YN == true).Count() > 0)
                                        {
                                            builtItems.Add(item.Key);
                                            parent.AddSubmenu(item.Value);
                                        }
                                        else if (item.Value.IssueMethodId != null &&
                                                item.Value.IssueMethodId == 0 &&
                                                roles[role].Where(w => w.classic_card_issue_YN == true).Count() > 0)
                                        {
                                            builtItems.Add(item.Key);
                                            parent.AddSubmenu(item.Value);
                                        }
                                        else if (item.Value.IssueMethodId != null &&
                                                item.Value.IssueMethodId == 2 &&
                                                roles[role].Where(w => w.classic_card_issue_YN == true).Count() > 0)
                                        {
                                            builtItems.Add(item.Key);
                                            parent.AddSubmenu(item.Value);
                                        }
                                        else if (item.Value.IssueMethodId == null)
                                        {
                                            builtItems.Add(item.Key);
                                            parent.AddSubmenu(item.Value);
                                        }

                                    }
                                }
                            }
                        }

                        //If there are subment items, then the user has access to the parent item. add it to the list.
                        if (parent.HasSubmenus())
                        {
                            _builtMenuItems.Add(parent);
                        }
                    }
                }
            }

            return _builtMenuItems;
        }

        private void Init(List<StatusFlowRole> FlowStatus)
        {
            //Build all the SubMenus.
            BuildReportingSubMenu();
            BuildUserAdminSubMenu();
            BuildUserGroupAdminSubMenu();
            BuildIssuerAdminAdminSubMenu();
            BuildBranchAdminAdminSubMenu();
            BuildLoadBatchAdminAdminSubMenu();
            BuildExportBatchSubMenu();
            BuildDistBatchAdminAdminSubMenu();
            BuildVaultAdminSubMenu();
            BuildCardIssuingSubMenu();
            BuildMyAdminSubMenu();
            BuildAuditSubMenu();
            BuildAdministrationSubMenu();
            BuildLicenseManagerSubMenu();
            BuildFileLoaderSubMenu();
            BuildPinSubMenu();
            BuildProductsMenu();
            BuildProductionMenus(FlowStatus);
            BuildSearchMenues();
            BuildCardFeesSubMenu();
            BuildTerminalSubMenu();
            BuildPinResetSubMenu();
            BuildRemoteSubMenu();
            BuildAuditorMenu();
            BuildPrintSubMenu();
            BuildThreeDSecureSubMenu();
            BuildFundsLoadSubMenu();
            BuildRenewalsSubMenu();
            //BuildCardManagementSubMenu();
            //BuildCardRequestMenu();
            //BuildCardProductionMenu();
            //BuildCardStockOrdering(FlowStatus);
            _initiated = true;
        }

        private void BuildFundsLoadSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            //Please update the excludeFromMenu method if the submenu item id's are changed.            
            items.Add(1, new DisplayMenu(Resources.MenuItems.FundsLoadCreate, "~\\webpages\\fundsload\\LoadFunds.aspx?", "fundsLoadCreate"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.FundsLoadReview,
                                         "~\\webpages\\fundsload\\LoadFundsList.aspx?status=" + ((int)FundsLoadStatusType.Created).ToString(),
                                         "fundsLoadListCreated"));
            items.Add(3, new DisplayMenu(Resources.MenuItems.FundsLoadApprove,
                                          "~\\webpages\\fundsload\\LoadFundsList.aspx?status=" + ((int)FundsLoadStatusType.Reviewed).ToString(),
                                          "fundsLoadListReviewed"));
            items.Add(4, new DisplayMenu(Resources.MenuItems.FundsLoadLoad,
                                          "~\\webpages\\fundsload\\LoadFundsList.aspx?status=" + ((int)FundsLoadStatusType.Approved).ToString(),
                                          "fundsLoadListApproved"));
            items.Add(5, new DisplayMenu(Resources.MenuItems.FundsLoadCompleted,
                                          "~\\webpages\\fundsload\\LoadFundsList.aspx?status=" + ((int)FundsLoadStatusType.SMSSent).ToString(),
                                          "fundsLoadListSMSSent"));

            userItems = items.Where(p => p.Key == 1 || p.Key == 4).ToDictionary(p => p.Key, p => p.Value);
            roleItems.Add(UserRole.BRANCH_OPERATOR, userItems);

            userItems = items.Where(p => p.Key == 2).ToDictionary(p => p.Key, p => p.Value);
            roleItems.Add(UserRole.BRANCH_CUSTODIAN, userItems);

            userItems = items.Where(p => p.Key == 3).ToDictionary(p => p.Key, p => p.Value);
            roleItems.Add(UserRole.CENTER_OPERATOR, userItems);

            menuItems.Add(MenuHeader.FUNDS_LOAD_MENU, roleItems.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value));
            parentMenuItems.Add(MenuHeader.FUNDS_LOAD_MENU, new DisplayMenu(Resources.MenuItems.FundsLoadHeader, "", "fundsLoad"));

        }

        private void BuildRenewalsSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            //Please update the excludeFromMenu method if the submenu item id's are changed.            
            items.Add(1, new DisplayMenu(Resources.MenuItems.RenewalApproveLoadedFile, "~\\webpages\\renewal\\RenewalList.aspx?batchstatus=" + ((int)RenewalStatusType.Loaded).ToString(), "renewalApproveLoadedFile"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.RenewalVerifyRenewalRequest, "~\\webpages\\renewal\\VerifyList.aspx?", "renewalVerifyRequests"));
            items.Add(3, new DisplayMenu(Resources.MenuItems.RenewalCreateBatch, "~\\webpages\\renewal\\BatchCreate.aspx?", "renewalCreateBatch"));
            items.Add(4, new DisplayMenu(Resources.MenuItems.RenewalApproveBatch, "~\\webpages\\renewal\\BatchList.aspx?batchstatus=" + ((int)RenewalBatchStatusType.Created).ToString(), "renewalApproveBatch"));
            items.Add(5, new DisplayMenu(Resources.MenuItems.RenewalReceive, "~\\webpages\\renewal\\BatchList.aspx?batchstatus=" + ((int)RenewalBatchStatusType.Exported).ToString(), "renewalReceive"));
            items.Add(6, new DisplayMenu(Resources.MenuItems.RenewalDistribute, "~\\webpages\\renewal\\BatchList.aspx?batchstatus=" + ((int)RenewalBatchStatusType.Received).ToString(), "renewalUpload"));

            userItems = items.Where(p => p.Key == 1 || p.Key == 4 || p.Key == 5).ToDictionary(p => p.Key, p => p.Value);
            roleItems.Add(UserRole.CENTER_MANAGER, userItems);

            userItems = items.Where(p => p.Key == 2 || p.Key == 3 || p.Key == 6).ToDictionary(p => p.Key, p => p.Value);
            roleItems.Add(UserRole.CENTER_OPERATOR, userItems);

            menuItems.Add(MenuHeader.RENEWALS_MENU, roleItems.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value));
            parentMenuItems.Add(MenuHeader.RENEWALS_MENU, new DisplayMenu(Resources.MenuItems.RenewalHeader, "", "cardRenewals"));
        }

        private void BuildCardManagementSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            //Please update the excludeFromMenu method if the submenu item id's are changed.            
            items.Add(1, new DisplayMenu(Resources.MenuItems.CardManagement, "~\\webpages\\cardmanagement\\CardSearch.aspx?", "cardManagementSearch"));

            userItems = items.ToDictionary(p => p.Key, p => p.Value);
            roleItems.Add(UserRole.CMS_OPERATOR, userItems);

            //menuItems.Add(MenuHeader.CARD_MANAGEMENT_MENU, roleItems.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value));
            //parentMenuItems.Add(MenuHeader.CARD_MANAGEMENT_MENU, new DisplayMenu(Resources.MenuItems.CardManagementHeader, "", "cardManagement"));
        }

        private void BuildThreeDSecureSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            items.Add(1, new DisplayMenu("ThreeDSecure Batches List", "~\\webpages\\ThreeDSecure\\ThreeDSecureList.aspx", "ThreeDSecureBatches"));
            items.Add(2, new DisplayMenu("ThreeDSecure Search", "~\\webpages\\ThreeDSecure\\ThreeDSecureSearch.aspx", "searchThreeDSecure"));

            roleItems.Add(UserRole.CENTER_MANAGER, items);
            roleItems.Add(UserRole.CENTER_OPERATOR, items);



            menuItems.Add(MenuHeader.THREEDSECURE_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.THREEDSECURE_MENU, new DisplayMenu("ThreeDSecure", "", "ThreeDSecure"));
            //Load batch Center Manager
            //var loadBatchMenu = new DisplayMenu("Load Batch", "", "LoadBatch");
        }

        /// <summary>
        /// Excludes menu items.
        /// </summary>
        /// <param name="subMenuId"></param>
        /// <param name="role"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        private bool excludeFromMenu(MenuHeader header, int subMenuId, UserRole role, Dictionary<UserRole, List<RolesIssuerResult>> roles)
        {
            ////If the issuer has auto create dist batch then hide the menu item to create a dist batch
            //if (role == UserRole.CENTER_OPERATOR && header == MenuHeader.DIST_BATCH_MENU && subMenuId == 1)
            //{
            //    List<RolesIssuerResult> issuers;
            //    if (roles.TryGetValue(UserRole.CENTER_OPERATOR, out issuers))
            //    {
            //        //Show create dist batch if there are any issuers who have auto create set to false;
            //        var manualBatchCreate = issuers.Where(m => m.auto_create_dist_batch == false).ToList();


            //        if (manualBatchCreate.Count == 0)
            //        {
            //            return true;
            //        }
            //    }
            //}

            List<RolesIssuerResult> issuersRole;
            //PIN Issue
            //if (header == MenuHeader.PIN_RESET_MENU && roles.TryGetValue(role, out issuersRole))
            //{
            //    if (issuersRole.Any(a => a.enable_instant_pin_reissue_YN == true))
            //        return true;
            //    else
            //        return false;
            //}



            //PIN Mailer
            if (header == MenuHeader.PIN_MENU && roles.TryGetValue(role, out issuersRole))
            {
                //if (issuersRole.Any(a => a.pin_mailer_reprint_YN == true))
                //    return false;
                //else
                //    return true;

                return false;
            }

            //Export Batch
            if (header == MenuHeader.EXPORT_BATCH_MENU && roles.TryGetValue(role, out issuersRole))
            {
                if (issuersRole.Any(a => a.cms_exportable_YN == true))
                    return false;
                else
                    return true;
            }

            //Issue Card
            if (header == MenuHeader.ISSUE_CARD_MENU && subMenuId == 7 && roles.TryGetValue(role, out issuersRole))
            {
                if (issuersRole.Any(a => a.pin_mailer_printing_YN == true))
                    return false;
                else
                    return true;
            }

            //Reports
            if (header == MenuHeader.REPORTS_MENU && roles.TryGetValue(role, out issuersRole))
            {
                if (subMenuId == 10 || subMenuId == 20)
                {
                    if (issuersRole.Any(a => a.pin_mailer_printing_YN == true))
                        return false;
                    else
                        return true;
                }
                else if (subMenuId == 11)
                {
                    if (issuersRole.Any(a => a.instant_card_issue_YN == true))
                        return false;
                    else
                        return true;
                }
                else if (subMenuId == 19)
                {
                    if (issuersRole.Any(a => a.enable_instant_pin_reissue_YN == true))
                        return true;
                    else
                        return false;
                }
            }


            //If the issuer has pin mailer reprint enabled then show the menu item for Pin Batch
            if ((role == UserRole.CENTER_OPERATOR || role == UserRole.CENTER_MANAGER || role == UserRole.CMS_OPERATOR)
                && (header == MenuHeader.PIN_MENU || (header == MenuHeader.SEARCH_MENU && subMenuId.Equals(5))))
            {
                List<RolesIssuerResult> issuers;
                if (roles.TryGetValue(UserRole.CENTER_OPERATOR, out issuers) ||
                    roles.TryGetValue(UserRole.CENTER_MANAGER, out issuers) ||
                    roles.TryGetValue(UserRole.CMS_OPERATOR, out issuers))
                {
                    var pinReprintMenu = issuers.Where(p => p.pin_mailer_reprint_YN == true).ToList();

                    if (pinReprintMenu.Count == 0)
                    {
                        return true;
                    }
                }
            }

            //if user is satellite branch user show issue hybrid card menu
            if (role == UserRole.BRANCH_OPERATOR && header == MenuHeader.ISSUE_CARD_MENU)
            {
                List<RolesIssuerResult> issuers;
                if (roles.TryGetValue(UserRole.BRANCH_OPERATOR, out issuers))
                {
                    if (subMenuId == 9)
                    {
                        var issuecardMenu = issuers.Where(p => p.SatelliteBranch_YN == true).ToList();

                        if (issuecardMenu.Count == 0)
                            return true;
                        else
                            return false;

                    }
                    else if (subMenuId == 2)
                    {
                        var issuecardMenu = issuers.Where(p => p.MainBranch_YN == true).ToList();

                        if (issuecardMenu.Count == 0)
                            return true;
                        else
                            return false;

                    }



                }


                if (roles.TryGetValue(UserRole.BRANCH_CUSTODIAN, out issuers))
                {

                    if (subMenuId == 10)
                    {
                        var issuecardMenu = issuers.Where(p => p.SatelliteBranch_YN == true).ToList();

                        if (issuecardMenu.Count == 0)
                            return true;
                        else
                            return false;

                    }
                }
            }

            // File Loader options
            if ((role == UserRole.CENTER_MANAGER)
                && (header == MenuHeader.FILE_LOAD_MENU || header == MenuHeader.LOAD_BATCH_MENU || (header == MenuHeader.SEARCH_MENU && subMenuId.Equals(5))))
            {
                List<RolesIssuerResult> issuers;
                if (roles.TryGetValue(UserRole.CENTER_MANAGER, out issuers))
                {
                    var fileLoaderMenu = issuers.Where(p => p.EnableCardFileLoader_YN == true).ToList();

                    if (fileLoaderMenu.Count == 0)
                    {
                        return true;
                    }
                }
            }


            return false;
        }

        /// <summary>
        /// Build Reporting SubMenu Items.
        /// </summary>
        /// <returns></returns>
        private void BuildReportingSubMenu()
        {
            //menuItems = new Dictionary<MenuHeader, Dictionary<UserRole, List<DisplayMenu>>>();
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();

            Dictionary<int, DisplayMenu> userItems;

            //Reporting
            var branchContextIssuedReport = new DisplayMenu(Resources.MenuItems.ReportsIssedCards, "~\\Reporting\\Branches\\IssuedCardsReportPage.aspx?Page=issuecard", "IssuedCardsReports");
            var cardCenterContextInventoryBalanceReporting = new DisplayMenu(Resources.MenuItems.ReportInventory, "~\\Reporting\\CardCenter\\InventoryReportPage.aspx?Page=branch", "InventoryReports");
            var IssuerCardsSummaryReport = new DisplayMenu(Resources.MenuItems.ReportIssuecardSummary, "~\\Reporting\\CardCenter\\IssuerCardsSummaryReportPage.aspx", "IssuerCardsSummaryReport");
            var SpoilCardReport = new DisplayMenu(Resources.MenuItems.ReportSpoilCardReport, "~\\Reporting\\Branches\\SpoilCardReportPage.aspx", "SpoilCardReport");
            var SpoilCardSummaryReport = new DisplayMenu(Resources.MenuItems.ReportspoilcadsSummary, "~\\Reporting\\CardCenter\\SpoilCardSummaryReportPage.aspx", "SpoilCardSummaryReport");
            var branchOrderReport = new DisplayMenu("Branch Order Report", "~\\Reporting\\Branches\\BranchOrderReport.aspx", "BranchOrderReport");
            var cardProductionReport = new DisplayMenu("Card Production Report", "~\\Reporting\\Branches\\CardProductionReport.aspx", "CardProductionReport");
            var cardDispatchReport = new DisplayMenu("Card Dispatch Report", "~\\Reporting\\Branches\\CardDispatchReport.aspx", "CardDispatchReport");
            var cardExpiryReport = new DisplayMenu("Card Expiry Report", "~\\Reporting\\Branches\\CardExpiryReport.aspx", "CardExpiryReport");
            var pinMailerReport = new DisplayMenu("PIN Mailer Report", "~\\Reporting\\Branches\\PinMailerReport.aspx", "PinMailerReport", 0);
            var burnRateReport = new DisplayMenu(Resources.MenuItems.ReportBurnRateReportHeading, "~\\Reporting\\Branches\\BurnRateReportPage.aspx", "BurnRateReport", 1);
            var branchCardStockmngReport = new DisplayMenu(Resources.MenuItems.ReportBranchcardStockHeading, "~\\Reporting\\Branches\\BranchCardStockMangementReportPage.aspx?Page=branch", "Branch Card Stock Management report", 1);
            var FeeRevenueReport = new DisplayMenu(Resources.MenuItems.ReportFeeRevenueHeading, "~\\Reporting\\Branches\\FeeRevenueReportPage.aspx", "PinMailerReport");

            var UserGroupAuditReport = new DisplayMenu("User Groups", "~\\Reporting\\System\\AuditReport_Usergroup.aspx", "User Groups");
            var UserperuserroleAuditReport = new DisplayMenu("Users Per User Role", "~\\Reporting\\System\\AuditReport_Usersperuserrole.aspx", "Users per user role");
            var branchesperuserAuditReport = new DisplayMenu("Branches Per User Group", "~\\Reporting\\System\\AuditReport_Branchesperusergroup.aspx", "Branches per user group");

            var CenterCardStockReport = new DisplayMenu("Centre Card Stock Management Report", "~\\Reporting\\Branches\\BranchCardStockMangementReportPage.aspx?Page=center", "Centre Card Stock Management report");
            var cardCenterInventorReport = new DisplayMenu("Centre Inventory Summary Report", "~\\Reporting\\CardCenter\\InventoryReportPage.aspx?Page=center", "InventoryReports");

            var PinreissuedReport = new DisplayMenu("PIN Re-issue Report ", "~\\Reporting\\Branches\\PinReissueReport.aspx", "PIN Re-issue Report");
            var PinMailerRePrintReport = new DisplayMenu("PIN Mailer Re-Print Report ", "~\\Reporting\\Branches\\PinMailerRePrintReport.aspx", "PIN Mailer Re-Print Report");

            var CardsInprogressReport = new DisplayMenu(Resources.MenuItems.ReportCardsInProgress, "~\\Reporting\\Branches\\CardsInProgressReport.aspx", "CardsInProgressReport");

            var FeeStatusReport = new DisplayMenu(Resources.MenuItems.FeeStatusReport, "~\\Reporting\\Branches\\FeeStatusReport.aspx", "CardsInProgressReport");

            var CMSErrorReport = new DisplayMenu(Resources.MenuItems.CMSErrorReport, "~\\Reporting\\Branches\\CMSErrorReport.aspx", "CardsInProgressReport");

            var BillingReport = new DisplayMenu("Billing Report", "~\\Reporting\\CardCenter\\BillingReport.aspx", "BillingReport");
            //Branch reports
            items.Add(1, branchContextIssuedReport);
            items.Add(2, cardCenterContextInventoryBalanceReporting);
            items.Add(3, IssuerCardsSummaryReport);
            items.Add(4, SpoilCardReport);
            items.Add(5, SpoilCardSummaryReport);
            items.Add(6, branchOrderReport);
            items.Add(7, cardProductionReport);
            items.Add(8, cardDispatchReport);
            items.Add(9, cardExpiryReport);
            items.Add(10, pinMailerReport);
            items.Add(11, burnRateReport);
            items.Add(12, branchCardStockmngReport);
            items.Add(13, FeeRevenueReport);
            items.Add(14, UserGroupAuditReport);
            items.Add(15, UserperuserroleAuditReport);
            items.Add(16, branchesperuserAuditReport);
            items.Add(17, CenterCardStockReport);
            items.Add(18, cardCenterInventorReport);
            items.Add(19, PinreissuedReport);
            items.Add(20, PinMailerRePrintReport);
            items.Add(21, CardsInprogressReport);
            items.Add(22, FeeStatusReport);
            items.Add(23, CMSErrorReport);
            items.Add(24, BillingReport);
            //Branch Operator
            userItems = new Dictionary<int, DisplayMenu>(items);

            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            userItems.Remove(18);
            userItems.Remove(19);
            userItems.Remove(20);
            userItems.Remove(21);
            userItems.Remove(22);
            userItems.Remove(23);
            userItems.Remove(24);

            roleItems.Add(UserRole.BRANCH_OPERATOR, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            userItems.Remove(18);
            userItems.Remove(20);
            userItems.Remove(21);
            userItems.Remove(22);
            userItems.Remove(23);
            userItems.Remove(24);
            roleItems.Add(UserRole.PIN_OPERATOR, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            userItems.Remove(18);
            userItems.Remove(20);

            roleItems.Add(UserRole.BRANCH_CUSTODIAN, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(20);

            roleItems.Add(UserRole.CENTER_MANAGER, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(20);

            roleItems.Add(UserRole.CENTER_OPERATOR, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            userItems.Remove(18);
            userItems.Remove(19);
            userItems.Remove(21);
            userItems.Remove(22);
            userItems.Remove(23);
            userItems.Remove(24);


            roleItems.Add(UserRole.REPORT_ADMIN, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(17);
            roleItems.Add(UserRole.AUDITOR, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(16);
            userItems.Remove(17);
            userItems.Remove(18);
            userItems.Remove(19);
            userItems.Remove(20);
            userItems.Remove(21);
            userItems.Remove(22);
            userItems.Remove(23);
            userItems.Remove(24);

            roleItems.Add(UserRole.USER_ADMIN, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(15);
            userItems.Remove(17);
            userItems.Remove(18);
            userItems.Remove(19);
            userItems.Remove(20);
            userItems.Remove(21);
            userItems.Remove(22);
            userItems.Remove(23);
            userItems.Remove(24);

            roleItems.Add(UserRole.USER_GROUP_ADMIN, userItems);

            menuItems.Add(MenuHeader.REPORTS_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.REPORTS_MENU, new DisplayMenu(Resources.MenuItems.ReportsHeading, "", "Reporting"));

        }

        /// <summary>
        /// Build User Management SubMenu Items.
        /// </summary>
        private void BuildUserAdminSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu(Resources.MenuItems.UserAdminCreate, "~\\webpages\\users\\ManageUser.aspx", "createUser"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.UserAdminManage, "~\\webpages\\users\\UserList.aspx", "manageUsers"));
            items.Add(3, new DisplayMenu(Resources.MenuItems.UserAdminSearch, "~\\webpages\\users\\SearchUserForm.aspx", "searchUsers"));

            roleItems.Add(UserRole.USER_ADMIN, items);

            menuItems.Add(MenuHeader.USER_ADMIN_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.USER_ADMIN_MENU, new DisplayMenu(Resources.MenuItems.UserAdminHeading, "", "ApplicationUsers"));
            //User Menu
            //var applicationUser = new DisplayMenu("Application Users", "", "ApplicationUsers");
        }

        /// <summary>
        /// Build Products.
        /// </summary>
        private void BuildProductsMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu(Resources.MenuItems.ProductsCreate, "~\\webpages\\product\\ProductAdminScreen.aspx", "createproduct"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.ProductsManage, "~\\webpages\\product\\ProductList.aspx", "manageproduct"));

            roleItems.Add(UserRole.ISSUER_ADMIN, items);

            menuItems.Add(MenuHeader.PRODUCT_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.PRODUCT_MENU, new DisplayMenu(Resources.MenuItems.ProductsHeading, "", "Product"));
            //User Menu
            //var applicationUser = new DisplayMenu("Application Users", "", "ApplicationUsers");
        }

        /// <summary>
        /// Build Card Request Menu
        /// </summary>
        private void BuildCardRequestMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu(Resources.MenuItems.CardRequestCreateBatch, "~\\webpages\\classic\\RequestBatchCreate.aspx?issueMethod=0", "requestCreate"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.CardRequestBatchApprove,
                                         "~\\webpages\\dist\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.CREATED).ToString()
                                         + "&issueMethod=0",
                                         "approveRequestBatch"));
            items.Add(3, new DisplayMenu(Resources.MenuItems.CardRequestBatchList, "~\\webpages\\dist\\DistBatchList.aspx?status=&issueMethod=0", "requestList"));

            //CENTER OPERATOR
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(2);
            roleItems.Add(UserRole.CENTER_OPERATOR, userItems);

            //CENTER MANAGER
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            roleItems.Add(UserRole.CENTER_MANAGER, userItems);

            menuItems.Add(MenuHeader.CARD_REQUEST_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.CARD_REQUEST_MENU, new DisplayMenu(Resources.MenuItems.CardRequestHeading, "", "CardRequest"));
        }

        private void BuildCardProductionMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu(Resources.MenuItems.CardProductionUpload,
                                         "~\\webpages\\dist\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.APPROVED_FOR_PRODUCTION).ToString()
                                         //+ "&issueMethod=0"
                                         , "cmsUploadRequestBatch"));

            items.Add(2, new DisplayMenu(Resources.MenuItems.CardProductionWorkInProgress,
                                         "~\\webpages\\dist\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.SENT_TO_CMS).ToString()
                                         //+ "&issueMethod=0",
                                         , "cmsUploadRequestBatch2"));

            items.Add(3, new DisplayMenu("Batches for Production",
                                         "~\\webpages\\dist\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.PROCESSED_IN_CMS).ToString()
                                         //+ "&issueMethod=0",
                                         , "cmsUploadRequestBatch3"));

            items.Add(4, new DisplayMenu("Confirm Production",
                                         "~\\webpages\\dist\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.AT_CARD_PRODUCTION).ToString()
                                         //+ "&issueMethod=0",
                                         , "cmsUploadRequestBatch3"));

            //CMS OPERATOR
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(3);
            userItems.Remove(4);
            roleItems.Add(UserRole.CMS_OPERATOR, userItems);

            //CARD PRODUCTION
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            roleItems.Add(UserRole.CARD_PRODUCTION, userItems);

            menuItems.Add(MenuHeader.CARD_PRODUCTION_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.CARD_PRODUCTION_MENU, new DisplayMenu(Resources.MenuItems.CardProductionHeading, "", "CardProduction"));
        }

        /// <summary>
        /// Build User Group Admin SubMenu Items.
        /// </summary>
        private void BuildUserGroupAdminSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu(Resources.MenuItems.UserGroupAdminCreate, "~\\webpages\\users\\UserGroupViewForm.aspx", "createUserGroup"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.UserGroupAdminManage, "~\\webpages\\users\\UserGroupMaintanance.aspx", "manageUserGroup"));

            roleItems.Add(UserRole.USER_GROUP_ADMIN, items);

            menuItems.Add(MenuHeader.USER_GROUP_ADMIN_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.USER_GROUP_ADMIN_MENU, new DisplayMenu(Resources.MenuItems.UserGroupAdminHeading, "", "userGroupManagement"));
            //Issuer Admin UserGroupManagement Menu
            //var userGroupManagement = new DisplayMenu("User Group Management", "", "userGroupManagement");
        }

        /// <summary>
        /// Build Issuer Admin SubMenu Items.
        /// </summary>
        private void BuildIssuerAdminAdminSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu(Resources.MenuItems.IssuerAdminCreate, "~\\webpages\\issuer\\IssuerManagement.aspx", "createIssuer"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.IssuerAdminManage, "~\\webpages\\issuer\\IssuerList.aspx", "manageIssuer"));

            roleItems.Add(UserRole.ISSUER_ADMIN, items);

            menuItems.Add(MenuHeader.ISSUER_ADMIN_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.ISSUER_ADMIN_MENU, new DisplayMenu(Resources.MenuItems.IssuerAdminHeading, "", "issuerMenu"));
            //System Admin
            //var issuerMenu = new DisplayMenu("Issuer", "", "issuerMenu");
        }

        /// <summary>
        /// Build Branch Admin SubMenu Items.
        /// </summary>
        private void BuildBranchAdminAdminSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu(Resources.MenuItems.BranchAdminCreate, "~\\webpages\\issuer\\ManageBranch.aspx", "createBranch"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.BranchAdminManage, "~\\webpages\\issuer\\BranchList.aspx", "manageBranch"));

            roleItems.Add(UserRole.BRANCH_ADMIN, items);

            menuItems.Add(MenuHeader.BRANCH_ADMIN_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.BRANCH_ADMIN_MENU, new DisplayMenu(Resources.MenuItems.BranchAdminHeading, "", "branchManagement"));
            //Branch Admin User Branch Management Menu
            //var branchManagement = new DisplayMenu("Branch Management", "", "branchManagement");
        }

        /// <summary>
        /// Build Load Batch Admin SubMenu Items.
        /// </summary>
        private void BuildLoadBatchAdminAdminSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            items.Add(1, new DisplayMenu(Resources.MenuItems.LoadBatchAdminAwaitingApproval, "~\\webpages\\load\\LoadBatchList.aspx", "awaitingApproval"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.LoadBatchAdminSearch, "~\\webpages\\load\\LoadBatchSearch.aspx", "searchLoadBatch"));

            roleItems.Add(UserRole.CENTER_MANAGER, items);
            roleItems.Add(UserRole.CARD_PRODUCTION, items);

            //AUDITOR
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            roleItems.Add(UserRole.AUDITOR, userItems);

            menuItems.Add(MenuHeader.LOAD_BATCH_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.LOAD_BATCH_MENU, new DisplayMenu(Resources.MenuItems.LoadBatchAdminHeading, "", "LoadBatch"));
            //Load batch Center Manager
            //var loadBatchMenu = new DisplayMenu("Load Batch", "", "LoadBatch");
        }

        /// <summary>
        /// Build Distribution Batch Admin SubMenu Items.
        /// </summary>
        private void BuildDistBatchAdminAdminSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            //Please update the excludeFromMenu method if the submenu item id's are changed.            
            items.Add(1, new DisplayMenu(Resources.MenuItems.DistBatchAdminCreate, "~\\webpages\\dist\\DistBatchCreate.aspx?direction=0", "createDistBatch"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.DistBatchAdminApproval,
                                         "~\\webpages\\dist\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.CREATED).ToString()
                                         + "&distBatchTypeId=1",
                                         "manageDistBatch"));
            items.Add(3, new DisplayMenu(Resources.MenuItems.DistBatchAdminDispatch,
                                         "~\\webpages\\dist\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.APPROVED).ToString()
                                         + "&distBatchTypeId=1",
                                         "manageDistOpBatch"));
            items.Add(4, new DisplayMenu(Resources.MenuItems.DistBatchAdminReceive,
                                         "~\\webpages\\dist\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.DISPATCHED).ToString()
                                         + "&distBatchTypeId=1",
                                         "BraAwaitingApproval"));
            items.Add(5, new DisplayMenu(Resources.MenuItems.DistBatchAdminReceive,
                                         "~\\webpages\\dist\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.CARDS_PRODUCED).ToString()
                                            + "&distBatchTypeId=1",
                                         "cmReceiveBatch"));
            items.Add(6, new DisplayMenu("Rejected At Branch",
                                         "~\\webpages\\dist\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.REJECTED_AT_BRANCH).ToString()
                                            + "&distBatchTypeId=1",
                                         "cmRejectedAtBranchBatch"));

            //CENTER_OPERATOR : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(2);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(6);
            roleItems.Add(UserRole.CENTER_OPERATOR, userItems);


            //CENTER_MANAGER : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(7);
            roleItems.Add(UserRole.CENTER_MANAGER, userItems);

            //BRANCH_CUSTODIAN : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            roleItems.Add(UserRole.BRANCH_CUSTODIAN, userItems);

            menuItems.Add(MenuHeader.DIST_BATCH_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.DIST_BATCH_MENU, new DisplayMenu(Resources.MenuItems.DistBatchAdminHeading, "", "distBatch"));
            //  Distribution Batch    
            //var distBatchOpMenu = new DisplayMenu("Distribution Batch", "", "distBatch");            
        }

        /// <summary>
        /// Build Vault SubMenu Items.
        /// </summary>
        private void BuildVaultAdminSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu(Resources.MenuItems.VaultAdminCheckInOut, "~\\webpages\\card\\CardCheckInOut.aspx", "checkinoutcards"));

            roleItems.Add(UserRole.BRANCH_CUSTODIAN, items);
            menuItems.Add(MenuHeader.VAULT_MENU, roleItems);

            parentMenuItems.Add(MenuHeader.VAULT_MENU, new DisplayMenu(Resources.MenuItems.VaultAdminHeading, "", "vaultManagement"));
            //var vaultMenu = new DisplayMenu("Vault Management", "", "vaultManagement");
        }

        /// <summary>
        /// Build Card Issuing SubMenu Items.
        /// </summary>
        private void BuildCardIssuingSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            items.Add(1, new DisplayMenu(Resources.MenuItems.CardAdminOrderCard, "~\\webpages\\card\\CardCapture.aspx?issuetype=0", "requestNewCard", 0));
            //items.Add(2, new DisplayMenu(Resources.MenuItems.CardAdminIssueCard, "~\\webpages\\card\\CardCapture.aspx?issuetype=1", "issueNewCard"));

            items.Add(2, new DisplayMenu(Resources.MenuItems.CardAdminIssueInstantCard, "~\\webpages\\card\\CardCapture.aspx?issuetype=1", "issueInstantCard", 1));

            items.Add(9, new DisplayMenu("Order Hybrid Card", "~\\webpages\\card\\CardCapture.aspx?issuetype=1", "OrderHybridCard", 1));
            items.Add(8, new DisplayMenu(Resources.MenuItems.CardAdminIssueCentralisedCard, "~\\webpages\\card\\CustomerCardSearch.aspx", "issueCentralisedCard", 0));

            items.Add(3, new DisplayMenu(Resources.MenuItems.CardAdminReviewCards, "~\\webpages\\card\\CardList.aspx?status=" + (int)BranchCardStatus.ALLOCATED_TO_CUST, "reviewcards"));
            items.Add(4, new DisplayMenu(Resources.MenuItems.CardAdminInProgress, "~\\webpages\\card\\CardList.aspx?status=" + (int)BranchCardStatus.APPROVED_FOR_ISSUE, "cardsinprogress", 1));
            items.Add(5, new DisplayMenu(Resources.MenuItems.CardAdminInError, "~\\webpages\\card\\CardList.aspx?status=" + (int)BranchCardStatus.PRINT_ERROR, "cardsinerror", 1));
            items.Add(6, new DisplayMenu("Rejected Cards", "~\\webpages\\card\\CardList.aspx?status=11", "cardsrejected", 0));
            items.Add(7, new DisplayMenu("Pin Mailer Requests", "~\\webpages\\pin\\PinMailerReprintList.aspx?status=0", "pinMailerRequests", 0));
            items.Add(10, new DisplayMenu("Hybrid Requests", "~\\webpages\\hybrid\\HybridRequestList.aspx?status=" + (int)HybridRequestStatuses.CREATED, "reviewcards"));
            items.Add(11, new DisplayMenu(Resources.MenuItems.CardAdminIssueHybridCard, "~\\webpages\\card\\CustomerCardSearch.aspx", "issueHybridCard", 1));

            items.Add(12, new DisplayMenu("Super InstantCard", "~\\webpages\\card\\InstantCard.aspx?issuetype=1", "issueInstantCard", 1));
            items.Add(13, new DisplayMenu("Credit Limit Review", "~\\webpages\\card\\CardList.aspx?status=" + (int)BranchCardStatus.CREDIT_READY_TO_ANALYZE, "reviewcreditlimit"));
            items.Add(14, new DisplayMenu("Credit Limit Approve", "~\\webpages\\card\\CardList.aspx?status=" + (int)BranchCardStatus.CREDIT_READY_TO_APPROVE, "finalizecreditlimit"));
            items.Add(15, new DisplayMenu("Credit Contract Number", "~\\webpages\\card\\CardList.aspx?status=" + (int)BranchCardStatus.CREDIT_CONTRACT_CAPTURE, "capturecreditcontractnumber"));
            items.Add(16, new DisplayMenu(Resources.MenuItems.CardAdminIssueCardRenewal, "~\\webpages\\card\\RenewalCardSearch.aspx", "issueRenewalCard", 0));
            items.Add(17, new DisplayMenu(Resources.MenuItems.CardActivation, "~\\webpages\\card\\CustomerCardSearch.aspx?ActivationType=2", "activeCard", 2));

            //DisplayMenu viewIssuedCard = new DisplayMenu("Recently issued cards", "\\webpages\\card\\CardList.aspx", "reIssuerCard");

            //BRANCH_CUSTODIAN : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(8);
            userItems.Remove(4);
            userItems.Remove(6);
            userItems.Remove(9);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            roleItems.Add(UserRole.BRANCH_CUSTODIAN, userItems);

            //PIN_OPERATOR : Remove items from list that role does not have access to.
            //userItems = new Dictionary<int, DisplayMenu>(items);
            //userItems.Remove(1);
            //userItems.Remove(2);
            //userItems.Remove(3);
            //userItems.Remove(5);
            //userItems.Remove(7);
            ////userItems.Remove(8);
            //userItems.Remove(8);
            //userItems.Remove(10);
            //userItems.Remove(13);
            //userItems.Remove(14);
            //userItems.Remove(15);
            //userItems.Remove(16);
            //userItems.Remove(17);
            //roleItems.Add(UserRole.PIN_OPERATOR, userItems);

            //BRANCH_OPERATOR : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(3);
            userItems.Remove(5);
            userItems.Remove(7);
            userItems.Remove(10);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(17);
            roleItems.Add(UserRole.BRANCH_OPERATOR, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            roleItems.Add(UserRole.CREDIT_ANALYST, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            roleItems.Add(UserRole.CREDIT_MANAGER, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            roleItems.Add(UserRole.CENTER_OPERATOR, userItems);

            menuItems.Add(MenuHeader.ISSUE_CARD_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.ISSUE_CARD_MENU, new DisplayMenu(Resources.MenuItems.CardAdminHeading, "", "cardIssuing"));
            //Card issue Menu
            //var cardIssuing = new DisplayMenu("Card Issuing", "", "cardIssuing");
        }

        /// <summary>
        /// Build My Admin SubMenu Items.
        /// </summary>
        private void BuildMyAdminSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            items.Add(1, new DisplayMenu(Resources.MenuItems.MyAdminPassword, "~\\webpages\\account\\UserPasswordMaintainance.aspx", "changePassword"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.MyAdminLanguage, "~\\webpages\\account\\UserLanguage.aspx", "changeLanguage"));
            items.Add(3, new DisplayMenu(Resources.MenuItems.MyAdminPinAuthorisation, "~\\webpages\\account\\UserPinAuthorisationMaintainance.aspx", "changeAuthPin"));

            if (LdapUser)
            {
                items.Remove(1);
                items.Remove(3);
            }

            //Add this for all roles.
            foreach (UserRole userRole in System.Enum.GetValues((typeof(UserRole))))
            {
                if (userRole.Equals(UserRole.BRANCH_CUSTODIAN))
                {
                    roleItems.Add(userRole, items);
                }
                else
                {
                    userItems = new Dictionary<int, DisplayMenu>(items);
                    userItems.Remove(3);
                    roleItems.Add(userRole, userItems);
                }

            }

            menuItems.Add(MenuHeader.MY_ACCOUNT_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.MY_ACCOUNT_MENU, new DisplayMenu(Resources.MenuItems.MyAdminHeading, "", "myAccount"));
            //My Account
            //var myAccount = new DisplayMenu("My Account", "", "myAccount");
        }

        /// <summary>
        /// Build Audit SubMenu Items.
        /// </summary>
        private void BuildAuditSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu(Resources.MenuItems.AuditAdminView, "~\\webpages\\audit\\AuditLogSelectionForm.aspx", "viewAuditReport"));
            //items.Add(2, new DisplayMenu("Batch search", "~\\webpages\\card\\PINSearch.aspx", "searchPINBatch"));

            roleItems.Add(UserRole.AUDITOR, items);

            menuItems.Add(MenuHeader.AUDITOR_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.AUDITOR_MENU, new DisplayMenu(Resources.MenuItems.AuditAdminHeading, "", "auditManager"));
            //Audit Management       
            //var auditManger = new DisplayMenu("Audit Manager", "", "auditManager");
        }

        /// <summary>
        /// Build Licensing SubMenu Items.
        /// </summary>
        private void BuildLicenseManagerSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu(Resources.MenuItems.LicenceAdminIssuer, "~\\webpages\\system\\IssuerLicenseManager.aspx", "issuerLicenseManager"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.LicenceAdminList, "~\\webpages\\system\\IssuerLicenseList.aspx", "listLicenses"));

            roleItems.Add(UserRole.ADMINISTRATOR, items);

            menuItems.Add(MenuHeader.LICENSE_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.LICENSE_MENU, new DisplayMenu(Resources.MenuItems.LicenceAdminHeading, "", "licenseManager"));
            //License Manager
            //var licenseManager = new DisplayMenu("License Manager", "", "licenseManager");
        }

        /// <summary>
        /// Build Licensing SubMenu Items.
        /// </summary>
        private void BuildAdministrationSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();

            //items.Add(1, new DisplayMenu(Resources.MenuItems.SystemAdminLDAPManagement, "~\\webpages\\system\\LdapManagement.aspx", "ldapManagement"));
            //items.Add(2, new DisplayMenu(Resources.MenuItems.SystemAdminLDAPList, "~\\webpages\\system\\LdapList.aspx", "ldapList"));

            items.Add(1, new DisplayMenu(Resources.MenuItems.SystemAdminInterfaceManagement, "~\\webpages\\system\\InterfaceManagement.aspx", "interfaceManagement"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.SystemAdminInterfaceList, "~\\webpages\\system\\InterfaceList.aspx", "interfaceList"));
            items.Add(3, new DisplayMenu(Resources.MenuItems.UseradminSettingMaintanance, "~\\webpages\\system\\UseradminSettingsViewForm.aspx", "UseradminSetting"));

            items.Add(4, new DisplayMenu(Resources.MenuItems.ExternalSystems, "~\\webpages\\system\\ExternalSystemsViewForm.aspx", "ExternalSystemsViewForm"));
            items.Add(5, new DisplayMenu(Resources.MenuItems.ManageExternalSystems, "~\\webpages\\system\\ExternalSystemsMaintanance.aspx", "ExternalSystemsMaintanance"));

            items.Add(6, new DisplayMenu(Resources.MenuItems.BatchNotificationList, "~\\webpages\\system\\BatchNotificationList.aspx", "BatchNotificationList"));
            items.Add(7, new DisplayMenu(Resources.MenuItems.ManageBatchNotifications, "~\\webpages\\system\\ManageBatchNotifications.aspx", "ManageBatchNotifications"));

            items.Add(8, new DisplayMenu(Resources.MenuItems.BranchNotificationList, "~\\webpages\\system\\BranchNotificationList.aspx", "BranchNotificationList"));
            items.Add(9, new DisplayMenu(Resources.MenuItems.ManageBranchNotification, "~\\webpages\\system\\ManageBranchNotification.aspx", "ManageBranchNotification"));

            items.Add(10, new DisplayMenu("Manage Authentication Configuration", "~\\webpages\\system\\ManageAuthConfiguration.aspx", "ManageAuthConfiguration"));
            items.Add(11, new DisplayMenu("Authentication Configuration List", "~\\webpages\\system\\AuthConfigurationList.aspx", "AuthConfigurationList"));
            items.Add(12, new DisplayMenu("Create Document Type", "~\\webpages\\system\\DocumentTypeEdit.aspx", "DocumentTypeEdit"));
            items.Add(13, new DisplayMenu("Document Type List", "~\\webpages\\system\\DocumentTypeList.aspx", "DocumentTypeList"));
            //items.Add(14, new DisplayMenu("Configure Rest Webservice", "~\\webpages\\system\\RestInterfaceManagement.aspx", "RestInterfaceManagement"));
            //items.Add(15, new DisplayMenu("Manage Rest Webservice", "~\\webpages\\system\\InterfaceList.aspx", "interfaceList"));

            roleItems.Add(UserRole.ADMINISTRATOR, items);

            menuItems.Add(MenuHeader.ADMIN_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.ADMIN_MENU, new DisplayMenu(Resources.MenuItems.SystemAdminHeading, "", "systemAdmin"));
        }

        /// <summary>
        /// Build File Loader SubMenu Items.
        /// </summary>
        private void BuildFileLoaderSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu(Resources.MenuItems.FileLoadList, "~\\webpages\\load\\FileLoadList.aspx", "fileLoadList"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.FileLoaderAdminView, "~\\webpages\\load\\FileLoaderLogSearch.aspx", "searchFileLoader"));

            roleItems.Add(UserRole.CENTER_MANAGER, items);
            roleItems.Add(UserRole.ADMINISTRATOR, items);
            roleItems.Add(UserRole.AUDITOR, items);
            roleItems.Add(UserRole.CARD_PRODUCTION, items);

            menuItems.Add(MenuHeader.FILE_LOAD_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.FILE_LOAD_MENU, new DisplayMenu(Resources.MenuItems.FileLoaderAdminHeading, "", "fileLoaderMenu"));
            //File loader menu
            //var fileLoaderMenu = new DisplayMenu("File Loader", "", "fileLoaderMenu");
        }

        //private void BuildCardStockOrdering(List<StatusFlowRole> FlowStatus)
        //{
        //    roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
        //    items = new Dictionary<int, DisplayMenu>();
        //    //Dictionary<int, DisplayMenu> userItems = new Dictionary<int, DisplayMenu>();

        //    //Based on DistBatchStatus and CardIssueMethod and DistBatchType, build the menu items 
        //    //Only Operator can Order stock
        //    //items.Add(1, new DisplayMenu(Resources.MenuItems.CardStockOrdering, "~\\webpages\\card\\CardStockOrdering.aspx", "CardStockOrdering"));

        //    //Start at the next idex after the above
        //    foreach (var roleId in FlowStatus.Select(s => s.RoleId).Distinct().ToList())
        //    {
        //        items = new Dictionary<int, DisplayMenu>();

        //        int i = 2;
        //        foreach (var item in FlowStatus.Where(w=>w.RoleId == roleId && w.CardIssueMethodId == 1 && w.DistBatchTypeId == 0))
        //        {
        //            items.Add(i, new DisplayMenu(item.MenuName,
        //                                     "~\\webpages\\card\\DistBatchList.aspx?status=" + item.DistBatchStatusId.ToString()
        //                                     + "&issueMethod=1"
        //                                     + "&distBatchTypeId=0",
        //                                     item.MenuName + i.ToString()));
        //            i++;
        //        }

        //        roleItems.Add((UserRole)roleId, items);
        //    }

        //    ResourceManager temp = new ResourceManager("Resources.MenuItems", System.Reflection.Assembly.Load("App_GlobalResources"));
        //    temp.GetString("menuItem1");

        //    //Must be
        //    //items.Add(2, new DisplayMenu(Resources.MenuItems.CardStocktBatchPreApprove,
        //    //                             "~\\webpages\\card\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.CREATED).ToString()
        //    //                             + "&issueMethod=1",
        //    //                             "preApproveStockRequestBatch"));
        //    //items.Add(3, new DisplayMenu(Resources.MenuItems.CardStocktBatchPostApprove,
        //    //                             "~\\webpages\\card\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.PROCESSED_IN_CMS).ToString()
        //    //                             + "&issueMethod=1",
        //    //                             "postApproveStockRequestBatch"));  


        //    ////CENTER OPERATOR
        //    //userItems = new Dictionary<int, DisplayMenu>(items);
        //    //userItems.Remove(2);
        //    //userItems.Remove(3);
        //    //roleItems.Add(UserRole.CENTER_OPERATOR, userItems);

        //    ////CENTER MANAGER
        //    //userItems = new Dictionary<int, DisplayMenu>(items);
        //    //userItems.Remove(1);
        //    //roleItems.Add(UserRole.CENTER_MANAGER, userItems);

        //    menuItems.Add(MenuHeader.CARD_STOCK_MENU, roleItems);
        //    parentMenuItems.Add(MenuHeader.CARD_STOCK_MENU, new DisplayMenu(Resources.MenuItems.CardStockOrdering, "", "CardStockOrdering"));
        //}


        private void BuildPrintSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            items.Add(1, new DisplayMenu("Approve For Print",
                                         "~\\webpages\\hybrid\\PrintBatchList.aspx?status=" + ((int)PrintBatchStatuses.CREATED),
                                         "approveprintBatch"));

            items.Add(2, new DisplayMenu("Approve For CMS",
                                       "~\\webpages\\hybrid\\PrintBatchList.aspx?status=" + ((int)PrintBatchStatuses.PRINT_SUCESSFUL),
                                       "approveprintBatch"));

            items.Add(3, new DisplayMenu("Create Batch",
                                        "~\\webpages\\classic\\RequestBatchCreate.aspx?issueMethod=1"
                                        , "requestCreate", 1));

            items.Add(4, new DisplayMenu("Re-upload to CMS",
                                    "~\\webpages\\hybrid\\PrintBatchList.aspx?status=" + ((int)PrintBatchStatuses.CMS_ERROR),
                                    "reuploadtocms"));


            items.Add(5, new DisplayMenu("Print Search",
                                       "~\\webpages\\hybrid\\PrintBatchSearch.aspx",
                                       "printBatch"));

            items.Add(6, new DisplayMenu("Hybrid Request Search",
                                                "~\\webpages\\hybrid\\HybridRequestSearch.aspx",
                                                "hybridRequestSearch"));


            items.Add(9, new DisplayMenu(Resources.MenuItems.DistBatchAdminDispatch,
                "~\\webpages\\dist\\DistBatchList.aspx?status=" + ((int)DistributionBatchStatus.APPROVED).ToString()
                                                                + "&distBatchTypeId=1", "manageDistOpBatch"));

            //PIN_PRINT_OPERATOR : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(3);
            userItems.Remove(2);
            userItems.Remove(4);
            roleItems.Add(UserRole.BRANCH_PRODUCT_MANAGER, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(7);
            userItems.Remove(9);

            roleItems.Add(UserRole.BRANCH_PRODUCT_OPERATOR, userItems);
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(3);
            userItems.Remove(2);
            userItems.Remove(4);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(9);

            roleItems.Add(UserRole.CENTER_MANAGER, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(3);
            userItems.Remove(2);
            userItems.Remove(4);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(9);

            roleItems.Add(UserRole.CENTER_OPERATOR, userItems);

            menuItems.Add(MenuHeader.PRINT_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.PRINT_MENU, new DisplayMenu("Print Batch", "", "printbatch"));
        }
        /// <summary>
        /// Build PIN SubMenu Items.
        /// </summary>
        private void BuildPinSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            items.Add(1, new DisplayMenu("Dispatch to Card Centre",
                                         "~\\webpages\\pin\\PinBatchList.aspx?status=0&pinBatchTypeId=0",
                                         "printPinBatch"));
            items.Add(2, new DisplayMenu("Receive At Card Centre",
                                            "~\\webpages\\pin\\PinBatchList.aspx?status=1",
                                            "recievePinsAtCC"));
            items.Add(3, new DisplayMenu("Dispatch To Branch",
                                            "~\\webpages\\pin\\PinBatchList.aspx?status=0&pinBatchTypeId=1",
                                            "dispatchPinsToBranch"));
            items.Add(4, new DisplayMenu("Recieve At Branch",
                                            "~\\webpages\\pin\\PinBatchList.aspx?status=3&pinBatchTypeId=1",
                                            "recievePinsAtBranch"));
            items.Add(5, new DisplayMenu("Rejected At Card Centre",
                                            "~\\webpages\\pin\\PinBatchList.aspx?status=5&pinBatchTypeId=0",
                                            "recievePinsAtBranch"));
            items.Add(6, new DisplayMenu("Rejected At Branch",
                                            "~\\webpages\\pin\\PinBatchList.aspx?status=6&pinBatchTypeId=1",
                                            "recievePinsAtBranch"));
            items.Add(7, new DisplayMenu("Create Reprint Batch",
                                            "~\\webpages\\pin\\ReprintBatchCreate.aspx",
                                            "reprintBatchCreate"));
            items.Add(8, new DisplayMenu("Approve Reprint Batch",
                                            "~\\webpages\\pin\\PinBatchList.aspx?status=0&pinBatchTypeId=2",
                                            "approveReprintBatch"));
            items.Add(9, new DisplayMenu("Reprint Pins",
                                            "~\\webpages\\pin\\PinBatchList.aspx?status=11&pinBatchTypeId=2",
                                            "reprintPinBatch"));
            items.Add(10, new DisplayMenu("Confirm Reprint Pins",
                                            "~\\webpages\\pin\\PinBatchList.aspx?status=7&pinBatchTypeId=2",
                                            "confirmReprintPinBatch"));
            items.Add(11, new DisplayMenu("Upload Reprint Pins",
                                            "~\\webpages\\pin\\PinBatchList.aspx?status=8&pinBatchTypeId=2",
                                            "uploadReprintBatch"));
            items.Add(12, new DisplayMenu("Confirm Upload Reprint Pins",
                                            "~\\webpages\\pin\\PinBatchList.aspx?status=9&pinBatchTypeId=2",
                                            "confirmUploadReprintBatch"));
            items.Add(13, new DisplayMenu("Issue Instant Electronic Pin",
                                           "~\\webpages\\pin\\instantEPinRequest.aspx?status=0",
                                           "IssuesInstantEPin"));
            items.Add(14, new DisplayMenu("Review Electronic Pin Requests", "~\\webpages\\pin\\PinRequestList.aspx?status=0&requestType=0", "ReviewEpinRequests"));
            items.Add(15, new DisplayMenu("Resend Electronic Pin", "~\\webpages\\pin\\PinRequestList.aspx?status=1&requestType=0", "ReviewSentPinRequests"));
            items.Add(16, new DisplayMenu("Rejected Pin Requests", "~\\webpages\\pin\\PinRequestList.aspx?status=2&requestType=0", "RejectedPinRequests"));
            items.Add(17, new DisplayMenu("Review Pin File Uploads", "~\\webpages\\pin\\PinMailerList.aspx?status=0", "ReviewPinFileUploads"));
            items.Add(18, new DisplayMenu("Search File Uploads", "~\\webpages\\pin\\PinMailerList.aspx?status=0", "ApprovedPinFileUploads"));
            // items.Add(19, new DisplayMenu("View Rejected Pin Files", "~\\webpages\\pin\\PinMailerList.aspx?status=2", "RejectedPinFileUploads"));

            //PIN_PRINT_OPERATOR : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            userItems.Remove(18);
            //  userItems.Remove(19);
            roleItems.Add(UserRole.PIN_PRINTER_OPERATOR, userItems);

            //PIN_PRINT_OPERATOR : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            userItems.Remove(18);
            //userItems.Remove(19);
            roleItems.Add(UserRole.PIN_OPERATOR, userItems);

            //CARD_CENTRE_PIN_OFFICER : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            userItems.Remove(18);
            //  userItems.Remove(19);
            roleItems.Add(UserRole.CARD_CENTRE_PIN_OFFICER, userItems);

            //BRANCH_PIN_OFFICER : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            userItems.Remove(18);
            // userItems.Remove(19);
            roleItems.Add(UserRole.BRANCH_PIN_OFFICER, userItems);

            //CENTER_OPERATOR : Remove items from list that role does not have access to.
            //userItems = new Dictionary<int, DisplayMenu>(items);
            //userItems.Remove(1);
            //userItems.Remove(2);
            //userItems.Remove(3);
            //userItems.Remove(4);
            //userItems.Remove(5);
            //userItems.Remove(6);
            //userItems.Remove(8);
            //userItems.Remove(9);
            //userItems.Remove(10);
            //userItems.Remove(11);
            //userItems.Remove(12);
            //userItems.Remove(13);
            //userItems.Remove(14);
            //userItems.Remove(17);
            //userItems.Remove(18);
            //userItems.Remove(19);
            //roleItems.Add(UserRole.CENTER_OPERATOR, userItems);

            //CENTER_MANAGER : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(11);
            userItems.Remove(12);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            roleItems.Add(UserRole.CENTER_MANAGER, userItems);

            //CMS_OPERATOR : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(13);
            userItems.Remove(14);
            userItems.Remove(15);
            userItems.Remove(16);
            userItems.Remove(17);
            userItems.Remove(18);
            // userItems.Remove(19);
            roleItems.Add(UserRole.CMS_OPERATOR, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            userItems.Remove(5);
            userItems.Remove(6);
            userItems.Remove(7);
            userItems.Remove(8);
            userItems.Remove(9);
            userItems.Remove(10);
            userItems.Remove(13);
            userItems.Remove(17);
            userItems.Remove(18);
            // userItems.Remove(19);
            roleItems.Add(UserRole.BRANCH_CUSTODIAN, userItems);

            menuItems.Add(MenuHeader.PIN_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.PIN_MENU, new DisplayMenu("Pin Batch", "", "pinMailer"));
        }

        private void BuildProductionMenus(List<StatusFlowRole> FlowStatus)
        {
            //First By Main Menu Item, then by submenu, which is ordered
            foreach (var mainMenuId in FlowStatus.OrderBy(o => o.MainMenuId).Select(s => s.MainMenuId).Distinct())
            {
                MenuHeader menuHeader;

                switch (mainMenuId)
                {
                    case 1: menuHeader = MenuHeader.MAIN_MENU_1; break;
                    case 2: menuHeader = MenuHeader.MAIN_MENU_2; break;
                    case 3: menuHeader = MenuHeader.MAIN_MENU_3; break;
                    case 4: menuHeader = MenuHeader.MAIN_MENU_4; break;
                    case 5: menuHeader = MenuHeader.MAIN_MENU_5; break;
                    case 6: menuHeader = MenuHeader.MAIN_MENU_6; break;
                    case 7: menuHeader = MenuHeader.MAIN_MENU_7; break;
                    case 8: menuHeader = MenuHeader.MAIN_MENU_8; break;
                    case 9: menuHeader = MenuHeader.MAIN_MENU_9; break;
                    case 10: menuHeader = MenuHeader.MAIN_MENU_10; break;
                    case -1: continue;
                    default: throw new ArgumentException("No Custom Main Menu Item with that ID.");
                }

                ResourceManager mainMenu = new ResourceManager("Resources.MainMenu", System.Reflection.Assembly.Load("App_GlobalResources"));
                ResourceManager subMenu = new ResourceManager("Resources.SubMenu", System.Reflection.Assembly.Load("App_GlobalResources"));

                roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();

                foreach (var roleId in FlowStatus.Where(w => w.MainMenuId == mainMenuId).Select(s => s.RoleId).Distinct())
                {
                    items = new Dictionary<int, DisplayMenu>();

                    int i = 0;

                    foreach (var item in FlowStatus.OrderBy(o => o.OrderId).Where(w => w.MainMenuId == mainMenuId && w.RoleId == roleId).ToList())
                    {
                        if (item.DistBatchStatusId == -1 && item.CardIssueMethodId == 0 && item.DistBatchTypeId == 0)
                            items.Add(i, new DisplayMenu(Resources.MenuItems.CardRequestCreateBatch, "~\\webpages\\classic\\RequestBatchCreate.aspx?issueMethod=" + item.CardIssueMethodId.ToString(), "requestCreate", item.CardIssueMethodId));
                        else if (item.DistBatchStatusId == -1 && item.CardIssueMethodId == 2 && item.DistBatchTypeId == 0)
                            items.Add(i, new DisplayMenu(Resources.MenuItems.CardRequestBatchCentral, "~\\webpages\\classic\\RequestBatchCreate.aspx?issueMethod=" + item.CardIssueMethodId.ToString(), "requestCreate", item.CardIssueMethodId));
                        else if (item.DistBatchStatusId == -1 && item.CardIssueMethodId == 1 && item.DistBatchTypeId == 0)
                            items.Add(i, new DisplayMenu(Resources.MenuItems.CardStockOrdering, "~\\webpages\\card\\CardStockOrdering.aspx", "CardStockOrdering", 1));
                        else if (item.DistBatchStatusId == -1 && item.CardIssueMethodId == 1 && item.DistBatchTypeId == 1)
                            items.Add(i, new DisplayMenu(subMenu.GetString("SubMenuId" + item.SubMenuId.ToString()), "~\\webpages\\dist\\DistBatchCreate.aspx?direction=1", "CreateDistributionBatch", 1));
                        else
                            items.Add(i, new DisplayMenu(subMenu.GetString("SubMenuId" + item.SubMenuId.ToString()),
                                                     "~\\webpages\\dist\\DistBatchList.aspx?status=" + item.DistBatchStatusId.ToString()
                                                     + "&issueMethod=" + item.CardIssueMethodId
                                                     + "&distBatchTypeId=" + item.DistBatchTypeId,
                                                     subMenu.GetString("SubMenuId" + item.SubMenuId.ToString()), item.CardIssueMethodId));

                        i++;
                    }
                    //Role doesnt matter because we've done the lookup on the roles the user has... mmmm???
                    roleItems.Add((UserRole)roleId, items);
                }

                menuItems.Add(menuHeader, roleItems);
                parentMenuItems.Add(menuHeader, new DisplayMenu(mainMenu.GetString("MainMenuId" + mainMenuId), "", "MainMenuId" + mainMenuId));
            }
        }

        private void BuildSearchMenues()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            items.Add(1, new DisplayMenu(Resources.MenuItems.ProdBatchAdminSearch, "~\\webpages\\dist\\DistBatchSearch.aspx?batchType=0", "searchDistBatch"));
            items.Add(2, new DisplayMenu(Resources.MenuItems.DistBatchAdminSearch, "~\\webpages\\dist\\DistBatchSearch.aspx?batchType=1", "searchProdBatch"));
            items.Add(3, new DisplayMenu(Resources.MenuItems.CardAdminSearch, "~\\webpages\\card\\CardSearch.aspx", "viewIssuedCard"));
            items.Add(4, new DisplayMenu("Customer Search", "~\\webpages\\card\\CustomerSearch.aspx", "viewCustomerCard"));
            items.Add(5, new DisplayMenu("Pin Batch Search", "~\\webpages\\pin\\PinBatchSearch.aspx", "SearchPinBatch"));

            //PIN_PRINT_OPERATOR : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            roleItems.Add(UserRole.PIN_PRINTER_OPERATOR, userItems);

            //CARD_CENTRE_PIN_OFFICER : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(2);
            userItems.Remove(3);
            roleItems.Add(UserRole.CARD_CENTRE_PIN_OFFICER, userItems);

            //BRANCH_PIN_OFFICER : Remove items from list that role does not have access to.
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            //userItems.Remove(3);
            roleItems.Add(UserRole.BRANCH_PIN_OFFICER, userItems);

            //BRANCH OPERATOR
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(5);
            roleItems.Add(UserRole.BRANCH_OPERATOR, userItems);

            //BRANCH_CUSTODIAN
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(5);
            roleItems.Add(UserRole.BRANCH_CUSTODIAN, userItems);

            //CENTER_OPERATOR
            userItems = new Dictionary<int, DisplayMenu>(items);
            //userItems.Remove(1);
            userItems.Remove(5);
            roleItems.Add(UserRole.CENTER_OPERATOR, userItems);

            //CENTER_MANAGER
            userItems = new Dictionary<int, DisplayMenu>(items);
            //userItems.Remove(1);
            roleItems.Add(UserRole.CENTER_MANAGER, userItems);

            //CMS OPERATOR
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(4);
            roleItems.Add(UserRole.CMS_OPERATOR, userItems);

            //CARD_PRODUCTION
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(2);
            userItems.Remove(3);
            userItems.Remove(5);
            roleItems.Add(UserRole.CARD_PRODUCTION, userItems);

            //AUDITOR
            userItems = new Dictionary<int, DisplayMenu>(items);
            roleItems.Add(UserRole.AUDITOR, userItems);

            menuItems.Add(MenuHeader.SEARCH_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.SEARCH_MENU, new DisplayMenu("Search", "", "search"));
        }

        private void BuildCardFeesSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();

            items.Add(1, new DisplayMenu("Create Fee Accounting", "~\\webpages\\product\\ProductFeeAccounting.aspx", "managefeescheme"));
            items.Add(2, new DisplayMenu("Manage Fee Accounting", "~\\webpages\\product\\ProductFeeAccountingList.aspx", "managefeeschemes"));

            items.Add(3, new DisplayMenu("Create Fee Scheme", "~\\webpages\\product\\ProductFeeDetails.aspx", "managefeescheme"));
            items.Add(4, new DisplayMenu("Manage Fee Schemes", "~\\webpages\\product\\ProductFeeList.aspx", "managefeeschemes"));
            items.Add(5, new DisplayMenu("Manage Fee Charges", "~\\webpages\\product\\ProductFeeCharges.aspx", "managefeecharges"));

            roleItems.Add(UserRole.ISSUER_ADMIN, items);

            menuItems.Add(MenuHeader.CARD_FEES_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.CARD_FEES_MENU, new DisplayMenu("Card Fees", "", "Card Fees"));
        }

        private void BuildTerminalSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            items.Add(1, new DisplayMenu("Create Master Key", "~\\webpages\\system\\MasterkeyManager.aspx", "mangeMasterKeys"));
            items.Add(2, new DisplayMenu("Manage Master Keys", "~\\webpages\\system\\MasterkeyList.aspx", "masterkeyList"));
            items.Add(3, new DisplayMenu("Create Terminal", "~\\webpages\\system\\TerminalManager.aspx", "manageTerminals"));
            items.Add(4, new DisplayMenu("Manage Terminals", "~\\webpages\\system\\TerminalList.aspx", "teminalsList"));
            items.Add(5, new DisplayMenu("Search Terminals", "~\\webpages\\system\\TerminalSearch.aspx", "TerminalSearch"));

            //items.Add(5, new DisplayMenu("<span style=\"cursor:pointer;\" onclick=\"$('#dlgPinPad').puidialog('show');\">Session Key</span>", "", "manageSessionKey", false));


            //Admin
            userItems = new Dictionary<int, DisplayMenu>(items);
            //userItems.Remove(5);
            roleItems.Add(UserRole.ADMINISTRATOR, userItems);

            //PinOperator
            //userItems = new Dictionary<int, DisplayMenu>(items);
            //userItems.Remove(1);
            //userItems.Remove(2);
            //userItems.Remove(3);
            //userItems.Remove(4);
            //roleItems.Add(UserRole.PIN_OPERATOR, userItems);

            menuItems.Add(MenuHeader.TERMINAL_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.TERMINAL_MENU, new DisplayMenu("Terminal Management", "", "Terminal Management"));
        }

        private void BuildPinResetSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            //RAB: replaced the PIN App with POS... kept this incase we need it
            items.Add(1, new DisplayMenu("PinPad PIN Reset", "~\\webpages\\pin\\PINReset.aspx", "padpinReset"));
            items.Add(2, new DisplayMenu("POS PIN Reset", "~\\webpages\\pin\\PINResetPOS.aspx", "pospinReset"));
            //items.Add(3, new DisplayMenu("Approve PIN Reset", "~\\webpages\\pin\\PINResetList.aspx?pinReissueStatusesId=0", "pinResetApprove"));

            items.Add(3, new DisplayMenu("Approve Pin Reset", "~\\webpages\\pin\\PinRequestList.aspx?status=0&requestType=1", "pinResetApprove"));

            //items.Add(4, new DisplayMenu("Finalise PIN Reset", "~\\webpages\\pin\\PINResetList.aspx?operatorFinalise=true", "pinResetFinalise"));
            items.Add(4, new DisplayMenu("Pin Reset Request",
                                          "~\\webpages\\pin\\instantEPinRequest.aspx?status=1",
                                          "IssuesInstantEPin"));
            items.Add(5, new DisplayMenu("Search", "~\\webpages\\pin\\PinSendSearch.aspx", "PinSendSearch"));
            //  items.Add(6, new DisplayMenu("e-PIN request", "~\\webpages\\pin\\E-PINRequest.aspx", "e-PINRequest"));


            //PinOperator
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(3);
            roleItems.Add(UserRole.PIN_OPERATOR, userItems);

            //BranchCust
            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(4);
            roleItems.Add(UserRole.BRANCH_CUSTODIAN, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            userItems.Remove(2);
            userItems.Remove(4);
            userItems.Remove(5);
            roleItems.Add(UserRole.CENTER_OPERATOR, userItems);
            roleItems.Add(UserRole.CENTER_MANAGER, userItems);


            menuItems.Add(MenuHeader.PIN_RESET_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.PIN_RESET_MENU, new DisplayMenu("Pin Reset", "", "Pin Reset"));
        }

        private void BuildExportBatchSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            //items.Add(1, new DisplayMenu("Export", "~\\webpages\\system\\PINReset.aspx", "export"));
            items.Add(1, new DisplayMenu("Approve Export", "~\\webpages\\export\\ExportBatchList.aspx?status=3", "exportApprove"));
            items.Add(2, new DisplayMenu("Search", "~\\webpages\\export\\ExportBatchSearch.aspx", "exportSearch"));


            userItems = new Dictionary<int, DisplayMenu>(items);
            //userItems.Remove(1);
            roleItems.Add(UserRole.CENTER_MANAGER, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            userItems.Remove(1);
            roleItems.Add(UserRole.CMS_OPERATOR, userItems);

            menuItems.Add(MenuHeader.EXPORT_BATCH_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.EXPORT_BATCH_MENU, new DisplayMenu("Export Batch", "", "Export Batch"));
        }

        private void BuildAuditorMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            items.Add(1, new DisplayMenu(Resources.MenuItems.UserPendingList, "~\\webpages\\users\\UserPendingList.aspx", "UserPendingList"));

            userItems = new Dictionary<int, DisplayMenu>(items);
            roleItems.Add(UserRole.USER_AUDIT, userItems);

            menuItems.Add(MenuHeader.USER_AUDIT_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.USER_AUDIT_MENU, new DisplayMenu("User Audit", "", "User Audit"));
        }

        private void BuildRemoteSubMenu()
        {
            roleItems = new Dictionary<UserRole, Dictionary<int, DisplayMenu>>();
            items = new Dictionary<int, DisplayMenu>();
            Dictionary<int, DisplayMenu> userItems;

            items.Add(1, new DisplayMenu("Failed Updates", "~\\webpages\\remote\\RemoteCardUpdateList.aspx?status=4", "updateFailedStatus"));
            items.Add(2, new DisplayMenu("Search", "~\\webpages\\remote\\RemoteCardUpdateSearch.aspx", "updateSearch"));

            userItems = new Dictionary<int, DisplayMenu>(items);
            roleItems.Add(UserRole.CENTER_MANAGER, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            roleItems.Add(UserRole.CENTER_OPERATOR, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            roleItems.Add(UserRole.CMS_OPERATOR, userItems);

            userItems = new Dictionary<int, DisplayMenu>(items);
            roleItems.Add(UserRole.AUDITOR, userItems);

            menuItems.Add(MenuHeader.REMOTE_MENU, roleItems);
            parentMenuItems.Add(MenuHeader.REMOTE_MENU, new DisplayMenu("Remote Service", "", "Remote Service"));
        }
    }
}
