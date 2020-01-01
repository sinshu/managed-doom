using System;

namespace ManagedDoom
{
    public sealed class Palette
    {
        byte[] data;

        public Palette(Wad wad)
        {
            data = wad.ReadLump("PLAYPAL");
        }

        public byte[] Data => data;
    }
}
