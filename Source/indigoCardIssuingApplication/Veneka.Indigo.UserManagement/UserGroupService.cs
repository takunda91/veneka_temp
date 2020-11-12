using System.Collections.Generic;
using Veneka.Indigo.UserManagement.dal;
using Veneka.Indigo.UserManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Models;
using System;

namespace Veneka.Indigo.UserManagement
{
    public class UserGroupService
    {
        private readonly UserGroupDAL _userGrpDAL = new UserGroupDAL();
        private readonly ResponseTranslator _translator = new ResponseTranslator();

        /// <summary>
        /// Persist new user group to DB
        /// </summary>
        /// <param name="userGroupName"></param>
        /// <param name="role"></param>
        /// <param name="issuerId"></param>
        /// <param name="allBranchAccess"></param>
        /// <param name="branchIdList"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstartion"></param>
        /// <returns></returns>
        public bool CreateUserGroup(user_group userGroup, List<int> branchIdList, int language, long auditUserId, string auditWorkstation, out int userGroupId, out string responseMessage)
        {
            var response = _userGrpDAL.CreateUserGroup(userGroup, branchIdList, auditUserId, auditWorkstation, out userGroupId);
            responseMessage = _translator.TranslateResponseCode(response, 0, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Persist changes to user group to DB
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="userGroupName"></param>
        /// <param name="role"></param>
        /// <param name="issuerId"></param>
        /// <param name="allBranchAccess"></param>
        /// <param name="branchIdList"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstartion"></param>
        /// <returns></returns>
        public bool UpdateUserGroup(user_group userGroup, List<int> branchIdList, int language, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _userGrpDAL.UpdateUserGroup(userGroup, branchIdList, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, 0, language, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a user group.
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public bool DeleteUserGroup(int userGroupId, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _userGrpDAL.DeleteUserGroup(userGroupId, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, 0, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a list of branch id's associated with the user group.
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public List<BranchIdResult> GetBranchesForUserGroup(int userGroupId)
        {
            return _userGrpDAL.GetBranchesForUserGroup(userGroupId);
        }

        /// <summary>
        /// Get user groups linked to issuer and by user_role_id.
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="user_role_id">User role id, can be null to return all roles</param>
        /// <returns></returns>
        public List<UserGroupResult> GetUserGroups(int issuerID, UserRole? userRole, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            return _userGrpDAL.GetUsergroups(issuerID, userRole, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);
        }

        /// <summary>
        /// Get user group data.
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public user_group GetUserGroup(int userGroupId, long auditUserId, string auditWorkstation)
        {
            return _userGrpDAL.GetUserGroup(userGroupId, auditUserId, auditWorkstation);
        }
    }
}