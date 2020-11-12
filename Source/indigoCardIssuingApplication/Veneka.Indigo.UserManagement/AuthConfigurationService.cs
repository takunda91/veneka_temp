using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.UserManagement.dal;
using Veneka.Indigo.UserManagement.objects;

namespace Veneka.Indigo.UserManagement
{
    public class AuthConfigurationService
    {
        private readonly IAuthConfigurationDAL _auth;

        //private readonly AuthConfigurationDAL _auth = new AuthConfigurationDAL();
        private readonly ResponseTranslator _translator = new ResponseTranslator();
        public AuthConfigurationService()
        {
            _auth = new AuthConfigurationDAL();
        }
        public AuthConfigurationService(IAuthConfigurationDAL dataSource)
        {
            _auth = dataSource;
        }
        public AuthConfigResult GetAuthConfiguration(int? authConfigurationId)
        {
            AuthConfigResult config_result = new AuthConfigResult();
            config_result.AuthConfig = _auth.GetAuthConfiguration(authConfigurationId);
            config_result.AuthConfigConnectionParams = _auth.GetAuthConfigParams(authConfigurationId);

            return config_result;
        }

        public List<auth_configuration_result> GetAuthConfigurationList(int? authConfigurationId, int? pageIndex, int? rowsPerPage)
        {
            return _auth.GetAuthConfigurationList(authConfigurationId, pageIndex, rowsPerPage);
        }
        public bool InsertAuthenticationConfiguration(AuthConfigResult authConfig, int language, long auditUserId, string auditWorkstation, out int authConfigurationId, out string responseMessage)
        {
            SystemResponseCode response = _auth.InsertAuthenticationConfiguration(authConfig, auditUserId, auditWorkstation, out authConfigurationId);
            responseMessage = _translator.TranslateResponseCode(response, 0, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;

        }

        public bool UpdateAuthenticationConfiguration(AuthConfigResult authConfig, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            SystemResponseCode response = _auth.UpdateAuthenticationConfiguration(authConfig, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, 0, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;

        }

        public bool DeleteAuthConfiguration(int authConfig, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            SystemResponseCode response = _auth.DeleteAuthConfiguration(authConfig, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, 0, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }
    }
}
