using Common.Logging;
using Jose;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndigoCardIssuanceService.bll
{
    public enum Action
    {
        PrintCard = 0
    }

    public sealed class BackOfficeAPIController
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(BackOfficeAPIController));

        private static object _checkSessionLock = new object();

        //Get a new random secret key each day from app
        private static byte[] secretKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };

        //private static 


        public static string CreateToken(Guid checkGuid, string SessionKey, long userId, Action action)
        {
            return GenerateToken(checkGuid, SessionKey, action, 0, userId);
        }

        private static string GenerateToken(Guid checkGuid, string sessionKey, Action action, int step, long userId)
        {
            var payload = new Dictionary<string, object>()
            {
                { "checkGuid", checkGuid.ToString() },
                { "sessionKey", sessionKey },
                { "action", action },
                { "userId", userId },
                //{ "branchId", branchId },
                { "step", step },
                { "exp", DateTime.UtcNow.AddMinutes(30) }
            };

            return JWT.Encode(payload, secretKey, JweAlgorithm.A256GCMKW, JweEncryption.A256CBC_HS512);
        }

        public static bool ValidateToken(string token, Action action, int step, out Guid checkGuid, out string sessionKey, out long userId, out string nextToken)
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
            userId = Convert.ToInt64(payload["userId"]);
            //branchId = Convert.ToInt32(payload["branchId"]);

            nextToken = GenerateToken(checkGuid,
                                        sessionKey,
                                        payloadAction,
                                        Convert.ToInt32(payload["step"]) + 1, userId);

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