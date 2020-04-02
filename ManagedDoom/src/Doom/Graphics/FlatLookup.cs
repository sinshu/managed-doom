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

        public FlatLookup(Wad wad) : this(wad, false)
        {
        }

        public FlatLookup(Wad wad, bool useDummy)
        {
            flats = new List<Flat>();
            nameToFlat = new Dictionary<string, Flat>();
            nameToNumber = new Dictionary<string, int>();

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
            foreach (var lump in EnumerateFlats(wad))
            {
                var name = wad.LumpInfos[lump].Name;

                if (nameToFlat.ContainsKey(name))
                {
                    continue;
                }

                var data = wad.ReadLump(lump);

                var flat = Flat.FromData(name, data);

                nameToNumber.Add(name, flats.Count);
                flats.Add(flat);
                nameToFlat.Add(name, flat);

            }

            skyFlatNumber = nameToNumber["F_SKY1"];
            skyFlat = nameToFlat["F_SKY1"];
        }

        private void InitDummy(Wad wad)
        {
            foreach (var lump in EnumerateFlats(wad))
            {
                var name = wad.LumpInfos[lump].Name;

                if (nameToFlat.ContainsKey(name))
                {
                    continue;
                }

                var flat = name != "F_SKY1" ? Dummy.GetFlat() : Dummy.GetSkyFlat();

                nameToNumber.Add(name, flats.Count);
                flats.Add(flat);
                nameToFlat.Add(name, flat);

            }

            skyFlatNumber = nameToNumber["F_SKY1"];
            skyFlat = nameToFlat["F_SKY1"];
        }

        private static IEnumerable<int> EnumerateFlats(Wad wad)
        {
            var flatSection = false;

            for (var lump = wad.LumpInfos.Count - 1; lump >= 0; lump--)
            {
                var name = wad.LumpInfos[lump].Name;

                if (name.StartsWith("F"))
                {
                    if (name.EndsWith("_END"))
                    {
                        flatSection = true;
                        continue;
                    }
                    else if (name.EndsWith("_START"))
                    {
                        flatSection = false;
                        continue;
                    }
                }

                if (flatSection)
                {
                    if (wad.LumpInfos[lump].Size == 4096)
                    {
                        yield return lump;
                    }
                }
            }
        }

        public int GetNumber(string name)
        {
            return nameToNumber[name];
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
