using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class SfmlRenderer : IDisposable
    {
        private RenderWindow sfmlWindow;
        private Palette palette;

        private uint[] colors;

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

        public SfmlRenderer(RenderWindow window, CommonResource resource, bool highResolution)
        {
            sfmlWindow = window;
            palette = resource.Palette;

            colors = InitColors(palette);

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
        }

        private static uint[] InitColors(Palette palette)
        {
            var colors = new uint[256];
            for (var i = 0; i < 256; i++)
            {
                var offset = 3 * i;
                var r = palette.Data[offset + 0];
                var g = palette.Data[offset + 1];
                var b = palette.Data[offset + 2];
                var a = 255;
                var color = new SFML.Graphics.Color(r, g, b);
                colors[i] = (uint)((r << 0) | (g << 8) | (b << 16) | (a << 24));
            }
            return colors;
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

            if (app.Menu.Active)
            {
                menu.Render(app.Menu);
            }
            else
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
                    statusBar.Render(player);
                }

                var scale = screen.Width / 320;
                if (player.MessageTime > 0)
                {
                    screen.DrawText(player.Message, 0, 7 * scale, scale);
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
            Display();
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

            Display();
        }

        public void InitializeWipe()
        {
            Array.Copy(screen.Data, wipeBuffer, screen.Data.Length);
        }

        private void Display()
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
    }
}
