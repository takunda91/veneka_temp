using Catel.Data;
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
    public class DacVariables : DispatcherObservableObject
    {
        private ObservableCollection<CmdVariable> _sqlCmdVars = new ObservableCollection<CmdVariable>();

        public ObservableCollection<CmdVariable> SqlCmdVars
        {
            get { return _sqlCmdVars; }
            set
            {
                var oldValue = _sqlCmdVars;
                _sqlCmdVars = value;

                RaisePropertyChanged(() => SqlCmdVars, oldValue, value);
            }
        }

        public void SetCollection(IDictionary<string, string> input)
        {
            foreach(var item in input)
            {
                _sqlCmdVars.Add(new CmdVariable(item.Key, item.Value ));
            }
        }

        public void SetDictionary(IDictionary<string, string> dictionaryToSet)
        {
            foreach(var item in _sqlCmdVars)
            {
                if(dictionaryToSet.ContainsKey(item.Key))
                {
                    dictionaryToSet[item.Key] = item.Value;
                }
            }
        }
    }

    public class CmdVariable
    {
        private string _key;
        private string _value;

        public CmdVariable(string key, string value)
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
