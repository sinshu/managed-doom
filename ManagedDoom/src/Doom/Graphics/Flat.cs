using System;

namespace ManagedDoom
{
    public sealed class Flat
    {
        private string name;
        private byte[] data;

        public Flat(string name, byte[] data)
        {
            this.name = name;
            this.data = data;
        }

        public static Flat FromData(string name, byte[] data)
        {
            return new Flat(name, data);
        }

        public override string ToString()
        {
            return name;
        }

        public string Name => name;
        public byte[] Data => data;
    }
}
