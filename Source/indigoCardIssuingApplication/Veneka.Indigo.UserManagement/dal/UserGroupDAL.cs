using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;
using System.Data.Objects;
using Veneka.Indigo.UserManagement.objects;

namespace Veneka.Indigo.UserManagement.dal
{
    internal class UserGroupDAL : IUserGroupDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        /// <summary>
        /// Calls the DB and returns a list of roles and the issuers linked to the roles for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<RolesIssuerResult> GetUserRoles(long userId)
        {
            List<RolesIssuerResult> rtnValue = new List<RolesIssuerResult>(); 

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<RolesIssuerResult> results = context.usp_get_user_roles_for_user(userId);         
                rtnValue = results.ToList();                                
            }

            return rtnValue;
        }

        public List<StatusFlowRole> getStatusFlowRoles(List<int> roleIds)
        {
            List<StatusFlowRole> rtn = new List<StatusFlowRole>();

            DataTable dt_BranchList = new DataTable();
            dt_BranchList.Columns.Add("key", typeof(int));
            dt_BranchList.Columns.Add("value", typeof(string));
            DataRow workRow;

            foreach (var item in roleIds)
            {
                workRow = dt_BranchList.NewRow();
                workRow["key"] = item;
                workRow["value"] = "";
                dt_BranchList.Rows.Add(workRow);
            }

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_status_flow_roles]";

                    command.Parameters.AddWithValue("@role_list", dt_BranchList);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = -2;
                    command.Parameters.Add("@audit_workStation", SqlDbType.VarChar).Value = "SYSTEM";

                    var reader = command.ExecuteReader();

                    StatusFlowRole item;

                    while (reader.Read())
                    {
                        //Hack
                        if (reader["main_menu_id"] != null)
                        {

                            item = new StatusFlowRole
                            {
                                RoleId = short.Parse(reader["user_role_id"].ToString()),
                                DistBatchStatusId = short.Parse(reader["dist_batch_statuses_id"].ToString()),
                                FlowDistBatchStatusId = short.Parse(reader["flow_dist_batch_statuses_id"].ToString()),
                                CardIssueMethodId = short.Parse(reader["card_issue_method_id"].ToString()),
                                DistBatchTypeId = short.Parse(reader["dist_batch_type_id"].ToString()),
                                MainMenuId = short.Parse(reader["main_menu_id"].ToString()),
                                SubMenuId = reader["sub_menu_id"] != DBNull.Value ? short.Parse(reader["sub_menu_id"].ToString()) : short.Parse(reader["flow_dist_batch_statuses_id"].ToString()),
                                OrderId = short.Parse(reader["sub_menu_order"].ToString())
                            };

                            rtn.Add(item);
                        }
                            
                        //    (0,
                        //                    reader["path"].ToString(),
                        //                    reader["address"].ToString(),
                        //                    reader["port"] != null && !string.IsNullOrWhiteSpace(reader["port"].ToString()) ? int.Parse(reader["port"].ToString()) : 0,
                        //                    reader["header_length"] != null && !string.IsNullOrWhiteSpace(reader["header_length"].ToString()) ? int.Parse(reader["header_length"].ToString()) : 0,
                        //                    reader["identification"].ToString());
                        //return p;
                    }
                }
            }

            return rtn;
        }

        /// <summary>
        /// Fetch all issuers, method used when gathering roles information.
        /// bit of a dirty fix.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<IssuersForRoleResult> GetAllIssuers(long userId)
        {
            List<IssuersForRoleResult> rtnValue = new List<IssuersForRoleResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_all_issuers_for_role(userId, "");
                rtnValue = results.ToList();
            }

            return rtnValue;
        }

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
        public SystemResponseCode CreateUserGroup(user_group userGroup, List<int> branchIdList, long auditUserId, string auditWorkstartion, out int userGroupId)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_BranchList = new DataTable();
                    dt_BranchList.Columns.Add("branch_id", typeof(int));
                    DataRow workRow;

                    foreach (var item in branchIdList)
                    {
                        workRow = dt_BranchList.NewRow();
                        workRow["branch_id"] = item;
                        dt_BranchList.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_create_user_group]";

                    command.Parameters.Add("@user_group_name", SqlDbType.VarChar).Value = userGroup.user_group_name;
                    command.Parameters.Add("@user_role_id", SqlDbType.Int).Value = userGroup.user_role_id;
                    command.Parameters.Add("@can_read", SqlDbType.Bit).Value = userGroup.can_read;
                    command.Parameters.Add("@can_update", SqlDbType.Bit).Value = userGroup.can_update;
                    command.Parameters.Add("@can_create", SqlDbType.Bit).Value = userGroup.can_create;
                    command.Parameters.Add("@mask_screen_pan", SqlDbType.Bit).Value = userGroup.mask_screen_pan;
                    command.Parameters.Add("@mask_report_pan", SqlDbType.Bit).Value = userGroup.mask_report_pan;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = userGroup.issuer_id;
                    command.Parameters.Add("@all_branch_access", SqlDbType.Bit).Value = userGroup.all_branch_access;
                    command.Parameters.AddWithValue("@branch_list", dt_BranchList);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstartion;
                    command.Parameters.Add("@new_user_group_id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    userGroupId = int.Parse(command.Parameters["@new_user_group_id"].Value.ToString());
                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());                 
                }
            }

            return (SystemResponseCode)resultCode;
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
        public SystemResponseCode UpdateUserGroup(user_group userGroup, List<int> branchIdList, long auditUserId, string auditWorkstartion)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_BranchList = new DataTable();
                    dt_BranchList.Columns.Add("branch_id", typeof(int));
                    DataRow workRow;

                    foreach (var item in branchIdList)
                    {
                        workRow = dt_BranchList.NewRow();
                        workRow["branch_id"] = item;
                        dt_BranchList.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_user_group]";

                    command.Parameters.Add("@user_group_id", SqlDbType.Int).Value = userGroup.user_group_id;
                    command.Parameters.Add("@user_group_name", SqlDbType.VarChar).Value = userGroup.user_group_name;
                    command.Parameters.Add("@user_role_id", SqlDbType.Int).Value = userGroup.user_role_id;
                    command.Parameters.Add("@can_read", SqlDbType.Bit).Value = userGroup.can_read;
                    command.Parameters.Add("@can_update", SqlDbType.Bit).Value = userGroup.can_update;
                    command.Parameters.Add("@can_create", SqlDbType.Bit).Value = userGroup.can_create;
                    command.Parameters.Add("@mask_screen_pan", SqlDbType.Bit).Value = userGroup.mask_screen_pan;
                    command.Parameters.Add("@mask_report_pan", SqlDbType.Bit).Value = userGroup.mask_report_pan;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = userGroup.issuer_id;
                    command.Parameters.Add("@all_branch_access", SqlDbType.Bit).Value = userGroup.all_branch_access;
                    command.Parameters.AddWithValue("@branch_list", dt_BranchList);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstartion;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());    
                }
            }

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Delete a user group.
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public SystemResponseCode DeleteUserGroup(int userGroupId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_delete_user_group(userGroupId, auditUserId, auditWorkstation, ResultCode);                
            }

            return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
        }

        /// <summary>
        /// Returns a list of branch id's associated with the user group.
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        public List<BranchIdResult> GetBranchesForUserGroup(int userGroupId)
        {
            List<BranchIdResult> rtnValue = new List<BranchIdResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_branches_for_user_group(userGroupId);
                rtnValue = results.ToList();
            }

            return rtnValue;
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
            List<user_group> rtnValue = new List<user_group>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_usergroup(userGroupId, auditUserId, auditWorkstation);

                rtnValue = results.ToList();
            }

            if (rtnValue.Count > 0)
            {
                return rtnValue[0];
            }

            return null;
        }

        /// <summary>
        /// Get user groups linked to issuer and by user_role_id.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="user_role_id">User role id, can be null to return all roles</param>
        /// <returns></returns>
        public List<UserGroupResult> GetUsergroups(int issuerId, UserRole? userRole, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            List<UserGroupResult> rtnValue = new List<UserGroupResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<UserGroupResult> results = context.usp_get_user_groups_by_issuer(issuerId, (int?)userRole, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);                

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;            
        }
    }
}