using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ManagedDoom
{
    public sealed class FlatLookup : IReadOnlyList<Flat>
    {
        private Flat[] flats;

        private Dictionary<string, Flat> nameToFlat;
        private Dictionary<string, int> nameToNumber;

        private int skyFlatNumber;
        private Flat skyFlat;

        public FlatLookup(Wad wad) : this(wad, false)
        {
        }

        public FlatLookup(Wad wad, bool useDummy)
        {
            if (!useDummy)
            {
                Init(wad);
            }
            else
            {
                InitDummy(wad);
            }
        }

        private void Init(Wad wad)
        {
            var firstFlat = wad.GetLumpNumber("F_START") + 1;
            var lastFlat = wad.GetLumpNumber("F_END") - 1;
            var count = lastFlat - firstFlat + 1;

            flats = new Flat[count];

            nameToFlat = new Dictionary<string, Flat>();
            nameToNumber = new Dictionary<string, int>();

            for (var lump = firstFlat; lump <= lastFlat; lump++)
            {
                if (wad.GetLumpSize(lump) != 4096)
                {
                    continue;
                }

                var number = lump - firstFlat;
                var name = wad.LumpInfos[lump].Name;
                var flat = new Flat(name, wad.ReadLump(lump));

                flats[number] = flat;
                nameToFlat[name] = flat;
                nameToNumber[name] = number;
            }

            skyFlatNumber = nameToNumber["F_SKY1"];
            skyFlat = nameToFlat["F_SKY1"];
        }

        private void InitDummy(Wad wad)
        {
            var firstFlat = wad.GetLumpNumber("F_START") + 1;
            var lastFlat = wad.GetLumpNumber("F_END") - 1;
            var count = lastFlat - firstFlat + 1;

            flats = new Flat[count];

            nameToFlat = new Dictionary<string, Flat>();
            nameToNumber = new Dictionary<string, int>();

            for (var lump = firstFlat; lump <= lastFlat; lump++)
            {
                if (wad.GetLumpSize(lump) != 4096)
                {
                    continue;
                }

                var number = lump - firstFlat;
                var name = wad.LumpInfos[lump].Name;
                var flat = name != "F_SKY1" ? Dummy.GetFlat() : Dummy.GetSkyFlat();

                flats[number] = flat;
                nameToFlat[name] = flat;
                nameToNumber[name] = number;
            }

            skyFlatNumber = nameToNumber["F_SKY1"];
            skyFlat = nameToFlat["F_SKY1"];
        }

        public int GetNumber(string name)
        {
            return nameToNumber[name];
        }

        public IEnumerator<Flat> GetEnumerator()
        {
            return ((IEnumerable<Flat>)flats).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return flats.GetEnumerator();
        }

        public int Count => flats.Length;
        public Flat this[int num] => flats[num];
        public Flat this[string name] => nameToFlat[name];
        public int SkyFlatNumber => skyFlatNumber;
        public Flat SkyFlat => skyFlat;
    }
}
