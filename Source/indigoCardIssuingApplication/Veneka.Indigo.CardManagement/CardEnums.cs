namespace Veneka.Indigo.CardManagement
{
    //public enum DBResponseMessage
    //{
    //    //RL -- this needs to move out of here?
    //    SUCCESS, //0
    //    INCORRECT_STATUS, //1
    //    CARD_ALREADY_ISSUED, //2
    //    INCORRECT_BRANCH, //3
    //    NO_RECORDS_RETURNED, //4
    //    SPROC_ERROR, //97
    //    SYSTEM_ERROR, //98
    //    FAILURE //99
    //}

    public enum DistCardStatus
    {
        ALLOCATED_TO_BRANCH = 0,
        //AVAILABLE_FOR_ISSUE,
        RECEIVED_AT_BRANCH = 2,
        //ALLOCATED_TO_CUST,
        //CARD_PRINTED,
        //PIN_CAPTURED,
        //ISSUED,
        REJECTED = 7,
        CANCELLED = 8,
        INVALID = 9,
        //LINKED_TO_ACCOUNT,
        //SPOILED
    }

    public enum BranchCardStatus
    {
        CHECKED_IN,
        AVAILABLE_FOR_ISSUE,
        ALLOCATED_TO_CUST,
        APPROVED_FOR_ISSUE,
        CARD_PRINTED,
        PIN_CAPTURED,
        ISSUED,
        SPOILED,
        PRINT_ERROR,
        CMS_ERROR,
        REQUESTED,
        MAKERCHECKER_REJECT,
        CARD_REQUEST_DELETED,
        REDISTRIBUTED,
        PIN_AUTHORISED,
        CMS_REUPLOAD,
        ASSIGN_TO_REQUEST,
        CREDIT_READY_TO_ANALYZE = 17,
        CREDIT_READY_TO_APPROVE = 18,
        CREDIT_CONTRACT_CAPTURE = 19
    }

    public enum BatchType
    {
        LOAD_BATCH,
        DISTRIBUTION_BATCH
    }

    public enum BatchStatus
    {
        LOAD_BATCH,
        DISTRIBUTION_BATCH
    }

    public enum DistributionBatchStatus
    {
        CREATED = 0,
        APPROVED = 1,
        DISPATCHED = 2,
        RECEIVED_AT_BRANCH = 3,
        REJECTED_AT_BRANCH = 4,
        REJECT_AND_REISSUE = 5,
        REJECT_AND_CANCEL = 6,
        INVALID = 7,
        REJECTED = 8,
        APPROVED_FOR_PRODUCTION = 9,
        SENT_TO_CMS = 10,
        PROCESSED_IN_CMS = 11,
        AT_CARD_PRODUCTION = 12,
        CARDS_PRODUCED = 13,
        RECEIVED_AT_CARD_CENTER = 14,
        FAILED_IN_CMS = 15,
        REJECTED_AT_CARD_CENTER = 16,
        SENT_TO_PRINTER = 17,
        PIN_PRINTED = 18
    }

    public enum LoadBatchStatus
    {
        LOADED,
        APPROVED,
        REJECTED,
        INVALID
        //    CONFIRMED //RL --- what is this used for?
    }
    public enum ThreedBatchStatus
    {
        CREATED,
        REGISTERED,
        FAILED,
        RECREATED

    }
    public enum LoadCardStatus
    {
        LOADED,
        AVAILABLE,
        ALLOCATED,
        REJECTED,
        CANCELLED,
        INVALID
    }

    public enum ReasonForIssue
    {
        NEW,
        LOST_CARD,
        STOLLEN,
        EXPIRED
    }
}