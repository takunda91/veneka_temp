using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;
using Microsoft.IdentityModel.Logging;

namespace Veneka.Indigo.COMS.Core.Validations
{
    public class CustomActions
    {
        public static string key = "D16E42B5-3C6F-4EE5-B2F4-727BF8B74A92";
        public static string logpath = "C:\\veneka\\ComsCore\\Logs";
        [CustomAction]
        public static ActionResult Validate(Session session)
        {
            try
            {

                session.Log("Begin CustomAction");
                string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (!Directory.Exists(logpath))
                    Directory.CreateDirectory(logpath);

                string token = session["Token"];
                bool status = ValidateToken(token);
                //   File.WriteAllText("C:\\veneka" + "\\Win App\\registrationInfo.txt", status.ToString());

                if (status)
                    return ActionResult.Success;


            }
            catch (Exception ex)
            {
                File.WriteAllText(logpath+"\\InstallerLog.txt", ex.Message.ToString());

                return ActionResult.Failure;
            }
            return ActionResult.Failure;

        }

        private static bool ValidateToken(string authToken)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(key);
            IdentityModelEventSource.ShowPII = true;
            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            return true;

        }

        private static TokenValidationParameters GetValidationParameters(string key)
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = "Sample",
                ValidAudience = "Sample",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) // The same key as the one that generate the token
            };
        }
    }
}
