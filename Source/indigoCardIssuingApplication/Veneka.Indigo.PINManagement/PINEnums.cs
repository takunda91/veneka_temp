namespace Veneka.Indigo.PINManagement
{
    public enum PINBatchStatus
    {
        LOADED,
        APPROVED,
        REJECTED,
        PRINT_STARTED,
        PRINT_IN_PROGRESS,
        PRINTED,
        REPRINT_REQUESTED,
        REPRINT_APPROVED,
        REPRINT_REJECTED,
        INVALID
    }

    //comment

    public enum PINMailerStatus
    {
        NOT_PRINTED,
        PRINTED,
        RE_PRINT_REQUESTED,
        RE_PRINT_APPROVED,
        RE_PRINTED,
        REJECTED,
        INVALID
    }

    public enum BatchType
    {
        PIN_BATCH
    }
}