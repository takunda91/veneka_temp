using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer
{
    public interface ICardImageDetail
    {
        byte[] ImageData { get; }
        float X { get; }
        float Y { get; }
        float Width { get; }
        float Height { get; }
    }
}
