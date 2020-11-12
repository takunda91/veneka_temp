using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.ProductPrinting;
using Zebra.Sdk.Card.Graphics;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra
{
    public class ZebraCardTextDetail : ICardTextDetail
    {
        public ZebraCardTextDetail(string text, float x, float y, string font, float fontSize, int fontColourARGB, FontStyle fontType)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (string.IsNullOrWhiteSpace(font))
            {
                throw new ArgumentNullException(nameof(font));
            }

            Text = text;
            X = x;
            Y = y;
            Font = font;
            FontSize = fontSize;
            FontColourARGB = fontColourARGB;
            FontType = fontType;
        }

        #region Properties
        public string Text { get; private set; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public string Font { get; private set; }

        public float FontSize { get; private set; }

        public int FontColourARGB { get; private set; }

        public FontStyle FontType { get; private set; }
        #endregion

        public void Add(ZebraCardGraphics zebraCardGraphics)
        {
            using (Font printFont = new Font(Font, FontSize, FontType))
            {
                zebraCardGraphics.DrawText(Text, printFont, Color.FromArgb(FontColourARGB), (int)Math.Round(X), (int)Math.Round(Y));
            }
        }
    }
}
