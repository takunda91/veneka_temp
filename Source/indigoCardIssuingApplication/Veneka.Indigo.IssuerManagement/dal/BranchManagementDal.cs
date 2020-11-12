using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.IssuerManagement.objects;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common;
using System.Data.Objects;

namespace Veneka.Indigo.IssuerManagement.dal
{
    internal class BranchManagementDal
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        /// <summary>
        /// Persist new branch to the DB.
        /// </summary>
        /// <param name="createBranch"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal SystemResponseCode CreateBranch(branch createBranch,List<int> satellite_branches, long auditUserId, string auditWorkstation, out int  branchId)
        {
            ObjectParameter branchIdResult = new ObjectParameter("new_branch_id", typeof(int));
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            string branch_ids= satellite_branches!=null? String.Join(",", satellite_branches) :"";
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_insert_branch(createBranch.branch_status_id, createBranch.issuer_id, createBranch.branch_code, 
                                            createBranch.branch_name, createBranch.location, createBranch.contact_person,
                                            createBranch.contact_email, createBranch.card_centre,createBranch.emp_branch_code, branch_ids, createBranch.branch_type_id, createBranch.main_branch_id, auditUserId,
                                            auditWorkstation, branchIdResult, ResultCode);
            }

            branchId = int.Parse(branchIdResult.Value.ToString());
            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;            
        }

        /// <summary>
        /// Persist updates to branch to the DB.
        /// </summary>
        /// <param name="createBranch"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal SystemResponseCode UpdateBranch(branch updateBranch,List<int> satellite_branches, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            string branch_ids = satellite_branches != null ? String.Join(",", satellite_branches) : "";
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_branch(updateBranch.branch_id, updateBranch.branch_status_id, updateBranch.issuer_id, 
                                         updateBranch.branch_code, updateBranch.branch_name,  updateBranch.location, 
                                         updateBranch.contact_person, updateBranch.contact_email, updateBranch.card_centre,updateBranch.emp_branch_code, branch_ids,updateBranch.branch_type_id,updateBranch.main_branch_id,
                                         auditUserId, auditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Get's all branches for a user based on issuer and role of the user.
        /// </summary>
        /// <param name="issuer_id"></param>
        /// <param name="user_role_id"></param>
        /// <param name="decryptedUserName"></param>
        /// <returns></returns>
        internal List<BranchesResult> getBranchesForUser(int? issuer_id, long userId, int? user_role_id, int? branch_type_id, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_branches_for_user(issuer_id, userId, user_role_id, branch_type_id, languageId, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// Get's all branches for a user based on issuer and multiple user roles.
        /// </summary>
        /// <param name="issuer_id"></param>
        /// <param name="user_role_id"></param>
        /// <param name="decryptedUserName"></param>
        /// <returns></returns>
        internal List<BranchesResult> getBranchesForUserroles(int? issuer_id, long userId, List<int> userRolesList, bool? branchtypeId, int? languageId, long auditUserId, string auditWorkstation)
        {
            //using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            //{
            //    var results = context.usp_get_branches_for_userroles(issuer_id, userId, user_role_id1, user_role_id2, user_role_id3, user_role_id4, languageId,auditUserId, auditWorkstation);

            //    return results.ToList();
            //}

            List<BranchesResult> results = new List<BranchesResult>();

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_RolesList = new DataTable();
                    dt_RolesList.Columns.Add("key", typeof(long));
                    dt_RolesList.Columns.Add("value", typeof(string));
                    DataRow workRow;

                    foreach (var item in userRolesList)
                    {
                        workRow = dt_RolesList.NewRow();
                        workRow["key"] = item;
                        workRow["value"] = String.Empty;
                        dt_RolesList.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_branches_for_userroles]";

                    command.Parameters.Add("@issuer_id", SqlDbType.BigInt).Value = issuer_id;
                    command.Parameters.Add("@user_id", SqlDbType.Int).Value = userId;
                    command.Parameters.AddWithValue("@roles_list", dt_RolesList);
                    command.Parameters.Add("@branch_type_id", SqlDbType.Bit).Value = branchtypeId;
                    command.Parameters.Add("@languageId", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                BranchesResult result = new BranchesResult
                                {
                                    branch_code = dataReader["branch_code"].ToString(),
                                    branch_id = int.Parse(dataReader["branch_id"].ToString()),
                                    branch_name = dataReader["branch_name"].ToString(),
                                    branch_status = dataReader["branch_status"].ToString(),
                                    branch_status_id = int.Parse(dataReader["branch_status_id"].ToString()),
                                    issuer_id = int.Parse(dataReader["issuer_id"].ToString())
                                };

                                results.Add(result);
                            }
                        }
                    }
                }
            }

            return results;
        }
        /// <summary>
        /// Get's all branches for a user based on issuer and role of the user.
        /// </summary>
        /// <param name="issuer_id"></param>
        /// <param name="user_role_id"></param>
        /// <param name="decryptedUserName"></param>
        /// <returns></returns>
        internal List<BranchesResult> getBranchesForUserAdmin(int? issuer_id, int? branchstatusid, long userId, int? languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_branches_for_user_admin(issuer_id,branchstatusid, userId,languageId, auditUserId, auditWorkstation);

                return results.ToList();
            }
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
        internal List<BranchLoadCardCountResult> GetBranchesLoadCardCount(int issuerId, long userId, int userRoleId, int? loadCardStatusId, long auditUserId, string auditWorkstation)
        {
            List<BranchLoadCardCountResult> rtnValue = new List<BranchLoadCardCountResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<BranchLoadCardCountResult> results = context.usp_get_branches_with_load_card_count(issuerId, userId, userRoleId, loadCardStatusId, auditUserId, auditWorkstation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        internal List<branch> getBranchesForIssuer(int issuer_id, int? cardCentre, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_branches_for_issuer(issuer_id, cardCentre);

                return results.ToList();
            }
        }

        internal int GetBranchCardCount(int branchId, int productId, int? cardIssueMethidId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_card_centre_card_count(branchId, productId, cardIssueMethidId, auditUserId, auditWorkstation);

                return results.First().GetValueOrDefault(0);
            }            
        }
        internal int GetDistBatchCount(long  batchId, int branchId, int productId, int? cardIssueMethidId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_dist_batch_card_count(batchId, branchId, productId, cardIssueMethidId, auditUserId, auditWorkstation);

                return results.First().GetValueOrDefault(0);
            }
        }
        /// <summary>
        /// Get card count for branch
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="loadBatchStatusId">May be null to fetch cards in any status</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal int? GetBranchLoadCardCount(int branchId, int? loadBatchStatusId, long auditUserId, string auditWorkstation)
        {
            List<int?> rtnValue = new List<int?>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<int?> results = context.usp_get_branch_card_count(branchId, loadBatchStatusId, auditUserId, auditWorkstation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            if(rtnValue.Count > 0)
            {
                return rtnValue[0];
            }

            return null;
        }


        /// <summary>
        /// Returns a branch by its Id.
        /// </summary>
        /// <param name="branch_id"></param>
        /// <returns></returns>
        internal branch GetBranchById(int branch_id)
        {
            branch rtnValue = new branch();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<branch> results = context.usp_get_branch_by_id(branch_id);

                List<branch> branchesList = new List<branch>();
                foreach (branch result in results)
                {
                    branchesList.Add(result);
                }

                if (branchesList.Count > 1)
                {
                    throw new Exception("Duplicate branch found. More than one branch was returned, only one branch should have been returned.");
                }
                else if (branchesList.Count == 0)
                {
                    rtnValue = null;
                }
                else
                {
                    rtnValue = branchesList[0];
                }
            }

            return rtnValue;
        }

       
       

        /// <summary>
        /// Returns a list of branches based on branch name, branch code and issuer ID. All branch statuses are returned.
        /// </summary>
        /// <param name="branchName">May be null</param>
        /// <param name="branchCode">May be null</param>
        /// <param name="issuerID"></param>
        /// <returns></returns>
        internal List<branch> SearchBranch(string branchName, string branchCode, int issuerID)
        {
            return SearchBranch(branchName, branchCode, null, issuerID);
        }

        /// <summary>
        /// Returns a list of branches based on branch name, branch code, branch status and issuer id.
        /// </summary>
        /// <param name="branchName"></param>
        /// <param name="branchCode"></param>
        /// <param name="branchStatusId"></param>
        /// <param name="issuerID"></param>
        /// <returns></returns>
        internal List<branch> SearchBranch(string branchName, string branchCode, BranchStatus? branchStatus, int issuerID)
        {
            var rtrnList = new List<branch>();

            var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString);
            ObjectResult<branch> results = context.usp_search_branch(branchName, branchCode, (int)branchStatus, issuerID);

            foreach (branch result in results)
            {
                rtrnList.Add(result);
            }

            return rtrnList;
        }

        //public bool InsertBranch(Branch branch)
        //{
        //    int rows = 0;
        //    try
        //    {
        //        using (SqlConnection con = dbObject.SQLConnection)
        //        {
        //            string sql =
        //                "INSERT INTO branch( branch_code,branch_name,issuer_id,location,contact_person,contact_email,branch_status)" +
        //                " VALUES( '" + branch.BranchCode +
        //                "','" + branch.BranchName +
        //                "'," + branch.IssuerID +
        //                ",'" + branch.BranchLocation +
        //                "','" + branch.ContactPerson +
        //                "','" + branch.ContactEmailAddress +
        //                "','" + branch.Status.ToString() + "')";


        //            using (var command = new SqlCommand(sql, con))
        //            {
        //                rows = command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogFileWriter.WriteWebServerError(ToString(), ex);
        //        throw ex;
        //    }
        //    return rows > 0;
        //}

        //private static Branch ConstructBranchObject(SqlDataReader reader)
        //{
        //    string id = reader["branch_ID"] != null ? reader["branch_ID"].ToString() : "0";
        //    string code = reader["branch_code"] != null ? reader["branch_code"].ToString() : null;
        //    string name = reader["branch_name"] != null ? reader["branch_name"].ToString() : null;
        //    string issuerID = reader["issuer_id"] != null ? reader["issuer_id"].ToString() : "0";
        //    string cardsFileLoc = reader["location"] != null ? reader["location"].ToString() : null;
        //    string contactPerson = reader["contact_person"] != null ? reader["contact_person"].ToString() : null;
        //    string contactEmail = reader["contact_email"] != null ? reader["contact_email"].ToString() : null;
        //    string branchStatus = reader["branch_status"] != null ? reader["branch_status"].ToString() : null;

        //    int idBranch = 0;
        //    Int32.TryParse(id, out idBranch);

        //    int idIssuer = 0;
        //    Int32.TryParse(issuerID, out idIssuer);

        //    var branch = new Branch(idBranch, name, code, cardsFileLoc, contactPerson, contactEmail, idIssuer,
        //                            csGeneral.GetInstitutionStatus(branchStatus));
        //    return branch;
        //}

        //public List<Branch> GetActiveBranchesForIssuer(int issuerID)
        //{
        //    var branches = new List<Branch>();
        //    try
        //    {
        //        using (SqlConnection con = dbObject.SQLConnection)
        //        {
        //            string sql = "SELECT * FROM  branch "
        //                         + "WHERE  branch_status='" + InstitutionStatus.ACTIVE + "' "
        //                         + " AND issuer_id =" + issuerID;

        //            using (var command = new SqlCommand(sql, con))
        //            {
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    if (reader != null)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            Branch branch = ConstructBranchObject(reader);
        //                            branches.Add(branch);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogFileWriter.WriteWebServerError(ToString(), ex);
        //    }
        //    return branches;
        //}

        //public Branch GetBranch(int issuerID, string branchCode, string branchName)
        //{
        //    Branch branch = null;
        //    try
        //    {
        //        using (SqlConnection con = dbObject.SQLConnection)
        //        {
        //            using (SqlCommand command = con.CreateCommand())
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.CommandText = "usp_get_branch";
        //                command.Parameters.Add("@branchName", SqlDbType.VarChar).Value = branchName;
        //                command.Parameters.Add("@branchCode", SqlDbType.VarChar).Value = branchCode;
        //                command.Parameters.Add("@issuerID", SqlDbType.Int).Value = issuerID;

        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    if (reader != null && reader.HasRows)
        //                    {
        //                        reader.Read();
        //                        branch = ConstructBranchObject(reader);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogFileWriter.WriteWebServerError(ToString(), ex);
        //        throw ex;
        //    }
        //    return branch;
        //}

        //internal bool UpdateBranch(Branch branch)
        //{
        //    int updated = 0;
        //    try
        //    {
        //        using (SqlConnection con = dbObject.SQLConnection)
        //        {
        //            string sql = "UPDATE branch SET location ='" + branch.BranchLocation + "' ," +
        //                         " contact_person ='" + branch.ContactPerson + "' ," +
        //                         " contact_email ='" + branch.ContactEmailAddress + "'," +
        //                         " branch_status ='" + branch.Status.ToString() + "'" +
        //                         " WHERE issuer_id=  " + branch.IssuerID + " " +
        //                         " AND branch_code= '" + branch.BranchCode + "'";

        //            using (var command = new SqlCommand(sql, con))
        //            {
        //                updated = command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogFileWriter.WriteWebServerError(ToString(), ex);
        //        throw ex;
        //    }
        //    return updated > 0;
        //}

        
    }
}