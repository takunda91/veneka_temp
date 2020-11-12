using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.ProductPrinting
{
    /// <summary>
    /// Contains information on the field and printing of the field.
    /// </summary>
    public interface IProductPrintField
    {
        int ProductPrintFieldId { get; set; }
        string Name { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        string Font { get; set; }
        int FontSize { get; set; }
        int FontColourRGB { get; set; }
        string MappedName { get; set; }
        string Label { get; set; }
        int MaxSize { get; set; }
        bool Editable { get; set; }
        bool Deleted { get; set; }
        int PrintSide { get; set; }
        bool Printable { get; set; }
    }
}
