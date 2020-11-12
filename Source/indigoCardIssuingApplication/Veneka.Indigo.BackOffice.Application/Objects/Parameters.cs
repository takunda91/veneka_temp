using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.Objects
{
    public class Parameters
    {
        private string parametername;
        private System.Data.DbType dataype;
        private string value;
        private ParameterDirection direction;
        public string ParameterName { get { return parametername; } }
        public DbType DataType { get { return dataype; } }
        public string Value { get { return value; } set { this.value = value; } }

        public ParameterDirection Direction { get { return direction; } }
        public Parameters(string _parameter, DbType _datatype, string _value, ParameterDirection _direction)
        {
            parametername = _parameter;
            dataype = _datatype;
            value = _value;
            direction = _direction;

        }
    }
}
