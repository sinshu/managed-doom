using System;

namespace ManagedDoom
{
    public sealed class Palette
    {
        byte[] data;

        public Palette(Wad wad)
        {
            data = wad.ReadLump("PLAYPAL");

            GammaCorrectionTest();
        }

        private void GammaCorrectionTest()
        {
            for (var i = 0; i < 256; i++)
            {
                var offset = 3 * i;

                var r = data[offset];
                var g = data[offset + 1];
                var b = data[offset + 2];

                r = (byte)Math.Round(255 * CorrectionCurve(r / 255.0));
                g = (byte)Math.Round(255 * CorrectionCurve(g / 255.0));
                b = (byte)Math.Round(255 * CorrectionCurve(b / 255.0));

                data[offset] = r;
                data[offset + 1] = g;
                data[offset + 2] = b;
            }
        }

        public static double CorrectionCurve(double x)
        {
            return Math.Pow(x, 0.9);
        }

        public byte[] Data => data;
    }
}
