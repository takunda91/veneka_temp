using indigoCardIssuingWeb.CardIssuanceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.utility.ProductFieldUtility
{
    public sealed class ProductFieldControl
    {
        public const string LABEL_ID = "lbl_{0}";
        public const string TEXTBOX_ID = "tb_{0}";
        public const string FILEUPLOAD_ID = "fu_{0}";


        public static Tuple<Label, System.Web.UI.Control> Create(ProductField productField)
        {
            //Create label for field
            Label lbl = new Label();
            lbl.ID = GetLabelControlID(productField);
            lbl.Text = productField.Name;
            lbl.CssClass = "label leftclr";

            if (productField.ProductPrintFieldTypeId == 0)
            {
                //Create textbox for text field
                TextBox tb = new TextBox();
                tb.ID = GetInputControlID(productField);
                tb.CssClass = "input";
                tb.Enabled = productField.Editable;

                //Populate value if its available
                if (productField.Value != null && productField.Value.Length > 0)
                    tb.Text = System.Text.Encoding.UTF8.GetString(productField.Value);

                return new Tuple<Label, System.Web.UI.Control>(lbl, tb);
            }
            else if(productField.ProductPrintFieldTypeId == 1)
            {                
                //Create fileupload for image field
                FileUpload fu = new FileUpload();
                fu.ID = GetInputControlID(productField);
                fu.CssClass = "input";
                fu.Enabled = productField.Editable;
                
                return new Tuple<Label, System.Web.UI.Control>(lbl, fu);
            }
            else
            {
                throw new Exception("Unkown Product Print Field Type of " + productField.ProductPrintFieldTypeId);
            }            
        }

        public static string GetLabelControlID(ProductField productField)
        {
            return String.Format(LABEL_ID, productField.ProductPrintFieldId);
        }

        public static string GetInputControlID(ProductField productField)
        {
            switch (productField.ProductPrintFieldTypeId)
            {
                case 0: return String.Format(TEXTBOX_ID, productField.ProductPrintFieldId);
                case 1: return String.Format(FILEUPLOAD_ID, productField.ProductPrintFieldId);
                default: throw new ArgumentException("Unknown product print field type ID of " + productField.ProductPrintFieldTypeId, "productField");
            }
        }

        public static bool TryGetValue(System.Web.UI.Control control, ProductField productField, out byte[] value)
        {
            value = null;

            if (control == null)
                throw new ArgumentNullException("control", "Control cannot be null.");

            if (productField == null)
                throw new ArgumentNullException("productField", "Cannot be null.");

            if(0 > productField.ProductPrintFieldTypeId && productField.ProductPrintFieldTypeId > 1)
                throw new ArgumentException("Unknown product print field type ID of " + productField.ProductPrintFieldTypeId, "productField");


            if (productField.ProductPrintFieldTypeId == 0 && control is TextBox)
            {                
                value = System.Text.Encoding.UTF8.GetBytes(((TextBox)control).Text ?? "");
                return true;
            }
            else if (productField.ProductPrintFieldTypeId == 1 && control is FileUpload)
            {
                var fileUpload = (FileUpload)control;
                if (fileUpload.HasFile)
                {
                    value = fileUpload.FileBytes;
                    return true;
                }                
            }
            else
                throw new ArgumentException("Control type does not match field type.");

            return false;
        }
    }
}