using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Config
{
    public sealed class ConfigFactory
    {
        public enum ConfigType
        {
            WebService = 0,
            FileSystem = 1,
            Thales = 2,
            Socket = 3,
            ActiveDirectory = 4,
            SMTP = 5

        }

        private ConfigFactory() { }

        public static IConfig GetConfig(int configTypeId)
        {
            switch ((ConfigType)configTypeId)
            {
                case ConfigType.WebService: return new WebServiceConfig();
                case ConfigType.FileSystem: return new FileSystemConfig();
                case ConfigType.Thales: return new ThalesConfig();
                case ConfigType.Socket: return new SocketConfig();
                case ConfigType.ActiveDirectory: return new ActiveDirectoryConfig();
                case ConfigType.SMTP: return new SMTPConfig(); 
                default: throw new ArgumentException("Unknown config type: " + configTypeId, "configTypeId");
            }
        }

        public static IConfig GetConfig(DataRow row)
        {
            IConfig config;
            switch (row.Field<int>(Config.FIELD_CONFIG_TYPE_ID))
            {
                case 0: config = new WebServiceConfig(); break;
                case 1: config = new FileSystemConfig(); break;
                case 2: config = new ThalesConfig(); break;
                case 3: config = new SocketConfig(); break;
                case 4: config = new ActiveDirectoryConfig(); break;
                case 5: config = new SMTPConfig(); break;

                default: throw new ArgumentException("Unknown config type: " + row.Field<int>(Config.FIELD_CONFIG_TYPE_ID), "configTypeId");
            }

            config.LoadConfig(row);
            return config;
        }

        public static List<IConfig> GetConfigs(DataTable configTable)
        {
            if (configTable.Rows.Count == 0)
                throw new ArgumentNullException("configTable", "No rows found in DataTable.");

            List<IConfig> configs = new List<IConfig>();

            foreach (DataRow row in configTable.Rows)            
                configs.Add(GetConfig(row));

            return configs;
        }
    }
}
