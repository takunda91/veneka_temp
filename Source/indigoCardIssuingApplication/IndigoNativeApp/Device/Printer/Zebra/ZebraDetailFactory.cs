using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra
{
    public class ZebraDetailFactory : PrinterDetailFactory
    {
        public ZebraDetailFactory()
        {

        }

        internal override ICardImageDetail CreateImageDetailObject(ProductField productField)
        {
            return new ZebraCardImageDetail(productField.Value, productField.X, productField.Y, productField.Width, productField.Height);
        }

        internal override ICardPrintDetails CreatePrintDetailsObject(ICardTextDetail[] frontPanelText, ICardTextDetail[] backPanelText, ICardImageDetail[] frontPanelImages, ICardImageDetail[] backPanelImages)
        {
            return new ZebraCardPrintDetails(frontPanelText, backPanelText, frontPanelImages, backPanelImages);
        }

        internal override ICardTextDetail CreateTextDetailObject(ProductField productField)
        {
            return new ZebraCardTextDetail(productField.ValueToString(), productField.X, productField.Y, productField.Font, productField.FontSize, productField.FontColourRGB, System.Drawing.FontStyle.Regular);
        }
    }
}
