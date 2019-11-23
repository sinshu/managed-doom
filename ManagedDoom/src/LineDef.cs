using System;

namespace ManagedDoom
{
    public sealed class LineDef
    {
        public const int DataSize = 14;

        private Vertex vertex1;
        private Vertex vertex2;

        private Fixed dx;
        private Fixed dy;

        private LineFlags flags;
        private LineSpecial special;
        private short tag;

        private SideDef side0;
        private SideDef side1;

        private Fixed bboxTop;
        private Fixed bboxBottom;
        private Fixed bboxLeft;
        private Fixed bboxRight;

        private SlopeType slopeType;

        private Sector frontSector;
        private Sector backSector;

        public LineDef(
            Vertex vertex1,
            Vertex vertex2,
            LineFlags flags,
            LineSpecial special,
            short tag,
            SideDef side0,
            SideDef side1)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.flags = flags;
            this.special = special;
            this.tag = tag;
            this.side0 = side0;
            this.side1 = side1;

            dx = vertex2.X - vertex1.X;
            dy = vertex2.Y - vertex1.Y;

            if (dx == Fixed.Zero)
            {
                slopeType = SlopeType.Vertical;
            }
            else if (dy == Fixed.Zero)
            {
                slopeType = SlopeType.Horizontal;
            }
            else
            {
                if (dy / dx > Fixed.Zero)
                {
                    slopeType = SlopeType.Positive;
                }
                else
                {
                    slopeType = SlopeType.Negative;
                }
            }

            bboxTop = Fixed.Max(vertex1.Y, vertex2.Y);
            bboxBottom = Fixed.Min(vertex1.Y, vertex2.Y);
            bboxLeft = Fixed.Min(vertex1.X, vertex2.X);
            bboxRight = Fixed.Max(vertex1.X, vertex2.X);

            frontSector = side0.Sector;
            backSector = side1?.Sector;
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
            if (length % 14 != 0)
            {
                throw new Exception();
            }

            var data = wad.ReadLump(lump);
            var count = length / 14;
            var lines = new LineDef[count]; ;

            for (var i = 0; i < count; i++)
            {
                var offset = 14 * i;
                lines[i] = LineDef.FromData(data, offset, vertices, sides);
            }

            return lines;
        }

        public Vertex Vertex1 => vertex1;
        public Vertex Vertex2 => vertex2;

        public Fixed Dx => dx;
        public Fixed Dy => dy;

        public LineFlags Flags => flags;
        public LineSpecial Special => special;
        public int Tag => tag;

        public SideDef Side0 => side0;
        public SideDef Side1 => side1;

        public Fixed BboxTop => bboxTop;
        public Fixed BboxBottom => bboxBottom;
        public Fixed BboxLeft => bboxLeft;
        public Fixed BboxRight => bboxRight;

        public SlopeType SlopeType => slopeType;

        public Sector FrontSector => frontSector;
        public Sector BackSector => backSector;
    }
}
