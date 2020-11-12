using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.App_Code.utility.Objects
{
    public sealed class HtmlSelectOption
    {
        public string label { get; set; }
        public string value { get; set; }

        public HtmlSelectOption()
        {
        }

        public HtmlSelectOption(string label, string value)
        {
            this.label = label;
            this.value = value;
        }
    }
}