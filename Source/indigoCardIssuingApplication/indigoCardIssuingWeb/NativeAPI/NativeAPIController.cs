using Common.Logging;
using indigoCardIssuingWeb.Old_App_Code.security;
using Jose;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    public enum Action
    {
        PrintCard = 0,
        PINSelect = 1
    }
   
    public sealed class NativeAPIController
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(NativeAPIController));

        private static object _checkSessionLock = new object();

        //Get a new random secret key each day from app
        private static byte[] secretKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };

        //private static 


        public static string CreateToken(Guid checkGuid, IndigoIdentity user, Action action, long cardId, int branchId, string printJobId, bool isPinreissue,string productBin)
        {
            return GenerateToken(checkGuid, user.SessionKey, action, cardId, branchId,  printJobId, productBin, isPinreissue, 0);
        }        

        private static string GenerateToken(Guid checkGuid, string sessionKey, Action action, long cardId, int branchId, string printJobId, string productBin, bool isPinreissue, int step)
        {
            var payload = new Dictionary<string, object>()
            {
                { "checkGuid", checkGuid.ToString() },
                { "sessionKey", sessionKey },
                { "action", action },
                { "cardId", cardId },
                { "branchId", branchId },
                { "printJobId", printJobId },
                { "productBin", productBin },
                { "isPinReissue", isPinreissue },
                { "step", step },
                { "exp", DateTime.UtcNow.AddMinutes(5) }
            };

            return JWT.Encode(payload, secretKey, JweAlgorithm.A256GCMKW, JweEncryption.A256CBC_HS512);
        }

        public static bool ValidateToken(string token, Action action, int step, out Guid checkGuid, out string sessionKey, out long cardId, out int branchId, out bool isPinReissue,out string printJobId,out string productBin, out string nextToken)
        {
            nextToken = String.Empty;

            string json = JWT.Decode(token, secretKey, JweAlgorithm.A256GCMKW, JweEncryption.A256CBC_HS512);

            var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            Action payloadAction = (Action)Enum.ToObject(typeof(Action), payload["action"]);

            // Check user us still logged on - Not sure how we get a list of logged in users?

            // Check the action
            if (payloadAction != action)
            {
                _log.Warn("Token payload action mismatch");
                throw new Exception("Token mismatch");
            }

            // Check the step
            if (Convert.ToInt32(payload["step"]) != step)
            {
                _log.Warn("Token payload step mismatch");
                throw new Exception("Token mismatch");
            }

            // Check token hasn't expired
            if (Convert.ToDateTime(payload["exp"]) < DateTime.UtcNow)
                throw new Exception("Token expired");

            checkGuid = Guid.Parse(payload["checkGuid"].ToString());
            sessionKey = payload["sessionKey"].ToString();
            cardId = Convert.ToInt64(payload["cardId"]);
            branchId = Convert.ToInt32(payload["branchId"]);
            printJobId = payload["printJobId"].ToString();
            productBin = payload["productBin"].ToString();
            isPinReissue = Convert.ToBoolean(payload["isPinReissue"]);

            nextToken = GenerateToken(checkGuid,
                                        sessionKey,
                                        payloadAction,
                                        cardId,
                                        branchId,
                                        printJobId, productBin,
                                        isPinReissue,
                                        Convert.ToInt32(payload["step"]) + 1);            

            return true;
        }

        public static Guid CreateStatusSession()
        {
            var sessionGuid = Guid.NewGuid();

            HttpRuntime.Cache.Insert(sessionGuid.ToString(), 0, null, DateTime.UtcNow.Add(new TimeSpan(0, 30, 0)), System.Web.Caching.Cache.NoSlidingExpiration);

            return sessionGuid;
        }

        public static int CheckStatusSession(Guid sessionGuid)
        {
            var status = HttpRuntime.Cache.Get(sessionGuid.ToString());

            if (status == null)
                return -1;

            return (int)status;
        }

        public static bool UpdateStatusSession(Guid sessionGuid, int status)
        {
            var oldStatus = HttpRuntime.Cache.Remove(sessionGuid.ToString());

            if (oldStatus == null)
                return false;

            HttpRuntime.Cache.Insert(sessionGuid.ToString(), status, null, DateTime.UtcNow.Add(new TimeSpan(0, 30, 0)), System.Web.Caching.Cache.NoSlidingExpiration);

            return true;
        }
    }
}