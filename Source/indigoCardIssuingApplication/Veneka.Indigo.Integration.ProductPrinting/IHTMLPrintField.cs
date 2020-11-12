using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.ProductPrinting
{
    //HTML output of the field.
    public interface IHTMLPrintField
    {
        string HTML { get; set; }
    }
}
