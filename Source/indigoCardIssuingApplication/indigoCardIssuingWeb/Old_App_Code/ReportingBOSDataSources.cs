using System.Collections.Generic;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;
using System;
using indigoCardIssuingWeb.CardIssuanceService;
using System.Web;
using System.Security.Principal;

namespace indigoCardIssuingWeb.App_Code
{
    public class InventoryReportingLables
    {
        public InventoryReportingLables()
        {

        }
        private List<InvReportLabels> bo_cards = new List<InvReportLabels>();
        public InventoryReportingLables(InvReportLabels labelvalues)
        {
            bo_cards.Add(labelvalues);

        }
        public List<InvReportLabels> GetReportLabels()
        {
            return bo_cards;
        }
    }
    public class IssueCardReportingLables
    {
        public IssueCardReportingLables()
        {

        }
        private List<IssueCardLabels> bo_cards = new List<IssueCardLabels>();
        public IssueCardReportingLables(IssueCardLabels labelvalues)
        {
            bo_cards.Add(labelvalues);

        }
        public List<IssueCardLabels> GetReportLabels()
        {
            return bo_cards;
        }
    }

    public class BranchOrderReportBO
    {
        private List<BranchOrderReportData> _results = new List<BranchOrderReportData>();

        public BranchOrderReportBO()
        {

        }

        public BranchOrderReportBO(List<BranchOrderReportResult> results)
        {
            _results = new List<BranchOrderReportData>();

            foreach (var item in results)
            {
                _results.Add(new BranchOrderReportData
                {
                    Issuer = String.Format("{0}-{1}", new string[] { item.issuer_name, item.issuer_code }),
                    Branch = String.Format("{0}-{1}", new string[] { item.branch_name, item.branch_code }),
                    CustomerFullName = String.Format("{0} {1} {2}", new string[] { item.customer_first_name, item.customer_middle_name ?? "", item.customer_last_name }),
                    MobileNumber = item.contact_number ?? String.Empty,
                    CustomerAccount = item.customer_account_number,
                    NameOnCard = item.name_on_card,
                    BatchReference = item.dist_batch_reference,
                    DateCreated = item.date_created
                });
            }
        }

        public List<BranchOrderReportData> GetResults()
        {
            return _results;
        }
    }

    public class CardProductionReportBO
    {
        private List<CardProductionReportData> _results = new List<CardProductionReportData>();

        public CardProductionReportBO()
        {

        }

        public CardProductionReportBO(List<CardProductionReportResult> results)
        {
            _results = new List<CardProductionReportData>();

            foreach (var item in results)
            {
                _results.Add(new CardProductionReportData
                {
                    Issuer = String.Format("{0}-{1}", new string[] { item.issuer_name, item.issuer_code }),
                    Branch = String.Format("{0}-{1}", new string[] { item.branch_name, item.branch_code }),
                    CustomerFullName = String.Format("{0} {1} {2}", new string[] { item.customer_first_name, item.customer_middle_name ?? "", item.customer_last_name }),
                    CustomerAccount = item.customer_account_number,
                    NameOnCard = item.name_on_card,
                    BatchReference = item.dist_batch_reference,
                    DateCreated = item.date_created,
                    PAN = item.card_number,
                    CardReferenceNumber = item.card_reference_number
                });
            }
        }

        public List<CardProductionReportData> GetResults()
        {
            return _results;
        }
    }

    public class CardDispatchReportBO
    {
        private List<CardDispatchReportData> _results = new List<CardDispatchReportData>();

        public CardDispatchReportBO()
        {

        }

        public CardDispatchReportBO(List<CardDispatchReportResult> results)
        {
            _results = new List<CardDispatchReportData>();

            foreach (var item in results)
            {
                _results.Add(new CardDispatchReportData
                {
                    Issuer = String.Format("{0}-{1}", new string[] { item.issuer_name, item.issuer_code }),
                    Branch = String.Format("{0}-{1}", new string[] { item.branch_name, item.branch_code }),
                    CustomerFullName = String.Format("{0} {1} {2}", new string[] { item.customer_first_name, item.customer_middle_name ?? "", item.customer_last_name }),
                    CustomerAccount = item.customer_account_number,
                    NameOnCard = item.name_on_card,
                    BatchReference = item.dist_batch_reference,
                    DateCreated = item.date_created,
                    PAN = item.card_number,
                    CardReferenceNumber = item.card_reference_number
                });
            }
        }

        public List<CardDispatchReportData> GetResults()
        {
            return _results;
        }
    }

    public class PinMailerReportBO
    {
        private List<PinMailerReportData> _results = new List<PinMailerReportData>();

        public PinMailerReportBO()
        {

        }

        public PinMailerReportBO(List<PinMailerReportResult> results)
        {
            _results = new List<PinMailerReportData>();

            foreach (var item in results)
            {
                _results.Add(new PinMailerReportData
                {
                    Issuer = String.Format("{0}-{1}", new string[] { item.issuer_name, item.issuer_code }),
                    Branch = String.Format("{0}-{1}", new string[] { item.branch_name, item.branch_code }),
                    CustomerAccount = item.customer_account_number,
                    BatchReference = item.pin_batch_reference,
                    DateCreated = item.date_created,
                    CustomerName=string.Empty
                });
            }
        }

        public PinMailerReportBO(List<PinMailerReprintReportResult> results)
        {
            _results = new List<PinMailerReportData>();

            foreach (var item in results)
            {
                _results.Add(new PinMailerReportData
                {
                    Issuer = String.Format("{0}-{1}", new string[] { item.issuer_name, item.issuer_code }),
                    Branch = String.Format("{0}-{1}", new string[] { item.branch_name, item.branch_code }),
                    CustomerAccount = item.customer_account_number,
                    BatchReference = item.pin_batch_reference,
                    DateCreated = item.date_created,
                    CustomerName=item.customer_name
                });
            }
        }

        public List<PinMailerReportData> GetResults()
        {
            return _results;
        }
    }

    public class CardExpiryReportBO
    {
        private List<CardExpiryReportData> _results = new List<CardExpiryReportData>();

        public CardExpiryReportBO()
        {

        }

        public CardExpiryReportBO(List<CardExpiryReportResult> results)
        {
            _results = new List<CardExpiryReportData>();

            foreach (var item in results)
            {
                _results.Add(new CardExpiryReportData
                {
                    Issuer = String.Format("{0}-{1}", new string[] { item.issuer_name, item.issuer_code }),
                    Branch = String.Format("{0}-{1}", new string[] { item.branch_name, item.branch_code }),
                    PAN = item.card_number,
                    CardReferenceNumber = item.card_reference_number,
                    Expiry = item.card_expiry_date.Value
                });
            }
        }

        public List<CardExpiryReportData> GetResults()
        {
            return _results;
        }
    }

    public class IssueCardReportBO
    {
        private List<IssueCardBO> bo_cards = new List<IssueCardBO>();

        public IssueCardReportBO(List<IssueCard> cards)
        {
            if (cards != null)
            {
                foreach (IssueCard cardObject in cards)
                {
                    IssueCardBO ibo = new IssueCardBO();
                    ibo.AccountNumber = cardObject.AccountNumber;
                    ibo.ReportUser = HttpContext.Current.User.Identity.Name;
                    ibo.BranchCode = cardObject.BranchCode;
                    ibo.CardNumber = cardObject.MaskedCardNumber;
                    ibo.CardReferenceNumber = cardObject.CardReferenceNumber;
                    ibo.CardStatus = cardObject.CardStatus.ToString();
                    ibo.IssuedBy = cardObject.IssuedBy;
                    ibo.IssuedDate = cardObject.IssueDate;
                    ibo.CustomerNames = cardObject.CustomerTitle + " " + cardObject.CustomerFirstName + " " + cardObject.CustomerLastName;

                    bo_cards.Add(ibo);
                }
            }
        }
        public IssueCardReportBO(List<issuedcardsreport_Result> cards)
        {
            if (cards != null)
            {
                foreach (issuedcardsreport_Result cardObject in cards)
                {
                    IssueCardBO ibo = new IssueCardBO();
                    ibo.AccountNumber = cardObject.customeraccountNumber;
                    ibo.ReportUser = HttpContext.Current.User.Identity.Name;
                    ibo.BranchCode = cardObject.BranchCode;
                    ibo.CardNumber = cardObject.card_number;
                    ibo.CardReferenceNumber = cardObject.card_reference_number;
                    ibo.CardStatus = cardObject.CardStatus.ToString();
                    ibo.IssuedBy = cardObject.IssuedBy;
                    ibo.IssuedDate = cardObject.IssuedDate;
                    ibo.ReasonforSpoil = "";
                    ibo.SpoliedUser = "";
                    ibo.CustomerNames = cardObject.CustomerNames;
                    ibo.ApprovedUser = cardObject.APPROVER_USER;
                    ibo.SpoilDate = DateTime.Now;
                    ibo.Scenario = cardObject.Scenario;
                    if (cardObject.fee_Charged != null)
                    {
                        ibo.fee_charged = (decimal)cardObject.fee_Charged;
                    }
                    else
                    {
                        ibo.fee_charged = 0;
                    }
                    bo_cards.Add(ibo);
                }
            }
        }

        public IssueCardReportBO(List<PINReissueReportResult> cards)
        {
            if (cards != null)
            {
                foreach (PINReissueReportResult cardObject in cards)
                {
                    IssueCardBO ibo = new IssueCardBO();
                    ibo.AccountNumber = string.Empty;
                    ibo.ReportUser = HttpContext.Current.User.Identity.Name;
                    ibo.BranchCode = cardObject.branchcode;
                    ibo.CardNumber = cardObject.cardnumber;
                    ibo.CardReferenceNumber = string.Empty;
                    ibo.CardStatus = string.Empty;
                    ibo.IssuedBy = cardObject.IssuedBy;
                    ibo.IssuedDate = cardObject.ReIssuedDate;
                    ibo.ReasonforSpoil = cardObject.Reason;
                    ibo.SpoliedUser = "";
                    ibo.Scenario = cardObject.Reason;
                    ibo.CustomerNames = string.Empty;
                    ibo.ApprovedUser = cardObject.APPROVER_USER;
                    ibo.SpoilDate = cardObject.approveddate.Value;
                   
                  
                    bo_cards.Add(ibo);
                }
            }
        }
        public IssueCardReportBO(List<spolicardsreport_Result> cards, string ReportName)
        {
            if (cards != null)
            {
                foreach (spolicardsreport_Result cardObject in cards)
                {
                    IssueCardBO ibo = new IssueCardBO();
                    ibo.AccountNumber = cardObject.customeraccountNumber;
                    ibo.ReportUser = HttpContext.Current.User.Identity.Name;
                    ibo.BranchCode = cardObject.BranchCode;
                    ibo.CardNumber = cardObject.cardnumber;
                    ibo.CardReferenceNumber = cardObject.cardreferencnumber;
                    ibo.CardStatus = cardObject.CardStatus.ToString();
                    ibo.IssuedBy = cardObject.IssuedBy;
                    ibo.IssuedDate = DateTime.Now;
                    ibo.ReasonforSpoil = cardObject.Reason;
                    ibo.SpoliedUser = cardObject.SpoliedBy;
                    ibo.CustomerNames = cardObject.CustomerNames;
                    ibo.ApprovedUser = "";
                    ibo.SpoilDate = cardObject.IssuedDate;

                    bo_cards.Add(ibo);
                }
            }
        }
        public IssueCardReportBO()
        {
        }

        public List<IssueCardBO> GetCards()
        {
            return bo_cards;
        }

    }

    public class InventoryCardReportBO
    {
        private List<InventoryCardBO> bo_cards = new List<InventoryCardBO>();

        public InventoryCardReportBO(List<IssueCard> cards, DateTime startTime, DateTime endTime)
        {
            if (cards != null)
            {
                foreach (IssueCard cardObject in cards)
                {
                    //do a filter on cards logical to do so on
                    if (cardObject.CardStatus == BranchCardStatus.ISSUED
                        || cardObject.CardStatus == BranchCardStatus.SPOILED)
                    {
                        if (cardObject.IssueDate >= startTime && cardObject.IssueDate <= endTime)
                        {
                            InventoryCardBO ibo = new InventoryCardBO();
                            ibo.ReportUser = HttpContext.Current.User.Identity.Name;
                            ibo.BranchCode = cardObject.BranchCode;
                            ibo.CardNumber = cardObject.MaskedCardNumber;
                            ibo.CardReferenceNumber = cardObject.CardReferenceNumber;
                            ibo.CardStatus = cardObject.CardStatus.ToString();
                            ibo.IssuedBy = cardObject.IssuedBy;
                            ibo.IssuedDate = cardObject.IssueDate;
                            bo_cards.Add(ibo);
                        }
                    }
                    else
                    {
                        InventoryCardBO ibo = new InventoryCardBO();
                        ibo.ReportUser = HttpContext.Current.User.Identity.Name;
                        ibo.BranchCode = cardObject.BranchCode;

                        ibo.CardNumber = cardObject.MaskedCardNumber;
                        ibo.CardReferenceNumber = cardObject.CardReferenceNumber;
                        ibo.CardStatus = cardObject.CardStatus.ToString();
                        ibo.IssuedBy = cardObject.IssuedBy;
                        ibo.IssuedDate = cardObject.IssueDate;
                        bo_cards.Add(ibo);
                    }





                }
            }
        }

        public InventoryCardReportBO(List<invetorysummaryreport_Result> cards)
        {
            if (cards != null)
            {
                foreach (invetorysummaryreport_Result cardObject in cards)
                {

                    InventoryCardBO ibo = new InventoryCardBO();
                    ibo.ReportUser = HttpContext.Current.User.Identity.Name;
                    ibo.BranchCode = cardObject.branch_code;
                    ibo.count = (int)cardObject.CardCount;
                    ibo.statusid = cardObject.statuses_id;
                    ibo.CardStatus = cardObject.statuses_name.ToString();
                    ibo.issuerid = cardObject.issuer_id;
                    ibo.issuername = cardObject.issuer_name;
                    ibo.IssuedBy = null;
                    ibo.branchid = cardObject.branch_id;
                    ibo.IssuedDate = DateTime.Now;
                    bo_cards.Add(ibo);


                }
            }
        }

        public InventoryCardReportBO()
        {
        }

        public List<InventoryCardBO> GetCards()
        {
            return bo_cards;
        }
    }

    public class IssuecardSummaryReportBO
    {
        private List<IssueCardSummaryBO> bo_cards = new List<IssueCardSummaryBO>();



        public IssuecardSummaryReportBO(List<issuecardsummaryreport_Result> cards)
        {
            if (cards != null)
            {
                foreach (issuecardsummaryreport_Result cardObject in cards)
                {

                    IssueCardSummaryBO ibo = new IssueCardSummaryBO();
                    ibo.ReportUser = HttpContext.Current.User.Identity.Name;
                    ibo.BranchCode = cardObject.branch_code;
                    ibo.branchid = cardObject.branch_id;
                    ibo.id = cardObject.card_issue_reason_id;
                    ibo.Count = (int)cardObject.CardCount;
                    ibo.ReasonforIssue = cardObject.card_issuer_reason_name;
                    ibo.issuerid = cardObject.issuer_id;
                    ibo.issuername = cardObject.issuer_name;
                    ibo.ReasonforSpoil = "";

                    bo_cards.Add(ibo);


                }
            }

        }
        // same class using for issuecardsummarycard and spoil card summary
        public IssuecardSummaryReportBO(List<Spoilcardsummaryreport_Result> cards, string ReportName)
        {
            if (cards != null)
            {
                foreach (Spoilcardsummaryreport_Result cardObject in cards)
                {

                    IssueCardSummaryBO ibo = new IssueCardSummaryBO();
                    ibo.ReportUser = HttpContext.Current.User.Identity.Name;
                    ibo.BranchCode = cardObject.branch_code;
                    ibo.id = cardObject.branch_card_code_id;
                    ibo.branchid = cardObject.branch_id;
                    ibo.Count = (int)cardObject.CardCount;
                    ibo.ReasonforIssue = "";
                    ibo.ReasonforSpoil = cardObject.Spoil_reason_name;
                    ibo.issuerid = cardObject.issuer_id;
                    ibo.issuername = cardObject.issuer_name;

                    bo_cards.Add(ibo);


                }
            }

        }

        public IssuecardSummaryReportBO()
        {
        }

        public List<IssueCardSummaryBO> GetCards()
        {
            return bo_cards;
        }
    }

    public class IssuecardSummaryReportBO2
    {
        private List<IssueCardSummaryBO> bo_cards = new List<IssueCardSummaryBO>();



        public IssuecardSummaryReportBO2(List<issuecardsummaryreport_Result> cards)
        {
            if (cards != null)
            {
                foreach (issuecardsummaryreport_Result cardObject in cards)
                {

                    IssueCardSummaryBO ibo = new IssueCardSummaryBO();
                    ibo.ReportUser = HttpContext.Current.User.Identity.Name;
                    ibo.BranchCode = cardObject.branch_code;
                    ibo.branchid = cardObject.branch_id;
                    ibo.id = cardObject.card_issue_reason_id;
                    ibo.Count = (int)cardObject.CardCount;
                    ibo.ReasonforIssue = cardObject.card_issuer_reason_name;
                    ibo.issuerid = cardObject.issuer_id;
                    ibo.issuername = cardObject.issuer_name;
                    ibo.ReasonforSpoil = "";

                    bo_cards.Add(ibo);


                }
            }

        }
        // same class using for issuecardsummarycard and spoil card summary
        public IssuecardSummaryReportBO2(List<Spoilcardsummaryreport_Result> cards, string ReportName)
        {
            if (cards != null)
            {
                foreach (Spoilcardsummaryreport_Result cardObject in cards)
                {

                    IssueCardSummaryBO ibo = new IssueCardSummaryBO();
                    ibo.ReportUser = HttpContext.Current.User.Identity.Name;
                    ibo.BranchCode = cardObject.branch_code;
                    ibo.id = cardObject.branch_card_code_id;
                    ibo.branchid = cardObject.branch_id;
                    ibo.Count = (int)cardObject.CardCount;
                    ibo.ReasonforIssue = "";
                    ibo.ReasonforSpoil = cardObject.Spoil_reason_name;
                    ibo.issuerid = cardObject.issuer_id;
                    ibo.issuername = cardObject.issuer_name;

                    bo_cards.Add(ibo);


                }
            }

        }

        public IssuecardSummaryReportBO2()
        {
        }

        public List<IssueCardSummaryBO> GetCards()
        {
            return bo_cards;
        }
    }


    public class BurnRateReportBO
    {
        private List<BurnRateRptData> bo_cards = new List<BurnRateRptData>();
        public BurnRateReportBO(List<burnrate_report_Result> list)
        {
            if (list != null)
            {
                foreach (burnrate_report_Result item in list)
                {

                    BurnRateRptData ibo = new BurnRateRptData();
                    ibo.BranchCode = item.branch_code;
                    ibo.Branchid = item.branch_id.ToString();
                    ibo.d180 = (int)item.C180_d;
                    ibo.d90 = (int)item.C90_d;
                    ibo.week1 = (int)item.Week_1;
                    ibo.week2 = (int)item.Week_2;
                    ibo.week3 = (int)item.Week_3;
                    ibo.week4 = (int)item.Week_4;
                    if (item.Current_Card_Stock != null)
                    {
                        ibo.currentstock = (int)item.Current_Card_Stock;
                    }
                    else
                    {
                        ibo.currentstock = 0;
                    }
                    ibo.weeksremaining = 0;
                    bo_cards.Add(ibo);

                }
            }
        }
        // same class using for issuecardsummarycard and spoil card summary    
        public BurnRateReportBO()
        {
        }

        public List<BurnRateRptData> GetCards()
        {
            return bo_cards;
        }
    }

    public class BranchCardStockReportBo
    {
        private List<BranchCardStockRptData> bo_cards = new List<BranchCardStockRptData>();
        public BranchCardStockReportBo(List<branchcardstock_report_Result> list)
        {
            if (list != null)
            {
                foreach (branchcardstock_report_Result item in list)
                {

                    BranchCardStockRptData ibo = new BranchCardStockRptData();
                    ibo.BranchCode = item.branch_code;
                    ibo.Branchid = item.branch_id.ToString();
                    ibo.PAN = item.card_number;
                    ibo.CardReferenceNumber = item.card_request_reference;
                    if (item.card_expiry_date != null)
                    {
                        ibo.ExpiryDate = item.card_expiry_date.ToString();
                    }
                    else
                    {
                        ibo.ExpiryDate = string.Empty;
                    }
                    if (item.card_production_date != null)
                    {
                        ibo.CardProductionDate = item.card_production_date.Value.ToShortDateString();
                    }
                    else
                    {
                        ibo.CardProductionDate = string.Empty;
                    }
                    bo_cards.Add(ibo);

                }
            }
        }

        public BranchCardStockReportBo()
        {
        }

        public List<BranchCardStockRptData> GetCards()
        {
            return bo_cards;
        }
    }
    public class FeeRevnueReportBo
    {
        private List<FeeRevenuRptData> bo_cards = new List<FeeRevenuRptData>();
        public FeeRevnueReportBo(List<feerevenue_report_Result> list)
        {
            if (list != null)
            {
                foreach (feerevenue_report_Result item in list)
                {


                    FeeRevenuRptData ibo = new FeeRevenuRptData();
                    ibo.BranchCode = item.branch_code;
                    ibo.Branchid = item.branch_id.ToString();
                    if (item.zero_no_fee != null)
                    {
                        ibo.zero_no_fee = (int)item.zero_no_fee;

                    }
                    else
                    {
                        ibo.zero_no_fee = 0;
                    }
                    if (item.amended_fee != null)
                    {
                        ibo.amended_fee = (int)item.amended_fee;
                    }
                    else
                    {
                        ibo.amended_fee = 0;
                    }
                    if (item.full_fee != null)
                    {
                        ibo.full_fee = (int)item.full_fee;
                    }
                    else
                    {
                        ibo.full_fee = 0;
                    }
                    bo_cards.Add(ibo);

                }
            }
        }

        public FeeRevnueReportBo()
        {
        }

        public List<FeeRevenuRptData> GetCards()
        {
            return bo_cards;
        }
    }
    public class FeeReVenueReportingLables
    {
        public FeeReVenueReportingLables()
        {

        }
        private List<FeeRevnueRptLables> bo_cards = new List<FeeRevnueRptLables>();
        public FeeReVenueReportingLables(FeeRevnueRptLables labelvalues)
        {
            bo_cards.Add(labelvalues);

        }
        public List<FeeRevnueRptLables> GetReportLabels()
        {
            return bo_cards;
        }
    }
    public class BranchCardStockReportingLables
    {
        public BranchCardStockReportingLables()
        {

        }
        private List<BCSRptLables> bo_cards = new List<BCSRptLables>();
        public BranchCardStockReportingLables(BCSRptLables labelvalues)
        {
            bo_cards.Add(labelvalues);

        }
        public List<BCSRptLables> GetReportLabels()
        {
            return bo_cards;
        }
    }
    public class BurnRateReportingLables
    {
        public BurnRateReportingLables()
        {

        }
        private List<BRRptLables> bo_cards = new List<BRRptLables>();
        public BurnRateReportingLables(BRRptLables labelvalues)
        {
            bo_cards.Add(labelvalues);

        }
        public List<BRRptLables> GetReportLabels()
        {
            return bo_cards;
        }
    }

    public class AuditUserGroupReportBO
    {
        private List<AuditRptData> _results = new List<AuditRptData>();

        public AuditUserGroupReportBO()
        {

        }

        public AuditUserGroupReportBO(List<auditreport_usergroup_Result> results)
        {
            _results = new List<AuditRptData>();

            foreach (var item in results)
            {
                _results.Add(new AuditRptData
                {
                    user_group_name = item.user_group_name,
                    user_role = item.user_role,
                    username = item.username,
                    can_delete = item.can_delete,
                    can_read = item.can_read,
                    can_update = item.can_update,
                    issuer_name = item.issuer_name,
                    branch_name = item.branch_name,
                    all_branch_access = item.all_branch_access,
                    mask_report_pan = item.mask_report_pan,
                    mask_screen_pan=item.mask_screen_pan,
                    can_create = item.can_create
                });
            }
        }

        public List<AuditRptData> GetResults()
        {
            return _results;
        }
    }

    public class AuditUserGroupReportLables
    {
        public AuditUserGroupReportLables()
        {

        }
        private List<AuditRptLabels> bo_cards = new List<AuditRptLabels>();
        public AuditUserGroupReportLables(AuditRptLabels labelvalues)
        {
            bo_cards.Add(labelvalues);

        }
        public List<AuditRptLabels> GetReportLabels()
        {
            return bo_cards;
        }
    }

}