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
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            sfmlSprite.Position = new Vector2f(0, 0);
            sfmlSprite.Rotation = 90;
            var scaleX = (float)sfmlWindowWidth / screen.Width;
            var scaleY = (float)sfmlWindowHeight / screen.Height;
            sfmlSprite.Scale = new Vector2f(scaleY, -scaleX);

            sfmlStates = new RenderStates(BlendMode.None);

            menu = new MenuRenderer(resource.Wad, screen);
            threeD = new ThreeDRenderer(resource, screen);
            statusBar = new StatusBarRenderer(resource.Wad, screen);
            intermission = new IntermissionRenderer(resource.Wad, screen);
            openingSequence = new OpeningSequenceRenderer(resource.Wad, screen, this);
            autoMap = new AutoMapRenderer(resource.Wad, screen);
            finale = new FinaleRenderer(resource, screen);
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

        /*
        public void BindWorld(World world)
        {
            this.world = world;

            threeD.BindWorld(world);
        }

        public void UnbindWorld()
        {
            threeD.UnbindWorld();
        }
        */

        public void RenderGame(DoomGame game)
        {
            if (game.gameState == GameState.Level)
            {
                var player = game.World.Players[game.Options.ConsolePlayer];
                if (game.World.AutoMap.Visible)
                {
                    autoMap.Render(player);
                    statusBar.Render(game.World.StatusBar, player);
                }
                else
                {
                    threeD.Render(player);
                    statusBar.Render(game.World.StatusBar, player);
                }
                if (player.MessageTime > 0)
                {
                    int scale = screen.Width / 320;
                    screen.DrawText(player.Message, 0, 7 * scale, scale);
                }
            }
            else if (game.gameState == GameState.Intermission)
            {
                intermission.Render(game.Intermission);
            }
            else if (game.gameState == GameState.Finale)
            {
                finale.Render(game.Finale);
            }
        }

        public void Render(DoomApplication app)
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

        public void IntermissionRenderTest(Intermission im)
        {
            intermission.Render(im);

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

            Console.WriteLine("SFML resources are disposed.");
        }
    }
}
