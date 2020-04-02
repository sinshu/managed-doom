using System;

namespace ManagedDoom
{
    public static class Dummy
    {
        private static Patch dummyPatch;

        public static Patch GetPatch()
        {
            if (dummyPatch != null)
            {
                return dummyPatch;
            }
            else
            {
                var width = 64;
                var height = 128;

                var data = new byte[height + 32];
                for (var y = 0; y < data.Length; y++)
                {
                    data[y] = y / 32 % 2 == 0 ? (byte)80 : (byte)96;
                }

                var columns = new Column[width][];
                var c1 = new Column[] { new Column(0, data, 0, height) };
                var c2 = new Column[] { new Column(0, data, 32, height) };
                for (var x = 0; x < width; x++)
                {
                    columns[x] = x / 32 % 2 == 0 ? c1 : c2;
                }

                dummyPatch = new Patch("DUMMY", width, height, 32, 128, columns);

                return dummyPatch;
            }
        }



        private static Texture dummyTexture;

        public static Texture GetTexture()
        {
            if (dummyTexture != null)
            {
                return dummyTexture;
            }
            else
            {
                var patch = new TexturePatch[] { new TexturePatch(0, 0, GetPatch()) };

                dummyTexture = new Texture("DUMMY", false, 64, 128, patch);

                return dummyTexture;
            }
        }



        private static Flat dummyFlat;

        public static Flat GetFlat()
        {
            if (dummyFlat != null)
            {
                return dummyFlat;
            }
            else
            {
                var data = new byte[64 * 64];
                var spot = 0;
                for (var y = 0; y < 64; y++)
                {
                    for (var x = 0; x < 64; x++)
                    {
                        data[spot] = ((x / 32) ^ (y / 32)) == 0 ? (byte)80 : (byte)96;
                        spot++;
                    }
                }

                dummyFlat = new Flat("DUMMY", data);

                return dummyFlat;
            }
        }



        private static Flat dummySkyFlat;

        public static Flat GetSkyFlat()
        {
            if (dummySkyFlat != null)
            {
                return dummySkyFlat;
            }
            else
            {
                dummySkyFlat = new Flat("DUMMY", GetFlat().Data);

                return dummySkyFlat;
            }
        }
    }
}
