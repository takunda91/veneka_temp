using OriginalIndigoDAT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace OriginalIndigoDAL
{
    public class OriginalIndigoRepo
    {

        private OriginalDataModel originalDataModel;
        private OriginalDataModel originalDataContext
        {
            get
            {
                if (originalDataModel == null)
                    originalDataModel = new OriginalDataModel();

                return originalDataModel;
            }
        }

        public OriginalIndigoRepo()
        {
            this.originalDataContext.Configuration.LazyLoadingEnabled = true;
        }// end constructor OriginalIndigoRepo

        public IEnumerable<issuer> GetIssuers()
        {
            IEnumerable<issuer> issuers = null;

            try
            {
                var results = from iss in originalDataContext.issuer
                              select iss;

                issuers = results.ToList<issuer>();

            }// end try
            catch
            {
                throw;
            }// end catch

            return issuers;
        }// end method IEnumerable<issuer> GetIssuers()

        public bool DisableUsers(int issuerID)
        {
            var usersDisabled = false;

            try
            {
                var userGroups = new List<user_group>();
                var users = new List<user>();

                userGroups = originalDataContext.user_group
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

                    var dbResults = originalDataContext.Database.ExecuteSqlCommand(sqlString, new Object[] { });

                    usersDisabled = dbResults != 0;
                }// end if (!string.IsNullOrEmpty(excludedUserIDs))

            }// end try
            catch
            {
                throw;
            }// end catch

            return usersDisabled;
        }// end method bool DisableUsers(int issuerID)

        public bool MigrateIssuer(int issuerID = 3)
        {
            var done = false;

            try
            {
                var sqlStr = string.Empty;

                for (int fileCount = 1; fileCount < 5; fileCount++)
                {
                    sqlStr = null;//FilesUtils.GetSQLScript("0" + fileCount);

                    var parameters = new object[] 
                    {
                        new SqlParameter("@selected_issuer_id", issuerID) 
                    };

                    var dbResults =
                        originalDataContext.Database.ExecuteSqlCommand(sqlStr, parameters);

                    done = dbResults > 0;
                }
            }// end try
            catch
            {
                throw;
            }// end catch

            return done;
        }// end method bool MigrateIssuer(int issuerID)

        public bool SetupNewDB()
        {
            var isDone = false;

            try
            {

            }// end try
            catch
            {   
                throw;
            }// end catch

            return isDone;
        }// end method bool SetupNewDB()

    }
}
