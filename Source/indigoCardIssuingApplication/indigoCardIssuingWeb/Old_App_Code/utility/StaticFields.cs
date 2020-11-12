namespace indigoCardIssuingWeb.utility
{
    public sealed class StaticFields
    {
        //Static Product Field codes.
        public const string IND_SYS_NOC = "IND_SYS_NOC";
        public const string IND_SYS_PAN = "IND_SYS_PAN";
        public static string ING_NOC = "ING_NOC";
        public const string IND_SYS_GENDER = "IND_SYS_GENDER";
        public const string IND_SYS_DOB = "IND_SYS_DOB";

        public static readonly string SOAP_ERROR =
            "The webserver is not available. Please close your browser and logon again";

        public static readonly string SUCCESS_RESPONSE = "SUCCESS";
        public static readonly string UNSUCCESS_RESPONSE = "UNSUCCESSFULL";
        public static readonly string SYSTEM_USER = "INDIGO_WEB_APP";
        public static readonly string UPDATE_SUCCESS_FULL = "Update Successful";
        public static readonly string DELETE_SUCCESS_FULL = "Delete Successful";
        public static readonly string[] ISSUE_CARDS_FOR_BATCH_REPORTHEADERS = new[] {"Card Number"};

        public static readonly string[] PIN_MAILER_LIST_REPORTHEADERS = new[]
            {"Card Number", "Status", "Printed", "Reprinted"};

        public static readonly string[] DIST_BATCH_STATUS_HIST_HEADING = new[] {"Date", "Batch Status", "User"};
        public static readonly string[] PIN_BATCH_STATUS_HIST_HEADING = new[] {"Date", "Batch Status", "User"};
        public static readonly float[] CARD_TABLE_COLUMNSIZE = new float[] {500};
        public static readonly float[] VAULT_INFORMATION_COLUMNSIZE = new float[] { 500, 500,500 };
        public static readonly float[] PIN_MAILER_TABLE_COLUMNSIZE = new float[] {500, 500, 500, 500};
        public static readonly float[] STATUS_TABLE_COLUMNSIZE = new float[] {500, 500, 500};
        public static readonly string DISTRIBUTION_BATCH_REPORT_HEADING = "Distribution Batch Report: ";
        public static readonly string PIN_BATCH_REPORT_HEADING = "PIN Mailer Batch Report: ";
        public static readonly string LOAD_BATCH_REPORT_HEADING = "Load Batch Report: ";
        public static readonly string[] CHECK_OUT_CARDS_HEADING = new[] { "Card Number", "Date", "Operator", "User" };
    }
}