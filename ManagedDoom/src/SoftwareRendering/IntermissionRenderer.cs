using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class IntermissionRenderer
    {
        // GLOBAL LOCATIONS
        private static readonly int WI_TITLEY = 2;
        private static readonly int WI_SPACINGY = 33;

        // SINGPLE-PLAYER STUFF
        private static readonly int SP_STATSX = 50;
        private static readonly int SP_STATSY = 50;

        private static readonly int SP_TIMEX = 16;
        private static readonly int SP_TIMEY = (200 - 32);


        // NET GAME STUFF
        private static readonly int NG_STATSY = 50;
        //private static readonly int NG_STATSX = (32 + SHORT(star->width) / 2 + 32 * !dofrags);

        private static readonly int NG_SPACINGX = 64;


        // DEATHMATCH STUFF
        private static readonly int DM_MATRIXX = 42;
        private static readonly int DM_MATRIXY = 68;

        private static readonly int DM_SPACINGX = 40;

        private static readonly int DM_TOTALSX = 269;

        private static readonly int DM_KILLERSX = 10;
        private static readonly int DM_KILLERSY = 100;
        private static readonly int DM_VICTIMSX = 5;
        private static readonly int DM_VICTIMSY = 50;






        private DrawScreen screen;
        private CommonPatches patches;

        private int scale;

        public Intermission Intermission;

        public IntermissionRenderer(CommonPatches patches, DrawScreen screen)
        {
            this.screen = screen;
            this.patches = patches;

            scale = screen.Width / 320;
        }

        private void DrawPatch(Patch patch, int x, int y)
        {
            screen.DrawPatch(patch, scale * x, scale * y, scale);
        }

        public void Render()
        {
            DrawStats();
        }

        private void DrawStats()
        {
            var im = Intermission;

            // line height
            var lh = (3 * patches.Numbers[0].Height) / 2;

            DrawPatch(patches.Background, 0, 0);

            // draw animated background
            //WI_drawAnimatedBack();

            WI_drawLF();

            DrawPatch(patches.Kills, SP_STATSX, SP_STATSY);
            WI_drawPercent(320 - SP_STATSX, SP_STATSY, im.cnt_kills[0]);

            DrawPatch(patches.Items, SP_STATSX, SP_STATSY + lh);
            WI_drawPercent(320 - SP_STATSX, SP_STATSY + lh, im.cnt_items[0]);

            DrawPatch(patches.SP_Secret, SP_STATSX, SP_STATSY + 2 * lh);
            WI_drawPercent(320 - SP_STATSX, SP_STATSY + 2 * lh, im.cnt_secret[0]);

            DrawPatch(patches.Time, SP_TIMEX, SP_TIMEY);
            WI_drawTime(320 / 2 - SP_TIMEX, SP_TIMEY, im.cnt_time);

            if (im.Wbs.Epsd < 3)
            {
                //V_DrawPatch(SCREENWIDTH / 2 + SP_TIMEX, SP_TIMEY, FB, par);
                //WI_drawTime(SCREENWIDTH - SP_TIMEX, SP_TIMEY, cnt_par);
            }
        }

        // Draws "<Levelname> Finished!"
        private void WI_drawLF()
        {
            var wbs = Intermission.Wbs;

            var y = WI_TITLEY;

            // draw <LevelName> 
            DrawPatch(
                patches.LevelNames[wbs.Last],
                (320 - patches.LevelNames[wbs.Last].Width) / 2, y);

            // draw "Finished!"
            y += (5 * patches.LevelNames[wbs.Last].Height) / 4;

            DrawPatch(
                patches.Finished,
                (320 - patches.Finished.Width) / 2, y);
        }

        //
        // Draws a number.
        // If digits > 0, then use that many digits minimum,
        //  otherwise only use as many as necessary.
        // Returns new x position.
        //

        private int WI_drawNum(int x, int y, int n, int digits)
        {
            var fontwidth = patches.Numbers[0].Width;

            if (digits < 0)
            {
                if (n == 0)
                {
                    // make variable-length zeros 1 digit long
                    digits = 1;
                }
                else
                {
                    // figure out # of digits in #
                    digits = 0;
                    var temp = n;

                    while (temp != 0)
                    {
                        temp /= 10;
                        digits++;
                    }
                }
            }

            var neg = n < 0;

            if (neg)
            {
                n = -n;
            }

            // if non-number, do not draw it
            if (n == 1994)
            {
                return 0;
            }

            // draw the new number
            while (digits-- != 0)
            {
                x -= fontwidth;
                DrawPatch(patches.Numbers[n % 10], x, y);
                n /= 10;
            }

            // draw a minus sign if necessary
            if (neg)
            {
                DrawPatch(patches.Minus, x -= 8, y);
            }

            return x;

        }

        private void WI_drawPercent(int x, int y, int p)
        {
            if (p < 0)
            {
                return;
            }

            DrawPatch(patches.Percent, x, y);
            WI_drawNum(x, y, p, -1);
        }

        //
        // Display level completion time and par,
        //  or "sucks" message if overflow.
        //
        private void WI_drawTime(int x, int y, int t)
        {

            int div;
            int n;

            if (t < 0)
            {
                return;
            }

            if (t <= 61 * 59)
            {
                div = 1;

                do
                {
                    n = (t / div) % 60;
                    x = WI_drawNum(x, y, n, 2) - patches.Colon.Width;
                    div *= 60;

                    // draw
                    if (div == 60 || t / div != 0)
                    {
                        DrawPatch(patches.Colon, x, y);
                    }

                } while (t / div != 0);
            }
            else
            {
                // "sucks"
                DrawPatch(patches.Sucks, x - patches.Sucks.Width, y);
            }
        }
    }
}
