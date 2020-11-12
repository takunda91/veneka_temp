using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.Security;
using System.Configuration;
using Common.Logging;

namespace Veneka.Indigo.Common
{
    public class SystemConfiguration
    {
        public const string SYSTEM_WORKSTATION = "SYSTEM";
        public const long SYSTEM_USER_ID = -2;
        public const int ROWS_PER_PAGE = 20;

        private static readonly ILog log = LogManager.GetLogger(typeof(SystemConfiguration));

        private static SystemConfiguration _instance;
        private static readonly object lockObject = new object();

        private string _baseConfigDir;

        private SystemConfiguration()
        {
            try
            {
                _baseConfigDir = ConfigurationManager.AppSettings["BaseConfigDir"].ToString();
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
            }
        }

        public static SystemConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new SystemConfiguration();
                        }
                    }
                }

                return _instance;
            }
        }

        public string GetBaseConfigDir()
        {
            return _baseConfigDir ?? String.Empty;
        }
    }
}