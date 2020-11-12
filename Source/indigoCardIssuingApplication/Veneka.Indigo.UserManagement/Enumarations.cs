using System.Runtime.Serialization;
namespace Veneka.Indigo.UserManagement
{
    //NOTE: MAKE SURE THAT THESE VALUES CORRESPOND TO THE VALUES IN THE DB LOOKUP TABLE AND ARE HAVE THE SAME ID.
    //public enum UserStatus
    //{
    //    ACTIVE,
    //    INACTIVE,
    //    DELETED,
    //    ACCOUNT_LOCKED
    //}

    public enum Gender
    {
        MALE,
        FEMALE,
        UNSPECIFIED
    }

    public enum PrintFieldType
    {
        NONE,
        IMAGE
    }
    public enum AuthTypes
    {
        ExternalAuth,
        MultiFactor
    }

    public enum UserRole
    {
        ADMINISTRATOR,
        AUDITOR,
        BRANCH_CUSTODIAN,
        BRANCH_OPERATOR,
        CENTER_MANAGER,
        CENTER_OPERATOR,
        ISSUER_ADMIN,
        PIN_OPERATOR,
        USER_GROUP_ADMIN,
        USER_ADMIN,
        BRANCH_ADMIN,
        CARD_PRODUCTION,
        CMS_OPERATOR,
        PIN_PRINTER_OPERATOR,
        CARD_CENTRE_PIN_OFFICER,
        BRANCH_PIN_OFFICER,
        REPORT_ADMIN,
        USER_AUDIT,
        BRANCH_PRODUCT_OPERATOR,
        BRANCH_PRODUCT_MANAGER,
        CREDIT_ANALYST,
        CREDIT_MANAGER,
    }
}