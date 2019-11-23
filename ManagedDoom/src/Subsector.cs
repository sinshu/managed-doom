using System;

namespace ManagedDoom
{
    public sealed class Subsector
    {
        public const int DataSize = 4;

        private Sector sector;
        private int segCount;
        private int firstSeg;

        public Subsector(Sector sector, int segCount, int firstSeg)
        {
            this.sector = sector;
            this.segCount = segCount;
            this.firstSeg = firstSeg;
        }

        public static Subsector FromData(byte[] data, int offset, Seg[] segs)
        {
            var segCount = BitConverter.ToInt16(data, offset);
            var firstSegNumber = BitConverter.ToInt16(data, offset + 2);

            return new Subsector(
                segs[firstSegNumber].SideDef.Sector,
                segCount,
                firstSegNumber);
        }

        public static Subsector[] FromWad(Wad wad, int lump, Seg[] segs)
        {
            var length = wad.GetLumpSize(lump);
            if (length % Subsector.DataSize != 0)
            {
                throw new Exception();
            }

            var data = wad.ReadLump(lump);
            var count = length / Subsector.DataSize;
            var subsectors = new Subsector[count];

            for (var i = 0; i < count; i++)
            {
                var offset = Subsector.DataSize * i;
                subsectors[i] = Subsector.FromData(data, offset, segs);
            }

            return subsectors;
        }

        public Sector Sector => sector;
        public int SegCount => segCount;
        public int FirstSeg => firstSeg;
    }
}
