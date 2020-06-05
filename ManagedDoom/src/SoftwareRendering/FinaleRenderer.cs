using System;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class FinaleRenderer
    {
        private Wad wad;
        private FlatLookup flats;
        private SpriteLookup sprites;

        private DrawScreen screen;

        public FinaleRenderer(CommonResource resource, DrawScreen screen)
        {
            wad = resource.Wad;
            flats = resource.Flats;
            sprites = resource.Sprites;

            this.screen = screen;
        }

        public void Render(Finale finale)
        {
            FillFlat(flats[finale.Flat]);
        }

        private void FillFlat(Flat flat)
        {
            var src = flat.Data;
            var dst = screen.Data;
            var scale = screen.Width / 320;
            var xFrac = Fixed.One / scale - Fixed.Epsilon;
            var step = Fixed.One / scale;
            for (var x = 0; x < screen.Width; x++)
            {
                var yFrac = Fixed.One / scale - Fixed.Epsilon;
                var p = screen.Height * x;
                for (var y = 0; y < screen.Height; y++)
                {
                    var spotX = xFrac.ToIntFloor() & 0x3F;
                    var spotY = yFrac.ToIntFloor() & 0x3F;
                    dst[p] = src[(spotY << 6) + spotX];
                    yFrac += step;
                    p++;
                }
                xFrac += step;
            }
        }
    }
}
