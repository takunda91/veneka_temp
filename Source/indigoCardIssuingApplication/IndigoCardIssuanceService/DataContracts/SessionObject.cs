using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndigoCardIssuanceService.Session.objects
{
    public class SessionObject
    {
        public string SessionKey { get; private set; }
        public long UserId { get; private set; }
        public string Username { get; private set; }
        public string Workstation { get; private set; }
        public DateTime LastAccess { get; set; }
        //public Veneka.Indigo.Common.IndigoLanguages UserLanguage { get; set; }
        public int LanguageId { get; set; }
        public bool AllowMultipleLogins { get; private set; }

        public SessionObject(string sessionKey, long userId, string username, string workstation, int languageId, bool allowMultipleLogins)
        {
            this.SessionKey = sessionKey;
            this.UserId = userId;
            this.Username = username;
            this.Workstation = workstation;
            this.LastAccess = DateTime.Now;            
            this.LanguageId = languageId;
            this.AllowMultipleLogins = allowMultipleLogins;
        }
    }
}