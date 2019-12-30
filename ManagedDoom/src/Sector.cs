using System;

namespace ManagedDoom
{
    public sealed class Sector
    {
        public const int DataSize = 26;

        public Sector(
            Fixed floorHeight,
            Fixed ceilingHeight,
            int floorFlat,
            int ceilingFlat,
            int lightLevel,
            SectorSpecial special,
            int tag)
        {
            this.floorHeight = floorHeight;
            this.ceilingHeight = ceilingHeight;
            this.floorFlat = floorFlat;
            this.ceilingFlat = ceilingFlat;
            this.lightLevel = lightLevel;
            this.special = special;
            this.tag = tag;
        }

        private Fixed floorHeight;
        private Fixed ceilingHeight;
        private int floorFlat;
        private int ceilingFlat;
        private int lightLevel;
        private SectorSpecial special;
        private int tag;

        public static Sector FromData(byte[] data, int offset, FlatLookup flats)
        {
            var floorHeight = BitConverter.ToInt16(data, offset);
            var ceilingHeight = BitConverter.ToInt16(data, offset + 2);
            var floorFlatName = DoomInterop.ToString(data, offset + 4, 8);
            var ceilingFlatName = DoomInterop.ToString(data, offset + 12, 8);
            var lightLevel = BitConverter.ToInt16(data, offset + 20);
            var special = BitConverter.ToInt16(data, offset + 22);
            var tag = BitConverter.ToInt16(data, offset + 24);

            return new Sector(
                Fixed.FromInt(floorHeight),
                Fixed.FromInt(ceilingHeight),
                flats.GetNumber(floorFlatName),
                flats.GetNumber(ceilingFlatName),
                lightLevel,
                (SectorSpecial)special,
                tag);
        }

        public static Sector[] FromWad(Wad wad, int lump, FlatLookup flats)
        {
            var length = wad.GetLumpSize(lump);
            if (length % DataSize != 0)
            {
                throw new Exception();
            }

            var data = wad.ReadLump(lump);
            var count = length / DataSize;
            var sectors = new Sector[count]; ;

            for (var i = 0; i < count; i++)
            {
                var offset = DataSize * i;
                sectors[i] = FromData(data, offset, flats);
            }

            return sectors;
        }

        public Fixed FloorHeight => floorHeight;
        public Fixed CeilingHeight => ceilingHeight;
        public int FloorFlat => floorFlat;
        public int CeilingFlat => ceilingFlat;
        public int LightLevel => lightLevel;
        public SectorSpecial Special => special;
        public int Tag => tag;
    }
}
