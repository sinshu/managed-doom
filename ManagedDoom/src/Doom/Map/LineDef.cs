using System;

namespace ManagedDoom
{
    public sealed class LineDef
    {
        public const int DataSize = 14;

        public Vertex Vertex1;
        public Vertex Vertex2;

        public Fixed Dx;
        public Fixed Dy;

        public LineFlags Flags;
        public LineSpecial Special;
        public short Tag;

        public SideDef Side0;
        public SideDef Side1;

        public Fixed[] Box;

        public SlopeType SlopeType;

        public Sector FrontSector;
        public Sector BackSector;

        public int ValidCount;

        public Thinker SpecialData;

        public DegenMobj SoundOrigin;

        public LineDef(
            Vertex vertex1,
            Vertex vertex2,
            LineFlags flags,
            LineSpecial special,
            short tag,
            SideDef side0,
            SideDef side1)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Flags = flags;
            Special = special;
            Tag = tag;
            Side0 = side0;
            Side1 = side1;

            Dx = vertex2.X - vertex1.X;
            Dy = vertex2.Y - vertex1.Y;

            if (Dx == Fixed.Zero)
            {
                SlopeType = SlopeType.Vertical;
            }
            else if (Dy == Fixed.Zero)
            {
                SlopeType = SlopeType.Horizontal;
            }
            else
            {
                if (Dy / Dx > Fixed.Zero)
                {
                    SlopeType = SlopeType.Positive;
                }
                else
                {
                    SlopeType = SlopeType.Negative;
                }
            }

            Box = new Fixed[4];
            Box[ManagedDoom.Box.Top] = Fixed.Max(vertex1.Y, vertex2.Y);
            Box[ManagedDoom.Box.Bottom] = Fixed.Min(vertex1.Y, vertex2.Y);
            Box[ManagedDoom.Box.Left] = Fixed.Min(vertex1.X, vertex2.X);
            Box[ManagedDoom.Box.Right] = Fixed.Max(vertex1.X, vertex2.X);

            FrontSector = side0?.Sector;
            BackSector = side1?.Sector;
        }

        public static LineDef FromData(byte[] data, int offset, Vertex[] vertices, SideDef[] sides)
        {
            var vertex1Number = BitConverter.ToInt16(data, offset);
            var vertex2Number = BitConverter.ToInt16(data, offset + 2);
            var flags = BitConverter.ToInt16(data, offset + 4);
            var special = BitConverter.ToInt16(data, offset + 6);
            var tag = BitConverter.ToInt16(data, offset + 8);
            var side0Number = BitConverter.ToInt16(data, offset + 10);
            var side1Number = BitConverter.ToInt16(data, offset + 12);

            return new LineDef(
                vertices[vertex1Number],
                vertices[vertex2Number],
                (LineFlags)flags,
                (LineSpecial)special,
                tag,
                sides[side0Number],
                side1Number != -1 ? sides[side1Number] : null);
        }

        public static LineDef[] FromWad(Wad wad, int lump, Vertex[] vertices, SideDef[] sides)
        {
            var length = wad.GetLumpSize(lump);
            if (length % DataSize != 0)
            {
                throw new Exception();
            }

            var data = wad.ReadLump(lump);
            var count = length / DataSize;
            var lines = new LineDef[count]; ;

            for (var i = 0; i < count; i++)
            {
                var offset = 14 * i;
                lines[i] = FromData(data, offset, vertices, sides);
            }

            return lines;
        }
    }
}
