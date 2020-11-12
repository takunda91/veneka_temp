using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Integration.DAL
{
  public  interface ICustomDataDAL
    {
        List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters,Dictionary<string, string> outputparameters);

        List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters, Dictionary<string, DataTable> inputkeyvaluetable, Dictionary<string, string> outputparameters);

    }
}
