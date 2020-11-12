using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace indigoCardIssuingWeb.Old_App_Code.security
{
    [Serializable]
    public class IndigoIdentity : IIdentity
    {
        public IIdentity Identity { get; set; }
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsDomainUser { get; set; }
        public bool MustChangePassword { get; private set; }
        public string SessionKey { get; set; }
        public string WorkStation { get; set; }

        public bool ISMultiFactorenabled { get; set; }

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <returns>
        /// The name of the user on whose behalf the code is running.
        /// </returns>
        public string Name
        {
            get { return Identity.Name; }
        }

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        /// <returns>
        /// The type of authentication used to identify the user.
        /// </returns>
        public string AuthenticationType
        {
            get { return Identity.AuthenticationType; }
        }


        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <returns>
        /// true if the user was authenticated; otherwise, false.
        /// </returns>
        public bool IsAuthenticated { get { return Identity.IsAuthenticated; } }

        public int AuthConfigId { get; set; }


        public IndigoIdentity(IIdentity identity, string sessionKey)
        {
            Identity = identity;

            var indigoMembershipUser = (IndigoMembershipUser)Membership.GetUser(new Tuple<string, string>(identity.Name, sessionKey));

            if (indigoMembershipUser != null)
            {
                UserId = indigoMembershipUser.UserId;
                FirstName = indigoMembershipUser.FirstName;
                LastName = indigoMembershipUser.LastName;
                Email = indigoMembershipUser.Email;
                IsDomainUser = indigoMembershipUser.IsDomainUser;
                MustChangePassword = indigoMembershipUser.MustChangePassword;
                SessionKey = sessionKey;
                WorkStation = indigoMembershipUser.Workstation;
                ISMultiFactorenabled = indigoMembershipUser.ISMultiFactorenabled;
                AuthConfigId = indigoMembershipUser.AuthConfigId;
            }
        }

        public IndigoIdentity(IIdentity identity, long userId, string firstName, string lastName,
                                string email, bool isDomainUser, bool mustChangePassword, bool iSMultiFactorenabled, int authConfigId, string sessionKey)
        {
            this.Identity = identity;
            this.UserId = userId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.IsDomainUser = isDomainUser;
            this.MustChangePassword = mustChangePassword;
            this.SessionKey = sessionKey;
            this.ISMultiFactorenabled = iSMultiFactorenabled;
            this.AuthConfigId = authConfigId;
        }


    }
}