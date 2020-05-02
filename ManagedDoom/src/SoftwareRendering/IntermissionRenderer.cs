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

        public void Render(Intermission intermission)
        {
            var im = intermission;
            switch (im.state)
            {
                case IntermissionState.StatCount:
                    if (im.Options.Deathmatch != 0)
                    {
                        // WI_drawDeathmatchStats();
                    }
                    else if (im.Options.NetGame)
                    {
                        // WI_drawNetgameStats();
                    }
                    else
                    {
                        DrawStats(im);
                    }
                    break;

                case IntermissionState.ShowNextLoc:
                    WI_drawShowNextLoc(im);
                    break;

                case IntermissionState.NoState:
                    WI_drawNoState(im);
                    break;
            }
        }

        private void DrawBackground(Intermission intermission)
        {
            if (intermission.Options.GameMode == GameMode.Commercial)
            {
                DrawPatch(patches.Background, 0, 0);
            }
            else
            {
                var e = intermission.Options.Episode - 1;
                if (e < patches.MapPictures.Count)
                {
                    DrawPatch(patches.MapPictures[e], 0, 0);
                }
                else
                {
                    DrawPatch(patches.Background, 0, 0);
                }
            }
        }

        private void DrawStats(Intermission intermission)
        {
            DrawBackground(intermission);

            var im = intermission;

            // line height
            var lh = (3 * patches.Numbers[0].Height) / 2;

            // draw animated background
            //WI_drawAnimatedBack();

            WI_drawLF(im);

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

        private void WI_drawNoState(Intermission im)
        {
            //snl_pointeron = true;
            WI_drawShowNextLoc(im);
        }

        private void WI_drawShowNextLoc(Intermission im)
        {
            DrawBackground(im);

            // draw animated background
            //WI_drawAnimatedBack();

            if (im.Options.GameMode != GameMode.Commercial)
            {
                if (im.Wbs.Epsd > 2)
                {
                    //WI_drawEL();
                    return;
                }

                var last = (im.Wbs.Last == 8) ? im.Wbs.Next - 1 : im.Wbs.Last;

                // draw a splat on taken cities.
                for (var i = 0; i <= last; i++)
                {
                    //WI_drawOnLnode(i, &splat);
                }

                // splat the secret level?
                if (im.Wbs.DidSecret)
                {
                    //WI_drawOnLnode(8, &splat);
                }

                // draw flashing ptr
                //if (snl_pointeron)
                {
                    //WI_drawOnLnode(wbs->next, yah);
                }
            }

            // draws which level you are entering..
            if ((im.Options.GameMode != GameMode.Commercial) || im.Wbs.Next != 30)
            {
                WI_drawEL(im);
            }
        }

        // Draws "<Levelname> Finished!"
        private void WI_drawLF(Intermission intermission)
        {
            var wbs = intermission.Wbs;
            var y = WI_TITLEY;

            var e = 0;
            if (intermission.Options.GameMode != GameMode.Commercial)
            {
                e = intermission.Options.Episode - 1;
            }

            // draw <LevelName> 
            DrawPatch(
                patches.LevelNames[e][wbs.Last],
                (320 - patches.LevelNames[e][wbs.Last].Width) / 2, y);

            // draw "Finished!"
            y += (5 * patches.LevelNames[e][wbs.Last].Height) / 4;

            DrawPatch(
                patches.Finished,
                (320 - patches.Finished.Width) / 2, y);
        }

        // Draws "Entering <LevelName>"
        private void WI_drawEL(Intermission im)
        {
            int y = WI_TITLEY;

            var e = 0;
            if (im.Options.GameMode != GameMode.Commercial)
            {
                e = im.Options.Episode - 1;
            }

            // draw "Entering"
            DrawPatch(
                patches.Entering,
                (320 - patches.Entering.Width) / 2, y);

            // draw level
            y += (5 * patches.LevelNames[e][im.Wbs.Next].Height) / 4;

            DrawPatch(
                patches.LevelNames[e][im.Wbs.Next],
                (320 - patches.LevelNames[e][im.Wbs.Next].Width) / 2, y);
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
