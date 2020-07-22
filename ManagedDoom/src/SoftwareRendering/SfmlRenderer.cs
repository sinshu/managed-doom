//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//



﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class SfmlRenderer : IRenderer, IDisposable
    {
        private static double[] gammaCorrectionParameters = new double[]
        {
            1.00,
            0.95,
            0.90,
            0.85,
            0.80,
            0.75,
            0.70,
            0.65,
            0.60,
            0.55,
            0.50
        };

        private RenderWindow sfmlWindow;
        private Palette palette;

        private int sfmlWindowWidth;
        private int sfmlWindowHeight;

        private DrawScreen screen;

        private int sfmlTextureWidth;
        private int sfmlTextureHeight;

        private byte[] sfmlTextureData;
        private SFML.Graphics.Texture sfmlTexture;
        private SFML.Graphics.Sprite sfmlSprite;
        private SFML.Graphics.RenderStates sfmlStates;

        private MenuRenderer menu;
        private ThreeDRenderer threeD;
        private StatusBarRenderer statusBar;
        private IntermissionRenderer intermission;
        private OpeningSequenceRenderer openingSequence;
        private AutoMapRenderer autoMap;
        private FinaleRenderer finale;

        private Patch pause;

        private int wipeBandWidth;
        private int wipeBandCount;
        private int wipeHeight;
        private byte[] wipeBuffer;

        private bool displayMessage;

        private int gammaCorrectionLevel;

        public SfmlRenderer(RenderWindow window, CommonResource resource, bool highResolution)
        {
            sfmlWindow = window;
            palette = resource.Palette;

            sfmlWindowWidth = (int)window.Size.X;
            sfmlWindowHeight = (int)window.Size.Y;

            if (highResolution)
            {
                screen = new DrawScreen(resource.Wad, 640, 400);
                sfmlTextureWidth = 512;
                sfmlTextureHeight = 1024;
            }
            else
            {
                screen = new DrawScreen(resource.Wad, 320, 200);
                sfmlTextureWidth = 256;
                sfmlTextureHeight = 512;
            }

            sfmlTextureData = new byte[4 * screen.Width * screen.Height];

            try
            {
                sfmlTexture = new SFML.Graphics.Texture((uint)sfmlTextureWidth, (uint)sfmlTextureHeight);
                sfmlSprite = new SFML.Graphics.Sprite(sfmlTexture);
            }
            catch (Exception e)
            {
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }

            sfmlSprite.Position = new Vector2f(0, 0);
            sfmlSprite.Rotation = 90;
            var scaleX = (float)sfmlWindowWidth / screen.Width;
            var scaleY = (float)sfmlWindowHeight / screen.Height;
            sfmlSprite.Scale = new Vector2f(scaleY, -scaleX);

            sfmlStates = new RenderStates(BlendMode.None);

            menu = new MenuRenderer(resource.Wad, screen);
            threeD = new ThreeDRenderer(resource, screen, 7);
            statusBar = new StatusBarRenderer(resource.Wad, screen);
            intermission = new IntermissionRenderer(resource.Wad, screen);
            openingSequence = new OpeningSequenceRenderer(resource.Wad, screen, this);
            autoMap = new AutoMapRenderer(resource.Wad, screen);
            finale = new FinaleRenderer(resource, screen);

            pause = Patch.FromWad("M_PAUSE", resource.Wad);

            var scale = screen.Width / 320;
            wipeBandWidth = 2 * scale;
            wipeBandCount = screen.Width / wipeBandWidth + 1;
            wipeHeight = screen.Height / scale;
            wipeBuffer = new byte[screen.Data.Length];

            displayMessage = true;

            gammaCorrectionLevel = 0;
            palette.ResetColors(gammaCorrectionParameters[gammaCorrectionLevel]);
        }

        private void RenderApplication(DoomApplication app)
        {
            if (app.State == ApplicationState.Opening)
            {
                openingSequence.Render(app.Opening);
            }
            else if (app.State == ApplicationState.Game)
            {
                RenderGame(app.Game);
            }

            if (!app.Menu.Active)
            {
                if (app.State == ApplicationState.Game &&
                    app.Game.State == GameState.Level &&
                    app.Game.Paused)
                {
                    var scale = screen.Width / 320;
                    screen.DrawPatch(
                        pause,
                        (screen.Width - scale * pause.Width) / 2,
                        4 * scale,
                        scale);
                }
            }
        }

        private void RenderMenu(DoomApplication app)
        {
            if (app.Menu.Active)
            {
                menu.Render(app.Menu);
            }
        }

        private void RenderGame(DoomGame game)
        {
            if (game.State == GameState.Level)
            {
                var player = game.World.Options.Players[game.Options.ConsolePlayer];
                if (game.World.AutoMap.Visible)
                {
                    autoMap.Render(player);
                    statusBar.Render(player);
                }
                else
                {
                    threeD.Render(player);
                    if (threeD.WindowSize < 8)
                    {
                        statusBar.Render(player);
                    }
                }

                if (displayMessage || ReferenceEquals(player.Message, (string)DoomInfo.Strings.MSGOFF))
                {
                    if (player.MessageTime > 0)
                    {
                        var scale = screen.Width / 320;
                        screen.DrawText(player.Message, 0, 7 * scale, scale);
                    }
                }
            }
            else if (game.State == GameState.Intermission)
            {
                intermission.Render(game.Intermission);
            }
            else if (game.State == GameState.Finale)
            {
                finale.Render(game.Finale);
            }
        }

        public void Render(DoomApplication app)
        {
            RenderApplication(app);
            RenderMenu(app);

            var colors = palette[0];
            if (app.State == ApplicationState.Game &&
                app.Game.State == GameState.Level)
            {
                colors = palette[GetPaletteNumber(app.Game.World.ConsolePlayer)];
            }

            Display(colors);
        }

        public void RenderWipe(DoomApplication app, Wipe wipe)
        {
            RenderApplication(app);

            var scale = screen.Width / 320;
            for (var i = 0; i < wipeBandCount - 1; i++)
            {
                var x1 = wipeBandWidth * i;
                var x2 = x1 + wipeBandWidth;
                var y1 = Math.Max(scale * wipe.Y[i], 0);
                var y2 = Math.Max(scale * wipe.Y[i + 1], 0);
                var dy = (float)(y2 - y1) / wipeBandWidth;
                for (var x = x1; x < x2; x++)
                {
                    var y = (int)MathF.Round(y1 + dy * ((x - x1) / 2 * 2));
                    var copyLength = screen.Height - y;
                    if (copyLength > 0)
                    {
                        var srcPos = screen.Height * x;
                        var dstPos = screen.Height * x + y;
                        Array.Copy(wipeBuffer, srcPos, screen.Data, dstPos, copyLength);
                    }
                }
            }

            RenderMenu(app);

            Display(palette[0]);
        }

        public void InitializeWipe()
        {
            Array.Copy(screen.Data, wipeBuffer, screen.Data.Length);
        }

        private void Display(uint[] colors)
        {
            var screenData = screen.Data;
            var p = MemoryMarshal.Cast<byte, uint>(sfmlTextureData);
            for (var i = 0; i < p.Length; i++)
            {
                p[i] = colors[screenData[i]];
            }
            sfmlTexture.Update(sfmlTextureData, (uint)screen.Height, (uint)screen.Width, 0, 0);
            sfmlWindow.Draw(sfmlSprite, sfmlStates);
            sfmlWindow.Display();
        }

        private static int GetPaletteNumber(Player plyr)
        {
            var cnt = plyr.DamageCount;

            if (plyr.Powers[(int)PowerType.Strength] != 0)
            {
                // Slowly fade the berzerk out.
                var bzc = 12 - (plyr.Powers[(int)PowerType.Strength] >> 6);

                if (bzc > cnt)
                {
                    cnt = bzc;
                }
            }

            int palette;

            if (cnt != 0)
            {
                palette = (cnt + 7) >> 3;

                if (palette >= Palette.NUMREDPALS)
                {
                    palette = Palette.NUMREDPALS - 1;
                }

                palette += Palette.STARTREDPALS;
            }
            else if (plyr.BonusCount != 0)
            {
                palette = (plyr.BonusCount + 7) >> 3;

                if (palette >= Palette.NUMBONUSPALS)
                {
                    palette = Palette.NUMBONUSPALS - 1;
                }

                palette += Palette.STARTBONUSPALS;
            }
            else if (plyr.Powers[(int)PowerType.IronFeet] > 4 * 32 ||
                (plyr.Powers[(int)PowerType.IronFeet] & 8) != 0)
            {
                palette = Palette.RADIATIONPAL;
            }
            else
            {
                palette = 0;
            }

            return palette;
        }

        public void Dispose()
        {
            if (sfmlSprite != null)
            {
                sfmlSprite.Dispose();
                sfmlSprite = null;
            }

            if (sfmlTexture != null)
            {
                sfmlTexture.Dispose();
                sfmlTexture = null;
            }
        }



        public int WipeBandCount => wipeBandCount;
        public int WipeHeight => wipeHeight;

        public int MaxWindowSize
        {
            get
            {
                return ThreeDRenderer.MaxScreenSize;
            }
        }

        public int WindowSize
        {
            get
            {
                return threeD.WindowSize;
            }

            set
            {
                threeD.WindowSize = value;
            }
        }

        public bool DisplayMessage
        {
            get
            {
                return displayMessage;
            }

            set
            {
                displayMessage = value;
            }
        }

        public int MaxGammaCorrectionLevel
        {
            get
            {
                return gammaCorrectionParameters.Length;
            }
        }

        public int GammaCorrectionLevel
        {
            get
            {
                return gammaCorrectionLevel;
            }

            set
            {
                gammaCorrectionLevel = value;
                palette.ResetColors(gammaCorrectionParameters[gammaCorrectionLevel]);
            }
        }
    }
}
