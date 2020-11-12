using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Simulator
{
    public class PrintSimCardImageDetail : ICardImageDetail
    {
        public PrintSimCardImageDetail(byte[] imageData, float x, float y, float width, float height)
        {
            ImageData = imageData;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public byte[] ImageData { get; private set; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public float Width { get; private set; }

        public float Height { get; private set; }
    }
}
