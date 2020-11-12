using System;
using Veneka.Indigo.Common;

namespace Veneka.Indigo.CardManagement
{
    public sealed class csGeneral
    {
        public static DBResponseMessage ValidateDBResponse(string strResponse)
        {
            int intResponse = 99;
            Int32.TryParse(strResponse, out intResponse);

            switch (intResponse)
            {
                case 0:
                    return DBResponseMessage.SUCCESS;
                case 1:
                    return DBResponseMessage.INCORRECT_STATUS;
                case 2:
                    return DBResponseMessage.CARD_ALREADY_ISSUED;
                case 3:
                    return DBResponseMessage.INCORRECT_BRANCH;
                case 4:
                    return DBResponseMessage.NO_RECORDS_RETURNED;
                case 97:
                    return DBResponseMessage.SPROC_ERROR;
                case 98:
                    return DBResponseMessage.SYSTEM_ERROR;
                case 99:
                    return DBResponseMessage.FAILURE;
                default:
                    return DBResponseMessage.FAILURE;
            }
        }

        public static AccountType GetAccountType(string accountType)
        {
            AccountType accType;
            switch (accountType)
            {
                case "SAVINGS":
                    accType = AccountType.SAVINGS;
                    break;
                case "CHEQUE":
                    accType = AccountType.CHEQUE;
                    break;
                case "CREDIT":
                    accType = AccountType.CREDIT;
                    break;
                default:
                    accType = AccountType.SAVINGS;
                    break;
            }

            return accType;
        }

        public static ReasonForIssue GetReasonForIssue(string reasonForIssue)
        {
            ReasonForIssue issueReason;
            switch (reasonForIssue)
            {
                case "NEW":
                    issueReason = ReasonForIssue.NEW;
                    break;
                case "LOST_CARD":
                    issueReason = ReasonForIssue.LOST_CARD;
                    break;
                case "STOLLEN":
                    issueReason = ReasonForIssue.STOLLEN;
                    break;
                default:
                    issueReason = ReasonForIssue.EXPIRED;
                    break;
            }
            return issueReason;
        }

        #region BATCH STATUSES

        public static LoadBatchStatus GetLoadBatchStatus(string batchStatus)
        {
            switch (batchStatus)
            {
                case "LOADED":
                    return LoadBatchStatus.LOADED;

                case "APPROVED":
                    return LoadBatchStatus.APPROVED;

                case "REJECTED":
                    return LoadBatchStatus.REJECTED;

                default:
                    return LoadBatchStatus.INVALID;
            }
        }

        public static DistributionBatchStatus GetDistBatchStatus(string batchStatus)
        {
            switch (batchStatus)
            {
                case "CREATED":
                    return DistributionBatchStatus.CREATED;

                case "APPROVED":
                    return DistributionBatchStatus.APPROVED;

                case "DISPATCHED":
                    return DistributionBatchStatus.DISPATCHED;

                case "RECEIVED_AT_BRANCH":
                    return DistributionBatchStatus.RECEIVED_AT_BRANCH;

                case "REJECTED_AT_BRANCH":
                    return DistributionBatchStatus.REJECTED_AT_BRANCH;

                case "REJECT_AND_REISSUE":
                    return DistributionBatchStatus.REJECT_AND_REISSUE;

                case "REJECT_AND_CANCEL":
                    return DistributionBatchStatus.REJECT_AND_CANCEL;

                default:
                    return DistributionBatchStatus.INVALID;
            }
        }

        #endregion

        #region CARD STATUSES

        internal static LoadCardStatus GetLoadCardStatus(string cardStatus)
        {
            switch (cardStatus)
            {
                case "LOADED":
                    return LoadCardStatus.LOADED;

                case "AVAILABLE":
                    return LoadCardStatus.AVAILABLE;

                case "ALLOCATED":
                    return LoadCardStatus.ALLOCATED;

                case "REJECTED":
                    return LoadCardStatus.REJECTED;

                case "CANCELLED":
                    return LoadCardStatus.CANCELLED;

                default:
                    return LoadCardStatus.INVALID;
            }
        }

        public static DistCardStatus GetIssueCardStatus(string issueCardStatus)
        {
            return DistCardStatus.ALLOCATED_TO_BRANCH;
            //switch (issueCardStatus.Trim())
            //{
            //    case "ALLOCATED_TO_BRANCH":
            //        return IssueCardStatus.ALLOCATED_TO_BRANCH;
            //    case "AVAILABLE_FOR_ISSUE":
            //        return IssueCardStatus.AVAILABLE_FOR_ISSUE;
            //    case "ALLOCATED_TO_CUST":
            //        return IssueCardStatus.ALLOCATED_TO_CUST;
            //    case "RECEIVED_AT_BRANCH":
            //        return IssueCardStatus.RECEIVED_AT_BRANCH;
            //    case "CARD_PRINTED":
            //        return IssueCardStatus.CARD_PRINTED;
            //    //case "PIN_CAPTURED":
            //    //    return IssueCardStatus.PIN_CAPTURED;
            //    case "ISSUED":
            //        return IssueCardStatus.ISSUED;
            //    case "REJECTED":
            //        return IssueCardStatus.REJECTED;
            //    case "CANCELLED":
            //        return IssueCardStatus.CANCELLED;
            //    //case "LINKED_TO_ACCOUNT":
            //    //    return IssueCardStatus.LINKED_TO_ACCOUNT;
            //    case "SPOILED":
            //        return IssueCardStatus.SPOILED;
            //    case "INVALID":
            //        return IssueCardStatus.INVALID;
            //    default:
            //        return IssueCardStatus.INVALID;
            //}
        }

        #endregion
    }
}