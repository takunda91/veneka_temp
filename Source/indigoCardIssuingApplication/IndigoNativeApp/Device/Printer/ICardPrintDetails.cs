using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.DesktopApp.Device.Printer
{
    /// <summary>
    /// Holder for the front and back panel print details. Contains a method to populate print details object from Indigo ProductFields
    /// </summary>
    public interface ICardPrintDetails
    {
        ICardTextDetail[] FrontPanelText { get; }
        ICardImageDetail[] FrontPanelImages { get; }

        ICardTextDetail[] BackPanelText { get; }
        ICardImageDetail[] BackPanelImages { get; }

        //void Populate(ProductField[] productFields);
    }
}
