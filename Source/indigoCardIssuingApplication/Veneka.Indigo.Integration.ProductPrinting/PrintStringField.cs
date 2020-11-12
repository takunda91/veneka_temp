using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.ProductPrinting
{
    public class PrintStringField: IProductPrintField
    {
        private string _html = String.Empty;

        public int ProductPrintFieldId { get; set; }
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Font { get; set; }
        public int FontSize { get; set; }
        public int FontColourRGB { get; set; }
        public string MappedName { get; set; }
        public string Label { get; set; }
        public int MaxSize { get; set; }
        public bool Editable { get; set; }
        public bool Deleted { get; set; }
        public int PrintSide { get; set; }
        public bool Printable { get; set; }

        public string FieldValue { get; set; }
        //public string HTML
        //{
        //    get
        //    {
        //        return GenerateHTML();
        //    }
        //    set
        //    {
        //        _html = value;
        //    }
        //}

        public string Value { get; set; }

    
        ///// <summary>
        ///// Generates the HTML for this PrintField. String element will generate a Span tag.
        ///// </summary>
        ///// <returns></returns>
        //private string GenerateHTML()
        //{
        //    StringWriter stringWriter = new StringWriter();

        //    // Put HtmlTextWriter in using block because it needs to call Dispose.
        //    using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
        //    {
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "absolute");
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.Top, Y.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + "px;");
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.Left, X.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + "px;");
        //        if(Height > 0)
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + "px;");

        //        if(Width > 0)
        //            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + "px;");
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "bold");
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, Font);
        //        writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, FontSize.ToString() + "pt");
        //        writer.RenderBeginTag(HtmlTextWriterTag.Span);
        //        writer.Write(Value);
        //        writer.RenderEndTag();
        //    }

        //    // Return the result.
        //    return stringWriter.ToString();
        //}
    }
}
