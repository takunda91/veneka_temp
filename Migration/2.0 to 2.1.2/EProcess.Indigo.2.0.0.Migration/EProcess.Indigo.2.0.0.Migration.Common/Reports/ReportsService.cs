using EProcess.Indigo._2._0._0.Migration.Common.Models;
using EProcess.Indigo._2._0._0.Migration.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace EProcess.Indigo._2._0._0.Migration.Common.Reports
{
    public class ReportsService
    {

        private static string reportScriptPath
        {
            get
            {
                var baseDirPath = FilesUtils.dirPath;
                var path = baseDirPath + "Reports";

                return path;
            }
        }

        /// <summary>
        /// Generates migration reports for an issuer being  migrated
        /// </summary>
        /// <param name="selectedIssuer"></param>
        /// <returns></returns>
        public static bool GenerateReport(DbContext context, dynamic selectedIssuer, bool afterReportMigration = false)
        {
            var isDone = false;

            try
            {
                if (!Directory.Exists(reportScriptPath))
                    throw new Exception("Scripts' Reports directory could not be located!");

                var scriptFiles = Directory.GetFiles(reportScriptPath);

                for (int i = 0; i < scriptFiles.Length; i++)
                {
                    scriptFiles[i] = scriptFiles[i].Replace(@"\", "/");
                }// end for (int i = 0; i < scriptFiles.Length; i++)

                var cardsReport = GenerateCardsReport(context, scriptFiles, selectedIssuer);
                var branchesReport = GenerateBranchesReport(context, scriptFiles, selectedIssuer);
                var usersReport = GenerateUsersReport(context, scriptFiles, selectedIssuer);
                var userGroupsReport = GenerateUserGroupsReport(context, scriptFiles, selectedIssuer);

                var reportVersion = afterReportMigration ? "After" : "Original";

                var cardsReportName = string.Format("{0}{1}/{1} Cards Report {2}", @"/Reports/", selectedIssuer.issuer_name, reportVersion);
                var branchReportName = string.Format("{0}{1}/{1} Branches Report {2}", @"/Reports/", selectedIssuer.issuer_name, reportVersion);
                var usersReportName = string.Format("{0}{1}/{1} Users Report {2}", @"/Reports/", selectedIssuer.issuer_name, reportVersion);
                var usergroupsReportName = string.Format("{0}{1}/{1} User Groups Report {2}", @"/Reports/", selectedIssuer.issuer_name, reportVersion);

                isDone = FilesUtils.CreateFile(branchReportName, branchesReport, FileExtension.CSV, FileMode.Create);

                if (isDone)
                    isDone = FilesUtils.CreateFile(cardsReportName, cardsReport, FileExtension.CSV, FileMode.Create);

                if (isDone)
                    isDone = FilesUtils.CreateFile(usersReportName, usersReport, FileExtension.CSV, FileMode.Create);

                if (isDone)
                    isDone = FilesUtils.CreateFile(usergroupsReportName, userGroupsReport, FileExtension.CSV, FileMode.Create);

            }// end try
            catch
            {
                throw;
            }// end catch

            return isDone;
        }// end method bool GenerateReport(issuer theIssuer)


        /// <summary>
        /// Returns a comma seperated string of cards to be outputted to a file
        /// </summary>
        /// <param name="scriptFiles"></param>
        /// <param name="selectedIssuer"></param>
        /// <returns></returns>
        private static string GenerateCardsReport(DbContext context, string[] scriptFiles, dynamic selectedIssuer)
        {
            var cardsReport = string.Empty;
            try
            {
                var fileName = "get cards." + FileExtension.SQL.ToString().ToLower();

                var scriptFile = scriptFiles
                        .SingleOrDefault(f => f.ToLower().EndsWith(fileName));

                if (scriptFile == null)
                    throw new Exception("Could not locate " + fileName);

                var sqlString = File.ReadAllText(scriptFile);

                if (sqlString.Contains("DB_NAME"))
                    sqlString = sqlString.Replace("DB_NAME", context.Database.Connection.Database);

                var parameters = new object[] 
                            {
                                new SqlParameter("@selected_issuer_id", selectedIssuer.issuer_id) 
                            };

                var results = context.Database.SqlQuery<Card>
                    (
                        sqlString,
                        parameters
                    )
                    .ToList();

                cardsReport = string.Format("Card ID,Card Number,Branch,Product{0}", Environment.NewLine);
                foreach (var card in results)
                {
                    cardsReport += string.Format("{0},{1},{2},{3}{4}", card.card_id,
                        card.card_number, card.branch_name, card.product_name, Environment.NewLine);
                }// end foreach (var card in results)

            }// end try
            catch
            {
                throw;
            }// end catch

            return cardsReport;
        }// end method  string GenerateCardsReport(string[] scriptFiles)

        
        /// <summary>
        /// Returns a comma seperated string of branches to be outputted to a file
        /// </summary>
        /// <param name="scriptFiles"></param>
        /// <param name="selectedIssuer"></param>
        /// <returns></returns>
        private static string GenerateBranchesReport(DbContext context, string[] scriptFiles, dynamic selectedIssuer)
        {
            var branchesReport = string.Empty;

            try
            {
                var fileName = "get branches." + FileExtension.SQL.ToString().ToLower();
                var scriptFile = scriptFiles
                        .SingleOrDefault(f => f.ToLower().EndsWith(fileName));

                if (scriptFile == null)
                    throw new Exception("Could not locate " + fileName);

                var sqlString = File.ReadAllText(scriptFile);

                if (sqlString.Contains("DB_NAME"))
                    sqlString = sqlString.Replace("DB_NAME", context.Database.Connection.Database);

                var parameters = new object[] 
                            {
                                new SqlParameter("@selected_issuer_id", selectedIssuer.issuer_id) 
                            };

                var results = context.Database.SqlQuery<Branch>
                    (
                        sqlString,
                        parameters
                    )
                    .ToList();


                branchesReport += "Branch Name,Branch Code,Contact Person,Contact email" + Environment.NewLine;
                foreach (var branch in results)
                {
                    branchesReport += string.Format("{0},{1},{2},{3}{4}",
                            branch.branch_name, branch.branch_code, branch.contact_person, branch.contact_email,
                            Environment.NewLine);
                }// end foreach (var bra in branches)

            }// end try
            catch
            {   
                throw;
            }// end catch

            return branchesReport;
        }// end method string GenerateBranchesReport(string[] scriptFiles, issuer selectedIssuer)
        
        /// <summary>
        /// Returns a comma seperated string of users to be outputted to a file
        /// </summary>
        /// <param name="scriptFiles"></param>
        /// <param name="selectedIssuer"></param>
        /// <returns></returns>
        private static string GenerateUsersReport(DbContext context, string[] scriptFiles, dynamic selectedIssuer)
        {

            var usersReport = string.Empty;
            try
            {
                var fileName = "get users." + FileExtension.SQL.ToString().ToLower();

                var scriptFile = scriptFiles
                        .SingleOrDefault(f => f.ToLower().EndsWith(fileName));

                if (scriptFile == null)
                    throw new Exception("Could not locate " + fileName);

                var sqlString = File.ReadAllText(scriptFile);

                if (sqlString.Contains("DB_NAME"))
                    sqlString = sqlString.Replace("DB_NAME", context.Database.Connection.Database);

                var parameters = new object[] 
                            {
                                new SqlParameter("@selected_issuer_id", selectedIssuer.issuer_id) 
                            };

                var results = context.Database.SqlQuery<User>
                    (
                        sqlString,
                        parameters
                    )
                    .ToList();

                usersReport = string.Format("User ID,Username,First Name,Last Name,Current Status,Gender,Email,Last Login Date,Last Login Attempt,Workstation{0}", Environment.NewLine);
                foreach (var user in results)
                {
                    usersReport += string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}{10}", 
                        user.user_id, user.username, user.first_name, user.last_name, user.user_status_value, user.user_gender_value, user.user_email,
                        user.last_login_date.HasValue ? user.last_login_date.Value.ToString("yyyy-MMM-dd hh:mm:ss") : null,
                        user.last_login_attempt.HasValue ? user.last_login_attempt.Value.ToString("yyyy-MMM-dd hh:mm:ss") : null, 
                        user.workstation, Environment.NewLine);
                }// end foreach (var card in results)

            }// end try
            catch
            {
                throw;
            }// end catch

            return usersReport;
        }// end method string GenerateUsersReport(string[] scriptFiles, issuer selectedIssuer)


        /// <summary>
        /// Returns a comma seperated string of user groups to be outputted to a file
        /// </summary>
        /// <param name="scriptFiles"></param>
        /// <param name="selectedIssuer"></param>
        /// <returns></returns>
        private static string GenerateUserGroupsReport(DbContext context, string[] scriptFiles, dynamic selectedIssuer)
        {
            var usergroupsReport = string.Empty;

            try
            {

                var fileName = "get user groups." + FileExtension.SQL.ToString().ToLower();
                var scriptFile = scriptFiles
                        .SingleOrDefault(f => f.ToLower().EndsWith(fileName));

                if (scriptFile == null)
                    throw new Exception("Could not locate " + fileName);

                var sqlString = File.ReadAllText(scriptFile);

                if (sqlString.Contains("DB_NAME"))
                    sqlString = sqlString.Replace("DB_NAME", context.Database.Connection.Database);

                var parameters = new object[] 
                            {
                                new SqlParameter("@selected_issuer_id", selectedIssuer.issuer_id) 
                            };

                var results = context.Database.SqlQuery<Groups>
                    (
                        sqlString,
                        parameters
                    )
                    .ToList();


                usergroupsReport += "User Group ID,User Group Name,Can Create,Can Read,Can Update,Can Delete,All Branch Access" + Environment.NewLine;
                foreach (var ug in results)
                {
                    usergroupsReport += string.Format("{0},{1},{2},{3},{4},{5},{6}{7}",
                        ug.user_group_id, ug.user_group_name, ug.can_create ? "Yes" : "No", ug.can_read ? "Yes" : "No", ug.can_update ? "Yes" : "No",
                        ug.can_delete  ? "Yes" : "No", ug.all_branch_access  ? "Yes" : "No", Environment.NewLine);
                }// end foreach (var bra in branches)

            }// end try
            catch
            {
                throw;
            }// end catch

            return usergroupsReport;
        }// end method string GenerateBranchesReport(string[] scriptFiles, issuer selectedIssuer)

    }
}
