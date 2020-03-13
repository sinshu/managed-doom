using System;
using System.Runtime.ExceptionServices;

namespace ManagedDoom
{
    public sealed class CommonResource : IDisposable
    {
        private Wad wad;
        private Palette palette;
        private ColorMap colorMap;
        private TextureLookup textures;
        private FlatLookup flats;
        private SpriteLookup sprites;

        public CommonResource(params string[] wadPaths)
        {
            try
            {
                wad = new Wad(wadPaths);
                palette = new Palette(wad);
                colorMap = new ColorMap(wad);
                textures = new TextureLookup(wad);
                flats = new FlatLookup(wad);
                sprites = new SpriteLookup(wad);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }
        }

        public void Dispose()
        {
            if (wad != null)
            {
                wad.Dispose();
                wad = null;
            }

            Console.WriteLine("Wad files are disposed.");
        }

        public Wad Wad => wad;
        public Palette Palette => palette;
        public ColorMap ColorMap => colorMap;
        public TextureLookup Textures => textures;
        public FlatLookup Flats => flats;
        public SpriteLookup Sprites => sprites;
    }
}
