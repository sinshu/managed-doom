using System;

namespace ManagedDoom
{
    public sealed class Node
    {
        public const int DataSize = 28;

        private Fixed x;
        private Fixed y;
        private Fixed dx;
        private Fixed dy;

        private Fixed[][] bbox;

        private int[] children;

        public Node(
            Fixed x, Fixed y,
            Fixed dx, Fixed dy,
            Fixed bbox0Top, Fixed bbox0Bottom, Fixed bbox0Left, Fixed bbox0Right,
            Fixed bbox1Top, Fixed bbox1Bottom, Fixed bbox1Left, Fixed bbox1Right,
            int children0, int children1)
        {
            this.x = x;
            this.y = y;
            this.dx = dx;
            this.dy = dy;

            var bbox0 = new Fixed[4]
            {
                bbox0Top,
                bbox0Bottom,
                bbox0Left,
                bbox0Right
            };

            var bbox1 = new Fixed[4]
            {
                bbox1Top,
                bbox1Bottom,
                bbox1Left,
                bbox1Right
            };

            bbox = new Fixed[][]
            {
                bbox0,
                bbox1
            };

            children = new int[]
            {
                children0,
                children1
            };
        }

        public static Node FromData(byte[] data, int offset)
        {
            var x = BitConverter.ToInt16(data, offset);
            var y = BitConverter.ToInt16(data, offset + 2);
            var dx = BitConverter.ToInt16(data, offset + 4);
            var dy = BitConverter.ToInt16(data, offset + 6);
            var bbox0Top = BitConverter.ToInt16(data, offset + 8);
            var bbox0Bottom = BitConverter.ToInt16(data, offset + 10);
            var bbox0Left = BitConverter.ToInt16(data, offset + 12);
            var bbox0Right = BitConverter.ToInt16(data, offset + 14);
            var bbox1Top = BitConverter.ToInt16(data, offset + 16);
            var bbox1Bottom = BitConverter.ToInt16(data, offset + 18);
            var bbox1Left = BitConverter.ToInt16(data, offset + 20);
            var bbox1Right = BitConverter.ToInt16(data, offset + 22);
            var children0 = BitConverter.ToInt16(data, offset + 24);
            var children1 = BitConverter.ToInt16(data, offset + 26);

            return new Node(
                Fixed.FromInt(x), Fixed.FromInt(y),
                Fixed.FromInt(dx), Fixed.FromInt(dy),
                Fixed.FromInt(bbox0Top), Fixed.FromInt(bbox0Bottom), Fixed.FromInt(bbox0Left), Fixed.FromInt(bbox0Right),
                Fixed.FromInt(bbox1Top), Fixed.FromInt(bbox1Bottom), Fixed.FromInt(bbox1Left), Fixed.FromInt(bbox1Right),
                children0, children1);
        }

        public static Node[] FromWad(Wad wad, int lump, Subsector[] subsectors)
        {
            var length = wad.GetLumpSize(lump);
            if (length % Node.DataSize != 0)
            {
                throw new Exception();
            }

            var data = wad.ReadLump(lump);
            var count = length / Node.DataSize;
            var nodes = new Node[count];

            for (var i = 0; i < count; i++)
            {
                var offset = Node.DataSize * i;
                nodes[i] = Node.FromData(data, offset);
            }

            return nodes;
        }

        public static bool IsSubsector(int node)
        {
            return (node & unchecked((int)0xFFFF8000)) != 0;
        }

        public static int GetSubsector(int node)
        {
            return node ^ unchecked((int)0xFFFF8000);
        }

        public Fixed X => x;
        public Fixed Y => y;
        public Fixed Dx => dx;
        public Fixed Dy => dy;
        public Fixed[][] Bbox => bbox;
        public int[] Children => children;
    }
}
