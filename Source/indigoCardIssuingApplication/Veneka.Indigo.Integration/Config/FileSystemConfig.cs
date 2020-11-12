using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.Integration.Config
{
    [DataContract]
    public class FileSystemConfig : Config, IConfig
    {
        #region Properties
        [DataMember]
        public string Path { get; private set; }
        [DataMember]
        public string Filename { get; private set; }
        [DataMember]
        public bool DeleteAfterLoad { get; private set; }
        [DataMember]
        public bool DuplicateFileCheck { get; private set; }
        [DataMember]
        public int Encryption { get; private set; }
        [DataMember]
        public string Password { get; private set; }
        [DataMember]
        public Guid InterfaceGuid { get; private set; }
        #endregion

        #region Constructors
        public FileSystemConfig()
        { }

        public FileSystemConfig(Guid interfaceGuid, string path, string filename, bool deleteAfterLoad, bool duplicateFileCheck, int encryption, string password)
        {
            ValidateArguments(path);

            this.InterfaceGuid = interfaceGuid;
            this.Path = path;
            this.Filename = filename;
            this.DeleteAfterLoad = deleteAfterLoad;
            this.DuplicateFileCheck = duplicateFileCheck;
            this.Encryption = encryption;
            this.Password = password;
        }

        public FileSystemConfig(Guid interfaceGuid, string path, string filename, bool deleteAfterLoad, bool duplicateFileCheck)
            : this(interfaceGuid, path, filename, deleteAfterLoad, duplicateFileCheck, -1, "")
        {
        }

        public FileSystemConfig(string interfaceGuid, string path, string filename, bool deleteAfterLoad, bool duplicateFileCheck, int encryption, string password)
            : this(Guid.Parse(interfaceGuid), path, filename, deleteAfterLoad, duplicateFileCheck, encryption, password)
        {
        }
        #endregion


        public void LoadConfig(DataRow configRow)
        {
            this.InterfaceGuid = Guid.Parse(configRow.Field<string>(FIELD_INTERFACE_GUID));
            this.Path = configRow.Field<string>(FIELD_PATH);
            this.Filename = configRow.Field<string>(FIELD_FILENAME);
            this.DeleteAfterLoad = configRow.Field<bool>(FIELD_DELETE_FILE);
            this.DuplicateFileCheck = configRow.Field<bool>(FIELD_DUPLICATE_FILE_CHECK);
            this.Encryption = configRow.Field<int>(FIELD_FILE_ENCRYPTION_TYPE);
            this.Password = configRow.Field<string>(FIELD_PASSWORD);

            ValidateArguments(this.Path);
        }

        private void ValidateArguments(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path", "Cannot be null or empty.");

        }
    }
}
