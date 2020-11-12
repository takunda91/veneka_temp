using indigoCardIssuingWeb.service;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Security.Principal;

namespace indigoCardIssuingWeb.Old_App_Code.security
{
    public sealed class IndigoMembershipProvider : MembershipProvider
    {
        public const string USER_DATA_CACHEKEY = "user_data_{0}";

        private const string _applicationName = "IndigoWebApp";
        private readonly UserManagementService userMan = new UserManagementService();
        private int _cacheTimeoutInMinutes = 30;

        #region Properties
        public override string ApplicationName
        {
            get
            {
                return _applicationName;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Initialize values from web.config.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            // Set Properties
            int val;
            if (!string.IsNullOrEmpty(config["cacheTimeoutInMinutes"]) && Int32.TryParse(config["cacheTimeoutInMinutes"], out val))
                _cacheTimeoutInMinutes = val;

            // Call base method
            base.Initialize(name, config);
        }

        public override bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            return userMan.LogIn(username, password);
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        /// <param name="username">The name of the user to get information for. </param><param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user. </param>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            //throw new NotImplementedException();
            //var cacheKey = string.Format(USER_DATA_CACHEKEY, username);

            //if (HttpRuntime.Cache[cacheKey] != null)
            //    return (IndigoMembershipUser)HttpRuntime.Cache[cacheKey];

            ////string[] data = username.Split('|');

            ////var user = userMan.GetUserByUsername(data[0], data[1]);
            //var user = new indigoCardIssuingWeb.CardIssuanceService.decrypted_user();

            //if (user == null)
            //    return null;

            //var membershipUser = new IndigoMembershipUser(user);

            ////Store in cache
            //HttpRuntime.Cache.Insert(cacheKey, membershipUser, null, DateTime.Now.AddMinutes(_cacheTimeoutInMinutes), Cache.NoSlidingExpiration);

            //return membershipUser;

            var user = userMan.GetUserByUsername(username);
            

            if (user == null)
                return null;

            var membershipUser = new IndigoMembershipUser(user);

            return membershipUser;
        }
      

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            //Tuple<username, SessionKey>
            var userKey = (Tuple<string, string>)providerUserKey;
            var cacheKey = string.Format(USER_DATA_CACHEKEY, userKey.Item2);

            if (HttpRuntime.Cache[cacheKey] != null)
                return (IndigoMembershipUser)HttpRuntime.Cache[cacheKey];

            var user = userMan.GetProfile(userKey.Item2);

            if (user == null)
                return null;

            var membershipUser = new IndigoMembershipUser(user);

            //Store in cache
            HttpRuntime.Cache.Insert(cacheKey, membershipUser, null, DateTime.Now.AddMinutes(_cacheTimeoutInMinutes), Cache.NoSlidingExpiration);

            return membershipUser;
        }
        #endregion

        #region NotImplemented
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            string result;

            if (userMan.UpdateUserPassword(null, oldPassword, newPassword, out result))
            {
                return true;
            }
            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}