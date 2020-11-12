using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Integration.EMP.BLL
{

    public static class Tracer
    {
        private static readonly ILog _cbsLog = LogManager.GetLogger("TracerLog");

        public static void PrintProperties(object obj, int indent)
        {
            if (obj == null) return;
            string indentString = new string(' ', indent);
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);
                if (property.PropertyType.Assembly == objType.Assembly && !property.PropertyType.IsEnum)
                {
                    _cbsLog.Trace($"{indentString}{property.Name}:");
                    PrintProperties(propValue, indent + 2);
                }
                else
                {
                    _cbsLog.Trace($"{indentString}{property.Name}: {propValue}");
                }
            }
        }
    }
}
