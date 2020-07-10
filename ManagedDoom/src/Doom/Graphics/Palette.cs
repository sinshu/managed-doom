using System;

namespace ManagedDoom
{
    public sealed class Palette
    {
        public static readonly int STARTREDPALS = 1;
        public static readonly int STARTBONUSPALS = 9;
        public static readonly int NUMREDPALS = 8;
        public static readonly int NUMBONUSPALS = 4;
        public static readonly int RADIATIONPAL = 13;

        private byte[] data;

        private uint[][] palettes;

        public Palette(Wad wad)
        {
            data = wad.ReadLump("PLAYPAL");

            var count = data.Length / (3 * 256);
            palettes = new uint[count][];
            for (var i = 0; i < palettes.Length; i++)
            {
                palettes[i] = new uint[256];
            }
        }

        public void ResetColors(double p)
        {
            for (var i = 0; i < palettes.Length; i++)
            {
                var paletteOffset = (3 * 256) * i;
                for (var j = 0; j < 256; j++)
                {
                    var colorOffset = paletteOffset + 3 * j;

                    var r = data[colorOffset];
                    var g = data[colorOffset + 1];
                    var b = data[colorOffset + 2];

                    r = (byte)Math.Round(255 * CorrectionCurve(r / 255.0, p));
                    g = (byte)Math.Round(255 * CorrectionCurve(g / 255.0, p));
                    b = (byte)Math.Round(255 * CorrectionCurve(b / 255.0, p));

                    palettes[i][j] = (uint)((r << 0) | (g << 8) | (b << 16) | (255 << 24));
                }
            }
        }

        private static double CorrectionCurve(double x, double p)
        {
            return Math.Pow(x, p);
        }

        public uint[] this[int paletteNumber]
        {
            get
            {
                return palettes[paletteNumber];
            }
        }
    }
}
