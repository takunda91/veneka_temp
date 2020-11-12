using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.WebServices
{
    public class Helper
    {
        public static Protocol GetProtocol(Config.Protocol protocol)
        {
            switch(protocol)
            {
                case Config.Protocol.HTTP: return Protocol.HTTP;
                case Config.Protocol.HTTPS: return Protocol.HTTPS;
                default: throw new Exception("Unsupported parameter protocol: " + protocol.ToString());
            }
        }

        public static Authentication GetAuth(Config.AuthType authType)
        {
            switch (authType)
            {
                case Config.AuthType.Basic: return Authentication.BASIC;
                case Config.AuthType.None: return Authentication.NONE;
                default: throw new Exception("Unsupported parameter authentication type: " + authType.ToString());
            }
        }
    }
}
