using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Diagnostics;
using System.Data.SqlClient;
using IndigoMigrationToolkit.Utils;
using System.IO;
using IndigoMigrationToolkit.Objects;
using System.Data;
using System.Security;

namespace IndigoToolkit.DatabaseServers
{
    public class SqlServer
    {
        //private SqlConnection _sqlConnection;
        private Server _server;
        private WindowLogging _loggingWindow;

        private Dictionary<int, ScriptTypes> _migrationFlow = new Dictionary<int, ScriptTypes>()
        {
            { 0, ScriptTypes.MigrationSetup },
            { 1, ScriptTypes.MigrationBulkCopy },
            { 2, ScriptTypes.MigrationCleanup }
        };

        private SqlServer(ServerConnection serverConnection, SqlConnectionInfo.AuthenticationMethod authType, bool encrypted, bool trustServer, int? commandTimeout)
        {
            _server = new Server(serverConnection);
            serverConnection.EncryptConnection = encrypted;            
            serverConnection.TrustServerCertificate = trustServer;

            serverConnection.LoginSecure = true;

            if (authType == SqlConnectionInfo.AuthenticationMethod.SqlPassword)
            {
                serverConnection.LoginSecure = false;
            }

            if (commandTimeout != null)
                _server.ConnectionContext.StatementTimeout = commandTimeout.Value;
        }

        public SqlServer(string server, string username, string password, SqlConnectionInfo.AuthenticationMethod authType, bool encrypted, bool trustServer, int? commandTimeout, WindowLogging loggingWindow)
            : this(new ServerConnection(server, username, password), authType, encrypted, trustServer, commandTimeout)
        {
            _loggingWindow = loggingWindow;
            ServerName = server;
        }

        public SqlServer(string server, bool encrypted, bool trustServer, int? commandTimeout, WindowLogging loggingWindow)
            : this(new ServerConnection(server), SqlConnectionInfo.AuthenticationMethod.NotSpecified, encrypted, trustServer, commandTimeout)
        {
            _loggingWindow = loggingWindow;
            ServerName = server;
        }

        public void Connect()
        {
            _server.ConnectionContext.Connect();
            Connected = true;
        }
        
        public void Disconnect()
        {
            _server.ConnectionContext.Disconnect();
            Connected = false;
        }

        public List<string> ServerCollations
        {
            get
            {
                var d = _server.EnumCollations();
                List<string> collations = new List<string>();
                foreach (DataRow r in d.Rows)
                {
                    collations.Add(r["Name"].ToString());
                }

                return collations;
            }
        }

        public string DefaultServerCollation
        {
            get
            {
                return _server.Collation;
            }
        }

        public string ServerName
        {
            get; set;
        }

        public string ServerInfo
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (Connected)
                {
                    sb.AppendFormat("Product: {0}", _server.Product);
                    sb.AppendLine();

                    sb.AppendFormat("Version: {0}", _server.VersionString);
                    sb.AppendLine();

                    sb.AppendFormat("Product Level: {0}", _server.ProductLevel);
                    sb.AppendLine();

                    sb.AppendFormat("Platform: {0}", _server.Platform);
                    sb.AppendLine();

                    sb.AppendFormat("OS Version: {0}", _server.OSVersion);
                }
                else
                {
                    sb.Append("Not connect to SQL Server");
                }
                
                return sb.ToString();
            }
        }

        public List<string> DatabaseList()
        {
            List<string> dbs = new List<string>();
            _server.Databases.Refresh();
            foreach (Database db in _server.Databases)
                dbs.Add(db.Name);            

            return dbs;
        }

        public string DatabaseCollation(string databaseName)
        {
            return _server.Databases[databaseName].Collation;
        }

        public bool Exists(string database)
        {
            _server.Databases.Refresh();

            if (_server.Databases[database] == null)
                return false;

            return true;
        }

        public bool CreateNewDatabase(IndigoInfo targetIndigo)
        {
            _loggingWindow.Write("-------------------------------------- Create Database --------------------------------------");
            var isDone = false;

            try
            {
                Database targetNewDB = new Database(_server, targetIndigo.DatabaseName);
                targetNewDB.Collation = targetIndigo.Collation;                
                targetNewDB.Create();

                double total = ExecuteScripts(targetIndigo, ScriptTypes.Create, false);

                _loggingWindow.WriteFormat("Done creating schema, total time: {0}.", TimeSpan.FromMilliseconds(total).ToString(@"hh\:mm\:ss\.fff"));
                _loggingWindow.Write("Validating Schema.");

                _server.Databases.Refresh();
                var newDB = _server.Databases[targetIndigo.DatabaseName];

                if (newDB != null)
                {
                    StringBuilder sb = new StringBuilder();
                    var dir = Directory.CreateDirectory(Path.Combine(targetIndigo.ExportPath.FullName, DateTime.Now.ToString("yyyyMMddhhmmss")));

                    //Tables
                    foreach (Table table in newDB.Tables)
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

                        if (targetIndigo.ExportEncryption)
                            cert.Export(Path.Combine(targetIndigo.ExportEncryptionPath.FullName, String.Format("{0}_{1}.crt", targetIndigo.DatabaseName, cert.Name)),
                                        Path.Combine(targetIndigo.ExportEncryptionPath.FullName, String.Format("{0}_{1}_pvtKey", targetIndigo.DatabaseName, cert.Name)),
                                        targetIndigo.ExportEncryptionPassword);
                    }

                    sb.AppendLine("Total Certificates: " + newDB.Certificates.Count);
                    File.WriteAllText(Path.Combine(dir.FullName, "encryption.txt"), sb.ToString());

                    sb.Clear();

                    string MasterKeyFile = Path.Combine(targetIndigo.ExportEncryptionPath.FullName, String.Format("{0}_MaskterKey", targetIndigo.DatabaseName));
                    //BackupKeys
                    if (targetIndigo.ExportEncryption)
                        newDB.MasterKey.Export(MasterKeyFile, targetIndigo.ExportEncryptionPassword);

                    //Load Defaults
                    _loggingWindow.Write("Loading lookup data.");
                    
                    total = ExecuteScripts(targetIndigo, ScriptTypes.LoadLookup, true);

                    _loggingWindow.WriteFormat("Done loading lookup data, elapsed time {0}ms", TimeSpan.FromMilliseconds(total).ToString(@"hh\:mm\:ss\.fff"));
                          
                    //Create Enterprise
                    if(targetIndigo.CreateEnterprise)
                    {
                        _loggingWindow.Write("Create enterprise data.");

                        total = ExecuteScripts(targetIndigo, ScriptTypes.CreateEnterprise, true);

                        _loggingWindow.WriteFormat("Done creating enterprise data, elapsed time {0}ms", TimeSpan.FromMilliseconds(total).ToString(@"hh\:mm\:ss\.fff"));
                    }


                    //if (newDB.Tables.Count == 155 &&
                    //        spCount == 355 &&
                    //        viewCount == 13 &&
                    //        newDB.SymmetricKeys.Count == 3 &&
                    //        newDB.Certificates.Count == 3)
                    //    isDone = true;
                    isDone = true;
                }
            }
            catch (Exception ex)
            {
                _loggingWindow.Write(ex.ToString());
            }

            if (isDone)
            {
                _loggingWindow.WriteFormat("Done creating {0}", targetIndigo.DatabaseName);
            }
            else
            {
                _loggingWindow.WriteFormat("Failed creating {0}", targetIndigo.DatabaseName);
            }

            //var completionSource = new TaskCompletionSource<bool>();
            //completionSource.SetResult(isDone);
            //return completionSource.Task;
            return isDone;
        }

        

        private double ExecuteScripts(IndigoInfo targetIndigo, ScriptTypes scriptType, bool useTransaction)
        {
            var scripts = targetIndigo.GetScripts(scriptType);
            double total = ExecuteNonQuery(targetIndigo, useTransaction, scripts);

            return total;
        }

        

        public void MigrateData(IndigoInfo targetIndigo, int? commandTimeout, int? migrationBatchSize)
        {
            _loggingWindow.Write("---------------------------------------- Migration ------------------------------------------");

            bool doPostScripts = true;
            double total = 0;
            Stopwatch sw = new Stopwatch();

            try
            {
                // Do setup
                _loggingWindow.Write("------------------------------------- Migration Setup -------------------------------------");
                var setupScripts = targetIndigo.GetScripts(ScriptTypes.MigrationSetup, targetIndigo.SourceIndigoVersion);
                total += ExecuteNonQuery(targetIndigo, false, setupScripts);
                _loggingWindow.Write("------------------------------------ Migration BulkCopy -----------------------------------");

                using (SqlConnection con = new SqlConnection(_server.ConnectionContext.ConnectionString))
                {
                    using (SqlConnection destinationConnection = new SqlConnection(_server.ConnectionContext.ConnectionString))
                    {
                        con.Open();
                        destinationConnection.Open();

                        using (SqlTransaction transaction = destinationConnection.BeginTransaction())
                        {
                            try
                            {
                                //Run PreMigration Scripts

                                //Migration has 5 parts, setup, PreMigration, BulkCopy, PostMigration and cleanup 
                                var scripts = targetIndigo.GetScripts(ScriptTypes.MigrationBulkCopy, targetIndigo.SourceIndigoVersion);

                                if (scripts.Count <= 0)
                                    throw new Exception("No migration scripts in directory.");

                                var keys = scripts.Keys.ToList();
                                keys.Sort();

                                byte[] barr = new byte[] { (byte)36 };


                                foreach (var key in keys)
                                {                                    
                                    var script = scripts[key];

                                    bool firstLine = true;
                                    string tableName = String.Empty;
                                    StringBuilder scriptText = new StringBuilder();

                                    //Extract table name and script text
                                    using (var fileReader = script.Item2.OpenText())
                                    {
                                        while (fileReader.Peek() >= 0)
                                        {
                                            string line = fileReader.ReadLine()
                                                                    .Replace("{DATABASE_NAME}", targetIndigo.DatabaseName)
                                                                    .Replace("{SOURCE_DATABASE_NAME}", targetIndigo.SourceDatabaseName);
                                            if (firstLine)
                                            {
                                                tableName = line;
                                                firstLine = false;
                                            }
                                            else
                                                scriptText.AppendLine(line);
                                        }
                                    }

                                    _loggingWindow.WriteFormat("Start Bulk Copy: {0}", script.Item2.Name);
                                    sw.Restart();
                                    SqlCommand cmd = new SqlCommand(scriptText.ToString().Trim(), con);
                                    //cmd.CommandType = System.Data.CommandType.StoredProcedure;

                                    cmd.CommandTimeout = 0;

                                    //if (commandTimeout != null)
                                    //    cmd.CommandTimeout = commandTimeout.Value;

                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        using (SqlBulkCopy sbc = new SqlBulkCopy(destinationConnection, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.TableLock, transaction))
                                        {
                                            //if (commandTimeout != null)
                                            //    sbc.BulkCopyTimeout = commandTimeout.Value;

                                            sbc.BulkCopyTimeout = 0;
                                            sbc.ColumnMappings.Clear();
                                            sbc.SqlRowsCopied += Sbc_SqlRowsCopied;
                                            sbc.NotifyAfter = 50000;

                                            if (migrationBatchSize != null)
                                                sbc.BatchSize = migrationBatchSize.Value;
                                            else
                                                sbc.BatchSize = 1000;

                                            for (int i = 0; i < reader.FieldCount; i++)
                                            {
                                                //columns.Add(reader.GetName(i));
                                                sbc.ColumnMappings.Add(reader.GetName(i), reader.GetName(i));
                                            }

                                            sbc.DestinationTableName = tableName;
                                            sbc.WriteToServer(reader);
                                        }
                                    }

                                    transaction.Save("save_mig_" + key);
                                    sw.Stop();

                                    total += sw.Elapsed.TotalMilliseconds;
                                    _loggingWindow.WriteFormat("End Bulk Copy, elapsed time: {0}", sw.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
                                }

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                doPostScripts = false;
                                sw.Stop();
                                total += sw.Elapsed.TotalMilliseconds;
                                transaction.Rollback();
                                _loggingWindow.WriteFormat("Failed after {0}", sw.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
                                _loggingWindow.Write(ex.ToString());
                            }
                        }
                    }
                }

                _loggingWindow.Write("---------------------------------- Migration Data Validation ---------------------------------");
                var validationScripts = targetIndigo.GetScripts(ScriptTypes.MigrationValidation, targetIndigo.SourceIndigoVersion);
                List<string> failedValidation;
                
                total += ExecuteValidationQuery(targetIndigo, false, validationScripts, out failedValidation);

                if (failedValidation.Count > 0)
                {
                    _loggingWindow.Write("*******************************************************************************************");
                    _loggingWindow.WriteFormat("Total Validations Failed {0}", failedValidation.Count);
                    foreach(var item in failedValidation)
                    {
                        _loggingWindow.WriteFormat("Validation Failed For: {0}", item);
                    }
                    _loggingWindow.Write("*******************************************************************************************");
                }

                _loggingWindow.WriteFormat("End Post Migration Scripts, elapsed time: {0}", sw.Elapsed.ToString(@"hh\:mm\:ss\.fff"));


                if (doPostScripts && failedValidation.Count == 0)
                {
                    //Run PostMigration Scripts
                    _loggingWindow.Write("---------------------------------- Post Migration Scripts ---------------------------------");
                    if (targetIndigo.PostMigrationScriptsPath != null && 
                            targetIndigo.PostMigrationScriptsPath.Exists)
                    {
                        var postScripts = targetIndigo.GetScripts(targetIndigo.PostMigrationScriptsPath);

                        if (postScripts != null && postScripts.Count > 0)
                            total += ExecuteNonQuery(targetIndigo, true, postScripts);
                        else
                            _loggingWindow.WriteFormat("No post scripts found in directory.");

                        _loggingWindow.WriteFormat("End Post Migration Scripts, elapsed time: {0}", sw.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
                    }
                    else
                        _loggingWindow.Write("No post scripts directory given, skipping...");
                }
            }
            catch (Exception ex)
            {
                _loggingWindow.Write(ex.ToString());
            }

            try
            {
                //Do Cleanup
                _loggingWindow.Write("------------------------------------- Migration Clean Up -------------------------------------");  
                var cleanupScripts = targetIndigo.GetScripts(ScriptTypes.MigrationCleanup, targetIndigo.SourceIndigoVersion);
                total += ExecuteNonQuery(targetIndigo, false, cleanupScripts);
            }
            catch(Exception ex)
            {
                _loggingWindow.Write(ex.ToString());
            }

            _loggingWindow.WriteFormat("TOTAL MIGRATION TIME: {0}", TimeSpan.FromMilliseconds(total).ToString(@"hh\:mm\:ss\.fff"));
        }

        public void ValidateMigrateData(IndigoInfo targetIndigo, int? commandTimeout, int? migrationBatchSize)
        {
            _loggingWindow.Write("---------------------------------------- Validation ------------------------------------------");

            bool doPostScripts = true;
            double total = 0;
            //Stopwatch sw = new Stopwatch();
            
            try
            {
                // Do setup
                _loggingWindow.Write("------------------------------------- Validation Setup -------------------------------------");
                var setupScripts = targetIndigo.GetScripts(ScriptTypes.MigrationSetup, targetIndigo.SourceIndigoVersion);
                total += ExecuteNonQuery(targetIndigo, false, setupScripts);
                                

                _loggingWindow.Write("---------------------------------- Migration Data Validation ---------------------------------");
                var validationScripts = targetIndigo.GetScripts(ScriptTypes.MigrationValidation, targetIndigo.SourceIndigoVersion);
                List<string> failedValidation;

                total += ExecuteValidationQuery(targetIndigo, false, validationScripts, out failedValidation);

           

                if (failedValidation.Count > 0)
                {
                    _loggingWindow.Write("*******************************************************************************************");
                    _loggingWindow.WriteFormat("Total Validations Failed {0}", failedValidation.Count);
                    foreach (var item in failedValidation)
                    {
                        _loggingWindow.WriteFormat("Validation Failed For: {0}", item);
                    }
                    _loggingWindow.Write("*******************************************************************************************");
                }
            }
            catch (Exception ex)
            {
                _loggingWindow.Write(ex.ToString());
            }

            try
            {
                //Do Cleanup
                _loggingWindow.Write("------------------------------------- Validation Clean Up -------------------------------------");
                var cleanupScripts = targetIndigo.GetScripts(ScriptTypes.MigrationCleanup, targetIndigo.SourceIndigoVersion);
                total += ExecuteNonQuery(targetIndigo, false, cleanupScripts);
            }
            catch (Exception ex)
            {
                _loggingWindow.Write(ex.ToString());
            }

            _loggingWindow.WriteFormat("TOTAL VALIDATION TIME: {0}", TimeSpan.FromMilliseconds(total).ToString(@"hh\:mm\:ss\.fff"));
        }

        private void Sbc_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            _loggingWindow.WriteFormat("Records copied {0:n0}", e.RowsCopied);
        }

        private double ExecuteNonQuery(IndigoInfo targetIndigo, bool useTransaction, Dictionary<int, Tuple<ScriptFunction, FileInfo>> scripts)
        {
            if (scripts.Count <= 0)
                throw new Exception("No setup scripts in directory.");

            var keys = scripts.Keys.ToList();
            keys.Sort();

            if (useTransaction && !scripts.Values.Any( a=> a.Item1 == ScriptFunction.C)) //If any of the scripts is Create we cant use transaction
                _server.ConnectionContext.BeginTransaction();

            double total = 0;
            Stopwatch sw = new Stopwatch();
            Tuple<ScriptFunction, FileInfo> script = null;

            try
            {
                foreach (var key in keys)
                {
                    script = scripts[key];
                    string scriptText = String.Empty;

                    using (var reader = script.Item2.OpenText())
                        scriptText = reader.ReadToEnd();

                    if (scriptText.Contains("{DATABASE_NAME}"))
                        scriptText = scriptText.Replace("{DATABASE_NAME}", targetIndigo.DatabaseName);

                    if (scriptText.Contains("{SOURCE_DATABASE_NAME}"))
                        scriptText = scriptText.Replace("{SOURCE_DATABASE_NAME}", targetIndigo.SourceDatabaseName);

                    if (scriptText.Contains("{MASTER_KEY}"))
                        scriptText = scriptText.Replace("{MASTER_KEY}", targetIndigo.MasterKey);

                    if (scriptText.Contains("{EXPITY_DATE}"))
                        scriptText = scriptText.Replace("{EXPITY_DATE}", DateTime.Now.AddYears(10).ToString("yyyyMMdd"));

                    //Do we need to Run this in its own transaction
                    if (script.Item1 == ScriptFunction.Q)
                        _server.ConnectionContext.BeginTransaction();

                    _loggingWindow.WriteFormat("Start Executing Script: {0}", script.Item2.Name);
                    sw.Restart();
                    var results = _server.ConnectionContext.ExecuteNonQuery(scriptText);
                    sw.Stop();

                    if (script.Item1 == ScriptFunction.Q)
                        _server.ConnectionContext.CommitTransaction();

                    total += sw.Elapsed.TotalMilliseconds;
                    _loggingWindow.WriteFormat("End Executing Script, elapsed time: {0}", sw.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
                }
            }
            catch
            {
                if (script.Item1 == ScriptFunction.Q)
                    _server.ConnectionContext.RollBackTransaction();

                if (useTransaction && !scripts.Values.Any(a => a.Item1 == ScriptFunction.C))
                    _server.ConnectionContext.RollBackTransaction();

                throw;
            }

            if (useTransaction && !scripts.Values.Any(a => a.Item1 == ScriptFunction.C))
                _server.ConnectionContext.CommitTransaction();
            return total;
        }

        private double ExecuteValidationQuery(IndigoInfo targetIndigo, bool useTransaction, Dictionary<int, Tuple<ScriptFunction, FileInfo>> scripts, out List<string> failedValidation)
        {
            failedValidation = new List<string>();

            if (scripts.Count <= 0)
                throw new Exception("No setup scripts in directory.");

            var keys = scripts.Keys.ToList();
            keys.Sort();

            //if (useTransaction && !scripts.Values.Any(a => a.Item1 == ScriptFunction.C)) //If any of the scripts is Create we cant use transaction
            //    _server.ConnectionContext.BeginTransaction();

            double total = 0;
            Stopwatch sw = new Stopwatch();
            Tuple<ScriptFunction, FileInfo> script = null;

            try
            {
                foreach (var key in keys)
                {
                    script = scripts[key];
                    string scriptText = String.Empty;

                    using (var reader = script.Item2.OpenText())
                        scriptText = reader.ReadToEnd();

                    if (scriptText.Contains("{DATABASE_NAME}"))
                        scriptText = scriptText.Replace("{DATABASE_NAME}", targetIndigo.DatabaseName);

                    if (scriptText.Contains("{SOURCE_DATABASE_NAME}"))
                        scriptText = scriptText.Replace("{SOURCE_DATABASE_NAME}", targetIndigo.SourceDatabaseName);

                    if (scriptText.Contains("{MASTER_KEY}"))
                        scriptText = scriptText.Replace("{MASTER_KEY}", targetIndigo.MasterKey);

                    if (scriptText.Contains("{EXPITY_DATE}"))
                        scriptText = scriptText.Replace("{EXPITY_DATE}", DateTime.Now.AddYears(10).ToString("yyyyMMdd"));

                    //Do we need to Run this in its own transaction
                    //if (script.Item1 == ScriptFunction.Q)
                    //    _server.ConnectionContext.BeginTransaction();

                    _loggingWindow.WriteFormat("Start Executing Script: {0}", script.Item2.Name);
                    sw.Restart();
                    var results = _server.ConnectionContext.ExecuteReader(scriptText);
                    sw.Stop();

                    if (results.HasRows)
                    {
                        _loggingWindow.WriteFormat("Validation FAILED ***********");
                        failedValidation.Add(script.Item2.Name);
                    }
                    else
                        _loggingWindow.Write("Validation PASSED");

                    //if (script.Item1 == ScriptFunction.Q)
                    //    _server.ConnectionContext.CommitTransaction();

                    results.Close();

                    total += sw.Elapsed.TotalMilliseconds;
                    _loggingWindow.WriteFormat("End Executing Script, elapsed time: {0}", sw.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
                }
            }
            catch
            {
                //if (script.Item1 == ScriptFunction.Q)
                //    _server.ConnectionContext.RollBackTransaction();

                if (useTransaction && !scripts.Values.Any(a => a.Item1 == ScriptFunction.C))
                    _server.ConnectionContext.RollBackTransaction();

                throw;
            }

            if (useTransaction && !scripts.Values.Any(a => a.Item1 == ScriptFunction.C))
                _server.ConnectionContext.CommitTransaction();

            return total;
        }

        public bool Connected
        {
            get; private set;
        }


        public void Bulk()
        {
            try
            {
                using (SqlBulkCopy sbc = new SqlBulkCopy(_server.ConnectionContext.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
                {
                    using (SqlConnection con = new SqlConnection(_server.ConnectionContext.ConnectionString))
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand("SELECT * from [indigo_database_main_dev].[dbo].[user]", con);

                        var reader = cmd.ExecuteReader();                       

                        sbc.DestinationTableName = "[indigo_database_v213].[dbo].[user]";
                        sbc.WriteToServer(reader);

                    }
                }
            }
            catch(Exception ex)
            {
                _loggingWindow.Write(ex.ToString());
            }

        }        
    }
}
