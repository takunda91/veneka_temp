using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndigoMigrationToolkit.Versions
{
    public enum ScriptTypes
    {
        Create, LoadLookup,
        MigrationSetup, PreMigration, MigrationBulkCopy, PostMigration, MigrationCleanup, CreateEnterprise
    }

    public enum ScriptFunction
    {
        C, //Create
        Q, //Query
        U //Unknown
    }

    public abstract class IndigoVersionInfo
    {
        protected const string SCRIPTS_FOLDER = "Scripts";
        protected const string MIGRATION_FOLDER = "Migration";

        protected const string MigrationSetup_FOLDER = "Setup";
        protected const string PreMigration_FOLDER = "Pre";
        protected const string MigrationBulkCopy_FOLDER = "BulkCopy";
        //protected const string PostMigration_FOLDER = "Post";
        protected const string MigrationCleanup_FOLDER = "Cleanup";

        protected readonly DirectoryInfo _rootScriptsPath;

        protected IndigoVersionInfo(string version)
        {
            this._rootScriptsPath = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, SCRIPTS_FOLDER, version));
        }

        public DirectoryInfo RootScriptsPath { get { return _rootScriptsPath; } }
        
        public Dictionary<int, Tuple<ScriptFunction, FileInfo>> GetScripts(ScriptTypes scriptType, IndigoVersionInfo migrateFrom = null)
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
                    scriptsDir = new DirectoryInfo(Path.Combine(_rootScriptsPath.FullName, MIGRATION_FOLDER, migrateFrom.FolderName, MigrationSetup_FOLDER));
                    break;
                //case ScriptTypes.PreMigration:
                //    break;
                case ScriptTypes.MigrationBulkCopy:
                    scriptsDir = new DirectoryInfo(Path.Combine(_rootScriptsPath.FullName, MIGRATION_FOLDER, migrateFrom.FolderName, MigrationBulkCopy_FOLDER));
                    break;
                //case ScriptTypes.PostMigration:
                //    break;
                case ScriptTypes.MigrationCleanup:
                    scriptsDir = new DirectoryInfo(Path.Combine(_rootScriptsPath.FullName, MIGRATION_FOLDER, migrateFrom.FolderName, MigrationCleanup_FOLDER));
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

                    if(fileName.Length > 1)
                    {
                        switch (fileName[1]) //This could override what happens with transactions etc.
                        {
                            case "C": scriptFunc = ScriptFunction.C; break;
                            case "Q": scriptFunc = ScriptFunction.Q; break;
                            default: break;
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

        public abstract bool ValidateDatabase();

        public abstract string Name { get; }
        public abstract string FolderName { get; }
    }
}
