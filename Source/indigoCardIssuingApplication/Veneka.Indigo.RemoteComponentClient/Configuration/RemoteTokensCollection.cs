using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.RemoteComponentClient.Configuration
{
    public class RemoteTokensCollection : ConfigurationElementCollection
    {
        public const string CollectionName = "RemoteTokens";

        protected override ConfigurationElement CreateNewElement()
        {
            return new TokenElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TokenElement)element).Token;
        }
    }

    public class TokenElement : ConfigurationElement
    {
        [ConfigurationProperty("token", IsRequired = true)]
        public string Token
        {
            get { return (string)this["token"]; }
            set { this["token"] = value; }
        }
    }
}
