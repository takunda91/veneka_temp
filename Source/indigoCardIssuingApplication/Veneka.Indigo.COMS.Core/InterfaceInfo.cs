using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Veneka.Indigo.Integration.Config;

namespace Veneka.Indigo.COMS.Core
{
    [DataContract]
    public class InterfaceInfo
    {
        [DataMember(IsRequired = true)]
        public string InterfaceGuid { get; set; }

        
        [IgnoreDataMember]
        [XmlIgnore]
        public IConfig Config {get; set;}


        [DataMember]
        public Veneka.Indigo.Integration.Config.WebServiceConfig WebServiceSettings {
            get { if (Config is WebServiceConfig) return (WebServiceConfig)Config; else return null; }
            set { Config=value; } }

        [DataMember]
        public Veneka.Indigo.Integration.Config.SocketConfig SocketSettings
        {
            get { if (Config is SocketConfig) return (SocketConfig)Config; else return null; }
            set { Config = value; }
        }
    

        [DataMember]
        public Veneka.Indigo.Integration.Config.FileSystemConfig FileSystemSettings
        {
            get { if (Config is FileSystemConfig) return (FileSystemConfig)Config; else return null; }
            set { Config = value; }
        }

        [DataMember]
        public Veneka.Indigo.Integration.Config.SMTPConfig SMTPConfig
        {
            get { if (Config is SMTPConfig) return (SMTPConfig)Config; else return null; }
            set { Config = value; }
        }

        [DataMember]
        public Veneka.Indigo.Integration.Config.ThalesConfig ThalesConfig
        {
            get { if (Config is ThalesConfig) return (ThalesConfig)Config; else return null; }
            set { Config = value; }
        }


       
    }
}
