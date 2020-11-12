using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;

namespace IndigoToolkit2.Models
{
    public class IndigoDatabase : ValidatableModelBase
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString
        {
            get { return GetValue<string>(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        /// <summary>
        /// Register the ConnectionString property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ConnectionStringProperty = RegisterProperty("ConnectionString", typeof(string), null);

        /// <summary>
        /// Gets or sets the target database name
        /// </summary>
        public string TargetDatabaseName
        {
            get { return GetValue<string>(TargetDatabaseNameProperty); }
            set { SetValue(TargetDatabaseNameProperty, value); }
        }

        /// <summary>
        /// Register the TargetDatabaseName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TargetDatabaseNameProperty = RegisterProperty("TargetDatabaseName", typeof(string), null);

        /// <summary>
        /// Gets or sets the DACPAC file to use.
        /// </summary>
        public string DacPacPath
        {
            get { return GetValue<string>(DacPacPathProperty); }
            set { SetValue(DacPacPathProperty, value); }
        }

        /// <summary>
        /// Register the DacPacPath property so it is known in the class.
        /// </summary>
        public static readonly PropertyData DacPacPathProperty = RegisterProperty("DacPacPath", typeof(string), null);

        public FileInfo DacPacFile
        {
            get
            {
                if (File.Exists(DacPacPath))
                {
                    return new FileInfo(DacPacPath);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the DACPAC file to use.
        /// </summary>
        public string PublishProfilePath
        {
            get { return GetValue<string>(PublishProfilePathProperty); }
            set { SetValue(PublishProfilePathProperty, value); }
        }

        /// <summary>
        /// Register the DacPacPath property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PublishProfilePathProperty = RegisterProperty("PublishProfilePath", typeof(string), null);

        public FileInfo PublishProfileFile
        {
            get
            {
                if (File.Exists(PublishProfilePath))
                {
                    return new FileInfo(PublishProfilePath);
                }

                return null;
            }
        }

        public override string ToString()
        {
            string fullString = string.Empty;
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                fullString += ConnectionString;
            }

            if (!string.IsNullOrEmpty(DacPacPath) && !string.IsNullOrWhiteSpace(PublishProfilePath))
            {
                fullString += Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(DacPacPath))
            {
                fullString += DacPacPath;
            }

            if (!string.IsNullOrEmpty(DacPacPath) && !string.IsNullOrWhiteSpace(PublishProfilePath))
            {
                fullString += Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(PublishProfilePath))
            {
                fullString += PublishProfilePath;
            }

            return fullString;
        }

        public bool CheckAllFieldPopulated()
        {
            if (!String.IsNullOrWhiteSpace(ConnectionString) &&
                    !String.IsNullOrWhiteSpace(TargetDatabaseName) &&
                    !String.IsNullOrWhiteSpace(DacPacPath) &&
                    !String.IsNullOrWhiteSpace(PublishProfilePath))
            {
                return true;
            }

            return false;
        }
    

        //protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        //{
        //    if (string.IsNullOrWhiteSpace(FirstName))
        //    {
        //        validationResults.Add(FieldValidationResult.CreateError(FirstNameProperty, "The first name is required"));
        //    }

        //    if (string.IsNullOrWhiteSpace(LastName))
        //    {
        //        validationResults.Add(FieldValidationResult.CreateError(LastNameProperty, "The last name is required"));
        //    }
        //}
    }
}
