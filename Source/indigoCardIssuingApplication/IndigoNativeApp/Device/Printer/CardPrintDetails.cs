using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer
{
    public abstract class CardPrintDetails : ICardPrintDetails
    {
        public CardPrintDetails(ICardTextDetail[] frontPanelText, ICardTextDetail[] backPanelText, ICardImageDetail[] frontPanelImages, ICardImageDetail[] backPanelImages)
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
