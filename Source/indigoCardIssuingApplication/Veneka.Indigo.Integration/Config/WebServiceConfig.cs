using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.Integration.Config
{
    /// <summary>
    /// Class holds configuration information for setting up Web services.
    /// </summary>

    [DataContract]
    public sealed class WebServiceConfig : Config, IConfig
    {
        #region Properties
        [DataMember]
        public Protocol Protocol { get; private set; }

        [DataMember]
        public string Address { get; private set; }

        [DataMember]
        public int Port { get; private set; }

        [DataMember]
        public string Path { get; private set; }

        [DataMember]
        public int? Timeout { get; private set; }

        [DataMember]
        public AuthType AuthType { get; private set; }

        [DataMember]
        public string Username { get; private set; }

        [DataMember]
        public string Password { get; private set; }

        [DataMember]
        public string Nonce { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string RemoteUsername { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string RemotePassword { get; set; }

        [DataMember]
        public Guid InterfaceGuid { get; private set; }

        [DataMember]
        public int? RemotePort { get;  private set; }
        #endregion

        #region Constructors
        public WebServiceConfig()
        {  }

        public WebServiceConfig(Guid interfaceGuid, Protocol protocol, string address, int port, string path, int? timeout, AuthType authType, string username, string password, int? remotePort)
        {
            ValidateArguments(address, path, timeout, authType, username);

            this.InterfaceGuid = interfaceGuid;
            this.Protocol = protocol;
            this.Address = address;
            this.Port = port;
            this.Path = path;
            this.Timeout = timeout;
            this.AuthType = authType;
            this.Username = username;
            this.Password = password;
            this.RemotePort = remotePort;
        }
        public WebServiceConfig(Guid interfaceGuid, Protocol protocol, string address, int port, string path, int? timeout, AuthType authType, string username, string password,string nonce, int? remotePort)
        {
            ValidateArguments(address, path, timeout, authType, username);

            this.InterfaceGuid = interfaceGuid;
            this.Protocol = protocol;
            this.Address = address;
            this.Port = port;
            this.Path = path;
            this.Timeout = timeout;
            this.AuthType = authType;
            this.Username = username;
            this.Password = password;
            this.Nonce = nonce;
            this.RemotePort = remotePort;
        }
        public WebServiceConfig(Guid interfaceGuid, Protocol protocol, string address, int port, string path, int? timeout)
            : this(interfaceGuid, protocol, address, port, path, timeout, AuthType.None, "", "", null)
        {
        }

        public WebServiceConfig(string interfaceGuid, string protocol, string address, int port, string path, int? timeout, string authType, string username, string password)
            : this(Guid.Parse(interfaceGuid), ToProtocol(protocol), address, port, path, timeout, ToAuthType(authType), username, password, null)
        {
        }

        public WebServiceConfig(string interfaceGuid, string protocol, string address, int port, string path, int? timeout)
            : this(Guid.Parse(interfaceGuid), ToProtocol(protocol), address, port, path, timeout, AuthType.None, "", "", null)
        {
        }
        #endregion

        /// <summary>
        /// Load config parameters from data row.
        /// </summary>
        /// <param name="configTable"></param>
        public void LoadConfig(DataRow configRow)
        {
            this.InterfaceGuid = Guid.Parse(configRow.Field<string>(FIELD_INTERFACE_GUID));
            this.Protocol = ToProtocol(configRow.Field<int>(FIELD_PROTOCOL));
            this.Address = configRow.Field<string>(FIELD_ADDRESS);
            this.Port = configRow.Field<int>(FIELD_PORT);
            this.Path = configRow.Field<string>(FIELD_PATH);
            this.Timeout = configRow.Field<int?>(FIELD_TIMEOUT);
            this.AuthType = ToAuthType(configRow.Field<int>(FIELD_AUTH_TYPE));
            this.Username = configRow.Field<string>(FIELD_USERNAME);
            this.Password = configRow.Field<string>(FIELD_PASSWORD);
            this.Nonce = configRow.Field<string>(FIELD_NONCE);

            this.RemotePort = configRow.Field<int?>(FIELD_REMOTE_PORT);
            this.RemoteUsername = configRow.Field<string>(FIELD_REMOTE_USERNAME);
            this.RemotePassword = configRow.Field<string>(FIELD_REMOTE_PASSWORD);

            ValidateArguments(this.Address, this.Path, this.Timeout, this.AuthType, this.Username);
        }

        /// <summary>
        /// Validate inpurt arguments.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="path"></param>
        /// <param name="timeout"></param>
        /// <param name="authType"></param>
        /// <param name="username"></param>
        private void ValidateArguments(string address, string path, int? timeout, AuthType authType, string username)
        {

            if (String.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address", "Cannot be null or empty.");

            if (path == null)
                throw new ArgumentNullException("path", "Cannot be null.");

            if (timeout != null && timeout < (10 * 1000))
                throw new ArgumentException("Timeout must be larger than 10 000 milliseconds.", "timeout");

            if (authType != AuthType.None && String.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("username", "Cannot be null or empty if authType is not None.");
            }
        }
    }
}
