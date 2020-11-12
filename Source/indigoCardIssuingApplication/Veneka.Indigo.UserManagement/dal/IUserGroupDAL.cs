using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.UserManagement.objects;

namespace Veneka.Indigo.UserManagement.dal
{
    public interface IUserGroupDAL
    {
         List<RolesIssuerResult> GetUserRoles(long userId);
        List<StatusFlowRole> getStatusFlowRoles(List<int> roleIds);

        List<IssuersForRoleResult> GetAllIssuers(long userId);

        SystemResponseCode CreateUserGroup(user_group userGroup, List<int> branchIdList, long auditUserId, string auditWorkstartion, out int userGroupId);

        SystemResponseCode UpdateUserGroup(user_group userGroup, List<int> branchIdList, long auditUserId, string auditWorkstartion);

        SystemResponseCode DeleteUserGroup(int userGroupId, long auditUserId, string auditWorkstation);

        List<BranchIdResult> GetBranchesForUserGroup(int userGroupId);

        user_group GetUserGroup(int userGroupId, long auditUserId, string auditWorkstation);

        List<UserGroupResult> GetUsergroups(int issuerId, UserRole? userRole, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation);

    }
}
