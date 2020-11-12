using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.COMS.DataSource.Callbacks
{
    public class WcfCustomDataDAL: ICustomDataDAL
    {
        private readonly IComsCallback _proxy;
        public WcfCustomDataDAL(IComsCallback proxy)
        {
            _proxy = proxy;
        }

        public List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters, Dictionary<string, string> outputparameters)
        {
           return  _proxy.DataCall(spname, inputparameters, outputparameters);
        }

        public List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters, Dictionary<string, DataTable> inputkeyvaluetable, Dictionary<string, string> outputparameters)
        {
            return _proxy.DataCall(spname, inputparameters,inputkeyvaluetable, outputparameters);
        }
    }
}
