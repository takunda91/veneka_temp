using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.Integration.Config
{
    /// <summary>
    /// Class holds configuration information for setting up socket connections.
    /// </summary>
    [DataContract]
    public sealed class SocketConfig : Config, IConfig
    {
        #region Properties
        [DataMember]
        public string Address { get; private set; }
        [DataMember]
        public int Port { get; private set; }
        [DataMember]
        public int? Timeout { get; private set; }
        [DataMember]
        public string Username { get; private set; }
        [DataMember]
        public string Password { get; private set; }
        [DataMember]
        public Guid InterfaceGuid { get; private set; }
        #endregion

        #region Constructors
        public SocketConfig()
        {  }

        public SocketConfig(Guid interfaceGuid, string address, int port, int? timeout, string username, string password)
        {
            ValidateArguments(address, timeout, username);

            this.InterfaceGuid = interfaceGuid;
            this.Address = address;
            this.Port = port;
            this.Timeout = timeout;
            this.Username = username;
            this.Password = password;
        }

        public SocketConfig(Guid interfaceGuid, string address, int port, int? timeout)
            : this(interfaceGuid, address, port, timeout, "", "")
        {
        }

        public SocketConfig(string interfaceGuid, string address, int port, string path, int? timeout, string username, string password)
            : this(Guid.Parse(interfaceGuid), address, port, timeout, username, password)
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
            this.Address = configRow.Field<string>(FIELD_ADDRESS);
            this.Port = configRow.Field<int>(FIELD_PORT);
            this.Timeout = configRow.Field<int?>(FIELD_TIMEOUT);
            this.Username = configRow.Field<string>(FIELD_USERNAME);
            this.Password = configRow.Field<string>(FIELD_PASSWORD);

            ValidateArguments(this.Address, this.Timeout, this.Username);
        }

        /// <summary>
        /// Validate inpurt arguments.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="timeout"></param>
        /// <param name="username"></param>
        private void ValidateArguments(string address, int? timeout, string username)
        {
            if (String.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address", "Cannot be null or empty.");

            if (timeout != null && timeout < (10 * 1000))
                throw new ArgumentException("Timeout must be larger than 10 000 milliseconds.", "timeout");
        }
    }
}
