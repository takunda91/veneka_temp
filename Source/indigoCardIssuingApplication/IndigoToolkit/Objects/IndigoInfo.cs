using IndigoMigrationToolkit.Versions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndigoMigrationToolkit.Objects
{
    public enum ScriptTypes
    {
        Create, LoadLookup,
        MigrationSetup, PreMigration, MigrationBulkCopy, PostMigration, MigrationCleanup, CreateEnterprise, MigrationValidation
    }

    public enum ScriptFunction
    {
        C, //Create
        Q, //Query
        U //Unknown
    }

    public enum PreviousIndigoVersion
    {
        //v2000,
        //v2100,
        v2110,
        //v2120,
        //v2130,
        v2131        
    }

    public sealed class IndigoInfo
    {
        #region Constants
        private const string SCRIPTS_FOLDER = "Scripts";
        private const string MIGRATION_FOLDER = "Migration";
        private const string MigrationSetup_FOLDER = "Setup";
        private const string PreMigration_FOLDER = "Pre";
        private const string MigrationBulkCopy_FOLDER = "BulkCopy";
        private const string MigrationCleanup_FOLDER = "Cleanup";
        private const string MigrationValidation_FOLDER = "Validation";
        #endregion

        private readonly DirectoryInfo _rootScriptsPath;

        //Constructor for create info
        public IndigoInfo(string databaseName, string collation, string masterKey, DirectoryInfo exportPath, 
                            bool exportEncryption, DirectoryInfo exportEncryptionPath, string exportEncryptionPassword, bool createEnterprise)
        {
            if (String.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentNullException("databaseName");
            
            this.DatabaseName = databaseName;
            this.Collation = collation;
            this.MasterKey = masterKey;            
            this.ExportPath = exportPath;
            this.ExportEncryption = exportEncryption;
            this.ExportEncryptionPath = exportEncryptionPath;
            this.ExportEncryptionPassword = exportEncryptionPassword;
            this.CreateEnterprise = createEnterprise;

            this._rootScriptsPath = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, SCRIPTS_FOLDER));
        }

        //Constructor for migration Info
        public IndigoInfo(string databaseName, string collation, string masterKey, DirectoryInfo exportPath, bool exportEncryption,
                                 DirectoryInfo exportEncryptionPath, string exportEncryptionPassword, 
                                string sourceDatabaseName, PreviousIndigoVersion sourceIndigoVersion, DirectoryInfo postMigrationScriptFolder) :
            this(databaseName, collation, masterKey, exportPath, exportEncryption, exportEncryptionPath, exportEncryptionPassword, false)
        {
            this.SourceDatabaseName = sourceDatabaseName;
            this.SourceIndigoVersion = sourceIndigoVersion;
            this.PostMigrationScriptsPath = postMigrationScriptFolder;
        }

        public Dictionary<int, Tuple<ScriptFunction, FileInfo>> GetScripts(ScriptTypes scriptType, PreviousIndigoVersion? migrateFrom = null)
        {
            Dictionary<int, Tuple<ScriptFunction, FileInfo>> scripts = new Dictionary<int, Tuple<ScriptFunction, FileInfo>>();

            DirectoryInfo scriptsDir;


            switch (scriptType)
            {
                case ScriptTypes.Create:
                case ScriptTypes.LoadLookup:
                case ScriptTypes.CreateEnterprise:
                    scriptsDir = RootScriptsPath.GetDirectories(scriptType.ToString(), SearchOption.TopDirectoryOnly).FirstOrDefault();
                    break;
                case ScriptTypes.MigrationSetup:
                    scriptsDir = new DirectoryInfo(Path.Combine(_rootScriptsPath.FullName, MIGRATION_FOLDER, migrateFrom.ToString(), MigrationSetup_FOLDER));
                    break;
                //case ScriptTypes.PreMigration:
                //    break;
                case ScriptTypes.MigrationBulkCopy:
                    scriptsDir = new DirectoryInfo(Path.Combine(_rootScriptsPath.FullName, MIGRATION_FOLDER, migrateFrom.ToString(), MigrationBulkCopy_FOLDER));
                    break;
                //case ScriptTypes.PostMigration:
                //    break;
                case ScriptTypes.MigrationCleanup:
                    scriptsDir = new DirectoryInfo(Path.Combine(_rootScriptsPath.FullName, MIGRATION_FOLDER, migrateFrom.ToString(), MigrationCleanup_FOLDER));
                    break;
                case ScriptTypes.MigrationValidation:
                    scriptsDir = new DirectoryInfo(Path.Combine(_rootScriptsPath.FullName, MIGRATION_FOLDER, migrateFrom.ToString(), MigrationValidation_FOLDER));
                    break;
                default:
                    throw new ArgumentException("Not supported: " + scriptType.ToString(), "scriptType");
            }

            scripts = GetScripts(scriptsDir);


            //if (scriptType == ScriptTypes.Create || scriptType == ScriptTypes.LoadLookup)
            //    scriptsDir = RootScriptsPath.GetDirectories(scriptType.ToString(), SearchOption.TopDirectoryOnly).FirstOrDefault();
            //else if (scriptType == ScriptTypes.Migration && migrateFrom != null)
            //    scriptsDir = new DirectoryInfo(Path.Combine(_rootScriptsPath.FullName, MIGRATION_FOLDER, migrateFrom.FolderName, "BulkCopy"));
            //else
            //    throw new ArgumentException("Cannot be null.", "migrateFrom");



            return scripts;
        }

        public Dictionary<int, Tuple<ScriptFunction, FileInfo>> GetScripts(DirectoryInfo scriptsDir)
        {
            Dictionary<int, Tuple<ScriptFunction, FileInfo>> scripts = new Dictionary<int, Tuple<ScriptFunction, FileInfo>>();

            if (scriptsDir != null && scriptsDir.Exists)
            {
                var files = scriptsDir.GetFiles("*.sql", SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                {
                    var fileName = file.Name.Split(' ');

                    ScriptFunction scriptFunc = ScriptFunction.U;

                    if (fileName.Length > 1)
                    {
                        switch (fileName[1]) //This could override what happens with transactions etc.
                        {
                            case "C": scriptFunc = ScriptFunction.C; break;
                            case "Q": scriptFunc = ScriptFunction.Q; break;
                            default: scriptFunc = ScriptFunction.Q; break;
                        }
                    }

                    int scriptKey;

                    if (int.TryParse(fileName[0], out scriptKey))
                        scripts.Add(scriptKey, new Tuple<ScriptFunction, FileInfo>(scriptFunc, file));
                    else
                        throw new Exception(String.Format("Script file {0} is not numbered and cannot be used.", file.Name));
                }
            }
            else
                throw new Exception(String.Format("Scripts directory {0} could not be located!", scriptsDir.FullName));

            return scripts;
        }

        #region Properties
        public DirectoryInfo RootScriptsPath { get { return _rootScriptsPath; } }
        public IndigoVersionInfo IndigoVersion { get; private set; }
        public string DatabaseName { get; private set; }
        public string Collation { get; private set; }
        public string MasterKey { get; private set; }
        public DirectoryInfo ExportPath { get; private set; }
        public bool CreateEnterprise { get; private set; }

        public bool ExportEncryption { get; private set; }
        public DirectoryInfo ExportEncryptionPath { get; private set; }
        public string ExportEncryptionPassword { get; private set; }

        public string SourceDatabaseName { get; private set; }
        public PreviousIndigoVersion SourceIndigoVersion { get; private set; }
        public DirectoryInfo PostMigrationScriptsPath { get; private set; }
        #endregion
    }
}
