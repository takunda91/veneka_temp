namespace IndigoFileLoader.utility
{
    internal sealed class PostilionDetailedRecordColumns
    {
        public static readonly int PAN = 0;
        public static readonly int PAN_MAX_LENGTH = 19;
        public static readonly int PAN_MIN_LENGTH = 16;

        public static readonly int SEQ_NR = 1;
        public static readonly int SEQ_NR_MAX_LENGTH = 3;

        public static readonly int EXPIRY_DATE = 2;
        public static readonly int EXPIRY_DATE_MAX_LENGTH = 4;

        public static readonly int CARD_HOLDER_NAME = 3;
        public static readonly int CARD_HOLDER_NAME_MAX_LENGTH = 26;

        public static readonly int CARD_HOLDER_TITLE = 4;
        public static readonly int CARD_HOLDER_TITLE_MAX_LENGTH = 5;

        public static readonly int CARD_HOLDER_FIRST_NAME = 5;
        public static readonly int CARD_HOLDER_FIRST_NAME_MAX_LENGTH = 20;

        public static readonly int CARD_HOLDER_MID_INITIALS = 6;
        public static readonly int CARD_HOLDER_MID_INITIALS_MAX_LENGTH = 10;

        public static readonly int CARD_HOLDER_LAST_NAME = 7;
        public static readonly int CARD_HOLDER_LAST_NAME_MAX_LENGTH = 20;

        public static readonly int CUSTOMER_ID = 8;
        public static readonly int CUSTOMER_ID_MAX_LENGTH = 25;

        public static readonly int ISSUER_REFRENCE = 9;
        public static readonly int ISSUER_REFRENCE_MAX_LENGTH = 20;

        public static readonly int COMPANY_NAME = 10;
        public static readonly int COMPANY_NAME_MAX_LENGTH = 26;

        public static readonly int CARD_TYPE = 11;
        public static readonly int CARD_TYPE_MAX_LENGTH = 10;

        public static readonly int CVV = 12;
        public static readonly int CVV_MAX_LENGTH = 3;

        public static readonly int CVV_2 = 13;
        public static readonly int CVV_2_MAX_LENGTH = 3;

        public static readonly int GENERATE_PIN = 14;

        public static readonly int PIN_OFFSET_OR_PVV = 15;
        public static readonly int PIN_OFFSET_OR_PVV_MAX_LENGTH = 12;

        public static readonly int PIN_INFO = 16;

        public static readonly int GENERATE_PIN_2 = 17;

        public static readonly int PIN_2_OFFSET_OR_PVV_2 = 18;
        public static readonly int PIN_2_OFFSET_OR_PVV_2_MAX_LENGTH = 12;

        public static readonly int PIN_2_INFO = 19;

        public static readonly int PRODUCE_CARD = 20;

        public static readonly int PRODUCE_CARD_MAILER = 21;

        public static readonly int PRODUCE_PIN_MAILER = 22;

        public static readonly int TRACK_2_DATA = 23;
        public static readonly int TRACK_2_DATA_MAX_LENGTH = 100;

        public static readonly int CARD_MAILER_ADDRESS_LINE_1 = 24;
        public static readonly int CARD_MAILER_ADDRESS_LINE_1_MAX_LENGTH = 30;

        public static readonly int CARD_MAILER_ADDRESS_LINE_2 = 25;
        public static readonly int CARD_MAILER_ADDRESS_LINE_2_MAX_LENGTH = 30;

        public static readonly int CARD_MAILER_ADDRESS_CITY = 26;
        public static readonly int CARD_MAILER_ADDRESS_CITY_MAX_LENGTH = 20;

        public static readonly int CARD_MAILER_ADDRESS_REGION = 27;
        public static readonly int CARD_MAILER_ADDRESS_REGION_MAX_LENGTH = 20;

        public static readonly int CARD_MAILER_ADDRESS_POSTAL_CODE = 28;
        public static readonly int CARD_MAILER_ADDRESS_POSTAL_CODE_MAX_LENGTH = 9;

        public static readonly int PIN_MAILER_ADDRESS_LINE_1 = 29;
        public static readonly int PIN_MAILER_ADDRESS_LINE_1_MAX_LENGTH = 30;

        public static readonly int PIN_MAILER_ADDRESS_LINE_2 = 30;
        public static readonly int PIN_MAILER_ADDRESS_LINE_2_MAX_LENGTH = 30;

        public static readonly int PIN_MAILER_ADDRESS_CITY = 31;
        public static readonly int PIN_MAILER_ADDRESS_CITY_MAX_LENGTH = 20;

        public static readonly int PIN_MAILER_ADDRESS_REGION = 32;
        public static readonly int PIN_MAILER_ADDRESS_REGION_MAX_LENGTH = 20;

        public static readonly int PIN_MAILER_ADDRESS_POSTAL_CODE = 33;
        public static readonly int PIN_MAILER_ADDRESS_POSTAL_CODE_MAX_LENGGTH = 9;
    }
}