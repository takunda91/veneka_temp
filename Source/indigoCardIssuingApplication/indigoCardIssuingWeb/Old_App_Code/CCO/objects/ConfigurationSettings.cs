using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.Old_App_Code.CCO.objects
{
    public class ConfigurationSettings
    {
        public string ColumnName { get; set; }
        public string Key { get; set; }
        public int Length { get; set; }
        public string PageName { get; set; }
        public Boolean Required { get; set; }
        public Boolean Visibility { get; set; }
        public string Values { get; set; }
        public string type { get; set; }
    }
}