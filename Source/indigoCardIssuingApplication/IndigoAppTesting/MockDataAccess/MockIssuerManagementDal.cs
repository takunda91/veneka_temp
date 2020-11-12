using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.IssuerManagement;
using Veneka.Indigo.IssuerManagement.dal;
using Veneka.Indigo.IssuerManagement.objects;

namespace IndigoAppTesting.MockDataAccess
{
    public class MockIssuerManagementDal : IIssuerManagementDal
    {
        public ConnectionParamsResult CreateConnectionParam(ConnectionParamsResult connectionParam, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode CreateIssuer(IssuerResult issuerObj, long auditUserId, string auditWorkstation, out IssuerResult issuerResult)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode CreateLdapSetting(LdapSettingsResult ldapSetting, long auditUserId, string auditWorkstation, out int ldapSettingId)
        {
            throw new NotImplementedException();
        }

        public void DeleteConnectionParam(int connectionParamId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public void DeleteLdapSetting(int ldap_setting_id, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<issuers_Result> GetAllIssuers(int? LanguageId, int? pageIndex, int? Rowsperpage)
        {
            throw new NotImplementedException();
        }

        public List<AuthenticationtypesResult> GetAuthenticationTypes(long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public ConnectionParamsResult GetConnectionParameter(int connParameterId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<ConnectionParamsResult> GetConnectionParameters(long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<country> GetCountries(long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public IssuerResult GetIssuer(int issuerId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public IssuerResult GetIssuer(string issuerCode, long auditUserId, string auditWorkstation)
        {
            if (issuerCode == "001")
            {
                IssuerResult result = new IssuerResult();

                result.Issuer = new issuer
                {
                    issuer_id = 0                    
                };

                return result;
            }
            else
                return null;
        }

        public List<IssuerInterfaceResult> GetIssuerConnectionParams(int connectionParamId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public ConnectionParamsResult GetIssuerInterface(int issuerId, int interfaceTypeId, int interfaceAreaId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<issuer_interface> GetIssuerInterfaces(int issuerId, int? interfaceTypeId, int? interfaceAreaId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<LdapSettingsResult> GetLdapSettings(long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<ProductInterfaceResult> GetProductInterfaces(int connectionParamId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public DBResponseMessage LoadLicense(global::Veneka.Licensing.Common.IndigoComponentLicense license, string key, byte[] xmlLicenseFile, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<issuer> SearchIssuer(int issuerID, string issuerName, InstitutionStatus status)
        {
            throw new NotImplementedException();
        }

        public void UpdateConnectionParam(ConnectionParamsResult connectionParam, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode UpdateIssuer(IssuerResult issuerObj, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode UpdateLdapSetting(LdapSettingsResult ldapSetting, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }
    }
}
