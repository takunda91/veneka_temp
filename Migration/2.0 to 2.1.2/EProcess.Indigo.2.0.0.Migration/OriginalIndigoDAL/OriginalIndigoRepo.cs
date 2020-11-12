using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using OriginalIndigoDAL.Models;
using EProcess.Indigo._2._0._0.Migration.Common.Utilities;
using EProcess.Indigo._2._0._0.Migration.Common.Models;
using EProcess.Indigo._2._0._0.Migration.Common.Reports;


namespace OriginalIndigoDAL
{
    public class OriginalIndigoRepo
    {
        #region Properties

        private OldIndigoDataModel _OriginalDataModel;
        public OldIndigoDataModel OriginalDataContext
        {
            get
            {
                if (_OriginalDataModel == null)
                    _OriginalDataModel = new OldIndigoDataModel();

                return _OriginalDataModel;
            }
        }


        private string _DatabaseName;

        public string DatabaseName
        {
            get
            {
                if (string.IsNullOrEmpty(_DatabaseName))
                {
                    _DatabaseName = this.OriginalDataContext.Database.Connection.Database;
                }

                return _DatabaseName;
            }
        }

        private string seedDirPath
        {
            get
            {
                var seedDirPath = string.Format("Logs/Seed/");

                return seedDirPath;
            }
        }

        #endregion

        public OriginalIndigoRepo()
        {
            this.OriginalDataContext.Configuration.LazyLoadingEnabled = true;
        }// end constructor OriginalIndigoRepo

        /// <summary>
        /// Returns a list of all issuers
        /// </summary>
        /// <returns>Whether the process is completed or not</returns>
        public IEnumerable<issuer> GetIssuers()
        {
            IEnumerable<issuer> issuers = null;

            try
            {
                var results = from iss in OriginalDataContext.issuer
                              select iss;

                issuers = results.ToList<issuer>();

            }// end try
            catch
            {
                throw;
            }// end catch

            return issuers;
        }// end method IEnumerable<issuer> GetIssuers()

        /// <summary>
        /// Disables all users for the selected issuer except user(s) who belong to the 'USER_ADMIN' user group
        /// </summary>
        /// <param name="issuerID">ID of the selected issuer</param>
        /// <returns>Whether the process is completed or not</returns>
        public bool DisableUsers(int issuerID)
        {
            var usersDisabled = false;

            try
            {
                var userGroups = new List<user_group>();
                var users = new List<user>();

                userGroups = OriginalDataContext.user_group
                                    .Include("user")
                                    .Where(usrGroup => usrGroup.issuer_id == issuerID)
                                    .ToList<user_group>();

                var exclud = "USER_ADMIN".ToLower();
                var disableGroups = userGroups
                                .Where(grp => !grp.user_group_name.ToLower().EndsWith(exclud))
                                .ToList<user_group>();


                var excludedUserIDs = string.Empty;

                foreach (var userGroup in disableGroups)
                {
                    foreach (var user in userGroup.user)
                    {
                        if (!string.IsNullOrEmpty(excludedUserIDs)) excludedUserIDs += ", ";
                        else excludedUserIDs += "(";

                        excludedUserIDs += user.user_id;
                    }// end foreach (var user in userGroup.user)
                }// end foreach(var userGroup in disableGroups)

                if (!string.IsNullOrEmpty(excludedUserIDs))
                {
                    excludedUserIDs += ")";

                    var sqlString = "UPDATE [dbo].[user]" +
                                    "SET [user_status_id] = 1 " +
                                    "WHERE [user_id] IN " + excludedUserIDs;

                    var dbResults = OriginalDataContext.Database.ExecuteSqlCommand(sqlString, new Object[] { });

                    usersDisabled = dbResults != 0;
                }// end if (!string.IsNullOrEmpty(excludedUserIDs))

            }// end try
            catch
            {
                throw;
            }// end catch

            return usersDisabled;
        }// end method bool DisableUsers(int issuerID)

        /// <summary>
        /// Migrates the selected issuer
        /// </summary>
        /// <param name="issuerID">ID of an issuer to be migrated</param>
        /// <returns>Whether the process is complete or not</returns>
        public bool MigrateIssuer(int issuerID = 3)
        {
            var done = false;

            //try
            //{
            //    string newDatabaseName = getNewDatabaseName();

            //    var sqlStr = string.Empty;
            //    var files = Directory.GetFiles(FilesUtils.dirPath + "Export");

            //    for (int fileCount = 1; fileCount < files.Length; fileCount++)
            //    {
            //        sqlStr = FilesUtils.GetSQLScript(fileCount < 10 ? ("0" + fileCount) : fileCount.ToString(), "Export", null, OriginalDataContext.Database.Connection.Database);

            //        var parameters = new object[] 
            //        {
            //            new SqlParameter("@selected_issuer_id", issuerID) 
            //        };

            //        if (sqlStr.Contains("DB_NAME"))
            //        {
            //            sqlStr =
            //                sqlStr.Replace("DB_NAME", newDatabaseName);
            //        }// end if (sqlString.Contains("DB_NAME"))

            //        var separateSQLBatches = sqlStr.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
            //        int dbResults = 0;

            //        if (separateSQLBatches != null && separateSQLBatches.Length > 1)
            //        {
            //            foreach (var sqlBatchString in separateSQLBatches)
            //            {
            //                dbResults =
            //                    OriginalDataContext.Database.ExecuteSqlCommand(sqlBatchString, parameters);
            //            }// end foreach (var sqlBatchString in separateSQLBatches)
            //        }// end if (separateSQLBatches != null && separateSQLBatches.Length > 0)
            //        else
            //        {
            //            dbResults =
            //                OriginalDataContext.Database.ExecuteSqlCommand(sqlStr, parameters);
            //        }// end else

            //        done = dbResults > 0;

            //        if (done)
            //            DisableUsers(issuerID);
            //    }
            //}// end try
            //catch
            //{
            //    throw;
            //}// end catch

            return done;
        }// end method bool MigrateIssuer(int issuerID)

        private string getNewDatabaseName()
        {
            var newDatabaseRepo = new NewIndigoDAL.NewIndigoRepo();

            return newDatabaseRepo.DatabaseName;
        }

        /// <summary>
        /// Backs up the old Indigo Database
        /// </summary>
        /// <returns>Whether the process is completed or not</returns>
        public bool BackupDatabase()
        {
            var isDone = false;

            //try
            //{

            //    var setupDir = FilesUtils.dirPath.Contains("Setup")
            //            ? FilesUtils.dirPath
            //            : FilesUtils.dirPath + "Setup/";

            //    var backupDirPath = setupDir.Replace("Scripts/", "") +
            //        "Output/Backups/" + DateTime.Now.ToString("dd-MMM-yy") + "/";

            //    if (Directory.Exists(setupDir))
            //    {
            //        var files = Directory.GetFiles(setupDir);
            //        var sqlString = FilesUtils.GetSQLScript("Backup.sql", "Setup", files);

            //        if (sqlString.Contains("DB_NAME"))
            //        {
            //            sqlString =
            //                sqlString.Replace("DB_NAME", OriginalDataContext.Database.Connection.Database);
            //        }// end if (sqlString.Contains("DB_NAME"))

            //        if (!Directory.Exists(backupDirPath))
            //            Directory.CreateDirectory(backupDirPath);

            //        var dbFileName = OriginalDataContext.Database.Connection.Database + " " +
            //            DateTime.Now.ToString("hh mm") + ".bak";

            //        backupDirPath = backupDirPath.Replace("Setup/", "");


            //        if (!Directory.Exists(backupDirPath))
            //            Directory.CreateDirectory(backupDirPath);

            //        if (sqlString.Contains("BACKUP_DIR_PATH"))
            //            sqlString = sqlString.Replace("BACKUP_DIR_PATH", backupDirPath + dbFileName);

            //        var results =
            //                    this.OriginalDataContext
            //                    .Database
            //                    .ExecuteSqlCommand
            //                    (
            //                        TransactionalBehavior.DoNotEnsureTransaction,
            //                        sqlString, new object[] { }
            //                    );

            //        isDone = results != 0;
            //    }// end if (Directory.Exists(setupDir))
            //    else
            //        throw new Exception("Scripts' Setup directory could not be located!");
            //}// end try
            //catch (Exception ex)
            //{
            //    throw;
            //}// end catch (Exception ex)

            return isDone;
        }// end method bool BackupDatabase()

        /// <summary>
        /// Returns a list of issuers that have not yet being migrated
        /// </summary>
        /// <param name="otherIssuers">issuer IDs of all migrated issuers</param>
        /// <returns>List of issuers that have not yet being migrated</returns>
        public List<issuer> GetOtherIssuers(int[] otherIssuers)
        {
            List<issuer> issuersList = null;

            try
            {
                if (otherIssuers != null && otherIssuers.Length > 0)
                {
                    var currentIssuers = "(";

                    foreach (var otherIssuer in otherIssuers)
                    {
                        currentIssuers += (otherIssuer + ",");
                    }// end foreach (var otherIssuer in otherIssuers)

                    currentIssuers = currentIssuers.Substring(0, currentIssuers.LastIndexOf(",")) + ")";

                    var sqlString = "SELECT * " +
                                    "FROM [issuer] " +
                                    "WHERE [issuer_id] NOT IN " + currentIssuers + " " +
                                    "  AND [issuer_id] > 0" +
                                    "  AND [issuer].[issuer_status_id] = 0";

                    var results = this.OriginalDataContext.Database.SqlQuery<issuer>(sqlString, new Object[] { });

                    if (results != null)
                    {
                        issuersList = results.ToList<issuer>();
                    }// end if (results != null)

                }// end if (otherIssuers != null && otherIssuers.Count > 0)
                else
                {
                    issuersList = this.OriginalDataContext
                    .issuer
                    .Where(iss => iss.issuer_id > 0)
                    .ToList<issuer>();
                }// end else

            }// end try
            catch (Exception ex)
            {
                throw;
            }// end catch (Exception ex)

            return issuersList;
        }// end method List<issuer> GetIssuers()

        /// <summary>
        /// Checks to ensure that the last logged seed values are the same as the current values
        /// </summary>
        /// <returns>Error message</returns>
        public string CompareSeedValues()
        {
            string error = string.Empty;

            try
            {
                var currentSeeds = GetCurrentSeed();
                var loggedSeeds = GetLastLoggedSeed();

                if (currentSeeds == null)
                    throw new Exception("currentSeeds is null.");

                if (loggedSeeds == null)
                    throw new Exception("loggedSeeds is null.");

                if (currentSeeds[0] != loggedSeeds[0])
                    error += string.Format("Users' seed value has changed from {0} to {1}.{2}"
                        , loggedSeeds[0], currentSeeds[0], Environment.NewLine);

                if (currentSeeds[1] != loggedSeeds[1])
                    error += string.Format("Issuers' seed value has changed from {0} to {1}.{2}"
                        , loggedSeeds[1], currentSeeds[1], Environment.NewLine);

                if (currentSeeds[2] != loggedSeeds[2])
                    error += string.Format("Branches' seed value has changed from {0} to {1}.{2}"
                        , loggedSeeds[2], currentSeeds[2], Environment.NewLine);

                if (currentSeeds[3] != loggedSeeds[3])
                    error += string.Format("User Groups' seed value has changed from {0} to {1}.{2}"
                        , loggedSeeds[3], currentSeeds[3], Environment.NewLine);

                if (currentSeeds[4] != loggedSeeds[4])
                    error += string.Format("Products' value seed has changed from {0} to {1}.{2}"
                        , loggedSeeds[4], currentSeeds[4], Environment.NewLine);

            }// end try
            catch
            {
                throw;
            }// end catch

            return error;
        }// end method string CompareSeedValues()

        /// <summary>
        /// Writes the current seed values to a file.
        /// </summary>
        /// <returns>Whether the process is completed or not</returns>
        public bool LogCurrentSeeds()
        {
            var isDone = false;

            try
            {
                var lastSeeds = GetCurrentSeed();

                long lastUserID = lastSeeds[0],
                     lastIssuerID = lastSeeds[1],
                     lastBranchID = lastSeeds[2],
                     lastUserGroupID = lastSeeds[3],
                     lastProductID = lastSeeds[4];

                var logMsg = string.Format("{0},{1},{2},{3},{4}", lastUserID, lastIssuerID, lastBranchID,
                        lastUserGroupID, lastProductID);

                var fileName = string.Format("{0}{1}", seedDirPath, DateTime.Now.ToString("dd-MMM-yy hh mm ss"));

                isDone = FilesUtils.CreateFile(fileName, logMsg, FileExtension.CSV);

            }// end try
            catch
            {
                throw;
            }// end catch

            return isDone;
        }// end method bool LogCurrentSeeds()

        /// <summary>
        /// Gets the current seed values for users, issuers, branches, user groups and products in the given order
        /// </summary>
        /// <returns>array containing current seed values</returns>
        public long[] GetCurrentSeed()
        {
            long[] response = null;

            try
            {

                var lastUser = this.OriginalDataContext.user
                        .OrderByDescending(u => u.user_id)
                        .Select(u => u)
                        .Take(1)
                        .ToList()[0];

                var lastIssuer = this.OriginalDataContext.issuer
                        .OrderByDescending(i => i.issuer_id)
                        .Select(i => i)
                        .Take(1)
                        .ToList()[0];

                var lastBranch = this.OriginalDataContext.branch
                        .OrderByDescending(b => b.branch_id)
                        .Select(b => b)
                        .Take(1)
                        .ToList()[0];

                var lastUserGroup = this.OriginalDataContext.user_group
                        .OrderByDescending(ug => ug.user_group_id)
                        .Select(ug => ug)
                        .Take(1)
                        .ToList()[0];

                var lastProduct = this.OriginalDataContext.issuer_product
                        .OrderByDescending(p => p.product_id)
                        .Select(p => p)
                        .Take(1)
                        .ToList()[0];

                response = new long[] 
                {
                    lastUser.user_id,
                    lastIssuer.issuer_id,
                    lastBranch.branch_id,
                    lastUserGroup.user_group_id,
                    lastProduct.product_id
                };
            }// end try
            catch
            {
                throw;
            }// end catch

            return response;
        }// end method long[] GetCurrentSeed()

        /// <summary>
        /// Retrieves the last stored seed values for users, issuers, branches, user groups and products in the given order
        /// </summary>
        /// <returns></returns>
        private long[] GetLastLoggedSeed()
        {

            long[] response = null;

            try
            {
                var baseDirPath = FilesUtils.dirPath.ToLower().Contains("script")
                          ? FilesUtils.dirPath.Replace("Scripts", "")
                          : FilesUtils.dirPath;

                var filesDirPath = string.Format("{0}Output/{1}", baseDirPath, seedDirPath);
                filesDirPath = filesDirPath.Replace("//", "/");

                var files = Directory.GetFiles(filesDirPath);

                if (files != null && files.Count() > 0)
                {

                    var logDirFiles = new DirectoryInfo(filesDirPath);

                    var lastLogFile = logDirFiles.GetFiles()
                            .OrderByDescending(f => f.LastWriteTime)
                            .First();

                    var fileName = lastLogFile.FullName;
                    
                    using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            var input = sr.ReadLine();
                            var values = input.Split(new char[] { ',' });

                            response = new long[]
                            {
                                long.Parse(values[0]),
                                long.Parse(values[1]),
                                long.Parse(values[2]),
                                long.Parse(values[3]),
                                long.Parse(values[4])
                            };
                        }// end using (var sr = new StreamReader(fs))
                    }// end using (var fs = new FileStream(lastLogFile, FileMode.Open, FileAccess.Read))

                }// end if (files != null && files.Count() > 0)
            }// end try
            catch
            {
                throw;
            }// end catch

            return response;
        }// end method long[] GetLastLoggedSeed()

        public OldIndigoDataModel New { get; set; }
    }
}
