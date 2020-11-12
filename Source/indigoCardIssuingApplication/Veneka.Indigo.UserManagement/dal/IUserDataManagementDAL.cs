using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.IssuerManagement;

namespace Veneka.Indigo.UserManagement.dal
{
   public interface IUserDataManagementDAL
    {
        List<user_list_result> GetUserByBranch(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation);

        List<user_list_result> GetUserByBranchAdmin(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation);

        List<user_list_result> GetUnassignedUsers(int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation);

        login_user LogIn(string decryptedUserName, string decryptedPassword, string decryptedWorkstation);

        void FinaliseLogin(bool loginSuccess, long userId, string workStation);
        decrypted_user GetUserByUsername(string decryptedUserName);

        decrypted_user GetUserByUserId(long decryptedUserId);

        decrypted_user GetPendingUserByUserId(long decryptedUserId);

        List<user_list_result> GetUsersPendingForApproval(int? issuerId, int? branch_id, BranchStatus? branchStatus, int? userStatus, UserRole? userRole, string username, string firstName, string lastName, int? languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation);

        List<user_passwords> GetUserPasswords(long userId);
        AuthPasscodeResult GetUserAuthPin(string username, int branchId, long auditUserId, string auditWorkstation);
        LdapSettingResult GetLdapSetting(int ldapSettingId, long auditUserId, string auditWorkstation);

        SystemResponseCode CreateUser(user createUser, string encryptedPassword, List<int> userGroupList, long auditUserId, string auditWorkstation, out long? newUserId);

        SystemResponseCode UpdateAuthorisationPin(long userId, string authorisation_pin_number, long auditUserId, string auditWorkstation);

        SystemResponseCode UpdateUser(user updateUser, List<int> userGroupList, long auditUserId, string auditWorkstation);

        bool ApproveUser(long? pendingUserId, long audit_user_id, string audit_workstation);

        bool RejectUserRequest(long? pendingUserId, long audit_user_id, string audit_workstation);

        bool ResetUserPassword(long userId, string encryptedPassword, long audit_user_id, string audit_workstation);

        List<UserGroupAdminResult> GetUserGroupForUserAdmin(int? issuerId, int? userRole, long? userId, int? branchId);

        List<UserGroupAdminResult> GetUserGroupForPendingUserAdmin(int? issuerId, int? userRole, long? userId, int? branchId);

        List<GroupsRolesResult> GetGroupRolesForUser(long userId, int languageId, long auditUserId, string auditWorkstation);

        List<GroupsRolesResult> GetGroupRolesForPendingUser(long userId, int languageId, long auditUserId, string auditWorkstation);

        void FinaliseUserLogout(long userId, long audit_user_id, string audit_workstation);
       
        List<user_list_result> GetUserList(string username, string firstname, string lastname, string branchid, string userrole, int issuerid, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation);

        bool UpdateUserLanguage(long? userId, int languageId, string auditWorkstation);
        bool UpdateUseradminSettings(useradminsettingslist item, long? userId, string auditWorkstation);
        useradminsettingslist GetUseradminSettings(long? auditUserId, string auditWorkstation);
        List<LangLookup> GetLangUserRoles(int languageId, int? enterprise_only, long auditUserId, string auditWorkstation);

       bool CreateUseradminSettings(useradminsettingslist item, long? userId, string auditWorkstation, out int user_admin_Id);

    }
}
