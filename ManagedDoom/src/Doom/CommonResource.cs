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
        private CommonPatches patches;

        private CommonResource()
        {
        }

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
                patches = new CommonPatches(wad);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }
        }

        public static CommonResource CreateDummy(params string[] wadPaths)
        {
            var resource = new CommonResource();
            resource.wad = new Wad(wadPaths);
            resource.palette = new Palette(resource.wad);
            resource.colorMap = new ColorMap(resource.wad);
            resource.textures = new TextureLookup(resource.wad, true);
            resource.flats = new FlatLookup(resource.wad, true);
            resource.sprites = new SpriteLookup(resource.wad, true);
            return resource;
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
        public CommonPatches Patches => patches;
    }
}
