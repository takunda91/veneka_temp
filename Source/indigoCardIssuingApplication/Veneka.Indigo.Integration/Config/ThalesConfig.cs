using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Config
{
    public class ThalesConfig : Config, IConfig
    {
        #region Properties
        public string Address { get; private set; }
        public int Port { get; private set; }
        public int HeaderLength { get; private set; }
        public int? Timeout { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public Guid InterfaceGuid { get; private set; }
        public char HsmDocumentType { get; private set; }
        #endregion

        #region Constructors
        internal ThalesConfig()
        { }

        public ThalesConfig(Guid interfaceGuid, string address, int port, int headerLength, char hsmDocumentType, int? timeout, string username, string password)
        {
            ValidateArguments(address, headerLength, timeout, username);

            this.InterfaceGuid = interfaceGuid;
            this.Address = address;
            this.Port = port;
            this.HeaderLength = headerLength;
            this.Timeout = timeout;
            this.Username = username;
            this.Password = password;
            this.HsmDocumentType = hsmDocumentType;
        }

        public ThalesConfig(Guid interfaceGuid, string address, int port, int headerLength, char hsmDocumentType, int? timeout)
            : this(interfaceGuid, address, port, headerLength,hsmDocumentType, timeout, "", "")
        {
        }

        public ThalesConfig(string interfaceGuid, string address, int port, int headerLength,char hsmDocumentType, string path, int? timeout, string username, string password)
            : this(Guid.Parse(interfaceGuid), address, port, headerLength, hsmDocumentType,timeout, username, password)
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
            this.HeaderLength = configRow.Field<int>(FIELD_HEADER_LENGTH);
            this.Timeout = configRow.Field<int?>(FIELD_TIMEOUT);
            this.Username = configRow.Field<string>(FIELD_USERNAME);
            this.Password = configRow.Field<string>(FIELD_PASSWORD);
            this.HsmDocumentType = configRow[FIELD_DOC_TYPE] != null ? Convert.ToChar(configRow[FIELD_DOC_TYPE].ToString().Trim()) : ' ';

            ValidateArguments(this.Address, this.HeaderLength, this.Timeout, this.Username);
        }

        /// <summary>
        /// Validate inpurt arguments.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="timeout"></param>
        /// <param name="username"></param>
        private void ValidateArguments(string address, int headerLength, int? timeout, string username)
        {
            if (String.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address", "Cannot be null or empty.");

            if(headerLength <= 0)
                throw new ArgumentException("Must have a value greater than 0.", "headerLength");

            if (timeout != null && timeout < (10 * 1000))
                throw new ArgumentException("Timeout must be larger than 10 000 milliseconds.", "timeout");
        }
    }
}
