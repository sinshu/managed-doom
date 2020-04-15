using System;

namespace ManagedDoom
{
    public sealed class Vertex
    {
        public const int DataSize = 4;

        private Fixed x;
        private Fixed y;

        public Vertex(Fixed x, Fixed y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vertex FromData(byte[] data, int offset)
        {
            var x = BitConverter.ToInt16(data, offset);
            var y = BitConverter.ToInt16(data, offset + 2);

            return new Vertex(Fixed.FromInt(x), Fixed.FromInt(y));
        }

        public static Vertex[] FromWad(Wad wad, int lump)
        {
            var length = wad.GetLumpSize(lump);
            if (length % DataSize != 0)
            {
                throw new Exception();
            }

            var data = wad.ReadLump(lump);
            var count = length / DataSize;
            var vertices = new Vertex[count]; ;

            for (var i = 0; i < count; i++)
            {
                var offset = DataSize * i;
                vertices[i] = FromData(data, offset);
            }

            return vertices;
        }

        public Fixed X => x;
        public Fixed Y => y;
    }
}
