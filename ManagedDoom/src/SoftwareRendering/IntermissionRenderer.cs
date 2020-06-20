using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

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

        private static readonly string[] mapPictures = new string[]
        {
            "WIMAP0",
            "WIMAP1",
            "WIMAP2"
        };

        private static readonly string[] playerBoxes = new string[]
        {
            "STPB0",
            "STPB1",
            "STPB2",
            "STPB3",
        };

        private static readonly string[] youAreHere = new string[]
        {
            "WIURH0",
            "WIURH1"
        };

        private static readonly string[][] doomLevels;
        private static readonly string[] doom2Levels;

        static IntermissionRenderer()
        {
            doomLevels = new string[4][];
            for (var e = 0; e < 4; e++)
            {
                doomLevels[e] = new string[9];
                for (var m = 0; m < 9; m++)
                {
                    doomLevels[e][m] = "WILV" + e + m;
                }
            }

            doom2Levels = new string[32];
            for (var m = 0; m < 32; m++)
            {
                doom2Levels[m] = "CWILV" + m.ToString("00");
            }
        }


        private Wad wad;
        private DrawScreen screen;      

        private PatchCache cache;

        Patch minus;
        Patch[] numbers;
        Patch percent;
        Patch colon;

        private int scale;

        public IntermissionRenderer(Wad wad, DrawScreen screen)
        {
            this.wad = wad;
            this.screen = screen;

            cache = new PatchCache(wad);

            minus = Patch.FromWad("WIMINUS", wad);
            numbers = new Patch[10];
            for (var i = 0; i < 10; i++)
            {
                numbers[i] = Patch.FromWad("WINUM" + i, wad);
            }
            percent = Patch.FromWad("WIPCNT", wad);
            colon = Patch.FromWad("WICOLON", wad);

            scale = screen.Width / 320;
        }

        private void DrawPatch(Patch patch, int x, int y)
        {
            screen.DrawPatch(patch, scale * x, scale * y, scale);
        }

        private void DrawPatch(string name, int x, int y)
        {
            var scale = screen.Width / 320;
            screen.DrawPatch(cache[name], scale * x, scale * y, scale);
        }

        private int GetWidth(string name)
        {
            return cache.GetWidth(name);
        }

        private int GetHeight(string name)
        {
            return cache.GetHeight(name);
        }

        public void Render(Intermission intermission)
        {
            var im = intermission;
            switch (im.State)
            {
                case IntermissionState.StatCount:
                    if (im.Options.Deathmatch != 0)
                    {
                        WI_DrawDeathmatchStats(im);
                    }
                    else if (im.Options.NetGame)
                    {
                        WI_DrawNetgameStats(im);
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
                DrawPatch("INTERPIC", 0, 0);
            }
            else
            {
                var e = intermission.Options.Episode - 1;
                if (e < mapPictures.Length)
                {
                    DrawPatch(mapPictures[e], 0, 0);
                }
                else
                {
                    DrawPatch("INTERPIC", 0, 0);
                }
            }
        }

        private void DrawStats(Intermission intermission)
        {
            DrawBackground(intermission);

            var im = intermission;

            // Line height.
            var lh = (3 * numbers[0].Height) / 2;

            // Draw animated background.
            WI_drawAnimatedBack(im);

            WI_drawLF(im);

            DrawPatch(
                "WIOSTK", // KILLS
                SP_STATSX,
                SP_STATSY);

            WI_drawPercent(
                320 - SP_STATSX,
                SP_STATSY,
                im.KillCount[0]);

            DrawPatch(
                "WIOSTI", // ITEMS
                SP_STATSX,
                SP_STATSY + lh);

            WI_drawPercent(
                320 - SP_STATSX,
                SP_STATSY + lh,
                im.ItemCount[0]);

            DrawPatch(
                "WISCRT2", // SECRET
                SP_STATSX,
                SP_STATSY + 2 * lh);

            WI_drawPercent(
                320 - SP_STATSX,
                SP_STATSY + 2 * lh,
                im.SecretCount[0]);

            DrawPatch(
                "WITIME", // TIME
                SP_TIMEX,
                SP_TIMEY);

            WI_drawTime(
                320 / 2 - SP_TIMEX,
                SP_TIMEY,
                im.TimeCount);

            if (im.Info.Episode < 3)
            {
                //V_DrawPatch(SCREENWIDTH / 2 + SP_TIMEX, SP_TIMEY, FB, par);
                //WI_drawTime(SCREENWIDTH - SP_TIMEX, SP_TIMEY, cnt_par);
            }
        }

        private void WI_DrawNetgameStats(Intermission im)
        {
            int pwidth = percent.Width;

            DrawBackground(im);

            // Draw animated background.
            WI_drawAnimatedBack(im);

            WI_drawLF(im);

            var NG_STATSX = 32 + GetWidth("STFST01") / 2;
            if (!im.DoFrags)
            {
                NG_STATSX += 32;
            }

            // Draw stat titles (top line).
            DrawPatch(
                "WIOSTK", // KILLS
                NG_STATSX + NG_SPACINGX - GetWidth("WIOSTK"),
                NG_STATSY);

            DrawPatch(
                "WIOSTI", // ITEMS
                NG_STATSX + 2 * NG_SPACINGX - GetWidth("WIOSTI"),
                NG_STATSY);

            DrawPatch(
                "WIOSTS", // SCRT
                NG_STATSX + 3 * NG_SPACINGX - GetWidth("WIOSTS"),
                NG_STATSY);

            if (im.DoFrags)
            {
                DrawPatch(
                    "WIFRGS", // FRAGS
                    NG_STATSX + 4 * NG_SPACINGX - GetWidth("WIFRGS"),
                    NG_STATSY);
            }

            // Draw stats.
            var y = NG_STATSY + GetHeight("WIOSTK");

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!im.Options.Players[i].InGame)
                {
                    continue;
                }

                var x = NG_STATSX;

                DrawPatch(
                    playerBoxes[i],
                    x - GetWidth(playerBoxes[i]),
                    y);

                if (i == im.Options.ConsolePlayer)
                {
                    DrawPatch(
                        "STFST01", // Player face
                        x - GetWidth(playerBoxes[i]),
                        y);
                }

                x += NG_SPACINGX;

                WI_drawPercent(x - pwidth, y + 10, im.KillCount[i]);
                x += NG_SPACINGX;

                WI_drawPercent(x - pwidth, y + 10, im.ItemCount[i]);
                x += NG_SPACINGX;

                WI_drawPercent(x - pwidth, y + 10, im.SecretCount[i]);
                x += NG_SPACINGX;

                if (im.DoFrags)
                {
                    WI_drawNum(x, y + 10, im.FragCount[i], -1);
                }

                y += WI_SPACINGY;
            }
        }

        private void WI_DrawDeathmatchStats(Intermission im)
        {
            DrawBackground(im);

            // Draw animated background.
            WI_drawAnimatedBack(im);

            WI_drawLF(im);

            // Draw stat titles (top line).
            DrawPatch(
                "WIMSTT", // TOTAL
                DM_TOTALSX - GetWidth("WIMSTT") / 2,
                DM_MATRIXY - WI_SPACINGY + 10);

            DrawPatch(
                "WIKILRS", // KILLERS
                DM_KILLERSX,
                DM_KILLERSY);

            DrawPatch(
                "WIVCTMS", // VICTIMS
                DM_VICTIMSX,
                DM_VICTIMSY);

            // Draw player boxes.
            var x = DM_MATRIXX + DM_SPACINGX;
            var y = DM_MATRIXY;

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (im.Options.Players[i].InGame)
                {
                    DrawPatch(
                        playerBoxes[i],
                        x - GetWidth(playerBoxes[i]) / 2,
                        DM_MATRIXY - WI_SPACINGY);

                    DrawPatch(
                        playerBoxes[i],
                        DM_MATRIXX - GetWidth(playerBoxes[i]) / 2,
                        y);

                    if (i == im.Options.ConsolePlayer)
                    {
                        DrawPatch(
                            "STFDEAD0", // Player face (dead)
                            x - GetWidth(playerBoxes[i]) / 2,
                            DM_MATRIXY - WI_SPACINGY);

                        DrawPatch(
                            "STFST01", // Player face
                            DM_MATRIXX - GetWidth(playerBoxes[i]) / 2,
                            y);
                    }
                }
                else
                {
                    // V_DrawPatch(x-SHORT(bp[i]->width)/2,
                    //   DM_MATRIXY - WI_SPACINGY, FB, bp[i]);
                    // V_DrawPatch(DM_MATRIXX-SHORT(bp[i]->width)/2,
                    //   y, FB, bp[i]);
                }

                x += DM_SPACINGX;
                y += WI_SPACINGY;
            }

            // Draw stats.
            y = DM_MATRIXY + 10;
            var w = numbers[0].Width;

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                x = DM_MATRIXX + DM_SPACINGX;

                if (im.Options.Players[i].InGame)
                {
                    for (var j = 0; j < Player.MaxPlayerCount; j++)
                    {
                        if (im.Options.Players[j].InGame)
                        {
                            WI_drawNum(x + w, y, im.DeathmatchFrags[i][j], 2);
                        }

                        x += DM_SPACINGX;
                    }

                    WI_drawNum(DM_TOTALSX + w, y, im.DeathmatchTotals[i], 2);
                }

                y += WI_SPACINGY;
            }
        }

        private void WI_drawNoState(Intermission im)
        {
            WI_drawShowNextLoc(im);
        }

        private void WI_drawShowNextLoc(Intermission im)
        {
            DrawBackground(im);

            // Draw animated background.
            WI_drawAnimatedBack(im);

            if (im.Options.GameMode != GameMode.Commercial)
            {
                if (im.Info.Episode > 2)
                {
                    WI_drawEL(im);
                    return;
                }

                var last = (im.Info.LastLevel == 8) ? im.Info.NextLevel - 1 : im.Info.LastLevel;

                // Draw a splat on taken cities.
                for (var i = 0; i <= last; i++)
                {
                    var x = WorldMap.Locations[im.Info.Episode][i].X;
                    var y = WorldMap.Locations[im.Info.Episode][i].Y;
                    DrawPatch("WISPLAT", x, y);
                }

                // Splat the secret level?
                if (im.Info.DidSecret)
                {
                    var x = WorldMap.Locations[im.Info.Episode][8].X;
                    var y = WorldMap.Locations[im.Info.Episode][8].Y;
                    DrawPatch("WISPLAT", x, y);
                }

                // Draw flashing ptr.
                if (im.ShowYouAreHere)
                {
                    var x = WorldMap.Locations[im.Info.Episode][im.Info.NextLevel].X;
                    var y = WorldMap.Locations[im.Info.Episode][im.Info.NextLevel].Y;
                    WI_drawOnLnode(youAreHere, x, y);
                }
            }

            // draws which level you are entering..
            if ((im.Options.GameMode != GameMode.Commercial) || im.Info.NextLevel != 30)
            {
                WI_drawEL(im);
            }
        }

        // Draws "<Levelname> Finished!"
        private void WI_drawLF(Intermission intermission)
        {
            var wbs = intermission.Info;
            var y = WI_TITLEY;

            string levelName;
            if (intermission.Options.GameMode != GameMode.Commercial)
            {
                var e = intermission.Options.Episode - 1;
                levelName = doomLevels[e][wbs.LastLevel];
            }
            else
            {
                levelName = doom2Levels[wbs.LastLevel];
            }

            // Draw level name. 
            DrawPatch(
                levelName,
                (320 - GetWidth(levelName)) / 2, y);

            // Draw "Finished!".
            y += (5 * GetHeight(levelName)) / 4;

            DrawPatch(
                "WIF",
                (320 - GetWidth("WIF")) / 2, y);
        }

        // Draws "Entering <LevelName>"
        private void WI_drawEL(Intermission im)
        {
            var wbs = im.Info;
            int y = WI_TITLEY;

            string levelName;
            if (im.Options.GameMode != GameMode.Commercial)
            {
                var e = im.Options.Episode - 1;
                levelName = doomLevels[e][wbs.NextLevel];
            }
            else
            {
                levelName = doom2Levels[wbs.NextLevel];
            }

            // Draw "Entering".
            DrawPatch(
                "WIENTER",
                (320 - GetWidth("WIENTER")) / 2, y);

            // Draw level name.
            y += (5 * GetHeight(levelName)) / 4;

            DrawPatch(
                levelName,
                (320 - GetWidth(levelName)) / 2, y);
        }





        //
        // Draws a number.
        // If digits > 0, then use that many digits minimum,
        //  otherwise only use as many as necessary.
        // Returns new x position.
        //

        private int WI_drawNum(int x, int y, int n, int digits)
        {
            var fontwidth = numbers[0].Width;

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
                DrawPatch(numbers[n % 10], x, y);
                n /= 10;
            }

            // draw a minus sign if necessary
            if (neg)
            {
                DrawPatch(minus, x -= 8, y);
            }

            return x;

        }

        private void WI_drawPercent(int x, int y, int p)
        {
            if (p < 0)
            {
                return;
            }

            DrawPatch(percent, x, y);
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
                    x = WI_drawNum(x, y, n, 2) - colon.Width;
                    div *= 60;

                    // draw
                    if (div == 60 || t / div != 0)
                    {
                        DrawPatch(colon, x, y);
                    }

                } while (t / div != 0);
            }
            else
            {
                DrawPatch(
                    "WISUCKS", // SUCKS
                    x - GetWidth("WISUCKS"),
                    y);
            }
        }

        private void WI_drawAnimatedBack(Intermission im)
        {
            if (im.Options.GameMode == GameMode.Commercial)
            {
                return;
            }

            if (im.Info.Episode > 2)
            {
                return;
            }

            for (var i = 0; i < im.Animations.Length; i++)
            {
                var a = im.Animations[i];

                if (a.PatchNumber >= 0)
                {
                    DrawPatch(a.Patches[a.PatchNumber], a.LocationX, a.LocationY);
                }
            }
        }


        private void WI_drawOnLnode(IReadOnlyList<string> c, int x, int y)
        {
            var fits = false;
            var i = 0;

            do
            {
                var patch = cache[c[i]];

                var left = x - patch.LeftOffset;
                var top = y - patch.TopOffset;
                var right = left + patch.Width;
                var bottom = top + patch.Height;

                if (left >= 0 && right < 320 && top >= 0 && bottom < 320)
                {
                    fits = true;
                }
                else
                {
                    i++;
                }

            } while (!fits && i != 2);

            if (fits && i < 2)
            {
                DrawPatch(c[i], x, y);
            }
            else
            {
                throw new Exception("Could not place patch!");
            }
        }
    }
}
