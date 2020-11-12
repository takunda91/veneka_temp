using System;
using System.Collections.Generic;
using Veneka.Indigo.IssuerManagement.dal;
using Veneka.Indigo.IssuerManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Objects;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.IssuerManagement.Exceptions;
using Veneka.Licensing.Common;

namespace Veneka.Indigo.IssuerManagement
{
    public class IssuerService
    {
        private readonly IIssuerManagementDal issuerDal;
        private readonly IResponseTranslator _translator;


        public IssuerService() : this(new IssuerManagementDal(), new ResponseTranslator())
        {

        }

        public IssuerService(IIssuerManagementDal issuerManagementDal, IResponseTranslator responseTranslator)
        {
            issuerDal = issuerManagementDal;
            _translator = responseTranslator;
        }

        #region Issuer

        /// <summary>
        /// Persist new issuer to the DB.
        /// Throws DuplicateIssuerException if trying to inset an issuer with the same name or code.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool CreateIssuer(IssuerResult issuer, string pin_notification_message, int languageId, long auditUserId, string auditWorkstation, out IssuerResult result, out string responseMessage)
        {
            var response = issuerDal.CreateIssuer(issuer,  pin_notification_message, auditUserId, auditWorkstation, out result);
            responseMessage = _translator.TranslateResponseCode(response, 0, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Persist changes to the DB.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool UpdateIssuer(IssuerResult issuer, string pin_notification_message, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = issuerDal.UpdateIssuer(issuer,  pin_notification_message, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, 0, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fetch all countires
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<country> GetCountries(long auditUserId, string auditWorkstation)
        {
            return issuerDal.GetCountries(auditUserId, auditWorkstation);
        }

        public List<issuer_interface> GetIssuerInterfaces(int issuerId, int? interfaceTypeId, int? interfaceAreaId, long auditUserId, string auditWorkstation)
        {
            return issuerDal.GetIssuerInterfaces(issuerId, interfaceTypeId, interfaceAreaId, auditUserId, auditWorkstation);
        }

        public List<issuer> GetIssuer(string issuerName)
        {
            return issuerDal.SearchIssuer(0, issuerName, InstitutionStatus.INACTIVE);
        }

        public List<issuers_Result> GetAllIssuers(int? LanguageId, int? pageIndex, int? Rowsperpage)
        {
            return issuerDal.GetAllIssuers(LanguageId, pageIndex, Rowsperpage);
        }

        /// <summary>
        /// Fetch issuer based on issuer id.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public IssuerResult GetIssuer(int issuerId, long auditUserId, string auditWorkstation)
        {
            return issuerDal.GetIssuer(issuerId, auditUserId, auditWorkstation);
        }

        public IssuerResult GetIssuerPinMessage(int issuerId, long auditUserId, string auditWorkstation)
        {
            return issuerDal.GetIssuerPinMessage(issuerId, auditUserId, auditWorkstation);
        }

        public IssuerResult GetIssuer(string issuerCode, long auditUserId, string auditWorkstation)
        {
            return issuerDal.GetIssuer(issuerCode, auditUserId, auditWorkstation);
        }

        #endregion

        #region LDAP Settings

        /// <summary>
        /// Fetch all ldap settings
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<LdapSettingsResult> GetLdapSettings(long auditUserId, string auditWorkstation)
        {
            return issuerDal.GetLdapSettings(auditUserId, auditWorkstation);
        }
        public List<AuthenticationtypesResult> GetAuthenticationTypes(long auditUserId, string auditWorkstation)
        {
            return issuerDal.GetAuthenticationTypes(auditUserId, auditWorkstation);
        }
        /// <summary>
        /// Persist new LDAP Setting to the database
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool CreateLdapSetting(LdapSettingsResult ldapSetting, int language, long auditUserId, string auditWorkstation, out int ldapSettingId, out string responseMessage)
        {
            var response = issuerDal.CreateLdapSetting(ldapSetting, auditUserId, auditWorkstation, out ldapSettingId);
            responseMessage = _translator.TranslateResponseCode(response, 0, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Persist changes to LDAP Setting to DB
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public bool UpdateLdapSetting(LdapSettingsResult ldapSetting, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = issuerDal.UpdateLdapSetting(ldapSetting, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, 0, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Persist changes to LDAP Setting to DB
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public void DeleteLdapSetting(int ldap_setting_id, long auditUserId, string auditWorkstation)
        {
            issuerDal.DeleteLdapSetting(ldap_setting_id, auditUserId, auditWorkstation);
        }

        #endregion

        #region Connection Parameters / Interfaces

        /// <summary>
        /// Fetch interface details for issuer.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceTypes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public ConnectionParamsResult GetIssuerInterface(int issuerId, int interfaceTypeId, int interfaceAreaId, long auditUserId, string auditWorkstation)
        {
            return issuerDal.GetIssuerInterface(issuerId, interfaceTypeId, interfaceAreaId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Fetch all connection parameters.
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<ConnectionParamsResult> GetConnectionParameters(long auditUserId, string auditWorkstation)
        {
            return issuerDal.GetConnectionParameters(auditUserId, auditWorkstation);
        }
        

        public ConnectionParametersResult GetConnectionParameter(int connParameterId, long auditUserId, string auditWorkstation)
        {
            ConnectionParametersResult rtnValue = new ConnectionParametersResult();

            rtnValue.ConnectionParams=issuerDal.GetConnectionParameter(connParameterId, auditUserId, auditWorkstation);
            rtnValue.additionaldata = issuerDal.GetConnectionParametersAdditionalData(connParameterId, auditUserId, auditWorkstation);

            return rtnValue;
        }

        /// <summary>
        /// Persist new connection param to the database
        /// </summary>
        /// <param name="connectionParam"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public ConnectionParametersResult CreateConnectionParam(ConnectionParametersResult connectionParam, long auditUserId, string auditWorkstation)
        {
            return issuerDal.CreateConnectionParam(connectionParam, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Persist changes to connection parameter to DB
        /// </summary>
        /// <param name="connectionParam"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public void UpdateConnectionParam(ConnectionParametersResult connectionParam, long auditUserId, string auditWorkstation)
        {
            issuerDal.UpdateConnectionParam(connectionParam, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Persist changes to connection parameter to DB
        /// </summary>
        /// <param name="connectionParam"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public void DeleteConnectionParam(int connectionParamId, long auditUserId, string auditWorkstation)
        {
            issuerDal.DeleteConnectionParam(connectionParamId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Connection Parameters: Check what products the connection is still linked to.
        /// </summary>
        /// <param name="connectionParamId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<ProductInterfaceResult> GetProductInterfaces(int connectionParamId, long auditUserId, string auditWorkstation)
        {
            return issuerDal.GetProductInterfaces(connectionParamId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Connection Parameters: Check what products the connection is still linked to.
        /// </summary>
        /// <param name="connectionParamId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<IssuerInterfaceResult> GetIssuerConnectionParams(int connectionParamId, long auditUserId, string auditWorkstation)
        {
            return issuerDal.GetIssuerConnectionParams(connectionParamId, auditUserId, auditWorkstation);
        }

        #endregion

        #region Licenses

        /// <summary>
        /// Persist license for issuer to DB.
        /// </summary>
        /// <param name="license"></param>
        /// <param name="key"></param>
        /// <param name="xmlLicenseFile"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public bool LoadLicense(IndigoComponentLicense license, string key, byte[] xmlLicenseFile, long auditUserId, string auditWorkstation, out string messages)
        {
            DBResponseMessage dbResponse = issuerDal.LoadLicense(license, key, xmlLicenseFile, auditUserId, auditWorkstation);
            messages = "";

            if (dbResponse == DBResponseMessage.RECORD_NOT_FOUND)
            {
                messages = "Could not find issuer for issuer licensing file.";
                return false;
                //throw new Exception("Could not find issuer for issuer licensing file.");
            }

            return true;
        }

        #endregion
    }
}