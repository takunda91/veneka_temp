using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Config
{
   public class SMTPConfig : Config, IConfig
    {
        #region Properties
       

        public string SMTPServer { get; private set; }
        public int Port { get; private set; }
        public string SMTPUsername { get; private set; }
        public string SMTPPassword { get; private set; }


        public Guid InterfaceGuid { get; private set; }
        #endregion

        #region Constructors
        internal SMTPConfig()
        { }

        public SMTPConfig(Guid interfaceGuid, string SMTPaddress, int port, int? timeout, string SMTPusername, string SMTPpassword)
        {

            this.InterfaceGuid = interfaceGuid;
            this.SMTPServer = SMTPaddress;
            this.Port = port;
            this.SMTPUsername = SMTPUsername;
            this.SMTPPassword = SMTPPassword;
        }

        private void ValidateArguments(string address, int? timeout, string username,string password)
        {
            if (String.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address", "Cannot be null or empty.");


            if (String.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username", "Cannot be null or empty.");

            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password", "Cannot be null or empty.");

        }

        public SMTPConfig(Guid interfaceGuid, string address, int port, int? timeout)
            : this(interfaceGuid, address, port, timeout, "", "")
        {
        }

        public SMTPConfig(string interfaceGuid, string address, int port, string path, int? timeout, string username, string password)
            : this(Guid.Parse(interfaceGuid), address, port, timeout, username, password)
        {
        }
        

        public void LoadConfig(DataRow configRow)
        {
            this.InterfaceGuid = Guid.Parse(configRow.Field<string>(FIELD_INTERFACE_GUID));
            this.SMTPServer = configRow.Field<string>(FIELD_ADDRESS);
            this.Port = configRow.Field<int>(FIELD_PORT);
            
            this.SMTPUsername = configRow.Field<string>(FIELD_USERNAME);
            this.SMTPPassword = configRow.Field<string>(FIELD_PASSWORD);
            ValidateArguments(this.SMTPServer, 0, SMTPUsername, SMTPPassword);
        }

        #endregion

        
    }
   
}
