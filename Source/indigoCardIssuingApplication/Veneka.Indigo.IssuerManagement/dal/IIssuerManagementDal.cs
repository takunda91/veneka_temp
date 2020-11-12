using System.Collections.Generic;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.IssuerManagement.objects;
using Veneka.Licensing.Common;

namespace Veneka.Indigo.IssuerManagement.dal
{
    public interface IIssuerManagementDal
    {
        ConnectionParametersResult CreateConnectionParam(ConnectionParametersResult connectionParam, long auditUserId, string auditWorkstation);
        SystemResponseCode CreateIssuer(IssuerResult issuerObj, string pin_notification_message, long auditUserId, string auditWorkstation, out IssuerResult issuerResult);
        SystemResponseCode CreateLdapSetting(LdapSettingsResult ldapSetting, long auditUserId, string auditWorkstation, out int ldapSettingId);
        void DeleteConnectionParam(int connectionParamId, long auditUserId, string auditWorkstation);
        void DeleteLdapSetting(int ldap_setting_id, long auditUserId, string auditWorkstation);
        List<issuers_Result> GetAllIssuers(int? LanguageId, int? pageIndex, int? Rowsperpage);
        List<AuthenticationtypesResult> GetAuthenticationTypes(long auditUserId, string auditWorkstation);
        ConnectionParamsResult GetConnectionParameter(int connParameterId, long auditUserId, string auditWorkstation);
        List<ConnectionParamAdditionalDataResult>  GetConnectionParametersAdditionalData(int connParameterId, long auditUserId, string auditWorkstation);

        List<ConnectionParamsResult> GetConnectionParameters(long auditUserId, string auditWorkstation);
        List<country> GetCountries(long auditUserId, string auditWorkstation);
        IssuerResult GetIssuer(string issuerCode, long auditUserId, string auditWorkstation);
        IssuerResult GetIssuer(int issuerId, long auditUserId, string auditWorkstation);
        IssuerResult GetIssuerPinMessage(int issuerId, long auditUserId, string auditWorkstation);
        List<IssuerInterfaceResult> GetIssuerConnectionParams(int connectionParamId, long auditUserId, string auditWorkstation);
        ConnectionParamsResult GetIssuerInterface(int issuerId, int interfaceTypeId, int interfaceAreaId, long auditUserId, string auditWorkstation);
        List<issuer_interface> GetIssuerInterfaces(int issuerId, int? interfaceTypeId, int? interfaceAreaId, long auditUserId, string auditWorkstation);
        List<LdapSettingsResult> GetLdapSettings(long auditUserId, string auditWorkstation);
        List<ProductInterfaceResult> GetProductInterfaces(int connectionParamId, long auditUserId, string auditWorkstation);
        DBResponseMessage LoadLicense(IndigoComponentLicense license, string key, byte[] xmlLicenseFile, long auditUserId, string auditWorkstation);
        List<issuer> SearchIssuer(int issuerID, string issuerName, InstitutionStatus status);
        void UpdateConnectionParam(ConnectionParametersResult connectionParam, long auditUserId, string auditWorkstation);
        SystemResponseCode UpdateIssuer(IssuerResult issuerObj, string pin_notification_message, long auditUserId, string auditWorkstation);
        SystemResponseCode UpdateLdapSetting(LdapSettingsResult ldapSetting, long auditUserId, string auditWorkstation);
    }
}