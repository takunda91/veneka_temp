using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using indigoCardIssuingWeb.Old_App_Code.CCO.objects;

namespace indigoCardIssuingWeb.App_Code
{
    public class IssueCardBO
    {
        public string ReportUser { get; set; }
        public string CardNumber { get; set; }
        public string CardReferenceNumber { get; set; }
        public string CardStatus { get; set; }
        public string BranchCode { get; set; }
        public string CustomerNames { get; set; }
        public string ApprovedUser { get; set; }
        public string SpoliedUser { get; set; }
        public string ReasonforSpoil { get; set; }
        public string AccountNumber { get; set; }
        public string IssuedBy { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime SpoilDate { get; set; }
        public string Scenario { get; set; }
        public decimal fee_charged { get; set; }
    }
    public class IssueCardSummaryBO
    {
        public string ReportUser { get; set; }
        public int branchid { get; set; }
        public int Count { get; set; }
        public string BranchCode { get; set; }       
        public string ReasonforIssue { get; set; }
        public string ReasonforSpoil { get; set; }
        public int id { get; set; }
        public int issuerid { get; set; }
        public string issuername { get; set; }

    }
    
    public class IssueCardLabels
    {
        public string Heading { get; set; }
        public string BranchCode { get; set; }
        public string IssuedBy { get; set; }
        public string CustomerNames { get; set; }
        public string AccountNo { get; set; }
        public string CardNumberLabel { get; set; }
        public string CardReferenceNumberLabel { get; set; }
        public string ApproverName { get; set; }
        public string SpoliedUser { get; set; }
        public string Reason { get; set; }
        public string IssuedDate { get; set; }
        public string EndofReport { get; set; }
        public string GeneratedBy { get; set; }
        public string ExecutionTime { get; set; }
        public string bottomline { get; set; }
        public string Total { get; set; }
        public string Issuer { get; set; }
        public string Branch { get; set; }
        public string Operator { get; set; }
        public string IssuerName { get; set; }
        public string BranchName { get; set; }
        public string OperatorName { get; set; }
        public string DateRangetext { get; set; }
        public string ReportedBy { get; set; }
        public string SpoiledDate { get; set; }
        public string Scenario { get; set; }
        public string feecharged { get; set; }
    }
    public class InvReportLabels
    {
        public string BranchCode { get; set; }
        public string Total { get; set; }
        public string GeneratedBy { get; set; }
        public string ReportEndDate { get; set; }
        public string Executiontime { get; set; }

        public string Heading { get; set; }
        public string footer { get; set; }
        public string bottomline { get; set; }
        public string Issuer { get; set; }
        public string Branch { get; set; }
        public string IssuerName { get; set; }
        public string BranchName { get; set; }
        public string ReportedBy { get; set; }
    }
    public class InventoryCardBO
    {
        public string ReportUser { get; set; }
        public string CardNumber { get; set; }
        public string CardReferenceNumber { get; set; }
        public string CardStatus { get; set; }
        public string BranchCode { get; set; }
        public string IssuedBy { get; set; }
        public int count { get; set; }
        public int statusid { get; set; }
        public int branchid { get; set; }
        public int issuerid { get; set; }
        public string issuername { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ReportStartDate { get; set; }
        public DateTime ReportEndDate { get; set; }

     
    }

    public class BranchOrderReportData
    {
        public string Issuer { get; set; }
        public string Branch { get; set; }
        public string CustomerFullName { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerAccount { get; set; }
        public string NameOnCard { get; set; }
        public string BatchReference { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class CardProductionReportData
    {
        public string Issuer { get; set; }
        public string Branch { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerAccount { get; set; }
        public string NameOnCard { get; set; }
        public string PAN { get; set; }
        public string CardReferenceNumber { get; set; }
        public string BatchReference { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class CardDispatchReportData
    {
        public string Issuer { get; set; }
        public string Branch { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerAccount { get; set; }
        public string NameOnCard { get; set; }
        public string PAN { get; set; }
        public string CardReferenceNumber { get; set; }
        public string BatchReference { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class PinMailerReportData
    {
        public string Issuer { get; set; }
        public string CustomerName { get; set; }
        public string Branch { get; set; }
        public string CustomerAccount { get; set; }
        public string BatchReference { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class CardExpiryReportData
    {
        public string Issuer { get; set; }
        public string Branch { get; set; }
        public string PAN { get; set; }
        public string CardReferenceNumber { get; set; }
        public DateTime Expiry { get; set; }
    }
    public class BRRptLables
    {
        public string BranchCode { get; set; }
        public string Total { get; set; }
        public string GeneratedBy { get; set; }
        public string ReportEndDate { get; set; }
        public string Executiontime { get; set; }

        public string ReportedBy { get; set; }
        public string Heading { get; set; }
        public string footer { get; set; }
        public string bottomline { get; set; }
        public string Issuer { get; set; }
        public string Branch { get; set; }
        public string IssuerName { get; set; }
        public string BranchName { get; set; }
        public string d180 { get; set; }
        public string d90 { get; set; }
        public string Week1 { get; set; }
        public string Week2 { get; set; }
        public string Week3 { get; set; }
        public string Week4 { get; set; }
        public string CurrentStock { get; set; }
        public string WeeksRemaining { get; set; }
        public string EndofReport { get; set; }
        public string ExecutionTime { get; set; }
    }
    public class BurnRateRptData
    {
        public string Branchid { get; set; }
        public string BranchCode { get; set; }
        public int d180 { get; set; }
        public int d90 { get; set; }
        public int week1 { get; set; }
        public int week2 { get; set; }
        public int week3 { get; set; }
        public int week4 { get; set; }
        public int weeksremaining { get; set; }
        public int currentstock { get; set; }
    }
    public class BCSRptLables
    {
        public string BranchCode { get; set; }
        public string Total { get; set; }
        public string GeneratedBy { get; set; }
        public string ReportEndDate { get; set; }
        public string Executiontime { get; set; }

        public string ReportedBy { get; set; }
        public string Heading { get; set; }
        public string footer { get; set; }
        public string bottomline { get; set; }
        public string Issuer { get; set; }
        public string Branch { get; set; }
        public string IssuerName { get; set; }
        public string BranchName { get; set; }
        public string CardNumberLabel { get; set; }
        public string CardReferenceNumberLabel { get; set; }
        public string Expirydate { get; set; }
        public string CardProductionDate { get; set; }
        public string EndofReport { get; set; }
        public string ExecutionTime { get; set; }
    }
    public class BranchCardStockRptData
    {

        public string Branchid { get; set; }
        public string BranchCode { get; set; }
        public string PAN { get; set; }
        public string CardReferenceNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CardProductionDate { get; set; }
    }

    public class FeeRevenuRptData
    {

        public string Branchid { get; set; }
        public string BranchCode { get; set; }
        public int zero_no_fee { get; set; }
        public int amended_fee { get; set; }
        public int full_fee { get; set; }
        public decimal fee_charged { get; set; }
    }
    public class FeeRevnueRptLables
    {
        public string BranchCode { get; set; }
        public string Total { get; set; }
        public string GeneratedBy { get; set; }
        public string ReportEndDate { get; set; }
        public string Executiontime { get; set; }

        public string ReportedBy { get; set; }
        public string Heading { get; set; }
        public string footer { get; set; }
        public string bottomline { get; set; }
        public string Issuer { get; set; }
        public string Branch { get; set; }
        public string IssuerName { get; set; }
        public string BranchName { get; set; }
        public string zero_no_fee { get; set; }
        public string amended_fee { get; set; }
        public string full_fee { get; set; }
        public string fee_charged { get; set; }
        public string DateRangetext { get; set; }
        public string EndofReport { get; set; }
        public string ExecutionTime { get; set; }
    }

    public class AuditRptData
    {

        public string user_group_name { get; set; }
        public string issuer_name { get; set; }
        public string branch_name { get; set; }
        public string user_role { get; set; }
        public string username { get; set; }
        public bool mask_report_pan { get; set; }
        public bool can_read { get; set; }
        public bool can_update { get; set; }
        public bool can_delete { get; set; }
        public bool all_branch_access { get; set; }
        public bool can_create { get; set; }
        public bool mask_screen_pan { get; set; }
    }

    public class AuditRptLabels
    {
       
        public string Total { get; set; }
        public string GeneratedBy { get; set; }
        public string ReportEndDate { get; set; }
        public string Executiontime { get; set; }

        public string ReportedBy { get; set; }
        public string Heading { get; set; }
        public string footer { get; set; }
        public string bottomline { get; set; }
        public string Issuer { get; set; }
        public string Branch { get; set; }
        public string UserGroup { get; set; }
        public string UserRole { get; set; }
        public string UserName { get; set; }
        public string mask_report_pan { get; set; }
        public string can_read { get; set; }
        public string can_update { get; set; }
        public string can_delete { get; set; }
        public string can_create { get; set; }
        public string mask_screen_pan { get; set; }
        public string all_branch_access { get; set; }
        public string EndofReport { get; set; }
        public string ExecutionTime { get; set; }
        public string issuername { get; set; }
        public string usergroupname { get; set; }
        public string userrolename { get; set; }

    }


}