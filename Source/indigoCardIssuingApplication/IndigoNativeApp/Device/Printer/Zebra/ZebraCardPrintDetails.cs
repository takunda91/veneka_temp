using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra
{
    public class ZebraCardPrintDetails : ICardPrintDetails
    {
        public ZebraCardPrintDetails(ICardTextDetail[] frontPanelText, ICardTextDetail[] backPanelText, ICardImageDetail[] frontPanelImages, ICardImageDetail[] backPanelImages)
        {
            FrontPanelText = frontPanelText;
            BackPanelText = backPanelText;
            FrontPanelImages = frontPanelImages;
            BackPanelImages = backPanelImages;
        }

        #region Properties
        public ICardTextDetail[] FrontPanelText { get; private set; }

        public ICardImageDetail[] FrontPanelImages { get; private set; }

        public ICardTextDetail[] BackPanelText { get; private set; }

        public ICardImageDetail[] BackPanelImages { get; private set; }
        #endregion

        #region Zebra Properties
        public ZebraCardTextDetail[] FrontPanelZebraText
        {
            get { return Array.ConvertAll(FrontPanelText, item => (ZebraCardTextDetail)item); }
        }

        public ZebraCardImageDetail[] FrontPanelZebraImages
        {
            get { return Array.ConvertAll(FrontPanelImages, item => (ZebraCardImageDetail)item); }
                
        }

        public ZebraCardTextDetail[] BackPanelZebraText
        {
            get { return Array.ConvertAll(BackPanelText, item => (ZebraCardTextDetail)item); }
        }

        public ZebraCardImageDetail[] BackPanelZebraImages
        {
            get { return Array.ConvertAll(BackPanelImages, item => (ZebraCardImageDetail)item); }
        }
        #endregion        
    }
}
