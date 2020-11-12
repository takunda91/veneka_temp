using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Licensing.Client;
using Veneka.Licensing.Common;

namespace Veneka.Indigo.Integration.FileLoader.Validation
{
    public sealed class IssuerValidation : Validation
    {
        private const string VALIDATE_ISSUER_START_INFO = "Validate issuer started.";
        private const string VALIDATE_ISSUER_END_INFO = "Validate issuer completed with status: {0}";
        private const string ISSUER_NOT_FOUND = "Could not find issuer for PAN starting with {0}.";
        private const string ISSUER_NOT_ACTIVE = "Issuer {0}-{1} is not active for PAN starting with {2}.";
        private const string ISSUER_NOT_LICENCED = "Issuer {0}-{1} not licenced.";
        private const string ISSUER_INVALID_LICENCE = "Issuer {0}-{1} has an invalid licence.";
        private const string ISSUER_EXPIRED_LICENCE = "Issuer {0}-{1} has an expired licence (expiry date:{2}).";
        private const string ISSUER_PRODUCT_NOT_LICENCED = "Issuer {0}-{1} is not licenced for BIN {2}.";

        private readonly ILog _logger;
        private readonly IIssuerDAL _issuerDal;
        private readonly string _vmgDirectory;

        private Dictionary<int, Issuer> _issuers = new Dictionary<int, Issuer>();//Key= issuer_id

        public IssuerValidation(IIssuerDAL issuerDAL, string vmgDirectory, string logger)
        {
            _issuerDal = issuerDAL;
            this._vmgDirectory = vmgDirectory;
            _logger = LogManager.GetLogger(logger);
        }

        public override FileStatuses ValidateCardFile(CardFile cardFile, List<FileCommentsObject> fileComments, long auditUserId, string auditWorkstation)
        {
            fileComments.Add(new FileCommentsObject(VALIDATE_ISSUER_START_INFO, _logger.Info));
            var result = base.ValidateCardFile(cardFile, fileComments, auditUserId, auditWorkstation);
            fileComments.Add(new FileCommentsObject(String.Format(VALIDATE_ISSUER_END_INFO, result.ToString()), _logger.Info));
            return result;
        }

        public override FileStatuses Validate(CardFileRecord record, long auditUserId, string auditWorkstation, out string fileComment)
        {
            fileComment = String.Empty;
            Issuer issuer;

            //Has issuer already been looked up
            if (!_issuers.TryGetValue(record.IssuerId.Value, out issuer))
            {
                issuer = _issuerDal.GetIssuer(record.IssuerId.Value, auditUserId, auditWorkstation);
                _issuers.Add(record.IssuerId.Value, issuer); //Add issuer to dictionary even if issuer is null, shows that we have already tried DB lookup
            }

            //Could we find the issuer
            if (issuer == null)
            {
                _logger.Error(fileComment = String.Format(ISSUER_NOT_FOUND, record.PAN.Substring(0, 6)));
                return FileStatuses.ISSUER_NOT_FOUND;
            }

            //Is the issuer active
            if(!issuer.IsActive)
            {
                _logger.Error(fileComment = String.Format(ISSUER_NOT_ACTIVE, issuer.IssuerCode, issuer.IssuerName, record.PAN.Substring(0, 6)));
                return FileStatuses.ISSUER_NOT_ACTIVE;
            }

            //Check that the product is licenced for the issuer.
            //Issuer not licenced, empty licence key
            if (String.IsNullOrWhiteSpace(issuer.LicenceKey))
            {
                _logger.Error(fileComment = String.Format(ISSUER_NOT_LICENCED, issuer.IssuerCode, issuer.IssuerName));
                return FileStatuses.UNLICENSED_ISSUER;
            }

            //Check the licence details
            var licenseInfo = ValidateClientLicense.VerifyComponentLicenseKey(issuer.LicenceKey, _vmgDirectory);

            if (licenseInfo != null)
            {
                IndigoComponentLicense indigoComLic = (IndigoComponentLicense)licenseInfo;

                //Make sure that the licence hasnt expired
                if (indigoComLic.ExpiryDate < DateTime.Now)
                {                    
                    _logger.Error(fileComment = String.Format(ISSUER_EXPIRED_LICENCE, issuer.IssuerCode, issuer.IssuerName, indigoComLic.ExpiryDate));
                    return FileStatuses.ISSUER_LICENCE_EXPIRED;
                }

                //Check if products BIN is licenced
                if (!indigoComLic.LicensedBinCodes.Exists(e => e.Trim() == record.BIN + record.SubProductCode))
                {
                    _logger.Error(fileComment = String.Format(ISSUER_PRODUCT_NOT_LICENCED, issuer.IssuerCode, issuer.IssuerName, record.BIN));
                    return FileStatuses.UNLICENSED_BIN;
                }
            }
            else
            {
                //Licence verification failed
                _logger.Error(fileComment = String.Format(ISSUER_INVALID_LICENCE, issuer.IssuerCode, issuer.IssuerName));
                return FileStatuses.INVALID_ISSUER_LICENSE;
            }

            record.IssuerId = issuer.IssuerId;
            return FileStatuses.READ;
        }

        public override FileStatuses Validate(BulkRequestRecord record, long auditUserId, string auditWorkstation, out string fileComment)
        {
            fileComment = String.Empty;
            Issuer issuer;

            //Has issuer already been looked up
            if (!_issuers.TryGetValue(record.IssuerId.Value, out issuer))
            {
                issuer = _issuerDal.GetIssuer(record.IssuerId.Value, auditUserId, auditWorkstation);
                _issuers.Add(record.IssuerId.Value, issuer); //Add issuer to dictionary even if issuer is null, shows that we have already tried DB lookup
            }

            //Could we find the issuer
            if (issuer == null)
            {
                _logger.Error(fileComment = String.Format(ISSUER_NOT_FOUND, record.ProductCode));
                return FileStatuses.ISSUER_NOT_FOUND;
            }

            //Is the issuer active
            if (!issuer.IsActive)
            {
                _logger.Error(fileComment = String.Format(ISSUER_NOT_ACTIVE, issuer.IssuerCode, issuer.IssuerName, record.ProductCode));
                return FileStatuses.ISSUER_NOT_ACTIVE;
            }

            //Check that the product is licenced for the issuer.
            //Issuer not licenced, empty licence key
            if (String.IsNullOrWhiteSpace(issuer.LicenceKey))
            {
                _logger.Error(fileComment = String.Format(ISSUER_NOT_LICENCED, issuer.IssuerCode, issuer.IssuerName));
                return FileStatuses.UNLICENSED_ISSUER;
            }

            //Check the licence details
            var licenseInfo = ValidateClientLicense.VerifyComponentLicenseKey(issuer.LicenceKey, _vmgDirectory);

            if (licenseInfo != null)
            {
                IndigoComponentLicense indigoComLic = (IndigoComponentLicense)licenseInfo;

                //Make sure that the licence hasnt expired
                if (indigoComLic.ExpiryDate < DateTime.Now)
                {
                    _logger.Error(fileComment = String.Format(ISSUER_EXPIRED_LICENCE, issuer.IssuerCode, issuer.IssuerName, indigoComLic.ExpiryDate));
                    return FileStatuses.ISSUER_LICENCE_EXPIRED;
                }

                //Check if products BIN is licenced
                if (!indigoComLic.LicensedBinCodes.Exists(e => e.Trim() == record.BIN + record.SubProductCode))
                {
                    _logger.Error(fileComment = String.Format(ISSUER_PRODUCT_NOT_LICENCED, issuer.IssuerCode, issuer.IssuerName, record.BIN));
                    return FileStatuses.UNLICENSED_BIN;
                }
            }
            else
            {
                //Licence verification failed
                _logger.Error(fileComment = String.Format(ISSUER_INVALID_LICENCE, issuer.IssuerCode, issuer.IssuerName));
                return FileStatuses.INVALID_ISSUER_LICENSE;
            }

            record.IssuerId = issuer.IssuerId;
            return FileStatuses.READ;
        }

        public override void Clear()
        {
            _issuers.Clear();
        }
    }
}