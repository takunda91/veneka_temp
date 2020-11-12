using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.RemoteComponentClient.Configuration
{
    public class ConfigReader
    {
        public static RemoteComponentSection RemoteComponent
        {
            get
            {
                ConfigurationManager.RefreshSection(RemoteComponentSection.SectionName);
                return ConfigurationManager.GetSection(RemoteComponentSection.SectionName) as RemoteComponentSection;
            }
        }

        public static int CardUpdateTimerMili
        {
            get { return RemoteComponent.CardUpdateTimer.Interval; }
        }

        public static System.IO.DirectoryInfo ApplicationConfigPath
        {
            get { return new System.IO.DirectoryInfo(RemoteComponent.ApplicationConfig.Path); }
        }

        public static bool AllowUntrustedSSL
        {
            get { return RemoteComponent.ApplicationConfig.AllowUntrustedSSL; }
        }

        public static List<string> GetRemoteTokens()
        {
            List<string> tokens = new List<string>();

            var remoteComponentSection = RemoteComponent;

            if (remoteComponentSection != null)
            {
                foreach (TokenElement tokenElement in remoteComponentSection.RemoteTokens)
                {
                    tokens.Add(tokenElement.Token);
                }
            }

            return tokens;
        }
    }
}
