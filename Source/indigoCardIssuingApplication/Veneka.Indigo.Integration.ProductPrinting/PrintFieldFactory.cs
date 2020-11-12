using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.ProductPrinting
{
    public class PrintFieldFactory
    {
        /// <summary>
        /// Create a new print field object based on the supplied PrintFieldTypeID.
        /// </summary>
        /// <param name="printFieldTypeId"></param>
        /// <returns></returns>
        public static IProductPrintField CreatePrintField(int printFieldTypeId)
        {
            return CreatePrintField(printFieldTypeId, 0, null, 0, 0, 0, 0, 
                                        null, 0, 0, null, null, 0,
                                        false, false, false,0, null);
        }

        public static IProductPrintField CreatePrintField(int printFieldTypeId, string name, float x, float y, 
                                                            float width, float height, string font, int fontSize, 
                                                            string mappedName, string label, int maxLength)
        {
            return CreatePrintField(printFieldTypeId, 0, name, x, y, width, height, 
                                        font, fontSize, 0, mappedName, label, maxLength,
                                        false, false, false,0, null);

        }

        public static IProductPrintField CreatePrintStringField(int productPrintFieldId, string name,
                                                            float x, float y, float width, float height, string font,
                                                            int fontSize, int fontColourRGB, string mappedName, string label,
                                                            int maxLength, bool editable, bool deleted, bool printable,int printside, string value)
        {
            return CreatePrintField(0, productPrintFieldId, name, x, y, width, height, font, fontSize,
                                       fontColourRGB, mappedName, label, maxLength, editable, deleted, printable,printside,
                                       System.Text.Encoding.UTF8.GetBytes(value));
        }

        public static IProductPrintField CreatePrintField(int printFieldTypeId, int productPrintFieldId, string name, 
                                                        float x, float y, float width, float height, string font,
                                                        int fontSize, int fontColourRGB, string mappedName, string label,
                                                        int maxLength, bool editable, bool deleted, bool printable, int printside, byte[] value)
        {
            switch (printFieldTypeId)
            {
                case 0:
                    string strValue = null;
                    if (value != null) { strValue = System.Text.Encoding.UTF8.GetString(value); }
                    return new PrintStringField
                    {
                        ProductPrintFieldId = productPrintFieldId,
                        Name = name,
                        X = x,
                        Y = y,
                        Width = width,
                        Height = height,
                        Font = font,
                        FontSize = fontSize,
                        FontColourRGB = fontColourRGB,
                        MappedName = mappedName,
                        Value = strValue,
                        Label = label,
                        MaxSize = maxLength,
                        Editable = editable,
                        Deleted = deleted,
                        Printable = printable,
                        PrintSide=printside
                    };
                case 1:
                    return new PrintImageField
                    {
                        ProductPrintFieldId = productPrintFieldId,
                        Name = name,
                        X = x,
                        Y = y,
                        Width = width,
                        Height = height,
                        Font = font,
                        FontSize = fontSize,
                        FontColourRGB = fontColourRGB,
                        MappedName = mappedName,
                        Value = value,
                        Label = label,
                        MaxSize = maxLength,
                        Editable = editable,
                        Deleted = deleted,
                        Printable = printable,
                        PrintSide=printside
                    };
                default: throw new ArgumentException("Value of " + printFieldTypeId + " not supported.", nameof(printFieldTypeId));
            }
        }
    }
}
