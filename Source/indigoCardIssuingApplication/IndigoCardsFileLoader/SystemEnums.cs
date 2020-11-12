namespace IndigoFileLoader
{
    public enum FileExtensionType
    {
        CSV,
        XML,
        TXT
    }

    public enum FileType
    {
        PIN_MAILER,
        CARD_IMPORT,
        UNKNOWN
    }

    public enum FileStatus
    {
        READ,
        VALID_CARDS,
        PROCESSED,
        DUPLICATE_FILE,
        LOAD_FAIL,
        FILE_CORRUPT,
        PARTIAL_LOAD,
        INVALID_FORMAT,
        INVALID_NAME,
        ISSUER_NOT_FOUND,
        INVALID_ISSUER_LICENSE,
        NO_ACTIVE_BRANCH_FOUND,
        DUPLICATE_CARDS_IN_FILE,
        DUPLICATE_CARDS_IN_DATABASE,
        FILE_DECRYPTION_FAILED,
        UNLICENSED_BIN,
        NO_PRODUCT_FOUND_FOR_CARD,
        UNLICENSED_ISSUER,
        BRANCH_PRODUCT_NOT_FOUND,
        CARD_FILE_INFO_READ_ERROR
    }

    public enum IssuerStatus
    {
        ACTIVE,
        INACTIVE,
        SUSPENDED,
        DELETED,
        UNKNOWN
    }    
}