using System.Runtime.Serialization;

namespace IndigoCardIssuanceService
{
    public enum RequestType
    {
        LOG_IN,
        UPDATE,
        CREATE,
        DELETE,
        ENQUIRE,
        ISSUE
    }

    public enum RequestObject
    {
        APPLICATION_USER,
        DISTRIBUTION_BATCH,
        LOAD_BATCH,
        LOAD_CARD,
        ISSUE_CARD,
    }

    [DataContract(Namespace = "http://schemas.veneka.com/Indigo", Name ="ResponseCode")]
    public enum ResponseType
    {
        [EnumMember(Value = "00")]
        SUCCESSFUL,
        [EnumMember(Value = "01")]
        UNSUCCESSFUL,
        [EnumMember(Value = "02")]
        FAILED,
        [EnumMember(Value = "03")]
        ERROR,
        [EnumMember(Value = "04")]
        SESSION_ERROR
    }
}