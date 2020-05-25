using System;

namespace ManagedDoom.SoftwareRendering
{
    public class OpeningSequenceRenderer
    {
        private Patch titlePic;
        private DrawScreen screen;
        private SfmlRenderer parent;

        public OpeningSequenceRenderer(Wad wad, DrawScreen screen, SfmlRenderer parent)
        {
            titlePic = Patch.FromWad("TITLEPIC", wad);
            this.screen = screen;
            this.parent = parent;
        }

        public void Render(OpeningSequence sequence)
        {
            var scale = screen.Width / 320;
            screen.DrawPatch(titlePic, 0, 0, scale);
        }
    }
}
