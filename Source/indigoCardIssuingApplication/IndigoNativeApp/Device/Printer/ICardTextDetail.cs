using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer
{
    public interface ICardTextDetail
    {
        string Text { get; }
        float X { get; }
        float Y { get; }
        string Font { get; }
        float FontSize { get; }
        int FontColourARGB { get; }
        FontStyle FontType { get; }
    }
}
