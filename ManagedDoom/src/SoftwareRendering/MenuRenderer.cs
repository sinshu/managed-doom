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
            var current = menu.Current;
            DrawMenuPatch(current.Name, current.TitleX, current.TitleY);
            foreach (var item in current.Items)
            {
                var simpleItem = item as SimpleMenuItem;
                if (simpleItem != null)
                {
                    DrawSimpleMenuItem(simpleItem);
                }
            }
        }

        private void DrawMenuPatch(string name, int x, int y)
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

        private void DrawSimpleMenuItem(SimpleMenuItem item)
        {
            DrawMenuPatch(item.Name, item.ItemX, item.ItemY);
        }
    }
}
