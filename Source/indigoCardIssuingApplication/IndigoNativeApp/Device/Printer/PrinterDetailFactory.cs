using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.DesktopApp.Device.Printer
{
    public abstract class PrinterDetailFactory : IPrinterDetailFactory
    {
        internal abstract ICardPrintDetails CreatePrintDetailsObject(ICardTextDetail[] frontPanelText, ICardTextDetail[] backPanelText, ICardImageDetail[] frontPanelImages, ICardImageDetail[] backPanelImages);
        internal abstract ICardTextDetail CreateTextDetailObject(ProductField productField);
        internal abstract ICardImageDetail CreateImageDetailObject(ProductField productField);

        public virtual ICardPrintDetails Populate(ProductField[] productFields)
        {
            if (productFields == null || productFields.Length == 0)
            {
                throw new ArgumentNullException(nameof(productFields));
            }

            int actualFieldsAdded = 0;

            List<ICardTextDetail> frontText = new List<ICardTextDetail>();
            List<ICardTextDetail> backText = new List<ICardTextDetail>();

            List<ICardImageDetail> frontImages = new List<ICardImageDetail>();
            List<ICardImageDetail> backImages = new List<ICardImageDetail>();

            foreach (var field in productFields)
            {
                if (field.Deleted) continue; // Skip any deleted fields that might have been sent by mistake
                if (!field.Printable) continue; // Skip any fields that arent marked as allowed for printing that may have been sent by mistake

                actualFieldsAdded++;

                if (field.ProductPrintFieldTypeId == (int)ProductPrintFieldTypeId.Text)
                {
                    if (field.PrintSide == 0)
                    {
                        frontText.Add(CreateTextDetailObject(field));                        
                    }
                    else if (field.PrintSide == 1)
                    {
                        backText.Add(CreateTextDetailObject(field));                        
                    }
                }
                else if (field.ProductPrintFieldTypeId == (int)ProductPrintFieldTypeId.Image)
                {
                    if (field.PrintSide == 0)
                    {
                        frontImages.Add(CreateImageDetailObject(field));                        
                    }
                    else if (field.PrintSide == 1)
                    {
                        backImages.Add(CreateImageDetailObject(field));                        
                    }
                }
                else
                {
                    throw new ArgumentException("Unknown printer field type in array.", nameof(field));
                }
            }

            if (actualFieldsAdded == 0)
            {
                throw new ArgumentNullException(nameof(productFields));
            }
            
            return CreatePrintDetailsObject(frontText.ToArray(), backText.ToArray(), frontImages.ToArray(), backImages.ToArray());
        }
    }
}
