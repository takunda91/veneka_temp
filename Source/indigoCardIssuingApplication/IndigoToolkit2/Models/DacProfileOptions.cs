using Catel.Data;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoToolkit2.Models
{
    /// <summary>
    /// Hold a collection of Key/Value pairs
    /// </summary>
    public class DacProfileOptions : DispatcherObservableObject
    {
        private ObservableCollection<DacProfileOption> _dacOptions = new ObservableCollection<DacProfileOption>();

        public ObservableCollection<DacProfileOption> Options
        {
            get { return _dacOptions; }
            set
            {
                var oldValue = _dacOptions;
                _dacOptions = value;

                RaisePropertyChanged(() => Options, oldValue, value);
            }
        }

        /// <summary>
        /// Extracts options which are useful to the user
        /// </summary>
        /// <param name="input"></param>
        public void SetCollection(Microsoft.SqlServer.Dac.DacDeployOptions input)
        {
            // Show if the DB is automatically backed up
            Options.Add(new DacProfileOption("Backup Database Before Update", input.BackupDatabaseBeforeChanges ? "Yes" : "No"));

            // Block on data loss
            Options.Add(new DacProfileOption("Stop if data loss occures", input.BlockOnPossibleDataLoss ? "Yes" : "No"));

            // Verify Deployment
            Options.Add(new DacProfileOption("Verify Deployment", input.VerifyDeployment ? "Yes" : "No"));
        }
    }

    public class DacProfileOption
    {
        private string _key;
        private string _value;

        public DacProfileOption(string key, string value)
        {
            if(String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "Cannot be null");
            }

            Key = key;
            Value = value;
        }

        public string Key
        {
            get { return _key; }
            private set
            {
                _key = value;
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
            }
        }
    }
}
