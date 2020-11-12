using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.Integration.ProductPrinting
{
    public enum ProductPrintFieldTypeId
    {
        Text = 0,
        Image = 1
    }

    /// <summary>
    /// Used for Webservice transmission. 
    /// </summary>
 
    public class ProductField : IProductPrintField
    {
        public int ProductPrintFieldId { get; set; }
        public int ProductPrintFieldTypeId { get; set; }
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
        public byte[] Value { get; set; }
        public bool Editable { get; set; }
        public bool Deleted { get; set; }
        public int PrintSide { get; set; }
        public bool Printable { get; set; }

        public ProductField() { }
        
        public ProductField(IProductPrintField productPrintField)
        {
            this.ProductPrintFieldId = productPrintField.ProductPrintFieldId;
            this.Name = productPrintField.Name;
            this.X = productPrintField.X;
            this.Y = productPrintField.Y;
            this.Width = productPrintField.Width;
            this.Height = productPrintField.Height;
            this.Font = productPrintField.Font;
            this.FontSize = productPrintField.FontSize;
            this.FontColourRGB = productPrintField.FontColourRGB;
            this.MappedName = productPrintField.MappedName;
            this.Label = productPrintField.Label;
            this.Editable = productPrintField.Editable;
            this.Deleted = productPrintField.Deleted;
            //this.PrintSide = productPrintField.PrintSide;
            this.Printable = productPrintField.Printable;

            if (productPrintField is PrintStringField)
            {
                this.ProductPrintFieldTypeId = 0;
                if(((PrintStringField)productPrintField).Value!=null)
                this.Value = System.Text.Encoding.UTF8.GetBytes(((PrintStringField)productPrintField).Value);
            }
            else if (productPrintField is PrintImageField)
            {
                this.ProductPrintFieldTypeId = 1;
                this.Value = ((PrintImageField)productPrintField).Value;
            }
            else
                throw new ArgumentException("Unknown ProductPrintField type.");

        }

        public string ValueToString()
        {
            return System.Text.Encoding.UTF8.GetString(Value);
        }
       
    }
    
}
