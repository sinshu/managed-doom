using System;

namespace ManagedDoom
{
    public sealed class Reject
    {
        private byte[] data;
        private int sectorCount;

        private Reject(byte[] data, int sectorCount)
        {
            this.data = data;
            this.sectorCount = sectorCount;
        }

        public static Reject FromWad(Wad wad, int lump, Sector[] sectors)
        {
            return new Reject(wad.ReadLump(lump), sectors.Length);
        }

        public bool Check(Sector sector1, Sector sector2)
        {
            var s1 = sector1.Number;
            var s2 = sector2.Number;

            var pnum = s1 * sectorCount + s2;
            var bytenum = pnum >> 3;
            var bitnum = 1 << (pnum & 7);

            return (data[bytenum] & bitnum) != 0;
        }

        public void Dump(string path, Sector[] sectors)
        {
            using (var writer = new System.IO.StreamWriter(path))
            {
                for (var s1 = 0; s1 < sectorCount; s1++)
                {
                    for (var s2 = 0; s2 < sectorCount; s2++)
                    {
                        var result = Check(sectors[s1], sectors[s2]);
                        writer.Write(result ? "1" : "0");
                        writer.Write(",");
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}
