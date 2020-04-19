using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManagedDoom
{
    public sealed class LumpInfo
    {
        public const int DataSize = 16;

        private string name;
        private Stream stream;
        private int position;
        private int size;

        public LumpInfo(string name, Stream stream, int position, int size)
        {
            this.name = name;
            this.stream = stream;
            this.position = position;
            this.size = size;
        }

        public string Name => name;
        public Stream Stream => stream;
        public int Position => position;
        public int Size => size;
    }
}
