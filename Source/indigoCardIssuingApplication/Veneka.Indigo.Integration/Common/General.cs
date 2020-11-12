using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Common
{
    public enum CardIssueReasons
    {
        NEW_ACCOUNT_NEW_CUSTOMER = 0,
        NEW_ACCOUNT_EXISTING_CUSTOMER = 1,
        RENEWAL = 2,
        REPLACEMENT = 3,
        SUPPLEMENTARY = 4
    }

    public sealed class General
    {
        public const string CBS_LOGGER = "CBSInterfacelogging";
        public const string CMS_LOGGER = "CMSInterfacelogging";
        public const string CPS_LOGGER = "CPSInterfacelogging";
        public const string HSM_LOGGER = "HSMInterfacelogging";
        public const string FILE_LOADER_LOGGER = "FileLoaderlogging";
        public const string EXTERNAL_AUTH_LOGGER = "ExternalAuthlogging";
        public const string Notification_LOGGER = "NotificationInterfacelogging";
        public const string ATMFLAGUPDATE_LOGGER = "AtmflagUpdateInterfacelogging";


    }
}
