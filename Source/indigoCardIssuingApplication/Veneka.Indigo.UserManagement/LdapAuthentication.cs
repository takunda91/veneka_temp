using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Veneka.Indigo.UserManagement
{
    /// <summary>
    /// This class provides LDAP user authentication.
    /// </summary>
    public class LdapAuthentication
    {  
        private string _path;
        private String _filterAttribute;
        private static readonly ILog log = LogManager.GetLogger(typeof(LdapAuthentication));
        private static readonly ILog ldaplog = LogManager.GetLogger("LDAPLogging");

        /// <summary>
        /// Method does an LDAP lookup for provided user.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>returns true if user can successfuly authenticate againt the LDAP domain.</returns>
        internal bool AuthenticateUser(string path, string domain, String username, String password)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("Domain path cannot be null or empty.");
            }

            if (String.IsNullOrWhiteSpace(domain))
            {
                throw new ArgumentNullException("Domain cannot be null or empty.");
            }           


            //String domainAndUsername = domain + @"\" + username;
            ldaplog.Debug(m => m("Using LDAP path: " + path + " to authenticate user " + username));

            DirectoryEntry entry = new DirectoryEntry(path, username, password);

            ldaplog.Debug(m => m("LDAP Authentication Type: " + entry.AuthenticationType.ToString()));            

            bool isValid = false;

            try
            {	//Bind to the native AdsObject to force authentication.			
                Object obj = entry.NativeObject;

                ldaplog.Debug(m => m("Searching for user"));
                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=rbrenchley)";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    ldaplog.Debug(m => m("No user found"));
                    //return isValid;
                }
                else
                {
                    //Update the new path to the user in the directory.
                    _path = result.Path;
                    _filterAttribute = (String)result.Properties["cn"][0];
                    isValid = true;
                }                
            }
            catch (DirectoryServicesCOMException dsex)
            {
                ldaplog.Error(dsex);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            try
            {
                ldaplog.Debug(m => m("Now checking with Principle Context"));
                //PrincipalContext adContext = new PrincipalContext(
                //LDAP:///
                //PrincipalContext adContext = new PrincipalContext(ContextType.Domain, "EPG-DC-01.ecobank.group", "OU=Usr,DC=ecobank,DC=group");
                //using (adContext)
                //{

                //}


                bool val = false;
                using (var adContext = new PrincipalContext(ContextType.Domain, 
                                                            "EPG-DC-01.ecobank.group", 
                                                            "OU=Usr,DC=ecobank,DC=group"
                                                            //,"rbrenchley","P@ssw0rd1"
                                                            ))
                {
                    val = adContext.ValidateCredentials("rbrenchley", "P@ssw0rd1");

                    UserPrincipal usr = UserPrincipal.FindByIdentity(adContext,
                                           IdentityType.SamAccountName,
                                           "rbrenchley");
                    var acountLockedout = usr.IsAccountLockedOut();

                    ldaplog.Debug(m => m("Principle Context reports user is: " + val));
                }

                
                
            }
            catch (Exception ex)
            {
                ldaplog.Error(ex);
            }

            return isValid;
        }
    }
}
