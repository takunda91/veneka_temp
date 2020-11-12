using EProcess.Indigo._2._0._0.Migration.Common.Utilities;
using NewIndigoDAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Diagnostics;

namespace NewIndigoDAL
{
    public class NewIndigoRepo
    {
        #region Properties

        private NewIndigoDataModel newIndigoDataModel;

        public NewIndigoDataModel NewIndigoDataModel
        {
            get
            {
                if (newIndigoDataModel == null)
                {
                    newIndigoDataModel = new NewIndigoDataModel();
                }

                return newIndigoDataModel;
            }
            set { newIndigoDataModel = value; }
        }


        private string _DatabaseName;

        public string DatabaseName
        {
            get
            {
                if (string.IsNullOrEmpty(_DatabaseName))
                {
                    _DatabaseName = this.NewIndigoDataModel.Database.Connection.Database;
                }

                return _DatabaseName;
            }
        }


        #endregion

        #region Public Methods

        public NewIndigoRepo()
        {
            try
            {

                this.NewIndigoDataModel.Configuration.LazyLoadingEnabled = true;
            }
            catch
            {
                throw;
            }
        }

        public bool DatabaseExists()
        {
            var exists = false;

            try
            {
                var test = from ldap in this.NewIndigoDataModel.ldap_setting
                           select ldap;

                var results = test != null
                          ? test.ToList<ldap_setting>()
                          : null;

                exists = results != null;

            }
            catch
            {
                throw;
            }

            return exists;
        }// end method bool DatabaseExists()

        public bool SetupDatabase(string masterKey)
        {
            var isDone = false;

            var scripts = FilesUtils.GetSQLScript("Create");

            if (scripts.Count <= 0)
                throw new Exception("No setup scripts in directory.");

            var keys = scripts.Keys.ToList();
            keys.Sort();

            using (SqlConnection conn = new SqlConnection("data source=veneka-dev;integrated security=True;App=EntityFramework"))
            {
                Server server = new Server(new ServerConnection(conn));
                Console.WriteLine("Connecting to Database Server...");
                server.ConnectionContext.Connect();
                //server.ConnectionContext.BeginTransaction();
                try
                {
                    double total = 0;                    
                    Stopwatch sw = new Stopwatch();
                    foreach (var key in keys)
                    {
                        var script = scripts[key];
                        string scriptText = String.Empty;

                        using (var reader = script.OpenText())
                            scriptText = reader.ReadToEnd();

                        if (scriptText.Contains("{DATABASE_NAME}"))
                            scriptText = scriptText.Replace("{DATABASE_NAME}", NewIndigoDataModel.Database.Connection.Database);
                        
                        if(scriptText.Contains("{MASTER_KEY}"))
                            scriptText = scriptText.Replace("{MASTER_KEY}", masterKey);

                        if (scriptText.Contains("{EXPITY_DATE}"))
                            scriptText = scriptText.Replace("{EXPITY_DATE}", DateTime.Now.AddYears(10).ToString("yyyyMMdd"));

                        Console.WriteLine("Start Executing Script: " + script.Name);
                        sw.Restart();
                        int result = server.ConnectionContext.ExecuteNonQuery(scriptText);
                        sw.Stop();
                        total += sw.Elapsed.TotalMilliseconds;
                        Console.WriteLine("End Executing Script, elapsed time: {0}ms", sw.Elapsed.TotalMilliseconds);
                    }

                    Console.WriteLine("Done creating schema, total time: {0}ms.", total);
                    Console.WriteLine("Validating Schema.");
                    var newDB = server.Databases[NewIndigoDataModel.Database.Connection.Database];

                    if (newDB != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        var dir = Directory.CreateDirectory(Path.Combine("C:\\veneka\\Migration\\", DateTime.Now.ToString("yyyyMMddhhmmss")));

                        //Tables
                        foreach(Table table in newDB.Tables)                        
                            sb.AppendLine(table.Name);
                        sb.AppendLine("Total: " + newDB.Tables.Count);
                        File.WriteAllText(Path.Combine(dir.FullName, "tables.txt"), sb.ToString());

                        //Views
                        sb.Clear();
                        int viewCount = 0;
                        foreach (View view in newDB.Views)
                        {
                            if (view.Schema == "dbo")
                            {
                                viewCount++;
                                sb.AppendLine(view.Name);
                            }
                        }
                        sb.AppendLine("Total: " + viewCount);
                        File.WriteAllText(Path.Combine(dir.FullName, "views.txt"), sb.ToString());

                        //SPs
                        int spCount = 0;
                        sb.Clear();
                        foreach (StoredProcedure sp in newDB.StoredProcedures)
                        {
                            if (sp.Schema != "sys")
                            {
                                spCount++;
                                sb.AppendLine(sp.Name);
                            }
                        }

                        sb.AppendLine("Total: " + spCount);
                        File.WriteAllText(Path.Combine(dir.FullName, "storedProcs.txt"), sb.ToString());
                        sb.Clear();

                        //MasterKey
                        sb.Clear();
                        sb.AppendLine("Master Key: created " + newDB.MasterKey.CreateDate);                        

                        //SymmetricKeys                        
                        foreach (SymmetricKey key in newDB.SymmetricKeys)                        
                            sb.AppendLine("SymmetricKey: " + key.Name + " " + key.EncryptionAlgorithm + " " + key.KeyLength);
                                                    
                        sb.AppendLine("Total SymmetricKeys: " + newDB.SymmetricKeys.Count);


                        //Certificates                        
                        foreach (Certificate cert in newDB.Certificates)
                        {
                            sb.AppendLine("Certificate:" + cert.Name + " " + cert.ExpirationDate + " " + cert.PrivateKeyEncryptionType);
                            cert.Export(Path.Combine("C:\\Veneka\\Migration\\", cert.Name + ".crt"),
                                        Path.Combine("C:\\Veneka\\Migration\\", cert.Name + "_pvtKey"),
                                        "Jungle01$");
                        }

                        sb.AppendLine("Total Certificates: " + newDB.Certificates.Count);
                        File.WriteAllText(Path.Combine(dir.FullName, "encryption.txt"), sb.ToString());

                        sb.Clear();

                        string MasterKeyFile = Path.Combine("C:\\Veneka\\Migration\\", "MaskterKey");
                        //BackupKeys
                        newDB.MasterKey.Export(MasterKeyFile, "Jungle01$");

                        if (newDB.Tables.Count == 155 &&
                                spCount == 355 &&
                                viewCount == 13 &&
                                newDB.SymmetricKeys.Count == 3 &&
                                newDB.Certificates.Count == 3)
                            isDone = true;  
                    }

                    

                    //trn.Commit();
                    //}
                    //server.ConnectionContext.CommitTransaction();
                }
                //catch
                //{
                //    //server.ConnectionContext.RollBackTransaction();
                //    throw;
                //}
                finally
                {
                    server.ConnectionContext.Disconnect();
                }
            }            

            return isDone;
        }// end method bool SetupDatabase()

        public bool DoesNewDatabaseExists()
        {
            var exists = false;

            try
            {
                exists = this.NewIndigoDataModel.Database.Exists();
            }// end try
            catch (Exception ex)
            {
                throw;
            }// end catch (Exception ex)

            return exists;
        }// end method bool DoesNewDatabaseExists()


        public bool BackupDatabase()
        {
            var isDone = false;

            //try
            //{

            //    var setupDir = FilesUtils.dirPath.Contains("Setup")
            //            ? FilesUtils.dirPath
            //            : FilesUtils.dirPath + "Setup/";

            //    var backupDirPath = FilesUtils.dirPath.Replace("Scripts/", "") +
            //        "Output/Backups/" + DateTime.Now.ToString("dd-MMM-yy") + "/";

            //    if (Directory.Exists(setupDir))
            //    {
            //        var files = Directory.GetFiles(setupDir);
            //        var sqlString = FilesUtils.GetSQLScript("Backup.sql", "Setup", files);

            //        if (sqlString.Contains("DB_NAME"))
            //        {
            //            sqlString =
            //                sqlString.Replace("DB_NAME", NewIndigoDataModel.Database.Connection.Database);
            //        }// end if (sqlString.Contains("DB_NAME"))

            //        if (!Directory.Exists(backupDirPath))
            //            Directory.CreateDirectory(backupDirPath);

            //        var dbFileName = NewIndigoDataModel.Database.Connection.Database + " " +
            //            DateTime.Now.ToString("hh mm") + ".bak";

            //        backupDirPath = backupDirPath.Replace("Setup/", "");


            //        if (!Directory.Exists(backupDirPath))
            //            Directory.CreateDirectory(backupDirPath);

            //        if (sqlString.Contains("BACKUP_DIR_PATH"))
            //            sqlString = sqlString.Replace("BACKUP_DIR_PATH", backupDirPath + dbFileName);
                    
            //        var results =
            //                    this.NewIndigoDataModel
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
            //catch
            //{
            //    throw;
            //}// end catch (Exception ex)

            return isDone;
        }// end bool BackupDatabase()

        public int[] GetIssuersIDs()
        {
            int[] issuersList = null;

            try
            {
                var sqlString = "SELECT DISTINCT [issuer_product].[issuer_id]               " +
                                "FROM [cards]                                               " +
                                "INNER JOIN [dbo].[issuer_product]                          " +
	                            "    ON [cards].[product_id] = [issuer_product].[product_id]";

                issuersList = this.NewIndigoDataModel.Database.SqlQuery<int>
                    (
                        sqlString,
                        new Object[] { }
                    ).ToArray<int>();
                                      
            }// end try
            catch
            {
                throw;
            }// end catch (Exception ex)

            return issuersList;
        }// end method List<issuer> GetIssuers()

        #endregion
    }
}
