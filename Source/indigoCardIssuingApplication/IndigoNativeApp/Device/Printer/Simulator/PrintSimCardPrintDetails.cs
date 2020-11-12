using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Simulator
{
    public class PrintSimCardPrintDetails : ICardPrintDetails
    {
        public PrintSimCardPrintDetails(ICardTextDetail[] frontPanelText, ICardTextDetail[] backPanelText, ICardImageDetail[] frontPanelImages, ICardImageDetail[] backPanelImages)
        {
            FrontPanelText = frontPanelText;
            BackPanelText = backPanelText;
            FrontPanelImages = frontPanelImages;
            BackPanelImages = backPanelImages;
        }

        public ICardTextDetail[] FrontPanelText { get; private set; }

        public ICardImageDetail[] FrontPanelImages { get; private set; }

        public ICardTextDetail[] BackPanelText { get; private set; }

        public ICardImageDetail[] BackPanelImages { get; private set; }
    }
}
