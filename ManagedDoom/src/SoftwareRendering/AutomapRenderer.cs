using System;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class AutomapRenderer
    {
        private DrawScreen screen;

        public AutomapRenderer(DrawScreen screen)
        {
            this.screen = screen;
        }

        public void Render(Player player)
        {
            var scale = screen.Width / 320;
            screen.FillRect(0, 0, screen.Width, screen.Height - 32 * scale, 120);
        }
    }
}
