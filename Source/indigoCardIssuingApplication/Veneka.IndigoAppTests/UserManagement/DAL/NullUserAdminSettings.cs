using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.IssuerManagement;
using Veneka.Indigo.UserManagement;
using Veneka.Indigo.UserManagement.dal;

namespace Veneka.IndigoAppTests.UserManagement.DAL
{
    public class NullUserAdminSettings : IUserDataManagementDAL
    {
        public bool ApproveUser(long? pendingUserId, long audit_user_id, string audit_workstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode CreateUser(user createUser, string encryptedPassword, List<int> userGroupList, long auditUserId, string auditWorkstation, out long? newUserId)
        {
            throw new NotImplementedException();
        }

        public bool CreateUseradminSettings(useradminsettingslist item, long? userId, string auditWorkstation, out int user_admin_Id)
        {
            throw new NotImplementedException();
        }

        public void FinaliseLogin(bool loginSuccess, long userId, string workStation)
        {
            throw new NotImplementedException();
        }

        public void FinaliseUserLogout(long userId, long audit_user_id, string audit_workstation)
        {
            throw new NotImplementedException();
        }

        public List<GroupsRolesResult> GetGroupRolesForPendingUser(long userId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<GroupsRolesResult> GetGroupRolesForUser(long userId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<LangLookup> GetLangUserRoles(int languageId, int? enterprise_only, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public LdapSettingResult GetLdapSetting(int ldapSettingId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public decrypted_user GetPendingUserByUserId(long decryptedUserId)
        {
            throw new NotImplementedException();
        }

        public List<user_list_result> GetUnassignedUsers(int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            throw new NotImplementedException();
        }

        public useradminsettingslist GetUseradminSettings(long? auditUserId, string auditWorkstation)
        {
            return null;
        }

        public AuthPasscodeResult GetUserAuthPin(string username, int branchId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<user_list_result> GetUserByBranch(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            throw new NotImplementedException();
        }

        public List<user_list_result> GetUserByBranchAdmin(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            throw new NotImplementedException();
        }

        public decrypted_user GetUserByUserId(long decryptedUserId)
        {
            throw new NotImplementedException();
        }

        public decrypted_user GetUserByUsername(string decryptedUserName)
        {
            throw new NotImplementedException();
        }

        public List<UserGroupAdminResult> GetUserGroupForPendingUserAdmin(int? issuerId, int? userRole, long? userId, int? branchId)
        {
            throw new NotImplementedException();
        }

        public List<UserGroupAdminResult> GetUserGroupForUserAdmin(int? issuerId, int? userRole, long? userId, int? branchId)
        {
            throw new NotImplementedException();
        }

        public List<user_list_result> GetUserList(string username, string firstname, string lastname, string branchid, string userrole, int issuerid, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            throw new NotImplementedException();
        }

        public List<user_passwords> GetUserPasswords(long userId)
        {
            throw new NotImplementedException();
        }

        public List<user_list_result> GetUsersPendingForApproval(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            throw new NotImplementedException();
        }

        public login_user LogIn(string decryptedUserName, string decryptedPassword, string decryptedWorkstation)
        {
            throw new NotImplementedException();
        }

        public bool RejectUserRequest(long? pendingUserId, long audit_user_id, string audit_workstation)
        {
            throw new NotImplementedException();
        }

        public bool ResetUserPassword(long userId, string encryptedPassword, long audit_user_id, string audit_workstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode UpdateAuthorisationPin(long userId, string authorisation_pin_number, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public SystemResponseCode UpdateUser(user updateUser, List<int> userGroupList, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUseradminSettings(useradminsettingslist item, long? userId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUserLanguage(long? userId, int languageId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }
    }
}
