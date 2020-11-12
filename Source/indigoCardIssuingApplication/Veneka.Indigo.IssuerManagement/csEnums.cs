namespace Veneka.Indigo.IssuerManagement
{
    public enum InstitutionStatus
    {
        ACTIVE,
        INACTIVE,
        DELETED,
    }

    public enum BranchStatus
    {
        ACTIVE,
        INACTIVE,
        DELETED
    }
    public enum HybridRequestStatuses
    {
        CREATED,
        APPROVED,
        ASSIGN_TO_BATCH
    }
    public enum PrintJobStatuses
    {
        CREATED,
        SENT_TO_PRINTER,
        PRINTED,
        FAILED
    }
    public enum PrintBatchStatuses
    {

    CREATED,
    APPROVED,
    SENT_TO_PRINT,
    PRINT_SUCESSFUL,
    PRINT_FAILED,
    REJECT,
    PROCESSED_IN_CMS,
    CMS_ERROR,
        SPOIL,
        PARTIAL_PRINT_SUCESSFUL
    }

    public enum BranchTypes
    {
        CARD_CENTER,
        MAIN,
        SATELLITE
    }
    //public enum InterfaceTypes
    //{
    //    CORE_BANKING_SYSTEM = 0,
    //    CARD_MANAGEMENT_SYSTEM = 1,
    //    HARDWARE_SECURITY_MODULE = 2,
    //    CARD_PRODUCTION = 3,
    //    FILE_LOADER = 4
    //}
}