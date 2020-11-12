using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zebra.Sdk.Card.Graphics;
using Zebra.Sdk.Card.Graphics.Enumerations;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra
{
    public class ZebraCardImageDetail : ICardImageDetail
    {
        public ZebraCardImageDetail(byte[] imageData, float x, float y, float width, float height)
        {
            if(imageData == null || imageData.Length == 0)
            {
                throw new ArgumentNullException(nameof(imageData));
            }

            ImageData = imageData;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        #region Properties
        public byte[] ImageData { get; private set; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public float Width { get; private set; }

        public float Height { get; private set; }
        #endregion

        public void Add(ZebraCardGraphics zebraCardGraphics)
        {
            zebraCardGraphics.DrawImage(ImageData, (int)Math.Round(X), (int)Math.Round(Y), (int)Math.Round(Width), (int)Math.Round(Height), RotationType.RotateNoneFlipNone);
        }
    }
}
