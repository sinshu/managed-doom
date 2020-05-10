using System;
using System.Collections.Generic;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class MenuRenderer
    {
        private Wad wad;
        private DrawScreen screen;

        private Dictionary<string, Patch> patches;

        public MenuRenderer(Wad wad, DrawScreen screen)
        {
            this.wad = wad;
            this.screen = screen;

            patches = new Dictionary<string, Patch>();
        }

        public void Render(DoomMenu menu)
        {

        }

        //
        // M_DrawMainMenu
        //
        private void M_DrawMainMenu()
        {
            DrawPatch(94, 2, "M_DOOM");
        }

        private void DrawPatch(int x, int y, string name)
        {
            Patch patch;
            if (!patches.TryGetValue(name, out patch))
            {
                patch = Patch.FromWad(name, wad);
                patches.Add(name, patch);
            }

            var scale = screen.Width / 320;
            screen.DrawPatch(patch, scale * x, scale * y, scale);
        }
    }
}
