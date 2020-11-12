using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using indigoCardIssuingWeb.security;
using Microsoft.Reporting.WebForms;

namespace indigoCardIssuingWeb.Old_App_Code.service
{
    public class ReportServerCredentials : IReportServerCredentials
    {
        private string reportServerUserName;
        private string reportServerPassword;
        private string reportServerDomain;
        public ReportServerCredentials()
        {
            reportServerUserName = ConfigurationManager.AppSettings["ReportUserName"].ToString();
            string decryptedpassword = EncryptionManager.DecryptString(ConfigurationManager.AppSettings["ReportPassword"].ToString(),
                                                                  SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                  SecurityParameters.EXTERNAL_SECURITY_KEY);
            reportServerPassword = decryptedpassword;

            reportServerDomain =  ConfigurationManager.AppSettings["ReportDomain"].ToString();
            
        }

        public ReportServerCredentials(string userName, string password, string domain)
        {
            reportServerUserName = userName;
            reportServerPassword = password;
            reportServerDomain = domain;
        }

        public WindowsIdentity ImpersonationUser
        {
            get
            {
                // Use default identity.
                return null;
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                // Use default identity.
                return new NetworkCredential(reportServerUserName, reportServerPassword, reportServerDomain);
            }
        }

        public void New(string userName, string password, string domain)
        {
            reportServerUserName = userName;
            reportServerPassword = password;
            reportServerDomain = domain;
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {
            // Do not use forms credentials to authenticate.
            authCookie = null;
            user = null;
            password = null;
            authority = null;

            return false;
        }
    }
}