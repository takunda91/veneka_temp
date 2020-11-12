namespace Veneka.Indigo.Common.Utilities
{
    public class StaticNames
    {
        

        #region File Paths

        public const string DEFAULT_CORE_NAME = "Luminus EVD Core";
        public const string NODES_FILE_NAME = "servicenodes.xml";
        public const string CONFIGURATION_PATH = @"C:\veneka\luminus\EVD\config\";
        public const string LOG_FILE_PATH = @"C:\veneka\apps\logs\";
        public const string TRACE_FILE_PATH = @"C:\veneka\luminus\EVD\traces\";
        public const string GENERAL_CONFIGURATION_FILE_NAME = "config.xml";
        public const string DATABASE_CONFIGURATION_FILE_NAME = "database_settings.xml";
        public const string DEFAULT_VOUCHER_FILE_PATH = @"C:\veneka\luminus\EVD\input\voucher File\";
        public const string WEB_SERV_LOG_FILE_PATH = @"C:\veneka\apps\logs\";
        public const string WEB_SERV_ERR_FILE_PATH = @"C:\veneka\apps\logs\";
        public const string FILE_LOAD_LOG_FILE_PATH = @"C:\veneka\apps\logs\fileloaderservice\";
        public const string FILE_LOAD_ERR_FILE_PATH = @"C:\veneka\apps\logs\fileloaderservice\errors\";

        #endregion

        #region NAMES FOR NODE XML ELEMENTS

        public const string XML_NODE_ROOT = "ServiceNodes";
        public const string XML_NODE_NODE = "Node";
        public const string XML_NODE_NAME = "Name";
        public const string XML_NODE_ID = "ID";
        public const string XML_NODE_TRACE_ON = "TraceOn";
        public const string XML_NODE_SIGN_ON = "SignOn";
        public const string XML_NODE_MESSAGE_IN_NODE_TIME_OUT = "MessageTimeOut";
        public const string XML_NODE_INSTANCE_TYPE = "InstanceType";
        public const string XML_NODE_CONNECTION_POLL_PERIOD = "ConnectionPollPeriod";
        public const string XML_NODE_ECHO_TIME_TEST = "EchoTimeTest";
        public const string XML_NODE_IMPLIMENTATION_TYPE = "ImplementationType";
        public const string XML_NODE_LISTENING_IP_ADDRESS = "ListeningIPAddress";
        public const string XML_NODE_LISTENING_PORT_NUMBER = "ListeningPortNumber";
        public const string XML_NODE_SERVICE_TYPE = "ServiceType";

        #endregion

        #region Names FOR GENERAL  SETTINGS XML ELEMENTS

        public const string XML_SETTINGS_ROOT = "Settings";
        public const string XML_SETTINGS_SETTING = "Setting";
        public const string XML_SETTINGS_CORE_NAME = "CoreName";
        public const string XML_SETTINGS_TRACE = "Trace";
        public const string XML_SETTINGS_MESSAGE_TIME_OUT = "CoreMessageTimeOUT";

        #endregion

        #region Names  for database setting

        public const string XML_SETTINGS_DATABASE_ROOT = "Database";
        public const string XML_SETTINGS_DATABASE = "DatabaseSetting";
        public const string XML_SETTINGS_DATABASE_SERVER = "ServerName";
        public const string XML_SETTINGS_DATABASE_AUTHENTICATION_MODE = "Auth-Mode";
        public const string XML_SETTINGS_DATABASE_UserName = "UserName";
        public const string XML_SETTINGS_DATABASE_Password = "Password";

        #endregion names for Database settings

        #region General Names

        public const string DATABASE_SQL_AUTHENTICATION = "SQL_Authetication";
        public const string DATABASE_INTEGRATED_AUTHENTICATION = "Intergrated_Authetication";

        #endregion  General Names
    }
}