namespace Veneka.Indigo.Common
{
    //public enum IndigoLanguages
    //{
    //    English,
    //    French,
    //    Portuguese
    //}

    public enum InterfaceTypes
    {
        CORE_BANKING_SYSTEM = 0,
        CARD_MANAGEMENT_SYSTEM = 1,
        HARDWARE_SECURITY_MODULE = 2,
        CARD_PRODUCTION = 3,
        FILE_LOADER = 4,
        FEE_SCHEME = 5,
        EXTERNAL_AUTHENTICATION = 6,
        NOTIFICATIONS = 7,
        FILE_EXPORT = 8,
        REMOTE_CMS = 9,
        MULTIFACTOR = 10,
        THREED_SECURE = 11,
        PREPAID_FUNDS_LOAD = 12
    }

    public enum InterfaceArea
    {
        PRODUCTION = 0,
        ISSUING = 1
    }

    public enum ConnectionProtocol
    {
        HTTP,
        HTTPS,
        TCPIP
    }

    public enum AuthenticationTypes
    {
        None, //e.g. FlexCube
        Basic //e.g. Tieto CMS
    }

    public enum DBResponseMessage
    {
        SUCCESS, //0
        INCORRECT_STATUS, //1
        CARD_ALREADY_ISSUED, //2
        INCORRECT_BRANCH, //3
        NO_RECORDS_RETURNED, //4
        DUPLICATE_RECORD = 69,
        INSUFFICIENT_AVAILABLE_CARDS = 70,
        RECORD_NOT_FOUND = 71,
        SPROC_ERROR = 97, //97
        SYSTEM_ERROR = 98, //98
        FAILURE = 99 //99
    }

    public enum IssuanceModes
    {
        Account_New_Customer,
        Account_Existing_Customer,
        Renew,
        Replace, Supplement
    }

    public enum CustomerType
    {
        PRIVATE,
        CORPORATE
    }

    public enum CustomerResidency
    {
        RESIDENT,
        NONRESIDENT
    }

    public enum Currency
    {
        GHS,
        USD,
        GBP,
        EUR,
        XOF,
        BIF,
        CDF,
        CVE,
        GMD,
        GNF,
        KES,
        LRD,
        MWK,
        NGN,
        RWF,
        SLL,
        SSP,
        STD,
        TZS,
        UGX,
        XAF,
        ZMW
    }

    public enum AccountType
    {
        CURRENT,
        SAVINGS,
        CHEQUE,
        CREDIT,
        UNIVERSAL,
        INVESTMENT
    }

    public enum FileEncryptionType
    {
        NONE = 1,
        PGP = 2
    }

    /// <summary>
    /// These codes are passed between backend and front end, 
    /// on the front end the code will be decoded to user friendly message in their chosen language
    /// </summary>
    public enum SystemResponseCode
    {
        SUCCESS = 0,
        GENERAL_FAILURE = 1,
        CONNECTION_ERROR = 2,
        CONFIGURATION_ERROR = 3,
        PARAMETER_ERROR = 4,
        LOGIN_SUCCESS = 10,
        LOGIN_FAIL_CREDENTIALS = 11,
        LOGIN_FAIL_LOCKEDOUT = 12,
        LOGIN_FAILED_ACCOUNT_INACTIVE = 13,
        SESSIONKEY_CREATE_FAIL = 15,
        SESSIONKEY_AUTHORISATION_FAIL = 16,
        SESSIONKEY_MULTI_LOGIN_FAIL = 17,
        ENDPOINT_NOT_FOUND = 20,
        CREATE_SUCCESS = 30,
        CREATE_FAIL = 31,
        UPDATE_SUCCESS = 32,
        UDPATE_FAIL = 33,
        DELETE_SUCCESS = 34,
        DELETE_FAIL = 35,
        LDAP_SERVER_DOWN = 50,
        LDAP_GENERAL_FAILURE = 51,
        LDAP_LOOKUP_FAILED = 52,
        DUPLICATE_USERNAME = 69,
        USER_PASSWORD_VALIDATION_FAILED = 70,
        USER_PASSWORD_MATCHES_PREVIOUS_PASSWORD = 71,
        USER_AUTHORISATION_PIN_INCORRECT = 72,
        NOT_IN_CORRECT_STATUS = 100,
        DUPLICATE_ISSUER_NAME = 200,
        DUPLICATE_ISSUER_CODE = 201,
        DUPLICATE_BRANCH_NAME = 210,
        DUPLICATE_BRANCH_CODE = 211,
        DUPLICATE_USER_GROUP_NAME = 215,
        DUPLICATE_PRODUCT_NAME = 220,
        DUPLICATE_PRODUCT_CODE = 221,
        DUPLICATE_PRODUCT_BIN = 222,
        DUPLICATE_SUB_PRODUCT_NAME = 223,
        DUPLICATE_SUB_PRODUCT_ID = 224,
        DUPLICATE_LDAP_NAME = 225,
        DUPLICATE_FEE_SCHEME_NAME = 226,
        DUPLICATE_FEE_DETAIL_NAME = 227,
        FILE_PARAMETER_ALREADY_IN_USE = 228,
        ACCOUNT_VALIDATION_FAIL = 300,
        NO_CARD_AVAILABLE_FOR_BATCH = 400,
        COREBANKING_SUCCESS = 500,
        COREBANKING_FAILED = 501,
        COREBANKING_PARAMETER_MISSING = 502,
        COREBANKING_NEW_CUSTOMER_AND_ACCOUNT_FAILED = 503,
        COREBANKING_EXISTING_CUSTOMER_NEW_ACCOUNT_FAILED = 504,
        COREBANKING_RELINK_FAILED = 505,
        COREBANKING_EDIT_FAILED = 506,
        COREBANKING_LIST_ACCOUNTS_FAILED = 507,
        UNLICENCED_ISSUER = 600,
        LICENCE_INVALID = 601,
        LICENCE_EXPIRED = 602,
        LICENCE_ERROR = 603,
        LINKED_FEE_SCHEME_PRODUCT = 702,
        LINKED_FEE_SCHEME_SUBPRODUCT = 703
    }

    /// <summary>
    /// List of system area's to be used with the response code. 
    /// System area enables you to provide a more specific message than just
    /// the generic message sent by SystemArea = 0.
    /// Please note that if using SystemArea you must insert records for all
    /// the ResponseCodes that might be returned by the database/calling method.
    /// Example: you'd have to insert SystemResponseCode = 0, SystemArea = 25 for
    /// Product insert success and SystemResponseCode = 1, SystemArea = 25 for
    /// general/unknown failure as well as any other SystemResponseCodes.
    /// </summary>
    public enum SystemArea
    {
        GENERIC = 0,
        LOAD_BATCH_APPROVE = 16,
        LOAD_BATCH_REJECT = 17,
        PRODUCT_ADMIN_CREATE = 25,
        PRODUCT_ADMIN_UPDATE = 26,
        ISSUE_CARD_STATUS_UPDATE_GENERAL = 30,
        ISSUE_CARD_STATUS_PRINT_SUCCESS = 31,
        ISSUE_CARD_STATUS_MAKERCHECKER_APPROVED = 32,
        ISSUE_CARD_STATUS_RESERVE_CARD = 33,
        ISSUE_CARD_STATUS_PRINT_FAILED = 34,
        ISSUE_CARD_STATUS_PIN_CAPTURED = 35,
        ISSUE_CARD_STATUS_MAKERCHECKER_REJECTED = 36,
        CENTRALISED_CARD_STATUS_MAKERCHECKER_APPROVED = 37,
        CENTRALISED_CARD_STATUS_MAKERCHECKER_REJECTED = 38
    }

    public enum CMS
    {
        POSTILION,
        SMART_VISTA,
        TIETO
    }

    public enum PrintSide
    {

        FRONT = 0,
        BACK = 1,
        NONE = 2,
    }
    public enum SERVICE
    {
        INSTANT_ISSUE,
        PIN_MAILER_PRINTING,
        CLASSIC_ISSUE
    }

    public enum AuditActionType
    {
        BranchAdmin,
        CardManagement,
        DistributionBatch,
        IssueCard,
        IssuerAdmin,
        LoadBatch,
        Logon,
        UserAdmin,
        UserGroupAdmin
    }
}