using System;
using System.Collections.Generic;
using Veneka.Indigo.IssuerManagement.dal;
using Veneka.Indigo.IssuerManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Exceptions;

namespace Veneka.Indigo.IssuerManagement
{
    public class BranchService
    {
        private readonly BranchManagementDal _branchDAL = new BranchManagementDal();
        private readonly ResponseTranslator _translator = new ResponseTranslator();

        /// <summary>
        /// Persist new branch to the DB.
        /// </summary>
        /// <param name="createBranch"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool CreateBranch(branch createBranch, List<int> satellite_branches,int language, long auditUserId, string auditWorkstation, out int branchId, out string responseMessage)
        {
            var response = _branchDAL.CreateBranch(createBranch, satellite_branches, auditUserId, auditWorkstation, out branchId);
            responseMessage = _translator.TranslateResponseCode(response, 0, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Persist updates to branch to the DB.
        /// </summary>
        /// <param name="createBranch"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public bool UpdateBranch(branch updateBranch, List<int> satellite_branches, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _branchDAL.UpdateBranch(updateBranch, satellite_branches, auditUserId, auditWorkstation);

            responseMessage = _translator.TranslateResponseCode(response, 0, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public List<branch> SearchBranches(int issuerID, string branchCode, string branchName)
        {
            return _branchDAL.SearchBranch(branchName, branchCode, issuerID);
        }

        public List<branch> getBranchesForIssuer(int issuerId, int? cardCentre, int languageId, long auditUserId, string auditWorkstation)
        {
            return _branchDAL.getBranchesForIssuer(issuerId, cardCentre, languageId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Get's all the branches for a user based on issuer, role.
        /// </summary>
        /// <param name="issuer_id"></param>
        /// <param name="user_role_id"></param>
        /// <param name="decryptedUsername"></param>
        /// <returns></returns>
        public List<BranchesResult> getAllBranchesForUser(int? issuer_id, long userId, int? user_role_id, int? cardCentreBranchYN, int languageId, long auditUserId, string auditWorkstation)
        {
            return _branchDAL.getBranchesForUser(issuer_id, userId, user_role_id, cardCentreBranchYN, languageId, auditUserId, auditWorkstation);
        }
        /// <summary>
        /// Get's all the branches for a user based on issuer, role.
        /// </summary>
        /// <param name="issuer_id"></param>
        /// <param name="user_role_id"></param>
        /// <param name="decryptedUsername"></param>
        /// <returns></returns>
        public List<BranchesResult> getBranchesForUserroles(int? issuer_id, long userId, List<int> userRolesList, bool? branch_type_id, int languageId, long auditUserId, string auditWorkstation)
        {
            return _branchDAL.getBranchesForUserroles(issuer_id, userId, userRolesList, branch_type_id, languageId, auditUserId, auditWorkstation);
        }
        /// <summary>
        /// Get's all the branches for a user based on issuer, role.
        /// </summary>
        /// <param name="issuer_id"></param>
        /// <param name="user_role_id"></param>
        /// <param name="decryptedUsername"></param>
        /// <returns></returns>
        public List<BranchesResult> getAllBranchesForUserAdmin(int? issuer_id, int? branchstatusid, long userId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _branchDAL.getBranchesForUserAdmin(issuer_id, branchstatusid, userId, languageId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Retrieve a list of branches as well as the number of cards they have according to the load batch status.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="userId"></param>
        /// <param name="userRoleId"></param>
        /// <param name="loadBatchStatusId">May be null to fetch cards in any status</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<BranchLoadCardCountResult> GetBranchesLoadCardCount(int issuerId, long userId, int userRoleId, int? loadBatchStatusId, long auditUserId, string auditWorkstation)
        {
            return _branchDAL.GetBranchesLoadCardCount(issuerId, userId, userRoleId, loadBatchStatusId, auditUserId, auditWorkstation);
        }

        public int GetBranchCardCount(int branchId, int productId, int? cardIssueMethidId, long auditUserId, string auditWorkstation)
        {
            return _branchDAL.GetBranchCardCount(branchId, productId, cardIssueMethidId, auditUserId, auditWorkstation);
        }

        public int GetDistBatchCount(long batchid, int branchId, int productId, int? cardIssueMethidId, long auditUserId, string auditWorkstation)
        {
            return _branchDAL.GetDistBatchCount(batchid,branchId,productId, cardIssueMethidId, auditUserId, auditWorkstation);
        }
        /// <summary>
        /// Get card count for branch
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="loadBatchStatusId">May be null to fetch cards in any status</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public int? GetBranchLoadCardCount(int branchId, int? loadBatchStatusId, long auditUserId, string auditWorkstation)
        {
            return _branchDAL.GetBranchLoadCardCount(branchId, loadBatchStatusId, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Returns a branch by its Id.
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public branch getBranchById(int branchId)
        {
            return _branchDAL.GetBranchById(branchId);
        }

        /// <summary>
        /// Get a branch by branch code.
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="branchCode"></param>
        /// <param name="branchStatus"></param>
        /// <returns></returns>
        public branch GetBranchByBranchCode(int issuerID, string branchCode, BranchStatus? branchStatus)
        {
            List<branch> branches = _branchDAL.SearchBranch(null, branchCode, branchStatus, issuerID);
            if (branches != null && branches.Count > 0)
                return branches[0];

            return null;
        }

        private string DecodeDBResponse(DBResponseMessage dbResponse)
        {
            switch (dbResponse)
            {
                case DBResponseMessage.SUCCESS:
                    return "";
                case DBResponseMessage.INCORRECT_STATUS:
                    return dbResponse.ToString();
                case DBResponseMessage.CARD_ALREADY_ISSUED:
                    return dbResponse.ToString();
                case DBResponseMessage.INCORRECT_BRANCH:
                    return dbResponse.ToString();
                case DBResponseMessage.NO_RECORDS_RETURNED:
                    return dbResponse.ToString();
                case DBResponseMessage.DUPLICATE_RECORD:
                    return "Duplicate branch code, please use another branch code.";
                case DBResponseMessage.SPROC_ERROR:
                    return dbResponse.ToString();
                case DBResponseMessage.SYSTEM_ERROR:
                    return dbResponse.ToString();
                case DBResponseMessage.FAILURE:
                    return dbResponse.ToString();
                default:
                    return dbResponse.ToString();
            }
        }
    }
}