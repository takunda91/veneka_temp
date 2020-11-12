using Common.Logging;
using IndigoCardIssuanceService.bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using Veneka.Indigo.Security;
using Veneka.Indigo.ServicesAuthentication.API;
using Veneka.Indigo.ServicesAuthentication.API.DataContracts;

namespace IndigoCardIssuanceService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AuthenticationAPI" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AuthenticationAPI.svc or AuthenticationAPI.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = Constants.ServicesAuthApiUrl)]
    [AspNetCompatibilityRequirements(
      RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AuthenticationAPI : IAuthentication
    {
        private readonly UserManagementController _userManContoller = new UserManagementController();
        private static readonly ILog _log = LogManager.GetLogger(typeof(AuthenticationAPI));

        #region Service Authentication API
        public AuthenticationResponse Login(string username, string password)
        {
            string message = String.Empty;

            try
            {
                string encryptedUsername = EncryptionManager.EncryptString(username,
                                                                    Veneka.Indigo.Common.Utilities.StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                    Veneka.Indigo.Common.Utilities.StaticFields.EXTERNAL_SECURITY_KEY);
                string encryptedpwd = EncryptionManager.EncryptString(password,
                                                                         Veneka.Indigo.Common.Utilities.StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                         Veneka.Indigo.Common.Utilities.StaticFields.EXTERNAL_SECURITY_KEY);

                string workstation = EncryptionManager.EncryptString("APILoginAttempt" + DateTime.Now.ToString(),
                                                                       Veneka.Indigo.Common.Utilities.StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                       Veneka.Indigo.Common.Utilities.StaticFields.EXTERNAL_SECURITY_KEY);
                var responseObj = _userManContoller.LogIn(encryptedUsername, encryptedpwd, workstation);
                if (responseObj.ResponseType == ResponseType.SUCCESSFUL)
                {
                    string SessionKey = EncryptionManager.DecryptString(responseObj.Value.encryptedSessionKey,
                                                                    Veneka.Indigo.Common.Utilities.StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                    Veneka.Indigo.Common.Utilities.StaticFields.EXTERNAL_SECURITY_KEY);
                    string UserId = EncryptionManager.DecryptString(responseObj.Value.encryptedUserId,
                                                                                       Veneka.Indigo.Common.Utilities.StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                                       Veneka.Indigo.Common.Utilities.StaticFields.EXTERNAL_SECURITY_KEY);
                    var token = BackOfficeAPIController.CreateToken(Guid.NewGuid(), SessionKey, int.Parse(UserId), bll.Action.PrintCard);


                    return new AuthenticationResponse() { ResponseCode = "00", ResponseMessage = "SUCCESSFUL", AuthToken = token };

                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new AuthenticationResponse() { ResponseCode = "01", ResponseMessage = "failed", AuthToken = null };
        }

        public AuthenticationResponse MultiFactor(int type, string mfToken, string authToken)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
