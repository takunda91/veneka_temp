using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    public enum PrintJobStatuses
    {
        CREATED = 0,
        SENT_TO_PRINTER = 1,
        FAILED = 2,
        PRINTED = 3
    }
    public class Constants
    {
        public const string NativeAppApiUrl = "http://schemas.veneka.com/Indigo/UX/NativeAppAPI";
    }
}