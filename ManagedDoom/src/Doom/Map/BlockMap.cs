using System;

namespace ManagedDoom
{
    public sealed class BlockMap
    {
        public const int MapBlockUnits = 128;
        public const int MapBlockShift = Fixed.FracBits + 7;

        public static readonly Fixed MapBlockSize = Fixed.FromInt(MapBlockUnits);
        public static readonly int MapBMask = MapBlockSize.Data - 1;
        public static readonly int MapBToFrac = MapBlockShift - Fixed.FracBits;

        private Fixed originX;
        private Fixed originY;

        private int width;
        private int height;

        private short[] table;

        private LineDef[] lines;

        private Mobj[] thingLists;

        private BlockMap(
            Fixed originX,
            Fixed originY,
            int width,
            int height,
            short[] table,
            LineDef[] lines)
        {
            this.originX = originX;
            this.originY = originY;
            this.width = width;
            this.height = height;
            this.table = table;
            this.lines = lines;

            thingLists = new Mobj[width * height];
        }

        public static BlockMap FromWad(Wad wad, int lump, LineDef[] lines)
        {
            var data = wad.ReadLump(lump);

            var table = new short[data.Length / 2];
            for (var i = 0; i < table.Length; i++)
            {
                var offset = 2 * i;
                table[i] = BitConverter.ToInt16(data, offset);
            }

            var originX = Fixed.FromInt(table[0]);
            var originY = Fixed.FromInt(table[1]);
            var width = table[2];
            var height = table[3];

            return new BlockMap(
                originX,
                originY,
                width,
                height,
                table,
                lines);
        }

        public int GetBlockX(Fixed x)
        {
            return (x - originX).Data >> MapBlockShift;
        }

        public int GetBlockY(Fixed y)
        {
            return (y - originY).Data >> MapBlockShift;
        }

        public int GetIndex(int blockX, int blockY)
        {
            if (0 <= blockX && blockX < width && 0 <= blockY && blockY < height)
            {
                return width * blockY + blockX;
            }
            else
            {
                return -1;
            }
        }

        public int GetIndex(Fixed x, Fixed y)
        {
            var blockX = GetBlockX(x);
            var blockY = GetBlockY(y);
            return GetIndex(blockX, blockY);
        }

        public bool IterateLines(int blockX, int blockY, Func<LineDef, bool> func, int validCount)
        {
            var index = GetIndex(blockX, blockY);

            if (index == -1)
            {
                return true;
            }

            for (var offset = table[4 + index]; table[offset] != -1; offset++)
            {
                var line = lines[table[offset]];

                if (line.ValidCount == validCount)
                {
                    continue;
                }

                line.ValidCount = validCount;

                if (!func(line))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IterateThings(int blockX, int blockY, Func<Mobj, bool> func)
        {
            var index = GetIndex(blockX, blockY);

            if (index == -1)
            {
                return true;
            }

            for (var mobj = thingLists[index]; mobj != null; mobj = mobj.BNext)
            {
                if (!func(mobj))
                {
                    return false;
                }
            }

            return true;
        }

        public Fixed OriginX => originX;
        public Fixed OriginY => originY;
        public int Width => width;
        public int Height => height;
        public Mobj[] ThingLists => thingLists;
    }
}
