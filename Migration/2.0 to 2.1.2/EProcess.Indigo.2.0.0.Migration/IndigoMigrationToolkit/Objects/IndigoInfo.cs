using IndigoMigrationToolkit.Versions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndigoMigrationToolkit.Objects
{
    public sealed class IndigoInfo
    {
        public IndigoInfo(IndigoVersionInfo indigoVersion, string databaseName, string collation, string masterKey, DirectoryInfo exportPath, bool createEnterprise)
        {
            if (indigoVersion == null)
                throw new ArgumentNullException("indigoVersion");

            if (String.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentNullException("databaseName");

            //if (String.IsNullOrWhiteSpace(masterKey))
            //    throw new ArgumentNullException("masterKey");

            //if (!exportPath.Exists)
            //    throw new DirectoryNotFoundException(exportPath.FullName);

            this.IndigoVersion = indigoVersion;
            this.DatabaseName = databaseName;
            this.Collation = collation;
            this.MasterKey = masterKey;
            this.ExportPath = exportPath;
            this.CreateEnterprise = createEnterprise;         
        }

        public IndigoInfo(IndigoVersionInfo indigoVersion, string databaseName, string collation, string masterKey, DirectoryInfo exportPath,
                                bool exportEncryption, DirectoryInfo exportEncryptionPath, string exportEncryptionPassword, DirectoryInfo postMigrationScriptFolder, bool createEnterprise) :
            this(indigoVersion, databaseName, collation, masterKey, exportPath, createEnterprise)
        {
            this.ExportEncryption = exportEncryption;
            this.ExportEncryptionPath = exportEncryptionPath;
            this.ExportEncryptionPassword = exportEncryptionPassword;

            this.PostMigrationScriptsPath = postMigrationScriptFolder;
        }

        public IndigoVersionInfo IndigoVersion { get; private set; }
        public string DatabaseName { get; private set; }
        public string Collation { get; private set; }
        public string MasterKey { get; private set; }
        public DirectoryInfo ExportPath { get; private set; }
        public bool CreateEnterprise { get; private set; }

        public bool ExportEncryption { get; private set; }
        public DirectoryInfo ExportEncryptionPath { get; private set; }
        public string ExportEncryptionPassword { get; private set; }

        public DirectoryInfo PostMigrationScriptsPath { get; private set; }
    }
}
