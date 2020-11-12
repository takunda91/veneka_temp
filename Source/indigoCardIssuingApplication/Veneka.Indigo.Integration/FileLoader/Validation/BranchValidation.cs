using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.Integration.FileLoader.Validation
{
    public sealed class BranchValidation : Validation
    {
        private const string VALIDATE_BRANCH_START_INFO = "Validate branch started.";
        private const string VALIDATE_BRANCH_END_INFO = "Validate branch completed with status: {0}";
        private const string BRANCH_NOT_ACTIVE = "Branch {0}-{1} not active.";
        private const string NO_ACTIVE_BRANCH_FOUND = "No active branch with code {0} was not found.";
        private const string BRANCH_NOT_VALID_FOR_ISSUER = "Branch {0}-{1} not valid for issuer.";

        private const string CARD_CENTRE_CODE = "_ind_cc_int_";

        private readonly ILog _logger;
        private Dictionary<string, BranchLookup> _branches = new Dictionary<string, BranchLookup>();

        private static IBranchDAL _branchDAL;

        public BranchValidation(IBranchDAL branchDAL, string logger)
        {
            _branchDAL = branchDAL;
            _logger = LogManager.GetLogger(logger);
        }

        public override FileStatuses ValidateCardFile(CardFile cardFile, List<FileCommentsObject> fileComments, long auditUserId, string auditWorkstation)
        {            
            fileComments.Add(new FileCommentsObject(VALIDATE_BRANCH_START_INFO, _logger.Info));
            var result = base.ValidateCardFile(cardFile, fileComments, auditUserId, auditWorkstation);
            fileComments.Add(new FileCommentsObject(String.Format(VALIDATE_BRANCH_END_INFO, result.ToString()), _logger.Info));
            return result;
        }

        public override FileStatuses Validate(CardFileRecord record, long auditUserId, string auditWorkstation, out string fileComment)
        {
            fileComment = String.Empty;
            //not sure why this was default to 0??
            int issuerId = record.IssuerId.Value;
            BranchLookup branchLookup = null;

            //Is branch code present from the file? If it is try and look it up in the DB. If not check what the load type is and if we need to find card centre.
            if (!string.IsNullOrEmpty(record.BranchCode))
            {
                if (!_branches.TryGetValue(record.BranchCode, out branchLookup))
                {
                    branchLookup = _branchDAL.GetBranch(record.BranchCode, "", issuerId);
                    _branches.Add(record.BranchCode, branchLookup);
                }
            }
            else
            {
                //Check what kinda load we are doing, if load to centre find an active card centre.
                if (record.ProductLoadTypeId != null && record.ProductLoadTypeId.Value == 3)
                {
                    if (!_branches.TryGetValue(CARD_CENTRE_CODE, out branchLookup))//Check the cache
                    {
                        var cardCentres = _branchDAL.GetCardCentreList(issuerId);
                        //Get the first Active card centre
                        branchLookup = cardCentres.Where(w => w.isActive == true).FirstOrDefault();
                        _branches.Add(CARD_CENTRE_CODE, branchLookup);
                    }
                }
                else if (record.ProductLoadTypeId != null && record.ProductLoadTypeId.Value == 4)
                {
                    return FileStatuses.READ;
                }
                else
                {
                    throw new Exception("Branch code is empty.");
                }
            }
            //No idea why this code is here. if the branch code is not set then shouldnt just use the first branch for the issue
            //Should try and get the card centre branch?
            //else
            //{
            //    if (!_branches.TryGetValue(issuerId.ToString(), out branchLookup))
            //    {
            //        branchLookup = _branchDAL.GetBranchesForIssuer(issuerId);
            //        _branches.Add(issuerId.ToString(), branchLookup);
            //    }
            //}

            if (branchLookup == null)
            {
                _logger.Error(fileComment = String.Format(NO_ACTIVE_BRANCH_FOUND, record.BranchCode));
                return FileStatuses.NO_ACTIVE_BRANCH_FOUND;
            }

            if (!branchLookup.IsValid)
            {
                _logger.Error(fileComment = String.Format(BRANCH_NOT_VALID_FOR_ISSUER, record.BranchCode, record.IssuerId));
                return FileStatuses.BRANCH_PRODUCT_NOT_FOUND;
            }

            if (!branchLookup.isActive)
            {
                _logger.Error(fileComment = String.Format(BRANCH_NOT_ACTIVE, record.BranchCode, record.IssuerId));
                return FileStatuses.NO_ACTIVE_BRANCH_FOUND;
            }

            record.BranchId = branchLookup.BranchId;
            return FileStatuses.READ;
        }

        public override FileStatuses Validate(BulkRequestRecord record, long auditUserId, string auditWorkstation, out string fileComment)
        {
            fileComment = String.Empty;
            int issuerId = record.IssuerId ?? 0;
            BranchLookup branchLookup;


            if (!string.IsNullOrEmpty(record.BranchCode))
            {
                if (!_branches.TryGetValue(record.BranchCode, out branchLookup))
                {
                    branchLookup = _branchDAL.GetBranch(record.BranchCode, "", issuerId);
                    _branches.Add(record.BranchCode, branchLookup);
                }
            }
            else
            {
                if (!_branches.TryGetValue(issuerId.ToString(), out branchLookup))
                {
                    branchLookup = _branchDAL.GetBranchesForIssuer(issuerId);
                    _branches.Add(issuerId.ToString(), branchLookup);
                }
            }

            if (branchLookup == null)
            {
                _logger.Error(fileComment = String.Format(BRANCH_NOT_ACTIVE, record.BranchCode, record.IssuerId));
                return FileStatuses.NO_ACTIVE_BRANCH_FOUND;
            }

            if (!branchLookup.IsValid)
            {
                _logger.Error(fileComment = String.Format(BRANCH_NOT_VALID_FOR_ISSUER, record.BranchCode, record.IssuerId));
                return FileStatuses.BRANCH_PRODUCT_NOT_FOUND;
            }

            if (!branchLookup.isActive)
            {
                _logger.Error(fileComment = String.Format(BRANCH_NOT_ACTIVE, record.BranchCode, record.IssuerId));
                return FileStatuses.NO_ACTIVE_BRANCH_FOUND;
            }

            record.BranchId = branchLookup.BranchId;
            return FileStatuses.READ;
        }

        public override void Clear()
        {
            _branches.Clear();
        }
    }
}
