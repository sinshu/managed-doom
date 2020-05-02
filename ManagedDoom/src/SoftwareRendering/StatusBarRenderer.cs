using System;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class StatusBarRenderer
    {
        private DrawScreen screen;
        private CommonPatches patches;

        private int scale;

        public StatusBarRenderer(CommonPatches patches, DrawScreen screen)
        {
            this.screen = screen;
            this.patches = patches;

            scale = screen.Width / 320;
        }

        public void Render(Player player)
        {
            screen.DrawPatch(patches.StatusBar, 0, scale * (200 - 32), scale);
        }
    }
}
