using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Simulator
{
    public class PrintSimCardTextDetail : ICardTextDetail
    {
        public PrintSimCardTextDetail(string text, float x, float y, string font, float fontSize, int fontColourARGB, FontStyle fontType)
        {
            Text = text;
            X = x;
            Y = y;
            Font = font;
            FontSize = fontSize;
            FontColourARGB = fontColourARGB;
            FontType = fontType;
        }

        public string Text { get; private set; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public string Font { get; private set; }

        public float FontSize { get; private set; }

        public int FontColourARGB { get; private set; }

        public FontStyle FontType { get; private set; }
    }
}
