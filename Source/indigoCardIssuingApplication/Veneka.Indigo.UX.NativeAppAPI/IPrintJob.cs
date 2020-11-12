using System.Collections.Generic;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    public interface IPrintJob
    {
        AppOptions[] ApplicationOptions { get; }
        bool MustReturnCardData { get; }
        string PrintJobId { get; }
        string ProductBin { get; }
        ProductField[] ProductFields { get; }

        Dictionary<string, string> AppOptionToDictionary();
    }
}