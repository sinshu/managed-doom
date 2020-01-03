using System;

namespace ManagedDoom
{
    public sealed class Sector
    {
        public const int DataSize = 26;

        public Fixed FloorHeight;
        public Fixed CeilingHeight;
        public int FloorFlat;
        public int CeilingFlat;
        public int LightLevel;
        public SectorSpecial Special;
        public int Tag;

        // 0 = untraversed, 1,2 = sndlines -1
        public int SoundTraversed;

        // thing that made a sound (or null)
        public Mobj SoundTarget;

        // mapblock bounding box for height changes
        public int[] BlockBox;

        // origin for any sounds played by the sector
        public DegenMobj SoundOrigin;

        // if == validcount, already checked
        public int ValidCount;

        // list of mobjs in sector
        public Mobj ThingList;

        // thinker_t for reversable actions
        public object SpecialData;

        public LineDef[] Lines;

        public Sector(
            Fixed floorHeight,
            Fixed ceilingHeight,
            int floorFlat,
            int ceilingFlat,
            int lightLevel,
            SectorSpecial special,
            int tag)
        {
            FloorHeight = floorHeight;
            CeilingHeight = ceilingHeight;
            FloorFlat = floorFlat;
            CeilingFlat = ceilingFlat;
            LightLevel = lightLevel;
            Special = special;
            Tag = tag;
        }

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
    }
}
