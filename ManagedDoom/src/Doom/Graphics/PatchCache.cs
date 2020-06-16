using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class PatchCache
    {
        private Wad wad;
        private Dictionary<string, Patch> cache;

        public PatchCache(Wad wad)
        {
            this.wad = wad;

            cache = new Dictionary<string, Patch>();
        }

        public Patch this[string name]
        {
            get
            {
                Patch patch;
                if (!cache.TryGetValue(name, out patch))
                {
                    patch = Patch.FromWad(name, wad);
                    cache.Add(name, patch);
                }
                return patch;
            }
        }

        public int GetWidth(string name)
        {
            return this[name].Width;
        }

        public int GetHeight(string name)
        {
            return this[name].Height;
        }
    }
}
