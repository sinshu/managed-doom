using System;
using System.Collections.Generic;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class FinaleRenderer
    {
        private Wad wad;
        private FlatLookup flats;
        private SpriteLookup sprites;

        private DrawScreen screen;
        private int scale;

        private Dictionary<string, Patch> cache;

        public FinaleRenderer(CommonResource resource, DrawScreen screen)
        {
            wad = resource.Wad;
            flats = resource.Flats;
            sprites = resource.Sprites;

            this.screen = screen;
            scale = screen.Width / 320;

            cache = new Dictionary<string, Patch>();
        }

        public void Render(Finale finale)
        {
            if (finale.Stage == 2)
            {
                RenderCast(finale);
                return;
            }

            if (finale.Stage == 0)
            {
                TextWrite(finale);
            }
            else
            {
                switch (finale.Options.Episode)
                {
                    case 1:
                        if (finale.Options.GameMode == GameMode.Retail)
                        {
                            // "CREDIT"
                        }
                        else
                        {
                            // "HELP2"
                        }
                        break;

                    case 2:
                        // "VICTORY2"
                        break;

                    case 3:
                        // F_BunnyScroll();
                        break;

                    case 4:
                        // "ENDPIC"
                        break;
                }
            }
        }

        private void TextWrite(Finale finale)
        {
            FillFlat(flats[finale.Flat]);

            // draw some of the text onto the screen
            var cx = 10 * scale;
            var cy = 17 * scale;
            var ch = 0;

            var count = (finale.Count - 10) / Finale.TextSpeed;
            if (count < 0)
            {
                count = 0;
            }

            for (; count > 0; count--)
            {
                if (ch == finale.Text.Length)
                {
                    break;
                }

                var c = finale.Text[ch++];

                if (c == '\n')
                {
                    cx = 10 * scale;
                    cy += 11 * scale;
                    continue;
                }

                screen.DrawChar(c, cx, cy, scale);

                cx += screen.MeasureChar(c, scale);
            }
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

        private void DrawPatch(string name, int x, int y)
        {
            Patch patch;
            if (!cache.TryGetValue(name, out patch))
            {
                patch = Patch.FromWad(name, wad);
                cache.Add(name, patch);
            }

            var scale = screen.Width / 320;
            screen.DrawPatch(patch, scale * x, scale * y, scale);
        }

        private void RenderCast(Finale finale)
        {
            DrawPatch("BOSSBACK", 0, 0);

            var frame = finale.CastState.Frame & 0x7fff;
            var patch = sprites[finale.CastState.Sprite].Frames[frame].Patches[0];
            if (sprites[finale.CastState.Sprite].Frames[frame].Flip[0])
            {
                screen.DrawPatchFlip(
                    patch,
                    screen.Width / 2,
                    screen.Height - scale * 30,
                    scale);
            }
            else
            {
                screen.DrawPatch(
                    patch,
                    screen.Width / 2,
                    screen.Height - scale * 30,
                    scale);
            }

            var width = screen.MeasureText(finale.CastName, scale);
            screen.DrawText(
                finale.CastName,
                (screen.Width - width) / 2,
                screen.Height - scale * 13,
                scale);
        }
    }
}
