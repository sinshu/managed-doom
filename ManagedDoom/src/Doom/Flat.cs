using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace ManagedDoom
{
    public sealed class Flat
    {
        private string name;
        private byte[] data;

        private Flat(string name, byte[] data)
        {
            this.name = name;
            this.data = data;
        }

        public static Flat FromData(string name, byte[] data)
        {
            return new Flat(name, data);
        }

        /*
        public Bitmap ToBitmap(byte[] palette)
        {
            var bitmap = new Bitmap(64, 64, PixelFormat.Format32bppArgb);
            for (var y = 0; y < 64; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    var value = data[64 * y + x];
                    var r = palette[3 * value];
                    var g = palette[3 * value + 1];
                    var b = palette[3 * value + 2];
                    var color = Color.FromArgb(r, g, b);
                    bitmap.SetPixel(x, y, color);
                }
            }
            return bitmap;
        }
        */

        public override string ToString()
        {
            return name;
        }

        public string Name => name;
        public byte[] Data => data;
    }
}
