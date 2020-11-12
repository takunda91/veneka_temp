using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veneka.Indigo.Remote
{
    public class RemoteErrorGenerator
    {
        public enum Error
        {
            TokenParseError = 1,
            InvalidTokenError = 2,
            FailedToProcess = 99
        };

        public static Integration.Remote.Response ResponseError(Error error)
        {
            switch (error)
            {
                case Error.TokenParseError: return new Integration.Remote.Response((int)error, "Error parsing remote token.");                    
                case Error.InvalidTokenError: return new Integration.Remote.Response((int)error, "Invalid token.");                    
                default: return new Integration.Remote.Response((int)Error.FailedToProcess, "Failed to process request.");                    
            }
        }
    }
}