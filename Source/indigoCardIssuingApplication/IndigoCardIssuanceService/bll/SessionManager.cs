using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Veneka.Indigo.UserManagement;
using Common.Logging;
using IndigoCardIssuanceService.Session.objects;
using Veneka.Indigo.Common.Exceptions;
using Veneka.Indigo.Common;
using System.Security.Cryptography;
using Veneka.Indigo.Security;
using Veneka.Indigo.Common.Utilities;

namespace IndigoCardIssuanceService.bll
{
    public class SessionManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SessionManager));
        private static SessionManager _instance;
        private static readonly object _lockObject = new object();
        private static readonly object _lockObject2 = new object();
        private static readonly Random random = new Random();

        private readonly Dictionary<string, SessionObject> _sessions = new Dictionary<string, SessionObject>();

        private SessionManager()
        {
        }

        public static SessionManager GetInstance()
        {
            if (_instance == null)
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new SessionManager();
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        /// Generate a new session key
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private string GenerateSessionKeys(string username)
        {
            var sessionKey = new StringBuilder(username + StaticFields.START_OF_SESSION_KEY);

            int tries = 0;
            do
            {
                if (tries > 10)
                {
                    throw new LoginFailedException("Failed to create session. Please try again.", Veneka.Indigo.Common.SystemResponseCode.SESSIONKEY_CREATE_FAIL);
                }

                tries++;
                sessionKey = new StringBuilder(username + StaticFields.START_OF_SESSION_KEY);
                //var rand = new Random(4);
                int randomNumber = random.Next(10000, 99999);
                sessionKey.Append(randomNumber);
            } while (_sessions.ContainsKey(sessionKey.ToString()));

            
            return sessionKey.ToString();
        }

        internal SessionObject AddSession(string username, string clientAddress, long userId, int userLanguageId, bool allowMultipleLogin)
        {
            SessionObject newSessionObj = null;

            lock (_lockObject2)
            {
                foreach (var sessionObject in _sessions.Values.Where(w => w.UserId == userId).ToArray())
                {
                    //RAB: Removed the time restriction, if user logs in successfully then kill his old session.
                    //if (!sessionObject.AllowMultipleLogins && sessionObject.LastAccess.AddMinutes(10) > DateTime.Now)
                    //{
                    //    throw new LoginFailedException("User not allowed to login multiple times.", Veneka.Indigo.Common.SystemResponseCode.SESSIONKEY_MULTI_LOGIN_FAIL);
                    //}
                    if (!sessionObject.AllowMultipleLogins)//if the elapsed time has passed kill the old session.
                    {
                        this.EndSession(sessionObject.SessionKey);
                    }

                    break;
                }

                newSessionObj = new SessionObject(this.GenerateSessionKeys(username), userId, username, clientAddress, userLanguageId, allowMultipleLogin);

                _sessions.Add(newSessionObj.SessionKey, newSessionObj);                
            }
            return newSessionObj;
        }

        /// <summary>
        /// Validate that the session key is not empy and that their is a valid current session.
        /// Log's warning if there is an issue.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public SessionObject isValidSession(string encryptedSessionKey, bool isLogin)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(encryptedSessionKey))
                {
                    throw new ArgumentException("Session key is null or empty.");
                }

                //string sessionKey = UtilityClass.DecryptSting(encryptedSessionKey);

                string sessionKey = EncryptionManager.DecryptString(encryptedSessionKey,
                                                                       StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                       StaticFields.EXTERNAL_SECURITY_KEY);

                //log.Debug(m => m("Validating session key: " + sessionKey));

                if (!String.IsNullOrWhiteSpace(sessionKey))
                {                    
                    //If this is a valid login attempt return a "valid" session object.
                    if(sessionKey.StartsWith("IndigoLoginAttempt") && isLogin)
                    {
                        return new SessionObject("login", 0, "login", "login", 0, false);
                    }

                    SessionObject sesionObj;
                    if(_sessions.TryGetValue(sessionKey, out sesionObj))
                    {
                        _sessions[sessionKey].LastAccess = DateTime.Now;
                        return sesionObj;
                    }                    
                }

                log.Warn("Attempted access with invalid session key.");
                log.Debug(m => m("Invalid session key used: " + sessionKey));

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }     

            return null;
        }

        /// <summary>
        /// Validate that the session key is not empy and that their is a valid current session.
        /// Log's warning if there is an issue.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public SessionObject isValidSession(string encryptedSessionKey)
        {
            return this.isValidSession(encryptedSessionKey, false);
        }

        /// <summary>
        /// Lock session list for session removal.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        internal bool EndUserSession(string sessionKey)
        {
            lock (_lockObject2)
            {
                return this.EndSession(sessionKey);
            }
        }

        /// <summary>
        /// Remove sesssion from list. NB does not place lock on the list.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private bool EndSession(string sessionKey)
        {
            return _sessions.Remove(sessionKey);
        }

        /// <summary>
        /// Use this to destroy all sessions belonging to a user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        internal bool KillSession(long userId)
        {
            try{
                lock (_lockObject2)
                {
                    //Find the session key for the user
                    List<string> sessionKeys = _sessions.Values.Where(w => w.UserId == userId)
                                                               .Select(s => s.SessionKey)
                                                               .ToList();

                    foreach (var sessionKey in sessionKeys)
                    {
                        _sessions.Remove(sessionKey);
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// Update the users langauge
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="languageId"></param>
        public void UpdateSessionLanguage(long userId, int language)
        {
            lock (_lockObject2)
            {
                foreach (var sessionObject in _sessions.Values.Where(w => w.UserId == userId).ToArray())
                {
                    //_sessions[sessionObject.SessionKey].UserLanguage = language;
                    _sessions[sessionObject.SessionKey].LanguageId = language;
                }
            }
        }


        public string GetUserName(string sessionKey)
        {
            SessionObject sessionObj;
            if (_sessions.TryGetValue(sessionKey, out sessionObj))
            {
                return sessionObj.Username;
            }

            return null;
        }

        public string GetClientAddress(string sessionKey)
        {
            SessionObject sessionObj;
            if (_sessions.TryGetValue(sessionKey, out sessionObj))
            {
                return sessionObj.Workstation;
            }
            return null;
        }

        public long? getUserId(string sessionKey)
        {
            SessionObject sessionObj;
            if (_sessions.TryGetValue(sessionKey, out sessionObj))
            {
                return sessionObj.UserId;
            }
            return null;
        }

        //public string decryptSessionKey(string encryptedSessionKey)
        //{
        //    return UtilityClass.DecryptSting(encryptedSessionKey);
        //}
    }
}