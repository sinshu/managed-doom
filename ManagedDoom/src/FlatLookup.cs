using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ManagedDoom
{
    public sealed class FlatLookup : IReadOnlyList<Flat>
    {
        private List<Flat> flats;
        private Dictionary<string, Flat> nameToFlat;
        private Dictionary<string, int> nameToNumber;
        private int skyFlatNumber;
        private Flat skyFlat;

        public FlatLookup(Wad wad)
        {
            var start = wad.GetLumpNumber("F_START") + 1;
            var end = wad.GetLumpNumber("F_END");

            flats = new List<Flat>();
            nameToFlat = new Dictionary<string, Flat>();
            nameToNumber = new Dictionary<string, int>();

            for (var lump = start; lump < end; lump++)
            {
                var size = wad.GetLumpSize(lump);

                if (size == 0)
                {
                    continue;
                }

                if (size != 4096)
                {
                    continue;
                    throw new Exception("The size of a flat must be 4096.");
                }

                var name = wad.LumpInfos[lump].Name;
                var data = wad.ReadLump(lump);
                var flat = Flat.FromData(name, data);

                // This ugly try-catch is to avoid crash in some PWADs.
                // Need to fix.
                try
                {
                    nameToNumber.Add(name, flats.Count);
                    flats.Add(flat);
                    nameToFlat.Add(name, flat);
                }
                catch
                {
                }
            }

            skyFlatNumber = nameToNumber["F_SKY1"];
            skyFlat = nameToFlat["F_SKY1"];
        }

        public int GetNumber(string name)
        {
            if (nameToFlat.ContainsKey(name))
            {
                return nameToNumber[name];
            }
            else
            {
                // This hack is to avoid crash in some PWADs.
                // Need to fix.
                return 3;
            }
        }

        public IEnumerator<Flat> GetEnumerator()
        {
            return flats.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return flats.GetEnumerator();
        }

        public int Count => flats.Count;
        public Flat this[int num] => flats[num];
        public Flat this[string name] => nameToFlat[name];
        public int SkyFlatNumber => skyFlatNumber;
        public Flat SkyFlat => skyFlat;
    }
}
