using System;

namespace ManagedDoom
{
    public sealed class Column
    {
        public const int Last = 0xFF;

        private int topDelta;
        private byte[] data;
        private int offset;
        private int length;

        public Column(int topDelta, byte[] data, int offset, int length)
        {
            this.topDelta = topDelta;
            this.data = data;
            this.offset = offset;
            this.length = length;
        }

        public int TopDelta => topDelta;
        public byte[] Data => data;
        public int Offset => offset;
        public int Length => length;
    }
}
