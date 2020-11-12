using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.DesktopApp.Device.Printer
{
    public interface IPrinterDetailFactory
    {
        ICardPrintDetails Populate(ProductField[] productFields);
    }
}