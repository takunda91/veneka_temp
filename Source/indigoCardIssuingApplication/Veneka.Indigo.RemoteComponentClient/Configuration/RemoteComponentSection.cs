using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.RemoteComponentClient.Configuration
{
    public class RemoteComponentSection : ConfigurationSection
    {
        /// <summary>
        /// The name of this section in the app.config.
        /// </summary>
        public const string SectionName = "RemoteComponent";        

        [ConfigurationProperty(RemoteTokensCollection.CollectionName)]
        [ConfigurationCollection(typeof(RemoteTokensCollection), AddItemName = "add")]
        public RemoteTokensCollection RemoteTokens { get { return (RemoteTokensCollection)base[RemoteTokensCollection.CollectionName]; } }

        [ConfigurationProperty("cardUpdateTimer")]
        public CardUpdateTimerElement CardUpdateTimer
        {
            get
            {
                return (CardUpdateTimerElement)this["cardUpdateTimer"];
            }
            set
            { this["cardUpdateTimer"] = value; }
        }

        [ConfigurationProperty("ApplicationConfig")]
        public ApplicationConfig ApplicationConfig
        {
            get
            {
                return (ApplicationConfig)this["ApplicationConfig"];
            }
            set
            { this["ApplicationConfig"] = value; }
        }
    }

    public class ApplicationConfig : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{};'\"|")]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }

        [ConfigurationProperty("allowUntrustedSSL", IsRequired = true, DefaultValue = false)]        
        public bool AllowUntrustedSSL
        {
            get { return Convert.ToBoolean(this["allowUntrustedSSL"]); }
            set { this["allowUntrustedSSL"] = value; }
        }
    }

    public class CardUpdateTimerElement : ConfigurationElement
    {
        [ConfigurationProperty("interval", IsRequired = true)]
        [IntegerValidator()]
        public int Interval
        {
            get { return (int)this["interval"]; }
            set { this["interval"] = value; }
        }
    }
}
