using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace indigoCardIssuingWeb.CardIssuanceService
{
    /// <summary>
    /// Partial class for implementing the HTML generation method for the product field when used for printing card using web pages.
    /// </summary>
    public partial class ProductField
    {

        public string ImageValue
        {
            get
            {
                if (ProductPrintFieldTypeId == 1)
                    return "data:image/jpg;base64," + Convert.ToBase64String(Value);
                else
                    return String.Empty;
            }
        }

        public string TextValue
        {
            get
            {
                if (ProductPrintFieldTypeId == 0)
                    return GetValue();
                else
                    return String.Empty;
            }
        }

        /// <summary>
        /// For string fields you may use this to set the value of the field. Field default is UTF8.
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(string value)
        {
            if (ProductPrintFieldTypeId != 0)
                throw new ArgumentException("Cannot set string value for a product field that isn't of a string type.");

            Value = System.Text.Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// Helper method for getting string value for product field. Field default is UTF8.
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            if (ProductPrintFieldTypeId != 0)
                throw new ArgumentException("Cannot get string value for a product field that isn't of a string type.");

            return System.Text.Encoding.UTF8.GetString(Value);
        }

        public string GetHTML()
        {
            switch (ProductPrintFieldTypeId)
            {
                case 0: return GenerateStringFieldHTML();
                case 1: return GenerateImageFieldHTML();
                default:
                    throw new ArgumentException("Unknown ProductPrintFieldTypeId.");
            }
        }

        /// <summary>
        /// Generates the HTML for this PrintField. String element will generate a Span tag.
        /// </summary>
        /// <returns></returns>
        private string GenerateStringFieldHTML()
        {
            StringWriter stringWriter = new StringWriter();

            string fontStyle = "normal",
                    fontVariant = "normal",
                    fontWeight = "bold",
                    fontSize = FontSize.ToString() + "pt",
                    fontFamily = "Lucida Console",
                    fontColour = "black";
                    

            //Work out font styles, if any... bit of a hack
            if (Font.Contains(','))
            {
                string[] fontAttr = Font.Split(',');

                switch (fontAttr.Length)
                {
                    case 2: fontFamily = fontAttr[0]; fontColour = fontAttr[1]; break;
                    case 3: fontFamily = fontAttr[0]; fontColour = fontAttr[1]; fontWeight = fontAttr[2]; break;
                    case 4: fontFamily = fontAttr[0]; fontColour = fontAttr[1]; fontWeight = fontAttr[2]; fontStyle = fontAttr[3];  break;
                    case 5: fontFamily = fontAttr[0]; fontColour = fontAttr[1]; fontWeight = fontAttr[2]; fontStyle = fontAttr[3]; fontVariant = fontAttr[4]; break;
                    default:
                        break;
                }
            }
            else
                fontFamily = Font;


            // Put HtmlTextWriter in using block because it needs to call Dispose.
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                //positioning stuff
                writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "absolute");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Top, Y.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + "px;");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Left, X.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + "px;");
                if (Height > 0) writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + "px;");
                if (Width > 0) writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + "px;");
                
                //font stuff
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontStyle, fontStyle.Trim());
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontVariant, fontVariant.Trim());
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, fontWeight.Trim());
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, fontSize.Trim());
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, fontFamily.Trim());
                writer.AddStyleAttribute(HtmlTextWriterStyle.Color, fontColour.Trim());

                //misc
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline-block");
                writer.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "pre");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write(HttpUtility.HtmlEncode((Label ?? String.Empty) + Value!=null?  System.Text.Encoding.UTF8.GetString(Value):String.Empty));
                writer.RenderEndTag();
            }

            // Return the result.
            return stringWriter.ToString();
        }

        /// <summary>
        /// Generates the HTML for this PrintField. String element will generate a Span tag.
        /// </summary>
        /// <returns></returns>
        private string GenerateImageFieldHTML()
        {
            StringWriter stringWriter = new StringWriter();

            // Put HtmlTextWriter in using block because it needs to call Dispose.
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "absolute");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Top, Y.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + "px;" + "px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Left, X.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + "px;" + "px");
                //TODO: Fix the image format type, currently only expects jpg.
                writer.AddAttribute(HtmlTextWriterAttribute.Src, "data:image/jpg;base64," + Convert.ToBase64String(Value));
                writer.AddAttribute(HtmlTextWriterAttribute.Width, Width.ToString("G", CultureInfo.CreateSpecificCulture("en-US")));
                writer.AddAttribute(HtmlTextWriterAttribute.Height, Height.ToString("G", CultureInfo.CreateSpecificCulture("en-US")));
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, "");
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();
            }

            // Return the result.
            return stringWriter.ToString();
        }
    }
}