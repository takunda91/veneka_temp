using System;
using Veneka.Indigo.Common;

namespace Veneka.Indigo.PINManagement
{
    public sealed class PINGeneral
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

        public static PINBatchStatus GetPINBatchStatus(string batchStatus)
        {
            switch (batchStatus)
            {
                case "LOADED":
                    return PINBatchStatus.LOADED;
                case "APPROVED":
                    return PINBatchStatus.APPROVED;
                case "REJECTED":
                    return PINBatchStatus.REJECTED;
                case "PRINT_STARTED":
                    return PINBatchStatus.PRINT_STARTED;
                case "PRINT_IN_PROGRESS":
                    return PINBatchStatus.PRINT_IN_PROGRESS;
                case "PRINTED":
                    return PINBatchStatus.PRINTED;
                case "REPRINT_APPROVED":
                    return PINBatchStatus.REPRINT_APPROVED;
                case "REPRINT_REJECTED":
                    return PINBatchStatus.REPRINT_REJECTED;
                case "REPRINT_REQUESTED":
                    return PINBatchStatus.REPRINT_REQUESTED;
                default:
                    return PINBatchStatus.INVALID;
            }
        }

        public static PINMailerStatus GetPINMailerStatus(string mailerStatus)
        {
            switch (mailerStatus)
            {
                case "NOT_PRINTED":
                    return PINMailerStatus.NOT_PRINTED;
                case "PRINTED":
                    return PINMailerStatus.PRINTED;
                case "RE_PRINT_REQUESTED":
                    return PINMailerStatus.RE_PRINT_REQUESTED;
                case "RE_PRINT_APPROVED":
                    return PINMailerStatus.RE_PRINT_APPROVED;
                case "RE_PRINTED":
                    return PINMailerStatus.RE_PRINTED;
                case "REJECTED":
                    return PINMailerStatus.REJECTED;
                default:
                    return PINMailerStatus.INVALID;
            }
        }

        /*
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

        public static IssueCardStatus GetIssueCardStatus(string issueCardStatus)
        {
            switch (issueCardStatus)
            {
                case "ALLOCATED_TO_BRANCH":
                    return IssueCardStatus.ALLOCATED_TO_BRANCH;

                case "AVAILABLE_FOR_ISSUE":
                    return IssueCardStatus.AVAILABLE_FOR_ISSUE;

                case "ISSUED":
                    return IssueCardStatus.ISSUED;

                case "REJECTED":
                    return IssueCardStatus.REJECTED;

                case "CANCELLED":
                    return IssueCardStatus.CANCELLED;

                default:
                    return IssueCardStatus.INVALID;
            }
        }

        #endregion

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
                    issueReason= ReasonForIssue.EXPIRED;
                    break;
            }
            return issueReason;
      }
        */
    }
}