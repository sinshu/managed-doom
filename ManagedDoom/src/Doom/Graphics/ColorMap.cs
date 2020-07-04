using System;

namespace ManagedDoom
{
    public sealed class ColorMap
    {
        public static readonly int Inverse = 32;

        private byte[][] data;

        public ColorMap(Wad wad)
        {
            var raw = wad.ReadLump("COLORMAP");
            var num = raw.Length / 256;
            data = new byte[num][];
            for (var i = 0; i < num; i++)
            {
                data[i] = new byte[256];
                var offset = 256 * i;
                for (var c = 0; c < 256; c++)
                {
                    data[i][c] = raw[offset + c];
                }
            }
        }

        public byte[] this[int index]
        {
            get
            {
                return data[index];
            }
        }

        public byte[] FullBright
        {
            get
            {
                return data[0];
            }
        }
    }
}
