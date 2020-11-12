using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Config
{
    public class ActiveDirectoryConfig : Config, IConfig
    {
        #region Properties
        public string Address { get; private set; }
        public int Port { get; private set; }
        public string Path { get; private set; }
        public int? Timeout { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public Guid InterfaceGuid { get; private set; }
        #endregion

        #region Constructors
        internal ActiveDirectoryConfig()
        { }

        public ActiveDirectoryConfig(Guid interfaceGuid, string address, int port, string path, int? timeout, string username, string password)
        {
            ValidateArguments(address, path, timeout, username);

            this.InterfaceGuid = interfaceGuid;
            this.Address = address;
            this.Port = port;
            this.Path = path;
            this.Timeout = timeout;
            this.Username = username;
            this.Password = password;
        }

        public ActiveDirectoryConfig(Guid interfaceGuid, string address, int port, string path, int? timeout)
            : this(interfaceGuid, address, port, path, timeout, "", "")
        {
        }

        public ActiveDirectoryConfig(string interfaceGuid, string address, int port, string path, int? timeout, string username, string password)
            : this(Guid.Parse(interfaceGuid), address, port, path, timeout, username, password)
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
            this.Path = configRow.Field<string>(FIELD_PATH);
            this.Timeout = configRow.Field<int?>(FIELD_TIMEOUT);
            this.Username = configRow.Field<string>(FIELD_USERNAME);
            this.Password = configRow.Field<string>(FIELD_PASSWORD);

            ValidateArguments(this.Address, this.Path, this.Timeout, this.Username);
        }

        /// <summary>
        /// Validate inpurt arguments.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="path"></param>
        /// <param name="timeout"></param>
        /// <param name="authType"></param>
        /// <param name="username"></param>
        private void ValidateArguments(string address, string path, int? timeout, string username)
        {
            if (String.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address", "Cannot be null or empty.");

            if (timeout != null && timeout < (10 * 1000))
                throw new ArgumentException("Timeout must be larger than 10 000 milliseconds.", "timeout");
        }
    }
}
