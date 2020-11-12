using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Exceptions;
using Veneka.Licensing.Client;
using Veneka.Licensing.Common;
using Veneka.Licensing.Common.Exceptions;

namespace Veneka.Indigo.Common.License
{
    public class LicenseManager
    {
        private static readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;
        private static readonly string VmgDirectory = Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), @"license\machine.vmg");

        public byte[] MachineId
        {
            get
            {
                byte[] machineIdBytes;

                if (System.IO.File.Exists(VmgDirectory))
                {
                    machineIdBytes = System.IO.File.ReadAllBytes(VmgDirectory);
                }
                else
                {
                    machineIdBytes = Veneka.Licensing.Client.MachineId.Generate();
                    System.IO.File.WriteAllBytes(VmgDirectory, machineIdBytes);
                }

                return machineIdBytes;
            }
        }

        public bool ValidateMainLicense()
        {
            using (XmlReader xmlReader = XmlReader.Create(@"C:\veneka\indigo_group\license\TestLic1.xml"))
            {
                ValidateClientLicense vcl = new ValidateClientLicense(xmlReader, VmgDirectory);

                if (vcl.Verify(false))
                {
                    MasterLicenseInfo m = (MasterLicenseInfo)vcl.LicenseInfo;
                }
                return vcl.Verify(false);
            }
        }

        /// <summary>
        /// Method checks that the license file has not been tampered with and is valid.
        /// </summary>
        /// <param name="licenseFileBytes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public IndigoComponentLicense ValidateAffiliateLicense(byte[] licenseFileBytes, out string key)
        {
            key = "";
            using (MemoryStream ms = new MemoryStream(licenseFileBytes))
            {
                using (XmlReader xmlReader = XmlReader.Create(ms))
                {
                    ValidateClientLicense vcl = new ValidateClientLicense(xmlReader, VmgDirectory);

                    if (vcl.Verify(true))
                    {
                        key = vcl.Key;
                        return (IndigoComponentLicense)vcl.LicenseInfo;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Return a list of valid bins for an affiliate.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<string> ValidateAffiliateKey(string key)
        {
            //var licenseInfo = ValidateClientLicense.VerifyComponentLicenseKey(key, VmgDirectory);

            //if (licenseInfo != null)
            //{
            //    IndigoComponentLicense indigoComLic = (IndigoComponentLicense)licenseInfo;

            //    if (indigoComLic.ExpiryDate > DateTime.Now)
            //    {
            //        return indigoComLic.LicensedBinCodes;
            //    }
            //}

            //return null;

            return ValidateAffiliateKey(key, VmgDirectory);
        }

        public static List<string> ValidateAffiliateKey(string key, string vmgDirectory)
        {
            var licenseInfo = ValidateClientLicense.VerifyComponentLicenseKey(key, vmgDirectory);

            if (licenseInfo != null)
            {
                IndigoComponentLicense indigoComLic = (IndigoComponentLicense)licenseInfo;

                if (indigoComLic.ExpiryDate > DateTime.Now)
                {
                    return indigoComLic.LicensedBinCodes;
                }
            }

            return null;
        }

        /// <summary>
        /// Return a list of valid bins for an affiliate.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IndigoComponentLicense ValidateAffiliateKeyLicense(string key)
        {
            var licenseInfo = ValidateClientLicense.VerifyComponentLicenseKey(key, VmgDirectory);

            if (licenseInfo != null)
            {
                IndigoComponentLicense indigoComLic = (IndigoComponentLicense)licenseInfo;

                return indigoComLic;
            }

            return null;
        }

        /// <summary>
        /// Validate a branch's affiliates licenes and return the valid BIN codes.
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public static List<string> ValidateAffiliateKeyByBranch(int branchId, long auditUserId, string auditWorkstation)
        {
            issuer licenseIssuer = new issuer();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_issuer_by_branch(branchId, auditUserId, auditWorkstation);

                List<issuer> issuerResults = results.ToList();

                if (issuerResults.Count > 0)
                {
                    licenseIssuer = issuerResults.First();
                }
            }

            if (!String.IsNullOrWhiteSpace(licenseIssuer.license_key))
            {
                var licenseInfo = ValidateClientLicense.VerifyComponentLicenseKey(licenseIssuer.license_key, VmgDirectory);

                if (licenseInfo != null)
                {
                    IndigoComponentLicense indigoComLic = (IndigoComponentLicense)licenseInfo;

                    if (indigoComLic.ExpiryDate > DateTime.Now)
                    {
                        return indigoComLic.LicensedBinCodes;
                    }
                    else
                    {
                        throw new BaseIndigoException("License for the issuer has expred. Expiry date: " + indigoComLic.ExpiryDate, SystemResponseCode.LICENCE_EXPIRED, null);                        
                    }
                }
                else
                {
                    throw new BaseIndigoException("Issuer has license, but there was a problem processing the license. License may be corrupt.", SystemResponseCode.LICENCE_ERROR, null);                  
                }
            }
            else
            {
                throw new BaseIndigoException("Issuer is not licensed.", SystemResponseCode.UNLICENCED_ISSUER, null);                
            }

            //return null;
        }

        /// <summary>
        /// Validate an affiliates licence and return the valid BIN codes.
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public static List<string> ValidateAffiliateKey(int issuerId, long auditUserId, string auditWorkstation)
        {
            issuer licenseIssuer = new issuer();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_issuer(issuerId, auditUserId, auditWorkstation);

                List<issuer> issuerResults = results.ToList();

                if (issuerResults.Count > 0)
                {
                    licenseIssuer = issuerResults.First();
                }
            }

            if (!String.IsNullOrWhiteSpace(licenseIssuer.license_key))
            {
                var licenseInfo = ValidateClientLicense.VerifyComponentLicenseKey(licenseIssuer.license_key, VmgDirectory);

                if (licenseInfo != null)
                {
                    IndigoComponentLicense indigoComLic = (IndigoComponentLicense)licenseInfo;

                    if (indigoComLic.ExpiryDate > DateTime.Now)
                    {
                        return indigoComLic.LicensedBinCodes;
                    }
                    else
                    {
                        throw new BaseIndigoException("License for the issuer has expred. Expiry date: " + indigoComLic.ExpiryDate, SystemResponseCode.LICENCE_EXPIRED, null);
                    }
                }
                else
                {
                    throw new BaseIndigoException("Issuer has license, but there was a problem processing the license. License may be corrupt.", SystemResponseCode.LICENCE_ERROR, null);
                }
            }
            else
            {
                throw new BaseIndigoException("Issuer is not licensed.", SystemResponseCode.UNLICENCED_ISSUER, null);
            }

            //return null;
        }

        public List<IndigoComponentLicense> GetLicenseListIssuers(bool? licensedIssuers, long auditUserId, string auditWorkstation)
        {
            List<IndigoComponentLicense> issuerlicensed = new List<IndigoComponentLicense>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_license_issuers(licensedIssuers, auditUserId, auditWorkstation);


                foreach (var result in results)
                {
                    IndigoComponentLicense comLicense = null;
                    if(!String.IsNullOrWhiteSpace(result.license_key))
                        comLicense = ValidateAffiliateKeyLicense(result.license_key);

                    if (comLicense != null)
                    {
                        issuerlicensed.Add(comLicense);
                    }
                    else
                    {
                        comLicense = new IndigoComponentLicense();
                        comLicense.IssuerCode = result.issuer_code;
                        comLicense.IssuerName = result.issuer_name;
                        //defaulting license time 
                        comLicense.ExpiryDate = DateTime.Now.AddYears(1);
                        issuerlicensed.Add(comLicense);
                    }
                }
            }

            return issuerlicensed;
        }
    }
}
