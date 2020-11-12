using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Simulator
{
    public class PrintSimDetailFactory : PrinterDetailFactory
    {
        public PrintSimDetailFactory()
        {

        }

        internal override ICardImageDetail CreateImageDetailObject(ProductField productField)
        {
            return new PrintSimCardImageDetail(productField.Value, productField.X, productField.Y, productField.Width, productField.Height);
        }

        internal override ICardPrintDetails CreatePrintDetailsObject(ICardTextDetail[] frontPanelText, ICardTextDetail[] backPanelText, ICardImageDetail[] frontPanelImages, ICardImageDetail[] backPanelImages)
        {
            return new PrintSimCardPrintDetails(frontPanelText, backPanelText, frontPanelImages, backPanelImages);
        }

        internal override ICardTextDetail CreateTextDetailObject(ProductField productField)
        {
            return new PrintSimCardTextDetail(productField.ValueToString(), productField.X, productField.Y, productField.Font, productField.FontSize, productField.FontColourRGB, System.Drawing.FontStyle.Regular);
        }
    }
}
