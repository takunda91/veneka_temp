using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.Integration.Config
{
    public enum Protocol
    {
        HTTP,
        HTTPS,
        TCPIP,
    }

    public enum AuthType
    {
        None,
        Basic
    }

    [DataContract]
    public abstract class Config
    {
        #region Constant Fields Names     
        protected internal const string FIELD_INTERFACE_GUID = "interface_guid";
        protected internal const string FIELD_CONFIG_TYPE_ID = "connection_parameter_type_id";
        protected const string FIELD_PROTOCOL = "protocol";
        protected const string FIELD_ADDRESS = "address";
        protected const string FIELD_PORT = "port";
        protected const string FIELD_REMOTE_PORT = "remote_port";
        protected const string FIELD_REMOTE_USERNAME = "remote_username";
        protected const string FIELD_REMOTE_PASSWORD = "remote_password";
        protected const string FIELD_HEADER_LENGTH = "header_length";
        protected const string FIELD_PATH = "path";
        protected const string FIELD_TIMEOUT= "timeout_milli";
        protected const string FIELD_AUTH_TYPE = "auth_type";
        protected const string FIELD_USERNAME = "username";
        protected const string FIELD_PASSWORD = "password";
        protected const string FIELD_NONCE= "nonce";
        protected const string FIELD_FILENAME = "name_of_file";
        protected const string FIELD_DOC_TYPE = "doc_type";        
        protected const string FIELD_DELETE_FILE = "file_delete_YN";
        protected const string FIELD_DUPLICATE_FILE_CHECK = "duplicate_file_check_YN";
        protected const string FIELD_FILE_ENCRYPTION_TYPE = "file_encryption_type_id";
        #endregion

        public static Protocol ToProtocol(string protocol)
        {
            switch (protocol.Trim().ToUpper())
            {
                case "HTTP": return Protocol.HTTP;
                case "HTTPS": return Protocol.HTTPS;
                case "TCPIP":
                case "TCP-IP":
                case "TCP/IP": return Protocol.TCPIP;
                default: throw new ArgumentException("Unknown protocol type: " + protocol, "protocol");                   
            }
        }

        public static Protocol ToProtocol(int protocol)
        {
            switch (protocol)
            {
                case 0: return Protocol.HTTP;
                case 1: return Protocol.HTTPS;
                case 2: return Protocol.TCPIP;
                default: throw new ArgumentException("Unknown protocol type: " + protocol, "protocol");
            }
        }

        public static AuthType ToAuthType(string authType)
        {
            switch (authType.Trim().ToUpper())
            {
                case "NONE": return AuthType.None;
                case "BASIC": return AuthType.Basic;
                default: throw new ArgumentException("Unknown authType type: " + authType, "authType");
            }
        }

        public static AuthType ToAuthType(int authType)
        {
            switch (authType)
            {
                case 0: return AuthType.None;
                case 1: return AuthType.Basic;
                default: throw new ArgumentException("Unknown authType type: " + authType, "authType");
            }
        }
    }
}
