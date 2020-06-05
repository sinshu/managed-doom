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
            screen.FillRect(0, 0, screen.Width, screen.Height, 120);
        }
    }
}
