using System;

namespace ManagedDoom
{
    public sealed class Reject
    {
        private byte[] data;
        private int sectorCount;

        private Reject(byte[] data, int sectorCount)
        {
            this.data = data;
            this.sectorCount = sectorCount;
        }

        public static Reject FromWad(Wad wad, int lump, Sector[] sectors)
        {
            return new Reject(wad.ReadLump(lump), sectors.Length);
        }

        public bool Check(Sector sector1, Sector sector2)
        {
            var s1 = sector1.Number;
            var s2 = sector2.Number;

            var p = s1 * sectorCount + s2;
            var byteIndex = p >> 3;
            var bitIndex = 1 << (p & 7);

            return (data[byteIndex] & bitIndex) != 0;
        }
    }
}
