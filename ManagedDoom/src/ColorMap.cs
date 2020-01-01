using System;

namespace ManagedDoom
{
    public sealed class ColorMap
    {
        byte[][] data;

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

        public byte[][] Data => data;
    }
}
